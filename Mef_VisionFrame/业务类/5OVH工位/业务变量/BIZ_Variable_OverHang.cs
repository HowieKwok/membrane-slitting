using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class BIZ_Variable_OverHang
    {
        /// <summary>
        /// 电池产品(多线程访问)
        /// </summary>
        public OVHBatteryProduct batteryProduct = new OVHBatteryProduct();
    
      
        /// <summary>
        /// 阳极
        /// </summary>
        public OVHPackgeNumLayerSim OVHPackgeNumLayerSim_A = new OVHPackgeNumLayerSim();

        /// <summary>
        /// 阴极
        /// </summary>
        public OVHPackgeNumLayerSim OVHPackgeNumLayerSim_C = new OVHPackgeNumLayerSim();

        /// <summary>
        /// 曲线图尺寸检测数据
        /// </summary>
        public Dictionary<string, SizeDetectionItemData> ChartSizeDetectionItemData = new Dictionary<string, SizeDetectionItemData>();

        /// <summary>
        /// 曲线图尺寸检测数据
        /// </summary>
        public Dictionary<string, SizeDetectionItemData> ChartSizeDetectionItemData_AA = new Dictionary<string, SizeDetectionItemData>();

        /// <summary>
        /// 曲线图尺寸检测数据
        /// </summary>
        public Dictionary<string, SizeDetectionItemData> ChartSizeDetectionItemData_CC = new Dictionary<string, SizeDetectionItemData>();

        /// <summary>
        /// 下电芯隔膜结果
        /// </summary>
        public bool DownDiaphragmResult = true;
        /// <summary>
        /// 下电芯极片结果
        /// </summary>
        public bool DownPolePieceResult = true;
        /// <summary>
        /// 上电芯隔膜结果
        /// </summary>
        public bool UpDiaphragmResult = true;
        /// <summary>
        /// 上电芯极片结果
        /// </summary>
        public bool UpPolePieceResult = true;

        /// <summary>
        /// 总表
        /// </summary>
        public static List<string> Title_TotalTable = new List<string>() 
        {
         "时间",
         "帧号",
         "帧号2",
         "电芯编号",
         "电芯",
         "极片层数",
         "LG_4X",
         "LG_5X",
         "LG_5Y",
         "LG_3Y",
         "LG_3X",
         "LG_2X",
         "LG_2Y",
         "LG_1X",
         "LG_1Y",
         "LG_4Y",
         "ARC",
         "COR2",
         "COR3",
         "AS1",
         "AS2",
         "AS3",
         "正面Tabneck",
         "反面Tabneck",
         "W1",
         "W2",
         "H1",
         "H2",
         "EarW",
         "EarH",
         "EarD",
         "Cam1MarkW",
         "Cam1MarkH",
         "Cam2MarkW",
         "Cam2MarkH",
         "Cam3MarkW",
         "Cam3MarkH",
         "Cam4MarkW",
         "Cam4MarkH",
         "AACC_4Y",
         "AACC_4X",
         "AACC_1Y",
         "AACC_1X",
         "AACC_2Y",
         "AACC_2X",
         "AACC_3X",
         "AACC_3Y",
         "AACC_5Y",
         "AACC_5X",
         "结果",
         "电芯结果",
         "二维码",       
        };

        /// <summary>
        /// NG数据
        /// </summary>
        public static List<string> Title_NGData = new List<string>()
        {
          "时间",
          "电芯编号",
          "电芯",
          "极片层数",
          "结果",
          "叠台A相机1NG截图相对路径",
          "叠台A相机2NG截图相对路径",
          "叠台A相机3NG截图相对路径",
          "叠台A相机4NG截图相对路径",
          "叠台B相机1NG截图相对路径",
          "叠台B相机2NG截图相对路径",
          "叠台B相机3NG截图相对路径",
          "叠台B相机4NG截图相对路径",
          "叠台A相机1NG原图相对路径",
          "叠台A相机2NG原图相对路径",
          "叠台A相机3NG原图相对路径",
          "叠台A相机4NG原图相对路径",
          "叠台B相机1NG原图相对路径",
          "叠台B相机2NG原图相对路径",
          "叠台B相机3NG原图相对路径",
          "叠台B相机4NG原图相对路径",
          "NG数据截图相对路径",

        };

        public static List<string> Title_AC = new List<string>();
        public static List<string> Title_CA = new List<string>();
        public static List<string> Title_AA = new List<string>();
        public static List<string> Title_CC = new List<string>();

        public int PhotoSignal_A = 0;

        public int PhotoSignal_B = 0;

        /// <summary>
        /// 电芯编号
        /// </summary>
        public long CellNumber = 0;

        /// <summary>
        /// 当前电芯开始标志
        /// </summary>
        public bool IsStrat = false;

        /// <summary>
        /// 当前层数
        /// </summary>
        public int CurrentLayerA = 0;

        /// <summary>
        /// 下或上电芯
        /// </summary>
        public int UpOrDownCellA = 0;

        /// <summary>
        /// 当前EA结果
        /// </summary>
        public bool CurrentEAResultA = false;
        /// <summary>
        /// 当前层数
        /// </summary>
        public int CurrentLayerB = 0;

        /// <summary>
        /// 下或上电芯
        /// </summary>
        public int UpOrDownCellB = 0;

        /// <summary>
        /// 当前EA结果
        /// </summary>
        public bool CurrentEAResultB = false;
    }

   
    public enum OVHDetectionMethodeEnum
    {
        AC,//阳到阴
        CA,//阴到阳
        AA,//阳到阳
        CC,//阴到阴
    }
}
