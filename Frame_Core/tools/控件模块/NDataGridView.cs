using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
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
    }

    public class MessageShow
    {
        public Color color { get; set; } = Color.White;
        public List<string> ShowRowMessage { get; set; } = new List<string>();
    }
}
