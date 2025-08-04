using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class NListView
    {
        public static readonly object LockVisable = new object();
        public static void EnsureEndVisible(ListView listView)
        {
            if (null == listView)
            {
                return;
            }
            int iEnd = listView.Items.Count - 1;
            if (iEnd < 0)
            {
                return;
            }
            listView.Items[iEnd].EnsureVisible();
        }

        public static void ChangeEndItemColor(ListView listView)
        {
            if (null == listView)
            {
                return;
            }
            int n = listView.Items.Count;
            if (n > 0)
            {
                System.Drawing.Color color = (0 == n % 2) ? System.Drawing.Color.FromArgb(255, 45, 45, 45) : System.Drawing.Color.FromArgb(255, 53, 53, 53);
                listView.Items[n - 1].BackColor = color;
            }
        }

        public static void Add(ListView listView, RowData[] strs, bool isBeginInvoke = false)
        {
            if (listView == null) return;

            if ((null == strs) || (strs.Length <= 0))
            {
                return;
            }

            if (isBeginInvoke)
            {
                listView.BeginInvoke(new MethodInvoker(() =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        AddItem(listView, strs);
                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {

                    }

                }));
            }
            else
            {
                if (listView.InvokeRequired)
                {
                    listView.Invoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            listView.BeginUpdate();
                            AddItem(listView, strs);
                            listView.EndUpdate();
                        }
                        catch (Exception)
                        {

                        }

                    }));

                }
                else
                {
                    AddItem(listView, strs);
                }

            }
        }

        private static void AddItem(ListView listView, RowData[] strs)
        {
            if (listView.Items.Count == 10)
            {
                listView.Items.Clear();
                //listView.Items.RemoveAt(0);
            }

            ListViewItem item = listView.Items.Add(strs[0].dataStr);
            item.UseItemStyleForSubItems = false;
            for (int i = 1; i < strs.Length; i++)
            {
                item.SubItems.Add(strs[i].dataStr);
                if (strs[i].resultType == ResultType.OK)
                {
                    item.SubItems[item.SubItems.Count - 1].BackColor = Color.White;
                }
                else if (strs[i].resultType == ResultType.Warn)
                {
                    item.SubItems[item.SubItems.Count - 1].BackColor = Color.DarkOrange;
                }
                else if (strs[i].resultType == ResultType.NG)
                {
                    item.SubItems[item.SubItems.Count - 1].BackColor = Color.Red;
                }
            }
            
        }

        public static void Add(ListView listView, string[] strs, bool isBeginInvoke = false)
        {
            if (listView == null) return;

            if ((null == strs) || (strs.Length <= 0))
            {
                return;
            }

            if (isBeginInvoke)
            {
                listView.BeginInvoke(new MethodInvoker(() =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        AddItem(listView, strs);
                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {

                    }

                }));
            }
            else
            {
                if (listView.InvokeRequired)
                {
                    listView.Invoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            listView.BeginUpdate();
                            AddItem(listView, strs);
                            listView.EndUpdate();
                        }
                        catch (Exception)
                        {

                        }

                    }));

                }
                else
                {
                    AddItem(listView, strs);
                }
            }
        }

        private static void AddItem(ListView listView, string[] strs)
        {
            if ((null == strs) || (strs.Length <= 0))
            {
                return;
            }
            ListViewItem item = listView.Items.Add(strs[0]);
            item.UseItemStyleForSubItems = false;
            for (int i = 1; i < strs.Length; i++)
            {
                item.SubItems.Add(strs[i]);
            }
            if (listView.Items.Count > 10)
            {
                listView.Items.Clear();
                //listView.Items.RemoveAt(0);
            }
        }

        private static void InsertItem(ListView listView, string[] strs)
        {
            if ((null == strs) || (strs.Length <= 0))
            {
                return;
            }
            ListViewItem item = listView.Items.Insert(0, strs[0]);
            item.UseItemStyleForSubItems = false;
            for (int i = 1; i < strs.Length; i++)
            {
                item.SubItems.Add(strs[i]);
            }
            if (listView.Items.Count > 50)
            {
                listView.Items.RemoveAt(listView.Items.Count - 1);
                //listView.Items.RemoveAt(0);
            }


        }

        public static void Insert(ListView listView, string[] strs, bool isBeginInvoke = false)
        {
            lock (LockVisable)
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Length <= 0))
                {
                    return;
                }
                if (isBeginInvoke)
                {
                    listView.BeginInvoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            listView.BeginUpdate();
                            InsertItem(listView, strs);
                            listView.EndUpdate();
                        }
                        catch (Exception)
                        {

                        }

                    }));
                }
                else
                {
                    if (listView.InvokeRequired)
                    {
                        listView.Invoke(new MethodInvoker(() =>
                        {
                            try
                            {
                                listView.BeginUpdate();
                                InsertItem(listView, strs);
                                listView.EndUpdate();
                            }
                            catch (Exception)
                            {

                            }

                        }));

                    }
                    else
                    {
                        InsertItem(listView, strs);
                    }
                }

            }


        }

        private static void InsertItem(ListView listView, RowData[] strs)
        {
            ListViewItem item = listView.Items.Insert(0, strs[0].dataStr);
            item.UseItemStyleForSubItems = false;
            for (int i = 1; i < strs.Length; i++)
            {
                item.SubItems.Add(strs[i].dataStr);
                if (strs[i].resultType == ResultType.OK)
                {
                    item.SubItems[item.SubItems.Count - 1].BackColor = Color.White;
                }
                else if (strs[i].resultType == ResultType.Warn)
                {
                    item.SubItems[item.SubItems.Count - 1].BackColor = Color.DarkOrange;
                }
                else if (strs[i].resultType == ResultType.NG)
                {
                    item.SubItems[item.SubItems.Count - 1].BackColor = Color.Red;
                }
            }
            if (listView.Items.Count > 50)
            {
                listView.Items.RemoveAt(listView.Items.Count - 1);
                //listView.Items.RemoveAt(0);
            }
        }

        public static void Insert(ListView listView, RowData[] strs, bool isBeginInvoke = false)
        {
            lock (LockVisable)
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Length <= 0))
                {
                    return;
                }
                if (isBeginInvoke)
                {
                    listView.BeginInvoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            listView.BeginUpdate();
                            InsertItem(listView, strs);
                            listView.EndUpdate();
                        }
                        catch (Exception)
                        {

                        }

                    }));
                }
                else
                {
                    if (listView.InvokeRequired)
                    {
                        listView.Invoke(new MethodInvoker(() =>
                        {
                            try
                            {
                                listView.BeginUpdate();
                                InsertItem(listView, strs);
                                listView.EndUpdate();
                            }
                            catch (Exception)
                            {

                            }

                        }));

                    }
                    else
                    {
                        InsertItem(listView, strs);
                    }
                }

            }


        }

        /// <summary>
        /// 如果ListView超过iMoreThen项，则删除前面的项
        /// </summary>
        /// <param name="m_listView"></param>
        /// <param name="iMoreThen"></param>
        public static void DeleteListItems(ListView m_listView, int iMoreThen)
        {
            if (null == m_listView)
            {
                return;
            }

            int n = m_listView.Items.Count - iMoreThen;
            for (int i = 0; i < n; i++)
            {
                m_listView.Items.RemoveAt(0);
            }
        }

        public static void DeleteListAllItems(ListView m_listView)
        {
            if (null == m_listView)
            {
                return;
            }

            m_listView.Items.Clear();
        }

        public static void ChangeEndColWidth(ListView listView)
        {
            if (null == listView)
                return;
            int iUpper = listView.Columns.Count - 1;
            if (iUpper < 0)
            {
                return;
            }
            int n = 0;
            for (int i = 0; i < iUpper; i++)
            {
                n += listView.Columns[i].Width;
            }
            listView.Columns[iUpper].Width = listView.Width - n;
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Titles"></param>
        public static void Init(ListView listView, string[] Titles)
        {
            if (true)
            {
                listView.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(listView, true);
                string[] temp = Titles;

                try
                {
                    listView.Clear();
                    listView.BorderStyle = BorderStyle.Fixed3D;
                    listView.GridLines = true;
                    listView.View = View.Details;
                    listView.HideSelection = false;
                    listView.Scrollable = true;
                    //listView.BackColor = Color.DarkGray;

                    //初始化列标题                           
                    for (int i = 0; i < temp.Length; i++)
                    {
                        listView.Columns.Add(temp[i]);
                        listView.Columns[listView.Columns.Count - 1].Width = 90;
                        listView.Columns[listView.Columns.Count - 1].TextAlign = HorizontalAlignment.Center;
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex);
                }
                //listView.BeginInvoke((EventHandler)delegate
                //{
                //    try
                //    {
                //        listView.Clear();
                //        listView.BorderStyle = BorderStyle.Fixed3D;
                //        listView.GridLines = true;
                //        listView.View = View.Details;
                //        listView.HideSelection = false;
                //        listView.Scrollable = true;
                //        //listView.BackColor = Color.DarkGray;

                //        //初始化列标题                           
                //        for (int i = 0; i < temp.Length; i++)
                //        {
                //            listView.Columns.Add(temp[i]);
                //            listView.Columns[listView.Columns.Count - 1].Width = 90;
                //            listView.Columns[listView.Columns.Count - 1].TextAlign = HorizontalAlignment.Center;
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Log4netRecord.ErrorLog.Debug(ex);
                //    }
                //});

            }
        }

        /// <summary>
        /// 刷新列标题
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Titles"></param>
        public static void UpdateColumnsTitles(ListView listView, string[] Titles)
        {
            string[] temp = Titles;
            listView.Invoke((EventHandler)delegate
            {
                try
                {
                    //刷新标题                           
                    for (int i = 0; i < temp.Length; i++)
                    {
                        listView.Columns[i].Text = temp[i];
                    }
                }
                catch (Exception ex)
                {

                }

            });
        }
    }
}
