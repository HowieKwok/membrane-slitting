using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    /// <summary>
    /// 参数控制
    ///2024-03-02 pengjiaqing
    /// </summary>
    public static class ControlTool
    {
        public static void SetParamShow(CheckBox control, ref bool bParam)
        {
            control.Checked = bParam;
        }

        public static void GetParamSave(CheckBox control, ref bool bParam)
        {
            bParam = control.Checked;
        }

        public static void SetParamShow(NumericUpDown control, ref double bParam)
        {
            if (bParam > (double)control.Maximum) bParam = (double)control.Maximum;
            if (bParam < (double)control.Minimum) bParam = (double)control.Minimum;

            control.Value = (decimal)bParam;
        }

        public static void GetParamSave(NumericUpDown control, ref double bParam)
        {
            bParam = (double)control.Value;
        }

        public static void SetParamShow(NumericUpDown control, ref float bParam)
        {
            if (bParam > (float)control.Maximum) bParam = (float)control.Maximum;
            if (bParam < (float)control.Minimum) bParam = (float)control.Minimum;

            control.Value = (decimal)bParam;
        }

        public static void GetParamSave(NumericUpDown control, ref float bParam)
        {
            bParam = (float)control.Value;
        }

        public static void SetParamShow(NumericUpDown control, ref int bParam)
        {
            if (bParam > (int)control.Maximum) bParam = (int)control.Maximum;
            if (bParam < (int)control.Minimum) bParam = (int)control.Minimum;

            control.Value = (decimal)bParam;
        }

        public static void GetParamSave(NumericUpDown control, ref int bParam)
        {
            bParam = (int)control.Value;
        }

        public static void SetParamShow(TextBox control, ref string bParam)
        {
            control.Text = bParam;
        }

        public static void GetParamSave(TextBox control, ref string bParam)
        {
            bParam = control.Text.Replace(" ", "");
        }

        public static void SetParamShow(ComboBox control, string bParam)
        {
            control.SelectedItem = (string)bParam;
        }
        public static string GetParamSave(ComboBox control)
        {
            return (string)control.SelectedItem;
        }

        public static void SetParamShow(ComboBox control, ref int bParam)
        {
            control.SelectedIndex = bParam < control.Items.Count ? bParam : -1;
        }

        public static void GetParamSave(ComboBox control, ref int bParam)
        {
            bParam = (int)control.SelectedIndex;
        }

        public static void SetParamShow(ControlComboBox control, ref int bParam)
        {
            control.SetSelectedIndex(bParam);
        }

        public static void GetParamSave(ControlComboBox control, ref int bParam)
        {
            bParam = control.GetSelectedIndex(); ;
        }

        public static void SetParamShow(DateTimePicker control, ref DateTime bParam)
        {
            control.Value = bParam;
        }

        public static void GetParamSave(DateTimePicker control, ref DateTime bParam)
        {
            bParam = control.Value;
        }

        public static void SetParamShow(DataGridView control, int rowIndex, int colIndex, ref string oParam)
        {
            control.Rows[rowIndex].Cells[colIndex].Value = oParam;
        }

        public static void GetParamSave(DataGridView control, int rowIndex, int colIndex, ref string oParam)
        {
            oParam = control.Rows[rowIndex].Cells[colIndex].Value.ToString();
        }

        public static void SetParamShow(DataGridView control, int rowIndex, int colIndex, ref int oParam)
        {
            control.Rows[rowIndex].Cells[colIndex].Value = oParam;
        }

        public static void GetParamSave(DataGridView control, int rowIndex, int colIndex, ref int oParam)
        {
            oParam = Convert.ToInt32(control.Rows[rowIndex].Cells[colIndex].Value);
        }

        public static void SetParamShow(DataGridView control, int rowIndex, int colIndex, ref double oParam)
        {
            control.Rows[rowIndex].Cells[colIndex].Value = oParam;
        }

        public static void GetParamSave(DataGridView control, int rowIndex, int colIndex, ref double oParam)
        {
            oParam = Convert.ToDouble(control.Rows[rowIndex].Cells[colIndex].Value);
        }

        /// <summary>
        /// 清除DataGridView的DataGridViewComboBox列信息
        /// </summary>
        /// <param name="dataGridViewCell"></param>
        public static void ClearItemsDataGridViewComboBoxColumn(DataGridView dataGridView, int colIndex)
        {
            DataGridViewComboBoxColumn dataGridViewColumn = dataGridView.Columns[colIndex] as DataGridViewComboBoxColumn;
            if (dataGridViewColumn != null)
            {
                dataGridViewColumn.Items.Clear();
            }

        }

        public static void ADDItemsDataGridViewComboBoxColumn(DataGridView dataGridView, int colIndex, string[] items)
        {
            DataGridViewComboBoxColumn dataGridViewColumn = dataGridView.Columns[colIndex] as DataGridViewComboBoxColumn;
            if (dataGridViewColumn != null)
            {
                foreach (var item in items)
                {
                    dataGridViewColumn.Items.Add(item);
                }
            }
        }
    }
}
