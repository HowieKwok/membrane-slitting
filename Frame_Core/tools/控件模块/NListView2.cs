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
    /// <summary>
    /// NListView2
    /// pengjiaqing
    /// 2024-11-6
    /// </summary>
    public class NListView2
    {
        /// <summary>
        /// 初始化表格
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Titles"></param>
        public static void Init(ListView listView, TitlesInfo[] titles)
        {
            if (true)
            {
                listView.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(listView, true);

                listView.Invoke((EventHandler)delegate
                {
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
                        for (int i = 0; i < titles.Length; i++)
                        {
                            listView.Columns.Add(titles[i].Title);

                            if (titles[i].AutoResize)
                            {
                                listView.Columns[i].Width = GetTitlesWidth(listView.Font, titles[i].Title);
                            }
                            else
                            {
                                listView.Columns[i].Width = titles[i].Width;
                            }
                            listView.Columns[listView.Columns.Count - 1].TextAlign = titles[i].TextAlign;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Debug(ex);
                    }
                });

            }
        }

        /// <summary>
        /// 刷新列标题
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Titles"></param>
        public static void UpdateColumnsTitles(ListView listView, TitlesInfo[] titles)
        {
            listView.Invoke((EventHandler)delegate
            {
                try
                {
                    //刷新标题                           
                    for (int i = 0; i < titles.Length; i++)
                    {
                        listView.Columns[i].Text = titles[i].Title;

                        if (titles[i].AutoResize)
                        {
                            listView.Columns[i].Width = GetTitlesWidth(listView.Font, titles[i].Title);
                        }
                        else
                        {
                            listView.Columns[i].Width = titles[i].Width;
                        }
                        listView.Columns[listView.Columns.Count - 1].TextAlign = titles[i].TextAlign;
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex);
                }

            });
        }

        /// <summary>
        /// 获取标题宽度
        /// </summary>
        /// <param name="font"></param>
        /// <param name="titels"></param>
        /// <returns></returns>
        public static int GetTitlesWidth(Font font, string titels)
        {
            int textWidth = TextRenderer.MeasureText(titels, font).Width;
            return textWidth + 20; // 加上一些额外空间
        }

        /// <summary>
        /// 清空表格
        /// </summary>
        /// <param name="listView"></param>
        public static void ClearItems(ListView listView)
        {
            if (listView == null) return;
            listView.Invoke((EventHandler)delegate
            {
                listView.Items.Clear();
            });
        }


        public static void Add(ListView listView, List<RowData[]> strs, int showRowCount, ListViewShowType listViewShowType, bool isBeginInvoke)
        {
            try
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Count <= 0))
                {
                    return;
                }

                Action updateAction = () =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        for (int i = 0; i < strs.Count; i++)
                        {
                            AddItem(listView, strs[i], showRowCount, listViewShowType);
                        }

                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };

                Update(listView, updateAction, isBeginInvoke);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        public static void Add(ListView listView, RowData[] strs, int showRowCount, ListViewShowType listViewShowType, bool isBeginInvoke)
        {
            try
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Length <= 0))
                {
                    return;
                }

                Action updateAction = () =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        AddItem(listView, strs, showRowCount, listViewShowType);
                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };

                Update(listView, updateAction, isBeginInvoke);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        public static void Add(ListView listView, List<string[]> strs, int showRowCount, ListViewShowType listViewShowType, bool isBeginInvoke, ListViewAlternateColor listViewAlternateColor = null)
        {
            try
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Count <= 0))
                {
                    return;
                }

                Action updateAction = () =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        for (int i = 0; i < strs.Count; i++)
                        {
                            AddItem(listView, strs[i], showRowCount, listViewShowType, listViewAlternateColor);
                        }

                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };

                Update(listView, updateAction, isBeginInvoke);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        public static void Add(ListView listView, string[] strs, int showRowCount, ListViewShowType listViewShowType, bool isBeginInvoke, ListViewAlternateColor listViewAlternateColor = null)
        {
            try
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Length <= 0))
                {
                    return;
                }

                Action updateAction = () =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        AddItem(listView, strs, showRowCount, listViewShowType, listViewAlternateColor);
                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };

                Update(listView, updateAction, isBeginInvoke);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public static void Insert(ListView listView, List<RowData[]> strs, int showRowCount, ListViewShowType listViewShowType, bool isBeginInvoke, ListViewAlternateColor listViewAlternateColor = null)
        {
            try
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Count <= 0))
                {
                    return;
                }

                Action updateAction = () =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        for (int i = 0; i < strs.Count; i++)
                        {
                            InsertItem(listView, strs[i], showRowCount, listViewShowType, listViewAlternateColor);
                        }

                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };

                Update(listView, updateAction, isBeginInvoke);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        public static void Insert(ListView listView, RowData[] strs, int showRowCount, ListViewShowType listViewShowType, bool isBeginInvoke, ListViewAlternateColor listViewAlternateColor = null)
        {
            try
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Length <= 0))
                {
                    return;
                }

                Action updateAction = () =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        InsertItem(listView, strs, showRowCount, listViewShowType, listViewAlternateColor);
                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };

                Update(listView, updateAction, isBeginInvoke);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        public static void Insert(ListView listView, List<string[]> strs, int showRowCount, ListViewShowType listViewShowType, bool isBeginInvoke, ListViewAlternateColor listViewAlternateColor = null)
        {
            try
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Count <= 0))
                {
                    return;
                }

                Action updateAction = () =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        for (int i = 0; i < strs.Count; i++)
                        {
                            InsertItem(listView, strs[i], showRowCount, listViewShowType, listViewAlternateColor);
                        }

                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };

                Update(listView, updateAction, isBeginInvoke);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        public static void Insert(ListView listView, string[] strs, int showRowCount, ListViewShowType listViewShowType, bool isBeginInvoke, ListViewAlternateColor listViewAlternateColor = null)
        {
            try
            {
                if (listView == null) return;

                if ((null == strs) || (strs.Length <= 0))
                {
                    return;
                }

                Action updateAction = () =>
                {
                    try
                    {
                        listView.BeginUpdate();
                        InsertItem(listView, strs, showRowCount, listViewShowType, listViewAlternateColor);
                        listView.EndUpdate();
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };

                Update(listView, updateAction, isBeginInvoke);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }


        private static void AddItem(ListView listView, RowData[] strs, int showRowCount, ListViewShowType listViewShowType)
        {
            if (listView == null || strs == null || strs.Length <= 0) return;
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

            showRowCount = showRowCount < 5 ? 5 : showRowCount;
            if (listView.Items.Count > showRowCount)
            {
                if (listViewShowType == ListViewShowType.Clear)
                {
                    listView.Items.Clear();
                }
                else if (listViewShowType == ListViewShowType.RemoveAtStartRow)
                {
                    listView.Items.RemoveAt(0);
                }
                else if (listViewShowType == ListViewShowType.RemoveAtStartRowAndEndVisible)
                {
                    listView.Items.RemoveAt(0);
                    listView.Items[listView.Items.Count - 1].EnsureVisible();
                }
                else
                {
                    listView.Items.Clear();
                }
            }
        }
        private static void AddItem(ListView listView, string[] strs, int showRowCount, ListViewShowType listViewShowType, ListViewAlternateColor listViewAlternateColor)
        {
            if (listView == null || strs == null || strs.Length <= 0) return;
            ListViewItem item = listView.Items.Add(strs[0]);
            //item.UseItemStyleForSubItems = false;
            for (int i = 1; i < strs.Length; i++)
            {
                item.SubItems.Add(strs[i]);
            }

            if (listViewAlternateColor != null && listViewAlternateColor.Colors.Length > 1)
            {
                item.UseItemStyleForSubItems = true;
                listView.Items[listView.Items.Count - 1].BackColor = listViewAlternateColor.GetCurrentColor();
            }

            showRowCount = showRowCount < 5 ? 5 : showRowCount;
            if (listView.Items.Count > showRowCount)
            {
                if (listViewShowType == ListViewShowType.Clear)
                {
                    listView.Items.Clear();
                }
                else if (listViewShowType == ListViewShowType.RemoveAtStartRow)
                {
                    listView.Items.RemoveAt(0);
                }
                else if (listViewShowType == ListViewShowType.RemoveAtStartRowAndEndVisible)
                {
                    listView.Items.RemoveAt(0);
                    listView.Items[listView.Items.Count - 1].EnsureVisible();
                }
                else
                {
                    listView.Items.Clear();
                }
            }
        }
        private static void InsertItem(ListView listView, RowData[] strs, int showRowCount, ListViewShowType listViewShowType, ListViewAlternateColor listViewAlternateColor)
        {
            if (listView == null || strs == null || strs.Length <= 0) return;
            Color backcolor = Color.White;
            if (listViewAlternateColor != null && listViewAlternateColor.Colors.Length > 1)
            {
                backcolor = listViewAlternateColor.GetCurrentColor();
            }
            ListViewItem item = listView.Items.Insert(0, strs[0].dataStr);
           
            item.BackColor = backcolor;
            item.UseItemStyleForSubItems = false;
            for (int i = 1; i < strs.Length; i++)
            {
                item.SubItems.Add(strs[i].dataStr);
                if (strs[i].resultType == ResultType.OK)
                {
                    item.SubItems[item.SubItems.Count - 1].BackColor = backcolor;

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

            showRowCount = showRowCount < 5 ? 5 : showRowCount;
            if (listView.Items.Count > showRowCount)
            {
                if (listViewShowType == ListViewShowType.Clear)
                {
                    listView.Items.Clear();
                }
                else if (listViewShowType == ListViewShowType.RemoveAtEndRow)
                {
                    listView.Items.RemoveAt(listView.Items.Count - 1);
                }
                else
                {
                    listView.Items.Clear();
                }
            }
        }
        private static void InsertItem(ListView listView, string[] strs, int showRowCount, ListViewShowType listViewShowType, ListViewAlternateColor listViewAlternateColor)
        {
            if (listView == null || strs == null || strs.Length <= 0) return;
            ListViewItem item = listView.Items.Insert(0, strs[0]);
            //item.UseItemStyleForSubItems = false;
            for (int i = 1; i < strs.Length; i++)
            {
                item.SubItems.Add(strs[i]);
            }

            if (listViewAlternateColor != null && listViewAlternateColor.Colors.Length > 1)
            {
                item.UseItemStyleForSubItems = true;
                listView.Items[0].BackColor = listViewAlternateColor.GetCurrentColor();
            }

            showRowCount = showRowCount < 5 ? 5 : showRowCount;
            if (listView.Items.Count > showRowCount)
            {
                if (listViewShowType == ListViewShowType.Clear)
                {
                    listView.Items.Clear();
                }
                else if (listViewShowType == ListViewShowType.RemoveAtEndRow)
                {
                    listView.Items.RemoveAt(listView.Items.Count - 1);
                }
                else
                {
                    listView.Items.Clear();
                }
            }
        }

        private static void Update(ListView listView, Action updateAction, bool isBeginInvoke)
        {
            if (listView == null || updateAction == null)
            {
                return;
            }

            if (isBeginInvoke)
            {
                listView.BeginInvoke(new MethodInvoker(updateAction));
            }
            else
            {
                if (listView.InvokeRequired)
                {
                    listView.Invoke(new MethodInvoker(updateAction));
                }
                else
                {
                    updateAction();
                }
            }
        }

        private static void EnsureEndVisible(ListView listView)
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
    }

    public class TitlesInfo
    {
        public string Title { get; set; } = "default";
        public int Width { get; set; } = 90;
        public bool AutoResize { get; set; } = false;

        public HorizontalAlignment TextAlign { get; set; } = HorizontalAlignment.Center;
    }
    public class ListViewAlternateColor
    {
        public Color[] Colors { get; set; } = new Color[] { Color.Blue, Color.LightGray };

        private int CurrentIndex { get; set; } = 0;

        public Color GetCurrentColor()
        {
            if (Colors == null || Colors.Length <= 1)
            {
                return Color.White;
            }
            if (CurrentIndex < 0) CurrentIndex = 0;
            if (CurrentIndex >= Colors.Length) CurrentIndex = 0;
            int currentIndex = CurrentIndex % Colors.Length;
            CurrentIndex++;
            return Colors[currentIndex];
        }
    }

    public enum ListViewShowType
    {
        /// <summary>
        /// listView.Items.Count > showRowCount，直接清空
        /// </summary>
        Clear,
        /// <summary>
        /// listView.Items.Count > showRowCount，删除最前面的N项(Add有效)
        /// </summary>
        RemoveAtStartRow,
        /// <summary>
        /// listView.Items.Count > showRowCount，删除最前面的N项并使最后一行可见(Add有效)
        /// </summary>
        RemoveAtStartRowAndEndVisible,
        /// <summary>
        /// listView.Items.Count > showRowCount，删除最后面的N项(Insert有效)
        /// </summary>
        RemoveAtEndRow,

    }
}
