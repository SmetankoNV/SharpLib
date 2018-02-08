namespace SharpLib.Source.Helpers.Hardware
{
    /// <summary>
    /// Общий класс, предоставляющий информации об оборудовании
    /// </summary>
    public static class Hardware
    {
        #region Свойства

        /// <summary>
        /// Информация об операционной системе
        /// </summary>
        public static HardwareOs Os { get; }

        /// <summary>
        /// Информация о сетевой подсистеме
        /// </summary>
        public static HardwareNet Net { get; }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        static Hardware()
        {
            Os = new HardwareOs();
            Net = new HardwareNet();
        }

        #endregion
    }
}