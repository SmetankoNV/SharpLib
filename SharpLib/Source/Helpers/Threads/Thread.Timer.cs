using System;
using System.Threading;
using SharpLib.Source.Events;

namespace SharpLib.Source.Helpers.Threads
{
    /// <summary>
    /// Обертка над таймером из Threading.Timer
    /// </summary>
    public class ThreadTimer : IDisposable
    {
        #region Поля

        /// <summary>
        /// Таймер
        /// </summary>
        private readonly Timer _timer;

        #endregion

        #region Свойства

        /// <summary>
        /// Интервал сработки таймера (мс)
        /// </summary>
        public int Interval { get; set; }

        #endregion

        #region События

        /// <summary>
        /// Событие сработки таймера
        /// </summary>
        public event EventHandler<EventArgs<DateTime>> Tick;

        #endregion

        #region Конструктор

        public ThreadTimer()
        {
            _timer = new Timer(TimerCallback);
            Interval = 0;
            Stop();
        }

        public ThreadTimer(int interval) : this()
        {
            Interval = interval;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Событие сработки таймера
        /// </summary>
        private void TimerCallback(object state)
        {
            if (Tick != null)
            {
                Tick(this, new EventArgs<DateTime>(DateTime.Now));
            }
        }

        /// <summary>
        /// Запуск таймера
        /// </summary>
        public void Start()
        {
            if (Interval > 0)
            {
                _timer.Change(Interval, Interval);
            }
        }

        /// <summary>
        /// Остановка таймера
        /// </summary>
        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Удаление таймера
        /// </summary>
        public void Dispose()
        {
            Stop();
            _timer.Dispose();
        }

        #endregion
    }
}