using HalconDotNet;
using Plugin.MeasureEar;
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
using YYSD.UICore.Control;
using YYSD.UICore;
using System.Windows.Documents;
using System.Windows.Shapes;
using NPOI.SS.Formula.Functions;
using NPOI.OpenXmlFormats.Vml;
using LogTool;

namespace Frame_Core
{
    public partial class Form_MsaLineWidth : Form
    {
        bool bl_RoiExist = false;

        public HObject ho_Rectangle2;
        public HObject ho_SmallRectangle2;
        DisplayWindowClass displayWindow;
        /// <summary>
        /// 当前显示的结果数据（包含图像、Region）
        /// </summary>
        ShowHalconWindow currentShowHalconWindow = new ShowHalconWindow();

        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage = new ImageSimManage();


        public Param_MsaLineWidth Param_MsaLineWidth;

        Dictionary<CameraEnum, CameraManager> cameraManager;

        FittingLine2 fittingLine = new FittingLine2();

        public Form_MsaLineWidth(Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
            cameraManager = _cameraManager;
            displayWindow = new DisplayWindowClass(HWC_Display, TB_PositionAndGray, FullScrean, "Form_MsaCircle");
         
            displayWindow.m_hWndCtrl_Display_Cam.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
           
            currentShowHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
            currentShowHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
            currentShowHalconWindow.m_lineShowInfos = new List<LineInfo>();
         
          
        }

        //private void ShowlineResult(ParamsMeasureLine measLineParams, HTuple hv_HWindowID, HTuple hv_size)
        //{
        //    try
        //    {
        //        if (displayWindow.m_roiController_Cam.activeROIidx == -1)
        //        {
        //            HObject ho_Cross, ho_RegionLines, ho_Point;
        //            HOperatorSet.GenEmptyObj(out ho_Cross);
        //            HOperatorSet.GenEmptyObj(out ho_RegionLines);
        //            HOperatorSet.GenEmptyObj(out ho_Point);
        //            ho_Rectangle2?.Dispose();
        //            ho_SmallRectangle2?.Dispose();

        //            HTuple hv_Phi;
        //            HTuple hv_Phi1;
                 

        //            //生成测量矩形
        //            if (measLineParams.str_RoiMoveIndex == "IncreaseROI")
        //            {

        //                HOperatorSet.TupleRad(measLineParams.hv_RoiAngle, out hv_Phi);
        //                HOperatorSet.GenRectangle2(out ho_Rectangle2, measLineParams.hv_RoiMidRow + measLineParams.hv_RoiIncreaseRow * measLineParams.hv_RoiIncreaseNum,
        //                                            measLineParams.hv_RoiMidColumn + measLineParams.hv_RoiIncreaseColumn * measLineParams.hv_RoiIncreaseNum,
        //                                             -hv_Phi, measLineParams.hv_RoiHeight / 2, measLineParams.hv_RoiWidth / 2);
        //            }
        //            else
        //            {
        //                HOperatorSet.TupleRad(measLineParams.hv_RoiAngle, out hv_Phi);
        //                HOperatorSet.GenRectangle2(out ho_Rectangle2, measLineParams.hv_RoiMidRow, measLineParams.hv_RoiMidColumn,
        //                                            -hv_Phi, measLineParams.hv_RoiHeight / 2, measLineParams.hv_RoiWidth / 2);
        //            }
        //            //生成小测量矩形
        //            HOperatorSet.TupleRad(-measLineParams.hv_RoiAngle - 90, out hv_Phi1);
        //            HOperatorSet.GenRectangle2(out ho_SmallRectangle2, measLineParams.hv_RoiMidRow + (-measLineParams.hv_RoiHeight / 2 + measLineParams.hv_RoiHeight / 2 / measLineParams.hv_RoiPointsNum +
        //                        measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum * (measLineParams.hv_RoiPointsNum - /*TrB_ROIIndex.Value*/ 0 + 1 - 1)) * Math.Cos(-hv_Phi1),
        //                        measLineParams.hv_RoiMidColumn - (-measLineParams.hv_RoiHeight / 2 + measLineParams.hv_RoiHeight / 2 / measLineParams.hv_RoiPointsNum +
        //                        measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum * (measLineParams.hv_RoiPointsNum - /*TrB_ROIIndex.Value*/ 0 + 1 - 1)) * Math.Sin(-hv_Phi1),
        //                        hv_Phi1, measLineParams.hv_RoiWidth / 2, measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum / 2);

        //            if (measLineParams.hv_CrossRow != null & displayWindow.m_hWndCtrl_Display_Cam.bl_RoiOperateFlag )
        //            {
        //                HOperatorSet.GenCrossContourXld(out ho_Cross, measLineParams.hv_CrossRow, measLineParams.hv_CrossColumn, hv_size * 3, (new HTuple(45)).TupleRad());
        //                HOperatorSet.SetDraw(hv_HWindowID, "fill");
        //                HOperatorSet.SetColor(hv_HWindowID, "green");
        //                HOperatorSet.SetLineWidth(hv_HWindowID, 2);
        //                HOperatorSet.DispObj(ho_Cross, hv_HWindowID);
        //            }

        //            //显示拟合直线及拾取点
        //            if (measLineParams.hv_LineRowBegin != null & displayWindow.m_hWndCtrl_Display_Cam.bl_RoiOperateFlag )
        //            {
        //                HOperatorSet.GenRegionLine(out ho_RegionLines, measLineParams.hv_LineRowBegin, measLineParams.hv_LineColumnBegin,
        //                                                    measLineParams.hv_LineRowEnd, measLineParams.hv_LineColumnEnd);

