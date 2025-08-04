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
    public partial class EmulationSetForm : Form
    {
        Dictionary<CameraEnum, CameraManager> cameraManager;
        List<EmulationPathForm> emulationPathForms = new List<EmulationPathForm>(); 
        public EmulationSetForm(Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
            cameraManager = _cameraManager;
        }

        private void EmulationSerForm_Load(object sender, EventArgs e)
        {

            try
            {
                TableLayoutPanel TablePanel = tableLayoutPanel_Main;
                TablePanel.AutoScroll = true;
                TablePanel.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(TablePanel, true);

                TablePanel.Controls.Clear();
                TablePanel.ColumnCount = 1;
                TablePanel.RowCount = cameraManager.Count;

                //tablePanel.ColumnStyles.Clear();
                //tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

                TablePanel.RowStyles.Clear();
                for (int i = 0; i < TablePanel.RowCount; i++)
                {
                    TablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
                }


                foreach (var item in cameraManager)
                {
                    emulationPathForms.Add(new EmulationPathForm(item.Value) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false });
                    emulationPathForms[emulationPathForms.Count - 1].Show();
                    TablePanel.Controls.Add(emulationPathForms[emulationPathForms.Count - 1]);
                }
            
            }
            catch (Exception)
            {
             
            }

        }

        private void EmulationSerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
          
            try
            {
                foreach (var item in cameraManager)
                {
                    item.Value.m_CamParam.Save();
                    item.Value.m_ImageSimManage.ReadImageSimPath(item.Value.m_CamParam.EmulationPath);
                }

                for (int i = 0; i < emulationPathForms.Count; i++)
                {
                    emulationPathForms[i].Close();
                }
            }
            catch (Exception)
            {

            }
           
        }


    }
}
