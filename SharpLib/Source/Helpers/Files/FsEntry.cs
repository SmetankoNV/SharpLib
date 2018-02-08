using System;
using SharpLib.Source.Extensions.Number;

namespace SharpLib.Source.Helpers.Files
{
    public class FsEntry
    {
        #region Свойства

        /// <summary>
        /// Абсолютное расположение в файловой системе
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Название элемента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип элемента
        /// </summary>
        public FileTyp Typ { get; set; }

        /// <summary>
        /// Размер элемента
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Время модификация элемента
        /// </summary>
        public DateTime Time { get; set; }

        #endregion

        #region Конструктор

        public FsEntry(string location, string name, FileTyp typ, int size, DateTime time)
        {
            Location = location;
            Name = name;
            Typ = typ;
            Size = size;
            Time = time;
        }

        public override string ToString()
        {
            return string.Format("{0} (size={1}))", Name, Size.ToFileSizeEx());
        }

        #endregion
    }
}