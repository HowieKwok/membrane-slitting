using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.FindHead
{
    public enum ErrorInfo
    {
        ERROR = 0,//出错
        SUCCECE//运行未出错
    };
    class splicingAndCropOperation
    {
        /// <summary>
        /// 用于图像拼接及其图像裁切的类
        /// </summary>

        public ErrorInfo E_PictureStitchingFanctionInfo;//图像拼接运行的状态
        public ErrorInfo E_CropImageFanctionInfo;//图像裁切运行的状态

        ///// <summary>
        ///// 标准构造函数
        ///// </summary>
        //public  C_Tool_ImageSplicingAndCrop()
        //{

        //}
        /// <summary>
        /// 图像拼接，可拼接多张不等高的图像,使用方法“hobject img = DifferentHeightPictureStitching(HObject _ho_Image)”
        /// </summary>
        /// <param name="_Images">输入图像数组</param>
        /// <returns></returns>
        public HObject DifferentHeightPictureStitching(HObject _ho_Images)
        {
            int AddHeight = 0;
            HTuple ImageNum1 = null;
            HTuple H, W;
            HTuple H1, W1;

            HObject Image = null;
            HObject selectImage = null;
            HObject concatImage = null;
            HObject addImage = null;

            HOperatorSet.GenEmptyObj(out Image);
            try
            {
                HOperatorSet.CountObj(_ho_Images, out ImageNum1);
                HOperatorSet.SelectObj(_ho_Images, out Image, 1);
                int[] offset = new int[2];
                int[] offset1 = new int[2];
                int[] offset2 = { -1, -1 };
                for (int i = 1; i < ImageNum1.I; i++)
                {
                    W = null;
                    H = null;
                    HOperatorSet.GetImageSize(Image, out W, out H);
                    HOperatorSet.SelectObj(_ho_Images, out selectImage, i + 1);
                    HOperatorSet.GetImageSize(selectImage, out W1, out H1);
                    AddHeight = H1.I + H.I;
                    HOperatorSet.ConcatObj(Image, selectImage, out concatImage);
                    offset[0] = 0;
                    offset[1] = 0;
                    offset1[0] = 0;
                    offset1[1] = H;
                    HOperatorSet.TileImagesOffset(concatImage, out addImage, offset1, offset, offset2, offset2, offset2, offset2, W, AddHeight);
                    Image = addImage;
                    E_PictureStitchingFanctionInfo = ErrorInfo.SUCCECE;//表示图像没出错
                }
            }
            catch
            {
                E_PictureStitchingFanctionInfo = ErrorInfo.ERROR;
            }
            return Image;
        }

        /// <summary>
        /// 两张图片的拼接
        /// </summary>
        /// <param name="_ho_image1">输入：图片1</param>
        /// <param name="_ho_Image2">输入：图片2</param>
        /// <returns></returns>
        public HObject DifferentHeightPictureStitching(HObject _ho_image1, HObject _ho_Image2)
        {
            int AddHeight = 0;
            HTuple ImageNum1 = null;
            HTuple H, W;
            HTuple H1, W1;

            HObject Image = null;
            HObject selectImage = null;
            HObject concatImage = null;
            HObject addImage = null;
            HObject _ho_Images;
            HOperatorSet.ConcatObj(_ho_image1, _ho_Image2, out _ho_Images);

            HOperatorSet.GenEmptyObj(out Image);
            try
            {
                HOperatorSet.CountObj(_ho_Images, out ImageNum1);
                HOperatorSet.SelectObj(_ho_Images, out Image, 1);
                int[] offset = new int[2];
                int[] offset1 = new int[2];
                int[] offset2 = { -1, -1 };
                for (int i = 1; i < ImageNum1.I; i++)
                {
                    W = null;
                    H = null;
                    HOperatorSet.GetImageSize(Image, out W, out H);
                    HOperatorSet.SelectObj(_ho_Images, out selectImage, i + 1);
                    HOperatorSet.GetImageSize(selectImage, out W1, out H1);
                    AddHeight = H1.I + H.I;
                    HOperatorSet.ConcatObj(Image, selectImage, out concatImage);
                    offset[0] = 0;
                    offset[1] = 0;
                    offset1[0] = 0;
                    offset1[1] = H;
                    HOperatorSet.TileImagesOffset(concatImage, out addImage, offset1, offset, offset2, offset2, offset2, offset2, W, AddHeight);
                    Image = addImage;
                    E_PictureStitchingFanctionInfo = ErrorInfo.SUCCECE;//表示图像没出错
                }
            }
            catch
            {
                E_PictureStitchingFanctionInfo = ErrorInfo.ERROR;
            }
            return Image;
        }


        /// <summary>
        /// 图像拼裁,使用方法：“hobject img = OutStitchingAndCropImage(List<HObject> _List_Images,int Row1, int Row2)”
        /// </summary>
        /// <param name="_List_Images">图像列表</param>
        /// <param name="Row1">首张图像裁切行</param>
        /// <param name="Row2">尾张图像裁切行</param>
        /// <returns></returns>
        public HObject OutStitchingAndCropImage(HObject _Image, double Row1, double Row2, double Col1, double Col2)
        {
            HTuple H, W;

            HObject ho_OutImage = null;
            HOperatorSet.GenEmptyObj(out ho_OutImage);
            try
            {
                HOperatorSet.GetImageSize(_Image, out W, out H);
                double cW = Col2 - Col1;
                double cH = Row2 - Row1;
                //裁切
                HOperatorSet.CropPart(_Image, out ho_OutImage, Row1, Col1, cW, cH);
                E_CropImageFanctionInfo = ErrorInfo.SUCCECE;
            }
            catch (Exception ee)
            {
                E_CropImageFanctionInfo = ErrorInfo.ERROR;
            }
            return ho_OutImage;
        }

        /// <summary>
        /// 图像拼裁,使用方法：“hobject img = OutStitchingAndCropImage(List<HObject> _List_Images,int Row1, int Row2)”
        /// </summary>
        /// <param name="_List_Images">图像列表</param>
        /// <param name="Row1">首张图像裁切行</param>
        /// <param name="Row2">尾张图像裁切行</param>
        /// <returns></returns>
        public HObject OutStitchingAndCropImage(List<HObject> _List_Images, double Row1, double Row2, double Col1, double Col2)
        {
            HTuple H, W;
            HTuple hv_LastH, hv_LastW;//结束图像的宽高
            int i_AllH = 0;//总图像的高度
            int i_NoNeedHeight = 0;//存尾部不需要的高度

            HObject ho_images = null;//将List中图片转换为Hobject数组
            HObject ho_OutImage = null;

            HOperatorSet.GenEmptyObj(out ho_OutImage);
            HOperatorSet.GenEmptyObj(out ho_images);
            try
            {
                foreach (var item in _List_Images)
                {
                    HOperatorSet.ConcatObj(ho_images, item, out ho_images);
                    HOperatorSet.GetImageSize(item, out W, out H);
                    i_AllH += H;
                }
                HTuple hv_Num;//图像数组中图片数量
                HOperatorSet.CountObj(ho_images, out hv_Num);
                HObject ho_LastImage;//图像数组中最后一张图片
                HOperatorSet.SelectObj(ho_images, out ho_LastImage, hv_Num);
                HOperatorSet.GetImageSize(ho_LastImage, out hv_LastW, out hv_LastH);
                i_NoNeedHeight = hv_LastH - Row2;
                double cW = Col2 - Col1;
                //拼接
                HObject Image1;
                Image1 = DifferentHeightPictureStitching(ho_images);//输出拼接后的图               
                //裁切
                HOperatorSet.CropPart(Image1, out ho_OutImage, Row1, Col1, cW, i_AllH - i_NoNeedHeight - Row1);
                E_CropImageFanctionInfo = ErrorInfo.SUCCECE;
            }
            catch
            {
                E_CropImageFanctionInfo = ErrorInfo.ERROR;
            }
            return ho_OutImage;
        }

        /// <summary>
        /// 图像拼裁,使用方法：“hobject img = OutStitchingAndCropImage(List<Struct_Cam> _List_Images, int ID1, int Row1, int ID2, int Row2)”
        /// </summary>
        /// <param name="_List_Images">图像列表</param>
        /// <param name="ID1">首张图像ID</param>
        /// <param name="Row1">首张图像裁切行</param>
        /// <param name="ID2">尾张图像ID</param>
        /// <param name="Row2">尾张图像裁切行</param>
        /// <returns></returns>
        public HObject OutStitchingAndCropImage(List<ImageInfo> _List_Images, int ID1, double Row1, int ID2, double Row2, double Col1, double Col2)
        {
            HTuple H, W;
            HTuple hv_LastH, hv_LastW;//结束图像的宽高
            double i_AllH = 0;//总图像的高度
            double i_NoNeedHeight = 0;//存尾部不需要的高度

            HObject ho_images = null;//将List中图片转换为Hobject数组
            HObject ho_OutImage = null;

            HOperatorSet.GenEmptyObj(out ho_OutImage);
            HOperatorSet.GenEmptyObj(out ho_images);


            try
            {
                foreach (var item in _List_Images)
                {
                    if (item.image_ID >= ID1 && item.image_ID <= ID2)
                    {
                        HOperatorSet.ConcatObj(ho_images, item.ho_image, out ho_images);
                        i_AllH += item.imageHeight;
                    }
                }
                HTuple hv_Num;//图像数组中图片数量
                HOperatorSet.CountObj(ho_images, out hv_Num);
                HObject ho_LastImage;//图像数组中最后一张图片
                HOperatorSet.SelectObj(ho_images, out ho_LastImage, hv_Num);
                HOperatorSet.GetImageSize(ho_LastImage, out hv_LastW, out hv_LastH);
                i_NoNeedHeight = hv_LastH - Row2;//计算需要丢掉的行高
                double cW = Col2 - Col1;
                //拼接
                HObject Image1;
                Image1 = DifferentHeightPictureStitching(ho_images);//输出拼接后的图
                int i_imageNum = ID2 - ID1 + 1;

                //裁切
                double imheight = i_AllH - i_NoNeedHeight - Row1;
                if (imheight > 0)
                {
                    HOperatorSet.CropPart(Image1, out ho_OutImage, Row1, Col1, cW, i_AllH - i_NoNeedHeight - Row1);
                }
              
                E_CropImageFanctionInfo = ErrorInfo.SUCCECE;
            }
            catch (Exception ee)
            {
                //Console.WriteLine("拼图模块::截图错误" + ee.ToString());
                E_CropImageFanctionInfo = ErrorInfo.ERROR;
            }
            return ho_OutImage;

        }

        public bool Crop_ImagePart(List<ImageInfo> ListImage, int ID1, double Row1, int ID2, double Row2, double Col1, double Col2, out HObject Image_part)
        {
            HOperatorSet.GenEmptyObj(out Image_part);
            ImageInfo imageInfo;
            int ImageIDmin = 0;
            int ImageIDmax = 0;
            double imageHeight = 0;
            double imageWidth = 0;
            if (ListImage.Count != 0)
            {
                ImageIDmin = (int)ListImage.First().image_ID;
                ImageIDmax = (int)ListImage.Last().image_ID;
                imageHeight = ListImage.Last().imageHeight;
                imageWidth = ListImage.Last().imageWidth;
            }

            int crop_ID_Begin = ID1;
            double crop_Row_Begin = Row1;
            while (crop_Row_Begin < 0)
            {
                crop_Row_Begin += imageHeight;
                crop_ID_Begin -= 1;
            }
            while (crop_Row_Begin > imageHeight)
            {
                crop_Row_Begin -= imageHeight;
                crop_ID_Begin += 1;
            }
            int crop_ID_End = ID2;
            double crop_Row_End = Row2;
            while (crop_Row_End < 0)
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
            double crop_Col_Begin = Col1;
            if (crop_Col_Begin < 0)
            {
                return false;
                //crop_Col_Begin = 0;              
            }
            double crop_Col_End = Col2;
            if (crop_Col_End > imageWidth)
            {
                return false;
                // crop_Col_End = imageWidth;
            }
            HObject Image_part0;
            HObject Image_part1;
            HOperatorSet.GenEmptyObj(out Image_part0);
            HOperatorSet.GenEmptyObj(out Image_part1);
            if (crop_ID_Begin == crop_ID_End)
            {
                imageInfo = ListImage.Find(c => c.image_ID == crop_ID_Begin);
                HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_part, crop_Row_Begin, crop_Col_Begin, crop_Row_End - 1, crop_Col_End - 1);
            }
            else//最多拼两张图，中间的图跳过
            {
                imageInfo = ListImage.Find(c => c.image_ID == crop_ID_Begin);
                HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_part0, crop_Row_Begin, crop_Col_Begin, imageInfo.imageHeight - 1, crop_Col_End - 1);
                imageInfo = ListImage.Find(c => c.image_ID == crop_ID_End);
                HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_part1, 0, crop_Col_Begin, crop_Row_End - 1, crop_Col_End - 1);
                TileImage(Image_part0, Image_part1, out Image_part);
            }
            Image_part0.Dispose();
            Image_part1.Dispose();
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
