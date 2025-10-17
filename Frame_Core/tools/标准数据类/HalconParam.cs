using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class Param_CircleModel
    {

        /// <summary>
        ///  输入变量：圆心坐标X
        /// </summary>
        public double center_X = 0;
        /// <summary>
        ///  输入变量：圆心坐标Y
        /// </summary>
        public double center_Y = 0;
        /// <summary>
        ///  输入变量：半径
        /// </summary>
        public double radius = 0;

        /// <summary>
        ///  输入变量：测量长度1
        /// </summary>
        public double MeasureLength1 = 20;

        /// <summary>
        ///  输入变量：测量长度2
        /// </summary>
        public double MeasureLength2 = 5;

        /// <summary>
        /// 测量平滑系数
        /// </summary>
        public double MesureSigma = 1;

        /// <summary>
        /// 测量阈值
        /// </summary>
        public double MesureThreshold = 30;

        /// <summary>
        /// 拟合数量：默认值： 1 值的列表： 1，2，3，4
        /// </summary>     
        public int MeasureNumInstances = 1;

        /// <summary>
        /// Roi密度：
        /// </summary>  
        public int RoiPointsNum = 10;


        /// <summary>
        /// 明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
        /// </summary>    
        public int MeasureTransition = (int)MeasureTransitionEnum.从暗到明;

        /// <summary>
        /// 边缘选取：'all'，'first'，'last'
        /// </summary>

        public int MeasureSelect = (int)MeasureSelectEnum.第一条边;


    }


    public class RectangleInfo
    {
        /// <summary>
        /// 左边缘起始行
        /// </summary>
        public double leftEdgeBeginRow = 0;

        /// <summary>
        /// 左边缘起始列
        /// </summary>
        public double leftEdgeBeginCol = 0;

        /// <summary>
        /// 左边缘结束行
        /// </summary>
        public double leftEdgeEndRow = 0;

        /// <summary>
        /// 左边缘结束列
        /// </summary>
        public double leftEdgeEndCol = 0;

        /// <summary>
        /// 上边缘起始行
        /// </summary>
        public double topEdgeBeginRow = 0;

        /// <summary>
        /// 上边缘起始列
        /// </summary>
        public double topEdgeBeginCol = 0;

        /// <summary>
        /// 上边缘结束行
        /// </summary>
        public double topEdgeEndRow = 0;

        /// <summary>
        /// 上边缘结束列
        /// </summary>
        public double topEdgeEndCol = 0;

        /// <summary>
        /// 右边缘起始行
        /// </summary>
        public double rightEdgeBeginRow = 0;

        /// <summary>
        /// 右边缘起始列
        /// </summary>
        public double rightEdgeBeginCol = 0;

        /// <summary>
        /// 右边缘结束行
        /// </summary>
        public double rightEdgeEndRow = 0;

        /// <summary>
        /// 右边缘结束列
        /// </summary>
        public double rightEdgeEndCol = 0;

        /// <summary>
        /// 下边缘起始行
        /// </summary>
        public double bottomEdgeBeginRow = 0;

        /// <summary>
        /// 下边缘起始列
        /// </summary>
        public double bottomEdgeBeginCol = 0;

        /// <summary>
        /// 下边缘结束行
        /// </summary>
        public double bottomEdgeEndRow = 0;

        /// <summary>
        /// 下边缘结束列
        /// </summary>
        public double bottomEdgeEndCol = 0;

        /// <summary>
        /// 左上角行
        /// </summary>
        public double leftTopAngle_Row = 0;

        /// <summary>
        /// 左上角列
        /// </summary>
        public double leftTopAngle_Col = 0;

        /// <summary>
        /// 右上角行
        /// </summary>
        public double rightTopAngle_Row = 0;

        /// <summary>
        /// 右上角列
        /// </summary>
        public double rightTopAngle_Col = 0;

        /// <summary>
        /// 左下角行
        /// </summary>
        public double leftBottomAngle_Row = 0;

        /// <summary>
        /// 左下角列
        /// </summary>
        public double leftBottomAngle_Col = 0;

        /// <summary>
        /// 右下角行
        /// </summary>
        public double rightBottomAngle_Row = 0;

        /// <summary>
        /// 右下角列
        /// </summary>
        public double rightBottomAngle_Col = 0;

        /// <summary>
        /// 左边缘中心行
        /// </summary>
        public double leftEdgeCenter_Row = 0;

        /// <summary>
        /// 左边缘中心列
        /// </summary>
        public double leftEdgeCenter_Col = 0;

        /// <summary>
        /// 上边缘中心行
        /// </summary>
        public double topEdgeCenter_Row = 0;

        /// <summary>
        /// 上边缘中心列
        /// </summary>
        public double topEdgeCenter_Col = 0;

        /// <summary>
        /// 右边缘中心行
        /// </summary>
        public double rightEdgeCenter_Row = 0;

        /// <summary>
        /// 右边缘中心列
        /// </summary>
        public double rightEdgeCenter_Col = 0;

        /// <summary>
        /// 下边缘中心行
        /// </summary>
        public double bottomEdgeCenter_Row = 0;

        /// <summary>
        /// 下边缘中心列
        /// </summary>
        public double bottomEdgeCenter_Col = 0;

        /// <summary>
        /// 矩形宽度
        /// </summary>
        public double widthReality = 0;

        /// <summary>
        /// 宽结果
        /// </summary>
        public bool widthResult = false;

        /// <summary>
        /// 矩形高度
        /// </summary>
        public double heightReality = 0;

        /// <summary>
        /// 高结果
        /// </summary>
        public bool heightResult = false;

    }

    [Serializable]
    public class Param_LineModel : XmlSerializationOperation
    {

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
        /// 输入变量：测量长度1
        /// </summary>
        public double RoiLength1 = 20;

        /// <summary>
        /// 输入变量：测量长度2
        /// </summary>
        public double RoiLength2 = 5;

        /// <summary>
        /// 测量平滑系数
        /// </summary>
        public double MesureSigma = 1;

        /// <summary>
        /// 测量阈值
        /// </summary>
        public double MesureThreshold = 30;

        /// <summary>
        /// 拟合数量：默认值： 1 值的列表： 1，2，3，4
        /// </summary>     
        public int MeasureNumInstances = 1;


        /// <summary>
        /// Roi密度：
        /// </summary>  
        public int RoiPointsNum = 10;


        /// <summary>
        /// 明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
        /// </summary>    
        public int MeasureTransition = (int)MeasureTransitionEnum.从暗到明;

        /// <summary>
        /// 边缘选取：'all'，'first'，'last'
        /// </summary>

        public int MeasureSelect = (int)MeasureSelectEnum.第一条边;

        /// <summary>
        /// 距离剔除：默认值：3.5  值的列表： 0，1.0，2.0， 3.5，5.0
        /// </summary>
        public double MeasureDistThreshold = 4;

        /// <summary>
        /// 最小得分：'min_score'
        /// </summary>    
        public double MeasureMinScore = 0.5;

        /// <summary>
        /// 迭代次数: 默认值：-1  值的列表： 10，100，1000
        /// </summary>    
        public int MaxNumIterations = -1;

        /// <summary>
        /// 插值类型：nearest_neighbor'灰度值是从最近像素的灰度值获得；'bilinear'，使用双线性插值 ；'bicubic'，则使用双三次插值。
        /// </summary>      
        public string MeasureInterpolation = "nearest_neighbor";

        /// <summary>
        /// 指定对测量结果的验证：默认值： “ false”  值列表： 'true'，'false'
        /// </summary>
        public string MeasureIORegions = "true";
    }

    public class CircleInfo : IDisposable
    {
        /// <summary>
        /// 圆轮廓
        /// </summary>
       public HObject ho_Contours = null;
        /// <summary>
        /// 圆心交叉点
        /// </summary>
        public HObject ho_Cross = null;

        /// <summary>
        /// 圆心坐标X
        /// </summary>
        public double center_X = 0;
        /// <summary>
        /// 圆心坐标Y
        /// </summary>
        public double center_Y = 0;
        /// <summary>
        /// 半径
        /// </summary>
        public double radius = 0;

        public void Dispose()
        {
            ho_Contours?.Dispose();
            ho_Cross?.Dispose();
        }
    }

    public class PointInfo
    {
        public double Row = 0;
        public double Col = 0;
    }

    [Serializable]
    public class PointRotateInfo
    {
        /// <summary>
        /// Y坐标
        /// </summary>
        [Description("Y坐标")]
        public double Row = 0;
        /// <summary>
        /// X坐标
        /// </summary>
        [Description("X坐标")]
        public double Col = 0;
        /// <summary>
        /// 角度
        /// </summary>
        [Description("角度")]
        public double Angle = 0;

        public bool IsRotate = false;
    }
    
    public class LineInfo1
    {
        public string StrLineName = "";
        /// <summary>
        /// 抓边是否成功
        /// </summary>
        public bool IsLineSuccessfull = false;

        public double RowBegin = 0;
        public double ColBegin = 0;
        public double RowEnd = 0;
        public double ColEnd = 0;


        /// <summary>
        /// 抓边ROI中点行坐标
        /// </summary>    
        public double RoiMidRow = 300;

        /// <summary>
        /// 抓边ROI中点列坐标
        /// </summary>     
        public double RoiMidColumn = 300;

        /// <summary>
        /// 抓边ROI旋转角度
        /// </summary>     
        public double RoiAngle = -90;

        /// <summary>
        /// 测量长度1
        /// </summary>
        public double RoiLength1 = 20;

        /// <summary>
        /// 测量长度2
        /// </summary>
        public double RoiLength2 = 5;
    }

    public class Rectangle1_Param
    {
        /// <summary>
        /// 输入变量：抓边ROI起始行坐标
        /// </summary>
        public double hv_RoiBeginRow = 100;

        /// <summary>
        /// 输入变量：抓边ROI起始列坐标
        /// </summary>
        public double hv_RoiBeginCol = 100;

        /// <summary>
        /// 输入变量：抓边ROI结束行坐标
        /// </summary>
        public double hv_RoiEndRow = 500;

        /// <summary>
        /// 输入变量：抓边ROI结束列坐标
        /// </summary>
        public double hv_RoiEndCol = 500;
    }
    public class Rectangle2_Param
    {
        /// <summary>
        /// 输入变量：抓边ROI中点行坐标
        /// </summary>
        public double hv_RoiMidRow = 300;

        /// <summary>
        /// 输入变量：抓边ROI中点列坐标
        /// </summary>
        public double hv_RoiMidColumn = 300;

        /// <summary>
        /// 输入变量：抓边ROI旋转角度
        /// </summary>
        public double hv_RoiAngle = -90;

        /// <summary>
        /// 输入变量：抓边ROI高度
        /// </summary>
        public double hv_RoiHeight = 200;

        /// <summary>
        /// 输入变量：抓边ROI宽度
        /// </summary>
        public double hv_RoiWidth = 200;
    }

    /// <summary>
    /// 枚举绑定的翻译特性
    /// </summary>
    public class AnotherNamerAttribute : Attribute
    {
        /// <summary>
        /// 别称
        /// </summary>
        public string AnotherNamer { get; set; }

        public AnotherNamerAttribute(string StrAnotherNamer)
        {
            this.AnotherNamer = StrAnotherNamer;
        }
    }

    public enum MeasureTransitionEnum
    {
        [AnotherNamer("positive")]
        从暗到明,
        [AnotherNamer("negative")]
        从明到暗,
        [AnotherNamer("all")]
        不区分明暗
    }

    public enum MeasureSelectEnum
    {
        [AnotherNamer("first")]
        第一条边,
        [AnotherNamer("last")]
        最后一条边,
        [AnotherNamer("all")]
        最强边
    }

   
}
