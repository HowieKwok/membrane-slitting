using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Cor
{
    public class LineWidthFilmParam
    {

     
        /// <summary>
        /// 点检区域X1
        /// </summary>
        public double EdgeX1 = 600;
        /// <summary>
        /// 点检区域X2
        /// </summary>
        public double EdgeX2 = 800;

        /// <summary>
        /// 菲林片最小高度
        /// </summary>
        public double FilmMinHeight = 1800;

        /// <summary>
        /// 菲林片最大高度
        /// </summary>
        public double FilmMaxHeight = 2200;

        /// <summary>
        /// 菲林片最小阈值
        /// </summary>
        public double CalibBlockMinThreshold = 150;

        /// <summary>
        /// 菲林片最大阈值
        /// </summary>
        public double CalibBlockMaxThreshold = 255;

        /// <summary>
        /// 检测开始行到中心距离
        /// </summary
        public int DetectionStartRowToCentreDistance = 100;

      

    }
}
