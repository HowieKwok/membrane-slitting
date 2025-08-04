using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frame_Core;

namespace Mef_VisionFrame
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {          
            if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                System.Windows.Forms.MessageBox.Show("已有另一個程序在运行！", "提示信息");
                Application.Exit();
            }
            else
            {
                //注入并激活HSL
                bool result = HookHSL.HSLAuthorization.SetAuthorizationCode();
                if (!result)
                    MessageBox.Show("通讯模块加载失败", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //加载插件        
                // pluginservice.CreateAllPlugins();
                PluginService.InitializPlugin();
                PluginService.BuildContainer();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form_Main());
            }
         
        }
    }
}
