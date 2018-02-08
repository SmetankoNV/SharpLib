using System.Drawing;
using System.Windows.Forms;
using SharpLib.Win.Source.Controls;

namespace SharpLib.Win.Source.Utils.Dialogs
{
    /// <summary>
    /// Отображение диалоговых окон
    /// </summary>
    public static class Dialogs
    {
        #region Методы

        /// <summary>
        /// Отображение окна с иконкой инфомрации
        /// </summary>
        public static void ShowInfo(IWin32Window owner, string text, string caption = null)
        {
            MessageBox.Show(owner, text, caption ?? "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Отображение окна с иконкой ошибки
        /// </summary>
        public static void ShowError(IWin32Window owner, string text, string caption = null)
        {
            MessageBox.Show(owner, text, caption ?? "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Отображение окна с иконкой вопроса
        /// </summary>
        public static bool ShowQuestion(IWin32Window owner, string text, string caption = null)
        {
            var result = MessageBox.Show(owner, text, caption ?? "Вопрос", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }

        /// <summary>
        /// Отображение диалогового окна с полем ввода
        /// </summary>
        /// <param name="text">Текст в поле</param>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="hint">Подсказка в поле ввода</param>
        /// <returns>Введенный в поле результат</returns>
        public static string ShowPromt(string caption, string text, string hint = null)
        {
            var prompt = new Form
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                MaximizeBox = false,
                MinimizeBox = false,
                StartPosition = FormStartPosition.CenterScreen
            };

            var font = new Font("Consolas", 10);
            var textBox = new WatermarkTextBox
            {
                Text = text,
                Left = 50,
                Top = 20,
                Width = 400,
                Watermark = hint,
                Font = font
            };
            var confirmation = new Button
            {
                Text = "OK",
                Left = 350,
                Width = 100,
                Height = 30,
                Top = 60,
                DialogResult = DialogResult.OK
            };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
        }

        /// <summary>
        /// Диалог выбора файла
        /// </summary>
        public static string OpenFile(IWin32Window owner, string filter, string caption = null)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = caption ?? "Выбор...";
                dialog.Filter = filter;

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }
            return null;
        }

        /// <summary>
        /// Диалог сохранения файла
        /// </summary>
        public static string SaveFile(IWin32Window owner, string filename, string filter = null, string caption = null)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.FileName = filename;
                dialog.Title = caption ?? "Сохранить...";
                if (filter != null)
                {
                    dialog.Filter = filter;
                }

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }
            return null;
        }

        #endregion
    }
}