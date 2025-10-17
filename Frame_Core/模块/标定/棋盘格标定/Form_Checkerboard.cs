using HalconDotNet;
using LogTool;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Frame_Core
{
    public partial class Form_Checkerboard : Form
    {
        DisplayWindowClass displayWindow;
        /// <summary>
        /// 当前显示的结果数据（包含图像、Region）
        /// </summary>
        ShowHalconWindow currentShowHalconWindow = new ShowHalconWindow();

        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage = new ImageSimManage();


        Param_Checkerboard param_Calib;
        Param_Checkerboard param_CalibTemp = new Param_Checkerboard();

        public Form_Checkerboard()
        {
            InitializeComponent();

        }

        private void Form_MultiCam_Load(object sender, EventArgs e)
        {
            try
            {
                displayWindow = new DisplayWindowClass(HWC_Display, TB_PositionAndGray, FullScrean, "MultiCamCalibrationModelForm");
                currentShowHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                currentShowHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                currentShowHalconWindow.m_lineShowInfos = new List<LineInfo>();

                cbx_MeasureTransition.DataSource = Enum.GetNames(typeof(MeasureTransitionEnum));
                cbx_MeasureSelect.DataSource = Enum.GetNames(typeof(MeasureSelectEnum));

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        private void Form_MultiCam_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                currentShowHalconWindow.Dispose();
                displayWindow.Dispose();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        private void LoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Multiselect = true;
            //FileDialog.InitialDirectory = Project.Instance.standImagePath;
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                m_ImageSimManage.InitSimFlag();
                m_ImageSimManage.ImageEmulationPaths = FileDialog.FileNames;
            }
        }

        private void btn_Preview_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetPreviewImage(out HObject Load_Image, out string imgPath);
                GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, param_CalibTemp);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }
        private void btn_Run_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image, out string imgPath);
                GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, param_CalibTemp);
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
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetNextImage(out HObject Load_Image, out string imgPath);
                GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, param_CalibTemp);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }

        public void SetParam(Param_Checkerboard _param_Calib)
        {
            param_Calib = _param_Calib;

            LoadParam();
        }
        private void LoadParam()
        {
            try
            {

                ControlTool.SetParamShow(nud_CheckerboardBoxMinThd, ref param_Calib.CheckerboardMinThd);
                ControlTool.SetParamShow(nud_CheckerboardBoxMaxThd, ref param_Calib.CheckerboardMaxThd);
                ControlTool.SetParamShow(nud_CheckerboardBoxSideLengthPix_X, ref param_Calib.CheckerboardBoxSideLengthPix_X);
                ControlTool.SetParamShow(nud_CheckerboardBoxSideLengthPix_Y, ref param_Calib.CheckerboardBoxSideLengthPix_Y);
                ControlTool.SetParamShow(cbx_MeasureTransition, ref param_Calib.LineModelParam.MeasureTransition);
                ControlTool.SetParamShow(cbx_MeasureSelect, ref param_Calib.LineModelParam.MeasureSelect);
                ControlTool.SetParamShow(nud_MesureThreshold, ref param_Calib.LineModelParam.MesureThreshold);
                ControlTool.SetParamShow(nud_CalibrationPointNum, ref param_Calib.CalibrationPointNum);


                param_CalibTemp.HomMat2D = param_Calib.HomMat2D;
                param_CalibTemp.HomMat2DInvert = param_Calib.HomMat2DInvert;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }
        private void GetFormParam(ref Param_Checkerboard CalibParam)
        {
            ControlTool.GetParamSave(nud_CheckerboardBoxMinThd, ref CalibParam.CheckerboardMinThd);
            ControlTool.GetParamSave(nud_CheckerboardBoxMaxThd, ref CalibParam.CheckerboardMaxThd);
            ControlTool.GetParamSave(nud_CheckerboardBoxSideLengthPix_X, ref CalibParam.CheckerboardBoxSideLengthPix_X);
            ControlTool.GetParamSave(nud_CheckerboardBoxSideLengthPix_Y, ref CalibParam.CheckerboardBoxSideLengthPix_Y);
            ControlTool.GetParamSave(cbx_MeasureTransition, ref CalibParam.LineModelParam.MeasureTransition);
            ControlTool.GetParamSave(cbx_MeasureSelect, ref CalibParam.LineModelParam.MeasureSelect);
            ControlTool.GetParamSave(nud_MesureThreshold, ref CalibParam.LineModelParam.MesureThreshold);
            ControlTool.GetParamSave(nud_CalibrationPointNum, ref CalibParam.CalibrationPointNum);
        }
        private void SaveParam()
        {
            try
            {
                GetFormParam(ref param_Calib);

                param_Calib.HomMat2D = param_CalibTemp.HomMat2D;
                param_Calib.HomMat2DInvert = param_CalibTemp.HomMat2DInvert;
                param_Calib.SaveCalibParam();

                param_Calib.Save();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }

        private void btn_Calculate_Click(object sender, EventArgs e)
        {
            try
            {

                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image, out string imgPath);
                if (Load_Image == null) return;
                GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, param_CalibTemp);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }

        private void btn_Calibration_Click(object sender, EventArgs e)
        {
            try
            {
                List<double> ptxs = new List<double>();
                List<double> ptys = new List<double>();
                List<double> ptxd = new List<double>();
                List<double> ptyd = new List<double>();

                for (int i = 0; i < dgv_Original.Rows.Count; i++)
                {
                    ptxs.Add(double.Parse(dgv_Original.Rows[i].Cells[1].Value.ToString()));
                    ptys.Add(double.Parse(dgv_Original.Rows[i].Cells[0].Value.ToString()));
                    ptxd.Add(double.Parse(dgv_Target.Rows[i].Cells[1].Value.ToString()));
                    ptyd.Add(double.Parse(dgv_Target.Rows[i].Cells[0].Value.ToString()));
                }

                HOperatorSet.VectorToHomMat2d(ptxs.ToArray(), ptys.ToArray(), ptxd.ToArray(), ptyd.ToArray(), out HTuple homMat2D);
                HOperatorSet.HomMat2dInvert(homMat2D, out HTuple homMat2DInvert);

                PopUpWindowsShow.MessageShowNotHalcon("标定完成");

                param_CalibTemp.HomMat2D = homMat2D;
                param_CalibTemp.HomMat2DInvert = homMat2DInvert;
                SaveParam();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

            SaveParam();
        }

        private void btn_Transition_Click(object sender, EventArgs e)
        {
            try
            {

                HOperatorSet.AffineTransPoint2d(param_CalibTemp.HomMat2D, (double)nud_CamCoordX.Value, (double)nud_CamCoordY.Value, out HTuple hrptxs, out HTuple hrptys);

                nud_WorldCoordX.Value = (decimal)hrptxs.D;
                nud_WorldCoordY.Value = (decimal)hrptys.D;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 执行标定
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="CalibParam"></param>
        /// <param name="useHomMat2D">使用标定的变换矩阵</param>
        private void Execute(HObject ho_Image, Param_Checkerboard CalibParam)
        {
            try
            {

                // 释放之前的数据              
                currentShowHalconWindow.Dispose();
                currentShowHalconWindow.m_hImage = ho_Image;

                //预处理找棋盘格
                ImageAlgorithm.FindCheckerboardPretreatment(ho_Image, CalibParam.CheckerboardBoxSideLengthPix_X, CalibParam.CheckerboardBoxSideLengthPix_Y, CalibParam.CheckerboardMinThd, CalibParam.CheckerboardMaxThd, out HObject ho_Region);
                HOperatorSet.SmallestRectangle1(ho_Region, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Region, m_stuGetColor = HalconColor.red });

                //找边线和角点
                List<PointInfo> imageCoordinatePointInfos = new List<PointInfo>();
                List<PointInfo> worldCoordinatePointInfos = new List<PointInfo>();
                for (int i = 0; i < ho_Region.CountObj(); i++)
                {
                    HObject ho_RegionTemp = ho_Region.SelectObj(i + 1);
                    ImageAlgorithm.FindRectangleEdge(ho_Image, ho_RegionTemp, MeasureTransitionEnum.不区分明暗,
                   out double leftEdgeBeginRow, out double leftEdgeBeginCol, out double leftEdgeEndRow, out double leftEdgeEndCol,
                   out double topEdgeBeginRow, out double topEdgeBeginCol, out double topEdgeEndRow, out double topEdgeEndCol,
                   out double rightEdgeBeginRow, out double rightEdgeBeginCol, out double rightEdgeEndRow, out double rightEdgeEndCol,
                   out double bottomEdgeBeginRow, out double bottomEdgeBeginCol, out double bottomEdgeEndRow, out double bottomEdgeEndCol,
                   out double leftTop_Row, out double leftTop_Col, out double rightTop_Row, out double rightTop_Col,
                   out double leftBottom_Row, out double leftBottom_Col, out double rightBottom_Row, out double rightBottom_Col);

                    //添加点坐标
                    imageCoordinatePointInfos.Add(new PointInfo() { Row = leftTop_Row, Col = leftTop_Col });
                    imageCoordinatePointInfos.Add(new PointInfo() { Row = rightTop_Row, Col = rightTop_Col });
                    imageCoordinatePointInfos.Add(new PointInfo() { Row = leftBottom_Row, Col = leftBottom_Col });
                    imageCoordinatePointInfos.Add(new PointInfo() { Row = rightBottom_Row, Col = rightBottom_Col });

                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"R:{leftTop_Row.ToString("F3")}_C:{leftTop_Col.ToString("F3")}", m_fRow = leftTop_Row, m_fCol = leftTop_Col, m_bIsWindow = false });
                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"R:{rightTop_Row.ToString("F3")}_C:{rightTop_Col.ToString("F3")}", m_fRow = rightTop_Row, m_fCol = rightTop_Col, m_bIsWindow = false });
                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"R:{leftBottom_Row.ToString("F3")}_C:{leftBottom_Col.ToString("F3")}", m_fRow = leftBottom_Row - 3, m_fCol = leftBottom_Col, m_bIsWindow = false });
                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"R:{rightBottom_Row.ToString("F3")}_C:{rightBottom_Col.ToString("F3")}", m_fRow = rightBottom_Row - 3, m_fCol = rightBottom_Col, m_bIsWindow = false });


                    //显示角点
                    HOperatorSet.GenCrossContourXld(out HObject ho_Cross, leftTop_Row, leftTop_Col, 200, 45);
                    currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.blue });
                    HOperatorSet.GenCrossContourXld(out ho_Cross, rightTop_Row, rightTop_Col, 200, 45);
                    currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.blue });
                    HOperatorSet.GenCrossContourXld(out ho_Cross, leftBottom_Row, leftBottom_Col, 200, 45);
                    currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.blue });
                    HOperatorSet.GenCrossContourXld(out ho_Cross, rightBottom_Row, rightBottom_Col, 200, 45);
                    currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.blue });

                    //显示边线
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = leftEdgeBeginRow, ColBegin = leftEdgeBeginCol, RowEnd = leftEdgeEndRow, ColEnd = leftEdgeEndCol, Color = HalconColor.green });
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = topEdgeBeginRow, ColBegin = topEdgeBeginCol, RowEnd = topEdgeEndRow, ColEnd = topEdgeEndCol, Color = HalconColor.green });
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rightEdgeBeginRow, ColBegin = rightEdgeBeginCol, RowEnd = rightEdgeEndRow, ColEnd = rightEdgeEndCol, Color = HalconColor.green });
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = bottomEdgeBeginRow, ColBegin = bottomEdgeBeginCol, RowEnd = bottomEdgeEndRow, ColEnd = bottomEdgeEndCol, Color = HalconColor.green });


                    //计算距离
                    HTuple hv_distance1 = 0;
                    HTuple hv_distance2 = 0;
                    HTuple hv_distance3 = 0;
                    HTuple hv_distance4 = 0;

                    HTuple hv_leftTop_Row = leftTop_Row;
                    HTuple hv_leftTop_Col = leftTop_Col;
                    HTuple hv_rightTop_Row = rightTop_Row;
                    HTuple hv_rightTop_Col = rightTop_Col;
                    HTuple hv_leftBottom_Row = leftBottom_Row;
                    HTuple hv_leftBottom_Col = leftBottom_Col;
                    HTuple hv_rightBottom_Row = rightBottom_Row;
                    HTuple hv_rightBottom_Col = rightBottom_Col;

                    if (CalibParam.HomMat2D == null && CalibParam.HomMat2D.Length == 0)
                    {
                        HOperatorSet.AffineTransPoint2d(CalibParam.HomMat2D, leftTop_Row, leftTop_Col, out hv_leftTop_Row, out hv_leftTop_Col);
                        HOperatorSet.AffineTransPoint2d(CalibParam.HomMat2D, rightTop_Row, rightTop_Col, out hv_rightTop_Row, out hv_rightTop_Col);
                        HOperatorSet.AffineTransPoint2d(CalibParam.HomMat2D, leftBottom_Row, leftBottom_Col, out hv_leftBottom_Row, out hv_leftBottom_Col);
                        HOperatorSet.AffineTransPoint2d(CalibParam.HomMat2D, rightBottom_Row, rightBottom_Col, out hv_rightBottom_Row, out hv_rightBottom_Col);
                    }              

                    HOperatorSet.DistancePp(hv_leftTop_Row, hv_leftTop_Col, hv_rightTop_Row, hv_rightTop_Col, out hv_distance1);//左上角到右上角距离
                    HOperatorSet.DistancePp(hv_leftBottom_Row, hv_leftBottom_Col, hv_rightBottom_Row, hv_rightBottom_Col, out hv_distance2);//左下角到右下角距离
                    HOperatorSet.DistancePp(hv_leftTop_Row, hv_leftTop_Col, hv_leftBottom_Row, hv_leftBottom_Col, out hv_distance3);//左上角到左下角距离
                    HOperatorSet.DistancePp(hv_rightTop_Row, hv_rightTop_Col, hv_rightBottom_Row, hv_rightBottom_Col, out hv_distance4);//右上角到右下角距离

                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = hv_distance1.D.ToString("F2"), m_stuGetColor = HalconColor.green, m_fRow = (leftTop_Row + rightTop_Row) / 2, m_fCol = (leftTop_Col + rightTop_Col) / 2, m_bIsWindow = false });
                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = hv_distance2.D.ToString("F2"), m_stuGetColor = HalconColor.green, m_fRow = (leftBottom_Row + rightBottom_Row) / 2, m_fCol = (leftBottom_Col + rightBottom_Col) / 2, m_bIsWindow = false });
                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = hv_distance3.D.ToString("F2"), m_stuGetColor = HalconColor.green, m_fRow = (leftTop_Row + leftBottom_Row) / 2, m_fCol = (leftTop_Col + leftBottom_Col) / 2, m_bIsWindow = false });
                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = hv_distance4.D.ToString("F2"), m_stuGetColor = HalconColor.green, m_fRow = (rightTop_Row + rightBottom_Row) / 2, m_fCol = (rightTop_Col + rightBottom_Col) / 2, m_bIsWindow = false });
                }

                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                screenshotImage?.Dispose();

                //显示标定点
                NDataGridView.ClearRows(dgv_Original);
                int pointNum = CalibParam.CalibrationPointNum < imageCoordinatePointInfos.Count ? CalibParam.CalibrationPointNum : imageCoordinatePointInfos.Count;
                for (int i = 0; i < pointNum; i++)
                {
                    NDataGridView.AddRow(dgv_Original, new string[] { imageCoordinatePointInfos[i].Row.ToString("F2"), imageCoordinatePointInfos[i].Col.ToString("F2") }, 1000, false);
                }

                while (dgv_Original.Rows.Count > dgv_Target.Rows.Count)
                {
                    NDataGridView.AddRow(dgv_Target, new string[] { "0", "0" }, 1000, false);
                }
                while (dgv_Original.Rows.Count < dgv_Target.Rows.Count)
                {
                    dgv_Target.Rows.RemoveAt(dgv_Target.Rows.Count - 1);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }
    }
}
