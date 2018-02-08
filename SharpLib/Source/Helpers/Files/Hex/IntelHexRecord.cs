using SharpLib.Source.Enums;
using SharpLib.Source.Extensions;
using SharpLib.Source.Extensions.Number;
using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Files.Hex
{
    public class IntelHexRecord
    {
        #region Свойства

        public byte Len => Buffer[0];

        public ushort Addr => Buffer.GetByte16Ex(1, Endianess.Big);

        public IntexHexRecordTyp Typ => (IntexHexRecordTyp)Buffer[3];

        public byte[] Data
        {
            get
            {
                var value = Mem.Clone(Buffer, 4, Len);

                return value;
            }
        }

        public byte Crc => Buffer[Buffer.Length - 1];

        public string Text { get; private set; }

        public byte[] Buffer { get; private set; }

        public bool IsValid
        {
            get
            {
                var crc0 = GetCrc(Buffer);
                var crc1 = Crc;

                return crc0 == crc1;
            }
        }

        public bool IsSegment => Typ == IntexHexRecordTyp.LinearAddr || Typ == IntexHexRecordTyp.SegmentAddr;

        public bool IsData => Typ == IntexHexRecordTyp.Data;

        public uint AddrSegment
        {
            get
            {
                uint value = 0;

                if (IsSegment)
                {
                    var tempAddr = Data.GetByte16Ex(0, Endianess.Big);

                    if (Typ == IntexHexRecordTyp.SegmentAddr)
                    {
                        value = (uint)(tempAddr << 4);
                    }
                    else
                    {
                        value = (uint)(tempAddr << 16);
                    }
                }

                return value;
            }
        }

        #endregion

        #region Конструктор

        public IntelHexRecord()
        {
            Buffer = null;
            Text = null;
        }

        public IntelHexRecord(string text) : this()
        {
            FromText(text);
        }

        public IntelHexRecord(ushort addr, IntexHexRecordTyp typ, byte[] data)
        {
            var list = new ByteList();

            var len = data?.Length ?? 0;

            list.AddByte8((byte)len);
            list.AddByte16(addr);
            list.AddByte8((byte)typ);
            list.AddBuffer(data);
            list.AddByte8(0xAA);

            Buffer = list.ToBuffer();

            UpdateCrc();
            UpdateText();
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Расчет контрольной суммы блока
        /// </summary>
        private byte GetCrc(byte[] record)
        {
            // Контрольная сумма расчитывается для всех полей, кроме последнего
            // (контрольной суммы)
            //
            // 020000040010EA = ~(0x02 + 0x00 + 0x00 + 0x04 + 0x10) + 1 = 0xEA
            //

            byte crc = 0;
            for (var i = 0; i < record.Length - 1; i++)
            {
                crc += record[i];
            }

            crc = (byte)(~crc + 1);

            return crc;
        }

        /// <summary>
        /// Заполнение полей записи на основании строки текста 
        /// Например ':020000040010EA'
        /// </summary>
        private void FromText(string text)
        {
            Text = text;
            // Удаление первого символа ":", поэтому offset = 1
            // :020000040010EA -> 020000040010EA
            text = text.TrimStart(':');
            // Преобразование: 020000040010EA -> 0x02 0x00 0x00 0x04 0x00 0x10 0xEA
            // 0x02      - количество байт данных
            // 0x00 0x00 - адрес
            // 0x04      - тип поля 
            // 0x00 0x10 - данные (зависит от типа записи)
            Buffer = text.ToAsciiBufferEx();
        }

        /// <summary>
        /// Перерасчет текстового представления
        /// </summary>
        private void UpdateText()
        {
            var text = ":" + Buffer.ToAsciiEx("");

            Text = text;
        }

        /// <summary>
        /// Перерасчет контрольной суммы (при изменении данных)
        /// </summary>
        public void UpdateCrc()
        {
            var localCrc = GetCrc(Buffer);

            Buffer[Buffer.Length - 1] = localCrc;
        }

        /// <summary>
        /// Формирование записи "Сегмент (расширенный)"
        /// </summary>
        public static IntelHexRecord SegmentRecord(uint addr)
        {
            // Для расширенного сегмента в поле данных адрес по 64K
            var addrSegment = (ushort)(addr >> 16);

            var data = addrSegment.ToBufferEx(Endianess.Big);

            var record = new IntelHexRecord(0, IntexHexRecordTyp.LinearAddr, data);

            return record;
        }

        /// <summary>
        /// Формирование записи "Данные"
        /// </summary>
        public static IntelHexRecord DataRecord(ushort addr, byte[] data)
        {
            var record = new IntelHexRecord(addr, IntexHexRecordTyp.Data, data);

            return record;
        }

        /// <summary>
        /// Формирование записи "Конец файла"
        /// </summary>
        public static IntelHexRecord EndRecord()
        {
            // Для расширенного сегмента в поле данных адрес по 64K
            var record = new IntelHexRecord(0, IntexHexRecordTyp.EndFile, null);

            return record;
        }

        #endregion
    }
}