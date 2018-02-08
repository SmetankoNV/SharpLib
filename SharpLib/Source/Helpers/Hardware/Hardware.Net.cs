using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace SharpLib.Source.Helpers.Hardware
{
    /// <summary>
    /// Общий класс, предоставляющий информации об сетевой подсистеме
    /// </summary>
    public class HardwareNet
    {
        #region Свойства

        /// <summary>
        /// Сетевые интерфейсы
        /// </summary>
        public List<HardwareNetIFace> Interfaces { get; }

        /// <summary>
        /// Имя машины 
        /// </summary>
        public string HostName
        {
            get { return Dns.GetHostName(); }
        }

        #endregion

        #region Конструктор

        public HardwareNet()
        {
            Interfaces = NetworkInterface
                .GetAllNetworkInterfaces()
                .Select(x => new HardwareNetIFace(x))
                .ToList();
        }

        #endregion
    }
}