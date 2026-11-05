using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frame_Core;
using HalconDotNet;
using ViewROI;
using System.IO;
using HalconAlgorithm;
using FrameCommunity.Data;

namespace Plugin.DetectEar
{
    public partial class FormDetectEar : FormPluginBase
    {
        public FormDetectEar()
        {
            InitializeComponent();
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果
        }
        public override void Init()
        {
            pluginsObj = (DetectEarObj)pluginsObjBase;
            PluginParams = (ParamsDetectEar)pluginsObj.PluginParams;
            PluginParams.backup(PluginParams); //备份参数，用于修改记录对比

            proc = Project.Instance.GetProcessByName(pluginsObj.moduleInfo.ProcessName);
            this.Text = pluginsObjBase.moduleInfo.ModuleName;

            LoadStandImage();
            INI_Params();
            InitListView(listView1);
            ShowROI_SearchJR(PluginParams.ImageInfo.ho_image, roiRectangle1, PluginParams.Column1, PluginParams.Column2);

            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_SmallRectangle2);
            //测试存参用
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Btn_Save_Click(object sender, EventArgs e)
        {
            //新的实现逻辑
            //string error = "";
            //视觉设置
            PluginParams.FilePath = (Application.StartupPath + "\\record\\参数修改记录\\视觉设置\\");
            // error = YControlAutobind.Saveing<PluginParamsBase, IYCustomAttribute>(this, PluginParams);
            // 调用父窗体
            base.Btn_Save_Click(sender, e);
            PluginParams.saveChangeLog(PluginParams, String.Format("测试视觉设置:{0} 中 {1}", pluginsObj.moduleInfo.ProcessName, pluginsObj.moduleInfo.ModuleName));
            PluginParams.backup(PluginParams);//备份参数，用于修改记录对比
            // if (error != "") System.Windows.Forms.MessageBox.Show(error);

        }

        private DetectEarObj pluginsObj { get; set; }
        private Process proc { get; set; }
        public ParamsDetectEar PluginParams { get; set; }

        public HObject ho_Image1 = null;
        public HWndCtrl hWndCtrl_Display;

        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle1 = new ROIRectangle2();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();
        bool bl_RoiExist = false;

        public HObject ho_Rectangle2;
        public HObject ho_SmallRectangle2;
        int Image_Id = 0;

        public int CurrentImageID = 0;
        public int CurrentImageID_Simulate = 0;
        public string[] simulateImagePath = null; //相机仿真图路径数组
        public List<ImageInfo> listImageInfo_Simulate = new List<ImageInfo>(); //相机图像队列_仿真运行

        private void INI_Params()
        {
            try
            {
                num_JRSizeX.Value = (decimal)PluginParams.JRSizeX;
                num_JRSizeY.Value = (decimal)PluginParams.JRSizeY;
                num_JRIntervalY.Value = (decimal)PluginParams.JRInterval;

                num_Column1.Value = (decimal)PluginParams.Column1;
                num_Column2.Value = (decimal)PluginParams.Column2;

                num_threshold_Min.Value = (decimal)PluginParams.threshold_Min;
                num_threshold_Max.Value = (decimal)PluginParams.threshold_Max;

                num_JRSizeY_PercentMin.Value = (decimal)PluginParams.JRSizeY_PercentMin;
                num_JRSizeY_PercentMax.Value = (decimal)PluginParams.JRSizeY_PercentMax;
                num_JRSizeX_PercentMin.Value = (decimal)PluginParams.JRSizeX_PercentMin;
                num_JRSizeX_PercentMax.Value = (decimal)PluginParams.JRSizeX_PercentMax;
            }
            catch (Exception)
            {


            }

        }
        /// <summary>
        /// 初始化ListView的方法
        /// </summary>
        /// <param name="lv"></param>
        public void InitListView(ListView lv)
        {
            lv.Clear();
            //添加列名
            ColumnHeader c1 = new ColumnHeader();
            c1.Width = 100;
            c1.Text = "极耳编号";
            ColumnHeader c2 = new ColumnHeader();
            c2.Width = 100;
            c2.Text = "相机名";
            ColumnHeader c3 = new ColumnHeader();
            c3.Width = 100;
            c3.Text = "左侧极耳";
            //把列名添加到listview中
            lv.Columns.Add(c1);
            lv.Columns.Add(c2);
            lv.Columns.Add(c3);
            lv.Columns.Add("起始图像ID", 100);  //相当于上面的添加列名的步骤
            lv.Columns.Add("起始行", 100);  //相当于上面的添加列名的步骤
            lv.Columns.Add("终止图像ID", 100);  //相当于上面的添加列名的步骤
            lv.Columns.Add("终止行", 100);  //相当于上面的添加列名的步骤
            lv.Columns.Add("中间图像ID", 100);  //相当于上面的添加列名的步骤
            lv.Columns.Add("中间行", 100);  //相当于上面的添加列名的步骤
            lv.Columns.Add("极耳图", 100);  //相当于上面的添加列名的步骤
            //设置属性
            lv.GridLines = true;  //显示网格线
            lv.FullRowSelect = true;  //显示全行
            lv.MultiSelect = false;  //设置只能单选
            lv.View = View.Details;  //设置显示模式为详细
            lv.HoverSelection = true;  //当鼠标停留数秒后自动选择

        }

