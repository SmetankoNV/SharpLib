using System;

namespace SharpLib.Win.Source.Utils.PropertyGrid
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        #region Поля

        //
        // Simple attribute to allow the order of a property to be specified
        //

        #endregion

        #region Свойства

        public int Order { get; }

        #endregion

        #region Конструктор

        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }

        #endregion
    }
}