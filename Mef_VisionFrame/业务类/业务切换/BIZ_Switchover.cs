using Eholly.ZChart;
using Frame_Core;
using LogTool;
using Mef_VisionFram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    /// <summary>
    /// 业务切换
    /// </summary>
    public static class BIZ_Switchover
    {
        #region 光源
        /// <summary>
        /// 控制光源开关
        /// </summary>
        public static bool LightOnOff = false;
        static object OpenColseLight_Lock = new object();
        /// <summary>
        /// 控制光源亮灭
        /// </summary>
        /// <param name="OpenOrClose"></param>
        public static void ControlLight(bool _isOpenLight)
        {
            try
            {
                lock (OpenColseLight_Lock)
                {
                    if (Project.Instance.lightManager != null)
                    {
                        for (int i = 0; i < Project.Instance.lightManager.Length; i++)
                        {
                            Project.Instance.lightManager[i].OpenColseLight(_isOpenLight);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 获取光源状态
        /// </summary>
        /// <returns></returns>
        public static bool GetLightState()
        {
            bool openOrClosing = true;
            try
            {
                if (Project.Instance.lightManager != null)
                {
                    for (int i = 0; i < Project.Instance.lightManager.Length; i++)
                    {
                        openOrClosing &= Project.Instance.lightManager[i].openOrCloseFlag;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return false;
            }
            return openOrClosing;
        }

        /// <summary>
        /// 获取光源连接状态
        /// </summary>
        /// <returns></returns>
        public static bool GetLightConnectState()
        {
            bool state = true;
            try
            {
                if (Project.Instance.lightManager != null)
                {
                    for (int i = 0; i < Project.Instance.lightManager.Length; i++)
                    {
                        state &= Project.Instance.lightManager[i].initState;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return false;
            }
            return state;
        }

        #endregion


        public static IBizWorkerBase IBIZWorkerBase;

        public static Control UI_DataShowForm;

        public static void InitBizDataAndForm()
        {
            Form_Main.FormMain.mark设置ToolStripMenuItem.Visible = false;
            Form_Main.FormMain.相机标定ToolStripMenuItem.Visible = false;
            Form_Main.FormMain.预定位平台标定ToolStripMenuItem.Visible = false;
            if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片预定位PP)
            {
                foreach (var item in Project.Instance.cameraManager)
                {
                    if (item.Value.initState)
                        if (!item.Value.Camera.IsCameraStatue)
                            item.Value.Camera.StartGrabImage();
                }

                UI_DataShowForm = new Form_Pretargeting() { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
                UI_DataShowForm.Show();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Clear();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Add(UI_DataShowForm);
                BIZ_DataWorker_Pretargeting BIZ_DataWorker = new BIZ_DataWorker_Pretargeting(UI_DataShowForm as Form_Pretargeting);
                IBIZWorkerBase = BIZ_DataWorker;
                IBIZWorkerBase.InitVariable();
                IBIZWorkerBase.InitForm();

            

                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                Project.Instance.baseProDevice.m_DicCamOperaParam[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);


                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);

                Form_Main.FormMain.相机标定ToolStripMenuItem.Visible = true;
                Form_Main.FormMain.预定位平台标定ToolStripMenuItem.Visible = true;
            }
            else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片对齐度OVH)
            {
                UI_DataShowForm = new Form_OverHang() { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
                UI_DataShowForm.Show();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Clear();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Add(UI_DataShowForm);
                BIZ_DataWorker_OverHang BIZ_DataWorker = new BIZ_DataWorker_OverHang(UI_DataShowForm as Form_OverHang);
                IBIZWorkerBase = BIZ_DataWorker;
                IBIZWorkerBase.InitVariable();
                IBIZWorkerBase.InitForm();

                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                Project.Instance.baseProDevice.m_DicCamOperaParam[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);

                Project.Instance.baseProDevice.m_DicCamOperaParam[4].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam5);
                Project.Instance.baseProDevice.m_DicCamOperaParam[5].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam6);
                Project.Instance.baseProDevice.m_DicCamOperaParam[6].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam7);
                Project.Instance.baseProDevice.m_DicCamOperaParam[7].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam8);

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[4].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam5);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[5].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam6);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[6].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam7);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[7].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam8);

                Form_Main.FormMain.mark设置ToolStripMenuItem.Visible = true;
                Form_Main.FormMain.相机标定ToolStripMenuItem.Visible = true;
            }
            else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阳极单面制片CS)
            {
                UI_DataShowForm = new Form_SingleAnode() { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
                UI_DataShowForm.Show();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Clear();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Add(UI_DataShowForm);
                BIZ_DataWorker_SingleAnode BIZ_DataWorker = new BIZ_DataWorker_SingleAnode(UI_DataShowForm as Form_SingleAnode);
                IBIZWorkerBase = BIZ_DataWorker;
                IBIZWorkerBase.InitVariable();
                IBIZWorkerBase.InitForm();

                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                //   Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1Prod);
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                // Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2Prod);

                Project.Instance.baseProDevice.m_DicCamOperaParam[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);
                Project.Instance.baseProDevice.m_DicCamOperaParam[4].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam5);


                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                //  Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1Prod);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                // Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2Prod);

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[4].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam5);
            }
            else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阳极双面制片CD)
            {
                UI_DataShowForm = new Form_DoubleAnode() { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
                UI_DataShowForm.Show();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Clear();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Add(UI_DataShowForm);
                BIZ_DataWorker_DoubleAnode BIZ_DataWorker = new BIZ_DataWorker_DoubleAnode(UI_DataShowForm as Form_DoubleAnode);
                IBIZWorkerBase = BIZ_DataWorker;
                IBIZWorkerBase.InitVariable();
                IBIZWorkerBase.InitForm();

                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                //  Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1Prod);
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                // Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2Prod);

                Project.Instance.baseProDevice.m_DicCamOperaParam[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);
                Project.Instance.baseProDevice.m_DicCamOperaParam[4].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam5);
                Project.Instance.baseProDevice.m_DicCamOperaParam[5].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam6);
                Project.Instance.baseProDevice.m_DicCamOperaParam[6].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam7);


                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                //   Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1Prod);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                //  Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2Prod);

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[4].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam5);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[5].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam6);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[6].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam7);
            }
            else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阴极双面制片AD)
            {
                UI_DataShowForm = new Form_DoubleCathode() { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
                UI_DataShowForm.Show();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Clear();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Add(UI_DataShowForm);
                BIZ_DataWorker_DoubleCathode BIZ_DataWorker = new BIZ_DataWorker_DoubleCathode(UI_DataShowForm as Form_DoubleCathode);
                IBIZWorkerBase = BIZ_DataWorker;
                IBIZWorkerBase.InitVariable();
                IBIZWorkerBase.InitForm();

                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                //  Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1Prod);
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                //  Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2Prod);

                Project.Instance.baseProDevice.m_DicCamOperaParam[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);
                Project.Instance.baseProDevice.m_DicCamOperaParam[4].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam5);
                Project.Instance.baseProDevice.m_DicCamOperaParam[5].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam6);
                Project.Instance.baseProDevice.m_DicCamOperaParam[6].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam7);


                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1);
                //  Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam1Prod);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2);
                // Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam2Prod);

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam3);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam4);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[4].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam5);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[5].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam6);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[6].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_Cam7);
            }
            else if(Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.PFT清洗机)
            {
                UI_DataShowForm = new Form_Narrow() { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
                UI_DataShowForm.Show();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Clear();
                Form_Main.FormMain.MainDetection.panel_DataShow.Controls.Add(UI_DataShowForm);
                BIZ_DataWorker_Narrow BIZ_DataWorker = new BIZ_DataWorker_Narrow(UI_DataShowForm as Form_Narrow);
                BIZ_DataWorker.BIZ_Variable.dic_EAproductsInfo_A= ((PFTCleanProParams)Project.Instance.baseProDevice.m_DicCamOperaParam[0]).dic_EAproductsInfo;
                BIZ_DataWorker.BIZ_Variable.dic_EAproductsInfo_B= ((PFTCleanProParams)Project.Instance.baseProDevice.m_DicCamOperaParam[1]).dic_EAproductsInfo;

                BIZ_DataWorker.BIZ_Variable.dic_EAproductsInfo_SimA = ((PFTCleanProParams)Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0]).dic_EAproductsInfo;
                BIZ_DataWorker.BIZ_Variable.dic_EAproductsInfo_SimB = ((PFTCleanProParams)Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1]).dic_EAproductsInfo;

                IBIZWorkerBase = BIZ_DataWorker;
                IBIZWorkerBase.InitVariable();
                IBIZWorkerBase.InitForm();

                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_FrameCam1);
                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_EACam1);
        

                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_FrameCam2);
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_EACam2);

                Project.Instance.baseProDevice.m_DicCamOperaParam[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData3D_A);
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData3D_B);


                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_FrameCam1);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_EACam1);

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_FrameCam2);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_EACam2);

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData3D_A);
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData3D_B);

                Form_Main.FormMain.相机标定ToolStripMenuItem.Visible = false;
                Form_Main.FormMain.打标机软件通讯ToolStripMenuItem.Visible = true;
                Form_Main.FormMain.尺寸点检ToolStripMenuItem.Visible = true;
                Form_Main.FormMain.纠偏设置ToolStripMenuItem.Visible = true;
             
            }



            if (IBIZWorkerBase != null)
            {
                Form_Main.FormMain.MainDetection.StartMojuanEvent -= IBIZWorkerBase.StartMojuan;
                Form_Main.FormMain.MainDetection.StartMojuanEvent += IBIZWorkerBase.StartMojuan;

                Form_Main.FormMain.MainDetection.StopMojuanEvent -= IBIZWorkerBase.StopMojuan;
                Form_Main.FormMain.MainDetection.StopMojuanEvent += IBIZWorkerBase.StopMojuan;

                Form_Main.FormMain.MainDetection.StartRunEvent -= IBIZWorkerBase.StartRun;
                Form_Main.FormMain.MainDetection.StartRunEvent += IBIZWorkerBase.StartRun;

                Form_Main.FormMain.MainDetection.StopRunEvent -= IBIZWorkerBase.StopRun;
                Form_Main.FormMain.MainDetection.StopRunEvent += IBIZWorkerBase.StopRun;

                Form_Main.FormMain.MainDetection.AlarmResetEvent -= IBIZWorkerBase.AlarmReset;
                Form_Main.FormMain.MainDetection.AlarmResetEvent += IBIZWorkerBase.AlarmReset;
            }


        }

        public static SoftwareState[] UpdateStatStrip()
        {
            SoftwareState[] softwareState = new SoftwareState[0];
            try
            {

                //检测用户登录时间
                if(Project.Instance.activeObject != null && Project.Instance.activeObject.GetType() == typeof(UC_MainDetection) )
                {
                    Project.Instance.userManage.MonitorLogonOutTime();
                }

                string userType = LanguageProject.GetLangeuageAttribute(StatusStripEnum.当前用户类型, Project.Instance.systemInfo.m_CurrentLanguageType) +
                     LanguageProject.GetLangeuageAttribute(Project.Instance.userManage.userData.CurrentUser, Project.Instance.systemInfo.m_CurrentLanguageType);
                string userID = LanguageProject.GetLangeuageAttribute(StatusStripEnum.当前用户ID, Project.Instance.systemInfo.m_CurrentLanguageType) + Project.Instance.userManage.userData.CurrentUserID;

                NStatusStrip.AddSoftwareState(userType, StatusStripStruct.Normal, ref softwareState);
                NStatusStrip.AddSoftwareState(userID, StatusStripStruct.Normal, ref softwareState);

                //相机
                foreach (var item in Project.Instance.cameraManager)
                {
                    if (item.Value.Camera == null)
                    {
                        continue;
                    }
                    bool b = item.Value.Camera.IsCameraOpen && item.Value.initState;
                    NStatusStrip.AddSoftwareState(item.Value.m_CamParam.CameraName, b ? 1 : 0, ref softwareState);
                }

                //光源
                for (int i = 0; i < Project.Instance.lightManager.Length; i++)
                {
                    NStatusStrip.AddSoftwareState(Project.Instance.lightManager[i].LightParam.LightName, Project.Instance.lightManager[i].initState ? 1 : 0, ref softwareState);
                }

                //仪器
                //设备PLC
                NStatusStrip.AddSoftwareState("设备PLC", Project.Instance.cutOverlayPLCManage.InitState ? 1 : 0, ref softwareState);

                //仪器
                //设备PLC
                NStatusStrip.AddSoftwareState("打标机", Project.Instance.dbjCardManager.initState ? 1 : 0, ref softwareState);




                //磁盘检测
                //检测磁盘状态
                bool disk_flag = StateManage.CheckDriveFreeSpace(out int diskState, out string diskSpace);
                diskSpace = LanguageProject.GetLangeuageAttribute(StatusStripEnum.磁盘剩余, Project.Instance.systemInfo.m_CurrentLanguageType) + diskSpace;
                NStatusStrip.AddSoftwareState(diskSpace, diskState, ref softwareState);

            }
            catch (Exception ex)
            {

            }
            return softwareState;
        }



    }
}
