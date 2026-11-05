using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public static class DataGridViewExtension
    {
        /// <summary>
        /// 设置DataGridView双缓冲，解决多列横向滚动、刷新闪烁
        /// </summary>
        /// <param name="dgv">表格控件</param>
        /// <param name="enable">开启/关闭双缓冲</param>
        public static DataGridView SetDoubleBuffered(this DataGridView dgv, bool enable)
        {
            if (dgv == null) return dgv;
            PropertyInfo prop = dgv.GetType().GetProperty(
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);

            prop?.SetValue(dgv, enable, null);

            return dgv;
        }

        /// <summary>
        /// 一键抗闪烁优化（纯可用属性，无报错）
        /// </summary>
        public static void EnableAntiFlicker(this DataGridView dgv)
        {
            dgv.SetDoubleBuffered(true);
            // 固定表头行高、行标题宽度，减少频繁重绘计算
            dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            // 关闭行列自动尺寸自适应，列数量极多时核心优化
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        public static void BindingSource<T>(this DataGridView dgv, BindingList<T> dataList)
        {
            if (dgv == null) return;

            dgv.Rows.Clear();
            dgv.Columns.Clear();
            //BindingSource bindingSource = new BindingSource();
            //bindingSource.DataSource = dataList;
            dgv.DataSource = dataList;
        }

        public static DataGridView SetColumnHeaders(this DataGridView dgv)
        {
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                // 通过反射获取 DisplayName
                var property = typeof(QualityData).GetProperty(column.DataPropertyName);
                if (property != null)
                {
                    var displayName = property.GetCustomAttribute<DisplayNameAttribute>();
                    if (displayName != null)
                    {
                        column.HeaderText = displayName.DisplayName;
                    
                    }
                }
            }
            return dgv;
        }
    }

    public class NDataGridView
    {
        /// <summary>
        /// 更改DataGridview行数
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="rowNum"></param>
        public static void ChangeDataGridviewRowNum(DataGridView dgv, int rowNum)
        {
            if (dgv == null) return;
            if (rowNum <= 0) return;
            if (dgv.Rows.Count == rowNum) return;
            if (dgv.Rows.Count > rowNum)
            {
                for (int i = rowNum; i < dgv.Rows.Count; i++)
                {
                    dgv.Rows.RemoveAt(i);
                }
            }
            else
            {
                for (int i = dgv.Rows.Count; i < rowNum; i++)
                {
                    dgv.Rows.Add();
                }
            }
        }

        public static void AddRow(DataGridView dataGridView, List<MessageShow> messageShows, int showRowCount, bool isBeginInvoke)
        {
            if (dataGridView == null) return;

            if ((null == messageShows) || (messageShows.Count <= 0))
            {
                return;
            }

            Action updateAction = () =>
            {
                try
                {
                    for (int i = 0; i < messageShows.Count; i++)
                    {
                        if (showRowCount > 0 && dataGridView.Rows.Count > showRowCount) dataGridView.Rows.RemoveAt(0);
                        AddItem(dataGridView, messageShows[i]);
                    }
                }
                catch (Exception)
                {

                }
            };

            Update(dataGridView, updateAction, isBeginInvoke);
        }

        public static void AddRow(DataGridView dataGridView, List<string[]> messageShows, int showRowCount, bool isBeginInvoke)
        {
            if (dataGridView == null) return;

            if ((null == messageShows) || (messageShows.Count <= 0))
            {
                return;
            }

            Action updateAction = () =>
            {
                try
                {
                    for (int i = 0; i < messageShows.Count; i++)
                    {
                        if (showRowCount > 0 && dataGridView.Rows.Count > showRowCount) dataGridView.Rows.RemoveAt(0);
                        AddItem(dataGridView, messageShows[i]);
                    }
                }
                catch (Exception)
                {

                }
            };

            Update(dataGridView, updateAction, isBeginInvoke);
        }

        public static void AddRow(DataGridView dataGridView, string[] messageShows, int showRowCount, bool isBeginInvoke)
        {
            if (dataGridView == null) return;

            if ((null == messageShows) || (messageShows.Length <= 0))
            {
                return;
            }

            Action updateAction = () =>
            {
                try
                {
                    if (showRowCount > 0 && dataGridView.Rows.Count > showRowCount) dataGridView.Rows.RemoveAt(0);
                    AddItem(dataGridView, messageShows);
                }
                catch (Exception)
                {

                }
            };

            Update(dataGridView, updateAction, isBeginInvoke);
        }
        public static void AddItem(DataGridView dataGridView, MessageShow messageShow)
        {
            dataGridView.Rows.Add();
            dataGridView.Rows[dataGridView.Rows.Count - 1].DefaultCellStyle.BackColor = messageShow.color;
            for (int i = 0; i < messageShow.ShowRowMessage.Count; i++)
            {
                if (dataGridView.Columns.Count > i)
                {
                    dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[i].Value = messageShow.ShowRowMessage[i];
                }
            }

        }

        public static void AddItem(DataGridView dataGridView, string[] messageShow)
        {
            dataGridView.Rows.Add();
            for (int i = 0; i < messageShow.Length; i++)
            {
                if (dataGridView.Columns.Count > i)
                {
                    dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[i].Value = messageShow[i];
                }
            }

        }

        public static void ChangeValue(DataGridView dataGridView, int rowIndex, int columnIndex, string value)
        {
            if (dataGridView == null) return;

            if (rowIndex < 0 || rowIndex >= dataGridView.Rows.Count) return;

            if (columnIndex < 0 || columnIndex >= dataGridView.Columns.Count) return;

            dataGridView.Invoke(new MethodInvoker(() =>
            {
                dataGridView.Rows[rowIndex].Cells[columnIndex].Value = value;
            }));
        }

        public static void ChangeRowCount(DataGridView dataGridView, int rowCount, string paddingValue)
        {
            if (dataGridView == null) return;

            if (rowCount < 0) return;

            int num = rowCount - dataGridView.Rows.Count;
            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    dataGridView.Rows.Add();
                    for (int j = 0; j < dataGridView.Columns.Count; j++)
                    {
                        if (dataGridView.Columns[j] is DataGridViewTextBoxColumn)
                        {
                            if (string.IsNullOrEmpty(paddingValue))
                            {
                                dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[j].Value = "";
                            }
                            else
                            {
                                dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[j].Value = paddingValue;
                            }
                        }
                        else if (dataGridView.Columns[j] is DataGridViewCheckBoxColumn)
                        {
                            dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[j].Value = false;
                        }


                    }
                }
            }
            else if (num < 0)
            {
                for (int i = 0; i < -num; i++)
                {
                    dataGridView.Rows.RemoveAt(dataGridView.Rows.Count - 1);
                }
            }
        }

        /// <summary>
        /// 改变列表的行数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param>
        /// <param name="rowCount"></param>
        public static void ChangeRowCount<T>(List<T> lists, int rowCount) where T : new()
        {
            if (lists == null) return;

            if (rowCount < 0) return;

            int num = rowCount - lists.Count;
            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    lists.Add(new T());
                }
            }
            else if (num < 0)
            {
                for (int i = 0; i < -num; i++)
                {
                    lists.RemoveAt(lists.Count - 1);
                }
            }
        }

        /// <summary>
        /// 改变列表的行数(只加不减)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lists"></param>
        /// <param name="rowCount"></param>
        public static void ChangeRowCount_Add<T>(List<T> lists, int rowCount) where T : new()
        {
            if (lists == null) return;

            if (rowCount < 0) return;

            int num = rowCount - lists.Count;
            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    lists.Add(new T());
                }
            }
        }

        private static void Update(DataGridView dataGridView, Action updateAction, bool isBeginInvoke)
        {
            if (dataGridView == null || updateAction == null)
            {
                return;
            }

            if (isBeginInvoke)
            {
                dataGridView.BeginInvoke(new MethodInvoker(updateAction));
            }
            else
            {
                if (dataGridView.InvokeRequired)
                {
                    dataGridView.Invoke(new MethodInvoker(updateAction));
                }
                else
                {
                    updateAction();
                }
            }
        }

        public static void ClearRows(DataGridView dataGridView)
        {
            if (dataGridView == null) return;
            dataGridView.Invoke(new MethodInvoker(() =>
            {
                dataGridView.Rows.Clear();
            }));
        }


        /// <summary>
        /// 一次性把 dgv 初始化成指定结构：
        /// 列全部来自 colHeaders；行全部来自 rowHeaders；
        /// 每行第 0 列写对应的行标题。
        /// </summary>
        public static void InitGrid(DataGridView dgv,
                                    List<string> colHeaders,
                                    List<string> rowHeaders)
        {
            if (dgv == null) throw new ArgumentNullException(nameof(dgv));
            if (colHeaders == null || rowHeaders == null)
                MessageNotify.AddErrorLog("表头不能为空");

            dgv.EnableAntiFlicker();
            dgv.SuspendLayout();
            dgv.Columns.Clear();
            dgv.Rows.Clear();

            // 1. 列
            foreach (var h in colHeaders)
                dgv.Columns.Add(h, h);

            // 2. 行
            foreach (var r in rowHeaders)
            {
                int idx = dgv.Rows.Add();
                dgv.Rows[idx].Cells[0].Value = r;
            }
            for (int r = 0; r < dgv.RowCount; r++)
            {
                for (int c = 1; c < dgv.ColumnCount; c++)
                {
                    dgv.Rows[r].Cells[c].Value = 0;   // 设置默认值为0
                                                      // dgv.Rows[r].Cells[c].Style.Alignment = DataGridViewContentAlignment.MiddleCenter; // 设置居中对齐
                }
            }
           
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.ResumeLayout();
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            //dgv.RowTemplate.Height = dgv.Height /
            //                        dgv.Rows.Count/* Math.Max(1, dgv.Rows.Count)*/;

            //foreach(DataGridViewRow row in dgv.Rows)
            //{

            //    row.Height = dgv.Height  /
            //                       dgv.Rows.Count; 
            //}
        }

        /// <summary>
        /// 根据列标题 + 行标题写单元格
        /// </summary>
        public static void SetCellValue(DataGridView dgv,
                                        string colHeader,
                                        string rowHeader,
                                        object value)
        {
            int c = ColumnIndex(dgv, colHeader);
            int r = RowIndex(dgv, rowHeader);
            dgv.Rows[r].Cells[c].Value = value;
        }

        /// <summary>
        /// 根据列标题 + 行标题读单元格
        /// </summary>
        public static object GetCellValue(DataGridView dgv,
                                          string colHeader,
                                          string rowHeader)
        {
            int c = ColumnIndex(dgv, colHeader);
            int r = RowIndex(dgv, rowHeader);
            return dgv.Rows[r].Cells[c].Value;
        }

        /* ---------- 内部小工具 ---------- */
        private static int ColumnIndex(DataGridView dgv, string header)
        {
            try
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (string.Equals(col.HeaderText, header, StringComparison.OrdinalIgnoreCase))
                        return col.Index;
                }

                // 未找到
                MessageNotify.AddErrorLog($"未找到列标题：{header}");
                return -1;
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog($"查找列标题异常：{ex.Message}");
                return -1;
            }
        }

        private static int RowIndex(DataGridView dgv, string header)
        {
            try
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells[0].Value != null &&
                        string.Equals(row.Cells[0].Value.ToString(), header,
                                      StringComparison.OrdinalIgnoreCase))
                    {
                        return row.Index;
                    }
                }

                // 走到这里说明没找到
                MessageNotify.AddErrorLog($"未找到行标题：{header}");
                return -1;
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog($"查找行标题异常：{ex.Message}");
                return -1;
            }
        }
    }

    public class MessageShow
    {
        public Color color { get; set; } = Color.White;
        public List<string> ShowRowMessage { get; set; } = new List<string>();
    }
}
