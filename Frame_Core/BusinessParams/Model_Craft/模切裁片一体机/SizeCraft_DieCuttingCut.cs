using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class SizeCraft_DieCuttingCut
    {
        #region 基础信息
        private int _id = 0;
        /// <summary>ID</summary>
        public int Id
        {
            get => _id;
            set { _id = value; }
        }

        private bool _Enable = true;
        /// <summary>启用</summary>
        public bool Enable
        {
            get => _Enable;
            set { _Enable = value; }
        }

        private string _name = "";
        /// <summary>检测项名称</summary>
        public string Name
        {
            get => _name;
            set { _name = value; }
        }

        private string _Describe = "";
        /// <summary>描述</summary>
        public string Describe
        {
            get => _Describe;
            set { _Describe = value; }
        }

        private double _calibrateCoeff = 1;
        /// <summary>校准系数</summary>
        public double CalibrateCoeff
        {
            get => _calibrateCoeff;
            set { _calibrateCoeff = value; }
        }

        private double _measureOffset = 0;
        /// <summary>测量偏移</summary>
        public double MeasureOffset
        {
            get => _measureOffset;
            set { _measureOffset = value; }
        }
        #endregion

        #region 计算公式
        private int _calcFormula = 0;
        /// <summary>计算公式</summary>
        public int CalcFormula
        {
            get => _calcFormula;
            set { _calcFormula = value; }
        }

        private string _dataSource1 = "";
        /// <summary>数据源1</summary>
        public string DataSource1
        {
            get => _dataSource1;
            set { _dataSource1 = value; }
        }

        private string _dataSource2 = "";
        /// <summary>数据源2</summary>
        public string DataSource2
        {
            get => _dataSource2;
            set { _dataSource2 = value; }
        }
        #endregion

        #region 打标
        private double _standardValue = 0;
        /// <summary>标准值</summary>
        public double StandardValue
        {
            get => _standardValue;
            set { _standardValue = value; }
        }

        private double _DBTolLower = 0;
        /// <summary>打标下公差</summary>
        public double DBTolLower
        {
            get => _DBTolLower;
            set { _DBTolLower = value; }
        }

        private double _DBTolUpper = 0;
        /// <summary>打标上公差</summary>
        public double DBTolUpper
        {
            get => _DBTolUpper;
            set { _DBTolUpper = value; }
        }

        private int _DBContinuous = 0;
        /// <summary>打标连续阈值</summary>
        public int DBContinuous
        {
            get => _DBContinuous;
            set { _DBContinuous = value; }
        }


        private bool _DBEnable = false;
        /// <summary>启用打标</summary>
        public bool DBEnable
        {
            get => _DBEnable;
            set { _DBEnable = value; }
        }
        #endregion

        #region BJ 报警配置
        private int _ContinuousCount = 0;
        /// <summary>连续超限次数</summary>
        public int ContinuousCount
        {
            get => _ContinuousCount;
            set { _ContinuousCount = value; }
        }


        private bool _BJ_Continuous = false;
        /// <summary>是否报警</summary>
        public bool BJ_Continuous
        {
            get => _BJ_Continuous;
            set { _BJ_Continuous = value; }
        }

        private bool _TJ_Continuous = false;
        /// <summary>是否报警</summary>
        public bool TJ_Continuous
        {
            get => _TJ_Continuous;
            set { _TJ_Continuous = value; }
        }
        #endregion

        #region TJ 停机配置

        private int _AccumulateCount = 0;
        /// <summary>累计超限次数</summary>
        public int AccumulateCount
        {
            get => _AccumulateCount;
            set { _AccumulateCount = value; }
        }

        private bool _BJ_Accumulate = false;
        /// <summary>是否报警</summary>
        public bool BJ_Accumulate
        {
            get => _BJ_Accumulate;
            set { _BJ_Accumulate = value; }
        }

        private bool _TJ_Accumulate = false;
        /// <summary>是否报警</summary>
        public bool TJ_Accumulate
        {
            get => _TJ_Accumulate;
            set { _TJ_Accumulate = value; }
        }
        #endregion


        #region 数据模型
        private bool _enableDataModel = false;
        /// <summary>是否启用数据模型</summary>
        public bool EnableDataModel
        {
            get => _enableDataModel;
            set { _enableDataModel = value; }
        }

        private double _compensateF = 0;
        /// <summary>补偿值/F</summary>
        public double CompensateF
        {
            get => _compensateF;
            set { _compensateF = value; }
        }

        private double _compensateO = 0;
        /// <summary>补偿值/O</summary>
        public double CompensateO
        {
            get => _compensateO;
            set { _compensateO = value; }
        }
        #endregion

    }
}
