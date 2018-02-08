using System;

namespace SharpLib.Source.Helpers.Time
{
    /// <summary>
    /// Интерфейс класса работающего с датой/временем
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Текущее время (локальное)
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Текущее время (UTC)
        /// </summary>
        DateTime UtcNow { get; }
    }
}