namespace SharpLib.Source.Extensions.Number
{
    /// <summary>
    /// Методы расширения для bool
    /// </summary>
    public static class ExtensionBool
    {
        /// <summary>
        /// Преобразование к int
        /// </summary>
        public static int ToIntEx(this bool self)
        {
            return self ? 1 : 0;
        }
    }
}