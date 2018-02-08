namespace SharpLib.Source.Helpers.Images
{
    /// <summary>
    /// Эквивалент класса Color из System.Drawing
    /// </summary>
    /// <remarks>
    /// Введен в SharpLib чтобы не тянуть зависимость System.Drawing в Embedded-проекты
    /// В отличие от System.Drawing.Color хранится в 4-х байтном значении (т.о. нет свойства IsEmpty)
    /// </remarks>
    public struct ImageColor
    {
        #region Поля

        /// <summary>
        /// Белый цвет
        /// </summary>
        public static ImageColor White = new ImageColor(0xFFFFFFFF);

        /// <summary>
        /// Черный цвет 
        /// </summary>
        public static ImageColor Black = new ImageColor(0xFF000000);

        /// <summary>
        /// Желтый цвет 
        /// </summary>
        public static ImageColor Yellow = new ImageColor(0xFFFFFF00);

        /// <summary>
        /// Красный цвет 
        /// </summary>
        public static ImageColor Red = new ImageColor(0xFFFF0000);

        /// <summary>
        /// Синий цвет 
        /// </summary>
        public static ImageColor Blue = new ImageColor(0xFF0000FF);

        /// <summary>
        /// Зеленый цвет 
        /// </summary>
        public static ImageColor Green = new ImageColor(0xFF00FF00);

        /// <summary>
        /// Значение цвета в ARGB формате
        /// </summary>
        private readonly uint _value;

        #endregion

        #region Свойства

        /// <summary>
        /// Чтение значения красного компонента 
        /// </summary>
        public byte R
        {
            get { return (byte)((_value >> 16) & 0xFF); }
        }

        /// <summary>
        /// Чтение значения зеленого компонента 
        /// </summary>
        public byte G
        {
            get { return (byte)((_value >> 8) & 0xFF); }
        }

        /// <summary>
        /// Чтение значения синего компонента 
        /// </summary>
        public byte B
        {
            get { return (byte)(_value & 0xFF); }
        }

        /// <summary>
        /// Чтение значения альфа-компонента 
        /// </summary>
        public byte A
        {
            get { return (byte)(_value >> 24 & 0xFF); }
        }

        #endregion

        #region Конструктор

        public ImageColor(byte alpha, byte red, byte green, byte blue)
        {
            _value = MakeArgb(alpha, red, green, blue);
        }

        public ImageColor(byte red, byte green, byte blue)
        {
            _value = MakeArgb(0xFF, red, green, blue);
        }

        public ImageColor(int value)
        {
            _value = (uint)value;
        }

        public ImageColor(uint value)
        {
            _value = value;
        }

        #endregion

        #region Методы

        private static uint MakeArgb(byte alpha, byte red, byte green, byte blue)
        {
            return (uint)(red << 16 | green << 8 | blue | alpha << 24);
        }

        /// <summary>
        /// Создание структуры <see cref="ImageColor"/>
        /// </summary>
        public static ImageColor FromArgb(int alpha, int red, int green, int blue)
        {
            var value = MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue);

            return new ImageColor(value);
        }

        /// <summary>
        /// Создание структуры <see cref="ImageColor"/>
        /// </summary>
        public static ImageColor FromArgb(int red, int green, int blue)
        {
            return FromArgb(0xFF, red, green, blue);
        }

        /// <summary>
        /// 32-разрядное значение ARGB структуры
        /// </summary>
        public int ToArgb()
        {
            return (int)_value;
        }

        /// <summary>
        /// Текстовое представление
        /// </summary>
        public override string ToString()
        {
            return $"A={A}, R={R}, G={G}, B={B}";
        }

        #endregion
    }
}