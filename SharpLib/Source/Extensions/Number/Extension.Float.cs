using System;
using System.Globalization;

namespace SharpLib.Source.Extensions.Number
{
    /// <summary>
    /// Класс расширения "Float"
    /// </summary>
    public static class ExtensionFloat
    {
        #region Константы

        public const float EPSILON = 0.0001f;

        #endregion

        #region Методы

        /// <summary>
        /// Сравнение чисел с указанной точностью
        /// </summary>
        public static bool EqualEx(this float value1, float value2, float epsilon = EPSILON)
        {
            return Math.Abs(value1 - value2) <= epsilon;
        }

        /// <summary>
        /// Сравнение числа с 0-м с указанной точностью
        /// </summary>
        public static bool EqualZeroEx(this float value1)
        {
            return value1.EqualEx(0);
        }

        /// <summary>
        /// Сравнение чисел с указанной точностью
        /// </summary>
        public static string ToStringEx(this float value, int precision = 2)
        {
            string result;

            switch (precision)
            {
                case 1:
                    result = value.ToString("F1");
                    break;
                case 2:
                    result = value.ToString("F2");
                    break;
                case 3:
                    result = value.ToString("F3");
                    break;
                case 4:
                    result = value.ToString("F4");
                    break;
                default:
                    result = value.ToString(CultureInfo.InvariantCulture);
                    break;
            }

            return result.Replace(',','.');
        }

        /// <summary>
        /// Упаковка Float в 4-х байтный буфер (LE)
        /// </summary>
        public static byte[] ToBufferEx(this float value)
        {
            byte[] temp = BitConverter.GetBytes(value);

            return temp;
        }


        #endregion
    }
}