using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Eholly.Lib.Controls;

using EHolly.FormControl;

using LogTool;

namespace Frame_Core
{
    public class NDataPanelViewModel
    {
        /// <summary>
        /// 初始化表格
        /// </summary>asa
        /// <param name="listView"></param>
        /// <param name="Titles"></param>
        public static void Init(DataPanelViewModelBase dataPanelViewModel, string[] Titles, int _DataInterval = 85, int MaxDisplayCount = 200)
        {
            dataPanelViewModel.DataInterval = _DataInterval;
            dataPanelViewModel.MaxDisplayCount = MaxDisplayCount;
            List<DynamicHeader> list = new List<DynamicHeader>();
            foreach (var title in Titles)
            {  //当前#FFFFFF为白色
                list.Add(new DynamicHeader(title, title, 0, "#00FF00")); //这个字段用来测试多种背景颜色

            }

            dataPanelViewModel.SetHeaderDynamicBinding(list); //设置动态表头，注意，表头中的属性名要跟后面设置属性名值的属性名一样才能绑定 data.SetPropertyValue($"Value{i}", n);

            foreach (var title in Titles)
            {
                //当前背景色为#FFFFFF，为橙色
                dataPanelViewModel.SetMoreBackground(title, "#FFA500");
                //当前背景色为#FFFFFF，为红色
                dataPanelViewModel.SetMoreBackground(title, "#FF0000");
            }

        }


        public static void Init(DynamicDataPanel dataPanelViewModel, string[] Titles, int _DataInterval = 85, int MaxDisplayCount = 200)
        {
            //清空列和数据
            dataPanelViewModel.ReSetHeader();   //可以在任何线程中调用

            dataPanelViewModel.MaxDisplayCount = MaxDisplayCount;
            //数据刷新频率（毫秒）,这个属性如果固定可以直接在属性窗口设置
            dataPanelViewModel.DataInterval = _DataInterval;
            List<DynamicHeader> list = new List<DynamicHeader>();
            foreach (var title in Titles)
            {  //当前#FFFFFF为白色
                //开始设置标题
                dataPanelViewModel.AddHeader(title, title, -1, HorizontalAlignment.Center);  //根据内容自动适配列宽度，居中

            }
            dataPanelViewModel.AutoSetWidthContent = true;  //开启列宽度自适应（有列设置了宽度为-1时生效，并且是在最后没有新数据加入时生效，不实时）



        }

        public static void Add(DynamicDataPanel dataPanelViewModel, string[] Titles, RowData[] strs, bool isBeginInvoke = false)
        {
            if (dataPanelViewModel == null) return;

            if ((null == strs) || (strs.Length <= 0))
            {
                return;
            }
            if (Titles.Length <= 0 || Titles.Length != strs.Length)
            {
                MessageNotify.AddErrorLog($"dataPanelViewModel_Titles.Length_{Titles.Length}!= strs.Length_{strs.Length}");
                return;
            }
            //声明动态数据
            EHolly.FormControl.DynamicData data = new EHolly.FormControl.DynamicData();
           
            for (int i = 0; i < Titles.Length; i++)
            {
                data.SetValue(Titles[i], strs[i].dataStr);
               
                if (strs[i].resultType == ResultType.OK&& strs[i].mode==0)
                {
                    data.SetColor(Titles[i], Color.Green);
                }
                else if (strs[i].resultType == ResultType.Warn)
                {
                    data.SetColor(Titles[i], Color.Orange);
                }
                else if (strs[i].resultType == ResultType.NG)
                {
                    data.SetColor(Titles[i], Color.Red);
                }

            }

            dataPanelViewModel.AddData(data);
        }
        public static void Add(DataPanelViewModelBase dataPanelViewModel, string[] Titles, RowData[] strs, bool isBeginInvoke = false)
        {
            if (dataPanelViewModel == null) return;

            if ((null == strs) || (strs.Length <= 0))
            {
                return;
            }
            if (Titles.Length <= 0 || Titles.Length != strs.Length)
            {
                MessageNotify.AddErrorLog($"dataPanelViewModel_Titles.Length_{Titles.Length}!= strs.Length_{strs.Length}");
                return;
            }
            Eholly.Lib.Controls.DynamicData data = new Eholly.Lib.Controls.DynamicData();
            for (int i = 0; i < Titles.Length; i++)
            {
                data.SetPropertyValue(Titles[i], strs[i].dataStr);
                if (strs[i].resultType == ResultType.OK)
                {
                    //data.SetBackground(Titles[i], "#00FF00");
                }
                else if (strs[i].resultType == ResultType.Warn)
                {
                    data.SetBackground(Titles[i], "#FFA500");
                }
                else if (strs[i].resultType == ResultType.NG)
                {
                    data.SetBackground(Titles[i], "#FF0000");
                }

            }

            dataPanelViewModel.AddData(data);



        }




    }
}
