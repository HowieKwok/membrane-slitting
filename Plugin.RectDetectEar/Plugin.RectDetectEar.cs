using Frame_Core;
using FrameCommunity.Data;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.RectDetectEar
{
    [Category("查找极耳")]
    [DisplayName("矩形法查找极耳")]
    [Export("RectDetectEarObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class RectDetectEarObj : IPlugin
    {
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        public PluginParamsBase PluginParams { get; set; } = new ParamsRectDetectEar();
        SreachJROperator Operator = new SreachJROperator();
        public ParamsRectDetectEar Params;
        ImageInfo imageinfo_now = new ImageInfo();

        long TempJRID = 0;

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public bool ExecuteObj()
        {
            Params = PluginParams as ParamsRectDetectEar; //里氏转换   4
            imageinfo_now = Params.ImageInfo;

            bool bl_Level = false; //每一格灰度均值的标志位--高低电平标志位 ,0代表false，表示非极耳区域；1代表true，表示非极耳区域。
            try
            {
                Params.ProPoleList_Out.Clear();
                Params.ProEdgeList_Out.Clear();

                Params.listGrayResult_Out.Clear();

                bl_Level = Params.bl_GrayLevel;
                TempJRID = 0;
                //每一个矩形的ROW高度
                Params.PreRectHeight = imageinfo_now.imageHeight / Params.hv_RegionNum;
                //图片中矩形阈值分布。
                Operator.JudgeWhiteRegion(imageinfo_now.ho_image, Params.threshold_Min, Params.threshold_Max,
                    Params.hv_RegionNum, Params.Column1, Params.Column2, out Params.hv_GrayResult, out Params.hv_GrayMean);

                //结果判断
                for (int i = 0; i < Params.hv_RegionNum; i++)
                {
                    //添加输出变量。
                    Params.listGrayResult_Out.Add(Params.hv_GrayResult[i].I);

                    if (bl_Level == false) //当前水平位为低位，所在区域为直边区
                    {
                        if (Params.hv_GrayResult[i] == 1)//当前矩形框为1，为极耳区，代表从直边到极耳的过渡 
                        {
                            //if (Params.ProPoleList_In.Count > 0)//注释掉，不接收传入，只局部计数极耳个数。
                            //{
                            //    TempJRID = Params.ProPoleList_In.Last().JR_ID;
                            //}
                            Params.ProPoleList_Out.Add(
                                new PoleEarsInfo
                                {
                                    JR_ID = TempJRID,
                                    image_ID_Begin = imageinfo_now.image_ID,
                                    Row_Begin = Params.PreRectHeight * (i - Params.hv_RegionShieldNum),
                                });
                            TempJRID++;
                            bl_Level = true;
                        }
                    }
                    else if (bl_Level == true)//当前水平位为高位，所在区域为极耳区
                    {
                        if (Params.hv_GrayResult[i] == 0)//当前矩形框为0，为直边区，代表从直边到极耳的过渡 
                        {
                            Params.ProEdgeList_Out.Add(
                                   new PoleEarsInfo
                                   {
                                       JR_ID = TempJRID,
                                       image_ID_Begin = imageinfo_now.image_ID,
                                       Row_Begin = Params.PreRectHeight * (i + Params.hv_RegionShieldNum),
                                   });
                            //PoleEarsInfo temp = Params.ProPoleList_In.Last();
                            //temp.image_ID_End = imageinfo_now.image_ID;
                            //temp.Row_End = PreRow * (i + 1);

                            //double JRSizeY = (temp.image_ID_End - temp.image_ID_Begin) * imageinfo_now.imageHeight + (temp.Row_End - temp.Row_Begin);

                            //if (JRSizeY > Y_length_Max)
                            //{
                            //    Params.ProPoleList_In.Remove(temp);//剔除超长
                            //}

                            bl_Level = false;
                        }
                    }
                }
                Params.bl_GrayLevel = bl_Level;
                Params.bl_ModelRunFlag = true;
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug("矩形查找极耳插件异常！原因：try_catch捕获异常" + ee.ToString());
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
            this.PluginParams = (ParamsRectDetectEar)param;
            Params = PluginParams as ParamsRectDetectEar; //里氏转换
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
