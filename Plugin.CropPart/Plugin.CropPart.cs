using Frame_Core;
using FrameCommunity.Data;
using HalconDotNet;
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

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public bool ExecuteObj()
        {
            HasImagepart = false;
            HOperatorSet.CountSeconds(out HTuple start);
            try
            {
                croppartParams = PluginParams as ParamsCropPart; //里氏转换
               
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
            finally
            {
                HOperatorSet.CountSeconds(out HTuple end);
                croppartParams.m_fElapsedMs = (end.D - start.D) * 1000.0;
                croppartParams.m_bRunSuccess = HasImagepart;
               ExecuteCompleted?.Invoke(
                this,
                new PluginExecuteCompletedEventArgs(
                    croppartParams,
                    croppartParams.m_bRunSuccess,
                    croppartParams.m_strRunMessage,
                    (long)croppartParams.m_fElapsedMs));

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
            HOperatorSet.GenImageConst(out HObject img1, "byte", 16384, 4000);
            HOperatorSet.GenImageConst(out HObject img2, "byte", 16384, 4000);
            ParamsCropPart CropPartParams = PluginParams as ParamsCropPart; //里氏转换

            ImageInfo image1 = new ImageInfo();
            image1.ho_image = img1;
            image1.image_ID = 1;
            image1.imageHeight = 4000;
            image1.imageWidth = 2000;

            ImageInfo image2 = new ImageInfo();
            image2.ho_image = img2;
            image2.image_ID = 2;
            image2.imageHeight = 4000;
            image2.imageWidth = 2000;
            List<ImageInfo> list_ImageInfo = new List<ImageInfo>();
            list_ImageInfo.Add(image1);
            list_ImageInfo.Add(image2);
            ParamsCropPart CropPartParams1 = PluginParams as ParamsCropPart; //里氏转换
            CropPartParams1.list_ImageInfo = list_ImageInfo;
            CropPartParams1.PartSizeRow = (4000 * 0.95);
            CropPartParams1.PartSizeCol = image1.imageWidth;
            CropPartParams1.PartCenterID = 1;
            CropPartParams1.PartCenterRow = 4000;
            CropPartParams1.PartCenterCol = image1.imageWidth / 2;
            CropPartParams1.CropShaftRow = 0;
            CropPartParams1.CropShaftCol = 0;
            bool flag = ExecuteObj();

            image1?.Dispose();
            image2?.Dispose();
            list_ImageInfo?.Clear();
            CropPartParams1.Image_part?.Dispose();
        }

        public void DisPoseObj()
        {
           


        }
    }
}
