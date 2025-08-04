using Frame_Core;
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

namespace Mef_VisionFrame
{
    /// <summary>
    /// 相机图片显示窗体
    /// </summary>
    public partial class CamImageDataForm : Form
    {
        MinImageShowForm minImageShowForm;
        public CamImageDataForm(MinImageShowForm _minImageShowForm)
        {
            InitializeComponent();
            minImageShowForm = _minImageShowForm;
        }

        private void CamImageDataForm_Load(object sender, EventArgs e)
        {
            this.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this, true);
            tableLayoutPanel1.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel1, true);

            //设置双缓存防止窗体闪烁，hSmartWindowControl控件会有图片不显示的问题，hWindowControl控件不会
            hWindowControl_OriginalImage.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(hWindowControl_OriginalImage, true);
            hWindowControl_DempImage.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(hWindowControl_DempImage, true);
            hWindowControl_DefectImage1.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(hWindowControl_DefectImage1, true);
            hWindowControl_DefectImage2.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(hWindowControl_DefectImage2, true);
            hWindowControl_DefectImage3.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(hWindowControl_DefectImage3, true);
        }

        private void CamImageDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //窗体关闭清理非托管内存            
                hWindowControl_OriginalImage.Dispose();
                hWindowControl_DempImage.Dispose();
                hWindowControl_DefectImage1.Dispose();
                hWindowControl_DefectImage2.Dispose();
                hWindowControl_DefectImage3.Dispose();

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 刷新窗体文本
        /// </summary>
        public void UpdateFormText(string camName)
        {
            gb_CamName.Invoke(new Action(() =>
            {
                gb_CamName.Text = camName;
            }));
        }

        int OriginalImageMouseDownCount = 0;
        int DempImageMouseDownCount = 0;
        int DefectImage1MouseDownCount = 0;
        int DefectImage2MouseDownCount = 0;
        int DefectImage3MouseDownCount = 0;
        HObject originalImage = new HObject();
        HObject dempImage = new HObject();
        HObject defectImage1 = new HObject();
        HObject defectImage2 = new HObject();
        HObject defectImage3 = new HObject();    
        /// <summary>
        /// 加载图片显示
        /// </summary>
        /// <param name="camImageData"></param>
        public void LoadImageShow(CamImageData camImageData)
        {
            try
            {
                if (camImageData == null) return;

                if (camImageData.OriginalImage_Path != "" && camImageData.OriginalImage_Path != null)
                {
                    originalImage.Dispose();
                    HOperatorSet.ReadImage(out originalImage, camImageData.OriginalImage_Path);
                    HWindowTool.DispImage(hWindowControl_OriginalImage.HalconWindow, originalImage);
                }


                if (camImageData.DempImage_Path != "" && camImageData.DempImage_Path != null)
                {
                    dempImage.Dispose();
                    HOperatorSet.ReadImage(out dempImage, camImageData.DempImage_Path);
                    HWindowTool.DispImage(hWindowControl_DempImage.HalconWindow, dempImage);
                }


                if (camImageData.DefectImage1_Path != "" && camImageData.DefectImage1_Path != null)
                {
                    defectImage1.Dispose();
                    HOperatorSet.ReadImage(out defectImage1, camImageData.DefectImage1_Path);
                    HWindowTool.DispImage(hWindowControl_DefectImage1.HalconWindow, defectImage1);
                }

                if (camImageData.DefectImage2_Path != "" && camImageData.DefectImage2_Path != null)
                {
                    defectImage2.Dispose();
                    HOperatorSet.ReadImage(out defectImage2, camImageData.DefectImage2_Path);
                    HWindowTool.DispImage(hWindowControl_DefectImage2.HalconWindow, defectImage2);
                }


                if (camImageData.DefectImage3_Path != "" && camImageData.DefectImage3_Path != null)
                {
                    defectImage3.Dispose();
                    HOperatorSet.ReadImage(out defectImage3, camImageData.DefectImage3_Path);
                    HWindowTool.DispImage(hWindowControl_DefectImage3.HalconWindow, defectImage3);
                }


            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// 小图显示
        /// </summary>
        /// <param name="_images"></param>
        private void MinImageShow(HObject image)
        {
            try
            {
                if (minImageShowForm == null) { return; }
                if (image == null) { return; }

                minImageShowForm.Close();
                if (minImageShowForm.IsDisposed == true)
                {
                    minImageShowForm = new MinImageShowForm();
                }
                minImageShowForm.StartPosition = FormStartPosition.CenterScreen;
                minImageShowForm.Show();

                HObject Img = image;
                if (Img != null)
                    minImageShowForm.ImageShow(Img, "");
            }
            catch (Exception)
            {


            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            OriginalImageMouseDownCount = 0;
            DempImageMouseDownCount= 0;
            DefectImage1MouseDownCount= 0;
            DefectImage2MouseDownCount = 0;
            DefectImage3MouseDownCount = 0;
        }


        private void hWindowControl_OriginalImage_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                OriginalImageMouseDownCount++;
                if (OriginalImageMouseDownCount >= 2)
                {                
                    MinImageShow(originalImage);

                }
            }
            catch (Exception)
            {

            }

        }

        private void hWindowControl_DempImage_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                DempImageMouseDownCount++;
                if (DempImageMouseDownCount >= 2)
                {
                    MinImageShow(dempImage);

                }
            }
            catch (Exception)
            {

            }
        }

        private void hWindowControl_DefectImage1_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                DefectImage1MouseDownCount++;
                if (DefectImage1MouseDownCount >= 2)
                {
                    MinImageShow(defectImage1);

                }
            }
            catch (Exception)
            {

            }
        }

        private void hWindowControl_DefectImage2_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                DefectImage2MouseDownCount++;
                if (DefectImage2MouseDownCount >= 2)
                {
                    MinImageShow(defectImage2);

                }
            }
            catch (Exception)
            {

            }
        }

        private void hWindowControl_DefectImage3_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                DefectImage3MouseDownCount++;
                if (DefectImage3MouseDownCount >= 2)
                {
                    MinImageShow(defectImage3);

                }
            }
            catch (Exception)
            {

            }
        }
    }
}
