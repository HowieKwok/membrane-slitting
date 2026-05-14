using System;
using System.IO;
using System.Windows;

namespace TemplateMatchBasePointStep1Demo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
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

            base.OnStartup(e);
        }
    }
}
