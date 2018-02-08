using System;
using System.Collections.Concurrent;

namespace SharpLib.Source.Extensions
{
    /// <summary>
    /// Класс расширения для "BlockingCollection"
    /// </summary>
    public static class ExtensionBlockingCollection
    {
        #region Методы

        /// <summary>
        /// Очистка коллекции
        /// </summary>
        public static void ClearEx<T>(this BlockingCollection<T> self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            while (self.Count > 0)
            {
                T item;
                self.TryTake(out item);
            }
        }

        #endregion
    }
}