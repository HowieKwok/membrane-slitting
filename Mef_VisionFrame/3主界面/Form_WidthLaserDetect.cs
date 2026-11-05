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



using Frame_Core;

namespace Mef_VisionFrame
{

    public partial class Form_WidthLaserDetect : Form
    {
        public List<DynamicDataPanel> DpModelEASizeDataPanels = new List<DynamicDataPanel>();
        public ComboBox comboBox = new ComboBox();
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

        //public DynamicDataPanel DpModelEASize //也可实现自己的类重载此基类来配置相关参数
        //{
        //    get
        //    {
        //        return Dp_EASize;
        //    }
        //}

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
        private int nSpliteIndex = 2;

        bool bIsLeftSide = true;


        public void SetGrooveChart(int _sliceIndex, int[] _grooveIndex,int _nSpliteIndex,bool _isLeftSide=true)
        {
            SliceIndex = _sliceIndex;
            GrooveIndex = _grooveIndex;
            nSpliteIndex=_nSpliteIndex;
            bIsLeftSide=_isLeftSide;

            if (_grooveIndex?.Length > 0)
            {
                List<string> strings = new List<string>();
                for (int i = 1; i <= _sliceIndex; i++)
                {
                    strings.Add($"第{i}条");
                }
                Cob_SliceIndex.DataSource = strings;
                Cob_SliceIndex.SelectedIndex = _isLeftSide?0:(_nSpliteIndex+1);

                strings = new List<string>();
                for (int i = 1; i <= _grooveIndex[0]; i++)
                {
                    strings.Add($"第{i}区");
                }

                Cob_GrooveIndex.DataSource = strings;
                Cob_GrooveIndex.SelectedIndex = 0;
            }

            comboBox.Items.Clear();
            DpModelEASizeDataPanels.Clear();
            for(int CamIndex=0; CamIndex < 2; CamIndex++)
            {

                for (int i = 0; i < SliceIndex; i++)
                {

                    bool needShow = bIsLeftSide ? i <= nSpliteIndex
                                                : i > nSpliteIndex;
                    if (needShow)
                    {
                        var p = new DynamicDataPanel
                        {
                            Name = $"{(CameraEnum)CamIndex}CZ_" + (i + 1),
                            Dock = DockStyle.Fill,
                            Margin = new Padding(0),
                            Width = groupBox1.Width,
                            Height = groupBox1.Height,
                            Font = groupBox1.Font,
                        };
                        // p.MouseDoubleClick += groupBox1_MouseDoubleClick;
                        DpModelEASizeDataPanels.Add(p);
                        comboBox.Items.Add($"第{(CameraEnum)CamIndex}CZ_{i + 1}条");   // 中文序号更直观
                    }
                      
                }

            }

            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Font = new Font("微软雅黑", 9F);
            comboBox.Margin = new Padding(0);
            comboBox.Visible = false;                // 先藏起来
            comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            comboBox.Leave += (s, _) => comboBox.Visible = false; // 失焦也隐藏
            groupBox1.Controls.Add(comboBox);
            groupBox1.MouseDoubleClick += groupBox1_MouseDoubleClick;

            comboBox.SelectedIndex = 0;   // 默认显示第一条

        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex < 0) return;

            // 先清旧面板
            for (int i = groupBox1.Controls.Count - 1; i >= 0; i--)
                if (groupBox1.Controls[i] is DynamicDataPanel)
                    groupBox1.Controls.RemoveAt(i);

            // 加载新面板
            var p = DpModelEASizeDataPanels[comboBox.SelectedIndex];
            p.Dock = DockStyle.Fill;
            groupBox1.Controls.Add(p);


            // 隐藏 ComboBox
            //comboBox.Visible = false;
        }

        private void groupBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;


            comboBox.Location = new Point(groupBox1.Width - comboBox.Width - 10, -2);
            comboBox.Visible = true;
            comboBox.DroppedDown = true;   // 直接拉下列表
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
            Timer_Narrow.Interval = 500;
            // 订阅 Tick 事件
            Timer_Narrow.Tick += (sender, e) =>
            {
                MyMetrics.GetPrivateWorkingSetMB();
                MyMetrics.GetCpuPercentLikeTaskManager();

                lbTotallength.Text = $"当前运行内存：{MyMetrics.fWorkingSetMBValue.ToString("00000.00")} MB";
                lbOklength.Text = $"当前CPU：{MyMetrics.fCpuValue.ToString("0000.00")}%";
               
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
                if (comboBox.SelectedIndex >= 0)
                {
                    bool needShow = bIsLeftSide ? comboBox.SelectedIndex <= nSpliteIndex : comboBox.SelectedIndex > nSpliteIndex;
                    if (!needShow)
                    {
                      MessageNotify.AddDebugLog($"当前第{comboBox.SelectedIndex + 1}条不在显示范围内，请重新选择条");

                        if (bIsLeftSide)
                        {
                            comboBox.SelectedIndex = 0;
                        }
                        else
                        {
                            comboBox.SelectedIndex = nSpliteIndex + 1;
                        }
                    }
                }

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

        public Action OutDataAct;
        private void Btn_OutData_Click(object sender, EventArgs e)
        {
            OutDataAct?.Invoke();
        }


        object tablecontrol1SelectedIndex = new object();
        public int Gettablecontrol1SelectedIndext()
        {
            lock (tablecontrol1SelectedIndex)
            {
                return tabControl1.TabIndex;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // 开启双缓冲
                return cp;
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                //int currentIndex = e.TabPageIndex;
                ////获取上一个选中的tab索引
                //if (currentIndex == 2)
                //{
                //    tableLayoutPanel3.Visible = true;
                //    //Dp_EASize.Visible = true;
                //    Dp_EADetect.Visible = true;
                //    Dp_EASizeDetect.Visible = true;

                //}
                //else
                //{
                //    tableLayoutPanel3.Visible = false;
                //   // Dp_EASize.Visible = false;
                //    Dp_EADetect.Visible = false;
                //    Dp_EASizeDetect.Visible = false;
                //}

                //if (currentIndex == 2)
                //{
                //    tableLayoutPanel3.Visible = true;
                //}
                //else
                //{
                //    tableLayoutPanel3.Visible = false;
                //}

                //if (currentIndex == 3)
                //{
                //    tableLayoutPanel4.Visible = true;
                //}
                //else
                //{
                //    tableLayoutPanel4.Visible = false;
                //}
                //if (currentIndex == 4)
                //{
                //    tableLayoutPanel5.Visible = true;
                //}
                //else
                //{
                //    tableLayoutPanel5.Visible = false;
                //}


            }));
            //获取当前选中的tab索引


        }
    }
}