        private void LoadStandImage()
        {
            try
            {
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ImageInfo.ho_image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
            }
            catch { }
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
        }

        /// <summary>
        /// Hobject转为Himage
        /// </summary>
        /// <param name="hobject"></param>
        /// <param name="image"></param>
        private void HobjectToHimage(HObject hobject, ref HImage image)
        {
            HTuple pointer, type, width, height;
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
        }

        private void ShowROI_SearchJR(HObject image, ROIRectangle2 roiRectangle2, double Col1, double Col2)
        {
            if (image == null) return;
            if (image.CountObj() == 0) return;
            if (Col1 >= Col2) return;
            HTuple hv_Phi;
            HOperatorSet.TupleRad(0, out hv_Phi);
            HTuple hv_Width, hv_Height;
            HOperatorSet.GetImageSize(image, out hv_Width, out hv_Height);

            roiRectangle2.PointNum = 1;
            roiRectangle2.ROIName = "找极耳区域";

            roiRectangle2.midC = (Col1 + Col2) / 2;
            roiRectangle2.midR = hv_Height / 2;
            roiRectangle2.phi = hv_Phi.D;
            roiRectangle2.length2 = hv_Height / 2;
            roiRectangle2.length1 = (Col2 - Col1) / 2;

            if (hv_Width < 1000)
            {
                roiController.size = roiRectangle2.size = 1;
            }
            else
            {
                roiController.size = roiRectangle2.size = hv_Width / 1000 + 1;
            }
            if (bl_RoiExist == false)
            {
                bl_RoiExist = true;
                roiController.setROIShape(roiRectangle2);
                roiController.roiMode.createROI(roiRectangle2.midC, roiRectangle2.midR);
                roiController.ROIList.Add(roiController.roiMode);
            }
            roiController.activeROIidx = -1;
            roiRectangle2.updateHandlePos();
            hWndCtrl_Display.repaint();
        }

        private void ShowMiniImage(HObject himage1, HObject himage2)
        {
            ////显示缩略图到图像选项
            if (himage1 == null) return;
            if (himage1.CountObj() != 0)
            {
                PaintWindows.DisplayImage(himage1, HWC_Image1);
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
            }
            if (himage2 == null) return;
            if (himage2.CountObj() != 0)
            {
                PaintWindows.DisplayImage(himage2, HWC_Image2);
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
            }
            //if (himage3 == null) return;
            //if (himage3.CountObj() != 0)
            //{
            //    PaintWindows.DisplayImage(himage3, HWC_Image3);
            //    HTuple hv_width, hv_heigth;
            //    HObject RegionLines = new HObject();
            //    HOperatorSet.GetImageSize(himage3, out hv_width, out hv_heigth);
            //    HOperatorSet.GenRegionLine(out RegionLines, hv_heigth / 2, 1, hv_heigth / 2, hv_width);
            //    HOperatorSet.DispLine(HWC_Image3.HalconWindow, hv_heigth / 2, 1, hv_heigth / 2, hv_width);
            //    // PaintWindows.DisplayXLD(RegionLines, himage3, HWC_Image3);
            //}
            //else
            //{
            //    HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
            //}
        }

