using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 只存放数据，涉及内存类型不要放在这个类
    /// </summary>
    public class ProductDataResult
    {
        public bool IsNG = true;

        /// <summary>
        /// 是否重度缺陷
        /// </summary>
        public bool IsSevereDefect = true;

        public long m_nProductID = 0;
        public long m_nImageFrameID = 0;


        /// <summary>
        /// 缺陷最大等级信息
        /// </summary>
        public DefectSummary highestPriorityNgDefect { get; set; }

        /// <summary>
        /// 尺寸NG最大等级信息
        /// </summary>
        public SizeInfo highestPriorityNgSize { get; set; }

        /// <summary>
        /// 尺寸NG最大等级信息（工位）
        /// </summary>
        public SizeInfo highestPriorityNgSize_Station { get; set; }


        /// <summary>
        /// 所有缺陷信息
        /// </summary>
        public List<DefectSummary> defectSummaries { get; set; } = new List<DefectSummary>();

        /// <summary>
        /// 所有尺寸信息
        /// </summary>
        public List<SizeInfo> sizeInfos { get; set; } = new List<SizeInfo>();

        /// <summary>
        /// 所有尺寸信息（工位）
        /// </summary>
        public List<SizeInfo> SizeInfos_Station { get; set; } = new List<SizeInfo>();


        /// <summary>
        /// 缺陷信息
        /// </summary>
        public List<List<RowData>> DefectData { get; set; } = new List<List<RowData>>();

        /// <summary>
        /// 浅拷贝克隆方法
        /// </summary>
        public ProductDataResult Clone()
        {
            return (ProductDataResult)this.MemberwiseClone();
        }
    }
}
