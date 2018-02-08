using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Crypto
{
    public class Aes
    {
        #region Константы

        private const string INIT_VECTOR = @"SharpLib";

        #endregion

        #region Методы

        /// <summary>
        /// Кодирование данных
        /// </summary>
        /// <remarks>
        /// Для преобразования ключи и вектора инициализации в массив используется MD5 (32 байта для ключа и первые 16 байт для вектора)
        /// Если вектор инициализации не указан, то используется строка "SharpLib"
        /// </remarks>
        public static string Encrypt(string key, string text, string init = null)
        {
            var keyBytes = Md5.Hash(key).ToBytesEx();
            var initBytes = init.IsValid() ? Md5.Hash(key).ToBytesEx() : Md5.Hash(INIT_VECTOR).ToBytesEx();
            var dataBytes = Encoding.UTF8.GetBytes(text);

            var result = Encrypt(keyBytes, Mem.Clone(initBytes, 0, 16), dataBytes);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Кодирование данных AES-256
        /// </summary>
        /// <param name="key">Ключ шифрования (256 бит = 32 байта)</param>
        /// <param name="initVector">Начальный вектор (128 бит = 16 байт)</param>
        /// <param name="data">Блок данных (должен быть кратен 16 байтам)</param>
        /// <returns>Зашифрованный блок данных</returns>
        public static byte[] Encrypt(byte[] key, byte[] initVector, byte[] data)
        {
            var cipher = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                BlockSize = 128,
                KeySize = 256,
                Key = key,
                IV = initVector,
                Padding = PaddingMode.None
            };

            if (data.Length % 16 != 0)
            {
                cipher.Padding = PaddingMode.Zeros;
            }

            var encryptor = cipher.CreateEncryptor();

            using (var memStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    var result = memStream.ToArray();
                    memStream.Close();
                    cryptoStream.Close();

                    return result;
                }
            }
        }

        /// <summary>
        /// Расшифровка блока данных
        /// </summary>
        /// <param name="key">Ключ шифрования (256 бит = 32 байта)</param>
        /// <param name="initVector">Начальный вектор (128 бит = 16 байт)</param>
        /// <param name="data">Блок данных (должен быть кратен 16 байтам)</param>
        /// <returns>Расшифрованный блок данных</returns>
        public static byte[] Decrypt(byte[] key, byte[] initVector, byte[] data)
        {
            var symmetricKey = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                BlockSize = 128,
                KeySize = 256,
                Key = key,
                IV = initVector,
                Padding = PaddingMode.Zeros
            };

            var decryptor = symmetricKey.CreateDecryptor();
            using (var memStream = new MemoryStream(data))
            {
                using (var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                {
                    var result = new byte[data.Length];
                    var count = cryptoStream.Read(result, 0, result.Length);
                    memStream.Close();
                    cryptoStream.Close();

                    return Mem.Clone(result, 0, count);
                }
            }
        }

        /// <summary>
        /// Декодирование данных
        /// </summary>
        public static string Decrypt(string key, string text, string init = null)
        {
            var keyBytes = Md5.Hash(key).ToBytesEx();
            var initBytes = init.IsValid() ? Md5.Hash(key).ToBytesEx() : Md5.Hash(INIT_VECTOR).ToBytesEx();
            var dataBytes = Convert.FromBase64String(text);

            var resultBytes = Decrypt(keyBytes, Mem.Clone(initBytes, 0, 16), dataBytes);

            var result = Encoding.UTF8.GetString(resultBytes);
            result = result.TrimEnd('\0');

            return result;
        }

        #endregion
    }
}