        /// <summary>
        /// ViewRoi回调函数：执行检测方法
        /// </summary>
        private void RunFunction()
        {
            if (roiController.activeROIidx == -1)
            {
                //if (roiRectangle2 != null)
                // RefreshRoiParams(roiRectangle2);
                if (PluginParams.ImageInfo.ho_image!=null)
                {
                    if (roiRectangle2 != null & PluginParams.ImageInfo.ho_image.CountObj() != 0)
                    {
                        pluginsObj.ExecuteObj();
                    }
                }
            }
        }

        /// <summary>
        /// ViewRoi回调函数：显示检测结果
        /// </summary>
        private void ShowResult()
        {
            ShowDamageToHWD(PluginParams, HWC_Display.HalconWindow, roiRectangle2.size);
        }
        /// <summary>
        /// 显示直线到窗体上
        /// </summary>
        /// <param name="DetectEarParams"></param>
        /// <param name="hv_HWindowID"></param>
        private void ShowDamageToHWD(ParamsDetectEar DetectEarParams, HTuple hv_HWindowID, HTuple hv_size)
        {
            if (roiController.activeROIidx == -1)
            {
                HObject ho_Rectangle1, ho_Point;

                HOperatorSet.GenEmptyObj(out ho_Rectangle1);
                HOperatorSet.GenEmptyObj(out ho_Point);

                //显示缩略图到图形选项中
                if (DetectEarParams.ImageInfo.ho_image.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(DetectEarParams.ImageInfo.ho_image, HWC_Image1);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
                }
                // 刷新结果表格
                ShowResultToListView();

                //显示极耳检测结果
                if (DetectEarParams.bl_ModelRunFlag & hWndCtrl_Display.bl_RoiOperateFlag)
                {
                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.SetColor(hv_HWindowID, "green");
                    for (int i = 0; i < DetectEarParams.JRcount_Matched; i++)
                    {
                        HOperatorSet.GenRectangle1(out ho_Rectangle1, DetectEarParams.ProPoleList_Out[i].Row_Begin, DetectEarParams.Column1, DetectEarParams.ProPoleList_Out[i].Row_End, DetectEarParams.Column2);
                        HOperatorSet.DispObj(ho_Rectangle1, HWC_Display.HalconWindow);
                    }
                   

                    //显示找到的缺陷
                    HOperatorSet.SetDraw(hv_HWindowID, "fill");
                    HOperatorSet.SetColor(hv_HWindowID, "orange");
                    if (DetectEarParams.JRcount_Matched > 0)
                    {
                        HOperatorSet.DispObj(DetectEarParams.JR_Region, HWC_Display.HalconWindow);
                        //PaintWindows.DisplayRegion(DetectEarParams.JR_Region, DetectEarParams.ho_Image, HWC_Image4);
                    }


                }

                ho_Rectangle1.Dispose();
                ho_Point.Dispose();
            }
        }


        #region 控件事件

        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.InitialDirectory = "C:\\";
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PluginParams.ImageInfo.ho_image = new HImage(FileDialog.FileName);
                    ho_Image1 = PluginParams.ImageInfo.ho_image;
                    hWndCtrl_Display.addIconicVar(PluginParams.ImageInfo.ho_image);

