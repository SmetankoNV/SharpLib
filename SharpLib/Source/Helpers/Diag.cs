using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SharpLib.Source.Helpers
{
    /// <summary>
    /// Вспомогательный класс для диагностики/отладки
    /// </summary>
    public static class Diag
    {
        /// <summary>
        /// Вывод в отладку с параметрами
        /// </summary>
        [SuppressMessage("ReSharper", "InvocationIsSkipped")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public static void WriteLine(string format, params object[] args)
        {
            format = string.Format(format, args);
            format = $"[{DateTime.Now:hh:mm:ss.fff}] {format}";

            Debug.WriteLine(format);
        }
    }
}