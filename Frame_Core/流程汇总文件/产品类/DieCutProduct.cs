using DefectRuleClassifier.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core.流程汇总文件.产品类
{





    /// <summary>
    /// 模切EA产品
    /// </summary>
    public class DieCutEAProduct : ProductBase
    {

        public List<LineInfo> LineInfosResult { get; set; } = new List<LineInfo>();

        public ObservableCollection<NgDefectResult> ngDefectResults = new ObservableCollection<NgDefectResult>();

     

        public override void Dispose()
        {
            foreach(var item in ngDefectResults)
            {
                item?.Dispose();
            }
            ngDefectResults?.Clear();
            LineInfosResult.Clear();
            base.Dispose();
        }
    }
    /// <summary>
    /// 模切帧图产品
    /// </summary>
    public class DieCutImageProduct : ProductBase
    {
        public List<LineInfo> LineInfosResult { get; set; } = new List<LineInfo>();
        public long nCurImageID = 0;
        public bool blSplitEA = false;
        public override void Dispose()
        {
            LineInfosResult.Clear();
            base.Dispose();
        }
    }

    public class SizeDectectProduct:ProductBase
    {
        public string CamName = "";
        public string SN = "";
        /// <summary>
        /// 交点工具结果输出
        /// </summary>
        public List<OutCrossGroup> CrossPointInfos { get; set; } = new List<OutCrossGroup>();
        /// <summary>
        /// 点结果输出
        /// </summary>
        public List<PointInfo> PointResults { get; set; } = new List<PointInfo>();
        /// <summary>
        /// 缺陷结果输出
        /// </summary>
        public ObservableCollection<NgDefectResult> ngDefectResults { get; set; } = new ObservableCollection<NgDefectResult>();
       /// <summary>
       /// 线结果输出
       /// </summary>
        public List<LineInfo> LineInfosResult { get; set; } = new List<LineInfo>();

   
     
        public override void Dispose()
        {
            foreach (var item in ngDefectResults)
            {
                item?.Dispose();
            }
            LineInfosResult.Clear();
            ngDefectResults?.Clear();
            CrossPointInfos.Clear();
            PointResults.Clear();
            base.Dispose();
        }
    
    
    }


    public class PosCorrectProduct : ProductBase
    {
        /// <summary>
        /// 交点工具结果输出
        /// </summary>
        public List<OutCrossGroup> CrossPointInfos { get; set; } = new List<OutCrossGroup>();
        public ObservableCollection<NgDefectResult> ngDefectResults { get; set; } = new ObservableCollection<NgDefectResult>();
        /// <summary>
        /// 点结果输出
        /// </summary>
        public List<PointInfo> PointResults { get; set; } = new List<PointInfo>();
        /// <summary>
        /// 线结果输出
        /// </summary>
        public List<LineInfo> LineInfosResult { get; set; } = new List<LineInfo>();
        public override void Dispose()
        {
            foreach (var item in ngDefectResults)
            {
                item?.Dispose();
            }
            PointResults.Clear();
            LineInfosResult.Clear();
            ngDefectResults?.Clear();
            CrossPointInfos.Clear();
            base.Dispose();
        }
    }
}
