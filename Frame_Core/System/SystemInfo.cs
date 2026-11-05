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
    public  class SystemInfo : FrameCommunity.DataOperation.JsonSerializationOperation
    {         
        /// <summary>
        /// 物料工艺类型
        /// </summary>
        private MaterialCrafEnum _CraftType = (MaterialCrafEnum)0;
        public MaterialCrafEnum CraftType
        {
            get => _CraftType;
            set { _CraftType = value; }
        }

        /// <summary>
        /// 工艺配方名称
        /// </summary>
        private string _RecipeName { get; set; } = "默认配方";
        public string RecipeName
        {
            get => _RecipeName;
            set { _RecipeName = value; }
        }

        /// <summary>
        /// 机台号
        /// </summary>
        private string _MachineNumber = "机台号";
        public string MachineNumber
        {
            get => _MachineNumber;
            set { _MachineNumber = value; }
        }


        /// <summary>
        /// 用户
        /// </summary>
        private string _EmployeeID = "14";
        public string EmployeeID
        {
            get => _EmployeeID;
            set { _EmployeeID = value; }
        }

        /// <summary>
        /// 当前语言类型
        /// </summary>
        private LanguageEnum _m_CurrentLanguageType = LanguageEnum.Chinese;
        public LanguageEnum m_CurrentLanguageType
        {
            get => _m_CurrentLanguageType;
            set { _m_CurrentLanguageType = value; }
        }

        /// <summary>
        /// 当前设备为主机设备
        /// </summary>
        private bool _IsHostMachine = true;
        public bool IsHostMachine
        {
            get => _IsHostMachine;
            set { _IsHostMachine = value; }
        }


        //四项生产基本信息---膜卷号，配方料号，生产型号，主设备标志位。
        /// <summary>
        /// 当前膜卷号
        /// </summary>
        private string _currentMojuanhao = "FV001";
        public string currentMojuanhao
        {
            get => _currentMojuanhao;
            set { _currentMojuanhao = value; }
        }




        /// <summary>
        /// 通讯工位选择
        /// </summary>
        public int ComStationsanwahl = (int)ComStationsanwahlEnum.上单面工位;

    }
}
