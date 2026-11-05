using DefectRuleClassifier.Core.Models;
using Frame_Core;
using Frame_Core.流程汇总文件.产品类;
using FrameCommunity;
using FrameCommunity.PID;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class Model_DieCuttingCut : ModelBase
    {
        public CraftParam_DieCuttingCut Craft => Project.Instance.craftBase as CraftParam_DieCuttingCut;

        #region 数据表头和保存路径
        public string Station1_ALineCamPath_Defect { get; set; } = "";
        public string Station1_BLineCamPath_Defect { get; set; } = "";

        public string Station1_DefectCamSendPath { get; set; } = "";

        public string Station2_CutCamPath { get; set; } = "";
        public string Station2_CutCamSendPath { get; set; } = "";

        public string Station3_Path_Size { get; set; } = "";

        public string Station3_Path_Defect { get; set; } = "";

        public string Station1_StatisticsPath { get; set; } = "";

        public string Station3_StatisticsPath { get; set; } = "";

        public string Station3_SizeCamSendPath { get; set; } = "";


        public string ALineBig_Time_Path { get; set; } = "";
        public string BLineBig_Time_Path { get; set; } = "";
        public string ALineEA_Time_Path { get; set; } = "";
        public string BLineEA_Time_Path { get; set; } = "";
        public string Cut_Time_Path { get; set; } = "";
        public string SizeCam4_Time_Path { get; set; } = "";
        public string SizeCam5_Time_Path { get; set; } = "";
        public string SizeCam6_Time_Path { get; set; } = "";

        public List<string> ALineBig_Time_Title { get; set; } = new List<string>() { "时间", "帧号", "EA", "图像处理", "数据处理", "总耗时" };
        public List<string> BLineBig_Time_Title { get; set; } = new List<string>() { "时间", "帧号", "EA", "图像处理", "数据处理", "总耗时" };
        public List<string> ALineEA_Time_Title { get; set; } = new List<string>() { "时间", "帧号", "EA", "图像处理", "数据处理", "总耗时" };
        public List<string> BLineEA_Time_Title { get; set; } = new List<string>() { "时间", "帧号", "EA", "图像处理", "数据处理", "总耗时" };
        public List<string> Cut_Time_Title { get; set; } = new List<string>() { "时间", "帧号", "EA", "图像处理", "数据处理", "总耗时" };
        public List<string> SizeCam4_Time_Title { get; set; } = new List<string>() { "时间", "帧号", "EA", "图像处理", "数据处理", "总耗时" };
        public List<string> SizeCam5_Time_Title { get; set; } = new List<string>() { "时间", "帧号", "EA", "图像处理", "数据处理", "总耗时" };
        public List<string> SizeCam6_Time_Title { get; set; } = new List<string>() { "时间", "帧号", "EA", "图像处理", "数据处理", "总耗时" };


        private List<string> _Station1_ALineCamTitle_Defect = new List<string>() { "时间", "帧号", "EA", "名称", "面积", "宽度", "高度", "是否超规格", "是否重度", "是否NG", "是否保存", "路径" };
        public List<string> Station1_ALineCamTitle_Defect => _Station1_ALineCamTitle_Defect;



        private List<string> _Station1_BLineCamTitle_Defect = new List<string>() { "时间", "帧号", "EA", "名称", "面积", "宽度", "高度", "是否超规格", "是否重度", "是否NG", "是否保存", "路径" };
        public List<string> Station1_BLineCamTitle_Defect => _Station1_BLineCamTitle_Defect;

        private List<string> _Station1_CamSendTitle_Defect = new List<string>() { "时间", "当前帧号", "发送EA", "请求信号耗时", "发送结果", "连续NG", "通讯耗时" };
        public List<string> Station1_CamSendTitle_Defect => _Station1_CamSendTitle_Defect;


        private List<string> _Station2_CutCamTitle = new List<string>() { "时间", "帧号", "裁切纠偏量", "V角纠偏量", /*"裁切帧号", "裁切发送值", "V角帧号", "V角发送值",*/ "无极耳", "寻极耳中", "清除队列", "X纠偏值", "裁切刀补偿值", "V角补偿值", "肩宽补偿值", "X", "Y", "运行内存", "CPU", "磁盘容量" };
        public List<string> Station2_CutCamTitle => _Station2_CutCamTitle;

        private List<string> _Station2_CutCamSendTitle = new List<string>() { "时间", "帧号", "裁切帧号", "裁切发送值", "V角帧号", "V角发送值", "无极耳" };
        public List<string> Station2_CutCamSendTitle => _Station2_CutCamSendTitle;


        private List<string> _Station3_Title_Size = new List<string>();
        public List<string> Station3_Title_Size
        {
            get
            {
                _Station3_Title_Size.Clear();
                _Station3_Title_Size.Add("时间");
                _Station3_Title_Size.Add("帧号");
                _Station3_Title_Size.AddRange(Craft.Get_Station3_SizeNames());
                return _Station3_Title_Size;
            }
        }


        private List<string> _Station3_Title_Defect = new List<string>() { "时间", "相机", "帧号", "名称", "面积", "宽度", "高度", "是否超规格", "是否NG", "是否保存", "路径" };
        public List<string> Station3_Title_Defect => _Station3_Title_Defect;

        private List<string> _Station3_CamSendTitle_Size = new List<string>() { "时间", "当前帧号", "发送帧号", "发送结果", "连续NG", "通讯耗时" };
        public List<string> Station3_CamSendTitle_Size => _Station3_CamSendTitle_Size;


        private List<string> _Station1_Title_Statistics = new List<string>() { "检测项", "数量", "占比" };
        public List<string> Station1_Title_Statistics => _Station1_Title_Statistics;


        private List<string> _Station3_Title_Statistics = new List<string>() { "检测项", "数量", "占比" };
        public List<string> Station3_Title_Statistics => _Station3_Title_Statistics;



        #endregion

        public TempData_DieCuttingCut tempData_DieCuttingCut = new TempData_DieCuttingCut();


        #region 缺陷工位数据汇总   
        //【】产品临时数据
        //public readonly object ALineCam_sync = new object();
        //public readonly Dictionary<long, ProductDataResult> ALine_DieCutEAProduct = new Dictionary<long, ProductDataResult>();


        //【】数据统计、数据分布
        public QueuePackage<List<QualityData>> DefectStation_BindingDatasShow { get; } = new QueuePackage<List<QualityData>>();
        public QueuePackage<List<QualityData>> DefectStation_BindingDatasSave { get; } = new QueuePackage<List<QualityData>>();


        public StatisticalData Station1_StatisticalData = new StatisticalData();
        public Dictionary<string, StatisticalDistribution> Station1_StatisticalDistribution { get; set; } = new Dictionary<string, StatisticalDistribution>();

        //【】产品
        public QueuePackage<ProductDataResult> ALine_DieCutEAProduct { get; } = new QueuePackage<ProductDataResult>();
        public QueuePackage<ProductDataResult> BLine_DieCutEAProduct { get; } = new QueuePackage<ProductDataResult>();
        public QueuePackage<Station1Product> Station1_Product { get; } = new QueuePackage<Station1Product>();

        public IntervalExecution DefectStation_IntervalExecution = new IntervalExecution();

        public int DefectStation_ContinuousCount = 0;

        public long DefectStation_CurrentFrame = 0;
        public DateTime DefectStation_UpdateTime = DateTime.Now;
        #endregion


        #region 裁切工位数据汇总 
        //【】产品   
        public CorrectPosDeal Cut_CorrectPosDeal { get; } = new CorrectPosDeal(10, 100);
        public CorrectPosDeal V_CorrectPosDeal { get; } = new CorrectPosDeal(10, 100);
        public IntervalExecution CutStation_IntervalExecution = new IntervalExecution();

        public CorrectLogic M_CorrectLogic { get; } = new CorrectLogic();
        public CorrectLogic correctLogic { get; } = new CorrectLogic();
        public CorrecInfo Cut_CorrecInfo { get; } = new CorrecInfo();//当前结果

        public CorrecInfo V_CorrecInfo { get; } = new CorrecInfo();//当前结果

        public int CutStation_ContinuousCount = 0;
        public bool IsFindTab = false;
        #endregion

        #region 尺寸工位数据汇总
        //【】合片
        public readonly object _sync = new object();
        public readonly AutoResetEvent Completed = new AutoResetEvent(false);
        public readonly Dictionary<string, Station3Product> _pending = new Dictionary<string, Station3Product>();

        //【】数据统计、数据分布   
        public QueuePackage<List<QualityData>> SizeStation_BindingDatasShow { get; } = new QueuePackage<List<QualityData>>();
        public QueuePackage<List<QualityData>> SizeStation_BindingDatasSave { get; } = new QueuePackage<List<QualityData>>();

        public StatisticalData Station3_StatisticalData = new StatisticalData();
        public Dictionary<string, StatisticalDistribution> Station3_StatisticalDistribution { get; set; } = new Dictionary<string, StatisticalDistribution>();

        //【】数据保存与显示
        public DataSaveWorkClass<DataSaveClass> DataSaveQueue = new DataSaveWorkClass<DataSaveClass>();
        public ConcurrentQueue<List<List<RowData>>> Station3_Defect_DataShowQueue = new ConcurrentQueue<List<List<RowData>>>();

        //【】产品
        public Station3Result Station3_LastResult { get; } = new Station3Result();

        public IntervalExecution SizeStation_IntervalExecution = new IntervalExecution();

        public BackSizeDeviationCompensator 肩宽_PID = new BackSizeDeviationCompensator();

        public int SizeStation_ContinuousCount = 0;
        #endregion


        #region 图像显示与保存数据
        //public ImageQueueData Cam1_SourceImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_SourceImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_SourceImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(0));
        //public ImageQueueData Cam1_SourceImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(0));       
        //public ImageQueueData Cam1_SourceImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(0));

        public ImageQueueData Cam1_EAImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_EAImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_EAImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_EAImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_EAImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(0));

        public ImageQueueData Cam1_BigImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_BigImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_BigImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(0));
        //public ImageQueueData Cam1_BigImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(0));
        //public ImageQueueData Cam1_BigImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(0));

        public ImageQueueData Cam1_NGDefectImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_NGDefectImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(0));
        public ImageQueueData Cam1_NGDefectImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(0));

        //public ImageQueueData Cam2_SourceImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_SourceImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_SourceImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(1));
        //public ImageQueueData Cam2_SourceImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(1));
        //public ImageQueueData Cam2_SourceImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(1));

        public ImageQueueData Cam2_EAImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_EAImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_EAImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_EAImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_EAImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(1));

        public ImageQueueData Cam2_BigImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_BigImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_BigImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(1));
        //public ImageQueueData Cam2_BigImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(1));
        //public ImageQueueData Cam2_BigImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(1));

        public ImageQueueData Cam2_NGDefectImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_NGDefectImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(1));
        public ImageQueueData Cam2_NGDefectImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(1));

        public ImageQueueData Cam3_SourceImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(2));
        public ImageQueueData Cam3_SourceImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(2));
        public ImageQueueData Cam3_SourceImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(2));
        public ImageQueueData Cam3_SourceImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(2));
        public ImageQueueData Cam3_SourceImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(2));


        public ImageQueueData Cam4_SourceImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(3));
        public ImageQueueData Cam4_SourceImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(3));
        public ImageQueueData Cam4_SourceImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(3));
        public ImageQueueData Cam4_SourceImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(3));
        public ImageQueueData Cam4_SourceImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(3));

        public ImageQueueData Cam4_NGDefectImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(3));
        public ImageQueueData Cam4_NGDefectImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(3));
        public ImageQueueData Cam4_NGDefectImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(3));

        public ImageQueueData Cam5_SourceImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(4));
        public ImageQueueData Cam5_SourceImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(4));
        public ImageQueueData Cam5_SourceImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(4));
        public ImageQueueData Cam5_SourceImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(4));
        public ImageQueueData Cam5_SourceImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(4));

        public ImageQueueData Cam5_NGDefectImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(4));
        public ImageQueueData Cam5_NGDefectImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(4));
        public ImageQueueData Cam5_NGDefectImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(4));

        public ImageQueueData Cam6_SourceImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(5));
        public ImageQueueData Cam6_SourceImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(5));
        public ImageQueueData Cam6_SourceImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(5));
        public ImageQueueData Cam6_SourceImage_ScreenshotSave = new ImageQueueData(Service_Camera.GetCam_Name(5));
        public ImageQueueData Cam6_SourceImage_HighScreenshotQueue = new ImageQueueData(Service_Camera.GetCam_Name(5));

        public ImageQueueData Cam6_NGDefectImage_ImageShow = new ImageQueueData(Service_Camera.GetCam_Name(5));
        public ImageQueueData Cam6_NGDefectImage_ImageSave = new ImageQueueData(Service_Camera.GetCam_Name(5));
        public ImageQueueData Cam6_NGDefectImage_ImageDispose = new ImageQueueData(Service_Camera.GetCam_Name(5));
        #endregion

        #region 曲线图
        public Dictionary<string, ScotPotUIUser> chartUIUser { get; } = new Dictionary<string, ScotPotUIUser>();
        #endregion
        public override void Init()
        {
            IsFindTab = false;
            InitFilePath();
            Reset_DefectStation();
            Reset_CutStation(true);
            Reset_SizeStation();



            tempData_DieCuttingCut = new TempData_DieCuttingCut();

            Station1_StatisticalData = new StatisticalData();
            Station1_StatisticalDistribution.Clear();
            Station3_StatisticalData = new StatisticalData();
            Station3_StatisticalDistribution.Clear();


            //Cam1_SourceImage_ImageShow.Dispose();
            Cam1_SourceImage_ImageSave.Dispose();
            Cam1_SourceImage_ImageDispose.Dispose();
            //Cam1_SourceImage_ScreenshotSave.Dispose();
            //Cam1_SourceImage_HighScreenshotQueue.Dispose();

            Cam1_EAImage_ImageShow.Dispose();
            Cam1_EAImage_ImageSave.Dispose();
            Cam1_EAImage_ImageDispose.Dispose();
            Cam1_EAImage_ScreenshotSave.Dispose();
            Cam1_EAImage_HighScreenshotQueue.Dispose();

            Cam1_BigImage_ImageShow.Dispose();
            Cam1_BigImage_ImageSave.Dispose();
            Cam1_BigImage_ImageDispose.Dispose();
            //Cam1_BigImage_ScreenshotSave.Dispose();
            //Cam1_BigImage_HighScreenshotQueue.Dispose();

            Cam1_NGDefectImage_ImageShow.Dispose();
            Cam1_NGDefectImage_ImageSave.Dispose();
            Cam1_NGDefectImage_ImageDispose.Dispose();


            //Cam2_SourceImage_ImageShow.Dispose();
            Cam2_SourceImage_ImageSave.Dispose();
            Cam2_SourceImage_ImageDispose.Dispose();
            //Cam2_SourceImage_ScreenshotSave.Dispose();
            //Cam2_SourceImage_HighScreenshotQueue.Dispose();

            Cam2_EAImage_ImageShow.Dispose();
            Cam2_EAImage_ImageSave.Dispose();
            Cam2_EAImage_ImageDispose.Dispose();
            Cam2_EAImage_ScreenshotSave.Dispose();
            Cam2_EAImage_HighScreenshotQueue.Dispose();

            Cam2_BigImage_ImageShow.Dispose();
            Cam2_BigImage_ImageSave.Dispose();
            Cam2_BigImage_ImageDispose.Dispose();
            //Cam2_BigImage_ScreenshotSave.Dispose();
            //Cam2_BigImage_HighScreenshotQueue.Dispose();

            Cam2_NGDefectImage_ImageShow.Dispose();
            Cam2_NGDefectImage_ImageSave.Dispose();
            Cam2_NGDefectImage_ImageDispose.Dispose();


            Cam3_SourceImage_ImageShow.Dispose();
            Cam3_SourceImage_ImageSave.Dispose();
            Cam3_SourceImage_ImageDispose.Dispose();
            Cam3_SourceImage_ScreenshotSave.Dispose();
            Cam3_SourceImage_HighScreenshotQueue.Dispose();

            Cam4_SourceImage_ImageShow.Dispose();
            Cam4_SourceImage_ImageSave.Dispose();
            Cam4_SourceImage_ImageDispose.Dispose();
            Cam4_SourceImage_ScreenshotSave.Dispose();
            Cam4_SourceImage_HighScreenshotQueue.Dispose();

            Cam4_NGDefectImage_ImageShow.Dispose();
            Cam4_NGDefectImage_ImageSave.Dispose();
            Cam4_NGDefectImage_ImageDispose.Dispose();


            Cam5_SourceImage_ImageShow.Dispose();
            Cam5_SourceImage_ImageSave.Dispose();
            Cam5_SourceImage_ImageDispose.Dispose();
            Cam5_SourceImage_ScreenshotSave.Dispose();
            Cam5_SourceImage_HighScreenshotQueue.Dispose();

            Cam5_NGDefectImage_ImageShow.Dispose();
            Cam5_NGDefectImage_ImageSave.Dispose();
            Cam5_NGDefectImage_ImageDispose.Dispose();


            Cam6_SourceImage_ImageShow.Dispose();
            Cam6_SourceImage_ImageSave.Dispose();
            Cam6_SourceImage_ImageDispose.Dispose();
            Cam6_SourceImage_ScreenshotSave.Dispose();
            Cam6_SourceImage_HighScreenshotQueue.Dispose();

            Cam6_NGDefectImage_ImageShow.Dispose();
            Cam6_NGDefectImage_ImageSave.Dispose();
            Cam6_NGDefectImage_ImageDispose.Dispose();
        }

        public override void InitFilePath()
        {
            Station1_ALineCamPath_Defect = Project.Instance.paramPathSet.ProductMojuanPath + $"缺陷过程数据_{Service_Camera.GetCam_Name(0)}.csv";
            Station1_BLineCamPath_Defect = Project.Instance.paramPathSet.ProductMojuanPath + $"缺陷过程数据_{Service_Camera.GetCam_Name(1)}.csv";
            Station2_CutCamPath = Project.Instance.paramPathSet.ProductMojuanPath + $"过程数据_{Service_Camera.GetCam_Name(2)}.csv";
            Station2_CutCamSendPath = Project.Instance.paramPathSet.ProductMojuanPath + $"过程发送数据_{Service_Camera.GetCam_Name(2)}.csv";

            Station1_DefectCamSendPath = Project.Instance.paramPathSet.ProductMojuanPath + $"过程发送数据_缺陷检测工位.csv";
            Station3_SizeCamSendPath = Project.Instance.paramPathSet.ProductMojuanPath + $"过程发送数据_极片尺寸工位.csv";

            Station3_Path_Size = Project.Instance.paramPathSet.ProductMojuanPath + $"尺寸过程数据_极片尺寸工位.csv";
            Station3_Path_Defect = Project.Instance.paramPathSet.ProductMojuanPath + $"缺陷过程数据_极片尺寸工位.csv";
            Station1_StatisticsPath = Project.Instance.paramPathSet.ProductMojuanPath + $"数据统计_缺陷检测工位.csv";
            Station3_StatisticsPath = Project.Instance.paramPathSet.ProductMojuanPath + $"数据统计_极片尺寸工位.csv";

            ALineBig_Time_Path = Project.Instance.paramPathSet.ProductMojuanPath + $"检测耗时_{Service_Camera.GetCam_Name(0)}大图.csv";
            ALineEA_Time_Path = Project.Instance.paramPathSet.ProductMojuanPath + $"检测耗时_{Service_Camera.GetCam_Name(0)}EA图.csv";
            BLineBig_Time_Path = Project.Instance.paramPathSet.ProductMojuanPath + $"检测耗时_{Service_Camera.GetCam_Name(1)}大图.csv";
            BLineEA_Time_Path = Project.Instance.paramPathSet.ProductMojuanPath + $"检测耗时_{Service_Camera.GetCam_Name(1)}EA图.csv";
            Cut_Time_Path = Project.Instance.paramPathSet.ProductMojuanPath + $"检测耗时_{Service_Camera.GetCam_Name(2)}.csv";
            SizeCam4_Time_Path = Project.Instance.paramPathSet.ProductMojuanPath + $"检测耗时_{Service_Camera.GetCam_Name(3)}.csv";
            SizeCam5_Time_Path = Project.Instance.paramPathSet.ProductMojuanPath + $"检测耗时_{Service_Camera.GetCam_Name(4)}.csv";
            SizeCam6_Time_Path = Project.Instance.paramPathSet.ProductMojuanPath + $"检测耗时_{Service_Camera.GetCam_Name(5)}.csv";
        }

        public void Reset_DefectStation()
        {


            DefectStation_BindingDatasShow.Clear();
            DefectStation_BindingDatasSave.Clear();

            DefectStation_ContinuousCount = 0;
            ALine_DieCutEAProduct.Clear();


            ALine_DieCutEAProduct.Clear();
            BLine_DieCutEAProduct.Clear();

            while (Station1_Product.Count > 0)
            {
                Station1_Product.Dequeue()?.Dispose();
            }
            //默认生成产品
            int num = 0;
            GlobalVariableItem globalVariableItem = Craft.Get_GlobalVariableItem("BackSideCount");
            if (globalVariableItem == null)
            {
                System.Windows.Forms.MessageBox.Show("获取背面片数数量失败");
            }
            else
            {
                num = globalVariableItem?.Value.ToInt() ?? 0;
            }
            //for (int i = 0; i < num; i++) 
            //{
            //    Station1_Product.Enqueue(new Station1Product());
            //}
        }


        public void Reset_CutStation(bool isSendPLC)
        {
            if (isSendPLC)
            {
                Project.Instance.server_DiecuttingCutPLC.m_DevAddress.清除纠偏数据.Value = 1;
                Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.清除纠偏数据, true, Project.Instance.server_DiecuttingCutPLC.CommunicationLog);

                Project.Instance.server_DiecuttingCutPLC.m_DevAddress.V角纠偏值.Value = (float)0.001;
                Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.V角纠偏值, true, Project.Instance.server_DiecuttingCutPLC.CommunicationLog);

                Project.Instance.server_DiecuttingCutPLC.m_DevAddress.裁切纠偏值.Value = (float)0.001;
                Project.Instance.server_DiecuttingCutPLC.Write(Project.Instance.server_DiecuttingCutPLC.m_DevAddress.裁切纠偏值, true, Project.Instance.server_DiecuttingCutPLC.CommunicationLog);
                MessageNotify.AddCommLog($"裁切工位——发送清除纠偏数据");
            }


            CutStation_ContinuousCount = 0;


            int 相机到切刀片数 = Craft.Get_GlobalVariableItem(2, "CameraToCutterDistanceNum").Value.ToInt();
            int 相机到V切刀片数 = Craft.Get_GlobalVariableItem(2, "CameraToVCutterDistanceNum").Value.ToInt();
            double fPieWidth = Craft.Get_Station3_SizeCraft("单片宽度").StandardValue;
            Cut_CorrectPosDeal.Init(相机到切刀片数, fPieWidth);
            V_CorrectPosDeal.Init(相机到V切刀片数, fPieWidth);
            M_CorrectLogic.Clera();

            Cut_CorrecInfo.UpdateData(new CorrecInfo());

            V_CorrecInfo.UpdateData(new CorrecInfo());


        }

        public void Reset_SizeStation()
        {
            SizeStation_BindingDatasShow.Clear();
            SizeStation_BindingDatasSave.Clear();

            SizeStation_ContinuousCount = 0;

            Station3_LastResult.Reset();


            foreach (var item in _pending)
            {
                item.Value.Dispose();
            }
            _pending.Clear();

            Reset_PID();
        }

        public void Reset_PID()
        {

            肩宽_PID.Reset();

        }

    }

    public class Station1Product : IDisposable
    {
        public bool IsNG = true;
        public long Frame = -1;
        public long ProductID = -1;
        public ProductDataResult ALine_DieCutEAProduct = new ProductDataResult();
        public ProductDataResult BLine_DieCutEAProduct = new ProductDataResult();

        public void Dispose()
        {

        }
    }
    public class Station2Product : IDisposable
    {
        public bool IsNG = true;
        public ProductDataResult Product;

        public double X偏差 = 0;
        public double V角补偿值 = 0;
        public double 裁切刀补偿值 = 0;

        public CorrecInfo correcInfo;

        public double V角纠偏值 = 0;
        public double 裁切纠偏值 = 0;

        public void Dispose()
        {

        }
    }

    public class Station3Product : Station3Result, IDisposable
    {
        public SizeDectectProduct Cam4_Product;
        public SizeDectectProduct Cam5_Product;
        public SizeDectectProduct Cam6_Product;

        public bool IsComplete
        {
            get
            {
                return Cam4_Product != null
                    && Cam5_Product != null
                    && Cam6_Product != null;
            }
        }

        public void SetProduct(int cameraId, SizeDectectProduct product)
        {
            switch (cameraId)
            {
                case 3:
                    Cam4_Product = product;
                    break;
                case 4:
                    Cam5_Product = product;
                    break;
                case 5:
                    Cam6_Product = product;
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            if (Cam4_Product != null) Cam4_Product.Dispose();
            if (Cam5_Product != null) Cam5_Product.Dispose();
            if (Cam6_Product != null) Cam6_Product.Dispose();
        }
    }

    public class Station3Result
    {
        object _lock = new object();
        //public long TriggerId;
        public string SN = "";
        public bool IsNG = true;
        public long Frame_Cam4 = -1;
        public long Frame_Cam5 = -1;
        public long Frame_Cam6 = -1;
        //public DateTime CreateTime = DateTime.Now;
        public void Reset()
        {
            lock (_lock)
            {
                SN = "";
                IsNG = true;
                Frame_Cam4 = -1;
                Frame_Cam5 = -1;
                Frame_Cam6 = -1;
            }
        }
        public void SetValue(string sn, bool isNG, long frame_Cam4, long frame_Cam5, long frame_Cam6)
        {
            lock (_lock)
            {
                SN = sn;
                IsNG = isNG;
                Frame_Cam4 = frame_Cam4;
                Frame_Cam5 = frame_Cam5;
                Frame_Cam6 = frame_Cam6;
            }
        }

        public Station3Result GetValue()
        {
            lock (_lock)
            {
                Station3Result station3Result = new Station3Result();
                station3Result.SN = SN;
                station3Result.IsNG = IsNG;
                station3Result.Frame_Cam4 = Frame_Cam4;
                station3Result.Frame_Cam5 = Frame_Cam5;
                station3Result.Frame_Cam6 = Frame_Cam6;
                return station3Result;
            }
        }

    }

    public class TempData_DieCuttingCut
    {
        private readonly object _sync = new object();
        private readonly object _sync1 = new object();
        private readonly Dictionary<string, CamTempData> Cam_TempDatas = new Dictionary<string, CamTempData>();
        private readonly Dictionary<string, SizeTypeTempData> Station3_SizeTypeTempDatas = new Dictionary<string, SizeTypeTempData>();

        public CamTempData GetCamTempDataInternal(string camName)
        {
            lock (_sync)
            {
                CamTempData data;

                if (!Cam_TempDatas.TryGetValue(camName, out data))
                {
                    data = new CamTempData();
                    Cam_TempDatas.Add(camName, data);
                }

                return data;
            }
        }

        public SizeTypeTempData GetStation3SizeTypeTempData(string sizeName)
        {
            lock (_sync1)
            {
                SizeTypeTempData data;

                if (!Station3_SizeTypeTempDatas.TryGetValue(sizeName, out data))
                {
                    data = new SizeTypeTempData();
                    Station3_SizeTypeTempDatas.Add(sizeName, data);
                }

                return data;
            }
        }
    }



    public class CamTempData
    {
        private readonly object _sync1 = new object();
        private readonly object _sync2 = new object();
        private readonly object _sync3 = new object();
        /// <summary>
        /// 尺寸检测项NG计数
        /// </summary>
        private Dictionary<string, SizeTypeTempData> Cam_SizeTypeTempDatas { get; } = new Dictionary<string, SizeTypeTempData>();
        /// <summary>
        /// 缺陷检测项NG计数
        /// </summary>
        private Dictionary<string, DefectTypeTempData> Cam_DefectTypeTempDatas { get; } = new Dictionary<string, DefectTypeTempData>();
        /// <summary>
        /// NG分布计数
        /// </summary>
        private Dictionary<string, StatisticalDistribution> Cam_StatisticalDistribution { get; } = new Dictionary<string, StatisticalDistribution>();

        public SizeTypeTempData GetSizeTypeTempData(string sizeName)
        {
            lock (_sync1)
            {
                SizeTypeTempData data;

                if (!Cam_SizeTypeTempDatas.TryGetValue(sizeName, out data))
                {
                    data = new SizeTypeTempData();
                    Cam_SizeTypeTempDatas.Add(sizeName, data);
                }

                return data;
            }
        }

        public DefectTypeTempData GetDefectTypeTempData(string defectName)
        {
            lock (_sync2)
            {
                DefectTypeTempData data;

                if (!Cam_DefectTypeTempDatas.TryGetValue(defectName, out data))
                {
                    data = new DefectTypeTempData();
                    Cam_DefectTypeTempDatas.Add(defectName, data);
                }

                return data;
            }
        }

        public StatisticalDistribution GetStatisticalDistribution(string name)
        {
            lock (_sync3)
            {
                StatisticalDistribution data;

                if (!Cam_StatisticalDistribution.TryGetValue(name, out data))
                {
                    data = new StatisticalDistribution();
                    Cam_StatisticalDistribution.Add(name, data);
                }

                return data;
            }
        }
    }

    public class DefectTypeTempData
    {
        public int ContinuousCount = 0;
        public int AccumulateCount = 0;
    }


    public class SizeTypeTempData
    {
        public int ContinuousCount = 0;
        public int AccumulateCount = 0;
    }

    public class RequestSignal
    {

        /// <summary>
        /// 默认NG数量
        /// </summary>
        public int DefaultNgCount = 0;

        /// <summary>
        /// 默认NG数量（裁切工位V角纠偏使用）
        /// </summary>
        public int DefaultNgCount2 = 0;

        public bool GettingResults = false;

        public long Frame = -1000;

        public DateTime RequestTime = DateTime.Now;
    }



}
