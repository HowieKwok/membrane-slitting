using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using HalconDotNet;

namespace Plugin.CamerSDK
{
    [Serializable]
   public  class ParamsCamerSDK :PluginParamsBase
    {
        /// <summary>
        /// 在线加图模式
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public  List<ImageInfo> ImageList_OnLine = new List<ImageInfo>();
        /// <summary>
        /// 离线仿真模式
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<ImageInfo> ImageList_OffLine = new List<ImageInfo>();


        public string str_CamName ="初始相机";

        public bool Is_Online = true;

    }
}
