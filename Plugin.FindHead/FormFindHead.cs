using HalconAlgorithm;
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
using ViewROI;
using Frame_Core;
using FrameCommunity.Data;

namespace Plugin.FindHead
{
    public partial class FormFindHead : FormPluginBase
    {
        private FindHeadObj pluginsObj { get; set; }
        private Process proc { get; set; }
        public ParamsFindHead PluginParams { get; set; }

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();
        public HObject ho_Rectangle2;
        public HObject ho_SmallRectangle2;

        public int ShowShrinkIImageIndex = 0;
        public bool IsLoaded = false;

        bool bl_LoadImage = false;
        public FormFindHead()
        {
            InitializeComponent();
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunMeasureLine);   //更改测量矩形，执行测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowMeasureLineResult);     //更改测量矩形，重绘测量结果
        }
        public override void Init()
        {
            try
            {
                pluginsObj = (FindHeadObj)pluginsObjBase;
                PluginParams = (ParamsFindHead)pluginsObj.PluginParams;

                proc = Project.Instance.GetProcessByName(pluginsObj.moduleInfo.ProcessName);
                this.Text = pluginsObjBase.moduleInfo.ModuleName;
                HOperatorSet.GenEmptyObj(out ho_Rectangle2);
                HOperatorSet.GenEmptyObj(out ho_SmallRectangle2);
                ShowModuleParams(PluginParams);
                //测试存参用

                //当前模块执行完成回调注册     
               // ExecuteObjFinshEvent += ExecuteObjFinshImageShow;
            }
            catch (Exception)
            {


            }

        }

