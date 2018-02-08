namespace SharpLib.Source.Helpers.Math
{
    /// <summary>
    /// Вспомогательный класс тригонометрических функций
    /// </summary>
    public static class Maths
    {
        #region Методы

        /// <summary>
        /// Расчет тангеса угла (в градусах)
        /// </summary>
        public static double Tan (double angle)
        {
            return System.Math.Tan(DegToRad(angle));
        }

        /// <summary>
        /// Расчет синуса угла (в градусах)
        /// </summary>
        public static double Sin(double angle)
        {
            return System.Math.Sin(DegToRad(angle));
        }

        /// <summary>
        /// Расчет косинуса угла (в градусах)
        /// </summary>
        public static double Cos(double angle)
        {
            return System.Math.Cos(DegToRad(angle));
        }

        /// <summary>
        /// Перевод градусов в радианы
        /// </summary>
        public static double DegToRad(double value)
        {
            return value * System.Math.PI / 180;
        }

        /// <summary>
        /// Перевод радиан в градусы
        /// </summary>
        public static double RadToDeg(double value)
        {
            return value * 180 / System.Math.PI;
        }

        #endregion
    }
}