using System.Net.NetworkInformation;

namespace SharpLib.Source.Helpers.Hardware
{
    /// <summary>
    /// Класс сетевого интерфейса
    /// </summary>
    public class HardwareNetIFace
    {
        #region Поля

        /// <summary>
        /// Ссылка на интерфейс-контейнер данных
        /// </summary>
        private readonly NetworkInterface _iface;

        #endregion

        #region Свойства

        /// <summary>
        /// Тип сетевого интерфейса
        /// </summary>
        public NetworkInterfaceType Typ
        {
            get { return _iface.NetworkInterfaceType; }
        }

        /// <summary>
        /// Тип сетевого интерфейса
        /// </summary>
        public string Ip
        {
            get
            {
                var unicast = _iface.GetIPProperties().UnicastAddresses;
                foreach (var ip in unicast)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }

                return string.Empty;
            }
        }

        #endregion

        #region Конструктор

        public HardwareNetIFace(NetworkInterface iface)
        {
            _iface = iface;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Текстовое представление
        /// </summary>
        public override string ToString()
        {
            return $"{Ip} {Typ}";
        }

        #endregion
    }
}