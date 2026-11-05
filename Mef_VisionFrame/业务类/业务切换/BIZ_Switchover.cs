using Eholly.ZChart;
using Frame_Core;
using LogTool;

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
        public static IBizWorkerBase IBIZWorkerBase;

        public static Control UI_DataShowForm;

        public static void InitBizDataAndForm()
        {
          


            Form_Main.FormMain.相机标定ToolStripMenuItem.Visible = false;


            //if (Project.Instance.systemInfo.CraftType == MaterialCrafEnum.PFT清洗机)
            //{
             
            //    BIZ_DataWorker_WidthLaser BIZ_DataWorker = new BIZ_DataWorker_WidthLaser(UI_DataShowForm as Form_WidthLaserDetect);
            //    BIZ_DataWorker.BIZ_Variable.dic_EAproductsInfo_A = ((PFTCleanProParams)Project.Instance.baseProDevice.m_DicCamOperaParam[0]).dic_EAproductsInfo;
            //    BIZ_DataWorker.BIZ_Variable.dic_EAproductsInfo_B = ((PFTCleanProParams)Project.Instance.baseProDevice.m_DicCamOperaParam[1]).dic_EAproductsInfo;

            //    BIZ_DataWorker.BIZ_Variable.dic_EAproductsInfo_SimA = ((PFTCleanProParams)Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0]).dic_EAproductsInfo;
            //    BIZ_DataWorker.BIZ_Variable.dic_EAproductsInfo_SimB = ((PFTCleanProParams)Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1]).dic_EAproductsInfo;

            //    IBIZWorkerBase = BIZ_DataWorker;
            //    IBIZWorkerBase.InitVariable();
            //    IBIZWorkerBase.InitForm();

            //    Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_FrameCam1);
            //    Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_EACam1);


            //    Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_FrameCam2);
            //    Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_EACam2);




            //    Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_FrameCam1);
            //    Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_EACam1);

            //    Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_FrameCam2);
            //    Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBackCraft = new Action<ProductBase>(BIZ_DataWorker.ProcessingData_EACam2);

            

            //    Form_Main.FormMain.相机标定ToolStripMenuItem.Visible = false;
            
            //    Form_Main.FormMain.打标机软件通讯ToolStripMenuItem.Visible = false;
            //    Form_Main.FormMain.尺寸点检ToolStripMenuItem.Visible = true;
            
            //}

            ////主副机通讯回调函数注册
            //Project.Instance.slaveMachineManager.StartMJEvent -= Form_Main.FormMain.MainDetection.StartMojuan;
            //Project.Instance.slaveMachineManager.StartMJEvent += Form_Main.FormMain.MainDetection.StartMojuan;
            //Project.Instance.slaveMachineManager.StopMJEvent -= Form_Main.FormMain.MainDetection.StopMojuan;
            //Project.Instance.slaveMachineManager.StopMJEvent += Form_Main.FormMain.MainDetection.StopMojuan;
            //Project.Instance.slaveMachineManager.StartRunEvent -= Form_Main.FormMain.MainDetection.Run_Auto;
            //Project.Instance.slaveMachineManager.StartRunEvent += Form_Main.FormMain.MainDetection.Run_Auto;
            //Project.Instance.slaveMachineManager.StopRunEvent -= Form_Main.FormMain.MainDetection.StopRun;
            //Project.Instance.slaveMachineManager.StopRunEvent += Form_Main.FormMain.MainDetection.StopRun;



        }

        public static SoftwareState[] UpdateStatStrip()
        {
            SoftwareState[] softwareState = new SoftwareState[0];
            try
            {

             





            }
            catch (Exception ex)
            {

            }
            return softwareState;
        }



    }
}
