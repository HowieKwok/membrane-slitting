using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class Param_LineToLineDist : XmlSerializationOperation
    {

        public string ProdDataPath = "";

        public bool SizeMeasurementEn = false;

        public List<Param_Line> LineParams;

        public List<Param_SizeLogic> SizeLogicParams;

        public List<Param_SizeLogic_UserDefined> SizeLogicParams_UserDefineds;

        public void Init()
        {
            if (LineParams == null) LineParams = new List<Param_Line>();
            if (SizeLogicParams == null) SizeLogicParams = new List<Param_SizeLogic>();
            if (SizeLogicParams_UserDefineds == null) SizeLogicParams_UserDefineds = new List<Param_SizeLogic_UserDefined>();
        }
    }

    public class Param_Line
    {
        public string LineName = "";

        /// <summary>
        /// Roi密度：
        /// </summary>  
        public int RoiPointsNum = 10;

        /// <summary>
        /// 输入变量：抓边ROI中点行坐标
        /// </summary>    
        public double RoiMidRow = 300;

        /// <summary>
        /// 输入变量：抓边ROI中点列坐标
        /// </summary>     
        public double RoiMidColumn = 300;

        /// <summary>
        /// 输入变量：抓边ROI旋转角度
        /// </summary>     
        public double RoiAngle = -90;

        /// <summary>
        /// 输入变量：测量长度1（W）
        /// </summary>
        public double RoiLength1 = 200;

        /// <summary>
        /// 输入变量：测量长度2（H）
        /// </summary>
        public double RoiLength2 = 200;

        /// <summary>
        /// 输入变量：ROI是否跟随
        /// </summary>
        public bool IsROIFollow = false;

        /// <summary>
        /// 输入变量：基准点X坐标
        /// </summary>
        public int BasePointX = 0;

        /// <summary>
        /// 输入变量：基准点Y坐标
        /// </summary>
        public int BasePointY = 0;

        /// <summary>
        /// 输入变量：基准点偏移X坐标
        /// </summary>
        public int BaseOffsetX = 0;

        /// <summary>
        /// 输入变量：基准点偏移Y坐标
        /// </summary>
        public int BaseOffsetY = 0;

        /// <summary>
        /// 输入变量：ROI缩放尺寸
        /// </summary>
        public int ROISize = 2;

        /// <summary>
        /// 测量阈值
        /// </summary>
        public double MesureThreshold = 30;

        /// <summary>
        /// 测量平滑系数
        /// </summary>
        public double MesureSigma = 3;

        /// <summary>
        /// 明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
        /// </summary>    
        public int MeasureTransition_ = (int)MeasureTransition.从暗到明;

        /// <summary>
        /// 边缘选取：'all'，'first'，'last'
        /// </summary>

        public int MeasureSelect = (int)MeasureLine.第一条边;

        /// <summary>
        /// 最小得分：'min_score'
        /// </summary>    
        public double MeasureMinScore = 0.5;

        /// <summary>
        /// 拟合数量：默认值： 1 值的列表： 1，2，3，4
        /// </summary>     
        public int MeasureNumInstances = 1;

        /// <summary>
        /// 拟合线的排序方式
        /// </summary>
        public int LineSortType = (int)SortEnum.X;

        /// <summary>
        /// 距离剔除：默认值：3.5  值的列表： 0，1.0，2.0， 3.5，5.0
        /// </summary>
        public double MeasureDistThreshold = 4;


        /// <summary>
        /// 迭代次数: 默认值：-1  值的列表： 10，100，1000
        /// </summary>    
        public int MaxNumIterations = -1;

        /// <summary>
        /// 指定对测量结果的验证：默认值： “ false”  值列表： 'true'，'false'
        /// </summary>
        public int MeasureIORegions_ = (int)MeasureIORegions.是;

        /// <summary>
        /// 插值类型：nearest_neighbor'灰度值是从最近像素的灰度值获得；'bilinear'，使用双线性插值 ；'bicubic'，则使用双三次插值。
        /// </summary>      
        public int MeasureInterpolation_ = (int)MeasureInterpolation.最近邻域法;

    }

    public class Param_SizeLogic
    {
        public int SizeID { get; set; } = (int)SizeEnum.极片尺寸X;

        public string Line1Name { get; set; } = "";

        public string Line2Name { get; set; } = "";

        public int SizeCalculateLogic { get; set; } = (int)SizeComputationalLogicEnum.X;
    }
    public class Param_SizeLogic_UserDefined
    {
        public string Name { get; set; } = "";

        public string Line1Name { get; set; } = "";

        public string Line2Name { get; set; } = "";

        public int SizeCalculateLogic { get; set; } = (int)SizeComputationalLogicEnum.X;

        public double StandardValue { get; set; } = 0.0;

        public double ToleranceValue { get; set; } = 0.0;

        public double CoefficientValue { get; set; } = 1;
    }

    public enum SizeComputationalLogicEnum
    {
        X,
        Y
    }

    public class SizeInfo_P
    {
        public string name = "";

        public int SizeID = (int)SizeEnum.极片尺寸X;

        public double value = -999;

        public bool result = false;

        public ExecuteResult executeResult = new ExecuteResult();
    }
}
