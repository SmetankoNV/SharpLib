using System;
using System.Collections.Generic;
using System.Linq;
using SharpLib.Source.Enums;

namespace SharpLib.Source.Extensions.Linq
{
    /// <summary>
    /// Расширения класса IEnumerableT
    /// </summary>
    public static class ExtensionEnumerableT
    {
        #region Методы

        /// <summary>
        /// Объединение строк 
        /// </summary>
        public static string JoinEx<T>(this IEnumerable<T> self, string separator)
        {
            return string.Join(separator, self);
        }

        /// <summary>
        /// Представление дерева в виде плоского списка
        /// </summary>
        public static IEnumerable<T> FlatternEx<T>(this IEnumerable<T> self, Func<T, IEnumerable<T>> func)
        {
            var list = self.ToList();

            return list.SelectMany(c => func(c).FlatternEx(func)).Concat(list);
        }

        /// <summary>
        /// ForEach для IEnumerable-T
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var v in self)
            {
                action(v);
            }
        }

        /// <summary>
        /// Удаление дубликатов с lamba-выражением
        /// </summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        /// <summary>
        /// Удаление дубликатов с IComparer
        /// </summary>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }
            return DistinctByImpl(source, keySelector, comparer);
        }

        /// <summary>
        /// Реализация для DistinctBy
        /// </summary>
        private static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        /// <summary>
        /// Реализация Except с лямбдой
        /// </summary>
        /// <returns>
        /// Список элементов из списка first, которых нет в списке second
        /// </returns>
        public static IEnumerable<TSource> ExceptBy<TSource>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            Func<TSource, TSource, bool> func)
        {
            return first.Except(second, new LambdaComparer<TSource>(func));
        }

        /// <summary>
        /// Сортировка
        /// </summary>
        public static IEnumerable<TSource> SortEx<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector, SortMode mode = SortMode.Ascending)
        {
            return mode == SortMode.Ascending
                ? self.OrderBy(keySelector).ToList()
                : self.OrderByDescending(keySelector).ToList();
        }

        #endregion

        #region Вложенный класс: LambdaComparer

        private class LambdaComparer<T> : IEqualityComparer<T>
        {
            #region Поля

            private readonly Func<T, T, bool> _lambdaComparer;

            private readonly Func<T, int> _lambdaHash;

            #endregion

            #region Конструктор

            public LambdaComparer(Func<T, T, bool> lambdaComparer) :
                this(lambdaComparer, o => 0)
            {
            }

            private LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
            {
                if (lambdaComparer == null)
                {
                    throw new ArgumentNullException(nameof(lambdaComparer));
                }
                if (lambdaHash == null)
                {
                    throw new ArgumentNullException(nameof(lambdaHash));
                }

                _lambdaComparer = lambdaComparer;
                _lambdaHash = lambdaHash;
            }

            #endregion

            #region Методы

            public bool Equals(T x, T y)
            {
                return _lambdaComparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _lambdaHash(obj);
            }

            #endregion
        }

        #endregion
    }
}