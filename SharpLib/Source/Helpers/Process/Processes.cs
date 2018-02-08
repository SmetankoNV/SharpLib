using System;
using System.Diagnostics;
using SharpLib.Source.Extensions.String;

namespace SharpLib.Source.Helpers.Process
{
    /// <summary>
    /// Класс работы с процессами
    /// </summary>
    public static class Processes
    {
        /// <summary>
        /// Запуск исполняемого файла 
        /// </summary>
        public static System.Diagnostics.Process RunExe(string exeName, string param = null, bool isVisible = false)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.FileName = exeName;
            process.StartInfo.Arguments = param ?? string.Empty;
            process.StartInfo.WindowStyle = isVisible ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden;
            process.Start();

            return process;
        }

        /// <summary>
        /// Запуск CLI файла
        /// </summary>
        public static int RunCli(string exeName, string param, Action<string> output, Action<string> error)
        {
            var processInfo = new ProcessStartInfo(exeName, param)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true
            };

            var process = System.Diagnostics.Process.Start(processInfo);
            if (process == null)
            {
                return -1;
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.OutputDataReceived += (sender, e) =>
            {
                if (output != null && e.Data.IsValid())
                {
                    output(e.Data);
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (error != null && e.Data.IsValid())
                {
                    error(e.Data);
                }
            };

            process.WaitForExit();

            int exitCode = process.ExitCode;

            process.Close();

            return exitCode;
        }
    }
}
