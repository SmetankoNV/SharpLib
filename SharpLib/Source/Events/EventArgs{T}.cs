using System;

namespace SharpLib.Source.Events
{
    public class EventArgs<T> : EventArgs
    {
        #region Свойства

        public T Value { get; private set; }

        #endregion

        #region Конструктор

        public EventArgs(T value)
        {
            Value = value;
        }

        #endregion
    }
}