        /// <summary>
        /// 初始化仿真参数(弃用)
        /// </summary>
        public void InitemulationalParam()
        {
            try
            {
                if (true)//建立一份仿真参数
                {
                    ParamsFindHead PluginParamsTemp = (ParamsFindHead)pluginsObj.PluginParams;
                    PluginParams.Is_ActiveFindHead = PluginParamsTemp.Is_ActiveFindHead;
                    PluginParams.Is_CropHeadimg = PluginParamsTemp.Is_CropHeadimg;
                    PluginParams.str_ModelName = PluginParamsTemp.str_ModelName;
                    PluginParams.hv_RoiPointsNum = PluginParamsTemp.hv_RoiPointsNum;
                    PluginParams.hv_RoiMidRow = PluginParamsTemp.hv_RoiMidRow;
                    PluginParams.hv_RoiMidColumn = PluginParamsTemp.hv_RoiMidColumn;
                    PluginParams.hv_RoiAngle = PluginParamsTemp.hv_RoiAngle;
                    PluginParams.hv_RoiHeight = PluginParamsTemp.hv_RoiHeight;
                    PluginParams.hv_RoiWidth = PluginParamsTemp.hv_RoiWidth;
                    PluginParams.hv_MeasureSigma = PluginParamsTemp.hv_MeasureSigma;
                    PluginParams.hv_MeasureThreshold = PluginParamsTemp.hv_MeasureThreshold;
                    PluginParams.hv_MeasureNumInstances = PluginParamsTemp.hv_MeasureNumInstances;
                    PluginParams.hv_MeasureDistThreshold = PluginParamsTemp.hv_MeasureDistThreshold;
                    PluginParams.hv_MeasureTransition = PluginParamsTemp.hv_MeasureTransition;
                    PluginParams.hv_MeasureSelect = PluginParamsTemp.hv_MeasureSelect;
                    PluginParams.hv_MeasureMinScore = PluginParamsTemp.hv_MeasureMinScore;
                    PluginParams.hv_MaxNumIterations = PluginParamsTemp.hv_MaxNumIterations;
                    PluginParams.hv_MeasureInterpolation = PluginParamsTemp.hv_MeasureInterpolation;
                    PluginParams.hv_MeasureIORegions = PluginParamsTemp.hv_MeasureIORegions;

                    PluginParams.hv_RegionThreshold = PluginParamsTemp.hv_RegionThreshold;
                    PluginParams.hv_RegionNum = PluginParamsTemp.hv_RegionNum;
                    PluginParams.hv_ColumnStart = PluginParamsTemp.hv_ColumnStart;
                    PluginParams.hv_ColumnEnd = PluginParamsTemp.hv_ColumnEnd;
                    PluginParams.EnStubBar = PluginParamsTemp.EnStubBar;
                    PluginParams.WhiteRegionGray = PluginParamsTemp.WhiteRegionGray;
                    PluginParams.CoatingRegionGary = PluginParamsTemp.CoatingRegionGary;


                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 加载抓边参数到窗体
        /// </summary>
        /// <param name="ParamsFH"></param>
        private void ShowModuleParams(ParamsFindHead ParamsFH)
        {
            try
            {
                NUD_RoiPointsNum.Value = Convert.ToDecimal(ParamsFH.hv_RoiPointsNum);
                NUD_MeasLColumnStart.Value = Convert.ToDecimal((ParamsFH.hv_RoiMidColumn - ParamsFH.hv_RoiHeight / 2));
                NUD_MeasLColumnEnd.Value = Convert.ToDecimal((ParamsFH.hv_RoiMidColumn + ParamsFH.hv_RoiHeight / 2));
                switch (ParamsFH.hv_RoiAngle == 0)
                {
                    case true:
                        CB_MeasureAngle.Text = "从上到下";
                        break;
                    case false:
                        CB_MeasureAngle.Text = "从下到上";
                        break;
                    default:
                        break;
                }

                NUD_MeasureThreshold.Value = Convert.ToDecimal(ParamsFH.hv_MeasureThreshold);
                NUD_MeasureSigma.Value = Convert.ToDecimal(ParamsFH.hv_MeasureSigma);
                switch (ParamsFH.hv_MeasureTransition)
                {
                    case "negative":
                        CB_MeasureTransition.Text = "从明到暗";
                        break;
                    case "positive":
                        CB_MeasureTransition.Text = "从暗到明";
                        break;
                    case "all":
                        CB_MeasureTransition.Text = "不区分明暗";
                        break;
                    default:
                        break;
                }
                switch (ParamsFH.hv_MeasureSelect)
                {
                    case "first":
                        CB_MeasureSelect.Text = "第一条边";
                        break;
                    case "last":
                        CB_MeasureSelect.Text = "最后一条边";
                        break;
                    case "all":
                        CB_MeasureSelect.Text = "最强边";
                        break;
                    default:
                        break;
                }
                NUD_MeasureMinScore.Value = Convert.ToDecimal(ParamsFH.hv_MeasureMinScore);
                //照料头参数
                NUD_ColumnStart.Value = Convert.ToDecimal(ParamsFH.hv_ColumnStart);
                NUD_ColumnEnd.Value = Convert.ToDecimal(ParamsFH.hv_ColumnEnd);
                NUD_RegionNum.Value = Convert.ToDecimal(ParamsFH.hv_RegionNum);
                NUD_RegionThread.Value = Convert.ToDecimal(ParamsFH.hv_RegionThreshold);
                //主动找料头
                CB_Is_ActiveFindHead.Checked = PluginParams.Is_ActiveFindHead;
                if (PluginParams.Is_ActiveFindHead)
                {
                    num_HeadLineID.Enabled = false;
                    num_HeadLine.Enabled = false;
                }
                else
                {
                    num_HeadLineID.Enabled = true;
                    num_HeadLine.Enabled = true;
                }

                //料头预处理
                cbx_EnStubBar.Checked = PluginParams.EnStubBar;
                nud_WhiteRegionGray.Value = PluginParams.WhiteRegionGray;
                nud_CoatingRegionGary.Value = PluginParams.CoatingRegionGary;

                IsLoaded = true;
            }
            catch (Exception)
            {
            }

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
        /// ViewRoi回调函数：执行直线测量方法
        /// </summary>
        private void RunMeasureLine()
        {
        }

        /// <summary>
        /// ViewRoi回调函数：重绘直线测量结果
        /// </summary>
        private void ShowMeasureLineResult()
        {
            if (ShowShrinkIImageIndex == 2)
            {
                ShowMeasureLineResultToHWC(PluginParams);
            }
        }

        /// <summary>
        /// 显示缩略图到图像列表
        /// </summary>
        private void ShowShrinkImageResult(ParamsFindHead ParamsFH)
        {
            //显示缩略图到图像选项
            if (ParamsFH.list_ImageInfo.Last().ho_image.CountObj() != 0)
            {
                PaintWindows.DisplayImage(ParamsFH.list_ImageInfo.Last().ho_image, HWC_Image1);
                ShowShrinkIImageIndex = 1;
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
            }
            if (ParamsFH.ho_ImageHead.CountObj() != 0)
            {
                PaintWindows.DisplayImage(ParamsFH.ho_ImageHead, HWC_Image2);
                ShowShrinkIImageIndex = 2;
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
            }
            if (ParamsFH.ho_ImageMeasureLine.CountObj() != 0)
            {
                PaintWindows.DisplayImage(ParamsFH.ho_ImageMeasureLine, HWC_Image3);
                ShowShrinkIImageIndex = 2;
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
            }

        }

        /// <summary>
        /// 显示留白检测结果
        /// </summary>
        /// <param name="ParamsFH"></param>
        /// <param name="ho_Image"></param>
        private void ShowOriginalResultToHWC(ParamsFindHead ParamsFH, HObject ho_Image)
        {
            HImage hImage = new HImage();
            HTuple hv_Width;
            HTuple hv_Height;
            HObject ho_Rectangle;
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            try
            {
                //显示原图
                HobjectToHimage(ho_Image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();

                //显示留白检测Roi及灰度均值
                HOperatorSet.GetImageSize(hImage, out hv_Width, out hv_Height);
                for (int i = 0; i < ParamsFH.hv_RegionNum; i++)
                {
                    HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Height / ParamsFH.hv_RegionNum * i,
                        ParamsFH.hv_ColumnStart, hv_Height / ParamsFH.hv_RegionNum * (i + 1) - 1,
                        ParamsFH.hv_ColumnEnd);
                    if (ParamsFH.hv_GrayResult[i].I > 0)
                    {
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, "red");
                        PaintWindows.WriteStringInHwindown(HWC_Display.HalconWindow, ParamsFH.hv_GrayMean[i].O.ToString(), 10, "red",
                            hv_Height / ParamsFH.hv_RegionNum * i + 5, ParamsFH.hv_ColumnStart + 100);
                    }
                    else
                    {
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                        PaintWindows.WriteStringInHwindown(HWC_Display.HalconWindow, ParamsFH.hv_GrayMean[i].O.ToString(), 10, "blue",
                            hv_Height / ParamsFH.hv_RegionNum * i + 5, ParamsFH.hv_ColumnStart + 100);
                    }

                    HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 1);
                    HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                    HOperatorSet.DispObj(ho_Rectangle, HWC_Display.HalconWindow);

                }
            }
            catch (Exception)
            {

            }
            finally
            {
                ho_Rectangle.Dispose();
            }


        }

        /// <summary>
        /// 显示找料头直线测量结果
        /// </summary>
        /// <param name="ParamsFH"></param>
        private void ShowMeasureLineResultToHWC(ParamsFindHead ParamsFH)
        {
            HObject ho_Cross, ho_Rectangle2;
            HObject ho_RegionLines;
            HObject ho_RegionLines_mid;
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_RegionLines_mid);
            HTuple hv_Phi;
            HObject ho_Region;
            HOperatorSet.GenEmptyObj(out ho_Region);

            HTuple R0, C0, R1, C1;
            try
            {
                HOperatorSet.TupleRad(ParamsFH.hv_RoiAngle, out hv_Phi);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, ParamsFH.hv_RoiMidRow, ParamsFH.hv_RoiMidColumn,
                                                               -hv_Phi, ParamsFH.hv_RoiHeight / 2, ParamsFH.hv_RoiWidth / 2);

                HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 2);
                HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                HOperatorSet.DispObj(ho_Rectangle2, HWC_Display.HalconWindow);



                HOperatorSet.SetColor(HWC_Display.HalconWindow, "orange");
                HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 2);
                HOperatorSet.SetDraw(HWC_Display.HalconWindow, "fill");
                HOperatorSet.DispArrow(HWC_Display.HalconWindow, ParamsFH.hv_RoiMidRow + (Math.Sin(hv_Phi - Math.PI / 2) * (ParamsFH.hv_RoiWidth / 2)),
                         ParamsFH.hv_RoiMidColumn + (Math.Cos(hv_Phi - Math.PI / 2) * (ParamsFH.hv_RoiWidth / 2)),
                         ParamsFH.hv_RoiMidRow - (Math.Sin(hv_Phi - Math.PI / 2) * ParamsFH.hv_RoiWidth / 2),
                         ParamsFH.hv_RoiMidColumn - (Math.Cos(hv_Phi - Math.PI / 2) * ParamsFH.hv_RoiWidth / 2), 7);
                HOperatorSet.GenRegionLine(out ho_RegionLines_mid, ParamsFH.hv_RoiMidRow, ParamsFH.hv_RoiMidColumn - ParamsFH.hv_RoiHeight / 2,
                  ParamsFH.hv_RoiMidRow, ParamsFH.hv_RoiMidColumn + ParamsFH.hv_RoiHeight / 2);
                HOperatorSet.DispObj(ho_RegionLines_mid, HWC_Display.HalconWindow);
                if (ParamsFH.hv_CrossRow.Length > 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross, ParamsFH.hv_CrossRow, ParamsFH.hv_CrossColumn, 16, (new HTuple(45)).TupleRad());
                    HOperatorSet.SetDraw(HWC_Display.HalconWindow, "fill");
                    HOperatorSet.SetColor(HWC_Display.HalconWindow, "green");
                    HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 2);
                    HOperatorSet.DispObj(ho_Cross, HWC_Display.HalconWindow);
                }

                if (ParamsFH.hv_LineRowBegin.Length > 0)
                {
                    HOperatorSet.GenRegionLine(out ho_RegionLines, ParamsFH.hv_LineRowBegin, ParamsFH.hv_LineColumnBegin,
                                ParamsFH.hv_LineRowEnd, ParamsFH.hv_LineColumnEnd);
                    HOperatorSet.SetDraw(HWC_Display.HalconWindow, "fill");
                    HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 2);
                    HOperatorSet.SetColor(HWC_Display.HalconWindow, "red");
                    HOperatorSet.DispObj(ho_RegionLines, HWC_Display.HalconWindow);

                    //HOperatorSet.TupleRad(-ParamsFH.hv_RoiAngle - 90, out  hv_Phi1);
                    //HOperatorSet.GenRectangle2(out ho_Region, ParamsFH.hv_RoiMidRow + (-ParamsFH.hv_RoiHeight / 2 + ParamsFH.hv_RoiHeight / 2 / ParamsFH.hv_RoiPointsNum +
                    //            ParamsFH.hv_RoiHeight / ParamsFH.hv_RoiPointsNum * (ParamsFH.hv_RoiPointsNum - TrB_ROIIndex.Value + 1 - 1)) * Math.Cos(-hv_Phi1),
                    //            ParamsFH.hv_RoiMidColumn - (-ParamsFH.hv_RoiHeight / 2 + ParamsFH.hv_RoiHeight / 2 / ParamsFH.hv_RoiPointsNum +
                    //            ParamsFH.hv_RoiHeight / ParamsFH.hv_RoiPointsNum * (ParamsFH.hv_RoiPointsNum - TrB_ROIIndex.Value + 1 - 1)) * Math.Sin(-hv_Phi1),
                    //            hv_Phi1, ParamsFH.hv_RoiWidth / 2, ParamsFH.hv_RoiHeight / ParamsFH.hv_RoiPointsNum / 2);

                    //HOperatorSet.SetColor(HWC_Display.HalconWindow, "red");
                    //HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 1);
                    //HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                    //HOperatorSet.DispObj(ho_Region, HWC_Display.HalconWindow);
                    //ho_Region.Dispose();
                }

            }
            catch (Exception)
            {
            }

            ho_Rectangle2.Dispose();
            ho_Cross.Dispose();
            ho_RegionLines.Dispose();
            ho_RegionLines_mid.Dispose();

        }

        /// <summary>
        /// 显示裁图结果
        /// </summary>
        /// <param name="cropAndSpliceParams"></param>
        private void ShowCropResult(ParamsFindHead ParamsFH)
        {
            if (ParamsFH.bl_ContainHeadLine)
            {
                LB_HeadLineID.Text = ParamsFH.headLineID.ToString();
                LB_HeadLine.Text = ParamsFH.headLine.ToString();
            }
            else if (ParamsFH.hv_LineRowEnd == null)
            {
                LB_HeadLineID.Text = "          ";
                LB_HeadLine.Text = "          ";
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
        private void RUNImage()
        {
            if (FileDialog.FileNames != null && !bl_LoadImage)
            {
                HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
                HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
                HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
                if (0 <= imgNum && imgNum < FileDialog.FileNames.Count())
                {
                    ImageInfo imageinfos = new ImageInfo();
                    imageinfos.ho_image = new HImage(FileDialog.FileNames[imgNum]);
                    HOperatorSet.Rgb1ToGray(imageinfos.ho_image, out imageinfos.ho_image);
                    imageinfos.image_ID = imgNum + 1;
                    imageinfos.cameraName = "A相机";
                    HTuple H, W;
                    HOperatorSet.GetImageSize(imageinfos.ho_image, out W, out H);
                    imageinfos.imageHeight = H.D;
                    imageinfos.imageWidth = W.D;
                    PluginParams.list_ImageInfo.Add(imageinfos);//将结构体加入队列

                    hWndCtrl_Display.addIconicVar(imageinfos.ho_image);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                    PaintWindows.WriteStringInHwindown(HWC_Display.HalconWindow, imageinfos.image_ID.ToString(), 20, "blue", 20, 20);
                    PluginParams.ImageNowID = imgNum + 1;
                    PluginParams.Is_TestFindHead = true;
                    pluginsObj.ExecuteObj();
                    if (PluginParams.Is_ActiveFindHead || true)
                    {
                        ShowOriginalResultToHWC(PluginParams, imageinfos.ho_image);
                    }
                    PaintWindows.WriteStringInHwindown(HWC_Display.HalconWindow, imageinfos.image_ID.ToString(), 20, "blue", 20, 20);
                    ShowCropResult(PluginParams);
                    ShowShrinkImageResult(PluginParams);

                    if (PluginParams.list_ImageInfo.Count > 1)
                    {
                        PluginParams.list_ImageInfo.RemoveAt(0);//使用完移除队列
                    }

                }
                GC.Collect();
            }
        }

        /// <summary>
        /// 执行完成图像显示
        /// </summary>
        public void ExecuteObjFinshImageShow()
        {
            try
            {
                //在调试界面只检测料头边
                pluginsObj.TestExecuteObj();


                if (PluginParams.Is_ActiveFindHead || true)
                {
                    ShowOriginalResultToHWC(PluginParams, PluginParams.list_ImageInfo.Find(p => p.image_ID == PluginParams.ImageNowID).ho_image);
                }
                PaintWindows.WriteStringInHwindown(HWC_Display.HalconWindow, PluginParams.ImageNowID.ToString(), 20, "blue", 20, 20);
                ShowCropResult(PluginParams);
                ShowShrinkImageResult(PluginParams);
                //if (PluginParams.list_ImageInfo.Count() > FileDialog.FileNames.Count())
                //{
                //    PluginParams.list_ImageInfo.RemoveAt(0);
                //}
            }
            catch (Exception)
            {


            }

        }
        #region 窗体控件重绘，防止闪烁
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;
        #endregion

        public OpenFileDialog FileDialog = new OpenFileDialog();
        int imgNum = 0;
        private void LoadImage_Click(object sender, EventArgs e)
        {
            if (!bl_LoadImage)
            {
                bl_LoadImage = true;
                //打开指定路径文件夹，加载图片
                OpenFileDialog FileDialog0 = new OpenFileDialog();
                FileDialog0.Multiselect = true;
                FileDialog0.InitialDirectory = System.Environment.CurrentDirectory;
                if (FileDialog0.ShowDialog() == DialogResult.OK)
                {
                    //PluginParams.list_ImageInfo.Clear();
                    imgNum = 0;
                    FileDialog = FileDialog0;
                }
                bl_LoadImage = false;
            }
        }
        private void btn_PreImg_Click(object sender, EventArgs e)
        {
            RUNImage();
            imgNum--;
            if (imgNum < 0 && FileDialog.FileNames.Count() > 0)
            {

                imgNum = FileDialog.FileNames.Count() - 1;
            }

        }
        private void btn_autoRun_Click(object sender, EventArgs e)
        {
            RUNImage();
            ExecuteObjFinshImageShow();
        }

        private void btn_NextImg_Click(object sender, EventArgs e)
        {
            RUNImage();
            imgNum++;
            if (imgNum > FileDialog.FileNames.Count() - 1)
            {
                imgNum = 0;
            }
        }
        //找料头参数
        private void NUD_ColumnStart_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            if (NUD_ColumnStart.Value > NUD_ColumnEnd.Value)
            {
                NUD_ColumnEnd.Value = NUD_ColumnStart.Value;
            }
            PluginParams.hv_ColumnStart = Convert.ToInt32(NUD_ColumnStart.Value);
        }
        private void NUD_ColumnEnd_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            if (NUD_ColumnStart.Value > NUD_ColumnEnd.Value)
            {
                NUD_ColumnStart.Value = NUD_ColumnEnd.Value;
            }
            PluginParams.hv_ColumnEnd = Convert.ToInt32(NUD_ColumnEnd.Value);
        }
        private void NUD_RegionNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.hv_RegionNum = Convert.ToInt32(NUD_RegionNum.Value);
        }
        private void NUD_RegionThread_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.hv_RegionThreshold = Convert.ToInt32(NUD_RegionThread.Value);
        }
        private void CB_Is_ActiveFindHead_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.Is_ActiveFindHead = CB_Is_ActiveFindHead.Checked;
            if (PluginParams.Is_ActiveFindHead)
            {
                num_HeadLineID.Enabled = false;
                num_HeadLine.Enabled = false;
            }
            else
            {
                num_HeadLineID.Enabled = true;
                num_HeadLine.Enabled = true;
            }

        }
        private void num_HeadLineID_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.headLineID = Convert.ToInt32(num_HeadLineID.Value);
        }
        private void num_HeadLine_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.headLine = Convert.ToDouble(num_HeadLine.Value);
        }

        //抓边参数
        private void NUD_MeasLColumnStart_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            if (NUD_MeasLColumnStart.Value > NUD_MeasLColumnEnd.Value)
            {
                NUD_MeasLColumnEnd.Value = NUD_MeasLColumnStart.Value;
            }
            PluginParams.hv_RoiMidColumn = Convert.ToInt32(NUD_MeasLColumnStart.Value + (NUD_MeasLColumnEnd.Value - NUD_MeasLColumnStart.Value) / 2);
            PluginParams.hv_RoiHeight = Convert.ToInt32(NUD_MeasLColumnEnd.Value - NUD_MeasLColumnStart.Value);
            if (PluginParams.ho_ImageMeasureLine.CountObj() > 0)
            {
                hWndCtrl_Display.repaint();
            }
        }

        private void NUD_MeasLColumnEnd_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            if (NUD_MeasLColumnStart.Value > NUD_MeasLColumnEnd.Value)
            {
                NUD_MeasLColumnStart.Value = NUD_MeasLColumnEnd.Value;
            }
            PluginParams.hv_RoiMidColumn = Convert.ToInt32(NUD_MeasLColumnStart.Value + (NUD_MeasLColumnEnd.Value - NUD_MeasLColumnStart.Value) / 2);
            PluginParams.hv_RoiHeight = Convert.ToInt32(NUD_MeasLColumnEnd.Value - NUD_MeasLColumnStart.Value);
            if (PluginParams.ho_ImageMeasureLine.CountObj() != 0)
            {
                hWndCtrl_Display.repaint();
            }
        }
        private void NUD_RoiPointsNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.hv_RoiPointsNum = Convert.ToInt32(NUD_RoiPointsNum.Value);
        }

        private void NUD_MeasureThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.hv_MeasureThreshold = Convert.ToInt32(NUD_MeasureThreshold.Value);
        }

        private void NUD_MeasureSigma_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.hv_MeasureSigma = Convert.ToDouble(NUD_MeasureSigma.Value);
        }

        private void NUD_MeasureMinScore_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.hv_MeasureMinScore = Convert.ToDouble(NUD_MeasureMinScore.Value);
        }

        private void CB_MeasureAngle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            switch (CB_MeasureAngle.Text)
            {
                case "从上到下":
                    PluginParams.hv_RoiAngle = 0;
                    break;
                case "从下到上":
                    PluginParams.hv_RoiAngle = 180;
                    break;
                default:
                    break;
            }
        }

        private void CB_MeasureTransition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            switch (CB_MeasureTransition.Text)
            {
                case "从明到暗":
                    PluginParams.hv_MeasureTransition = "negative";
                    break;
                case "从暗到明":
                    PluginParams.hv_MeasureTransition = "positive";
                    break;
                case "不区分明暗":
                    PluginParams.hv_MeasureTransition = "all";
                    break;
                default:
                    break;
            }
        }

        private void CB_MeasureSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            switch (CB_MeasureSelect.Text)
            {
                case "第一条边":
                    PluginParams.hv_MeasureSelect = "first";
                    break;
                case "最后一条边":
                    PluginParams.hv_MeasureSelect = "last";
                    break;
                case "最强边":
                    PluginParams.hv_MeasureSelect = "all";
                    break;
                default:
                    break;
            }

        }

        private void HWC_Image1_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                if (PluginParams.list_ImageInfo.Last().ho_image.CountObj() != 0)
                {
                    hWndCtrl_Display.bl_RoiOperateFlag = false;
                    //显示原图
                    HImage hImage = new HImage();
                    HobjectToHimage(PluginParams.list_ImageInfo.Last().ho_image, ref hImage);
                    hWndCtrl_Display.addIconicVar(hImage);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                    LB_IMageSize.Text = ShowIMageSize(hImage);
                }
            }
            catch (Exception)
            {


            }

        }

        private void HWC_Image2_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                if (PluginParams.ho_ImageHead.CountObj() != 0)
                {
                    hWndCtrl_Display.bl_RoiOperateFlag = false;
                    //显示原图
                    HImage hImage = new HImage();
                    HobjectToHimage(PluginParams.ho_ImageHead, ref hImage);
                    hWndCtrl_Display.addIconicVar(hImage);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                    LB_IMageSize.Text = ShowIMageSize(hImage);
                }
            }
            catch (Exception)
            {


            }

        }

        private void HWC_Image3_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                if (PluginParams.ho_ImageMeasureLine.CountObj() != 0)
                {
                    hWndCtrl_Display.bl_RoiOperateFlag = false;
                    //显示料头图
                    HImage hImage = new HImage();
                    HobjectToHimage(PluginParams.ho_ImageMeasureLine, ref hImage);
                    hWndCtrl_Display.addIconicVar(hImage);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                    LB_IMageSize.Text = ShowIMageSize(hImage);
                }
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
                }
                catch (Exception)
                {
                }
        }

        private void cbx_EnStubBar_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.EnStubBar = cbx_EnStubBar.Checked;

        }

        private void nud_WhiteRegionGray_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.WhiteRegionGray = Convert.ToInt32(nud_WhiteRegionGray.Value);
        }

        private void nud_CoatingRegionGary_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.CoatingRegionGary = Convert.ToInt32(nud_CoatingRegionGary.Value);
        }
    }
}
