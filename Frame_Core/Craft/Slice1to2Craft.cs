using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class Slice1to2Craft : CraftBase
    {

        public Slice1to2GeneralCarftInfo GeneralCarft;
        //分相机工艺参数
        public Dictionary<CameraEnum, Slice1to2CamerCraftInfo> CamerCraft;

        public override void Init()
        {
            CraftName = "分条机1出2";
            BaseCamera = CameraEnum.Cam1;
            electrode = ElectrodeEnum.阴极;
            materialCrafEnum = MaterialCrafEnum.分条机1出2;

            GeneralCarft = new Slice1to2GeneralCarftInfo();
            CamerCraft = new Dictionary<CameraEnum, Slice1to2CamerCraftInfo>();

            foreach (CameraEnum item in Enum.GetValues(typeof(CameraEnum)))
            {
                CamerCraft.Add(item, new Slice1to2CamerCraftInfo());

            }
        }
    }

    [Serializable]
    public class Slice1to2GeneralCarftInfo
    {
        // 产品尺寸工艺  

        /// <summary>
        /// EA单片尺寸_X(垂直运动方向)
        /// </summary>
        [Description("EA单片尺寸_X")]
        public double EASizeX = 300;
        /// <summary>
        /// 涂层宽度_X(垂直运动方向)
        /// </summary>
        [Description("涂层宽度_X")]
        public double CoatSizeX = 300;

        /// <summary>
        /// AT尺寸_X (垂直运动方向)
        /// </summary>
        [Description("AT尺寸_X")]
        public double ATSizeX = 20;

        /// <summary>
        /// 极耳尺寸_X(垂直运动方向)
        /// </summary>
        [Description("极耳尺寸_X")]
        public double JRSizeX = 100;

    }

    [Serializable]
    public class Slice1to2CamerCraftInfo
    {
        /// <summary>
        /// 隐藏未启用项
        /// </summary>
        [Description("隐藏未启用项")]
        public bool IsShowAllItems = false;

        /// <summary>
        /// 启用所有尺寸检测项
        /// </summary>
        [Description("启用所有尺寸检测项")]
        public bool IsEnableAllSize = true;

        /// <summary>
        /// 启用所有缺陷检测项
        /// </summary>
        [Description("启用所有缺陷检测项")]
        public bool IsEnableAllDefect = true;

        /// <summary>
        /// 极耳在图像左侧
        /// </summary>
        [Description("极耳在图像左侧")]
        public bool IsLeft = true;

        /// <summary>
        /// 相机测基准边
        /// </summary>
        [Description("相机测基准边")]
        public double BaseSideX = 200;

        /// <summary>
        /// 相机平均灰度
        /// </summary>
        [Description("相机平均灰度")]
        public double GrayValue = 128;


        /// <summary>
        /// 分条一出2项目尺寸检测项
        /// </summary>
        [Description("分条一出2项目尺寸检测项")]
        public List<SizeItemInfo> SizeInfoList = new List<SizeItemInfo>();
        /// <summary>
        /// 分条一出2项目缺陷检测项
        /// </summary>
        [Description("分条一出2项目缺陷检测项")]
        public List<DefectItemInfo> DefectInfoList = new List<DefectItemInfo>();

        /// <summary>
        /// 相机抓边队列
        /// </summary>
        [Description("相机抓边队列")]
        public List<LineInfo> ListLines = new List<LineInfo>();
        /// <summary>
        /// 相机检测项编辑队列
        /// </summary>
        [Description("相机检测项编辑队列")]
        public List<SizeItemEditor> ListsizeItemEditors = new List<SizeItemEditor>();

    }


}
