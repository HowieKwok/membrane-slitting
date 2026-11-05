using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Frame_Core;

namespace Mef_VisionFrame
{
    public partial class Form_DefectRules : Form
    {
        enum ModelEnum { 编辑, 添加 }

        ModelEnum currentModel = ModelEnum.编辑;
        List<DefectCraft_DieCuttingCut> defectCrafts = null;
        DefectCraft_DieCuttingCut currentDefectCraft = null;
        public Form_DefectRules(List<DefectCraft_DieCuttingCut> _defectCrafts, int index)
        {
            InitializeComponent();
            Init();
            defectCrafts = _defectCrafts;
            currentModel = ModelEnum.编辑;
            input_detectionItem.Enabled = false;
            if (defectCrafts != null)
            {
                // 提取所有名称，过滤空值
                string[] names = defectCrafts.Select(x => x.Name).ToArray();
                select_detectionItem.Items.Clear();
                select_detectionItem.Items.AddRange(names);

                if (index >= 0 && _defectCrafts.Count > index) select_detectionItem.SelectedIndex = index;
            }
        }


        public Form_DefectRules(DefectCraft_DieCuttingCut _currentDefectCraft)
        {
            InitializeComponent();
            Init();
            currentDefectCraft = _currentDefectCraft;
            currentModel = ModelEnum.添加;
            select_detectionItem.Enabled = false;

            ParamToView(currentDefectCraft);
        }

        private void Form_DefectRules_Load(object sender, EventArgs e)
        {
            btnOk.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
        }
        private void Form_DefectRules_FormClosing(object sender, FormClosingEventArgs e)
        {

            // 点击确定按钮且校验不通过，取消关闭
            if (this.DialogResult == DialogResult.OK)
            {
                if (currentModel == ModelEnum.添加)
                {
                    if (string.IsNullOrEmpty(input_detectionItem.Text.Trim()))
                    {
                        MessageBox.Show("检测项名称不能为空");
                        e.Cancel = true;
                        // 重置窗体结果，避免残留OK状态
                        this.DialogResult = DialogResult.None;
                    }
                }
                else if (currentModel == ModelEnum.编辑)
                {
                    e.Cancel = true;
                    // 重置窗体结果，避免残留OK状态
                    this.DialogResult = DialogResult.None;
                }
            }
        }
        private void Init()
        {
            Condition1AndCondition2_Logic.Items.Clear();
            string[] AndOrLogic = Enum.GetNames(typeof(AndOrLogicEnum));
            string[] DefectDataSource = Enum.GetNames(typeof(DefectDataSourceEnum));
            Condition1AndCondition2_Logic.Items.AddRange(AndOrLogic);
            Condition2AndCondition3_Logic.Items.AddRange(AndOrLogic);
            Condition3AndCondition4_Logic.Items.AddRange(AndOrLogic);
            Condition2_Logic1.Items.AddRange(AndOrLogic);
            Condition2_Logic2.Items.AddRange(AndOrLogic);
            Condition3_Logic1.Items.AddRange(AndOrLogic);
            Condition4_Logic1.Items.AddRange(AndOrLogic);

            Condition2_DataSource1.Items.AddRange(DefectDataSource);
            Condition2_DataSource2.Items.AddRange(DefectDataSource);
            Condition2_DataSource3.Items.AddRange(DefectDataSource);
            Condition3_DataSource1.Items.AddRange(DefectDataSource);
            Condition3_DataSource2.Items.AddRange(DefectDataSource);
            Condition4_DataSource1.Items.AddRange(DefectDataSource);
            Condition4_DataSource2.Items.AddRange(DefectDataSource);
        }


