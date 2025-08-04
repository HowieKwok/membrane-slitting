using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI2;

namespace Mef_VisionFrame
{
    public partial class UC_HWCshow : UserControl
    {
        /// <summary>
        /// 检测结果图像显示窗体对象
        /// </summary>
        private HWndCtrl hWndCtrl_Display_Cam;
        private ROIController roiController_Cam = new ROIController();
        public ROIRectangle2 roiRectangle2_Cam = new ROIRectangle2();
        public UC_HWCshow(HWndCtrl hWndCtrl, ROIController roiController)//有参构造，接收外部传入的hwndctrl，用户控件仅做显示
        {
            InitializeComponent();
            hWndCtrl_Display_Cam = hWndCtrl;
            hWndCtrl_Display_Cam.viewPort = HWC_Cam;
            roiController_Cam = roiController;
        }
        /// <summary>
        /// 延时刷新图标函数
        /// </summary>
        /// <param name="milliSecond">毫秒</param>
        private void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {

                //可执行某无聊的操作
            }
        }

        private void PB_FItWindow_Click(object sender, EventArgs e)
        {
            //按钮点击效果
            this.PB_FItWindow.BackgroundImage = Properties.Resources.收缩;
            Task ImageReset_Active = new Task(() =>
            {
                Delay(100);
                this.PB_FItWindow.BackgroundImage = Properties.Resources.搜索;
            });
            ImageReset_Active.Start();
            RepaintHWc();
        }

        public void RepaintHWc()
        {
            try
            {
                //重置显示画面
                hWndCtrl_Display_Cam.resetAll();
                hWndCtrl_Display_Cam.repaint();
            }
            catch (Exception)
            {
            }
        }

        private void HWC_Cam_HMouseMove(object sender, HalconDotNet.HMouseEventArgs e)
        {
            try
            {
                if (hWndCtrl_Display_Cam.ho_Image_Changed != null & roiController_Cam.activeROIidx == -1)
                    TB_PositionAndGray.Text = hWndCtrl_Display_Cam.Get_Mouseposition_and_Gray(HWC_Cam.HalconID, hWndCtrl_Display_Cam.ho_Image_Changed, e);
            }
            catch (Exception)
            {

            }
        }
    }
}