        //                HOperatorSet.SetLineWidth(hv_HWindowID, 3);
        //                HOperatorSet.SetColor(hv_HWindowID, "yellow");
        //                //HOperatorSet.DispLine(hv_HWindowID, measLineParams.hv_LineRowBegin, measLineParams.hv_LineColumnBegin,
        //                //                                    measLineParams.hv_LineRowEnd, measLineParams.hv_LineColumnEnd);
        //                HOperatorSet.DispObj(ho_RegionLines, hv_HWindowID);
        //            }

        //            //显示测量矩形
        //            if (!displayWindow.m_hWndCtrl_Display_Cam.bl_RoiOperateFlag || measLineParams.str_RoiMoveIndex == "IncreaseROI" & displayWindow.m_roiController_Cam.activeROIidx == -1)
        //            {
        //                HOperatorSet.SetColor(hv_HWindowID, "green");
        //                HOperatorSet.SetLineWidth(hv_HWindowID, 2);
        //                HOperatorSet.SetDraw(hv_HWindowID, "margin");
        //                HOperatorSet.DispObj(ho_Rectangle2, hv_HWindowID);
        //            }

        //            if (measLineParams.str_RoiMoveIndex == "StandardROI" & displayWindow.m_hWndCtrl_Display_Cam.bl_RoiOperateFlag)
        //            {
        //                HOperatorSet.GenCrossContourXld(out ho_Point, measLineParams.hv_RoiStandardRow, measLineParams.hv_RoiStandardColumn, 50 * hv_size, 0);
        //                HOperatorSet.SetColor(hv_HWindowID, "red");
        //                HOperatorSet.SetLineWidth(hv_HWindowID, 2);
        //                HOperatorSet.SetDraw(hv_HWindowID, "margin");
        //                HOperatorSet.DispObj(ho_Point, hv_HWindowID);
        //            }

        //            HOperatorSet.SetColor(hv_HWindowID, "red");
        //            HOperatorSet.SetLineWidth(hv_HWindowID, 1);
        //            HOperatorSet.SetDraw(hv_HWindowID, "margin");
        //            HOperatorSet.DispObj(ho_SmallRectangle2, hv_HWindowID);


        //            HOperatorSet.SetColor(HWC_Image3.HalconWindow, "blue");
        //            HOperatorSet.SetLineWidth(HWC_Image3.HalconWindow, 2);
        //            HOperatorSet.SetDraw(HWC_Image3.HalconWindow, "margin");
        //           // PaintWindows.DisplayRegion(ho_Rectangle2, measLineParams.ho_Image, HWC_Image3);

        //            ho_Cross.Dispose();
        //            ho_RegionLines.Dispose();
        //            ho_Point.Dispose();
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

          
        //}

        private void 抓边ROI方向设置_Click(object sender, EventArgs e)
        {

        }

