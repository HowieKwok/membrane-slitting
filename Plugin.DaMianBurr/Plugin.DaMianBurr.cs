using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Plugin.DaMianBurr
{
    [Category("大面毛刺")]
    [DisplayName("大面毛刺")]
    [Export("DaMianBurrObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class DaMianBurrObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsDaMianBurr();
        ParamsDaMianBurr EarBurrParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public string grooveName;
        public bool isExistModel = false;
        DaMianBurrAlgorithm Battery = new DaMianBurrAlgorithm();


        /// <summary>
        /// 面积标定
        /// </summary>
        private double AreaCali = 0.003;

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public bool ExecuteObj()
        {
            EarBurrParams = PluginParams as ParamsDaMianBurr; //里氏转换

            EarBurrParams.ho_ImageProcessed?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.ho_ImageProcessed);
            EarBurrParams.DefectCount = 0;
            foreach (NgTypeInfo item in EarBurrParams.List_Defect)
            {
                item.Dispose();
            }
            EarBurrParams.List_Defect.Clear();



            EarBurrParams.outputDaMianMaoCiRegions?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.outputDaMianMaoCiRegions);

            EarBurrParams.outputDaMianLine?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.outputDaMianLine);

            EarBurrParams.outputDaMianMaoCiDetectionRectangle?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.outputDaMianMaoCiDetectionRectangle);

            HObject ho_Rectangle2, ho_ImageReduce;
            HObject ho_TransRegion, ho_SelRegion;

            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduce);


            HOperatorSet.GenEmptyObj(out ho_TransRegion);
            HOperatorSet.GenEmptyObj(out ho_SelRegion);

            HTuple DefectNum = 0;


            try
            {
                EarBurrParams.outputDaMianTuBuPointY = 0;
                EarBurrParams.outputCorrectionValue = 0;
                HObject DaMianMaoCiRegions;
                HOperatorSet.GenEmptyObj(out DaMianMaoCiRegions);
                HObject DaMianMaoCiDetectionRectangle;
                HOperatorSet.GenEmptyRegion(out DaMianMaoCiDetectionRectangle);

                HObject DaMianLine;
                HOperatorSet.GenEmptyObj(out DaMianLine);

                HTuple count = 0;

                double TuCengHight = 0;

                AreaCali = EarBurrParams.Pix_X * EarBurrParams.Pix_Y;






                Battery.FindDaMianMaoCi(EarBurrParams.ImageInfo.ho_image, EarBurrParams.Pix_X, EarBurrParams.Pix_Y, AreaCali, EarBurrParams,
                    out EarBurrParams.List_Defect, out DaMianLine, out DaMianMaoCiDetectionRectangle, out DaMianMaoCiRegions,
                    out HTuple outputPoint, out count, out double DaMianMaoCiHeight);
                EarBurrParams.outputDaMianTuBuPointY = outputPoint;             
                EarBurrParams.outputCorrectionValue = (outputPoint - EarBurrParams.DaMianOriginValue) * EarBurrParams.Pix_Y;

                EarBurrParams.DefectCount = count.I;
                //EarBurrParams.outputDaMianMaoCiRegions = DaMianMaoCiRegions.CopyObj(1, 1);
                EarBurrParams.outputDaMianLine = DaMianLine.CopyObj(1, 1);
                EarBurrParams.outputDaMianMaoCiDetectionRectangle = DaMianMaoCiDetectionRectangle.CopyObj(1, 1);
                //Product.TuCengHight = TuCengHight;
                //if (DetectionApp.Instance.AutoDisposeRes)
                //{
                DaMianMaoCiRegions.Dispose();
                DaMianLine.Dispose();
                DaMianMaoCiDetectionRectangle.Dispose();
                //}



                EarBurrParams.bl_ModelRunFlag = true;
            }
            catch (Exception)
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
            this.PluginParams = (ParamsDaMianBurr)param;
            EarBurrParams = PluginParams as ParamsDaMianBurr; //里氏转换
        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }
    }
}
