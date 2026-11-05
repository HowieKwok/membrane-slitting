using ControlBoard.Model;
using ControlBoard.服务类;
using ControlBoard.板卡库;
using DefectRuleClassifier.Core.Models;
using DevMgmt;
using DevMgmt.Cameras.Models;
using DevMgmt.Light.Models;
using Eholly.Lib.Controls;
using Eholly.ZChart.Models;
using EHolly.FormControl;
using Frame_Core;
using Frame_Core.流程汇总文件.产品类;
using FrameCommunity;
using FrameCommunity.PID;
using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using ScottPlot.Drawing.Colormaps;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Mef_VisionFrame
{

    public delegate ExecuteResult<double> SizeComputationalLogicDelegate(int camIndex, SizeCraftStation_DieCuttingCut sizeCraft, SizeDectectProduct sizeDectectProduct);
    public delegate ExecuteResult<double> SizeStationComputationalLogicDelegate(SizeCraftStation_DieCuttingCut sizeCraftStation, SizeDectectProduct sizeDectectProduct4, SizeDectectProduct sizeDectectProduct5, SizeDectectProduct sizeDectectProduct6);
    public class Server_DieCuttingCut : ServerBase
    {
        UC_DieCuttingCut View => ProjectFactory.F_View.ProjectView as UC_DieCuttingCut;
        public Model_DieCuttingCut Data;
        public override event Action RefreshToolView;
        public override event Action InitView;
        public override event Action RefreshView;


        public IODigitalInput DefectStation_RequestSignal = null;

        public IOPulseOutput CutStation_Heartbeat = null;

        public IOPulseOutput DefectStation_Heartbeat = null;
        public IOPulseOutput DefectStation_MinorNGSignal = null;
        public IOPulseOutput DefectStation_SevereNGSignal = null;
        public IOPulseOutput DefectStation_OKSignal = null;
        public IOPulseOutput DefectStation_ContinuousNGSignal = null;



        public IOPulseOutput SizeStation_Heartbeat = null;
        public IOPulseOutput SizeStation_NGSignal = null;
        public IOPulseOutput SizeStation_OKSignal = null;
        public IOPulseOutput SizeStation_ContinuousNGSignal = null;


        IOC0640_OPERATE_DATA iOC0640_OPERATE_DATA = new IOC0640_OPERATE_DATA();
        IOC0640_OPERATE m_IniCallBack;
        public Server_DieCuttingCut(Model_DieCuttingCut _Data) : base(_Data)
        {
            Data = _Data;

            //缺陷相机绑定开始回调事件
            var camObj = Project.Instance.M_CamParam.GetCamObj(0);
            if (camObj != null)
            {

                camObj.FrameStartEvent -= DefectCam_FramStartCallback;
                camObj.FrameStartEvent += DefectCam_FramStartCallback;
                //camObj.CamCallbackStartEvent -= DefectCam_FramStartCallback;
                //camObj.CamCallbackStartEvent += DefectCam_FramStartCallback;
            }
            var cameraRelevancy0 = Service_Camera.Get_CameraRelevancy(0);
            if (cameraRelevancy0?.m_ImageSimManage != null)
            {
                cameraRelevancy0.m_ImageSimManage.CamCallbackStartEvent -= DefectStation_IOSendResult;
                cameraRelevancy0.m_ImageSimManage.CamCallbackStartEvent += DefectStation_IOSendResult;
            }

            //缺陷相机绑定开始回调事件
            var camObj2 = Project.Instance.M_CamParam.GetCamObj(2);
            if (camObj2 != null)
            {
                camObj2.CamCallbackStartEvent -= CutCam_CamCallbackStart;
                camObj2.CamCallbackStartEvent += CutCam_CamCallbackStart;
            }
            var cameraRelevancy2 = Service_Camera.Get_CameraRelevancy(2);
            if (cameraRelevancy2?.m_ImageSimManage != null)
            {
                cameraRelevancy2.m_ImageSimManage.CamCallbackStartEvent -= CutCam_CamCallbackStart;
                cameraRelevancy2.m_ImageSimManage.CamCallbackStartEvent += CutCam_CamCallbackStart;
            }

            //尺寸相机绑定开始回调事件
            var camObj3 = Project.Instance.M_CamParam.GetCamObj(3);
            if (camObj3 != null)
            {
                camObj3.CamCallbackStartEvent -= SizeCam_CamCallbackStart;
                camObj3.CamCallbackStartEvent += SizeCam_CamCallbackStart;
            }
            var cameraRelevancy3 = Service_Camera.Get_CameraRelevancy(3);
            if (cameraRelevancy3?.m_ImageSimManage != null)
            {
                cameraRelevancy3.m_ImageSimManage.CamCallbackStartEvent -= SizeCam_CamCallbackStart;
                cameraRelevancy3.m_ImageSimManage.CamCallbackStartEvent += SizeCam_CamCallbackStart;
            }

            ServiceCardManage.Instance.GetIControlCard().Config_Intbitmode(Project.Instance.M_ParamControlCord.CardNo, 2, 1, 1);
            //ServiceCardManage.Instance.GetIControlCard().Config_Intporten(0, 0, 0xffffffff);
            iOC0640_OPERATE_DATA.cardno = Project.Instance.M_ParamControlCord.CardNo;
            iOC0640_OPERATE_DATA.on_off = 1;
            iOC0640_OPERATE_DATA.bitno = 2;

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(iOC0640_OPERATE_DATA));
            Marshal.StructureToPtr(iOC0640_OPERATE_DATA, ptr, false);

            m_IniCallBack = new IOC0640_OPERATE(IOC0640_OPERATE_Callback);
            ServiceCardManage.Instance.GetIControlCard().Int_Enable(Project.Instance.M_ParamControlCord.CardNo, m_IniCallBack, ptr);



            //状态机初始化
            // DefectStation_RequestSignal = new IODigitalInput(Project.Instance.M_ParamControlCord.CardNo, 2, ServiceCardManage.Instance.GetIControlCard());
            // DefectStation_RequestSignal.InvertLogic(false).OnRising(DefectStation_IOSendResult);

            CutStation_Heartbeat = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 1, ServiceCardManage.Instance.GetIControlCard()).SetIsSaveLog(false);

            DefectStation_Heartbeat = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 2, ServiceCardManage.Instance.GetIControlCard()).SetIsSaveLog(false);
            DefectStation_MinorNGSignal = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 3, ServiceCardManage.Instance.GetIControlCard());
            DefectStation_SevereNGSignal = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 4, ServiceCardManage.Instance.GetIControlCard());
            DefectStation_OKSignal = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 5, ServiceCardManage.Instance.GetIControlCard());
            DefectStation_ContinuousNGSignal = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 6, ServiceCardManage.Instance.GetIControlCard());


            SizeStation_Heartbeat = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 7, ServiceCardManage.Instance.GetIControlCard()).SetIsSaveLog(false);
            SizeStation_NGSignal = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 8, ServiceCardManage.Instance.GetIControlCard());
            SizeStation_OKSignal = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 9, ServiceCardManage.Instance.GetIControlCard());
            SizeStation_ContinuousNGSignal = new IOPulseOutput(Project.Instance.M_ParamControlCord.CardNo, 10, ServiceCardManage.Instance.GetIControlCard());

            Udpate();

            InitThread();
        }

        #region 流程任务



        public void ALineCam_SourceImage_Process(ProductBase productBase)
        {
            DieCutImageProduct dieCutEAProduct = productBase as DieCutImageProduct;
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                HOperatorSet.CountSeconds(out dieCutEAProduct.detectionTime.Data_StartTime);
                int camIndex = 0;
                string camName = Service_Camera.GetCam_Name(camIndex);
                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);

                ShowHalconWindow sourceImage = dieCutEAProduct.showHalconGroup[0];
                ShowHalconWindow bigImage = dieCutEAProduct.showHalconGroup[1];


                sourceImage.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{sourceImage.m_nImageFrameID}" });
                sourceImage.m_bIsNG = false;
                sourceImage.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "原图");
                sourceImage.m_imageSavePathInfo.FileName = sourceImage.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (sourceImage.m_nProductID).ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"); ;
                sourceImage.m_imageSavePathInfo.FileFormat = saveParamConfig.SourceImageSaveParamer.ImageFormat;
                sourceImage.m_imageSavePathInfo.IsShow = saveParamConfig.SourceImageSaveParamer.IsShow;
                sourceImage.m_imageSavePathInfo.IsSave = SaveImageTool.GetIsSave(saveParamConfig.SourceImageSaveParamer.SaveMode, sourceImage.m_bIsNG);

                sourceImage.m_imageSavePathInfo.ScreenshotFileName = sourceImage.m_imageSavePathInfo.FileName;
                sourceImage.m_imageSavePathInfo.ScreenshotFileFormat = saveParamConfig.ScreenshotSaveParamer.ImageFormat;
                sourceImage.m_imageSavePathInfo.IsHighDefinitionCapture = saveParamConfig.ScreenshotSaveParamer.IsHighDefinitionCapture;
                sourceImage.m_imageSavePathInfo.IsScreenshot = SaveImageTool.GetIsSave(saveParamConfig.ScreenshotSaveParamer.SaveMode, sourceImage.m_bIsNG);

                bigImage.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{bigImage.m_nImageFrameID}" });
                bigImage.m_bIsNG = false;
                bigImage.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "大图");
                bigImage.m_imageSavePathInfo.FileName = bigImage.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (bigImage.m_nProductID).ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"); ;
                bigImage.m_imageSavePathInfo.FileFormat = saveParamConfig.BigImageSaveParamer.ImageFormat;
                bigImage.m_imageSavePathInfo.IsShow = saveParamConfig.BigImageSaveParamer.IsShow;
                bigImage.m_imageSavePathInfo.IsSave = SaveImageTool.GetIsSave(saveParamConfig.BigImageSaveParamer.SaveMode, bigImage.m_bIsNG);

                bigImage.m_imageSavePathInfo.ScreenshotFileName = bigImage.m_imageSavePathInfo.FileName;
                bigImage.m_imageSavePathInfo.ScreenshotFileFormat = saveParamConfig.BigScreenshotSaveParamer.ImageFormat;
                bigImage.m_imageSavePathInfo.IsHighDefinitionCapture = saveParamConfig.BigScreenshotSaveParamer.IsHighDefinitionCapture;
                bigImage.m_imageSavePathInfo.IsScreenshot = SaveImageTool.GetIsSave(saveParamConfig.BigScreenshotSaveParamer.SaveMode, bigImage.m_bIsNG);

                HOperatorSet.CountSeconds(out dieCutEAProduct.detectionTime.Data_EndTime);
                var detectionTime = DetectionTime(bigImage.m_nImageFrameID, bigImage.m_nProductID, dieCutEAProduct.detectionTime);
                foreach (var item in detectionTime.DisplayStrings)
                {
                    bigImage.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item });
                }

                FileOperationCSV.WriteData_CSV(Data.ALineBig_Time_Path, Data.ALineBig_Time_Title.ToArray(), detectionTime.DataStrings.ToArray(), camName);

                Data.Cam1_SourceImage_ImageSave.DataQueue.Enqueue(sourceImage);
                Data.Cam1_BigImage_ImageShow.DataQueue.Enqueue(bigImage);
            });
        }

        public void BLineCam_SourceImage_Process(ProductBase productBase)
        {
            DieCutImageProduct dieCutEAProduct = productBase as DieCutImageProduct;
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                HOperatorSet.CountSeconds(out dieCutEAProduct.detectionTime.Data_StartTime);
                int camIndex = 1;
                string camName = Service_Camera.GetCam_Name(camIndex);
                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);

                ShowHalconWindow sourceImage = dieCutEAProduct.showHalconGroup[0];
                ShowHalconWindow bigImage = dieCutEAProduct.showHalconGroup[1];

                sourceImage.m_bIsNG = false;
                sourceImage.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "原图");
                sourceImage.m_imageSavePathInfo.FileName = sourceImage.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (sourceImage.m_nProductID).ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"); ;
                sourceImage.m_imageSavePathInfo.FileFormat = saveParamConfig.SourceImageSaveParamer.ImageFormat;
                sourceImage.m_imageSavePathInfo.IsShow = saveParamConfig.SourceImageSaveParamer.IsShow;
                sourceImage.m_imageSavePathInfo.IsSave = SaveImageTool.GetIsSave(saveParamConfig.SourceImageSaveParamer.SaveMode, sourceImage.m_bIsNG);

                sourceImage.m_imageSavePathInfo.ScreenshotFileName = sourceImage.m_imageSavePathInfo.FileName;
                sourceImage.m_imageSavePathInfo.ScreenshotFileFormat = saveParamConfig.ScreenshotSaveParamer.ImageFormat;
                sourceImage.m_imageSavePathInfo.IsHighDefinitionCapture = saveParamConfig.ScreenshotSaveParamer.IsHighDefinitionCapture;
                sourceImage.m_imageSavePathInfo.IsScreenshot = SaveImageTool.GetIsSave(saveParamConfig.ScreenshotSaveParamer.SaveMode, sourceImage.m_bIsNG);

                bigImage.m_bIsNG = false;
                bigImage.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "大图");
                bigImage.m_imageSavePathInfo.FileName = bigImage.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (bigImage.m_nProductID).ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"); ;
                bigImage.m_imageSavePathInfo.FileFormat = saveParamConfig.BigImageSaveParamer.ImageFormat;
                bigImage.m_imageSavePathInfo.IsShow = saveParamConfig.BigImageSaveParamer.IsShow;
                bigImage.m_imageSavePathInfo.IsSave = SaveImageTool.GetIsSave(saveParamConfig.BigImageSaveParamer.SaveMode, bigImage.m_bIsNG);

                bigImage.m_imageSavePathInfo.ScreenshotFileName = bigImage.m_imageSavePathInfo.FileName;
                bigImage.m_imageSavePathInfo.ScreenshotFileFormat = saveParamConfig.BigScreenshotSaveParamer.ImageFormat;
                bigImage.m_imageSavePathInfo.IsHighDefinitionCapture = saveParamConfig.BigScreenshotSaveParamer.IsHighDefinitionCapture;
                bigImage.m_imageSavePathInfo.IsScreenshot = SaveImageTool.GetIsSave(saveParamConfig.BigScreenshotSaveParamer.SaveMode, bigImage.m_bIsNG);

                HOperatorSet.CountSeconds(out dieCutEAProduct.detectionTime.Data_EndTime);
                var detectionTime = DetectionTime(bigImage.m_nImageFrameID, bigImage.m_nProductID, dieCutEAProduct.detectionTime);
                foreach (var item in detectionTime.DisplayStrings)
                {
                    bigImage.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item });
                }

                FileOperationCSV.WriteData_CSV(Data.BLineBig_Time_Path, Data.BLineBig_Time_Title.ToArray(), detectionTime.DataStrings.ToArray(), camName);

                Data.Cam2_SourceImage_ImageSave.DataQueue.Enqueue(sourceImage);
                Data.Cam2_BigImage_ImageShow.DataQueue.Enqueue(bigImage);
            });
        }


        public void ALineCam_EAImage_Process(ProductBase productBase)
        {
            //【】初始值
            DieCutEAProduct dieCutEAProduct = productBase as DieCutEAProduct;
            int camIndex = 0;
            string camName = Service_Camera.GetCam_Name(camIndex);
            ShowHalconWindow showHalconWindow = null;

            try
            {
                HOperatorSet.CountSeconds(out dieCutEAProduct.detectionTime.Data_StartTime);
                //【】

                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);
                CamTempData camTempData = Data.tempData_DieCuttingCut.GetCamTempDataInternal(camName);

                showHalconWindow = dieCutEAProduct.showHalconGroup[0];
                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{showHalconWindow.m_nImageFrameID}" });
                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"EA号：{showHalconWindow.m_nProductID}" });

                //【】数据处理
                var defectResult = Station1_DefectJudgment(camIndex, dieCutEAProduct, Data.Craft, camTempData, showHalconWindow, Data.Cam1_NGDefectImage_ImageShow);
                showHalconWindow.m_bIsNG = !defectResult.result;


                //【】数据保存与显示
                FileOperationCSV.WriteData_CSV(Data.Station1_ALineCamPath_Defect, Data.Station1_ALineCamTitle_Defect.ToArray(), dieCutEAProduct.productDataResult.DefectData, camName);
                NDataPanelViewModel.Add(View.Dp_ALineCamEADetect, Data.Station1_ALineCamTitle_Defect.ToArray(), dieCutEAProduct.productDataResult.DefectData);


                //【】图像结果存队列
                dieCutEAProduct.productDataResult.m_nImageFrameID = showHalconWindow.m_nImageFrameID;
                dieCutEAProduct.productDataResult.m_nProductID = showHalconWindow.m_nProductID;
                dieCutEAProduct.productDataResult.IsNG = showHalconWindow.m_bIsNG;
                dieCutEAProduct.productDataResult.IsSevereDefect = defectResult.isSevere;

                Data.ALine_DieCutEAProduct.Enqueue(dieCutEAProduct.productDataResult, 100);


                HOperatorSet.CountSeconds(out dieCutEAProduct.detectionTime.Data_EndTime);
                var detectionTime = DetectionTime(showHalconWindow.m_nImageFrameID, showHalconWindow.m_nProductID, dieCutEAProduct.detectionTime);
                foreach (var item in detectionTime.DisplayStrings)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item });
                }
                FileOperationCSV.WriteData_CSV(Data.ALineEA_Time_Path, Data.ALineEA_Time_Title.ToArray(), detectionTime.DataStrings.ToArray(), camName);
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.Message + "\r\n" + ee.StackTrace);
                TJBJInfo.AlarmWriteState(AlarmName.数据处理异常, StopAlarmStateEnum.停机, camName);
            }

            ExecuteMethodClass.ExecuteMethod(() =>
            {
                //【】图像保存与显示
                LineCam_ImageShowAndSave(camIndex, showHalconWindow, Data.Cam1_EAImage_ImageShow);
            });
        }

        public void BLineCam_EAImage_Process(ProductBase productBase)
        {
            DieCutEAProduct dieCutEAProduct = productBase as DieCutEAProduct;
            int camIndex = 1;
            string camName = Service_Camera.GetCam_Name(camIndex);
            ShowHalconWindow showHalconWindow = null;

            try
            {
                HOperatorSet.CountSeconds(out dieCutEAProduct.detectionTime.Data_StartTime);
                //【】


                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);
                showHalconWindow = dieCutEAProduct.showHalconGroup[0];
                CamTempData camTempData = Data.tempData_DieCuttingCut.GetCamTempDataInternal(camName);

                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{showHalconWindow.m_nImageFrameID}" });
                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"EA号：{showHalconWindow.m_nProductID}" });

                //【】数据处理
                var defectResult = Station1_DefectJudgment(camIndex, dieCutEAProduct, Data.Craft, camTempData, showHalconWindow, Data.Cam2_NGDefectImage_ImageShow);
                showHalconWindow.m_bIsNG = !defectResult.result;

                //数据统计
                //if (dieCutEAProduct.highestPriorityNgDefect != null)
                //{
                //    StatisticalDistribution statisticalDistribution = camTempData.GetStatisticalDistribution(dieCutEAProduct.highestPriorityNgDefect.T_Name);
                //    statisticalDistribution.Name = dieCutEAProduct.highestPriorityNgDefect.T_Name;
                //    statisticalDistribution.Count++;
                //}

                //【】数据保存与显示
                FileOperationCSV.WriteData_CSV(Data.Station1_BLineCamPath_Defect, Data.Station1_BLineCamTitle_Defect.ToArray(), dieCutEAProduct.productDataResult.DefectData, camName);
                NDataPanelViewModel.Add(View.Dp_BLineCamEADetect, Data.Station1_BLineCamTitle_Defect.ToArray(), dieCutEAProduct.productDataResult.DefectData);


                //【】图像结果存队列
                dieCutEAProduct.productDataResult.m_nImageFrameID = showHalconWindow.m_nImageFrameID;
                dieCutEAProduct.productDataResult.m_nProductID = showHalconWindow.m_nProductID;
                dieCutEAProduct.productDataResult.IsNG = showHalconWindow.m_bIsNG;
                dieCutEAProduct.productDataResult.IsSevereDefect = defectResult.isSevere;
                Data.BLine_DieCutEAProduct.Enqueue(dieCutEAProduct.productDataResult, 100);



                //UIService.Instance.AddAction               

                HOperatorSet.CountSeconds(out dieCutEAProduct.detectionTime.Data_EndTime);
                var detectionTime = DetectionTime(showHalconWindow.m_nImageFrameID, showHalconWindow.m_nProductID, dieCutEAProduct.detectionTime);
                foreach (var item in detectionTime.DisplayStrings)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item });
                }
                FileOperationCSV.WriteData_CSV(Data.BLineEA_Time_Path, Data.BLineEA_Time_Title.ToArray(), detectionTime.DataStrings.ToArray(), camName);

            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.Message + "\r\n" + ee.StackTrace);
                TJBJInfo.AlarmWriteState(AlarmName.数据处理异常, StopAlarmStateEnum.停机, camName);
            }
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                //【】图像保存与显示
                LineCam_ImageShowAndSave(camIndex, showHalconWindow, Data.Cam2_EAImage_ImageShow);
            });
        }

        public void SizeCam4_Process(ProductBase productBase)
        {
            SizeDectectProduct sizeDectectProduct = productBase as SizeDectectProduct;
            int camIndex = 3;
            string camName = Service_Camera.GetCam_Name(camIndex);
            ShowHalconWindow showHalconWindow = null;
            try
            {
                HOperatorSet.CountSeconds(out sizeDectectProduct.detectionTime.Data_StartTime);
                //【】            
                string cam5Name = Service_Camera.GetCam_Name(4);
                string cam6Name = Service_Camera.GetCam_Name(5);
                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);
                showHalconWindow = sizeDectectProduct.showHalconGroup[0];
                CamTempData camTempData = Data.tempData_DieCuttingCut.GetCamTempDataInternal(camName);
                sizeDectectProduct.CamName = camName;

                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{showHalconWindow.m_nImageFrameID}" });


                //【】数据处理
                bool sizeResult = Station3_SizeJudgment(camIndex, sizeDectectProduct, SizeCam_SizeComputationalLogic, Data, Data.Craft, camTempData, out List<MessageShowInfo> messageShowInfo);
                showHalconWindow.m_messageShowInfos.AddRange(messageShowInfo);
                bool defectResult = Station3_DefectJudgment(camIndex, sizeDectectProduct, Data.Craft, camTempData, showHalconWindow, Data.Cam4_NGDefectImage_ImageShow);

                //【】数据保存与显示
                FileOperationCSV.WriteData_CSV(Data.Station3_Path_Defect, Data.Station3_Title_Defect.ToArray(), sizeDectectProduct.productDataResult.DefectData, camName);
                NDataPanelViewModel.Add(View.Dp_PoleDeleft, Data.Station3_Title_Defect.ToArray(), sizeDectectProduct.productDataResult.DefectData);

                //【】数据汇总
                Submit(camIndex, sizeDectectProduct, Data);
                int SizeStationTimeout = Data.Craft.Get_GlobalVariableItem("SizeStationTimeout").Value.ToInt();
                ProcessExecutionResult result = WaitAndTake(sizeDectectProduct.SN, SizeStationTimeout, Data, out Station3Product station3Product);

                if (station3Product.Cam4_Product == null && station3Product.Cam5_Product == null && station3Product.Cam6_Product == null)
                {
                    result = ProcessExecutionResult.Exception;
                }

                if (result == ProcessExecutionResult.Success)
                {
                    bool sizeStationResult = Station3_SizeJudgment(sizeDectectProduct, station3Product.Cam5_Product, station3Product.Cam6_Product, Station3_SizeComputationalLogic, Data, Data.Craft, Data.tempData_DieCuttingCut, out List<MessageShowInfo> messageShowInfo1);
                    showHalconWindow.m_messageShowInfos.AddRange(messageShowInfo1);

                    bool isNG = !(sizeResult && defectResult && sizeStationResult);
                    showHalconWindow.m_bIsNG = isNG || station3Product.Cam5_Product.showHalconGroup[0].m_bIsNG || station3Product.Cam6_Product.showHalconGroup[0].m_bIsNG;

                }
                else
                {
                    showHalconWindow.m_bIsNG = true;
                    string str = "";
                    if (station3Product.Cam5_Product == null)
                    {
                        station3Product.Cam5_Product = new SizeDectectProduct() { CamName = cam5Name };
                        str += cam5Name;
                    }
                    if (station3Product.Cam6_Product == null)
                    {
                        station3Product.Cam6_Product = new SizeDectectProduct() { CamName = cam6Name };
                        str += $"和{cam6Name}";
                    }
                    if (result == ProcessExecutionResult.Exception) str += "获取数据结果异常";
                    if (result == ProcessExecutionResult.Overtime) str += "获取数据结果超时";

                    TJBJInfo.AlarmWriteState(AlarmName.工位三综合数据超时, StopAlarmStateEnum.报警, $"{str}");
                    MessageNotify.AddErrorLog("工位三超时报警");
                }

                //【】数据显示与保存
                Station3SizeData_ShowAndSave(showHalconWindow.m_nImageFrameID, showHalconWindow.m_bIsNG, sizeDectectProduct, station3Product.Cam5_Product, station3Product.Cam6_Product,
                                             Data.Craft, Data.Station3_Path_Size, Data.Station3_Title_Size.ToArray(), View.Dp_PoleSize);


                //缺陷
                if (sizeDectectProduct.productDataResult.DefectData.Count > 0)
                {
                    DataSaveClass dataSaveClass = new DataSaveClass();
                    dataSaveClass.Source = camName;
                    dataSaveClass.FileSave_Path = Data.Station3_Path_Defect;
                    dataSaveClass.FileSave_Title = Data.Station3_Title_Defect;
                    dataSaveClass.rowDatas = sizeDectectProduct.productDataResult.DefectData;
                    Data.DataSaveQueue.Data.Enqueue(dataSaveClass, 1000);
                    Data.Station3_Defect_DataShowQueue.Enqueue(sizeDectectProduct.productDataResult.DefectData, 1000);
                }


                sizeDectectProduct.productDataResult.m_nImageFrameID = showHalconWindow.m_nImageFrameID;
                sizeDectectProduct.productDataResult.m_nProductID = showHalconWindow.m_nProductID;
                sizeDectectProduct.productDataResult.IsNG = showHalconWindow.m_bIsNG;

                //【】数据统计
                station3Product.IsNG = showHalconWindow.m_bIsNG;
                station3Product.Frame_Cam4 = showHalconWindow.m_nProductID;
                station3Product.Frame_Cam5 = station3Product.Cam5_Product.productDataResult.m_nImageFrameID;
                station3Product.Frame_Cam6 = station3Product.Cam6_Product.productDataResult.m_nImageFrameID;
                Station3_StatisticalData((int)showHalconWindow.m_nProductID, station3Product.IsNG, Data, station3Product);

                //【】添加产品,发送结果至PLC
                Data.Station3_LastResult.SetValue(station3Product.SN, station3Product.IsNG, station3Product.Frame_Cam4, station3Product.Frame_Cam5, station3Product.Frame_Cam6);


                HOperatorSet.CountSeconds(out sizeDectectProduct.detectionTime.Data_EndTime);
                var detectionTime = DetectionTime(showHalconWindow.m_nImageFrameID, showHalconWindow.m_nProductID, sizeDectectProduct.detectionTime);
                foreach (var item in detectionTime.DisplayStrings)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item });
                }
                FileOperationCSV.WriteData_CSV(Data.SizeCam4_Time_Path, Data.SizeCam4_Time_Title.ToArray(), detectionTime.DataStrings.ToArray(), camName);
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.Message + "\r\n" + ee.StackTrace);
                TJBJInfo.AlarmWriteState(AlarmName.数据处理异常, StopAlarmStateEnum.停机, camName);
            }
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                //【】图像保存与显示
                SizeCam_ImageShowAndSave(camIndex, showHalconWindow, Data.Cam4_SourceImage_ImageShow);
            });
        }

        public void SizeCam5_Process(ProductBase productBase)
        {
            SizeDectectProduct sizeDectectProduct = productBase as SizeDectectProduct;
            int camIndex = 4;
            string camName = Service_Camera.GetCam_Name(camIndex);
            ShowHalconWindow showHalconWindow = null;
            try
            {
                HOperatorSet.CountSeconds(out sizeDectectProduct.detectionTime.Data_StartTime);
                //【】

                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);
                showHalconWindow = sizeDectectProduct.showHalconGroup[0];
                CamTempData camTempData = Data.tempData_DieCuttingCut.GetCamTempDataInternal(camName);
                sizeDectectProduct.CamName = camName;

                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{showHalconWindow.m_nImageFrameID}" });

                //【】数据处理
                bool sizeResult = Station3_SizeJudgment(camIndex, sizeDectectProduct, SizeCam_SizeComputationalLogic, Data, Data.Craft, camTempData, out List<MessageShowInfo> messageShowInfo);
                showHalconWindow.m_messageShowInfos.AddRange(messageShowInfo);

                bool defectResult = Station3_DefectJudgment(camIndex, sizeDectectProduct, Data.Craft, camTempData, showHalconWindow, Data.Cam5_NGDefectImage_ImageShow);
                showHalconWindow.m_bIsNG = !(sizeResult && defectResult);

                //【】数据显示与保存
                if (sizeDectectProduct.productDataResult.DefectData.Count > 0)
                {
                    DataSaveClass dataSaveClass = new DataSaveClass();
                    dataSaveClass.Source = camName;
                    dataSaveClass.FileSave_Path = Data.Station3_Path_Defect;
                    dataSaveClass.FileSave_Title = Data.Station3_Title_Defect;
                    dataSaveClass.rowDatas = sizeDectectProduct.productDataResult.DefectData;
                    Data.DataSaveQueue.Data.Enqueue(dataSaveClass, 1000);
                    Data.Station3_Defect_DataShowQueue.Enqueue(sizeDectectProduct.productDataResult.DefectData, 1000);
                }

                sizeDectectProduct.productDataResult.m_nImageFrameID = showHalconWindow.m_nImageFrameID;
                sizeDectectProduct.productDataResult.m_nProductID = showHalconWindow.m_nProductID;
                sizeDectectProduct.productDataResult.IsNG = showHalconWindow.m_bIsNG;

                //【】提交数据
                Submit(camIndex, sizeDectectProduct, Data);


                HOperatorSet.CountSeconds(out sizeDectectProduct.detectionTime.Data_EndTime);
                var detectionTime = DetectionTime(showHalconWindow.m_nImageFrameID, showHalconWindow.m_nProductID, sizeDectectProduct.detectionTime);
                foreach (var item in detectionTime.DisplayStrings)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item });
                }
                FileOperationCSV.WriteData_CSV(Data.SizeCam5_Time_Path, Data.SizeCam5_Time_Title.ToArray(), detectionTime.DataStrings.ToArray(), camName);

            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.Message + "\r\n" + ee.StackTrace);
                TJBJInfo.AlarmWriteState(AlarmName.数据处理异常, StopAlarmStateEnum.停机, camName);
            }
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                //【】图像保存与显示
                SizeCam_ImageShowAndSave(camIndex, showHalconWindow, Data.Cam5_SourceImage_ImageShow);
            });
        }

        public void SizeCam6_Process(ProductBase productBase)
        {
            SizeDectectProduct sizeDectectProduct = productBase as SizeDectectProduct;
            int camIndex = 5;
            string camName = Service_Camera.GetCam_Name(camIndex);
            ShowHalconWindow showHalconWindow = null;
            try
            {
                HOperatorSet.CountSeconds(out sizeDectectProduct.detectionTime.Data_StartTime);
                //【】

                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);
                showHalconWindow = sizeDectectProduct.showHalconGroup[0];
                CamTempData camTempData = Data.tempData_DieCuttingCut.GetCamTempDataInternal(camName);
                sizeDectectProduct.CamName = camName;

                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{showHalconWindow.m_nImageFrameID}" });

                //【】数据处理
                bool sizeResult = Station3_SizeJudgment(camIndex, sizeDectectProduct, SizeCam_SizeComputationalLogic, Data, Data.Craft, camTempData, out List<MessageShowInfo> messageShowInfo);
                showHalconWindow.m_messageShowInfos.AddRange(messageShowInfo);

                bool defectResult = Station3_DefectJudgment(camIndex, sizeDectectProduct, Data.Craft, camTempData, showHalconWindow, Data.Cam6_NGDefectImage_ImageShow);
                showHalconWindow.m_bIsNG = !(sizeResult && defectResult);

                //【】数据显示与保存
                if (sizeDectectProduct.productDataResult.DefectData.Count > 0)
                {
                    DataSaveClass dataSaveClass = new DataSaveClass();
                    dataSaveClass.Source = camName;
                    dataSaveClass.FileSave_Path = Data.Station3_Path_Defect;
                    dataSaveClass.FileSave_Title = Data.Station3_Title_Defect;
                    dataSaveClass.rowDatas = sizeDectectProduct.productDataResult.DefectData;
                    Data.DataSaveQueue.Data.Enqueue(dataSaveClass, 1000);
                    Data.Station3_Defect_DataShowQueue.Enqueue(sizeDectectProduct.productDataResult.DefectData, 1000);
                }

                sizeDectectProduct.productDataResult.m_nImageFrameID = showHalconWindow.m_nImageFrameID;
                sizeDectectProduct.productDataResult.m_nProductID = showHalconWindow.m_nProductID;
                sizeDectectProduct.productDataResult.IsNG = showHalconWindow.m_bIsNG;


                //【】提交数据
                Submit(camIndex, sizeDectectProduct, Data);

                HOperatorSet.CountSeconds(out sizeDectectProduct.detectionTime.Data_EndTime);
                var detectionTime = DetectionTime(showHalconWindow.m_nImageFrameID, showHalconWindow.m_nProductID, sizeDectectProduct.detectionTime);
                foreach (var item in detectionTime.DisplayStrings)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item });
                }
                FileOperationCSV.WriteData_CSV(Data.SizeCam6_Time_Path, Data.SizeCam6_Time_Title.ToArray(), detectionTime.DataStrings.ToArray(), camName);
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.Message + "\r\n" + ee.StackTrace);
                TJBJInfo.AlarmWriteState(AlarmName.数据处理异常, StopAlarmStateEnum.停机, camName);
            }
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                //【】图像保存与显示
                SizeCam_ImageShowAndSave(camIndex, showHalconWindow, Data.Cam6_SourceImage_ImageShow);
            });
        }

        public void 纠偏检测数据处理_Process(ProductBase productBase)
        {
            PosCorrectProduct posCorrectProduct = productBase as PosCorrectProduct;
            int camIndex = 2;
            string camName = Service_Camera.GetCam_Name(camIndex);
            ShowHalconWindow showHalconWindow = null;
            try
            {
                HOperatorSet.CountSeconds(out posCorrectProduct.detectionTime.Data_StartTime);
                //【】

                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);
                showHalconWindow = posCorrectProduct.showHalconGroup[0];
                CamTempData camTempData = Data.tempData_DieCuttingCut.GetCamTempDataInternal(camName);

                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{showHalconWindow.m_nImageFrameID}" });

                //【】数据处理
                double calibValueX = Service_Camera.GetCam_CalibrationValue_X((int)camIndex);
                double calibValueY = Service_Camera.GetCam_CalibrationValue_Y((int)camIndex);
                PointInfo pointInfo = posCorrectProduct.PointResults.Count > 1 ? posCorrectProduct.PointResults[1] : new PointInfo();
                bool 无极耳 = posCorrectProduct.ngDefectResults.Where(p => p.ClassName == "无极耳").Count() > 0;
                if (无极耳) showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"无极耳", m_stuGetColor = HalconColor.red });

                double 基准点X = Data.Craft.Get_GlobalVariableItem(camIndex, "AlignCamBaseCol").Value.ToDouble();
                double 基准点Y = Data.Craft.Get_GlobalVariableItem(camIndex, "AlignCamBaseRow").Value.ToDouble();
                double 最大切刀纠偏量 = Data.Craft.Get_GlobalVariableItem(camIndex, "MaxKnifeCorrection").Value.ToDouble();
                double 纠偏允许偏差X = Data.Craft.Get_GlobalVariableItem(camIndex, "CorrectionToleranceX").Value.ToDouble();
                double 裁切刀补偿值 = Data.Craft.Get_GlobalVariableItem(camIndex, "CutterCompensation").Value.ToDouble();
                double 肩宽补偿值 = Data.Craft.Get_GlobalVariableItem(camIndex, "ShoulderWidthCompensation").Value.ToDouble();
                double 相机到切刀长度 = Data.Craft.Get_GlobalVariableItem(camIndex, "CameraToCutterDistanceNum").Value.ToDouble();
                double 相机到V切刀长度 = Data.Craft.Get_GlobalVariableItem(camIndex, "CameraToVCutterDistanceNum").Value.ToDouble();
                double 极耳夹角公差 = Data.Craft.Get_GlobalVariableItem(camIndex, "极耳夹角公差").Value.ToInt();
                int 寻极耳个数 = Data.Craft.Get_GlobalVariableItem(camIndex, "FindTabSum").Value.ToInt();
                bool 裁切纠偏取反 = Data.Craft.Get_GlobalVariableItem(camIndex, "裁切纠偏取反").Value.ToBool();
                bool V角纠偏取反 = Data.Craft.Get_GlobalVariableItem(camIndex, "V角纠偏取反").Value.ToBool();
                bool V角纠偏启用 = Data.Craft.Get_GlobalVariableItem(camIndex, "VCornerCorrectionEnabled").Value.ToBool();
                double V角补偿值 = Data.Craft.Get_GlobalVariableItem(camIndex, "VCornerCompensation").Value.ToDouble();
                double V角深补偿值 = Data.Craft.Get_GlobalVariableItem(camIndex, "VCornerDepthCompensation").Value.ToDouble();

                double 单片宽度标准值 = Data.Craft.Get_Station3_SizeCraft("单片宽度").StandardValue;

                double X偏差 = 0;
                double Y偏差 = 0;
                if (pointInfo.bIsSuccess)
                {
                    X偏差 = (基准点X - pointInfo.Col) * calibValueX;
                    Y偏差 = 基准点Y - pointInfo.Row * calibValueY;
                }



                if (Math.Abs(X偏差) > 纠偏允许偏差X) X偏差 = 0;

                double V角纠偏值 = V角纠偏启用 ? X偏差 + V角补偿值 + 肩宽补偿值 : 0;
                double 裁切纠偏值 = X偏差 + 裁切刀补偿值 + 肩宽补偿值;


                //【】写入纠偏值
                var cutResult = CutStation_SendResult(camIndex, showHalconWindow.m_nImageFrameID, 无极耳);

                if (cutResult.寻极耳中)
                {
                    ClearCutData(false);
                }
                else if (cutResult.清除队列)
                {
                    ClearCutData(true);
                    CorrecInfo Cut_CorrectPosDealTemp = ExecuteMethodClass.ExecuteMethod(() =>
                    {
                        return Data.Cut_CorrectPosDeal.GetOffsetValueAndRemove();
                    });

                    //获取
                    CorrecInfo V_CorrecInfoTemp = ExecuteMethodClass.ExecuteMethod(() =>
                    {
                        return Data.V_CorrectPosDeal.GetOffsetValueAndRemove(true, Data.Cut_CorrectPosDeal.fRunLength, Cut_CorrectPosDealTemp.Value);
                    });
                }

                if (无极耳) V角纠偏值 = 0;
                if (无极耳) 裁切纠偏值 = 0;
                if (cutResult.寻极耳中 || 裁切纠偏值 > (单片宽度标准值 / 4))
                {
                    裁切纠偏值 = 单片宽度标准值 / 4;
                }
                if (V角纠偏值 > (单片宽度标准值 / 4))
                {
                    V角纠偏值 = 单片宽度标准值 / 4;
                }

                裁切纠偏值 = Math.Round(裁切纠偏值, 3);
                V角纠偏值 = Math.Round(V角纠偏值, 3);
                if (V角纠偏取反) V角纠偏值 = -V角纠偏值;
                if (裁切纠偏取反) 裁切纠偏值 = -裁切纠偏值;

                if (cutResult.寻极耳中)
                {
                    //foreach (var item in Data.Cut_CorrectPosDeal.correcInfos)
                    //{
                    //    item.NotTab = 无极耳;
                    //    item.FindTab = cutResult.寻极耳中;
                    //} 

                    //Data.Cut_CorrecInfo.UpdateData(new CorrecInfo() { Value = 裁切纠偏值, NotTab = 无极耳, FindTab = cutResult.寻极耳中 });
                    //Data.V_CorrecInfo.UpdateData(new CorrecInfo() { NotTab = 无极耳 });

                    //添加
                    if (!Data.Cut_CorrectPosDeal.RefreshValue((int)showHalconWindow.m_nImageFrameID, 裁切纠偏值, 无极耳, cutResult.寻极耳中, 裁切纠偏值))
                    {
                        MessageNotify.AddErrorLog($"获取队列裁切纠偏值，队列超设定片数，当前片数{Data.Cut_CorrectPosDeal.correcInfos.Count},设定片数{Data.Cut_CorrectPosDeal.nPieNum}");
                    }



                    //添加

                    if (!Data.V_CorrectPosDeal.RefreshValue((int)showHalconWindow.m_nImageFrameID, 裁切纠偏值, 无极耳, cutResult.寻极耳中, 裁切纠偏值))
                    {
                        MessageNotify.AddErrorLog($"获取队列V角纠偏值，队列超设定片数，当前片数{Data.V_CorrectPosDeal.correcInfos.Count},设定片数{Data.V_CorrectPosDeal.nPieNum}");
                    }


                }
                else
                {


                    //添加
                    if (!Data.Cut_CorrectPosDeal.RefreshValue((int)showHalconWindow.m_nImageFrameID, 裁切纠偏值, 无极耳, cutResult.寻极耳中))
                    {
                        MessageNotify.AddErrorLog($"获取队列裁切纠偏值，队列超设定片数，当前片数{Data.Cut_CorrectPosDeal.correcInfos.Count},设定片数{Data.Cut_CorrectPosDeal.nPieNum}");
                    }



                    //添加

                    if (!Data.V_CorrectPosDeal.RefreshValue((int)showHalconWindow.m_nImageFrameID, V角纠偏值, 无极耳, cutResult.寻极耳中))
                    {
                        MessageNotify.AddErrorLog($"获取队列V角纠偏值，队列超设定片数，当前片数{Data.V_CorrectPosDeal.correcInfos.Count},设定片数{Data.V_CorrectPosDeal.nPieNum}");
                    }


                }






                //Data.M_CorrectLogic.GetCorrectData(out CorrecInfo cutCorrecInfo, out CorrecInfo vCorrecInfo);
                //Data.M_CorrectLogic.AddCut_CorrecInfo((int)showHalconWindow.m_nImageFrameID, (int)相机到切刀长度, (int)相机到V切刀长度, 无极耳, cutResult.寻极耳中, 裁切纠偏值, V角纠偏值);

                //Data.Cut_CorrecInfo.UpdateData(cutCorrecInfo);
                //Data.V_CorrecInfo.UpdateData(vCorrecInfo);


                //var tempCut = CutStation_SendResult(showHalconWindow.m_nImageFrameID);

                posCorrectProduct.productDataResult.m_nImageFrameID = showHalconWindow.m_nImageFrameID;
                posCorrectProduct.productDataResult.m_nProductID = showHalconWindow.m_nProductID;
                posCorrectProduct.productDataResult.IsNG = showHalconWindow.m_bIsNG;


                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"裁切纠偏值:{裁切纠偏值}" });
                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"V角纠偏值:{V角纠偏值}" });


                //【】数据保存与显示
                List<RowData> rowDatas = new List<RowData>();
                rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{showHalconWindow.m_nImageFrameID}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{裁切纠偏值.ToString("F3")}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{V角纠偏值.ToString("F3")}", resultType = ResultType.OK });

                //rowDatas.Add(new RowData() { dataStr = $"{tempCut.裁切纠偏.nFrameID}", resultType = ResultType.OK });
                //rowDatas.Add(new RowData() { dataStr = $"{tempCut.裁切纠偏.Value}", resultType = ResultType.OK });

                //rowDatas.Add(new RowData() { dataStr = $"{tempCut.V角纠偏.nFrameID}", resultType = ResultType.OK });
                //rowDatas.Add(new RowData() { dataStr = $"{tempCut.V角纠偏.Value}", resultType = ResultType.OK });

                //rowDatas.Add(new RowData() { dataStr = $"{cutCorrecInfo.nFrameID}", resultType = ResultType.OK });
                //rowDatas.Add(new RowData() { dataStr = $"{cutCorrecInfo.Value}", resultType = ResultType.OK });

                //rowDatas.Add(new RowData() { dataStr = $"{vCorrecInfo.nFrameID}", resultType = ResultType.OK });
                //rowDatas.Add(new RowData() { dataStr = $"{vCorrecInfo.Value}", resultType = ResultType.OK });

                rowDatas.Add(new RowData() { dataStr = $"{无极耳}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{cutResult.寻极耳中}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{cutResult.清除队列}", resultType = ResultType.OK });


                rowDatas.Add(new RowData() { dataStr = $"{X偏差.ToString("F3")}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{裁切刀补偿值.ToString("F3")}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{V角补偿值.ToString("F3")}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{肩宽补偿值.ToString("F3")}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{pointInfo.Col.ToString("F3")}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{pointInfo.Row.ToString("F3")}", resultType = ResultType.OK });

                rowDatas.Add(new RowData() { dataStr = $"{MyMetrics.fWorkingSetMBValue.ToString("00000.00")}MB", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{MyMetrics.fCpuValue.ToString("0000.00")}%", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{StateManage.DiskSpace}", resultType = ResultType.OK });



                FileOperationCSV.WriteData_CSV(Data.Station2_CutCamPath, Data.Station2_CutCamTitle.ToArray(), rowDatas.ToArray(), camName);
                NDataPanelViewModel.Add(View.Dp_Cut, Data.Station2_CutCamTitle.ToArray(), rowDatas.ToArray());

                HOperatorSet.CountSeconds(out posCorrectProduct.detectionTime.Data_EndTime);
                var detectionTime = DetectionTime(showHalconWindow.m_nImageFrameID, showHalconWindow.m_nProductID, posCorrectProduct.detectionTime);
                foreach (var item in detectionTime.DisplayStrings)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item });
                }
                FileOperationCSV.WriteData_CSV(Data.Cut_Time_Path, Data.Cut_Time_Title.ToArray(), detectionTime.DataStrings.ToArray(), camName);
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.Message + "\r\n" + ee.StackTrace);
                TJBJInfo.AlarmWriteState(AlarmName.数据处理异常, StopAlarmStateEnum.停机, camName);
            }
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                //【】图像保存与显示
                CutCam_ImageShowAndSave(camIndex, showHalconWindow, Data.Cam3_SourceImage_ImageShow);
            });

        }

        #endregion

        #region 方法工具
        public static ExecuteResult<double> SizeCam_SizeComputationalLogic(int camIndex, SizeCraftStation_DieCuttingCut sizeCraft, SizeDectectProduct sizeDectectProduct)
        {
            ExecuteResult<double> executeResult = new ExecuteResult<double>();
            executeResult.IsSuccess = false;
            executeResult.Value = -999;
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                double calibValueX = Service_Camera.GetCam_CalibrationValue_X((int)camIndex);
                double calibValueY = Service_Camera.GetCam_CalibrationValue_Y((int)camIndex);
                double CalibrateCoeff = sizeCraft.CalibrateCoeff;
                double MeasureOffset = sizeCraft.MeasureOffset;

                string[] DataSource1 = sizeCraft.DataSource1.Split('.');
                string[] DataSource2 = sizeCraft.DataSource2.Split('.');
                CalcFormulaEnum CalcFormula = (CalcFormulaEnum)sizeCraft.CalcFormula;

                string dataSource1_ModelName = DataSource1?.Count() > 0 ? DataSource1[0] : "";
                string dataSource1_ModeOutName = DataSource1?.Count() > 1 ? DataSource1[1] : "";
                string dataSource2_ModelName = DataSource2?.Count() > 0 ? DataSource2[0] : "";
                string dataSource2_ModeOutName = DataSource2?.Count() > 1 ? DataSource2[1] : "";


                if (CalcFormula == CalcFormulaEnum.点线)
                {
                    PointInfo pointInfo = GetPointInfo(sizeDectectProduct.CrossPointInfos, dataSource1_ModelName, dataSource1_ModeOutName);
                    if (pointInfo == null) pointInfo = GetPointInfo(sizeDectectProduct.PointResults, dataSource1_ModelName);
                    if (pointInfo == null) executeResult.Message += "数据源1未找到；";

                    Frame_Core.LineInfo lineInfo = GetLineInfo(sizeDectectProduct.CrossPointInfos, dataSource2_ModelName, dataSource2_ModeOutName);
                    if (lineInfo == null) lineInfo = GetLineInfo(sizeDectectProduct.LineInfosResult, dataSource2_ModelName);
                    if (lineInfo == null) executeResult.Message += "数据源2未找到；";

                    if ((pointInfo?.bIsSuccess ?? false) && (lineInfo?.IsLineSuccessfull ?? false))
                    {
                        if (SizeComputationalFormula.DistancePl(pointInfo, lineInfo, CalibrateCoeff, MeasureOffset, out double val))
                        {
                            executeResult.IsSuccess = true;
                            if (sizeCraft.EnableDataModel) val = TopLevelMode(val, sizeCraft.StandardValue, sizeCraft.CompensateO, sizeCraft.CompensateF);
                            executeResult.Value = val;
                            return executeResult;
                        }
                    }
                    else return executeResult;
                }
                if (CalcFormula == CalcFormulaEnum.点点)
                {
                    PointInfo pointInfo1 = GetPointInfo(sizeDectectProduct.CrossPointInfos, dataSource1_ModelName, dataSource1_ModeOutName);
                    if (pointInfo1 == null) pointInfo1 = GetPointInfo(sizeDectectProduct.PointResults, dataSource1_ModelName);
                    if (pointInfo1 == null) executeResult.Message += "数据源1未找到；";

                    PointInfo pointInfo2 = GetPointInfo(sizeDectectProduct.CrossPointInfos, dataSource2_ModelName, dataSource2_ModeOutName);
                    if (pointInfo2 == null) pointInfo2 = GetPointInfo(sizeDectectProduct.PointResults, dataSource2_ModelName);
                    if (pointInfo2 == null) executeResult.Message += "数据源2未找到；";

                    if ((pointInfo1?.bIsSuccess ?? false) && (pointInfo2?.bIsSuccess ?? false))
                    {
                        if (SizeComputationalFormula.DistancePp(pointInfo1, pointInfo2, CalibrateCoeff, MeasureOffset, out double val))
                        {
                            executeResult.IsSuccess = true;
                            if (sizeCraft.EnableDataModel) val = TopLevelMode(val, sizeCraft.StandardValue, sizeCraft.CompensateO, sizeCraft.CompensateF);
                            executeResult.Value = val;
                            return executeResult;
                        }
                    }
                    else return executeResult;
                }
                else if (CalcFormula == CalcFormulaEnum.线线X || CalcFormula == CalcFormulaEnum.线线Y)
                {
                    SizeDirectionEnum sizeDirectionEnum = CalcFormula == CalcFormulaEnum.线线X ? SizeDirectionEnum.X : SizeDirectionEnum.Y;
                    Frame_Core.LineInfo lineInfo1 = GetLineInfo(sizeDectectProduct.CrossPointInfos, dataSource1_ModelName, dataSource1_ModeOutName);
                    if (lineInfo1 == null) lineInfo1 = GetLineInfo(sizeDectectProduct.LineInfosResult, dataSource1_ModelName);
                    if (lineInfo1 == null) executeResult.Message += "数据源1未找到；";

                    Frame_Core.LineInfo lineInfo2 = GetLineInfo(sizeDectectProduct.CrossPointInfos, dataSource2_ModelName, dataSource2_ModeOutName);
                    if (lineInfo2 == null) lineInfo2 = GetLineInfo(sizeDectectProduct.LineInfosResult, dataSource2_ModelName);
                    if (lineInfo2 == null) executeResult.Message += "数据源2未找到；";

                    if ((lineInfo1?.IsLineSuccessfull ?? false) && (lineInfo2?.IsLineSuccessfull ?? false))
                    {
                        if (SizeComputationalFormula.DistanceLl(lineInfo1, lineInfo2, sizeDirectionEnum, CalibrateCoeff, MeasureOffset, out double val))
                        {
                            executeResult.IsSuccess = true;
                            if (sizeCraft.EnableDataModel) val = TopLevelMode(val, sizeCraft.StandardValue, sizeCraft.CompensateO, sizeCraft.CompensateF);
                            executeResult.Value = val;
                            return executeResult;
                        }
                    }
                    else return executeResult;
                }
                else if (CalcFormula == CalcFormulaEnum.角度)
                {
                    Frame_Core.LineInfo lineInfo1 = GetLineInfo(sizeDectectProduct.CrossPointInfos, dataSource1_ModelName, dataSource1_ModeOutName);
                    if (lineInfo1 == null) lineInfo1 = GetLineInfo(sizeDectectProduct.LineInfosResult, dataSource1_ModelName);
                    if (lineInfo1 == null) executeResult.Message += "数据源1未找到；";

                    Frame_Core.LineInfo lineInfo2 = GetLineInfo(sizeDectectProduct.CrossPointInfos, dataSource2_ModelName, dataSource2_ModeOutName);
                    if (lineInfo2 == null) lineInfo2 = GetLineInfo(sizeDectectProduct.LineInfosResult, dataSource2_ModelName);
                    if (lineInfo2 == null) executeResult.Message += "数据源2未找到；";


                    if ((lineInfo1?.IsLineSuccessfull ?? false) && (lineInfo2?.IsLineSuccessfull ?? false))
                    {
                        if (SizeComputationalFormula.DistanceAngle(lineInfo1, lineInfo2, out double val))
                        {
                            executeResult.IsSuccess = true;
                            if (sizeCraft.EnableDataModel) val = TopLevelMode(val, sizeCraft.StandardValue, sizeCraft.CompensateO, sizeCraft.CompensateF);
                            executeResult.Value = val;
                            return executeResult;
                        }
                    }
                    else return executeResult;
                }
                executeResult.Message = "未检测到算法类型";
                return executeResult;
            });

        }


        public static ExecuteResult<double> Station3_SizeComputationalLogic(SizeCraftStation_DieCuttingCut sizeCraftStation, SizeDectectProduct sizeDectectProduct4, SizeDectectProduct sizeDectectProduct5, SizeDectectProduct sizeDectectProduct6)
        {
            ExecuteResult<double> executeResult = new ExecuteResult<double>();
            executeResult.IsSuccess = false;
            executeResult.Value = -999;
            return ExecuteMethodClass.ExecuteMethod(() =>
             {
                 //double calibValueX = Service_Camera.GetCam_CalibrationValue_X((int)camIndex);
                 //double calibValueY = Service_Camera.GetCam_CalibrationValue_Y((int)camIndex);
                 double CalibrateCoeff = sizeCraftStation.CalibrateCoeff;
                 double MeasureOffset = sizeCraftStation.MeasureOffset;

                 string[] DataSource1 = sizeCraftStation.DataSource1.Split('.');
                 string[] DataSource2 = sizeCraftStation.DataSource2.Split('.');
                 CalcFormulaEnum CalcFormula = (CalcFormulaEnum)sizeCraftStation.CalcFormula;
                 int camId1 = sizeCraftStation.CameraId1;
                 int camId2 = sizeCraftStation.CameraId2;

                 string dataSource1_ModelName = DataSource1?.Count() > 0 ? DataSource1[0] : "";
                 string dataSource1_ModeOutName = DataSource1?.Count() > 1 ? DataSource1[1] : "";
                 string dataSource2_ModelName = DataSource2?.Count() > 0 ? DataSource2[0] : "";
                 string dataSource2_ModeOutName = DataSource2?.Count() > 1 ? DataSource2[1] : "";

                 SizeDectectProduct sizeDectectProduct1 = camId1 == 3 ? sizeDectectProduct4 : camId1 == 4 ? sizeDectectProduct5 : camId1 == 5 ? sizeDectectProduct6 : null;
                 SizeDectectProduct sizeDectectProduct2 = camId2 == 3 ? sizeDectectProduct4 : camId2 == 4 ? sizeDectectProduct5 : camId2 == 5 ? sizeDectectProduct6 : null;

                 if (CalcFormula == CalcFormulaEnum.点线)
                 {
                     PointInfo pointInfo = GetPointInfo(sizeDectectProduct1.CrossPointInfos, dataSource1_ModelName, dataSource1_ModeOutName);
                     if (pointInfo == null) pointInfo = GetPointInfo(sizeDectectProduct1.PointResults, dataSource1_ModelName);
                     if (pointInfo == null) executeResult.Message += "数据源1未找到；";

                     Frame_Core.LineInfo lineInfo = GetLineInfo(sizeDectectProduct2.CrossPointInfos, dataSource2_ModelName, dataSource2_ModeOutName);
                     if (lineInfo == null) lineInfo = GetLineInfo(sizeDectectProduct2.LineInfosResult, dataSource2_ModelName);
                     if (lineInfo == null) executeResult.Message += "数据源2未找到；";

                     if ((pointInfo?.bIsSuccess ?? false) && (lineInfo?.IsLineSuccessfull ?? false))
                     {
                         if (SizeComputationalFormula.DistancePl(pointInfo, lineInfo, CalibrateCoeff, MeasureOffset, out double val))
                         {
                             executeResult.IsSuccess = true;
                             if (sizeCraftStation.EnableDataModel) val = TopLevelMode(val, sizeCraftStation.StandardValue, sizeCraftStation.CompensateO, sizeCraftStation.CompensateF);
                             executeResult.Value = val;
                             return executeResult;
                         }
                     }
                     else return executeResult;
                 }
                 if (CalcFormula == CalcFormulaEnum.点点)
                 {
                     PointInfo pointInfo1 = GetPointInfo(sizeDectectProduct1.CrossPointInfos, dataSource1_ModelName, dataSource1_ModeOutName);
                     if (pointInfo1 == null) pointInfo1 = GetPointInfo(sizeDectectProduct1.PointResults, dataSource1_ModelName);
                     if (pointInfo1 == null) executeResult.Message += "数据源1未找到；";

                     PointInfo pointInfo2 = GetPointInfo(sizeDectectProduct1.CrossPointInfos, dataSource2_ModelName, dataSource2_ModeOutName);
                     if (pointInfo2 == null) pointInfo2 = GetPointInfo(sizeDectectProduct1.PointResults, dataSource2_ModelName);
                     if (pointInfo2 == null) executeResult.Message += "数据源2未找到；";

                     if ((pointInfo1?.bIsSuccess ?? false) && (pointInfo2?.bIsSuccess ?? false))
                     {
                         if (SizeComputationalFormula.DistancePp(pointInfo1, pointInfo2, CalibrateCoeff, MeasureOffset, out double val))
                         {
                             executeResult.IsSuccess = true;
                             if (sizeCraftStation.EnableDataModel) val = TopLevelMode(val, sizeCraftStation.StandardValue, sizeCraftStation.CompensateO, sizeCraftStation.CompensateF);
                             executeResult.Value = val;
                             return executeResult;
                         }
                     }
                     else return executeResult;
                 }
                 else if (CalcFormula == CalcFormulaEnum.线线X || CalcFormula == CalcFormulaEnum.线线Y)
                 {
                     SizeDirectionEnum sizeDirectionEnum = CalcFormula == CalcFormulaEnum.线线X ? SizeDirectionEnum.X : SizeDirectionEnum.Y;
                     Frame_Core.LineInfo lineInfo1 = GetLineInfo(sizeDectectProduct1.CrossPointInfos, dataSource1_ModelName, dataSource1_ModeOutName);
                     if (lineInfo1 == null) lineInfo1 = GetLineInfo(sizeDectectProduct1.LineInfosResult, dataSource1_ModelName);
                     if (lineInfo1 == null) executeResult.Message += "数据源1未找到；";

                     Frame_Core.LineInfo lineInfo2 = GetLineInfo(sizeDectectProduct2.CrossPointInfos, dataSource2_ModelName, dataSource2_ModeOutName);
                     if (lineInfo2 == null) lineInfo2 = GetLineInfo(sizeDectectProduct2.LineInfosResult, dataSource2_ModelName);
                     if (lineInfo2 == null) executeResult.Message += "数据源2未找到；";

                     if ((lineInfo1?.IsLineSuccessfull ?? false) && (lineInfo2?.IsLineSuccessfull ?? false))
                     {
                         if (SizeComputationalFormula.DistanceLl(lineInfo1, lineInfo2, sizeDirectionEnum, CalibrateCoeff, MeasureOffset, out double val))
                         {
                             executeResult.IsSuccess = true;
                             if (sizeCraftStation.EnableDataModel) val = TopLevelMode(val, sizeCraftStation.StandardValue, sizeCraftStation.CompensateO, sizeCraftStation.CompensateF);
                             executeResult.Value = val;
                             return executeResult;
                         }
                     }
                     else return executeResult;
                 }
                 else if (CalcFormula == CalcFormulaEnum.角度)
                 {
                     Frame_Core.LineInfo lineInfo1 = GetLineInfo(sizeDectectProduct1.CrossPointInfos, dataSource1_ModelName, dataSource1_ModeOutName);
                     if (lineInfo1 == null) lineInfo1 = GetLineInfo(sizeDectectProduct1.LineInfosResult, dataSource1_ModelName);
                     if (lineInfo1 == null) executeResult.Message += "数据源1未找到；";

                     Frame_Core.LineInfo lineInfo2 = GetLineInfo(sizeDectectProduct2.CrossPointInfos, dataSource2_ModelName, dataSource2_ModeOutName);
                     if (lineInfo2 == null) lineInfo2 = GetLineInfo(sizeDectectProduct2.LineInfosResult, dataSource2_ModelName);
                     if (lineInfo2 == null) executeResult.Message += "数据源2未找到；";


                     if ((lineInfo1?.IsLineSuccessfull ?? false) && (lineInfo2?.IsLineSuccessfull ?? false))
                     {
                         if (SizeComputationalFormula.DistanceAngle(lineInfo1, lineInfo2, out double val))
                         {
                             executeResult.IsSuccess = true;
                             if (sizeCraftStation.EnableDataModel) val = TopLevelMode(val, sizeCraftStation.StandardValue, sizeCraftStation.CompensateO, sizeCraftStation.CompensateF);
                             executeResult.Value = val;
                             return executeResult;
                         }
                     }
                     else return executeResult;
                 }

                 executeResult.Message = "未检测到算法类型";
                 return executeResult;
             });

        }



        #region 数据统计
        public static void DataStatistical(StatisticalData statisticalData, long sumupCount, bool isNG)
        {
            statisticalData.SumupCount = sumupCount;
            if (isNG) statisticalData.NGCount++;
            statisticalData.OKCount = sumupCount - statisticalData.NGCount; ;

            if (statisticalData.SumupCount != 0)
            {
                statisticalData.Yield = statisticalData.OKCount / statisticalData.SumupCount * 100;
                statisticalData.Yield = Math.Round(statisticalData.Yield, 2);
            }
        }
        public static void Station1_StatisticalData(int totalCount, bool isNG, Model_DieCuttingCut data, Station1Product station1Product)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                ProductDataResult productDataResultA = station1Product.ALine_DieCutEAProduct;
                ProductDataResult productDataResultB = station1Product.BLine_DieCutEAProduct;

                if (isNG) data.DefectStation_ContinuousCount++;
                else data.DefectStation_ContinuousCount = 0;
                //数据统计
                DataStatistical(data.Station1_StatisticalData, totalCount, station1Product.IsNG);

                if (true)
                {
                    //数据分布
                    List<DefectSummary> defectSummaries = new List<DefectSummary>();
                    if (station1Product.ALine_DieCutEAProduct.highestPriorityNgDefect != null)
                    {
                        defectSummaries.Add(station1Product.ALine_DieCutEAProduct.highestPriorityNgDefect);
                    }
                    if (station1Product.BLine_DieCutEAProduct.highestPriorityNgDefect != null)
                    {
                        defectSummaries.Add(station1Product.BLine_DieCutEAProduct.highestPriorityNgDefect);
                    }


                    //优先级汇总
                    if (defectSummaries.Count > 0)
                    {
                        DefectSummary defectSummary = GetHighestPriority(defectSummaries.ToArray());
                        if (defectSummary != null)
                        {
                            string name = defectSummary.T_Name;
                            if (!data.Station1_StatisticalDistribution.TryGetValue(name, out StatisticalDistribution statisticalDistribution))
                            {
                                statisticalDistribution = new StatisticalDistribution();
                                data.Station1_StatisticalDistribution.Add(name, statisticalDistribution);
                            }
                            statisticalDistribution.Name = name;
                            statisticalDistribution.Count++;
                        }
                    }


                    List<string> displayDefects = data.Craft.Get_Station1_DefectNames();

                    // 字典有、工艺没有的缺陷，追加到最后。
                    foreach (var defectName in data.Station1_StatisticalDistribution.Keys)
                    {
                        if (!displayDefects.Contains(defectName))
                        {
                            displayDefects.Add(defectName);
                        }
                    }

                    List<QualityData> qualityDatas = new List<QualityData>();
                    QualityData sum = GetOrCreateItem(qualityDatas, "总数");
                    sum.Count = totalCount;
                    sum.Ratio = "100%";

                    QualityData OK = GetOrCreateItem(qualityDatas, "OK数");
                    OK.Count = (int)data.Station1_StatisticalData.OKCount;
                    OK.Ratio = CalcRatio((int)data.Station1_StatisticalData.OKCount, totalCount);

                    QualityData NG = GetOrCreateItem(qualityDatas, "NG数");
                    NG.Count = (int)data.Station1_StatisticalData.NGCount;
                    NG.Ratio = CalcRatio((int)data.Station1_StatisticalData.NGCount, totalCount);

                    foreach (var item in displayDefects)
                    {
                        int count = data.Station1_StatisticalDistribution.TryGetValue(item, out StatisticalDistribution value)
                          ? value.Count
                          : 0;

                        QualityData qualityData = GetOrCreateItem(qualityDatas, item);
                        qualityData.Count = count;
                        qualityData.Ratio = CalcRatio(count, totalCount);
                    }

                    data.DefectStation_BindingDatasShow.Enqueue(qualityDatas, 1);
                    data.DefectStation_BindingDatasSave.Enqueue(qualityDatas, 1);

                }
            });
        }

        public static void Station3_StatisticalData(int totalCount, bool isNG, Model_DieCuttingCut data, Station3Product station3Product)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                if (isNG) data.SizeStation_ContinuousCount++;
                else data.SizeStation_ContinuousCount = 0;
                //数据统计
                DataStatistical(data.Station3_StatisticalData, totalCount, isNG);

                if (true)
                {
                    //数据分布
                    List<DefectSummary> defectSummaries = new List<DefectSummary>();
                    if (station3Product.Cam4_Product.productDataResult.highestPriorityNgDefect != null)
                    {
                        defectSummaries.Add(station3Product.Cam4_Product.productDataResult.highestPriorityNgDefect);
                    }
                    if (station3Product.Cam5_Product.productDataResult.highestPriorityNgDefect != null)
                    {
                        defectSummaries.Add(station3Product.Cam5_Product.productDataResult.highestPriorityNgDefect);
                    }
                    if (station3Product.Cam6_Product.productDataResult.highestPriorityNgDefect != null)
                    {
                        defectSummaries.Add(station3Product.Cam6_Product.productDataResult.highestPriorityNgDefect);
                    }

                    List<SizeInfo> sizeInfos = new List<SizeInfo>();
                    if (station3Product.Cam4_Product.productDataResult.highestPriorityNgSize != null)
                    {
                        sizeInfos.Add(station3Product.Cam4_Product.productDataResult.highestPriorityNgSize);
                    }
                    if (station3Product.Cam5_Product.productDataResult.highestPriorityNgSize != null)
                    {
                        sizeInfos.Add(station3Product.Cam5_Product.productDataResult.highestPriorityNgSize);
                    }
                    if (station3Product.Cam6_Product.productDataResult.highestPriorityNgSize != null)
                    {
                        sizeInfos.Add(station3Product.Cam6_Product.productDataResult.highestPriorityNgSize);
                    }


                    //优先级汇总
                    if (defectSummaries.Count > 0 || sizeInfos.Count > 0)
                    {
                        DefectSummary defectSummary = GetHighestPriority(defectSummaries.ToArray());
                        SizeInfo sizeInfo = GetHighestPriority(sizeInfos.ToArray());
                        if (defectSummary != null || sizeInfo != null)
                        {

                            string name = defectSummary?.T_Name ?? sizeInfo?.Name;

                            if (!data.Station3_StatisticalDistribution.TryGetValue(name, out StatisticalDistribution statisticalDistribution))
                            {
                                statisticalDistribution = new StatisticalDistribution();
                                data.Station3_StatisticalDistribution.Add(name, statisticalDistribution);
                            }
                            statisticalDistribution.Name = name;
                            statisticalDistribution.Count++;
                        }
                    }


                    List<string> displayDefects = new List<string>();
                    displayDefects.AddRange(data.Craft.Get_Station3_SizeNames());
                    displayDefects.AddRange(data.Craft.Get_Station3_DefectNames());


                    // 字典有、工艺没有的缺陷，追加到最后。
                    foreach (var defectName in data.Station3_StatisticalDistribution.Keys)
                    {
                        if (!displayDefects.Contains(defectName))
                        {
                            displayDefects.Add(defectName);
                        }
                    }



                    List<QualityData> qualityDatas = new List<QualityData>();
                    QualityData sum = GetOrCreateItem(qualityDatas, "总数");
                    sum.Count = totalCount;
                    sum.Ratio = "100%";

                    QualityData OK = GetOrCreateItem(qualityDatas, "OK数");
                    OK.Count = (int)data.Station3_StatisticalData.OKCount;
                    OK.Ratio = CalcRatio((int)data.Station3_StatisticalData.OKCount, totalCount);

                    QualityData NG = GetOrCreateItem(qualityDatas, "NG数");
                    NG.Count = (int)data.Station3_StatisticalData.NGCount;
                    NG.Ratio = CalcRatio((int)data.Station3_StatisticalData.NGCount, totalCount);

                    foreach (var item in displayDefects)
                    {
                        int count = data.Station3_StatisticalDistribution.TryGetValue(item, out StatisticalDistribution value)
                          ? value.Count
                          : 0;

                        QualityData qualityData = GetOrCreateItem(qualityDatas, item);
                        qualityData.Count = count;
                        qualityData.Ratio = CalcRatio(count, totalCount);
                    }

                    data.SizeStation_BindingDatasShow.Enqueue(qualityDatas, 1);
                    data.SizeStation_BindingDatasSave.Enqueue(qualityDatas, 1);

                }
            });
        }

        // 封装查找或创建的方法
        private static QualityData GetOrCreateItem(List<QualityData> qualityDatas, string name)
        {
            var item = qualityDatas.FirstOrDefault(p => p.Name == name);
            if (item == null)
            {
                item = new QualityData() { Name = name };
                qualityDatas.Add(item);
            }
            return item;
        }

        // 计算比率的辅助方法
        private static string CalcRatio(int count, int total)
        {
            if (total == 0 || count == 0) return "0%";
            return Math.Round(count * 100d / total, 2).ToString() + "%";
        }
        #endregion

        #region 极片尺寸工位线程同步
        private static void Submit(int cameraId, SizeDectectProduct product, Model_DieCuttingCut data)
        {
            lock (data._sync)
            {
                Station3Product sizeCamProduct;

                if (!data._pending.TryGetValue(product.SN, out sizeCamProduct))
                {
                    sizeCamProduct = new Station3Product();
                    data._pending.Add(product.SN, sizeCamProduct);
                }

                sizeCamProduct.SetProduct(cameraId, product);


                // 最后到达的相机负责通知相机 4
                if (sizeCamProduct.IsComplete)
                {
                    data.Completed.Set();
                }
            }
        }

        private static ProcessExecutionResult WaitAndTake(string sn, int timeoutMs, Model_DieCuttingCut data, out Station3Product sizeCamProduct)
        {
            lock (data._sync)
            {
                if (!data._pending.TryGetValue(sn, out sizeCamProduct))
                    return ProcessExecutionResult.Exception;
            }

            if (!data.Completed.WaitOne(timeoutMs))
            {

                lock (data._sync)
                {
                    // 取出并删除，确保同一个 SN 只汇总一次
                    if (!data._pending.Remove(sn))
                        return ProcessExecutionResult.Exception;
                }
                return ProcessExecutionResult.Overtime;
            }

            lock (data._sync)
            {
                // 取出并删除，确保同一个 SN 只汇总一次
                if (!data._pending.Remove(sn))
                    return ProcessExecutionResult.Exception;
            }
            return ProcessExecutionResult.Success;
        }
        public static void Station3SizeData_ShowAndSave(long frame, bool isNG, SizeDectectProduct sizeDectectProduct4, SizeDectectProduct sizeDectectProduct5, SizeDectectProduct sizeDectectProduct6,
                                            CraftParam_DieCuttingCut craft, string path, string[] Titles, DynamicDataPanel dataPanelViewModel)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                List<RowData> strData = new List<RowData>
                        {
                            new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK },
                            new RowData() { dataStr = $"{frame}", resultType =  isNG ?ResultType.NG:ResultType.OK },
                         };

                List<SizeCraftStation_DieCuttingCut> sizeCraftStation_DieCuttingCuts = craft.Get_Station3_SizeCrafts();

                foreach (var item in sizeCraftStation_DieCuttingCuts)
                {
                    //item.SizeCraftStation_DieCuttingCutToSizeInfo();

                    SizeInfo sizeInfo = sizeDectectProduct4.productDataResult.sizeInfos.Find(p => p.Name == item.Name);

                    if (sizeInfo == null) sizeInfo = sizeDectectProduct5.productDataResult.sizeInfos.Find(p => p.Name == item.Name);
                    if (sizeInfo == null) sizeInfo = sizeDectectProduct6.productDataResult.sizeInfos.Find(p => p.Name == item.Name);

                    strData.Add(new RowData() { dataStr = (sizeInfo?.DataResult ?? double.NaN).ToString(), resultType = (sizeInfo?.resultType ?? ResultType.NG) });
                }


                FileOperationCSV.WriteData_CSV(path, Titles, strData.ToArray(), "工位三");

                NDataPanelViewModel.Add(dataPanelViewModel, Titles, strData.ToArray());
            });
        }


        #endregion



        #region 
        static (List<string> DisplayStrings, List<string> DataStrings) DetectionTime(long farmeID, long eaID, DetectionTime detectionTime)
        {
            List<string> displayStrings = new List<string>();
            List<string> dataStrings = new List<string>();

            double imageTime = ((detectionTime.Image_EndTime - detectionTime.Image_StartTime) * 1000).D;//总耗时
            double dataTime = ((detectionTime.Data_EndTime - detectionTime.Data_StartTime) * 1000).D;//总耗时
            double sumTime = ((detectionTime.Data_EndTime - detectionTime.Image_StartTime) * 1000).D;//总耗时
            displayStrings.Add("图像处理：" + imageTime.ToString("0.00") + "ms");
            displayStrings.Add("数据处理：" + dataTime.ToString("0.00") + "ms");
            displayStrings.Add("总处理时长：" + sumTime.ToString("0.00") + "ms");

            dataStrings.Add(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"));
            dataStrings.Add(farmeID.ToString());
            dataStrings.Add(eaID.ToString());
            dataStrings.Add(imageTime.ToString("0.00"));
            dataStrings.Add(dataTime.ToString("0.00"));
            dataStrings.Add(sumTime.ToString("0.00"));

            return (displayStrings, dataStrings);
        }

        static PointInfo GetPointInfo(List<PointInfo> pointInfos, string modelName)
        {
            if (pointInfos == null || pointInfos.Count == 0 || string.IsNullOrEmpty(modelName))
                return null;

            return pointInfos.FirstOrDefault(item => item.ModuleName == modelName);
        }

        static PointInfo GetPointInfo(List<OutCrossGroup> outCrossGroups, string modelName, string modelOutName)
        {
            if (outCrossGroups == null || outCrossGroups.Count == 0 || string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(modelOutName))
                return null;

            OutCrossGroup outCrossGroup = outCrossGroups.FirstOrDefault(item => item.ModuleName == modelName);
            if (outCrossGroup == null) return null;
            if (modelOutName == "交点") return outCrossGroup.point;
            return null;
        }
        static Frame_Core.LineInfo GetLineInfo(List<OutCrossGroup> outCrossGroups, string modelName, string modelOutName)
        {
            if (outCrossGroups == null || outCrossGroups.Count == 0 || string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(modelOutName))
                return null;


            OutCrossGroup outCrossGroup = outCrossGroups.FirstOrDefault(item => item.ModuleName == modelName);
            if (outCrossGroup == null) return null;
            if (modelOutName == "抓边1") return outCrossGroup.LineInfo1;
            if (modelOutName == "抓边2") return outCrossGroup.LineInfo2;

            return null;
        }
        static Frame_Core.LineInfo GetLineInfo(List<Frame_Core.LineInfo> lineInfos, string modelName)
        {
            if (lineInfos == null || lineInfos.Count == 0 || string.IsNullOrEmpty(modelName))
                return null;

            return lineInfos.FirstOrDefault(item => item.strMoudleName == modelName);
        }

        /// <summary>
        /// 返回最高优先级缺陷信息
        /// </summary>
        /// <param name="defectSummaries"></param>
        /// <returns></returns>
        public static DefectSummary GetHighestPriority(params DefectSummary[] defectSummaries)
        {
            return defectSummaries.Where(item => item.T_IsNG == true).OrderBy(item => item.T_DefectLevel).FirstOrDefault();
        }
        /// <summary>
        /// 返回最高优先级尺寸信息
        /// </summary>
        /// <param name="defectSummaries"></param>
        /// <returns></returns>
        public static SizeInfo GetHighestPriority(params SizeInfo[] sizeInfo)
        {
            return sizeInfo.Where(item => item.IsNG).OrderBy(item => item.Level).FirstOrDefault();
        }

        private static bool LineCam_ImageShowAndSave(int camIndex, ShowHalconWindow showHalconWindow, ImageQueueData eaImage)
        {
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                string camName = Service_Camera.GetCam_Name(camIndex);
                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);

                showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = showHalconWindow.m_bIsNG ? "NG" : "OK", m_stuGetColor = showHalconWindow.m_bIsNG ? HalconColor.red : HalconColor.green, m_nFontSize = 40 });

                if (showHalconWindow.m_bIsNG)
                {
                    showHalconWindow.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "EA图", "NG");
                    showHalconWindow.m_imageSavePathInfo.ScreenshotPath = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "EA截图", "NG");
                }
                else
                {
                    showHalconWindow.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "EA图", "OK");
                    showHalconWindow.m_imageSavePathInfo.ScreenshotPath = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "EA截图", "OK");
                }
                showHalconWindow.m_imageSavePathInfo.FileName = showHalconWindow.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (showHalconWindow.m_nProductID).ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"); ;
                showHalconWindow.m_imageSavePathInfo.FileFormat = saveParamConfig.EAImageSaveParamer.ImageFormat;
                showHalconWindow.m_imageSavePathInfo.IsShow = saveParamConfig.EAImageSaveParamer.IsShow;
                showHalconWindow.m_imageSavePathInfo.IsSave = SaveImageTool.GetIsSave(saveParamConfig.EAImageSaveParamer.SaveMode, showHalconWindow.m_bIsNG);

                showHalconWindow.m_imageSavePathInfo.ScreenshotFileName = showHalconWindow.m_imageSavePathInfo.FileName;
                showHalconWindow.m_imageSavePathInfo.ScreenshotFileFormat = saveParamConfig.EAScreenshotSaveParamer.ImageFormat;
                showHalconWindow.m_imageSavePathInfo.IsHighDefinitionCapture = saveParamConfig.EAScreenshotSaveParamer.IsHighDefinitionCapture;
                showHalconWindow.m_imageSavePathInfo.IsScreenshot = SaveImageTool.GetIsSave(saveParamConfig.EAScreenshotSaveParamer.SaveMode, showHalconWindow.m_bIsNG);

                eaImage.DataQueue.Enqueue(showHalconWindow);

                return true;
            });
        }
        private static bool CutCam_ImageShowAndSave(int camIndex, ShowHalconWindow showHalconWindow, ImageQueueData sourceImage)
        {
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                string camName = Service_Camera.GetCam_Name(camIndex);
                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);

                showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = showHalconWindow.m_bIsNG ? "NG" : "OK", m_stuGetColor = showHalconWindow.m_bIsNG ? HalconColor.red : HalconColor.green, m_nFontSize = 40 });

                if (showHalconWindow.m_bIsNG)
                {
                    showHalconWindow.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "原图", "NG");
                    showHalconWindow.m_imageSavePathInfo.ScreenshotPath = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "截图", "NG");
                }
                else
                {
                    showHalconWindow.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "原图", "OK");
                    showHalconWindow.m_imageSavePathInfo.ScreenshotPath = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "截图", "OK");
                }
                showHalconWindow.m_imageSavePathInfo.FileName = showHalconWindow.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (showHalconWindow.m_nProductID).ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"); ;
                showHalconWindow.m_imageSavePathInfo.FileFormat = saveParamConfig.SourceImageSaveParamer.ImageFormat;
                showHalconWindow.m_imageSavePathInfo.IsShow = saveParamConfig.SourceImageSaveParamer.IsShow;
                showHalconWindow.m_imageSavePathInfo.IsSave = SaveImageTool.GetIsSave(saveParamConfig.SourceImageSaveParamer.SaveMode, showHalconWindow.m_bIsNG);

                showHalconWindow.m_imageSavePathInfo.ScreenshotFileName = showHalconWindow.m_imageSavePathInfo.FileName;
                showHalconWindow.m_imageSavePathInfo.ScreenshotFileFormat = saveParamConfig.ScreenshotSaveParamer.ImageFormat;
                showHalconWindow.m_imageSavePathInfo.IsHighDefinitionCapture = saveParamConfig.ScreenshotSaveParamer.IsHighDefinitionCapture;
                showHalconWindow.m_imageSavePathInfo.IsScreenshot = SaveImageTool.GetIsSave(saveParamConfig.ScreenshotSaveParamer.SaveMode, showHalconWindow.m_bIsNG);

                sourceImage.DataQueue.Enqueue(showHalconWindow);

                return true;
            });
        }

        private static bool SizeCam_ImageShowAndSave(int camIndex, ShowHalconWindow showHalconWindow, ImageQueueData sourceImage)
        {
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                string camName = Service_Camera.GetCam_Name(camIndex);
                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);

                showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = showHalconWindow.m_bIsNG ? "NG" : "OK", m_stuGetColor = showHalconWindow.m_bIsNG ? HalconColor.red : HalconColor.green, m_nFontSize = 40 });

                if (showHalconWindow.m_bIsNG)
                {
                    showHalconWindow.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "原图", "NG");
                    showHalconWindow.m_imageSavePathInfo.ScreenshotPath = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "截图", "NG");
                }
                else
                {
                    showHalconWindow.m_imageSavePathInfo.Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "原图", "OK");
                    showHalconWindow.m_imageSavePathInfo.ScreenshotPath = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "截图", "OK");
                }
                showHalconWindow.m_imageSavePathInfo.FileName = showHalconWindow.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"); ;
                showHalconWindow.m_imageSavePathInfo.FileFormat = saveParamConfig.SourceImageSaveParamer.ImageFormat;
                showHalconWindow.m_imageSavePathInfo.IsShow = saveParamConfig.SourceImageSaveParamer.IsShow;
                showHalconWindow.m_imageSavePathInfo.IsSave = SaveImageTool.GetIsSave(saveParamConfig.SourceImageSaveParamer.SaveMode, showHalconWindow.m_bIsNG);

                showHalconWindow.m_imageSavePathInfo.ScreenshotFileName = showHalconWindow.m_imageSavePathInfo.FileName;
                showHalconWindow.m_imageSavePathInfo.ScreenshotFileFormat = saveParamConfig.ScreenshotSaveParamer.ImageFormat;
                showHalconWindow.m_imageSavePathInfo.IsHighDefinitionCapture = saveParamConfig.ScreenshotSaveParamer.IsHighDefinitionCapture;
                showHalconWindow.m_imageSavePathInfo.IsScreenshot = SaveImageTool.GetIsSave(saveParamConfig.ScreenshotSaveParamer.SaveMode, showHalconWindow.m_bIsNG);

                sourceImage.DataQueue.Enqueue(showHalconWindow);

                return true;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sizeDectectProduct"></param>
        /// <param name="sizeComputationalLogicDelegate"></param>
        /// <param name="craft"></param>
        /// <param name="tempData"></param>
        /// <param name="showHalconWindow"></param>
        /// <returns>结果ture为OK</returns>
        private static bool Station3_SizeJudgment(SizeDectectProduct sizeDectectProduct4, SizeDectectProduct sizeDectectProduct5, SizeDectectProduct sizeDectectProduct6, SizeStationComputationalLogicDelegate sizeComputationalLogicDelegate,
                                    Model_DieCuttingCut modelData, CraftParam_DieCuttingCut craft, TempData_DieCuttingCut tempData, out List<MessageShowInfo> messageShowInfos)
        {
            messageShowInfos = new List<MessageShowInfo>();
            List<MessageShowInfo> lmessageShowInfos = messageShowInfos;
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                bool isNG = false;
                List<SizeCraftStation_DieCuttingCut> dieCuttingCuts = craft.Get_Station3_SizeCrafts_CamDifferent();
                #region   //【1】数据处理    
                if (dieCuttingCuts != null)
                {
                    foreach (var item in dieCuttingCuts)
                    {
                        SizeInfo sizeInfo = item.SizeCraftStation_DieCuttingCutToSizeInfo();
                        try
                        {
                            ExecuteResult<double> executeResult = sizeComputationalLogicDelegate?.Invoke(item, sizeDectectProduct4, sizeDectectProduct5, sizeDectectProduct6);
                            if (executeResult == null) { sizeInfo.DataResult = -999; }
                            else sizeInfo.DataResult = executeResult.IsSuccess ? executeResult.Value : -999;

                            sizeInfo.IsOverspecification = !ConditionalJudgment(sizeInfo.DataResult, sizeInfo.StandardValue, sizeInfo.upperLimitValue, sizeInfo.lowerLimitValue);

                            if (sizeInfo.IsOverspecification && item.DBEnable)
                            {
                                sizeInfo.IsNG = true;
                            }
                            else
                            {
                                sizeInfo.IsNG = false;
                            }
                            isNG |= sizeInfo.IsNG;

                            sizeInfo.resultType = GetResultType(sizeInfo.IsOverspecification, sizeInfo.IsNG);

                            SizeTypeTempData sizeTypeTempData = tempData.GetStation3SizeTypeTempData(sizeInfo.Name);
                            if (sizeInfo.IsNG)
                            {
                                sizeTypeTempData.ContinuousCount++;
                                sizeTypeTempData.AccumulateCount++;
                                StopAlarmJudgment(sizeTypeTempData, item);
                            }
                            else
                            {
                                sizeTypeTempData.ContinuousCount = 0;
                            }
                            lmessageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{sizeInfo.Name}：{sizeInfo.DataResult}({sizeInfo.lowerLimitValue}_{sizeInfo.upperLimitValue})  {executeResult.Message}", m_stuGetColor = GetHalconColor(sizeInfo.IsOverspecification, sizeInfo.IsNG) });

                            //曲线图
                            if (modelData.chartUIUser.ContainsKey(sizeInfo.Name))
                            {
                                modelData.chartUIUser[sizeInfo.Name].UpdateCurve("当前值", sizeInfo.DataResult);
                            }

                            //【】PID
                            if (sizeInfo.Name == "极耳左肩宽")
                            {
                                PID(modelData, sizeInfo.DataResult, sizeInfo.StandardValue);
                            }
                        }
                        catch (Exception ex) { MessageNotify.AddErrorLog(ex.Message + "\r\n" + ex.StackTrace); }

                        sizeDectectProduct4.productDataResult.sizeInfos.Add(sizeInfo);



                    }
                    sizeDectectProduct4.productDataResult.highestPriorityNgSize = GetHighestPriority(sizeDectectProduct4.productDataResult.sizeInfos.ToArray());
                }
                #endregion

                return !isNG;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camIndex"></param>
        /// <param name="sizeDectectProduct"></param>
        /// <param name="sizeComputationalLogicDelegate"></param>
        /// <param name="craft"></param>
        /// <param name="tempData"></param>
        /// <param name="showHalconWindow"></param>
        /// <returns>结果ture为OK</returns>
        private static bool Station3_SizeJudgment(int camIndex, SizeDectectProduct sizeDectectProduct, SizeComputationalLogicDelegate sizeComputationalLogicDelegate,
                                      Model_DieCuttingCut modelData, CraftParam_DieCuttingCut craft, CamTempData tempData, out List<MessageShowInfo> messageShowInfos)
        {
            messageShowInfos = new List<MessageShowInfo>();
            List<MessageShowInfo> lmessageShowInfos = messageShowInfos;
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                bool isNG = false;

                string camName = Service_Camera.GetCam_Name(camIndex);
                List<SizeCraftStation_DieCuttingCut> sizeCrafts = craft.Get_Station3_SizeCrafts(camIndex);
                #region   //【1】数据处理    
                if (sizeCrafts != null)
                {
                    foreach (var item in sizeCrafts)
                    {
                        SizeInfo sizeInfo = item.SizeCraftStation_DieCuttingCutToSizeInfo();
                        try
                        {
                            ExecuteResult<double> executeResult = sizeComputationalLogicDelegate?.Invoke(camIndex, item, sizeDectectProduct);
                            if (executeResult == null) { sizeInfo.DataResult = -999; }
                            else sizeInfo.DataResult = executeResult.IsSuccess ? executeResult.Value : -999;

                            sizeInfo.IsOverspecification = !ConditionalJudgment(sizeInfo.DataResult, sizeInfo.StandardValue, sizeInfo.upperLimitValue, sizeInfo.lowerLimitValue);

                            if (sizeInfo.IsOverspecification && item.DBEnable)
                            {
                                sizeInfo.IsNG = true;
                            }
                            else
                            {
                                sizeInfo.IsNG = false;
                            }
                            isNG |= sizeInfo.IsNG;

                            sizeInfo.resultType = GetResultType(sizeInfo.IsOverspecification, sizeInfo.IsNG);

                            SizeTypeTempData sizeTypeTempData = tempData.GetSizeTypeTempData(sizeInfo.Name);
                            StatisticalDistribution statisticalDistribution = tempData.GetStatisticalDistribution(sizeInfo.Name);
                            if (sizeInfo.IsNG)
                            {
                                sizeTypeTempData.ContinuousCount++;
                                sizeTypeTempData.AccumulateCount++;
                                //statisticalDistribution.Count++;
                                StopAlarmJudgment(sizeTypeTempData, item);

                            }
                            else
                            {
                                sizeTypeTempData.ContinuousCount = 0;
                            }

                            lmessageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{sizeInfo.Name}：{sizeInfo.DataResult}({sizeInfo.lowerLimitValue}_{sizeInfo.upperLimitValue})  {executeResult.Message}", m_stuGetColor = GetHalconColor(sizeInfo.IsOverspecification, sizeInfo.IsNG) });

                            //曲线图
                            if (modelData.chartUIUser.ContainsKey(sizeInfo.Name))
                            {
                                modelData.chartUIUser[sizeInfo.Name].UpdateCurve("当前值", sizeInfo.DataResult);
                            }

                            //【】PID
                            if (sizeInfo.Name == "极耳左肩宽")
                            {
                                PID(modelData, sizeInfo.DataResult, sizeInfo.StandardValue);
                            }

                        }
                        catch (Exception ex) { MessageNotify.AddErrorLog(camName + "\r\n" + ex.Message + "\r\n" + ex.StackTrace); }


                        sizeDectectProduct.productDataResult.sizeInfos.Add(sizeInfo);




                    }
                    sizeDectectProduct.productDataResult.highestPriorityNgSize = GetHighestPriority(sizeDectectProduct.productDataResult.sizeInfos.ToArray());
                }
                #endregion

                return !isNG;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camIndex"></param>
        /// <param name="sizeDectectProduct"></param>
        /// <param name="craft"></param>
        /// <param name="tempData"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="ngImage"></param>
        /// <returns>结果ture为OK</returns>
        private static bool Station3_DefectJudgment(int camIndex, SizeDectectProduct sizeDectectProduct, CraftParam_DieCuttingCut craft, CamTempData tempData, ShowHalconWindow showHalconWindow, ImageQueueData ngImage)
        {
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                string camName = Service_Camera.GetCam_Name(camIndex);
                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);


                #region   //【1】数据处理
                sizeDectectProduct.productDataResult.DefectData.Clear();
                foreach (var item in sizeDectectProduct.ngDefectResults)
                {
                    try
                    {

                        string defectName = item.ClassName;
                        item.Area = Math.Round(item.Area, 2);
                        item.Height = Math.Round(item.Height, 2);
                        item.Width = Math.Round(item.Width, 2);

                        DefectCraft_DieCuttingCut defectCraft = craft.Get_Station3_DefectCrafts(defectName);
                        string noCraftMessage = "";
                        if (defectCraft == null)
                        {
                            item.T_IsOverspecification = true;
                            item.T_IsNG = true;
                            item.T_Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "缺陷小图", "无工艺", defectName);
                            noCraftMessage = "未找到工艺类型";
                            MessageNotify.AddErrorLog($"{noCraftMessage}：{defectName}");
                        }
                        else
                        {
                            item.T_IsOverspecification = defectCraft.HaveOrNot ? true : DefectResultJudgment(item, defectCraft);
                            if (item.T_IsOverspecification && defectCraft.DB)
                            {
                                item.T_IsNG = true;
                                item.T_Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "缺陷小图", "NG", defectName);
                            }
                            else
                            {
                                item.T_IsNG = false;
                                item.T_Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "缺陷小图", "OK", defectName);
                            }
                        }

                        item.T_nImgIndex = showHalconWindow.m_nImageFrameID;
                        item.T_iProductIndex = showHalconWindow.m_nProductID;
                        item.T_FileName = item.T_nImgIndex.ToString().PadLeft(5, '0') + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + $"(A[{item.Area.ToString("0.00")}],W[{item.Width.ToString("0.00")}],H[{item.Height.ToString("0.00")}])";
                        item.T_FileFormat = SaveImageTool.GetImageFormat(saveParamConfig.NGImageSaveParamer.ImageFormat);
                        item.T_IsSave = saveParamConfig.NGImageSaveParamer.IsSave;



                        showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = item.Region, m_stuGetColor = GetHalconColor(item.T_IsOverspecification, item.T_IsNG) });
                        showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{defectName}：面积（{item.Area}）宽度（{item.Width}）高度（{item.Height}）{noCraftMessage}", m_stuGetColor = GetHalconColor(item.T_IsOverspecification, item.T_IsNG) });

                        ShowHalconWindow defectInfo = NgDefectResultToShowHalconWindow(item, camName);
                        defectInfo.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = camName });
                        defectInfo.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{defectName}：面积（{item.Area}）宽度（{item.Width}）高度（{item.Height}）{noCraftMessage}", m_stuGetColor = GetHalconColor(item.T_IsOverspecification, item.T_IsNG) });
                        defectInfo.m_imageSavePathInfo.Path = item.T_Path;
                        defectInfo.m_imageSavePathInfo.FileName = item.T_FileName;
                        defectInfo.m_imageSavePathInfo.FileFormat = saveParamConfig.NGImageSaveParamer.ImageFormat;
                        defectInfo.m_imageSavePathInfo.IsShow = saveParamConfig.NGImageSaveParamer.IsShow;
                        defectInfo.m_imageSavePathInfo.IsSave = saveParamConfig.NGImageSaveParamer.IsSave;
                        ngImage.DataQueue.Enqueue(defectInfo);


                        ResultType resultType = GetResultType(item.T_IsOverspecification, item.T_IsNG);
                        List<RowData> strDeftData = new List<RowData>
                        {
                            new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK },
                            new RowData() { dataStr = $"{camName}", resultType = resultType },
                            new RowData() { dataStr = $"{item.T_nImgIndex}", resultType = resultType },
                            //new RowData() { dataStr = $"{item.T_iProductIndex}", resultType = resultType },
                            new RowData() { dataStr = item.ClassName.ToString(), resultType =  resultType },
                            new RowData() { dataStr =item.Area.ToString("0.00"), resultType = resultType },
                            new RowData() { dataStr =item.Width.ToString("0.00"), resultType = resultType },
                            new RowData() { dataStr =item.Height.ToString("0.00"), resultType = resultType },
                            new RowData() { dataStr =item.T_IsOverspecification.ToString(), resultType = resultType },
                            new RowData() { dataStr =item.T_IsNG.ToString(), resultType = resultType },
                            new RowData() { dataStr =item.T_IsSave.ToString(), resultType = resultType },
                            new RowData() { dataStr = Path.Combine( item.T_Path,item.T_FileName+item.T_FileFormat), resultType = resultType },
                            };

                        sizeDectectProduct.productDataResult.DefectData.Add(strDeftData);
                    }
                    catch (Exception ee)
                    {
                        MessageNotify.AddErrorLog(camName + "\r\n" + ee.Message + "\r\n" + ee.StackTrace);
                        item.Dispose();
                    }

                }


                List<DefectSummary> defectSummaries = sizeDectectProduct.ngDefectResults
                                                  .GroupBy(item => item.ClassName)
                                                  .Select(group =>
                                                  {
                                                      NgDefectResult maxAreaNgItem = group
                                                          .Where(item => item.T_IsNG == true)
                                                          .OrderByDescending(item => item.Area)
                                                          .FirstOrDefault();

                                                      return new DefectSummary
                                                      {
                                                          T_TotalCount = group.Count(),
                                                          T_NgCount = group.Count(item => item.T_IsNG == true),
                                                          T_Name = maxAreaNgItem?.ClassName ?? "",
                                                      };
                                                  })
                                                  .ToList();

                if (defectSummaries != null)
                {
                    foreach (var item in defectSummaries)
                    {
                        if (string.IsNullOrEmpty(item.T_Name)) { item.T_IsNG = false; continue; }
                        string defectName = item.T_Name;

                        DefectCraft_DieCuttingCut defectCraft = craft.Get_Cam_DefectCraft(camIndex, defectName);

                        if (defectCraft != null)
                        {
                            item.T_DefectLevel = defectCraft.DefectLevel;
                            item.T_IsNG = TotalDefectResultJudgment(item, defectCraft);
                            DefectTypeTempData defectTypeTempData = tempData.GetDefectTypeTempData(defectName);
                            if (item.T_IsNG)
                            {
                                defectTypeTempData.ContinuousCount++;
                                defectTypeTempData.AccumulateCount++;
                                StopAlarmJudgment(camName, defectTypeTempData, defectCraft);
                            }
                            else
                            {
                                defectTypeTempData.ContinuousCount = 0;
                            }
                        }
                        else
                        {
                            item.T_DefectLevel = 10000;
                            item.T_IsNG = true;
                        }

                        StatisticalDistribution statisticalDistribution = tempData.GetStatisticalDistribution(defectName);
                        if (item.T_IsNG)
                        {
                            //statisticalDistribution.Count++;
                        }
                    }
                }


                #endregion

                bool defectIsNG = false;
                if (defectSummaries != null)
                {
                    defectIsNG = defectSummaries.Where(p => p.T_IsNG == true).Count() > 0;
                    sizeDectectProduct.productDataResult.highestPriorityNgDefect = GetHighestPriority(defectSummaries.ToArray());

                }
                return !defectIsNG;
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camIndex"></param>
        /// <param name="dieCutEAProduct"></param>
        /// <param name="craft"></param>
        /// <param name="tempData"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="ngImage"></param>
        /// <returns>result结果ture为OK,isSevere是否重度缺陷</returns>
        private static (bool result, bool isSevere) Station1_DefectJudgment(int camIndex, DieCutEAProduct dieCutEAProduct, CraftParam_DieCuttingCut craft, CamTempData tempData, ShowHalconWindow showHalconWindow, ImageQueueData ngImage)
        {
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                string camName = Service_Camera.GetCam_Name(camIndex);
                ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);

                #region   //【1】数据处理
                dieCutEAProduct.productDataResult.DefectData.Clear();
                foreach (var item in dieCutEAProduct.ngDefectResults)
                {
                    try
                    {
                        string defectName = item.ClassName;
                        item.Area = Math.Round(item.Area, 2);
                        item.Height = Math.Round(item.Height, 2);
                        item.Width = Math.Round(item.Width, 2);

                        DefectCraft_DieCuttingCut defectCraft = craft.Get_Station1_DefectCrafts(defectName);
                        string noCraftMessage = "";
                        if (defectCraft == null)
                        {
                            item.T_IsOverspecification = true;
                            item.T_IsNG = true;
                            item.T_Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "缺陷小图", "无工艺", defectName);
                            noCraftMessage = "未找到工艺类型";
                            MessageNotify.AddErrorLog($"{noCraftMessage}：{defectName}");
                        }
                        else
                        {
                            item.T_IsSevere = defectCraft.IsSevere;
                            item.T_IsOverspecification = defectCraft.HaveOrNot ? true : DefectResultJudgment(item, defectCraft);
                            if (item.T_IsOverspecification && defectCraft.DB)
                            {
                                item.T_IsNG = true;
                                item.T_Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "缺陷小图", "NG", defectName);
                            }
                            else
                            {
                                item.T_IsNG = false;
                                item.T_Path = Path.Combine(Project.Instance.paramPathSet.ProductMojuanPath, camName, "缺陷小图", "OK", defectName);
                            }
                        }

                        item.T_nImgIndex = showHalconWindow.m_nImageFrameID;
                        item.T_iProductIndex = showHalconWindow.m_nProductID;
                        item.T_FileName = item.T_nImgIndex.ToString().PadLeft(5, '0') + "_" + item.T_iProductIndex.ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + $"(A[{item.Area.ToString()}],W[{item.Width.ToString()}],H[{item.Height.ToString()}])";
                        item.T_FileFormat = SaveImageTool.GetImageFormat(saveParamConfig.NGImageSaveParamer.ImageFormat);
                        item.T_IsSave = saveParamConfig.NGImageSaveParamer.IsSave;

                        showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = item.Region, m_stuGetColor = GetHalconColor(item.T_IsOverspecification, item.T_IsNG) });
                        showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{defectName}：面积（{item.Area}）宽度（{item.Width}）高度（{item.Height}）{noCraftMessage}", m_stuGetColor = GetHalconColor(item.T_IsOverspecification, item.T_IsNG) });

                        ShowHalconWindow defectInfo = NgDefectResultToShowHalconWindow(item, camName);
                        defectInfo.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = camName });
                        defectInfo.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{defectName}：面积（{item.Area}）宽度（{item.Width}）高度（{item.Height}）{noCraftMessage}", m_stuGetColor = GetHalconColor(item.T_IsOverspecification, item.T_IsNG) });
                        defectInfo.m_imageSavePathInfo.Path = item.T_Path;
                        defectInfo.m_imageSavePathInfo.FileName = item.T_FileName;
                        defectInfo.m_imageSavePathInfo.FileFormat = saveParamConfig.NGImageSaveParamer.ImageFormat;
                        defectInfo.m_imageSavePathInfo.IsShow = saveParamConfig.NGImageSaveParamer.IsShow;
                        defectInfo.m_imageSavePathInfo.IsSave = saveParamConfig.NGImageSaveParamer.IsSave;
                        ngImage.DataQueue.Enqueue(defectInfo);


                        ResultType resultType = GetResultType(item.T_IsOverspecification, item.T_IsNG);
                        List<RowData> strDeftData = new List<RowData>
                        {
                            new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK },
                            new RowData() { dataStr = $"{item.T_nImgIndex}", resultType = resultType },
                            new RowData() { dataStr = $"{item.T_iProductIndex}", resultType = resultType },
                            new RowData() { dataStr = item.ClassName.ToString(), resultType =  resultType },
                            new RowData() { dataStr =item.Area.ToString(), resultType = resultType },
                            new RowData() { dataStr =item.Width.ToString(), resultType = resultType },
                            new RowData() { dataStr =item.Height.ToString(), resultType = resultType },
                            new RowData() { dataStr =item.T_IsOverspecification.ToString(), resultType = resultType },
                            new RowData() { dataStr =item.T_IsSevere.ToString(), resultType = resultType },
                            new RowData() { dataStr =item.T_IsNG.ToString(), resultType = resultType },
                            new RowData() { dataStr =item.T_IsSave.ToString(), resultType = resultType },
                            new RowData() { dataStr = Path.Combine( item.T_Path,item.T_FileName+item.T_FileFormat), resultType = resultType },
                            };
                        dieCutEAProduct.productDataResult.DefectData.Add(strDeftData);
                    }
                    catch (Exception ee)
                    {
                        MessageNotify.AddErrorLog(camName + "\r\n" + ee.Message + "\r\n" + ee.StackTrace);
                        item.Dispose();
                    }
                }

                List<DefectSummary> defectSummaries = dieCutEAProduct.ngDefectResults
                                                  .GroupBy(item => item.ClassName)
                                                  .Select(group =>
                                                  {
                                                      NgDefectResult maxAreaNgItem = group
                                                          .Where(item => item.T_IsNG == true)
                                                          .OrderByDescending(item => item.Area)
                                                          .FirstOrDefault();

                                                      return new DefectSummary
                                                      {
                                                          T_TotalCount = group.Count(),
                                                          T_NgCount = group.Count(item => item.T_IsNG == true),
                                                          T_Name = maxAreaNgItem?.ClassName ?? "",
                                                      };
                                                  })
                                                  .ToList();


                if (defectSummaries != null)
                {
                    foreach (var item in defectSummaries)
                    {
                        if (string.IsNullOrEmpty(item.T_Name)) { item.T_IsNG = false; continue; }
                        string defectName = item.T_Name;
                        DefectCraft_DieCuttingCut defectCraft = craft.Get_Cam_DefectCraft(camIndex, defectName);

                        if (defectCraft != null)
                        {
                            item.T_DefectLevel = defectCraft.DefectLevel;
                            item.T_IsSevere = defectCraft.IsSevere;
                            item.T_IsNG = TotalDefectResultJudgment(item, defectCraft);
                            DefectTypeTempData defectTypeTempData = tempData.GetDefectTypeTempData(defectName);
                            if (item.T_IsNG)
                            {
                                defectTypeTempData.ContinuousCount++;
                                defectTypeTempData.AccumulateCount++;

                                StopAlarmJudgment(camName, defectTypeTempData, defectCraft);
                            }
                            else
                            {
                                defectTypeTempData.ContinuousCount = 0;
                            }
                        }
                        else
                        {
                            item.T_DefectLevel = 10000;
                            item.T_IsNG = true;
                        }


                    }
                }



                #endregion

                bool defectIsNG = false;
                bool isSevereDefect = false;
                if (defectSummaries != null)
                {
                    defectIsNG = defectSummaries.Where(p => p.T_IsNG == true).Count() > 0;
                    isSevereDefect = defectSummaries.Where(p => p.T_IsNG == true && p.T_IsSevere == true).Count() > 0;
                    dieCutEAProduct.productDataResult.highestPriorityNgDefect = GetHighestPriority(defectSummaries.ToArray());
                }

                return (!defectIsNG, isSevereDefect);
            });

        }

        private static ShowHalconWindow NgDefectResultToShowHalconWindow(NgDefectResult ngDefectResult, string camName)
        {
            ShowHalconWindow showHalconWindow = new ShowHalconWindow();
            showHalconWindow.m_strCamName = camName;
            showHalconWindow.m_hImage = ngDefectResult.CropImage;
            ngDefectResult.DisposePortion();
            return showHalconWindow;
        }


        private static bool TotalDefectResultJudgment(DefectSummary defectSummary, DefectCraft_DieCuttingCut defectCraft_DieCuttingCut)
        {
            if (defectCraft_DieCuttingCut.Condition1_En)
            {
                if (defectCraft_DieCuttingCut.Condition1AndCondition2_Logic == AndOrLogicEnum.或)
                {
                    return ConditionalJudgment(defectSummary.T_TotalCount, defectCraft_DieCuttingCut.Condition1_MembraneNum) || (defectSummary.T_NgCount > 0);
                }
                else
                {
                    return ConditionalJudgment(defectSummary.T_TotalCount, defectCraft_DieCuttingCut.Condition1_MembraneNum) && (defectSummary.T_NgCount > 0);
                }
            }
            else
            {
                return defectSummary.T_NgCount > 0;
            }
        }
        /// <summary>
        /// 缺陷结果判断
        /// </summary>
        /// <param name="ngDefectResult"></param>
        /// <param name="defectCraft"></param>
        /// <returns>是否NG</returns>
        private static bool DefectResultJudgment(NgDefectResult ngDefectResult, DefectCraft_DieCuttingCut defectCraft)
        {
            var conditionGroups = GetConditionFormat(defectCraft);
            bool totalResult = true;//默认缺陷NG
            if (conditionGroups == null || conditionGroups.Count == 0) return false;
            if (!(conditionGroups.Where(p => p.ConditionItems.Count > 0).Count() > 0)) return false;

            for (int i = 0; i < conditionGroups.Count; i++)
            {
                var conditions = conditionGroups[i].ConditionItems;
                if (conditions == null || conditions.Count == 0)
                {
                    continue;
                }

                bool groupResult = EvaluateCondition(conditions[0], ngDefectResult);

                for (int j = 1; j < conditions.Count; j++)
                {
                    var condition = conditions[j];
                    bool currentResult = EvaluateCondition(condition, ngDefectResult);

                    groupResult = condition.Logic == AndOrLogicEnum.或
                        ? groupResult || currentResult
                        : groupResult && currentResult;
                }

                if (i == 0)
                {
                    totalResult = groupResult;
                    continue;
                }

                // 当前组首条件的 Logic 表示它与上一组的关系。
                var groupLogic = conditions[0].Logic;
                totalResult = groupLogic == AndOrLogicEnum.或
                    ? totalResult || groupResult
                    : totalResult && groupResult;
            }

            // 根据实际属性名称回写判定结果。
            return totalResult;
        }

        private static List<ListConditionItem> GetConditionFormat(DefectCraft_DieCuttingCut defectCraft_DieCuttingCut)
        {
            List<ListConditionItem> ConditionItemss = new List<ListConditionItem>();

            if (defectCraft_DieCuttingCut.Condition2_En)
            {
                ListConditionItem listConditionItem2 = new ListConditionItem();
                listConditionItem2.ConditionItems.Add(new ConditionItem()
                {
                    Logic = defectCraft_DieCuttingCut.Condition1AndCondition2_Logic,
                    DataSource = defectCraft_DieCuttingCut.Condition2_DataSource1,
                    Data = defectCraft_DieCuttingCut.Condition2_Data1
                });

                listConditionItem2.ConditionItems.Add(new ConditionItem()
                {
                    Logic = defectCraft_DieCuttingCut.Condition2_Logic1,
                    DataSource = defectCraft_DieCuttingCut.Condition2_DataSource2,
                    Data = defectCraft_DieCuttingCut.Condition2_Data2
                });

                listConditionItem2.ConditionItems.Add(new ConditionItem()
                {
                    Logic = defectCraft_DieCuttingCut.Condition2_Logic2,
                    DataSource = defectCraft_DieCuttingCut.Condition2_DataSource3,
                    Data = defectCraft_DieCuttingCut.Condition2_Data3
                });
                ConditionItemss.Add(listConditionItem2);
            }

            if (defectCraft_DieCuttingCut.Condition3_En)
            {
                ListConditionItem listConditionItem3 = new ListConditionItem();
                listConditionItem3.ConditionItems.Add(new ConditionItem()
                {
                    Logic = defectCraft_DieCuttingCut.Condition2AndCondition3_Logic,
                    DataSource = defectCraft_DieCuttingCut.Condition3_DataSource1,
                    Data = defectCraft_DieCuttingCut.Condition3_Data1
                });

                listConditionItem3.ConditionItems.Add(new ConditionItem()
                {
                    Logic = defectCraft_DieCuttingCut.Condition3_Logic1,
                    DataSource = defectCraft_DieCuttingCut.Condition3_DataSource2,
                    Data = defectCraft_DieCuttingCut.Condition3_Data2
                });
                ConditionItemss.Add(listConditionItem3);
            }

            if (defectCraft_DieCuttingCut.Condition4_En)
            {
                ListConditionItem listConditionItem4 = new ListConditionItem();
                listConditionItem4.ConditionItems.Add(new ConditionItem()
                {
                    Logic = defectCraft_DieCuttingCut.Condition3AndCondition4_Logic,
                    DataSource = defectCraft_DieCuttingCut.Condition4_DataSource1,
                    Data = defectCraft_DieCuttingCut.Condition4_Data1
                });

                listConditionItem4.ConditionItems.Add(new ConditionItem()
                {
                    Logic = defectCraft_DieCuttingCut.Condition4_Logic1,
                    DataSource = defectCraft_DieCuttingCut.Condition4_DataSource2,
                    Data = defectCraft_DieCuttingCut.Condition4_Data2
                });

                ConditionItemss.Add(listConditionItem4);
            }

            return ConditionItemss;
        }

        private static bool EvaluateCondition(ConditionItem condition, NgDefectResult ngDefectResult)
        {
            double detectionValue = GetConditionalData(condition.DataSource, ngDefectResult);
            return ConditionalJudgment(detectionValue, condition.Data);
        }

        private static double GetConditionalData(DefectDataSourceEnum defectDataSourceEnum, NgDefectResult ngDefectResult)
        {
            if (defectDataSourceEnum == DefectDataSourceEnum.面积)
            {
                return ngDefectResult.Area;
            }
            else if (defectDataSourceEnum == DefectDataSourceEnum.高度)
            {
                return ngDefectResult.Height;
            }
            else if (defectDataSourceEnum == DefectDataSourceEnum.宽度)
            {
                return ngDefectResult.Width;
            }
            return 0;
        }

        private static bool ConditionalJudgment(double detectionValue, double cafectValue)
        {
            return detectionValue > cafectValue;
        }

        private static bool ConditionalJudgment(double value, double cafectStandardValue, double tolUpper, double tolLower)
        {
            return ((value <= tolUpper) && (value >= tolLower));
        }


        private static ResultType GetResultType(bool IsOverspecification, bool IsNG)
        {
            if (IsNG)
            {
                return ResultType.NG;
            }
            else if (IsOverspecification && !IsNG)
            {
                return ResultType.Warn;
            }
            return ResultType.OK;
        }

        private static HalconColor GetHalconColor(bool IsOverspecification, bool IsNG)
        {
            if (IsNG)
            {
                return HalconColor.red;
            }
            else if (IsOverspecification && !IsNG)
            {
                return HalconColor.yellow;
            }
            return HalconColor.green;
        }

        /// <summary>
        /// 数据顶级模式
        /// </summary>
        /// <param name="OriginData">原始数据</param>
        /// <param name="StandData">标准值</param>
        /// <param name="offset">偏差顶级范围：与标准值在一定范围内才使用顶级模式</param>
        /// <param name="factor">顶级系数：大于等于1</param>
        /// <returns></returns>
        private static double TopLevelMode(double OriginData, double StandData, double offset, double factor)
        {
            double Topdata = OriginData;
            try
            {
                double SubAbs = Math.Abs(OriginData - StandData);
                if (factor >= 1 && SubAbs < Math.Abs(offset)) //顶级系数大于等于1，且偏差值小于顶级范围。
                {
                    Topdata = (StandData * (factor - 1) + OriginData) / factor;
                }
            }
            catch (Exception ex)
            {
                //Log4netRecord.ErrorLog.Error(ex);
            }
            return Topdata;
        }

        private static void StopAlarmJudgment(string camName, SizeTypeTempData tempData, SizeCraft_DieCuttingCut sizeCraft)
        {
            if (tempData.ContinuousCount >= sizeCraft.ContinuousCount)
            {
                if (sizeCraft.TJ_Continuous)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{camName}-{sizeCraft.Name}_连续：" + $"当前个数{tempData.ContinuousCount}> 设定个数{sizeCraft.ContinuousCount}");
                }
                else if (sizeCraft.BJ_Continuous)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, $"{camName}-{sizeCraft.Name}_连续：" + $"当前个数{tempData.ContinuousCount}> 设定个数{sizeCraft.ContinuousCount}");
                }

                tempData.ContinuousCount = 0;
            }

            if (tempData.AccumulateCount >= sizeCraft.AccumulateCount && sizeCraft.BJ_Continuous)
            {
                if (sizeCraft.TJ_Accumulate)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{camName}-{sizeCraft.Name}_累计：" + $"当前个数{tempData.AccumulateCount}> 设定个数{sizeCraft.AccumulateCount}");
                }
                else if (sizeCraft.BJ_Accumulate)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, $"{camName}-{sizeCraft.Name}_累计：" + $"当前个数{tempData.AccumulateCount}> 设定个数{sizeCraft.AccumulateCount}");
                }

                tempData.AccumulateCount = 0;
            }
        }

        private static void StopAlarmJudgment(string camName, DefectTypeTempData tempData, DefectCraft_DieCuttingCut defectCraft)
        {
            if (tempData.ContinuousCount >= defectCraft.ContinuousCount)
            {
                if (defectCraft.TJ_Continuous)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{camName}-{defectCraft.Name}_连续：" + $"当前个数{tempData.ContinuousCount}> 设定个数{defectCraft.ContinuousCount}");
                }
                else if (defectCraft.BJ_Continuous)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, $"{camName}-{defectCraft.Name}_连续：" + $"当前个数{tempData.ContinuousCount}> 设定个数{defectCraft.ContinuousCount}");
                }

                tempData.ContinuousCount = 0;
            }

            if (tempData.AccumulateCount >= defectCraft.AccumulateCount && defectCraft.BJ_Continuous)
            {
                if (defectCraft.TJ_Accumulate)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{camName}-{defectCraft.Name}_累计：" + $"当前个数{tempData.AccumulateCount}> 设定个数{defectCraft.AccumulateCount}");
                }
                else if (defectCraft.BJ_Accumulate)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, $"{camName}-{defectCraft.Name}_累计：" + $"当前个数{tempData.AccumulateCount}> 设定个数{defectCraft.AccumulateCount}");
                }

                tempData.AccumulateCount = 0;
            }
        }

        private static void StopAlarmJudgment(SizeTypeTempData tempData, SizeCraftStation_DieCuttingCut _Craft)
        {
            if (tempData.ContinuousCount >= _Craft.ContinuousCount)
            {
                if (_Craft.TJ_Continuous)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"工位三-{_Craft.Name}_连续：" + $"当前个数{tempData.ContinuousCount}> 设定个数{_Craft.ContinuousCount}");
                }
                else if (_Craft.BJ_Continuous)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, $"工位三-{_Craft.Name}_连续：" + $"当前个数{tempData.ContinuousCount}> 设定个数{_Craft.ContinuousCount}");
                }

                tempData.ContinuousCount = 0;
            }

            if (tempData.AccumulateCount >= _Craft.AccumulateCount && _Craft.BJ_Continuous)
            {
                if (_Craft.TJ_Accumulate)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"工位三-{_Craft.Name}_累计：" + $"当前个数{tempData.AccumulateCount}> 设定个数{_Craft.AccumulateCount}");
                }
                else if (_Craft.BJ_Accumulate)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, $"工位三-{_Craft.Name}_累计：" + $"当前个数{tempData.AccumulateCount}> 设定个数{_Craft.AccumulateCount}");
                }

                tempData.AccumulateCount = 0;
            }
        }
        #endregion

        #endregion

        #region PLC结果发送
        /// <summary>
        /// 缺陷工位帧开始回调事件
        /// </summary>
        public void DefectCam_FramStartCallback(long currentFrame)
        {
            Data.DefectStation_CurrentFrame = currentFrame;
            Data.DefectStation_UpdateTime = DateTime.Now;
            MessageNotify.AddCommLog($"缺陷工位——帧开始信号：帧号{currentFrame}");
        }

        /// <summary>
        /// 裁切工位回调开始事件
        /// </summary>
        /// <param name="currentFrame"></param>
        public void CutCam_CamCallbackStart(long currentFrame)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                CutStation_SendResult(currentFrame);
            });
        }

        /// <summary>
        /// 尺寸工位回调开始事件
        /// </summary>
        /// <param name="currentFrame"></param>
        public void SizeCam_CamCallbackStart(long currentFrame)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                SizeStation_SendResult(currentFrame);
            });
        }

        public uint IOC0640_OPERATE_Callback(IntPtr operate_data)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                object Data = Marshal.PtrToStructure(operate_data, typeof(IOC0640_OPERATE_DATA));//从地址中读取数据

                IOC0640_OPERATE_DATA m_OperateData = (IOC0640_OPERATE_DATA)Data;//转换为自定义结构体

                Int32 status;
                ushort bitno = 0;

                //读取中断状态，同时解除中断
                status = IOC0640.ioc_read_intportstatus(0, 0);
                if ((status & 0x1) == 1)//输入口1中断有效
                {
                }
                if ((status & 0x2) == 0x2)//输入口2中断有效
                {
                    DefectStation_IOSendResult(0);
                }
                if ((status & 0x4) == 0x4)//输入口3中断有效
                {
                }
                if ((status & 0x8) == 0x8)//输入口4中断有效
                {
                }

            });
            return 0;
        }
        public void DefectStation_IOSendResult(long currentFrame)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                DateTime startTime = DateTime.Now;
                double time = startTime.Subtract(Data.DefectStation_UpdateTime).TotalMilliseconds;

                int FrontSideCount = Data.Craft.Get_GlobalVariableItem("FrontSideCount").Value.ToInt();
                int BackSideCount = Data.Craft.Get_GlobalVariableItem("BackSideCount").Value.ToInt();
                long resultID = Data.DefectStation_CurrentFrame - BackSideCount;
                MessageNotify.AddCommLog($"缺陷工位——请求结果ID{resultID}=={Data.DefectStation_CurrentFrame}-{BackSideCount}");

                Station1Product product = null;
                if (time > 0) product = Data.Station1_Product.DequeueUntilMatch(p => p.ProductID == resultID);


                //发送结果
                if (product == null)
                {
                    product = new Station1Product();
                    product.Frame = -1;
                    product.ProductID = -1;
                    product.IsNG = true;
                    product.ALine_DieCutEAProduct.IsSevereDefect = true;
                    product.BLine_DieCutEAProduct.IsSevereDefect = true;
                }
                int slice = BackSideCount - FrontSideCount;

                long ACamFrame = product.ALine_DieCutEAProduct.m_nImageFrameID;
                long ACamEA = product.ALine_DieCutEAProduct.m_nProductID;
                long BCamFrame = product.BLine_DieCutEAProduct.m_nImageFrameID;
                long BCamEA = product.BLine_DieCutEAProduct.m_nProductID;
                bool IsSevereDefect = product.ALine_DieCutEAProduct.IsSevereDefect || product.BLine_DieCutEAProduct.IsSevereDefect;
                string result = !product.IsNG ? "OK" : IsSevereDefect ? "重度NG" : "轻度NG";
                bool continuousNG = false;

                string str = $"发送帧号：背面{ACamFrame},正面{BCamFrame},发送EA号：背面{ACamEA}，正面{BCamEA}，间隔片数{BackSideCount}-{FrontSideCount}={slice}";
                if (product.IsNG)
                {
                    if (IsSevereDefect)
                    {
                        if (!DefectStation_SevereNGSignal.Trigger($"缺陷工位——发送_重度NG信号:{str}"))
                        {
                            MessageNotify.AddCommLog($"缺陷工位——发送_重度NG信号失败");
                        }
                    }
                    else
                    {
                        if (!DefectStation_MinorNGSignal.Trigger($"缺陷工位——发送_轻度NG信号:{str}"))
                        {
                            MessageNotify.AddCommLog($"缺陷工位——发送_轻度NG信号失败");
                        }
                    }
                }
                else
                {
                    if (!DefectStation_OKSignal.Trigger($"缺陷工位——发送_OK信号:{str}"))
                    {
                        MessageNotify.AddCommLog($"缺陷工位——发送_OK信号失败");
                    }

                }

                GlobalVariableItem globalVariableItem = Data.Craft.Get_GlobalVariableItem("DefectStation_ContinuousCount");
                if (globalVariableItem == null) { MessageNotify.AddErrorLog("缺陷工位——获取参数 DefectStation_ContinuousCount 失败"); }
                if (globalVariableItem != null)
                {
                    if (Data.DefectStation_ContinuousCount >= globalVariableItem.Value.ToInt())
                    {
                        Data.DefectStation_ContinuousCount = 0;
                        continuousNG = true;

                        if (!DefectStation_ContinuousNGSignal.Trigger($"缺陷工位——发送_连续NG信号:{str}"))
                        {
                            MessageNotify.AddCommLog($"缺陷工位——发送_连续NG信号失败");
                        }
                    }
                }


                DateTime endTime = DateTime.Now;
                int sumTime = (int)endTime.Subtract(startTime).TotalMilliseconds;
                //【】数据保存与显示
                List<RowData> rowDatas = new List<RowData>();
                rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{Data.DefectStation_CurrentFrame}", resultType = ResultType.OK });

                rowDatas.Add(new RowData() { dataStr = $"{product.ProductID}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{time}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{result}", resultType = ResultType.OK });

                rowDatas.Add(new RowData() { dataStr = $"{continuousNG}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{sumTime}", resultType = ResultType.OK }

               );


                FileOperationCSV.WriteData_CSV(Data.Station1_DefectCamSendPath, Data.Station1_CamSendTitle_Defect.ToArray(), rowDatas.ToArray(), "裁切工位");
                NDataPanelViewModel.Add(View.DP_DefectSendData, Data.Station1_CamSendTitle_Defect.ToArray(), rowDatas.ToArray());

            });
        }


        /// <summary>
        /// 尺寸工位发送结果
        /// </summary>
        public void SizeStation_SendResult(long currentFrame)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                DateTime startTime = DateTime.Now;
                Station3Result product = Data.Station3_LastResult.GetValue();
                MessageNotify.AddCommLog($"尺寸工位——请求结果：帧号{currentFrame},目标帧号{currentFrame - 1}");

                long cam4Frame = product.Frame_Cam4;
                long cam5Frame = product.Frame_Cam5;
                long cam6Frame = product.Frame_Cam6;
                bool continuousNG = false;

                string str = $"当前帧号{currentFrame},上一片帧号{cam4Frame},{cam5Frame},{cam6Frame}";
                if (currentFrame > 1)
                    if (currentFrame - 1 != product.Frame_Cam4)
                    {
                        product.IsNG = true;
                        //TJBJInfo.AlarmWriteState(AlarmName.工位三没找到上一片数据, StopAlarmStateEnum.报警, str);
                    }
                string result = !product.IsNG ? "OK" : "NG";


                if (product.IsNG)
                {
                    if (!SizeStation_NGSignal.Trigger($"尺寸工位——发送_NG信号:{str}"))
                    {
                        MessageNotify.AddCommLog($"尺寸工位——发送_NG信号失败");
                    }
                }
                else
                {
                    if (!SizeStation_OKSignal.Trigger($"尺寸工位——发送_OK信号:{str}"))
                    {
                        MessageNotify.AddCommLog($"尺寸工位——发送_OK信号失败");
                    }
                }

                GlobalVariableItem globalVariableItem = Data.Craft.Get_GlobalVariableItem("SizeStation_ContinuousCount");
                if (globalVariableItem != null)
                {
                    if (Data.SizeStation_ContinuousCount >= globalVariableItem.Value.ToInt())
                    {
                        Data.SizeStation_ContinuousCount = 0;
                        continuousNG = true;
                        if (!SizeStation_ContinuousNGSignal.Trigger($"尺寸工位——发送_连续NG信号:{str}"))
                        {
                            MessageNotify.AddCommLog($"尺寸工位——发送_连续NG信号失败");
                        }

                    }
                }
                else
                {
                    MessageNotify.AddErrorLog("尺寸工位——获取参数 SizeStation_ContinuousCount 失败");
                }

                DateTime endTime = DateTime.Now;
                int sumTime = (int)endTime.Subtract(startTime).TotalMilliseconds;
                //【】数据保存与显示
                List<RowData> rowDatas = new List<RowData>();
                rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{currentFrame}", resultType = ResultType.OK });

                rowDatas.Add(new RowData() { dataStr = $"{product.Frame_Cam4}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{result}", resultType = ResultType.OK });

                rowDatas.Add(new RowData() { dataStr = $"{continuousNG}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{sumTime}", resultType = ResultType.OK }

                );


                FileOperationCSV.WriteData_CSV(Data.Station3_SizeCamSendPath, Data.Station3_CamSendTitle_Size.ToArray(), rowDatas.ToArray(), "裁切工位");
                NDataPanelViewModel.Add(View.DP_SizeSendData, Data.Station3_CamSendTitle_Size.ToArray(), rowDatas.ToArray());
            });
        }


        /// <summary>
        /// 裁切工位发送结果
        /// </summary>
        public (CorrecInfo 裁切纠偏, CorrecInfo V角纠偏) CutStation_SendResult(long currentFrame)
        {
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                MessageNotify.AddCommLog($"裁切工位——请求结果：帧号{currentFrame}");

                double 单片宽度标准值 = Data.Craft.Get_Station3_SizeCraft("单片宽度").StandardValue;

                //CorrecInfo cut_CorrecInfo = ExecuteMethodClass.ExecuteMethod(() =>
                //{
                //    MessageNotify.AddCommLog($"获取裁切纠偏值,帧号{currentFrame}");
                //    return Data.Cut_CorrectPosDeal.GetOffsetValue();
                //});
                //CorrecInfo v_CorrecInfo = ExecuteMethodClass.ExecuteMethod(() =>
                //{
                //    MessageNotify.AddCommLog($"获取V角纠偏值,帧号{currentFrame}");
                //    return Data.V_CorrectPosDeal.GetOffsetValue();
                //});

                CorrecInfo cut_CorrecInfo = Data.Cut_CorrecInfo.GetSnapshot();
                CorrecInfo v_CorrecInfo = Data.V_CorrecInfo.GetSnapshot();
                {
                    CorrecInfo Cut_CorrectPosDealTemp = ExecuteMethodClass.ExecuteMethod(() =>
                    {
                        return Data.Cut_CorrectPosDeal.GetOffsetValueAndRemove();
                    });

                    //获取
                    CorrecInfo V_CorrecInfoTemp = ExecuteMethodClass.ExecuteMethod(() =>
                    {
                        return Data.V_CorrectPosDeal.GetOffsetValueAndRemove(true, Data.Cut_CorrectPosDeal.fRunLength, Cut_CorrectPosDealTemp.Value);
                    });

                    Cut_CorrectPosDealTemp = Data.Cut_CorrectPosDeal.GetCorrecInfo();


                    //获取

                    if (Cut_CorrectPosDealTemp == null)
                    {
                        MessageNotify.AddErrorLog($"获取队列裁切纠偏值，队列无数据");
                    }
                    else
                    {
                        MessageNotify.AddCommLog($"设置裁切纠偏值，当前帧号：{currentFrame}，获取帧号{Cut_CorrectPosDealTemp.nFrameID}");
                        Data.Cut_CorrecInfo.UpdateData(Cut_CorrectPosDealTemp);
                    }


                    V_CorrecInfoTemp = Data.V_CorrectPosDeal.GetCorrecInfo();
                    V_CorrecInfoTemp.Value -= Cut_CorrectPosDealTemp.Value;

                    if (V_CorrecInfoTemp == null)
                    {
                        MessageNotify.AddErrorLog($"获取队列V角纠偏值，队列无数据");
                    }
                    else
                    {
                        MessageNotify.AddCommLog($"设置V角纠偏值，当前帧号：{currentFrame}，获取帧号{V_CorrecInfoTemp.nFrameID}");
                        Data.V_CorrecInfo.UpdateData(V_CorrecInfoTemp);
                    }



                    cut_CorrecInfo = Data.Cut_CorrecInfo.GetSnapshot();
                    MessageNotify.AddCommLog($"获取裁切纠偏值，当前帧号：{currentFrame}，获取帧号{cut_CorrecInfo.nFrameID}");

                    v_CorrecInfo = Data.V_CorrecInfo.GetSnapshot();
                    // v_CorrecInfo.Value = cut_CorrecInfo.Value- v_CorrecInfo.Value ;
                    MessageNotify.AddCommLog($"获取V角纠偏值，当前帧号：{currentFrame}，获取帧号{v_CorrecInfo.nFrameID}");
                }

                //Data.Cut_CorrecInfo.UpdateData(new CorrecInfo());
                //Data.V_CorrecInfo.UpdateData(new CorrecInfo());

                if (cut_CorrecInfo == null) cut_CorrecInfo = new CorrecInfo();
                if (v_CorrecInfo == null) v_CorrecInfo = new CorrecInfo();
                cut_CorrecInfo.Value = Math.Round(cut_CorrecInfo.Value, 3);
                v_CorrecInfo.Value = Math.Round(v_CorrecInfo.Value, 3);




                //V角、裁切纠偏发送
                Project.Instance.server_DiecuttingCutPLC.m_DevAddress.V角纠偏值.Value = (float)v_CorrecInfo.Value;
                Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.V角纠偏值, true, Project.Instance.server_DiecuttingCutPLC.CommunicationLog);
                MessageNotify.AddCommLog($"裁切工位——发送V角纠偏值: 当前帧号{currentFrame},发送帧号{v_CorrecInfo.nFrameID},发送纠偏值{v_CorrecInfo.Value}");

                if (v_CorrecInfo.NotTab) Project.Instance.server_DiecuttingCutPLC.m_DevAddress.有无极耳.Value = 2;
                else Project.Instance.server_DiecuttingCutPLC.m_DevAddress.有无极耳.Value = 1;
                Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.有无极耳, true, Project.Instance.server_DiecuttingCutPLC.CommunicationLog);
                string str = v_CorrecInfo.NotTab ? "无极耳" : "有极耳";
                MessageNotify.AddCommLog($"裁切工位——发送极耳有无: 当前帧号{currentFrame},发送帧号{v_CorrecInfo.nFrameID},发送值{Project.Instance.server_DiecuttingCutPLC.m_DevAddress.有无极耳.Value}{str}");

                Project.Instance.server_DiecuttingCutPLC.m_DevAddress.裁切纠偏值.Value = (float)cut_CorrecInfo.Value;
                Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.裁切纠偏值, true, Project.Instance.server_DiecuttingCutPLC.CommunicationLog);
                MessageNotify.AddCommLog($"裁切工位——发送裁切纠偏值: 当前帧号{currentFrame},发送帧号{cut_CorrecInfo.nFrameID},发送纠偏值{cut_CorrecInfo.Value}");

                MessageNotify.AddCommLog($"裁切工位——发送结束：帧号{currentFrame}");

                //【】数据保存与显示
                List<RowData> rowDatas = new List<RowData>();
                rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{currentFrame}", resultType = ResultType.OK });

                rowDatas.Add(new RowData() { dataStr = $"{cut_CorrecInfo.nFrameID}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{cut_CorrecInfo.Value}", resultType = ResultType.OK });

                rowDatas.Add(new RowData() { dataStr = $"{v_CorrecInfo.nFrameID}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{v_CorrecInfo.Value}", resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{v_CorrecInfo.NotTab}", resultType = ResultType.OK }
            );


                FileOperationCSV.WriteData_CSV(Data.Station2_CutCamSendPath, Data.Station2_CutCamSendTitle.ToArray(), rowDatas.ToArray(), "裁切工位");
                NDataPanelViewModel.Add(View.Dp_CutSend, Data.Station2_CutCamSendTitle.ToArray(), rowDatas.ToArray());

                return (cut_CorrecInfo, v_CorrecInfo);
            });
        }

        public (bool 清除队列, bool 寻极耳中) CutStation_SendResult(int camIndex, long currentFrame, bool 无极耳)
        {
            bool 清除队列 = false;
            bool 寻极耳中 = false;
            ExecuteMethodClass.ExecuteMethod(() =>
           {
               int 寻极耳个数 = Data.Craft.Get_GlobalVariableItem(camIndex, "FindTabSum").Value.ToInt();

               //有无极耳发送
               if (无极耳)
               {
                   if (!Data.IsFindTab) Data.CutStation_ContinuousCount++;

                   if (Data.CutStation_ContinuousCount >= 寻极耳个数)
                   {
                       //清除队列 = true;

                       Data.CutStation_ContinuousCount = 0;
                       Data.IsFindTab = true;
                   }
               }
               else
               {
                   if (Data.IsFindTab)
                   {
                       清除队列 = true;
                   }

                   Data.CutStation_ContinuousCount = 0;
                   Data.IsFindTab = false;
               }

               //寻极耳中发送
               寻极耳中 = Data.IsFindTab;
               if (Data.IsFindTab) Project.Instance.server_DiecuttingCutPLC.m_DevAddress.寻极耳中.Value = 1;
               else Project.Instance.server_DiecuttingCutPLC.m_DevAddress.寻极耳中.Value = 0;
               string str = Data.IsFindTab ? "寻极耳中" : "不寻极耳";
               Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.寻极耳中, true, Project.Instance.server_DiecuttingCutPLC.CommunicationLog);
               MessageNotify.AddCommLog($"裁切工位——发送寻极耳中,发送帧号{currentFrame},发送值{Project.Instance.server_DiecuttingCutPLC.m_DevAddress.寻极耳中.Value}{str}");

           });
            return (清除队列, 寻极耳中);
        }

        public static void PID(Model_DieCuttingCut data, double 肩宽测量值, double 肩宽标准值)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                data.肩宽_PID.Enabled = data.Craft.Get_GlobalVariableItem("PID启用").Value.ToBool();
                data.肩宽_PID.Gain = data.Craft.Get_GlobalVariableItem("PID输出比例").Value.ToDouble();
                data.肩宽_PID.SampleCount = data.Craft.Get_GlobalVariableItem("PID样本个数").Value.ToInt();
                data.肩宽_PID.DeadBand = data.Craft.Get_GlobalVariableItem("PID死区范围").Value.ToDouble();
                data.肩宽_PID.MaxSingleSizeError = data.Craft.Get_GlobalVariableItem("PID超限范围").Value.ToDouble();
                data.肩宽_PID.IqrFactor = data.Craft.Get_GlobalVariableItem("PID异常值剔除倍数").Value.ToDouble();
                data.肩宽_PID.MaxOutputCompensation = data.Craft.Get_GlobalVariableItem("PID输出最大值").Value.ToDouble();
                BackSizeDeviationResult result = data.肩宽_PID.Update(肩宽测量值, 肩宽标准值);
                bool 是否输出补偿值 = result.Updated;
                double 闭环补偿值 = result.Step;
                string 闭环信息 = result.Message;

                GlobalVariableItem globalVariableItem = data.Craft.Get_GlobalVariableItem(2, "ShoulderWidthCompensation");
                double 当前闭环补偿值 = globalVariableItem.Value.ToDouble();

                if (data.肩宽_PID.Enabled)
                {
                    if (是否输出补偿值)
                    {
                        double 写入全局补偿值 = 当前闭环补偿值 + 闭环补偿值;
                        globalVariableItem.Value = Math.Round(写入全局补偿值, 3).ToString();
                    }
                }
                else
                {
                    globalVariableItem.Value = 0.ToString();
                }


            });
        }
        #endregion

        #region 线程任务
        public override void InitThread()
        {
            base.InitThread();
            thd_Communication.Start(5, "ThdCommunication");
            thd_Communication.PerformWork -= Communication;
            thd_Communication.PerformWork += Communication;

            thd_ImageShowAndSave.Start(5, "thd_ImageShowAndSave");//在UI线程注册回调

            thd_DataSave.Start(500, "thd_DataSave");
            thd_DataSave.PerformWork -= DataSave;
            thd_DataSave.PerformWork += DataSave;

            thd_DataProcess.Start(70, "thd_DataProcess");
            thd_DataProcess.PerformWork -= DataProcess;
            thd_DataProcess.PerformWork += DataProcess;
        }

        #endregion

        #region 父类任务

        public override void SimLoadImage()
        {
            if (Project.Instance.Is_simulation)
                ExecuteMethodClass.ExecuteMethod(() =>
                {
                    if (ProjectFactory.F_Model.WorkModelBase.M_SimRunCommand == SimRunCommand.SimPrev)
                    {
                        Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                        {
                            CameraEnum cameraEnum = (CameraEnum)item.Key;
                            bool flag = false;

                            flag = SimControl.LoadPrevImage_Sim(Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_ImageSimManage,
                                         ref item.Value.QueueGrabImage, $"{(cameraEnum).ToString()}仿真加载图片");
                            Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[(int)cameraEnum].CallBackImgDataDeal();

                        });



                        ProjectFactory.F_Model.WorkModelBase.M_SimRunCommand = SimRunCommand.Stop;

                        foreach (ProcessServices item in Project.Instance.m_ProcessList)
                        {
                            Project.Instance.ExecuteOnce(item.process.processInfo.ProcessID);
                        }
                    }
                    else if (ProjectFactory.F_Model.WorkModelBase.M_SimRunCommand == SimRunCommand.SimRun)
                    {
                        Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                        {
                            CameraEnum cameraEnum = (CameraEnum)item.Key;
                            bool flag = false;

                            flag = SimControl.LoadNextImage_Sim(Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_ImageSimManage,
                                         ref item.Value.QueueGrabImage, $"{(cameraEnum).ToString()}仿真加载图片");

                            Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[(int)cameraEnum].CallBackImgDataDeal();

                        });


                    }
                    else if (ProjectFactory.F_Model.WorkModelBase.M_SimRunCommand == SimRunCommand.SimNext)
                    {
                        Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                        {
                            CameraEnum cameraEnum = (CameraEnum)item.Key;
                            bool flag = false;

                            flag = SimControl.LoadNextImage_Sim(Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_ImageSimManage,
                                          ref item.Value.QueueGrabImage, $"{(cameraEnum).ToString()}仿真加载图片");
                            Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[(int)cameraEnum].CallBackImgDataDeal();


                        });

                        ProjectFactory.F_Model.WorkModelBase.M_SimRunCommand = SimRunCommand.Stop;

                        foreach (ProcessServices item in Project.Instance.m_ProcessList)
                        {
                            Project.Instance.ExecuteOnce(item.process.processInfo.ProcessID);
                        }
                    }



                });
        }

        public override bool StartFilmRoll(string filmRollName)
        {
            return ExecuteMethodClass.ExecuteMethodContainErrShow(() =>
            {
                //用正则表达式判断是否为合法的膜卷号，合法的膜卷号由数字和字母和正常符号（-）并且无空格
                if (!System.Text.RegularExpressions.Regex.IsMatch(filmRollName, "^[a-zA-Z0-9-]+$"))
                {
                    // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机开始膜卷失败,膜卷号不合法");
                    System.Windows.Forms.MessageBox.Show("膜卷号不合法，只能包含字母、数字、-，不能有空格");
                    return false;
                }
                string strPath = Project.Instance.paramPathSet.ProductRootPath + $"{filmRollName}";
                if (Directory.Exists(strPath))
                {
                    System.Windows.Forms.MessageBox.Show("膜卷号重复");
                    return false;


                }

                Project.Instance.Is_MojuanStart = true;
                //Project.Instance.systemInfo.EmployeeID = input_UserID.Text.Trim();
                Project.Instance.systemInfo.currentMojuanhao = filmRollName;
                Project.Instance.systemInfo.Save();

                Project.Instance.NewMujuanhao(Project.Instance.systemInfo.RecipeName, filmRollName, Project.Instance.systemInfo.CraftType);

                UIDelegate.LightParamChange();


                //清除业务数据
                Data.Init();

                Project.Instance.baseProDevice.INIPro();
                Project.Instance.baseProDevice.INIProduct();
                //Project.Instance.baseProDevice.InitProduct_Sim();
                //BIZ_Switchover.IBIZWorkerBase.InitVariable();
                //BIZ_Switchover.IBIZWorkerBase.DisposeImageAndClearImageQueue();

                //相机回调计数清零
                foreach (var item in Project.Instance.M_CameraRelevancies)
                {
                    //相机回调计数清零
                    item.Value.Item1.Camera.GrabCount = 0;

                    item.Value.Item2.m_ImageSimManage.InitSimFlag();

                    //实时点检初始化
                    item.Value.Item2.m_SizePointInspectionManage.sizePointInspectionParam.sizePointInspectionWorkParam.ResetStartDJ();
                }

                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_Start_MJ", "副机开始膜卷");

                RefreshToolView?.Invoke();
                InitView?.Invoke();
                return true;
            });
        }

        public override bool StopFilmRoll()
        {
            return ExecuteMethodClass.ExecuteMethodContainErrShow(() =>
            {
                //结束膜卷
                if (Project.Instance.Is_Run)
                {
                    //Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机结束膜卷失败");
                    System.Windows.Forms.MessageBox.Show("CCD正在运行中，请先暂停");
                    return false;
                }

                //Project.Instance.dBDataManage.SaveDBData();

                Project.Instance.Is_MojuanStart = false;

                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_Stop_MJ", "副机停止膜卷");

                RefreshToolView?.Invoke();
                return true;
            });
        }

        public override bool StartRun()
        {
            return ExecuteMethodClass.ExecuteMethodContainErrShow(() =>
            {
                if (Project.Instance.Is_simulation)
                {
                    // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机开始运行失败");
                    System.Windows.Forms.MessageBox.Show(" “仿真运行”状态禁止自动运行。");
                    return false;
                }

                if (!Project.Instance.Is_MojuanStart)
                {
                    // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机开始运行失败");
                    System.Windows.Forms.MessageBox.Show("请点击“开始膜卷”按钮。");
                    return false;
                }



                Form_Main.FormMain.menuStrip1.Invoke(new Action(() =>
                {
                    Form_Main.FormMain.menuStrip1.Enabled = false;
                }));


                foreach (ProcessServices item in Project.Instance.m_ProcessList)
                {
                    Project.Instance.StartRun(item.process.processInfo.ProcessID);
                }

                //foreach (var item in Service_Camera.Get_CameraDevs())
                //{
                //    if (item.Camera?.ConnStatus ?? false)
                //        if (!item.Camera.IsCameraStatue)
                //            item.Camera.StartGrabImage();
                //}

                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_RUN", "副机运行中");
                RefreshToolView?.Invoke();
                return true;
            });
        }
        public override bool StopRun()
        {
            return ExecuteMethodClass.ExecuteMethodContainErrShow(() =>
            {
                if (Project.Instance.cutOverlayPLCManage.ReadMachineRunningState() == 1)
                {
                    // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机暂停运行失败");
                    System.Windows.Forms.MessageBox.Show("主设备运行中，CCD禁止暂停");
                    return false;
                }
                if (Project.Instance.slaveMachineManager.initState)
                {
                    if (((SlaveMachineParam)Project.Instance.slaveMachineManager.tcpClientParam).RunningFlag == 1)
                    {
                        System.Windows.Forms.MessageBox.Show("主软件运行中，CCD禁止暂停");
                        return false;
                    }
                }

                Data.M_SimRunCommand = SimRunCommand.Stop;

                if (Project.Instance.systemInfo.IsHostMachine)
                {
                    //Project.Instance.mDevPLCManager.mDevParamAddress.CCDRUN.value = 0;
                    //Project.Instance.mDevPLCManager.mDevParamAddress.CCDRUN.execute = true;

                    //Project.Instance.dBJPLCManager.dBJParamAddress.DBJWriteParam[(int)DBJSendParamName.运行暂停].value = 0;
                    //Project.Instance.dBJPLCManager.dBJParamAddress.DBJWriteParam[(int)DBJSendParamName.运行暂停].execute = true;
                }
                else
                {

                }


                Form_Main.FormMain.menuStrip1.Invoke(new Action(() =>
                {
                    Form_Main.FormMain.menuStrip1.Enabled = true;

                }));

                foreach (ProcessServices item in Project.Instance.m_ProcessList)
                {
                    Project.Instance.StopRun(item.process.processInfo.ProcessID);
                }



                //foreach (var item in Project.Instance.M_CameraRelevancies)
                //{
                //   if (item.Value.initState)
                //     item.Value.Camera.StopGrabImage();
                //}


                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_STOP", "副机暂停中");
                RefreshToolView?.Invoke();
                return true;
            });
        }

        public override void Reset()
        {
            ExecuteMethodClass.ExecuteMethodContainErrShow(() =>
            {
                Project.Instance.baseProDevice.INIProduct();
            });
        }

        public override void RunModelSwitch()
        {
            ExecuteMethodClass.ExecuteMethodContainErrShow(() =>
            {
                if (Project.Instance.Is_simulation == true)
                {
                    //切换为自动模式
                    Project.Instance.Is_simulation = false;

                    //切换为自动模式清除仿真数据
                    Project.Instance.baseProDevice.InitProduct_Sim();

                    if (Project.Instance.Is_MojuanStart)
                    {
                        Project.Instance.NewMujuanhao(Project.Instance.systemInfo.RecipeName, Project.Instance.systemInfo.currentMojuanhao, Project.Instance.systemInfo.CraftType);
                    }

                }
                else
                {
                    //切换为仿真模式
                    Project.Instance.Is_simulation = true;


                    Project.Instance.NewMujuanhao(Project.Instance.systemInfo.RecipeName, "SimTest", Project.Instance.systemInfo.CraftType);
                    Data.InitFilePath();
                }
            });
        }

        public override List<StatusLabelItem> GetStatusBarInfo()
        {
            return ExecuteMethodClass.ExecuteMethod(() =>
            {
                List<StatusLabelItem> statusLabelItems = new List<StatusLabelItem>();

                //检测用户登录时间
                Project.Instance.userManage.MonitorLogonOutTime();

                string userType = LanguageProject.GetLangeuageAttribute(StatusStripEnum.当前用户类型, Project.Instance.systemInfo.m_CurrentLanguageType) +
                     LanguageProject.GetLangeuageAttribute(Project.Instance.userManage.userData.CurrentUser, Project.Instance.systemInfo.m_CurrentLanguageType);
                string userID = LanguageProject.GetLangeuageAttribute(StatusStripEnum.当前用户ID, Project.Instance.systemInfo.m_CurrentLanguageType) + Project.Instance.userManage.userData.CurrentUserID;

                statusLabelItems.Add(new StatusLabelItem() { Text = userType, Status = StatusLevel.Normal });
                statusLabelItems.Add(new StatusLabelItem() { Text = userID, Status = StatusLevel.Normal });

                //相机
                foreach (var item in Project.Instance.M_CamParam.l_CameraDevs)
                {
                    statusLabelItems.Add(new StatusLabelItem() { Text = item.CameraParameter.CameraName, Status = item.ConnStatus ? StatusLevel.Normal : StatusLevel.Alarm });
                }

                //光源
                foreach (var item in Project.Instance.M_LightControllerParam.l_LightControllerDevs)
                {
                    statusLabelItems.Add(new StatusLabelItem() { Text = item.LightControllerParameter.LightControllerName, Status = item.ConnStatus ? StatusLevel.Normal : StatusLevel.Alarm });
                }


                //仪器
                //设备PLC
                statusLabelItems.Add(new StatusLabelItem() { Text = "设备PLC", Status = Project.Instance.server_DiecuttingCutPLC.InitState ? StatusLevel.Normal : StatusLevel.Alarm });

                //控制卡
                statusLabelItems.Add(new StatusLabelItem() { Text = "控制卡", Status = ServiceCardManage.Instance.GetIControlCard()?.IsConnnected() != 0 ? StatusLevel.Normal : StatusLevel.Alarm });

                //仪器
                //设备PLC
                //  NStatusStrip.AddSoftwareState("打标机", Project.Instance.dbjCardManager.initState ? 1 : 0, ref softwareState);


                //仪器
                //设备PLC
                //NStatusStrip.AddSoftwareState("主副机通讯", Project.Instance.slaveMachineManager.initState ? 1 : 0, ref softwareState);

                MyMetrics.GetPrivateWorkingSetMB();
                MyMetrics.GetCpuPercentLikeTaskManager();

                statusLabelItems.Add(new StatusLabelItem() { Text = $"当前运行内存：{MyMetrics.fWorkingSetMBValue.ToString("00000.00")} MB", Status = StatusLevel.Normal });
                statusLabelItems.Add(new StatusLabelItem() { Text = $"当前CPU：{MyMetrics.fCpuValue.ToString("0000.00")}%", Status = StatusLevel.Normal });


                //磁盘检测
                //检测磁盘状态
                StateManage.CheckDriveFreeSpace();
                statusLabelItems.Add(new StatusLabelItem() { Text = $"磁盘剩余：{StateManage.DiskSpace}", Status = StateManage.DiskInterval });


                int index = 0;
                foreach (var item in statusLabelItems)
                {
                    item.Key = index++.ToString();
                }
                return statusLabelItems;
            }) ?? new List<StatusLabelItem>();
        }

        public override void DataSave()
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                if (Project.Instance.Is_MojuanStart)
                {
                    if (Data.DefectStation_BindingDatasSave.Count > 0)
                    {
                        var RowData = Data.DefectStation_BindingDatasSave.Dequeue().ToDataList();
                        RowData.Insert(0, Data.Station1_Title_Statistics.ToArray());
                        FileOperationCSV.WriteData_CSV(Data.Station1_StatisticsPath, RowData, "缺陷检测工位数据统计");
                    }

                    if (Data.SizeStation_BindingDatasSave.Count > 0)
                    {
                        var RowData = Data.SizeStation_BindingDatasSave.Dequeue().ToDataList();
                        FileOperationCSV.WriteData_CSV(Data.Station3_StatisticsPath, RowData, "极片尺寸工位数据统计");
                    }

                }


                var dataSaveClasses = Data.DataSaveQueue.Data.DequeueBatch();
                var dataSaveClassesGroupBy = dataSaveClasses.GroupBy(p => p.FileSave_Path).ToList();

                foreach (var item in dataSaveClassesGroupBy)
                {
                    var firstItem = item.First();


                    // 收集所有行
                    var allRows = item
                        .Where(x => x.rowDatas != null && x.rowDatas.Count > 0)
                        .SelectMany(x => x.rowDatas)
                        .ToList();

                    // 如果没有数据，跳过
                    if (allRows.Count == 0) continue;

                    FileOperationCSV.WriteData_CSV(firstItem.FileSave_Path, firstItem.FileSave_Title.ToArray(), allRows, firstItem.Source);
                }
            });
        }

        public override void DataProcess()
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    //合片
                    GlobalVariableItem FrontSideCount = Data.Craft.Get_GlobalVariableItem("FrontSideCount");
                    GlobalVariableItem BackSideCount = Data.Craft.Get_GlobalVariableItem("BackSideCount");

                    int slice = BackSideCount.Value.ToInt() - FrontSideCount.Value.ToInt();

                    long AProductID = 0;
                    long BProductID = 0;
                    if (Data.ALine_DieCutEAProduct.Count > 0)
                    {
                        AProductID = Data.ALine_DieCutEAProduct.Peek().m_nProductID;
                    }

                    if (Data.BLine_DieCutEAProduct.Count > 0)
                    {
                        BProductID = Data.BLine_DieCutEAProduct.Peek().m_nProductID;
                    }

                    if (AProductID != 0 && BProductID != 0)
                    {
                        if (AProductID == BProductID - slice)
                        {
                            Station1Product station1Product = new Station1Product();
                            station1Product.ProductID = AProductID;
                            station1Product.ALine_DieCutEAProduct = Data.ALine_DieCutEAProduct.Dequeue();
                            station1Product.BLine_DieCutEAProduct = Data.BLine_DieCutEAProduct.Dequeue();
                            station1Product.IsNG = station1Product.ALine_DieCutEAProduct.IsNG || station1Product.BLine_DieCutEAProduct.IsNG;

                            //【】处理完成入队列
                            Data.Station1_Product.Enqueue(station1Product, 100);
                            MessageNotify.AddCommLog($"缺陷工位——添加产品ID{AProductID}");

                            //【】数据统计                    
                            Station1_StatisticalData((int)AProductID, station1Product.IsNG, Data, station1Product);

                        }
                        else if (AProductID > BProductID - slice)
                        {
                            var tempProduce = Data.BLine_DieCutEAProduct.Dequeue();
                            MessageNotify.AddErrorLog($"A线扫当前产品ID大于B线扫产品ID，移除B线扫产品{tempProduce.m_nProductID}");
                        }
                        else if (AProductID < BProductID - slice)
                        {
                            MessageNotify.AddErrorLog($"A线扫当前产品ID小于比B线扫产品ID");
                        }

                    }
                }

            });
        }

        public override void Communication()
        {

            ExecuteMethodClass.ExecuteMethod(() =>
            {
                //心跳
                int HeartbeatTime = Data.Craft.Get_GlobalVariableItem("HeartbeatPollingTime").Value.ToInt();

                if (Project.Instance.Is_Run)
                {
                    //裁切工位心跳
                    //if (Data.CutStation_IntervalExecution.GetWhetherToReachIntervalTime_Heartbeat(HeartbeatTime))
                    //{
                    //    Data.CutStation_IntervalExecution.Set_DateTime();
                    //    Project.Instance.server_DiecuttingCutPLC.m_DevAddress.心跳.Value = true;
                    //    Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.心跳, true, Project.Instance.server_DiecuttingCutPLC.HeartbeatLog);
                    //}
                    //else if (Data.CutStation_IntervalExecution.GetWhetherToReachIntervalTime_Heartbeat(heartbeatPulseWidth))
                    //{
                    //    Project.Instance.server_DiecuttingCutPLC.m_DevAddress.心跳.Value = false;
                    //    Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.心跳, true, Project.Instance.server_DiecuttingCutPLC.HeartbeatLog);
                    //}

                    if (CutStation_Heartbeat.GetIsArrivalTime(HeartbeatTime))
                    {
                        CutStation_Heartbeat.Trigger("裁切工位——心跳");
                    }

                    if (DefectStation_Heartbeat.GetIsArrivalTime(HeartbeatTime))
                    {
                        DefectStation_Heartbeat.Trigger("缺陷工位——心跳");
                    }

                    if (SizeStation_Heartbeat.GetIsArrivalTime(HeartbeatTime))
                    {
                        SizeStation_Heartbeat.Trigger("尺寸工位——心跳");
                    }
                }

                CutStation_Heartbeat.OnTimerTick();

                DefectStation_Heartbeat.OnTimerTick();
                DefectStation_MinorNGSignal.OnTimerTick();
                DefectStation_SevereNGSignal.OnTimerTick();
                DefectStation_OKSignal.OnTimerTick();
                DefectStation_ContinuousNGSignal.OnTimerTick();

                SizeStation_Heartbeat.OnTimerTick();
                SizeStation_NGSignal.OnTimerTick();
                SizeStation_OKSignal.OnTimerTick();
                SizeStation_ContinuousNGSignal.OnTimerTick();

                //DefectStation_RequestSignal.OnTimerTick();
            });


        }

        public override void AlarmReset()
        {

        }

        public override void SendStopAlarm(int index)
        {

        }

        public override void SendStopAlarm(string msg)
        {

        }

        public override void SendAlarm(int index)
        {

        }

        public override void SendAlarm(string msg)
        {

        }

        public override void ClearCutData(bool isSendPLC)
        {
            Data.Reset_CutStation(isSendPLC);

        }

        public override void Udpate()
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                int HeartbeatTime = Data.Craft.Get_GlobalVariableItem("HeartbeatPollingTime").Value.ToInt();
                int heartbeatPulseWidth = Data.Craft.Get_GlobalVariableItem("HeartbeatPulseWidth").Value.ToInt();
                int pulseWidth = Data.Craft.Get_GlobalVariableItem("PulseWidth").Value.ToInt();
                int 防抖时间 = Data.Craft.Get_GlobalVariableItem("防抖时间").Value.ToInt();

                //DefectStation_RequestSignal.SetDebounce(防抖时间);

                CutStation_Heartbeat.SetPulseWidth(heartbeatPulseWidth);

                DefectStation_Heartbeat.SetPulseWidth(heartbeatPulseWidth);
                DefectStation_MinorNGSignal.SetPulseWidth(pulseWidth);
                DefectStation_SevereNGSignal.SetPulseWidth(pulseWidth);
                DefectStation_OKSignal.SetPulseWidth(pulseWidth);
                DefectStation_ContinuousNGSignal.SetPulseWidth(pulseWidth);

                SizeStation_Heartbeat.SetPulseWidth(heartbeatPulseWidth);
                SizeStation_NGSignal.SetPulseWidth(pulseWidth);
                SizeStation_OKSignal.SetPulseWidth(pulseWidth);
                SizeStation_ContinuousNGSignal.SetPulseWidth(pulseWidth);
            });
        }




        #endregion

    }

}
