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
using System.Drawing;
using System.Drawing.Text;


namespace Plugin.D3MeasureDepth
{
    [Category("尺寸测量")]
    [DisplayName("3D测深度")]
    //[ExportMetadata("measure1", "IPlugin")]
    [Export("D3MeasureDepthObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class D3MeasureDepthObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsD3MeasureDepth();
        ParamsD3MeasureDepth DefectParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        public string grooveName;
        public bool isExistModel = false;

        public bool ExecuteObj()
        {
            DefectParams = PluginParams as ParamsD3MeasureDepth; //里氏转换
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
            HObject regionMoved01;
            HOperatorSet.GenEmptyObj(out regionMoved01);
            HObject regionMoved02;
            HOperatorSet.GenEmptyObj(out regionMoved02);
            //输出结果区域处理
            HObject imageSurface;
            HOperatorSet.GenEmptyObj(out imageSurface);
            HObject imageSub;
            HOperatorSet.GenEmptyObj(out imageSub);
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

                //检测图像ROI深度。
                //gen_rectangle2(Region, 1187.49, 262.586, rad(-3.66779), 45.7969, 26.7857)
                //move_region(Region, inputPlaneRegion1, 100, 0)
                //move_region(Region, inputPlaneRegion2, -100, 0)
                //union2(inputPlaneRegion1, inputPlaneRegion2, inputPlaneRegion)
                //get_region_points(inputPlaneRegion, PlaneRows, PlaneColumns)
                //get_region_points(Region, Rows, Columns)

                HOperatorSet.GetRegionPoints(ho_Rectangle2, out HTuple Rows, out HTuple Columns);
                HOperatorSet.MoveRegion(ho_Rectangle2, out regionMoved01, DefectParams.MoveRegionRow, DefectParams.MoveRegionColumn);
                HOperatorSet.MoveRegion(ho_Rectangle2, out regionMoved02, -DefectParams.MoveRegionRow, DefectParams.MoveRegionColumn);
                HOperatorSet.Union2(regionMoved01, regionMoved02, out DefectParams.ho_UnionPlaneRegion);
                // HOperatorSet.Union1(regionMoved01, out DefectParams.ho_UnionPlaneRegion);
                HOperatorSet.GetRegionPoints(DefectParams.ho_UnionPlaneRegion, out HTuple PlaneRows, out HTuple PlaneColumns);

                //get_image_size(ImageZ, imageWidth, imageHeight)
                //fit_surface_first_order(inputPlaneRegion, ImageZ, inputAlgorithm, inputIterations, inputClippingFactor, Alpha, Beta, Gamma)
                //area_center(inputPlaneRegion, Area, Row, Column)
                //gen_image_surface_first_order(ImageSurface, inputImageType, Alpha, Beta, Gamma, Row, Column, imageWidth, imageHeight)
                //sub_image(ImageZ, ImageSurface, outputSubImage, inputMult, inputAdd)
                //get_grayval(outputSubImage, Rows, Columns, Grayvaldiff)

                HOperatorSet.GetImageSize(DefectParams.ho_Image, out HTuple width, out HTuple height);
                HOperatorSet.FitSurfaceFirstOrder(DefectParams.ho_UnionPlaneRegion, DefectParams.ho_Image, DefectParams.hv_Algorithm,
                    DefectParams.hv_Iterations, DefectParams.hv_ClippingFactor, out HTuple alpha, out HTuple beta, out HTuple gamma);
                HOperatorSet.AreaCenter(DefectParams.ho_UnionPlaneRegion, out HTuple area, out HTuple row, out HTuple column);
                HOperatorSet.GenImageSurfaceFirstOrder(out imageSurface, DefectParams.hv_ImageType, alpha, beta, gamma, row, column, width, height);
                HOperatorSet.SubImage(DefectParams.ho_Image, imageSurface, out imageSub, DefectParams.hv_Mult, DefectParams.hv_Add);

                HOperatorSet.SmallestRectangle1(ho_Rectangle2, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);

                //  HOperatorSet.GetGrayval(imageSub, row, column, out HTuple grayvalDiff1);
                //row.TupleConcat(row2);
                //row.TupleConcat(row1);
                //column.TupleConcat(column2);
                //column.TupleConcat(column1);
                //HOperatorSet.WriteImage(DefectParams.ho_Image, "tiff", 0, "C:\\Users\\01961\\Desktop\\111.tif");

                int count = Rows.Length;
                HTuple RowFilter = new HTuple();
                HTuple ColFilter = new HTuple();
                int i = 0;
                while (i < count)
                {
                    RowFilter = RowFilter.TupleConcat(Rows.TupleSelect(i));
                    ColFilter = ColFilter.TupleConcat(Columns.TupleSelect(i));
                    i += 20;
                }

                HOperatorSet.GetGrayval(imageSub, RowFilter, ColFilter, out HTuple grayvalDiff);
                HTuple Hv_result = 0;
                if (DefectParams.DataFilter == "均值滤波")
                {
                    Hv_result = Filter.AvgFilter(grayvalDiff, DefectParams.Averagefactor);
                }
                else if (DefectParams.DataFilter == "滑动窗口滤波")
                {
                    Hv_result = Filter.MoveAvgFilterOptimized(grayvalDiff, DefectParams.Averagefactor);
                }
                else if (DefectParams.DataFilter == "剔值平均")
                {
                    Filter.data_filter_output_index(grayvalDiff, DefectParams.rateLowRemove, DefectParams.rateHighRemove, out Hv_result, out HTuple hv_dataFilterIndex);
                }
                else
                {
                    Hv_result = grayvalDiff;
                }

                HTuple point_Z_diff = (-Hv_result).ToDArr();
                HOperatorSet.TupleMean(point_Z_diff, out DefectParams.hv_DepthMean);
                HOperatorSet.TupleMax(point_Z_diff, out DefectParams.hv_DepthMax);
                HOperatorSet.TupleMin(point_Z_diff, out DefectParams.hv_DepthMin);


                if (DefectParams.InvertValue)
                {
                    DefectParams.hv_DepthMean *= -1000;
                    DefectParams.hv_DepthMax *= -1000;
                    DefectParams.hv_DepthMin *= -1000;
                }
                else
                {
                    DefectParams.hv_DepthMean *= 1000;
                    DefectParams.hv_DepthMax *= 1000;
                    DefectParams.hv_DepthMin *= 1000;
                }

                switch (DefectParams.nOutputValueIndex)
                {
                    case 0:
                        DefectParams.hv_DepthValue = DefectParams.hv_DepthMean;
                        break;
                    case 1:
                        DefectParams.hv_DepthValue = DefectParams.hv_DepthMax;
                        break;
                    case 2:
                        DefectParams.hv_DepthValue = DefectParams.hv_DepthMin;
                        break;
                }


                //double val = mean.D * 1000;
                //Console.WriteLine(val.ToString());

                DefectParams.bl_ModelRunFlag = true;
            }
            catch (Exception e)
            {
                DefectParams.bl_ModelRunFlag = false;
            }
            ho_Rectangle2.Dispose();
            ho_ImagePart.Dispose();
            ho_ImageScale.Dispose();
            ho_ImagePartProcessed.Dispose();
            ho_ImageReduce.Dispose();
            ho_Region.Dispose();
            regionMoved01.Dispose();
            regionMoved02.Dispose();
            imageSurface.Dispose();
            imageSub.Dispose();
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
            this.PluginParams = (ParamsD3MeasureDepth)param;
            DefectParams = PluginParams as ParamsD3MeasureDepth; //里氏转换
        }

        public void IniReslut()
        {
            HOperatorSet.GenEmptyObj(out DefectParams.ho_ImageScale);
            HOperatorSet.GenEmptyObj(out DefectParams.ho_ImageSharpen);
            HOperatorSet.GenEmptyObj(out DefectParams.ho_ImageProcessed);
            HOperatorSet.GenEmptyObj(out DefectParams.ho_UnionPlaneRegion);

            DefectParams.hv_DepthMean = 0;
            DefectParams.hv_DepthMax = 0;
            DefectParams.hv_DepthMin = 0;
        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }

        /// <summary>
        /// 滑动平均数滤波
        /// </summary>
        /// <param name="pointDic"></param>
        /// <param name="xishu"></param>
        /// <returns></returns>
        public static List<double> MoveAvgFilter(List<double> pointDic, int filterNum)
        {
            try
            {
                List<double> a1 = new List<double>();

                for (int i = 0; i < pointDic.Count; i++)
                {
                    List<double> list = new List<double>();

                    if (i < pointDic.Count - (filterNum / 2))
                    {
                        for (int j = filterNum / 2; j >= -(filterNum / 2); j--)
                        {
                            list.Add(pointDic[Math.Abs(i - j)]);
                        }
                    }
                    else
                    {
                        for (int j = filterNum / 2; j >= -(filterNum / 2); j--)
                        {
                            if (j >= 0)
                            {
                                list.Add(pointDic[Math.Abs(i - j)]);
                            }
                            else
                            {
                                list.Add(pointDic[Math.Abs(i + j)]);
                            }
                        }
                    }

                    Dictionary<double, int> quan = new Dictionary<double, int>();
                    for (int l = 0; l < list.Count; l++)
                    {
                        if (quan.ContainsKey(list[l]) == false)
                        {
                            quan[list[l]] = 1;
                        }
                        else
                        {
                            quan[list[l]]++;
                        }
                    }


                    if (list.Count > 0)
                    {
                        double sum = 0;
                        double quansum = 0;

                        for (int k = 0; k < list.Count; k++)
                        {
                            double c = list[k];
                            int a = quan[list[k]];
                            sum += list[k] * a;
                            quansum += a;
                        }

                        a1.Add(sum / quansum);
                    }
                }
                return a1;
            }
            catch (Exception ex)
            {
                return pointDic;
                //LogName.ErrorLog.Fatal(ex);
            }

        }


        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="pointDic"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static List<double> AvgFilter(List<double> pointDic, int factor)
        {
            try
            {
                List<double> a2 = new List<double>();
                for (int i = 0; i < pointDic.Count; i++)
                {
                    List<double> list = new List<double>();
                    if (i < pointDic.Count - (factor / 2))
                    {
                        for (int j = factor / 2; j >= -(factor / 2); j--)
                        {
                            list.Add(pointDic[Math.Abs(i - j)]);
                        }
                    }
                    else
                    {
                        for (int j = factor / 2; j >= -(factor / 2); j--)
                        {
                            if (j >= 0)
                            {
                                list.Add(pointDic[Math.Abs(i - j)]);
                            }
                            else
                            {
                                list.Add(pointDic[Math.Abs(i + j)]);
                            }
                        }
                    }
                    a2.Add(list.Average());
                }
                return a2;
            }
            catch (Exception ex)
            {
                //LogName.ErrorLog.Fatal(ex);
            }
            return null;

        }
    }
}
