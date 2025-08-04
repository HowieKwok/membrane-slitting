using Frame_Core;
using HalconAlgorithm;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI;
using YYSD.UICore.Control;
using YYSD.UICore;

namespace Plugin.MeasureEar
{
    public partial class FormMeasureEar : FormPluginBase
    {
        private MeasEarObj pluginsObj { get; set; }
        private ParamsMeasureEar PluginParams { get; set; }
        private ParamsMeasureLine measLineParams { get; set; }

        bool bl_ShowObj = false;
        bool bl_RoiExist = false;
        public HObject ho_Rectangle2;
        public HObject ho_SmallRectangle2;
        public int ShowShrinkIImageIndex = 0;


        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();

        public HObject ho_Image1 = null;
        public FormMeasureEar()
        {
            InitializeComponent();


            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果

            InitListView(listView1);
        }

        public override void Init()
        {

            pluginsObj = (MeasEarObj)pluginsObjBase;
            PluginParams = (ParamsMeasureEar)pluginsObj.PluginParams;

            PluginParams.backup(PluginParams);
            YControlAutobind.Loading<ParamsMeasureEar, IYCustomAttribute>(this, PluginParams);//测试存修改记录

            //measLineParams = PluginParams.array_MeasureLineParam[(int)Params.JRLine];
            measLineParams = PluginParams.array_MeasureLineParam[(int)PluginParams.JRLine];
            this.Text = pluginsObjBase.moduleInfo.ModuleName;
            CB_SelectLine.DataSource = Enum.GetNames(typeof(JRLineEnum));

            LoadStandImage();
            ShowParamsToView(PluginParams);
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

            PluginParams.FilePath = (Application.StartupPath + "\\record\\参数修改记录\\视觉设置\\");
            error = YControlAutobind.Saveing<PluginParamsBase, IYCustomAttribute>(this, PluginParams);
            // 调用父窗体
            base.Btn_Save_Click(sender, e);
            PluginParams.saveChangeLog(PluginParams, String.Format("测试视觉设置:{0} 中 {1}", pluginsObj.moduleInfo.ProcessName, pluginsObj.moduleInfo.ModuleName));
            PluginParams.backup(PluginParams);
            if (error != "") System.Windows.Forms.MessageBox.Show(error);

        }

        private void FormMeasureEar_Load(object sender, EventArgs e)
        {

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
                    PluginParams.ho_Image = ho_Image1;
                }
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_Image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
            }
            catch (Exception ee)
            {

            }
            //SetRoiParams(PluginParams, roiRectangle2, roiController);
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
            //ShowFitLineParams(PluginParams);
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
        /// 初始化ListView的方法
        /// </summary>
        /// <param name="lv"></param>
        public void InitListView(ListView lv)
        {
            lv.Clear();
            //添加列名
            ColumnHeader c1 = new ColumnHeader();
            c1.Width = 100;
            c1.Text = "编号";
            ColumnHeader c2 = new ColumnHeader();
            c2.Width = 150;
            c2.Text = "检测项";
            ColumnHeader c3 = new ColumnHeader();
            c3.Width = 150;
            c3.Text = "测量值";
            //把列名添加到listview中
            lv.Columns.Add(c1);
            lv.Columns.Add(c2);
            lv.Columns.Add(c3);
            // lv.Columns.Add("测量值", 100);  //相当于上面的添加列名的步骤

            //设置属性
            lv.GridLines = true;  //显示网格线
            lv.FullRowSelect = true;  //显示全行
            lv.MultiSelect = false;  //设置只能单选
            lv.View = View.Details;  //设置显示模式为详细
            lv.HoverSelection = true;  //当鼠标停留数秒后自动选择

        }

