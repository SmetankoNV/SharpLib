using System;

namespace SharpLib.Win.Source.Utils.PropertyGrid
{
    public class PropertyOrderPair : IComparable
    {
        #region ����

        private readonly int _order;

        #endregion

        #region ��������

        public string Name { get; }

        #endregion

        #region �����������

        public PropertyOrderPair(string name, int order)
        {
            _order = order;
            Name = name;
        }

        #endregion

        #region ������

        public int CompareTo(object obj)
        {
            //
            // Sort the pair objects by ordering by order value
            // Equal values get the same rank
            //
            var otherOrder = ((PropertyOrderPair)obj)._order;
            if (otherOrder == _order)
            {
                //
                // If order not specified, sort by name
                //
                var otherName = ((PropertyOrderPair)obj).Name;
                return string.Compare(Name, otherName);
            }
            if (otherOrder > _order)
            {
                return -1;
            }
            return 1;
        }

        #endregion
    }
}