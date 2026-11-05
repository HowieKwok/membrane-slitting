using FrameCommunity.DataOperation;
using NPOI.HSSF.Record;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class CraftBase : BusinessParamBase
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
        /// 机台号
        /// </summary>
        private string _MachineNumber = "机台号";
        public string MachineNumber
        {
            get => _MachineNumber;
            set { _MachineNumber = value; }
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
        /// 产品极性
        /// </summary>
        private ElectrodeEnum _Electrode = (ElectrodeEnum)0;
        public ElectrodeEnum Electrode
        {
            get => _Electrode;
            set { _Electrode = value; }
        }

        private GlobalVariable _M_GlobalVariable = new GlobalVariable();
        public GlobalVariable M_GlobalVariable
        {
            get => _M_GlobalVariable;
            set { _M_GlobalVariable = value; }
        }
    }
}
