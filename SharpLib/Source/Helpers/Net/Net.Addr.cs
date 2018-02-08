using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Net
{
    /// <summary>
    /// Своя реализация представления сетевого адреса (IP + Port) (в .NET намудрили)
    /// </summary>
    public class NetAddr
    {
        #region Свойства

        /// <summary>
        /// IP адрес
        /// </summary>
        public NetIpAddr Ip { get; set; }

        /// <summary>
        /// Порт
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region Конструктор

        public NetAddr()
        {
            Ip = new NetIpAddr(NetIpAddr.IF_ANY);
            Port = 0;
        }

        public NetAddr(int port)
            : this()
        {
            Port = port;
        }

        public NetAddr(string ip, int port)
        {
            Ip = new NetIpAddr(ip);
            Port = port;
        }

        public NetAddr(uint ip, int port)
        {
            Ip = new NetIpAddr(ip);
            Port = port;
        }

        public NetAddr(string text)
        {
            Parse(text);
        }

        #endregion

        #region Методы

        public override string ToString()
        {
            return ToText(Ip.Value, Port);
        }

        public static string ToText(uint ip, int port)
        {
            string result = $"{NetIpAddr.ToText(ip)}:{port}";

            return result;
        }

        private void Parse(string value)
        {
            string[] blocks = value.Split(':');
            if (blocks.Length != 2)
            {
                return;
            }

            Ip = new NetIpAddr(blocks[0]);
            Port = blocks[1].ToIntEx();
        }

        #endregion
    }
}