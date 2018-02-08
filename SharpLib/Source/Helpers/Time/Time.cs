using System;

namespace SharpLib.Source.Helpers.Time
{
    /// <summary>
    /// Прослойка для предоставления доступа к времени
    /// Класс введен для
    ///  + подмены провайдера времени в Unit-тестировании
    ///  + расширения функциональности класса DateTime
    /// </summary>
    public static class Time
    {
        #region Поля

        /// <summary>
        /// Пройвадер системного времени
        /// </summary>
        private static readonly ITimeProvider _real;

        /// <summary>
        /// Вспомогательная переменная для расчетов с Unix-датами
        /// </summary>
        private static readonly DateTime _unixEpoch;

        /// <summary>
        /// Текущий провайдер 
        /// </summary>
        private static ITimeProvider _current;

        #endregion

        #region Свойства

        /// <summary>
        /// Текущий провайдер
        /// </summary>
        public static ITimeProvider Provider
        {
            get { return _current; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _current = value;
            }
        }

        /// <summary>
        /// Текущее время (локальное)
        /// </summary>
        public static DateTime Now
        {
            get { return _current.Now; }
        }

        /// <summary>
        /// Текущее время (UTC)
        /// </summary>
        public static DateTime UtcNow
        {
            get { return _current.UtcNow; }
        }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        static Time()
        {
            _real = new TimeProviderReal();
            _current = _real;
            _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        #endregion

        #region Методы

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

        /// <summary>
        /// Сброс провайдера в значение по умолчанию
        /// </summary>
        public static void SetDefaultProvider()
        {
            _current = _real;
        }

        #endregion
    }
}