namespace SharpLib.Source.Helpers.Images
{
    /// <summary>
    /// Базовый класс изображений
    /// </summary>
    public abstract class ImageBase
    {
        #region Свойства

        /// <summary>
        /// Тип изображения
        /// </summary>
        public ImageTyp Typ { get; private set; }

        /// <summary>
        /// Ширина изображения (пиксели)
        /// </summary>
        public int Width { get; protected set; }

        /// <summary>
        /// Высота изображения (пиксели)
        /// </summary>
        public int Height { get; protected set; }

        /// <summary>
        /// Глубина цвета (количество цветов в 1 пикселе)
        /// </summary>
        public int Depth { get; protected set; }

        /// <summary>
        /// Количество точек на дюйм (Dot Per Inch)
        /// </summary>
        public int Dpi { get; protected set; }

        /// <summary>
        /// Пиксели изображения
        /// </summary>
        public ImagePixels Pixels { get; protected set; }

        #endregion

        #region Конструктор

        protected ImageBase(ImageTyp typ)
        {
            Typ = typ;
        }

        #endregion
    }
}