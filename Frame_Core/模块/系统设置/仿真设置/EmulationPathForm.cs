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
    public partial class EmulationPathForm : Form
    {
        CameraManager  cameraManager;
        public EmulationPathForm(CameraManager _cameraManager)
        {
            InitializeComponent();
            cameraManager = _cameraManager;
        }

        private void EmulationPathForm_Load(object sender, EventArgs e)
        {
            linkLabel_EmulationPathLoad.Text = cameraManager.m_CamParam.CameraName;
            tb_EmulationPath.Text = cameraManager.m_CamParam.EmulationPath;
        }

        private void EmulationPathForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void linkLabel_EmulationPathLoad_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // 获取文件夹绝对路径    显示在 txtbox 控件里
                using (FolderBrowserDialog folder = new FolderBrowserDialog())
                {
                    if (Project.Instance.paramPathSet.ProductRootPath != null && System.IO.Directory.Exists(Project.Instance.paramPathSet.ProductRootPath))
                    {
                        folder.SelectedPath = Project.Instance.paramPathSet.ProductRootPath;
                    
                    }
                    if (folder.ShowDialog() == DialogResult.OK && folder.SelectedPath != "")
                    {
                        tb_EmulationPath.Text = folder.SelectedPath;
                        cameraManager.m_CamParam.EmulationPath = folder.SelectedPath;
                    }
                }
            
            }
            catch (Exception)
            {

            }
           
        }

        private void tb_EmulationPath_TextChanged(object sender, EventArgs e)
        {
            cameraManager.m_CamParam.EmulationPath = tb_EmulationPath.Text.Trim();
        }
    }
}
