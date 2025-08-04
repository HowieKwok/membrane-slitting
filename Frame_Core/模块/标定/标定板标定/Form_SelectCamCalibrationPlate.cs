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
    public partial class Form_SelectCamCalibrationPlate : Form
    {
        Dictionary<CameraEnum, CameraManager> cameraManager;
        Form_CalibrationPlate form_CalibrationPlate = new Form_CalibrationPlate() { FormBorderStyle = FormBorderStyle.None, TopLevel = false, Dock = DockStyle.Fill };
        public Form_SelectCamCalibrationPlate(Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
            cameraManager = _cameraManager;
        }

        private void Form_SelectCamCalibrationPlate_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_CamSelect.Items.Clear();
                foreach (var item in cameraManager)
                {
                    cbx_CamSelect.Items.Add(item.Value.m_CamParam.CameraName.ToString());
                }
                form_CalibrationPlate.Show();
                panel2.Controls.Clear();
                panel2.Controls.Add(form_CalibrationPlate);
                cbx_CamSelect.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void Form_SelectCamCalibrationPlate_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                form_CalibrationPlate.Close();
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
                form_CalibrationPlate.SetParam(cameraManager[(CameraEnum)cbx_CamSelect.SelectedIndex].param_CalibrationPlate);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
