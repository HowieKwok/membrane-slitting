using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Frame_Core;

namespace Mef_VisionFrame
{


    public partial class Form_Narrow : Form
    {

        public ListView ListView_A { get; set; }
        public ListView ListView_B { get; set; }
        public ListView ListView_AB{ get; set; }

        public ListView ListView_AA { get; set; }
        public ListView ListView_BB { get; set; }

        public Timer Timer_Narrow { get; set; }
        //public ListView ListView_AABB { get; set; }
        public Form_Narrow()
        {
            InitializeComponent();
            // 设置自定义绘制模式
            //tabChartControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            ListView_A = new ListView();
            ListView_B = new ListView();
            ListView_AB = new ListView();

            ListView_AA = new ListView();
            ListView_BB = new ListView();
           // ListView_AABB = new ListView();

            ListView_AA.Dock = DockStyle.Fill;
            ListView_BB.Dock = DockStyle.Fill;
           // ListView_AABB.Dock = DockStyle.Fill;

            //ListView_AA.BackColor = Color.Gray;
            //ListView_BB.BackColor = Color.Gray;
            //ListView_AABB.BackColor = Color.Gray;

            GroupEA_AA.Controls.Add(ListView_AA);
            GroupEA_BB.Controls.Add(ListView_BB);
            //GroupEA_AABB.Controls.Add(ListView_AABB);

            ListView_A.Dock = DockStyle.Fill;
            ListView_B.Dock = DockStyle.Fill;
            ListView_AB.Dock = DockStyle.Fill;

            //ListView_A.BackColor = Color.Gray;
            //ListView_B.BackColor = Color.Gray;
            //ListView_AB.BackColor = Color.Gray;

            GroupEA_A.Controls.Add(ListView_A);
            
            GroupEA_B.Controls.Add(ListView_B);
           
            GroupEA_AB.Controls.Add(ListView_AB);


            splitContainer1.SplitterDistance = 320;
            splitContainer2.SplitterDistance = 375;


            Timer_Narrow = new Timer();
            Timer_Narrow.Interval = 1000;
            // 订阅 Tick 事件
            Timer_Narrow.Tick += (sender, e) =>
            {
                if (Project.Instance.Is_Run)
                {
                    tableLayoutPanel3.Enabled = false;
                    tableLayoutPanel23.Enabled = false;
                    tableLayoutPanel25.Enabled = false;
                }
                else
                {
                    tableLayoutPanel3.Enabled = true;
                    tableLayoutPanel23.Enabled = true;
                    tableLayoutPanel25.Enabled = true;
                }

            };

            // 启动定时器
            Timer_Narrow.Start();

            // tabControl1.TabPages.Remove(tabPage2);
        }

        private void lbTotallength_Click(object sender, EventArgs e)
        {
            MessageNotify.AddDebugLog("帆帆帆帆帆帆帆帆");
        }

        private void tabChartControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            SolidBrush _Brush = new SolidBrush(Color.Black);//单色画刷
            RectangleF _TabTextArea = (RectangleF)tabChartControl.GetTabRect(e.Index);//绘制区域
            StringFormat _sf = new StringFormat();//封装文本布局格式信息
            _sf.LineAlignment = StringAlignment.Center;
            _sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(tabChartControl.Controls[e.Index].Text, SystemInformation.MenuFont, _Brush, _TabTextArea, _sf);
        }
        //tabControl1.TabPages.Remove(tabPage2);

    }
    }

