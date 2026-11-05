using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Vml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Frame_Core
{
    public static class DefectPointInspectionWorks
    {
        ///// <summary>
        ///// 缺陷检测
        ///// </summary>
        ///// <param name="cameraEnum"></param>
        ///// <param name="ho_Image"></param>
        ///// <param name="imgPath"></param>
        ///// <param name="showHalconWindow"></param>
        ///// <param name="dieCutLaminationParameter"></param>
        ///// <returns></returns>
        //public static ExecuteResult DefectMeasureFlow(CameraEnum cameraEnum, HObject ho_Image, string imgPath, ref ShowHalconWindow showHalconWindow, ref DieCutLaminationParameter dieCutLaminationParameter)
        //{
        //    ExecuteResult executeResult = new ExecuteResult();
        //    HTuple start = null, end = null;
        //    HOperatorSet.CountSeconds(out start);


        //    try
        //    {
        //        //图像信息显示
        //        if (showHalconWindow.m_messageShowInfos == null) showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
        //        if (showHalconWindow.m_lineShowInfos == null) showHalconWindow.m_lineShowInfos = new List<LineInfo>();
        //        if (showHalconWindow.m_regionShowInfos == null) showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();


        //        DirectoryHelper.ParseFilePath(imgPath, out string driveLetter, out List<string> directoryLevels, out string fileName, out string fileExtension);
        //        int camIndex = (int)cameraEnum;
        //        Process proc = Project.Instance.GetProcessById(camIndex);
        //        //ImgDetectParam imgDetectParam = null;
        //        //List<string> defectTypes = new List<string>();
        //        //string currentDefectType = "";
        //        //if (true)
        //        //{

        //        //    //加载图像数据
        //        //    HOperatorSet.GetImageSize(ho_Image, out HTuple imgWidth, out HTuple imgHeight);
        //        //    dieCutLaminationParameter.m_queuePreProcessImgData.Enqueue(
        //        //                                                 new PreProcessImgData()
        //        //                                                 {
        //        //                                                     m_hoProduceImg = ho_Image,
        //        //                                                     m_nlastProduceEarRow = new PoleEarsInfo(),
        //        //                                                     m_nCurProduceEarRow = new PoleEarsInfo()
        //        //                                                     {
        //        //                                                         image_ID_Begin = 0,
        //        //                                                         Row_Begin = 0,
        //        //                                                         image_ID_End = 0,
        //        //                                                         Row_End = imgHeight,
        //        //                                                         image_ID_Mid = 0,
        //        //                                                         Row_Mid = imgHeight / 2,
        //        //                                                     },


        //        //                                                     m_stdCamType = CameraEnum.Cam1,

        //        //                                                     m_stdProduceImgBaseInfo = new ImgInfo()
        //        //                                                     {
        //        //                                                         m_nImgIndex = 1,
        //        //                                                         m_nImgHeadPixelPos = 0,
        //        //                                                         m_nImgHeight = imgHeight,
        //        //                                                         m_nImgTailPixelPos = imgHeight,
        //        //                                                         m_nImgWidth = imgWidth,
        //        //                                                         m_nRealtivePos = imgHeight / 2

        //        //                                                     },

        //        //                                                     // 当前图像的基本信息
        //        //                                                     m_stdCurImgBaseInfo = new ImgInfo()
        //        //                                                     {
        //        //                                                         // 当前图像在相机中的索引
        //        //                                                         m_nImgIndex = 1,
        //        //                                                         // 当前图像的头部像素位置（在整个拼接图像中的位置）
        //        //                                                         m_nImgHeadPixelPos = 1,
        //        //                                                         // 当前图像的尾部像素位置
        //        //                                                         m_nImgTailPixelPos = 1,

        //        //                                                         m_nImgHeight = 10000,

        //        //                                                         m_nImgWidth = 16384
        //        //                                                     }
        //        //                                                 });
        //        //    // 开始执行
        //        //    if (camIndex == 0)
        //        //    {
        //        //        Project.Instance.DieCutLaminationProDervice.RunCraftProcessA(proc, true, ref dieCutLaminationParameter, out showHalconWindow, out imgDetectParam);
        //        //    }
        //        //    else if (camIndex == 1)
        //        //    {
        //        //        Project.Instance.DieCutLaminationProDervice.RunCraftProcessB(proc, true, ref dieCutLaminationParameter, out showHalconWindow, out imgDetectParam);
        //        //    }
        //        //    else if (camIndex == 2)
        //        //    {
        //        //        Project.Instance.DieCutLaminationProDervice.RunCraftProcessC(proc, true, ref dieCutLaminationParameter, out showHalconWindow, out imgDetectParam);
        //        //    }
        //        //}

        //        //if (imgDetectParam != null)
        //        //{
        //        //    //数据解析
        //        //    defectTypes = imgDetectParam.m_stdDetectData.Select(p => LanguageProject.GetLangeuageAttribute(p.m_strNgTypeName, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType)).ToList();
        //        //    currentDefectType = AnalyzeDefectType(directoryLevels.Count > 0 ? directoryLevels[directoryLevels.Count - 1] : "");

        //        //    string str1 = LanguageProject.GetLangeuageAttribute(TitleEnum.时间, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //    string str2 = LanguageProject.GetLangeuageAttribute(TitleEnum.图像路径, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //    string str3 = LanguageProject.GetLangeuageAttribute(TitleEnum.所属相机, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //    string str4 = LanguageProject.GetLangeuageAttribute(TitleEnum.NG类型, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //    string str5 = LanguageProject.GetLangeuageAttribute(TitleEnum.实测NG类型, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //    string str6 = LanguageProject.GetLangeuageAttribute(TitleEnum.实测结果, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //    string str7 = LanguageProject.GetLangeuageAttribute(TitleEnum.是否通过, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = str2 + ":" + imgPath, m_stuGetColor = HalconColor.green });
        //        //    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = str3 + ":" + (CameraEnum)camIndex, m_stuGetColor = HalconColor.green });
        //        //    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = str4 + ":" + currentDefectType, m_stuGetColor = HalconColor.green });
        //        //    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = str5 + ":" + string.Join("-|-", defectTypes), m_stuGetColor = HalconColor.green });
        //        //    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = str6 + ":" + (defectTypes.Exists(p => p == currentDefectType) ? "OK" : "NG"), m_stuGetColor = (defectTypes.Exists(p => p == currentDefectType) ? HalconColor.green : HalconColor.red) });


        //        //    imgDetectParam?.Dispose();
        //        //}

        //        executeResult.IsSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        executeResult.IsSuccess = false;
        //        executeResult.Message = ex.Message;
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        HOperatorSet.CountSeconds(out end);
        //        executeResult.Time = ((end - start) * 1000).D;
        //        executeResult.Time = Math.Round(executeResult.Time, 2);
        //    }
        //    return executeResult;
        //}

        ///// <summary>
        ///// 相机一键点检
        ///// </summary>
        ///// <param name="cameraEnum"></param>
        ///// <param name="filePath"></param>
        ///// <param name="fileName"></param>
        ///// <param name="isPass"></param>
        ///// <param name="datass"></param>
        ///// <returns></returns>
        //public static ExecuteResult OnekeyPointInspectionFlow(CameraEnum cameraEnum, string filePath, out bool isPass, out List<RowData[]> datass)
        //{
        //    ExecuteResult executeResult = new ExecuteResult();
        //    HTuple start = null, end = null;
        //    HOperatorSet.CountSeconds(out start);

        //    isPass = false;
        //    datass = new List<RowData[]>();
        //    try
        //    {
        //        if (Project.Instance.M_CameraRelevancies[cameraEnum].m_PointInspectionStateParam.PointInspectionState_Defect == (int)PointInspectionStateEnum.已忽略) return executeResult;           

        //        string str = "缺陷点检";
        //        string pointCheckResultStr = "缺陷点检结果";
        //        string strPass = "通过";
        //        string strFail = "不通过";

        //        DefectPointInspectionParam defectPointInspectionParam = Project.Instance.M_CameraRelevancies[cameraEnum].m_DefectPointInspectionManage.defectPointInspectionParam;
        //        PointInspectionStateParam pointInspectionStateParam = Project.Instance.M_CameraRelevancies[cameraEnum].m_PointInspectionStateParam;
        //        pointInspectionStateParam.PointInspectionState_Defect = (int)PointInspectionStateEnum.点检中;

        //        //加载图像路径
        //        ImageSimManage m_ImageSimManage = new ImageSimManage();

        //        //获取数据表头
        //        string[] pointCheckDataTitle = DefectPointInspectionWorks.GetDefectPointCheckTitles();

        //        //图像信息显示
        //        ShowHalconWindow currentShowHalconWindow = new ShowHalconWindow();
        //        DieCutLaminationParameter dieCutLaminationParameter = new DieCutLaminationParameter();
        //        ImgDetectParam imgDetectParam = null;

        //        int camIndex = (int)cameraEnum;


        //        bool camResults = true;//单相机点检结果

        //        string camName = cameraEnum.ToString();
        //        ExecuteResult DirectoryExecuteResult = DirectoryHelper.GetImagePathsFromSubfolders(defectPointInspectionParam.PointCheckImgPath, DirectoriesNoImage_CallBack, out List<string> imagePaths);
        //        if (imagePaths.Count > 0)
        //        {
        //            m_ImageSimManage.InitSimFlag();
        //            m_ImageSimManage.ImageEmulationPaths = imagePaths.ToArray();

        //            int imgNum = imagePaths.Count;//单相机点检数量
        //            for (int i = 0; i < imgNum; i++)
        //            {
        //                //初始化
        //                m_ImageSimManage.GetZeroIndexStartNextImage(out HObject ho_Image, out string imgPath);
        //                currentShowHalconWindow.Dispose();
        //                currentShowHalconWindow.m_hImage = ho_Image;
        //                dieCutLaminationParameter.Dispose();
        //                imgDetectParam?.Dispose();

        //                //执行
        //                HOperatorSet.GetImageSize(ho_Image, out HTuple imgWidth, out HTuple imgHeight);
        //                dieCutLaminationParameter.m_queuePreProcessImgData.Enqueue(
        //                                                             new PreProcessImgData()
        //                                                             {
        //                                                                 m_hoProduceImg = ho_Image,
        //                                                                 m_nlastProduceEarRow = new PoleEarsInfo(),
        //                                                                 m_nCurProduceEarRow = new PoleEarsInfo()
        //                                                                 {
        //                                                                     image_ID_Begin = 0,
        //                                                                     Row_Begin = 0,
        //                                                                     image_ID_End = 0,
        //                                                                     Row_End = imgHeight,
        //                                                                     image_ID_Mid = 0,
        //                                                                     Row_Mid = imgHeight / 2,
        //                                                                 },


        //                                                                 m_stdCamType = CameraEnum.Cam1,

        //                                                                 m_stdProduceImgBaseInfo = new ImgInfo()
        //                                                                 {
        //                                                                     m_nImgIndex = 1,
        //                                                                     m_nImgHeadPixelPos = 0,
        //                                                                     m_nImgHeight = imgHeight,
        //                                                                     m_nImgTailPixelPos = imgHeight,
        //                                                                     m_nImgWidth = imgWidth,
        //                                                                     m_nRealtivePos = imgHeight / 2

        //                                                                 },

        //                                                                 // 当前图像的基本信息
        //                                                                 m_stdCurImgBaseInfo = new ImgInfo()
        //                                                                 {
        //                                                                     // 当前图像在相机中的索引
        //                                                                     m_nImgIndex = 1,
        //                                                                     // 当前图像的头部像素位置（在整个拼接图像中的位置）
        //                                                                     m_nImgHeadPixelPos = 1,
        //                                                                     // 当前图像的尾部像素位置
        //                                                                     m_nImgTailPixelPos = 1,

        //                                                                     m_nImgHeight = 10000,

        //                                                                     m_nImgWidth = 16384
        //                                                                 }
        //                                                             });

        //                Process proc = Project.Instance.GetProcessById((int)cameraEnum);


        //                // 开始执行
        //                if (camIndex == 0)
        //                {
        //                    Project.Instance.DieCutLaminationProDervice.RunCraftProcessA(proc, true, ref dieCutLaminationParameter, out currentShowHalconWindow, out imgDetectParam);
        //                }
        //                else if (camIndex == 1)
        //                {
        //                    Project.Instance.DieCutLaminationProDervice.RunCraftProcessB(proc, true, ref dieCutLaminationParameter, out currentShowHalconWindow, out imgDetectParam);
        //                }
        //                else if (camIndex == 2)
        //                {
        //                    Project.Instance.DieCutLaminationProDervice.RunCraftProcessC(proc, true, ref dieCutLaminationParameter, out currentShowHalconWindow, out imgDetectParam);
        //                }


        //                //数据解析
        //                DirectoryHelper.ParseFilePath(imgPath, out string driveLetter, out List<string> directoryLevels, out string fileName1, out string fileExtension);
        //                List<string> defectTypes = new List<string>();
        //                string currentDefectType = "";
        //                defectTypes = imgDetectParam.m_stdDetectData.Select(p => LanguageProject.GetLangeuageAttribute(p.m_strNgTypeName, Project.Instance.systemInfo.m_CurrentLanguageType)).ToList();
        //                currentDefectType = DefectPointInspectionWorks.AnalyzeDefectType(directoryLevels.Count > 0 ? directoryLevels[directoryLevels.Count - 1] : "");
        //                bool ActualResult = defectTypes.Exists(p => p == currentDefectType);
        //                camResults &= ActualResult;

        //                //数据处理
        //                List<RowData> rowDatas = new List<RowData>();
        //                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), resultType = ResultType.OK };
        //                RowData imgPathRowData = new RowData() { dataStr = imgPath, resultType = ResultType.OK };
        //                RowData camRowData = new RowData() { dataStr = camName };
        //                RowData NgTypeRowData = new RowData() { dataStr = currentDefectType, resultType = ResultType.OK };
        //                RowData ActualNgTypeRowData = new RowData() { dataStr = string.Join("-|-", defectTypes), resultType = ResultType.OK };
        //                RowData ActualResultRowData = new RowData() { dataStr = ActualResult ? "OK" : "NG", resultType = ActualResult ? ResultType.OK : ResultType.NG };
        //                RowData SumResultRowData = new RowData() { dataStr = camResults ? "OK" : "NG", resultType = camResults ? ResultType.OK : ResultType.NG };
        //                rowDatas.Add(timeRowData);
        //                rowDatas.Add(imgPathRowData);
        //                rowDatas.Add(camRowData);
        //                rowDatas.Add(NgTypeRowData);
        //                rowDatas.Add(ActualNgTypeRowData);
        //                rowDatas.Add(ActualResultRowData);
        //                rowDatas.Add(SumResultRowData);

        //                datass.Add(rowDatas.ToArray());

        //                // 写入点检数据                                       
        //                FileOperationCSV.WriteData_CSV(filePath, pointCheckDataTitle, rowDatas.ToArray(), "");

        //            }

        //        }

        //        //结果
        //        if (!DirectoryExecuteResult.IsSuccess) camResults = false;
        //        isPass = camResults;
        //        if (isPass)
        //        {
        //            pointInspectionStateParam.djLastOKTime_Defect = DateTime.Now;
        //            pointInspectionStateParam.PointInspectionState_Defect = (int)PointInspectionStateEnum.点检成功;

        //            // 写入点检日志
        //            string strResult = cameraEnum.ToString() + pointCheckResultStr + "：" + (isPass ? strPass : strFail) + "\r\n";
        //            PointInspectionWorks.FileOperationINI.Write_SendLog_ini(Project.Instance.paramPathSet.CheckPointRecordPath, strResult);
        //        }
        //        else
        //        {
        //            pointInspectionStateParam.PointInspectionState_Defect = (int)PointInspectionStateEnum.点检失败;
        //        }
        //        pointInspectionStateParam.Save();

        //        //释放资源
        //        currentShowHalconWindow?.Dispose();
        //        dieCutLaminationParameter?.Dispose();
        //        imgDetectParam?.Dispose();


        //        executeResult.IsSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        executeResult.IsSuccess = false;
        //        executeResult.Message = ex.Message;
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        HOperatorSet.CountSeconds(out end);
        //        executeResult.Time = ((end - start) * 1000).D;
        //        executeResult.Time = Math.Round(executeResult.Time, 2);
        //    }
        //    return executeResult;
        //}

        //public static string[] GetDefectPointCheckTitles()
        //{
        //    string str1 = "时间";
        //    string str2 = "图像路径";
        //    string str3 = "所属相机";
        //    string str4 = "NG类型";
        //    string str5 = "实测NG类型";
        //    string str6 = "实测结果";
        //    string str7 = "是否通过";
        //    try
        //    {
        //        //str1 = LanguageProject.GetLangeuageAttribute(TitleEnum.时间, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //str2 = LanguageProject.GetLangeuageAttribute(TitleEnum.图像路径, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //str3 = LanguageProject.GetLangeuageAttribute(TitleEnum.所属相机, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //str4 = LanguageProject.GetLangeuageAttribute(TitleEnum.NG类型, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //str5 = LanguageProject.GetLangeuageAttribute(TitleEnum.实测NG类型, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //str6 = LanguageProject.GetLangeuageAttribute(TitleEnum.实测结果, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
        //        //str7 = LanguageProject.GetLangeuageAttribute(TitleEnum.是否通过, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //    }
        //    return new string[]
        //          {
        //        str1,
        //        str2,
        //        str3,
        //        str4,
        //        str5,
        //        str6,
        //        str7,
        //          };
        //}

        ///// <summary>
        ///// 解析文件名获取缺陷类型
        ///// </summary>
        ///// <param name="imgPath"></param>
        ///// <returns></returns>
        //public static string AnalyzeDefectType(string imgPath)
        //{
        //    string[] strings = imgPath.Split('_');
        //    if (strings.Length == 0) return imgPath;
        //    Defct1Enum? defectTypeTemp = LanguageProject.MatchStringToEnum<Defct1Enum>(strings[0]);

        //    if (defectTypeTemp == null) return strings[0];

        //    Defct1Enum defct1Enum = defectTypeTemp ?? Defct1Enum.其它;

        //    return LanguageProject.GetLangeuageAttribute(defct1Enum, Project.Instance.systemInfo.m_CurrentLanguageType);
        //}

        //public static bool DirectoriesNoImage_CallBack(string path)
        //{
        //    string str = "请先在软件根目录存放NG大图路径如下";
        //    if (MessageBox.Show($"{str}\n{path}", "", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
        //    {
        //        return false;
        //    }
        //    System.Diagnostics.Process.Start(path);
        //    return true;
        //}
    }
}
