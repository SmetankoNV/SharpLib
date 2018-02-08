using System;

namespace SharpLib.Source.Helpers.Crypto
{
    /// <summary>
    /// Кодирование Base64
    /// </summary>
    public class Base64
    {
        #region Методы

        /// <summary>
        /// Кодирование
        /// </summary>
        public static string Encrypt(byte[] value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var res = Convert.ToBase64String(value);

            return res;
        }

        /// <summary>
        /// Декодирование
        /// </summary>
        public static byte[] Decrypt(string value)
        {
            var res = Convert.FromBase64String(value);

            return res;
        }

        #endregion
    }
}