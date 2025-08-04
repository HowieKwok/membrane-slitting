using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using System.ComponentModel.Composition;
using System.ComponentModel;
using HalconDotNet;

namespace Plugin.CropImage
{

    [Category("其它")]
    [DisplayName("图像裁切")]
    //[ExportMetadata("measure1", "IPlugin")]
    [Export("CropImageObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    class CropImageObj : IPlugin
    {
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public PluginParamsBase PluginParams { get; set; } = new ParamsCropImage();
        CropAndTileOperator Operator = new CropAndTileOperator();
        public ParamsCropImage Cropparams;
        public bool ExecuteObj()
        {
            Cropparams = PluginParams as ParamsCropImage; //里氏转换    
            try
            {
                bool flagcrop = Operator.CropImagePart(Cropparams.ho_Image, Cropparams.colBegin, Cropparams.cropWidth, Cropparams.cropHeight, out Cropparams.ho_CropIamge_top, out Cropparams.ho_CropIamge_bottom);
                //HTuple w, h;
                //HOperatorSet.GetImageSize(Cropparams.ho_CropImage_Lastbottom, out w, out h);
                bool flagtile = Operator.TileImage(Cropparams.ho_CropImage_Lastbottom, Cropparams.ho_CropIamge_top, out Cropparams.ho_TiledIamge);

                Cropparams.ho_CropImage_Lastbottom = Cropparams.ho_CropIamge_bottom; //传递新底部图
                if (flagcrop && flagtile)
                {
                    return true;
                }
                else
                {
                    return false;
                }

                // Console.WriteLine("裁拼图结果完成");
            }
            catch (Exception ee)
            {
                return false;
            }
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
            this.PluginParams = (ParamsCropImage)param;
            Cropparams = PluginParams as ParamsCropImage; //里氏转换
        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }
    }
}
