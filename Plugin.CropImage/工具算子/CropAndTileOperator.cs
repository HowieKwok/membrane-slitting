using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;

namespace Plugin.CropImage
{
    class CropAndTileOperator
    {
        /// <summary>
        /// 裁切图像的顶部，底部小图。小图起始列相同，尺寸相同。
        /// </summary>
        /// <param name="ho_image"></param>
        /// <param name="hv_CropColumnBegin"></param>
        /// <param name="hv_CropWidth">小图宽</param>
        /// <param name="hv_CropHeiht">小图高</param>
        /// <param name="ho_ImageCroppedTOP"></param>
        /// <param name="ho_ImageCroppedBottom"></param>
        public bool CropImagePart(HObject ho_image, HTuple hv_CropColumnBegin, HTuple hv_CropWidth, HTuple hv_CropHeiht, out HObject ho_ImageCroppedTOP, out HObject ho_ImageCroppedBottom)
        {
            HTuple hv_CropColumnEnd = hv_CropColumnBegin + hv_CropWidth;
            HTuple hv_Width, hv_Height; //获取图像宽高
            HOperatorSet.GetImageSize(ho_image, out hv_Width, out hv_Height);
            if (hv_Width > hv_CropColumnEnd && hv_Height > hv_CropHeiht)
            {
                HOperatorSet.CropPart(ho_image, out ho_ImageCroppedTOP, 0, hv_CropColumnBegin, hv_CropWidth, hv_CropHeiht);
                HOperatorSet.CropPart(ho_image, out ho_ImageCroppedBottom, hv_Height - hv_CropHeiht, hv_CropColumnBegin, hv_CropWidth, hv_CropHeiht);
                return true;
            }
            else
            {
                HOperatorSet.GenEmptyObj(out ho_ImageCroppedTOP);
                HOperatorSet.GenEmptyObj(out ho_ImageCroppedBottom);
                return false;
            }
        }
        /// <summary>
        /// 两张图图像拼接，要求：数量2，宽度相同。
        /// </summary>
        /// <param name="ho_SourceImage0"></param>
        /// <param name="ho_SourceImage1"></param>
        /// <param name=""></param>
        public bool TileImage(HObject ho_SourceImage0, HObject ho_SourceImage1, out HObject ho_ImageTiled)
        {
            HObject ho_ObjectsConcat = null;
            HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);

            HTuple hv_Width0, hv_Height0;  //获取图像宽高 
            HOperatorSet.GetImageSize(ho_SourceImage0, out hv_Width0, out hv_Height0);

            HTuple hv_Width1, hv_Height1;  //获取图像宽高 
            HOperatorSet.GetImageSize(ho_SourceImage1, out hv_Width1, out hv_Height1);

            if (hv_Height0.Length > 0 && hv_Height1.Length > 0)
            {
                if (hv_Width0.I == hv_Width1.I)
                {
                    HOperatorSet.ConcatObj(ho_SourceImage0, ho_SourceImage1, out ho_ObjectsConcat);
                    HOperatorSet.TileImagesOffset(ho_ObjectsConcat, out ho_ImageTiled, (new HTuple(0)).TupleConcat(hv_Height0), (new HTuple(0)).TupleConcat(0),
                                                 (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1),
                                                  hv_Width0, hv_Height0 + hv_Height1);
                    ho_ObjectsConcat.Dispose();
                    return true;
                }
            }
            HOperatorSet.GenEmptyObj(out ho_ImageTiled);
            ho_ObjectsConcat.Dispose();
            return false;


        }

        public void Search_JR(HObject ho_image, bool IsLeft, HTuple hv_StandLineColumn, HTuple hv_Offset_Min, HTuple hv_Offset_Max, HTuple hv_threshold_Min, HTuple hv_threshold_Max,
             HTuple hv_Y_Length_Min, HTuple hv_Y_length_Max, HTuple hv_X_Length_Min, HTuple hv_X_length_Max, out HTuple JR_RowUp, out HTuple JR_RowDown)
        {
            //获取图像宽高
            HTuple hv_Width, hv_Height;
            HOperatorSet.GetImageSize(ho_image, out hv_Width, out hv_Height);

            HObject ho_Rectangle = null;
            HObject ho_ImageReduced = null;
            HObject ho_Region = null;
            HObject ho_RegionClosing1 = null;
            HObject ho_RegionOpening = null;
            HObject ho_RegionFillUp = null;
            HObject ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null;
            HObject ho_SortedRegions = null;
            HTuple Column1, Column2;

            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            //*******************生成找极耳区域**********************************************************
            if (IsLeft)
            {
                HOperatorSet.GenRectangle1(out ho_Rectangle, 0, hv_StandLineColumn - hv_Offset_Max, hv_Height - 1, hv_StandLineColumn - hv_Offset_Min);
            }
            else
            {
                HOperatorSet.GenRectangle1(out ho_Rectangle, 0, hv_StandLineColumn + hv_Offset_Max, hv_Height - 1, hv_StandLineColumn + hv_Offset_Min);
            }
            HOperatorSet.ReduceDomain(ho_image, ho_Rectangle, out ho_ImageReduced);
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, hv_threshold_Min, hv_threshold_Max);
            HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing1, 20, 20);
            HOperatorSet.OpeningRectangle1(ho_RegionClosing1, out ho_RegionOpening, 20, 20);
            HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFillUp);
            HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
            //*******************筛选极耳，从上到下排序**********************************************************
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "width", "and", hv_X_Length_Min, hv_X_length_Max);  //图像宽度方向是极耳高度方向
            HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions, "height", "and", hv_Y_Length_Min, hv_Y_length_Max);//图像高度方向是极耳宽度方向
            HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "upper_left", "true", "row");
            HOperatorSet.SmallestRectangle1(ho_SortedRegions, out JR_RowUp, out Column1, out JR_RowDown, out Column2);
        }

        public void Match_JR(HTuple JR_RowUp, HTuple JR_RowDown, bool IsInTiledImage)
        {

        }
        public void Crop_JRPart(HObject ho_image, HTuple JR_RowUp, HTuple JR_RowDown)
        {

        }


    }
}
