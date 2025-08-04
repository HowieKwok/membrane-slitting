using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{

    /// <summary>
    /// 相机图片数据
    /// </summary>
    public class CamImageData
    {
        /// <summary>
        /// 相机名称
        /// </summary>
        public string CamName = "";

        /// <summary>
        /// 片索引（显示名称）
        /// </summary>
        public string SliceIndex = "";

        /// <summary>
        /// 结果
        /// </summary>
        public bool result = false; 

        /// <summary>
        /// 原图路径
        /// </summary>
        public string OriginalImage_Path = "";
        /// <summary>
        /// 截图路径
        /// </summary>
        public string DempImage_Path = "";
        /// <summary>
        /// 瑕疵图片1路径
        /// </summary>
        public string DefectImage1_Path = "";
        /// <summary>
        /// 瑕疵图片2路径
        /// </summary>
        public string DefectImage2_Path = "";
        /// <summary>
        /// 瑕疵图片3路径
        /// </summary>
        public string DefectImage3_Path = "";
    }
}
