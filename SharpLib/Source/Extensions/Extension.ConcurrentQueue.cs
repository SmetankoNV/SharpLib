using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SharpLib.Source.Extensions
{
    /// <summary>
    /// Класс расширения для "BlockingCollection"
    /// </summary>
    public static class ExtensionConcurrentQueue
    {
        #region Методы

        /// <summary>
        /// Извлечение всех элементов из очереди
        /// </summary>
        public static List<T> PopAllEx<T>(this ConcurrentQueue<T> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            var list = new List<T>();

            T item;
            while (self.TryDequeue(out item))
            {
                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Очистка очереди
        /// </summary>
        public static void ClearEx<T>(this ConcurrentQueue<T> self)
        {
            self.PopAllEx();
        }

        #endregion
    }
}