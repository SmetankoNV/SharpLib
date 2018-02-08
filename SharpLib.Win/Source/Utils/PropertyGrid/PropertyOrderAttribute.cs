using System;

namespace SharpLib.Win.Source.Utils.PropertyGrid
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        #region ����

        //
        // Simple attribute to allow the order of a property to be specified
        //

        #endregion

        #region ��������

        public int Order { get; }

        #endregion

        #region �����������

        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }

        #endregion
    }
}