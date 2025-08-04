using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class Param_ControlCard : XmlSerializationOperation
    {
        [Description("端面相机工作位置")]
        public double duanM_WorkPosition = 0;

        [Description("端面相机自动聚焦")]
        public bool duanM_IsAutoFocusing = false;

        [Description("端面相机符号取反")]
        public bool duanM_Sign = false;

        [Description("端面相机工作位置公差范围")]
        public double duanM_WorkTolerances = 1;

        [Description("大面相机基准坐标")]
        public double daM_ReferenceCoordinate = 0;

        [Description("大面相机采样数量")]
        public int daM_SamplingQuantity = 5;


        public Axis_Param axis_Param;

        public void Init()
        {
            axis_Param = new Axis_Param();
        }
    }

    public class Axis_Param
    {
        /// <summary>
        ///  轴号
        /// </summary>
        public int AxisIndex = 0;

        /// <summary>
        ///  轴号
        /// </summary>       
        public short AxisID => Convert.ToInt16(AxisIndex + 1);


        /// <summary>
        /// 齿轮比（分子）
        /// </summary>
        public double GearRatioMolecule = 1;

        /// <summary>
        /// 齿轮比（分母）
        /// </summary>
        public double GearRatioDenominator = 5000;

        /// <summary>
        /// 速度
        /// </summary>
        public double Velocity = 200;

        /// <summary>
        /// 加速度
        /// </summary>
        public double Acceleration = 10;

        /// <summary>
        /// 平滑时间
        /// </summary>
        public double SmoothTime = 20;

        /// <summary>
        /// 工作位置1
        /// </summary>
        public double WorkPosition1 = 0;

        /// <summary>
        /// 移动距离mm（单步移动）
        /// </summary>
        public double MovementDistance = 5;
    }
    public class Axis_State
    {
        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected = false;

        /// <summary>
        /// 轴使能
        /// </summary>
        public bool AxisEnable = false;

        /// <summary>
        ///  初始化完成   
        /// </summary>
        public bool IsInitFinish = false;

        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy = false;


        /// <summary>
        /// 当前位置值(pulse)
        /// </summary>
        public int CurrentPosition_Pulse = 0;

        /// <summary>
        /// 目标位置
        /// </summary>
        public int AxisTargetPosition = 0;

        /// <summary>
        /// 原点
        /// </summary>
        public bool Home;

        /// <summary>
        /// 负限位
        /// </summary>
        public bool NegativeLimit;

        /// <summary>
        /// 正限位
        /// </summary>
        public bool PositiveLimit;

        /// <summary>
        /// 报警
        /// </summary>
        public bool Alarm;

        /// <summary>
        /// 异常
        /// </summary>
        public bool Abnormal;
    }
}
