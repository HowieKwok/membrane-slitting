using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconAlgorithm;

namespace Plugin.DetectDefect
{
    [Category("缺陷检测")]
    [DisplayName("缺陷检测工具")]
    //[ExportMetadata("measure1", "IPlugin")]
    [Export("DetectDefectObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class DetectDefectObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsDetectDefect();
        ParamsDetectDefect DefectParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        public string grooveName;
        public bool isExistModel = false;

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public bool ExecuteObj()
        {
            DefectParams = PluginParams as ParamsDetectDefect; //里氏转换
            HTuple hv_RoiMidRow = DefectParams.hv_RoiMidRow;
            HTuple hv_RoiMidColumn = DefectParams.hv_RoiMidColumn;
            HObject ho_Rectangle2, ho_ImagePart, ho_ImageScale, ho_ImagePartProcessed, ho_ImageReduce, ho_Region;
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);

            HOperatorSet.GenEmptyObj(out ho_ImagePart);
            HOperatorSet.GenEmptyObj(out ho_ImageScale);
            HOperatorSet.GenEmptyObj(out ho_ImagePartProcessed);
            HOperatorSet.GenEmptyObj(out ho_ImageReduce);
            HOperatorSet.GenEmptyObj(out ho_Region);
            IniReslut();
            HObject ho_RegionUnion1;
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HObject ho_RegionUnion2;
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            //输出结果区域处理
            HObject ho_RegionOperate;
            HOperatorSet.GenEmptyObj(out ho_RegionOperate);


            HTuple hv_Row;
            HTuple hv_Column;
            try
            {
                HTuple hv_Channels = 1;
                HTuple hv_RoiPhi;
                HOperatorSet.TupleRad(DefectParams.hv_RoiAngle, out hv_RoiPhi);

                //ROI运动
                switch (DefectParams.str_RoiMoveIndex)
                {
                    case "StaticROI":
                        break;

                    case "StandardROI":
                        hv_RoiMidRow = DefectParams.hv_RoiStandardRow + DefectParams.hv_RoiStandardOffsetRow;
                        hv_RoiMidColumn = DefectParams.hv_RoiStandardColumn + DefectParams.hv_RoiStandardOffsetColumn;
                        break;

                    case "IncreaseROI":
                        hv_RoiMidRow = DefectParams.hv_RoiMidRow + DefectParams.hv_RoiIncreaseRow * DefectParams.hv_RoiIncreaseNum;
                        hv_RoiMidColumn = DefectParams.hv_RoiMidColumn + DefectParams.hv_RoiIncreaseColumn * DefectParams.hv_RoiIncreaseNum;
                        break;
                }
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_RoiMidRow, hv_RoiMidColumn, -hv_RoiPhi, DefectParams.hv_RoiHeight / 2, DefectParams.hv_RoiWidth / 2);
                HOperatorSet.Difference(ho_Rectangle2, DefectParams.ho_ShieldRegion, out ho_Rectangle2);

                //图像预处理
                if (DefectParams.bl_ImageProcessIndex)
                {
                    HOperatorSet.CountChannels(DefectParams.ho_Image, out hv_Channels);

                    if (hv_Channels > 1)
                    {
                        HOperatorSet.Rgb1ToGray(DefectParams.ho_Image, out DefectParams.ho_ImageProcessed);

                        if (DefectParams.hv_ScaleFactor != 0 || DefectParams.hv_SharpenIterations != 0)
                        {

                            if (DefectParams.hv_ScaleFactor != 0)
                            {
                                HObject ho_ChannelEmphasis;
                                HObject ho_ImageR;
                                HObject ho_ImageG;
                                HObject ho_ImageB;
                                DefectDetection.ImageCrop(DefectParams.ho_Image, ho_Rectangle2, out ho_ImagePart);
                                DefectDetection.SelectImageChannel(ho_ImagePart, DefectParams.hv_ScaleValueR, DefectParams.hv_ScaleValueG, DefectParams.hv_ScaleValueB,
                                           out ho_ChannelEmphasis, out ho_ImageR, out ho_ImageG, out ho_ImageB);
                                ho_ImageR.Dispose();
                                ho_ImageG.Dispose();
                                ho_ImageB.Dispose();
                                DefectDetection.ScaleEmphasis(ho_ChannelEmphasis, 255, DefectParams.hv_ScaleFactor, out ho_ImageScale);
                                ho_ChannelEmphasis.Dispose();
                                DefectDetection.ImageStitching(DefectParams.ho_ImageProcessed, ho_ImageScale, ho_Rectangle2, out DefectParams.ho_ImageScale);
                            }
                            else
                            {
                                DefectDetection.ImageCrop(DefectParams.ho_ImageProcessed, ho_Rectangle2, out ho_ImageScale);
                            }

                            if (DefectParams.hv_SharpenIterations != 0)
                            {
                                DefectDetection.OutlineSharpen(ho_ImageScale, out ho_ImagePartProcessed, DefectParams.hv_SharpenIterations, DefectParams.hv_SharpenSigma);
                                DefectDetection.ImageStitching(DefectParams.ho_ImageProcessed, ho_ImagePartProcessed, ho_Rectangle2, out DefectParams.ho_ImageSharpen);
                                DefectParams.ho_ImageProcessed = DefectParams.ho_ImageSharpen;
                            }
                            else
                            {
                                DefectParams.ho_ImageProcessed = DefectParams.ho_ImageScale;
                            }
                        }
                    }
                    else
                    {

                        if (DefectParams.hv_ScaleFactor != 0 || DefectParams.hv_SharpenIterations != 0)
                        {
                            DefectDetection.ImageCrop(DefectParams.ho_Image, ho_Rectangle2, out ho_ImagePart);

                            if (DefectParams.hv_ScaleFactor != 0)
                            {
                                DefectDetection.ScaleEmphasis(ho_ImagePart, DefectParams.hv_ScaleValueR, DefectParams.hv_ScaleFactor, out ho_ImageScale);
                                DefectDetection.ImageStitching(DefectParams.ho_Image, ho_ImageScale, ho_Rectangle2, out DefectParams.ho_ImageScale);

                            }
                            else
                            {
                                ho_ImageScale = ho_ImagePart;
                            }

                            if (DefectParams.hv_SharpenIterations != 0)
                            {
                                DefectDetection.OutlineSharpen(ho_ImageScale, out ho_ImagePartProcessed, DefectParams.hv_SharpenIterations, DefectParams.hv_SharpenSigma);
                                DefectDetection.ImageStitching(DefectParams.ho_Image, ho_ImagePartProcessed, ho_Rectangle2, out DefectParams.ho_ImageSharpen);
                                DefectParams.ho_ImageProcessed = DefectParams.ho_ImageSharpen;
                            }
                            else
                            {
                                DefectParams.ho_ImageProcessed = DefectParams.ho_ImageScale;
                            }
                        }
                        else
                        {
                            DefectParams.ho_ImageProcessed = DefectParams.ho_Image;
                        }
                    }
                }
                else
                {
                    HOperatorSet.Rgb1ToGray(DefectParams.ho_Image, out DefectParams.ho_ImageProcessed);
                }

                //破损检测
                HOperatorSet.ReduceDomain(DefectParams.ho_ImageProcessed, ho_Rectangle2, out ho_ImageReduce);

                //阈值分割
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, DefectParams.hv_SelectThresholdDown, DefectParams.hv_SelectThresholdUp);

                //区域拆分,筛选
                HOperatorSet.Connection(ho_Region, out DefectParams.ho_SelectedRegion);
                // HOperatorSet.SelectShape(DefectParams.ho_SelectedRegion, out DefectParams.ho_ResidueRegion, DefectParams.hv_RegionSelectIndex, "and", 7, 99999999);//按缺陷外接圆半径筛选缺陷。

                HOperatorSet.SelectShape(DefectParams.ho_SelectedRegion, out DefectParams.ho_ResidueRegion1, "area", "and", DefectParams.hv_ResidueMinValue, DefectParams.hv_ResidueMaxValue);//按面积筛选缺陷。
                HOperatorSet.CountObj(DefectParams.ho_ResidueRegion1, out DefectParams.hv_ResidueNum1);
                //HOperatorSet.Union1(DefectParams.ho_ResidueRegion1, out ho_RegionUnion1);
                //HOperatorSet.AreaCenter(ho_RegionUnion1, out Area1, out hv_Row, out hv_Column);

                HOperatorSet.SelectShape(DefectParams.ho_SelectedRegion, out DefectParams.ho_ResidueRegion2, "area", "and", DefectParams.hv_ResidueMaxValue, 99999999);
                HOperatorSet.CountObj(DefectParams.ho_ResidueRegion2, out DefectParams.hv_ResidueNum2);
                //HOperatorSet.Union1(DefectParams.ho_ResidueRegion2, out ho_RegionUnion2);
                //HOperatorSet.AreaCenter(ho_RegionUnion2, out Area2, out hv_Row, out hv_Column);

                HOperatorSet.Union2(DefectParams.ho_ResidueRegion1, DefectParams.ho_ResidueRegion2, out DefectParams.ho_ResidueRegion);
                HOperatorSet.Union1(DefectParams.ho_ResidueRegion, out ho_RegionUnion1);
                HOperatorSet.AreaCenter(ho_RegionUnion1, out DefectParams.hv_totalAreaPix, out hv_Row, out hv_Column);
                if (DefectParams.hv_totalAreaPix.Length == 0)
                {
                    DefectParams.hv_totalAreaPix = 0;
                }
                DefectParams.hv_ResidueNum = DefectParams.hv_ResidueNum1 + DefectParams.hv_ResidueNum2;
                //DefectParams.hv_totalAreaPix = Area1.D + Area2.D;
                if (DefectParams.IsRegionOperate)
                {
                    HOperatorSet.FillUp(ho_RegionUnion1, out ho_RegionOperate);

                    if (DefectParams.hv_OpenRadius > 0)//开运算
                    {
                        HOperatorSet.ClosingCircle(ho_RegionOperate, out ho_RegionOperate, DefectParams.hv_OpenRadius);
                        HOperatorSet.OpeningCircle(ho_RegionOperate, out ho_RegionOperate, DefectParams.hv_OpenRadius);
                    }
                    if (DefectParams.hv_DilationRadius > 0)//膨胀
                    {
                        HOperatorSet.DilationCircle(ho_RegionOperate, out ho_RegionOperate, DefectParams.hv_DilationRadius);
                    }
                    if (DefectParams.hv_ErosionRadius > 0)//腐蚀
                    {
                        HOperatorSet.ErosionCircle(ho_RegionOperate, out ho_RegionOperate, DefectParams.hv_ErosionRadius);
                    }
                    HOperatorSet.Union1(ho_RegionOperate, out DefectParams.ho_ResidueRegion);
                }
                DefectParams.bl_ModelRunFlag = true;
            }
            catch (Exception)
            {
                DefectParams.bl_ModelRunFlag = false;
            }
            ho_Rectangle2.Dispose();
            ho_ImagePart.Dispose();
            ho_ImageScale.Dispose();
            ho_ImagePartProcessed.Dispose();
            ho_ImageReduce.Dispose();
            ho_Region.Dispose();
            ho_RegionUnion1.Dispose();
            ho_RegionUnion2.Dispose();
            ho_RegionOperate.Dispose();
            return DefectParams.bl_ModelRunFlag;


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
            this.PluginParams = (ParamsDetectDefect)param;
            DefectParams = PluginParams as ParamsDetectDefect; //里氏转换
        }

        public void IniReslut()
        {
            HOperatorSet.GenEmptyObj(out DefectParams.ho_ImageScale);
            HOperatorSet.GenEmptyObj(out DefectParams.ho_ImageSharpen);
            HOperatorSet.GenEmptyObj(out DefectParams.ho_ImageProcessed);
            HOperatorSet.GenEmptyObj(out DefectParams.ho_SelectedRegion);
            HOperatorSet.GenEmptyObj(out DefectParams.ho_ResidueRegion);
            HOperatorSet.GenEmptyObj(out DefectParams.ho_ResidueCircle);
            DefectParams.hv_ResidueNum = 0;
        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }
    }
}
