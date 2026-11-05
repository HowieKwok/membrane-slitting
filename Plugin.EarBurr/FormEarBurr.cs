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

namespace Plugin.EarBurr
{
    public partial class FormEarBurr : FormPluginBase
    {
        private EarBurrObj pluginsObj { get; set; }
        public ParamsEarBurr PluginParams { get; set; }
        bool bl_RoiExist = false;

        public HObject ho_Image = null;

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();

        public int SelectIndex = 0;
        public FormEarBurr()
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
            pluginsObj = (EarBurrObj)pluginsObjBase;
            PluginParams = (ParamsEarBurr)pluginsObj.PluginParams;
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
            catch (Exception) { }
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
        /// 加载抓边参数到窗体
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowParamsToForm(ParamsEarBurr Params)
        {
            try
            {
                NUD_RoiNum.Value = Convert.ToDecimal(Params.hv_RoiPointsNum);
                NUD_RoiWidth.Value = Convert.ToDecimal(Params.hv_RoiWidth);
                NUD_RoiHeight.Value = Convert.ToDecimal(Params.hv_RoiHeight);
                NUD_RoiAngle.Value = Convert.ToDecimal(Params.hv_RoiAngle);
                NUD_RoiMidRow.Value = Convert.ToDecimal(Params.hv_RoiMidRow);
                NUD_RoiMidColumn.Value = Convert.ToDecimal(Params.hv_RoiMidColumn);

                //极耳毛刺
                NUD_RminArea.Value = Convert.ToDecimal(Params.hv_RminArea);
                NUD_RminWidth.Value = Convert.ToDecimal(Params.hv_RminWidth);
                NUD_RminHeigth.Value = Convert.ToDecimal(Params.hv_RminHeigth);

                NUD_HYminArea.Value = Convert.ToDecimal(Params.hv_HyminArea);
                NUD_HYminWidth.Value = Convert.ToDecimal(Params.hv_HyminWidth);
                NUD_HYminHeigth.Value = Convert.ToDecimal(Params.hv_HyminHeigth);

                NUD_HminArea.Value = Convert.ToDecimal(Params.hv_HminArea);
                NUD_HminWidth.Value = Convert.ToDecimal(Params.hv_HminWidth);
                NUD_HminHeigth.Value = Convert.ToDecimal(Params.hv_HminHeigth);

                //  是否是半极耳图，只检测极耳底部R角
                CB_IsHalfEar.Checked = Params.Bl_IsHalfEar;
                

                NUD_Pix_X.Value = Convert.ToDecimal(Params.Pix_X);
                NUD_Pix_Y.Value = Convert.ToDecimal(Params.Pix_X);

                label_RminArea.Text = (PluginParams.hv_RminArea / PluginParams.Pix_X / PluginParams.Pix_Y).ToString("0.0");
                label_RminWidth.Text = (PluginParams.hv_RminWidth / PluginParams.Pix_X).ToString("0.0");
                label_RminHeigth.Text = (PluginParams.hv_RminHeigth / PluginParams.Pix_Y).ToString("0.0");

                label_HYminArea.Text = (PluginParams.hv_HyminArea / PluginParams.Pix_X / PluginParams.Pix_Y).ToString("0.0");
                label_HYminWidth.Text = (PluginParams.hv_HyminWidth / PluginParams.Pix_X).ToString("0.0");
                label_HYminHeigth.Text = (PluginParams.hv_HyminHeigth / PluginParams.Pix_Y).ToString("0.0");

                label_HminArea.Text = (PluginParams.hv_HminArea / PluginParams.Pix_X / PluginParams.Pix_Y).ToString("0.0");
                label_HminWidth.Text = (PluginParams.hv_HminWidth / PluginParams.Pix_X).ToString("0.0");
                label_HminHeigth.Text = (PluginParams.hv_HminHeigth / PluginParams.Pix_Y).ToString("0.0");

            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="Params">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(ParamsEarBurr Params, ROIRectangle2 roiRectangle2, ROIController roiController, HWndCtrl hWndCtrl)
        {
            try
            {
                roiRectangle2.PointNum = Params.hv_RoiPointsNum;
                roiRectangle2.ROIName = "";

                roiRectangle2.midR = Params.hv_RoiMidRow;
                roiRectangle2.midC = Params.hv_RoiMidColumn;
                HTuple hv_Phi;
                HOperatorSet.TupleRad(Params.hv_RoiAngle, out hv_Phi);
                roiRectangle2.phi = hv_Phi.D;
                roiRectangle2.length2 = Params.hv_RoiWidth / 2;
                roiRectangle2.length1 = Params.hv_RoiHeight / 2;
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
        /// 获取ROI参数并显示
        /// </summary>
        /// <param name="roiRectangle2"></param>
        /// <param name="Params"></param>
        public void GetRoiParams(ROIRectangle2 roiRectangle2, ParamsEarBurr Params)
        {
            Params.hv_RoiPointsNum = roiRectangle2.PointNum;
            roiRectangle2.ROIName = "";

            Params.hv_RoiMidRow = roiRectangle2.midR;
            Params.hv_RoiMidColumn = roiRectangle2.midC;
            HTuple hv_Phi;
            HTuple hv_Angle;
            HOperatorSet.TupleRad(Params.hv_RoiAngle, out hv_Phi);
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Angle);
            Params.hv_RoiAngle = hv_Angle;
            Params.hv_RoiWidth = roiRectangle2.length2 * 2;
            Params.hv_RoiHeight = roiRectangle2.length1 * 2;
        }

        /// <summary>
        /// ROI参数显示刷新
        /// </summary>
        /// <param name="roiRectangle2"></param>
        private void RefreshRoiParams(ROIRectangle2 roiRectangle2)
        {
            HTuple hv_Deg;
            NUD_RoiWidth.Value = Convert.ToDecimal(roiRectangle2.length2 * 2);
            NUD_RoiHeight.Value = Convert.ToDecimal(roiRectangle2.length1 * 2);
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Deg);
            NUD_RoiAngle.Value = Convert.ToDecimal(hv_Deg.D);
            NUD_RoiMidRow.Value = Convert.ToDecimal(roiRectangle2.midR);
            NUD_RoiMidColumn.Value = Convert.ToDecimal(roiRectangle2.midC);

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
                //else if (TLP_Image3.BackgroundImage != null)
                //{
                //    if (PluginParams.ho_ImageSharpen.CountObj() != 0)
                //    {
                //        hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_ImageSharpen;
                //    }
                //}

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
        private void ShowDamageToHWD(ParamsEarBurr Params, HTuple hv_HWindowID, HTuple hv_size)
        {
            HObject ho_Rectangle2, ho_Point;

            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_Point);
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


                //if (Params.ho_ImageSharpen.CountObj() != 0)
                //{
                //    PaintWindows.DisplayImage(Params.ho_ImageSharpen, HWC_Image3);
                //}
                //else
                //{
                //    HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
                //}

                // 刷新结果表格
                ShowResultToListView(listView1);

                //显示破损检测结果
                if (Params.bl_ModelRunFlag & hWndCtrl_Display.bl_RoiOperateFlag)
                {
                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.SetDraw(hv_HWindowID, "margin");
                    //显示屏蔽区
                    HOperatorSet.SetColor(hv_HWindowID, "blue");
                    HOperatorSet.DispObj(Params.ho_ShieldRegion, HWC_Display.HalconWindow);
                    PaintWindows.DisplayRegion(Params.ho_ShieldRegion, Params.ho_ImageProcessed, HWC_Image2);

                    //显示找到的缺陷

                    for (int i = 0; i < Params.DefectCount; i++)
                    {
                        HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                        HOperatorSet.SetColor(hv_HWindowID, "red");
                        if (i == SelectIndex)
                        {

                            HOperatorSet.SetColor(hv_HWindowID, "orange");
                        }
                        HOperatorSet.SetDraw(hv_HWindowID, "fill");
                        HOperatorSet.DispObj(Params.List_Defect[i].m_hRegion, HWC_Display.HalconWindow);

                        HTuple row1, row2;
                        HTuple col1, col2;
                        row1 = Params.List_Defect[i].NgMidRow - Params.List_Defect[i].m_fNgTypHeight / 2 - 50;
                        row2 = Params.List_Defect[i].NgMidRow + Params.List_Defect[i].m_fNgTypHeight / 2 + 50;

                        col1 = Params.List_Defect[i].NgMidColumn - Params.List_Defect[i].m_fNgTypWidth / 2 - 50;
                        col2 = Params.List_Defect[i].NgMidColumn + Params.List_Defect[i].m_fNgTypWidth / 2 + 50;
                        HOperatorSet.GenRectangle1(out HObject rect, row1, col1, row2, col2);

                        HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                        HOperatorSet.SetColor(hv_HWindowID, "blue");
                        if (i == SelectIndex)
                        {
                            HOperatorSet.SetLineWidth(hv_HWindowID, 3);
                            HOperatorSet.SetColor(hv_HWindowID, "green");
                        }
                        HOperatorSet.SetDraw(hv_HWindowID, "margin");
                        HOperatorSet.DispObj(rect, HWC_Display.HalconWindow);
                        rect.Dispose();
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
                HTuple hv_Phi;
                //生成检测ROI
                HOperatorSet.TupleRad(Params.hv_RoiAngle, out hv_Phi);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, Params.hv_RoiMidRow, Params.hv_RoiMidColumn,
                                            -hv_Phi, Params.hv_RoiHeight / 2, Params.hv_RoiWidth / 2);
                //显示Roi框
                if (!hWndCtrl_Display.bl_RoiOperateFlag)
                {
                    HOperatorSet.SetColor(hv_HWindowID, "green");
                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.SetDraw(hv_HWindowID, "margin");
                    HOperatorSet.DispObj(ho_Rectangle2, hv_HWindowID);
                }

            }



            //if (Params.str_RoiMoveIndex == "StandardROI" & hWndCtrl_Display.bl_RoiOperateFlag)
            //{
            //    HOperatorSet.GenCrossContourXld(out ho_Point, Params.hv_RoiStandardRow, Params.hv_RoiStandardColumn, 50 * hv_size, 0);
            //    HOperatorSet.SetColor(hv_HWindowID, "red");
            //    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
            //    HOperatorSet.SetDraw(hv_HWindowID, "margin");
            //    HOperatorSet.DispObj(ho_Point, hv_HWindowID);
            //}


            //HOperatorSet.SetColor(HWC_Image4.HalconWindow, "blue");
            //HOperatorSet.SetLineWidth(HWC_Image4.HalconWindow, 2);
            //HOperatorSet.SetDraw(HWC_Image4.HalconWindow, "margin");
            //PaintWindows.DisplayRegion(ho_Rectangle2, Params.ho_Image, HWC_Image4);

            ho_Rectangle2.Dispose();
            ho_Point.Dispose();

        }

        #endregion

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

            lv.Columns.Add("圆度", 80);  //相当于上面的添加列名的步骤
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
                li.SubItems.Add(PluginParams.List_Defect[i].m_fNgTypWidth.ToString("0.0"));
                li.SubItems.Add(PluginParams.List_Defect[i].m_fNgTypHeight.ToString("0.0"));
                li.SubItems.Add(PluginParams.List_Defect[i].m_fNgTypArea.ToString("0.0"));
                li.SubItems.Add(PluginParams.List_Defect[i].circularity.ToString("0.0"));
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

        //ROi参数
        private void NUD_RoiNum_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiPointsNum = Convert.ToInt32(NUD_RoiNum.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiWidth_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiWidth = Convert.ToInt32(NUD_RoiWidth.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiHeight_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiHeight = Convert.ToInt32(NUD_RoiHeight.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiAngle_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiAngle = Convert.ToInt32(NUD_RoiAngle.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiMidRow_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiMidRow = Convert.ToInt32(NUD_RoiMidRow.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiMidColumn_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiMidColumn = Convert.ToInt32(NUD_RoiMidColumn.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        //缺陷阈值参数
        //极耳毛刺R
        private void NUD_RminArea_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_RminArea = Convert.ToDouble(NUD_RminArea.Value);
            label_RminArea.Text = (PluginParams.hv_RminArea / PluginParams.Pix_X / PluginParams.Pix_Y).ToString("0.0");
            hWndCtrl_Display.repaint();
        }

        private void NUD_RminWidth_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_RminWidth = Convert.ToDouble(NUD_RminWidth.Value);
            label_RminWidth.Text = (PluginParams.hv_RminWidth / PluginParams.Pix_X).ToString("0.0");
            hWndCtrl_Display.repaint();
        }

        private void NUD_RminHeigth_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_RminHeigth = Convert.ToDouble(NUD_RminHeigth.Value);
            label_RminHeigth.Text = (PluginParams.hv_RminHeigth / PluginParams.Pix_Y).ToString("0.0");
            hWndCtrl_Display.repaint();
        }
        //极耳毛刺HY
        private void NUD_HYminArea_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_HyminArea = Convert.ToDouble(NUD_HYminArea.Value);
            label_HYminArea.Text = (PluginParams.hv_HyminArea / PluginParams.Pix_X / PluginParams.Pix_Y).ToString("0.0");
            hWndCtrl_Display.repaint();
        }

        private void NUD_HYminWidth_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_HyminWidth = Convert.ToDouble(NUD_HYminWidth.Value);
            label_HYminWidth.Text = (PluginParams.hv_HyminWidth / PluginParams.Pix_X).ToString("0.0");
            hWndCtrl_Display.repaint();
        }

        private void NUD_HYminHeigth_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_HyminHeigth = Convert.ToDouble(NUD_HYminHeigth.Value);
            label_HYminHeigth.Text = (PluginParams.hv_HyminHeigth / PluginParams.Pix_Y).ToString("0.0");
            hWndCtrl_Display.repaint();
        }
        //极耳毛刺H
        private void NUD_HminArea_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_HminArea = Convert.ToDouble(NUD_HminArea.Value);
            label_HminArea.Text = (PluginParams.hv_HminArea / PluginParams.Pix_X / PluginParams.Pix_Y).ToString("0.0");
            hWndCtrl_Display.repaint();
        }

        private void NUD_HminWidth_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_HminWidth = Convert.ToDouble(NUD_HminWidth.Value);
            label_HminWidth.Text = (PluginParams.hv_HminWidth / PluginParams.Pix_X).ToString("0.0");
            hWndCtrl_Display.repaint();
        }

        private void NUD_HminHeigth_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_HminHeigth = Convert.ToDouble(NUD_HminHeigth.Value);
            label_HminHeigth.Text = (PluginParams.hv_HminHeigth / PluginParams.Pix_Y).ToString("0.0");
            hWndCtrl_Display.repaint();
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
            catch (Exception)
            {
                return str;
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices != null && listView1.SelectedIndices.Count > 0)
            {
                ListView.SelectedIndexCollection c = listView1.SelectedIndices;
                string str = listView1.Items[c[0]].SubItems[0].Text.ToString();
                SelectIndex = Convert.ToInt32(str) - 1;
                // int index = PluginParams.List_Defect[Convert.ToInt32(str) - 1].hv_RegionIndex.I;
                ShowResult();
            }
        }

        private void CB_IsHalfEar_CheckedChanged(object sender, EventArgs e)
        {
            PluginParams.Bl_IsHalfEar = CB_IsHalfEar.Checked;
            hWndCtrl_Display.repaint();
        }
    }
}
