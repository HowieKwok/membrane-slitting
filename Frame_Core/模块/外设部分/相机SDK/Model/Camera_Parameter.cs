using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Frame_Core;

namespace Frame_Core
{
    [Serializable]
    public class CameraParameter : XmlSerializationOperation
    {
        [My("相机名称")]
        [Description("相机名称")]
        public string CameraName;

        [My("相机种类")]
        [Description("相机种类")]
        public string CameraType;

        [My("序列号/采集卡型号")]
        [Description("序列号/采集卡型号")]
        public string CameraSerial;

        [My("相机IP/CCF路径")]
        [Description("相机IP/CCF路径")]
        public string CameraIP;

        [My("本地IP/采集卡端口")]
        [Description("本地IP/采集卡端口")]
        public string PCIP;

        [My("图像高度")]
        [Description("图像高度")]
        public int ImageHeight;

        [My("图像宽度")]
        [Description("图像宽度")]
        public int ImageWidth;

        [My("仿真路径")]
        [Description("仿真路径")]
        public string EmulationPath = "";

        /// <summary>
        /// 横向校准值
        /// </summary>
        [My("横向校准值")]
        [Description("横向校准值")]
        public double CalibrationValue_X = 1;

        /// <summary>
        /// 纵向校准值
        /// </summary>
        [My("纵向校准值")]
        [Description("纵向校准值")]
        public double CalibrationValue_Y = 1;


        /// <summary>
        /// 到主相机的距离
        /// </summary>
        [My("到主相机的距离")]
        [Description("到主相机的距离")]
        public double ToMainCamDistance = 0;

        /// <summary>
        /// OutputIndex
        /// </summary>
        [My("OutputIndex")]
        [Description("OutputIndex")]
        public int OutputIndex = 1;

        /// <summary>
        /// 水平镜像
        /// </summary>
        public bool HorizontalMirrorImage = false;

        /// <summary>
        /// 垂直镜像
        /// </summary>
        public bool VerticalMirrorImage = false;

        /// <summary>
        /// 旋转角度
        /// </summary>
        public int RotationAngle = (int)RotationAngleEnum.零度;

        public CameraParameter()
        {
            Init();
        }
        public void Init()
        {

            CameraName = "默认相机";
            CameraType = "DALSCameraLink相机";
            CameraSerial = "Xtium-CL_MX4_1";
            CameraIP = "D:\\CameraConfig\\camera1.ccf";
            PCIP = "0";
            ImageHeight = 5000;
            ImageWidth = 8192;
            CalibrationValue_X = 1;
            CalibrationValue_Y = 1;
            ToMainCamDistance = 0;
            OutputIndex = 1;
        }
    }


}
