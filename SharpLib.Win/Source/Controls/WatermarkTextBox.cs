using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SharpLib.Win.Source.Controls
{
    public class WatermarkTextBox : TextBox
    {
        #region Поля

        private string _watermark;

        #endregion

        #region Свойства

        [Category("SharpLib")]
        public string Watermark
        {
            get { return _watermark; }
            set
            {
                _watermark = value;
                UpdateWatermark();
            }
        }

        #endregion

        #region Методы

        private void UpdateWatermark()
        {
            if (IsHandleCreated && _watermark != null)
            {
                SendMessage(Handle, 0x1501, (IntPtr)1, _watermark);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateWatermark();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, string lp);

        #endregion
    }
}