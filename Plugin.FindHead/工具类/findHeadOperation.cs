using HalconAlgorithm;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;

namespace Plugin.FindHead
{

    class findHeadOperation
    {
        public FittingLine fittingLine = new FittingLine();
        public splicingAndCropOperation splicingAndCrop = new splicingAndCropOperation();
        /// <summary>
        /// 区域判断：判断图像中留白位置
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hv_Threshold">留白阈值</param>
        /// <param name="hv_RegionNum">ROI数量</param>
        /// <param name="hv_ColumnStart">起始列</param>
        /// <param name="hv_ColumnEnd">终止列</param>
        /// <param name="hv_Result">输出结果</param>
        public void JudgeWhiteRegion(HObject ho_Image, HTuple hv_Threshold, HTuple hv_RegionNum, HTuple hv_ColumnStart, HTuple hv_ColumnEnd, out HTuple hv_Result, out HTuple hv_GrayMean)
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
                if (hv_Mean >= hv_Threshold)
                    HOperatorSet.TupleConcat(hv_Result, 1, out hv_Result);
                else
                    HOperatorSet.TupleConcat(hv_Result, 0, out hv_Result);
            }
            ho_Rectangle.Dispose();
        }

        /// <summary>
        /// 找料头，从图像队列中读一张图片时调用此函数。
        /// </summary>
        /// <param name="resultHead">判断留白结果数组</param>
        /// <param name="picID">传入图像编号</param>
        /// <param name="himage">传入图像</param>
        /// <param name="imageHeight">原图像高度</param>
        /// <param name="hv_RegionNum">判断留白ROi数量</param>
        /// <param name="headLineID">输出：料头图片ID</param>
        /// <param name="headLine">料头行数</param>
        public void findHeadLineAndID(ParamsFindHead _Params, HTuple resultHead, out int headLineID, out double headLine)
        {
            headLine = -1;
            headLineID = -1;
            int a_flag = 0;   //初步判断有料头  //料头处在图像边缘，需联合图像抓边        
            HTuple hv_RoiPhi;
            HOperatorSet.TupleRad(_Params.hv_RoiAngle, out hv_RoiPhi);

            // int picID = _Params.list_ImageInfo.Last().image_ID; 
            int picID = _Params.ImageNowID;
            HObject himage = null;
            HOperatorSet.GenEmptyObj(out himage);
            himage = _Params.list_ImageInfo.Find(c => c.image_ID == _Params.ImageNowID).ho_image;
            double imageHeight = _Params.list_ImageInfo.Find(c => c.image_ID == _Params.ImageNowID).imageHeight;
            double cellHeight = imageHeight / _Params.hv_RegionNum;
            //先存图，可以开线程节省时间；或者等最后确认需要了再存，
            try
            {
                //图片中有白到黑
                for (int i = 0; i < resultHead.Length - 1; i++)
                {
                    a_flag = resultHead[i] - resultHead[i + 1];

                    if (a_flag == 1)  //有料头
                    {
                        _Params.Cache_imageID = -1;
                        //当前图像有料头，就直接抓边得到料头行坐标                                         
                        _Params.ho_ImageHead = splicingAndCrop.OutStitchingAndCropImage(himage, cellHeight * i, cellHeight * (i + 2), _Params.hv_ColumnStart, _Params.hv_ColumnEnd);

                        //料头预处理
                        ImagePretreatment(_Params);

                        //_Params.ho_ImageMeasureLine = _Params.ho_ImageHead;
                        _Params.hv_ImageHeadBeginID = picID;
                        _Params.hv_ImageHeadBeginRow = cellHeight * i;
                        _Params.hv_ImageHeadBeginCol = _Params.hv_ColumnStart;
                        _Params.hv_ImageHeadEndID = picID;
                        _Params.hv_ImageHeadEndRow = cellHeight * (i + 2);
                        _Params.hv_ImageHeadEndCol = _Params.hv_ColumnEnd;

                        // HOperatorSet.WriteImage(_Params.ho_ImageMeasureLine, "bmp", -1, @"D:\2.项目文档\HMX宽幅清洗机\极片凹槽测试\手动测试图片/head.bmp");
                        _Params.hv_RoiMidRow = cellHeight /** (i + 1)*/;
                        _Params.hv_RoiWidth = cellHeight * 2;
                        HTuple hv_MeasureRow1 = _Params.hv_RoiMidRow - _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                        HTuple hv_MeasureColumn1 = _Params.hv_RoiMidColumn - _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;
                        HTuple hv_MeasureRow2 = _Params.hv_RoiMidRow + _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                        HTuple hv_MeasureColumn2 = _Params.hv_RoiMidColumn + _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;

                        fittingLine.CreateLineModel(hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2,
                                     _Params.hv_RoiWidth / 2, _Params.hv_RoiHeight / _Params.hv_RoiPointsNum / 2,
                                     _Params.hv_MeasureSigma, _Params.hv_MeasureThreshold, out _Params.hv_MetrologyHandle,
                                     out _Params.hv_Index);
                        fittingLine.FindLine(_Params.ho_ImageMeasureLine, _Params.hv_MetrologyHandle, _Params.hv_Index, _Params.hv_MeasureNumInstances, _Params.hv_MeasureDistThreshold, _Params.hv_MeasureTransition, _Params.hv_MeasureSelect,
                                _Params.hv_RoiPointsNum, _Params.hv_MeasureMinScore, _Params.hv_MaxNumIterations, _Params.hv_MeasureInterpolation, _Params.hv_MeasureIORegions,
                                     out _Params.hv_CrossRow, out _Params.hv_CrossColumn, out _Params.hv_LineRowBegin, out _Params.hv_LineColumnBegin, out _Params.hv_LineRowEnd, out _Params.hv_LineColumnEnd);

                        if (_Params.hv_LineRowBegin != null && _Params.hv_LineRowBegin.Length > 0)
                        {
                            HTuple Temp = (_Params.hv_LineRowBegin + _Params.hv_LineRowEnd) / 2;
                            headLine = Temp.D + cellHeight * i;
                            headLineID = picID;
                        }
                        break;
                    }
                }
                if (_Params.Cache_imageID != -1 && resultHead[0] == 0) //上一张图尾部有留白，这张图第一个ROI就为料区，说明料头被图裁断。
                {
                    //当前图像没有料头，就先拼图，再抓边得到料头行坐标
                    _Params.ho_ImageHead = splicingAndCrop.OutStitchingAndCropImage(_Params.list_ImageInfo, picID - 1, imageHeight - cellHeight, picID, cellHeight, _Params.hv_ColumnStart, _Params.hv_ColumnEnd);
                    _Params.ho_ImageMeasureLine = _Params.ho_ImageHead;
                    _Params.hv_ImageHeadBeginID = picID - 1;
                    _Params.hv_ImageHeadBeginRow = imageHeight - cellHeight;
                    _Params.hv_ImageHeadBeginCol = _Params.hv_ColumnStart;
                    _Params.hv_ImageHeadEndID = picID;
                    _Params.hv_ImageHeadEndRow = cellHeight;
                    _Params.hv_ImageHeadEndCol = _Params.hv_ColumnEnd;

                    // HOperatorSet.WriteImage(_Params.ho_ImageMeasureLine, "bmp", -1, @"D:\2.项目文档\HMX宽幅清洗机\极片凹槽测试\手动测试图片/head_cut.bmp");
                    _Params.Cache_imageID = -1;

                    _Params.hv_RoiMidRow = cellHeight;
                    _Params.hv_RoiWidth = cellHeight * 2;
                    HTuple hv_MeasureRow1 = _Params.hv_RoiMidRow - _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                    HTuple hv_MeasureColumn1 = _Params.hv_RoiMidColumn - _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;
                    HTuple hv_MeasureRow2 = _Params.hv_RoiMidRow + _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                    HTuple hv_MeasureColumn2 = _Params.hv_RoiMidColumn + _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;

                    fittingLine.CreateLineModel(hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2,
                                 _Params.hv_RoiWidth / 2, _Params.hv_RoiHeight / _Params.hv_RoiPointsNum / 2,
                                 _Params.hv_MeasureSigma, _Params.hv_MeasureThreshold, out _Params.hv_MetrologyHandle,
                                 out _Params.hv_Index);
                    fittingLine.FindLine(_Params.ho_ImageMeasureLine, _Params.hv_MetrologyHandle, _Params.hv_Index, _Params.hv_MeasureNumInstances, _Params.hv_MeasureDistThreshold, _Params.hv_MeasureTransition, _Params.hv_MeasureSelect,
                            _Params.hv_RoiPointsNum, _Params.hv_MeasureMinScore, _Params.hv_MaxNumIterations, _Params.hv_MeasureInterpolation, _Params.hv_MeasureIORegions,
                                 out _Params.hv_CrossRow, out _Params.hv_CrossColumn, out _Params.hv_LineRowBegin, out _Params.hv_LineColumnBegin, out _Params.hv_LineRowEnd, out _Params.hv_LineColumnEnd);

                    if (_Params.hv_LineRowBegin != null && _Params.hv_LineRowBegin.Length > 0)
                    {
                        //料头行坐标,仅为拼接图的相对位置，需换算为原图的位置。
                        double headLineJoint = (_Params.hv_LineRowBegin.D + _Params.hv_LineRowEnd.D) / 2;
                        //判断料头所在图像ID和行数。
                        if (headLineJoint >= cellHeight)
                        {
                            headLine = headLineJoint - cellHeight;
                            headLineID = picID;
                        }
                        else
                        {
                            headLineID = picID - 1;
                            headLine = imageHeight - cellHeight + headLineJoint;
                        }
                    }
                 
                }
                else
                {
                    _Params.Cache_imageID = -1;
                }
                //本图中没有料头，但是有留白区，即保留当前图至抓边队列0
                if (resultHead[resultHead.Length - 1] == 1)
                {
                    _Params.Cache_imageID = picID;
                }
            }
            catch (Exception ee)
            {
                //Console.WriteLine("找料头失败");
            }
        }

        /// <summary>
        /// 找料尾，从图像队列中读一张图片时调用此函数。
        /// </summary>
        /// <param name="resultHead">判断留白结果数组</param>
        /// <param name="picID">传入图像编号</param>
        /// <param name="himage">传入图像</param>
        /// <param name="imageHeight">原图像高度</param>
        /// <param name="hv_RegionNum">判断留白ROi数量</param>
        /// <param name="trailLineID">输出：料尾图片ID</param>
        /// <param name="trailLine">料尾行数</param>
        public void findTrailLineAndID(ParamsFindHead _Params, HTuple resultHead, out int trailLineID, out double trailLine)
        {
            trailLine = -1;
            trailLineID = -1;
            int a_flag = 0;   //初步判断有料尾  //料尾处在图像边缘，需联合图像抓边        
            HTuple hv_RoiPhi;
            HOperatorSet.TupleRad(_Params.hv_RoiAngle, out hv_RoiPhi);

            // int picID = _Params.list_ImageInfo.Last().image_ID; 
            int picID = _Params.ImageNowID;
            HObject himage = null;
            HOperatorSet.GenEmptyObj(out himage);
            HObject trailRegion = null;
            HOperatorSet.GenEmptyObj(out trailRegion);
            himage = _Params.list_ImageInfo.Find(c => c.image_ID == _Params.ImageNowID).ho_image;
            double imageHeight = _Params.list_ImageInfo.Find(c => c.image_ID == _Params.ImageNowID).imageHeight;
            double cellHeight = imageHeight / _Params.hv_RegionNum;
            //先存图，可以开线程节省时间；或者等最后确认需要了再存，
            try
            {
                //图片中有白到黑
                for (int i = 0; i < resultHead.Length - 1; i++)
                {
                    a_flag = resultHead[i] - resultHead[i + 1];

                    if (a_flag == -1)  //有料尾
                    {
                        _Params.Cache_imageID = -1;
                        //当前图像有料尾，就直接抓边得到料尾行坐标                                         
                        _Params.ho_ImageHead = splicingAndCrop.OutStitchingAndCropImage(himage, cellHeight * i, cellHeight * (i + 2), _Params.hv_ColumnStart, _Params.hv_ColumnEnd);

                        //料尾预处理
                        ImagePretreatment(_Params);

                        //_Params.ho_ImageMeasureLine = _Params.ho_ImageHead;
                        _Params.hv_ImageHeadBeginID = picID;
                        _Params.hv_ImageHeadBeginRow = cellHeight * i;
                        _Params.hv_ImageHeadBeginCol = _Params.hv_ColumnStart;
                        _Params.hv_ImageHeadEndID = picID;
                        _Params.hv_ImageHeadEndRow = cellHeight * (i + 2);
                        _Params.hv_ImageHeadEndCol = _Params.hv_ColumnEnd;

                        // HOperatorSet.WriteImage(_Params.ho_ImageMeasureLine, "bmp", -1, @"D:\2.项目文档\HMX宽幅清洗机\极片凹槽测试\手动测试图片/head.bmp");
                        _Params.hv_RoiMidRow = cellHeight /** (i + 1)*/;
                        _Params.hv_RoiWidth = cellHeight * 2;
                        HTuple hv_MeasureRow1 = _Params.hv_RoiMidRow - _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                        HTuple hv_MeasureColumn1 = _Params.hv_RoiMidColumn - _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;
                        HTuple hv_MeasureRow2 = _Params.hv_RoiMidRow + _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                        HTuple hv_MeasureColumn2 = _Params.hv_RoiMidColumn + _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;

                        fittingLine.CreateLineModel(hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2,
                                     _Params.hv_RoiWidth / 2, _Params.hv_RoiHeight / _Params.hv_RoiPointsNum / 2,
                                     _Params.hv_MeasureSigma, _Params.hv_MeasureThreshold, out _Params.hv_MetrologyHandle,
                                     out _Params.hv_Index);
                        fittingLine.FindLine(_Params.ho_ImageMeasureLine, _Params.hv_MetrologyHandle, _Params.hv_Index, _Params.hv_MeasureNumInstances, _Params.hv_MeasureDistThreshold, _Params.hv_MeasureTransition, _Params.hv_MeasureSelect,
                                _Params.hv_RoiPointsNum, _Params.hv_MeasureMinScore, _Params.hv_MaxNumIterations, _Params.hv_MeasureInterpolation, _Params.hv_MeasureIORegions,
                                     out _Params.hv_CrossRow, out _Params.hv_CrossColumn, out _Params.hv_LineRowBegin, out _Params.hv_LineColumnBegin, out _Params.hv_LineRowEnd, out _Params.hv_LineColumnEnd);


                        if (_Params.hv_LineRowBegin != null && _Params.hv_LineRowBegin.Length > 0)
                        {
                            HTuple Temp = (_Params.hv_LineRowBegin.D + _Params.hv_LineRowEnd) / 2;
                            trailLine = Temp.D + cellHeight * i;
                            trailLineID = picID;
                        }
                        break;
                    }
                }
                if (_Params.Cache_imageID != -1 && resultHead[0] == 1) //上一张图尾部有留白，这张图第一个ROI就为料区，说明料尾被图裁断。
                {
                    //当前图像没有料尾，就先拼图，再抓边得到料尾行坐标
                    _Params.ho_ImageHead = splicingAndCrop.OutStitchingAndCropImage(_Params.list_ImageInfo, picID - 1, imageHeight - cellHeight, picID, cellHeight, _Params.hv_ColumnStart, _Params.hv_ColumnEnd);
                    _Params.ho_ImageMeasureLine = _Params.ho_ImageHead;
                    _Params.hv_ImageHeadBeginID = picID - 1;
                    _Params.hv_ImageHeadBeginRow = imageHeight - cellHeight;
                    _Params.hv_ImageHeadBeginCol = _Params.hv_ColumnStart;
                    _Params.hv_ImageHeadEndID = picID;
                    _Params.hv_ImageHeadEndRow = cellHeight;
                    _Params.hv_ImageHeadEndCol = _Params.hv_ColumnEnd;

                    // HOperatorSet.WriteImage(_Params.ho_ImageMeasureLine, "bmp", -1, @"D:\2.项目文档\HMX宽幅清洗机\极片凹槽测试\手动测试图片/head_cut.bmp");
                    _Params.Cache_imageID = -1;

                    _Params.hv_RoiMidRow = cellHeight;
                    _Params.hv_RoiWidth = cellHeight * 2;
                    HTuple hv_MeasureRow1 = _Params.hv_RoiMidRow - _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                    HTuple hv_MeasureColumn1 = _Params.hv_RoiMidColumn - _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;
                    HTuple hv_MeasureRow2 = _Params.hv_RoiMidRow + _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                    HTuple hv_MeasureColumn2 = _Params.hv_RoiMidColumn + _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;

                    fittingLine.CreateLineModel(hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2,
                                 _Params.hv_RoiWidth / 2, _Params.hv_RoiHeight / _Params.hv_RoiPointsNum / 2,
                                 _Params.hv_MeasureSigma, _Params.hv_MeasureThreshold, out _Params.hv_MetrologyHandle,
                                 out _Params.hv_Index);
                    fittingLine.FindLine(_Params.ho_ImageMeasureLine, _Params.hv_MetrologyHandle, _Params.hv_Index, _Params.hv_MeasureNumInstances, _Params.hv_MeasureDistThreshold, _Params.hv_MeasureTransition, _Params.hv_MeasureSelect,
                            _Params.hv_RoiPointsNum, _Params.hv_MeasureMinScore, _Params.hv_MaxNumIterations, _Params.hv_MeasureInterpolation, _Params.hv_MeasureIORegions,
                                 out _Params.hv_CrossRow, out _Params.hv_CrossColumn, out _Params.hv_LineRowBegin, out _Params.hv_LineColumnBegin, out _Params.hv_LineRowEnd, out _Params.hv_LineColumnEnd);

                    double headLineJoint = 0;
                    if (_Params.hv_LineRowBegin != null && _Params.hv_LineRowBegin.Length > 0)//判断是否找到边未找到边使用粗定位
                    {
                        //料尾行坐标,仅为拼接图的相对位置，需换算为原图的位置。
                        headLineJoint = (_Params.hv_LineRowBegin.D + _Params.hv_LineRowEnd.D) / 2;

                        //判断料尾所在图像ID和行数。
                        if (headLineJoint >= cellHeight)
                        {
                            trailLine = headLineJoint - cellHeight;
                            trailLineID = picID;
                        }
                        else
                        {
                            trailLineID = picID - 1;
                            trailLine = imageHeight - cellHeight + headLineJoint;
                        }
                    }
                    else
                    {
                        //HOperatorSet.Threshold(_Params.ho_ImageMeasureLine, out trailRegion, _Params.hv_RegionThreshold, 255);
                        //HTuple rowTrail1, colTrail1, rowTrail2, colTrail2;
                        //HOperatorSet.SmallestRectangle1(trailRegion, out rowTrail1, out colTrail1, out rowTrail2, out colTrail2);
                        //headLineJoint = rowTrail1.D;
                    }


                }
                else
                {
                    _Params.Cache_imageID = -1;
                }
                //本图中没有料尾，但是有留白区，即保留当前图至抓边队列0
                if (resultHead[resultHead.Length - 1] == 0)
                {
                    _Params.Cache_imageID = picID;
                }
            }
            catch (Exception ee)
            {
                //Console.WriteLine("找料尾失败");
            }
            finally
            {
                trailRegion?.Dispose();
            }
        }
        /// <summary>
        /// 找料头图像预处理
        /// </summary>
        public void ImagePretreatment(ParamsFindHead _Params)
        {
            try
            {
                if (_Params.EnStubBar)
                {
                    int Gmax = _Params.WhiteRegionGray;
                    int Gmin = _Params.CoatingRegionGary >= Gmax ? Gmax - 5 : _Params.CoatingRegionGary;
                    double mult = 255 / (Gmax - Gmin);
                    double add = -mult * Gmin;

                    HOperatorSet.ScaleImage(_Params.ho_ImageHead, out _Params.ho_ImageMeasureLine, mult, add);
                }
                else
                {
                    _Params.ho_ImageMeasureLine = _Params.ho_ImageHead;
                }
            }
            catch (Exception)
            {


            }
        }
        public void RefindHeadLine(ParamsFindHead _Params, out double headLine)
        {
            HTuple H1 = 110;
            HTuple W1 = 110;
            headLine = -1;
            HOperatorSet.GetImageSize(_Params.ho_ImageHead, out W1, out H1);
            if (W1.Length == 0) return;

            HTuple hv_RoiPhi;
            HOperatorSet.TupleRad(_Params.hv_RoiAngle, out hv_RoiPhi);
            _Params.hv_RoiMidRow = H1 / 2;
            _Params.hv_RoiWidth = W1 / 2;

            HTuple hv_MeasureRow1 = _Params.hv_RoiMidRow - _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
            HTuple hv_MeasureColumn1 = _Params.hv_RoiMidColumn - _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;
            HTuple hv_MeasureRow2 = _Params.hv_RoiMidRow + _Params.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
            HTuple hv_MeasureColumn2 = _Params.hv_RoiMidColumn + _Params.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;

            fittingLine.CreateLineModel(hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2,
                         _Params.hv_RoiWidth / 2, _Params.hv_RoiHeight / _Params.hv_RoiPointsNum / 2,
                         _Params.hv_MeasureSigma, _Params.hv_MeasureThreshold, out _Params.hv_MetrologyHandle,
                         out _Params.hv_Index);

            fittingLine.FindLine(_Params.ho_ImageMeasureLine, _Params.hv_MetrologyHandle, _Params.hv_Index, _Params.hv_MeasureNumInstances, _Params.hv_MeasureDistThreshold, _Params.hv_MeasureTransition, _Params.hv_MeasureSelect,
                    _Params.hv_RoiPointsNum, _Params.hv_MeasureMinScore, _Params.hv_MaxNumIterations, _Params.hv_MeasureInterpolation, _Params.hv_MeasureIORegions,
                         out _Params.hv_CrossRow, out _Params.hv_CrossColumn, out _Params.hv_LineRowBegin, out _Params.hv_LineColumnBegin, out _Params.hv_LineRowEnd, out _Params.hv_LineColumnEnd);
            HTuple Temp;
            if (_Params.hv_LineRowEnd.Length == 0) return;
            Temp = (_Params.hv_LineRowBegin + _Params.hv_LineRowEnd) / 2;
            headLine = Temp.D;

        }


    }
}
