using System.Security.Cryptography;
using SharpLib.Source.Extensions;
using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Crypto
{
    /// <summary>
    /// Хэширование MD5
    /// </summary>
    public class Md5
    {
        #region Методы

        public static string Hash(byte[] buffer)
        {
            if (buffer == null)
            {
                return string.Empty;
            }
            byte[] bufOut;
            using (var md5Hasher = MD5.Create())
            {
                bufOut = md5Hasher.ComputeHash(buffer);
            }

            var hash = bufOut.ToAsciiEx(string.Empty).ToLower();

            return hash;
        }

        public static string Hash(string text)
        {
            return Hash(text.ToBytesEx());
        }

        #endregion
    }
}