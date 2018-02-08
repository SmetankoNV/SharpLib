using System.Drawing;

namespace SharpLib.Win.Source.Extensions
{
    /// <summary>
    /// Класс расширения для "Color"
    /// </summary>
    public static class ExtensionColor
    {
        #region Constants

        private static readonly int[] _colorTable5 =
        {
            0, 8, 16, 25, 33, 41, 49, 58, 66, 74, 82, 90, 99, 107, 115, 123, 132,
            140, 148, 156, 165, 173, 181, 189, 197, 206, 214, 222, 230, 239, 247, 255
        };

        private static readonly int[] _colorTable6 =
        {
            0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 45, 49, 53, 57, 61, 65, 69,
            73, 77, 81, 85, 89, 93, 97, 101, 105, 109, 113, 117, 121, 125, 130, 134, 138,
            142, 146, 150, 154, 158, 162, 166, 170, 174, 178, 182, 186, 190, 194, 198,
            202, 206, 210, 215, 219, 223, 227, 231, 235, 239, 243, 247, 251, 255
        };

        #endregion

        #region Members

        /// <summary>
        /// Преобразование RGB565->RGB888
        /// </summary>
        public static Color ToRgb888(ushort value)
        {
            var r5 = (value >> 11) & 0x1F;
            var g6 = (value >> 5) & 0x3F;
            var b5 = value & 0x1F;

            var r8 = _colorTable5[r5];
            var g8 = _colorTable6[g6];
            var b8 = _colorTable5[b5];

            return Color.FromArgb(r8, g8, b8);
        }

        /// <summary>
        /// Преобразование RGB888->RGB565
        /// </summary>
        public static ushort ToRgb565(Color self)
        {
            var r565 = (self.R << 13) | ((self.G & 0x3F) << 5) | (self.B & 0x1F);

            return (ushort) r565;
        }

        #endregion
    }
}