        private void RunFunction()
        {
            try
            {
                //GetRoiParams(displayWindow.m_roiRectangle2_Cam, Param_MsaLineWidth.paramsMeasureLine);
                //RefreshRoiParams(displayWindow.m_roiRectangle2_Cam);
                //ShowFitLineParams(Param_MsaLineWidth.paramsMeasureLine);
                //displayWindow.m_hWndCtrl_Display_Cam.bl_RoiOperateFlag = true;
                //FitOneLine(Param_MsaLineWidth.paramsMeasureLine);

                //ShowlineResult(Param_MsaLineWidth.paramsMeasureLine, HWC_Display.HalconWindow, displayWindow.m_roiRectangle2_Cam.size);

              
            }
            catch ( Exception eE)
            {

            }
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


            try
            {
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
                        // bl_ShowObj = true;
                    }
                }
            }
            catch (Exception ee)
            {

            }
        }

        /// <summary>
        /// 获取单条抓边ROI参数并显示
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




        Dictionary<int, string> dicCobInputXorY = new Dictionary<int, string>();
        Dictionary<int, string> dicCobDirection = new Dictionary<int, string>();
        Dictionary<int, string> dicCobinputTransition = new Dictionary<int, string>();
        Dictionary<int, string> dicCobinputPosition = new Dictionary<int, string>();
        private void Forrm_MsaLineWidth_Load(object sender, EventArgs e)
        {
            dicCobInputXorY.Add(0, "X");
            dicCobInputXorY.Add(1, "Y");
            dicCobDirection.Add(0, "L2R_New");
            dicCobDirection.Add(1, "U2B_New");
            dicCobinputTransition.Add(0, "negative");
            dicCobinputTransition.Add(1, "positive");
            dicCobinputTransition.Add(2, "all");
            dicCobinputPosition.Add(0, "first");
            dicCobinputPosition.Add(1, "last");
            dicCobinputPosition.Add(2, "all");
            

            intCobCam.Items.Clear();
            foreach (var item in cameraManager)
            {
                intCobCam.Items.Add(item.Value.m_CamParam.CameraName.ToString());
            }

            intCobCam.SelectedIndex = 0;
            iCobInputXorY.Items.Clear();
            foreach (var item in dicCobInputXorY)
            {
                iCobInputXorY.Items.Add(item.Value);
            }
            iCobInputXorY.SelectedIndex=Param_MsaLineWidth.iCobInputXorY;


            iCobDirection.Items.Clear();
            foreach (var item in dicCobDirection)
            {
                iCobDirection.Items.Add(item.Value);
            }
            iCobDirection.SelectedIndex = Param_MsaLineWidth.iCobDirection;
        
            iCobinputTransition.Items.Clear();
            foreach (var item in dicCobinputTransition)
            {
                iCobinputTransition.Items.Add(item.Value);
            }
            iCobinputTransition.SelectedIndex = Param_MsaLineWidth.iCobinputTransition;
         
            iCobinputPosition.Items.Clear();
            foreach (var item in dicCobinputPosition)
            {
                iCobinputPosition.Items.Add(item.Value);
            }
            iCobinputPosition.SelectedIndex = Param_MsaLineWidth.iCobinputPosition;

            linkLabel1.Text = Project.Instance.paramPathSet.ProductMojuanPath;
        }

        private void intCobCam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Param_MsaLineWidth = cameraManager[(CameraEnum)intCobCam.SelectedIndex].param_MsaLineWidth;
                YControlAutobind.Loading<Param_MsaLineWidth, IYCustomAttribute>(this, Param_MsaLineWidth);//测试存修改记录
               
                //SetRoiParams(Param_MsaLineWidth.paramsMeasureLine, displayWindow.m_roiRectangle2_Cam, displayWindow.m_roiController_Cam);
                //ShowFitLineParams(Param_MsaLineWidth.paramsMeasureLine);
            }   
            catch (Exception ee)
            {

            }
          
        }


        public bool FitOneLine(ParamsMeasureLine measLineParams)
        {
            HTuple hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2, hv_RoiPhi;
            HTuple hv_RoiMidRow = measLineParams.hv_RoiMidRow;
            HTuple hv_RoiMidColumn = measLineParams.hv_RoiMidColumn;
            //    HObject ResultLine;
            // HOperatorSet.GenEmptyObj(out ResultLine);
            // HObject ho_Rectangle2, ho_ImagePart, ho_ImagePartProcessed;
            // HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            //  HOperatorSet.GenEmptyObj(out ho_ImagePart);
            // HOperatorSet.GenEmptyObj(out ho_ImagePartProcessed);

            HOperatorSet.GenEmptyObj(out measLineParams.ho_ImageProcessed);
            measLineParams.hv_MetrologyHandle = null;
            measLineParams.hv_Index = null;
            measLineParams.hv_CrossRow = null;
            measLineParams.hv_CrossColumn = null;
            measLineParams.hv_LineRowBegin = null;
            measLineParams.hv_LineColumnBegin = null;
            measLineParams.hv_LineRowEnd = null;
            measLineParams.hv_LineColumnEnd = null;
            measLineParams.bl_FitSuccess = false;

            HOperatorSet.TupleRad(measLineParams.hv_RoiAngle, out hv_RoiPhi);

            hv_MeasureRow1 = hv_RoiMidRow - measLineParams.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
            hv_MeasureColumn1 = hv_RoiMidColumn - measLineParams.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;
            hv_MeasureRow2 = hv_RoiMidRow + measLineParams.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
            hv_MeasureColumn2 = hv_RoiMidColumn + measLineParams.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;

            //抓边算法
            if (measLineParams.hv_RoiPointsNum > 1 & measLineParams.ho_Image.CountObj() != 0)
            {
                fittingLine.CreateLineModel(hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2, measLineParams.hv_RoiWidth / 2,
                         measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum / 2, measLineParams.hv_MeasureSigma, measLineParams.hv_MeasureThreshold, out measLineParams.hv_MetrologyHandle, out measLineParams.hv_Index);
                fittingLine.FindLine(measLineParams.ho_Image, measLineParams.hv_MetrologyHandle, measLineParams.hv_Index, measLineParams.hv_MeasureNumInstances, measLineParams.hv_MeasureDistThreshold, measLineParams.hv_MeasureTransition, measLineParams.hv_MeasureSelect,
                     measLineParams.hv_RoiPointsNum, measLineParams.hv_MeasureMinScore, measLineParams.hv_MaxNumIterations, measLineParams.hv_MeasureInterpolation, measLineParams.hv_MeasureIORegions, out measLineParams.hv_CrossRow,
                    out measLineParams.hv_CrossColumn, out measLineParams.hv_LineRowBegin, out measLineParams.hv_LineColumnBegin, out measLineParams.hv_LineRowEnd, out measLineParams.hv_LineColumnEnd);

                if (measLineParams.hv_LineRowBegin >= 0)
                {
                    measLineParams.bl_FitSuccess = true;
                }
                else
                {
                    measLineParams.bl_FitSuccess = false;
                }
            }
            else
            {
                measLineParams.bl_FitSuccess = false;
            }
            return measLineParams.bl_FitSuccess;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string error = "";
            //视觉设置
            Param_MsaLineWidth.backup(Param_MsaLineWidth);
            Param_MsaLineWidth.FilePath = Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\";
            error = YControlAutobind.Saveing<Param_MsaLineWidth, IYCustomAttribute>(this, Param_MsaLineWidth);
            Param_MsaLineWidth.saveChangeLog(Param_MsaLineWidth, String.Format("测试视觉设置:{0} 中 {1}", "MSA圆点", $"{(CameraEnum)intCobCam.SelectedIndex}MSA圆点参数"));
            if (error != "") System.Windows.Forms.MessageBox.Show(error);

            Param_MsaLineWidth.Save();
        }

        private void LoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Multiselect = true;

            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                m_ImageSimManage.InitSimFlag();
                m_ImageSimManage.ImageEmulationPaths = FileDialog.FileNames;

                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image, out string imgPath);

                Execute(Load_Image);

            }
        }

        private void Execute(HObject image, bool flag = false)
        {
            try
            {
                if (image == null) return;
                currentShowHalconWindow.Dispose();
                currentShowHalconWindow.m_hImage = image;
                if (!flag)
                {
                    displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                    screenshotImage?.Dispose();
                }
                else
                {
                 
                    HOperatorSet.GenRectangle1(out HObject rectangle, Param_MsaLineWidth.fRoiStartRow, Param_MsaLineWidth.fRoiStartCol, Param_MsaLineWidth.fRoiEndRow, Param_MsaLineWidth.fRoiEndCol);
                    currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });
                    HObject ho_outputContour1;
                    try
                    {
                        //*从亮到暗，从下到上

                        Find_Line_ForRectangle(image, rectangle, out  ho_outputContour1, dicCobInputXorY[Param_MsaLineWidth.iCobInputXorY],
                            Param_MsaLineWidth.iInputInterval, dicCobDirection[Param_MsaLineWidth.iCobDirection], Param_MsaLineWidth.fInputRoiWidthLen, Param_MsaLineWidth.fInputSigma, Param_MsaLineWidth.iInputAmplitudeThreshold, dicCobinputTransition[Param_MsaLineWidth.iCobinputTransition], dicCobinputPosition[Param_MsaLineWidth.iCobinputPosition], out HTuple hv_OutPointRow1,
                            out HTuple hv_OutPointCol1);
                        currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_outputContour1, m_stuGetColor = HalconColor.yellow });
                        HOperatorSet.GenCrossContourXld(out HObject ho_Cross1, hv_OutPointRow1, hv_OutPointCol1, 5, (new HTuple(45)).TupleRad());
                        currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross1, m_stuGetColor = HalconColor.blue });
                      


                    }
                    catch (Exception ee)
                    {

                        MessageBox.Show("未找到边缘！" + ee.Message);
                        return;
                    }

                    try
                    {
                        HOperatorSet.FitLineContourXld(ho_outputContour1, "tukey", -1, 0, 5, 2, out HTuple hv_RowBegin1,
                     out HTuple hv_ColBegin1, out HTuple hv_RowEnd1, out HTuple hv_ColEnd1, out HTuple hv_Nr, out HTuple hv_Nc,
                    out HTuple hv_Dist);

                        //获取直线中点
                        HTuple hv_RowCenter = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowCenter = (hv_RowEnd1 + hv_RowBegin1) / 2;
                        }
                        HTuple hv_ColCenter = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ColCenter = (hv_ColBegin1 + hv_ColEnd1) / 2;
                        }
                        HTuple hv_Phi = 0;
                        HOperatorSet.AngleLx(hv_RowBegin1, hv_ColBegin1, hv_RowEnd1, hv_ColEnd1,
                            out hv_Phi);
                        //2.垂线长度  根据实际情况调整

                        HTuple hv_LineLength1 = Param_MsaLineWidth.fToMidStartPointLength;

                        HTuple hv_LineLength2 = Param_MsaLineWidth.fToMidEndPointLength;
                        //起点
                        HTuple hv_RowStart = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowStart = hv_RowCenter - ((hv_Phi.TupleCos()
                                ) * hv_LineLength1);
                        }
                        HTuple hv_ColStart = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ColStart = hv_ColCenter - ((hv_Phi.TupleSin()
                                ) * hv_LineLength1);
                        }
                        //终点
                        HTuple hv_RowEnd = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowEnd = hv_RowCenter + ((hv_Phi.TupleCos()
                                ) * hv_LineLength2);
                        }
                        HTuple hv_ColEnd = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ColEnd = hv_ColCenter + ((hv_Phi.TupleSin()
                                ) * hv_LineLength2);
                        }

                        HOperatorSet.GenRegionLine(out HObject ho_Line1, hv_RowStart, hv_ColStart, hv_RowEnd,
                            hv_ColEnd);



                        HTuple hv_AmplitudeThreshold = Param_MsaLineWidth.iDetectmplitudeThreshold;
                        HTuple hv_RoiWidthLen2 = Param_MsaLineWidth.fDetectRoIWidth;


                        //Measure 01: Coordinates for line Measure 01 [0]

                        HTuple hv_LineRowStart_Measure_01_0 = new HTuple(hv_RowStart);

                        HTuple hv_LineColumnStart_Measure_01_0 = new HTuple(hv_ColStart);

                        HTuple hv_LineRowEnd_Measure_01_0 = new HTuple(hv_RowEnd);

                        HTuple hv_LineColumnEnd_Measure_01_0 = new HTuple(hv_ColEnd);
                        //Measure 01: Convert coordinates to rectangle2 type
                        HTuple hv_TmpCtrl_Row = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TmpCtrl_Row = 0.5 * (hv_LineRowStart_Measure_01_0 + hv_LineRowEnd_Measure_01_0);
                        }
                        HTuple hv_TmpCtrl_Column = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TmpCtrl_Column = 0.5 * (hv_LineColumnStart_Measure_01_0 + hv_LineColumnEnd_Measure_01_0);
                        }
                        HTuple hv_TmpCtrl_Dr = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TmpCtrl_Dr = hv_LineRowStart_Measure_01_0 - hv_LineRowEnd_Measure_01_0;
                        }
                        HTuple hv_TmpCtrl_Dc = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TmpCtrl_Dc = hv_LineColumnEnd_Measure_01_0 - hv_LineColumnStart_Measure_01_0;
                        }
                        HTuple hv_TmpCtrl_Phi = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(
                                hv_TmpCtrl_Dc);
                        }
                        HTuple hv_TmpCtrl_Len1 = 0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_TmpCtrl_Len1 = 0.5 * ((((hv_TmpCtrl_Dr * hv_TmpCtrl_Dr) + (hv_TmpCtrl_Dc * hv_TmpCtrl_Dc))).TupleSqrt()
                                );
                        }

                        HTuple hv_TmpCtrl_Len2 = Param_MsaLineWidth.fDetectRoIWidth;
                        //Measure 01: Create measure for line Measure 01 [0]
                        //Measure 01: Attention: This assumes all images have the same size!
                        HOperatorSet.GetImageSize(image, out HTuple hv_Width, out HTuple hv_Height);
                        HOperatorSet.GenMeasureRectangle2(hv_TmpCtrl_Row, hv_TmpCtrl_Column, hv_TmpCtrl_Phi,
                            hv_TmpCtrl_Len1, hv_TmpCtrl_Len2, hv_Width, hv_Height, "nearest_neighbor", out HTuple hv_MsrHandle_Measure_01_0);

                        HOperatorSet.MeasurePairs(image, hv_MsrHandle_Measure_01_0, 0.9, hv_AmplitudeThreshold,
                          "all", "all", out HTuple hv_Row1_Measure_01_0, out HTuple hv_Column1_Measure_01_0,
                                 out HTuple hv_Amplitude1_Measure_01_0, out HTuple hv_Row2_Measure_01_0, out HTuple hv_Column2_Measure_01_0,
                         out HTuple hv_Amplitude2_Measure_01_0, out HTuple hv_Width_Measure_01_0, out HTuple hv_Distance_Measure_01_0);


                        currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Line1, m_stuGetColor = HalconColor.green });

                        HOperatorSet.GenCrossContourXld(out HObject ho_Cross1, hv_Row1_Measure_01_0, hv_Column1_Measure_01_0,
                        60, 0);
                        ;
                        currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross1, m_stuGetColor = HalconColor.yellow });

                        HOperatorSet.GenCrossContourXld(out HObject ho_Cross2, hv_Row2_Measure_01_0, hv_Column2_Measure_01_0,
                            60, 0);

                        currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross2, m_stuGetColor = HalconColor.orange });
                        HTuple hv_width=0;
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_width = hv_Width_Measure_01_0 * Param_MsaLineWidth.fScaleY;
                        }

                        if(hv_width?.Length>0)
                        {
                            List<string> stringTitles = new List<string>();
                            List<string> stringData = new List<string>();
                            for (int i=0;i<hv_width.Length;i++)
                            {
                                currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"Y线宽{i}:{hv_width[i].D.ToString("0.000")}mm", m_stuGetColor = HalconColor.green });
                                stringTitles.Add($"Y线宽{i}");
                                stringData.Add(hv_width[i].D.ToString("0.000"));
                            }
                            stringTitles.Insert(0, "序号");
                            stringData.Insert(0, m_ImageSimManage.currentImageIndex.ToString());
                            currentShowHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = $"ID: {m_ImageSimManage.currentImageIndex.ToString()}", m_stuGetColor = HalconColor.green });
                            FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + intCobCam.Text + "MSA线宽数据", stringTitles.ToArray(), stringData.ToArray(), "");
                        }
                        else
                        {
                            MessageBox.Show("未找到线宽！");
                        }
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show("未找到线宽！\n"+ee.StackTrace);
                    }
                   
                    displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                    screenshotImage?.Dispose();
                }
               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


     public void Find_Line_ForRectangle(HObject ho_inputImage, HObject ho_inputRegion,
     out HObject ho_outputContour, HTuple hv_inputXorY, HTuple hv_inputInterval,
    HTuple hv_inputDirection, HTuple hv_inputRoiWidthLen, HTuple hv_inputSigma,
    HTuple hv_inputAmplitudeThreshold, HTuple hv_inputTransition, HTuple hv_inputPosition,
    out HTuple hv_OutPointRow, out HTuple hv_OutPointCol)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ProcessContour, ho_Rectangle1 = null;
            HObject ho_Cross1 = null, ho_Cross3 = null, ho_Cross2 = null;
            HObject ho_ResultContour = null, ho_Cross = null;

            // Local control variables 

            HTuple hv_Row11 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row21 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_r2 = new HTuple(), hv_c2 = new HTuple(), hv_r4 = new HTuple();
            HTuple hv_c4 = new HTuple(), hv_Row1 = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Col2 = new HTuple();
            HTuple hv_LineRowStart_Measure_01_0 = new HTuple(), hv_LineColumnStart_Measure_01_0 = new HTuple();
            HTuple hv_LineRowEnd_Measure_01_0 = new HTuple(), hv_LineColumnEnd_Measure_01_0 = new HTuple();
            HTuple hv_MeasureLength1 = new HTuple(), hv_r1 = new HTuple();
            HTuple hv_c1 = new HTuple(), hv_r3 = new HTuple(), hv_c3 = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_MetrologyHandle = new HTuple(), hv_Index1 = new HTuple();
            HTuple hv_LineParameter = new HTuple(), hv_DebugMode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_outputContour);
            HOperatorSet.GenEmptyObj(out ho_ProcessContour);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Cross1);
            HOperatorSet.GenEmptyObj(out ho_Cross3);
            HOperatorSet.GenEmptyObj(out ho_Cross2);
            HOperatorSet.GenEmptyObj(out ho_ResultContour);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            hv_OutPointRow = new HTuple();
            hv_OutPointCol = new HTuple();
            try
            {

                //inputXorY:沿Y轴搜索,或者X轴
                //inputInterval:间隔
                //inputDirection:方向 L2R_New从左到右,U2B_New从上到下
                //inputRoiWidthLen:卡尺ROI长度
                //inputSigma:平滑系数
                //inputAmplitudeThreshold:阈值
                //inputTransition:所以’negative‘从亮到暗，‘positive’从暗到亮，‘all’。
                //inputPosition:'first'第一点,'last'最后一个,'all' 所有的边缘


                hv_OutPointRow.Dispose();
                hv_OutPointRow = new HTuple();
                hv_OutPointCol.Dispose();
                hv_OutPointCol = new HTuple();
                ho_ProcessContour.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ProcessContour);

                //*****************************************二维卡尺方法**************************
                if ((int)((new HTuple((new HTuple((new HTuple(hv_inputDirection.TupleEqual(
                    "L2R_New"))).TupleOr(new HTuple(hv_inputDirection.TupleEqual("R2L_New"))))).TupleOr(
                    new HTuple(hv_inputDirection.TupleEqual("U2B_New"))))).TupleOr(new HTuple(hv_inputDirection.TupleEqual(
                    "B2U_New")))) != 0)
                {
                    hv_Row11.Dispose(); hv_Column1.Dispose(); hv_Row21.Dispose(); hv_Column2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_inputRegion, out hv_Row11, out hv_Column1,
                        out hv_Row21, out hv_Column2);
                    hv_Length1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length1 = (((hv_Column2 - hv_Column1)).TupleAbs()
                            ) / 2;
                    }
                    hv_Length2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length2 = (((hv_Row21 - hv_Row11)).TupleAbs()
                            ) / 2;
                    }
                    hv_Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row = (hv_Row21 + hv_Row11) / 2;
                    }
                    hv_Column.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column = (hv_Column2 + hv_Column1) / 2;
                    }
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, 0, hv_Length1,
                        hv_Length2);
                    //an := rad(Phi)
                    if ((int)(new HTuple(hv_inputXorY.TupleEqual("Y"))) != 0)
                    {
                        hv_r2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_r2 = hv_Row - hv_Length2;
                        }
                        hv_c2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_c2 = hv_Column - hv_Length1;
                        }
                        hv_r4.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_r4 = hv_Row + hv_Length2;
                        }
                        hv_c4.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_c4 = hv_Column + hv_Length1;
                        }
                        ho_Cross1.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_r2, hv_c2, 100, 0);
                        ho_Cross3.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_r4, hv_c4, 100, 0);
                        hv_Row1.Dispose();
                        hv_Row1 = new HTuple(hv_r2);
                        hv_Col1.Dispose();
                        hv_Col1 = new HTuple(hv_c2);
                        hv_Row2.Dispose();
                        hv_Row2 = new HTuple(hv_r4);
                        hv_Col2.Dispose();
                        hv_Col2 = new HTuple(hv_c4);

                        if ((int)(new HTuple(hv_inputDirection.TupleEqual("L2R_New"))) != 0)
                        {
                            hv_LineRowStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowStart_Measure_01_0 = hv_Row2 + (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineColumnStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnStart_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                            }
                            hv_LineRowEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowEnd_Measure_01_0 = hv_Row1 - (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineColumnEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnEnd_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                            }
                        }
                        else if ((int)(new HTuple(hv_inputDirection.TupleEqual("R2L_New"))) != 0)
                        {
                            hv_LineRowStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowStart_Measure_01_0 = hv_Row1 - (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineColumnStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnStart_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                            }
                            hv_LineRowEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowEnd_Measure_01_0 = hv_Row2 + (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineColumnEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnEnd_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                            }
                        }
                        hv_MeasureLength1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_MeasureLength1 = ((hv_Col2 - hv_Col1) / 2) - 2;
                        }
                    }
                    else if ((int)(new HTuple(hv_inputXorY.TupleEqual("X"))) != 0)
                    {
                        hv_r1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_r1 = hv_Row - hv_Length2;
                        }
                        hv_c1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_c1 = hv_Column - hv_Length1;
                        }
                        ho_Cross2.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_r1, hv_c1, 100, 0);
                        hv_r3.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_r3 = hv_Row + hv_Length2;
                        }
                        hv_c3.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_c3 = hv_Column + hv_Length1;
                        }
                        ho_Cross1.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_r1, hv_c1, 100, 0);
                        ho_Cross3.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_r3, hv_c3, 100, 0);
                        //an := deg(Phi)
                        hv_Row1.Dispose();
                        hv_Row1 = new HTuple(hv_r1);
                        hv_Col1.Dispose();
                        hv_Col1 = new HTuple(hv_c1);
                        hv_Row2.Dispose();
                        hv_Row2 = new HTuple(hv_r3);
                        hv_Col2.Dispose();
                        hv_Col2 = new HTuple(hv_c3);
                        //*         endif
                        if ((int)(new HTuple(hv_inputDirection.TupleEqual("U2B_New"))) != 0)
                        {
                            hv_LineRowStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowStart_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                            }
                            hv_LineColumnStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnStart_Measure_01_0 = hv_Col1 - (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineRowEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowEnd_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                            }
                            hv_LineColumnEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnEnd_Measure_01_0 = hv_Col2 + (hv_inputRoiWidthLen / 2);
                            }
                        }
                        else if ((int)(new HTuple(hv_inputDirection.TupleEqual("B2U_New"))) != 0)
                        {
                            hv_LineRowStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowStart_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                            }
                            hv_LineColumnStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnStart_Measure_01_0 = hv_Col2 + (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineRowEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowEnd_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                            }
                            hv_LineColumnEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnEnd_Measure_01_0 = hv_Col1 - (hv_inputRoiWidthLen / 2);
                            }
                        }
                        hv_MeasureLength1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_MeasureLength1 = (hv_Row2 - hv_Row1) / 2;
                        }
                    }


                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_inputImage, out hv_Width, out hv_Height);
                    //Create the metrology model data structure
                    hv_MetrologyHandle.Dispose();
                    HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                    //The image size is set in advance to speed up the
                    //first call of apply_metrology_model.
                    HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                    //Define the parameters of the metrology line objects
                    //Create One metrology line objects and set parameters
                    //gen_contour_polygon_xld (Contour1, [LineRowStart_Measure_01_0,LineRowEnd_Measure_01_0], [LineColumnStart_Measure_01_0, LineColumnEnd_Measure_01_0])
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Index1.Dispose();
                        HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_LineRowStart_Measure_01_0,
                            hv_LineColumnStart_Measure_01_0, hv_LineRowEnd_Measure_01_0, hv_LineColumnEnd_Measure_01_0,
                            hv_MeasureLength1, hv_inputRoiWidthLen / 2, hv_inputSigma, hv_inputAmplitudeThreshold,
                            (((new HTuple("measure_distance")).TupleConcat("measure_transition")).TupleConcat(
                            "measure_select")).TupleConcat("min_score"), ((((hv_inputInterval.TupleConcat(
                            hv_inputTransition))).TupleConcat(hv_inputPosition))).TupleConcat(0.3),
                            out hv_Index1);
                    }

                    //set_metrology_object_param (MetrologyHandle, 'all', 'min_score', 0.3)

                    //Create region of interest for the alignment
                    HOperatorSet.ApplyMetrologyModel(ho_inputImage, hv_MetrologyHandle);
                    hv_LineParameter.Dispose();
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                        "all_param", out hv_LineParameter);
                    ho_outputContour.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_outputContour, hv_MetrologyHandle,
                        "all", "all", 1.5);
                    hv_DebugMode.Dispose();
                    hv_DebugMode = 1;
                    if ((int)(hv_DebugMode) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_outputContour, ho_ProcessContour, out ExpTmpOutVar_0
                                );
                            ho_ProcessContour.Dispose();
                            ho_ProcessContour = ExpTmpOutVar_0;
                        }
                        ho_ResultContour.Dispose(); hv_OutPointRow.Dispose(); hv_OutPointCol.Dispose();
                        HOperatorSet.GetMetrologyObjectMeasures(out ho_ResultContour, hv_MetrologyHandle,
                            "all", "all", out hv_OutPointRow, out hv_OutPointCol);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultContour, ho_ProcessContour, out ExpTmpOutVar_0
                                );
                            ho_ProcessContour.Dispose();
                            ho_ProcessContour = ExpTmpOutVar_0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Cross.Dispose();
                            HOperatorSet.GenCrossContourXld(out ho_Cross, hv_OutPointRow, hv_OutPointCol,
                                5, (new HTuple(45)).TupleRad());
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_Cross, ho_ProcessContour, out ExpTmpOutVar_0);
                            ho_ProcessContour.Dispose();
                            ho_ProcessContour = ExpTmpOutVar_0;
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_ProcessContour, HDevWindowStack.GetActive());
                        }
                    }

                    HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                    ho_ProcessContour.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Cross1.Dispose();
                    ho_Cross3.Dispose();
                    ho_Cross2.Dispose();
                    ho_ResultContour.Dispose();
                    ho_Cross.Dispose();

                    hv_Row11.Dispose();
                    hv_Column1.Dispose();
                    hv_Row21.Dispose();
                    hv_Column2.Dispose();
                    hv_Length1.Dispose();
                    hv_Length2.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_r2.Dispose();
                    hv_c2.Dispose();
                    hv_r4.Dispose();
                    hv_c4.Dispose();
                    hv_Row1.Dispose();
                    hv_Col1.Dispose();
                    hv_Row2.Dispose();
                    hv_Col2.Dispose();
                    hv_LineRowStart_Measure_01_0.Dispose();
                    hv_LineColumnStart_Measure_01_0.Dispose();
                    hv_LineRowEnd_Measure_01_0.Dispose();
                    hv_LineColumnEnd_Measure_01_0.Dispose();
                    hv_MeasureLength1.Dispose();
                    hv_r1.Dispose();
                    hv_c1.Dispose();
                    hv_r3.Dispose();
                    hv_c3.Dispose();
                    hv_Width.Dispose();
                    hv_Height.Dispose();
                    hv_MetrologyHandle.Dispose();
                    hv_Index1.Dispose();
                    hv_LineParameter.Dispose();
                    hv_DebugMode.Dispose();

                    return;
                }
                ho_ProcessContour.Dispose();
                ho_Rectangle1.Dispose();
                ho_Cross1.Dispose();
                ho_Cross3.Dispose();
                ho_Cross2.Dispose();
                ho_ResultContour.Dispose();
                ho_Cross.Dispose();

                hv_Row11.Dispose();
                hv_Column1.Dispose();
                hv_Row21.Dispose();
                hv_Column2.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_r2.Dispose();
                hv_c2.Dispose();
                hv_r4.Dispose();
                hv_c4.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_LineRowStart_Measure_01_0.Dispose();
                hv_LineColumnStart_Measure_01_0.Dispose();
                hv_LineRowEnd_Measure_01_0.Dispose();
                hv_LineColumnEnd_Measure_01_0.Dispose();
                hv_MeasureLength1.Dispose();
                hv_r1.Dispose();
                hv_c1.Dispose();
                hv_r3.Dispose();
                hv_c3.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index1.Dispose();
                hv_LineParameter.Dispose();
                hv_DebugMode.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ProcessContour.Dispose();
                ho_Rectangle1.Dispose();
                ho_Cross1.Dispose();
                ho_Cross3.Dispose();
                ho_Cross2.Dispose();
                ho_ResultContour.Dispose();
                ho_Cross.Dispose();

                hv_Row11.Dispose();
                hv_Column1.Dispose();
                hv_Row21.Dispose();
                hv_Column2.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_r2.Dispose();
                hv_c2.Dispose();
                hv_r4.Dispose();
                hv_c4.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_LineRowStart_Measure_01_0.Dispose();
                hv_LineColumnStart_Measure_01_0.Dispose();
                hv_LineRowEnd_Measure_01_0.Dispose();
                hv_LineColumnEnd_Measure_01_0.Dispose();
                hv_MeasureLength1.Dispose();
                hv_r1.Dispose();
                hv_c1.Dispose();
                hv_r3.Dispose();
                hv_c3.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index1.Dispose();
                hv_LineParameter.Dispose();
                hv_DebugMode.Dispose();

                throw HDevExpDefaultException;
            }
        }

        private void ROISET_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) {  MessageBox.Show("请先加载图像！"); return; } ;
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image, out string imgPath);
                // 释放之前的数据              
                currentShowHalconWindow.Dispose();
                currentShowHalconWindow.m_hImage = Load_Image;

                HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                HOperatorSet.DrawRectangle1(HWC_Display.HalconWindow,out HTuple rowStart, out HTuple columnStart, out HTuple rowEnd, out HTuple columnEnd);
                HOperatorSet.GenRectangle1(out HObject rectangle, rowStart, columnStart, rowEnd, columnEnd);
              
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });
                fRoiStartRow.Value = Convert.ToDecimal(rowStart.D);
                fRoiStartCol.Value = Convert.ToDecimal(columnStart.D);
                fRoiEndRow.Value = Convert.ToDecimal(rowEnd.D);
                fRoiEndCol.Value = Convert.ToDecimal(columnEnd.D);
              

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void EdgeFind_Click(object sender, EventArgs e)
        {
            if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) { MessageBox.Show("请先加载图像！"); return; };
            m_ImageSimManage.GetCurrentImage(out HObject Load_Image, out string imgPath);
            // 释放之前的数据              
            currentShowHalconWindow.Dispose();
            currentShowHalconWindow.m_hImage = Load_Image;
            HOperatorSet.GenRectangle1(out HObject rectangle, Param_MsaLineWidth.fRoiStartRow, Param_MsaLineWidth.fRoiStartCol, Param_MsaLineWidth.fRoiEndRow, Param_MsaLineWidth.fRoiEndCol);
            currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });
            try
            {
                //*从亮到暗，从下到上
               
                Find_Line_ForRectangle(Load_Image, rectangle, out HObject ho_outputContour1, dicCobInputXorY[Param_MsaLineWidth.iCobInputXorY],
                    Param_MsaLineWidth.iInputInterval, dicCobDirection[Param_MsaLineWidth.iCobDirection], Param_MsaLineWidth.fInputRoiWidthLen, Param_MsaLineWidth.fInputSigma, Param_MsaLineWidth.iInputAmplitudeThreshold, dicCobinputTransition[Param_MsaLineWidth.iCobinputTransition], dicCobinputPosition[Param_MsaLineWidth.iCobinputPosition], out HTuple hv_OutPointRow1,
                    out HTuple hv_OutPointCol1);
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_outputContour1, m_stuGetColor = HalconColor.yellow });
                HOperatorSet.GenCrossContourXld(out HObject ho_Cross1, hv_OutPointRow1, hv_OutPointCol1, 5, (new HTuple(45)).TupleRad());
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross1, m_stuGetColor = HalconColor.blue });
                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                screenshotImage?.Dispose();

            }
            catch (Exception ee)
            {

                MessageBox.Show("未找到边缘！"+ee.Message);
            }
               
        }


        public void genLengthenLine(out HObject ho_inputContour, HTuple hv_inputImageWidth,
            HTuple hv_inputImageHeight, HTuple hv_inputRowBegin, HTuple hv_inputColBegin,
            HTuple hv_inputRowEnd, HTuple hv_inputColEnd)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_k = new HTuple(), hv_b = new HTuple();
            HTuple hv_row1 = new HTuple(), hv_row2 = new HTuple();
            HTuple hv_col1 = new HTuple(), hv_col2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_inputContour);
            try
            {
                //特殊情况水平线
                //y=kx+b
                if ((int)(new HTuple(((hv_inputRowEnd - hv_inputRowBegin)).TupleEqual(0))) != 0)
                {
                    hv_k.Dispose();
                    hv_k = 0;
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_inputContour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_inputContour, hv_inputRowEnd.TupleConcat(
                            hv_inputRowEnd), (new HTuple(0)).TupleConcat(hv_inputImageWidth));
                    }
                }
                else
                {
                    hv_k.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_k = (hv_inputColEnd - hv_inputColBegin) / (hv_inputRowEnd - hv_inputRowBegin);
                    }
                    hv_b.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_b = hv_inputColEnd - (hv_k * hv_inputRowEnd);
                    }
                    hv_row1.Dispose();
                    hv_row1 = 0;
                    hv_row2.Dispose();
                    hv_row2 = new HTuple(hv_inputImageHeight);
                    hv_col1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_col1 = (hv_k * hv_row1) + hv_b;
                    }
                    hv_col2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_col2 = (hv_k * hv_row2) + hv_b;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_inputContour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out ho_inputContour, hv_row1.TupleConcat(
                            hv_row2), hv_col1.TupleConcat(hv_col2));
                    }
                }

                hv_k.Dispose();
                hv_b.Dispose();
                hv_row1.Dispose();
                hv_row2.Dispose();
                hv_col1.Dispose();
                hv_col2.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_k.Dispose();
                hv_b.Dispose();
                hv_row1.Dispose();
                hv_row2.Dispose();
                hv_col1.Dispose();
                hv_col2.Dispose();

                throw HDevExpDefaultException;
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) { MessageBox.Show("请先加载图像！"); return; };
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image, out string imgPath);
                //GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, true);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
           
            try
            {
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) { MessageBox.Show("请先加载图像！"); return; };
                m_ImageSimManage.GetNextImage(out HObject Load_Image, out string imgPath);
                //GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, true);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        private void Form_MsaLineWidth_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
             
                displayWindow?.Dispose();
                currentShowHalconWindow?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Project.Instance.paramPathSet.ProductMojuanPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