                    ShowROI_SearchJR(PluginParams.ImageInfo.ho_image, roiRectangle1, PluginParams.Column1, PluginParams.Column2);
                    //SetRoiParams(PluginParams, roiRectangle2, roiController);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                }
                catch (Exception)
                {
                }
            }
        }

        private void num_threshold_Min_ValueChanged(object sender, EventArgs e)
        {
            if (num_threshold_Min.Value >= num_threshold_Max.Value)
            {
                num_threshold_Min.Value = 0;
            }
            PluginParams.threshold_Min = Convert.ToDouble(num_threshold_Min.Value);
        }

        private void num_threshold_Max_ValueChanged(object sender, EventArgs e)
        {
            if (num_threshold_Min.Value >= num_threshold_Max.Value)
            {
                num_threshold_Max.Value = 255;
            }
            PluginParams.threshold_Max = Convert.ToDouble(num_threshold_Max.Value);
        }

        private void num_JRSizeX_PercentMin_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.JRSizeX_PercentMin = Convert.ToDouble(num_JRSizeX_PercentMin.Value);
        }

        private void num_JRSizeX_PercentMax_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.JRSizeX_PercentMax = Convert.ToDouble(num_JRSizeX_PercentMax.Value);
        }

        private void num_JRSizeY_PercentMin_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.JRSizeY_PercentMin = Convert.ToDouble(num_JRSizeY_PercentMin.Value);
        }

        private void num_JRSizeY_PercentMax_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.JRSizeY_PercentMax = Convert.ToDouble(num_JRSizeY_PercentMax.Value);
            //PluginParams.JRSizeY_PercentMax = (double)num_JRSizeY_PercentMax.Value;
        }

        #endregion

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int index = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text.ToString());
                HObject JRimage = PluginParams.ProPoleList_Out[index].Image_JRpart;
                if (JRimage != null)
                {
                    HImage Himage = new HImage(); ;
                    HobjectToHimage(JRimage, ref Himage);
                    hWndCtrl_Display.addIconicVar(Himage);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                }
            }
        }

        public void ShowResultToListView()
        {
            InitListView(listView1);
            listView1.BeginUpdate();
            for (int i = 0; i < PluginParams.ProPoleList_Out.Count; i++)
            {
                //创建行对象
                ListViewItem li = new ListViewItem(PluginParams.ProPoleList_Out[i].JR_ID.ToString());
                //添加同一行的数据
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].cameraName.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].IsLeft.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].image_ID_Begin.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].Row_Begin.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].image_ID_End.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].Row_End.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].image_ID_Mid.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].Row_Mid.ToString());
                if (PluginParams.ProPoleList_Out[i].Image_JRpart != null)
                {
                    li.SubItems.Add("true");
                }
                else
                {
                    li.SubItems.Add("false");
                }
                //将行对象绑定在listview对象中
                listView1.Items.Add(li);
            }
            listView1.EndUpdate();
        }
        private void btn_autoRun_Click(object sender, EventArgs e)
        {
            ShowROI_SearchJR(PluginParams.ImageInfo.ho_image, roiRectangle1, PluginParams.Column1, PluginParams.Column2);
            InitListView(listView1);
            listView1.BeginUpdate();
            for (int i = 0; i < PluginParams.ProPoleList_Out.Count; i++)
            {
                //创建行对象
                ListViewItem li = new ListViewItem(PluginParams.ProPoleList_Out[i].JR_ID.ToString());
                //添加同一行的数据
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].cameraName.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].IsLeft.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].image_ID_Begin.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].Row_Begin.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].image_ID_End.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].Row_End.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].image_ID_Mid.ToString());
                li.SubItems.Add(PluginParams.ProPoleList_Out[i].Row_Mid.ToString());
                if (PluginParams.ProPoleList_Out[i].Image_JRpart != null)
                {
                    li.SubItems.Add("true");
                }
                else
                {
                    li.SubItems.Add("false");
                }
                //将行对象绑定在listview对象中
                listView1.Items.Add(li);
            }
            listView1.EndUpdate();
            try
            {
                ShowMiniImage(PluginParams.ImageInfo.ho_image, PluginParams.ImageInfo.ho_image);
            }
            catch (Exception)
            {


            }
        }


        private void FullScrean_Click(object sender, EventArgs e)
        {
            //重置显示画面
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
        }

        private void HWC_Display_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (hWndCtrl_Display.ho_Image_Changed != null & roiController.activeROIidx == -1)
                try
                {
                    TB_PositionAndGray.Text = hWndCtrl_Display.Get_Mouseposition_and_Gray(HWC_Display.HalconWindow, hWndCtrl_Display.ho_Image_Changed, e);
                    LB_IMageSize.Text = ShowIMageSize(hWndCtrl_Display.ho_Image_Changed);
                }
                catch (Exception)
                {

                }
        }
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
            catch (Exception)
            {
                return str;
            }
        }


        private void num_Column1_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.Column1 = Convert.ToDouble(num_Column1.Value);
            ShowROI_SearchJR(PluginParams.ImageInfo.ho_image, roiRectangle1, PluginParams.Column1, PluginParams.Column2);
        }
        private void num_Column2_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.Column2 = Convert.ToDouble(num_Column2.Value);
            ShowROI_SearchJR(PluginParams.ImageInfo.ho_image, roiRectangle1, PluginParams.Column1, PluginParams.Column2);
        }

        private void num_JRSizeX_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.JRSizeX = Convert.ToDouble(num_JRSizeX.Value);
        }

        private void num_JRSizeY_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.JRSizeY = Convert.ToDouble(num_JRSizeY.Value);
        }
        private void num_JRIntervalY_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.JRInterval = Convert.ToDouble(num_JRIntervalY.Value);
        }
    }
}
