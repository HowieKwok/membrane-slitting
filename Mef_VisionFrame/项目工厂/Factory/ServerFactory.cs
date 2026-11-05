using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public class ServerFactory : IServerFactory
    {
        public ServerBase WorkServerBase;

        public void Load(ModelFactory modelFactory)
        {
            MaterialCrafEnum crafEnum = Project.Instance.systemInfo.CraftType;
            if (crafEnum == MaterialCrafEnum.激光模切一体机)
            {
                Server_DieCuttingCut server_DieCuttingCut= new Server_DieCuttingCut(modelFactory.WorkModelBase as Model_DieCuttingCut); ;
                WorkServerBase = server_DieCuttingCut;
                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack -= server_DieCuttingCut.ALineCam_SourceImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBack += server_DieCuttingCut.ALineCam_SourceImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBackCraft -= server_DieCuttingCut.ALineCam_EAImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[0].ProductCallBackCraft += server_DieCuttingCut.ALineCam_EAImage_Process;

                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack -= server_DieCuttingCut.BLineCam_SourceImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBack += server_DieCuttingCut.BLineCam_SourceImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBackCraft -= server_DieCuttingCut.BLineCam_EAImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].ProductCallBackCraft += server_DieCuttingCut.BLineCam_EAImage_Process;

                Project.Instance.baseProDevice.m_DicCamOperaParam[2].ProductCallBack -= server_DieCuttingCut.纠偏检测数据处理_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[2].ProductCallBack += server_DieCuttingCut.纠偏检测数据处理_Process;

                Project.Instance.baseProDevice.m_DicCamOperaParam[3].ProductCallBack -= server_DieCuttingCut.SizeCam4_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].ProductCallBack += server_DieCuttingCut.SizeCam4_Process;

                Project.Instance.baseProDevice.m_DicCamOperaParam[4].ProductCallBack -= server_DieCuttingCut.SizeCam5_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[4].ProductCallBack += server_DieCuttingCut.SizeCam5_Process;

                Project.Instance.baseProDevice.m_DicCamOperaParam[5].ProductCallBack -= server_DieCuttingCut.SizeCam6_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam[5].ProductCallBack += server_DieCuttingCut.SizeCam6_Process;

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack -= server_DieCuttingCut.ALineCam_SourceImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBack += server_DieCuttingCut.ALineCam_SourceImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBackCraft -= server_DieCuttingCut.ALineCam_EAImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].ProductCallBackCraft += server_DieCuttingCut.ALineCam_EAImage_Process;
                                       
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack -= server_DieCuttingCut.BLineCam_SourceImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBack += server_DieCuttingCut.BLineCam_SourceImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBackCraft -= server_DieCuttingCut.BLineCam_EAImage_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].ProductCallBackCraft += server_DieCuttingCut.BLineCam_EAImage_Process;

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].ProductCallBack -= server_DieCuttingCut.纠偏检测数据处理_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].ProductCallBack += server_DieCuttingCut.纠偏检测数据处理_Process;

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].ProductCallBack -= server_DieCuttingCut.SizeCam4_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].ProductCallBack += server_DieCuttingCut.SizeCam4_Process;
                                       
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[4].ProductCallBack -= server_DieCuttingCut.SizeCam5_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[4].ProductCallBack += server_DieCuttingCut.SizeCam5_Process;
                                           
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[5].ProductCallBack -= server_DieCuttingCut.SizeCam6_Process;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[5].ProductCallBack += server_DieCuttingCut.SizeCam6_Process;

              
            }
        }

    }
}
