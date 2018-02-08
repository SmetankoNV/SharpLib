namespace SharpLib.Source.Events
{
    public class CancelEventArgs<T> : CancelEventArgs
    {
        #region Свойства

        public T Value { get; private set; }

        #endregion

        #region Конструктор

        public CancelEventArgs(T value)
        {
            Value = value;
        }

        #endregion
    }
}