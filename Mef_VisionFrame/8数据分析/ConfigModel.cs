using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class ConfigModel
    {
        /// <summary>
        /// 默认打开目录
        /// </summary>
        public string RootPath { get; set; }
        /// <summary>
        /// 主窗口位置
        /// </summary>
        public  Point MfromStartPos { get; set; }
        /// <summary>
        /// 图像展示窗口位置
        /// </summary>
        public Point StartPos { get; set; }
        /// <summary>
        /// 图像展示窗口大小
        /// </summary>
        public Size Size { get; set; }
        /// <summary>
        /// 是否最大化
        /// </summary>
        public bool Maximizd { get; set; }
    }
}
