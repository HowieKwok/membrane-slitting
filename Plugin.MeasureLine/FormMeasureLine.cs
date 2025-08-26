using Frame_Core;
using HalconAlgorithm;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI;
using YYSD.UICore.Control;
using YYSD.UICore;

namespace Plugin.MeasureLine
{
    public partial class FormMeasureLine : FormPluginBase
    {
        // private IPlugin m_moduleobj;
        private MeasLineObj pluginsObj { get; set; }
        private Process proc { get; set; }
        public ParamsMeasureLine PluginParams { get; set; }

        public HObject ho_Rectangle2;
        public HObject ho_SmallRectangle2;

        public HObject ho_Image1 = null;

        bool bl_SelectColor = false;
        bool bl_RoiExist = false;

        bool bl_RefreshObj = true;
        bool bl_ShowObj = false;

        bool bl_RefreshChart = true;
        bool bl_ShowChart = false;

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();

        public int showHwindIndex = 0;
        Thread thread_RunObj;
        public override void Init()
        {
            //string error = "";
            pluginsObj = (MeasLineObj)pluginsObjBase;
            PluginParams = (ParamsMeasureLine)pluginsObj.PluginParams;

            PluginParams.backup(PluginParams);//备份参数，用于修改记录对比
            // error = YControlAutobind.Saveing<PluginParamsBase, IYCustomAttribute>(this, PluginParams);
            proc = Project.Instance.GetProcessByName(pluginsObj.moduleInfo.ProcessName);
            this.Text = pluginsObjBase.moduleInfo.ModuleName;
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_SmallRectangle2);



            //ShowFitLineParams(PluginParams);
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
            string error = "";
            //视觉设置
            //  PluginParams.FilePath = (Application.StartupPath + "\\record\\参数修改记录\\视觉设置\\");
            PluginParams.FilePath = Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\";
            error = YControlAutobind.Saveing<PluginParamsBase, IYCustomAttribute>(this, PluginParams);
            // 调用父窗体
            base.Btn_Save_Click(sender, e);
            PluginParams.saveChangeLog(PluginParams, String.Format("测试视觉设置:{0} 中 {1}", pluginsObj.moduleInfo.ProcessName, pluginsObj.moduleInfo.ModuleName));
            PluginParams.backup(PluginParams);//备份参数，用于修改记录对比
            if (error != "") System.Windows.Forms.MessageBox.Show(error);

        }

        public FormMeasureLine()
        {

            InitializeComponent();
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);

            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行直线测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘直线测量结果



            CB_MeasureTransition.DataSource = Enum.GetNames(typeof(MeasureTransition));
            CB_MeasureSelect.DataSource = Enum.GetNames(typeof(Frame_Core.MeasureLine));
            CB_PeakValleySelect.DataSource = Enum.GetNames(typeof(Frame_Core.PeakValleymum));

            CB_MeasureIORegions.DataSource = Enum.GetNames(typeof(MeasureIORegions));
            CB_MeasureInterpolation.DataSource = Enum.GetNames(typeof(MeasureInterpolation));

            //thread_RunObj = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        if (bl_RefreshObj & bl_ShowObj)
            //        {
            //            Thread.Sleep(100);
            //            bl_ShowObj = false;
            //            if (roiRectangle2 != null & PluginParams.ho_Image.CountObj() != 0)
            //            {
            //                pluginsObj.ExecuteObj();
            //            }

            //            Invoke(new MethodInvoker(delegate
            //            {
            //                hWndCtrl_Display.repaint();

