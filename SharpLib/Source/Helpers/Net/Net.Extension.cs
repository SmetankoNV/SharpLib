using System.Diagnostics;
using System.Net;
using SharpLib.Source.Enums;
using SharpLib.Source.Extensions;
using SharpLib.Source.Extensions.Number;

namespace SharpLib.Source.Helpers.Net
{
    public static class NetExtension
    {
        #region Методы

        public static uint ToIpv4Ex(this IPAddress value)
        {
            var buf = value.GetAddressBytes();
            var result = buf.GetByte32Ex(0, Endianess.Big);

            return result;
        }

        public static EndPoint ToEndPointEx(this NetAddr value)
        {
            IPAddress ipAddr = new IPAddress(value.Ip.Value.SwitchOrderEx());
            IPEndPoint result = new IPEndPoint(ipAddr, value.Port);

            return result;
        }

        public static NetAddr ToNetAddrEx(this EndPoint value)
        {
            IPEndPoint ipEndPoint = value as IPEndPoint;
            Debug.Assert(ipEndPoint != null, "ipEndPoint != null");

            // ReSharper disable once PossibleNullReferenceException
            var ip = ipEndPoint.Address.ToIpv4Ex();
            var port = (ushort)ipEndPoint.Port;

            NetAddr result = new NetAddr(ip, port);

            return result;
        }

        #endregion
    }
}