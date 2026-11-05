using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public partial class Form_SelectCamCheckerboard : Form
    {

        Form_Checkerboard form_Checkerboard = new Form_Checkerboard() { FormBorderStyle = FormBorderStyle.None, TopLevel = false, Dock = DockStyle.Fill };
        public Form_SelectCamCheckerboard()
        {
            InitializeComponent();          
        }
        private void Form_SelectCamCheckerboard_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_CamSelect.Items.Clear();
                cbx_CamSelect.Items.AddRange(Service_Camera.GetCam_Names());
                form_Checkerboard.Show();
                panel2.Controls.Clear();
                panel2.Controls.Add(form_Checkerboard);
                cbx_CamSelect.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void Form_SelectCamCheckerboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                form_Checkerboard.Close();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void cbx_CamSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                form_Checkerboard.SetParam(Service_Camera.Get_CameraRelevancy(cbx_CamSelect.SelectedIndex).param_Checkerboard);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

    
    
    }
}
