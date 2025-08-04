using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using HalconDotNet;

namespace Frame_Core
{
    [Serializable]
    public class ParamsCropImage : PluginParamsBase
    {
        //输入变量
        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;
        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_CropImage_Lastbottom;

        /// <summary>
        /// 输入变量：起始列
        /// </summary>
        public double colBegin = 200;

        /// <summary>
        /// 输入变量：裁图宽
        /// </summary>
        public double cropWidth = 300;

        /// <summary>
        /// 输入变量：裁图高
        /// </summary>
        public double cropHeight = 300;

        #region 2.输出变量
        /// <summary>
        /// 输出变量：处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_CropIamge_top;

        /// <summary>
        /// 输出变量：处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_CropIamge_bottom;

        /// <summary>
        /// 输出变量：处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_TiledIamge;
        #endregion

        public ParamsCropImage()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_CropImage_Lastbottom);

            HOperatorSet.GenEmptyObj(out ho_CropIamge_top);
            HOperatorSet.GenEmptyObj(out ho_CropIamge_bottom);
            HOperatorSet.GenEmptyObj(out ho_TiledIamge);
        }
    }
}
