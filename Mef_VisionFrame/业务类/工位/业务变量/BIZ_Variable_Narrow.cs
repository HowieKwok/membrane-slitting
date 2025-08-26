using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Frame_Core;

namespace Mef_VisionFrame
{
    public class BIZ_Variable_Narrow :IDisposable
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
        /// A面释放队列
        /// </summary>
        public QueuePackage<PFTCleanEAProductInfo> queueProductsDispose_A = new QueuePackage<PFTCleanEAProductInfo>();
        /// <summary>
        /// B面释放队列
        /// </summary>
        public QueuePackage<PFTCleanEAProductInfo> queueProductsDispose_B = new QueuePackage<PFTCleanEAProductInfo>();

        /// <summary>
        /// A面图片释放队列
        /// </summary>
        public QueuePackage<PFTImageProduct> queueImgDispose_A = new QueuePackage<PFTImageProduct>();
        /// <summary>
        /// B面图片释放队列
        /// </summary>
        public QueuePackage<PFTImageProduct> queueImgDispose_B = new QueuePackage<PFTImageProduct>();

        /// <summary>
        /// 尺寸列表，用于承接尺寸工艺使用,用于停机报警判断
        /// </summary>
        public List<SizeItemInfo> ListEASizes { get; set; }
        /// <summary>
        /// 缺项列表，用于承接缺项工艺使用,用于停机报警判断
        /// </summary>
        public List<DefectItemInfo> ListDefectInfos { get; set; }


        public List<string> SizeProcessTitles { get; set; }

        public List<string> DeftProcessTitles { get; set; }

        public List<string> Size3DProcessTitles { get; set; }

        public List<string> EATitles_A { get; set; }

        public List<string> EATitles_B { get; set; }

        public List<string> EATitles_AB { get; set; }

        /// <summary>
        /// 相机A尺寸统计信息
        /// </summary>
        public List<SizeItemInfo> sizeItemInfos_A { get; set; }
        /// <summary>
        /// 相机B尺寸统计信息
        /// </summary>
        public List<SizeItemInfo> sizeItemInfos_B { get; set; }
        /// <summary>
        /// 相机C尺寸统计信息
        /// </summary>
        public List<SizeItemInfo> sizeItemInfos_C { get; set; }
        /// <summary>
        /// 相机D尺寸统计信息
        /// </summary>
        public List<SizeItemInfo> sizeItemInfos_D { get; set; }

        /// <summary>
        /// 相机A缺项统计信息
        /// </summary>
        public List<DefectItemInfo> defectItemInfos_A { get; set; }
        /// <summary>
        /// 相机B缺项统计信息
        /// </summary>
        public List<DefectItemInfo> defectItemInfos_B { get; set; }

        public void Dispose()
        {
           
          
        }
    }


   


}
