using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class PFTCleanCraft : CraftBase
    {
        #region 通用设置

        /// <summary>
        /// 极片尺寸TD
        /// </summary>
        [Description("极片尺寸TD")]
        public double EASizeTD { get; set; } = 80;

        /// <summary>
        /// 极片尺寸MD
        /// </summary>
        [Description("极片尺寸MD")]
        public double EASizeMD { get; set; } = 100;


        /// <summary>
        /// 极片尺寸裁图预留MD
        /// </summary>
        [Description("极片尺寸裁图预留MD")]
        public double EACropShiledMD { get; set; } = 100;


        /// <summary>
        /// 涂布尺寸TD
        /// </summary>
        [Description("涂布尺寸TD")]
        public double CoatSizeTD { get; set; } = 100;

        /// <summary>
        /// 涂布尺寸MD
        /// </summary>
        [Description("涂布尺寸MD")]
        public double FrontCoatSizeMD { get; set; } = 100;

        /// <summary>
        /// 涂布尺寸MD
        /// </summary>
        [Description("涂布尺寸MD")]
        public double BackCoatSizeMD { get; set; } = 100;

        /// <summary>
        /// 左留白尺寸TD
        /// </summary>
        [Description("左留白尺寸TD")]
        public double LeftFoilSizeTD { get; set; } = 100;

        /// <summary>
        /// 右留白尺寸TD
        /// </summary>
        [Description("右留白尺寸TD")]
        public double RightFoilSizeTD { get; set; } = 100;


        /// <summary>
        /// A面料头到B面料头距离
        /// </summary>
        [Description("A面料头到B面料头距离")]
        public double HeadMis_AtoB { get; set; } = 0;

        //涂布屏蔽区参数
        /// <summary>
        /// 涂布MD料头缩进
        /// </summary>
        [Description("涂布MD料头缩进")]
        public double CoatShieldMDHead { get; set; } = 0;
        /// <summary>
        /// 涂布MD料尾缩进
        /// </summary>
        [Description("涂布MD料尾缩进")]
        public double CoatShieldMDTile { get; set; } = 0;
        /// <summary>
        /// 涂布TD左缩进
        /// </summary>
        [Description("涂布TD左缩进")]
        public double CoatShieldTDLeft { get; set; } = 0;
        /// <summary>
        /// 涂布TD右缩进
        /// </summary>
        [Description("涂布TD右缩进")]
        public double CoatShieldTDRight { get; set; } = 0;

        /// <summary>
        /// 目标分条数
        /// </summary>
        [Description("目标分条数")]
        public int SliceCount { get; set; } = 8;

        /// <summary>
        /// 单条打孔区数
        /// </summary>
        [Description("单条打孔区数")]
        public int PerSlotCount { get; set; } = 12;

        /// <summary>
        /// 常规料(F)/CTP料(T)
        /// </summary>
        [Description("常规料(F)/CTP料(T)")]
        public bool MaterialType { get; set; } = false;

        /// <summary>
        /// 常规料(F)/CTP料(T)
        /// </summary>
        [Description("常规料(F)/CTP料(T)")]
        public bool CurCamIsLeft { get; set; } = false;


        /// <summary>
        /// 打孔区尺寸TD
        /// </summary>
        [Description("打孔区尺寸TD")]
        public double SlotSizeTD { get; set; } = 10;

        /// <summary>
        /// 打孔区尺寸MD
        /// </summary>
        [Description("打孔区尺寸MD")]
        public double SlotSizeMD { get; set; } = 5;


        /// <summary>
        /// 槽位TDis间隙,槽位右边到左边
        /// </summary>
        [Description("槽位TDis间隙")]
        public List<double> SlotGapTDis { get; set; }

        /// <summary>
        /// 槽位TDis间隙上公差
        /// </summary>
        [Description("槽位TDis间隙上公差")]
        public List<double> SlotGapTDisUp { get; set; }
        /// <summary>
        /// 槽位TDis间隙下公差
        /// </summary>
        [Description("槽位TDis下公差")]
        public List<double> SlotGapTDisDown { get; set; }

        /// <summary>
        /// 槽位MDis间距,槽位上边到上边
        /// </summary>
        [Description("槽位MDis")]
        public List<double> SlotMDis { get; set; }

        /// <summary>
        /// 槽位MDis间距,槽位上边到上边
        /// </summary>
        [Description("槽位MDis")]
        public List<double> SlotMDisBSide { get; set; }

        /// <summary>
        ///槽位MDis上公差
        /// </summary>
        [Description("槽位MDis上公差")]
        public List<double> SlotMDisUp { get; set; }
        /// <summary>
        /// 槽位MDis下公差
        /// </summary>
        [Description("槽位MDis下公差")]
        public List<double> SlotMDisDown { get; set; }

        /// <summary>
        /// Cam1左侧基准边列坐标
        /// </summary>
        [Description("Cam1左侧基准边列坐标")]
        public double Cam1BaseCol { get; set; } = 1000;
        /// <summary>
        /// Cam2左侧基准边列坐标
        /// </summary>
        [Description("Cam2左侧基准边列坐标")]
        public double Cam2BaseCol { get; set; } = 1000;

        /// <summary>
        /// 特征在Cam1中的列坐标
        /// </summary>
        [Description("特征在Cam1中的列坐标")]
        public double EXCam1Col { get; set; } = 14000;

        /// <summary>
        /// 特征在Cam2中的列坐标
        /// </summary>
        [Description("特征在Cam2中的列坐标")]
        public double EXCam2Col { get; set; } = 1000;


        #region 边距间隙公差
        ///// <summary>
        ///// 第一条距边标准值
        ///// </summary>
        //[Description("第一条距边标准值")]
        //public double TDFirstGap { get; set; } = 2;

        ///// <summary>
        ///// 第一条距边公差
        ///// </summary>
        //[Description("第一条距边公差")]
        //public double TDFirstGapTol { get; set; } = 0.5;

        ///// <summary>
        ///// 中间条间隙标准值
        ///// </summary>
        //[Description("中间条间隙标准值")]
        //public double TDMidGap { get; set; } = 4;

        ///// <summary>
        ///// 中间条间隙公差
        ///// </summary>
        //[Description("中间条间隙公差")]
        //public double TDMidGapTol { get; set; } = 1;

        ///// <summary>
        ///// 最后条距边标准值
        ///// </summary>
        //[Description("最后条距边标准值")]
        //public double TDLastGap { get; set; } = 2;
        ///// <summary>
        ///// 最后条距边公差
        ///// </summary>
        //[Description("最后条距边公差")]
        //public double TDLastGapTol { get; set; } = 0.5;
        #endregion


        #endregion

        /// <summary>
        /// 槽位削薄区信息队列-AB面公用
        /// </summary>
        [Description("槽位削薄区信息队列")]
        public List<SlotInfo> ListSlotInfos { get; set; }

        /// <summary>
        /// 抓边队列(所有相机的抓边集合)
        /// </summary>

        [Description("抓边队列")]
        public List<List<LineInfo>> ListLines { get; set; }

        /// <summary>
        /// 极片Size工艺队列-可编辑
        /// </summary>
        [Description("极片Size工艺队列-可编辑")]
        public List<SizeItemInfo> ListEASizes { get; set; }

        /// <summary>
        /// 极片Defect缺陷检测项
        /// </summary>
        [Description("极片Defect缺陷检测项")]
        public List<DefectItemInfo> ListDefectInfos { get; set; }

        /// <summary>
        /// 极片槽位公差值检测项
        /// </summary>
        [Description("极片槽位公差值检测项")]
        public List<SlotSizeLimit> ListSlotSizeLimits { get; set; }



        /// <summary>
        /// 极片分条工艺设置
        /// </summary>
        [Description("极片分条工艺设置")]
        public List<SliceInfo> ListSliceSet { get; set; }
        public override void Init()
        {
            CraftName = "PFT工艺测试";
            BaseCamera = CameraEnum.Cam1;
            electrode = ElectrodeEnum.阴极;
            materialCrafEnum = MaterialCrafEnum.PFT清洗机;

            //初始化TD间隙尺寸
            if (SlotGapTDis == null)
            {
                SlotGapTDis = new List<double>();
            }
            else { SlotGapTDis.Clear(); }

            if (SlotGapTDisUp == null)
            {
                SlotGapTDisUp = new List<double>();
            }
            else { SlotGapTDisUp.Clear(); }
            if (SlotGapTDisDown == null)
            {
                SlotGapTDisDown = new List<double>();
            }
            else { SlotGapTDisDown.Clear(); }
            for (int i = 0; i < SliceCount + 1; i++)
            {
                SlotGapTDis.Add(50);
                SlotGapTDisUp.Add(2);
                SlotGapTDisDown.Add(2);
            }
            //初始化MD尺寸
            if (SlotMDis == null)
            {
                SlotMDis = new List<double>();
            }
            else { SlotMDis.Clear(); }
            if (SlotMDisBSide == null)
            {
                SlotMDisBSide = new List<double>();
            }
            else { SlotMDisBSide.Clear(); }
            if (SlotMDisUp == null)
            {
                SlotMDisUp = new List<double>();
            }
            else { SlotMDisUp.Clear(); }
            if (SlotMDisDown == null)
            {
                SlotMDisDown = new List<double>();
            }
            else { SlotMDisDown.Clear(); }
            for (int i = 0; i < PerSlotCount; i++)
            {
                SlotMDis.Add(50);
                SlotMDisBSide.Add(50);
                SlotGapTDisUp.Add(2);
                SlotGapTDisDown.Add(2);
            }

            //初始化抓边
            if (ListLines == null)
            {
                ListLines = new List<List<LineInfo>>();
            }
            else { ListLines.Clear(); }
            for (int i = 0; i < CamNum; i++)
            {
                ListLines.Add(new List<LineInfo>());
            }

            //初始化检测项
            if (ListEASizes == null)
            {
                ListEASizes = new List<SizeItemInfo>();
            }
            else { ListEASizes.Clear(); }

            //初始化缺陷检测项
            if (ListDefectInfos == null)
            {
                ListDefectInfos = new List<DefectItemInfo>();
            }
            else { ListDefectInfos.Clear(); }

            //初始化槽位信息
            if (ListSlotInfos == null)
            {
                ListSlotInfos = new List<SlotInfo>();
            }
            else { ListSlotInfos.Clear(); }

            //添加槽位上下限表格
            if (ListSlotSizeLimits == null)
            {
                ListSlotSizeLimits = new List<SlotSizeLimit>();
            }
            else { ListSlotSizeLimits.Clear(); }

            //极片分条工艺设置
            if (ListSliceSet == null)
            {
                ListSliceSet = new List<SliceInfo>();
            }
            else { ListSliceSet.Clear(); }

        }
    }
    /// <summary>
    /// 单个槽位标准工艺信息
    /// </summary>
    [Serializable]
    public class SlotInfo
    {
        //编号 槽位名 A/B面  第几条  MD定位距(上边)   TD尺寸 MD尺寸    深度(3D)  内屏蔽左TD 内屏蔽右TD  内屏蔽上MD 内屏蔽下MD  外屏蔽左TD 外屏蔽右TD  外屏蔽上MD 外屏蔽下MD
        /// <summary>
        /// 槽位编号
        /// </summary>
        [My("编号")]
        [Description("编号")]
        public int InfoID { get; set; } = 0;

        /// <summary>
        /// 槽位名
        /// </summary>
        [My("名称")]
        [Description("名称")]
        public string Name { get; set; } = "默认名称";

        [My("备注")]
        [Description("备注")]
        public string Description { get; set; } = "备注";

        /// <summary>
        /// A/B面
        /// </summary>
        [My("A/B面")]
        [Description("A/B面")]
        public bool IsASide { get; set; } = true;

        /// <summary>
        /// A/B面
        /// </summary>
        [My("所属相机")]
        [Description("所属相机")]
        public CameraEnum BelongCam { get; set; } = 0;

        /// <summary>
        /// 第几条
        /// </summary>
        [My("第几条")]
        [Description("第几条")]
        public int SliceID { get; set; } = 0;

        /// <summary>
        /// 第几条
        /// </summary>
        [My("第几个打孔区")]
        [Description("第几个打孔区")]
        public int ShaveID { get; set; } = 0;

        /// <summary>
        /// TD定位距(左边)
        /// </summary>
        [My("TD定位距(左边)")]
        [Description("TD定位距(左边)")]
        public double SlotTDPosition { get; set; } = 0;

        /// <summary>
        /// MD定位距(上边)
        /// </summary>
        [My("MD定位距(上边)")]
        [Description("MD定位距(上边)")]
        public double SlotMDPosition { get; set; } = 100;

        /// <summary>
        /// TD尺寸
        /// </summary>
        [My("TD尺寸")]
        [Description("TD尺寸")]
        public double SlotTDSize { get; set; } = 100;

        /// <summary>
        /// MD尺寸
        /// </summary>
        [My("MD尺寸")]
        [Description("MD尺寸")]
        public double SlotMDSize { get; set; } = 100;

        /// <summary>
        /// TD间距
        /// </summary>
        [My("TD间距")]
        [Description("TD间距")]
        public double SlotTDDist { get; set; } = 100;

        /// <summary>
        /// MD间距
        /// </summary>
        [My("MD间距")]
        [Description("MD间距")]
        public double SlotMDDist { get; set; } = 100;


        /// <summary>
        /// TD左间隙
        /// </summary>
        [My("TD左间隙")]
        [Description("TD左间隙")]
        public double SlotTDLeftGap { get; set; } = 4;

        /// <summary>
        /// TD右间隙
        /// </summary>
        [My("TD右间隙")]
        [Description("TD右间隙")]
        public double SlotTDRightGap { get; set; } = 4;

        /// <summary>
        /// TD对齐
        /// </summary>
        [My("TD对齐")]
        [Description("TD对齐")]
        public double SlotTDAlign { get; set; } = 0;

        /// <summary>
        /// MD对齐
        /// </summary>
        [My("MD对齐")]
        [Description("MD对齐")]
        public double SlotMDAlign { get; set; } = 0;

        /// <summary>
        /// TD错位
        /// </summary>
        [My("TD错位")]
        [Description("TD错位")]
        public double SlotTDMiss { get; set; } = 0;

        /// <summary>
        /// MD错位
        /// </summary>
        [My("MD错位")]
        [Description("MD错位")]
        public double SlotMDMiss { get; set; } = 0;



        /// <summary>
        /// // 数组属性：通过索引器一次性访问所有值
        /// </summary>
        [My("校准系数", false)]
        [Description("校准系数")]
        public double[] RealFixRito = new double[12] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        /// <summary>
        /// // 数组属性：通过索引器一次性访问所有值
        /// </summary>
        [My("测量校准偏移", false)]
        [Description("测量校准偏移")]
        public double[] RealFixOffset = new double[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


        /// <summary>
        /// // 数组属性：通过索引器一次性访问所有值
        /// </summary>
        [My("数组属性", false)]
        [Description("数组属性")]
        public double[] AllValues
        {
            get => new double[] { SlotTDPosition, SlotMDPosition, SlotTDSize, SlotMDSize, SlotTDDist, SlotMDDist, SlotTDLeftGap, SlotTDRightGap, SlotTDAlign, SlotMDAlign, SlotTDMiss, SlotMDMiss }; // 返回新数组
            set { }
        }

        ///// <summary>
        ///// 深度(3D)
        ///// </summary>
        //[My("深度(3D)")]
        //[Description("深度(3D)")]
        //public double SlotDeepSize { get; set; } = 100;


        /// <summary>
        /// 内左屏蔽TD
        /// </summary>
        [My("内左屏蔽TD")]
        [Description("内左屏蔽TD")]
        public double InwardShieldLeft { get; set; } = 1;
        /// <summary>
        /// 内右屏蔽TD
        /// </summary>
        [My("内右屏蔽TD")]
        [Description("内右屏蔽TD")]
        public double InwardShieldRight { get; set; } = 1;
        /// <summary>
        /// 内上屏蔽MD
        /// </summary>
        [My("内上屏蔽MD")]
        [Description("内上屏蔽MD")]
        public double InwardShieldUp { get; set; } = 1;
        /// <summary>
        /// 内下屏蔽TD
        /// </summary>
        [My("内下屏蔽TD")]
        [Description("内下屏蔽MD")]
        public double InwardShieldDown { get; set; } = 1;


        /// <summary>
        /// 外左屏蔽TD
        /// </summary>
        [My("外左屏蔽TD")]
        [Description("外左屏蔽TD")]
        public double OutwardShieldLeft { get; set; } = 1;
        /// <summary>
        /// 外右屏蔽TD
        /// </summary>
        [My("外右屏蔽TD")]
        [Description("外右屏蔽TD")]
        public double OutwardShieldRight { get; set; } = 1;
        /// <summary>
        /// 外上屏蔽MD
        /// </summary>
        [My("外上屏蔽MD")]
        [Description("外上屏蔽MD")]
        public double OutwardShieldUp { get; set; } = 1;
        /// <summary>
        /// 外下屏蔽MD
        /// </summary>
        [My("外下屏蔽MD")]
        [Description("外下屏蔽MD")]
        public double OutwardShieldDown { get; set; } = 1;


    }

    [Serializable]
    public class SlotSizeLimit
    {
        [My("编号")]
        [Description("编号")]
        public int Index { get; set; } = 0;//名称
        [My("启用")]
        [Description("启用")]
        public bool Enable { get; set; } = true;//打标
        [My("名称")]
        [Description("名称")]
        public string Name { get; set; } = "名称";//名称   

        [My("备注")]
        [Description("备注")]
        public string Description { get; set; } = "备注";

        /// <summary>
        /// 负公差
        /// </summary>
        [My("负公差")]
        [Description("负公差")]
        public double lowLimitValue { get; set; } = -0.5;
        /// <summary>
        /// 正公差
        /// </summary>
        [My("正公差")]
        [Description("正公差")]
        public double UpLimitValue { get; set; } = 0.5;
        /// <summary>
        /// 启用打标
        /// </summary>
        [My("启用打标")]
        [Description("启用打标")]
        public bool IsDB { get; set; } = true;

        /// <summary>
        /// 启用报警
        /// </summary>
        [My("启用报警")]
        [Description("启用报警")]
        public bool IsBJ { get; set; } = true;

        /// <summary>
        /// 累计NGEA报警
        /// </summary>
        [My("累计NGEA报警")]
        [Description("累计NGEA报警")]
        public int NGEACount { get; set; } = 5;


        [My("按条打标", false)]
        [Description("按条打标")]
        public double[] ArrayDBSlice { get; set; } = new double[8];//按条打标
        [My("按条累计打标", false)]
        [Description("按条累计打标")]
        public double[] ArrayDBSliceCount { get; set; } = new double[8];//按条累计打标

        [My("按条报警", false)]
        [Description("按条报警")]
        public double[] ArrayBJSlice { get; set; } = new double[8];//按条报警
        [My("按条累计报警", false)]
        [Description("按条累计报警")]
        public double[] ArrayBJSliceCount { get; set; } = new double[8];//按条累计报警

    }

    /// <summary>
    /// 单个槽位标准工艺信息
    /// </summary>
    [Serializable]
    public class SliceInfo
    {
        /// <summary>
        /// 槽位编号
        /// </summary>
        [My("编号")]
        [Description("编号")]
        public int SliceID { get; set; } = 0;

        /// <summary>
        /// 分条名称
        /// </summary>
        [My("分条名称")]
        [Description("分条名称")]
        public string InfoName { get; set; } = "默认名称";

        /// <summary>
        /// 左/右分区
        /// </summary>
        [My("左/右分区")]
        [Description("左/右分区")]
        public bool IsLeftSide { get; set; } = true;

        /// <summary>
        /// A/B面
        /// </summary>
        [My("所属相机")]
        [Description("所属相机")]
        public string BelongCam { get; set; } = ((CameraEnum)0).ToString();

        /// <summary>
        /// 分条起始TD
        /// </summary>
        [My("分条起始TD")]
        [Description("分条起始TD")]
        public double SliceStartPos { get; set; } = 0;
        /// <summary>
        /// 分条结束TD
        /// </summary>
        [My("分条结束TD")]
        [Description("分条结束TD")]
        public double SliceEndPos { get; set; } = 0;


        /// <summary>
        /// 打孔区数量
        /// </summary>
        [My("打孔区数量")]
        [Description("打孔区数量")]
        public int ShaveNum { get; set; } = 0;

        /// <summary>
        /// 打孔TD定位距
        /// </summary>
        [My("打孔TD定位距")]
        [Description("打孔TD定位距")]
        public double ShaveDisTD { get; set; } = 0;

        /// <summary>
        /// 打孔MD定位距(起始)
        /// </summary>
        [My("打孔MD定位距(起始)", false)]
        [Description("打孔MD定位距(起始)")]
        public double ShaveDisMDStart { get; set; } = 0;

        /// <summary>
        /// 打孔MD定位距(步长)
        /// </summary>
        [My("打孔MD定位距(步长)", false)]
        [Description("打孔MD定位距(步长)")]
        public double ShaveDisMDStep { get; set; } = 0;

        /// <summary>
        /// 打孔区TD尺寸
        /// </summary>
        [My("打孔区TD尺寸")]
        [Description("打孔区TD尺寸")]
        public double ShaveSizeTD { get; set; } = 0;
        /// <summary>
        /// 打孔区MD尺寸
        /// </summary>
        [My("打孔区MD尺寸")]
        [Description("打孔区MD尺寸")]
        public double ShaveSizeMD { get; set; } = 0;

    }

}
