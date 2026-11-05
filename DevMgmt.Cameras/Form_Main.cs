using DevMgmt.Cameras.Models;
using DevMgmt.Cameras.View;
using FrameCommunity.DataOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevMgmt.Cameras
{
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CamParam Params = new CamParam();
            Params = Params.ReadExtension<CamParam>($"{System.Environment.CurrentDirectory}\\Param\\", $"CamAllParam");


            WpfCameraDevWindow window = new WpfCameraDevWindow(Params);
            window.ShowDialog();
     
        }
    }
}
