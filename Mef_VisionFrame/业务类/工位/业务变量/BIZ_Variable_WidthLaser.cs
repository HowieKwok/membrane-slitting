using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Frame_Core;

using LogTool;

namespace Mef_VisionFrame
{
    public class BIZ_Variable_WidthLaser: IDisposable
    {

        /// <summary>
        /// A面EA产品
        /// </summary>
        public ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_A { get; set; }
        /// <summary>
        /// B面EA产品
        /// </summary>
        public ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_B { get; set; }

        /// <summary>
        /// A面EA产品
        /// </summary>
        public ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_SimA { get; set; }
        /// <summary>
        /// B面EA产品
        /// </summary>
        public ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_SimB { get; set; }

        /// <summary>
        /// EA数据统计表，AB相机数据合体再统计，报警停机也在里面
        /// </summary>
        public List<StatisticalData_Slice> StatisticalData_EASlices{ get; set; }


        public List<StatisticalData_Slice> StatisticalData_EASlicesA { get; set; }
        public List<StatisticalData_Slice> StatisticalData_EASlicesB { get; set; }


        /// <summary>
        /// dgv行头
        /// </summary>
        public List<string> DgvRowheaders { get; set; } = new List<string>();

        /// <summary>
        /// dgv列头
        /// </summary>
        public List<string> DgvColumnheaders { get; set; } = new List<string>();

        /// <summary>
        /// 原图缺项检测表头
        /// </summary>
        public List<string> strDeftInitCam1Title { get; set; } = new List<string>();

        /// <summary>
        /// 原图缺项检测表文件路径
        /// </summary>
        public string strDeftInitCam1Path { get; set; } = "E:\\相机1原图缺项检测表.csv";



        /// <summary>
        /// 原图缺项检测表头
        /// </summary>
        public List<string> strDeftInitCam2Title { get; set; } = new List<string>();

        /// <summary>
        /// 原图缺项检测表文件路径
        /// </summary>
        public string strDeftInitCam2Path { get; set; } = "E:\\相机1原图缺项检测表.csv";

        /// <summary>
        /// 配置尺寸总表
        /// </summary>
        public List<string> strEASizeConfigTitle { get; set; } = new List<string>();
        /// <summary>
        ///  配置尺寸总表文件路径
        /// </summary>
        public string strEASizeConfigPath { get; set; } = "E:\\配置尺寸总表.csv";


        /// <summary>
        ///  生产统计文件路径
        /// </summary>
        public string strProductDataPath { get; set; } = "E:\\生产统计文件.csv";


        /// <summary>
        /// 双相机尺寸汇总表头
        /// </summary>
        public List<string> strEASizeTitle { get; set; } = new List<string>();

        /// <summary>
        /// 条槽位尺寸汇总表头
        /// </summary>
        public List<string> strEASizeSliceTitle { get; set; } = new List<string>();
        /// <summary>
        /// 双相机尺寸汇总文件路径
        /// </summary>
        public string strEASizePath { get; set; } = "E:\\双相机尺寸汇总表.csv";



        /// <summary>
        /// 双相机缺项汇总表
        /// </summary>
        public List<string> strEADeftTitle { get; set; } = new List<string>();
        /// <summary>
        /// 双相机缺项汇总文件路径
        /// </summary>
        public string strEADeftPath { get; set; } = "E:\\双相机缺项汇总表.csv";

        /// <summary>
        /// 双相机尺寸缺项汇总表
        /// </summary>
        public List<string> strEASizeDeftTitle { get; set; } = new List<string>();
        /// <summary>
        ///  双相机尺寸缺项文件路径
        /// </summary>
        public string strEASizeDeftPath { get; set; } = "E:\\双相机尺寸缺项汇总表.csv";


        public int nSpliteIndex = 2;


        /// <summary>
        /// A面释放队列
        /// </summary>
        public QueuePackage<PFTCleanEAProductInfo> queueProductsDispose_A = new QueuePackage<PFTCleanEAProductInfo>();
        /// <summary>
        /// B面释放队列
        /// </summary>
        public QueuePackage<PFTCleanEAProductInfo> queueProductsDispose_B = new QueuePackage<PFTCleanEAProductInfo>();


        public QueuePackage<PFTCleanEAProductInfo> queueProductProtect_A = new QueuePackage<PFTCleanEAProductInfo>();

        public QueuePackage<PFTCleanEAProductInfo> queueProductProtect_B = new QueuePackage<PFTCleanEAProductInfo>();

        /// <summary>
        /// 这是用来做数据处理延迟一帧使用
        /// </summary>
        public QueuePackage<PFTImageProduct>   queueImgProductsDisposeAB = new QueuePackage<PFTImageProduct>();

        public void Dispose()
        {


        }

    }

    /// <summary>
    /// 条数据统计
    /// </summary>
    [Serializable]
    public class StatisticalData_Slice
    {

        public StatisticalData statisticalData { get; set; } = new StatisticalData();

        public List<SizeItemInfo> sizeItemInfos { get; set; } = new List<SizeItemInfo>();

        public List<DefectItemInfo> defectItemInfos { get; set; } = new List<DefectItemInfo>();

        public List<SizeItemInfo> sizeTJDBBJItemInfos { get; set; } = new List<SizeItemInfo>();

        public List<DefectItemInfo> defectTJDBBJItemInfos { get; set; } = new List<DefectItemInfo>();


    }

}
