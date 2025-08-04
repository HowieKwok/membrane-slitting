using Frame_Core;
using HalconDotNet;
using IKapBoardClassLibrary;
using LogTool;
using Mef_VisionFrame;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using OpenTK.Input;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFram
{
    public class BIZ_DataWorker_Pretargeting : IBizWorkerBase
    {
        private BIZ_ThreadManage_Pretargeting bIZ_ThreadManage = new BIZ_ThreadManage_Pretargeting();
        BIZ_Variable_Pretargeting bIZ_Variable = new BIZ_Variable_Pretargeting();
        Form_Pretargeting form;
        OverHangCraft overHangCraft;

        public DisplayWindowClass HWC1;
        public DisplayWindowClass HWC2;
        public DisplayWindowClass HWC3;
        public DisplayWindowClass HWC4;


        IntervalExecution intervalExecution = new IntervalExecution();
        IntervalExecution intervalExecution_Read = new IntervalExecution();
        public object Prod_Lock = new object();

        public BIZ_DataWorker_Pretargeting(Form_Pretargeting _form)
        {

            form = _form;
            UpdateCraft();

            HWC1 = new DisplayWindowClass(form.HWC1, form.TB_PositionAndGray1, form.PB_FitWin1, CameraEnum.Cam1, form.HWC1.Name);
            HWC2 = new DisplayWindowClass(form.HWC2, form.TB_PositionAndGray2, form.PB_FitWin2, CameraEnum.Cam2, form.HWC2.Name);
            HWC3 = new DisplayWindowClass(form.HWC3, form.TB_PositionAndGray3, form.PB_FitWin3, CameraEnum.Cam3, form.HWC3.Name);
            HWC4 = new DisplayWindowClass(form.HWC4, form.TB_PositionAndGray4, form.PB_FitWin4, CameraEnum.Cam4, form.HWC4.Name);

            InitVariable();
            InitForm();

            ThreadManage = bIZ_ThreadManage;

            bIZ_ThreadManage.InitThread();

            bIZ_ThreadManage.thd_ImageShowAndSave.Start(5, "ThdImageShowAndSave");
            bIZ_ThreadManage.thd_ImageShowAndSave.PerformWork -= ImageShowSave;
            bIZ_ThreadManage.thd_ImageShowAndSave.PerformWork += ImageShowSave;

            bIZ_ThreadManage.thd_Communication.Start(500, "ThdCommunication");
            bIZ_ThreadManage.thd_Communication.PerformWork -= Communication;
            bIZ_ThreadManage.thd_Communication.PerformWork += Communication;

        }


        public void WorkerProcess(CamEACornerProduct camEAProduct)
        {
            try
            {
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionVariable;

                //========================================================
                //【1】开始处理数据,在这里处理数据，超时时间为1000ms
                ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, Project.Instance.cutOverlayPLCManage.m_DevParam.TimeOutTime, GetProdResult);

                double pointX = 0;
                double pointY = 0;
                double coord_x = 0;
                double coord_y = 0;
                double coord_angle = 0;
                bool result = false;
                bool coordResult_x = false;
                bool coordResult_y = false;
                bool coordResult_angle = false;

                short returnResult = 0;
                //读取相机拍照信号
                bIZ_Variable.PhotoSignal = ReadPhotoSignal();
                bIZ_Variable.pretargetingBatteryProducts.EAIndex = bIZ_Variable.PhotoSignal;

                if (processExecutionResult == ProcessExecutionResult.Success)
                {
                    //========================================================
                    //【2】处理数据              
                    IntegratedData(out coord_x, out coord_y, out coord_angle, out result, out  pointX, out  pointY, out coordResult_x, out coordResult_y, out coordResult_angle);
                    returnResult = result ? (short)1 : (short)2;

                }
                else if (processExecutionResult == ProcessExecutionResult.Overtime)
                {

                }
                else if (processExecutionResult == ProcessExecutionResult.Exception)
                {

                }


                RowData ReturnResultData = new RowData() { dataStr = "0" };
                if (bIZ_Variable.PhotoSignal > 0)
                {
                    //================================================================
                    //【3.1】发送数据到PLC
                    SendResult((float)coord_x, (float)coord_y, (float)coord_angle, returnResult);
                    ReturnResultData.dataStr = returnResult.ToString();
                    ReturnResultData.resultType = returnResult == 1 ? ResultType.OK : ResultType.NG;
                }


                //================================================================
                //【3】数据显示、保存
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData XOffsetData = new RowData() { dataStr = coord_x.ToString("F3"), resultType = coordResult_x ? ResultType.OK : ResultType.NG };
                RowData YOffsetData = new RowData() { dataStr = coord_y.ToString("F3"), resultType = coordResult_y ? ResultType.OK : ResultType.NG };
                RowData AngleData = new RowData() { dataStr = coord_angle.ToString("F3"), resultType = coordResult_angle ? ResultType.OK : ResultType.NG };
                RowData resultData = new RowData() { dataStr = result ? "OK" : "NG", resultType = result ? ResultType.OK : ResultType.NG };
                RowData PolePlateNumberData = new RowData() { dataStr = bIZ_Variable.PhotoSignal.ToString(), resultType = ResultType.OK };

                showRowDataList.Add(timeRowData);
                showRowDataList.Add(XOffsetData);
                showRowDataList.Add(YOffsetData);
                showRowDataList.Add(AngleData);
                showRowDataList.Add(resultData);
                showRowDataList.Add(PolePlateNumberData);
                showRowDataList.Add(ReturnResultData);


                saveRowDataList = showRowDataList;
                NListView.Add(form.listView1, showRowDataList.ToArray(), true);

                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.PretargetingProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), "预定位");

                //================================================================
                //【4】原图显示、保存
                CamEACornerProduct EAProduct = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1;
                string str1 = $"X偏移:{coord_x.ToString("F3")}mm  #[{bIZ_Variable.YBiasMin}_{bIZ_Variable.YBiasMax}]";
                string str2 = $"Y偏移:{coord_y.ToString("F3")}mm  #[{bIZ_Variable.XBiasMin}_{bIZ_Variable.XBiasMax}]";
                string str3 = $"角度:{coord_angle.ToString("F3")}  #[{bIZ_Variable.AngleBiasMin}_{bIZ_Variable.AngleBiasMax}]";
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = result ? "OK" : "NG", m_stuGetColor = result ? HalconColor.green : HalconColor.red, m_nFontSize = 20 });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = str1, m_stuGetColor = coordResult_x ? HalconColor.green : HalconColor.red });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = str2, m_stuGetColor = coordResult_y ? HalconColor.green : HalconColor.red });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = str3, m_stuGetColor = coordResult_angle ? HalconColor.green : HalconColor.red });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"对角中心X{pointX}", m_stuGetColor = HalconColor.green });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"对角中心Y{pointY}", m_stuGetColor = HalconColor.green });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Cam2X:{bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX}", m_stuGetColor = HalconColor.green });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Cam2Y:{bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY}", m_stuGetColor = HalconColor.green });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Cam3X:{bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX}", m_stuGetColor = HalconColor.green });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Cam3Y:{bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY}", m_stuGetColor = HalconColor.green });
               

                LineInfo currentLine = new LineInfo();
                currentLine.RowBegin = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
                currentLine.ColBegin = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
                currentLine.RowEnd = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
                currentLine.ColEnd = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;

                if (lineInfoTemp != null)
                {
                    HOperatorSet.IntersectionLines(currentLine.RowBegin, currentLine.ColBegin, currentLine.RowEnd, currentLine.ColEnd,
                                              lineInfoTemp.RowBegin, lineInfoTemp.ColBegin, lineInfoTemp.RowEnd, lineInfoTemp.ColEnd, out HTuple row1, out HTuple col1, out HTuple dd);
                    camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Row:{row1}    Col:{col1}", m_stuGetColor = HalconColor.blue });
                }
                else
                {
                    lineInfoTemp = new LineInfo();
                }
                lineInfoTemp.RowBegin = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
                lineInfoTemp.ColBegin = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
                lineInfoTemp.RowEnd = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
                lineInfoTemp.ColEnd = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;




                //初始化变量
                bIZ_Variable.pretargetingBatteryProducts = new PretargetingBatteryProduct();

            }
            catch (Exception ex)
            {
                bIZ_Variable.pretargetingBatteryProducts = new PretargetingBatteryProduct();
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        LineInfo lineInfoTemp = null;
        public void ProcessingData_Cam1(ProductBase camEAProduct1)
        {

            CameraEnum cameraEnum = CameraEnum.Cam1;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;

                //计算图像坐标在世界坐标系下目标点
                double currentPointCamX = camEAProduct.coorEaPoints[0].CPointX;
                double currentPointCamY = camEAProduct.coorEaPoints[0].CPointY;

                if (false)
                {
                    ////非主相机，需要进行坐标转换
                    //CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                    //if (cameraEnum != mainCameraEnum)
                    //{
                    //    //获取当前相机到主相机到的变换矩阵
                    //    bool isSuccess = ImageAlgorithm.GetHomMat2dCompose(Project.Instance.cameraManager[mainCameraEnum].param_Checkerboard.HomMat2DInvert, Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, out HTuple homMat2d);
                    //    if (isSuccess)
                    //    {
                    //        //计算图像坐标在主相机坐标系下目标点
                    //        isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2d, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                    //        if (isSuccess)
                    //        {
                    //            camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    //            camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    //            camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //        }
                    //        else
                    //        {
                    //            Log4netRecord.RunLog.Info($"获取当前相机到主相机坐标失败：{cameraEnum}->{mainCameraEnum}");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Log4netRecord.RunLog.Info($"获取当前相机到主相机变换矩阵失败：{cameraEnum}->{mainCameraEnum}");
                    //    }
                    //}
                    ////主相机，直接赋值
                    //else
                    //{
                    //    camEAProduct.coorEaPoints[0].AffinePointX = currentPointCamX;
                    //    camEAProduct.coorEaPoints[0].AffinePointY = currentPointCamY;
                    //    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //}
                }



                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1 = camEAProduct;
                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.CamFinish = true;


                //综合数据流程
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 ||
                 Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {

                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 ||
                       Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    WorkerProcess(camEAProduct);
                }


                //图像显示
                PrimitiveTransition(CameraEnum.Cam1, camEAProduct.showHalconWindow, true, camEAProduct.CamFrame, out Operation_ImageInfo operation_ImageInfo);

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);

                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                HWC1.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                Log4netRecord.RunLog.Info($"{cameraEnum}处理完成");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam2(ProductBase camEAProduct1)
        {

            CameraEnum cameraEnum = CameraEnum.Cam2;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;

                //计算图像坐标在世界坐标系下目标点
                double currentPointCamX = camEAProduct.coorEaPoints[0].CPointX;
                double currentPointCamY = camEAProduct.coorEaPoints[0].CPointY;

                if (false)
                {
                    ////非主相机，需要进行坐标转换
                    //CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                    //if (cameraEnum != mainCameraEnum)
                    //{
                    //    //获取当前相机到主相机到的变换矩阵
                    //    bool isSuccess = ImageAlgorithm.GetHomMat2dCompose(Project.Instance.cameraManager[mainCameraEnum].param_Checkerboard.HomMat2DInvert, Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, out HTuple homMat2d);
                    //    if (isSuccess)
                    //    {
                    //        //计算图像坐标在主相机坐标系下目标点
                    //        isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2d, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                    //        if (isSuccess)
                    //        {
                    //            camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    //            camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    //            camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //        }
                    //        else
                    //        {
                    //            Log4netRecord.RunLog.Info($"获取当前相机到主相机坐标失败：{cameraEnum}->{mainCameraEnum}");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Log4netRecord.RunLog.Info($"获取当前相机到主相机变换矩阵失败：{cameraEnum}->{mainCameraEnum}");
                    //    }
                    //}
                    ////主相机，直接赋值
                    //else
                    //{
                    //    camEAProduct.coorEaPoints[0].AffinePointX = currentPointCamX;
                    //    camEAProduct.coorEaPoints[0].AffinePointY = currentPointCamY;
                    //    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //}
                }

                //当前图像坐标映射到棋盘格              
                bool isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.HomMat2D, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                //棋盘格映射到平台
                if (isSuccess)
                {
                    camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;

                }
                else
                {
                    Log4netRecord.RunLog.Info($"获取当前相机到棋盘格坐标失败：{cameraEnum}");
                }



                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2 = camEAProduct;
                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.CamFinish = true;

                //图像显示
                PrimitiveTransition(CameraEnum.Cam2, camEAProduct.showHalconWindow, true, camEAProduct.CamFrame, out Operation_ImageInfo operation_ImageInfo);


                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);

                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                HWC2.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);


                Log4netRecord.RunLog.Info($"{cameraEnum}处理完成");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam3(ProductBase camEAProduct1)
        {

            CameraEnum cameraEnum = CameraEnum.Cam3;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;

                //计算图像坐标在世界坐标系下目标点
                double currentPointCamX = camEAProduct.coorEaPoints[0].CPointX;
                double currentPointCamY = camEAProduct.coorEaPoints[0].CPointY;

                if (false)
                {
                    ////非主相机，需要进行坐标转换
                    //CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                    //if (cameraEnum != mainCameraEnum)
                    //{
                    //    //获取当前相机到主相机到的变换矩阵
                    //    bool isSuccess = ImageAlgorithm.GetHomMat2dCompose(Project.Instance.cameraManager[mainCameraEnum].param_Checkerboard.HomMat2DInvert, Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, out HTuple homMat2d);
                    //    if (isSuccess)
                    //    {
                    //        //计算图像坐标在主相机坐标系下目标点
                    //        isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2d, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                    //        if (isSuccess)
                    //        {
                    //            camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    //            camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    //            camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //        }
                    //        else
                    //        {
                    //            Log4netRecord.RunLog.Info($"获取当前相机到主相机坐标失败：{cameraEnum}->{mainCameraEnum}");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Log4netRecord.RunLog.Info($"获取当前相机到主相机变换矩阵失败：{cameraEnum}->{mainCameraEnum}");
                    //    }
                    //}
                    ////主相机，直接赋值
                    //else
                    //{
                    //    camEAProduct.coorEaPoints[0].AffinePointX = currentPointCamX;
                    //    camEAProduct.coorEaPoints[0].AffinePointY = currentPointCamY;
                    //    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //}
                }


                //当前图像坐标映射到棋盘格              
                bool isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.HomMat2D, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                //棋盘格映射到平台
                if (isSuccess)
                {
                    camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;

                }
                else
                {
                    Log4netRecord.RunLog.Info($"获取当前相机到棋盘格坐标失败：{cameraEnum}");
                }


                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3 = camEAProduct;
                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.CamFinish = true;

                //图像显示
                PrimitiveTransition(CameraEnum.Cam3, camEAProduct.showHalconWindow, true, camEAProduct.CamFrame, out Operation_ImageInfo operation_ImageInfo);



                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);

                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC3.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);


                Log4netRecord.RunLog.Info($"{cameraEnum}处理完成");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam4(ProductBase camEAProduct1)
        {

            CameraEnum cameraEnum = CameraEnum.Cam4;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;

                //计算图像坐标在世界坐标系下目标点
                double currentPointCamX = camEAProduct.coorEaPoints[0].CPointX;
                double currentPointCamY = camEAProduct.coorEaPoints[0].CPointY;

                if (false)
                {
                    //非主相机，需要进行坐标转换
                    CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                    if (cameraEnum != mainCameraEnum)
                    {
                        //获取当前相机到主相机到的变换矩阵
                        bool isSuccess = ImageAlgorithm.GetHomMat2dCompose(Project.Instance.cameraManager[mainCameraEnum].param_Checkerboard.HomMat2DInvert, Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, out HTuple homMat2d);
                        if (isSuccess)
                        {
                            //计算图像坐标在主相机坐标系下目标点
                            isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2d, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                            if (isSuccess)
                            {
                                camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                                camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                                camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                            }
                            else
                            {
                                Log4netRecord.RunLog.Info($"获取当前相机到主相机坐标失败：{cameraEnum}->{mainCameraEnum}");
                            }
                        }
                        else
                        {
                            Log4netRecord.RunLog.Info($"获取当前相机到主相机变换矩阵失败：{cameraEnum}->{mainCameraEnum}");
                        }
                    }
                    //主相机，直接赋值
                    else
                    {
                        camEAProduct.coorEaPoints[0].AffinePointX = currentPointCamX;
                        camEAProduct.coorEaPoints[0].AffinePointY = currentPointCamY;
                        camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    }
                }




                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4 = camEAProduct;
                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.CamFinish = true;


                //综合数据流程
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 ||
                 Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    WorkerProcess(camEAProduct);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 ||
                       Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {

                }

                //图像显示
                PrimitiveTransition(CameraEnum.Cam4, camEAProduct.showHalconWindow, true, camEAProduct.CamFrame, out Operation_ImageInfo operation_ImageInfo);

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                HWC4.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                Log4netRecord.RunLog.Info($"{cameraEnum}处理完成");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 综合数据
        /// </summary>
        public void IntegratedData(out double coord_x, out double coord_y, out double coord_angle, out bool result, out double pointX, out double pointY, out bool coordResult_x, out bool coordResult_y, out bool coordResult_angle)
        {
            pointX = 0;
            pointY = 0;
            coord_x = -999;
            coord_y = -999;
            coord_angle = -999;
            result = false;
            coordResult_x = false;
            coordResult_y = false;
            coordResult_angle = false;
            try
            {
                //================================================================
                //【1】处理数据、计算X、Y、角度
                //使用四相机计算
                #region
                if (false)
                {
                    double leftUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointY;
                    double leftUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointX;
                    double rightUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
                    double rightUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
                    double leftDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
                    double leftDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;
                    double rightDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointY;
                    double rightDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointX;

                    if (bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].MainCamCoordGetSucceed &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].MainCamCoordGetSucceed &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].MainCamCoordGetSucceed &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].MainCamCoordGetSucceed)
                    {
                        //计算主相机坐标系到机械手平台坐标系的坐标
                        CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                        bool isSuccess1 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2D, leftUpCol, leftUpRow, out double leftUpColX, out double leftUpRowY);
                        bool isSuccess2 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2D, rightUpCol, rightUpRow, out double rightUpColX, out double rightUpRowY);
                        bool isSuccess3 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2D, leftDownCol, leftDownRow, out double leftDownColX, out double leftDownRowY);
                        bool isSuccess4 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2D, rightDownCol, rightDownRow, out double rightDownColX, out double rightDownRowY);
                        if (isSuccess1 && isSuccess2 && isSuccess3 && isSuccess4)
                        {
                            ImageAlgorithm.Calculate_X_Y_Angle(leftUpRowY, leftUpColX, rightUpRowY, rightUpColX, leftDownRowY, leftDownColX, rightDownRowY, rightDownColX,
                                         out double centreRow, out double centreCol, out double leftUpAngle, out double rightUpAngle, out double leftDownAngle, out double rightDownAngle);

                            //计算一个带旋转的变换矩阵
                            double centreColX = 0;
                            double centreRowY = 0;
                            bool isSuccess = ImageAlgorithm.Calculate_HomMat2dRotate(leftDownAngle, Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX,
                                  Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY, out HTuple homMat2dRotate);
                            if (isSuccess)
                            {
                                //计算旋转后的中心点坐标
                                isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2dRotate, centreCol, centreRow, out centreColX, out centreRowY);

                                if (isSuccess)
                                {
                                    //================================================================
                                    //【2】工艺判断
                                    coord_y = centreRowY + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Row;
                                    coord_x = centreColX + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Col;
                                    coord_angle = leftDownAngle + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Angle;
                                    coord_y = Math.Round(coord_y, 2);
                                    coord_x = Math.Round(coord_x, 2);
                                    coord_angle = Math.Round(coord_angle, 2);

                                    if (coord_y >= bIZ_Variable.YBiasMin &&
                                        coord_y <= bIZ_Variable.YBiasMax)
                                    {
                                        coordResult_y = true;
                                    }
                                    if (coord_x >= bIZ_Variable.XBiasMin &&
                                        coord_x <= bIZ_Variable.XBiasMax)
                                    {
                                        coordResult_x = true;
                                    }
                                    if (coord_angle >= bIZ_Variable.YBiasMin &&
                                        coord_angle <= bIZ_Variable.YBiasMax)
                                    {
                                        coordResult_angle = true;
                                    }
                                    result = coordResult_y && coordResult_x && coordResult_angle;
                                }
                                else
                                {
                                    Log4netRecord.RunLog.Info("计算旋转后的中心点坐标失败");
                                }

                            }
                            else
                            {
                                Log4netRecord.RunLog.Info("计算旋转矩阵失败");
                            }
                        }
                        else
                        {
                            string message = "获取主相机坐标系到机械手平台坐标系的坐标失败--";
                            if (!isSuccess1) message += "获取左上角坐标失败，";
                            if (!isSuccess2) message += "获取右上角坐标失败，";
                            if (!isSuccess3) message += "获取左下角坐标失败，";
                            if (!isSuccess4) message += "获取右下角坐标失败，";

                            Log4netRecord.RunLog.Info(message);
                        }
                    }
                }
                #endregion
                //使用单相机计算
                else
                {


                    foreach (var item in bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints)
                    {
                        if (!item.CPointFinish)
                        {
                            return;
                        }
                    }

                    foreach (var item in bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints)
                    {
                        if (!item.CPointFinish)
                        {
                            return;
                        }
                    }

                    foreach (var item in bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints)
                    {
                        if (!item.CPointFinish)
                        {
                            return;
                        }
                    }

                    foreach (var item in bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints)
                    {
                        if (!item.CPointFinish)
                        {
                            return;
                        }
                    }

                    if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
                    !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
                    {
                        return;
                    }


                    if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
                    !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
                    {
                        return;
                    }

                    CameraEnum mainCameraEnum = overHangCraft.BaseCamera;


                    //单相机交点测试不准
                    if (false)
                    {
                        //double line1_BeginRow = 0;
                        //double line1_BeginCol = 0;
                        //double line1_EndRow = 0;
                        //double line1_EndCol = 0;

                        //line1_BeginRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].RowBegin;
                        //line1_BeginCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].ColBegin;
                        //line1_EndRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].RowEnd;
                        //line1_EndCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].ColEnd;


                        //line2_BeginRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].RowBegin;
                        //line2_BeginCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].ColBegin;
                        //line2_EndRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].RowEnd;
                        //line2_EndCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].ColEnd;

                        ////=========================
                        ////获取图像坐标及角度
                        ////计算两条线的交点
                        //HOperatorSet.IntersectionLines(line1_BeginRow, line1_BeginCol, line1_EndRow, line1_EndCol,
                        //                       line2_BeginRow, line2_BeginCol, line2_EndRow, line2_EndCol,
                        //                       out HTuple pointY, out HTuple pointX, out HTuple isOverlapping);
                    }

                    //获取棋盘格角点
                    double leftUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointY;
                    double leftUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointX;
                    double rightUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
                    double rightUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
                    double leftDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
                    double leftDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;
                    double rightDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointY;
                    double rightDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointX;
                    pointX= (leftDownCol + rightUpCol) / 2;
                    pointY = (leftDownRow + rightUpRow) / 2;

                    double pointX1 = pointX - Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointX;
                    double pointY1 = pointY - Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointY;

                    
                    //bool isSuccess = ImageAlgorithm.Calculate_Angle_ll(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY,
                    //                                 Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX,
                    //                                 Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointY,
                    //                                 Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointX,
                    //                                 pointY, pointX, out HTuple angle);

                    //double lineAngle = angle.D;
                    //计算两条线的角度
                    //水平线
                    double line2_BeginRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].RowBegin;
                    double line2_BeginCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].ColBegin;
                    double line2_EndRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].RowEnd;
                    double line2_EndCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].ColEnd;
                    bool isSuccess = ImageAlgorithm.Calculate_LevelLine_AcuteAngle(line2_BeginRow, line2_BeginCol, line2_EndRow, line2_EndCol, out double lineAngle);

                    lineAngle -=  0.43274619659388458;
                    lineAngle = (int)lineAngle;


                    if (!isSuccess)
                    {
                        Log4netRecord.RunLog.Info("计算两条线的角度失败");
                        return;
                    }
                 
                    ////=================================================
                    ////计算主相机坐标系到机械手平台坐标系的坐标
                    //isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2D, pointX, pointY, out double leftDownColX, out double leftDownRowY);
                    //if (!isSuccess)
                    //{
                    //    Log4netRecord.RunLog.Info("获取主相机坐标系到机械手平台坐标系的坐标失败");
                    //    return;
                    //}

                    //计算一个带旋转的变换矩阵
                    isSuccess = ImageAlgorithm.Calculate_HomMat2dRotate(lineAngle, Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX,
                                  Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY, out HTuple homMat2dRotate);
                    if (!isSuccess)
                    {
                        Log4netRecord.RunLog.Info("计算旋转矩阵失败");
                        return;
                    }

                    //计算旋转后的中心点坐标
                    isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2dRotate, pointX1, pointY1, out double centreColX, out double centreRowY);
                    if (!isSuccess)
                    {
                        Log4netRecord.RunLog.Info("计算旋转后的中心点坐标失败");
                        return;
                    }

                    //是否需要取反
                    if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueX)
                    {
                        centreColX = -centreColX;
                    }
                    if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueY)
                    {
                        centreRowY = -centreRowY;
                    }
                    if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueR)
                    {
                        lineAngle = -lineAngle;
                    }
                    centreRowY = centreRowY + (lineAngle * 0.11);
                    //================================================================
                    //【2】工艺判断
                    double offsetValueY = 0;//补偿值
                    double offsetValueX = 0;
                    double offsetValueR = 0;

                    //1.A下单片
                    //2.A上单片
                    //3.A极片
                    //4.B极片
                    //11.A下单片第二次拍照
                    //12.A上单片第二次拍照
                    //13.A极片第二次拍照
                    //14.B极片第二次拍照
                    if (bIZ_Variable.PhotoSignal == 1 || bIZ_Variable.PhotoSignal == 11)//下单片
                    {
                        offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Row;
                        offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Col;
                        offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Angle;
                    }
                    else if (bIZ_Variable.PhotoSignal == 2 || bIZ_Variable.PhotoSignal == 12)//上单片
                    {
                        offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Row;
                        offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Col;
                        offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Angle;
                    }
                    else if (bIZ_Variable.PhotoSignal == 3 || bIZ_Variable.PhotoSignal == 13 ||
                             bIZ_Variable.PhotoSignal == 4 || bIZ_Variable.PhotoSignal == 14)//极片
                    {
                        offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Row;
                        offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Col;
                        offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Angle;
                    }
                    coord_y = centreRowY + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Row + offsetValueY;
                    coord_x = centreColX + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Col + offsetValueX;
                    coord_angle = lineAngle + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Angle + offsetValueR;
                    coord_y = Math.Round(coord_y, 2);
                    coord_x = Math.Round(coord_x, 2);
                    coord_angle = Math.Round(coord_angle, 2);

                    if (coord_y >= bIZ_Variable.YBiasMin &&
                        coord_y <= bIZ_Variable.YBiasMax)
                    {
                        coordResult_y = true;
                    }
                    if (coord_x >= bIZ_Variable.XBiasMin &&
                        coord_x <= bIZ_Variable.XBiasMax)
                    {
                        coordResult_x = true;
                    }
                    if (coord_angle >= bIZ_Variable.YBiasMin &&
                        coord_angle <= bIZ_Variable.YBiasMax)
                    {
                        coordResult_angle = true;
                    }
                    result = coordResult_y && coordResult_x && coordResult_angle;
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        /// <summary>
        /// 获取产品结果
        /// </summary>
        /// <returns></returns>
        public ProcessExecutionResult GetProdResult()
        {
            try
            {
                //================================================================
                //【1】产品判断         
                lock (Prod_Lock)
                {
                    if (bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.CamFinish == true &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.CamFinish == true &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.CamFinish == true &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.CamFinish == true)
                    {

                        return ProcessExecutionResult.Success;
                    }
                }

                return ProcessExecutionResult.ContinueExecute;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return ProcessExecutionResult.Exception;
            }
        }


        /// <summary>
        /// 将原始图像转换为显示图像
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="result"></param>
        /// <param name="frame"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <param name="operation_ImageInfo"></param>
        private void PrimitiveTransition(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, bool result, long frame, out Operation_ImageInfo operation_ImageInfo)
        {
            operation_ImageInfo = null;
            try
            {
                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;

                string imageSavePath = in_imageSaveParamer.ImageSave_Path_Primitive;
                ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Primitive;
                ImageSaveSelect imageSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_Primitive;
                bool isShowImage = in_imageSaveParamer.IsShowImage_Primitive;
                bool isSaveScreenshot = in_imageSaveParamer.IsSaveImage_Screenshot && !result;
                string screenshotSavePath = in_imageSaveParamer.ImageSave_Path_Screenshot;
                ImageSaveFormat screenshotSaveFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Screenshot;
                string imageSaveName = frame.ToString().PadLeft(5, '0');
                operation_ImageInfo = BIZ_General.ImageDataTypeTransition(showHalconWindow, imageSavePath, imageSaveName, saveImageFormat, imageSaveSelect, isShowImage,
                   isSaveScreenshot, screenshotSavePath, imageSaveName, screenshotSaveFormat, cameraEnum.ToString());
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }


        }

        /// <summary>
        /// 将图像队列转换为显示图像队列
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="imageSaveParamer"></param>
        /// <param name="imageInfosShow"></param>
        /// <param name="ngTypeInfos"></param>
        private void NGImageQueueTransition(CameraEnum cameraEnum, List<NgTypeInfo> ngTypeInfos, int frame, int PackgeNum, int EAIndex, ref QueuePackage<Operation_ImageInfo> out_operationImageInfos)
        {
            try
            {
                if (ngTypeInfos == null || ngTypeInfos.Count == 0) return;

                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;
                //获取显示图像
                int count = ngTypeInfos.Count;
                for (int i = 0; i < count; i++)
                {
                    NgTypeInfo ngTypeInfo = ngTypeInfos[i];
                    string imageSavePath = in_imageSaveParamer.ImageSave_Path_Min;
                    ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Min;
                    bool isSaveImage = in_imageSaveParamer.IsSaveImage_Min;
                    bool isShowImage = in_imageSaveParamer.IsShowImage_Min;
                    string imageSaveName = frame.ToString().PadLeft(5, '0') + $"_{PackgeNum}_{EAIndex}";
                    Operation_ImageInfo operation_ImageInfo = BIZ_General.NGImageDataTypeTransition(ngTypeInfo, imageSavePath, imageSaveName, saveImageFormat, isSaveImage, isShowImage,
                        cameraEnum.ToString());

                    out_operationImageInfos.Enqueue(operation_ImageInfo);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }


        }



        /// <summary>
        /// 读取拍照信号
        /// </summary>
        /// <returns></returns>
        public short ReadPhotoSignal()
        {
            short readValue = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_AAYDW, Project.Instance.cutOverlayPLCManage.CommunicationLog, true, out readValue);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_ABYDW, Project.Instance.cutOverlayPLCManage.CommunicationLog, true, out readValue);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BAYDW, Project.Instance.cutOverlayPLCManage.CommunicationLog, true, out readValue);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BBYDW, Project.Instance.cutOverlayPLCManage.CommunicationLog, true, out readValue);
            }

            return readValue;
        }

        public void SendHeartBeat()
        {
            bool IsIntervalTime = intervalExecution.GetWhetherToReachIntervalTime(1000);
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_AAYDW, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_ABYDW, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BAYDW, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BBYDW, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
            else
            {
                Log4netRecord.ErrorLog.Debug("工位选择错误");
                TJBJInfo.AlarmWriteState(AlarmName.工位选择错误, StopAlarmStateEnum.停机, "");
                return;
            }
        }
        public void SendResult(float OffsetValueX, float OffsetValueY, float OffsetValueR, short PhotoResult)
        {
            if (Project.Instance.Is_simulation) return;

            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {


                //第一次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 0 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 10)
                {
                    //X偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_AAYDW.Value = OffsetValueX;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //Y偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_AAYDW.Value = OffsetValueY;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //角度
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_AAYDW.Value = OffsetValueR;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AAYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                //第二次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 10 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 20)
                {
                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_AAYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }


            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {


                //第一次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 0 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 10)
                {
                    //X偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_ABYDW.Value = OffsetValueX;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //Y偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_ABYDW.Value = OffsetValueY;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //角度
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_ABYDW.Value = OffsetValueR;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_ABYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                //第二次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 10 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 20)
                {
                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_ABYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }


            }

            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {


                //第一次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 0 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 10)
                {
                    //X偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_BAYDW.Value = OffsetValueX;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //Y偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_BAYDW.Value = OffsetValueY;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //角度
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_BAYDW.Value = OffsetValueR;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BAYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                //第二次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 10 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 20)
                {
                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BAYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }


            }

            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {


                //第一次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 0 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 10)
                {
                    //X偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_BBYDW.Value = OffsetValueX;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //Y偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_BBYDW.Value = OffsetValueY;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //角度
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_BBYDW.Value = OffsetValueR;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BBYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                //第二次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 10 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 20)
                {
                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BBYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }


            }
        }

        public void ReadCraft(ReadLogSaveType readLogSaveType)
        {
            try
            {
                if (Project.Instance.Is_simulation)
                {
                    bIZ_Variable.XBiasMax = 10;
                    bIZ_Variable.XBiasMin = -10;
                    bIZ_Variable.YBiasMax = 10;
                    bIZ_Variable.YBiasMin = -10;
                    bIZ_Variable.AngleBiasMax = 10;
                    bIZ_Variable.AngleBiasMin = -10;
                    return;
                }
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {


                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    bIZ_Variable.XBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_AAYDW.Value;
                    bIZ_Variable.XBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_AAYDW.Value;
                    bIZ_Variable.YBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_AAYDW.Value;
                    bIZ_Variable.YBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_AAYDW.Value;
                    bIZ_Variable.AngleBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_AAYDW.Value;
                    bIZ_Variable.AngleBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_AAYDW.Value;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    bIZ_Variable.XBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_ABYDW.Value;
                    bIZ_Variable.XBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_ABYDW.Value;
                    bIZ_Variable.YBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_ABYDW.Value;
                    bIZ_Variable.YBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_ABYDW.Value;
                    bIZ_Variable.AngleBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_ABYDW.Value;
                    bIZ_Variable.AngleBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_ABYDW.Value;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    bIZ_Variable.XBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_BAYDW.Value;
                    bIZ_Variable.XBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_BAYDW.Value;
                    bIZ_Variable.YBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_BAYDW.Value;
                    bIZ_Variable.YBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_BAYDW.Value;
                    bIZ_Variable.AngleBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_BAYDW.Value;
                    bIZ_Variable.AngleBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_BAYDW.Value;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    bIZ_Variable.XBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_BBYDW.Value;
                    bIZ_Variable.XBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_BBYDW.Value;
                    bIZ_Variable.YBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_BBYDW.Value;
                    bIZ_Variable.YBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_BBYDW.Value;
                    bIZ_Variable.AngleBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_BBYDW.Value;
                    bIZ_Variable.AngleBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_BBYDW.Value;
                }
                //overHangCraft.Save();

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        #region 通信
        /// <summary>
        /// 通信
        /// </summary>
        public void Communication()
        {
            try
            {
                if (bIZ_ThreadManage.thd_Communication.ExecuteTime > 20)
                {

                }
                //==========================
                //【】心跳信号
                if (Project.Instance.Is_Run)
                    SendHeartBeat();

                ReadCraft(ReadLogSaveType.ReadDifferentState);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        #endregion


        #region 接口实现

        public AThreadManageBase ThreadManage { get; set; }

        public void InitForm()
        {
            NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
            NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());


            //初始化数据显示界面
            CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionVariable;
            NListView.Init(form.listView1, camDetectionVariable.DataTitlesShow);

            NLabel.UpdateControls(form.LB_Process1, Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process2, Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process3, Project.Instance.cameraManager[CameraEnum.Cam3].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process4, Project.Instance.cameraManager[CameraEnum.Cam4].m_CamParam.CameraName);
        }

        public void ClearForm()
        {
            NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
            NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());


            NListView.DeleteListAllItems(form.listView1);

        }

        public void UpdateForm()
        {

        }

        public void InitVariable()
        {
            bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1?.Dispose();
            bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2?.Dispose();
            bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3?.Dispose();
            bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4?.Dispose();
            bIZ_Variable.pretargetingBatteryProducts = new PretargetingBatteryProduct();
            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化   
                camDetectionVariable.DataTitlesShow = new string[] { "时间", "X偏移", "Y偏移", "角度", "结果", "极片编号", "返回结果" };
                camDetectionVariable.DataTitlesSave = camDetectionVariable.DataTitlesShow.ToArray();

            }
        }

        public void ImageShowSave()
        {
            try
            {
                int disposeImageRetainLenght = 5;
                BIZ_General.Image_Process(HWC1.operation_Image, disposeImageRetainLenght, HWC1);
                BIZ_General.Image_Process(HWC2.operation_Image, disposeImageRetainLenght, HWC2);
                BIZ_General.Image_Process(HWC3.operation_Image, disposeImageRetainLenght, HWC3);
                BIZ_General.Image_Process(HWC4.operation_Image, disposeImageRetainLenght, HWC4);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void DisposeImageAndClearImageQueue()
        {
            HWC1?.Dispose();
            HWC2?.Dispose();
            HWC3?.Dispose();
            HWC4?.Dispose();
        }

        public void SimLoadImage(bool simAutoFlag, ref bool simRunFlag)
        {
            try
            {
                if (simRunFlag)
                {
                    simRunFlag = false;
                    simAutoFlag = false;
                    foreach (ProcessServices item in Project.Instance.m_ProcessList)
                    {
                        Project.Instance.StopRun(item.process.processInfo.ProcessID);
                    }


                    Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                    {
                        bool flag = SimControl.LoadImage_Sim(Project.Instance.cameraManager[(CameraEnum)item.Key].m_ImageSimManage,
                                            ref item.Value.QueueGrabImage, $"{((CameraEnum)item.Key).ToString()}仿真加载图片");
                        if (flag)
                        {
                            (item.Value as AreaCamParam).CallBackImgDataDeal();
                            //(item.Value as AreaCamParam).semaphoreSlimCam.Release();
                            string str = item.Key.ToString() + "信号量--" + (item.Value as AreaCamParam).semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + item.Value.QueueGrabImage.Count.ToString();
                            //  Console.WriteLine(str);

                        }
                    });

                    //拍照信号
                    //bIZ_Variable.PhotoSignal = 1;

                    if (!simAutoFlag)
                    {
                        simRunFlag = false;
                        foreach (ProcessServices item in Project.Instance.m_ProcessList)
                        {
                            Project.Instance.ExecuteOnce(item.process.processInfo.ProcessID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void StartMojuan()
        {
            try
            {
                ReadCraft(ReadLogSaveType.True);
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void StopMojuan()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void StartRun()
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                }


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void StopRun()
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAYDW.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABYDW.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAYDW.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBYDW.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void AlarmReset()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void SendStopAlarm(int index)
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void SendAlarm(int index)
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void UpdateCraft()
        {
            overHangCraft = Project.Instance.craftBase as OverHangCraft;
        }
        #endregion
    }
}
