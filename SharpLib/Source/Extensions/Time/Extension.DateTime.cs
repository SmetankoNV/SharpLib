using System;

namespace SharpLib.Source.Extensions.Time
{
    /// <summary>
    /// Методы расширения для класса 'DateTime'
    /// </summary>
    public static class ExtensionDateTime
    {
        #region Методы

        /// <summary>
        /// Количество миллисекунд содержащихся в штампе времени
        /// </summary>
        public static long GetMillisecondsEx(this DateTime self)
        {
            var result = self.Ticks / TimeSpan.TicksPerMillisecond;

            return result;
        }

        /// <summary>
        /// Форматирование времени в формат "2016-10-01 12:23:12.123"
        /// </summary>
        public static string ToStringDateEx(this DateTime self)
        {
            return $"{self:yyyy-MM-dd HH:mm:ss}";
        }

        #endregion
    }
}