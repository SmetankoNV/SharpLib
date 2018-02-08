using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SharpLib.Source.Extensions;

namespace SharpLib.Source.Helpers
{
    /// <summary>
    /// Класс, расширяющий возможности класса Enviroment
    /// </summary>
    public class Env
    {
        #region Свойства

        /// <summary>
        /// Признак запущенного приложения в профиле Debug
        /// </summary>
        public static bool IsDebug { get; }

        /// <summary>
        /// Признак выполнения кода под управлением отладчика Visual Studio
        /// </summary>
        public static bool IsVisualStudio { get; }

        /// <summary>
        /// Признак выполнения кода под Unit-тестированием
        /// </summary>
        public static bool IsUnit { get; }

        #endregion

        #region Конструктор

        static Env()
        {
            IsUnit = AppDomain.CurrentDomain.GetAssemblies()
                .Any(assem => assem.FullName.ToLowerInvariant()
                .StartsWith("nunit.framework"));

            var asm = IsUnit ? Assembly.GetExecutingAssembly() : Assembly.GetEntryAssembly();
            IsDebug = asm.IsDebugEx();
            IsVisualStudio = Debugger.IsAttached;
        }

        #endregion
    }
}