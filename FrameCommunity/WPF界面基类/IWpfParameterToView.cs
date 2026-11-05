
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FrameCommunity
{
   public interface  IWpfParameterToView
    {
     
        Dictionary<string, ControlDisplayBase> ControlDisplay { get; set; }   
        /// <summary>
        /// 控件参数显示配置初始化
        /// </summary>
        void InitDisplayConfig();
        /// <summary>
        /// 控件是否显示初始化
        /// </summary>
        void InitControlDisplay();
        /// <summary>
        /// 控件是否启用初始化
        /// </summary>
        void InitIsEnabled();
    }

   
}
