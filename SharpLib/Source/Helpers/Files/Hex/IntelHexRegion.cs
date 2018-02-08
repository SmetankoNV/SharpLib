using System;
using System.Linq;
using System.Text;
using SharpLib.Source.Extensions.Number;

namespace SharpLib.Source.Helpers.Files.Hex
{
    public sealed class IntelHexRegion : IComparable
    {
        #region Константы

        public const int REGION_SIZE = 64 * 1024;

        #endregion

        #region Поля

        private readonly byte[] _buffer;

        private readonly byte[] _present;

        #endregion

        #region Свойства

        public uint AddrStart { get; set; }

        public uint AddrEnd => AddrStart + REGION_SIZE - 1;

        public bool IsEmpty
        {
            get { return _present.All(present => present == 0); }
        }

        #endregion

        #region Конструктор

        public IntelHexRegion(uint addr)
        {
            AddrStart = addr;
            _buffer = new byte[REGION_SIZE];
            _present = new byte[REGION_SIZE];
        }

        #endregion

        #region Методы

        /// <summary>
        /// Текстовое представление класса (для отладки)
        /// </summary>
        public override string ToString()
        {
            string text = $"Addr: {AddrStart.ToStringEx(16)}";

            return text;
        }

        /// <summary>
        /// Реализация сравнения (по начальному адресу)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj)
        {
            var region = (IntelHexRegion)obj;

            if (AddrStart > region.AddrStart)
            {
                return 1;
            }
            if (AddrStart < region.AddrStart)
            {
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Проверка принадлежности адреса региону
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool IsValidAddr(uint addr)
        {
            if (addr >= AddrStart && addr <= AddrEnd)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Запись блока данных
        /// </summary>
        /// <param name="addr">Полный адрес записи</param>
        /// <param name="data">Блок данных</param>
        /// <returns>Количество записанных данных</returns>
        public int Write(uint addr, byte[] data)
        {
            if (IsValidAddr(addr))
            {
                var offsetAddr = (int)(addr % REGION_SIZE);
                // Расчет размера записываемых данных (с учетом возможного перехода на следующий регион)
                var remainSize = REGION_SIZE - offsetAddr;
                var writeSize = data.Length > remainSize ? remainSize : data.Length;

                for (var i = 0; i < writeSize; i++)
                {
                    _buffer[offsetAddr + i] = data[i];
                    _present[offsetAddr + i] = 1;
                }

                return writeSize;
            }

            return 0;
        }

        /// <summary>
        /// Чтение блока данных
        /// </summary>
        /// <param name="addr">Полный адрес записи</param>
        /// <param name="size">Размер блока данных</param>
        /// <returns>Прочитанный блок</returns>
        public byte[] Read(uint addr, int size)
        {
            if (IsValidAddr(addr))
            {
                var offsetAddr = (int)(addr % REGION_SIZE);
                // Расчет размера данных (с учетом возможного перехода на следующий регион)
                var remainSize = REGION_SIZE - offsetAddr;
                var readSize = size > remainSize ? remainSize : size;

                var data = new byte[readSize];

                for (var i = 0; i < readSize; i++)
                {
                    // Адрес не существует
                    if (_present[offsetAddr + i] == 0)
                    {
                        return null;
                    }

                    data[i] = _buffer[offsetAddr + i];
                }

                return data;
            }

            return null;
        }

        /// <summary>
        /// Удаление данных из региона
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public int Remove(uint addr, int size)
        {
            if (IsValidAddr(addr))
            {
                var offsetAddr = (int)(addr % REGION_SIZE);
                // Расчет размера данных (с учетом возможного перехода на следующий регион)
                var remainSize = REGION_SIZE - offsetAddr;
                var removeSize = size > remainSize ? remainSize : size;

                for (var i = 0; i < removeSize; i++)
                {
                    // Адрес не существует
                    _present[offsetAddr + i] = 0;
                    _buffer[offsetAddr + i] = 0;
                }

                return removeSize;
            }

            return 0;
        }

        /// <summary>
        /// Чтение буфера с заполнением
        /// </summary>
        public byte[] ReadFill(uint addr, int size, byte fill)
        {
            if (IsValidAddr(addr))
            {
                var offsetAddr = (int)(addr % REGION_SIZE);
                // Расчет размера данных (с учетом возможного перехода на следующий регион)
                var remainSize = REGION_SIZE - offsetAddr;
                var readSize = size > remainSize ? remainSize : size;

                var data = Mem.Set(readSize, fill);

                for (var i = 0; i < readSize; i++)
                {
                    if (_present[offsetAddr + i] != 0)
                    {
                        // Данные есть
                        data[i] = _buffer[offsetAddr + i];
                    }
                }

                return data;
            }

            return null;
        }

        /// <summary>
        /// Преобразование региона в текстовое представление
        /// </summary>
        public string ToText(int width)
        {
            var result = new StringBuilder();

            // Формирование записи "Регион" (для 0-го адреса описание региона не добавляется)
            if (AddrStart != 0)
            {
                var recordSegment = IntelHexRecord.SegmentRecord(AddrStart);
                result.Append(recordSegment.Text);
                result.Append(Environment.NewLine);
            }

            var buffer = new byte[width];
            var count = 0;
            var addr = -1;
            var flag = false;

            for (var i = 0; i < REGION_SIZE; i++)
            {
                if (_present[i] != 0)
                {
                    // Сохранение адреса начала блока
                    if (addr == -1)
                    {
                        addr = i;
                    }
                    // Сохранение байта в буфер
                    buffer[count++] = _buffer[i];

                    if (count >= width || (i == REGION_SIZE - 1))
                    {
                        // Набрана строка: Формирование записи
                        flag = true;
                    }
                } // end if (байт присутствует)
                else
                {
                    // Нет данных
                    if (addr != -1)
                    {
                        // Если предыдущие данные были -> Формирование записи
                        flag = true;
                    }
                }

                if (flag)
                {
                    // Набрана строка: Формирование записи
                    flag = false;
                    // Выделение данных из буфера (если количество меньше)
                    var data = Mem.Clone(buffer, 0, count);
                    // Формирование записи
                    var recordData = IntelHexRecord.DataRecord((ushort)addr, data);

                    // Добавление текстового представления к результату
                    result.Append(recordData.Text);
                    result.Append(Environment.NewLine);

                    // Сброс адреса и счетчика байт
                    addr = -1;
                    count = 0;
                }
            } // end for (перебор всех байт в регионе)

            return result.ToString();
        }

        /// <summary>
        /// Формирование сообщение об ошибке адреса
        /// </summary>
        public static string ErrorAddrText(uint addr)
        {
            string text = $"Адрес {addr.ToStringEx(16)} не существует";

            return text;
        }

        #endregion
    }
}