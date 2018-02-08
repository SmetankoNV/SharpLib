using System;
using System.IO;
using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Hardware
{
    /// <summary>
    /// Общий класс, предоставляющий информации об операционной системе
    /// </summary>
    public class HardwareOs
    {
        #region Свойства

        /// <summary>
        /// Тип операционной системы
        /// </summary>
        public HardwareOsTyp Typ { get; }

        /// <summary>
        /// Версия операционной системы
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// Система - Windows
        /// </summary>
        public bool IsWindows
        {
            get { return Typ == HardwareOsTyp.Windows; }
        }

        /// <summary>
        /// Признак 64-битной OS
        /// </summary>
        public bool Is64Bit
        {
            get { return Environment.Is64BitOperatingSystem; }
        }

        /// <summary>
        /// Система - Linux
        /// </summary>
        public bool IsLinux
        {
            get { return Typ == HardwareOsTyp.Debian || Typ == HardwareOsTyp.Ubuntu || Typ == HardwareOsTyp.Yocto; }
        }

        #endregion

        #region Конструктор

        public HardwareOs()
        {
            Version = Environment.OSVersion.Version;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    Typ = HardwareOsTyp.Mac;
                    break;

                case PlatformID.Unix:
                    {
                        // Для определения yocto используется проверка существования директории /etc/opkg
                        if (Directory.Exists("/etc/opkg"))
                        {
                            Typ = HardwareOsTyp.Yocto;
                        }
                        else
                        {
                            // Для определения версии дистрибутива выполняется чтение файла /etc/lsb_release
                            var text = Files.Files.ReadText("/etc/lsb-release");
                            if (text.IsNotValid())
                            {
                                Typ = HardwareOsTyp.Unknown;
                            }
                            else if (text.ContainsEx("ubuntu"))
                            {
                                Typ = HardwareOsTyp.Ubuntu;
                            }
                            else if (text.ContainsEx("debian"))
                            {
                                Typ = HardwareOsTyp.Debian;
                            }
                            else
                            {
                                Typ = HardwareOsTyp.Unknown;
                            }
                        }
                    }

                    break;

                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                    Typ = HardwareOsTyp.Windows;
                    break;

                default:
                    Typ = HardwareOsTyp.Unknown;
                    break;
            }
        }

        #endregion
    }
}