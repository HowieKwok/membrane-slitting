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
        /// 涂布尺寸MD_A面
        /// </summary>
        [Description("涂布尺寸MD_A面")]
        public double CoatSizeMD_A { get; set; } = 100;

        /// <summary>
        /// 涂布尺寸MD_B面
        /// </summary>
        [Description("涂布尺寸MD_B面")]
        public double CoatSizeMD_B { get; set; } = 100;

        /// <summary>
        /// EA料头是毛边料头
        /// </summary>
        [Description("EA料头是毛边料头")]
        public bool IsBurrHead { get; set; } = false;

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
        /// A面槽位个数
        /// </summary>
        [Description("A面槽位个数")]
        public int SlotNum_A { get; set; } = 1;

        /// <summary>
        /// B面槽位个数
        /// </summary>
        [Description("B面槽位个数")]
        public int SlotNum_B { get; set; } = 1;

        /// <summary>
        /// A面削薄区个数
        /// </summary>
        [Description("A面削薄区个数")]
        public int ShaveNum_A { get; set; } = 1;

        /// <summary>
        /// B面削薄区个数
        /// </summary>
        [Description("B面削薄区个数")]
        public int ShaveNum_B { get; set; } = 1;

        /// <summary>
        /// 槽位削薄区信息队列-AB面公用
        /// </summary>
        [Description("槽位削薄区信息队列")]
        public List<SlotInfo> ListSlotInfos { get; set; }





        #endregion




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

        public override void Init()
        {
            CraftName = "PFT工艺测试";
            BaseCamera = CameraEnum.Cam1;
            electrode = ElectrodeEnum.阴极;
            materialCrafEnum = MaterialCrafEnum.PFT清洗机;

            //初始化槽位信息
            if (ListSlotInfos == null)
            {
                ListSlotInfos = new List<SlotInfo>();
            }
            else { ListSlotInfos.Clear(); }


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
        }
    }
    /// <summary>
    /// 单个槽位标准工艺信息
    /// </summary>
    [Serializable]
    public class SlotInfo
    {

        //编号 槽位名 A/B面 MD定位距(上边)   TD尺寸 MD尺寸    深度(3D)  内屏蔽左TD 内屏蔽右TD  内屏蔽上MD 内屏蔽下MD  外屏蔽左TD 外屏蔽右TD  外屏蔽上MD 外屏蔽下MD
        /// <summary>
        /// 槽位编号
        /// </summary>
        [My("编号")]
        [Description("编号")]
        public int InfoID { get; set; } = 0;

        /// <summary>
        /// 槽位名
        /// </summary>
        [My("槽位名")]
        [Description("槽位名")]
        public string InfoName { get; set; } = "默认名称";

        /// <summary>
        /// A/B面
        /// </summary>
        [My("A/B面")]
        [Description("A/B面")]
        public bool IsASide { get; set; } = true;

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
        /// 深度(3D)
        /// </summary>
        [My("深度(3D)")]
        [Description("深度(3D)")]
        public double SlotDeepSize { get; set; } = 100;


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
        public double OutwardShieldLeft { get; set; } = 5;
        /// <summary>
        /// 外右屏蔽TD
        /// </summary>
        [My("外右屏蔽TD")]
        [Description("外右屏蔽TD")]
        public double OutwardShieldRight { get; set; } = 5;
        /// <summary>
        /// 外上屏蔽MD
        /// </summary>
        [My("外上屏蔽MD")]
        [Description("外上屏蔽MD")]
        public double OutwardShieldUp { get; set; } = 5;
        /// <summary>
        /// 外下屏蔽MD
        /// </summary>
        [My("外下屏蔽MD")]
        [Description("外下屏蔽MD")]
        public double OutwardShieldDown { get; set; } = 5;




    }
}
