using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYSD.UICore.Control;

namespace Frame_Core
{
    [Serializable]
    /// <summary>
    /// 磁盘参数
    /// </summary>
    public class DriveParameter : XmlSerializationOperation
    {
        [ShowTextAttribute("是否启用分盘存储")]
        [Description("是否启用分盘存储")]
        public bool IsEnable { set; get; }

        [Description("当前盘符")]
        public string driveName { set; get; } = AppDomain.CurrentDomain.BaseDirectory;

        [Description("设定盘符容量上限")]
        public int DriveCapacityLimit { set; get; }=90;

        [ShowTextAttribute("是否报警停机")]
        [Description("是否报警停机")]
        public bool IsAlarm { set; get; }=false;

        [Description("是否已显示弹窗")]
        public bool  IsShowedWarning { set; get; } = false;

    }
}
