using System;
using System.Collections.Generic;
using System.Text;
using SharpLib.Source.Enums;
using SharpLib.Source.Extensions;

namespace SharpLib.Source.Helpers
{
    public class ByteList
    {
        #region Поля

        /// <summary>
        /// Внутренний список байт
        /// </summary>
        private readonly List<byte> _list;

        /// <summary>
        /// Endianess списка
        /// </summary>
        private readonly Endianess _endian;

        #endregion

        #region Свойства

        /// <summary>
        /// Количество байт в массиве
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// Байтовый массив
        /// </summary>
        public byte[] Buffer => ToBuffer();

        /// <summary>
        /// Внутреннее смещение в массиве
        /// (используется в операциях GetXXX)
        /// </summary>
        public int Offset { get; set; }

        #endregion

        #region Конструктор

        public ByteList(Endianess endian = Endianess.Big)
        {
            _endian = endian;
            _list = new List<byte>();
        }

        public ByteList(IEnumerable<byte> buffer, Endianess endian = Endianess.Big)
            : this(endian)
        {
            _list.AddRange(buffer);
        }

        #endregion

        #region Методы

        public void AddByte8(byte value)
        {
            _list.Add(value);
        }

        public void AddByte8(int value)
        {
            _list.Add((byte)value);
        }

        public void AddByte16(int value)
        {
            var buffer = new byte[2];
            Mem.PutByte16(buffer, 0, (ushort)value, _endian);

            _list.AddRange(buffer);
        }

        public void AddByte16(ushort value, Endianess endian = Endianess.Unknown)
        {
            var buffer = new byte[2];
            Mem.PutByte16(buffer, 0, value, endian == Endianess.Unknown ? _endian : endian);

            _list.AddRange(buffer);
        }

        /// <summary>
        /// Добавление 3-х байтового числа
        /// </summary>
        public void AddByte24(int value)
        {
            var buffer = new byte[3];
            Mem.PutByte24(buffer, 0, (uint)value);

            _list.AddRange(buffer);
        }

        /// <summary>
        /// Добавление 4-х байтового числа
        /// </summary>
        /// <param name="value"></param>
        public void AddByte32(int value)
        {
            var buffer = new byte[4];
            Mem.PutByte32(buffer, 0, (uint)value, _endian);

            _list.AddRange(buffer);
        }

        public void AddByte32(uint value, Endianess endian = Endianess.Unknown)
        {
            var buffer = new byte[4];
            Mem.PutByte32(buffer, 0, value, endian == Endianess.Unknown ? _endian : endian);

            _list.AddRange(buffer);
        }

        public void AddByte64(ulong value, Endianess endian = Endianess.Unknown)
        {
            var buffer = new byte[8];
            Mem.PutByte64(buffer, 0, value, endian == Endianess.Unknown ? _endian : endian);

            _list.AddRange(buffer);
        }

        /// <summary>
        /// Добавление буфера
        /// </summary>
        public void AddBuffer(byte[] buffer)
        {
            if (buffer != null)
            {
                _list.AddRange(buffer);
            }
        }

        /// <summary>
        /// Добавление буфера
        /// </summary>
        public void AddBuffer(byte[] buffer, int offset, int size)
        {
            if (buffer != null)
            {
                buffer = Mem.Clone(buffer, offset, size);
                _list.AddRange(buffer);
            }
        }

        /// <summary>
        /// Добавление строки
        /// </summary>
        public void AddString(string value, int arraySize = 0)
        {
            if (value == null)
            {
                return;
            }
            // Длина не передана: размещение всей строки
            if (arraySize == 0)
            {
                arraySize = value.Length + 1;
            }

            // Для строки используется кодировка 1251
            var bufStr = Encoding.GetEncoding(1251).GetBytes(value);
            var bufOut = new byte[arraySize];
            // Резервирование места под последний байт для создания
            // нуль-терминированной строки
            var size = System.Math.Min(bufStr.Length, arraySize - 1);
            // Формирование данных в выходной буфере
            Mem.Copy(bufOut, 0, bufStr, 0, size);
            // Добавление данных в список
            AddBuffer(bufOut);
        }

        /// <summary>
        /// Добавление Float
        /// </summary>
        public void AddFloat(float value)
        {
            var temp = BitConverter.GetBytes(value);

            _list.AddRange(temp);
        }

        /// <summary>
        /// Добавление double
        /// </summary>
        public void Adddouble(double value)
        {
            var temp = BitConverter.GetBytes(value);

            _list.AddRange(temp);
        }

        public void AddInt(int value, Endianess endian = Endianess.Unknown)
        {
            AddByte32((uint)value, endian);
        }

        public void AddLong(long value, Endianess endian = Endianess.Unknown)
        {
            AddByte64((ulong)value, endian);
        }

        /// <summary>
        /// Добавление времени как 4-х байтного Unix времени
        /// </summary>
        public void AddUnixSec(DateTime value)
        {
            var unix = (uint)(Date.DateTimeToUnixTime(value) / 1000);
            AddByte32(unix);
        }

        /// <summary>
        /// Добавление времени как 8-х байтного Unix времени (миллисекунды)
        /// </summary>
        public void AddUnixMilliseconds(DateTime value)
        {
            var unix = (ulong)Date.DateTimeToUnixTime(value);
            AddByte64(unix);
        }

        /// <summary>
        /// Добавление 2-х байтового знакового числа с умножением на 10
        /// </summary>
        public void AddS16X10(float value)
        {
            AddByte16((short)(value * 10));
        }

        /// <summary>
        /// Добавление 2-х байтового знакового числа с умножением на 100
        /// </summary>
        public void AddS16X100(float value)
        {
            AddByte16((short)(value * 100));
        }

