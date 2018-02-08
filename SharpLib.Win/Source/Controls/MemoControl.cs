using System;
using System.Drawing;
using System.Windows.Forms;

namespace SharpLib.Win.Source.Controls
{
    public sealed class MemoControl : RichTextBox
    {
        #region Fields

        private bool _autoscroll;

        #endregion

        #region Constructors

        public MemoControl()
        {
            var menu = new ContextMenuStrip();
            var clearItem = new ToolStripMenuItem("Clear", null, OnClearClick);
            var scrollItem = new ToolStripMenuItem("Scroll", null, OnScrollClick);
            scrollItem.CheckOnClick = true;
            scrollItem.Checked = true;

            menu.Items.Add(clearItem);
            menu.Items.Add(scrollItem);

            _autoscroll = true;

            ContextMenuStrip = menu;
            Font = new Font("Consolas", 10);
        }

        #endregion

        #region Members

        /// <summary>
        /// Контекстное меню "Scroll"
        /// </summary>
        private void OnScrollClick(object sender, EventArgs args)
        {
            _autoscroll = !_autoscroll;
        }

        /// <summary>
        /// Контекстное меню "Clear"
        /// </summary>
        private void OnClearClick(object sender, EventArgs args)
        {
            Clear();
        }

        /// <summary>
        /// Добавление текста с переносом строки
        /// </summary>
        internal void AddLineIntenal(string value, Color color)
        {
            SelectionColor = color;
            AppendText(value);
            AppendText(Environment.NewLine);
            SelectionColor = ForeColor;

            if (_autoscroll)
            {
                SelectionStart = TextLength;
                SelectionLength = 0;
                ScrollToCaret();
            }
        }

        /// <summary>
        /// Добавление текста с переносом строки
        /// </summary>
        public void AddLine(string value, Color color)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker) delegate { AddLineIntenal(value, color); });
            }
            else
            {
                AddLineIntenal(value, color);
            }
        }

        #endregion
    }
}