using HalconDotNet;
using IKapBoardClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class GrayScaleAdaptationParameter : XmlSerializationOperation
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable = false;

        /// <summary>
        /// 灰度范围最小值
        /// </summary>
        public int GrayRangeMin = 30;
        /// <summary>
        /// 灰度范围最大值
        /// </summary>
        public int GrayRangeMax = 250;

        /// <summary>
        /// 自适应间隔调整帧数
        /// </summary>
        public int IntervalFrame = 20;

        /// <summary>
        /// 单次调节比例
        /// </summary>
        public double SingleAdjustmentRatio = 1;

        /// <summary>
        /// 调节系数
        /// </summary>
        public double AdjustFactor = 1;

        /// <summary>
        /// 目标灰度
        /// </summary>
        public double TargetGray = 90;

        /// <summary>
        /// 调节灰度公差
        /// </summary>
        public int AdjustmentGrayTolerance = 10;

        /// <summary>
        /// 目标灰度容差
        /// </summary>
        public int TargetGrayTolerance = 5;

        /// <summary>
        /// 连续N帧图像涂布平均灰度超出目标灰度范围，则进行闭环调节光源亮度
        /// </summary>
        public int ContinuousNFrameOutOfScopeGray = 5;

        /// <summary>
        /// 连续N帧图像涂布平均灰度超出目标灰度范围报警停机
        /// </summary>

        public bool ContinuousNFrameOutOfScopeGrayAlarm = false;

        /// <summary>
        /// 涂布灰度波动大于
        /// </summary>
        public int GrayBodong = 15;

        /// <summary>
        /// 灰度波动报警停机
        /// </summary>
        public bool GrayBodongAlarm = false;

        /// <summary>
        /// N次调节数量
        /// </summary>
        public int AdjustFinishNum = 20;

        /// <summary>
        /// N次未调节完成报警
        /// </summary>
        public bool NNotAdjustFinishAlarm = false;

        /// <summary>
        /// 自适应调节光源端口号
        /// </summary>
        public int LightSourcePort = 0;

        /// <summary>
        /// 自适应调节光源通道
        /// </summary>
        public int LightSourceChannel = 1;



        /// <summary>
        /// 开始标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool Temp_StartFlag = false;

        /// <summary>
        /// 是否调节
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool Temp_IsAdjust = false;

        /// <summary>
        /// 上一帧调节帧
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public long Temp_PreviousFrame = 0;

     
        /// <summary>
        /// 当前光源值
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int Temp_CurrentLightValue = 0;


        /// <summary>
        /// 灰度连续在范围之外计数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int Temp_OutOfScopeGrayCount = 0;

        /// <summary>
        /// 灰度连续正常计数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int Temp_GaryNormalCount = 0;

        /// <summary>
        /// 调节计数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int Temp_AdjustCount = 0;

        /// <summary>
        /// 调节OK计数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int Temp_AdjustOKCount = 0;

        /// <summary>
        /// OK灰度队列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public QueuePackage<double> Temp_OKGrayQueue = new QueuePackage<double>();

        /// <summary>
        /// 连续灰度队列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple Temp_ContinuousGrayQueue = new HTuple();

        /// <summary>
        /// 复位
        /// </summary>      
        public void Reset(int currentLightValue)
        {
            Temp_CurrentLightValue = currentLightValue;
            Temp_StartFlag = true;
            Temp_IsAdjust = false;
            Temp_PreviousFrame = 0;
            Temp_OutOfScopeGrayCount = 0;
            Temp_GaryNormalCount = 0;
            Temp_AdjustCount = 0;
            Temp_AdjustOKCount = 0;
            Temp_OKGrayQueue.Clear();
            Temp_ContinuousGrayQueue = new HTuple();
        }
    }
}
