using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class BIZ_Variable_Pretargeting
    {
        /// <summary>
        /// 曲线图尺寸检测数据
        /// </summary>
        public  Dictionary<string, SizeDetectionItemData> ChartSizeDetectionItemData = new Dictionary<string, SizeDetectionItemData>();

        public List<string> sizeDataTitle = new List<string>();

        /// <summary>
        /// 预定位电池产品
        /// </summary>
        public PretargetingBatteryProduct pretargetingBatteryProducts = new PretargetingBatteryProduct();

        /// <summary>
        /// 上或下电芯
        /// </summary>
        public int UpOrDownCell = 0;

        /// <summary>
        /// 拍照信号
        /// </summary>
        public int PhotoSignal = 0;

        /// <summary>
        /// 基准点X偏差范围 ±
        /// </summary>
        public double XBiasMin { get; set; } = -10;
        /// <summary>
        /// 基准点Y偏差范围 ±
        /// </summary>
        public double YBiasMin { get; set; } = -10;
        /// <summary>
        /// 基准点角度偏差范围 ±
        /// </summary>
        public double AngleBiasMin { get; set; } = -10;

        /// <summary>
        /// 基准点X偏差范围 
        /// </summary>
        public double XBiasMax { get; set; } = 10;
        /// <summary>
        /// 基准点Y偏差范围 ±
        /// </summary>
        public double YBiasMax { get; set; } = 10;
        /// <summary>
        /// 基准点角度偏差范围 ±
        /// </summary>
        public double AngleBiasMax { get; set; } = 10;

    }
}
