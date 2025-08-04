using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CamerSDK
{
    [Category("相机SDK")]
    [DisplayName("相机采图工具")]
    //[ExportMetadata("measure1", "IPlugin")]
    [Export("CamerSDkobj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
     public class CamerSDkobj : IPlugin
    {
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public PluginParamsBase PluginParams { get; set; } = new ParamsCamerSDK();
        ParamsCamerSDK camerSDKParams;
        public  int index = 0;
        public bool ExecuteObj()
        {
            Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
            camerSDKParams = PluginParams as ParamsCamerSDK; //里氏转换
            if (camerSDKParams.Is_Online)
            {

            }
            if (camerSDKParams.ImageList_OffLine.Count>0)
            {
                camerSDKParams.str_CamName = "图像"/* + camerSDKParams.ImageList.Count.ToString()*/;  
                //GetResultParams(camerSDKParams.str_CamName, camerSDKParams.ImageList_OffLine[index].ho_image);
                GetResultParams("图像ID", camerSDKParams.ImageList_OffLine[index].image_ID);
              //  proc.ProImageList = camerSDKParams.ImageList_OffLine;
                index++;
                Console.WriteLine("----图像传出成功--！");
                if (index == camerSDKParams.ImageList_OffLine.Count)
                {
                    //proc.ProImageList.Clear();
                    index = 0;
                }
               // camerSDKParams.ImageList.RemoveAt(0);
            }
            return true;
        }

        public void GetResultParams(string varName, object obj)
        {
            Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
            proc.AddOutPutParam(moduleInfo.ModuleName, varName, obj);
        }

        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath;
            if (PluginParams.Save())
            {
                return true;
            }
            return false;
        }

        public void SetParams(PluginParamsBase param)
        {
            this.PluginParams = (ParamsCamerSDK)param;
            camerSDKParams = PluginParams as ParamsCamerSDK; //里氏转换
        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }
    }
}
