using System;

namespace SharpLib.Source.Extensions
{
    /// <summary>
    /// Метод расширения класса Guid
    /// </summary>
    public static class ExtensionGuid
    {
        /// <summary>
        /// Преобразование Guid в буфер в формате "как вижу, так байты и идут"
        /// </summary>
        /// <remarks>
        /// Общий формат Guid        : G1G2G3G4-G5G6-G7G8-G9G10G11G12G13G14G15G16
        /// В байтовом представлении : G4 G3 G2 G1 G6 G5 G9 G10 G11 G12 G13 G14 G15 G16
        /// Например: 22345200-ABE8-4F60-90C8-0D43C5F6C0F6
        ///           00 52 34 22 E8 AB 60 4F 90 C8 0D 43 C5 F6 C0 F6
        /// 
        /// Такой формат не удобен для работы с микроконтроллерами, поэтому ToBufferEx использует формат
        /// 22345200-ABE8-4F60-90C8-0D43C5F6C0F6
        /// 22 34 52 00 AB E8 4F 60 90 C8 0D 43 C5 F6 C0 F6
        /// </remarks>
        public static byte[] ToBufferEx(this Guid self)
        {
            var bytes = new byte[16];
            var lebytes = self.ToByteArray();
            
            bytes[0] = lebytes[3];
            bytes[1] = lebytes[2];
            bytes[2] = lebytes[1];
            bytes[3] = lebytes[0];
            bytes[4] = lebytes[5];
            bytes[5] = lebytes[4];
            bytes[6] = lebytes[7];
            bytes[7] = lebytes[6];
            bytes[8] = lebytes[8];
            bytes[9] = lebytes[9];
            bytes[10] = lebytes[10];
            bytes[11] = lebytes[11];
            bytes[12] = lebytes[12];
            bytes[13] = lebytes[13];
            bytes[14] = lebytes[14];
            bytes[15] = lebytes[15];

            return bytes;
        }
    }
}