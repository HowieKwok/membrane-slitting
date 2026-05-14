using System;
using System.IO;
using System.Windows;

namespace TemplateMatchBasePointStep1Demo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            string halconBin = @"E:\halconPath\bin\x64-win64";
            if (Directory.Exists(halconBin))
            {
                string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                if (path.IndexOf(halconBin, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    Environment.SetEnvironmentVariable("PATH", halconBin + ";" + path);
                }
            }

            App app = new App();
            app.Run(new MainWindow());
        }
    }
}
