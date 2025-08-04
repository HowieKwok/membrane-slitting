using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Frame_Core
{
    /// <summary>
    /// 图片保存参数
    /// </summary>
    [Serializable]
    public class ImageSaveParamer : XmlSerializationOperation
    {

        /// <summary>
        /// 原图保存路径
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public string ImageSave_Path_Primitive = "E:\\原图保存路径\\";

        /// <summary>
        /// 截图保存路径
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public string ImageSave_Path_Screenshot = "E:\\截图保存路径\\";

        /// <summary>
        /// 小图保存路径
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public string ImageSave_Path_Min = "E:\\小图保存路径\\";

        /// <summary>
        /// 产品图保存路径
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public string ImageSave_Path_ProdImg = "E:\\产品图保存路径\\";

        /// <summary>
        /// 产品截图保存路径
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public string ImageSave_Path_ProdScreenshot = "E:\\截图保存路径\\";

        /// <summary>
        /// 图片保存格式-原图
        /// </summary>
        [Description("图片保存格式-原图")]
        public int SaveImageFormat_Primitive = (int)ImageSaveFormat.JPEG20;

        /// <summary>
        /// 图片保存格式-原图
        /// </summary>
        [Description("3D深度图片保存格式-原图")]
        public int SaveImageFormat_Deepth = (int)ImageDeepthSaveFormat.TIF;

        /// <summary>
        /// 图片保存选择-原图
        /// </summary>
        [Description("图片保存选择-原图")]
        public int SaveImageSelect_Primitive = (int)ImageSaveSelect.保存所有;

        /// <summary>
        /// 显示图像-原图
        /// </summary>
        [Description("显示图像-原图")]
        public bool IsShowImage_Primitive = true;

        /// <summary>
        /// 深度图片是否压缩
        /// </summary>
        [Description("深度图片是否压缩")]
        public bool IsCompression_Deepth = true;

        /// <summary>
        /// 图片保存格式-截图
        /// </summary>
        [Description("图片保存格式-截图")]
        public int SaveImageFormat_Screenshot = (int)ImageSaveFormat.JPEG20;

        /// <summary>
        /// 图片保存选择-截图
        /// </summary>
        [Description("图片保存选择-截图")]
        public int SaveImageSelect_Screenshot = (int)ImageSaveSelect.保存所有;

        /// <summary>
        /// 高清晰度-截图
        /// </summary>
        [Description("高清晰度-截图")]
        public bool HighDefinition_Screenshot = false;

        /// <summary>
        /// 高清晰度-产品截图
        /// </summary>
        [Description("高清晰度-产品截图")]
        public bool HighDefinition_ProScreenshot = false;
        /// <summary>
        /// 图片保存格式-小图
        /// </summary>
        [Description("图片保存格式-小图")]
        public int SaveImageFormat_Min = (int)ImageSaveFormat.JPEG20;

        /// <summary>
        /// 保存-小图
        /// </summary>
        [Description("保存-小图")]
        public bool IsSaveImage_Min = false;

        /// <summary>
        /// 显示图像-小图
        /// </summary>
        [Description("显示图像-小图")]
        public bool IsShowImage_Min = true;


        /// <summary>
        /// 图片保存格式-产品图
        /// </summary>
        [Description("图片保存格式-产品图")]
        public int SaveImageFormat_ProdImg = (int)ImageSaveFormat.JPEG20;

        /// <summary>
        /// 图片保存选择-产品图
        /// </summary>
        [Description("图片保存选择-产品图")]
        public int SaveImageSelect_ProdImg = (int)ImageSaveSelect.保存所有;

        /// <summary>
        /// 图片保存格式-产品截图
        /// </summary>
        [Description("图片保存格式-产品截图")]
        public int SaveImageFormat_ProdScreenshot = (int)ImageSaveFormat.JPEG20;

        /// <summary>
        /// 图片保存选择-产品截图
        /// </summary>
        [Description("图片保存选择-产品截图")]
        public int SaveImageSelect_ProdScreenshot = (int)ImageSaveSelect.保存所有;

        /// <summary>
        /// 高清晰度-产品截图
        /// </summary>
        [Description("高清晰度-产品截图")]
        public bool HighDefinition_ProdScreenshot = false;

        /// <summary>
        /// 调试模式，0默认，1调试
        /// </summary>
        [Description("调试模式，0默认，1调试")]
        public int IsDebug = 0;

        /// <summary>
        /// 每小时保存N张图片使能
        /// </summary>
        [Description("每小时保存N张图片使能")]
        public bool EnHourSaveImage = false;

        /// <summary>
        /// 每小时保存N张图片数量
        /// </summary>
        [Description("每小时保存N张图片数量")]
        public int HourSaveImageNum = 0;


        /// <summary>
        /// N张图像保存使能
        /// </summary>
        [Description("N张图像保存使能")]
        public bool EnNSaveImage = false;

        /// <summary>
        /// N张图像保存一次
        /// </summary>
        [Description("N张图像保存一次")]
        public int NSaveImageNum = 0;
    }
}
