namespace SharpLib.Source.Helpers.Threads
{
    /// <summary>
    /// Потокобезопасный общий идентификаторов
    /// </summary>
    public class SharedId
    {
        private readonly object _locker;

        private int _id;

        #region Свойства

        #endregion

        #region Конструктор

        public SharedId()
        {
            _locker = new object();
            _id = 0;
        }

        #endregion

        #region Методы

        public int GetNext()
        {
            lock (_locker)
            {
                if (++_id < 0)
                {
                    _id = 0;
                }

                return _id;
            }
        }

        #endregion
    }
}