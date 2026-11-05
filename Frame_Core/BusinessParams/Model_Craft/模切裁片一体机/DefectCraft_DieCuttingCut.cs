using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class DefectCraft_DieCuttingCut
    {
        #region Basic Info 基础信息
        private int _id = 0;
        /// <summary>主键ID</summary>
        public int Id
        {
            get => _id;
            set { _id = value; }
        }

        public int DefectLevel => Id;

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
        /// <summary>检测项名称</summary>
        public string Describe
        {
            get => _Describe;
            set { _Describe = value; }
        }
        #endregion

        #region Formula & DataSource 计算公式与数据源
        #region 条件1
        private bool _condition1_En = true;
        /// <summary>条件1使能</summary>
        public bool Condition1_En
        {
            get => _condition1_En;
            set { _condition1_En = value; }
        }

        private int _condition1_MembraneNum = 100;
        /// <summary>膜区数量</summary>
        public int Condition1_MembraneNum
        {
            get => _condition1_MembraneNum;
            set { _condition1_MembraneNum = value; }
        }
        #endregion

        #region 条件2
        private AndOrLogicEnum _condition1AndCondition2_Logic = AndOrLogicEnum.或;
        /// <summary>条件1和条件2逻辑</summary>
        public AndOrLogicEnum Condition1AndCondition2_Logic
        {
            get => _condition1AndCondition2_Logic;
            set { _condition1AndCondition2_Logic = value; }
        }

        private bool _condition2_En = true;
        /// <summary>条件2使能</summary>
        public bool Condition2_En
        {
            get => _condition2_En;
            set { _condition2_En = value; }
        }

        private DefectDataSourceEnum _condition2_DataSource1 = DefectDataSourceEnum.面积;
        /// <summary>条件2数据源1</summary>
        public DefectDataSourceEnum Condition2_DataSource1
        {
            get => _condition2_DataSource1;
            set { _condition2_DataSource1 = value; }
        }

        private double _condition2_Data1 = 0;
        /// <summary>条件2数据1阈值</summary>
        public double Condition2_Data1
        {
            get => _condition2_Data1;
            set { _condition2_Data1 = value; }
        }

        private AndOrLogicEnum _condition2_Logic1 = AndOrLogicEnum.且;
        /// <summary>条件2内部逻辑1</summary>
        public AndOrLogicEnum Condition2_Logic1
        {
            get => _condition2_Logic1;
            set { _condition2_Logic1 = value; }
        }

        private DefectDataSourceEnum _condition2_DataSource2 = DefectDataSourceEnum.高度;
        /// <summary>条件2数据源2</summary>
        public DefectDataSourceEnum Condition2_DataSource2
        {
            get => _condition2_DataSource2;
            set { _condition2_DataSource2 = value; }
        }

        private double _condition2_Data2 = 0;
        /// <summary>条件2数据2阈值</summary>
        public double Condition2_Data2
        {
            get => _condition2_Data2;
            set { _condition2_Data2 = value; }
        }

        private AndOrLogicEnum _condition2_Logic2 = AndOrLogicEnum.且;
        /// <summary>条件2内部逻辑2</summary>
        public AndOrLogicEnum Condition2_Logic2
        {
            get => _condition2_Logic2;
            set { _condition2_Logic2 = value; }
        }

        private DefectDataSourceEnum _condition2_DataSource3 = DefectDataSourceEnum.宽度;
        /// <summary>条件2数据源3</summary>
        public DefectDataSourceEnum Condition2_DataSource3
        {
            get => _condition2_DataSource3;
            set { _condition2_DataSource3 = value; }
        }

        private double _condition2_Data3 = 0;
        /// <summary>条件2数据3阈值</summary>
        public double Condition2_Data3
        {
            get => _condition2_Data3;
            set { _condition2_Data3 = value; }
        }
        #endregion

        #region 条件3
        private AndOrLogicEnum _condition2AndCondition3_Logic = AndOrLogicEnum.且;
        /// <summary>条件2和条件3逻辑</summary>
        public AndOrLogicEnum Condition2AndCondition3_Logic
        {
            get => _condition2AndCondition3_Logic;
            set { _condition2AndCondition3_Logic = value; }
        }

        private bool _condition3_En = false;
        /// <summary>条件3使能</summary>
        public bool Condition3_En
        {
            get => _condition3_En;
            set { _condition3_En = value; }
        }

        private DefectDataSourceEnum _condition3_DataSource1 = DefectDataSourceEnum.面积;
        /// <summary>条件3数据源1</summary>
        public DefectDataSourceEnum Condition3_DataSource1
        {
            get => _condition3_DataSource1;
            set { _condition3_DataSource1 = value; }
        }

        private double _condition3_Data1 = 0;
        /// <summary>条件3数据1阈值</summary>
        public double Condition3_Data1
        {
            get => _condition3_Data1;
            set { _condition3_Data1 = value; }
        }

        private AndOrLogicEnum _condition3_Logic1 = AndOrLogicEnum.且;
        /// <summary>条件3内部逻辑1</summary>
        public AndOrLogicEnum Condition3_Logic1
        {
            get => _condition3_Logic1;
            set { _condition3_Logic1 = value; }
        }

        private DefectDataSourceEnum _condition3_DataSource2 = DefectDataSourceEnum.高度;
        /// <summary>条件3数据源2</summary>
        public DefectDataSourceEnum Condition3_DataSource2
        {
            get => _condition3_DataSource2;
            set { _condition3_DataSource2 = value; }
        }

        private double _condition3_Data2 = 0;
        /// <summary>条件3数据2阈值</summary>
        public double Condition3_Data2
        {
            get => _condition3_Data2;
            set { _condition3_Data2 = value; }
        }
        #endregion

        #region 条件4
        private AndOrLogicEnum _condition3AndCondition4_Logic = AndOrLogicEnum.且;
        /// <summary>条件3和条件4逻辑</summary>
        public AndOrLogicEnum Condition3AndCondition4_Logic
        {
            get => _condition3AndCondition4_Logic;
            set { _condition3AndCondition4_Logic = value; }
        }

        private bool _condition4_En = false;
        /// <summary>条件4使能</summary>
        public bool Condition4_En
        {
            get => _condition4_En;
            set { _condition4_En = value; }
        }

        private DefectDataSourceEnum _condition4_DataSource1 = DefectDataSourceEnum.面积;
        /// <summary>条件4数据源1</summary>
        public DefectDataSourceEnum Condition4_DataSource1
        {
            get => _condition4_DataSource1;
            set { _condition4_DataSource1 = value; }
        }

        private double _condition4_Data1 = 0;
        /// <summary>条件4数据1阈值</summary>
        public double Condition4_Data1
        {
            get => _condition4_Data1;
            set { _condition4_Data1 = value; }
        }

        private AndOrLogicEnum _condition4_Logic1 = AndOrLogicEnum.且;
        /// <summary>条件4内部逻辑1</summary>
        public AndOrLogicEnum Condition4_Logic1
        {
            get => _condition4_Logic1;
            set { _condition4_Logic1 = value; }
        }

        private DefectDataSourceEnum _condition4_DataSource2 = DefectDataSourceEnum.高度;
        /// <summary>条件4数据源2</summary>
        public DefectDataSourceEnum Condition4_DataSource2
        {
            get => _condition4_DataSource2;
            set { _condition4_DataSource2 = value; }
        }

        private double _condition4_Data2 = 0;
        /// <summary>条件4数据2阈值</summary>
        public double Condition4_Data2
        {
            get => _condition4_Data2;
            set { _condition4_Data2 = value; }
        }
        #endregion

        #region 有无检测
        private bool _HaveOrNot = false;
        /// <summary>有无检测</summary>
        public bool HaveOrNot
        {
            get => _HaveOrNot;
            set { _HaveOrNot = value; }
        }
        #endregion
        #endregion

        #region DB 打标配置
        private bool _IsSevere = false;
        /// <summary>是否重度</summary>
        public bool IsSevere
        {
            get => _IsSevere;
            set { _IsSevere = value; }
        }
        #endregion

        #region DB 打标配置
        private bool _db = false;
        /// <summary>是否打标</summary>
        public bool DB
        {
            get => _db;
            set { _db = value; }
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
    }

    public static class DefectCraftExtension
    {
        public static string GetConditionalFormat(this DefectCraft_DieCuttingCut defectCraft)
        {
            string conditionalFormat = defectCraft.HaveOrNot ? "有无检测" : "";
            if (defectCraft.HaveOrNot) return conditionalFormat;
            if (defectCraft.Condition1_En)
            {
                conditionalFormat = $"(缺陷数量>{defectCraft.Condition1_MembraneNum})";
            }
            if (defectCraft.Condition2_En)
            {
                conditionalFormat += $" {defectCraft.Condition1AndCondition2_Logic} ";
                conditionalFormat += $"(";
                conditionalFormat += $"{defectCraft.Condition2_DataSource1}>{defectCraft.Condition2_Data1}";
                conditionalFormat += $" {defectCraft.Condition2_Logic1} ";
                conditionalFormat += $"{defectCraft.Condition2_DataSource2}>{defectCraft.Condition2_Data2}";
                conditionalFormat += $" {defectCraft.Condition2_Logic2} ";
                conditionalFormat += $"{defectCraft.Condition2_DataSource3}>{defectCraft.Condition2_Data3}";
                conditionalFormat += $")";
            }
            if (defectCraft.Condition3_En)
            {
                conditionalFormat += $" {defectCraft.Condition2AndCondition3_Logic} ";
                conditionalFormat += $"(";
                conditionalFormat += $"{defectCraft.Condition3_DataSource1}>{defectCraft.Condition3_Data1}";
                conditionalFormat += $" {defectCraft.Condition3_Logic1} ";
                conditionalFormat += $"{defectCraft.Condition3_DataSource2}>{defectCraft.Condition3_Data2}";
                conditionalFormat += $")";
            }
            if (defectCraft.Condition4_En)
            {
                conditionalFormat += $" {defectCraft.Condition3AndCondition4_Logic} ";
                conditionalFormat += $"(";
                conditionalFormat += $"{defectCraft.Condition4_DataSource1}>{defectCraft.Condition4_Data1}";
                conditionalFormat += $" {defectCraft.Condition4_Logic1} ";
                conditionalFormat += $"{defectCraft.Condition4_DataSource2}>{defectCraft.Condition4_Data2}";
                conditionalFormat += $")";
            }
            return conditionalFormat;
        }
    }

}
