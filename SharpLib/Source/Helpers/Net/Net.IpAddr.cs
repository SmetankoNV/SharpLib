using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Net
{
    /// <summary>
    /// Своя реализация представления IP-адреса (в .NET намудрили)
    /// </summary>
    public class NetIpAddr
    {
        #region Константы

        /// <summary>
        /// IP-адрес локального доступа (только для той же машины, где запущено приложение)
        /// </summary>
        public const string IF_LOOPBACK = "127.0.0.1";

        /// <summary>
        /// IP-адрес общего доступа (для любых сетевых интерфейсов)
        /// </summary>
        public const string IF_ANY = "0.0.0.0";

        #endregion

        #region Поля

        #endregion

        #region Свойства

        /// <summary>
        /// Текстовое представление адреса
        /// </summary>
        public string Text
        {
            get { return ToString(); }
            set { Value = ToIp(value); }
        }

        public uint Value { get; private set; }

        #endregion

        #region Конструктор

        public NetIpAddr()
        {
        }

        public NetIpAddr(string text)
        {
            Text = text;
        }

        public NetIpAddr(uint ip)
        {
            Value = ip;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Перевод в текстовое представление
        /// </summary>
        public override string ToString()
        {
            return ToText(Value);
        }

        /// <summary>
        /// Преобразование в 4 байтовое значение
        /// </summary>
        public static uint ToIp(string text)
        {
            text = text.ToLower();
            text = text.Replace(',', '.');
            if (text == "localhost")
            {
                text = IF_LOOPBACK;
            }
            var arr = text.Split('.');
            if (arr.Length == 4)
            {
                var a = arr[0].ToIntEx();
                var b = arr[1].ToIntEx();
                var c = arr[2].ToIntEx();
                var d = arr[3].ToIntEx();

                return Pack(a, b, c, d);
            }

            return 0;
        }

        /// <summary>
        /// Преобразование в текстовое значение
        /// </summary>
        public static string ToText(uint value)
        {
            var a = (byte)(value >> 24);
            var b = (byte)(value >> 16);
            var c = (byte)(value >> 8);
            var d = (byte)(value >> 0);

            string text = $"{a}.{b}.{c}.{d}";

            return text;
        }

        /// <summary>
        /// Упаковка 4-х байт в IPv4
        /// </summary>
        public static uint Pack(int a, int b, int c, int d)
        {
            var value = ((uint)a << 24) +
                        ((uint)b << 16) +
                        ((uint)c << 8) +
                        ((uint)d << 0);
            return value;
        }

        /// <summary>
        /// Сравнение IP
        /// </summary>
        public bool IsEqual(NetIpAddr other)
        {
            uint ip1 = ToIp(Text);
            uint ip2 = ToIp(other.Text);

            return ip1 == ip2;
        }

        #endregion
    }
}