using BatteryLugClassLibrary;
using Frame_Core;
using HalconDotNet;
using Plugin.DetectDefect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Plugin.EndBurr
{
    [Category("端面毛刺")]
    [DisplayName("端面毛刺")]
    [Export("EndBurrObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class EndBurrObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsEndBurr();
        ParamsEndBurr EarBurrParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public string grooveName;
        public bool isExistModel = false;
        EndBurrAlgorithm Battery = new EndBurrAlgorithm();

      
        /// <summary>
        /// 面积标定
        /// </summary>
        private double AreaCali = 0.003;


        public bool ExecuteObj()
        {
            EarBurrParams = PluginParams as ParamsEndBurr; //里氏转换

            EarBurrParams.ho_ImageProcessed?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.ho_ImageProcessed);
            EarBurrParams.DefectCount = 0;
            foreach (NgTypeInfo item in EarBurrParams.List_Defect)
            {
                item.Dispose();
            }
            EarBurrParams.List_Defect.Clear();



            EarBurrParams.outputFenQieMaoCiRegions?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.outputFenQieMaoCiRegions);

            EarBurrParams.outputFenQieBoCaiRegions?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.outputFenQieBoCaiRegions);

            EarBurrParams.outputTuCengRegion?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.outputTuCengRegion);

            HObject ho_Rectangle2, ho_ImageReduce;
            HObject ho_TransRegion, ho_SelRegion;

            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduce);


            HOperatorSet.GenEmptyObj(out ho_TransRegion);
            HOperatorSet.GenEmptyObj(out ho_SelRegion);

            HTuple DefectNum = 0;


            try
            {

                HObject regions;
                HOperatorSet.GenEmptyObj(out regions);
                HObject outputTuCengRegion;
                HOperatorSet.GenEmptyRegion(out outputTuCengRegion);

                HObject regions2;
                HOperatorSet.GenEmptyObj(out regions2);
               
                HTuple count = 0;

                double TuCengHight = 0;

                AreaCali = EarBurrParams.Pix_X * EarBurrParams.Pix_Y;


              



                Battery.Entry1(EarBurrParams.ImageInfo.ho_image, EarBurrParams.Pix_X, EarBurrParams.Pix_Y, AreaCali, EarBurrParams,
                    out EarBurrParams.List_Defect, out regions, out regions2, out count,
                    out TuCengHight, out outputTuCengRegion);
                EarBurrParams.DefectCount = count;
                //EarBurrParams.outputFenQieMaoCiRegions = regions.CopyObj(1, 1);
                EarBurrParams.outputFenQieBoCaiRegions = regions2.CopyObj(1, 1);
                EarBurrParams.outputTuCengRegion = outputTuCengRegion.CopyObj(1, 1);
                //Product.TuCengHight = TuCengHight;
                //if (DetectionApp.Instance.AutoDisposeRes)
                //{
                regions.Dispose();
                regions2.Dispose();
                outputTuCengRegion.Dispose();
                //}



                EarBurrParams.bl_ModelRunFlag = true;
            }
            catch (Exception e)
            {
                EarBurrParams.bl_ModelRunFlag = false;
            }
            finally
            {
                ho_Rectangle2.Dispose();
                ho_ImageReduce.Dispose();

                ho_TransRegion.Dispose();
                ho_SelRegion.Dispose();
            }

            return EarBurrParams.bl_ModelRunFlag;


        }

        public void GetResultParams(string varName, object obj)
        {
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
            this.PluginParams = (ParamsEndBurr)param;
            EarBurrParams = PluginParams as ParamsEndBurr; //里氏转换
        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }
    }
}
