using Frame_Core;
using Frame_Core.pluginParams;
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

namespace Plugin.DetectDefect
{
    public partial class FormDetectDefect : FormPluginBase
    {

        private DetectDefectObj pluginsObj { get; set; }
        private Process proc { get; set; }
        public ParamsDetectDefect PluginParams { get; set; }
        bool bl_RoiExist = false;

        public HObject ho_Rectangle2;
        public HObject ho_SmallRectangle2;

        public HObject ho_Image = null;

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();

        public FormDetectDefect()
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
            pluginsObj = (DetectDefectObj)pluginsObjBase;
            PluginParams = (ParamsDetectDefect)pluginsObj.PluginParams;
            proc = Project.Instance.GetProcessByName(pluginsObj.moduleInfo.ProcessName);
            this.Text = pluginsObjBase.moduleInfo.ModuleName;
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_SmallRectangle2);

            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
            ShowDetectResidueParams(PluginParams);

            if (ho_Image != null)
            {
                if (!pluginsObj.isExistModel)
                {
                    PluginParams.ho_Image = ho_Image;
                }
            }
            try
            {
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_Image, ref hImage);
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
        /// 破损检测模块名称显示
        /// </summary>
        /// <param name="label">Label名称</param>
        /// <param name="str_Text">模块名称</param>
        private void SetLabelText(Label label, string str_Text)
        {
            label.Text = "残留检测：" + str_Text;
        }

