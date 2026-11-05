using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Frame_Core
{
    public partial class FormPointInspection : Form
    {

        Param_PointInspection param;

        public FormPointInspection(Param_PointInspection _param)
        {
            InitializeComponent();
     
            param = _param;
        }
        private void FormPointInspection_Load(object sender, EventArgs e)
        {
            CameraEnum cameraEnum = CameraEnum.Cam1;
            UpdateUCCameraCard(CameraEnum.Cam1, ucCameraCard1);
            UpdateUCCameraCard(CameraEnum.Cam2, ucCameraCard2);
        
            timer1.Enabled = true;

            ShowFormParam();
        }
        private void FormPointInspection_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!once_Size || !once_Defect)
            {
                MessageBox.Show("正在进行点检无法关闭窗口");
                e.Cancel = true;
                return;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            UpdateUCCameraCard(CameraEnum.Cam1, ucCameraCard1);
            UpdateUCCameraCard(CameraEnum.Cam2, ucCameraCard2);
          
            timer1.Enabled = true;
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Project.Instance.paramPathSet.pointCheckRootPath);
            }
            catch (Exception)
            {

            }
        }
        public static void UpdateUCCameraCard(CameraEnum cameraEnum, UCCameraCard ucCameraCard)
        {
            ucCameraCard.Init(cameraEnum, Service_Camera.Get_CameraRelevancy((int)cameraEnum));
            ucCameraCard.lb_CamName.Text = cameraEnum.ToString();

            ucCameraCard.lb_SizePointInspection.Text = "尺寸点检";
            ucCameraCard.lb_SizePointInspectionTime.Text = Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_PointInspectionStateParam.djLastOKTime_Size.ToString("yyyy-MM-dd HH:mm:ss");
            ucCameraCard.lb_SizePointInspectionState.Text = LanguageProject.GetLangeuageAttribute((PointInspectionStateEnum)Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_PointInspectionStateParam.PointInspectionState_Size, Project.Instance.systemInfo.m_CurrentLanguageType);
            if (Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_PointInspectionStateParam.PointInspectionState_Size == (int)PointInspectionStateEnum.已忽略)
            {
                ucCameraCard.HidSizePointInspectionFunction();
            }

            ucCameraCard.lb_DefectPointInspection.Text = "缺陷点检";
            ucCameraCard.lb_DefectPointInspectionTime.Text = Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_PointInspectionStateParam.djLastOKTime_Defect.ToString("yyyy-MM-dd HH:mm:ss");
            ucCameraCard.lb_DefectPointInspectionState.Text = LanguageProject.GetLangeuageAttribute((PointInspectionStateEnum)Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_PointInspectionStateParam.PointInspectionState_Defect, Project.Instance.systemInfo.m_CurrentLanguageType);
            if (Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_PointInspectionStateParam.PointInspectionState_Defect == (int)PointInspectionStateEnum.已忽略)
            {
                ucCameraCard.HideDefectPointInspectionFunction();
            }
        }

        bool once_Size = true;
        private void btn_StartSizePointInspection_Click(object sender, EventArgs e)
        {
            if (!once_Size) return;
            once_Size = false;

            Thread thread = new Thread(new ThreadStart(Execute_Size));
            thread.IsBackground = true;
            thread.Start();

        }
        public void Execute_Size()
        {
            ExecuteResult executeResult = new ExecuteResult();
            HTuple start = null, end = null;
            HOperatorSet.CountSeconds(out start);
            try
            {
                string str = "尺寸点检";

                string filePath = Project.Instance.paramPathSet.pointCheckRootPath + DateTime.Now.ToString("yyyy.MM.dd_HH-mm-ss-fff") + str;
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                SizePointInspectionWorks.OnekeyPointInspectionFlow(CameraEnum.Cam1, filePath + $"\\{str}_{CameraEnum.Cam1}", out bool isPass_Cam1, out List<RowData[]> sizeDatas_Cam1);
                SizePointInspectionWorks.OnekeyPointInspectionFlow(CameraEnum.Cam2, filePath + $"\\{str}_{CameraEnum.Cam2}", out bool isPass_Cam2, out List<RowData[]> sizeDatas_Cam2);

                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.IsSuccess = false;
                executeResult.Message = ex.Message;
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                HOperatorSet.CountSeconds(out end);
                executeResult.Time = ((end - start) * 1000).D;
                executeResult.Time = Math.Round(executeResult.Time, 2);
            }
            once_Size = true;
        }

        bool once_Defect = true;
        private void btn_StartDefectPointInspection_Click(object sender, EventArgs e)
        {
            if (!once_Defect) return;
            once_Defect = false;

            Thread thread = new Thread(new ThreadStart(Execute_Defect));
            thread.IsBackground = true;
            thread.Start();
        }

        public void Execute_Defect()
        {
            ExecuteResult executeResult = new ExecuteResult();
            HTuple start = null, end = null;
            HOperatorSet.CountSeconds(out start);
            try
            {
                //DefectPointInspectionManage.CreationDefectFolder(M_CameraRelevancies[CameraEnum.Cam1].m_DefectPointInspectionManage.defectPointInspectionParam.PointCheckImgPath, CameraEnum.Cam1, Project.Instance.craftInfo);
                //DefectPointInspectionManage.CreationDefectFolder(M_CameraRelevancies[CameraEnum.Cam2].m_DefectPointInspectionManage.defectPointInspectionParam.PointCheckImgPath, CameraEnum.Cam2, Project.Instance.craftInfo);
                //DefectPointInspectionManage.CreationDefectFolder(M_CameraRelevancies[CameraEnum.Cam3].m_DefectPointInspectionManage.defectPointInspectionParam.PointCheckImgPath, CameraEnum.Cam3, Project.Instance.craftInfo);

                //string str = LanguageProject.GetLangeuageAttribute_ChineseAndCurrentLanguage(PointInspectionTypeEnum.缺陷点检, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
                //string filePath = Project.Instance.paramPathSet.pointCheckRootPath + DateTime.Now.ToString("yyyy.MM.dd_HH-mm-ss-fff") + str;
                //if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                //DefectPointInspectionWorks.OnekeyPointInspectionFlow(CameraEnum.Cam1, filePath + $"\\{str}_{CameraEnum.Cam1}", out bool isPass_Cam1, out List<RowData[]> sizeDatas_Cam1);
                //DefectPointInspectionWorks.OnekeyPointInspectionFlow(CameraEnum.Cam2, filePath + $"\\{str}_{CameraEnum.Cam2}", out bool isPass_Cam2, out List<RowData[]> sizeDatas_Cam2);
                //DefectPointInspectionWorks.OnekeyPointInspectionFlow(CameraEnum.Cam3, filePath + $"\\{str}_{CameraEnum.Cam3}", out bool isPass_Cam3, out List<RowData[]> sizeDatas_Cam3);

                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.IsSuccess = false;
                executeResult.Message = ex.Message;
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                HOperatorSet.CountSeconds(out end);
                executeResult.Time = ((end - start) * 1000).D;
                executeResult.Time = Math.Round(executeResult.Time, 2);
            }
            once_Defect = true;
        }



        public void ShowFormParam()
        {
            ControlTool.SetParamShow(dtp_dtCaliTimeBaiban, ref param.djCaliTimeBaiban);
            ControlTool.SetParamShow(dtp_dtCaliTimeYeBan, ref param.djCaliTimeYeBan);
            ControlTool.SetParamShow(cbx_djEnabled, ref param.djEnabled);
        }

        public void GetFormParam()
        {
            ControlTool.GetParamSave(dtp_dtCaliTimeBaiban, ref param.djCaliTimeBaiban);
            ControlTool.GetParamSave(dtp_dtCaliTimeYeBan, ref param.djCaliTimeYeBan);
            ControlTool.GetParamSave(cbx_djEnabled, ref param.djEnabled);
        }

        private void btn_SaveParam_Click(object sender, EventArgs e)
        {
            GetFormParam();
            param.Save();
        }
    }


}
