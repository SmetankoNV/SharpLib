using System;

namespace SharpLib.Source.Extensions
{
    public static class ExtensionVersion
    {
        /// <summary>
        /// Преобразование версии в формату микроконтроллеров a.b.c
        /// a - 0-й байт
        /// b - 1-й байт
        /// c - 2,3-й байт в big-endian
        /// </summary>
        public static byte[] ToBufferMcu(this Version self)
        {
            var result = new byte[4];

            result[0] = (byte)self.Major;
            result[1] = (byte)self.Minor;
            result[2] = (byte)(self.Build >> 8);
            result[3] = (byte)(self.Build & 0xFF);

            return result;
        }

        /// <summary>
        /// Преобразование версии в формату микроконтроллеров a.b.c
        /// a - 0-й байт
        /// b - 1-й байт
        /// c - 2,3-й байт в big-endian
        /// </summary>
        public static uint ToIntMcu(this Version self)
        {
            uint result = (uint)(
                ((self.Major & 0xFF) << 24) | 
                ((self.Minor & 0xFF) << 16) | 
                (self.Build & 0xFF00) | 
                (self.Build & 0x00FF)
                );

            return result;
        }
    }
}