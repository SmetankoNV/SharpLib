using System.Collections.Generic;

namespace SharpLib.Source.Extensions.Linq
{
    /// <summary>
    /// Класс расширения для "QueueT"
    /// </summary>
    public static class QueueT
    {
        public static void AddRange<T>(this Queue<T> self, IEnumerable<T> items)
        {
            foreach (T obj in items)
            {
                self.Enqueue(obj);
            }
        }
    }
}