        private void ParamToView(DefectCraft_DieCuttingCut defectCraft)
        {
            if (defectCraft == null) return;
            input_detectionItem.Text = defectCraft.Name;
            Condition1_En.Checked = defectCraft.Condition1_En;
            Condition1_MembraneNum.Value = defectCraft.Condition1_MembraneNum;

            Condition1AndCondition2_Logic.SelectedIndex = (int)defectCraft.Condition1AndCondition2_Logic;
            Condition2_En.Checked = defectCraft.Condition2_En;
            Condition2_DataSource1.SelectedIndex = (int)defectCraft.Condition2_DataSource1;
            Condition2_Data1.Value = (decimal)defectCraft.Condition2_Data1;
            Condition2_Logic1.SelectedIndex = (int)defectCraft.Condition2_Logic1;
            Condition2_DataSource2.SelectedIndex = (int)defectCraft.Condition2_DataSource2;
            Condition2_Data2.Value = (decimal)defectCraft.Condition2_Data2;
            Condition2_Logic2.SelectedIndex = (int)defectCraft.Condition2_Logic2;
            Condition2_DataSource3.SelectedIndex = (int)defectCraft.Condition2_DataSource3;
            Condition2_Data3.Value = (decimal)defectCraft.Condition2_Data3;

            Condition2AndCondition3_Logic.SelectedIndex = (int)defectCraft.Condition2AndCondition3_Logic;
            Condition3_En.Checked = defectCraft.Condition3_En;
            Condition3_DataSource1.SelectedIndex = (int)defectCraft.Condition3_DataSource1;
            Condition3_Data1.Value = (decimal)defectCraft.Condition3_Data1;
            Condition3_Logic1.SelectedIndex = (int)defectCraft.Condition3_Logic1;
            Condition3_DataSource2.SelectedIndex = (int)defectCraft.Condition3_DataSource2;
            Condition3_Data2.Value = (decimal)defectCraft.Condition3_Data2;

            Condition3AndCondition4_Logic.SelectedIndex = (int)defectCraft.Condition3AndCondition4_Logic;
            Condition4_En.Checked = defectCraft.Condition4_En;
            Condition4_DataSource1.SelectedIndex = (int)defectCraft.Condition4_DataSource1;
            Condition4_Data1.Value = (decimal)defectCraft.Condition4_Data1;
            Condition4_Logic1.SelectedIndex = (int)defectCraft.Condition4_Logic1;
            Condition4_DataSource2.SelectedIndex = (int)defectCraft.Condition4_DataSource2;
            Condition4_Data2.Value = (decimal)defectCraft.Condition4_Data2;

            cbx_HaveOrNot.Checked = defectCraft.HaveOrNot;
        }

        private void ViewToParam(DefectCraft_DieCuttingCut defectCraft)
        {
            if (defectCraft == null) return;
            defectCraft.Name = input_detectionItem.Text;
            defectCraft.Condition1_En = Condition1_En.Checked;
            defectCraft.Condition1_MembraneNum = (int)Condition1_MembraneNum.Value;

            defectCraft.Condition1AndCondition2_Logic = (AndOrLogicEnum)Condition1AndCondition2_Logic.SelectedIndex;
            defectCraft.Condition2_En = Condition2_En.Checked;
            defectCraft.Condition2_DataSource1 = (DefectDataSourceEnum)Condition2_DataSource1.SelectedIndex;
            defectCraft.Condition2_Data1 = (double)Condition2_Data1.Value;
            defectCraft.Condition2_Logic1 = (AndOrLogicEnum)Condition2_Logic1.SelectedIndex;
            defectCraft.Condition2_DataSource2 = (DefectDataSourceEnum)Condition2_DataSource2.SelectedIndex;
            defectCraft.Condition2_Data2 = (double)Condition2_Data2.Value;
            defectCraft.Condition2_Logic2 = (AndOrLogicEnum)Condition2_Logic2.SelectedIndex;
            defectCraft.Condition2_DataSource3 = (DefectDataSourceEnum)Condition2_DataSource3.SelectedIndex;
            defectCraft.Condition2_Data3 = (double)Condition2_Data3.Value;

            defectCraft.Condition2AndCondition3_Logic = (AndOrLogicEnum)Condition2AndCondition3_Logic.SelectedIndex;
            defectCraft.Condition3_En = Condition3_En.Checked;
            defectCraft.Condition3_DataSource1 = (DefectDataSourceEnum)Condition3_DataSource1.SelectedIndex;
            defectCraft.Condition3_Data1 = (double)Condition3_Data1.Value;
            defectCraft.Condition3_Logic1 = (AndOrLogicEnum)Condition3_Logic1.SelectedIndex;
            defectCraft.Condition3_DataSource2 = (DefectDataSourceEnum)Condition3_DataSource2.SelectedIndex;
            defectCraft.Condition3_Data2 = (double)Condition3_Data2.Value;

            defectCraft.Condition3AndCondition4_Logic = (AndOrLogicEnum)Condition3AndCondition4_Logic.SelectedIndex;
            defectCraft.Condition4_En = Condition4_En.Checked;
            defectCraft.Condition4_DataSource1 = (DefectDataSourceEnum)Condition4_DataSource1.SelectedIndex;
            defectCraft.Condition4_Data1 = (double)Condition4_Data1.Value;
            defectCraft.Condition4_Logic1 = (AndOrLogicEnum)Condition4_Logic1.SelectedIndex;
            defectCraft.Condition4_DataSource2 = (DefectDataSourceEnum)Condition4_DataSource2.SelectedIndex;
            defectCraft.Condition4_Data2 = (double)Condition4_Data2.Value;

            defectCraft.HaveOrNot = cbx_HaveOrNot.Checked;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            ViewToParam(currentDefectCraft);
        }


        private void detectionItem_SelectedIndexChanged(object sender, AntdUI.IntEventArgs e)
        {
            if (defectCrafts == null) return;
            int index = select_detectionItem.SelectedIndex;
            currentDefectCraft = defectCrafts[index];
            ParamToView(currentDefectCraft);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (currentDefectCraft != null)
                {
                    input_ConditionalFormat.Text = currentDefectCraft.GetConditionalFormat();
                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}
