using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Mef_VisionFrame._5工艺标准
{
    public partial class FormSlotFix : Form
    {
        public FormSlotFix()
        {
            InitializeComponent();
            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);
        }
        public SlotInfo slot = new SlotInfo();
        bool FirstCalib = true;

        public void ShowSet()
        {
            INi_DGV_Offset(dataGridView1);
        }
        public void closeSet()
        {

            RefreshData(dataGridView1);
        }

        private void FormSlotFix_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeSet();
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

            int count = Enum.GetValues(typeof(MeasureSlotEnum)).Length;
            //初始化相机0的尺寸。
            for (int i = 0; i < count; i++)
            {
                DataGridViewColumn dgvC = new DataGridViewTextBoxColumn();
                dgvC.Name = ((MeasureSlotEnum)i).ToString();
                dgvC.HeaderText = ((MeasureSlotEnum)i).ToString();
                dgvC.SortMode = DataGridViewColumnSortMode.NotSortable;//禁用排序
                dgvC.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;  //设置所有列自适应宽度
                DGView.Columns.Add(dgvC);
            }

            try
            {
                if (slot.RealFixRito == null)
                {
                    slot.RealFixRito = new double[6] { 1, 1, 1, 1, 1, 1 };
                }
                if (slot.RealFixOffset == null)
                {
                    slot.RealFixOffset = new double[6] { 0, 0, 0, 0, 0, 0 };
                }
                AddOffSetData(DGView, slot.RealFixRito, "校准系数");
                AddOffSetData(DGView, slot.RealFixOffset, "测量校准偏移");
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
                if (str0.Contains("校准系数"))
                {
                    for (int j = 1; j < DGView.Columns.Count; j++)
                    {
                        slot.RealFixRito[j - 1] = Convert.ToDouble(DGView.Rows[i].Cells[j].Value.ToString());
                    }
                }
                else if (str0.Contains("测量校准偏移"))
                {
                    for (int j = 1; j < DGView.Columns.Count; j++)
                    {
                        slot.RealFixOffset[j - 1] = Convert.ToDouble(DGView.Rows[i].Cells[j].Value.ToString());
                    }
                }
            }
        }

    }
}
