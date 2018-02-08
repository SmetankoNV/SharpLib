using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpLib.Source.Extensions
{
    /// <summary>
    /// Класс расширения для IntPtr
    /// </summary>
    public static class ExtensionsIntPtr
    {
        /// <summary>
        /// Преобразование строки в unmanaged-буфер 
        /// </summary>
        /// <remarks>
        /// Функция использует Marshal.AllocHGlobal, поэтому нужно не забыть выполнить Marshal.FreeHGlobal
        /// </remarks>
        public static string ToStringEx(this IntPtr self)
        {
            if (self == IntPtr.Zero)
            {
                return null;
            }

            var offset = 0;
            var bytes = new byte[1024];

            for (;;)
            {
                var b = Marshal.ReadByte(self, offset);

                if (b != 0)
                {
                    bytes[offset++] = b;
                }
                else
                {
                    break;
                }
            }

            var res = Encoding.GetEncoding(1251).GetString(bytes).TrimEnd('\0');

            return res;
        }

        /// <summary>
        /// Преобразование unmanaged-блока в буфер
        /// </summary>
        public static byte[] ToBytesEx(this IntPtr self, int size)
        {
            var res = new byte[size];

            for (var i = 0; i < size; i++)
            {
                res[i] = Marshal.ReadByte(self, i);
            }

            return res;
        }
    }
}