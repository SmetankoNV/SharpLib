using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using SharpLib.Source.Enums;
using SharpLib.Source.Helpers;

namespace SharpLib.Source.Extensions.String
{
    /// <summary>
    /// Класс расширения String
    /// </summary>
    public static class ExtensionString
    {
        #region Методы

        public static string RegexEx(this string value, string pattern)
        {
            // Пример   : '1234 (567) 890'
            // Pattern  : '4 ({0}) 8'
            // Результат: 567

            var patternConst = "{0}";

            int index = pattern.SearchEx(patternConst);
            if (index == -1)
            {
                return string.Empty;
            }

            var textLeft = pattern.Substring(0, index - patternConst.Length);
            var textRight = pattern.Substring(index);

            if (textLeft.IsNotValid() && textRight.IsNotValid())
            {
                return string.Empty;
            }

            var indexLeft = -1;
            var indexRight = -1;

            if (textLeft.IsNotValid())
            {
                indexLeft = 0;
            }
            else if (textRight.IsNotValid())
            {
                indexRight = value.Length;
            }

            if (indexLeft == -1)
            {
                indexLeft = value.SearchEx(textLeft);
            }
            if (indexRight == -1)
            {
                indexRight = value.SearchEx(textRight, indexLeft == -1 ? 0 : indexLeft);
            }

            if (indexLeft == -1 || indexRight == -1)
            {
                return string.Empty;
            }
            if (indexLeft >= indexRight)
            {
                return string.Empty;
            }

            var result = value.Substring(indexLeft, indexRight - indexLeft - textRight.Length);

            return result;
        }

        public static string SubstringEx(this string value, int startIndex, int endIndex)
        {
            if (endIndex > startIndex)
            {
                var length = endIndex - startIndex;
                var result = value.Substring(startIndex, length);

                return result;
            }

            return string.Empty;
        }

        public static string RemoveEx(this string value, int startIndex, int endIndex)
        {
            var length = endIndex - startIndex;
            var result = value.Remove(startIndex, length);

            return result;
        }

        public static int SearchEx(this string value, string substring, int offset)
        {
            var result = -1;
            var index = value.IndexOf(substring, offset, StringComparison.Ordinal);

            if (index >= 0)
            {
                result = index + substring.Length;
            }

            return result;
        }

        public static int SearchEx(this string value, string substring)
        {
            return SearchEx(value, substring, 0);
        }

        public static string ReplaceEx(this string text, string value, int startIndex, int endIndex)
        {
            if (value.IsNotValid())
            {
                return text;
            }
            if (startIndex >= endIndex)
            {
                return text;
            }
            if (startIndex < 0 || endIndex >= text.Length)
            {
                return text;
            }

            var textBefore = text.Substring(0, startIndex);
            var textAfter = text.Substring(endIndex, text.Length - endIndex);

            var result = textBefore + value + textAfter;

            return result;
        }

        public static int GetIntEx(this string value, int offset)
        {
            var result = 0;

            for (var i = offset; i < value.Length; i++)
            {
                var ch = value[i];

                if (ch.IsDigit() == false)
                {
                    break;
                }

                result *= 10;
                result += ch.ToDigitEx();
            }

            return result;
        }

        public static int ToIntEx(this string value)
        {
            int result;

            int.TryParse(value, out result);

            return result;
        }

        public static int ToIntFromHexEx(this string value)
        {
            int result;

            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                result = int.Parse(value, NumberStyles.HexNumber);
            }
            else
            {
                result = ToIntEx(value);
            }

            return result;
        }

        public static byte ToByteEx(this string value)
        {
            return (byte)value.ToUInt32Ex();
        }

        public static ushort ToUInt16Ex(this string value)
        {
            return (ushort)value.ToUInt32Ex();
        }

        public static uint ToUInt32Ex(this string value)
        {
            if (value == null)
            {
                return 0;
            }
            if (value == "")
            {
                return 0;
            }
            uint result = 0;
            try
            {
                result = Convert.ToUInt32(value);
            }
            catch
            {
            }
            return result;
        }

