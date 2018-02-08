using System;
using SharpLib.Source.Enums;

namespace SharpLib.Source.Helpers.Images
{
    /// <summary>
    /// BMP изображение (https://en.wikipedia.org/wiki/BMP_file_format)
    /// </summary>
    public class ImageBmp : ImageBase
    {
        #region Константы

        /// <summary>
        /// Сигнатура файла (042 0x4D = 'B''M' = Windows 3.1x, 95, NT)
        /// </summary>
        private const int MAGIC_CODE_BM = 0x424D;

        /// <summary>
        /// Значение Plains в заголовке BMP. Всегда 1
        /// </summary>
        private const int PLANES_DEFAUTL = 1;

        /// <summary>
        /// Размер BITMAPINFOHEADER Windows формата
        /// </summary>
        private const int BITMAPINFOHEADER_SIZE_WINDOWS = 40;

        /// <summary>
        /// Размер заголовка BITMAP_FILE_HEADER (байты)
        /// </summary>
        private const int BITMAP_FILE_HEADER_SIZE = 14;

        /// <summary>
        /// Коэффициент пересчета дюймов в метры
        /// </summary>
        private const double FACTOR_INCH_METERS = 39.3701;

        #endregion

        #region Конструктор

        public ImageBmp(string filename) : base(ImageTyp.Bmp)
        {
            var content = Files.Files.ReadBuffer(filename);
            LoadFromBuffer(content);
        }

