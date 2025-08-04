using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CropPart
{
    [Category("图像裁切")]
    [DisplayName("图像裁切工具(槽位，侧基准边)")]
    //[ExportMetadata("measure1", "IPlugin")]
    [Export("CropPartObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class CropPartObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsCropPart();
        ParamsCropPart croppartParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public ImageInfo imageInfo_now = new ImageInfo();
        CropOperation cropOperation = new CropOperation();
        bool HasImagepart = false;
        public bool ExecuteObj()
        {
            try
            {
                croppartParams = PluginParams as ParamsCropPart; //里氏转换
                HasImagepart = false;
                if (croppartParams.list_ImageInfo.Count == 0)
                {
                    Console.WriteLine("--未加载图片--！");
                    return false;
                }

                HasImagepart = cropOperation.Crop_ImagePart(croppartParams.list_ImageInfo, croppartParams.PartCenterID, croppartParams.PartCenterRow, croppartParams.PartCenterCol,
                    croppartParams.PartSizeRow, croppartParams.PartSizeCol, croppartParams.CropShaftRow, croppartParams.CropShaftCol, 
                    out croppartParams.Image_part,out croppartParams.hv_ImagePartBeginID,out croppartParams.hv_ImagePartBeginRow,out croppartParams.hv_ImagePartBeginCol,
                    out croppartParams.hv_ImagePartEndID, out croppartParams.hv_ImagePartEndRow, out croppartParams.hv_ImagePartEndCol);

                if (HasImagepart)
                {
                  //  Console.WriteLine("裁拼小图成功！裁拼编号："+ croppartParams.int_PartID + ",裁拼图名："+ croppartParams.str_PartName);
                    return true;
                }
                else
                {
                   // Console.WriteLine("裁拼小图失败");
                    return false;
                }
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug("裁拼模块异常"+ee.ToString());
               // Console.WriteLine("裁拼小图异常");
                return false;
            }
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
            this.PluginParams = (ParamsCropPart)param;
            croppartParams = PluginParams as ParamsCropPart; //里氏转换
        }

        public void GetResultParams(string varName, object obj)
        {
            Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
            proc.AddOutPutParam(moduleInfo.ModuleName, varName, obj);
        }

        public void IniObj()
        {
           
        }

        public void DisPoseObj()
        {
           
        }
    }
}
