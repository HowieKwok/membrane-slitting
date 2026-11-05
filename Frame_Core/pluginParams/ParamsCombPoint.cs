using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core.pluginParams
{


    public class ParamsCombPoint : PluginParamsBase
    {

        //输入变量
        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;


        [Description("判断是否启用")]
        public bool m_bIsEnable = true;

        /// <summary>
        /// 输入变量Line1
        /// </summary>
        [Description(" 输入变量Line1类")]
        public LineInfo LineInfo1 = new LineInfo();

        /// <summary>
        /// 输入变量Line2
        /// </summary>
        [Description(" 输入变量Line2类")]
        public LineInfo LineInfo2 = new LineInfo();


        #region 4. 输出变量

        /// <summary>
        /// 交点行坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_IntersectionRow = null;

        /// <summary>
        /// 交点列坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_IntersectionColumn = null;

        /// <summary>
        /// 两条直线夹角(度)
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_LineAngle = null;

        /// <summary>
        /// 两直线是否平行(平行则无交点)
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_IsParallel = null;

        /// <summary>
        /// 直线1拟合成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_Line1_FitSuccess = false;

        /// <summary>
        /// 直线2拟合成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_Line2_FitSuccess = false;

        /// <summary>
        /// 交点计算成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_IntersectionSuccess = false;

        #endregion


    }
}
