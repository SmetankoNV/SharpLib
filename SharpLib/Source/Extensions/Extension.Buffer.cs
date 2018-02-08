using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SharpLib.Source.Enums;
using SharpLib.Source.Helpers;

namespace SharpLib.Source.Extensions
{
    /// <summary>
    /// Метод расширения класса byte[]
    /// </summary>
    public static class ExtensionBuffer
    {
        #region Методы

        /// <summary>
        /// Преобразование буфера с строку (кодировка UTF-8 без BOM)
        /// </summary>
        public static string ToStringEx(this byte[] buffer)
        {
            var text = Encoding.UTF8.GetString(buffer);
            text = text.TrimEnd('\0');

            return text;
        }

        /// <summary>
        /// Преобразование байтового массива 0x31, 0x32, 0x33 в кодировке Win1251 в строку "123"
        /// </summary>
        public static string ToString1251Ex(this byte[] self)
        {
            var res = Encoding.GetEncoding(1251).GetString(self);

            return res;
        }

        /// <summary>
        /// Преобразование строки вида "11-22-33-AF" в байтовый массив 0x11, 0x22, 0x33, 0xAF
        /// </summary>
        public static IntPtr ToIntPtrEx(this byte[] self)
        {
            if (self == null)
            {
                return IntPtr.Zero;
            }
            var ptr = Marshal.AllocHGlobal(self.Length);
            for (var i = 0; i < self.Length; i++)
            {
                Marshal.WriteByte(ptr, i, self[i]);
            }

            return ptr;
        }

        public static MemoryStream ToMemoryStreamEx(this byte[] buffer)
        {
            var stream = new MemoryStream(buffer.Length);

            stream.Write(buffer, 0, buffer.Length);

            return stream;
        }

        public static string ToAsciiEx(this byte[] buffer, string delimeter = " ")
        {
            var value = BitConverter.ToString(buffer);
            value = value.Replace("-", delimeter);

            return value;
        }

        /// <summary>
        /// Преобразование буфера в Guid 
        /// </summary>
        public static Guid ToGuidEx(this byte[] self)
        {
            if (self.Length != 16)
            {
                throw new ArgumentException();
            }

            var bytes = new byte[16];

            bytes[0] = self[3];
            bytes[1] = self[2];
            bytes[2] = self[1];
            bytes[3] = self[0];
            bytes[4] = self[5];
            bytes[5] = self[4];
            bytes[6] = self[7];
            bytes[7] = self[6];
            bytes[8] = self[8];
            bytes[9] = self[9];
            bytes[10] = self[10];
            bytes[11] = self[11];
            bytes[12] = self[12];
            bytes[13] = self[13];
            bytes[14] = self[14];
            bytes[15] = self[15];

            var result = new Guid(bytes);

            return result;
        }


        public static List<byte[]> SplitByEx(this byte[] value, int chunkSize)
        {
            var list = new List<byte[]>();

            if (value != null && chunkSize > 0)
            {
                var valueLength = value.Length;

                for (var i = 0; i < valueLength; i += chunkSize)
                {
                    if (i + chunkSize > valueLength)
                    {
                        chunkSize = valueLength - i;
                    }

                    var temp = Mem.Clone(value, i, chunkSize);

                    list.Add(temp);
                }
            }

            return list;
        }

        public static byte GetByte8Ex(this byte[] buf, int offset)
        {
            var value = buf[offset];

            return value;
        }

        public static ushort GetByte16Ex(this byte[] buf, int offset, Endianess endian)
        {
            ushort value;

            if (endian == Endianess.Little)
            {
                value = (ushort)
                    (
                        (buf[offset + 0] << 0) +
                        (buf[offset + 1] << 8)
                        );
            }
            else
            {
                value = (ushort)
                    (
                        (buf[offset + 0] << 8) +
                        (buf[offset + 1] << 0)
                        );
            }

            return value;
        }

        public static uint GetByte24Ex(this byte[] buf, int offset, Endianess endian)
        {
            uint value;

            if (endian == Endianess.Little)
            {
                value = ((uint)buf[offset + 0] << 0) +
                        ((uint)buf[offset + 1] << 8) +
                        ((uint)buf[offset + 2] << 16);
            }
            else
            {
                value = ((uint)buf[offset + 1] << 16) +
                        ((uint)buf[offset + 2] << 8) +
                        ((uint)buf[offset + 3] << 0);
            }

            return value;
        }

        public static uint GetByte32Ex(this byte[] buf, int offset, Endianess endian)
        {
            uint value;

            if (endian == Endianess.Little)
            {
                value = ((uint)buf[offset + 0] << 0) +
                        ((uint)buf[offset + 1] << 8) +
                        ((uint)buf[offset + 2] << 16) +
                        ((uint)buf[offset + 3] << 24);
            }
            else
            {
                value = ((uint)buf[offset + 0] << 24) +
                        ((uint)buf[offset + 1] << 16) +
                        ((uint)buf[offset + 2] << 8) +
                        ((uint)buf[offset + 3] << 0);
            }
            return value;
        }

