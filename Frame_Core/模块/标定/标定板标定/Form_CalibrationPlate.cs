using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Vml;
using NPOI.OpenXmlFormats.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public partial class Form_CalibrationPlate : Form
    {
        DisplayWindowClass displayWindow;
        /// <summary>
        /// 当前显示的结果数据（包含图像、Region）
        /// </summary>
        ShowHalconWindow currentShowHalconWindow = new ShowHalconWindow();
        /// <summary>
        /// 预处理显示的结果数据（包含图像、Region）
        /// </summary>
        ShowHalconWindow PretreatmentShowHalconWindow = new ShowHalconWindow();
        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage = new ImageSimManage();

      
        Param_CalibrationPlate param_Calib;
        Param_CalibrationPlate param_CalibTemp = new Param_CalibrationPlate();
   
        public Form_CalibrationPlate()
        {
            InitializeComponent();           
        }
     
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                displayWindow = new DisplayWindowClass(HWC_Display, TB_PositionAndGray, FullScrean, "SingleCamCalibrationModelForm");
                currentShowHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                currentShowHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                currentShowHalconWindow.m_lineShowInfos = new List<LineInfo>();
                PretreatmentShowHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                PretreatmentShowHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                PretreatmentShowHalconWindow.m_lineShowInfos = new List<LineInfo>();

                cbx_CaltabType.DataSource = Enum.GetNames(typeof(CaltabTypeEnum));
                cbx_MeasureTransition.DataSource = Enum.GetNames(typeof(MeasureTransitionEnum));
                cbx_MeasureSelect.DataSource = Enum.GetNames(typeof(MeasureSelectEnum));
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                currentShowHalconWindow.Dispose();
                PretreatmentShowHalconWindow.Dispose();
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
                m_ImageSimManage.GetPreviewImage(out HObject Load_Image);
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
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image);
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
                m_ImageSimManage.GetNextImage(out HObject Load_Image);
                GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, param_CalibTemp);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }

        private void btn_OneCalibration_Click(object sender, EventArgs e)
        {
            try
            {
                if (nud_CalibrationTargetDiameter.Value <= 0 || nud_CalibrationTargetCenterDistance.Value <= 0)
                {
                    PopUpWindowsShow.MessageShowNotHalcon("标定版直径或中心距离不能为0，请重新输入后保存在执行！");
                    return;
                }
                m_ImageSimManage.GetAllImage(out HObject[] Load_Images);

                ImageAlgorithm.GenCaltab((CaltabTypeEnum)cbx_CaltabType.SelectedIndex, (double)nud_CalibrationTargetDiameter.Value, (double)nud_CalibrationTargetCenterDistance.Value, out string caltabFileName);
                ImageAlgorithm.CalibCalculateCamInternalAndExternalParameters(Load_Images, (double)nud_Focus.Value, (double)nud_Kappa.Value, (double)nud_Sx.Value, (double)nud_Sy.Value, (double)nud_Dz.Value, caltabFileName, out HTuple hv_CamInternalParam, out HTuple hv_ExternalParam);

                param_CalibTemp.hv_CameraParam = hv_CamInternalParam;
                param_CalibTemp.hv_WorldPose = hv_ExternalParam;

                string str1 = ImageAlgorithm.HTupleToString(param_CalibTemp.hv_CameraParam);
                string str2 = ImageAlgorithm.HTupleToString(param_CalibTemp.hv_WorldPose);
                //ControlTool.SetParamShow(tbx_CamInternalParam, ref str1);
                //ControlTool.SetParamShow(tbx_ExternalParam, ref str2);
                richTextBox_CamInternalParam.Clear();
                List<string> list1 = new List<string>(str1.Split(','));
                foreach (string item in list1)
                {
                    richTextBox_CamInternalParam.AppendText(item+"\r\n");
                }
                richTextBox_ExternalParam.Clear();
                List<string> list2 = new List<string>(str1.Split(','));
                foreach (string item in list2)
                {
                    richTextBox_ExternalParam.AppendText(item + "\r\n");
                }


                PopUpWindowsShow.MessageShowNotHalcon("标定完成");

                foreach (HObject image in Load_Images)
                {
                    image?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        private void btn_Transition_Click(object sender, EventArgs e)
        {
            try
            {
                ImageAlgorithm.GetImageCoordToWorldCoord(param_CalibTemp.hv_CameraParam, param_CalibTemp.hv_WorldPose,
                (double)nud_CamCoordY.Value, (double)nud_CamCoordX.Value, out double row, out double col);

                nud_WorldCoordX.Value = (decimal)col;
                nud_WorldCoordY.Value = (decimal)row;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SaveParam();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }

        private void HWC_Image1_HMouseDown(object sender, HMouseEventArgs e)
        {

        }

        private void HWC_Image2_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PretreatmentShowHalconWindow.m_hImage != null && PretreatmentShowHalconWindow.m_hImage.CountObj() != 0)
            {
                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = PretreatmentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
            }
        }

        private void HWC_Image3_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (currentShowHalconWindow.m_hImage != null && currentShowHalconWindow.m_hImage.CountObj() != 0)
            {
                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
            }
        }

        public void SetParam(Param_CalibrationPlate _param_Calib)
        {
            param_Calib = _param_Calib;

            LoadParam();
        }
        private void LoadParam()
        {
            try
            {

                ControlTool.SetParamShow(nud_CirclMinThd, ref param_Calib.CirclMinThd);
                ControlTool.SetParamShow(nud_CirclMaxThd, ref param_Calib.CirclMaxThd);
                ControlTool.SetParamShow(nud_Radius, ref param_Calib.Radius);
                ControlTool.SetParamShow(cbx_MeasureTransition, ref param_Calib.CircleModelParam.MeasureTransition);
                ControlTool.SetParamShow(cbx_MeasureSelect, ref param_Calib.CircleModelParam.MeasureSelect);
                ControlTool.SetParamShow(nud_MesureThreshold, ref param_Calib.CircleModelParam.MesureThreshold);


                
                ControlTool.SetParamShow(nud_MarkthrMin, ref param_Calib.MarkCirclMinThd);
                ControlTool.SetParamShow(nud_MarkthrMax, ref param_Calib.MarkCirclMaxThd);
                ControlTool.SetParamShow(nud_MarkWidthMin, ref param_Calib.MarkWidthMin);
                ControlTool.SetParamShow(nud_MarkWidthMax, ref param_Calib.MarkWidthMax);
                ControlTool.SetParamShow(nud_RecHeight, ref param_Calib.MarkCircleModelParam.MeasureLength1);
                ControlTool.SetParamShow(nud_RecWidth, ref param_Calib.MarkCircleModelParam.MeasureLength2);
               
                ControlTool.SetParamShow(cbx_IsEnbleMarrk, ref param_Calib.bIsEnableMark);

                ControlTool.SetParamShow(cbx_CaltabType, ref param_Calib.CaltabType);
                ControlTool.SetParamShow(nud_CalibrationTargetDiameter, ref param_Calib.CalibrationTargetDiameter);
                ControlTool.SetParamShow(nud_CalibrationTargetCenterDistance, ref param_Calib.CalibrationTargetCenterDistance);
                ControlTool.SetParamShow(nud_Dz, ref param_Calib.Dz);
                ControlTool.SetParamShow(nud_Focus, ref param_Calib.Focus);
                ControlTool.SetParamShow(nud_Kappa, ref param_Calib.Kappa);
                ControlTool.SetParamShow(nud_Sx, ref param_Calib.Sx);
                ControlTool.SetParamShow(nud_Sy, ref param_Calib.Sy);

                ControlTool.SetParamShow(nud_BasePointX, ref param_Calib.BasePointX);
                ControlTool.SetParamShow(nud_BasePointY, ref param_Calib.BasePointY);

                param_CalibTemp.hv_CameraParam = param_Calib.hv_CameraParam;
                param_CalibTemp.hv_WorldPose = param_Calib.hv_WorldPose;
                string str1 = ImageAlgorithm.HTupleToString(param_CalibTemp.hv_CameraParam);
                string str2 = ImageAlgorithm.HTupleToString(param_CalibTemp.hv_WorldPose);
                //ControlTool.SetParamShow(tbx_CamInternalParam, ref str1);
                //ControlTool.SetParamShow(tbx_ExternalParam, ref str2);
                richTextBox_CamInternalParam.Clear();
                List<string> list1 = new List<string>(str1.Split(','));
                foreach (string item in list1)
                {
                    richTextBox_CamInternalParam.AppendText(item + "\r\n");
                }
                richTextBox_ExternalParam.Clear();
                List<string> list2 = new List<string>(str1.Split(','));
                foreach (string item in list2)
                {
                    richTextBox_ExternalParam.AppendText(item + "\r\n");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }
        private void GetFormParam(ref Param_CalibrationPlate CalibrationPlateParam)
        {
            ControlTool.GetParamSave(nud_CirclMinThd, ref CalibrationPlateParam.CirclMinThd);
            ControlTool.GetParamSave(nud_CirclMaxThd, ref CalibrationPlateParam.CirclMaxThd);
            ControlTool.GetParamSave(nud_Radius, ref CalibrationPlateParam.Radius);
            ControlTool.GetParamSave(cbx_MeasureTransition, ref CalibrationPlateParam.CircleModelParam.MeasureTransition);
            ControlTool.GetParamSave(cbx_MeasureSelect, ref CalibrationPlateParam.CircleModelParam.MeasureSelect);
            ControlTool.GetParamSave(nud_MesureThreshold, ref CalibrationPlateParam.CircleModelParam.MesureThreshold);



            
            
            ControlTool.GetParamSave(nud_MarkWidthMin, ref param_Calib.MarkWidthMin);
            ControlTool.GetParamSave(nud_MarkWidthMax, ref param_Calib.MarkWidthMax);
            ControlTool.GetParamSave(nud_MarkthrMin, ref param_Calib.MarkCirclMinThd);
            ControlTool.GetParamSave(nud_MarkthrMax, ref param_Calib.MarkCirclMaxThd);
            ControlTool.GetParamSave(nud_RecHeight, ref CalibrationPlateParam.MarkCircleModelParam.MeasureLength1);
            ControlTool.GetParamSave(nud_RecHeight, ref CalibrationPlateParam.MarkCircleModelParam.MeasureLength2);
          
            ControlTool.GetParamSave(cbx_IsEnbleMarrk, ref param_Calib.bIsEnableMark);

            ControlTool.GetParamSave(cbx_CaltabType, ref CalibrationPlateParam.CaltabType);
            ControlTool.GetParamSave(nud_CalibrationTargetDiameter, ref CalibrationPlateParam.CalibrationTargetDiameter);
            ControlTool.GetParamSave(nud_CalibrationTargetCenterDistance, ref CalibrationPlateParam.CalibrationTargetCenterDistance);
            ControlTool.GetParamSave(nud_Dz, ref CalibrationPlateParam.Dz);
            ControlTool.GetParamSave(nud_Focus, ref CalibrationPlateParam.Focus);
            ControlTool.GetParamSave(nud_Kappa, ref CalibrationPlateParam.Kappa);
            ControlTool.GetParamSave(nud_Sx, ref CalibrationPlateParam.Sx);
            ControlTool.GetParamSave(nud_Sy, ref CalibrationPlateParam.Sy);

            ControlTool.GetParamSave(nud_BasePointX, ref param_Calib.BasePointX);
            ControlTool.GetParamSave(nud_BasePointY, ref param_Calib.BasePointY);
        }
        private void SaveParam()
        {
            try
            {
                GetFormParam(ref param_Calib);

                param_Calib.hv_CameraParam = param_CalibTemp.hv_CameraParam;
                param_Calib.hv_WorldPose = param_CalibTemp.hv_WorldPose;
                param_Calib.SaveCalibParam();

                param_Calib.Save();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }


        public bool FindMarkCircle(HObject ho_Image, out HTuple Row,out HTuple Column,out HObject ho_Circle, out HObject ho_Contours)
        {
            HTuple hv_Row = param_Calib.BasePointY;
            HTuple hv_Column = param_Calib.BasePointX;
            HTuple hv_Radius = param_Calib.MarkRadius;
            HTuple Hv_MeasureThreshold = param_Calib.MarkCircleModelParam.MesureThreshold;
            HTuple Hv_Length1 = param_Calib.MarkCircleModelParam.MeasureLength1;
            HTuple Hv_Length2 = param_Calib.MarkCircleModelParam.MeasureLength2;
            try
            {
              
                //HOperatorSet.GenCircleContourXld(out HObject ho_ContCircle, hv_Row, hv_Column, hv_Radius,
                //0, 6.28318, "positive", 1);
                HOperatorSet.CreateMetrologyModel(out HTuple hv_MetrologyHandle);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_Row.TupleConcat(
                hv_Column))).TupleConcat(hv_Radius), Hv_Length1, Hv_Length2, 1, Hv_MeasureThreshold, new HTuple(), new HTuple(),
                out HTuple hv_Index);
                HOperatorSet.ApplyMetrologyModel(ho_Image, hv_MetrologyHandle);
             
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out HTuple hv_Parameter);
          
                HOperatorSet.GetMetrologyObjectResultContour(out  ho_Circle, hv_MetrologyHandle,
                    "all", "all", 1.5);
              
                HOperatorSet.GetMetrologyObjectMeasures(out  ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                Column = hv_Parameter[1];
                Row = hv_Parameter[0];
                return true;
            }
            catch (Exception ee)
            {
                Column = param_Calib.BasePointY;
                Row = param_Calib.BasePointX;
                ho_Circle = new HObject();
                ho_Contours = new HObject();
                return false;
           
            }
        }

        private void Execute(HObject image, Param_CalibrationPlate CalibrationPlateParam)
        {
            HObject ho_Circle = null;
            try
            {

                if (image != null)
                {
                    // 释放之前的数据              
                    currentShowHalconWindow.Dispose();
                    currentShowHalconWindow.m_hImage = image;
                    PretreatmentShowHalconWindow.Dispose();
                    PretreatmentShowHalconWindow.m_hImage = image;


                    //加载图像数据
                    HOperatorSet.GetImageSize(image, out HTuple imgWidth, out HTuple imgHeight);

                    // 预处理
                    ImageAlgorithm.FindCirclePretreatment(image, CalibrationPlateParam, out HObject ho_CircleRegion);
                    PretreatmentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_CircleRegion, m_stuGetColor = HalconColor.red });

                    //找圆(单列的圆)
                    List<CircleInfo> circleInfo1 = new List<CircleInfo>();
                    if (ho_CircleRegion != null)
                    {
                        //对圆列坐标进行排序
                        HTuple hv_CenterRow, hv_CenterCol, hv_Radius;
                        HOperatorSet.SmallestCircle(ho_CircleRegion, out hv_CenterRow, out hv_CenterCol, out hv_Radius);
                        HOperatorSet.TupleSortIndex(hv_CenterCol, out HTuple index);

                        HTuple colLocation = 0;
                        for (int i = 0; i < index.Length; i++)
                        {
                            HOperatorSet.SelectObj(ho_CircleRegion, out ho_Circle, index[i] + 1);
                            HOperatorSet.SmallestCircle(ho_Circle, out hv_CenterRow, out hv_CenterCol, out hv_Radius);

                            // 限制圆列坐标的范围
                            if (colLocation.D == 0) colLocation = hv_CenterCol;
                            if (colLocation.D <= (hv_CenterCol.D - hv_Radius.D) ||
                                colLocation.D >= (hv_CenterCol.D + hv_Radius.D))
                            {
                                break;
                            }

                            //找圆
                            CalibrationPlateParam.CircleModelParam.center_X = hv_CenterCol.D;
                            CalibrationPlateParam.CircleModelParam.center_Y = hv_CenterRow.D;
                            CalibrationPlateParam.CircleModelParam.radius = hv_Radius.D;
                            ImageAlgorithm.FindCircle(image, CalibrationPlateParam.CircleModelParam, out CircleInfo circleInfo);
                            if (circleInfo != null)
                            {
                                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = circleInfo.ho_Contours, m_stuGetColor = HalconColor.green });
                                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = circleInfo.ho_Cross, m_stuGetColor = HalconColor.green });

                                circleInfo1.Add(new CircleInfo() { center_X = circleInfo.center_X, center_Y = circleInfo.center_Y, radius = circleInfo.radius });

                            }
                        }
                    }

                    //对圆行坐标进行排序
                    circleInfo1.Sort((a, b) => a.center_Y.CompareTo(b.center_Y));

                    //计算距离（至少两个圆）                    
                    if (circleInfo1.Count >= 2)
                    {
                        HTuple row1 = new HTuple(circleInfo1.Select(c => c.center_Y).ToArray());
                        HTuple col1 = new HTuple(circleInfo1.Select(c => c.center_X).ToArray());

                        HOperatorSet.DistancePp(row1.TupleSelectRange(0, row1.Length - 2), col1.TupleSelectRange(0, col1.Length - 2),
                           row1.TupleSelectRange(1, row1.Length - 1), col1.TupleSelectRange(1, col1.Length - 1), out HTuple distance);

                        //计算世界坐标
                        ImageAlgorithm.GetImageCoordToWorldCoord(CalibrationPlateParam.hv_CameraParam, CalibrationPlateParam.hv_WorldPose,
                         row1.DArr, col1.DArr, out double[] rows, out double[] cols);
                        HTuple hv_row = new HTuple(rows);
                        HTuple hv_col = new HTuple(cols);

                        HOperatorSet.DistancePp(hv_row.TupleSelectRange(0, hv_row.Length - 2), hv_col.TupleSelectRange(0, hv_col.Length - 2),
                        hv_row.TupleSelectRange(1, hv_row.Length - 1), hv_col.TupleSelectRange(1, hv_col.Length - 1), out HTuple transitionDistance);

                        for (int i = 0; i < distance.Length; i++)
                        {
                            currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"Distance: {distance[i].D.ToString("F3")}___{transitionDistance[i].D.ToString("F3")}", m_stuGetColor = HalconColor.blue, m_nFontSize=20 });
                        }
                    }

                    displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                    screenshotImage?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
            finally
            {
                ho_Circle?.Dispose();
            }
        }

    
        private void Btn_FindMark_Click(object sender, EventArgs e)
        {
            HObject ho_Contours;
            HObject ho_ContoursBaseCircle;
            if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
            m_ImageSimManage.GetPreviewImage(out HObject Load_Image);
            GetFormParam(ref param_CalibTemp);
            double row = param_Calib.BasePointY;
            double col = param_Calib.BasePointX;
       
            ImageAlgorithm.FindCirclePretreatmentTemp(Load_Image, param_Calib, out HObject region,out HObject rec, out HTuple Row, out HTuple Column);
  
            if (region != null)
            {
                currentShowHalconWindow.m_hImage = Load_Image;
               
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = region, m_stuGetColor = HalconColor.green });
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rec, m_stuGetColor = HalconColor.green });


                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                screenshotImage?.Dispose();
            }
            else
            {
                if(Row?.Length==0||Row?.Length == null)
                {
                    MessageBox.Show("未找到圆圈");
                }
             
            }
            //redImg?.Dispose();
            Nud_MarkX.Value =Convert.ToDecimal( Column.D);
            Nud_MarkY.Value = Convert.ToDecimal(Row.D);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HObject ho_Contours;
            HObject ho_ContoursBaseCircle;
            if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
            m_ImageSimManage.GetPreviewImage(out HObject Load_Image);
            GetFormParam(ref param_CalibTemp);
            double row = param_Calib.BasePointY;
            double col = param_Calib.BasePointX;

           bool bFindCircle  =ImageAlgorithm.FindCirclePretreatmentTemp(Load_Image, param_Calib, out HObject region, out HObject rec, out HTuple Row, out HTuple Column,true);

            if(bFindCircle)
            {
                if (region?.CountObj() != null)
                {
                    currentShowHalconWindow.m_hImage = Load_Image;

                    currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = region, m_stuGetColor = HalconColor.green });
                    currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rec, m_stuGetColor = HalconColor.green });


                    displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                    screenshotImage?.Dispose();
                    nud_BasePointX.Value = Convert.ToDecimal(Column.D);
                    nud_BasePointY.Value = Convert.ToDecimal(Row.D);
                }
                else
                {
               

                }

            }
            else
            {
                region?.Dispose();
                rec?.Dispose();
                MessageBox.Show("未找到圆圈");
            }
           
            //redImg?.Dispose();
            Nud_MarkX.Value = Convert.ToDecimal(Column.D);
            Nud_MarkY.Value = Convert.ToDecimal(Row.D);
           
        }
    }
}


   






  

