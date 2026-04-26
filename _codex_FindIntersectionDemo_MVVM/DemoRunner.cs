using System;
using System.IO;
using System.Windows;

namespace Plugin.FindIntersection
{
    public static class DemoRunner
    {
        [STAThread]
        public static void Main()
        {
            string halconDir = @"D:\BaiduNetdiskDownload\宽幅打孔新方案1107\宽幅打孔新方案1017\ProcessWideHoleLaser\Mef_VisionFrame\bin\Release";
            if (Directory.Exists(halconDir))
            {
                string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                if (path.IndexOf(halconDir, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    Environment.SetEnvironmentVariable("PATH", halconDir + ";" + path);
                }
            }

            var app = new Application();
            app.Run(new FormFindIntersectionDemo());
        }
    }
}
