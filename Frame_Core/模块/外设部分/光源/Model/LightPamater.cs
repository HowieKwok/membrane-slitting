using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class LightPamater : XmlSerializationOperation
    {
        [Description("光源名称")]
        public string LightName { set; get; }
        [Description("光源类型")]
        public string LightType { set; get; }
        [Description("光源连接COM")]
        public int LightCOM { set; get; }
        [Description("光源亮度-通道1")]
        public int LightChannel1 { set; get; }
        [Description("光源亮度-通道2")]
        public int LightChannel2 { set; get; }

        [Description("光源亮度-通道3")]
        public int LightChannel3 { set; get; }
        [Description("光源亮度-通道4")]
        public int LightChannel4 { set; get; }

        [Description("光源亮度-通道5")]
        public int LightChannel5 { set; get; }
        [Description("光源亮度-通道6")]
        public int LightChannel6 { set; get; }

        [Description("光源亮度-通道7")]
        public int LightChannel7 { set; get; }
        [Description("光源亮度-通道8")]
        public int LightChannel8 { set; get; }



        [Description("光源亮度-通道1-OverhangLight")]
        public int[] LightChannel1valueGroup { set; get; }= new int[3];

        [Description("光源亮度-通道2-OverhangLight")]
        public int[] LightChannel2valueGroup { set; get; } = new int[3];

        [Description("控制切换")]
        public bool bIsControlChange = false;

        [Description("是否启用")]
        public bool bIsEnableOverhang = false;

        public LightPamater()
        {
            //Init();
        }
        public void Init()
        {

            LightName = "默认光源";
            LightType = "WDP（沃德普38400）";
            LightCOM = 1;
            LightChannel1 = 10;
            LightChannel2 = 10;


        }
    }
}