        /// <summary>
        /// 加载抓边参数到窗体
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowDetectResidueParams(ParamsDetectDefect DetectResidueParams)
        {
            try
            {
                NUD_RoiNum.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiPointsNum);
                NUD_RoiWidth.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiWidth);
                NUD_RoiHeight.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiHeight);
                NUD_RoiAngle.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiAngle);
                NUD_RoiMidRow.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiMidRow);
                NUD_RoiMidColumn.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiMidColumn);

                switch (DetectResidueParams.str_RoiMoveIndex)
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
                NUD_RoiIncreaseNum.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiIncreaseNum);
                NUD_RoiIncreaseRow.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiIncreaseRow);
                NUD_RoiIncreaseColumn.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiIncreaseColumn);

                NUD_RoiStandardRow.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiStandardRow);
                NUD_RoiStandardColumn.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiStandardColumn);
                NUD_RoiStandardOffsetRow.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiStandardOffsetRow);
                NUD_RoiStandardOffsetColumn.Value = Convert.ToDecimal(DetectResidueParams.hv_RoiStandardOffsetColumn);

                NUD_SelectThresholdUp.Value = Convert.ToDecimal(DetectResidueParams.hv_SelectThresholdUp);
                NUD_SelectThresholdDown.Value = Convert.ToDecimal(DetectResidueParams.hv_SelectThresholdDown);

                NUD_AreaUp.Value = Convert.ToDecimal(DetectResidueParams.hv_ResidueMaxValue);
                NUD_AreaDown.Value = Convert.ToDecimal(DetectResidueParams.hv_ResidueMinValue);


                if (DetectResidueParams.bl_ImageProcessIndex)
                {
                    RB_ScaleSharpen.Checked = true;
                }
                else
                {
                    RB_ProcessNone.Checked = true;
                }

                NUD_ScaleValueR.Value = Convert.ToDecimal(DetectResidueParams.hv_ScaleValueR);
                NUD_ScaleValueG.Value = Convert.ToDecimal(DetectResidueParams.hv_ScaleValueG);
                NUD_ScaleValueB.Value = Convert.ToDecimal(DetectResidueParams.hv_ScaleValueB);
                NUD_ScaleFactor.Value = Convert.ToDecimal(DetectResidueParams.hv_ScaleFactor);
                NUD_SharpenIterations.Value = Convert.ToDecimal(DetectResidueParams.hv_SharpenIterations);
                NUD_SharpenSigma.Value = Convert.ToDecimal(DetectResidueParams.hv_SharpenSigma);

                //输出区域处理
                checkBox_RegionOperate.Checked = DetectResidueParams.IsRegionOperate;
                NUD_OpenRadius.Value = Convert.ToDecimal(DetectResidueParams.hv_OpenRadius);
                NUD_ErosionRadius.Value = Convert.ToDecimal(DetectResidueParams.hv_ErosionRadius);
                NUD_DilationRadius.Value = Convert.ToDecimal(DetectResidueParams.hv_DilationRadius);
                NUD_ErosionRadiusCig.Value = Convert.ToDecimal(DetectResidueParams.hv_ErosionRadiusCof);
                NUD_DilationRadiusCig.Value = Convert.ToDecimal(DetectResidueParams.hv_DilationRadiusCof);

            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="DetectResidueParams">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(ParamsDetectDefect DetectResidueParams, ROIRectangle2 roiRectangle2, ROIController roiController, HWndCtrl hWndCtrl)
        {
            try
            {
                roiRectangle2.PointNum = DetectResidueParams.hv_RoiPointsNum;
                roiRectangle2.ROIName = "";

                roiRectangle2.midR = DetectResidueParams.hv_RoiMidRow;
                roiRectangle2.midC = DetectResidueParams.hv_RoiMidColumn;
                HTuple hv_Phi;
                HOperatorSet.TupleRad(DetectResidueParams.hv_RoiAngle, out hv_Phi);
                roiRectangle2.phi = hv_Phi.D;
                roiRectangle2.length2 = DetectResidueParams.hv_RoiWidth / 2;
                roiRectangle2.length1 = DetectResidueParams.hv_RoiHeight / 2;
                if (DetectResidueParams.ho_Image == null)
                {
                    HOperatorSet.GenEmptyObj(out DetectResidueParams.ho_Image);
                }
                if (DetectResidueParams.ho_Image.CountObj() != 0)
                {
                    HTuple hv_Width;
                    HTuple hv_Height;
                    HOperatorSet.GetImageSize(DetectResidueParams.ho_Image, out hv_Width, out hv_Height);
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
        /// <param name="DetectResidueParams"></param>
        public void GetRoiParams(ROIRectangle2 roiRectangle2, ParamsDetectDefect DetectResidueParams)
        {
            DetectResidueParams.hv_RoiPointsNum = roiRectangle2.PointNum;
            roiRectangle2.ROIName = "";

            DetectResidueParams.hv_RoiMidRow = roiRectangle2.midR;
            DetectResidueParams.hv_RoiMidColumn = roiRectangle2.midC;
            HTuple hv_Phi;
            HTuple hv_Angle;
            HOperatorSet.TupleRad(DetectResidueParams.hv_RoiAngle, out hv_Phi);
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Angle);
            DetectResidueParams.hv_RoiAngle = hv_Angle;
            DetectResidueParams.hv_RoiWidth = roiRectangle2.length2 * 2;
            DetectResidueParams.hv_RoiHeight = roiRectangle2.length1 * 2;

            DetectResidueParams.hv_RoiStandardOffsetRow = DetectResidueParams.hv_RoiMidRow - DetectResidueParams.hv_RoiStandardRow;
            DetectResidueParams.hv_RoiStandardOffsetColumn = DetectResidueParams.hv_RoiMidColumn - DetectResidueParams.hv_RoiStandardColumn;
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

            NUD_RoiStandardOffsetRow.Value = NUD_RoiMidRow.Value - NUD_RoiStandardRow.Value;
            NUD_RoiStandardOffsetColumn.Value = NUD_RoiMidColumn.Value - NUD_RoiStandardColumn.Value;
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

                if (roiRectangle2 != null & PluginParams.ho_Image.CountObj() != 0)
                {
                    pluginsObj.ExecuteObj();
                }
                if (TLP_Image1.BackgroundImage != null)
                {
                    if (PluginParams.ho_Image.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_Image;
                    }
                }
                else if (TLP_Image2.BackgroundImage != null)
                {
                    if (PluginParams.ho_ImageScale.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_ImageScale;
                    }
                }
                else if (TLP_Image3.BackgroundImage != null)
                {
                    if (PluginParams.ho_ImageSharpen.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_ImageSharpen;
                    }
                }
                else if (TLP_Image4.BackgroundImage != null)
                {
                    if (PluginParams.ho_Image.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_Image;
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
        /// <param name="DetectResidueParams"></param>
        /// <param name="hv_HWindowID"></param>
        private void ShowDamageToHWD(ParamsDetectDefect DetectResidueParams, HTuple hv_HWindowID, HTuple hv_size)
        {
            if (roiController.activeROIidx == -1)
            {
                HObject ho_Rectangle2, ho_Point;

                HOperatorSet.GenEmptyObj(out ho_Rectangle2);
                HOperatorSet.GenEmptyObj(out ho_Point);

                //显示缩略图到图形选项中
                if (DetectResidueParams.ho_Image.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(DetectResidueParams.ho_Image, HWC_Image1);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
                }

                if (RB_ProcessNone.Checked)
                {
                    HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
                    HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
                }
                else if (RB_ScaleSharpen.Checked)
                {

                    if (DetectResidueParams.ho_ImageScale.CountObj() != 0)
                    {
                        PaintWindows.DisplayImage(DetectResidueParams.ho_ImageProcessed, HWC_Image2);
                    }
                    else
                    {
                        HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
                    }

                    if (DetectResidueParams.ho_ImageSharpen.CountObj() != 0)
                    {
                        PaintWindows.DisplayImage(DetectResidueParams.ho_ImageSharpen, HWC_Image3);
                    }
                    else
                    {
                        HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
                    }
                }

                if (DetectResidueParams.ho_Image.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(DetectResidueParams.ho_Image, HWC_Image4);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image4.HalconWindow);
                }


                //显示破损检测结果
                if (DetectResidueParams.bl_ModelRunFlag & hWndCtrl_Display.bl_RoiOperateFlag)
                {
                    //HTuple hv_MiniHoleReslut;
                    //DefectDetection.GetRegionCircleNumDia(DetectResidueParams.ho_ResidueRegion, out DetectResidueParams.ho_ResidueCircle, out DetectResidueParams.hv_ResidueNum, out  hv_MiniHoleReslut);
                    //HObject ho_RegionUnion = null;
                    //HTuple hv_Area;
                    //HTuple hv_Row;
                    //HTuple hv_Column;
                    //HOperatorSet.Union1(DetectResidueParams.ho_ResidueRegion, out ho_RegionUnion);
                    //HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                    //ho_RegionUnion.Dispose();

                    LinkLabel_Residue1Num.Text = "  " + DetectResidueParams.hv_ResidueNum1.ToString() + "  ";
                    LinkLabel_Residue2Num.Text = "  " + DetectResidueParams.hv_ResidueNum2.ToString() + "  ";

                    LinkLabel_ResidueNum.Text = "  " + DetectResidueParams.hv_ResidueNum.ToString() + "  ";
                    LinkLabel_ResidueArea.Text = "  " + DetectResidueParams.hv_totalAreaPix.ToString() + "  ";

                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.SetDraw(hv_HWindowID, "margin");
                    //显示屏蔽区
                    HOperatorSet.SetColor(hv_HWindowID, "blue");
                    HOperatorSet.DispObj(DetectResidueParams.ho_ShieldRegion, HWC_Display.HalconWindow);
                    PaintWindows.DisplayRegion(DetectResidueParams.ho_ShieldRegion, DetectResidueParams.ho_Image, HWC_Image4);
                    //显示找到的缺陷
                    HOperatorSet.SetColor(hv_HWindowID, "orange");
                    if (DetectResidueParams.hv_ResidueNum > 0)
                    {
                        HOperatorSet.DispObj(DetectResidueParams.ho_ResidueRegion, HWC_Display.HalconWindow);
                        PaintWindows.DisplayRegion(DetectResidueParams.ho_ResidueRegion, DetectResidueParams.ho_Image, HWC_Image4);
                    }
                }
                HTuple hv_Phi;
                //生成检测ROI
                if (DetectResidueParams.str_RoiMoveIndex == "IncreaseROI")
                {
                    HOperatorSet.TupleRad(DetectResidueParams.hv_RoiAngle, out hv_Phi);
                    HOperatorSet.GenRectangle2(out ho_Rectangle2, DetectResidueParams.hv_RoiMidRow + DetectResidueParams.hv_RoiIncreaseRow * DetectResidueParams.hv_RoiIncreaseNum,
                                                DetectResidueParams.hv_RoiMidColumn + DetectResidueParams.hv_RoiIncreaseColumn * DetectResidueParams.hv_RoiIncreaseNum,
                                                 -hv_Phi, DetectResidueParams.hv_RoiHeight / 2, DetectResidueParams.hv_RoiWidth / 2);
                }
                else
                {
                    HOperatorSet.TupleRad(DetectResidueParams.hv_RoiAngle, out hv_Phi);
                    HOperatorSet.GenRectangle2(out ho_Rectangle2, DetectResidueParams.hv_RoiMidRow, DetectResidueParams.hv_RoiMidColumn,
                                                -hv_Phi, DetectResidueParams.hv_RoiHeight / 2, DetectResidueParams.hv_RoiWidth / 2);
                }

                //显示Roi框
                if (!hWndCtrl_Display.bl_RoiOperateFlag || DetectResidueParams.str_RoiMoveIndex == "IncreaseROI" & roiController.activeROIidx == -1)
                {
                    HOperatorSet.SetColor(hv_HWindowID, "green");
                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.SetDraw(hv_HWindowID, "margin");
                    HOperatorSet.DispObj(ho_Rectangle2, hv_HWindowID);
                }

                if (DetectResidueParams.str_RoiMoveIndex == "StandardROI" & hWndCtrl_Display.bl_RoiOperateFlag)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Point, DetectResidueParams.hv_RoiStandardRow, DetectResidueParams.hv_RoiStandardColumn, 50 * hv_size, 0);
                    HOperatorSet.SetColor(hv_HWindowID, "red");
                    HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                    HOperatorSet.SetDraw(hv_HWindowID, "margin");
                    HOperatorSet.DispObj(ho_Point, hv_HWindowID);
                }


                HOperatorSet.SetColor(HWC_Image4.HalconWindow, "blue");
                HOperatorSet.SetLineWidth(HWC_Image4.HalconWindow, 2);
                HOperatorSet.SetDraw(HWC_Image4.HalconWindow, "margin");
                PaintWindows.DisplayRegion(ho_Rectangle2, DetectResidueParams.ho_Image, HWC_Image4);

                ho_Rectangle2.Dispose();
                ho_Point.Dispose();
            }
        }

        #endregion

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
                    ho_Image = PluginParams.ho_Image;
                    hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);
                    SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();

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
        private void NUD_SelectThresholdUp_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_SelectThresholdUp = Convert.ToInt32(NUD_SelectThresholdUp.Value);
            if (NUD_SelectThresholdDown.Value > NUD_SelectThresholdUp.Value)
            {
                NUD_SelectThresholdDown.Value = NUD_SelectThresholdUp.Value;
            }
            hWndCtrl_Display.repaint();
        }

        private void NUD_SelectThresholdDown_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_SelectThresholdDown = Convert.ToInt32(NUD_SelectThresholdDown.Value);
            if (NUD_SelectThresholdDown.Value > NUD_SelectThresholdUp.Value)
            {
                NUD_SelectThresholdUp.Value = NUD_SelectThresholdDown.Value;
            }
            hWndCtrl_Display.repaint();
        }
        private void NUD_AreaUp_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_ResidueMaxValue = Convert.ToDouble(NUD_AreaUp.Value);
            if (NUD_SelectThresholdDown.Value > NUD_AreaUp.Value)
            {
                NUD_SelectThresholdDown.Value = NUD_AreaUp.Value;
            }
            hWndCtrl_Display.repaint();
        }

        private void NUD_AreaDown_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_ResidueMinValue = Convert.ToDouble(NUD_AreaDown.Value);
            if (NUD_AreaDown.Value > NUD_AreaUp.Value)
            {
                NUD_AreaDown.Value = NUD_AreaUp.Value;
            }
            hWndCtrl_Display.repaint();
        }
        //图像处理选择
        private void RB_ImageProcess_CheckedChanged(object sender, EventArgs e)
        {
            PluginParams.bl_ImageProcessIndex = false;
            LB_ScaleMean.ForeColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            LB_ScaleValueR.ForeColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            LB_ScaleValueG.ForeColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            LB_ScaleValueB.ForeColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            LB_ScaleFactor.ForeColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            LB_SharpenIterations.ForeColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            LB_SharpenSigma.ForeColor = Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            NUD_ScaleValueR.Enabled = false;
            NUD_ScaleValueG.Enabled = false;
            NUD_ScaleValueB.Enabled = false;
            PB_SelectColor.Enabled = false;
            NUD_ScaleFactor.Enabled = false;
            NUD_SharpenIterations.Enabled = false;
            NUD_SharpenSigma.Enabled = false;

            if (RB_ScaleSharpen.Checked == true)
            {
                PluginParams.bl_ImageProcessIndex = true;
                LB_ScaleMean.ForeColor = Color.Black;
                LB_ScaleValueR.ForeColor = Color.Black;
                LB_ScaleValueG.ForeColor = Color.Black;
                LB_ScaleValueB.ForeColor = Color.Black;
                LB_ScaleFactor.ForeColor = Color.Black;
                LB_SharpenIterations.ForeColor = Color.Black;
                LB_SharpenSigma.ForeColor = Color.Black;
                NUD_ScaleValueR.Enabled = true;
                NUD_ScaleValueG.Enabled = true;
                NUD_ScaleValueB.Enabled = true;
                PB_SelectColor.Enabled = true;
                NUD_ScaleFactor.Enabled = true;
                NUD_SharpenIterations.Enabled = true;
                NUD_SharpenSigma.Enabled = true;
            }
            hWndCtrl_Display.repaint();

        }

        private void NUD_ScaleValueR_ValueChanged(object sender, EventArgs e)
        {
            if (PluginParams.bl_ImageProcessIndex)
            {
                PluginParams.hv_ScaleValueR = Convert.ToInt32(NUD_ScaleValueR.Value);

                if (PluginParams.ho_Image.CountObj() != 0)
                {
                    HTuple hv_Channels;
                    HOperatorSet.CountChannels(PluginParams.ho_Image, out hv_Channels);

                    if (hv_Channels == 1)
                    {
                        NUD_ScaleValueG.Value = NUD_ScaleValueR.Value;
                        NUD_ScaleValueB.Value = NUD_ScaleValueR.Value;
                    }

                    PB_SelectColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueR.ToString()))))),
                                                       ((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueG.ToString()))))),
                                                       ((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueB.ToString()))))));
                }
            }
            hWndCtrl_Display.repaint();
        }

        private void NUD_ScaleValueG_ValueChanged(object sender, EventArgs e)
        {
            if (PluginParams.bl_ImageProcessIndex)
            {
                PluginParams.hv_ScaleValueG = Convert.ToInt32(NUD_ScaleValueG.Value);

                if (PluginParams.ho_Image.CountObj() != 0)
                {
                    HTuple hv_Channels;
                    HOperatorSet.CountChannels(PluginParams.ho_Image, out hv_Channels);

                    if (hv_Channels == 1)
                    {
                        NUD_ScaleValueR.Value = NUD_ScaleValueG.Value;
                        NUD_ScaleValueB.Value = NUD_ScaleValueG.Value;
                    }

                    PB_SelectColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueR.ToString()))))),
                                                       ((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueG.ToString()))))),
                                                       ((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueB.ToString()))))));
                }
            }
            hWndCtrl_Display.repaint();
        }

        private void NUD_ScaleValueB_ValueChanged(object sender, EventArgs e)
        {
            if (PluginParams.bl_ImageProcessIndex)
            {
                PluginParams.hv_ScaleValueB = Convert.ToInt32(NUD_ScaleValueB.Value);

                if (PluginParams.ho_Image.CountObj() != 0)
                {
                    HTuple hv_Channels;
                    HOperatorSet.CountChannels(PluginParams.ho_Image, out hv_Channels);

                    if (hv_Channels == 1)
                    {
                        NUD_ScaleValueR.Value = NUD_ScaleValueB.Value;
                        NUD_ScaleValueG.Value = NUD_ScaleValueB.Value;
                    }

                    PB_SelectColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueR.ToString()))))),
                                                       ((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueG.ToString()))))),
                                                       ((int)(((byte)(Convert.ToInt16(PluginParams.hv_ScaleValueB.ToString()))))));
                }
            }
            hWndCtrl_Display.repaint();
        }

        private void NUD_ScaleFactor_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_ScaleFactor = Convert.ToDouble(NUD_ScaleFactor.Value);
            hWndCtrl_Display.repaint();
        }

        private void NUD_SharpenIterations_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_SharpenIterations = Convert.ToInt32(NUD_SharpenIterations.Value);
            hWndCtrl_Display.repaint();
        }

        private void NUD_SharpenSigma_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_SharpenSigma = Convert.ToDouble(NUD_SharpenSigma.Value);
            hWndCtrl_Display.repaint();
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

        private void checkBox_RegionOperate_CheckedChanged(object sender, EventArgs e)
        {
            PluginParams.IsRegionOperate = checkBox_RegionOperate.Checked;
            hWndCtrl_Display.repaint();
        }

        private void NUD_OpenRadius_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_OpenRadius = Convert.ToInt32(NUD_OpenRadius.Value);
            hWndCtrl_Display.repaint();
        }

        private void NUD_ErosionRadius_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_ErosionRadius = Convert.ToInt32(NUD_ErosionRadius.Value);
            hWndCtrl_Display.repaint();
        }

        private void NUD_DilationRadius_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_DilationRadius = Convert.ToInt32(NUD_DilationRadius.Value);
            hWndCtrl_Display.repaint();
        }

        private void Btn_SaveRegion_Click(object sender, EventArgs e)
        {
            try
            {        //默认路径
                string filePath = Project.Instance.paramPathSet.ProcessPath;
                filePath = filePath.Replace("Process", "");
                filePath = filePath + pluginsObj.moduleInfo.ProcessName;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                };

                if (PluginParams.ho_ResidueRegion?.CountObj() != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "hobj(*.hobj)|*.hobj";
                    saveFileDialog.InitialDirectory = filePath;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (Directory.Exists(filePath))
                        {
                            filePath = filePath + "\\Region";
                            HOperatorSet.WriteRegion(PluginParams.ho_ResidueRegion, saveFileDialog.FileName);
                        }
                   
                        string fileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                        PluginParams.m_strModelGroupPath = saveFileDialog.FileName;
                        linkLabel_RegionPath.Text = PluginParams.m_strModelGroupPath;

                        PluginParams.Save();
                        MessageBox.Show("截取模板成功");
                    }
                }
                else
                {
                    MessageBox.Show("截取模板失败-模板为空");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("截取模板失败");
            }

        }

        private void NUD_ErosionRadiusCig_ValueChanged(object sender, EventArgs e)
        {
           
            PluginParams.hv_ErosionRadiusCof = Convert.ToInt32(NUD_ErosionRadiusCig.Value);
        }

        private void NUD_DilationRadiusCig_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.hv_DilationRadiusCof = Convert.ToInt32(NUD_DilationRadiusCig.Value);
        }
    }
}
