using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class Param_PointInspection : FrameCommunity.DataOperation.JsonSerializationOperation
    {
        //生产清零
        public bool ClearProdct_PerRoll { get; set; } = true;  //每卷
        public bool ClearProdct_PerShift { get; set; } = false; //每班次，换班清零

        [Description("点检早班时间具体到分钟")]
        public DateTime djCaliTimeBaiban = new DateTime(2018, 11, 9, 8, 0, 0);

        [Description("点检晚班时间具体到分钟")]
        public DateTime djCaliTimeYeBan = new DateTime(2018, 11, 9, 20, 0, 0);

        [Description("上一次点检OK的时间")]
        public DateTime djLastOKTime = new DateTime(2018, 11, 9, 19, 30, 0);

        [Description("点检使能")]
        public bool djEnabled = true;

        /// <summary>
        /// 点检完成
        /// </summary>
        public bool djFinish { get; set; } = false;

        /// <summary>
        /// 初始化完成使能
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool InitFinishEN = false;
    }
}
