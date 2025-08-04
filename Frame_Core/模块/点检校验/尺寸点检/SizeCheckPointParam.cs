using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 尺寸点检参数
    /// </summary>
    public class SizeCheckPointParam : XmlSerializationOperation
    {
        /// <summary>
        /// 开始点检标志
        /// </summary>
        public bool StartDJFlag { get; set; } = false;

        /// <summary>
        /// 结束点检标志
        /// </summary>
        public bool EndDJFlag { get; set; } = false;

        /// <summary>
        /// 尺寸点检结果
        /// </summary>
        public bool SizeDJResult { get; set; } = false;

        /// <summary>
        /// 当前点检计数
        /// </summary>
        public int CueerntDJCount { get; set; } = 0;

        /// <summary>
        /// 失败累计计数
        /// </summary>
        public int FailAccumulativeCount { get; set; } = 0;

        /// <summary>
        /// 报警累计计数
        /// </summary>
        public int AlarmAccumulativeCount { get; set; } = 0;

        /// <summary>
        /// 停机累计计数
        /// </summary>
        public int StopAccumulativeCount { get; set; } = 0;

        /// <summary>
        /// 复位开始点检
        /// </summary>
        public void ResetStartDJ()
        {
            StartDJFlag = true;
            EndDJFlag = false;
            SizeDJResult = false;
            CueerntDJCount = 0;
            FailAccumulativeCount = 0;
            AlarmAccumulativeCount = 0;
            StopAccumulativeCount = 0;
        }

        /// <summary>
        /// 新建膜卷初始化（重新开始实时点检）
        /// </summary>
        public void NewMojuanInitSSDJ()
        {
            StartDJFlag = false;
            EndDJFlag = false;
        }

        /// <summary>
        /// 点检图片路径
        /// </summary>
        public string PointCheckImgPath = @"E:\PointCheckImg\";

        /// <summary>
        /// 实时点检
        /// </summary>
        public bool SSDJ = false;

        /// <summary>
        /// 实时点检速度限制
        /// </summary>
        public int PointCheckSpeedRestrict = 10;

        /// <summary>
        /// 自动点检开始帧
        /// </summary>
        public int CheckStartFrame = 0;

        /// <summary>
        /// 点检次数
        /// </summary>
        public int CheckNumber = 50;

        /// <summary>
        /// 失败累计个数
        /// </summary>
        public int FailAccumulativeNum = 3;

        /// <summary>
        /// 报警累计个数
        /// </summary>
        public int AlarmAccumulativeNum = 3;

        /// <summary>
        /// 停机累计个数
        /// </summary>
        public int StopAccumulativeNum = 3;

        /// <summary>
        /// 报警
        /// </summary>
        public bool Alarm = true;

        /// <summary>
        /// 停机
        /// </summary>
        public bool Stop = true;

        /// <summary>
        ///菲林片宽度
        /// </summary>
        public double CalibBlockWidth = 5;
        /// <summary>
        ///菲林片宽度公差
        /// </summary>
        public double CalibBlockWidthTolerance = 1;

        /// <summary>
        ///菲林片高度
        /// </summary>
        public double CalibBlockHeiht = 5;

        /// <summary>
        ///菲林片高度公差
        /// </summary>
        public double CalibBlockHeihtTolerance = 1;

        /// <summary>
        /// 点检区域X1
        /// </summary>
        public double EdgeX1 = 600;
        /// <summary>
        /// 点检区域X2
        /// </summary>
        public double EdgeX2 = 800;


        /// <summary>
        /// 菲林片最小阈值
        /// </summary>
        public double CalibBlockMinThreshold = 0;

        /// <summary>
        /// 菲林片最大阈值
        /// </summary>
        public double CalibBlockMaxThreshold = 10;

        /// <summary>
        /// 菲林片边长X(Pixel)
        /// </summary>
        public double SideLengthPixX = 100;

        /// <summary>
        /// 菲林片边长YPixel)
        /// </summary>
        public double SideLengthPixY = 100;


        /// <summary>
        /// 线模型参数
        /// </summary>
        public Param_LineModel LineModelParam = new Param_LineModel();

        /// <summary>
        /// 只能复制值类型参数
        /// </summary>
        /// <returns></returns>
        public SizeCheckPointParam Clone()
        {

            return this.MemberwiseClone() as SizeCheckPointParam;

        }
    }
}
