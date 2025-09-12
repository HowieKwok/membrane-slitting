using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Eholly.Lib.Controls;

using EHolly.FormControl;

using FlyCapture2Managed;

using Frame_Core;

namespace Mef_VisionFrame
{

    public partial class Form_WidthLaserDetect : Form
    {
        public DynamicDataPanel DpModelDetectProcessCam1//也可实现自己的类重载此基类来配置相关参数
        {
            get
            {
                return Dp_DetectProcessCam1;
            }
        }

        public DynamicDataPanel DpModelDetectProcessCam2  //也可实现自己的类重载此基类来配置相关参数
        {
            get
            {
                return Dp_DetectProcessCam2;
            }
        }

        public DynamicDataPanel DpModelSizeConfig  //也可实现自己的类重载此基类来配置相关参数
        {
            get
            {
                return Dp_DetectSizeConfig;
            }
        }

        public DynamicDataPanel DpModelEASize //也可实现自己的类重载此基类来配置相关参数
        {
            get
            {
                return Dp_EASize;
            }
        }

        public DynamicDataPanel DpModelEADetect//也可实现自己的类重载此基类来配置相关参数
        {
            get
            {
                return Dp_EADetect;
            }
        }

        public DynamicDataPanel DpModelEADetectSize  //也可实现自己的类重载此基类来配置相关参数
        {
            get
            {
                return Dp_EASizeDetect;
            }
        }

        public Action<object, EventArgs> CobSelectedIndexChangedAction;

        public DataGridView Dgv_StatsEA
        {
            get
            {
                return Dgv_Static;
            }
        }

        private Timer Timer_Narrow { get; set; }

        private int SliceIndex = 0;
        private int[] GrooveIndex = { 1 };


        public void SetGrooveChart(int _sliceIndex, int[] _grooveIndex)
        {
            SliceIndex = _sliceIndex;
            GrooveIndex = _grooveIndex;
            if (_grooveIndex?.Length > 0)
            {
                List<string> strings = new List<string>();
                for (int i = 1; i <= _sliceIndex; i++)
                {
                    strings.Add($"第{i}条");
                }
                Cob_SliceIndex.DataSource = strings;
                Cob_SliceIndex.SelectedIndex = 0;

                strings = new List<string>();
                for (int i = 1; i <= _grooveIndex[0]; i++)
                {
                    strings.Add($"第{i}区");
                }

                Cob_GrooveIndex.DataSource = strings;
                Cob_GrooveIndex.SelectedIndex = 0;
            }

        }
        object Cob_Slice = new object();
        public int GetCob_SliceIndex()
        {
            lock (Cob_Slice)
            {
                int index = 0;
                this.Invoke(new Action(() => { index = Cob_SliceIndex.SelectedIndex; }));
                return index;
            }

        }
        object Cob_Groove = new object();
        public int GetCob_GrooveIndex()
        {
            lock (Cob_Groove)
            {
                int index = 0;
                this.Invoke(new Action(() => { index = Cob_GrooveIndex.SelectedIndex; }));
                return index;
            }

        }



        public Form_WidthLaserDetect()
        {
            InitializeComponent();

            splitContainer1.SplitterDistance = (int)(splitContainer1.Width * 2.3) / 4;
           
            Dp_DetectProcessCam1.DataListView.MouseDoubleClick -= listView1_DoubleClick;
            Dp_DetectProcessCam1.DataListView.MouseDoubleClick += listView1_DoubleClick;
            Dp_DetectProcessCam2.DataListView.MouseDoubleClick -= listView1_DoubleClick;
            Dp_DetectProcessCam2.DataListView.MouseDoubleClick += listView1_DoubleClick;

            Timer_Narrow = new Timer();
            Timer_Narrow.Interval = 1000;
            // 订阅 Tick 事件
            Timer_Narrow.Tick += (sender, e) =>
            {

                if (Project.Instance.Is_Run)
                {
                    tableLayoutPanel1.Enabled = false;

                }
                else
                {
                    tableLayoutPanel1.Enabled = true;

                }

            };

            // 启动定时器
            Timer_Narrow.Start();
        }






        private void CobIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            CobSelectedIndexChangedAction?.Invoke(sender, e);
        }

        private void Cob_SliceIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            //判断是否是这个Combox是否是Cob_SliceIndex这个
            if (sender is ComboBox comboBox && comboBox.Name == "Cob_SliceIndex")
            {
                List<string> strings = new List<string>();

                if (GrooveIndex?.Length > 0)
                {
                    for (int i = 1; i <= GrooveIndex[comboBox.SelectedIndex]; i++)
                    {
                        strings.Add($"第{i}区");
                    }
                    Cob_GrooveIndex.DataSource = strings;
                    Cob_GrooveIndex.SelectedIndex = 0;
                }

            }
        }


        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (sender is ListView)
            {
                ListView dp = sender as ListView;

                // 使用 SelectedIndices 而不是 SelectedItems
                if (dp.SelectedIndices.Count > 0)
                {
                    // 获取第一个选中项的索引
                    int selectedIndex = dp.SelectedIndices[0];

                    // 通过索引获取 ListViewItem（这会触发 RetrieveVirtualItem 事件）
                    ListViewItem selectedItem = dp.Items[selectedIndex];

                    // 获取"是否保存"和"路径"列的索引
                    int saveIndex = 8;
                    int pathIndex = 10;

                    if (saveIndex >= 0 && pathIndex >= 0)
                    {
                        string saveStatus = selectedItem.SubItems[saveIndex].Text;
                        string imagePath = selectedItem.SubItems[pathIndex].Text;

                        if (saveStatus.Equals("True", StringComparison.OrdinalIgnoreCase))
                        {
                            if (File.Exists(imagePath))
                            {
                                // 使用默认程序打开图片
                                System.Diagnostics.Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });
                            }
                            else
                            {
                                MessageBox.Show("图片文件不存在！");
                            }
                        }
                    }
                }
            }


        }
    }
}
