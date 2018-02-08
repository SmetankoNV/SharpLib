using System.IO;

namespace SharpLib.Source.Helpers
{
    /// <summary>
    /// Класс расширяющий возможности стандартного Path
    /// </summary>
    public static class PathEx
    {
        #region Методы

        /// <summary>
        /// Объединение путей
        /// </summary>
        public static string Combine(string path1, string path2)
        {
            if (path1 == null || path2 == null)
            {
                return path1;
            }

            path1 = path1.TrimEnd('/','\\');
            path2 = path2.TrimStart('/', '\\');

            var result = path1 + "/" + path2;
            result = Path.GetFullPath(result).TrimEnd('/', '\\');

            return result;
        }

        /// <summary>
        /// Объединение путей
        /// </summary>
        public static string Combine(string path1, string path2, string path3)
        {
            if (path1 == null || path2 == null || path3 == null)
            {
                return path1;
            }

            var result = Combine(path1, path2);
            result = Combine(result, path3);

            return result;
        }

        /// <summary>
        /// Переход на директорию выше
        /// </summary>
        public static string SetUp(string path)
        {
            return Path.Combine(path, "..");
        }

        #endregion
    }
}