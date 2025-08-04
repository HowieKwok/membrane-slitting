using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.DetectEar
{
    [Category("查找极耳")]
    [DisplayName("查找拼接极耳")]
    [Export("DetectEarObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class DetectEarObj : IPlugin
    {
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        public PluginParamsBase PluginParams { get; set; } = new ParamsDetectEar();
        SreachJROperator Operator = new SreachJROperator();
        public ParamsDetectEar Params;
        ImageInfo imageinfo_now = new ImageInfo();
        int JRcount_Matched = 0;
        List<PoleEarsInfo> ProPoleList_tempIn = new List<PoleEarsInfo>();
        List<PoleEarsInfo> ProPoleList_tempTile = new List<PoleEarsInfo>();
        List<PoleEarsInfo> ProPoleList_tempOrign = new List<PoleEarsInfo>();
        public bool ExecuteObj()
        {
            Params = PluginParams as ParamsDetectEar; //里氏转换   4
            imageinfo_now = Params.ImageInfo;
            JRcount_Matched = 0;
            Params.ProPoleList_Out.Clear();
            Params.JR_Region.Dispose();
            try
            {
                //if (Params.IsDetectTiledImage) //检测裁拼图
                //{
                //    ProPoleList_tempIn.Clear();
                //    ProPoleList_tempTile.Clear();
                //    ProPoleList_tempOrign.Clear();
                //    if (imageinfo_now.TiledImage.CountObj() > 0)
                //    {
                //        double StandLineColumn_tiledImage = Params.StandLineColumn - imageinfo_now.CropColumnBegin;
                //        Operator.Search_JR(imageinfo_now.TiledImage, Params.IsLeft, StandLineColumn_tiledImage, Params.Offset_Min, Params.Offset_Max, Params.threshold_Min, Params.threshold_Max,
                //                           Params.Y_Length_Min, Params.Y_length_Max, Params.X_Length_Min, Params.X_length_Max,
                //                           out Params.JR_RowUp, out Params.JR_RowDown);
                //        Operator.Match_JR(Params.ProPoleList_In, ProPoleList_tempTile, Params.IsLeft, imageinfo_now.image_ID, imageinfo_now.CropHeight, imageinfo_now.imageHeight, Params.JR_RowUp, Params.JR_RowDown, true, out JRcount_Matched);
                //        //Operator.Crop_JRPart(Params.ProPoleList, Params.list_ImageInfo, Params.IsLeft, Params.JRcount_Matched, Params.StandLineColumn, Params.X_length_Max, Params.JRcropShaft_X, Params.JRcropShaft_Y);
                //    }
                //}
                double Y_Length_Min = Params.JRSizeY * Params.JRSizeY_PercentMin;
                double Y_length_Max = Params.JRSizeY * Params.JRSizeY_PercentMax;
                double X_Length_Min = Params.JRSizeX * Params.JRSizeX_PercentMin;
                double X_length_Max = Params.JRSizeX * Params.JRSizeX_PercentMax;

                Operator.Search_JR(imageinfo_now.ho_image, Params.Column1, Params.Column2, Params.threshold_Min, Params.threshold_Max,
                                      Y_Length_Min, Y_length_Max, X_Length_Min, X_length_Max,
                                      out Params.JR_RowUp, out Params.JR_RowDown, out Params.JR_Region);
                //if (Params.IsDetectTiledImage)//检测裁拼图
                //{
                //    ProPoleList_tempIn = Params.ProPoleList_In.Concat(ProPoleList_tempTile).ToList();
                //    Operator.Match_JR(ProPoleList_tempIn, ProPoleList_tempOrign, Params.IsLeft, imageinfo_now.image_ID, imageinfo_now.CropHeight, imageinfo_now.imageHeight, Params.JR_RowUp, Params.JR_RowDown, false, out Params.JRcount_Matched);
                //    Params.JRcount_Matched = JRcount_Matched + Params.JRcount_Matched;
                //    Params.ProPoleList_Out = ProPoleList_tempTile.Concat(ProPoleList_tempOrign).ToList();
                //}
                //else
                {
                    Operator.ExpandJR(Params.ProPoleList_In, Params.ProPoleList_Out, imageinfo_now.image_ID, imageinfo_now.imageHeight, Params.JR_RowUp, Params.JR_RowDown, Params.JRInterval, Params.JRSizeY, out Params.JRcount_Matched);
                }
                // Operator.Crop_JRPart(Params.ProPoleList, Params.list_ImageInfo, Params.IsLeft, Params.JRcount_Matched, Params.StandLineColumn, Params.X_length_Max, Params.JRcropShaft_X, Params.JRcropShaft_Y);

                Params.bl_ModelRunFlag = true;
            }
            catch (Exception ee)
            {
                Params.bl_ModelRunFlag = false;
                return false;
            }
            return Params.bl_ModelRunFlag;
        }


        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath;
            if (/*moduleInfo.Save() &&*/ PluginParams.Save())
            {
                return true;
            }
            return false;
        }

        public void SetParams(PluginParamsBase param)
        {
            this.PluginParams = (ParamsDetectEar)param;
            Params = PluginParams as ParamsDetectEar; //里氏转换
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
