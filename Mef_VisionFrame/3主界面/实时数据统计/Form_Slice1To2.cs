using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_Slice1To2 : Form
    {
        /// <summary>
        /// 生产数据页签是否被选中
        /// </summary>
        public bool prodDataTabPageIsFocused = false;
        public Form_Slice1To2()
        {
            InitializeComponent();
            this.DoubleBuffered = true;//设置本窗体
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
        }

        private void Form_Slice1To2_Load(object sender, EventArgs e)
        {
            tabPage2.Parent = null;
            tabPage4.Parent = null;
            GetProdDataTabPageIsFocused();
        }

        private void GetProdDataTabPageIsFocused()
        {
            if (tabControl1.SelectedIndex == 1)
            {
                prodDataTabPageIsFocused = true;
            }
            else
            {
                prodDataTabPageIsFocused = false;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            tabPage2.Parent = null;
            tabPage4.Parent = null;
            GetProdDataTabPageIsFocused();

        }

        private void label5_Click(object sender, EventArgs e)
        {
            Form_OverHang form_OverHang = new Form_OverHang();
            form_OverHang.Show();
        }
    }
}