        private void btn_autoRun_Click(object sender, EventArgs e)
        {
            RefreshParamsFromView(PluginParams);
            RefreshLineParamsFromView(measLineParams);
            if (roiRectangle2 != null & PluginParams.ho_Image.CountObj() != 0)
            {
                pluginsObj.ExecuteObj();
            }
            measLineParams.ho_Image = PluginParams.ho_JRImage;
            SetRoiParams(measLineParams, roiRectangle2, roiController);
            ShowShrinkImageResult(PluginParams);
            ShowResult();
            // ShowLineToHWD(measLineParams, HWC_Display.HalconWindow, roiRectangle2.size);
            if (!PluginParams.IsFitOneLine)
            {
                ShowItemsToListView(PluginParams, listView1);
            }
        }

        /// <summary>
        /// ViewRoi回调函数：执行检测方法
        /// </summary>
        private void RunFunction()
        {
            try
            {
                //if (ShowShrinkIImageIndex == 2)
                //{
                //    if (roiController.activeROIidx == -1)
                //    {
                //        if (roiRectangle2 != null && PluginParams.ho_JRImage.CountObj() != 0)
                //        {
                //            hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_JRImage;
                //            cB_IsFitOneLine.Checked = true;
                //            RefreshLineParamsFromView(measLineParams);
                //            pluginsObj.ExecuteObj();
                //        }
                //    }
                //}
            }
            catch { }
        }

        /// <summary>
        /// ViewRoi回调函数：显示检测结果
        /// </summary>
        private void ShowResult()
        {
            if (ShowShrinkIImageIndex == 1)//窗体1显示原图，和裁切区域
            {
                ShowCropRectToHWD(PluginParams, HWC_Display.HalconWindow);
            }
            else if (ShowShrinkIImageIndex == 2)//窗体2显示裁图，和检测极耳抓边
            {
                if (roiController.activeROIidx == -1)
                {
                    if (PluginParams.IsFitOneLine)
                    {
                        bl_ShowObj = false;
                        hWndCtrl_Display.bl_RoiOperateFlag = true; //禁用ROI操作
                        if (roiRectangle2 != null)
                        {
                            GetRoiParams(roiRectangle2, measLineParams);
                            RefreshRoiParams(roiRectangle2);
                        }
                        ShowLineToHWD(measLineParams, HWC_Display.HalconWindow, roiRectangle2.size);
                    }
                    else
                    {
                        hWndCtrl_Display.bl_RoiOperateFlag = false; //禁用ROI操作
                        ShowResultToHWD(PluginParams, HWC_Display.HalconWindow);
                        ShowMarkToHWD(PluginParams, HWC_Display.HalconWindow);
                    }
                }
            }
            else if (ShowShrinkIImageIndex == 3)//窗体3显示裁图，和检测极耳R角
            {
                ShowCircleToHWD(PluginParams, HWC_Display.HalconWindow);
            }


        }
        /// <summary>
        /// 在 原图 中显示：裁切位置
        /// </summary>
        /// <param name="EarParams"></param>
        /// <param name="hv_HWindowID"></param>
        private void ShowCropRectToHWD(ParamsMeasureEar EarParams, HTuple hv_HWindowID)
        {
            HObject ho_RegionCrop;
            HOperatorSet.GenEmptyObj(out ho_RegionCrop);
            HTuple w, h;
            HOperatorSet.GetImageSize(EarParams.ho_Image, out w, out h);

            HOperatorSet.GenRectangle1(out ho_RegionCrop, 0, EarParams.JRCenterCol - EarParams.JRShaft_Left, h - 1, EarParams.JRCenterCol + EarParams.JRShaft_Left - 1);

            HOperatorSet.SetColor(HWC_Display.HalconWindow, "orange");
            HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 2);
            HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
            HOperatorSet.DispObj(ho_RegionCrop, HWC_Display.HalconWindow);

