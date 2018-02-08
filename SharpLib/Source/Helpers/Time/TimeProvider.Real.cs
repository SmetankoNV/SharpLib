using System;

namespace SharpLib.Source.Helpers.Time
{
    /// <summary>
    /// Провайдер реального системного времени
    /// </summary>
    public class TimeProviderReal : ITimeProvider
    {
        #region Свойства

        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        #endregion
    }
}