        /// <summary>
        /// Добавление 2-х байтового ,беззнакового числа с умножением на 10
        /// </summary>
        public void AddU16X10(float value)
        {
            AddByte16((ushort)(value * 10));
        }

        /// <summary>
        /// Добавление 2-х байтового беззнакового числа с умножением на 100
        /// </summary>
        public void AddU16X100(float value)
        {
            AddByte16((ushort)(value * 100));
        }

        public void AddGuid(Guid value)
        {
            var buffer = value.ToBufferEx();

            AddBuffer(buffer);
        }

        private ulong GetByteCustom(int sizeValue, Endianess endian)
        {
            ulong value = 0;

            if (Offset + sizeValue <= Count)
            {
                var buffer = new byte[sizeValue];
                _list.CopyTo(Offset, buffer, 0, sizeValue);

                switch (sizeValue)
                {
                    case 1:
                        value = buffer.GetByte8Ex(0);
                        break;
                    case 2:
                        value = buffer.GetByte16Ex(0, endian);
                        break;
                    case 3:
                        value = buffer.GetByte24Ex(0, endian);
                        break;
                    case 4:
                        value = buffer.GetByte32Ex(0, endian);
                        break;
                    case 8:
                        value = buffer.GetByte64Ex(0, endian);
                        break;
                    default:
                        value = 0;
                        break;
                }

                Offset += sizeValue;
            }

            return value;
        }

        public byte GetByte8()
        {
            var value = (byte)GetByteCustom(1, Endianess.Little);

            return value;
        }

        public ushort GetByte16(Endianess endian = Endianess.Unknown)
        {
            var value = (ushort)GetByteCustom(2, endian == Endianess.Unknown ? _endian : endian);

            return value;
        }

        public uint GetByte24(Endianess endian = Endianess.Unknown)
        {
            var value = (uint)GetByteCustom(3, endian == Endianess.Unknown ? _endian : endian);

            return value;
        }

        public uint GetByte32(Endianess endian = Endianess.Unknown)
        {
            var value = (uint)GetByteCustom(4, endian == Endianess.Unknown ? _endian : endian);

            return value;
        }

        public ulong GetByte64(Endianess endian = Endianess.Unknown)
        {
            var value = GetByteCustom(8, endian == Endianess.Unknown ? _endian : endian);

            return value;
        }

        public byte[] GetBuffer(int size)
        {
            if (size == 0)
            {
                size = Count - Offset;
            }

            var buffer = new byte[size];

            if (Offset + size <= Count)
            {
                _list.CopyTo(Offset, buffer, 0, size);

                Offset += size;
            }

            return buffer;
        }

        public string GetString(int size = 0)
        {
            var includeNullCh = false;

            if (size == 0)
            {
                // Строка определяется по байту '\0'
                var offset = Offset;

                while (offset < Count)
                {
                    if (_list[offset++] == 0)
                    {
                        includeNullCh = true;
                        break;
                    }

                    size++;
                }

                // Данных в строке не обнаружено
                if (size == 0)
                {
                    Offset++;
                    return string.Empty;
                }
            }

            // Указан желаемый размер строки
            var buffer = GetBuffer(size);
            var value = Encoding.GetEncoding(1251).GetString(buffer);

            if (includeNullCh)
            {
                Offset++;
            }

            // Удаление завершающий нулей (\0) в случае со строкой фиксированного размера
            value = value.TrimEnd('\0');

            return value;
        }

        public float GetFloat()
        {
            var buffer = GetBuffer(4);
            var value = BitConverter.ToSingle(buffer, 0);

            return value;
        }

        public double Getdouble()
        {
            var buffer = GetBuffer(8);
            var value = BitConverter.ToDouble(buffer, 0);

            return value;
        }

        public int GetInt(Endianess endian = Endianess.Little)
        {
            return (int)GetByte32(endian);
        }

        public long GetLong(Endianess endian = Endianess.Little)
        {
            return (long)GetByte64(endian);
        }

        /// <summary>
        /// Извлечение 2-х байтового знакового числа умноженного на 10
        /// </summary>
        public float GetS16X10()
        {
            var value = (short)GetByte16();

            return (float)value / 10;
        }

        /// <summary>
        /// Извлечение 2-х байтового знакового числа умноженного на 100
        /// </summary>
        public float GetS16X100()
        {
            var value = (short)GetByte16();

            return (float)value / 100;
        }

        /// <summary>
        /// Извлечение 2-х байтового беззнакового числа умноженного на 10
        /// </summary>
        public float GetU16X10()
        {
            var value = GetByte16();

            return (float)value / 10;
        }

        /// <summary>
        /// Извлечение 2-х байтового беззнакового числа умноженного на 100
        /// </summary>
        public float GetU16X100()
        {
            var value = GetByte16();

            return (float)value / 100;
        }

        /// <summary>
        /// Извлечение времени как 4-х байтного Unix времени (секунды)
        /// </summary>
        public DateTime GetUnixSec()
        {
            var unix = GetByte32();

            return Date.UnixTimeToDateTime((long)unix * 1000);
        }

        /// <summary>
        /// Извлечение времени как 8-и байтного Unix времени (миллисекунды)
        /// </summary>
        public DateTime GetUnixMilliseconds()
        {
            var unix = GetByte64();

            return Date.UnixTimeToDateTime((long)unix);
        }

        public Guid GetGuid()
        {
            var buffer = GetBuffer(16);
            var guid = buffer.ToGuidEx();

            return guid;
        }

        public byte[] ToBuffer()
        {
            var buffer = _list.ToArray();

            return buffer;
        }

        /// <summary>
        /// Очистка списка
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            Offset = 0;
        }

        /// <summary>
        /// Пропуск байт
        /// </summary>
        public void GetSkip(int size)
        {
            GetBuffer(size);
        }

        #endregion
    }
}