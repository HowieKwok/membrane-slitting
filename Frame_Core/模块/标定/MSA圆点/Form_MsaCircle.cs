using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YYSD.UICore.Control;
using YYSD.UICore;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula.Functions;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Vml;
using System.Windows.Shapes;

namespace Frame_Core
{
    public partial class Form_MsaCircle : Form
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


        public Param_MsaCircle Param_MsaCircle;

        Dictionary<CameraEnum, CameraManager> cameraManager;

        public Form_MsaCircle(Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();

            cameraManager = _cameraManager;

            displayWindow = new DisplayWindowClass(HWC_Display, TB_PositionAndGray, FullScrean, "Form_MsaCircle");
            currentShowHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
            currentShowHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
            currentShowHalconWindow.m_lineShowInfos = new List<LineInfo>();

        }


        private void Form_MsaCircle_Load(object sender, EventArgs e)
        {
            intCobCam.Items.Clear();
            foreach (var item in cameraManager)
            {
                intCobCam.Items.Add(item.Value.m_CamParam.CameraName.ToString());
            }
            intCobCam.SelectedIndex = 0;
            linkLabel1.Text = Project.Instance.paramPathSet.ProductMojuanPath;
        }


        private void Execute(HObject image, bool flag = false)
        {
            try
            {
                if (image != null)
                {
                    // 释放之前的数据              
                    currentShowHalconWindow.Dispose();
                    currentShowHalconWindow.m_hImage = image;
                    if (fROI_YLenght1.Value != 0 && flag)
                    {
                        HOperatorSet.GenRectangle2(out HObject rectangle, Convert.ToDouble(fROI_YCenterY.Value), Convert.ToDouble(fROI_YCenterX.Value), Convert.ToDouble(fROI_YCenterR.Value), Convert.ToDouble(fROI_YLenght1.Value), Convert.ToDouble(fROI_YLenght2.Value));
                        if (Param_MsaCircle.bIsEnabelMobel)
                        {
                            if(!isModelCreat)
                            {
                               MessageBox.Show("请先创建模型");
                                return;
                            }
                            else
                            {
                               try
                                {
                                    HOperatorSet.FindShapeModel(image, modelY_handle, 0, (new HTuple(30)).TupleRad()
                                      , 0.3, 1, 0.5, "least_squares", 0, 0.9, out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Angle,
                                    out HTuple hv_Score);
                                    HOperatorSet.VectorAngleToRigid(Convert.ToDouble(fROI_YCenterY.Value), Convert.ToDouble(fROI_YCenterX.Value), 0, hv_Row,
                                     hv_Column, hv_Angle, out HTuple hv_HomMat2D);
                                    HOperatorSet.AffineTransRegion(rectangle, out rectangle, hv_HomMat2D, "nearest_neighbor");
                                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "Y方向模型匹配成功", m_stuGetColor = HalconColor.green });
                                }
                                catch(Exception ex)
                                {
                                    MessageBox.Show("Y方向模型未匹配成功\n"+ex.Message);
                                    return;
                                }
                            }
                        }
                       
                        currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });
                        HOperatorSet.ReduceDomain(image, rectangle, out HObject imageReduced);
                        HOperatorSet.Threshold(imageReduced, out imageReduced, Param_MsaCircle.Ithreoldmin, Param_MsaCircle.Ithreoldmax);
                        HOperatorSet.Connection(imageReduced, out imageReduced);
                        HOperatorSet.SelectShape(imageReduced, out imageReduced, "height", "and", Param_MsaCircle.fHeightMin, Param_MsaCircle.fHeightMax);
                        HOperatorSet.SelectShape(imageReduced, out imageReduced, "width", "and", Param_MsaCircle.fwidthmin, Param_MsaCircle.fwidthmax);
                        HOperatorSet.SelectShape(imageReduced, out imageReduced, "area", "and", Param_MsaCircle.fAreaMin, Param_MsaCircle.fAreaMax);
                        if (imageReduced?.CountObj() != null && imageReduced?.CountObj() != 0)
                        {
                            HOperatorSet.SortRegion(imageReduced, out imageReduced, "character", "true", "row");
                            HOperatorSet.GenContourRegionXld(imageReduced, out imageReduced, "border");

                            HOperatorSet.FitCircleContourXld(imageReduced, "algebraic", -1, 0, 0, 3, 2,
                            out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi,
                            out HTuple hv_PointOrder);

                            HOperatorSet.GenCircleContourXld(out imageReduced, hv_Row, hv_Column, hv_Radius,
                           hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
                            currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = imageReduced, m_stuGetColor = HalconColor.green });

