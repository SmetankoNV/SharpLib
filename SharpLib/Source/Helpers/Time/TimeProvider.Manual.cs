using System;

namespace SharpLib.Source.Helpers.Time
{
    /// <summary>
    /// Провайдер времени с ручной установкой дат
    /// </summary>
    public class TimeProviderManual : ITimeProvider
    {
        #region Поля

        /// <summary>
        /// Смещение относительно реального времени (тики процессора)
        /// </summary>
        private long _offset;

        #endregion

        #region Свойства

        /// <summary>
        /// Текущее время (локальное)
        /// </summary>
        public DateTime Now
        {
            get { return DateTime.Now.AddTicks(_offset); }
            set { _offset = (value - DateTime.Now).Ticks; }
        }

        /// <summary>
        /// Текущее время (UTC)
        /// </summary>
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow.AddTicks(_offset); }
            set { _offset = (value - DateTime.UtcNow).Ticks; }
        }

        #endregion

        #region Конструктор

        public TimeProviderManual()
        {
            _offset = 0;
        }

        #endregion

        #region Методы

        public void AddDays(double value)
        {
            Now = Now.AddDays(value);
        }

        public void AddHours(double value)
        {
            Now = Now.AddHours(value);
        }

        public void AddMinutes(double value)
        {
            Now = Now.AddMinutes(value);
        }

        public void AddSeconds(double value)
        {
            Now = Now.AddSeconds(value);
        }

        public void AddMilliseconds(double value)
        {
            Now = Now.AddMilliseconds(value);
        }

        #endregion
    }
}