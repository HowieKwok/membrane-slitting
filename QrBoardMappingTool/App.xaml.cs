using System;
using System.IO;
using System.Text;
using System.Windows;

namespace QrBoardMappingTool
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigureHalconRuntime();
            ConfigureExceptionLogging();
            base.OnStartup(e);
        }

        private static void ConfigureExceptionLogging()
        {
            System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);
            System.Windows.Forms.Application.ThreadException += (s, e) =>
            {
                WriteExceptionLog("WinForms.ThreadException", e.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                WriteExceptionLog("AppDomain.UnhandledException", e.ExceptionObject as Exception);
            };

            Current.DispatcherUnhandledException += (s, e) =>
            {
                WriteExceptionLog("WPF.DispatcherUnhandledException", e.Exception);
                MessageBox.Show(
                    "程序发生异常，已写入日志：\n" + GetLogPath() + "\n\n" + e.Exception.Message,
                    "QR Board Mapping",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                e.Handled = true;
            };
        }

        private static void WriteExceptionLog(string source, Exception ex)
        {
            try
            {
                string logPath = GetLogPath();
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                var builder = new StringBuilder();
                builder.AppendLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + source);
                builder.AppendLine(ex == null ? "<null exception>" : ex.ToString());
                builder.AppendLine();
                File.AppendAllText(logPath, builder.ToString(), Encoding.UTF8);
            }
            catch
            {
            }
        }

        private static string GetLogPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "QrBoardMappingTool_error.log");
        }

        private static void ConfigureHalconRuntime()
        {
            string root = Environment.GetEnvironmentVariable("HALCONROOT");
            if (string.IsNullOrWhiteSpace(root))
            {
                string defaultRoot = @"E:\halconPath";
                if (Directory.Exists(defaultRoot))
                {
                    Environment.SetEnvironmentVariable("HALCONROOT", defaultRoot);
                    root = defaultRoot;
                }
            }

            Environment.SetEnvironmentVariable("HALCONARCH", "x64-win64");

            if (!string.IsNullOrWhiteSpace(root))
            {
                string bin = Path.Combine(root, "bin", "x64-win64");
                if (Directory.Exists(bin))
                {
                    string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                    if (path.IndexOf(bin, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        Environment.SetEnvironmentVariable("PATH", bin + ";" + path);
                    }
                }
            }
        }
    }
}
