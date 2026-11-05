using FrameCommunity.语言;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMgmt.Light.Models
{
    // 设备型号枚举
    public enum LightControllerModelEnum
    {
        [UiDisplay("OPTv1")]
        OPTv1,

        [UiDisplay("OPTv2")]
        OPTv2,

        [UiDisplay("OPTv3")]
        OPTv3,

        [UiDisplay("OPTControllerAPI")]
        OPTControllerAPI,    

        [UiDisplay("乐视1（'$'）")]
        乐视1,

        [UiDisplay("乐视2（'#'）")]
        乐视2,

        [UiDisplay("乐视3（'&'）")]
        乐视3,

        [UiDisplay("康视达")]
        康视达,

        [UiDisplay("沃德普PSC3线光")]
        沃德普PSC3线光,

        [UiDisplay("沃德普PD5")]
        沃德普PD5,

        [UiDisplay("嘉励")]
        嘉励
    }

    // 波特率枚举
    public enum BaudRateEunm
    {
        [UiDisplay("9600")]
        B9600 = 9600,
        [UiDisplay("19200")]
        B19200 = 19200,
        [UiDisplay("38400")]
        B38400 = 38400,
        [UiDisplay("115200")]
        B115200 = 115200
    }

    // 校验位枚举
    public enum ParityEunm
    {
        [UiDisplay("无校验")]
        None = 0,
        [UiDisplay("奇校验")]
        Odd = 1,
        [UiDisplay("偶校验")]
        Even = 2
    }
}
