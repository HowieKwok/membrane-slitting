using AntdUI;
using Frame_Core;
using HalconDotNet;
using LiveChartsCore.Drawing;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using ViewROI2;

namespace Mef_VisionFrame
{
    public partial class FormDataAnalysis : Form, IMessageFilter

    {
        private string path = string.Empty;  //膜卷路径
        private string Stype = string.Empty; //原图或者截图。
        private List<string> List_camName = new List<string>(); //相机名 ；N个相机
        private List<int> List_Frame = new List<int>();         //帧号 ；N个相机帧号 
        private event Action FormCloseCountEvent;


        public DisplayWindowClass HWC1; //主窗体
        /// <summary>
        /// 当前显示的结果数据（包含图像、Region）
        /// </summary>
        ShowHalconWindow currentShowHalconWindow = new ShowHalconWindow();
        private string CurrentPath = string.Empty; //当前主屏幕显示图像路径

        private HWindowControl[] hwinds = null;
        private System.Windows.Forms.GroupBox[] gbs = null;

        private List<string> list_ImagePath = new List<string>();//缓存图像路径
        public FormDataAnalysis(string Path1, List<string> LImagePath, Action formCloseCountEvent = null)
        {
            InitializeComponent();

            path = Path1;
            FormCloseCountEvent = formCloseCountEvent;
            //if (OriginImage)
            //{
            //    Stype = "原图";
            //}
            //else
            //{
            //    Stype = "截图";
            //}
            //List_camName = LCamName;
            //List_Frame = LFrame;
            //screenFrame = ScreenFrame;


            foreach (string imgpath in LImagePath)
            {
                string temppath = path + imgpath;
                list_ImagePath.Add(temppath);
            }

            HWC1 = new DisplayWindowClass(HWC_Display, TB_PositionAndGray, FullScrean, HWC_Display.Name);
            hwinds = new HWindowControl[] { HWC_Image1, HWC_Image2, HWC_Image3, HWC_Image4, HWC_Image5, HWC_Image6, HWC_Image7, HWC_Image8, HWC_ImageScreen };
            gbs = new System.Windows.Forms.GroupBox[] { gb1, gb2, gb3, gb4, gb5, gb6, gb7, gb8, gb9 };

            //开启监听键盘和鼠标操作
            Application.AddMessageFilter(this);
        }

        public string LoadHminImage()
        {
            //加载相机图像
            for (int i = 0; i < list_ImagePath.Count; i++)
            {
                string Imagepath = list_ImagePath[i];
                //加载相机图像
                //list_ImagePath.Add(Imagepath);
                bool flag = string.IsNullOrEmpty(Imagepath);
                if (!flag)
                {
                    HImage hImage = new HImage(Imagepath);
                    DisplayImage(hImage, hwinds[i]);

                    CurrentPath = Imagepath;
                }
            }
            return CurrentPath;
        }

        public string LoadHImage()
        {
            //加载相机图像
            for (int i = 0; i < List_camName.Count; i++)
            {
                string Imagepath = LoadImagePath(path, List_camName[i], Stype, List_Frame[i]);
                //加载相机图像
                list_ImagePath.Add(Imagepath);
                bool flag = string.IsNullOrEmpty(Imagepath);
                if (!flag)
                {
                    HImage hImage = new HImage(Imagepath);
                    DisplayImage(hImage, hwinds[i]);

                    CurrentPath = Imagepath;
                }
            }
            return CurrentPath;
        }

        /// <summary>
        /// 读取相机本地图像
        /// </summary>
        /// <param name="path">膜卷路径</param>
        /// <param name="camName">相机名</param>
        /// <param name="Stype">原图文件夹或截图文件夹</param>
        /// <param name="frame">帧号</param>
        /// <returns></returns>
        public string LoadImagePath(string path, string camName, string Stype, int frame)
        {
            string filePath = string.Empty;
            string tpath = Path.Combine(path, camName, Stype);
            filePath = SearchFile(tpath, frame);
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show($"{camName}：图像【{filePath}】不存在！");
                return string.Empty;
            }
            return filePath;
        }

        private string SearchFile(string path, int frame)
        {
            //根据帧号位数判断
            string Sframe = frame.ToString();
            int length = (int)Math.Log10(frame) + 1;
            if (length <= 5)
            {
                Sframe = frame.ToString("D5");
            }
            //索引对应帧号图像
            string pathOK = Path.Combine(path, "OK");
            string[] files = Directory.GetFiles(pathOK, $"{Sframe}_*.jpeg");
            if (files.Length > 0)
                return files[0];
            string pathNG = Path.Combine(path, "NG");
            files = Directory.GetFiles(pathNG, $"{Sframe}_*.jpeg");
            if (files.Length > 0)
                return files[0];
            return string.Empty;

        }

