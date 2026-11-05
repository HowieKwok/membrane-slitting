using DevMgmt.Light.Models;
using DevMgmt.Light.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Windows;
using FrameCommunity.DataOperation;

namespace DevMgmt.Light
{
    internal static class Program
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
                //Application.Exit();
            }
            else
            {

                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form1());

                LightControllerParam Params = new LightControllerParam();
                Params = Params.ReadExtension<LightControllerParam>($"{System.Environment.CurrentDirectory}\\Param\\", $"LightAllParam");

                // WPF 标准启动流程
                var app = new Application();
                app.Run(new WpfLightDevWindow(Params));
            }
       
        }
    }
}
