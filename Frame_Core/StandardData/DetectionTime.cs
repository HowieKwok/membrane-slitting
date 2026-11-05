using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class DetectionTime
    {
        #region 图像处理部分
        /// <summary>
        /// 图像开始时间
        /// </summary>
        public HTuple Image_StartTime = 0;

        /// <summary>
        /// 图像找边时间
        /// </summary>
        public HTuple Image_FindLineTime = 0;

        /// <summary>
        /// 图像缺陷检测时间
        /// </summary>
        public HTuple Image_FindDefectTime = 0;

        /// <summary>
        /// 图像算法处理时间
        /// </summary>
        public HTuple Image_ArithmeticProcessTime = 0;
        /// <summary>
        /// 图像数据开始处理时间
        /// </summary>
        public HTuple Image_StartDataProcessTime = 0;

        /// <summary>
        /// 图像数据处理时间
        /// </summary>
        public HTuple Image_DataProcessTime = 0;
        /// <summary>
        /// 图像结束时间
        /// </summary>
        public HTuple Image_EndTime = 0;
        #endregion



        #region 数据处理部分
        public HTuple Data_StartTime = 0;

        public HTuple Data_EndTime = 0;
        #endregion

    }
}
