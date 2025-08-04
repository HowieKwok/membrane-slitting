using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    /// <summary>
    /// 自定义ComboBox控件
    /// 作者：PengJiaQing
    /// 描述：自定义ComboBox控件，实现隐藏项功能
    /// 使用：string[] items = { "1", "2", "3" };  ComboBoxDataItem[] comboBoxDataItems = items.Select(item => new ComboBoxDataItem { DataItem = item, IsHide = false }).ToArray();
    /// </summary>
    public class ControlComboBox : ComboBox
    {
        ComboBoxDataItem[] dataSourceItems;
        public void AddItems(ComboBoxDataItem[] dataSource)
        {
            dataSourceItems = dataSource;
            string[] showItems = dataSource.Where(x => x.IsShow).Select(x => x.DataItem).ToArray();
            Items.Clear();
            Items.AddRange(showItems);
        }

        /// <summary>
        /// 添加数据源
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="dataShow">数据源中要显示的集合</param>
        public void AddItems(string[] dataSource, string[] dataShow)
        {
            dataSourceItems = dataSource.Select(item => new ComboBoxDataItem { DataItem = item }).ToArray();
            // 设置 dataShow 数组中的元素对应的 IsShow 为 true
            foreach (var item in dataSourceItems)
            {
                if (dataShow.Contains(item.DataItem))
                {
                    item.IsShow = true;
                }
            }
            string[] showItems = dataSourceItems.Where(x => x.IsShow).Select(x => x.DataItem).ToArray();
            Items.Clear();
            Items.AddRange(showItems);
        }

        public int GetSelectedIndex()
        {
            string selectedItem = SelectedItem.ToString();
            int selectedIndex = -1;

            for (int i = 0; i < dataSourceItems.Length; i++)
            {
                if (dataSourceItems[i].DataItem == selectedItem)
                {
                    selectedIndex = i;
                    break;
                }
            }

            return selectedIndex;
        }
        public void SetSelectedIndex(int index)
        {   
            SelectedIndex = index < Items.Count ? index : -1;
        }
    }

    public class ComboBoxDataItem
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public string DataItem { get; set; } = "";

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; } = false;
    }
}
