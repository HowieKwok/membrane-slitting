using Frame_Core;
using LogTool;
using ScottPlot;
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
    public partial class FormLimitAlarmFix : Form
    {
        public FormLimitAlarmFix()
        {
            InitializeComponent();
            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);
        }
        public SlotSizeLimit slotlimit = new SlotSizeLimit();
        bool FirstCalib = true;

        public void ShowSet()
        {
            linkLabel_SizeItem.Text = slotlimit.Name;
            TextBox_Desp.Text = slotlimit.Description;
            INi_DGV_Offset(dataGridView1);
        }
        public void closeSet()
        {
            slotlimit.Description = TextBox_Desp.Text;
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

            int count = ((PFTCleanCraft)Project.Instance.craftBase).SliceCount;
            //初始化相机0的尺寸。
            for (int i = 0; i < count; i++)
            {
                DataGridViewColumn dgvC = new DataGridViewTextBoxColumn();
                dgvC.Name = $"第{i + 1}条";
                dgvC.HeaderText = $"第{i + 1}条";
                dgvC.SortMode = DataGridViewColumnSortMode.NotSortable;//禁用排序
                dgvC.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;  //设置所有列自适应宽度
                DGView.Columns.Add(dgvC);
            }

            try
            {
                if (slotlimit.ArrayDBSlice == null)
                {
                    slotlimit.ArrayDBSlice = new double[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
                }
                if (slotlimit.ArrayDBSliceCount == null)
                {
                    slotlimit.ArrayDBSliceCount = new double[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
                }
                if (slotlimit.ArrayBJSlice == null)
                {
                    slotlimit.ArrayBJSlice = new double[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
                }
                if (slotlimit.ArrayBJSliceCount == null)
                {
                    slotlimit.ArrayBJSliceCount = new double[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
                }
                AddOffSetData(DGView, slotlimit.ArrayDBSlice, "按条打标(非0即打标)");
                AddOffSetData(DGView, slotlimit.ArrayDBSliceCount, "按条累计打标2");
                AddOffSetData(DGView, slotlimit.ArrayBJSlice, "按条报警(非0即报警)");
                AddOffSetData(DGView, slotlimit.ArrayBJSliceCount, "按条累计报警4");
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
                if (str0.Contains("按条打标(非0即打标)"))
                {
                    for (int j = 1; j < DGView.Columns.Count; j++)
                    {
                        slotlimit.ArrayDBSlice[j - 1] = Convert.ToDouble(DGView.Rows[i].Cells[j].Value.ToString());
                    }
                }
                else if (str0.Contains("按条累计打标2"))
                {
                    for (int j = 1; j < DGView.Columns.Count; j++)
                    {
                        slotlimit.ArrayDBSliceCount[j - 1] = Convert.ToDouble(DGView.Rows[i].Cells[j].Value.ToString());
                    }
                }
                else if (str0.Contains("按条报警(非0即报警)"))
                {
                    for (int j = 1; j < DGView.Columns.Count; j++)
                    {
                        slotlimit.ArrayBJSlice[j - 1] = Convert.ToDouble(DGView.Rows[i].Cells[j].Value.ToString());
                    }
                }
                else if (str0.Contains("按条累计报警4"))
                {
                    for (int j = 1; j < DGView.Columns.Count; j++)
                    {
                        slotlimit.ArrayBJSliceCount[j - 1] = Convert.ToDouble(DGView.Rows[i].Cells[j].Value.ToString());
                    }
                }
            }
        }

    }
}
