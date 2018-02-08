using System.Collections;
using System.Linq;

namespace SharpLib.Source.Extensions.Linq
{
    /// <summary>
    /// Расширения класса IEnumerable
    /// </summary>
    public static class ExtensionEnumerable
    {
        public static bool Contains(this IEnumerable self, object item)
        {
            return self.Cast<object>().Any(o => o == item);
        }
    }
}