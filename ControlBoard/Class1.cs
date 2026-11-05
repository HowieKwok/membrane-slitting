using FrameCommunity.DataOperation;
using ControlBoard.Model;
using ControlBoard.View;
using ControlBoard.服务类;
using System.Windows;

namespace ControlBoard
{
    public class Class1
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ServiceCardManage.Instance.strPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Param\\";
            ParamControlCord paramControlCord = new ParamControlCord();
            paramControlCord = paramControlCord.ReadExtension<ParamControlCord>(ServiceCardManage.Instance.strPath, "ControlCordParam");

          
            int ret = ServiceCardManage.Instance.InitServiceCard(paramControlCord);
            if (ret == 0)
            {
                MessageBox.Show("初始化失败");
            }
            MessageBox.Show($"编号{ret}");
            ServiceCardManage.Instance.ShowWindow();
        }
    }

}
