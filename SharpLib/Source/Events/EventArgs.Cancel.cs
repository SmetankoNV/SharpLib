using System;

namespace SharpLib.Source.Events
{
    public class CancelEventArgs : EventArgs
    {
        #region Свойства

        public bool Cancel { get; set; }

        #endregion

        #region Конструктор

        public CancelEventArgs()
            : this(false)
        {
        }

        public CancelEventArgs(bool cancel)
        {
            Cancel = cancel;
        }

        #endregion
    }
}