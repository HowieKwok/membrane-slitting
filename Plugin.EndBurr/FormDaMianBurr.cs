using Frame_Core;
using HalconAlgorithm;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI;

namespace Plugin.DaMianBurr
{
    public partial class FormDaMianBurr : FormPluginBase
    {
        private DaMianBurrObj pluginsObj { get; set; }
        public ParamsDaMianBurr PluginParams { get; set; }
        bool bl_RoiExist = false;

        public HObject ho_Image = null;

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();

        public int SelectIndex = 0;
        public FormDaMianBurr()
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
            pluginsObj = (DaMianBurrObj)pluginsObjBase;
            PluginParams = (ParamsDaMianBurr)pluginsObj.PluginParams;
            this.Text = pluginsObjBase.moduleInfo.ModuleName;


            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
            ShowParamsToForm(PluginParams);
            InitListView(listView1);

            if (ho_Image != null)
            {
                if (!pluginsObj.isExistModel)
                {
                    PluginParams.ImageInfo.ho_image = ho_Image;
                }
            }
            try
            {
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ImageInfo.ho_image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
            }
            catch (Exception ee) { }
            //测试存参用
        }
        #region 功能函数
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


      

        /// <summary>
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="Params">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(ParamsDaMianBurr Params, ROIRectangle2 roiRectangle2, ROIController roiController, HWndCtrl hWndCtrl)
        {
            try
            {
            
                if (Params.ImageInfo.ho_image == null)
                {
                    HOperatorSet.GenEmptyObj(out Params.ImageInfo.ho_image);
                }
                if (Params.ImageInfo.ho_image.CountObj() != 0)
                {
                    HTuple hv_Width;
                    HTuple hv_Height;
                    HOperatorSet.GetImageSize(Params.ImageInfo.ho_image, out hv_Width, out hv_Height);
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
                    hWndCtrl.repaint();
                }
            }
            catch { }
        }



        /// <summary>
        /// 加载抓边参数到窗体
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowParamsToForm(ParamsDaMianBurr Params)
        {
            try
            {
               
            }
            catch 
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
            c1.Width = 70;
            c1.Text = "编号";
            lv.Columns.Add(c1);

            lv.Columns.Add("图像ID", 70);  //相当于上面的添加列名的步骤
            lv.Columns.Add("中心行", 80);  //相当于上面的添加列名的步骤
            lv.Columns.Add("中心列", 80);  //相当于上面的添加列名的步骤

            lv.Columns.Add("宽", 80);  //相当于上面的添加列名的步骤
            lv.Columns.Add("高", 80);  //相当于上面的添加列名的步骤
            lv.Columns.Add("面积", 80);  //相当于上面的添加列名的步骤

            //lv.Columns.Add("圆度", 80);  //相当于上面的添加列名的步骤
            //lv.Columns.Add("中间行", 100);  //相当于上面的添加列名的步骤
            //lv.Columns.Add("极耳图", 100);  //相当于上面的添加列名的步骤
            //设置属性
            lv.GridLines = true;  //显示网格线
            lv.FullRowSelect = true;  //显示全行
            lv.MultiSelect = false;  //设置只能单选
            lv.View = View.Details;  //设置显示模式为详细
            lv.HoverSelection = true;  //当鼠标停留数秒后自动选择

        }

        public void ShowResultToListView(ListView lv)
        {
            InitListView(lv);
            lv.BeginUpdate();
            for (int i = 0; i < PluginParams.List_Defect.Count; i++)
            {
                //创建行对象
                ListViewItem li = new ListViewItem((i + 1).ToString());
                //添加同一行的数据
                li.SubItems.Add(PluginParams.List_Defect[i].m_nImgIndex.ToString());
                li.SubItems.Add(PluginParams.List_Defect[i].NgMidRow.ToString("0.0"));
                li.SubItems.Add(PluginParams.List_Defect[i].NgMidColumn.ToString("0.0"));
                li.SubItems.Add(PluginParams.List_Defect[i].m_fNgTypWidth.ToString());
                li.SubItems.Add(PluginParams.List_Defect[i].m_fNgTypHeight.ToString());
                li.SubItems.Add(PluginParams.List_Defect[i].m_fNgTypArea.ToString());
                //li.SubItems.Add(PluginParams.List_Defect[i].circularity.ToString("0.0"));
                //li.SubItems.Add(PluginParams.ProPoleList_Out[i].Row_Mid.ToString());
                //if (PluginParams.ProPoleList_Out[i].Image_JRpart != null)
                //{
                //    li.SubItems.Add("true");
                //}
                //else
                //{
                //    li.SubItems.Add("false");
                //}
                //将行对象绑定在listview对象中
                lv.Items.Add(li);
            }
            lv.EndUpdate();
        }

