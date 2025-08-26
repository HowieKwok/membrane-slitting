using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class DetectionData
    {

    }
    /// <summary>
    /// 单个相机业务变量
    /// </summary>
    public class CamDetectionVariable
    {
        /// <summary>
        /// 生产数据路径
        /// </summary>
        public string ProdDataPath = "";

        public string ProdSizeDataPath = "";


        public string ProdDeftDataPath = "";


        public string ProdEADataPath = "";



        /// <summary>
        /// 统计数据
        /// </summary>
        public StatisticalData statisticalData = new StatisticalData();

        /// <summary>
        /// 尺寸检测数据
        /// </summary>
        public Dictionary<string, SizeDetectionItemData> sizeDetectionItemData = new Dictionary<string, SizeDetectionItemData>();

        /// <summary>
        /// 缺陷检测数据
        /// </summary>
        public Dictionary<Defct1Enum, DefectDetectionItemData> defectDetectionItemData = new Dictionary<Defct1Enum, DefectDetectionItemData>();

        /// <summary>
        /// 数据显示表头
        /// </summary>
        public string[] DataTitlesShow = new string[0];

        /// <summary>
        /// 数据保存表头
        /// </summary>
        public string[] DataTitlesSave = new string[0];

        /// <summary>
        /// 数据显示表头
        /// </summary>
        public string[] DataTitlesShow2 = new string[0];

        /// <summary>
        /// 数据保存表头
        /// </summary>
        public string[] DataTitlesSave2 = new string[0];
    }

    /// <summary>
    /// 尺寸检测项信息
    /// </summary>
    public class SizeDetectionItemData
    {
        /// <summary>
        /// 当前检测数据
        /// </summary>
        public RowData currentDetectionData = new RowData();

        /// <summary>
        /// 历史检测数据平均值
        /// </summary>
        public double MeanValue = 0;

        /// <summary>
        /// 打标NG计数
        /// </summary>
        public int DBNGCount = 0;
        /// <summary>
        /// 报警NG计数
        /// </summary>
        public int BJNGCount = 0;
        /// <summary>
        /// 停机NG计数
        /// </summary>
        public int TJNGCount = 0;

        /// <summary>
        /// 检测项打标分布计数
        /// </summary>
        public long dbDistributionCount = 0;

        /// <summary>
        /// 检测项曲线图数据
        /// </summary>
        public ListPackage<ChartPoint> lChartAllData = new ListPackage<ChartPoint>();

        /// <summary>
        /// 检测项曲线图临时数据
        /// </summary>
        public ListPackage<ChartPoint> lChartAllTempData = new ListPackage<ChartPoint>();

    }
    public class ChartPoint
    {
        /// <summary>
        /// 检测值
        /// </summary>
        public double ChartDetectionData = 0;

        /// <summary>
        /// 平均值
        /// </summary>
        public double ChartMeanData = 0;
    }
    /// <summary>
    /// 缺陷检测项信息
    /// </summary>
    public class DefectDetectionItemData
    {
        #region 整个膜卷数据记录
        /// <summary>
        /// 打标NG计数
        /// </summary>
        public int DBNGCount = 0;
        /// <summary>
        /// 报警NG计数
        /// </summary>
        public int BJNGCount = 0;
        /// <summary>
        /// 停机NG计数
        /// </summary>
        public int TJNGCount = 0;

        /// <summary>
        /// 检测项打标分布计数
        /// </summary>
        public long dbDistributionCount = 0;

        #endregion

        #region 单帧图像处理数据
        public void InitDetectionInfo()
        {
            IsNG = false;
            IsWarn = false;
            resultType = ResultType.NG;
            MaxArea = 0;
        }

        public bool IsNG = false;
        public bool IsWarn = false;
        /// <summary>
        /// 结果类型
        /// </summary>
        public ResultType resultType = ResultType.NG;

        /// <summary>
        /// 最大面积
        /// </summary>
        public double MaxArea = 0;
        #endregion
    }

    /// <summary>
    /// 统计数据
    /// </summary>
    [Serializable]
    public class StatisticalData
    {
        /// <summary>
        /// 总数
        /// </summary>
        public double SumupCount = 0;

        /// <summary>
        /// OK数量
        /// </summary>
        public double OKCount = 0;

        /// <summary>
        /// NG数量
        /// </summary>
        public double NGCount = 0;

        /// <summary>
        /// 良率
        /// </summary>
        public double Yield = 0;


        /// <summary>
        /// 打标发送数量
        /// </summary>
        public int DBSendCount = 0;

        /// <summary>
        /// 打标接收数量
        /// </summary>
        public int DBReceiveCount = 0;

        /// <summary>
        /// 打标完成数量
        /// </summary>
        public int DBFinishNum = 0;


        /// <summary>
        /// 生产总长度
        /// </summary>
        public double fTotolLength = 0;
        /// <summary>
        /// 生产OK长度
        /// </summary>
        public double fOkLength = 0;
        /// <summary>
        /// 生产NG长度
        /// </summary>
        public double fNgLength = 0;

        public void InitData()
        {
            SumupCount = 0;
            OKCount = 0;
            NGCount = 0;
            Yield = 0;
            DBSendCount = 0;
            DBReceiveCount = 0;
            DBFinishNum = 0;
            fTotolLength = 0;
            fOkLength = 0;
            fNgLength = 0;


        }
    }
}