            //            }));
            //            bl_ShowChart = true;
            //        }
            //        Thread.Sleep(10);
            //    }
            //});
            //thread_RunObj.IsBackground = true;
            //thread_RunObj.Start();
        }
        private void FormMeasureLine_Load(object sender, EventArgs e)
        {
            LanguageProject.ReBindCombox<MeasureTransition>(CB_MeasureTransition, FormLanguage);
            LanguageProject.ReBindCombox<Frame_Core.MeasureLine>(CB_MeasureSelect, FormLanguage);
            LanguageProject.ReBindCombox<Frame_Core.PeakValleymum>(CB_PeakValleySelect, FormLanguage);

            LanguageProject.ReBindCombox<MeasureIORegions>(CB_MeasureIORegions, FormLanguage);
            LanguageProject.ReBindCombox<MeasureInterpolation>(CB_MeasureInterpolation, FormLanguage);
            LoadStandImage();
            ShowFitLineParams(PluginParams);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bl_RefreshObj & bl_ShowObj)
            {
                // Thread.Sleep(100);
                //  refrshCombox(PluginParams);
                bl_ShowObj = false;
                if (roiRectangle2 != null & PluginParams.ho_Image.CountObj() != 0)
                {
                    pluginsObj.ExecuteObj();
                }

                Invoke(new MethodInvoker(delegate
                {
                    hWndCtrl_Display.repaint();

                }));
                bl_ShowChart = true;
            }
        }

        /// <summary>
        /// 加载模板图，或者正在检测的图片
        /// </summary>
        private void LoadStandImage()
        {
            try
            {
                if (ho_Image1 != null)
                {
                    if (!pluginsObj.isExistModel)
                    {
                        PluginParams.ho_Image = ho_Image1;
                    }
                }
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_Image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
            }
            catch (Exception ee)
            {

            }
            SetRoiParams(PluginParams, roiRectangle2, roiController);
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
        /// <summary>
        /// 加载抓边参数到窗体
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowFitLineParams(ParamsMeasureLine measLineParams)
        {
            try
            {
                NUD_RoiNum.Value = (decimal)measLineParams.hv_RoiPointsNum;
                NUD_RoiWidth.Value = (decimal)measLineParams.hv_RoiWidth;
                NUD_RoiHeight.Value = (decimal)measLineParams.hv_RoiHeight;
                NUD_RoiAngle.Value = (decimal)measLineParams.hv_RoiAngle;
                NUD_RoiMidRow.Value = (decimal)measLineParams.hv_RoiMidRow;
                NUD_RoiMidColumn.Value = (decimal)measLineParams.hv_RoiMidColumn;
                //NUD_RoiNum.Value = Convert.ToDecimal(measLineParams.hv_RoiPointsNum);
                //NUD_RoiWidth.Value = Convert.ToDecimal(measLineParams.hv_RoiWidth);
                //NUD_RoiHeight.Value = Convert.ToDecimal(measLineParams.hv_RoiHeight);
                //NUD_RoiAngle.Value = Convert.ToDecimal(measLineParams.hv_RoiAngle);
                //NUD_RoiMidRow.Value = Convert.ToDecimal(measLineParams.hv_RoiMidRow);
                //NUD_RoiMidColumn.Value = Convert.ToDecimal(measLineParams.hv_RoiMidColumn);

                switch (measLineParams.str_RoiMoveIndex)
                {
                    case "StaticROI":
                        RB_StaticROI.Checked = true;
                        break;
                    case "StandardROI":
                        RB_StaticROI.Checked = true;
                        break;
                    case "IncreaseROI":
                        RB_StaticROI.Checked = true;
                        break;
                    default:
                        break;
                }
                NUD_RoiIncreaseNum.Value = Convert.ToDecimal(measLineParams.hv_RoiIncreaseNum);
                NUD_RoiIncreaseRow.Value = Convert.ToDecimal(measLineParams.hv_RoiIncreaseRow);
                NUD_RoiIncreaseColumn.Value = Convert.ToDecimal(measLineParams.hv_RoiIncreaseColumn);

                NUD_RoiStandardRow.Value = Convert.ToDecimal(measLineParams.hv_RoiStandardRow);
                NUD_RoiStandardColumn.Value = Convert.ToDecimal(measLineParams.hv_RoiStandardColumn);
                NUD_RoiStandardOffsetRow.Value = Convert.ToDecimal(measLineParams.hv_RoiStandardOffsetRow);
                NUD_RoiStandardOffsetColumn.Value = Convert.ToDecimal(measLineParams.hv_RoiStandardOffsetColumn);

                NUD_MeasureThreshold.Value = Convert.ToDecimal(measLineParams.hv_MeasureThreshold);
                NUD_MeasureSigma.Value = Convert.ToDecimal(measLineParams.hv_MeasureSigma);
                switch (measLineParams.hv_MeasureTransition)
                {
                    case "negative":
                        CB_MeasureTransition.SelectedIndex = 0;
                        // CB_MeasureTransition.Text = "从明到暗";
                        break;
                    case "positive":
                        CB_MeasureTransition.SelectedIndex = 1;
                        // CB_MeasureTransition.Text = "从暗到明";
                        break;
                    case "all":
                        CB_MeasureTransition.SelectedIndex = 2;
                        //CB_MeasureTransition.Text = "不区分明暗";
                        break;
                    default:
                        break;
                }
                switch (measLineParams.hv_MeasureSelect)
                {
                    case "first":
                        CB_MeasureSelect.SelectedIndex = 0;
                        //  CB_MeasureSelect.Text = "第一条边";
                        break;
                    case "last":
                        CB_MeasureSelect.SelectedIndex = 1;
                        // CB_MeasureSelect.Text = "最后一条边";
                        break;
                    case "all":
                        CB_MeasureSelect.SelectedIndex = 2;
                        // CB_MeasureSelect.Text = "最强边";
                        break;
                    default:
                        break;
                }
                switch (measLineParams.hv_PeakValleySelect)
                {
                    case "None":
                        CB_PeakValleySelect.SelectedIndex = 0;
                        //  CB_PeakValleySelect.Text = "无选择";
                        break;
                    case "peak":
                        CB_PeakValleySelect.SelectedIndex = 1;
                        // CB_PeakValleySelect.Text = "峰值";
                        break;
                    case "valley":
                        CB_PeakValleySelect.SelectedIndex = 2;
                        // CB_PeakValleySelect.Text = "谷值";
                        break;
                    case "RejectionAverage":
                        CB_PeakValleySelect.SelectedIndex = 3;
                        // CB_PeakValleySelect.Text = "剔值平均";
                        break;
                    default:
                        break;
                }


                NUD_MeasureMinScore.Value = Convert.ToDecimal(measLineParams.hv_MeasureMinScore);
                NUD_MeasureNumInstances.Value = Convert.ToDecimal(measLineParams.hv_MeasureNumInstances);
                NUD_MeasureDistThreshold.Value = Convert.ToDecimal(measLineParams.hv_MeasureDistThreshold);
                NUD_MaxNumIterations.Value = Convert.ToDecimal(measLineParams.hv_MaxNumIterations);
                switch (measLineParams.hv_MeasureIORegions)
                {
                    case "true":
                        CB_MeasureIORegions.SelectedIndex = 0;
                        break;
                    case "false":
                        CB_MeasureIORegions.SelectedIndex = 1;
                        break;
                    default:
                        break;
                }
                switch (measLineParams.hv_MeasureInterpolation)
                {
                    case "nearest_neighbor":
                        CB_MeasureInterpolation.SelectedIndex = 0;
                        // CB_MeasureInterpolation.Text = "最近邻域法";
                        break;
                    case "bilinear":
                        CB_MeasureInterpolation.SelectedIndex = 1;
                        // CB_MeasureInterpolation.Text = "双线性插值法";
                        break;
                    case "bicubic":
                        CB_MeasureInterpolation.SelectedIndex = 2;
                        // CB_MeasureInterpolation.Text = "双三次插值法";
                        break;
                    default:
                        break;
                }

                if (measLineParams.bl_ImageProcessIndex)
                {
                    RB_ScaleSharpen.Checked = true;
                }
                else
                {
                    RB_ProcessNone.Checked = true;
                }

                NUD_ScaleMean.Value = Convert.ToDecimal(measLineParams.hv_ScaleMean);
                NUD_ScaleFactor.Value = Convert.ToDecimal(measLineParams.hv_ScaleFactor);
                NUD_SharpenIterations.Value = Convert.ToDecimal(measLineParams.hv_SharpenIterations);
                NUD_SharpenSigma.Value = Convert.ToDecimal(measLineParams.hv_SharpenSigma);

                CB_DefaultLine.Checked = measLineParams.bl_DefaultLine;

            }
            catch (Exception e)
            {

            }

        }

        /// <summary>
        /// 获取ROI参数并显示
        /// </summary>
        /// <param name="roiRectangle2"></param>
        /// <param name="measLineParams"></param>
        public void GetRoiParams(ROIRectangle2 roiRectangle2, ParamsMeasureLine measLineParams)
        {
            HTuple hv_Phi;
            HTuple hv_Angle;
            //measLineParams.ho_Image = roiRectangle2.
            measLineParams.hv_RoiPointsNum = roiRectangle2.PointNum;
            roiRectangle2.ROIName = "";

            measLineParams.hv_RoiMidRow = roiRectangle2.midR;
            measLineParams.hv_RoiMidColumn = roiRectangle2.midC;
            HOperatorSet.TupleRad(measLineParams.hv_RoiAngle, out hv_Phi);
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Angle);
            measLineParams.hv_RoiAngle = hv_Angle;
            measLineParams.hv_RoiWidth = roiRectangle2.length2 * 2;
            measLineParams.hv_RoiHeight = roiRectangle2.length1 * 2;

            measLineParams.hv_RoiStandardOffsetRow = measLineParams.hv_RoiMidRow - measLineParams.hv_RoiStandardRow;
            measLineParams.hv_RoiStandardOffsetColumn = measLineParams.hv_RoiMidColumn - measLineParams.hv_RoiStandardColumn;
        }

        /// <summary>
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="measLineParams">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(ParamsMeasureLine measLineParams, ROIRectangle2 roiRectangle2, ROIController roiController)
        {
            HTuple hv_Phi;
            HTuple hv_Width;
            HTuple hv_Height;
            roiRectangle2.PointNum = measLineParams.hv_RoiPointsNum;
            roiRectangle2.ROIName = "";

            roiRectangle2.midR = measLineParams.hv_RoiMidRow;
            roiRectangle2.midC = measLineParams.hv_RoiMidColumn;
            HOperatorSet.TupleRad(measLineParams.hv_RoiAngle, out hv_Phi);
            roiRectangle2.phi = hv_Phi.D;
            roiRectangle2.length2 = measLineParams.hv_RoiWidth / 2;
            roiRectangle2.length1 = measLineParams.hv_RoiHeight / 2;

            if (measLineParams.ho_Image != null)
            {
                if (measLineParams.ho_Image.CountObj() != 0)
                {
                    HOperatorSet.GetImageSize(measLineParams.ho_Image, out hv_Width, out hv_Height);
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
                    bl_ShowObj = true;
                }
            }
        }

        /// <summary>
        /// 设置ROI位置拖动条参数
        /// </summary>
        /// <param name="int_Value"></param>
        private void SetTrakBarParams(int int_Value)
        {
            //TrB_ROIIndex.Maximum = int_Value;
            //TrB_ROIIndex.Value = Convert.ToInt32(int_Value / 2 + 1);
        }

        /// <summary>
        /// ViewRoi回调函数：执行检测方法
        /// </summary>
        private void RunFunction()
        {
            try
            {
                bl_RefreshObj = false;
                if (roiController.activeROIidx == -1)
                {
                    if (showHwindIndex == 0)
                    {
                        if (HWC_Image1.ImagePart != null)
                        {
                            if (PluginParams.ho_Image.CountObj() != 0)
                            {
                                hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_Image;
                            }
                        }
                    }
                    else if (showHwindIndex == 1)
                    {
                        if (HWC_Image2.ImagePart != null)
                        {
                            if (PluginParams.ho_ImageProcessed.CountObj() != 0)
                            {
                                hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_ImageProcessed;
                            }
                        }
                    }
                    else
                    {
                        if (PluginParams.ho_ImageProcessed.CountObj() != 0)
                        {
                            hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_ImageProcessed;
                        }
                        else if (PluginParams.ho_Image.CountObj() != 0)
                        {
                            hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_Image;
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// ViewRoi回调函数：显示检测结果
        /// </summary>
        private void ShowResult()
        {
            ShowLineToHWD(PluginParams, HWC_Display.HalconWindow, roiRectangle2.size);
            bl_RefreshObj = true;
        }
        /// <summary>
        /// 显示直线到窗体上
        /// </summary>
        /// <param name="measLineParams"></param>
        /// <param name="hv_HWindowID"></param>
        private void ShowLineToHWD(ParamsMeasureLine measLineParams, HTuple hv_HWindowID, HTuple hv_size)
        {
            SendMessage(tableLayoutPanel3.Handle, WM_SETREDRAW, 0, IntPtr.Zero);//开始重绘
            if (roiController.activeROIidx == -1)
            {
                if (roiRectangle2 != null)
                    RefreshRoiParams(roiRectangle2);
                HObject ho_Cross, ho_RegionLines, ho_Point;
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_RegionLines);
                HOperatorSet.GenEmptyObj(out ho_Point);


                ////显示缩略图到图像选项
                if (measLineParams.ho_Image.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(measLineParams.ho_Image, HWC_Image1);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
                }

                if (measLineParams.ho_ImageProcessed.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(measLineParams.ho_ImageProcessed, HWC_Image2);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
                }

                if (RB_ProcessNone.Checked)
                {
                    if (measLineParams.ho_Image.CountObj() != 0)
                    {
                        PaintWindows.DisplayImage(measLineParams.ho_Image, HWC_Image3);
                    }
                    else
                    {
                        HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
                    }
                }
                else if (RB_ScaleSharpen.Checked)
                {

                    if (measLineParams.ho_ImageProcessed.CountObj() != 0)
                    {
                        PaintWindows.DisplayImage(measLineParams.ho_ImageProcessed, HWC_Image3);
                    }
                    else if (measLineParams.ho_Image.CountObj() != 0)
                    {
                        PaintWindows.DisplayImage(measLineParams.ho_Image, HWC_Image3);
                    }
                    else
                    {
                        HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
                    }
                }

                HTuple hv_Phi;
                HTuple hv_Phi1;
                //生成测量矩形
                if (measLineParams.str_RoiMoveIndex == "IncreaseROI")
                {

                    HOperatorSet.TupleRad(measLineParams.hv_RoiAngle, out hv_Phi);
                    HOperatorSet.GenRectangle2(out ho_Rectangle2, measLineParams.hv_RoiMidRow + measLineParams.hv_RoiIncreaseRow * measLineParams.hv_RoiIncreaseNum,
                                                measLineParams.hv_RoiMidColumn + measLineParams.hv_RoiIncreaseColumn * measLineParams.hv_RoiIncreaseNum,
                                                 -hv_Phi, measLineParams.hv_RoiHeight / 2, measLineParams.hv_RoiWidth / 2);

                }
                else
                {
                    HOperatorSet.TupleRad(measLineParams.hv_RoiAngle, out hv_Phi);
                    HOperatorSet.GenRectangle2(out ho_Rectangle2, measLineParams.hv_RoiMidRow, measLineParams.hv_RoiMidColumn,
                                                -hv_Phi, measLineParams.hv_RoiHeight / 2, measLineParams.hv_RoiWidth / 2);
                }




                //生成小测量矩形
                HOperatorSet.TupleRad(-measLineParams.hv_RoiAngle - 90, out hv_Phi1);
                HOperatorSet.GenRectangle2(out ho_SmallRectangle2, measLineParams.hv_RoiMidRow + (-measLineParams.hv_RoiHeight / 2 + measLineParams.hv_RoiHeight / 2 / measLineParams.hv_RoiPointsNum +
                            measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum * (measLineParams.hv_RoiPointsNum - TrB_ROIIndex.Value + 1 - 1)) * Math.Cos(-hv_Phi1),
                            measLineParams.hv_RoiMidColumn - (-measLineParams.hv_RoiHeight / 2 + measLineParams.hv_RoiHeight / 2 / measLineParams.hv_RoiPointsNum +
                            measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum * (measLineParams.hv_RoiPointsNum - TrB_ROIIndex.Value + 1 - 1)) * Math.Sin(-hv_Phi1),
                            hv_Phi1, measLineParams.hv_RoiWidth / 2, measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum / 2);

                if (measLineParams.hv_CrossRow != null & hWndCtrl_Display.bl_RoiOperateFlag & !bl_ShowObj)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross, measLineParams.hv_CrossRow, measLineParams.hv_CrossColumn, hv_size * 3, (new HTuple(45)).TupleRad());
                    HOperatorSet.SetDraw(hv_HWindowID, "fill");
                    HOperatorSet.SetColor(hv_HWindowID, "green");
                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.DispObj(ho_Cross, hv_HWindowID);
                }

                //显示拟合直线及拾取点
                if (measLineParams.hv_LineRowBegin != null & hWndCtrl_Display.bl_RoiOperateFlag & !bl_ShowObj)
                {
                    HOperatorSet.GenRegionLine(out ho_RegionLines, measLineParams.hv_LineRowBegin, measLineParams.hv_LineColumnBegin,
                                                        measLineParams.hv_LineRowEnd, measLineParams.hv_LineColumnEnd);

                    HOperatorSet.SetLineWidth(hv_HWindowID, 3);
                    HOperatorSet.SetColor(hv_HWindowID, "yellow");
                    //HOperatorSet.DispLine(hv_HWindowID, measLineParams.hv_LineRowBegin, measLineParams.hv_LineColumnBegin,
                    //                                    measLineParams.hv_LineRowEnd, measLineParams.hv_LineColumnEnd);
                    HOperatorSet.DispObj(ho_RegionLines, hv_HWindowID);

                }

                //显示测量矩形
                if (!hWndCtrl_Display.bl_RoiOperateFlag || measLineParams.str_RoiMoveIndex == "IncreaseROI" & roiController.activeROIidx == -1)
                {
                    HOperatorSet.SetColor(hv_HWindowID, "green");
                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.SetDraw(hv_HWindowID, "margin");
                    HOperatorSet.DispObj(ho_Rectangle2, hv_HWindowID);
                }

                if (measLineParams.str_RoiMoveIndex == "StandardROI" & hWndCtrl_Display.bl_RoiOperateFlag)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Point, measLineParams.hv_RoiStandardRow, measLineParams.hv_RoiStandardColumn, 50 * hv_size, 0);
                    HOperatorSet.SetColor(hv_HWindowID, "red");
                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.SetDraw(hv_HWindowID, "margin");
                    HOperatorSet.DispObj(ho_Point, hv_HWindowID);
                }

                HOperatorSet.SetColor(hv_HWindowID, "red");
                HOperatorSet.SetLineWidth(hv_HWindowID, 1);
                HOperatorSet.SetDraw(hv_HWindowID, "margin");
                HOperatorSet.DispObj(ho_SmallRectangle2, hv_HWindowID);

                HOperatorSet.SetColor(HWC_Image3.HalconWindow, "blue");
                HOperatorSet.SetLineWidth(HWC_Image3.HalconWindow, 2);
                HOperatorSet.SetDraw(HWC_Image3.HalconWindow, "margin");
                PaintWindows.DisplayRegion(ho_Rectangle2, measLineParams.ho_Image, HWC_Image3);

                ho_Cross.Dispose();
                ho_RegionLines.Dispose();
                ho_Point.Dispose();
            }
            SendMessage(tableLayoutPanel3.Handle, WM_SETREDRAW, 1, IntPtr.Zero);//开始重绘
            tableLayoutPanel3.Refresh();

        }
        /// <summary>
        /// ROI参数显示刷新
        /// </summary>
        /// <param name="roiRectangle2"></param>
        private void RefreshRoiParams(ROIRectangle2 roiRectangle2)
        {
            HTuple hv_Deg;
            NUD_RoiWidth.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.length2 * 2));
            NUD_RoiHeight.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.length1 * 2));
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Deg);
            NUD_RoiAngle.Value = Convert.ToDecimal(Convert.ToInt32(hv_Deg.D));
            NUD_RoiMidRow.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.midR));
            NUD_RoiMidColumn.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.midC));

            NUD_RoiStandardOffsetRow.Value = NUD_RoiMidRow.Value - NUD_RoiStandardRow.Value;
            NUD_RoiStandardOffsetColumn.Value = NUD_RoiMidColumn.Value - NUD_RoiStandardColumn.Value;
        }


        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.InitialDirectory = "E:\\";
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PluginParams.ho_Image = new HImage(FileDialog.FileName);
                    ho_Image1 = PluginParams.ho_Image;
                    hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);

                    SetRoiParams(PluginParams, roiRectangle2, roiController);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();

                }
                catch (Exception)
                {
                }
            }
            // if (PluginParams.ImageSourceIndex == 0)
            // {
            //     // ho_Image1 = new HImage("E:/异形极耳工艺/1234/1Q/2.jpeg");

            //     // ho_Image1 = new HImage(@"D:\2.项目文档\HMX宽幅清洗机\极片凹槽测试\手动测试图片/1.bmp");
            //     ho_Image1 = new HImage(@"D:\2.项目文档\HMX宽幅清洗机\极片凹槽测试\手动测试图片/head.bmp");
            // }
            // else
            // {
            //     ho_Image1 = (HImage)proc.GetOutPutParamByName(PluginParams.ImageSourceModule, PluginParams.ImageSourceName);
            // }
            // if (ho_Image1==null)
            // {
            //     ho_Image1 = new HImage("E:/异形极耳工艺/1234/1Q/2.jpeg");
            // }
            // PluginParams.ho_Image = ho_Image1;
            //// pluginsObj.SetParams(PluginParams);
            // hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);
            // SetRoiParams(PluginParams, roiRectangle2, roiController);
            // hWndCtrl_Display.resetAll();
            // hWndCtrl_Display.repaint();

            // ho_Image1.Dispose();
        }

        //===============ROI位置设置===========

        private void NUD_RoiNum_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiPointsNum = Convert.ToInt32(NUD_RoiNum.Value);
            // SetTrakBarParams(Convert.ToInt32(NUD_RoiNum.Value));
            SetRoiParams(PluginParams, roiRectangle2, roiController);

        }

        private void NUD_RoiWidth_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiWidth = Convert.ToInt32(NUD_RoiWidth.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController);
        }

        private void NUD_RoiHeight_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiHeight = Convert.ToInt32(NUD_RoiHeight.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController);
        }

        private void NUD_RoiAngle_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiAngle = Convert.ToInt32(NUD_RoiAngle.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController);
        }

        private void NUD_RoiMidRow_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiMidRow = Convert.ToInt32(NUD_RoiMidRow.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController);
        }

        private void NUD_RoiMidColumn_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiMidColumn = Convert.ToInt32(NUD_RoiMidColumn.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController);
        }
        ///=================抓边设置============
        private void NUD_MeasureThreshold_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_MeasureThreshold = Convert.ToInt32(NUD_MeasureThreshold.Value);
            bl_ShowObj = true;
            // bl_ShowChart = true;
        }

        private void NUD_MeasureSigma_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_MeasureSigma = Convert.ToDouble(NUD_MeasureSigma.Value);
            bl_ShowObj = true;
            //bl_ShowChart = true;
        }


        private void CB_MeasureTransition_SelectedIndexChanged(object sender, EventArgs e)
        {
            // return;
            if (PluginParams == null) return;
            switch (CB_MeasureTransition.SelectedIndex)
            {
                case 0:
                    PluginParams.hv_MeasureTransition = "negative";
                    PluginParams.hv_HistoryMeasureTransition = "negative";
                    break;
                case 1:
                    PluginParams.hv_MeasureTransition = "positive";
                    PluginParams.hv_HistoryMeasureTransition = "positive";
                    break;
                case 2:
                    PluginParams.hv_MeasureTransition = "all";
                    PluginParams.hv_HistoryMeasureTransition = "all";
                    break;
                default:
                    break;
            }
            bl_ShowObj = true;

            //bl_ShowChart = true;
        }

        private void CB_MeasureSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // return;
            if (PluginParams == null) return;
            switch (CB_MeasureSelect.SelectedIndex)
            {
                case 0:
                    PluginParams.hv_MeasureSelect = "first";
                    break;
                case 1:
                    PluginParams.hv_MeasureSelect = "last";
                    break;
                case 2:
                    PluginParams.hv_MeasureSelect = "all";
                    break;
                default:
                    break;

            }
            bl_ShowObj = true;
        }
        private void CB_PeakValleySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // return;
            if (PluginParams == null) return;
            switch (CB_PeakValleySelect.SelectedIndex)
            {
                case 0:
                    PluginParams.hv_PeakValleySelect = "None";
                    break;
                case 1:
                    PluginParams.hv_PeakValleySelect = "peak";
                    break;
                case 2:
                    PluginParams.hv_PeakValleySelect = "valley";
                    break;
                case 3:
                    PluginParams.hv_PeakValleySelect = "RejectionAverage";
                    // CB_PeakValleySelect.Text = "剔值平均";
                    break;
                default:
                    break;
            }
            bl_ShowObj = true;
        }
        private void NUD_MeasureMinScore_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_MeasureMinScore = Convert.ToDouble(NUD_MeasureMinScore.Value);
            bl_ShowObj = true;
        }

        private void NUD_MeasureNumInstances_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_MeasureNumInstances = Convert.ToInt32(NUD_MeasureNumInstances.Value);
            bl_ShowObj = true;
        }

        private void NUD_MeasureDistThreshold_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_MeasureDistThreshold = Convert.ToInt32(NUD_MeasureDistThreshold.Value);
            bl_ShowObj = true;
        }

        private void NUD_MaxNumIterations_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_MaxNumIterations = Convert.ToInt32(NUD_MaxNumIterations.Value);
            bl_ShowObj = true;
        }

        private void CB_MeasureIORegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            // return;
            if (PluginParams == null) return;
            switch (CB_MeasureIORegions.SelectedIndex)
            {
                case 0:
                    PluginParams.hv_MeasureIORegions = "true";
                    break;
                case 1:
                    PluginParams.hv_MeasureIORegions = "false";
                    break;
                default:
                    break;
            }
            bl_ShowObj = true;
        }

        private void CB_MeasureInterpolation_SelectedIndexChanged(object sender, EventArgs e)
        {
            // return;
            if (PluginParams == null) return;
            switch (CB_MeasureInterpolation.SelectedIndex)
            {
                case 0:
                    PluginParams.hv_MeasureInterpolation = "nearest_neighbor";
                    break;
                case 1:
                    PluginParams.hv_MeasureInterpolation = "bilinear";
                    break;
                case 2:
                    PluginParams.hv_MeasureInterpolation = "bicubic";
                    break;
                default:
                    break;
            }
            bl_ShowObj = true;
        }

        private void RB_ImageProcess_CheckedChanged(object sender, EventArgs e)
        {
            PluginParams.bl_ImageProcessIndex = false;
            NUD_ScaleMean.Enabled = false;
            PB_SelectColor.Enabled = false;
            NUD_ScaleFactor.Enabled = false;
            NUD_SharpenIterations.Enabled = false;
            NUD_SharpenSigma.Enabled = false;

            if (RB_ScaleSharpen.Checked == true)
            {
                PluginParams.bl_ImageProcessIndex = true;
                NUD_ScaleMean.Enabled = true;
                PB_SelectColor.Enabled = true;
                NUD_ScaleFactor.Enabled = true;
                NUD_SharpenIterations.Enabled = true;
                NUD_SharpenSigma.Enabled = true;
            }
            bl_ShowObj = true;
            //bl_ShowChart = true;
            //hWndCtrl_Display.repaint();

        }


        private void NUD_ScaleMean_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_ScaleMean = Convert.ToInt32(NUD_ScaleMean.Value);
            bl_ShowObj = true;
            //bl_ShowChart = true;
        }

        private void NUD_ScaleFactor_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_ScaleFactor = Convert.ToDouble(NUD_ScaleFactor.Value);
            bl_ShowObj = true;
            //bl_ShowChart = true;
        }

        private void NUD_SharpenIterations_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_SharpenIterations = Convert.ToInt32(NUD_SharpenIterations.Value);
            bl_ShowObj = true;
            // bl_ShowChart = true;
        }

        private void NUD_SharpenSigma_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_SharpenSigma = Convert.ToDouble(NUD_SharpenSigma.Value);
            bl_ShowObj = true;
            // bl_ShowChart = true;
        }

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

        private void FullScrean_Click(object sender, EventArgs e)
        {
            //重置显示画面
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
        }

        private void CB_DefaultLine_CheckedChanged(object sender, EventArgs e)
        {
            PluginParams.bl_DefaultLine = CB_DefaultLine.Checked;
            bl_ShowObj = true;
        }

        private void HWC_Image1_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                if (PluginParams.ho_Image.CountObj() != 0)
                {
                    hWndCtrl_Display.bl_RoiOperateFlag = true;
                    //显示原图
                    HImage hImage = new HImage();
                    HobjectToHimage(PluginParams.ho_Image, ref hImage);
                    hWndCtrl_Display.addIconicVar(hImage);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                    LB_IMageSize.Text = ShowIMageSize(hImage);
                    showHwindIndex = 0;
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
                if (PluginParams.ho_ImageProcessed.CountObj() != 0)
                {
                    hWndCtrl_Display.bl_RoiOperateFlag = true;
                    //显示原图
                    HImage hImage = new HImage();
                    HobjectToHimage(PluginParams.ho_ImageProcessed, ref hImage);
                    hWndCtrl_Display.addIconicVar(hImage);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                    LB_IMageSize.Text = ShowIMageSize(hImage);
                    showHwindIndex = 1;
                }
            }
            catch (Exception)
            {


            }
        }
    }
}
