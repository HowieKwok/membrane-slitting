using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class Param_MarkSet : XmlSerializationOperation
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
        /// 基准点R
        /// </summary>
        [Description("基准点R")]
        public double BasePointR = 0;

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
        /// Mark的位置范围±（Pix）
        /// </summary>
        [Description("Mark的位置范围±（Pix")]
        public double MarkPosRange = 3;

        /// <summary>
        /// 启用Mark的位置范围报警停机
        /// </summary>
        [Description("启用Mark的位置范围报警停机")]
        public bool MarkPosRangeBJTJ = true;

        /// <summary>
        /// 连续Mark识别失败数量
        /// </summary>
        [Description("连续Mark识别失败数量")]
        public int UnidentificationMarkNum = 3;

        /// <summary>
        /// 连续Mark识别失败次数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        [Description("连续Mark识别失败次数")]
        public int UnidentificationMarkCount = 0;

        /// <summary>
        /// 连续Mark识别失败报警停机
        /// </summary>
        [Description("连续Mark识别失败报警停机")]
        public bool UnidentificationMarkBJTJ = true;
    }
}
