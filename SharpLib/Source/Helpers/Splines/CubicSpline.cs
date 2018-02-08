using System;

namespace SharpLib.Source.Helpers.Splines
{
    /// <summary>
    /// Класс кубического сплайна 
    /// Исходный код https://ru.wikipedia.org/wiki/%D0%9A%D1%83%D0%B1%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B8%D0%B9_%D1%81%D0%BF%D0%BB%D0%B0%D0%B9%D0%BD
    /// </summary>
    /// <remarks>
    /// Формула кубического сплайна: 
    ///  y(x)  = a + b * dx + c/2 * dx^2 + d/6 * dx^3 
    ///    где dx = x - xi
    ///        x  = значение x 
    ///        xi = значение x в начале сплайна
    ///  
    ///  y'(x) = b + c * dx + d/2 * dx^2 = b + (c + d/2 * dx) * dx
    ///          
    /// </remarks>
    public class CubicSpline
    {
        #region Поля

        /// <summary>
        /// Массив сплайнов
        /// </summary>
        private SplineTuple[] _splines;

        #endregion

        #region Свойства

        /// <summary>
        /// Признак расчитанных сплайнов
        /// </summary>
        public bool IsBuild
        {
            get { return _splines != null; }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Построение сплайна
        /// </summary>
        /// <param name="x">узлы сетки, должны быть упорядочены по возрастанию, кратные узлы запрещены</param>
        /// <param name="y">значения функции в узлах сетки</param>
        /// <param name="n">количество узлов сетки</param>
        public void Build(double[] x, double[] y, int n)
        {
            if (n < 2)
            {
                // Нельзя построить сплайн для точек меньше чем 1
                _splines = null;
                return;
            }

            // Инициализация массива сплайнов
            _splines = new SplineTuple[n];
            for (var i = 0; i < n; ++i)
            {
                _splines[i]._x = x[i];
                _splines[i]._a = y[i];
            }
            _splines[0]._c = _splines[n - 1]._c = 0.0;

            // Решение СЛАУ относительно коэффициентов сплайнов c[i] методом прогонки для трехдиагональных матриц
            // Вычисление прогоночных коэффициентов - прямой ход метода прогонки
            var alpha = new double[n - 1];
            var beta = new double[n - 1];
            alpha[0] = beta[0] = 0.0;
            for (var i = 1; i < n - 1; ++i)
            {
                var hi = x[i] - x[i - 1];
                var hi1 = x[i + 1] - x[i];
                var a = hi;
                var c = 2.0 * (hi + hi1);
                var b = hi1;
                var f = 6.0 * ((y[i + 1] - y[i]) / hi1 - (y[i] - y[i - 1]) / hi);
                var z = a * alpha[i - 1] + c;
                alpha[i] = -b / z;
                beta[i] = (f - a * beta[i - 1]) / z;
            }

            // Нахождение решения - обратный ход метода прогонки
            for (var i = n - 2; i > 0; --i)
            {
                _splines[i]._c = alpha[i] * _splines[i + 1]._c + beta[i];
            }

            // По известным коэффициентам c[i] находим значения b[i] и d[i]
            for (var i = n - 1; i > 0; --i)
            {
                var hi = x[i] - x[i - 1];
                _splines[i]._d = (_splines[i]._c - _splines[i - 1]._c) / hi;
                _splines[i]._b = hi * (2.0 * _splines[i]._c + _splines[i - 1]._c) / 6.0 + (y[i] - y[i - 1]) / hi;
            }
        }

        /// <summary>
        /// Поиск сплайна, содержащего нужное значение X
        /// </summary>
        /// <param name="x">Значение X</param>
        /// <param name="isExtrem">true: X вне диапазона [minX, maxX]</param>
        /// <returns>Ссылка на сплайн</returns>
        private SplineTuple FindSpline(double x, out bool isExtrem)
        {
            isExtrem = false;
            if (_splines == null)
            {
                throw new Exception("Spline need build");
            }

            var n = _splines.Length;
            SplineTuple spline;

            if (x <= _splines[0]._x)
            {
                // Если x меньше точки сетки x[0] - используется первый элемент массива
                spline = _splines[0];
                isExtrem = true;
            }
            else if (x >= _splines[n - 1]._x)
            {
                // Если x больше точки сетки x[n - 1] - используется послединий элемент массива
                spline = _splines[n - 1];
                isExtrem = true;
            }
            else
            {
                // Иначе x лежит между граничными точками сетки - выполяется бинарный поиск нужного эл-та массива
                var i = 0;
                var j = n - 1;
                while (i + 1 < j)
                {
                    var k = i + (j - i) / 2;
                    if (x <= _splines[k]._x)
                    {
                        j = k;
                    }
                    else
                    {
                        i = k;
                    }
                }
                spline = _splines[j];
            }

            return spline;
        }

        /// <summary>
        /// Вычисление значения интерполированной функции в произвольной точке
        /// </summary>
        /// <param name="x">Значение X</param>
        /// <returns>Значение Y</returns>
        public double Interpolate(double x)
        {
            bool isExtrem;
            var spline = FindSpline(x, out isExtrem);
            var dx = isExtrem ? 0 : x - spline._x;

            // Вычисление значение сплайна в заданной точке по схеме Горнера 
            // (в принципе, "умный" компилятор применил бы схему Горнера сам, но ведь не все так умны, как кажутся)
            var result = spline._a + (spline._b + (spline._c / 2.0 + spline._d * dx / 6.0) * dx) * dx;

            return result;
        }

        /// <summary>
        /// Вычислене значения 1-й производной
        /// </summary>
        /// <param name="x">Значение X</param>
        /// <returns>Значение Y</returns>
        public double Derived(double x)
        {
            bool isExtrem;
            var spline = FindSpline(x, out isExtrem);
            var dx = isExtrem ? 0 : x - spline._x;

            // Формула b + (c + d / 2 * dx) * dx
            var result = spline._b + (spline._c + spline._d / 2.0 * dx) * dx;

            return result;
        }

        public double GetMaxX()
        {
            if (_splines != null)
            {
                return _splines[_splines.Length - 1]._x;
            }
            return 0;
        }

        #endregion

        #region Вложенный класс: SplineTuple

        /// <summary>
        /// Структура, описывающая сплайн на каждом сегменте сетки
        /// 
        /// </summary>
        private struct SplineTuple
        {
            #region Поля

            /// <summary>
            /// Коэффициент a
            /// </summary>
            internal double _a;

            /// <summary>
            /// Коэффициент b
            /// </summary>
            internal double _b;

            /// <summary>
            /// Коэффициент c
            /// </summary>
            internal double _c;

            /// <summary>
            /// Коэффициент d
            /// </summary>
            internal double _d;

            /// <summary>
            /// Значение X в опорной точке сплайна
            /// </summary>
            internal double _x;

            #endregion

            #region Методы

            /// <summary>
            /// Текстовое представление
            /// </summary>
            public override string ToString()
            {
                return string.Format("{0} a={1:F2}, b={2:F2}, c={3:F2}, d={4:F2}", _x, _a, _b, _c, _d);
            }

            #endregion
        }

        #endregion
    }
}