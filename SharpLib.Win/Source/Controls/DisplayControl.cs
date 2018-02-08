using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpLib.Source.Extensions.Linq;
using SharpLib.Source.Helpers.Math;
using SharpLib.Win.Source.Extensions;

namespace SharpLib.Win.Source.Controls
{
    public sealed partial class DisplayControl : UserControl
    {
        #region Константы

        private const int COL_COUNT = 96;

        private const int MARGIN = 25;

        private const int ROW_COUNT = 96;

        #endregion

        #region Поля

        /// <summary>
        /// Карта точек
        /// </summary>
        private readonly PointMap _map;

        /// <summary>
        /// Цвет текста цифр
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly Color COLOR_NUMBERS = Color.White;

        /// <summary>
        /// Выделенная ячейка (выключена)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly Color COLOR_SEL_OFF = Color.AntiqueWhite;

        /// <summary>
        /// Выделенная ячейка (включена)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly Color COLOR_SEL_ON = Color.Yellow;

        /// <summary>
        /// Признак движения мыши
        /// </summary>
        private bool _moveMouse;

        /// <summary>
        /// Признак переноса экрана
        /// </summary>
        private bool _moveScreen;

        /// <summary>
        /// Начальная точка переноса
        /// </summary>
        private GridPoint _moveScreenPoint;

        /// <summary>
        /// Признак движения при выделении
        /// </summary>
        private bool _moveWithSelection;

        /// <summary>
        /// Начальная точка выделения
        /// </summary>
        private GridPoint _selectStart;

        #endregion

        /// <summary>
        /// Количество строк в дисплее
        /// </summary>
        [Description("Rows count")]
        [DefaultValue(ROW_COUNT)]
        public int Rows { get; set; }

        /// <summary>
        /// Количество колонок в дисплее
        /// </summary>
        [Description("Columns count")]
        [DefaultValue(COL_COUNT)]
        public int Cols { get; set; }

        /// <summary>
        /// true - возможно пользовательское редактирование
        /// </summary>
        [Description("Can edit on mouse click")]
        [DefaultValue(false)]
        public bool Editable { get; set; }

        /// <summary>
        /// Цвет фона дисплея
        /// </summary>
        [Description("Background color")]
        public Color Background { get; set; }

        /// <summary>
        /// Цвет вывода дисплея
        /// </summary>
        [Description("Foreground color")]
        public Color Foreground { get; set; }

        /// <summary>
        /// Отображать сетку
        /// </summary>
        [Description("Show grid cells")]
        [DefaultValue(true)]
        public bool ShowGrid { get; set; }

        /// <summary>
        /// Отображать подписи по осям
        /// </summary>
        [Description("Show numbers (cols/rows)")]
        [DefaultValue(true)]
        public bool ShowNumbers { get; set; }

        #region Конструктор

        public DisplayControl()
        {
            InitializeComponent();

            ResizeRedraw = true;
            DoubleBuffered = true;

            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Items.Add("Установить все", null, MenuSetAllClick);
            ContextMenuStrip.Items.Add("Сбросить все", null, MenuClearAllClick);
            ContextMenuStrip.Items.Add("Случайно", null, MenuRandomClick);
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add("Открыть BMP...", null, MenuOpenBmpClick);
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add("Сохранить в BMP...", null, MenuSaveBmpClick);

            Rows = ROW_COUNT;
            Cols = COL_COUNT;
            Editable = false;
            Background = Color.Black;
            Foreground = Color.Aqua;
            ShowGrid = true;
            ShowNumbers = true;

            _map = new PointMap(Background);
            _map.Random(Foreground, Background);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Пользовательская перерисовка
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(Background);
            var rect = ClientRectangle;
            var gridRect = new Rectangle(0 + MARGIN, 0 + MARGIN, rect.Width - MARGIN * 2, rect.Height - MARGIN * 2);

            if (ShowGrid)
            {
                DrawGrid(e.Graphics, gridRect);
            }

            if (ShowNumbers)
            {
                DrawNumbers(e.Graphics, gridRect);
            }
            
            DrawMap(e.Graphics, gridRect);
        }