        /// <summary>
        /// Преобразование к Float
        /// </summary>
        public static float ToFloatEx(this string value)
        {
            if (value == "")
            {
                return 0;
            }

            value = value.Replace(',', '.');

            return Convert.ToSingle(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Преобразование к Float (без генерации исключения)
        /// </summary>
        public static bool TryToFloatEx(this string value, out float result)
        {
            try
            {
                result = value.ToFloatEx();
                return true;
            }
            catch (Exception)
            {
                result = 0;
                return false;
            }
        }

        public static double ToDoubleEx(this string value)
        {
            if (value == "")
            {
                return 0;
            }

            value = value.Replace(',', '.');

            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        public static char ToCharEx(this string value)
        {
            if (value.IsValid())
            {
                return value[0];
            }

            return '\0';
        }

        public static IPAddress ToIpEx(this string value)
        {
            value = value.ToLower();

            if (value == "localhost")
            {
                value = "127.0.0.1";
            }

            return IPAddress.Parse(value);
        }

        /// <summary>
        /// Преобразование строки вида "123" в байтовый массив в кодировке UTF-8
        /// </summary>
        public static byte[] ToBytesEx(this string text)
        {
            var buffer = Encodings.Utf8.GetBytes(text);

            return buffer;
        }

        /// <summary>
        /// Преобразование строки в кодировке UTF-8 в байтовый массив
        /// </summary>
        public static byte[] ToBytesEx(this string value, int offset, int size)
        {
            if (offset > 0 && offset + size < value.Length)
            {
                value = value.Substring(offset, size);

                return ToBytesEx(value);
            }

            return null;
        }

        /// <summary>
        /// Преобразование строки вида "123" в байтовый массив 0x31, 0x32, 0x33 в кодировке Win1251
        /// </summary>
        public static byte[] ToBytes1251Ex(this string self)
        {
            var res = Encoding.GetEncoding(1251).GetBytes(self);

            return res;
        }

        public static string CleanAscii(this string text)
        {
            var newCh = '.';
            var res = "";

            foreach (var t in text)
            {
                var ch = t;
                int code = ch;

                if ((code < 0x20) || ((code > 0x7E) && (code < 0xC0)))
                {
                    ch = newCh;
                }

                res = res + ch;
            }

            return res;
        }

        public static string[] SplitEx(this string value, string delimeter)
        {
            var result = value.Split(new[] { delimeter }, StringSplitOptions.RemoveEmptyEntries);

            return result;
        }

        /// <summary>
        /// Разделение строки на блоки фиксированного размера
        /// </summary>
        public static IEnumerable<string> SplitByEx(this string self, int chunkSize)
        {
            var res = Enumerable.Range(0, self.Length / chunkSize)
                .Select(i => self.Substring(i * chunkSize, chunkSize));

            return res;
        }

        public static bool IsValid(this string value)
        {
            var result = string.IsNullOrEmpty(value) == false;

            return result;
        }

        public static bool IsNotValid(this string value)
        {
            return value.IsValid() == false;
        }

        public static string Remove(this string text, string subString)
        {
            text = text.Replace(subString, "");

            return text;
        }

        public static string TrimEx(this string text, string subString)
        {
            text = text.TrimStartEx(subString);
            text = text.TrimEndEx(subString);

            return text;
        }

        public static string TrimStartEx(this string text, string subString)
        {
            if (text.StartsWith(subString))
            {
                text = text.Remove(0, subString.Length);
            }

            return text;
        }

        public static string TrimEndEx(this string text, string subString)
        {
            if (subString.IsNotValid())
            {
                return text;
            }

            if (text.EndsWith(subString))
            {
                text = text.Remove(text.Length - subString.Length);
            }

            return text;
        }

        public static byte GetByte8(this string text, int offset)
        {
            if (offset < text.Length)
            {
                byte[] buffer = text.ToBytesEx(offset, 1);

                if (buffer != null)
                {
                    return buffer[0];
                }
            }

            return 0x00;
        }

        public static ushort GetByte16(this string text, int offset, Endianess endian)
        {
            if (offset < text.Length)
            {
                byte[] buffer = text.ToBytesEx(offset, 2);

                if (buffer != null)
                {
                    var result = buffer.GetByte16Ex(offset, endian);

                    return result;
                }
            }

            return 0x0000;
        }

        public static uint GetByte32(this string text, int offset, Endianess endian)
        {
            if (offset < text.Length)
            {
                byte[] buffer = text.ToBytesEx(offset, 4);

                if (buffer != null)
                {
                    var result = buffer.GetByte32Ex(offset, endian);

                    return result;
                }
            }

            return 0x00000000;
        }

        public static ulong GetByte64(this string text, int offset, Endianess endian)
        {
            if (offset < text.Length)
            {
                byte[] buffer = text.ToBytesEx(offset, 8);

                if (buffer != null)
                {
                    var result = buffer.GetByte64Ex(offset, endian);

                    return result;
                }
            }

            return 0x0000000000000000;
        }

        public static Stream ToStreamEx(this string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(text);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        public static DateTime ToDateTimeEx(this string text, string pattern)
        {
            var result = DateTime.ParseExact(text, pattern, CultureInfo.InvariantCulture);

            return result;
        }

        public static byte ToAsciiByteEx(this string text)
        {
            byte result = 0x00;

            if (text != null && text.Length > 1)
            {
                var chA = text[0];
                var chB = text[1];

                var a = chA.ToAsciiByteEx();
                var b = chB.ToAsciiByteEx();

                if (char.IsDigit(chA))
                {
                    a &= 0x0F;
                }
                else if (char.IsUpper(chA))
                {
                    a -= 0x37;
                }
                else
                {
                    a -= 0x57;
                }

                if (char.IsDigit(chB))
                {
                    b &= 0x0F;
                }
                else if (char.IsUpper(chB))
                {
                    b -= 0x37;
                }
                else
                {
                    b -= 0x57;
                }

                result = (byte)((a << 4) | b);
            }

            return result;
        }

        public static byte[] ToAsciiBufferEx(this string text)
        {
            if (text != null && text.Length > 1)
            {
                var countIn = text.Length / 2;
                var result = new byte[countIn];

                var offset = 0;
                while (offset < countIn)
                {
                    var temp = text.Substring(offset * 2, 2);
                    var b = ToAsciiByteEx(temp);

                    result[offset] = b;

                    offset++;
                }

                return result;
            }

            return null;
        }

        public static byte[] ToAsciiBufferEx(this string text, string delimeter)
        {
            if (text.IsNotValid())
            {
                return null;
            }

            if (delimeter.IsValid())
            {
                text = text.Replace(delimeter, string.Empty);
            }

            return ToAsciiBufferEx(text);
        }

        public static string TabsToSpacesEx(this string value, int numSpaces)
        {
            var spaces = new string(' ', numSpaces);
            var result = value.Replace("\t", spaces);

            return result;
        }

        public static string ExpandRightEx(this string value, int width, string ch = " ")
        {
            var c = ch[0];
            var result = value.PadRight(width, c);

            return result;
        }

        public static bool ContainsEx(this string source, string text, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            var index = source.IndexOf(text, comp);

            return index >= 0;
        }

        /// <summary>
        /// Проверка вхождения по маске
        /// </summary>
        /// <param name="source">Строка (пример 1.jpg)</param>
        /// <param name="mask">Маска (пример *.jpg)</param>
        /// <returns>true - строка содержит маску</returns>
        public static bool ContainsByMaskEx(this string source, string mask)
        {
            var pattern = "^" + Regex.Escape(mask)
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".");

            var result = Regex.IsMatch(source, pattern);

            return result;
        }

        public static bool EqualsOrdinalEx(this string value1, string value2)
        {
            return value1.Equals(value2, StringComparison.Ordinal);
        }

        public static bool EqualsIgnoreCaseEx(this string value1, string value2)
        {
            return value1.Equals(value2, StringComparison.OrdinalIgnoreCase);
        }

        public static string ToLinuxPathEx(this string self)
        {
            return self.Replace('\\', '/');
        }

        public static T ParseEnumEx<T>(this string self) where T : struct
        {
            T result;

            Enum.TryParse(self, true, out result);

            return result;
        }

        public static int CompareToEx(this string self, string value, StringComparison comparsionType = StringComparison.Ordinal)
        {
            return string.Compare(self, value, comparsionType);
        }

        /// <summary>
        /// Преобразование строки в unmanaged-буфер 
        /// </summary>
        /// <remarks>
        /// Функция использует Marshal.AllocHGlobal, поэтому нужно не забыть выполнить Marshal.FreeHGlobal
        /// </remarks>
        public static IntPtr ToIntPtrEx(this string self)
        {
            if (self == null)
            {
                return IntPtr.Zero;
            }

            var ptr = Marshal.AllocHGlobal(self.Length + 1);
            var bytes = Encoding.GetEncoding(1251).GetBytes(self);
            for (var i = 0; i < bytes.Length; i++)
            {
                Marshal.WriteByte(ptr, i, bytes[i]);
            }
            Marshal.WriteByte(ptr, self.Length, 0x00);

            return ptr;
        }

        #endregion
    }
}