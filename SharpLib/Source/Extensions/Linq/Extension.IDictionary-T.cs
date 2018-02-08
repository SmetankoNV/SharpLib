using System.Collections.Generic;

namespace SharpLib.Source.Extensions.Linq
{
    /// <summary>
    /// Расширения класса IDictionaryT
    /// </summary>
    public static class ExtensionDictionaryT
    {
        /// <summary>
        /// Чтение значения в словаре с указанием данных по умолчанию
        /// </summary>
        public static TValue GetValueEx<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue valueDefault)
        {
            TValue valueOut;
            if (dict.TryGetValue(key, out valueOut))
            {
                return valueOut;
            }

            return valueDefault;
        }
    }
}