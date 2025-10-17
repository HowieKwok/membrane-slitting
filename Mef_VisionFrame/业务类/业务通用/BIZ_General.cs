using Frame_Core;
using HalconDotNet;
using IKapBoardClassLibrary;
using LogTool;
using ScottPlot;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Mef_VisionFrame
{
    /// <summary>
    /// 业务类-图像操作工具
    /// </summary>
    public class BIZ_General
    {

        /// <summary>
        /// 图像处理业务流程
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="isShowImage"></param>
        /// <param name="isSaveImage"></param>
        /// <param name="isSaveScreenshot"></param>
        /// <param name="disposeImageRetainLenght"></param>
        /// <param name="displayWindowClass"></param>
        public static void Image_Process(Operation_ImageProcessor processor, int disposeImageRetainLenght, DisplayWindowClass displayWindowClass)
        {
            if (processor != null && processor.imageInfosShow != null && processor.imageInfosShow.Count > 0)
                processor.ShowImagesAsync(processor.imageInfosShow, displayWindowClass, processor.imageInfosSave, processor.imageInfosScreenshotSave);

            if (processor != null && processor.imageInfosSave != null && processor.imageInfosSave.Count > 0)
                processor.SaveImagesAsync(processor.imageInfosSave, processor.imageInfosDispose);

            if (processor != null && processor.imageInfosScreenshotSave != null && processor.imageInfosScreenshotSave.Count > 0)
                processor.SaveDisposeScreenshotImagesAsync(processor.imageInfosScreenshotSave);


            if (processor != null && processor.imageInfosDispose != null && processor.imageInfosDispose.Count > 0)
                processor.DisposeImagesAsync(processor.imageInfosDispose, disposeImageRetainLenght);
        }

        /// <summary>
        /// 图像处理业务流程
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="isShowImage"></param>
        /// <param name="isSaveImage"></param>
        /// <param name="isSaveScreenshot"></param>
        /// <param name="disposeImageRetainLenght"></param>
        /// <param name="displayWindowClass"></param>
        public static void Image_Process(Operation_ImageProcessor processor, int disposeImageRetainLenght)
        {
            if (processor != null && processor.imageInfosSave != null && processor.imageInfosSave.Count > 0)
                processor.SaveImagesAsync(processor.imageInfosSave, processor.imageInfosDispose);

            if (processor != null && processor.imageInfosScreenshotSave != null && processor.imageInfosScreenshotSave.Count > 0)
                processor.SaveDisposeScreenshotImagesAsync(processor.imageInfosScreenshotSave);

            if (processor != null && processor.imageInfosDispose != null && processor.imageInfosDispose.Count > 0)
                processor.DisposeImagesAsync(processor.imageInfosDispose, disposeImageRetainLenght);
        }


        /// <summary>
        /// 获取是否保存图片
        /// </summary>
        /// <param name="imageSaveSelect"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool GetIsSaveImage(ImageSaveSelect imageSaveSelect, bool result)
        {
            if (imageSaveSelect == ImageSaveSelect.保存所有)
            {
                return true;
            }
            else if (imageSaveSelect == ImageSaveSelect.保存OK && result)
            {
                return true;
            }
            else if (imageSaveSelect == ImageSaveSelect.保存NG && !result)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 图像数据类型转换
        /// </summary>
        /// <param name="showHalconWindow"></param>
        /// <param name="imageSavePath"></param>
        /// <param name="imageSaveName"></param>
        /// <param name="imageSaveFormat"></param>
        /// <param name="imageSaveSelect"></param>
        /// <param name="isShowImage"></param>
        /// <param name="isSaveScreenshot"></param>
        /// <param name="_source"></param>
        /// <returns></returns>
        public static Operation_ImageInfo ImageDataTypeTransition(ShowHalconWindow showHalconWindow, string imageSaveBasePath, string imageSaveRelativePath, string imageSaveName, ImageSaveFormat imageSaveFormat, ImageSaveSelect imageSaveSelect, bool isShowImage,
             bool screenshotHighDefinition, ImageSaveSelect screenshotSaveSelect, string screenshotSaveBasePath, string screenshotSaveRelativePath, string screenshotSaveName, ImageSaveFormat screenshotSaveFormat, string _source)
        {
            if (showHalconWindow == null) return null;

            Operation_ImageInfo operation_ImageInfo = new Operation_ImageInfo();
            try
            {
                if (!showHalconWindow.m_bIsNG)
                {
                    imageSaveRelativePath += "\\OK\\";
                    screenshotSaveRelativePath += "\\OK\\";
                }
                else
                {
                    imageSaveRelativePath += "\\NG\\";
                    screenshotSaveRelativePath += "\\NG\\";
                }

                operation_ImageInfo.showHalconWindow = showHalconWindow;
                operation_ImageInfo.str_ImageSaveBasePath = imageSaveBasePath;
                operation_ImageInfo.str_ImageSaveRelativePath = imageSaveRelativePath;
                operation_ImageInfo.str_ImageSaveName = imageSaveName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + SaveImageTool.GetImageFormat(imageSaveFormat);
                operation_ImageInfo.imageSaveFormat = imageSaveFormat;
                operation_ImageInfo.bIsSaveImage = GetIsSaveImage(imageSaveSelect, !showHalconWindow.m_bIsNG);
                         
                operation_ImageInfo.bIsShowImage = isShowImage;
                operation_ImageInfo.HighDefinition_Screenshot = screenshotHighDefinition;
                operation_ImageInfo.bIsSaveScreenshot = GetIsSaveImage(screenshotSaveSelect, !showHalconWindow.m_bIsNG);
                operation_ImageInfo.str_ScreenshotSaveBasePath = screenshotSaveBasePath;
                operation_ImageInfo.str_ScreenshotSaveRelativePath = screenshotSaveRelativePath;
                operation_ImageInfo.str_ScreenshotSaveName = screenshotSaveName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + SaveImageTool.GetImageFormat(screenshotSaveFormat); 
                operation_ImageInfo.screenshotSaveFormat = screenshotSaveFormat;


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(_source, ex);
            }
            return operation_ImageInfo;
        }


        /// <summary>
        /// 图像数据类型转换
        /// </summary>
        /// <param name="showHalconWindow"></param>
        /// <param name="imageSavePath"></param>
        /// <param name="imageSaveName"></param>
        /// <param name="imageSaveFormat"></param>
        /// <param name="imageSaveSelect"></param>
        /// <param name="isShowImage"></param>
        /// <param name="isSaveScreenshot"></param>
        /// <param name="_source"></param>
        /// <returns></returns>
        public static Operation_ImageInfo ImageDataTypeTransition(ShowHalconWindow showHalconWindow, string imageSaveBasePath, string imageSaveRelativePath, string imageSaveName, ImageSaveFormat imageSaveFormat, ImageSaveSelect imageSaveSelect, bool isShowImage,
            bool bIsDeepthCompression, bool screenshotHighDefinition, ImageSaveSelect screenshotSaveSelect, string screenshotSaveBasePath, string screenshotSaveRelativePath, string screenshotSaveName, ImageSaveFormat screenshotSaveFormat, ImageDeepthSaveFormat imageDeepthSaveFormat, string _source)
        {
            if (showHalconWindow == null) return null;

            Operation_ImageInfo operation_ImageInfo = new Operation_ImageInfo();
            try
            {
                if (!showHalconWindow.m_bIsNG)
                {
                    imageSaveRelativePath += "\\OK\\";
                    screenshotSaveRelativePath += "\\OK\\";
                }
                else
                {
                    imageSaveRelativePath += "\\NG\\";
                    screenshotSaveRelativePath += "\\NG\\";
                }

                operation_ImageInfo.showHalconWindow = showHalconWindow;
                operation_ImageInfo.str_ImageSaveBasePath = imageSaveBasePath;
                operation_ImageInfo.str_ImageSaveRelativePath = imageSaveRelativePath;
                operation_ImageInfo.str_ImageSaveName = imageSaveName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + SaveImageTool.GetImageFormat(imageSaveFormat);
                operation_ImageInfo.imageSaveFormat = imageSaveFormat;
                operation_ImageInfo.bIsSaveImage = GetIsSaveImage(imageSaveSelect, !showHalconWindow.m_bIsNG);

                operation_ImageInfo.bIsShowImage = isShowImage;
                operation_ImageInfo.bIsCompression = bIsDeepthCompression;
                operation_ImageInfo.imageDeepthSaveFormat = imageDeepthSaveFormat;
                operation_ImageInfo.HighDefinition_Screenshot = screenshotHighDefinition;
                operation_ImageInfo.bIsSaveScreenshot = GetIsSaveImage(screenshotSaveSelect, !showHalconWindow.m_bIsNG);
                operation_ImageInfo.str_ScreenshotSaveBasePath = screenshotSaveBasePath;
                operation_ImageInfo.str_ScreenshotSaveRelativePath = screenshotSaveRelativePath;
                operation_ImageInfo.str_ScreenshotSaveName = screenshotSaveName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + SaveImageTool.GetImageFormat(screenshotSaveFormat);
                operation_ImageInfo.screenshotSaveFormat = screenshotSaveFormat;


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(_source, ex);
            }
            return operation_ImageInfo;
        }
        /// <summary>
        /// 图像数据类型转换
        /// </summary>
        /// <param name="showHalconWindow"></param>
        /// <param name="imageSavePath"></param>
        /// <param name="imageSaveName"></param>
        /// <param name="imageSaveFormat"></param>
        /// <param name="imageSaveSelect"></param>
        /// <param name="isShowImage"></param>
        /// <param name="isSaveScreenshot"></param>
        /// <param name="_source"></param>
        /// <returns></returns>
        public static Operation_ImageInfo ImageDataTypeTransition_Pretargeting(ShowHalconWindow showHalconWindow, string imageSaveBasePath, string imageSaveRelativePath, string imageSaveName, ImageSaveFormat imageSaveFormat, ImageSaveSelect imageSaveSelect, bool isShowImage,
            bool screenshotHighDefinition, ImageSaveSelect screenshotSaveSelect, string screenshotSaveBasePath, string screenshotSaveRelativePath, string screenshotSaveName, ImageSaveFormat screenshotSaveFormat, string _source, string addPath)
        {
            if (showHalconWindow == null) return null;

            Operation_ImageInfo operation_ImageInfo = new Operation_ImageInfo();
            try
            {
                if (!showHalconWindow.m_bIsNG)
                {
                    imageSaveRelativePath += $"\\OK\\{addPath}\\";
                    screenshotSaveRelativePath += $"\\OK\\{addPath}\\";
                }
                else
                {
                    imageSaveRelativePath += $"\\NG\\{addPath}\\";
                    screenshotSaveRelativePath += $"\\NG\\{addPath}\\";
                }

                operation_ImageInfo.showHalconWindow = showHalconWindow;
                operation_ImageInfo.str_ImageSaveBasePath = imageSaveBasePath;
                operation_ImageInfo.str_ImageSaveRelativePath = imageSaveRelativePath;
                operation_ImageInfo.str_ImageSaveName = imageSaveName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + SaveImageTool.GetImageFormat(imageSaveFormat); ;
                operation_ImageInfo.imageSaveFormat = imageSaveFormat;
                operation_ImageInfo.bIsSaveImage = GetIsSaveImage(imageSaveSelect, !showHalconWindow.m_bIsNG);
               

                operation_ImageInfo.bIsShowImage = isShowImage;
                operation_ImageInfo.HighDefinition_Screenshot = screenshotHighDefinition;
                operation_ImageInfo.bIsSaveScreenshot = GetIsSaveImage(screenshotSaveSelect, !showHalconWindow.m_bIsNG);
                operation_ImageInfo.str_ScreenshotSaveBasePath = screenshotSaveBasePath;
                operation_ImageInfo.str_ScreenshotSaveRelativePath = screenshotSaveRelativePath;
                operation_ImageInfo.str_ScreenshotSaveName = screenshotSaveName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + SaveImageTool.GetImageFormat(screenshotSaveFormat);
                operation_ImageInfo.screenshotSaveFormat = screenshotSaveFormat;


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(_source, ex);
            }
            return operation_ImageInfo;
        }

        /// <summary>
        /// NG图像数据类型转换
        /// </summary>
        /// <param name="ngTypeInfo"></param>
        /// <param name="imageSavePath"></param>
        /// <param name="imageSaveName"></param>
        /// <param name="imageSaveFormat"></param>
        /// <param name="_source"></param>
        /// <returns></returns>
        public static Operation_ImageInfo NGImageDataTypeTransition(NgTypeInfo ngTypeInfo, string imageSaveBasePath, string imageSaveRelativePath, string imageSaveName, ImageSaveFormat imageSaveFormat, bool isSaveImage, bool isShowImage, string _source)
        {
            if (ngTypeInfo == null) return null;

            Operation_ImageInfo operation_ImageInfo = new Operation_ImageInfo();
            try
            {

                operation_ImageInfo.showHalconWindow = new ShowHalconWindow();
                operation_ImageInfo.showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                operation_ImageInfo.showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                operation_ImageInfo.showHalconWindow.m_hImage = ngTypeInfo.m_hImage;
                operation_ImageInfo.showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ngTypeInfo.m_hRegion, m_stuGetColor = HalconColor.red });

                operation_ImageInfo.str_ImageSaveBasePath = imageSaveBasePath;
                operation_ImageInfo.str_ImageSaveRelativePath = imageSaveRelativePath;
                operation_ImageInfo.str_ImageSaveName = imageSaveName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff")+$"(H[{ngTypeInfo.m_fNgTypHeight.ToString("0.00")}],W[{ngTypeInfo.m_fNgTypWidth.ToString("0.00")}],A[{ngTypeInfo.m_fNgTypArea.ToString("0.00")}])" + SaveImageTool.GetImageFormat(imageSaveFormat); ;
                operation_ImageInfo.imageSaveFormat = imageSaveFormat;
                operation_ImageInfo.bIsSaveImage = isSaveImage;
                operation_ImageInfo.bIsShowImage = isShowImage;
                operation_ImageInfo.showHalconWindow.m_nMode = 3;
                ngTypeInfo.bIsSave = isSaveImage;
                ngTypeInfo.strPath = imageSaveBasePath + imageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                int rowLocation = 20;
                int rowInterval = 20;

                operation_ImageInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo()
                {
                    m_strMessage = _source,
                    m_stuGetColor = HalconColor.green,
                    m_fRow = rowLocation += rowInterval,
                    m_fCol = 100,
                    //m_nFontSize = 20
                });

                operation_ImageInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo()
                {
                    m_strMessage = ngTypeInfo.m_nImgIndex.ToString(),
                    m_stuGetColor = HalconColor.red,
                    m_fRow = rowLocation += rowInterval,
                    m_fCol = 100,
                });
                operation_ImageInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo()
                {
                    m_strMessage = ngTypeInfo.m_strNgTypeName.ToString(),
                    m_stuGetColor = HalconColor.red,
                    m_fRow = rowLocation += rowInterval,
                    m_fCol = 100,
                });
                operation_ImageInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo()
                {
                    m_strMessage = "Area:" + ngTypeInfo.m_fNgTypArea.ToString("F0"),
                    m_stuGetColor = HalconColor.red,
                    m_fRow = rowLocation += rowInterval,
                    m_fCol = 100,
                });
                operation_ImageInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo()
                {
                    m_strMessage = "Height:" + ngTypeInfo.m_fNgTypHeight.ToString("F0"),
                    m_stuGetColor = HalconColor.red,
                    m_fRow = rowLocation += rowInterval,
                    m_fCol = 100,
                });
                operation_ImageInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo()
                {
                    m_strMessage = "Width:" + ngTypeInfo.m_fNgTypWidth.ToString("F0"),
                    m_stuGetColor = HalconColor.red,
                    m_fRow = rowLocation += rowInterval,
                    m_fCol = 100,
                });
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(_source, ex);
            }

            return operation_ImageInfo;
        }


        /// <summary>
        /// 根据图像窗体队列依次获取图像窗体
        /// </summary>
        /// <param name="displayWindowClasses"></param>
        /// <param name="displayWindowClasses_Index"></param>
        /// <returns></returns>
        public static DisplayWindowClass GetDisplayWindowClass(string _source, List<DisplayWindowClass> displayWindowClasses, ref int displayWindowClasses_Index)
        {

            DisplayWindowClass displayWindowClass = null;
            try
            {
                if (displayWindowClasses == null) return null;

                if (displayWindowClasses_Index >= displayWindowClasses.Count || displayWindowClasses_Index < 0)
                {
                    displayWindowClasses_Index = 0;
                }

                for (int i = 0; i < displayWindowClasses.Count; i++)
                {
                    displayWindowClass = displayWindowClasses[0];
                    if (displayWindowClasses_Index == i)
                    {
                        displayWindowClass = displayWindowClasses[i];
                        break;
                    }

                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(_source, ex);
            }
            return displayWindowClass;
        }


        static DateTime GCIntervalTime = DateTime.Now;
        /// <summary>
        /// 执行GC
        /// </summary>
        public static void ExecuteGC()
        {
            if (true)
            {
                DateTime date1 = DateTime.Now;
                if (date1.Subtract(GCIntervalTime).TotalMilliseconds > 2000)
                {
                    GCIntervalTime = date1;
                    GC.Collect();
                }
            }
        }


        /// <summary>
        /// 获取尺寸检测项
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<SizeEnum> GetSizeDetectionItems(List<SizeItemInfo> inputList)
        {
            if (inputList == null) return null;
            if (inputList.Count == 0) return new List<SizeEnum>();
            List<SizeEnum> _SizeDetectionItems = new List<SizeEnum>();
            for (int i = 0; i < Enum.GetValues(typeof(SizeEnum)).Length; i++)
            {
                if (inputList.Count > i && inputList[i].Enable)
                {
                    _SizeDetectionItems.Add((SizeEnum)(i));
                }

            }
            return _SizeDetectionItems;
        }

        /// <summary>
        /// 获取尺寸检测项
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<SizeItemInfo> GetSizeDetectionItemInfos(List<SizeItemInfo> inputList)
        {
            if (inputList == null) return null;
            if (inputList.Count == 0) return new List<SizeItemInfo>();

            return inputList;
            //for (int i = 0; i < Enum.GetValues(typeof(SizeEnum)).Length; i++)
            //{
            //    if (inputList[i].Enable)
            //    {
            //        _SizeDetectionItems.Add(inputList[i]);
            //    }
            //}
            // return _SizeDetectionItems;
        }

        /// <summary>
        /// 获取缺陷检测项
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<Defct1Enum> GetDefectDetectionItems(List<DefectItemInfo> inputList)
        {
            if (inputList == null) return null;
            if (inputList.Count == 0) return new List<Defct1Enum>();
            List<Defct1Enum> _SizeDetectionItems = new List<Defct1Enum>();
            for (int i = 0; i < Enum.GetValues(typeof(Defct1Enum)).Length; i++)
            {
                if (inputList[i].Enable)
                {
                    _SizeDetectionItems.Add((Defct1Enum)(i));
                }

            }
            return _SizeDetectionItems;
        }

        /// <summary>
        /// 获取启用缺陷检测项工艺信息
        /// </summary>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<DefectItemInfo> GetDefectDetectionItemInfos(List<DefectItemInfo> inputList)
        {
            if (inputList == null) return null;
            if (inputList.Count == 0) return new List<DefectItemInfo>();

            List<DefectItemInfo> _SizeDetectionItems = new List<DefectItemInfo>();
            for (int i = 0; i < Enum.GetValues(typeof(Defct1Enum)).Length; i++)
            {
                if (inputList[i].Enable)
                {
                    _SizeDetectionItems.Add(inputList[i]);
                }
            }

            return _SizeDetectionItems;
        }

        public static void InitDicDefectDetectionItemData(List<DefectItemInfo> inputList, ref Dictionary<Defct1Enum, DefectDetectionItemData> defectDetectionItemData)
        {
            if (inputList == null) return;
            if (inputList.Count == 0) return;

            for (int i = 0; i < inputList.Count; i++)
            {
                defectDetectionItemData[(Defct1Enum)i] = new DefectDetectionItemData();
            }

        }

        public static void InitDicSizeDetectionItemData(List<SizeItemInfo> inputList, ref Dictionary<string, SizeDetectionItemData> sizeDetectionItemData)
        {
            if (inputList == null) return;
            if (inputList.Count == 0) return;

            foreach (var item in inputList)
            {
                sizeDetectionItemData[item.Name] = new SizeDetectionItemData();
            }

        }


        /// <summary>
        /// 将图像队列转换为显示图像队列
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="imageSaveParamer"></param>
        /// <param name="imageInfosShow"></param>
        /// <param name="ngTypeInfos"></param>
        public static void NGImageQueueTransition(CameraEnum cameraEnum, List<NgTypeInfo> ngTypeInfos, long frame, bool isDB, ref QueuePackage<Operation_ImageInfo> out_operationImageInfos, bool abnormal = false)
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
                    string imageSavePath = "";
                    if (abnormal) imageSavePath = in_imageSaveParamer.ImageSave_Path_Min + "\\" + "异常";
                    else imageSavePath = in_imageSaveParamer.ImageSave_Path_Min + "\\" + (isDB ? "打标" : "不打标");
                    imageSavePath += $"\\{ngTypeInfo.m_strNgTypeName}\\";
                    ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Min;
                    bool isSaveImage = in_imageSaveParamer.IsSaveImage_Min;
                    bool isShowImage = in_imageSaveParamer.IsShowImage_Min;
                    string imageSaveName = ngTypeInfo.m_nImgIndex.ToString().PadLeft(5, '0') + "_" + (ngTypeInfo.m_iProductIndex).ToString();
                    Operation_ImageInfo operation_ImageInfo = BIZ_General.NGImageDataTypeTransition(ngTypeInfo, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, isSaveImage, isShowImage,
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
        /// 将图像队列转换为显示图像队列
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="imageSaveParamer"></param>
        /// <param name="imageInfosShow"></param>
        /// <param name="ngTypeInfos"></param>
        public static void NGImageQueueTransition_Slice(CameraEnum cameraEnum, List<NgTypeInfo> ngTypeInfos, long frame, bool isDB, ref QueuePackage<Operation_ImageInfo> out_operationImageInfos, bool abnormal = false, int nSliceIndex = 0)
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
                    string imageSavePath = "";

                    if (nSliceIndex > 0)
                    {
                        imageSavePath = in_imageSaveParamer.ImageSave_Path_Min + $"\\{nSliceIndex}条";
                    }
                    else
                    {
                        imageSavePath = in_imageSaveParamer.ImageSave_Path_Min;
                    }
                    if (abnormal) imageSavePath = imageSavePath + "\\" + "异常";
                    else imageSavePath = imageSavePath + "\\" + (isDB ? "打标" : "不打标");
                    imageSavePath += $"\\{ngTypeInfo.m_strNgTypeName}\\";
                    ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Min;
                    bool isSaveImage = in_imageSaveParamer.IsSaveImage_Min;
                    bool isShowImage = in_imageSaveParamer.IsShowImage_Min;
                    string imageSaveName = ngTypeInfo.m_nImgIndex.ToString().PadLeft(5, '0') + "_" + (ngTypeInfo.m_iProductIndex).ToString();
                    Operation_ImageInfo operation_ImageInfo = BIZ_General.NGImageDataTypeTransition(ngTypeInfo, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, isSaveImage, isShowImage,
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
        /// 计算处理时间
        /// </summary>
        /// <param name="productBase"></param>
        public static void CalculateProcessingTime(ProductBase productBase)
        {
            double hTime1 = ((productBase.Image_FindLineTime - productBase.Image_StartTime) * 1000).D;//抓边算法耗时
            double hTime2 = ((productBase.Image_FindDefectTime - productBase.Image_FindLineTime) * 1000).D;//缺陷检测耗时
            double hTime3 = ((productBase.Image_ArithmeticProcessTime - productBase.Image_StartTime) * 1000).D;//算法总耗时
            double hTime4 = ((productBase.Image_DataProcessTime - productBase.Image_StartDataProcessTime) * 1000).D;//数据处理总耗时
            double hTime5 = ((productBase.Image_EndTime - productBase.Image_StartTime) * 1000).D;//总耗时

            productBase.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = "抓边处理时长：" + hTime1.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
            productBase.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = "缺陷处理时长：" + hTime2.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
            productBase.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = "算法总处理时长：" + hTime3.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
            productBase.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = "数据总处理时长：" + hTime4.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
            productBase.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = "总处理时长：" + hTime5.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
        }

        #region 工艺判断逻辑
        /// <summary>
        /// 尺寸工艺处理
        /// </summary>
        /// <param name="cameraEnum">相机</param>
        /// <param name="sizeItemInfosCraft">尺寸工艺</param>
        /// <param name="ngSizeInfos">尺寸数据</param>
        /// <param name="sizeDetectionItemData">NG缺陷计数</param>
        /// <param name="result">输出结果</param>
        /// <param name="defectRowDatas">输出缺陷数据显示保存</param>
        public static void SizeCraftJudgment(CameraEnum cameraEnum, List<SizeItemInfo> sizeItemInfosCraft, List<SizeInfo> sizeInfos,
            ref Dictionary<string, SizeDetectionItemData> sizeDetectionItemData, out ResultType sizeAllResultType)
        {
            try
            {
                bool IsNG = false;
                bool IsWarn = false;

                //==========================================================
                //【】尺寸数据处理
                foreach (var sizeInfo in sizeInfos)
                {
                    //==================
                    //获取工艺
                    SizeItemInfo craftInfo = sizeItemInfosCraft.Find(x => x.Name == sizeInfo.SizeName);

                    //判断是否启用
                    if (craftInfo != null && !craftInfo.Enable)
                    {
                        continue;
                    }

                    //判断工艺是否存在、尺寸类型计数是否存在
                    if (craftInfo == null)
                    {
                        string str1 = "当前尺寸工艺不存在";
                        string str = $"{cameraEnum}-{sizeInfo.SizeName}-描述：{craftInfo?.Description}——{str1},请检查工艺参数配置";
                        Log4netRecord.ErrorLog.Error(str);
                        TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, str);

                        continue;
                    }



                    //判断单个缺陷结果
                    SizeSpecificationJudgment(sizeInfo, craftInfo, out bool DBResult, out bool TJResult, out bool BJResult);

                    //保证字典键值安全
                    DictionaryKeySafety(sizeInfo.SizeName, ref sizeDetectionItemData);

                    //打标、报警、停机判断
                    if (true)
                    {
                        //打标判断
                        if (!DBResult)
                        {
                            if (craftInfo.DB)
                            {
                                IsNG = true;
                                sizeInfo.resultType = ResultType.NG;
                            }
                            else
                            {
                                IsWarn = true;
                                sizeInfo.resultType = ResultType.Warn;
                            }
                        }
                        else
                        {
                            sizeInfo.resultType = ResultType.OK;
                        }

                        //停机判断
                        bool isTJ = false;
                        if (!TJResult)
                        {
                            if (craftInfo.TJ)
                            {
                                sizeDetectionItemData[sizeInfo.SizeName].TJNGCount++;
                                isTJ = sizeDetectionItemData[sizeInfo.SizeName].TJNGCount >= craftInfo.CountNGTJ;
                                if (isTJ)
                                {
                                    sizeDetectionItemData[sizeInfo.SizeName].TJNGCount = 0;
                                    sizeDetectionItemData[sizeInfo.SizeName].BJNGCount = 0;
                                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{cameraEnum}-{sizeInfo.SizeName}-描述：{craftInfo.Description}");
                                }
                            }
                        }
                        //报警判断
                        if (!isTJ)
                        {
                            if (!BJResult)
                            {
                                if (craftInfo.BJ)
                                {
                                    sizeDetectionItemData[sizeInfo.SizeName].BJNGCount++;
                                    bool isBJ = sizeDetectionItemData[sizeInfo.SizeName].BJNGCount >= craftInfo.CountNGBJ;
                                    if (isBJ)
                                    {
                                        sizeDetectionItemData[sizeInfo.SizeName].BJNGCount = 0;
                                        TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, $"{cameraEnum}-{sizeInfo.SizeName}-描述：{craftInfo.Description}");
                                    }
                                }
                            }
                        }

                        //尺寸数据处理
                        sizeDetectionItemData[sizeInfo.SizeName].currentDetectionData.resultType = sizeInfo.resultType;
                        sizeDetectionItemData[sizeInfo.SizeName].currentDetectionData.dataStr = sizeInfo.DataResult.ToString("F2");
                        sizeDetectionItemData[sizeInfo.SizeName].lChartAllData.Add(new ChartPoint() { ChartDetectionData = sizeInfo.DataResult }, 100);
                        sizeDetectionItemData[sizeInfo.SizeName].lChartAllTempData.Add(new ChartPoint() { ChartDetectionData = sizeInfo.DataResult }, 100);

                    }

                }

                //====================================================
                //【】判断结果
                if (IsNG)
                {
                    sizeAllResultType = ResultType.NG;
                }
                else if (IsWarn)
                {
                    sizeAllResultType = ResultType.Warn;
                }
                else
                {
                    sizeAllResultType = ResultType.OK;
                }
            }
            catch (Exception ex)
            {
                sizeAllResultType = ResultType.NG;
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{cameraEnum}尺寸数据处理异常");
            }
        }

        /// <summary>
        /// 缺陷工艺处理
        /// </summary>
        /// <param name="cameraEnum">相机</param>
        /// <param name="defectItemInfosCraft">缺陷工艺</param>
        /// <param name="ngDefectInfos">NG缺陷数据</param>
        /// <param name="defectDetectionItemData">NG缺陷计数</param>
        /// <param name="result">输出结果</param>
        /// <param name="defectRowDatas">输出缺陷数据显示保存</param>
        public static void DefectCraftJudgment(CameraEnum cameraEnum, List<DefectItemInfo> defectItemInfosCraft, List<NgTypeInfo> ngDefectInfos,
            ref Dictionary<Defct1Enum, DefectDetectionItemData> defectDetectionItemData, ref DisplayWindowClass HWC, out ResultType defectAllResultType)
        {
            try
            {
                //初始化
                bool IsNG = false;
                bool IsWarn = false;
                foreach (var item in defectDetectionItemData)
                {
                    item.Value.InitDetectionInfo();
                }

                //==========================================================
                //【】NG数据处理
                foreach (var ngInfo in ngDefectInfos)
                {
                    //==================
                    //获取工艺
                    DefectItemInfo craftInfo = defectItemInfosCraft.Find(x => x.Name == ngInfo.m_strNgTypeName.ToString());

                    //判断是否启用
                    if (craftInfo != null && !craftInfo.Enable)
                    {
                        //添加NG图像至显示、保存
                        NGImageQueueTransition(cameraEnum, new List<NgTypeInfo>() { ngInfo }, ngInfo.m_nImgIndex, false, ref HWC.operation_Image.imageInfosShow, true);
                        continue;
                    }

                    //判断工艺是否存在、缺陷类型计数是否存在
                    if (craftInfo == null)
                    {
                        string str1 = "当前缺陷工艺不存在";
                        string str = $"{cameraEnum}-{ngInfo.m_strNgTypeName}-描述：{craftInfo?.Description}——{str1},请检查工艺参数配置";
                        Log4netRecord.ErrorLog.Error(str);
                        TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, str);

                        //添加NG图像至显示、保存
                        NGImageQueueTransition(cameraEnum, new List<NgTypeInfo>() { ngInfo }, ngInfo.m_nImgIndex, false, ref HWC.operation_Image.imageInfosShow, true);
                        continue;
                    }

                    //保证字典键值安全
                    DictionaryKeySafety(ngInfo.m_strNgTypeName, ref defectDetectionItemData);

                    //判断单个缺陷结果
                    DefectSpecificationJudgment(ngInfo, craftInfo, out bool ngResult);

                    //打标、报警、停机判断
                    if (!ngResult)
                    {
                        //打标判断
                        if (craftInfo.DB)
                        {
                            defectDetectionItemData[ngInfo.m_strNgTypeName].DBNGCount++;
                            bool isDB = defectDetectionItemData[ngInfo.m_strNgTypeName].DBNGCount >= craftInfo.CountNGDB;
                            if (isDB)
                            {
                                defectDetectionItemData[ngInfo.m_strNgTypeName].DBNGCount = 0;

                                IsNG = true;
                                defectDetectionItemData[ngInfo.m_strNgTypeName].IsNG = true;
                                ngInfo.resultType = ResultType.NG;

                                if (ngInfo.m_fNgTypArea > defectDetectionItemData[ngInfo.m_strNgTypeName].MaxArea)
                                {
                                    //更新最大面积
                                    defectDetectionItemData[ngInfo.m_strNgTypeName].MaxArea = ngInfo.m_fNgTypArea;
                                }
                            }
                            else
                            {
                                if (defectDetectionItemData[ngInfo.m_strNgTypeName].resultType != ResultType.NG)
                                {
                                    IsWarn = true;
                                    defectDetectionItemData[ngInfo.m_strNgTypeName].IsWarn = true;
                                    ngInfo.resultType = ResultType.Warn;

                                    if (ngInfo.m_fNgTypArea > defectDetectionItemData[ngInfo.m_strNgTypeName].MaxArea)
                                    {
                                        //更新最大面积
                                        defectDetectionItemData[ngInfo.m_strNgTypeName].MaxArea = ngInfo.m_fNgTypArea;

                                    }
                                }
                            }
                        }
                        else
                        {
                            IsWarn = true;
                            defectDetectionItemData[ngInfo.m_strNgTypeName].IsWarn = true;
                            ngInfo.resultType = ResultType.Warn;

                            if (ngInfo.m_fNgTypArea > defectDetectionItemData[ngInfo.m_strNgTypeName].MaxArea)
                            {
                                //更新最大面积
                                defectDetectionItemData[ngInfo.m_strNgTypeName].MaxArea = ngInfo.m_fNgTypArea;
                            }
                        }

                        //停机判断
                        bool isTJ = false;
                        if (craftInfo.TJ)
                        {
                            defectDetectionItemData[ngInfo.m_strNgTypeName].TJNGCount++;
                            isTJ = defectDetectionItemData[ngInfo.m_strNgTypeName].TJNGCount >= craftInfo.CountNGTJ;
                            if (isTJ)
                            {
                                defectDetectionItemData[ngInfo.m_strNgTypeName].TJNGCount = 0;
                                defectDetectionItemData[ngInfo.m_strNgTypeName].BJNGCount = 0;
                                TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{cameraEnum}-{ngInfo.m_strNgTypeName}-描述：{craftInfo.Description}");
                            }
                        }
                        //报警判断
                        if (!isTJ)
                        {
                            if (craftInfo.BJ)
                            {
                                defectDetectionItemData[ngInfo.m_strNgTypeName].BJNGCount++;
                                bool isBJ = defectDetectionItemData[ngInfo.m_strNgTypeName].BJNGCount >= craftInfo.CountNGBJ;
                                if (isBJ)
                                {
                                    defectDetectionItemData[ngInfo.m_strNgTypeName].BJNGCount = 0;
                                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, $"{cameraEnum}-{ngInfo.m_strNgTypeName}-描述：{craftInfo.Description}");
                                }
                            }
                        }

                    }


                    //添加NG图像至显示、保存
                    bool isDBTemp = defectDetectionItemData[ngInfo.m_strNgTypeName].resultType == ResultType.NG;
                    NGImageQueueTransition(cameraEnum, new List<NgTypeInfo>() { ngInfo }, ngInfo.m_nImgIndex, isDBTemp, ref HWC.operation_Image.imageInfosShow);
                }

                //====================================================
                //【】判断结果
                if (IsNG)
                {
                    defectAllResultType = ResultType.NG;
                }
                else if (IsWarn)
                {
                    defectAllResultType = ResultType.Warn;
                }
                else
                {
                    defectAllResultType = ResultType.OK;
                }

                foreach (var item in defectDetectionItemData)
                {
                    if (item.Value.IsNG)
                    {
                        item.Value.resultType = ResultType.NG;
                    }
                    else if (item.Value.IsWarn)
                    {
                        item.Value.resultType = ResultType.Warn;
                    }
                    else
                    {
                        item.Value.resultType = ResultType.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                defectAllResultType = ResultType.NG;
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{cameraEnum}缺陷数据处理异常");
            }
        }

        /// <summary>
        /// 尺寸工艺处理
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="sizeItemInfosCraft"></param>
        /// <param name="in_sizeInfos"></param>
        public static void SizeOffsetCaftJudgment(CameraEnum cameraEnum, List<SizeItemInfo> sizeItemInfosCraft, List<SizeInfo> in_sizeInfos, out ResultType sizeAllResultType)
        {
            try
            {
                bool IsNG = false;
                foreach (var sizeInfo in in_sizeInfos)
                {
                    //获取工艺
                    SizeItemInfo craftInfo = sizeItemInfosCraft.Find(x => x.Name == sizeInfo.SizeName);

                    //判断是否启用
                    if (craftInfo != null && !craftInfo.Enable)
                    {
                        continue;
                    }

                    //判断工艺是否存在、尺寸类型计数是否存在
                    if (craftInfo == null)
                    {
                        string str1 = "当前尺寸工艺不存在";
                        string str = $"{cameraEnum}-{sizeInfo.SizeName}-描述：{craftInfo?.Description}——{str1},请检查工艺参数配置";
                        Log4netRecord.ErrorLog.Error(str);
                        TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, str);

                        continue;
                    }

                    //结果判断                  
                    SizeSpecificationJudgment(sizeInfo, craftInfo, out bool DBResult, out bool TJResult, out bool BJResult);
                    if (DBResult)
                    {
                        sizeInfo.resultType = ResultType.OK;
                    }
                    else
                    {
                        sizeInfo.resultType = ResultType.NG;
                        IsNG = true;
                    }
                }

                //====================================================
                //【】判断结果
                if (IsNG)
                {
                    sizeAllResultType = ResultType.NG;
                }
                else
                {
                    sizeAllResultType = ResultType.OK;
                }
            }
            catch (Exception ex)
            {
                sizeAllResultType = ResultType.NG;
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{cameraEnum}尺寸数据处理异常");
            }
        }

        /// <summary>
        /// 尺寸工艺处理
        /// </summary>
        /// <param name="cameraEnum">相机</param>
        /// <param name="sizeItemInfosCraft">尺寸工艺</param>
        /// <param name="ngSizeInfos">尺寸数据</param>
        /// <param name="sizeDetectionItemData">NG缺陷计数</param>
        /// <param name="result">输出结果</param>
        /// <param name="defectRowDatas">输出缺陷数据显示保存</param>
        public static void SizeCraftJudgment_DieCutFolding(CameraEnum cameraEnum, List<SizeItemInfo> sizeItemInfosCraft, List<SizeInfo> sizeInfos,
            ref Dictionary<string, SizeDetectionItemData> sizeDetectionItemData, out ResultType sizeAllResultType)
        {
            try
            {
                bool IsNG = false;
                bool IsWarn = false;

                //==========================================================
                //【】尺寸数据处理
                foreach (var sizeInfo in sizeInfos)
                {
                    //==================
                    //获取工艺
                    SizeItemInfo craftInfo = sizeItemInfosCraft.Find(x => x.Name == sizeInfo.SizeName);

                    //判断是否启用
                    if (craftInfo != null && !craftInfo.Enable)
                    {
                        continue;
                    }

                    //判断工艺是否存在、尺寸类型计数是否存在
                    if (craftInfo == null)
                    {
                        string str1 = "当前尺寸工艺不存在";
                        string str = $"{cameraEnum}-{sizeInfo.SizeName}-描述：{craftInfo?.Description}——{str1},请检查工艺参数配置";
                        Log4netRecord.ErrorLog.Error(str);
                        TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, str);

                        continue;
                    }


                    //判断单个缺陷结果
                    SizeSpecificationJudgment(sizeInfo, craftInfo, out bool DBResult, out bool TJResult, out bool BJResult);

                    //保证字典键值安全
                    DictionaryKeySafety(sizeInfo.SizeName, ref sizeDetectionItemData);

                    //打标、报警、停机判断
                    //打标判断
                    if (!DBResult)
                    {
                        if (craftInfo.DB)
                        {
                            IsNG = true;
                            sizeInfo.resultType = ResultType.NG;
                        }
                        else
                        {
                            IsWarn = true;
                            sizeInfo.resultType = ResultType.Warn;
                        }
                    }
                    else
                    {
                        sizeInfo.resultType = ResultType.OK;
                    }

                    //尺寸数据处理                 
                    sizeDetectionItemData[sizeInfo.SizeName].currentDetectionData.resultType = sizeInfo.resultType;
                    sizeDetectionItemData[sizeInfo.SizeName].currentDetectionData.dataStr = sizeInfo.DataResult.ToString("F2");
                    sizeDetectionItemData[sizeInfo.SizeName].lChartAllData.Add(new ChartPoint() { ChartDetectionData = sizeInfo.DataResult }, 100);
                    sizeDetectionItemData[sizeInfo.SizeName].lChartAllTempData.Add(new ChartPoint() { ChartDetectionData = sizeInfo.DataResult }, 100);
                }

                //====================================================
                //【】判断结果
                if (IsNG)
                {
                    sizeAllResultType = ResultType.NG;
                }
                else if (IsWarn)
                {
                    sizeAllResultType = ResultType.Warn;
                }
                else
                {
                    sizeAllResultType = ResultType.OK;
                }
            }
            catch (Exception ex)
            {
                sizeAllResultType = ResultType.NG;
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{cameraEnum}尺寸数据处理异常");
            }
        }


        /// <summary>
        /// 缺陷工艺处理-切叠项目模切段缺陷处理
        /// </summary>
        /// <param name="cameraEnum">相机</param>
        /// <param name="defectItemInfosCraft">缺陷工艺</param>
        /// <param name="ngDefectInfos">NG缺陷数据</param>
        /// <param name="defectDetectionItemData">NG缺陷计数</param>
        /// <param name="result">输出结果</param>
        /// <param name="defectRowDatas">输出缺陷数据显示保存</param>
        public static void DefectCraftProcessing_DieCutFolding(CameraEnum cameraEnum, List<DefectItemInfo> defectItemInfosCraft, List<NgTypeInfo> ngDefectInfos,
              ref Dictionary<Defct1Enum, DefectDetectionItemData> defectDetectionItemData, ref DisplayWindowClass HWC, out bool isJD, out bool isDefect)
        {
            isJD = false;
            isDefect = false;

            try
            {
                //初始化              
                foreach (var item in defectDetectionItemData)
                {
                    item.Value.InitDetectionInfo();
                }

                //==========================================================
                //【】NG数据处理
                foreach (var ngInfo in ngDefectInfos)
                {
                    //==================
                    //获取工艺
                    DefectItemInfo craftInfo = defectItemInfosCraft.Find(x => x.Name == ngInfo.m_strNgTypeName.ToString());

                    //判断是否启用
                    if (craftInfo != null && !craftInfo.Enable)
                    {
                        //添加NG图像至显示、保存
                        NGImageQueueTransition(cameraEnum, new List<NgTypeInfo>() { ngInfo }, ngInfo.m_nImgIndex, false, ref HWC.operation_Image.imageInfosShow, true);
                        continue;
                    }

                    //判断工艺是否存在
                    if (craftInfo == null)
                    {
                        string str1 = "当前缺陷工艺不存在";
                        string str = $"{cameraEnum}-{ngInfo.m_strNgTypeName}-描述：{craftInfo?.Description}——{str1},请检查工艺参数配置";
                        Log4netRecord.ErrorLog.Error(str);
                        TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, str);

                        //添加NG图像至显示、保存
                        NGImageQueueTransition(cameraEnum, new List<NgTypeInfo>() { ngInfo }, ngInfo.m_nImgIndex, false, ref HWC.operation_Image.imageInfosShow, true);
                        continue;
                    }

                    //保证字典键值安全
                    DictionaryKeySafety(ngInfo.m_strNgTypeName, ref defectDetectionItemData);

                    //判断单个缺陷结果
                    DefectSpecificationJudgment(ngInfo, craftInfo, out bool ngResult);

                    //打标、报警、停机判断
                    if (!ngResult)
                    {
                        if (craftInfo.DB)
                        {
                            ngInfo.resultType = ResultType.NG;
                            isDefect = true;
                            if (ngInfo.m_strNgTypeName == Defct1Enum.接带)
                            {
                                isJD = true;
                            }
                        }
                        else
                        {
                            ngInfo.resultType = ResultType.Warn;
                        }
                    }
                    else
                    {
                        ngInfo.resultType = ResultType.OK;
                    }


                    //添加NG图像至显示、保存
                    bool isDBTemp = isJD || isDefect;
                    NGImageQueueTransition(cameraEnum, new List<NgTypeInfo>() { ngInfo }, ngInfo.m_nImgIndex, isDBTemp, ref HWC.operation_Image.imageInfosShow);
                }


            }
            catch (Exception ex)
            {
                isDefect = true;
                isJD = true;
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, $"{cameraEnum}缺陷数据处理异常");
            }
        }

        #endregion

        /// <summary>
        /// 单个规格判断逻辑
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="currentDefect"></param>
        /// <param name="defectCraft"></param>
        /// <param name="result"></param>
        public static void SizeSpecificationJudgment(SizeInfo currentDefect, SizeItemInfo defectCraft, out bool DBResult, out bool TJResult, out bool BJResult)
        {
            currentDefect.upperLimitValue = defectCraft.StDValue + Math.Abs(defectCraft.StD_Up);
            currentDefect.lowerLimitValue = defectCraft.StDValue - Math.Abs(defectCraft.StD_Down);
            currentDefect.StandardValue = defectCraft.StDValue;
            currentDefect.SizeName = defectCraft.Description;
            if (currentDefect.DataResult >= defectCraft.StDValue - Math.Abs(defectCraft.StD_Down) &&
                currentDefect.DataResult <= defectCraft.StDValue + Math.Abs(defectCraft.StD_Up))
            {
                DBResult = true;
            }
            else
            {
                DBResult = false;
            }

            if (currentDefect.DataResult >= defectCraft.StDValue - Math.Abs(defectCraft.StD_TJ_Down) &&
               currentDefect.DataResult <= defectCraft.StDValue + Math.Abs(defectCraft.StD_TJ_Up))
            {
                TJResult = true;
            }
            else
            {
                TJResult = false;
            }

            if (currentDefect.DataResult >= defectCraft.StDValue - Math.Abs(defectCraft.StD_BJ_Down) &&
               currentDefect.DataResult <= defectCraft.StDValue + Math.Abs(defectCraft.StD_BJ_Up))
            {
                BJResult = true;
            }
            else
            {
                BJResult = false;
            }
        }

        /// <summary>
        /// 单个缺陷判断逻辑
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="currentDefect"></param>
        /// <param name="defectCraft"></param>
        /// <param name="result"></param>
        public static void DefectSpecificationJudgment(NgTypeInfo currentDefect, DefectItemInfo defectCraft, out bool result)
        {
            bool areaResult = true;//单个缺陷面积结果
            bool heightResult = true;//单个缺陷高度结果
            bool widthResult = true;//单个缺陷宽度结果
            bool ngResult = true;//单个缺陷结果
            if (currentDefect.m_fNgTypArea > defectCraft.Area)
            {
                areaResult = false;
            }

            if (currentDefect.m_fNgTypWidth > defectCraft.Width)
            {
                widthResult = false;
            }

            if (currentDefect.m_fNgTypHeight > defectCraft.Height)
            {
                heightResult = false;
            }

            //=====================================================================================================
            //缺陷逻辑
            //面积
            ngResult = areaResult;
            //宽
            if (defectCraft.WidthLogic) ngResult |= widthResult;
            else ngResult &= widthResult;

            //高
            if (defectCraft.HeightLogic) ngResult |= heightResult;
            else ngResult &= heightResult;

            result = ngResult;
        }

        /// <summary>
        /// 字典键值安全
        /// </summary>
        /// <param name="defectName"></param>
        /// <param name="dicDefectDetectionItemData"></param>
        public static void DictionaryKeySafety(Defct1Enum defectName, ref Dictionary<Defct1Enum, DefectDetectionItemData> dicDefectDetectionItemData)
        {
            if (dicDefectDetectionItemData.ContainsKey(defectName))
            {
                return;
            }
            else
            {
                dicDefectDetectionItemData.Add(defectName, new DefectDetectionItemData());
            }
        }

        /// <summary>
        /// 字典键值安全
        /// </summary>
        /// <param name="defectName"></param>
        /// <param name="dicDefectDetectionItemData"></param>
        public static void DictionaryKeySafety(string sizeName, ref Dictionary<string, SizeDetectionItemData> dicSizeDetectionItemData)
        {
            if (dicSizeDetectionItemData.ContainsKey(sizeName))
            {
                return;
            }
            else
            {
                dicSizeDetectionItemData.Add(sizeName, new SizeDetectionItemData());
            }
        }

        /// <summary>
        /// 生产统计计算
        /// </summary>
        /// <param name="statisticalData"></param>
        /// <param name="sumupCount"></param>
        /// <param name="ngCount"></param>
        public static void ProdStatisticsCalculate(StatisticalData statisticalData, double sumupCount, double ngCount)
        {
            statisticalData.SumupCount = sumupCount;
            statisticalData.NGCount = ngCount;
            statisticalData.OKCount = statisticalData.SumupCount - statisticalData.NGCount;

            if (statisticalData.SumupCount != 0)
            {
                statisticalData.Yield = statisticalData.OKCount / statisticalData.SumupCount * 100;
            }
        }


        public static bool CalculateLines(CameraEnum cameraEnum, LineInfo lineInfo1, LineInfo lineInfo2, string Edtype, double coefficient, out double Val)
        {
            Val = -999;
            try
            {

                double calibValueX = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_X;
                double calibValueY = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_Y;
                if (Edtype == EditorTypeEnum.线距离X.ToString())
                {
                    double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                    double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                    Val = line1midcol - line2midcol;
                    Val = Val * calibValueX * coefficient;
                }
                else if (Edtype == EditorTypeEnum.线距离Y.ToString() || Edtype == EditorTypeEnum.极片隔膜Y.ToString())
                {
                    double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                    double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;
                    Val = line1midrow - line2midrow;
                    Val = Val * calibValueY * coefficient;
                }

                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CalculateLines_coefficient(LineInfo lineInfo1, LineInfo lineInfo2, string Edtype, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {

                if (Edtype == EditorTypeEnum.线距离X.ToString())
                {
                    double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                    double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                    Val = line1midcol - line2midcol;
                    Val = Val * coefficient + RealFixOffset;
                }
                else if (Edtype == EditorTypeEnum.线距离Y.ToString())
                {
                    double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                    double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;
                    Val = line1midrow - line2midrow;
                    Val = Val * coefficient + RealFixOffset;
                }
                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 计算点线距离，预定位工位使用此方法。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="Edtype"></param>
        /// <param name="coefficient"></param>
        /// <param name="RealFixOffset"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public static bool CalculateLinesPl(LineInfo lineInfo1, LineInfo lineInfo2, string Edtype, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {

                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                if (Edtype == EditorTypeEnum.线距离X.ToString())
                {

                    HOperatorSet.DistancePl(line1midrow, line1midcol, lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple dist);
                    Val = dist.D;
                    Val = Val * coefficient + RealFixOffset;
                }
                else if (Edtype == EditorTypeEnum.线距离Y.ToString())
                {
                    HOperatorSet.DistancePl(line1midrow, line1midcol, lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple dist);
                    Val = dist.D;
                    Val = Val * coefficient + RealFixOffset;
                }

                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void AddMessageShowWindow(List<SizeInfo> sizeInfos, ShowHalconWindow showHalconWindow)
        {
            for (int i = 0; i < sizeInfos.Count; i++)
            {

                string message = $"{sizeInfos[i].SizeName}：{sizeInfos[i].DataResult.ToString("F3")}  #[{sizeInfos[i].lowerLimitValue}_{sizeInfos[i].upperLimitValue}]";

                if (sizeInfos[i].resultType == ResultType.OK)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = message, m_stuGetColor = HalconColor.green });
                }
                else if (sizeInfos[i].resultType == ResultType.NG)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = message, m_stuGetColor = HalconColor.red });
                }
                else if (sizeInfos[i].resultType == ResultType.Warn)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = message, m_stuGetColor = HalconColor.yellow });
                }
            }
        }

        public static void AddMessageShowWindow(List<NgTypeInfo> ngTypeInfos, ShowHalconWindow showHalconWindow, int sliceIndex)
        {
            try
            {
                for (int i = 0; i < ngTypeInfos.Count; i++)
                {
                    string message = $"S{sliceIndex}_{ngTypeInfos[i].m_strNgTypeName.ToString()}：{ngTypeInfos[i].m_fNgTypArea.ToString("F3")}";
                    showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ngTypeInfos[i].m_hRegion.Clone(), m_stuGetColor = BIZ_General.Color(ngTypeInfos[i].resultType) }); ;
                    if (ngTypeInfos[i].resultType == ResultType.OK)
                    {
                        showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = message, m_stuGetColor = HalconColor.green });

                    }
                    else if (ngTypeInfos[i].resultType == ResultType.NG)
                    {
                        showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = message, m_stuGetColor = HalconColor.red });

                    }
                    else if (ngTypeInfos[i].resultType == ResultType.Warn)
                    {
                        showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = message, m_stuGetColor = HalconColor.orange });

                    }
                }
            }
            catch (Exception ee)
            {

                MessageNotify.AddErrorLog("AddMessageShowWindow：" + ee.ToString()); ;
            }
       
        }




        public static HalconColor Color(ResultType resultType)
        {
            if (resultType == ResultType.OK)
            {
                return HalconColor.green;
            }
            else if (resultType == ResultType.Warn)
            {
                return HalconColor.orange;
            }
            else
            {
                return HalconColor.red;
            }

        }

        public static List<(string ControlName, string ControlType)> GetAllControlNames(Control control)
        {
            List<(string ControlName, string ControlType)> controlNames = new List<(string, string)>();

            // 添加当前控件的名称和类型
            if (!string.IsNullOrEmpty(control.Name))
            {
                if(control.GetType().Name== "FormsPlot")
                {

                }
                controlNames.Add((control.Text, control.GetType().Name));
            }

            // 递归遍历子控件
            foreach (Control childControl in control.Controls)
            {
                controlNames.AddRange(GetAllControlNames(childControl));
            }

            return controlNames;
        }
    }
}
