using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class RectangleFilmParam
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
  
    }
}
