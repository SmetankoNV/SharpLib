using System;
using System.Text;

namespace SharpLib.Source.Extensions.Time
{
    /// <summary>
    /// Методы расширения для класса 'TimeSpan'
    /// </summary>
    public static class ExtensionTimeSpan
    {
        #region Методы

        /// <summary>
        /// Интервал в виде строки минуты:секунды (12:23)
        /// </summary>
        public static string ToStringMinEx(this TimeSpan self, bool showHours = false)
        {
            return string.Format("{0:00}:{1:00}", self.Minutes, self.Seconds);
        }

        /// <summary>
        /// Интервал в виде строки "4d 23m 12s"
        /// </summary>
        public static string ToStringPrettyEx(this TimeSpan self)
        {
            if (self == TimeSpan.Zero)
            {
                return "0";
            }

            var sb = new StringBuilder();
            if (self.Days > 0)
            {
                sb.AppendFormat("{0}d ", self.Days);
            }
            if (self.Hours > 0)
            {
                sb.AppendFormat("{0:D2}h ", self.Hours);
            }
            if (self.Minutes > 0)
            {
                sb.AppendFormat("{0:D2}m ", self.Minutes);
            }
            if (self.Seconds > 0)
            {
                sb.AppendFormat("{0:D2}s", self.Seconds);
            }

            var result = sb.ToString().Trim();

            return result;
        }

        #endregion
    }
}