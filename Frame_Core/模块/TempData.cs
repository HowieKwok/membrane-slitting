using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 临时数据
    /// </summary>
    public class TempData
    {

        DateTime currentTime = DateTime.Now;

        /// <summary>
        /// 每小时保存N张图片开始帧号
        /// </summary>
        public long HourSaveImageBeginFrame
        {
            get
            {
                return _HourSaveImageBeginFrame;
            }
            set
            {
                DateTime dateTime = DateTime.Now;
                double dg = dateTime.Subtract(currentTime).TotalHours;
                if (dateTime.Subtract(currentTime).TotalHours >= 1)
                {
                    currentTime = dateTime;
                    _HourSaveImageBeginFrame = value;

                }
            }
        }
        private long _HourSaveImageBeginFrame = 0;

        /// <summary>
        /// 启用每小时保存N张图像完成标志
        /// </summary>
        public bool hourSaveImageFinsh = false;
        /// <summary>
        /// 获取小时保存N张图像结果
        /// </summary>
        /// <param name="EnHourSaveImage"></param>
        /// <param name="saveImageNum"></param>
        /// <param name="currentImageFrame"></param>
        /// <returns></returns>
        public bool GetHourSaveImageResult(bool EnHourSaveImage, long saveImageNum, long currentImageFrame)
        {
            HourSaveImageBeginFrame = currentImageFrame;
            long imageEndFrame = _HourSaveImageBeginFrame + saveImageNum;
            hourSaveImageFinsh = EnHourSaveImage && (currentImageFrame > imageEndFrame);
            return hourSaveImageFinsh;
        }


    }
}
