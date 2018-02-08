using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpLib.Source.Enums;
using SharpLib.Source.Extensions;
using SharpLib.Source.Extensions.Number;
using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Files.Hex
{
    public class IntelHex
    {
        #region Константы

        private const int SAVE_HEX_WIDTH = 16;

        #endregion

        #region Свойства

        public List<IntelHexRegion> Regions { get; set; }

        #endregion

        #region Конструктор

        public IntelHex()
        {
            Regions = new List<IntelHexRegion>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Расчет адреса, выровненного по границе блока
        /// </summary>
        /// <param name="addr">Невыровненный адрес</param>
        /// <param name="blockSize">Размер блока</param>
        /// <returns>Выровенный адрес</returns>
        private uint GetAlignAddr(uint addr, int blockSize)
        {
            // Пример:
            // Невыровненный адрес: 5
            // Размер блока       : 4
            // Выровненный адрес  : 4

            var alignAddr = (uint)(addr / blockSize);

            alignAddr = (uint)(alignAddr * blockSize);

            return alignAddr;
        }

        public void LoadFile(string filename)
        {
            Regions = new List<IntelHexRegion>();

            // Проверка существования файла
            if (Files.IsFile(filename) == false)
            {
                throw new Exception($"Файл {filename} не существует");
            }

            // Чтение файла
            var text = Files.ReadText(filename);
            if (text == null)
            {
                throw new Exception($"Ошибка чтения файла {filename}");
            }

            LoadText(text);
        }

        /// <summary>
        /// Загрузка HEX-текста для разбора
        /// </summary>
        public void LoadText(string text)
        {
            // Разделение текста на записи (разделитель #13#10)
            var lines = text.SplitEx(Environment.NewLine);

            if (lines.Length == 0)
            {
                throw new Exception("Файл не IntelHex формата");
            }

            var records = new List<IntelHexRecord>();

            // Проверка контрольных суммы записей
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                var record = new IntelHexRecord(line);

                if (record.IsValid == false)
                {
                    throw new Exception($"Неверный формат. Строка {i + 1}");
                }

                records.Add(record);
            }

            // Добавление записей в регионы (с автоматическим созданием регионов)
            uint baseAddr = 0;

            foreach (var record in records)
            {
                if (record.IsData)
                {
                    // Запись: данные
                    WriteBuffer(baseAddr + record.Addr, record.Data);
                }
                else if (record.IsSegment)
                {
                    // Запись: сегмент -> установка нового базового адреса
                    baseAddr = record.AddrSegment;
                }
            }

            // Проверка
            if (Regions.Count == 0)
            {
                throw new Exception("Не найдено ни одного региона");
            }

            // Сортировка регионов по возрастанию адреса
            Regions.Sort();
        }

        public void SaveFile(string filename, int width = SAVE_HEX_WIDTH)
        {
            if (Regions.Count == 0)
            {
                throw new Exception("Нет регионов с данными");
            }

            var result = new StringBuilder();

            // Формирование строк для каждого региона
            foreach (var region in Regions)
            {
                var temp = region.ToText(width);

                result.Append(temp);
            }

            // Добавление записи "Конец файла"
            var record = IntelHexRecord.EndRecord();
            result.Append(record.Text + Environment.NewLine);

            // Сохранение результата в файл
            if (Files.WriteText(filename, result.ToString()) == false)
            {
                throw new Exception($"Ошибка записи в файл: {filename}");
            }
        }

        public bool SaveBin(string filename, uint addr, int size, byte fill)
        {
            var data = ReadBufferFill(addr, size, fill);

            Files.WriteBuffer(filename, data);

            return true;
        }

        /// <summary>
        /// Создание региона для определенного адреса и добавление его в список
        /// </summary>
        private IntelHexRegion CreateRegion(uint addr)
        {
            var baseAddr = GetAlignAddr(addr, IntelHexRegion.REGION_SIZE);

            var region = new IntelHexRegion(baseAddr);

            Regions.Add(region);
            Regions.Sort();

            return region;
        }

        /// <summary>
        /// Удаление региона
        /// </summary>
        /// <param name="region"></param>
        private void RemoveRegion(IntelHexRegion region)
        {
            Regions.Remove(region);
            Regions.Sort();
        }

        /// <summary>
        /// Поиск региона, содержащего адрес
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private IntelHexRegion SearchRegion(uint addr)
        {
            return Regions.FirstOrDefault(region => region.IsValidAddr(addr));
        }

        /// <summary>
        /// Удаление данных
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        public void RemoveBuffer(uint addr, int size)
        {
            while (size > 0)
            {
                // Поиск расположения данных в 64к регионах
                var region = SearchRegion(addr);
                int removeSize;

                // Регион не существует для текущего адреса
                if (region != null)
                {
                    // Удаление данных из региона
                    removeSize = region.Remove(addr, size);

                    if (removeSize == 0)
                    {
                        // Данных нет для удаления: Переход к следующему региону
                        removeSize = IntelHexRegion.REGION_SIZE;
                    }
                    else
                    {
                        // Данные удалены из региона: Возможно регион тоже нужно удалить
                        // т.к. он может уже не содержать данных
                        if (region.IsEmpty)
                        {
                            RemoveRegion(region);
                        }
                    }
                }
                else
                {
                    // Регион не существует: Переход к следующему адресу через регион
                    removeSize = IntelHexRegion.REGION_SIZE;
                }

                size -= removeSize;
                addr += (uint)removeSize;
            } // end while (поиск адресов)
        }

        /// <summary>
        /// Запись данных по указанному адресу
        /// </summary>
        public void WriteBuffer(uint addr, byte[] data)
        {
            // Поиск расположения данных в 64к регионах
            var region = SearchRegion(addr) ?? CreateRegion(addr);

            // Регион не существует для текущего адреса
            var totalSize = data.Length;
            var writeSize = region.Write(addr, data);

            if (writeSize != 0 && writeSize < totalSize)
            {
                // Буфер записан не весь: Часть переноситься на другой регион
                data = Mem.Clone(data, writeSize, totalSize - writeSize);
                addr += (uint)writeSize;

                // Запись следующего региона
                WriteBuffer(addr, data);
            }
        }

        /// <summary>
        /// Чтение данных по указанному адресу
        /// </summary>
        public byte[] ReadBuffer(uint addr, int size)
        {
            // Поиск расположения данных в 64к регионах
            var region = SearchRegion(addr);

            // Регион не существует для текущего адреса
            if (region == null)
            {
                throw new Exception(IntelHexRegion.ErrorAddrText(addr));
            }

            var data = region.Read(addr, size);

            // Адрес не существует
            if (data == null)
            {
                return null;
            }

            var readSize = data.Length;

            if (readSize != 0 && readSize < size)
            {
                // Буфер прочитан не весь: Часть переноситься на другой регион
                addr += (uint)readSize;
                var remain = ReadBuffer(addr, size - readSize);
                if (remain == null)
                {
                    return null;
                }

                data = Mem.Concat(data, remain);
            }

            return data;
        }

        /// <summary>
        /// Чтение данных по указанному адресу с заполнением недостающих данных
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBufferFill(uint addr, int size, byte fill)
        {
            byte[] data = null;

            while (size > 0)
            {
                // Поиск расположения данных в 64к регионах
                var region = SearchRegion(addr);
                byte[] block;
                int readSize;

                // Регион не существует для текущего адреса
                if (region != null)
                {
                    // Чтение данных
                    block = region.ReadFill(addr, size, fill);
                    readSize = block.Length;
                }
                else
                {
                    readSize = size > IntelHexRegion.REGION_SIZE ? IntelHexRegion.REGION_SIZE : size;
                    block = Mem.Set(readSize, fill);
                }

                // Добавление данных к существующему буферу
                data = Mem.Concat(data, block);

                addr += (uint)readSize;
                size -= readSize;
            } // end while (перебор всего размера)

            return data;
        }

        /// <summary>
        /// Чтение байта по указанному адресу в HEX-формате
        /// </summary>
        public byte ReadByte8(uint addr)
        {
            var buffer = ReadBuffer(addr, 1);

            if (buffer == null)
            {
                throw new Exception($"Адрес 0x{addr:X8} не существует");
            }

            return buffer[0];
        }

        /// <summary>
        /// Чтение 4-х байтного значения по указанному адресу
        /// </summary>
        public ushort ReadByte16(uint addr, Endianess endian)
        {
            var buffer = ReadBuffer(addr, 2);

            if (buffer == null)
            {
                throw new Exception($"Адрес 0x{addr:X8} не существует");
            }

            var value = buffer.GetByte16Ex(0, endian);

            return value;
        }

        /// <summary>
        /// Чтение 4-х байтного значения по указанному адресу
        /// </summary>
        public uint ReadByte32(uint addr, Endianess endian)
        {
            var buffer = ReadBuffer(addr, 4);

            if (buffer == null)
            {
                throw new Exception($"Адрес 0x{addr:X8} не существует");
            }

            var value = buffer.GetByte32Ex(0, endian);

            return value;
        }

        /// <summary>
        /// Запись байта по указанному адресу 
        /// </summary>
        public void WriteByte8(uint addr, byte value)
        {
            var buffer = value.ToBufferEx();

            WriteBuffer(addr, buffer);
        }

        /// <summary>
        /// Запись 2-х байтового значения по указанному адресу 
        /// </summary>
        public void WriteByte16(uint addr, ushort value, Endianess endian)
        {
            var buffer = value.ToBufferEx(endian);

            WriteBuffer(addr, buffer);
        }

        /// <summary>
        /// Запись 2-х байтового значения по указанному адресу 
        /// </summary>
        public void WriteByte32(uint addr, uint value, Endianess endian)
        {
            var buffer = value.ToBufferEx(endian);

            WriteBuffer(addr, buffer);
        }

        #endregion

        /// <summary>
        /// Загрузка файла
        /// </summary>
        public static IntelHex LoadFromFile(string location)
        {
            var hex = new IntelHex();
            hex.LoadFile(location);

            return hex;
        }
    }
}