                            int num = (int)imageReduced?.CountObj();
                            List<double> double_Y;
                            List<double> double_X;
                            if (Param_MsaCircle.pointYRotateInfos.Count == num)
                            {
                                double_Y = Param_MsaCircle.pointYRotateInfos.Select(x => x.Row).ToList();
                                double_X = Param_MsaCircle.pointYRotateInfos.Select(x => x.Col).ToList();
                                HOperatorSet.VectorToHomMat2d(hv_Column, hv_Row, double_X.ToArray(),
                                double_Y.ToArray(), out HTuple hv_HomMat2D1);
                                HOperatorSet.AffineTransPoint2d(hv_HomMat2D1, hv_Column, hv_Row,
                                out HTuple hv_Qx, out HTuple hv_Qy);
                                List<string> stringTitles = new List<string>();
                                List<string> stringData = new List<string>();
                                for (int i = 1; i < hv_Qx.Length; i++)
                                {
                                    HOperatorSet.DistancePp(hv_Qy[0], hv_Qx[0], hv_Qy[i], hv_Qx[i],out HTuple distance);
                                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage =$"Y距离{i}:{distance}mm", m_stuGetColor = HalconColor.green });
                                    stringTitles.Add($"Y距离{i}");
                                    stringData.Add(distance.D.ToString("0.000"));

                                }
                                stringTitles.Insert(0, "序号");
                                stringData.Insert(0, m_ImageSimManage.currentImageIndex.ToString());
                                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath+intCobCam.Text+"MSAY方向数据", stringTitles.ToArray(), stringData.ToArray(), "");
                            }
                            else
                            {
                                currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"Y方向已识别的目标数量与设置的数量不符;;;已识别目标数量{num};;;已设置的数量为{Param_MsaCircle.pointYRotateInfos.Count}", m_stuGetColor = HalconColor.red });

                            }


                        }
                        else
                        {
                            currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "Y方向未识别到目标", m_stuGetColor = HalconColor.red });
                            imageReduced?.Dispose();
                        }


                    }

                    if (fROI_XLenght1.Value != 0 && flag)
                    {

                        

                        HOperatorSet.GenRectangle2(out HObject rectangle, Convert.ToDouble(fROI_XCenterY.Value), Convert.ToDouble(fROI_XCenterX.Value), Convert.ToDouble(fROI_XCenterR.Value), Convert.ToDouble(fROI_XLenght1.Value), Convert.ToDouble(fROI_XLenght2.Value));
                        if (Param_MsaCircle.bIsEnabelMobel)
                        {
                            if (!isModelCreat)
                            {
                                MessageBox.Show("请先创建模型");
                                return;
                            }
                            else
                            {
                                try
                                {
                                    HOperatorSet.FindShapeModel(image, modelX_handle, 0, (new HTuple(30)).TupleRad()
                                      , 0.4, 1, 0.5, "least_squares", 0, 0.9, out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Angle,
                                    out HTuple hv_Score);
                                    HOperatorSet.VectorAngleToRigid(Convert.ToDouble(fROI_XCenterY.Value), Convert.ToDouble(fROI_XCenterX.Value), 0, hv_Row,
                                     hv_Column, hv_Angle, out HTuple hv_HomMat2D);
                                    HOperatorSet.AffineTransRegion(rectangle, out rectangle, hv_HomMat2D, "nearest_neighbor");
                                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "X方向模型匹配成功", m_stuGetColor = HalconColor.green });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Y方向模型未匹配成功\n" + ex.Message);
                                    return;
                                }
                            }
                        }

                        currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });
                        HOperatorSet.ReduceDomain(image, rectangle, out HObject imageReduced);
                        HOperatorSet.Threshold(imageReduced, out imageReduced, Param_MsaCircle.Ithreoldmin, Param_MsaCircle.Ithreoldmax);
                        HOperatorSet.Connection(imageReduced, out imageReduced);
                        HOperatorSet.SelectShape(imageReduced, out imageReduced, "height", "and", Param_MsaCircle.fHeightMin, Param_MsaCircle.fHeightMax);
                        HOperatorSet.SelectShape(imageReduced, out imageReduced, "width", "and", Param_MsaCircle.fwidthmin, Param_MsaCircle.fwidthmax);
                        HOperatorSet.SelectShape(imageReduced, out imageReduced, "area", "and", Param_MsaCircle.fAreaMin, Param_MsaCircle.fAreaMax);
                        if (imageReduced?.CountObj() != null && imageReduced?.CountObj() != 0)
                        {
                            HOperatorSet.SortRegion(imageReduced, out imageReduced, "character", "true", "row");
                            HOperatorSet.GenContourRegionXld(imageReduced, out imageReduced, "border");

                            HOperatorSet.FitCircleContourXld(imageReduced, "algebraic", -1, 0, 0, 3, 2,
                            out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Radius, out HTuple hv_StartPhi, out HTuple hv_EndPhi,
                            out HTuple hv_PointOrder);

                            HOperatorSet.GenCircleContourXld(out imageReduced, hv_Row, hv_Column, hv_Radius,
                           hv_StartPhi, hv_EndPhi, hv_PointOrder, 1);
                            currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = imageReduced, m_stuGetColor = HalconColor.green });

                            int num = (int)imageReduced?.CountObj();
                            List<double> double_Y;
                            List<double> double_X;
                            if (Param_MsaCircle.pointYRotateInfos.Count == num)
                            {
                                double_Y = Param_MsaCircle.pointXRotateInfos.Select(x => x.Row).ToList();
                                double_X = Param_MsaCircle.pointXRotateInfos.Select(x => x.Col).ToList();
                                HOperatorSet.VectorToHomMat2d(hv_Column, hv_Row, double_X.ToArray(),
                                double_Y.ToArray(), out HTuple hv_HomMat2D1);
                                HOperatorSet.AffineTransPoint2d(hv_HomMat2D1, hv_Column, hv_Row,
                                out HTuple hv_Qx, out HTuple hv_Qy);
                                List<string> stringTitles = new List<string>();
                                List<string> stringData = new List<string>();
                                for (int i = 1; i < hv_Qx.Length; i++)
                                {
                                    HOperatorSet.DistancePp(hv_Qy[0], hv_Qx[0], hv_Qy[i], hv_Qx[i], out HTuple distance);
                                    currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"X距离{i}:{distance}mm", m_stuGetColor = HalconColor.green });
                                    stringTitles.Add($"X距离{i}");
                                    stringData.Add(distance.D.ToString("0.000"));

                                }
                                stringTitles.Insert(0, "序号");
                                stringData.Insert(0, m_ImageSimManage.currentImageIndex.ToString());
                                currentShowHalconWindow.m_messageShowInfos.Insert(0,new MessageShowInfo() { m_strMessage = $"ID: {m_ImageSimManage.currentImageIndex.ToString()}", m_stuGetColor = HalconColor.green });
                                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + intCobCam.Text + "MSAX方向数据", stringTitles.ToArray(), stringData.ToArray(), "");
                            }
                            else
                            {
                                currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"X方向已识别的目标数量与设置的数量不符;;已识别目标数量{num};;已设置的数量为{Param_MsaCircle.pointYRotateInfos.Count}", m_stuGetColor = HalconColor.red });

                            }


                        }
                        else
                        {
                            currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "X方向未识别到目标", m_stuGetColor = HalconColor.red });
                            imageReduced?.Dispose();
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
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image);

                Execute(Load_Image);

            }
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetNextImage(out HObject Load_Image);
                //GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, true);
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
                //GetFormParam(ref param_CalibTemp);
                Execute(Load_Image, true);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        private void ROI_X方向设置_Click(object sender, EventArgs e)
        {
            try
            {
                HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                HOperatorSet.DrawRectangle2(HWC_Display.HalconWindow, out HTuple row, out HTuple column, out HTuple phi, out HTuple length1, out HTuple length2);

                fROI_XCenterY.Value = Convert.ToDecimal(row.D);
                fROI_XCenterX.Value = Convert.ToDecimal(column.D);
                fROI_XCenterR.Value = Convert.ToDecimal(phi.D);
                fROI_XLenght1.Value = Convert.ToDecimal(length1.D);
                fROI_XLenght2.Value = Convert.ToDecimal(length2.D);

            }
            catch (Exception)
            {

                throw;
            }


        }

        private void ROI_Y方向设置_Click(object sender, EventArgs e)
        {
            try
            {
                HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                HOperatorSet.DrawRectangle2(HWC_Display.HalconWindow, out HTuple row, out HTuple column, out HTuple phi, out HTuple length1, out HTuple length2);

                fROI_YCenterY.Value = Convert.ToDecimal(row.D);
                fROI_YCenterX.Value = Convert.ToDecimal(column.D);
                fROI_YCenterR.Value = Convert.ToDecimal(phi.D);
                fROI_YLenght1.Value = Convert.ToDecimal(length1.D);
                fROI_YLenght2.Value = Convert.ToDecimal(length2.D);

            }
            catch (Exception)
            {

                throw;
            }


        }

        private void intCobCam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                Param_MsaCircle = cameraManager[(CameraEnum)intCobCam.SelectedIndex].param_MsaCircle;
                //Param_MsaCircle.backup(Param_MsaCircle);
                YControlAutobind.Loading<Param_MsaCircle, IYCustomAttribute>(this, Param_MsaCircle);//测试存修改记录
                if (Param_MsaCircle.pointYRotateInfos == null) Param_MsaCircle.pointYRotateInfos = new List<PointRotateInfo>();
                NDataGridView.ChangeRowCount_Add(Param_MsaCircle.pointYRotateInfos, Param_MsaCircle.IYDectectNum);


                if (Param_MsaCircle.pointXRotateInfos == null) Param_MsaCircle.pointXRotateInfos = new List<PointRotateInfo>();
                NDataGridView.ChangeRowCount_Add(Param_MsaCircle.pointXRotateInfos, Param_MsaCircle.IXDectectNum);

                DataToGrid(dataGridViewY, Param_MsaCircle.pointYRotateInfos, Param_MsaCircle.IYDectectNum);
                DataToGrid(dataGridViewX, Param_MsaCircle.pointXRotateInfos, Param_MsaCircle.IXDectectNum);

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //新的实现逻辑
            string error = "";
            //视觉设置
            Param_MsaCircle.backup(Param_MsaCircle);
            Param_MsaCircle.FilePath = Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\";
            error = YControlAutobind.Saveing<Param_MsaCircle, IYCustomAttribute>(this, Param_MsaCircle);
            Param_MsaCircle.saveChangeLog(Param_MsaCircle, String.Format("测试视觉设置:{0} 中 {1}", "MSA圆点", $"{(CameraEnum)intCobCam.SelectedIndex}MSA圆点参数"));
            if (error != "") System.Windows.Forms.MessageBox.Show(error);
            GridToData(dataGridViewY, Param_MsaCircle.pointYRotateInfos);
            GridToData(dataGridViewX, Param_MsaCircle.pointXRotateInfos);
            Param_MsaCircle.Save();

        }

        private void IYDectectNum_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    //             
                    int num = (int)IYDectectNum.Value;
                    NDataGridView.ChangeRowCount(dataGridViewY, num, "0");

                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Error(ex);
                    MessageBox.Show(ex.ToString());
                }
            }
            catch (Exception ee)
            {

               //throw;
            }
        }


        /// <summary>
        /// 将数据填充到表格中
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="data"></param>
        public void DataToGrid(DataGridView dataGridView, List<PointRotateInfo> data, int count)
        {
            if (dataGridView == null) return;
            if (data == null) return;

            NDataGridView.ClearRows(dataGridView);
            int num = data.Count > count ? count : data.Count;

            for (int i = 0; i < num; i++)
            {
                List<string> rows = new List<string>();
                if (dataGridView.Columns.Count > 0)
                {
                    rows.Add(data[i].Row.ToString("F3"));
                }
                if (dataGridView.Columns.Count > 1)
                {
                    rows.Add(data[i].Col.ToString("F3"));
                }


                NDataGridView.AddRow(dataGridView, rows.ToArray(), 100, false);
            }

            num = count - data.Count;
            if (num > 0)
            {
                NDataGridView.ChangeRowCount(dataGridView, count, "0");
            }
        }

        /// <summary>
        /// 将表格数据填充到数据中
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="data"></param>
        public void GridToData(DataGridView dataGridView, List<PointRotateInfo> data)
        {
            if (dataGridView == null) return;
            if (data == null) return;

            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (dataGridView.Rows.Count - data.Count > 0)
                {
                    data.Add(new PointRotateInfo());
                }

                if (dataGridView.Columns.Count > 0)
                {
                    data[i].Row = double.Parse(dataGridView.Rows[i].Cells[0].Value.ToString());
                }
                if (dataGridView.Columns.Count > 1)
                {
                    data[i].Col = double.Parse(dataGridView.Rows[i].Cells[1].Value.ToString());
                }

               

            }
            if (data.Count > dataGridView.Rows.Count)
            {
                for (int j = data.Count - 1; j >= dataGridView.Rows.Count; j--)
                {
                    data.RemoveAt(j);
                }
            }

        }

        private void IXDectectNum_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    //             
                    int num = (int)IXDectectNum.Value;
                    NDataGridView.ChangeRowCount(dataGridViewX, num, "0");

                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Error(ex);
                    MessageBox.Show(ex.ToString());
                }
            }
            catch (Exception ee)
            {

               // throw;
            }
        }
        //判断模型是否创建
        bool isModelCreat = false;
        HTuple modelY_handle=0;
        HTuple modelX_handle = 0;
        HObject modelY = null;
        HObject modelX = null;
        private void btn_CreatModel_Click(object sender, EventArgs e)
        {
            
            if(!Param_MsaCircle.bIsEnabelMobel)
            {
                MessageBox.Show("请先启用模型！");
                return;
            }
            if(Param_MsaCircle.fROI_XLenght1==0 || Param_MsaCircle.fROI_XLenght2==0 || Param_MsaCircle.fROI_YLenght1==0 || Param_MsaCircle.fROI_YLenght2==0)
            {
                MessageBox.Show("请先设置ROI！");
                return;
            }
            modelY?.Dispose();
            modelX?.Dispose();
            modelY_handle = 0;
            modelX_handle = 0;
            m_ImageSimManage.GetCurrentImage(out HObject Load_Image);
            currentShowHalconWindow.Dispose();
            currentShowHalconWindow.m_hImage = Load_Image;
            try
            {
                HOperatorSet.GenRectangle2(out HObject rectangleY, Convert.ToDouble(fROI_YCenterY.Value), Convert.ToDouble(fROI_YCenterX.Value), Convert.ToDouble(fROI_YCenterR.Value), Convert.ToDouble(fROI_YLenght1.Value), Convert.ToDouble(fROI_YLenght2.Value));
                HOperatorSet.ReduceDomain(Load_Image, rectangleY,out modelY);
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangleY, m_stuGetColor = HalconColor.yellow });
                HOperatorSet.GetImageSize(modelY, out HTuple width, out HTuple height);
                HOperatorSet.SetPart(HWC_Image1.HalconWindow, 0, 0, width, height);
                HOperatorSet.DispObj(modelY, HWC_Image1.HalconWindow);
                HOperatorSet.CreateShapeModel(modelY, "auto", 0, (new HTuple(360)).TupleRad()
             , "auto", "auto", "use_polarity", "auto", "auto", out modelY_handle);
                HOperatorSet.GenRectangle2(out HObject rectangleX, Convert.ToDouble(fROI_XCenterY.Value), Convert.ToDouble(fROI_XCenterX.Value), Convert.ToDouble(fROI_XCenterR.Value), Convert.ToDouble(fROI_XLenght1.Value), Convert.ToDouble(fROI_XLenght2.Value));
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangleX, m_stuGetColor = HalconColor.yellow });
                HOperatorSet.ReduceDomain(Load_Image, rectangleX, out modelX);
                HOperatorSet.GetImageSize(modelX, out  width, out  height);
                HOperatorSet.SetPart(HWC_Image2.HalconWindow, 0, 0, width, height);
                HOperatorSet.DispObj(modelX, HWC_Image2.HalconWindow);
                HOperatorSet.CreateShapeModel(modelX, "auto", 0, (new HTuple(360)).TupleRad()
             , "auto", "auto", "use_polarity", "auto", "auto", out modelX_handle);
                isModelCreat = true;
                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                screenshotImage?.Dispose();
                MessageBox.Show("模型创建成功！");
            }
            catch (Exception ex)
            {
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace+"\n(模型创建失败)");
            }
        }

        private void Form_MsaCircle_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                displayWindow?.Dispose();
                currentShowHalconWindow?.Dispose();
                modelX?.Dispose();
                modelY?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
