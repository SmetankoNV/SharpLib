using System;
using System.Diagnostics;

namespace SharpLib.Source.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с консолью
    /// </summary>
    public class Consoler
    {
        #region Поля

        /// <summary>
        /// Объект для синхронизации многопоточного вывода
        /// </summary>
        private static readonly object _locker = new object();

        #endregion

        #region Методы

        /// <summary>
        /// Вывод текста в консоль
        /// </summary>
        public static void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            lock (_locker)
            {
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.Write(text);
                Console.ForegroundColor = currentColor;

                // Вывод в окно Output IDE (в режиме Debug)
                if (Env.IsDebug)
                {
                    Debug.WriteLine(text);
                }
            }
        }

        /// <summary>
        /// Вывод текста красным цветом
        /// </summary>
        public static void Error(string text, ConsoleColor color = ConsoleColor.Red)
        {
            PrintLine(text, color);
        }

        public static void PrintLine(string text, ConsoleColor color = ConsoleColor.White)
        {
            Print(text + Environment.NewLine, color);
        }

        public static ConsoleKey WaitKey(string text = "Нажмите любую клавишу...")
        {
            Print(text);

            // Ожидание нажатия клавиши
            var info = Console.ReadKey(true);

            return info.Key;
        }

        #endregion
    }
}