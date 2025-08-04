using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CropPart
{
    public class CropOperation
    {
        public bool Crop_ImagePart(List<ImageInfo> ListImage, long PartCenterID, double PartCentreRow, double PartCenterCol, double PartSizeRow, double PartSizeCol, double CropShaftRow, double CropShaftCol,
            out HObject Image_part, out HTuple beginID, out HTuple beginRow, out HTuple beginCol, out HTuple endID, out HTuple endRow, out HTuple endCol)
        {
            HOperatorSet.GenEmptyObj(out Image_part);
            beginID = -1;
            beginRow = -1;
            beginCol = -1;
            endID = -1;
            endRow = -1;
            endCol = -1;

            ImageInfo imageInfo;
            long ImageIDmin = 0;
            long ImageIDmax = 0;
            double imageHeight = 0;
            double imageWidth = 0;
            if (ListImage.Count != 0)
            {
                ImageIDmin = ListImage.First().image_ID;
                ImageIDmax = ListImage.Last().image_ID;
                imageHeight = ListImage.Last().imageHeight;
                imageWidth = ListImage.Last().imageWidth;
            }

            long crop_ID_Begin = PartCenterID;
            double crop_Row_Begin = PartCentreRow - PartSizeRow / 2 - CropShaftRow;
            while (crop_Row_Begin < -1)
            {
                crop_Row_Begin += imageHeight;
                crop_ID_Begin -= 1;
            }
            while (crop_Row_Begin > imageHeight)
            {
                crop_Row_Begin -= imageHeight;
                crop_ID_Begin += 1;
            }
            long crop_ID_End = PartCenterID;
            double crop_Row_End = PartCentreRow + PartSizeRow / 2 + CropShaftRow;
            while (crop_Row_End < -1)
            {
                crop_Row_End += imageHeight;
                crop_ID_End -= 1;
            }
            while (crop_Row_End > imageHeight)
            {
                crop_Row_End -= imageHeight;
                crop_ID_End += 1;
            }
            if (crop_ID_Begin < ImageIDmin || crop_ID_End > ImageIDmax)
            {
                return false;
            }
            double crop_Col_Begin = PartCenterCol - PartSizeCol / 2 - CropShaftCol;
            if (crop_Col_Begin < 0)
            {
                return false;
                //crop_Col_Begin = 0;              
            }
            double crop_Col_End = PartCenterCol + PartSizeCol / 2 + CropShaftCol;
            if (crop_Col_End > imageWidth)
            {
                return false;
                // crop_Col_End = imageWidth;
            }

            if (crop_Row_Begin > imageHeight - 1) crop_Row_Begin = imageHeight - 1;
            if (crop_Col_Begin > crop_Col_End - 1) crop_Col_Begin = crop_Col_End - 1;
            if (crop_Col_Begin > imageWidth - 1) crop_Col_Begin = imageWidth - 1;

            HObject Image_part0;
            HObject Image_part1;
            HOperatorSet.GenEmptyObj(out Image_part0);
            HOperatorSet.GenEmptyObj(out Image_part1);

            if (crop_ID_Begin == crop_ID_End)
            {
                imageInfo = ListImage.Find(c => c.image_ID == crop_ID_Begin);
                HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_part, crop_Row_Begin, crop_Col_Begin, crop_Row_End - 1, crop_Col_End - 1);
                beginID = crop_ID_Begin;
                beginRow = crop_Row_Begin;
                beginCol = crop_Col_Begin;
                endID = crop_ID_End;
                endRow = crop_Row_End - 1;
                endCol = crop_Col_End - 1;
            }
            else//最多拼两张图，中间的图跳过
            {
                imageInfo = ListImage.Find(c => c.image_ID == crop_ID_Begin);
                HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_part0, crop_Row_Begin, crop_Col_Begin, imageInfo.imageHeight - 1, crop_Col_End - 1);
                imageInfo = ListImage.Find(c => c.image_ID == crop_ID_End);
                HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_part1, 0, crop_Col_Begin, crop_Row_End - 1, crop_Col_End - 1);
                TileImage(Image_part0, Image_part1, out Image_part);
                beginID = crop_ID_Begin;
                beginRow = crop_Row_Begin;
                beginCol = crop_Col_Begin;
                endID = crop_ID_End;
                endRow = crop_Row_End - 1;
                endCol = crop_Col_End - 1;
            }

            HTuple w, h;
            HOperatorSet.GetImageSize(Image_part, out w, out h);
            if (w > 0 && h > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            Image_part0.Dispose();
            Image_part1.Dispose();
        }
        /// <summary>
        /// 两张图图像拼接，要求：数量2，宽度相同。
        /// </summary>
        /// <param name="ho_SourceImage0"></param>
        /// <param name="ho_SourceImage1"></param>
        /// <param name=""></param>
        public void TileImage(HObject ho_SourceImage0, HObject ho_SourceImage1, out HObject ho_ImageTiled)
        {
            HObject ho_ObjectsConcat = null;
            HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);

            HTuple hv_Width0, hv_Height0;  //获取图像宽高 
            HOperatorSet.GetImageSize(ho_SourceImage0, out hv_Width0, out hv_Height0);
            HTuple hv_Width1, hv_Height1;  //获取图像宽高 
            HOperatorSet.GetImageSize(ho_SourceImage1, out hv_Width1, out hv_Height1);
            if (hv_Width0.I != hv_Width1.I)
            {
                HOperatorSet.GenEmptyObj(out ho_ImageTiled);
                ho_ObjectsConcat.Dispose();
                return;
            }
            HOperatorSet.ConcatObj(ho_SourceImage0, ho_SourceImage1, out ho_ObjectsConcat);
            HOperatorSet.TileImagesOffset(ho_ObjectsConcat, out ho_ImageTiled, (new HTuple(0)).TupleConcat(hv_Height0), (new HTuple(0)).TupleConcat(0),
                                         (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1),
                                          hv_Width0, hv_Height0 + hv_Height1);
            ho_ObjectsConcat.Dispose();
        }
    }
}
