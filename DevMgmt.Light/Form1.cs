using DevMgmt.Light.Models;
using DevMgmt.Light.View;
using DevMgmt.Light.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrameCommunity.DataOperation;

namespace DevMgmt.Light
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Light_Click(object sender, EventArgs e)
        {
            LightControllerParam Params = new LightControllerParam();
            Params = Params.ReadExtension<LightControllerParam>($"{System.Environment.CurrentDirectory}\\Param\\", $"LightAllParam");

            WpfLightDevWindow window = new WpfLightDevWindow(Params);
            window.ShowDialog();
        }
    }
}
