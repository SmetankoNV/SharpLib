using System.Threading;

namespace SharpLib.Source.Helpers.Threads
{
    /// <summary>
    /// Обертка над таймером из Threading.Timer с установкой события
    /// </summary>
    public class ThreadTimerEvent : ThreadTimer
    {
        #region Свойства

        public AutoResetEvent Event { get; }

        #endregion

        #region Конструктор

        public ThreadTimerEvent(int period) : base(period)
        {
            Event = new AutoResetEvent(false);
            Tick += (sender, args) => { Event.Set(); };
        }

        #endregion
    }
}