        public static ulong GetByte64Ex(this byte[] buf, int offset, Endianess endian)
        {
            ulong value;

            if (endian == Endianess.Little)
            {
                value = ((ulong)buf[offset + 0] << 0) +
                        ((ulong)buf[offset + 1] << 8) +
                        ((ulong)buf[offset + 2] << 16) +
                        ((ulong)buf[offset + 3] << 24) +
                        ((ulong)buf[offset + 4] << 32) +
                        ((ulong)buf[offset + 5] << 40) +
                        ((ulong)buf[offset + 6] << 48) +
                        ((ulong)buf[offset + 7] << 56);
            }
            else
            {
                value = ((ulong)buf[offset + 0] << 56) +
                        ((ulong)buf[offset + 1] << 48) +
                        ((ulong)buf[offset + 2] << 40) +
                        ((ulong)buf[offset + 3] << 32) +
                        ((ulong)buf[offset + 4] << 24) +
                        ((ulong)buf[offset + 5] << 16) +
                        ((ulong)buf[offset + 6] << 8) +
                        ((ulong)buf[offset + 7] << 0);
            }
            return value;
        }

        public static void SetByte8Ex(this byte[] buf, int offset, byte value)
        {
            buf[offset] = value;
        }

        public static void SetByte16Ex(this byte[] buf, int offset, ushort value, Endianess endian)
        {
            var src = new byte[2];

            if (endian == Endianess.Little)
            {
                src[0] = (byte)(value >> 0);
                src[1] = (byte)(value >> 8);
            }
            else
            {
                src[0] = (byte)(value >> 8);
                src[1] = (byte)(value >> 0);
            }

            Mem.Copy(buf, offset, src, 0, 2);
        }

        public static void SetByte32Ex(this byte[] buf, int offset, uint value, Endianess endian)
        {
            var src = new byte[4];

            if (endian == Endianess.Little)
            {
                src[0] = (byte)(value >> 0);
                src[1] = (byte)(value >> 8);
                src[2] = (byte)(value >> 16);
                src[3] = (byte)(value >> 24);
            }
            else
            {
                src[0] = (byte)(value >> 24);
                src[1] = (byte)(value >> 16);
                src[2] = (byte)(value >> 8);
                src[3] = (byte)(value >> 0);
            }

            Mem.Copy(buf, offset, src, 0, 4);
        }

        public static void SetByte64Ex(this byte[] buf, int offset, ulong value, Endianess endian)
        {
            var src = new byte[8];

            if (endian == Endianess.Little)
            {
                src[0] = (byte)(value >> 0);
                src[1] = (byte)(value >> 8);
                src[2] = (byte)(value >> 16);
                src[3] = (byte)(value >> 24);
                src[4] = (byte)(value >> 32);
                src[5] = (byte)(value >> 40);
                src[6] = (byte)(value >> 48);
                src[7] = (byte)(value >> 56);
            }
            else
            {
                src[0] = (byte)(value >> 56);
                src[1] = (byte)(value >> 48);
                src[2] = (byte)(value >> 40);
                src[3] = (byte)(value >> 32);
                src[4] = (byte)(value >> 24);
                src[5] = (byte)(value >> 16);
                src[6] = (byte)(value >> 8);
                src[7] = (byte)(value >> 0);
            }
            Mem.Copy(buf, offset, src, 0, 8);
        }

        public static void SetFloatEx(this byte[] buf, int offset, float value)
        {
            var temp = BitConverter.GetBytes(value);

            Mem.Copy(buf, offset, temp, 0, 4);
        }

        public static void SetDoubleEx(this byte[] buf, int offset, double value)
        {
            var temp = BitConverter.GetBytes(value);

            Mem.Copy(buf, offset, temp, 0, 8);
        }

        public static byte[] ResizeEx(this byte[] buffer, int addSize)
        {
            var plus = addSize > 0;
            var size = Math.Abs(addSize);
            var result = new byte[buffer.Length + size];

            Mem.Copy(result, plus ? size : 0, buffer);

            return result;
        }

        public static float GetFloatEx(this byte[] buf, int offset)
        {
            var value = BitConverter.ToSingle(buf, offset);

            return value;
        }

        public static double GetDoubleEx(this byte[] buf, int offset)
        {
            var value = BitConverter.ToDouble(buf, offset);

            return value;
        }

        public static byte[] AddEx(this byte[] buf, byte[] addBuf)
        {
            return Mem.Concat(buf, addBuf);
        }

        public static byte[] CloneEx(this byte[] buf, int offset, int size)
        {
            return Mem.Clone(buf, offset, size);
        }

        public static int SearchEx(this byte[] buf, byte[] value)
        {
            if (value == null || buf.Length == 0 || value.Length == 0)
            {
                return -1;
            }

            var count = 0;
            var sizeBuf = buf.Length;
            var sizeValue = value.Length;
            var indexBuf = 0;
            var indexValue = 0;

            for (; sizeBuf > 0; --sizeBuf)
            {
                var b1 = buf[indexBuf++];
                var b2 = value[indexValue];

                if (b1 == b2)
                {
                    if (++indexValue == sizeValue)
                    {
                        return indexBuf;
                    }
                    count++;
                }
                else
                {
                    if (count > 0)
                    {
                        indexValue -= count;
                        count = 0;
                    }
                }
            }

            return -1;
        }

        public static bool IsValid(this byte[] value)
        {
            return value != null && value.Length > 0;
        }

        #endregion
    }
}