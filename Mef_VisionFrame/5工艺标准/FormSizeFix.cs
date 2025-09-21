using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Mef_VisionFrame._5工艺标准
{
    public partial class FormSizeFix : Form
    {
        public FormSizeFix()
        {
            InitializeComponent();
            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);
     
        }
        public SizeItemInfo size = new SizeItemInfo();
        bool FirstCalib = true;
        public void ShowSet()
        {
            linkLabel_SizeItem.Text = size.Name;
            TextBox_Desp.Text = size.Description;

            nud_CCDMeasureValue.Value = 1;
            nud_ArtificialMeasureValue.Value = 1;

            nud_CalibValue.Value = (decimal)size.StD_RealFixRito;//旧校准系数
            nud_CalibValueNew.Value = (decimal)size.StD_RealFixRito;//新校准系数
            nud_CalibCompensateValue.Value = (decimal)size.StD_RealFixOffset;//校准补偿值

            //顶级模式系数
            CB_EnableTopModel.Checked = size.EnableTopModel;
            nud_Factor.Value = (decimal)size.StD_TopModelRito;
            nud_Offset.Value = (decimal)size.StD_TopModelOffset;
            if (size.EnableTopModel)
            {
                nud_Factor.Enabled = true;
                nud_Offset.Enabled = true;
            }
            else
            {
                nud_Factor.Enabled = false;
                nud_Offset.Enabled = false;
            }

            FirstCalib = true;
            btn_AutoCalib.BackColor = Color.Green;

            INi_DGV_Offset(dataGridView1);
        }
        public void closeSet()
        {
            size.Description = TextBox_Desp.Text;

            size.StD_RealFixRito = (double)nud_CalibValueNew.Value;//新校准系数
            size.StD_RealFixOffset = (double)nud_CalibCompensateValue.Value;//校准补偿值


            //顶级模式系数
            size.EnableTopModel = CB_EnableTopModel.Checked;
            size.StD_TopModelRito = (double)nud_Factor.Value;
            size.StD_TopModelOffset = (double)nud_Offset.Value;

            RefreshData(dataGridView1);
        }

        private void FormSizeFix_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeSet();
        }

        private void btn_AutoCalib_Click(object sender, EventArgs e)
        {
            try
            {
                if (FirstCalib == false)
                {
                    MessageBox.Show("关闭弹窗继续校准");
                    FirstCalib = true;
                    btn_AutoCalib.BackColor = Color.Green;
                    return;
                }
                if (nud_CCDMeasureValue.Value == 0 || nud_ArtificialMeasureValue.Value == 0)
                {
                    MessageBox.Show("请输入非0值");
                    return;
                }
                //标定值=(实际量测值/软件测量值)*标定值
                nud_CalibValueNew.Value = Math.Round((nud_ArtificialMeasureValue.Value / nud_CCDMeasureValue.Value * (decimal)size.StD_RealFixRito), 6);
                FirstCalib = false;
                btn_AutoCalib.BackColor = Color.Gray;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void btn_FixReset_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("确定要重置校准系数？", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No) return;

            nud_CalibValueNew.Value = 1;
        }
        private void CB_EnableTopModel_CheckedChanged(object sender, EventArgs e)
        {
            size.EnableTopModel = CB_EnableTopModel.Checked;
            if (size.EnableTopModel)
            {
                nud_Factor.Enabled = true;
                nud_Offset.Enabled = true;
            }
            else
            {
                nud_Factor.Enabled = false;
                nud_Offset.Enabled = false;
            }
        }


        public void INi_DGV_Offset(DataGridView DGView)
        {
            DGView.Rows.Clear();
            DGView.Columns.Clear();
            //根据Header和所有单元格的内容自动调整行的高度
            DGView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //设置对齐方式和字体
            DGView.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGView.Font = new Font("黑体", 14);

            DataGridViewColumn dgvitem = new DataGridViewTextBoxColumn();
            dgvitem.Name = $"项目";
            dgvitem.HeaderText = $"项目";
            dgvitem.SortMode = DataGridViewColumnSortMode.NotSortable;//禁用排序
            dgvitem.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;  //设置所有列自适应宽度
            DGView.Columns.Add(dgvitem);
            DGView.Columns[0].ReadOnly = true;

            int count = ((PFTCleanCraft)Project.Instance.craftBase).SliceCount;
            for (int i = 0; i < count; i++)
            {
                DataGridViewColumn dgvC = new DataGridViewTextBoxColumn();
                dgvC.Name = $"分条{i + 1}";
                dgvC.HeaderText = $"分条{i + 1}";
                dgvC.SortMode = DataGridViewColumnSortMode.NotSortable;//禁用排序
                dgvC.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;  //设置所有列自适应宽度
                DGView.Columns.Add(dgvC);
            }

            try
            {
                if (size.ArrayBelongSlice == null)
                {
                    size.ArrayBelongSlice = new double[8] { 0, 0, 0, 0, 1, 1, 1, 1 };
                }

                AddOffSetData(DGView, size.ArrayBelongSlice, "所属分条");

            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug(ee);
            }
        }

        /// <summary>
        /// 添加尺寸DataGridView数据
        /// </summary>
        /// <param name="DGView"></param>
        /// <param name="item"></param>
        public void AddOffSetData(DataGridView DGView, double[] ArrayOffset, string ItemName)
        {
            int count = DGView.Rows.Count;
            DGView.Rows.Add();
            DGView.Rows[count - 1].Cells[0].Value = ItemName.ToString();
            for (int i = 0; i < ArrayOffset.Length; i++)
            {
                DGView.Rows[count - 1].Cells[i + 1].Value = ArrayOffset[i];
            }
        }
        public void RefreshData(DataGridView DGView)
        {
            for (int i = 0; i < DGView.Rows.Count - 1; i++)
            {
                string str0 = DGView.Rows[i].Cells[0].Value.ToString();
                if (str0.Contains("所属分条"))
                {
                    for (int j = 1; j < DGView.Columns.Count; j++)
                    {
                        size.ArrayBelongSlice[j - 1] = Convert.ToDouble(DGView.Rows[i].Cells[j].Value.ToString());
                    }
                }
            }
        }
    }
}