            ho_RegionCrop.Dispose();
        }

        /// <summary>
        /// 在 裁切图 中显示：所有抓边和计算交点
        /// </summary>
        /// <param name="EarParams"></param>
        /// <param name="hv_HWindowID"></param>
        private void ShowResultToHWD(ParamsMeasureEar EarParams, HTuple hv_HWindowID)
        {
            HObject ho_RegionLines, ho_OneLine, ho_RegionRects, ho_OneRect, ho_Points, ho_OnePoint;

            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_OneLine);
            HOperatorSet.GenEmptyObj(out ho_RegionRects);
            HOperatorSet.GenEmptyObj(out ho_OneRect);
            HOperatorSet.GenEmptyObj(out ho_Points);
            HOperatorSet.GenEmptyObj(out ho_OnePoint);
            HTuple hv_Phi;

            foreach (ParamsMeasureLine item in EarParams.array_MeasureLineParam)
            {
                //矩形框
                HOperatorSet.TupleRad(item.hv_RoiAngle, out hv_Phi);
                HOperatorSet.GenRectangle2(out ho_OneRect, item.hv_RoiMidRow, item.hv_RoiMidColumn,
                                            -hv_Phi, item.hv_RoiHeight / 2, item.hv_RoiWidth / 2);
                HOperatorSet.ConcatObj(ho_OneRect, ho_RegionRects, out ho_RegionRects);
                //抓边
                if (item.bl_FitSuccess)
                {
                    HOperatorSet.GenRegionLine(out ho_OneLine, item.hv_LineRowBegin, item.hv_LineColumnBegin,
                                    item.hv_LineRowEnd, item.hv_LineColumnEnd);
                    HOperatorSet.ConcatObj(ho_OneLine, ho_RegionLines, out ho_RegionLines);
                }
            }
            //显示矩形框
            HOperatorSet.SetLineWidth(hv_HWindowID, 2);
            HOperatorSet.SetColor(hv_HWindowID, "blue");
            HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
            HOperatorSet.DispObj(ho_RegionRects, hv_HWindowID);
            //显示抓边
            HOperatorSet.SetLineWidth(hv_HWindowID, 3);
            HOperatorSet.SetColor(hv_HWindowID, "yellow");
            HOperatorSet.DispObj(ho_RegionLines, hv_HWindowID);

            HOperatorSet.GenCrossContourXld(out ho_OnePoint, EarParams.hv_PointRow0, EarParams.hv_PointCol0, 45, (new HTuple(45)).TupleRad());
            HOperatorSet.ConcatObj(ho_OnePoint, ho_Points, out ho_Points);
            HOperatorSet.GenCrossContourXld(out ho_OnePoint, EarParams.hv_PointRow1, EarParams.hv_PointCol1, 45, (new HTuple(45)).TupleRad());
            HOperatorSet.ConcatObj(ho_OnePoint, ho_Points, out ho_Points);
            HOperatorSet.GenCrossContourXld(out ho_OnePoint, EarParams.hv_PointRow2, EarParams.hv_PointCol2, 45, (new HTuple(45)).TupleRad());
            HOperatorSet.ConcatObj(ho_OnePoint, ho_Points, out ho_Points);
            HOperatorSet.GenCrossContourXld(out ho_OnePoint, EarParams.hv_PointRow3, EarParams.hv_PointCol3, 45, (new HTuple(45)).TupleRad());
            HOperatorSet.ConcatObj(ho_OnePoint, ho_Points, out ho_Points);

            HOperatorSet.SetColor(hv_HWindowID, "green");
            HOperatorSet.SetLineWidth(hv_HWindowID, 2);
            HOperatorSet.DispObj(ho_Points, hv_HWindowID);

            ho_RegionLines.Dispose();
            ho_OneLine.Dispose();
            ho_RegionRects.Dispose();
            ho_OneRect.Dispose();
            ho_Points.Dispose();
            ho_OnePoint.Dispose();

        }
        private void ShowCircleToHWD(ParamsMeasureEar EarParams, HTuple hv_HWindowID)
        {

            double XYRatio = EarParams.Pix_X / EarParams.Pix_Y;
            HObject ReduceImage1, ReduceImage2;
            HObject ho_RegionCircle1, ho_RegionCircle2;
            //HObject ho_OnePoint1, ho_OnePoint2;
            HOperatorSet.GenEmptyObj(out ReduceImage1);
            HOperatorSet.GenEmptyObj(out ReduceImage2);
            HOperatorSet.GenEmptyObj(out ho_RegionCircle1);
            HOperatorSet.GenEmptyObj(out ho_RegionCircle2);
            //HOperatorSet.GenEmptyObj(out ho_OnePoint1);
            //HOperatorSet.GenEmptyObj(out ho_OnePoint2);

            try
            {

            }
            catch (Exception ee)
            {

            }
            //拟合极耳R1角矩形。
            HTuple row0, col0, row1, col1;
            row0 = EarParams.hv_PointRow0 * XYRatio - EarParams.AngleShaftY;
            row1 = EarParams.hv_PointRow0 * XYRatio + EarParams.AngleShaftY * 0.5;
            col0 = EarParams.hv_PointCol0 - EarParams.AngleShaftX * 0.5;
            col1 = EarParams.hv_PointCol0 + EarParams.AngleShaftX;
            HOperatorSet.GenRectangle1(out ReduceImage1, row0, col0, row1, col1);
            //拟合极耳R2角矩形。
            row0 = col0 = row1 = col1 = null;
            row0 = EarParams.hv_PointRow1 * XYRatio - EarParams.AngleShaftY * 0.5;
            row1 = EarParams.hv_PointRow1 * XYRatio + EarParams.AngleShaftY;
            col0 = EarParams.hv_PointCol1 - EarParams.AngleShaftX * 0.5;
            col1 = EarParams.hv_PointCol1 + EarParams.AngleShaftX;
            HOperatorSet.GenRectangle1(out ReduceImage2, row0, col0, row1, col1);
            //显示R角矩形区域
            HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
            HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 2);
            HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
            HOperatorSet.DispObj(ReduceImage1, hv_HWindowID);
            HOperatorSet.DispObj(ReduceImage2, hv_HWindowID);
            //显示R角分割XLD
            HOperatorSet.SetColor(HWC_Display.HalconWindow, "green");
            HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 3);
            HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
            HOperatorSet.DispObj(EarParams.ho_R1_XLD, hv_HWindowID);
            HOperatorSet.DispObj(EarParams.ho_R2_XLD, hv_HWindowID);

            //显示拟合R角圆
            HOperatorSet.GenCircle(out ho_RegionCircle1, EarParams.R1Row, EarParams.R1Col, EarParams.R1Radius);
            HOperatorSet.GenCircle(out ho_RegionCircle2, EarParams.R2Row, EarParams.R2Col, EarParams.R2Radius);
            //HOperatorSet.GenCrossContourXld(out ho_OnePoint1, EarParams.R1Row, EarParams.R1Col, 25, (new HTuple(45)).TupleRad());
            //HOperatorSet.GenCrossContourXld(out ho_OnePoint2, EarParams.R2Row, EarParams.R2Col, 25, (new HTuple(45)).TupleRad());

            HOperatorSet.SetColor(HWC_Display.HalconWindow, "orange");
            HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 2);
            HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
            HOperatorSet.DispObj(ho_RegionCircle1, hv_HWindowID);
            HOperatorSet.DispObj(ho_RegionCircle2, hv_HWindowID);
            //HOperatorSet.DispObj(ho_OnePoint1, hv_HWindowID);
            //HOperatorSet.DispObj(ho_OnePoint2, hv_HWindowID);

            ReduceImage1.Dispose();
            ReduceImage2.Dispose();
            ho_RegionCircle1.Dispose();
            ho_RegionCircle2.Dispose();
            //ho_OnePoint1.Dispose();
            //ho_OnePoint2.Dispose();
        }
        private void ShowMarkToHWD(ParamsMeasureEar EarParams, HTuple hv_HWindowID)
        {
            try
            {
                HOperatorSet.SetColor(HWC_Display.HalconWindow, "magenta");
                HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 3);
                HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                HOperatorSet.DispObj(EarParams.ho_MarkregionRect1, hv_HWindowID);
                HOperatorSet.DispObj(EarParams.ho_MarkregionRect2, hv_HWindowID);
               
                HOperatorSet.SetColor(HWC_Display.HalconWindow, "orange");
                HOperatorSet.SetLineWidth(HWC_Display.HalconWindow, 3);
                HOperatorSet.SetDraw(HWC_Display.HalconWindow, "fill");
                HOperatorSet.DispObj(EarParams.ho_Markregion1  , hv_HWindowID);
                HOperatorSet.DispObj(EarParams.ho_Markregion2, hv_HWindowID);

            }
            catch (Exception ee)
            {

            }
        }

        /// <summary>
        /// 显示直线到窗体上
        /// </summary>
        /// <param name="measLineParams"></param>
        /// <param name="hv_HWindowID"></param>
        private void ShowLineToHWD(ParamsMeasureLine measLineParams, HTuple hv_HWindowID, HTuple hv_size)
        {
            SendMessage(TLPanel_BGM1.Handle, WM_SETREDRAW, 0, IntPtr.Zero);//开始重绘
            if (roiController.activeROIidx == -1)
            {
                HObject ho_Cross, ho_RegionLines, ho_Point;
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_RegionLines);
                HOperatorSet.GenEmptyObj(out ho_Point);
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
                            measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum * (measLineParams.hv_RoiPointsNum - /*TrB_ROIIndex.Value*/ 0 + 1 - 1)) * Math.Cos(-hv_Phi1),
                            measLineParams.hv_RoiMidColumn - (-measLineParams.hv_RoiHeight / 2 + measLineParams.hv_RoiHeight / 2 / measLineParams.hv_RoiPointsNum +
                            measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum * (measLineParams.hv_RoiPointsNum - /*TrB_ROIIndex.Value*/ 0 + 1 - 1)) * Math.Sin(-hv_Phi1),
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
            SendMessage(TLPanel_BGM1.Handle, WM_SETREDRAW, 1, IntPtr.Zero);//开始重绘
            TLPanel_BGM1.Refresh();

        }

        private void ShowItemsToListView(ParamsMeasureEar EarParams, ListView lv)
        {
            InitListView(lv);
            lv.BeginUpdate();
            int i = 0;
            foreach (SizeEnum item in EarParams.dic_SizeItem.Keys)
            {
                //创建行对象
                ListViewItem li = new ListViewItem(i.ToString());
                //添加同一行的数据
                li.SubItems.Add(item.ToString());
                li.SubItems.Add(EarParams.dic_SizeItem[item].ToString("f3"));
                //将行对象绑定在listview对象中
                lv.Items.Add(li);
                i++;
            }
            lv.EndUpdate();
        }



        /// <summary>
        /// 加载所有参数到窗体，基础参数和抓边参数
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowParamsToView(ParamsMeasureEar Params)
        {
            try
            {
                //复选框
                CB_SelectLine.SelectedIndex = this.CB_SelectLine.FindString(Params.JRLine.ToString());

                //勾选框
                IsBackImage.Checked = Params.IsBackImage;
                cB_IsCathode.Checked = Params.IsCathode;
                cB_IsFitOneLine.Checked = Params.IsFitOneLine;

                //基础nud数据
                //NUD_JRCenterX.Value = Convert.ToDecimal(Params.Pix_X);
                //NUD_JRCenterY.Value = Convert.ToDecimal(Params.Pix_Y);
                JRCenterCol.Value = Convert.ToDecimal(Params.JRCenterCol);
                NUD_JRCenterRow.Value = Convert.ToDecimal(Params.JRCenterRow);
                NUD_JRCropLeft.Value = Convert.ToDecimal(Params.JRShaft_Left);
                NUD_JRCropRight.Value = Convert.ToDecimal(Params.JRShaft_Right);
                NUD_AngleShaftX.Value = Convert.ToDecimal(Params.AngleShaftX);
                NUD_AngleShaftY.Value = Convert.ToDecimal(Params.AngleShaftY);
                //mark测量
                cB_EnableMark1.Checked = Params.Mark1Enable;
                cB_EnableMark2.Checked = Params.Mark2Enable;
                NUD_MarkGrayMin.Value = Convert.ToDecimal(Params.MarkGrayMin);
                NUD_MarkGrayMax.Value = Convert.ToDecimal(Params.MarkGrayMax);
                NUD_Mark1ShaftRow.Value = Convert.ToDecimal(Params.Mark1ShaftRow);
                NUD_Mark2ShaftRow.Value = Convert.ToDecimal(Params.Mark2ShaftRow);
                NUD_MarkRegionX.Value = Convert.ToDecimal(Params.MarkRegionX);
                NUD_MarkRegionY.Value = Convert.ToDecimal(Params.MarkRegionY);

                //获取当前边,显示当前抓边参数
                measLineParams = Params.array_MeasureLineParam[(int)Params.JRLine];
                ShowFitLineParams(measLineParams);

            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// 加载抓边参数到窗体
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowFitLineParams(ParamsMeasureLine measLineParams)
        {
            try
            {
                //ROI位置参数
                NUD_RoiNum.Value = Convert.ToDecimal(measLineParams.hv_RoiPointsNum);
                NUD_RoiWidth.Value = Convert.ToDecimal(measLineParams.hv_RoiWidth);
                NUD_RoiHeight.Value = Convert.ToDecimal(measLineParams.hv_RoiHeight);
                NUD_RoiAngle.Value = Convert.ToDecimal(measLineParams.hv_RoiAngle);
                hv_RoiMidRow.Value = Convert.ToDecimal(measLineParams.hv_RoiMidRow);
                NUD_RoiMidColumn.Value = Convert.ToDecimal(measLineParams.hv_RoiMidColumn);

                //抓边参数
                NUD_MeasureThreshold.Value = Convert.ToDecimal(measLineParams.hv_MeasureThreshold);
                NUD_MeasureSigma.Value = Convert.ToDecimal(measLineParams.hv_MeasureSigma);
                switch (measLineParams.hv_MeasureTransition)
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
                switch (measLineParams.hv_MeasureSelect)
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
                //高级抓边参数
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
                    case "bilinear":
                        CB_MeasureInterpolation.Text = "双线性插值法";
                        break;
                    case "bicubic":
                        CB_MeasureInterpolation.Text = "双三次插值法";
                        break;
                    case "nearest_neighbor":
                        CB_MeasureInterpolation.Text = "最近邻域法";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {

            }

        }
        /// <summary>
        /// 从界面获取刷新基础极耳参数
        /// </summary>
        /// <param name="Params"></param>
        private void RefreshParamsFromView(ParamsMeasureEar Params)
        {
            //复选框
            Params.JRLine = (JRLineEnum)Enum.Parse(typeof(JRLineEnum), CB_SelectLine.SelectedItem.ToString(), false);
            //勾选框
            Params.IsBackImage = IsBackImage.Checked;
            Params.IsCathode = cB_IsCathode.Checked;
            Params.IsFitOneLine = cB_IsFitOneLine.Checked;
            //基础nud数据
            Params.JRCenterCol = (double)JRCenterCol.Value;
            Params.JRCenterRow = (double)NUD_JRCenterRow.Value;
            Params.JRShaft_Left = (double)NUD_JRCropLeft.Value;
            Params.JRShaft_Right = (double)NUD_JRCropRight.Value;
            Params.AngleShaftX = (double)NUD_AngleShaftX.Value;
            Params.AngleShaftY = (double)NUD_AngleShaftY.Value;
            //mark测量
            Params.Mark1Enable = cB_EnableMark1.Checked;
            Params.Mark2Enable = cB_EnableMark2.Checked;
            Params.MarkGrayMin = (int)NUD_MarkGrayMin.Value;
            Params.MarkGrayMax = (int)NUD_MarkGrayMax.Value;
            Params.Mark1ShaftRow = (double)NUD_Mark1ShaftRow.Value;
            Params.Mark2ShaftRow = (double)NUD_Mark2ShaftRow.Value;
            Params.MarkRegionX = (int)NUD_MarkRegionX.Value;
            Params.MarkRegionY = (int)NUD_MarkRegionY.Value;

            measLineParams = Params.array_MeasureLineParam[(int)Params.JRLine];

        }

        /// <summary>
        /// 从界面获取刷新单条抓边参数
        /// </summary>
        /// <param name="Params"></param>
        private void RefreshLineParamsFromView(ParamsMeasureLine Params)
        {
            //ROI位置参数
            measLineParams.hv_RoiPointsNum = Convert.ToInt32(NUD_RoiNum.Value);
            measLineParams.hv_RoiWidth = Convert.ToDouble(NUD_RoiWidth.Value);
            measLineParams.hv_RoiHeight = Convert.ToDouble(NUD_RoiHeight.Value);
            measLineParams.hv_RoiAngle = Convert.ToDouble(NUD_RoiAngle.Value);
            measLineParams.hv_RoiMidRow = Convert.ToDouble(hv_RoiMidRow.Value);
            measLineParams.hv_RoiMidColumn = Convert.ToDouble(NUD_RoiMidColumn.Value);

            //抓边参数
            Params.hv_MeasureThreshold = Convert.ToInt32(NUD_MeasureThreshold.Value);
            Params.hv_MeasureSigma = Convert.ToDouble(NUD_MeasureSigma.Value);
            switch (CB_MeasureTransition.Text)
            {
                case "从明到暗":
                    Params.hv_MeasureTransition = "negative";
                    break;
                case "从暗到明":
                    Params.hv_MeasureTransition = "positive";
                    break;
                case "不区分明暗":
                    Params.hv_MeasureTransition = "all";
                    break;
                default:
                    break;
            }
            switch (CB_MeasureSelect.Text)
            {
                case "第一条边":
                    Params.hv_MeasureSelect = "first";
                    break;
                case "最后一条边":
                    Params.hv_MeasureSelect = "last";
                    break;
                case "最强边":
                    Params.hv_MeasureSelect = "all";
                    break;
                default:
                    break;
            }

            //高级抓边参数
            Params.hv_MeasureMinScore = Convert.ToDouble(NUD_MeasureMinScore.Value);
            Params.hv_MeasureNumInstances = Convert.ToInt32(NUD_MeasureNumInstances.Value);
            Params.hv_MeasureDistThreshold = Convert.ToInt32(NUD_MeasureDistThreshold.Value);
            Params.hv_MaxNumIterations = Convert.ToInt32(NUD_MaxNumIterations.Value);
            switch (CB_MeasureIORegions.Text)
            {
                case "是":
                    Params.hv_MeasureIORegions = "true";
                    break;
                case "否":
                    Params.hv_MeasureIORegions = "false";
                    break;
                default:
                    break;
            }
            switch (CB_MeasureInterpolation.Text)
            {
                case "双线性插值法":
                    Params.hv_MeasureInterpolation = "bilinear";
                    break;
                case "双三次插值法":
                    Params.hv_MeasureInterpolation = "bicubic";
                    break;
                case "最近邻域法":
                    Params.hv_MeasureInterpolation = "nearest_neighbor";
                    break;
                default:
                    break;
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
        /// ROI参数显示刷新
        /// </summary>
        /// <param name="roiRectangle2"></param>
        private void RefreshRoiParams(ROIRectangle2 roiRectangle2)
        {
            HTuple hv_Deg;
            NUD_RoiNum.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.PointNum));
            NUD_RoiWidth.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.length2 * 2));
            NUD_RoiHeight.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.length1 * 2));
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Deg);
            NUD_RoiAngle.Value = Convert.ToDecimal(Convert.ToInt32(hv_Deg.D));
            if (Convert.ToInt32(roiRectangle2.midR) < 0)
            {
                hv_RoiMidRow.Value = 1;
            }
            else
            {
                hv_RoiMidRow.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.midR));
            }
            if (Convert.ToInt32(roiRectangle2.midC) < 0)
            {
                hv_RoiMidRow.Value = 1;
            }
            else
            {
                NUD_RoiMidColumn.Value = Convert.ToDecimal(Convert.ToInt32(roiRectangle2.midC));
            }

        }

        //参数修改事件
        //勾选框
        private void cB_IsBackImage_CheckedChanged(object sender, EventArgs e)
        {
            PluginParams.IsBackImage = IsBackImage.Checked;
        }
        private void cB_IsCathode_CheckedChanged(object sender, EventArgs e)
        {
            PluginParams.IsCathode = cB_IsCathode.Checked;
        }
        private void cB_IsFitOneLine_CheckedChanged(object sender, EventArgs e)
        {
            PluginParams.IsFitOneLine = cB_IsFitOneLine.Checked;
        }
        //复选框
        private void CB_SelectLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            PluginParams.JRLine = (JRLineEnum)Enum.Parse(typeof(JRLineEnum), CB_SelectLine.SelectedItem.ToString(), false);
            while ((int)PluginParams.JRLine > PluginParams.array_MeasureLineParam.Length - 1)
            {
                Array.Resize(ref PluginParams.array_MeasureLineParam, PluginParams.array_MeasureLineParam.Length + 1);
                PluginParams.array_MeasureLineParam[PluginParams.array_MeasureLineParam.Length - 1] = new ParamsMeasureLine();
            }
            measLineParams = PluginParams.array_MeasureLineParam[(int)PluginParams.JRLine];

            measLineParams.ho_Image = PluginParams.ho_JRImage;
            ShowFitLineParams(measLineParams);
            SetRoiParams(measLineParams, roiRectangle2, roiController);
            //RefreshRoiParams(roiRectangle2);
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
                    // hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);
                    measLineParams.ho_Image = PluginParams.ho_Image;
                    hWndCtrl_Display.addIconicVar(ho_Image1);
                    SetRoiParams(measLineParams, roiRectangle2, roiController);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();

                }
                catch (Exception)
                {
                }
            }
        }


        /// <summary>
        /// 显示缩略图到图像列表
        /// </summary>
        private void ShowShrinkImageResult(ParamsMeasureEar ParamsEar)
        {
            //显示缩略图到图像选项
            if (ParamsEar.ho_Image.CountObj() != 0)
            {
                PaintWindows.DisplayImage(ParamsEar.ho_Image, HWC_Image1);
                //ShowShrinkIImageIndex = 1;
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
            }
            if (ParamsEar.ho_JRImage.CountObj() != 0)
            {
                PaintWindows.DisplayImage(ParamsEar.ho_JRImage, HWC_Image2);
                //ShowShrinkIImageIndex = 2;
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
            }
            if (ParamsEar.ho_ZoomJRImage.CountObj() != 0)
            {
                PaintWindows.DisplayImage(ParamsEar.ho_ZoomJRImage, HWC_Image3);
                //ShowShrinkIImageIndex = 3;
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
            }

        }

        private void HWC_Image1_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PluginParams.ho_Image.CountObj() != 0)
            {
                hWndCtrl_Display.bl_RoiOperateFlag = false;
                //显示原图
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_Image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                LB_IMageSize.Text = ShowIMageSize(hImage);
                ShowShrinkIImageIndex = 1;
            }
        }

        private void HWC_Image2_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PluginParams.ho_JRImage.CountObj() != 0)
            {
                hWndCtrl_Display.bl_RoiOperateFlag = true;
                //显示原图
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_JRImage, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                //roiController.reset();
                //bl_RoiExist = false;
                //SetRoiParams(measLineParams, roiRectangle2, roiController);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                LB_IMageSize.Text = ShowIMageSize(hImage);
                ShowShrinkIImageIndex = 2;
            }
        }

        private void HWC_Image3_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PluginParams.ho_ZoomJRImage.CountObj() != 0)
            {
                hWndCtrl_Display.bl_RoiOperateFlag = false;
                //显示原图
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_ZoomJRImage, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                LB_IMageSize.Text = ShowIMageSize(hImage);
                ShowShrinkIImageIndex = 3;
            }
        }

    }
}
