using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI2;
using Frame_Core;

namespace Mef_VisionFrame
{
    /// <summary>
    /// 小图显示窗体
    /// </summary>
    public partial class MinImageShowForm : Form
    {

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();
        public MinImageShowForm()
        {
            InitializeComponent();

            hWndCtrl_Display = new HWndCtrl(hWindowControl1);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);

            //hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行直线测量方法
            //hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘直线测量结果
        }
        private void MinImageShowForm_Load(object sender, EventArgs e)
        {

        }

        private void MinImageShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //窗体关闭清理非托管内存
                hWndCtrl_Display.ho_Image.Dispose();
                hWndCtrl_Display.ho_Image_Changed.Dispose();
                hWindowControl1.Dispose();

            }
            catch (Exception)
            {

            }

        }
        /// <summary>
        /// Hobject转为Himage
        /// </summary>
        /// <param name="hobject"></param>
        /// <param name="image"></param>
        private void HobjectToHimage(HObject hobject, ref HImage image)
        {
            HTuple pointerR, pointerG, pointerB, type, width, height;
            HOperatorSet.GetImagePointer3(hobject, out pointerR, out pointerG, out pointerB, out type, out width, out height);
            image.GenImage3(type, width, height, pointerR, pointerG, pointerB);
        }

        /// <summary>
        /// 图像显示
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageInfo"></param>
        public void ImageShow(HObject image, string imageInfo)
        {
            try
            {
                this.Text = imageInfo;
                HImage img = new HImage();
                HobjectToHimage(image, ref img);
                hWndCtrl_Display.addIconicVar(img);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                //HOperatorSet.DispObj(image, hWindowControl1.HalconWindow);
            }
            catch (Exception)
            {


            }

        }


    }
}
