using System.Globalization;
using SharpLib.Source.Enums;
using SharpLib.Source.Helpers;

namespace SharpLib.Source.Extensions.Number
{
    public static class ExtensionInteger
    {
        #region Методы

        public static string ToStringEx(this int value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            return $"{value:X8}";
        }

        public static string ToStringEx(this byte value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
            return $"{value:X2}";
        }

        public static string ToStringEx(this ushort value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
            return $"{value:X4}";
        }

        public static string ToStringEx(this uint value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }
            return $"{value:X8}";
        }

        public static string ToStringEx(this ulong value, int radix = 10)
        {
            if (radix == 10)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            return $"{value:X16}";
        }

        public static byte[] ToBufferEx(this byte value)
        {
            byte[] buffer = { value };

            return buffer;
        }

        public static byte[] ToBufferEx(this ushort value, Endianess endian)
        {
            var buffer = new byte[2];

            Mem.PutByte16(buffer, 0, value, endian);

            return buffer;
        }

        public static byte[] ToBufferEx(this uint value, Endianess endian)
        {
            var buffer = new byte[4];

            Mem.PutByte32(buffer, 0, value, endian);

            return buffer;
        }

        public static byte[] ToBufferEx(this ulong value, Endianess endian)
        {
            var buffer = new byte[8];

            Mem.PutByte64(buffer, 0, value, endian);

            return buffer;
        }

        public static byte[] ToBufferEx(this int value, Endianess endian)
        {
            if (value < 0)
            {
                return new byte[0];
            }
            if (value < 0xFF + 1)
            {
                return ((byte)value).ToBufferEx();
            }
            if (value < 0xFFFF + 1)
            {
                return ((ushort)value).ToBufferEx(endian);
            }

            return ((uint)value).ToBufferEx(endian);
        }

        public static ushort SwitchOrderEx(this ushort value)
        {
            var result = (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);

            return result;
        }

        public static uint SwitchOrderEx(this uint value)
        {
            var result =
                (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;

            return result;
        }

        public static ulong SwitchOrderEx(this ulong value)
        {
            var result =
                (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;

            return result;
        }

        /// <summary>
        /// Определение позиции установленного бита (маска -> позиция)
        /// </summary>
        /// <example>
        /// 0x0010 = 0000 0000 1000 0000 -> 7-я позиция
        /// </example>
        public static int GetBitPosEx(this uint value)
        {
            var mask = value;
            int pos = 0;

            while (mask != 0)
            {
                if ((mask & 1) == 1)
                {
                    return pos;
                }

                pos++;
                mask >>= 1;
            }

            return 0;
        }

        /// <summary>
        /// Проверка, установлен ли бит
        /// </summary>
        public static bool IsBitEx(this int self, int bitIndex)
        {
            return (self & (1 << bitIndex)) != 0;
        }

        /// <summary>
        /// Установка бита в нужное состояние
        /// </summary>
        public static int SetBitEx(this int self, int bitIndex, bool state)
        {
            if (state)
            {
                self = self + (1 << bitIndex);
            }
            else
            {
                self = self & ~(1 << bitIndex);
            }

            return self;
        }

        /// <summary>
        /// Преобразование значение в файловый формат (1565485 = 1.5MB)
        /// </summary>
        public static string ToFileSizeEx(this int self, int divider = 1000)
        {
            return ((long)self).ToFileSizeEx(divider);
        }

        #endregion
    }
}