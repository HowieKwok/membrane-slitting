using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class DieCutSliceProduct:ProductBase
    {
        public bool bIsNG = false;

        public long nCurFrameImgId { get; set; } = 0;
        public long nProducrtID { get; set; } = 0;
        public long nRealativeProductID = 0;
        public long nRealativeFrameImgID = 0;

        public double fRealativeJRY { get; set; } = 0;

        // <summary>
        /// 判断是否是阴极
        /// </summary>
        public bool bIsCathode = false;

        /// <summary>
        /// 单个EA尺寸检测，
        /// </summary>
        public List<SizeItemInfo> Listsize { get; set; } = new List<SizeItemInfo>();

        /// <summary>
        /// 单个EA尺寸检测，
        /// </summary>
        public List<DefectItemInfo> Listdefect { get; set; } = new List<DefectItemInfo>();


    }
}