        public ImageBmp(byte[] content) : base(ImageTyp.Bmp)
        {
            LoadFromBuffer(content);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Загрузка BMP из файла
        /// </summary>
        private void LoadFromBuffer(byte[] content)
        {
            var list = new ByteList(content, Endianess.Little);

            // =========================================
            // BITMAPFILEHEADER (14 bytes)
            // =========================================
            // Чтение сигнатуры файла
            var magicNum = list.GetByte16(Endianess.Big);
            if (magicNum != MAGIC_CODE_BM)
            {
                throw new Exception("Invalid BMP signature");
            }

            // Чтение размера файла
            var fileSize = list.GetByte32();
            // Пропуск зарезервированных поле
            list.GetByte32();
            // Чтение начала данных (смещение)
            var dataOffset = list.GetByte32();

            // =========================================
            // BITMAPINFOHEADER
            // =========================================
            var headerSize = list.GetByte32();
            if (headerSize != BITMAPINFOHEADER_SIZE_WINDOWS)
            {
                throw new Exception($"Unsupported BMP header format: {headerSize} bytes");
            }
            var imageWidth = System.Math.Abs((int)list.GetByte32());
            var imageHeight = (int)list.GetByte32();
            // Если height отрицательное, то отсчет идет Top->Bottom
            var topToBottom = imageHeight < 0;
            imageHeight = System.Math.Abs(imageHeight);
            var planes = list.GetByte16();
            var bitsPerPixel = list.GetByte16();
            var compression = list.GetByte32();
            var imageSize = list.GetByte32();
            var horizontalResolution = (int)list.GetByte32();
            var verticalResolution = (int)list.GetByte32();
            var colorsUsed = (int)list.GetByte32();
            var colorsImportant = (int)list.GetByte32();

            // Проверки параметров заголовка
            if (imageWidth <= 0)
            {
                throw new Exception($"Invalid width: {imageWidth}");
            }
            if (imageHeight == 0)
            {
                throw new Exception($"Invalid height: {imageHeight}");
            }
            if (planes != PLANES_DEFAUTL)
            {
                throw new Exception($"Unsupported planes: {planes}");
            }

            if (horizontalResolution != verticalResolution)
            {
                throw new Exception($"Wrong resolution: {horizontalResolution} != {verticalResolution}");
            }

            if (imageSize > fileSize)
            {
                throw new Exception($"Wrong raw size: {imageSize}");
            }

            if (bitsPerPixel == 1 || bitsPerPixel == 4 || bitsPerPixel == 8)
            {
                if (colorsUsed == 0)
                {
                    colorsUsed = 1 << bitsPerPixel;
                }
                else if (colorsUsed > 1 << bitsPerPixel)
                {
                    throw new Exception($"Invalid colors used: {colorsUsed}");
                }
            }
            else if (bitsPerPixel == 24 || bitsPerPixel == 32)
            {
                if (colorsUsed != 0)
                {
                    throw new Exception($"Invalid colors used: {colorsUsed}");
                }
            }
            else
            {
                throw new Exception($"Unsupported bits per pixel: {bitsPerPixel}");
            }

            if (compression == 0)
            {
            }
            else if (bitsPerPixel == 8 && compression == 1 || bitsPerPixel == 4 && compression == 2)
            {
                if (topToBottom)
                {
                    throw new Exception("Top-to-bottom order not supported for compression = 1 or 2");
                }
            }
            else
            {
                throw new Exception($"Unsupported compression: {compression}");
            }

            if (colorsImportant < 0 || colorsImportant > colorsUsed)
            {
                throw new Exception($"Invalid important colors: {colorsImportant}");
            }

            // Дополнительные проверки
            if (14 + headerSize + 4 * colorsUsed > dataOffset)
            {
                throw new Exception($"Invalid image data offset: {dataOffset}");
            }
            if (dataOffset > fileSize)
            {
                throw new Exception($"Invalid file size: {fileSize}");
            }

            // =============================
            // Чтение данных 
            // =============================

            ImagePixels pixels;

            // Пропуск опциональных байт 
            var passCount = dataOffset - (BITMAP_FILE_HEADER_SIZE + headerSize + 4 * colorsUsed);
            if (passCount != 0)
            {
                list.GetBuffer((int)passCount);
            }

            if (bitsPerPixel == 24 || bitsPerPixel == 32)
            {
                pixels = readRgb24Or32Image(list, imageWidth, imageHeight, topToBottom, bitsPerPixel);
            }

            else
            {
                var palette = new int[colorsUsed];
                for (var i = 0; i < colorsUsed; i++)
                {
                    // На каждую палитру по 4 байта (
                    var entry = list.GetBuffer(4);
                    palette[i] = (entry[2] & 0xFF) << 16 | (entry[1] & 0xFF) << 8 | (entry[0] & 0xFF);
                }

                pixels = compression == 0
                    ? ReadPalettedImage(list, imageWidth, imageHeight, topToBottom, bitsPerPixel, palette)
                    : ReadRleImage(list, imageWidth, imageHeight, bitsPerPixel, palette);
            }

            // ===================================
            // Заполнение полей
            // ===================================
            Width = imageWidth;
            Height = imageHeight;
            Depth = bitsPerPixel;
            Pixels = pixels;
            Dpi = (int)(horizontalResolution / FACTOR_INCH_METERS);
        }

        /// <summary>
        /// Формирование таблицы пикселей для несжатого изображения, использующего индексированные цвета
        /// </summary>
        private ImagePixels ReadPalettedImage(ByteList list, int width, int height, bool topToBottom, int bitsPerPixel, int[] palette)
        {
            var rowSize = (width * bitsPerPixel + 31) / 32 * 4;
            var pixelsPerByte = 8 / bitsPerPixel;
            var mask = (1 << bitsPerPixel) - 1;

            var pixels = new ImagePixels(width, height);

            int y;
            int end;
            int inc;
            if (topToBottom)
            {
                y = 0;
                end = height;
                inc = 1;
            }
            else
            {
                y = height - 1;
                end = -1;
                inc = -1;
            }

            for (; y != end; y += inc)
            {
                var row = list.GetBuffer(rowSize);

                for (var x = 0; x < width; x++)
                {
                    var index = x / pixelsPerByte;
                    var shift = (pixelsPerByte - 1 - x % pixelsPerByte) * bitsPerPixel;
                    var b = row[index];
                    var colorIndex = (byte)(b >> shift & mask);
                    var color = palette[colorIndex];
                    pixels[x, y] = color;
                }
            }

            return pixels;
        }

        /// <summary>
        /// Формирование таблицы пикселей для несжатого изображения, использующего полные цвета
        /// </summary>
        private ImagePixels readRgb24Or32Image(ByteList list, int width, int height, bool topToBottom, int bitsPerPixel)
        {
            var pixels = new ImagePixels(width, height);
            var bytesPerPixel = bitsPerPixel / 8;
            var rowSize = (width * bytesPerPixel + 3) / 4 * 4;

            int y;
            int end;
            int inc;
            if (topToBottom)
            {
                y = 0;
                end = height;
                inc = 1;
            }
            else
            {
                y = height - 1;
                end = -1;
                inc = -1;
            }

            for (; y != end; y += inc)
            {
                var row = list.GetBuffer(rowSize);

                for (var x = 0; x < width; x++)
                {
                    var offset = x * bytesPerPixel;
                    var color =
                        row[offset + 2] << 16 |
                        row[offset + 1] << 8 |
                        row[offset + 0] << 0;

                    pixels[x, y] = color;
                }
            }

            return pixels;
        }

        /// <summary>
        /// Формирование таблицы пикселей для сжатого изображения, использующего полные цвета
        /// </summary>
        private ImagePixels ReadRleImage(ByteList list, int width, int height, int bitsPerPixel, int[] palette)
        {
            var pixels = new ImagePixels(width, height);
            var x = 0;
            var y = height - 1;
            while (true)
            {
                var b = list.GetBuffer(2);
                if (b[0] == 0)
                {
                    // Special
                    if (b[1] == 0)
                    {
                        // End of scanline
                        x = 0;
                        y--;
                    }
                    else if (b[1] == 1)
                    {
                        // End of RLE data
                        break;
                    }
                    else if (b[1] == 2)
                    {
                        // Delta code

                        b = list.GetBuffer(2);
                        x += b[0] & 0xFF;
                        y -= b[1] & 0xFF;
                        if (x >= width)
                        {
                            throw new ArgumentOutOfRangeException($"{nameof(x)} coordinate out of bounds");
                        }
                    }
                    else
                    {
                        // Literal run
                        var n = b[1] & 0xFF;
                        // Round up to multiple of 2 bytes
                        var tempSize = (n * bitsPerPixel + 15) / 16 * 2;
                        b = list.GetBuffer(tempSize);
                        for (var i = 0; i < n; i++, x++)
                        {
                            if (x == width) // Ignore image data past end of line
                            {
                                break;
                            }

                            int colorIndex;
                            switch (bitsPerPixel)
                            {
                                case 8:
                                    colorIndex = b[i];
                                    break;
                                case 4:
                                    colorIndex = (byte)(b[i / 2] >> ((1 - i % 2) * 4) & 0xF);
                                    break;
                                default:
                                    throw new ArgumentException();
                            }

                            var color = palette[colorIndex];
                            pixels[x, y] = color;
                        }
                    }
                }
                else
                {
                    // Run
                    var n = b[0] & 0xFF;
                    for (var i = 0; i < n; i++, x++)
                    {
                        if (x == width) // Ignore image data past end of line
                        {
                            break;
                        }

                        int colorIndex;

                        switch (bitsPerPixel)
                        {
                            case 8:
                                colorIndex = b[1];
                                break;
                            case 4:
                                colorIndex = (byte)(b[1] >> ((1 - i % 2) * 4) & 0xF);
                                break;
                            default:
                                throw new ArgumentException();
                        }

                        var color = palette[colorIndex];
                        pixels[x, y] = color;
                    }
                }
            }

            return pixels;
        }

        /// <summary>
        /// Сохранение изображения в файл
        /// </summary>
        /// <remarks>
        /// Реализован только 24-ный формат
        /// </remarks>
        public void Save(string filename)
        {
            var list = new ByteList(Endianess.Little);

            var rowSize = (Width * 3 + 3) / 4 * 4; // 3 bytes per pixel in RGB888, round up to multiple of 4
            var imageSize = rowSize * Height;
            var resolution = (int)(Dpi * FACTOR_INCH_METERS);

            // BITMAPFILEHEADER
            list.AddByte16(MAGIC_CODE_BM, Endianess.Big);
            list.AddByte32(BITMAP_FILE_HEADER_SIZE + BITMAPINFOHEADER_SIZE_WINDOWS + imageSize);
            list.AddByte16(0);
            list.AddByte16(0);
            list.AddByte32(BITMAP_FILE_HEADER_SIZE + BITMAPINFOHEADER_SIZE_WINDOWS);

            // BITMAPINFOHEADER
            list.AddByte32(40);
            list.AddByte32(Width);
            list.AddByte32(Height);
            list.AddByte16(1);
            list.AddByte16(24);
            list.AddByte32(0);
            list.AddByte32(imageSize);
            list.AddByte32(resolution);
            list.AddByte32(resolution);
            list.AddByte32(0);
            list.AddByte32(0);

            // Image data
            var row = new byte[rowSize];
            var width = Width;
            for (var y = Height - 1; y >= 0; y--)
            {
                for (var x = 0; x < width; x++)
                {
                    var color = Pixels[x, y];
                    row[x * 3 + 0] = (byte)(color >> 0); // Blue
                    row[x * 3 + 1] = (byte)(color >> 8); // Green
                    row[x * 3 + 2] = (byte)(color >> 16); // Red
                }

                list.AddBuffer(row);
            }

            var bytes = list.ToBuffer();
            Files.Files.WriteBuffer(filename, bytes);
        }

        #endregion
    }
}