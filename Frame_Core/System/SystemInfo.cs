using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public  class SystemInfo : XmlSerializationOperation
    {
        //四项生产基本信息---膜卷号，配方料号，生产型号，主设备标志位。
        /// <summary>
        /// 当前膜卷号
        /// </summary>
        public string currentMojuanhao = "FV001";
        /// <summary>
        /// 当前配方料号
        /// </summary>
        public string CurrentCraft = "新建配方";
        /// <summary>
        /// 当前生产类型
        /// </summary>
        public MaterialCrafEnum CurrentMaterial = MaterialCrafEnum.PFT清洗机;
        /// <summary>
        /// 当前设备为主机设备
        /// </summary>
        public bool IsHostMachine = true;

        /// <summary>
        /// 当前语言类型
        /// </summary>
        public LanguageEnum m_CurrentLanguageType = LanguageEnum.Chinese;

        /// <summary>
        /// 机台编号
        /// </summary>
        public string MachineID { get; set; } = "14";

        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmployeeID { get; set; } = "14";


        /// <summary>
        /// 通讯工位选择
        /// </summary>
        public int ComStationsanwahl = (int)ComStationsanwahlEnum.上单面工位;

        //生产清零
        public bool ClearProdct_PerRoll { get; set; } = true;  //每卷
        public bool ClearProdct_PerShift { get; set; } = false; //每班次，换班清零

        [Description("点检早班时间具体到分钟")]
        public DateTime djCaliTimeBaiban  = new DateTime(2018, 11, 9, 8, 0, 0);

        [Description("点检晚班时间具体到分钟")]
        public DateTime djCaliTimeYeBan = new DateTime(2018, 11, 9, 20, 0, 0);

        [Description("上一次点检OK的时间")]
        public DateTime djLastOKTime  = new DateTime(2018, 11, 9, 19, 30, 0);

        [Description("点检使能")]
        public bool djEnabled  = true;

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
