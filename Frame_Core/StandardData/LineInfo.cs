using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{

    [Serializable]
    /// <summary>
    /// 字符信息显示
    /// </summary>
    public class MessageShowInfo
    {
        public string m_strMessage = "";
        public HalconColor m_stuGetColor=HalconColor.green;
        public double m_fCol = 0;
        public double m_fRow = 0;
        public int m_nFontSize = 10;
        public bool m_bIsWindow = true;
    }

    /// <summary>
    /// 区域信息显示
    /// </summary>
    public class RegionShowInfo
    {
        public HObject m_hRegion;
        public HalconColor m_stuGetColor;
    }

    [Serializable]
    public class LineInfo
    {
        public CameraEnum m_camera;

        public String StrLineName = "默认边";   //直线名称，string

        public bool IsStandLine = false;       //直线是否为基准边,默认为插件抓边。 //OverHang工位为是否为隔膜边，

        public bool IsHorizonLine = false;     //X方向抓边，水平边。用于计算线线距离，水平边直接计算纵向距离，垂直边直接计算横向距离
        /// <summary>
        /// 抓边是否成功
        /// </summary>
        public bool IsLineSuccessfull = false;
        /// <summary>
        /// 代表已加入
        /// </summary>
        public bool IsAdd = false;

        public int RowBeginID = 0;
        public double RowBegin = 0;
        public double ColBegin = 0;
        public int RowEndID = 0;
        public double RowEnd = 0;
        public double ColEnd = 0;

        /// <summary>
        /// 线颜色
        /// </summary>
        public HalconColor Color;
        /// <summary>
        /// 线宽
        /// </summary>
        public int m_nLineWidth = 2;

    }

}
