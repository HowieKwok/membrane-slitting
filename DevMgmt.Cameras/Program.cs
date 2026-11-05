using DevMgmt.Cameras;
using DevMgmt.Cameras.Models;
using DevMgmt.Cameras.View;
using MvFGCtrlC.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FrameCommunity.DataOperation;
//using System.Windows.Forms;

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
                //Application.Exit();
            }
            else
            {

                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                //Application.Run(new Form_Main());

                CamParam Params = new CamParam();        
                Params = Params.ReadExtension<CamParam>($"{System.Environment.CurrentDirectory}\\Param\\", $"AllCamParam");
                
                // WPF 标准启动流程
                var app = new Application();
                app.Run(new WpfCameraDevWindow(Params));
            }

        }
    }
}
