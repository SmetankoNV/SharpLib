using System;

namespace SharpLib.Source.Helpers.Math
{
    /// <summary>
    /// Реализация генерации случайных чисел
    /// </summary>
    public class Rand
    {
        #region Поля

        public static readonly Random Value;

        private static readonly Random _rand;

        #endregion

        #region Конструктор

        static Rand()
        {
            Value = new Random();
            _rand = new Random(DateTime.Now.Ticks.GetHashCode());
        }

        #endregion

        #region Методы

        public static uint Get(uint min, uint max)
        {
            int value = _rand.Next((int)min, (int)max);

            return (uint)value;
        }

        public static uint Get(uint max)
        {
            return Get(0, max);
        }

        public static uint Get()
        {
            return Get(0, int.MaxValue);
        }

        public static int GetInt()
        {
            return (int)Get(0, int.MaxValue);
        }

        public static int GetInt(int max)
        {
            return (int)Get(0, (uint)max);
        }

        public static byte[] GetBuffer(int size)
        {
            byte[] buffer = new byte[size];

            _rand.NextBytes(buffer);

            return buffer;
        }

        #endregion
    }
}