        private void HWC_Image1_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                HWindowControl HWC = (HWindowControl)sender;
                int Index = Array.IndexOf(gbs, HWC.Parent);
                if (!string.IsNullOrEmpty(list_ImagePath[Index]))
                {
                    HImage hImage = new HImage(list_ImagePath[Index]);
                    CurrentPath = list_ImagePath[Index];

                    // 释放之前的数据              
                    currentShowHalconWindow.Dispose();
                    currentShowHalconWindow.m_hImage = hImage;
                    HWC1.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                    screenshotImage?.Dispose();

                    LB_IMageSize.Text = ShowIMageSize(hImage);
                    LB_ModleName.Text = HWC.Parent.Text;

                    l_link.Text = CurrentPath;
                    foreach (System.Windows.Forms.GroupBox gb in gbs)
                    {
                        if (HWC.Parent.Equals(gb))
                        {
                            gb.BackColor = Color.CadetBlue;
                        }
                        else
                        {
                            gb.BackColor = this.BackColor;
                        }
                    }
                }
            }
            catch (Exception ee)
            {

            }

        }

        private void LoadImage_Click(object sender, EventArgs e)
        {
            try
            {
                string file = l_link.Text;
                if (!string.IsNullOrEmpty(file) && File.Exists(file))
                {
                    System.Collections.Specialized.StringCollection strcoll = new System.Collections.Specialized.StringCollection(); //收集路径
                    strcoll.Add(file);
                    Clipboard.SetFileDropList(strcoll);//将要复制的文件货文件夹路径放入剪切板

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"复制文件出错，错误【{ex.Message}】", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void l_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                OpenExplorer();
        }

        private void FormDataAnalysis_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                FormCloseCountEvent?.Invoke();
                FMain.config.StartPos = this.Location;
                FMain.config.Size = this.Size;
                FMain.config.Maximizd = this.WindowState == FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"关闭窗口出错，错误【{ex.Message}】", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 图像显示：适应窗口，等比显示图像
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hWindowControl">输入Halcon窗口</param>
        /// <returns>显示结果</returns>
        public bool DisplayImage(HObject ho_Image, HWindowControl hWindowControl)
        {
            HTuple hv_StartX, hv_StartY, hv_ZoomFactor;
            HOperatorSet.ClearWindow(hWindowControl.HalconWindow);
            //获取图像大小及纵横比
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            int im_width = int.Parse(hv_Width.ToString());
            int im_height = int.Parse(hv_Height.ToString());
            double im_AspectRatio = (double)(im_width) / (double)(im_height);
            double halconWindowHeight = 0;
            double halconWindowWidth = 0;

            halconWindowHeight = hWindowControl.WindowSize.Height;
            halconWindowWidth = hWindowControl.WindowSize.Width;

            double w_AspectRatio = halconWindowWidth / halconWindowHeight;
            HOperatorSet.SetSystem("int_zooming", "false");//图像缩放之前最好将此参数设置为false.

            HTuple para = new HTuple("constant");
            HObject ho_zoomImage;
            HOperatorSet.GenEmptyObj(out ho_zoomImage);

            ho_zoomImage.Dispose();
            HOperatorSet.ClearWindow(hWindowControl.HalconWindow);

            if (halconWindowWidth < im_width && im_AspectRatio > w_AspectRatio)
            {
                //超宽图像               
                HOperatorSet.ZoomImageSize(ho_Image, out ho_zoomImage, halconWindowWidth, halconWindowWidth / im_AspectRatio, para);
                hv_ZoomFactor = halconWindowWidth / im_width;
                hv_StartX = 0 - (halconWindowHeight - halconWindowWidth / im_AspectRatio) / 2;
                hv_StartY = 0;
            }
            else
            {
                //超高图像                
                HOperatorSet.ZoomImageSize(ho_Image, out ho_zoomImage, halconWindowHeight * im_AspectRatio, halconWindowHeight, para);
                hv_ZoomFactor = halconWindowHeight / im_height;
                hv_StartX = 0;
                hv_StartY = 0 - (halconWindowWidth - halconWindowHeight * im_AspectRatio) / 2;
            }

            try
            {
                HOperatorSet.SetPart(hWindowControl.HalconWindow, hv_StartX, hv_StartY, halconWindowHeight - 1 + hv_StartX,
                    halconWindowWidth - 1 + hv_StartY);//设置居中显示
                HOperatorSet.DispObj(ho_zoomImage, hWindowControl.HalconWindow);
            }
            catch (Exception exc)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 显示图像尺寸
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private string ShowIMageSize(HObject image)
        {
            string str = "Size  R:—   C:—";
            try
            {
                HTuple w, h;
                HOperatorSet.GetImageSize(image, out w, out h);
                str = String.Format("Size  R:{0}   C:{1}", h.ToString(), w.ToString()); //格式化字符串
                return str;
            }
            catch (Exception ee)
            {
                return str;
            }
        }
        /// <summary>
        /// 打开文件路径
        /// </summary>
        private void OpenExplorer()
        {
            try
            {
                string file = l_link.Text;
                if (!string.IsNullOrEmpty(file) && File.Exists(file))
                {
                    System.Diagnostics.Process.Start("Explorer.exe", "/select," + file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开目录并定位文件出错，错误【{ex.Message}】", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region 窗体长时间无操作，自动退出
        IntervalExecution intervalExecution = new IntervalExecution();
        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x0200 || m.Msg == 0x0202 || m.Msg == 0x0202 || m.Msg == 0x0203 || m.Msg == 0x0205 || m.Msg == 0x204 || m.Msg == 0x100)
            {
                if (timer1 != null)
                {
                    timer1.Stop();
                    timer1.Start();
                }
            }
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                if (timer1 != null)
                {
                    timer1.Enabled = false;
                    timer1.Dispose();
                }               
                timer1 = null;

            }
            catch (Exception ex)
            {

            }           
        }
        #endregion
    }
}
