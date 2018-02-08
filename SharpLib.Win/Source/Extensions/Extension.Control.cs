using System;
using System.Windows.Forms;

namespace SharpLib.Win.Source.Extensions
{
    /// <summary>
    /// Класс расширения для "Control" из WinForms
    /// </summary>
    public static class ExtensionControl
    {
        /// <summary>
        /// Вызов Invoke с использованием lambda
        /// </summary>
        public static void InvokeEx(this Control control, Action action)
        {
            control.Invoke(action);
        }
    }
}