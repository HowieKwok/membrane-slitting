using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class ConditionItem
    {
        private AndOrLogicEnum __Logic = AndOrLogicEnum.且;
        /// <summary>条件2内部逻辑1</summary>
        public AndOrLogicEnum Logic
        {
            get => __Logic;
            set { __Logic = value; }
        }

        private DefectDataSourceEnum _DataSource = DefectDataSourceEnum.高度;
        /// <summary>条件2数据源2</summary>
        public DefectDataSourceEnum DataSource
        {
            get => _DataSource;
            set { _DataSource = value; }
        }

        private double _Data = 0;
        /// <summary>条件2数据2阈值</summary>
        public double Data
        {
            get => _Data;
            set { _Data = value; }
        }
    }
    public class ListConditionItem
    {
        private List<ConditionItem> _ConditionItems = new List<ConditionItem>();
        /// <summary>条件</summary>
        public List<ConditionItem> ConditionItems
        {
            get => _ConditionItems;
            set { _ConditionItems = value; }
        }
      
    }
}
