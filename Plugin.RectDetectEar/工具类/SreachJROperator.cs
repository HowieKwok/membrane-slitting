using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using FrameCommunity.Data;

namespace Plugin.RectDetectEar
{
    class SreachJROperator
    {
        ////方法一，裁切图像检测极耳，查找所有目标极耳，不检测缺失极耳。
        /// <summary>
        /// 裁切图像的顶部，底部小图。小图起始列相同，尺寸相同。
        /// </summary>
        /// <param name="ho_image"></param>
        /// <param name="hv_CropColumnBegin"></param>
        /// <param name="hv_CropWidth">小图宽</param>
        /// <param name="hv_CropHeiht">小图高</param>
        /// <param name="ho_ImageCroppedTOP"></param>
        /// <param name="ho_ImageCroppedBottom"></param>
        public void CropImagePart(HObject ho_image, HTuple hv_CropColumnBegin, HTuple hv_CropWidth, HTuple hv_CropHeiht, out HObject ho_ImageCroppedTOP, out HObject ho_ImageCroppedBottom)
        {

            HTuple hv_CropColumnEnd = hv_CropColumnBegin + hv_CropWidth;
            HTuple hv_Width, hv_Height; //获取图像宽高
            HOperatorSet.GetImageSize(ho_image, out hv_Width, out hv_Height);
            if (hv_Width > hv_CropColumnEnd && hv_Height > hv_CropHeiht)
            {
                HOperatorSet.CropPart(ho_image, out ho_ImageCroppedTOP, 0, hv_CropColumnBegin, hv_CropWidth, hv_CropHeiht);
                HOperatorSet.CropPart(ho_image, out ho_ImageCroppedBottom, hv_Height - hv_CropHeiht, hv_CropColumnBegin, hv_CropWidth, hv_CropHeiht);
            }
            else
            {
                HOperatorSet.GenEmptyObj(out ho_ImageCroppedTOP);
                HOperatorSet.GenEmptyObj(out ho_ImageCroppedBottom);
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

        public void Search_JR(HObject ho_image, HTuple hv_Column1, HTuple hv_Column2, HTuple hv_threshold_Min, HTuple hv_threshold_Max,
             HTuple hv_Y_Length_Min, HTuple hv_Y_length_Max, HTuple hv_X_Length_Min, HTuple hv_X_length_Max, out HTuple JR_RowUp, out HTuple JR_RowDown, out HObject Region)
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
            HTuple Column1, Column2;

            HOperatorSet.GenEmptyObj(out Region);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);

            //*******************生成找极耳区域**********************************************************
            HOperatorSet.GenRectangle1(out ho_Rectangle, 0, hv_Column1, hv_Height - 1, hv_Column2);

            HOperatorSet.ReduceDomain(ho_image, ho_Rectangle, out ho_ImageReduced);
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, hv_threshold_Min, hv_threshold_Max);
            HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing1, 20, 20);
            HOperatorSet.OpeningRectangle1(ho_RegionClosing1, out ho_RegionOpening, 20, 20);
            HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFillUp);
            HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
            //*******************筛选极耳，从上到下排序**********************************************************
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "width", "and", hv_X_Length_Min, hv_X_length_Max);  //图像宽度方向是极耳高度方向
            HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions, "height", "and", hv_Y_Length_Min, hv_Y_length_Max);//图像高度方向是极耳宽度方向
            //剔除图像顶部和底部的极耳
            HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions, "row1", "and", 10, hv_Height);  //图像宽度方向是极耳高度方向
            HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions, "row2", "and", 0, hv_Height - 10);//图像高度方向是极耳宽度方向

            HOperatorSet.SortRegion(ho_SelectedRegions, out Region, "upper_left", "true", "row");
            HOperatorSet.SmallestRectangle1(Region, out JR_RowUp, out Column1, out JR_RowDown, out Column2);

            ho_Rectangle.Dispose();
            ho_ImageReduced.Dispose();
            ho_Region.Dispose();
            ho_RegionClosing1.Dispose();
            ho_RegionOpening.Dispose();
            ho_RegionFillUp.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
        }

        public void Match_JR(List<PoleEarsInfo> listJRparams_in, List<PoleEarsInfo> listJRparams_out, bool IsLeft, long Image_Id, HTuple hv_CropHeiht, HTuple ImageHeight, HTuple JR_RowUp, HTuple JR_RowDown, bool IsTiledImage, out long JRcount_Matched)
        {
            PoleEarsInfo JRInfo;
            long JR_id = 0;
            long JRcount_InImage = (new HTuple(JR_RowUp.TupleLength()));
            long JRcount_toMatch = JRcount_InImage; //检测出疑似极耳待匹配入列的数量。
            long[] Image_Id_Begin = new long[JRcount_InImage];
            for (long i = 0; i < JRcount_InImage; i++)
            {
                Image_Id_Begin[i] = Image_Id;
            }
            long[] Image_Id_End = new long[JRcount_InImage];
            for (int i = 0; i < JRcount_InImage; i++)
            {
                Image_Id_End[i] = Image_Id;
            }
            if (listJRparams_in.Count != 0)
            {
                JRInfo = listJRparams_in.Last();
                JR_id = JRInfo.JR_ID;
            }
            else //队列为空时不匹配，直接加入队列
            {
                for (int i = 0; i < JRcount_InImage; i++)
                {
                    PoleEarsInfo JRparams = new PoleEarsInfo();

                    JRparams.JR_ID = i;
                    JRparams.IsLeft = IsLeft;
                    JRparams.image_ID_Begin = Image_Id_Begin[i];
                    JRparams.Row_Begin = JR_RowUp[i];
                    JRparams.image_ID_End = Image_Id_End[i];
                    JRparams.Row_End = JR_RowDown[i];
                    double add = (JRparams.image_ID_Begin - JRparams.image_ID_End) * ImageHeight.D + JRparams.Row_End + JRparams.Row_Begin;
                    JRparams.image_ID_Mid = Image_Id_Begin[i] + (int)((add / 2) / ImageHeight.D); //除图像高取商。
                    JRparams.Row_Mid = (add / 2) % ImageHeight.D; //除图像高取余。
                    listJRparams_out.Add(JRparams);
                    JRparams = null;
                }
                JRcount_Matched = JRcount_InImage;
                return;
            }
            JRcount_Matched = 0; //匹配成功，入列数量。
            bool bl_recordJR = false;//新极耳，加入队列标志位。
            bool bl_BeginMatched = true;//起始行相同标志位
            bool bl_EndMatched = false;//终止行相同标志位
            bool bl_OverLong = false; //超长余料标志位。

            if (JRcount_InImage > 0)
            {
                if (IsTiledImage)
                {
                    for (int i = 0; i < JRcount_InImage; i++)
                    {     //上半部分拼图需补一个图像高度。
                        if (JR_RowUp[i].D <= hv_CropHeiht)
                        {
                            JR_RowUp[i] = JR_RowUp[i] - hv_CropHeiht + ImageHeight;
                            Image_Id_Begin[i] = Image_Id - 1;
                        }
                        else
                        {
                            JR_RowUp[i] = JR_RowUp[i] - hv_CropHeiht;
                            Image_Id_Begin[i] = Image_Id;
                        }
                        //下半部分拼图不用补。
                        if (JR_RowDown[i].D <= hv_CropHeiht)
                        {
                            JR_RowDown[i] = JR_RowDown[i] - hv_CropHeiht + ImageHeight;
                            Image_Id_End[i] = Image_Id - 1;
                        }
                        else
                        {
                            JR_RowDown[i] = JR_RowDown[i] - hv_CropHeiht;
                            Image_Id_End[i] = Image_Id;
                        }
                    }
                }
                //遍历图片中找到的极耳，与已有的极耳参数队列匹配。
                for (int i = 0; i < JRcount_InImage; i++)
                {
                    bl_recordJR = true;
                    for (int j = 0; j < JRcount_toMatch; j++)
                    {
                        long listMatchingIndex = JR_id + 1 - JRcount_toMatch + j; //队列中 正在匹配 的index
                        if (listMatchingIndex < 0)
                        {
                            listMatchingIndex = 0;
                        }
                        JRInfo = listJRparams_in.Find(c => c.JR_ID == listMatchingIndex);
                        bl_BeginMatched = Math.Abs(JRInfo.Row_Begin - JR_RowUp[i].D) < 5.0;
                        bl_EndMatched = Math.Abs(JRInfo.Row_End - JR_RowDown[i].D) < 5.0;

                        if (JRInfo.Row_End == (ImageHeight.D - 1) && IsTiledImage)//极耳list中的极耳尾部行，等于极耳高度，默认极耳还未结束，可能超长余料，也可能是正常极耳的巧合。
                        {
                            bl_OverLong = true;
                        }
                        else if (JRInfo.Row_End == (hv_CropHeiht.D - 1) && !IsTiledImage)//拼图匹配，则list最后是等于原图高度，原图匹配，则list最后是等于拼图高度。
                        {
                            bl_OverLong = true;
                        }
                        //常规情况：没有超长余料，则极耳在拼图或原图中一定能有一条自然边。自然边会被检测两次，通过自然边匹配为同一个极耳。
                        if (bl_BeginMatched || bl_EndMatched) //只要有一条边能匹配上，则说明队列中已经有该极耳，直接更新尾部数据。
                        {
                            JRInfo.image_ID_End = Image_Id_End[i];
                            JRInfo.Row_End = JR_RowDown[i].D;
                            JRcount_toMatch -= j + 1;//快速顺序匹配
                            bl_recordJR = false;
                        }
                        //超长余料情况
                        else if (bl_OverLong)
                        {
                            JRInfo.image_ID_End = Image_Id_End[i];
                            JRInfo.Row_End = JR_RowDown[i].D;
                            JRcount_toMatch -= j + 1;//快速顺序匹配
                            bl_recordJR = false;
                        }
                        bl_BeginMatched = false;
                        bl_EndMatched = false;
                    }
                    if (bl_recordJR)
                    {
                        JRcount_Matched++;
                        PoleEarsInfo JRparams = new PoleEarsInfo();

                        JRparams.JR_ID = JR_id + 1;
                        JRparams.IsLeft = IsLeft;
                        JRparams.image_ID_Begin = Image_Id_Begin[i];
                        JRparams.Row_Begin = JR_RowUp[i];
                        JRparams.image_ID_End = Image_Id_End[i];
                        JRparams.Row_End = JR_RowDown[i];
                        double add = (JRparams.image_ID_Begin - JRparams.image_ID_End) * ImageHeight.D + JRparams.Row_End + JRparams.Row_Begin;
                        JRparams.image_ID_Mid = Image_Id_Begin[i] + (int)((add / 2) / ImageHeight.D); //除图像高取商。
                        JRparams.Row_Mid = (add / 2) % ImageHeight.D; //除图像高取余。
                        listJRparams_out.Add(JRparams);
                        JR_id++;
                        JRparams = null;
                    }
                }
            }

        }
        public void Crop_JRPart(List<PoleEarsInfo> listJRparams, List<ImageInfo> ListImage, bool Is_left, long JRcount_Matched, double CropColumnBegin, double X_length_Max, double JRcropShaft_X, double JRcropShaft_Y)
        {
            PoleEarsInfo JRInfo;
            ImageInfo imageInfo;
            long JR_id = 0;
            if (listJRparams.Count != 0)
            {
                JRInfo = listJRparams.Last();
                JR_id = JRInfo.JR_ID;
            }

            long JRcrop_ID_Begin;
            long JRcrop_ID_End;
            double JRcrop_Row_Begin;
            double JRcrop_Row_End;

            HObject Image_JRpart0;
            HObject Image_JRpart1;
            HOperatorSet.GenEmptyObj(out Image_JRpart0);
            HOperatorSet.GenEmptyObj(out Image_JRpart1);


            for (long i = JR_id - JRcount_Matched; i < JR_id; i++)//新加入队列的JR，需要拼图。最后一个JR可能不完整，不拼图
            {
                if (i < 0)
                {
                    i = 0;
                }
                JRInfo = listJRparams.Find(c => c.JR_ID == i);
                imageInfo = ListImage.Find(c => c.image_ID == JRInfo.image_ID_Begin);
                JRcrop_Row_Begin = JRInfo.Row_Begin - JRcropShaft_Y;
                JRcrop_Row_End = JRInfo.Row_End + JRcropShaft_Y;
                if (JRcrop_Row_Begin >= 0)
                {
                    JRcrop_ID_Begin = JRInfo.image_ID_Begin;
                }
                else
                {
                    JRcrop_ID_Begin = JRInfo.image_ID_Begin - 1;
                    JRcrop_Row_Begin = JRcrop_Row_Begin + imageInfo.imageHeight;
                    if (JRcrop_ID_Begin < 0)
                    {
                        JRcrop_ID_Begin = 0;
                        JRcrop_Row_Begin = 0;
                    }

                }
                if (JRcrop_Row_End <= imageInfo.imageHeight)
                {
                    JRcrop_ID_End = JRInfo.image_ID_End;
                }
                else
                {
                    JRcrop_ID_End = JRInfo.image_ID_End + 1;
                    JRcrop_Row_End = JRcrop_Row_End - imageInfo.imageHeight;
                }

                if (JRcrop_ID_Begin == JRcrop_ID_End)
                {
                    imageInfo = ListImage.Find(c => c.image_ID == JRcrop_ID_Begin);
                    if (Is_left)
                    {
                        HOperatorSet.CropRectangle1(imageInfo.ho_image, out JRInfo.Image_JRpart, JRcrop_Row_Begin, CropColumnBegin - JRcropShaft_X - X_length_Max, JRcrop_Row_End - 1, CropColumnBegin + JRcropShaft_X - 1);
                    }
                    else
                    {
                        HOperatorSet.CropRectangle1(imageInfo.ho_image, out JRInfo.Image_JRpart, JRcrop_Row_Begin, CropColumnBegin - JRcropShaft_X, JRcrop_Row_End - 1, CropColumnBegin + JRcropShaft_X + X_length_Max - 1);
                    }
                }
                else
                {
                    if (Is_left)
                    {
                        imageInfo = ListImage.Find(c => c.image_ID == JRcrop_ID_Begin);
                        // HOperatorSet.CropPart(imageInfo.ho_image, out Image_JRpart0, JRcrop_Row_Begin, CropColumnBegin - JRcropShaft_X - X_length_Max,);
                        HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_JRpart0, JRcrop_Row_Begin, CropColumnBegin - JRcropShaft_X - X_length_Max, imageInfo.imageHeight - 1, CropColumnBegin + JRcropShaft_X - 1);
                        imageInfo = ListImage.Find(c => c.image_ID == JRcrop_ID_End);
                        HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_JRpart1, 0, CropColumnBegin - JRcropShaft_X - X_length_Max, JRcrop_Row_End - 1, CropColumnBegin + JRcropShaft_X - 1);
                        TileImage(Image_JRpart0, Image_JRpart1, out JRInfo.Image_JRpart);
                    }
                    else
                    {
                        imageInfo = ListImage.Find(c => c.image_ID == JRcrop_ID_Begin);
                        HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_JRpart0, JRcrop_Row_Begin, CropColumnBegin - JRcropShaft_X, imageInfo.imageHeight - 1, CropColumnBegin + JRcropShaft_X + X_length_Max - 1);
                        imageInfo = ListImage.Find(c => c.image_ID == JRcrop_ID_End);
                        HOperatorSet.CropRectangle1(imageInfo.ho_image, out Image_JRpart1, 0, CropColumnBegin - JRcropShaft_X, JRcrop_Row_End - 1, CropColumnBegin + JRcropShaft_X + X_length_Max - 1);
                        TileImage(Image_JRpart0, Image_JRpart1, out JRInfo.Image_JRpart);
                    }
                }
            }
        }

        ////方法二，只检测原图中的特征，查找目标极耳，可以补全缺失极耳，适用于等间距。
        //public void ExpandJR(List<PoleEarsInfo> listJRparams_in, List<PoleEarsInfo> listJRparams_out, int Image_Id, double ImageHeight, HTuple JR_RowUP, HTuple JR_RowDown, double JRInterval, double JRLengthY, out int JRcount_Matched)
        //{
        //    List<double> HeadRowUp = new List<double>();
        //    List<double> HeadRowDown = new List<double>();
        //    JRcount_Matched = 0;
        //    //匹配检测值和预测值
        //    HeadRowUp.Clear();
        //    HeadRowDown.Clear();
        //    HTuple tempstart = 0;
        //    if (listJRparams_in.Count() > 2)
        //    {
        //        tempstart = (listJRparams_in.Last().image_ID_End - Image_Id) * ImageHeight + listJRparams_in.Last().Row_End;
        //    }
        //    HOperatorSet.TupleConcat(tempstart, JR_RowDown, out JR_RowDown);
        //    HOperatorSet.TupleConcat(tempstart, JR_RowUP, out JR_RowUP);
        //    HTuple tempend = ImageHeight;
        //    HOperatorSet.TupleConcat(JR_RowDown, tempend, out JR_RowDown);
        //    HOperatorSet.TupleConcat(JR_RowUP, tempend - JRLengthY, out JR_RowUP);

        //    int matchCount = (new HTuple(JR_RowDown.TupleLength()));
        //    double tempRow0 = 0;
        //    bool tempflag = true;
        //    for (int i = 0; i < matchCount - 1; i++)
        //    {
        //        tempRow0 = JR_RowDown[i].D + JRInterval;
        //        tempflag = JR_RowDown[i + 1].D - tempRow0 > JRLengthY; //推测的temp比下一个料边大一个槽位高度，则直接使用下一个料边。小于则使用temp
        //        while (tempflag)
        //        {
        //            HeadRowUp.Add(tempRow0 - JRLengthY);
        //            HeadRowDown.Add(tempRow0);

        //            tempRow0 = tempRow0 + JRInterval;
        //            tempflag = JR_RowDown[i + 1].D - tempRow0 > JRLengthY; //推测的temp比下一个料边大一个槽位高度，则直接使用下一个料边。小于则使用temp
        //        }
        //        if (i != matchCount - 2)
        //        {
        //            HeadRowUp.Add(JR_RowUP[i + 1].D);
        //            HeadRowDown.Add(JR_RowDown[i + 1].D);

        //        }
        //    }
        //    //添加极耳队列
        //    int count0 = HeadRowDown.Count;
        //    for (int i = 0; i < count0; i++)
        //    {
        //        PoleEarsInfo JRparams = new PoleEarsInfo();

        //        JRparams.JR_ID = i;
        //        JRparams.IsLeft = true;
        //        JRparams.image_ID_Begin = Image_Id;
        //        JRparams.Row_Begin = HeadRowUp[i];
        //        JRparams.image_ID_End = Image_Id;
        //        JRparams.Row_End = HeadRowDown[i];
        //        double add = (JRparams.image_ID_Begin - JRparams.image_ID_End) * ImageHeight + JRparams.Row_End + JRparams.Row_Begin;
        //        JRparams.image_ID_Mid = Image_Id + (int)((add / 2) / ImageHeight); //除图像高取商。
        //        JRparams.Row_Mid = (add / 2) % ImageHeight; //除图像高取余。
        //        listJRparams_out.Add(JRparams);
        //        JRparams = null;
        //    }
        //    JRcount_Matched = count0;
        //}

        public void ExpandJR(List<PoleEarsInfo> listJRparams_in, List<PoleEarsInfo> listJRparams_out, long Image_Id, double ImageHeight,
             HTuple JR_RowUP, HTuple JR_RowDown, double JRInterval, double JRLengthY, out int JRcount_Matched)
        {
            // 初始化匹配计数器
            JRcount_Matched = 0;
            long JRstartIndex = 0;
            if (listJRparams_in.Count > 0)
            {
                JRstartIndex = listJRparams_in.Last().JR_ID + 1;
            }

            // 计算起始点
            HTuple tempstartmid = listJRparams_in.Count > 2 ? (listJRparams_in.Last().image_ID_Mid - Image_Id) * ImageHeight + listJRparams_in.Last().Row_Mid : 0;
            HTuple tempstart = listJRparams_in.Count > 2 ? (listJRparams_in.Last().image_ID_End - Image_Id) * ImageHeight + listJRparams_in.Last().Row_End : 0;

            // 更新极耳的上下行坐标
            HTuple JR_RowMid = tempstartmid.TupleConcat((JR_RowUP + JR_RowDown) / 2).TupleConcat(ImageHeight - JRLengthY / 2);
            JR_RowDown = tempstart.TupleConcat(JR_RowDown).TupleConcat(ImageHeight);
            JR_RowUP = tempstart.TupleConcat(JR_RowUP).TupleConcat(ImageHeight - JRLengthY);

            // 循环遍历匹配的极耳，并补全缺失的极耳信息
            for (int i = 0, count = JR_RowDown.TupleLength() - 1; i < count; i++)
            {
                double tempRowmid = JR_RowMid[i].D + JRInterval;
                for (double tempRow = JR_RowDown[i].D + JRInterval; JR_RowDown[i + 1].D - tempRow > JRLengthY; tempRow += JRInterval)
                //for (double tempRow = tempRowmid + JRInterval; tempRowmid - tempRow > JRLengthY; tempRow += JRInterval)
                {

                    long idbegin, idend, idmid;
                    double rowbegin, rowend, rowmid;
                    NormalizaLocation(Image_Id, tempRowmid - JRLengthY / 2, ImageHeight, out idbegin, out rowbegin);
                    NormalizaLocation(Image_Id, tempRowmid + JRLengthY / 2, ImageHeight, out idend, out rowend);
                    NormalizaLocation(Image_Id, tempRowmid, ImageHeight, out idmid, out rowmid);
                    listJRparams_out.Add(CreatePoleEarsInfo(idbegin, rowbegin, idend, rowend, idmid, rowmid, ImageHeight, JRstartIndex + listJRparams_out.Count));
                    tempRowmid += JRInterval;
                }

                if (i < count - 1)
                {
                    long idbegin, idend, idmid;
                    double rowbegin, rowend, rowmid;
                    NormalizaLocation(Image_Id, JR_RowUP[i + 1].D, ImageHeight, out idbegin, out rowbegin);
                    NormalizaLocation(Image_Id, JR_RowDown[i + 1].D, ImageHeight, out idend, out rowend);
                    NormalizaLocation(Image_Id, (JR_RowUP[i + 1].D + JR_RowDown[i + 1].D) / 2, ImageHeight, out idmid, out rowmid);
                    listJRparams_out.Add(CreatePoleEarsInfo(idbegin, rowbegin, idend, rowend, idmid, rowmid, ImageHeight, JRstartIndex + listJRparams_out.Count));
                }
            }

            // 设置匹配的极耳数量
            JRcount_Matched = listJRparams_out.Count;
        }

        private PoleEarsInfo CreatePoleEarsInfo(long imageId0, double rowBegin, long imageId1, double rowEnd, long imageId2, double rowMid, double imageHeight, long id)
        {
            return new PoleEarsInfo
            {
                JR_ID = id,
                IsLeft = true,
                image_ID_Begin = imageId0,
                Row_Begin = rowBegin,
                image_ID_End = imageId1,
                Row_End = rowEnd,
                image_ID_Mid = imageId2,
                Row_Mid = rowMid
                //image_ID_Mid = imageId0 + (int)(((rowBegin + rowEnd) / 2) / imageHeight),
                //Row_Mid = ((rowBegin + rowEnd) / 2) % imageHeight
            };
        }


        private void NormalizaLocation(long LineID, double Line, double ImageHeight, out long standID, out double StandLine)
        {
            while (Line < 0)
            {
                Line += ImageHeight;
                LineID -= 1;
            }
            while (Line > ImageHeight)
            {
                Line -= ImageHeight;
                LineID += 1;
            }
            standID = LineID;
            StandLine = Line;
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
        public void JudgeWhiteRegion(HObject ho_Image, HTuple hv_ThresholdMin, HTuple hv_ThresholdMax, HTuple hv_RegionNum, HTuple hv_ColumnStart, HTuple hv_ColumnEnd, out HTuple hv_Result, out HTuple hv_GrayMean)
        {
            hv_Result = null;
            hv_GrayMean = null;
            HTuple hv_Width;
            HTuple hv_Height;
            HObject ho_Rectangle = null;
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HTuple hv_Mean;
            HTuple hv_Deviation;
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            for (int i = 0; i < hv_RegionNum; i++)
            {
                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Height / hv_RegionNum * i, hv_ColumnStart, hv_Height / hv_RegionNum * (i + 1) - 1, hv_ColumnEnd);
                HOperatorSet.Intensity(ho_Rectangle, ho_Image, out hv_Mean, out hv_Deviation);
                HOperatorSet.TupleConcat(hv_GrayMean, hv_Mean, out hv_GrayMean);

                if (hv_Mean >= hv_ThresholdMin && hv_Mean <= hv_ThresholdMax)
                    HOperatorSet.TupleConcat(hv_Result, 1, out hv_Result);
                else
                    HOperatorSet.TupleConcat(hv_Result, 0, out hv_Result);
            }
            ho_Rectangle.Dispose();
        }
    }
}
