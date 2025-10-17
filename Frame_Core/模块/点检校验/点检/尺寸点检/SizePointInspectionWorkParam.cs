using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DALSA.SaperaLT.SapClassBasic.SapLut;

namespace Frame_Core
{
    public class SizePointInspectionWorkParam
    {

        /// <summary>
        /// 实时点检
        /// </summary>
        public bool SSDJ = false;

        /// <summary>
        /// 实时点检速度限制
        /// </summary>
        public int PointCheckSpeedRestrict = 10;

        /// <summary>
        /// 自动点检间隔帧数
        /// </summary>
        public int CheckIntervalFrame = 100;

        /// <summary>
        /// 点检次数
        /// </summary>
        public int CheckNumber = 10;

        /// <summary>
        /// 失败累计个数
        /// </summary>
        public int FailAccumulativeNum = 3;


        /// <summary>
        /// 报警
        /// </summary>
        public bool Alarm = true;

        /// <summary>
        /// 停机
        /// </summary>
        public bool Stop = true;


        /// <summary>
        /// 复位开始点检
        /// </summary>
        public void ResetStartDJ()
        {        
            SizeDJResult = false;
            LastCheckFrame = 0;
            CueerntDJCount = 0;
            FailAccumulativeCount = 0;
        }

   

        /// <summary>
        /// 尺寸点检结果
        /// </summary>
        public bool SizeDJResult { get; set; } = false;

        /// <summary>
        /// 上一次点检帧号
        /// </summary>
        public int LastCheckFrame = 0;

        /// <summary>
        /// 当前点检计数
        /// </summary>
        public int CueerntDJCount { get; set; } = 0;

        /// <summary>
        /// 失败累计计数
        /// </summary>
        public int FailAccumulativeCount { get; set; } = 0;


    }
}
