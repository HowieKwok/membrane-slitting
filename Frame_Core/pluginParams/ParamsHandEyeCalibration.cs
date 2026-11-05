using Frame_Core;
using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 端面毛刺插件参数
    /// </summary>
    public class ParamsHandEyeCalibration : PluginParamsBase
    {
        #region 1. 输入参数
        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public ImageInfo ImageInfo;
        /// <summary>
        /// 输入变量：屏蔽区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ShieldRegion;


        /// <summary>
        /// 输入变量：图像X向像素当量
        /// </summary>
        public double Pix_X = 0.002;
        /// <summary>
        /// 输入变量：图像Y向像素当量
        /// </summary>
        public double Pix_Y = 0.002;



    


        /// <summary>
        /// 九点标定的图像坐标X
        /// </summary>
        public double[] ImageX = new double[30];


        /// <summary>
        /// 九点标定的图像坐标Y
        /// </summary>
        public double[] ImageY = new double[30];

        /// <summary>
        /// 九点标定的机器人坐标X
        /// </summary>
        public double[] RobotX = new double[30];


        /// <summary>
        /// 九点标定的机器人坐标Y
        /// </summary>
        public double[] RobotY = new double[30];

        [NonSerialized]
        /// <summary>
        /// 输入的待转化的 图像X坐标
        /// </summary>
        public  double inputImageX = 1;

        [NonSerialized]
        /// <summary>
        /// 输入的待转化的 图像Y坐标
        /// </summary>
        public double inputImageY = 1;

        #endregion


        #region 2.输出变量






        /// <summary>
        /// 输出变量：模块运行成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_ModelRunFlag = false;

        /// <summary>
        /// 输出变量：图像处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageProcessed;




        /// <summary>
        /// 矩阵
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple HomMat2D;



        [NonSerialized]
        /// <summary>
        /// 输出的转化后的 机械手X坐标
        /// </summary>
        public double outputRobotX = 0;

        [NonSerialized]
        /// <summary>
        /// 输出的转化后的 机械手Y坐标
        /// </summary>
        public double outputRobotY = 0;



        #endregion

        public ParamsHandEyeCalibration()
        {
            ImageInfo =new ImageInfo();
            HOperatorSet.GenEmptyObj(out ho_ShieldRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);


         
        }
    }
}