        /// <summary>
        /// 获取ROI参数并显示
        /// </summary>
        /// <param name="roiRectangle2"></param>
        /// <param name="Params"></param>
        public void GetRoiParams(ROIRectangle2 roiRectangle2, ParamsDaMianBurr Params)
        {
          
        }

        /// <summary>
        /// ROI参数显示刷新
        /// </summary>
        /// <param name="roiRectangle2"></param>
        private void RefreshRoiParams(ROIRectangle2 roiRectangle2)
        {
            HTuple hv_Deg;
           
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Deg);
            

        }

        /// <summary>
        /// ViewRoi回调函数：执行检测方法
        /// </summary>
        private void RunFunction()
        {

            if (roiController.activeROIidx == -1)
            {
                if (roiRectangle2 != null)
                    RefreshRoiParams(roiRectangle2);

                if (roiRectangle2 != null & PluginParams.ImageInfo.ho_image.CountObj() != 0)
                {
                    Delay(10);
                    pluginsObj.ExecuteObj();
                }

                if (TLP_Image1.BackgroundImage != null)
                {
                    if (PluginParams.ImageInfo.ho_image.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ImageInfo.ho_image;
                    }
                }
                else if (TLP_Image2.BackgroundImage != null)
                {
                    if (PluginParams.ho_ImageProcessed.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_ImageProcessed;
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
        /// <param name="Params"></param>
        /// <param name="hv_HWindowID"></param>
        private void ShowDamageToHWD(ParamsDaMianBurr Params, HTuple hv_HWindowID, HTuple hv_size)
        {
           
            if (roiController.activeROIidx == -1)
            {
                //显示缩略图到图形选项中
                if (Params.ImageInfo.ho_image.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(Params.ImageInfo.ho_image, HWC_Image1);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
                }

                if (Params.ho_ImageProcessed.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(Params.ho_ImageProcessed, HWC_Image2);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
                }
                ShowResultToListView(listView1);

                //显示破损检测结果
                if (Params.bl_ModelRunFlag & hWndCtrl_Display.bl_RoiOperateFlag)
                {
                    HOperatorSet.SetColor(hv_HWindowID, "red");
                    HOperatorSet.DispObj(PluginParams.outputDaMianMaoCiRegions, hv_HWindowID);
                    HOperatorSet.SetColor(hv_HWindowID, "orange");
                    HOperatorSet.DispObj(PluginParams.outputDaMianLine, hv_HWindowID);
                    HOperatorSet.SetColor(hv_HWindowID, "green");
                    HOperatorSet.DispObj(PluginParams.outputDaMianMaoCiDetectionRectangle, hv_HWindowID);

                    for (int i=0;i< Params.List_Defect.Count;i++)
                    {
                        if (i == SelectIndex)
                        {
                            //显示裁图
                            if (Params.List_Defect[i].m_hImage.CountObj() != 0)
                            {
                                PaintWindows.DisplayImage(Params.List_Defect[i].m_hImage, HWC_Image3);
                            }
                            else
                            {
                                HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
                            }
                        }
                    }
                    
                }
              
            }
        }

        #endregion

       

    


        #region 控件事件    

        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PluginParams.ImageInfo.ho_image = new HImage(FileDialog.FileName);
                    ho_Image = PluginParams.ImageInfo.ho_image;
                    hWndCtrl_Display.addIconicVar(ho_Image);

                    SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                    linkLabel1.Text = FileDialog.FileName;
                }
                catch (Exception)
                {
                }
            }
        }

        private void ImageReset_Click(object sender, EventArgs e)
        {
            try
            {
                //重置显示画面
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                //
            }
            catch (Exception)
            {
            }
        }

      

  
    

   
 


    

     

        #endregion

        #region 窗体控件重绘，防止闪烁
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;

        #endregion

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
            catch (Exception ee)
            {
                return str;
            }
        }











        private void FormEarBurr_Load(object sender, EventArgs e)
        {

        }

        private void NUD_Pix_X_ValueChanged(object sender, EventArgs e)
        {

        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices != null && listView1.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = listView1.SelectedIndices;
                string str = listView1.Items[c[0]].SubItems[0].Text.ToString();
                SelectIndex = Convert.ToInt32(str) - 1;
                ShowResult();
            }
        }

    }
}
