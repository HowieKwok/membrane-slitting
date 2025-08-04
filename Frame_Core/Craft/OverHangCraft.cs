using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class OverHangCraft : CraftBase
    {
        public OverHangCraft()
        {
            // Init();
        }

        /// <summary>
        /// AA标准下限公差
        /// </summary>
        [Description("AA标准下限公差")]
        public double AAStandardDown { get; set; } = -0.1;

        /// <summary>
        /// AA标准上限公差
        /// </summary>
        [Description("AA标准上限公差")]
        public double AAStandardUp { get; set; } = 0.1;

        /// <summary>
        /// 启用AACC打标
        /// </summary>
        [Description("启用AACC打标")]
        public bool EnAACCDB { get; set; } = false;


        /// <summary>
        /// 启用叠台测量极片尺寸
        /// </summary>
        [Description("启用叠台测量极片尺寸")]
        public bool EnOVHSize { get; set; } = false;

        /// <summary>
        /// 二次拍照基准点X偏差范围 ±
        /// </summary>
        /// 
        [Description(" 二次拍照基准点X偏差范围下限")]
        public double XBiasMin { get; set; } = -10;
        /// <summary>
        /// 二次拍照基准点Y偏差范围 ±
        /// </summary>
        [Description(" 二次拍照基准点Y偏差范围下限")]
        public double YBiasMin { get; set; } = -10;
        /// <summary>
        /// 二次拍照基准点角度偏差范围 ±
        /// </summary>
        [Description(" 二次拍照基准点R偏差范围下限")]
        public double AngleBiasMin { get; set; } = -10;

        /// <summary>
        /// 二次拍照基准点X偏差范围 
        /// </summary>

        [Description(" 二次拍照基准点X偏差范围上限")]
        public double XBiasMax { get; set; } = 10;
        /// <summary>
        /// 二次拍照基准点Y偏差范围 ±
        /// </summary>
        [Description(" 二次拍照基准点Y偏差范围上限")]
        public double YBiasMax { get; set; } = 10;
        /// <summary>
        /// 二次拍照基准点角度偏差范围 ±
        /// </summary>
        [Description(" 二次拍照基准点R偏差范围上限")]
        public double AngleBiasMax { get; set; } = 10;

        /// <summary>
        /// 最后一层明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
        /// </summary>    
        public string LastFloorMeasureTransition = "positive";

        // 产品尺寸工艺  --按相机布局 
        /// <summary>
        /// 电芯总层数
        /// </summary>
        [Description("电芯总层数")]
        public int StakeCathodeCount { get; set; } = 37;
        ///// <summary>
        ///// 预定位基准点X -映射坐标系
        ///// </summary>
        //public double BasePointX { get; set; } = 100;
        ///// <summary>
        ///// 预定位基准点Y -映射坐标系
        ///// </summary>
        //public double BasePointY { get; set; } = 100;
        /// <summary>
        /// 实测点和基准点X偏差范围 ±
        /// </summary>
        //public double XBiasMin { get; set; } = 1;
        ///// <summary>
        ///// 实测点和基准点Y偏差范围 ±
        ///// </summary>
        //public double YBiasMin { get; set; } = 1;
        ///// <summary>
        ///// 实测点和基准点角度偏差范围 ±
        ///// </summary>
        //public double AngleBiasMin { get; set; } = 3;

        ///// <summary>
        ///// 实测点和基准点X偏差范围 
        ///// </summary>
        //public double XBiasMax { get; set; } = 5;
        ///// <summary>
        ///// 实测点和基准点Y偏差范围 ±
        ///// </summary>
        //public double YBiasMax { get; set; } = 6;
        ///// <summary>
        ///// 实测点和基准点角度偏差范围 ±
        ///// </summary>
        //public double AngleBiasMax { get; set; } = 7;


        /// <summary>
        /// 抓边队列(所有相机的抓边集合)
        /// </summary>
        [Description("抓边队列")]
        public List<List<LineInfo>> ListLines { get; set; }

        /// <summary>
        /// Size工艺编辑队列(基于抓边匹配)
        /// </summary>
        [Description("Size工艺编辑队列")]
        public List<List<SizeItemInfo>> ListSizeEdits { get; set; }

        /// <summary>
        /// 阴阳极极片Size工艺编辑队列
        /// </summary>
        [Description("阴阳极极片Size工艺编辑队列")]
        public List<SizeItemInfo> ListEASizes { get; set; }

        public override void Init()
        {
            CraftName = "叠台工艺测试";
            BaseCamera = CameraEnum.Cam1;
            electrode = ElectrodeEnum.阴极;
            materialCrafEnum = MaterialCrafEnum.叠片对齐度OVH;

            //初始化抓边
            if (ListLines == null)
            {
                ListLines = new List<List<LineInfo>>();
            }
            else { ListLines.Clear(); }

            //初始化检测项
            if (ListSizeEdits == null)
            {
                ListSizeEdits = new List<List<SizeItemInfo>>();
            }
            else { ListSizeEdits.Clear(); }

            for (int i = 0; i < CamNum; i++)
            {
                ListLines.Add(new List<LineInfo>());
                ListSizeEdits.Add(new List<SizeItemInfo>());
            }
            //初始化检测项
            if (ListEASizes == null)
            {
                ListEASizes = new List<SizeItemInfo>();
            }
            
        }
    }


}
