using System;

namespace SharpLib.Source.Helpers
{
    public class Date
    {
        private static readonly DateTime _unixEpoch;

        static Date()
        {
            _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Преобразование "Unix (миллисекунды) -> DateTime"
        /// </summary>
        public static DateTime UnixTimeToDateTime(long millis)
        {
            return _unixEpoch.AddMilliseconds(millis);
        }

        /// <summary>
        /// Преобразование "DateTime-> Unix (миллисекунды)"
        /// </summary>
        public static long DateTimeToUnixTime(DateTime value)
        {
            return (long)(value - _unixEpoch).TotalMilliseconds;
        }
    }
}