        /// <summary>
        /// Обработка нажатия "Esc" (сброс выделения)
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                _map.Points.ForEach(x => x.Selected = false);
                _selectStart = null;
                Refresh();
            }
        }

        /// <summary>
        /// Нажатие мыши "Mouse Down"
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            var point = SearchGridPoint(e.X, e.Y);
            if (point == null)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    _selectStart = point;
                    _moveWithSelection = true;
                }
                else
                {
                    _moveMouse = true;

                    Refresh();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                _moveScreen = true;
                _moveScreenPoint = point;
            }
        }

        /// <summary>
        /// Нажатие мыши "Mouse Up"
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _moveWithSelection = false;
            _moveMouse = false;
            _moveScreen = false;
            _moveScreenPoint = null;
        }

        /// <summary>
        /// Нажатие мыши "Mouse Move"
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_moveWithSelection)
            {
                var point = SearchGridPoint(e.X, e.Y);

                if (point != null)
                {
                    var minX = Math.Min(_selectStart.X, point.X);
                    var minY = Math.Min(_selectStart.Y, point.Y);
                    var maxX = Math.Max(_selectStart.X, point.X);
                    var maxY = Math.Max(_selectStart.Y, point.Y);

                    _map.Points.ForEach(p => p.Selected = false);
                    _map.SearchGroup(minX, minY, maxX, maxY)
                        .ForEach(p => p.Selected = true);

                    Refresh();
                }
            }
            else if (_moveMouse)
            {
                var point = SearchGridPoint(e.X, e.Y);

                if (point != null)
                {
                    point.Set(Foreground);
                    Refresh();
                }
            }
            else if (_moveScreen)
            {
                var point = SearchGridPoint(e.X, e.Y);
                if (point == null || _moveScreenPoint == null)
                {
                    return;
                }

                // Расчет дельты сдвига
                var deltaX = point.X - _moveScreenPoint.X;
                var deltaY = point.Y - _moveScreenPoint.Y;

                if (deltaX == 0 && deltaY == 0)
                {
                    return;
                }

                _map.Scroll(deltaX, deltaY, Background);
                Refresh();

                _moveScreenPoint = point;
            }
        }

        /// <summary>
        /// Поиск пикселя по экранным координатам
        /// </summary>
        private GridPoint SearchGridPoint(float x, float y)
        {
            var rect = ClientRectangle;

            var stepX = ((float)rect.Width - 2 * MARGIN) / COL_COUNT;
            var stepY = ((float)rect.Height - 2 * MARGIN) / ROW_COUNT;

            var indexX = (int)((x - MARGIN) / stepX);
            var indexY = (int)((y - MARGIN) / stepY);

            var point = _map.Points.FirstOrDefault(p => p.X == indexX && p.Y == indexY);

            return point;
        }

        /// <summary>
        /// Обработка нажатия "Установить все"
        /// </summary>
        private void MenuSetAllClick(object sender, EventArgs eventArgs)
        {
            _map.SetAll(Foreground);
            Refresh();
        }

        /// <summary>
        /// Обработка нажатия "Сбросить все"
        /// </summary>
        private void MenuClearAllClick(object sender, EventArgs eventArgs)
        {
            _map.SetAll(Background);
            Refresh();
        }

        /// <summary>
        /// Обработка нажатия "Сбросить все"
        /// </summary>
        private void MenuRandomClick(object sender, EventArgs eventArgs)
        {
            _map.Random(Foreground, Background);
            Refresh();
        }

        /// <summary>
        /// Обработка нажатия "Открыть BMP"
        /// </summary>
        private void MenuOpenBmpClick(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    var bitmap = (Bitmap)Image.FromFile(dialog.FileName);

                    var maxX = Math.Min(COL_COUNT, bitmap.Width);
                    var maxY = Math.Min(ROW_COUNT, bitmap.Height);

                    _map.SetAll(Background);

                    // Перевод изображения в монохром + вывод на дисплей
                    for (var y = 0; y < maxY; y++)
                    {
                        for (var x = 0; x < maxX; x++)
                        {
                            var color = bitmap.GetPixel(x, y);
                            var grayScale = (int)((color.R * 0.3) + (color.G * 0.59) + (color.B * 0.11));

                            var p = _map.Search(x, y);
                            if (grayScale < 128)
                            {
                                p?.Set(Foreground);
                            }
                            else
                            {
                                p?.Set(Background);
                            }
                        }
                    }

                    // Обновление дисплея
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Сохранить изображение в BMP
        /// </summary>
        private void MenuSaveBmpClick(object sender, EventArgs e)
        {
            List<GridPoint> points;
            if (_map.Points.Any(x => x.Selected))
            {
                // Сохранение выделенного фрагмента
                points = _map.Points.Where(x => x.Selected).ToList();
            }
            else
            {
                // Сохранение всего изображения
                points = _map.Points;
            }

            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Bitmap Image (.bmp)|*.bmp";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var bitmap = CreateBitmap(points);
                bitmap.Save(dialog.FileName);
            }
        }

        /// <summary>
        /// Создание изображения из картинки
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private Bitmap CreateBitmap(List<GridPoint> points)
        {
            int minX = points.Min(p => p.X);
            int maxX = points.Max(p => p.X);
            int minY = points.Min(p => p.Y);
            int maxY = points.Max(p => p.Y);
            int w = maxX - minX + 1;
            int h = maxY - minY + 1;

            byte[] data = new byte[w * h];

            for (int i = 0; i < points.Count; i++)
            {
                data[i] = (byte)(points[i].Color != Background ? 255 : 0);
            }

            var bitmap = new Bitmap(w, h, PixelFormat.Format8bppIndexed);
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bmData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            Marshal.Copy(data, 0, bmData.Scan0, w * h);
            bitmap.UnlockBits(bmData);

            return bitmap;
        }

        /// <summary>
        /// Отображение сетки
        /// </summary>
        private void DrawGrid(Graphics g, Rectangle rect)
        {
            using (var pen = new Pen(Color.DarkGray, 2))
            {
                g.DrawRectangle(pen, rect);

                var p1 = new Point();
                var p2 = new Point();
                var stepX = (double)rect.Width / COL_COUNT;
                var stepY = (double)rect.Height / ROW_COUNT;

                // Вертикальные линии
                for (var x = 0; x < COL_COUNT; x++)
                {
                    p1.X = p2.X = rect.X + (int)(x * stepX);
                    p1.Y = rect.Y;
                    p2.Y = rect.Y + rect.Height;
                    g.DrawLine(pen, p1, p2);
                }

                // Горизонтальные линии
                for (var y = 0; y < ROW_COUNT; y++)
                {
                    p1.Y = p2.Y = rect.Y + (int)(y * stepY);
                    p1.X = rect.X;
                    p2.X = rect.X + rect.Width;
                    g.DrawLine(pen, p1, p2);
                }
            }
        }

        /// <summary>
        /// Отображение подписей X, Y
        /// </summary>
        private void DrawNumbers(Graphics g, Rectangle rect)
        {
            using (Brush brush = new SolidBrush(COLOR_NUMBERS))
            {
                using (var font = new Font("Consolas", 8))
                {
                    float x0;
                    float y0;
                    var stepX = (float)(rect.Width) / COL_COUNT;
                    var stepY = (float)(rect.Height) / ROW_COUNT;
                    var offsetX = rect.X + stepX / 2;
                    var offsetY = rect.Y + stepY / 2;

                    // Подписи по X
                    for (var x = 0; x < COL_COUNT; x++)
                    {
                        x0 = offsetX - 10 + stepX * x;
                        y0 = (float)(rect.Y - MARGIN * 0.8);
                        g.DrawString(x.ToString(), font, brush, x0, y0);
                    }

                    // Подписи по Y
                    for (var y = 0; y < ROW_COUNT; y++)
                    {
                        x0 = (float)(rect.X - MARGIN * 0.9);
                        y0 = offsetY - 10 + stepY * y;
                        g.DrawString(y.ToString(), font, brush, x0, y0);
                    }
                }
            }
        }

        /// <summary>
        /// Отображение карты точек
        /// </summary>
        private void DrawMap(Graphics g, Rectangle rect)
        {
            using (var brushSelOn = new SolidBrush(COLOR_SEL_ON))
            {
                using (var brushSelOff = new SolidBrush(COLOR_SEL_OFF))
                {
                    var boxWidth = (float)rect.Width / COL_COUNT;
                    var boxHeight = (float)rect.Height / ROW_COUNT;

                    foreach (var point in _map.Points)
                    {
                        var x0 = rect.X + point.X * boxWidth;
                        var y0 = rect.Y + point.Y * boxHeight;

                        Brush b = null;

                        using (var brushOn = new SolidBrush(point.Color))
                        {
                            if (point.Selected)
                            {
                                b = point.Color != Background ? brushSelOn : brushSelOff;
                            }
                            else if (point.Color != Background)
                            {
                                b = brushOn;
                            }

                            if (b != null)
                            {
                                g.FillRectangle(b, x0 + 1, y0 + 1, boxWidth - 1, boxHeight - 1);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Установка пискеля
        /// </summary>
        /// <param name="x">Позиция по X</param>
        /// <param name="y">Позиция по Y</param>
        /// <param name="value">Цвет</param>
        public void SetPixel(int x, int y, Color value)
        {
            _map.PointsArr[x, y].Color = value;
        }

        /// <summary>
        /// Установка пискеля
        /// </summary>
        /// <param name="x">Позиция по X</param>
        /// <param name="y">Позиция по Y</param>
        /// <param name="value">Цвет в формате RGB565</param>
        public void SetPixel(int x, int y, UInt16 value)
        {
            var color = ExtensionColor.ToRgb888(value);

            SetPixel(x, y, color);
        }

        #endregion

        #region Вложенный класс: GridPoint

        internal class GridPoint
        {
            #region Поля

            public int Index;

            public bool Selected;

            public int X;

            public int Y;

            public Color Color;

            #endregion

            #region Конструктор

            public GridPoint(int index, int x, int y, Color color)
            {
                Index = index;
                X = x;
                Y = y;
                Color = color;
                Selected = false;
            }

            #endregion

            #region Методы

            public void Set(Color color)
            {
                Color = color;
            }

            public override string ToString()
            {
                return $"Index={Index} X={X}, Y={Y}";
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: PointMap

        internal class PointMap
        {
            #region Поля

            /// <summary>
            /// Точки в виде массива
            /// </summary>
            internal readonly GridPoint[,] PointsArr;

            #endregion

            #region Свойства

            /// <summary>
            /// Карта точек
            /// </summary>
            public List<GridPoint> Points { get; }

            #endregion

            #region Конструктор

            public PointMap(Color back)
            {
                Points = new List<GridPoint>();
                PointsArr = new GridPoint[COL_COUNT, ROW_COUNT];
                var index = 0;
                for (var y = 0; y < ROW_COUNT; y++)
                {
                    for (var x = 0; x < COL_COUNT; x++)
                    {
                        var p = new GridPoint(index++, x, y, back);
                        PointsArr[x, y] = p;
                        Points.Add(p);
                    }
                }
            }

            #endregion

            #region Методы

            public void SetAll(Color color)
            {
                Points.ForEach(x => x.Color = color);
            }

            public void Random(Color fore, Color back)
            {
                Points.ForEach(x => x.Color = Rand.Get(10) > 5 ? back : fore);
            }

            public GridPoint Search(int x, int y)
            {
                return Points.FirstOrDefault(p => p.X == x && p.Y == y);
            }

            public IEnumerable<GridPoint> SearchGroup(int x0, int y0, int x1, int y1)
            {
                return Points.Where(p => p.X >= x0 && p.X <= x1 && p.Y >= y0 && p.Y <= y1);
            }

            /// <summary>
            /// Сдвиг всех точек
            /// </summary>
            public void Scroll(int deltaX, int deltaY, Color back)
            {
                var offsetX = Math.Abs(deltaX);
                if (deltaX < 0)
                {
                    for (var x = 0; x < COL_COUNT; x++)
                    {
                        for (var y = 0; y < ROW_COUNT; y++)
                        {
                            if ((x + offsetX) < COL_COUNT)
                            {
                                PointsArr[x, y].Color = PointsArr[x + offsetX, y].Color;
                                PointsArr[x, y].Selected = PointsArr[x + offsetX, y].Selected;
                            }
                            else
                            {
                                PointsArr[x, y].Color = back;
                                PointsArr[x, y].Selected = false;
                            }
                        }
                    }
                }
                else if (deltaX > 0)
                {
                    for (var x = COL_COUNT - 1; x >= 0; x--)
                    {
                        for (var y = 0; y < ROW_COUNT; y++)
                        {
                            if ((x - offsetX) >= 0)
                            {
                                PointsArr[x, y].Color = PointsArr[x - offsetX, y].Color;
                                PointsArr[x, y].Selected = PointsArr[x - offsetX, y].Selected;
                            }
                            else
                            {
                                PointsArr[x, y].Color = back;
                                PointsArr[x, y].Selected = false;
                            }
                        }
                    }
                }

                var offsetY = Math.Abs(deltaY);
                if (deltaY < 0)
                {
                    for (var y = 0; y < ROW_COUNT; y++)
                    {
                        for (var x = 0; x < COL_COUNT; x++)
                        {
                            if ((y + offsetY) < ROW_COUNT)
                            {
                                PointsArr[x, y].Color = PointsArr[x, y + offsetY].Color;
                                PointsArr[x, y].Selected = PointsArr[x, y + offsetY].Selected;
                            }
                            else
                            {
                                PointsArr[x, y].Color = back;
                                PointsArr[x, y].Selected = false;
                            }
                        }
                    }
                }
                else if (deltaY > 0)
                {
                    for (var y = ROW_COUNT - 1; y >= 0; y--)
                    {
                        for (var x = 0; x < COL_COUNT; x++)
                        {
                            if ((y - offsetY) >= 0)
                            {
                                PointsArr[x, y].Color = PointsArr[x, y - offsetY].Color;
                                PointsArr[x, y].Selected = PointsArr[x, y - offsetY].Selected;
                            }
                            else
                            {
                                PointsArr[x, y].Color = back;
                                PointsArr[x, y].Selected = false;
                            }
                        }
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}