using DevMgmt.Cameras.SDK;
using FrameCommunity;
using FrameCommunity.DataOperation;

using FrameCommunity.错误代码;
using MvFGCtrlC.NET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace DevMgmt.Cameras.Models
{
    [Serializable]
    public class CamParam : BindableBase
    {
        public List<CameraDev> l_CameraDevs = new List<CameraDev>();


        /// <summary>
        /// 读取参数后执行初始化
        /// </summary>
        public override void Init()
        {
          
        }
    }

    [Serializable]
    public class CameraDev : BindableBase  // 👈 这里必须加！
    {
        [XmlIgnore]
        [JsonIgnore]
        public CameraObj Camera { get; set; }

        private CameraParameterBase _CameraParameter;
        public CameraParameterBase CameraParameter
        {
            get => _CameraParameter;
            set => SetProperty(ref _CameraParameter, value); // 👈 绑定用
        }


        // 核心布尔状态字段
        private bool _connStatus = false;
        [XmlIgnore]
        [JsonIgnore]
        public bool ConnStatus
        {
            get => _connStatus;
            set => SetProperty(ref _connStatus, value);
        }
    }


}
