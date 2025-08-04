using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace Plugin.D3MeasureDepth
{
    class DefectDetection
    {
        /// <summary>
        /// 图像拼割：图像裁切
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="ho_Region">输入区域</param>
        /// <param name="ho_ImagePart">裁切图</param>
        public static void ImageCrop(HObject ho_Image, HObject ho_Region, out HObject ho_ImagePart)
        {
            HObject ho_ImageReduced; 
            HOperatorSet.ReduceDomain(ho_Image, ho_Region, out ho_ImageReduced);
            HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
            ho_ImageReduced.Dispose();
        }


        /// <summary>
        /// 图像拼割：图像拼接
        /// </summary>
        /// <param name="ho_Image">灰度全幅图像</param>
        /// <param name="ho_ImagePart">裁切灰度图像</param>
        /// <param name="ho_Reigion">裁切区域</param>
        /// <param name="ho_ImageStich">拼接后图像</param>
        public static void ImageStitching(HObject ho_Image, HObject ho_ImagePart, HObject ho_Reigion, out HObject ho_ImageStich)
        {
            HTuple hv_Width;
            HTuple hv_Height;
            HObject ho_ImagePaint;
            HTuple hv_WidthPart;
            HTuple hv_HeightPart;
            HObject ho_RectanglePart;
            HTuple hv_AreaPart;
            HTuple hv_RowPart;
            HTuple hv_ColumnPart;
            HTuple hv_RowsPart;
            HTuple hv_ColumnsPart;
            HTuple hv_GrayvalPart;
            HObject ho_ImagePartPaint;
            HTuple hv_Area;
            HTuple hv_Row;
            HTuple hv_Column;
            HOperatorSet.GetImageSize(ho_Image, out  hv_Width, out  hv_Height);
            HOperatorSet.PaintRegion(ho_Reigion, ho_Image, out  ho_ImagePaint, 0, "fill");

            HOperatorSet.GetImageSize(ho_ImagePart, out  hv_WidthPart, out  hv_HeightPart);
            HOperatorSet.GenRectangle1(out  ho_RectanglePart, 0, 0, hv_HeightPart - 1, hv_WidthPart - 1);
            HOperatorSet.AreaCenter(ho_ImagePart, out  hv_AreaPart, out  hv_RowPart, out hv_ColumnPart);
            HOperatorSet.GetRegionPoints(ho_RectanglePart, out  hv_RowsPart, out  hv_ColumnsPart);
            HOperatorSet.GetGrayval(ho_ImagePart, hv_RowsPart, hv_ColumnsPart, out  hv_GrayvalPart);

            HOperatorSet.GenImageConst(out  ho_ImagePartPaint, "byte", hv_Width, hv_Height);
            HOperatorSet.AreaCenter(ho_Reigion, out  hv_Area, out  hv_Row, out  hv_Column);
            HOperatorSet.SetGrayval(ho_ImagePartPaint, (hv_RowsPart + hv_Row) - (hv_HeightPart / 2), (hv_ColumnsPart + hv_Column) - (hv_WidthPart / 2), hv_GrayvalPart);
            HOperatorSet.AddImage(ho_ImagePaint, ho_ImagePartPaint, out ho_ImageStich, 1, 0);

            ho_ImagePaint.Dispose();
            ho_RectanglePart.Dispose();
            ho_ImagePartPaint.Dispose();
        }



        ///<summary>
        ///图像拼接
        ///</summary>
        public static void ImageStitching(HObject ho_ImageResult, HObject ho_Image, HTuple hv_CenterRow, HTuple hv_CenterCol, HTuple hv_Phi, HTuple hv_Length1,
                                   HTuple hv_Length2,  out HObject ho_ImageStich)
        {
            // Local iconic variables 

            HObject ho_Rectangle2Scaled, ho_Image1, ho_Imagepaint, ho_Rectangle2;

            // Local control variables 

            HTuple hv_Area = null, hv_PartRow = null, hv_PartCol = null;
            HTuple hv_ScaledRows = null, hv_ScaledColumns = null, hv_GrayvalScaled = null;
            HTuple hv_rows = null, hv_cols = null, hv_Width = null;
            HTuple hv_Height = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageStich);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2Scaled);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Imagepaint);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_CenterRow, hv_CenterCol, hv_Phi, hv_Length1, hv_Length2);

            HOperatorSet.AreaCenter(ho_ImageResult, out hv_Area, out hv_PartRow, out hv_PartCol);
            ho_Rectangle2Scaled.Dispose();
            HOperatorSet.GenRectangle2(out ho_Rectangle2Scaled, hv_PartRow, hv_PartCol, hv_Phi,
                hv_Length1 + 10, hv_Length2 + 10);
            HOperatorSet.GetRegionPoints(ho_Rectangle2Scaled, out hv_ScaledRows, out hv_ScaledColumns);
            HOperatorSet.GetGrayval(ho_ImageResult, hv_ScaledRows, hv_ScaledColumns, out hv_GrayvalScaled);
            hv_rows = (hv_ScaledRows + hv_CenterRow) - hv_PartRow;
            hv_cols = (hv_ScaledColumns + hv_CenterCol) - hv_PartCol;

            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            ho_Image1.Dispose();
            HOperatorSet.GenImageConst(out ho_Image1, "byte", hv_Width, hv_Height);
            HOperatorSet.SetGrayval(ho_Image1, hv_rows, hv_cols, hv_GrayvalScaled);

            ho_Imagepaint.Dispose();
            HOperatorSet.PaintRegion(ho_Rectangle2, ho_Image, out ho_Imagepaint, 0, "fill");
            ho_ImageStich.Dispose();
            HOperatorSet.AddImage(ho_Image1, ho_Imagepaint, out ho_ImageStich, 1, 0);
            ho_Rectangle2Scaled.Dispose();
            ho_Image1.Dispose();
            ho_Imagepaint.Dispose();
            ho_Rectangle2.Dispose();
            return;
        }



        /// <summary>
        /// 图像合成：底色提取，图像增强
        /// </summary>
        /// <param name="ho_ImageRGB">输入彩色图像</param>
        /// <param name="hv_ValueR">底色R值</param>
        /// <param name="hv_ValueG">底色G值</param>
        /// <param name="hv_ValueB">底色B值</param>
        /// <param name="ho_ImageRGBEmphasis">增强后图像</param>
        /// <param name="ho_ImageR">R通道图像</param>
        /// <param name="ho_ImageG">G通道图像</param>
        /// <param name="ho_ImageB">B通道图像</param>
        public static void SelectImageChannel(HObject ho_ImageRGB, HTuple hv_ValueR, HTuple hv_ValueG, HTuple hv_ValueB,
                                       out HObject ho_ImageRGBEmphasis, out HObject ho_ImageR, out HObject ho_ImageG, out HObject ho_ImageB)
        {
            HObject ho_ImageScaledR;
            HObject ho_ImageScaledG;
            HObject ho_ImageScaledB;
            HObject ho_ImageResult1;
            HObject ho_ImageResult2;


            HOperatorSet.Decompose3(ho_ImageRGB, out ho_ImageR, out ho_ImageG, out ho_ImageB);

            HOperatorSet.ScaleImage(ho_ImageR, out  ho_ImageScaledR, hv_ValueR / ((hv_ValueR + hv_ValueG) + hv_ValueB), 0);
            HOperatorSet.ScaleImage(ho_ImageG, out  ho_ImageScaledG, hv_ValueG / ((hv_ValueR + hv_ValueG) + hv_ValueB), 0);
            HOperatorSet.ScaleImage(ho_ImageB, out  ho_ImageScaledB, hv_ValueB / ((hv_ValueR + hv_ValueG) + hv_ValueB), 0);

            HOperatorSet.AddImage(ho_ImageScaledR, ho_ImageScaledG, out  ho_ImageResult1, 1, 0);
            HOperatorSet.AddImage(ho_ImageResult1, ho_ImageScaledB, out  ho_ImageResult2, 1, 0);

            HOperatorSet.ScaleImage(ho_ImageResult2, out ho_ImageRGBEmphasis,
                                    255 * (hv_ValueR + hv_ValueG + hv_ValueB) / (hv_ValueR * hv_ValueR + hv_ValueG * hv_ValueG + hv_ValueB * hv_ValueB), 0);

            ho_ImageScaledR.Dispose();
            ho_ImageScaledG.Dispose();
            ho_ImageScaledB.Dispose();
            ho_ImageResult1.Dispose();
            ho_ImageResult2.Dispose();
        }


        /// <summary>
        /// 图像预处理：轮廓增强
        /// </summary>
        /// <param name="ho_Image">输入单通道图像</param>
        /// <param name="hv_MaskSize">滤波模型尺寸</param>
        /// <param name="hv_MultiValue">轮廓增强因子</param>
        /// <param name="ho_ImageEmphasis">输出图像结果</param>
        public static void OutlineEmphasis(HObject ho_Image, HTuple hv_MaskSize, HTuple hv_MultiValue, out HObject ho_ImageEmphasis)
        {
            HObject ho_ImageMean;
            HObject ho_ImageSub;
            HOperatorSet.MeanImage(ho_Image, out  ho_ImageMean, hv_MaskSize, hv_MaskSize);
            HOperatorSet.SubImage(ho_Image, ho_ImageMean, out  ho_ImageSub, hv_MultiValue * (-1), 128);
            HOperatorSet.SubImage(ho_Image, ho_ImageSub, out ho_ImageEmphasis, 1, 128);
            ho_ImageMean.Dispose();
            ho_ImageSub.Dispose();
        }


        /// <summary>
        /// 图像预处理：轮廓锐化
        /// </summary>
        /// <param name="image">输入图像</param>
        /// <param name="sharpenedImage">输出图像</param>
        /// <param name="iterations">迭代次数（建议1,2,3）</param>
        /// <param name="sigma">平滑系数（建议0.2,0.6,1.0）</param>
        public static void OutlineSharpen(HObject image, out HObject sharpenedImage, HTuple iterations, HTuple sigma)
        {
            HOperatorSet.ShockFilter(image, out sharpenedImage, 0.5, iterations, "canny", sigma);
        }


        /// <summary>
        /// 图像预处理：灰度拉伸，轮廓增强
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hv_Mean">选取灰度</param>
        /// <param name="hv_ScaleFactor">灰度拉伸因子</param>
        /// <param name="ho_ImageEmphasize">输出增强图像</param>
        public static void ScaleEmphasis(HObject ho_Image, HTuple hv_Mean, HTuple hv_ScaleFactor, out HObject ho_ImageEmphasize)
        {
            HObject ho_ImageScaled1;
            HObject ho_ImageScaled2;
            HObject ho_ImageScaled3;
            HOperatorSet.ScaleImage(ho_Image, out  ho_ImageScaled1, 255 / hv_Mean.D, 0);

            HOperatorSet.ScaleImage(ho_ImageScaled1, out  ho_ImageScaled2, -1, 255);
            HOperatorSet.ScaleImage(ho_ImageScaled2, out  ho_ImageScaled3, hv_ScaleFactor, 0);
            HOperatorSet.ScaleImage(ho_ImageScaled3, out ho_ImageEmphasize, -1, 255);

            ho_ImageScaled1.Dispose();
            ho_ImageScaled2.Dispose();
            ho_ImageScaled3.Dispose();
        }


        /// <summary>
        /// 图像合成：光度立体
        /// </summary>
        /// <param name="ho_Image1">输入图像1</param>
        /// <param name="ho_Image2">输入图像2</param>
        /// <param name="ho_Image3">输入图像3</param>
        /// <param name="ho_Image4">输入图像4</param>
        /// <param name="hv_MaskSize">轮廓提取模型尺寸</param>
        /// <param name="hv_MultiValue">图像灰度拉伸因子</param>
        /// <param name="ho_ImageOutline">输出轮廓合成图像</param>
        /// <param name="ho_SharpenedImage">输出轮廓锐化图像</param>
        public static void FourImagesMerge(HObject ho_Image1, HObject ho_Image2, HObject ho_Image3, HObject ho_Image4, HTuple hv_MaskSize, HTuple hv_MultiValue,
                                           out HObject ho_ImageOutline, out HObject ho_SharpenedImage)
        {
            HObject ho_EdgeAmplitude1;
            HObject ho_EdgeAmplitude2;
            HObject ho_EdgeAmplitude3;
            HObject ho_EdgeAmplitude4;
            HObject ho_ImageResult1;
            HObject ho_ImageResult2;

            HOperatorSet.SobelAmp(ho_Image1, out ho_EdgeAmplitude1, "sum_abs", hv_MaskSize);
            HOperatorSet.SobelAmp(ho_Image2, out ho_EdgeAmplitude2, "sum_abs", hv_MaskSize);
            HOperatorSet.SobelAmp(ho_Image3, out ho_EdgeAmplitude3, "sum_abs", hv_MaskSize);
            HOperatorSet.SobelAmp(ho_Image4, out ho_EdgeAmplitude4, "sum_abs", hv_MaskSize);

            HOperatorSet.AddImage(ho_EdgeAmplitude1, ho_EdgeAmplitude2, out  ho_ImageResult1, hv_MultiValue, 0);
            HOperatorSet.AddImage(ho_EdgeAmplitude3, ho_EdgeAmplitude4, out  ho_ImageResult2, hv_MultiValue, 0);
            HOperatorSet.AddImage(ho_ImageResult1, ho_ImageResult2, out ho_ImageOutline, -1 * hv_MultiValue, 256);

            HOperatorSet.ShockFilter(ho_ImageOutline, out ho_SharpenedImage, 0.7, 3, "canny", 2);

            ho_EdgeAmplitude1.Dispose();
            ho_EdgeAmplitude2.Dispose();
            ho_EdgeAmplitude3.Dispose();
            ho_EdgeAmplitude4.Dispose();
            ho_ImageResult1.Dispose();
            ho_ImageResult2.Dispose();
        }


        /// <summary>
        /// 图像预处理：绘制骨骼图
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hv_NumLevel">膨胀半径</param>
        /// <param name="ho_ImageSkeleton">输出骨骼图</param>
        public static void PaintSkeletonImage(HObject ho_Image, HTuple hv_NumLevel, out HObject ho_ImageSkeleton)
        {
            HObject ho_region;
            HTuple hv_UsedThreshold;
            HObject ho_Skeleton;
            HObject ho_RegionDilation;
            HTuple hv_Width;
            HTuple hv_Height;
            HObject ho_Rectangle;
            HObject ho_Image1;
            HObject ho_ImageResult;
            HOperatorSet.BinaryThreshold(ho_Image, out  ho_region, "max_separability", "dark", out  hv_UsedThreshold);
            HOperatorSet.Skeleton(ho_region, out  ho_Skeleton);
            HOperatorSet.DilationCircle(ho_Skeleton, out  ho_RegionDilation, hv_NumLevel);

            HOperatorSet.GetImageSize(ho_Image, out  hv_Width, out  hv_Height);
            HOperatorSet.GenRectangle1(out  ho_Rectangle, 0, 0, hv_Height - 1, hv_Width - 1);
            HOperatorSet.GenImageConst(out  ho_Image1, "byte", hv_Width, hv_Height);
            HOperatorSet.PaintRegion(ho_Rectangle, ho_Image1, out  ho_ImageResult, 255, "fill");
            HOperatorSet.PaintRegion(ho_RegionDilation, ho_ImageResult, out ho_ImageSkeleton, 0, "fill");

            ho_region.Dispose();
            ho_Skeleton.Dispose();
            ho_RegionDilation.Dispose();
            ho_Rectangle.Dispose();
            ho_Image1.Dispose();
            ho_ImageResult.Dispose();
        }


        /// <summary>
        /// 区域筛选：颜色筛选
        /// </summary>
        /// <param name="ho_Image">输入RGB图像</param>
        /// <param name="hv_ValueR_Min">R最小值</param>
        /// <param name="hv_ValueR_Max">R最大值</param>
        /// <param name="hv_ValueG_Min">G最小值</param>
        /// <param name="hv_ValueG_Max">G最大值</param>
        /// <param name="hv_ValueB_Min">B最小值</param>
        /// <param name="hv_ValueB_Max">B最大值</param>
        /// <param name="selectFeatures">筛选方法</param>
        /// <param name="value_Min">筛选最小值</param>
        /// <param name="ho_ConnectionRegions">阈值分割区域</param>
        /// <param name="ho_SelectedRegions">筛选后区域</param>      
        public static void SelectColor(HObject ho_Image, HTuple hv_ValueR_Min, HTuple hv_ValueR_Max, HTuple hv_ValueG_Min, HTuple hv_ValueG_Max, HTuple hv_ValueB_Min,
                                HTuple hv_ValueB_Max, HTuple selectFeatures, HTuple value_Min, HTuple value_Max, out HObject ho_ImageR, out HObject ho_ImageG, out HObject ho_ImageB,
                                out HObject ho_ConnectionRegions, out HObject ho_SelectedRegions)
        {

            HObject ho_RegionR;
            HObject ho_ImageReducedR;
            HObject ho_RegionG;
            HObject ho_ImageReducedG;
            HObject ho_RegionB;
            HObject ho_ImageReducedB;
            HOperatorSet.Decompose3(ho_Image, out ho_ImageR, out ho_ImageG, out ho_ImageB);

            //R通道筛选
            HOperatorSet.Threshold(ho_ImageR, out  ho_RegionR, hv_ValueR_Min, hv_ValueR_Max);
            HOperatorSet.ReduceDomain(ho_Image, ho_RegionR, out  ho_ImageReducedR);

            //G通道筛选
            HOperatorSet.Threshold(ho_ImageG, out  ho_RegionG, hv_ValueG_Min, hv_ValueG_Max);
            HOperatorSet.ReduceDomain(ho_ImageReducedR, ho_RegionG, out  ho_ImageReducedG);

            //B通道筛选
            HOperatorSet.Threshold(ho_ImageB, out  ho_RegionB, hv_ValueB_Min, hv_ValueB_Max);
            HOperatorSet.ReduceDomain(ho_ImageReducedG, ho_RegionB, out  ho_ImageReducedB);

            HOperatorSet.Connection(ho_RegionB, out ho_ConnectionRegions);

            //区域筛选
            HOperatorSet.SelectShape(ho_ConnectionRegions, out ho_SelectedRegions, selectFeatures, "and", value_Min, value_Max);

            ho_RegionR.Dispose();
            ho_RegionG.Dispose();
            ho_RegionB.Dispose();
            ho_ImageReducedR.Dispose();
            ho_ImageReducedG.Dispose();
            ho_ImageReducedB.Dispose();
        }


        /// <summary>
        /// 区域筛选：针孔、破损筛选
        /// </summary>
        /// <param name="ho_Image">输入单通道图像</param>
        /// <param name="threshold">图像分割阈值</param>
        /// <param name="selectFeatures">筛选方法</param>
        /// <param name="value_Min">筛选最小值</param>
        /// <param name="value_Max">筛选最大值</param>
        /// <param name="ho_ConnectionRegions">阈值分割区域</param>
        /// <param name="ho_SelectedRegions_Min">微孔区域</param>
        /// <param name="ho_SelectedRegions_Max">针孔区域</param>
        /// <param name="ho_SelectedRegions_Max">破损区域</param>
        public static void SelectHoles(HObject ho_Image, HObject ho_ReduceRigion, HTuple threshold, HTuple selectFeatures, HTuple value_Min, HTuple value_Max, out HObject ho_ConnectionRegions,
                                        out HObject ho_SelectedRegions_Min, out HObject ho_SelectedRegions_Max)
        {
            HObject ho_ReduceImage;
            HObject ho_Region;
            HOperatorSet.ReduceDomain(ho_Image, ho_ReduceRigion, out  ho_ReduceImage);
            //阈值分割
            HOperatorSet.Threshold(ho_ReduceImage, out  ho_Region, threshold, 255);

            //区域拆分
            HOperatorSet.Connection(ho_Region, out ho_ConnectionRegions);

            //区域筛选
            HOperatorSet.SelectShape(ho_ConnectionRegions, out ho_SelectedRegions_Min, selectFeatures, "and", value_Min, value_Max);
            HOperatorSet.SelectShape(ho_ConnectionRegions, out ho_SelectedRegions_Max, selectFeatures, "and", value_Max + 0.000001, 9999999999);

            ho_Region.Dispose();
        }


        /// <summary>
        /// 区域筛选：残留筛选
        /// </summary>
        /// <param name="ho_Image">输入单通道图像</param>
        /// <param name="threshold">图像分割阈值</param>
        /// <param name="selectFeatures">筛选方法</param>
        /// <param name="value_Min">筛选最小值</param>
        /// <param name="value_Max">筛选最大值</param>
        /// <param name="ho_ConnectionRegions">阈值分割区域</param>
        /// <param name="ho_SelectedRegions">残留区域</param>
        public static void SelectResidue(HObject ho_Image, HTuple threshold, HTuple selectFeatures, HTuple value_Min, HTuple value_Max, out HObject ho_ConnectionRegions,
                                          out HObject ho_SelectedRegions_Min, out HObject ho_SelectedRegions_Max)
        {
            HObject ho_Region;
            //阈值分割
            HOperatorSet.Threshold(ho_Image, out  ho_Region, 0, threshold);

            //区域拆分
            HOperatorSet.Connection(ho_Region, out ho_ConnectionRegions);

            //区域筛选
            HOperatorSet.SelectShape(ho_ConnectionRegions, out ho_SelectedRegions_Min, selectFeatures, "and", value_Min, value_Max);
            HOperatorSet.SelectShape(ho_ConnectionRegions, out ho_SelectedRegions_Max, selectFeatures, "and", value_Max + 0.000001, 9999999999);
            ho_Region.Dispose();
        }


        /// <summary>
        /// 缺陷检测侧：极耳打折检测
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="ho_Region">输入区域</param>
        /// <param name="hv_ThresholdMin">灰度阈值最小值</param>
        /// <param name="hv_ThresholdMax">灰度阈值最大值</param>
        /// <param name="ho_ResultRegion">输出区域</param>
        /// <param name="hv_ResultnArea">输出区域面积</param>
        public static void FoldDetect(HObject ho_Image, HObject ho_Region, HTuple hv_ThresholdMin, HTuple hv_ThresholdMax, out HObject ho_ResultRegion)
        {
            HObject ho_ImageReduced;
            HOperatorSet.ReduceDomain(ho_Image, ho_Region, out  ho_ImageReduced);

            //阈值分割
            HOperatorSet.Threshold(ho_Image, out  ho_ResultRegion, hv_ThresholdMin, hv_ThresholdMax);

            ho_ImageReduced.Dispose();
        }


        /// <summary>
        /// 缺陷检测：打皱检测
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="ho_Region">输入区域</param>
        /// <param name="hv_MeanSize">滤波尺寸</param>
        /// <param name="hv_ThresholdMin">最小对比度</param>
        /// <param name="hv_ThresholdMax">最大对比度</param>
        /// <param name="ho_ResultContours">输出轮廓</param>
        public static void EdgeDetection(HObject ho_Image, HObject ho_Region, HTuple hv_MeanSize, HTuple hv_ThresholdMin, HTuple hv_ThresholdMax, out HObject ho_ImagePorcess,out HObject ho_ResultContours)
        {
            HObject ho_ImageReduced;
            HObject ho_ImageMean;
            HObject ho_ImageSub;
            HObject ho_RegionEorsion;
            HOperatorSet.ReduceDomain(ho_Image, ho_Region, out  ho_ImageReduced);
            HOperatorSet.MeanImage(ho_ImageReduced,out ho_ImageMean,hv_MeanSize, hv_MeanSize);

            HOperatorSet.SubImage(ho_ImageMean, ho_Image,out  ho_ImageSub,1,128);
            HOperatorSet.ErosionCircle(ho_Region,out  ho_RegionEorsion, hv_MeanSize);

            HOperatorSet.ReduceDomain(ho_ImageSub, ho_RegionEorsion, out ho_ImagePorcess);
            HOperatorSet.EdgesSubPix(ho_ImageSub,out ho_ResultContours, "canny", 1,hv_ThresholdMin, hv_ThresholdMax);

            ho_ImageReduced.Dispose();
            ho_ImageMean.Dispose();
            ho_ImageSub.Dispose();
            ho_RegionEorsion.Dispose();
        }


        /// <summary>
        /// Region变换：平移变换Region
        /// </summary>
        /// <param name="ho_ImagePart"></param>
        /// <param name="ho_PaintRegion"></param>
        /// <param name="ho_SelectedRegions"></param>
        /// <param name="ho_RegionAffineTrans"></param>
        public static void TranslateRegion(HObject ho_ImagePart, HObject ho_PaintRegion, HObject ho_SelectedRegions, out HObject ho_RegionAffineTrans)
        {
            HTuple hv_WidthPart;
            HTuple hv_HeightPart;
            HTuple hv_Area;
            HTuple hv_Row;
            HTuple hv_Column;
            HTuple hv_HomMat2D;
            HOperatorSet.GetImageSize(ho_ImagePart, out  hv_WidthPart, out  hv_HeightPart);
            HOperatorSet.AreaCenter(ho_PaintRegion, out  hv_Area, out  hv_Row, out  hv_Column);

            HOperatorSet.HomMat2dIdentity(out  hv_HomMat2D);
            HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_Row - (hv_HeightPart / 2), hv_Column - (hv_WidthPart / 2), out hv_HomMat2D);
            HOperatorSet.AffineTransRegion(ho_SelectedRegions, out ho_RegionAffineTrans, hv_HomMat2D, "nearest_neighbor");

        }


        /// <summary>
        /// 图像变换：平移变换图像
        /// </summary>
        /// <param name="ho_ImagePart"></param>
        /// <param name="ho_PaintRegion"></param>
        /// <param name="ho_SelectedRegions"></param>
        /// <param name="ho_RegionAffineTrans"></param>
        public static void TranslateImage(HObject ho_ImagePart, HObject ho_PaintRegion, out HObject ho_ImageAffineTrans)
        {
            HTuple hv_WidthPart;
            HTuple hv_HeightPart;
            HTuple hv_Area;
            HTuple hv_Row;
            HTuple hv_Column;
            HTuple hv_HomMat2D;
            HOperatorSet.GetImageSize(ho_ImagePart, out  hv_WidthPart, out  hv_HeightPart);
            HOperatorSet.AreaCenter(ho_PaintRegion, out  hv_Area, out  hv_Row, out  hv_Column);
            HOperatorSet.VectorAngleToRigid(hv_HeightPart / 2, hv_WidthPart / 2, 0, hv_Row, hv_Column, 0, out  hv_HomMat2D);
            HOperatorSet.AffineTransImage(ho_ImagePart, out ho_ImageAffineTrans, hv_HomMat2D, "constant", "false");
        }


        /// <summary>
        /// Region参数：获取Region外接圆、数量、直径
        /// </summary>
        /// <param name="ho_Region">输入区域</param>
        /// <param name="ho_RegionCircle">外接圆区域</param>
        /// <param name="hv_Num">数量</param>
        /// <param name="hv_Diameters">直径</param>
        public static void GetRegionCircleNumDia(HObject ho_Region, out HObject ho_RegionCircle, out HTuple hv_Num, out HTuple hv_Diameters)
        {
            HTuple hv_CircleRows;
            HTuple hv_CircleColumns;
            HTuple hv_CircleRadius;
            HOperatorSet.CountObj(ho_Region, out hv_Num);
            if (hv_Num > 0)
            {
                HOperatorSet.SmallestCircle(ho_Region, out  hv_CircleRows, out  hv_CircleColumns, out  hv_CircleRadius);
                HOperatorSet.GenCircle(out ho_RegionCircle, hv_CircleRows, hv_CircleColumns, hv_CircleRadius);
                hv_Diameters = hv_CircleRadius * 2;
            }
            else
            {
                ho_RegionCircle = null;
                hv_Diameters = null;
            }
        }


        /// <summary>
        /// 模板匹配：可变形XLd模板匹配
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="ho_RegionReduced">裁切区域</param>
        /// <param name="ho_Contours">匹配XLD轮廓</param>
        /// <param name="hv_NumLevel">图像压缩等级</param>
        /// <param name="hv_AngleRange">匹配角度范围</param>
        /// <param name="hv_ScaleRange">匹配缩放范围</param>
        /// <param name="hv_Count">最大匹配数量</param>
        /// <param name="hv_MinScore">最小匹配得分</param>
        /// <param name="hv_Row">输出目标行坐标</param>
        /// <param name="hv_Column">输出目标列坐标</param>
        /// <param name="hv_Angle">输出目标旋转角度</param>
        /// <param name="hv_Scale">输出目标缩放大小</param>
        /// <param name="hv_Score">输出目标匹配得分</param>
        public static void FindScaledShapeModel(HObject ho_Image, HObject ho_RegionReduced, HObject ho_Contours, HTuple hv_NumLevel, HTuple hv_AngleRange, HTuple hv_ScaleRange, HTuple hv_Count, HTuple hv_MinScore,
                                                out HObject ho_ModelContours, out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Angle, out HTuple hv_Scale, out HTuple hv_Score)
        {
            HTuple hv_HomMat2D;
            HObject ho_TransContours;
            HObject ho_TransRegionReduced;
            HTuple hv_Width;
            HTuple hv_Height;
            HObject ho_ImageZoom;
            HObject ho_ImageZoomReduced;
            HTuple hv_ModelID;
            HObject Contours1;
            HTuple hv_Row1;
            HTuple hv_Column1;
            HTuple hv_HomMat2D1;
            HOperatorSet.HomMat2dIdentity(out  hv_HomMat2D);
            HOperatorSet.HomMat2dScale(hv_HomMat2D, 1 / Convert.ToDouble(hv_NumLevel.ToString()), 1 / Convert.ToDouble(hv_NumLevel.ToString()), 0, 0, out hv_HomMat2D);
            HOperatorSet.AffineTransContourXld(ho_Contours, out  ho_TransContours, hv_HomMat2D);
            HOperatorSet.AffineTransRegion(ho_RegionReduced, out  ho_TransRegionReduced, hv_HomMat2D, "nearest_neighbor");

            HOperatorSet.GetImageSize(ho_Image, out  hv_Width, out  hv_Height);
            HOperatorSet.ZoomImageSize(ho_Image, out  ho_ImageZoom, hv_Width / hv_NumLevel, hv_Height / hv_NumLevel, "constant");
            HOperatorSet.ReduceDomain(ho_ImageZoom, ho_TransRegionReduced, out  ho_ImageZoomReduced);

            HOperatorSet.CreateScaledShapeModelXld(ho_TransContours, 1, (-1 * hv_AngleRange).TupleRad(), (hv_AngleRange).TupleRad(), (new HTuple(hv_AngleRange / 10 + 0.5)).TupleRad(), (1 - hv_ScaleRange),
                                                   (1 + hv_ScaleRange), hv_ScaleRange / 10 + 0.01, "auto", "ignore_local_polarity", 5, out  hv_ModelID);

            HOperatorSet.GetShapeModelContours(out  Contours1, hv_ModelID, 1);

            HOperatorSet.FindScaledShapeModel(ho_ImageZoomReduced, hv_ModelID, (-1 * hv_AngleRange).TupleRad(), (hv_AngleRange).TupleRad(), (1 - hv_ScaleRange), (1 + hv_ScaleRange), hv_MinScore, hv_Count,
                                              hv_MinScore, "least_squares", 1, hv_MinScore, out  hv_Row1, out  hv_Column1, out hv_Angle, out hv_Scale, out hv_Score);

            hv_Row = hv_Row1 * hv_NumLevel;
            hv_Column = hv_Column1 * hv_NumLevel;

            HOperatorSet.HomMat2dIdentity(out hv_HomMat2D1);
            HOperatorSet.HomMat2dRotate(hv_HomMat2D1, hv_Angle, 0, 0, out hv_HomMat2D1);
            HOperatorSet.HomMat2dScale(hv_HomMat2D1, hv_Scale * Convert.ToDouble(hv_NumLevel.ToString()), hv_Scale * Convert.ToDouble(hv_NumLevel.ToString()), 0, 0, out hv_HomMat2D1);
            HOperatorSet.HomMat2dTranslate(hv_HomMat2D1, hv_Row, hv_Column, out hv_HomMat2D1);

            HOperatorSet.AffineTransContourXld(Contours1, out ho_ModelContours, hv_HomMat2D1);

            ho_TransContours.Dispose();
            ho_TransRegionReduced.Dispose();
            ho_ImageZoom.Dispose();
            ho_ImageZoomReduced.Dispose();
            Contours1.Dispose();
        }



        /// <summary>
        /// 区域判断：判断图像中留白位置
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hv_Threshold">留白阈值</param>
        /// <param name="hv_RegionNum">ROI数量</param>
        /// <param name="hv_ColumnStart">起始列</param>
        /// <param name="hv_ColumnEnd">终止列</param>
        /// <param name="hv_Result">输出结果</param>
        public static void JudgeWhiteRegion(HObject ho_Image, HTuple hv_Threshold, HTuple hv_RegionNum, HTuple hv_ColumnStart, HTuple hv_ColumnEnd, out HTuple hv_Result)
        {
            hv_Result = null;
            HTuple hv_Width;
            HTuple hv_Height;
            HObject ho_Rectangle;
            HTuple hv_Mean;
            HTuple hv_Deviation;
            HOperatorSet.GetImageSize(ho_Image, out  hv_Width, out  hv_Height);
            for (int i = 0; i < hv_RegionNum; i++)
            {
                HOperatorSet.GenRectangle1(out  ho_Rectangle, hv_Height / hv_RegionNum * i, hv_ColumnStart, hv_Height / hv_RegionNum * (i + 1) - 1, hv_ColumnEnd);
                HOperatorSet.Intensity(ho_Rectangle, ho_Image, out  hv_Mean, out  hv_Deviation);
                if (hv_Mean >= hv_Threshold)
                    HOperatorSet.TupleConcat(hv_Result, 1, out hv_Result);
                else
                    HOperatorSet.TupleConcat(hv_Result, 0, out hv_Result);
            }

        }
    }
}
