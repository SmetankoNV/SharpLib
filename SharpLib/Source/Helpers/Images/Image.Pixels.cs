namespace SharpLib.Source.Helpers.Images
{
    /// <summary>
    /// Базовый класс изображений
    /// </summary>
    public class ImagePixels
    {
        #region Поля

        /// <summary>
        /// Массив пикселей (каждый пиксель хранит цвет)
        /// </summary>
        private readonly int[,] _map;

        #endregion

        #region Свойства

        /// <summary>
        /// Высота карты (пиксели)
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Ширины карты (пиксели)
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Оператор индексов
        /// </summary>
        public int this[int x, int y]
        {
            get { return _map[x, y]; }
            set { _map[x, y] = value; }
        }

        #endregion

        #region Конструктор

        public ImagePixels(int width, int height)
        {
            Width = width;
            Height = height;
            _map = new int[width, height];
        }

        #endregion
    }
}