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
    public class Param_CalibrationPlate : XmlSerializationOperation
    {
        /// <summary>
        /// 基准点X
        /// </summary>
        [Description("基准点X")]
        public double BasePointX = 0;

        /// <summary>
        /// 基准点Y
        /// </summary>
        [Description("基准点Y")]
        public double BasePointY = 0;

        /// <summary>
        /// OVH检测到的Mark基准点X
        /// </summary>
        [Description("OVH检测到的Mark基准点X")]
        public double m_DetectionBaseCenterX = 1.00;

        /// <summary>
        /// OVH检测到的Mark基准点Y
        /// </summary>
        [Description("OVH检测到的Mark基准点Y")]
        public double m_DetectionBaseCenterY = 1.00;

        /// <summary>
        /// OVH检测到的Mark基准点R
        /// </summary>
        [Description("OVH检测到的Mark基准点R")]
        public double m_DetectionBaseCenterR = 0;

        /// <summary>
        /// 标定块类型
        /// </summary>
        [Description("标定块类型")]
        public int CaltabType = (int)CaltabTypeEnum.矩形标记;

        /// <summary>
        /// 焦距
        /// </summary>
        [Description("焦距")]
        public double Focus = 0.008;

        /// <summary>
        /// 畸变系数
        /// </summary>
        [Description("畸变系数")]
        public double Kappa = 0;

        /// <summary>
        /// 传感器两个相邻单元水平距离
        /// </summary>
        [Description("传感器两个相邻单元水平距离")]
        public double Sx = 5.2e-6;

        /// <summary>
        /// 传感器两个相邻单元垂直距离
        /// </summary>
        [Description("传感器两个相邻单元垂直距离")]
        public double Sy = 5.2e-6;

        /// <summary>
        /// 标定版的厚度
        /// </summary>
        [Description("标定版的厚度")] 
        public double Dz = 0;

        /// <summary>
        /// 标定版的直径
        /// </summary>
        [Description("标定版的直径")]
        public double CalibrationTargetDiameter = 0;

        /// <summary>
        /// 标定版的中心距离
        /// </summary>
        [Description("标定版的中心距离")]
        public double CalibrationTargetCenterDistance = 0;


        /// <summary>
        /// 圆心X坐标
        /// </summary>
        [Description("圆心X坐标")]
        public double Center_X = 0;

        /// <summary>
        /// 圆心Y坐标
        /// </summary>
        [Description("圆心Y坐标")]
        public double Center_Y = 0;

        /// <summary>
        /// 半径(单位像素)
        /// </summary>
        [Description("半径(单位像素)")]
        public double Radius = 5;

        /// <summary>
        /// 圆心最小阈值
        /// </summary>
        [Description("圆心最小阈值")]
        public double CirclMinThd = 150;

        /// <summary>
        /// 圆心最大阈值
        /// </summary>
        [Description("圆心最大阈值")]
        public double CirclMaxThd = 255;

        /// <summary>
        /// 圆模型参数
        /// </summary>
        [Description("圆模型参数")]
        public Param_CircleModel CircleModelParam = new Param_CircleModel();


        /// <summary>
        /// Mark圆心X坐标
        /// </summary>
        [Description("Mark圆心X坐标")]
        public double MarkCenter_X = 0;

        /// <summary>
        /// Mark圆心Y坐标
        /// </summary>
        [Description("Mark圆心Y坐标")]
        public double MarkCenter_Y = 0;

        /// <summary>
        /// Mark半径(单位像素)
        /// </summary>
        [Description("Mark半径(单位像素)")]
        public double MarkRadius = 5;

        /// <summary>
        /// Mark最小宽度(单位像素)
        /// </summary>
        [Description("Mark最小宽度(单位像素)")]
        public double MarkWidthMin = 250;

        /// <summary>
        /// Mark最大宽度(单位像素)
        /// </summary>
        [Description("Mark最大宽度(单位像素)")]
        public double MarkWidthMax = 1000;

        /// <summary>
        /// Mark圆心最小阈值
        /// </summary>
        [Description("Mark圆心最小阈值")]
        public double MarkCirclMinThd = 150;

        /// <summary>
        /// Mark圆心最大阈值
        /// </summary>
        [Description("Mark圆心最大阈值")]
        public double MarkCirclMaxThd = 255;

        /// <summary>
        /// Mark圆模型参数
        /// </summary>
        [Description("Mark圆模型参数")]
        public Param_CircleModel MarkCircleModelParam =new Param_CircleModel();

        /// <summary>
        /// 启用Mark
        /// </summary>
        [Description("启用Mark")]
        public bool bIsEnableMark = false;

        /// <summary>
        /// 相机内参
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_CameraParam = new HTuple();

        /// <summary>
        /// 相机外参
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_WorldPose = new HTuple();

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public string cameraParampath = System.Environment.CurrentDirectory + @"\\hv_CamParam_data.tup";

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public string worldPosepath = System.Environment.CurrentDirectory + @"\\hv_CamPose_data.tup";
        public void ReadCalibParam()
        {
            if (System.IO.File.Exists(cameraParampath))
            {
                hv_CameraParam = HTuple.ReadTuple(cameraParampath);
            }

            if (System.IO.File.Exists(worldPosepath))
            {
                hv_WorldPose = HTuple.ReadTuple(worldPosepath);
            }
        }

        public void SaveCalibParam()
        {
            hv_CameraParam?.WriteTuple(cameraParampath);
            hv_WorldPose?.WriteTuple(worldPosepath);
        }
    }

    public enum CaltabTypeEnum
    {
        矩形标记,//RectangularlyArrangedMarks
        六边形标记,//HexagonallyArrangedMarks
    }

}
