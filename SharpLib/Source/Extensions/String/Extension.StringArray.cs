using System;

namespace SharpLib.Source.Extensions.String
{
    public static class ExtensionStringArray
    {
        #region Методы

        public static void SortEx(this string[] value, bool descending = false)
        {
            Array.Sort(value);
            if (descending)
            {
                Array.Reverse(value);
            }
        }

        #endregion
    }
}