using Frame_Core;
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
    public partial class RealTimeMinImageForm : Form
    {
        /// <summary>
        /// 小图显示
        /// </summary>
        MinImageShowForm minImageShowForm = new MinImageShowForm();

        public CamMinImageForm camMinImageForm1;

        public CamMinImageForm camMinImageForm2;

        public CamMinImageForm camMinImageForm3;

        public CamMinImageForm camMinImageForm4;




        public RealTimeMinImageForm()
        {
            InitializeComponent();

            camMinImageForm1 = new CamMinImageForm(minImageShowForm) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
            camMinImageForm2 = new CamMinImageForm(minImageShowForm) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
            camMinImageForm3 = new CamMinImageForm(minImageShowForm) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
            camMinImageForm4 = new CamMinImageForm(minImageShowForm) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
        }

        private void RealTimeMinImageForm_Load(object sender, EventArgs e)
        {
            this.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this, true);
            tableLayoutPanel_Main.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel_Main, true);



            panel1.Controls.Clear();
            panel1.Controls.Add(camMinImageForm1);
            camMinImageForm1.Show();

            panel2.Controls.Clear();
            panel2.Controls.Add(camMinImageForm2);
            camMinImageForm2.Show();

            panel3.Controls.Clear();
            panel3.Controls.Add(camMinImageForm3);
            camMinImageForm3.Show();

            panel4.Controls.Clear();
            panel4.Controls.Add(camMinImageForm4);
            camMinImageForm4.Show();

            UpdateFormText(Project.Instance.cameraManager);
        }

        private void RealTimeMinImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //camImageDataForm1.Close();
            //camImageDataForm2.Close();
            //camImageDataForm3.Close();
            //camImageDataForm4.Close();
        }

        /// <summary>
        /// 刷新窗体显示文本
        /// </summary>
        public void UpdateFormText(Dictionary<CameraEnum, CameraManager> cameraManager)
        {
            camMinImageForm1.UpdateFormText("相机1直边毛刺");
            camMinImageForm2.UpdateFormText("相机1极耳毛刺");
            camMinImageForm3.UpdateFormText("相机2直边毛刺");
            camMinImageForm4.UpdateFormText("相机2极耳毛刺");
        }
    }
}
