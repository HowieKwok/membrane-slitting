using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Frame_Core
{
    //产品基类
    /// <summary>
    /// 产品参数基类
    /// </summary>
    public class ProductBase : IDisposable
    {
        public ShowHalconWindow showHalconWindow { get; set; }

        //抓边插件查找的边，用于计算。
        public List<LineInfo> lineInfos { get; set; }
        public List<NgTypeInfo> NgTypeInfos { get; set; }
        public List<SizeInfo> sizeInfos { get; set; }

 
        /// <summary>
        /// 尺寸监控信息
        /// </summary>
        public List<SizeInfo> sizeMonitorInfos { get; set; } = new List<SizeInfo>();

        public List<LineInfo> m_lineShowInfos { get; set; }

        /// <summary>
        /// 图像开始时间
        /// </summary>
        public HTuple Image_StartTime = 0;

        /// <summary>
        /// 图像找边时间
        /// </summary>
        public HTuple Image_FindLineTime = 0;

        /// <summary>
        /// 图像缺陷检测时间
        /// </summary>
        public HTuple Image_FindDefectTime = 0;

        /// <summary>
        /// 图像算法处理时间
        /// </summary>
        public HTuple Image_ArithmeticProcessTime = 0;
        /// <summary>
        /// 图像数据开始处理时间
        /// </summary>
        public HTuple Image_StartDataProcessTime = 0;

        /// <summary>
        /// 图像数据处理时间
        /// </summary>
        public HTuple Image_DataProcessTime = 0;
        /// <summary>
        /// 图像结束时间
        /// </summary>
        public HTuple Image_EndTime = 0;

        public ProductBase()
        {
            showHalconWindow = new ShowHalconWindow();
            lineInfos = new List<LineInfo>();
            NgTypeInfos = new List<NgTypeInfo>();
            sizeInfos = new List<SizeInfo>();
        }


        public virtual void Dispose()
        {
            showHalconWindow?.Dispose();
            lineInfos.Clear();
            foreach (var item in NgTypeInfos)
            {
                item?.Dispose();
            }
            NgTypeInfos.Clear();
            foreach (var item in sizeInfos)
            {
                item?.Dispose();
            }
            sizeInfos.Clear();
        }
    }
}
