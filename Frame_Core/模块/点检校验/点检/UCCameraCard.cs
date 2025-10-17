using Basler.Pylon;
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
    public partial class UCCameraCard : UserControl
    {
        CameraEnum currentCamera = CameraEnum.Cam1;
        CameraManager currentCameraManager;
        public UCCameraCard()
        {
            InitializeComponent();      
        }
        public void Init(CameraEnum camera, CameraManager _cameraManager)
        {
            currentCamera = camera;
            currentCameraManager = _cameraManager;
        }
        private void UCCameraCard_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 隐藏尺寸点检调试功能
        /// </summary>
        public void HidSizePointInspectionFunction()
        {
            lb_SizePointInspection.Visible = false;
            lb_SizePointInspectionTime.Visible = false;
            lb_SizePointInspectionState.Visible = false;
            btn_SizePointInspectionDebug.Visible = false;
        }
        private void btn_SizePointInspectionDebug_Click(object sender, EventArgs e)
        {
            using (FormSizePointInspection2 form = new FormSizePointInspection2(currentCamera, currentCameraManager))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.Text= currentCamera.ToString();
                form.ShowDialog();
                
            }
        }
        /// <summary>
        /// 隐藏缺陷点检调试功能
        /// </summary>
        public void HideDefectPointInspectionFunction()
        {
            lb_DefectPointInspection.Visible = false;
            lb_DefectPointInspectionTime.Visible = false;
            lb_DefectPointInspectionState.Visible = false;
            btn_DefectPointInspectionDebug.Visible = false;
        }

        private void btn_DefectPointInspectionDebug_Click(object sender, EventArgs e)
        {
            using (FormDefectPointInspection form = new FormDefectPointInspection(currentCamera, currentCameraManager))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.Text = currentCamera.ToString();
                form.ShowDialog();

            }
        }
    }
}
