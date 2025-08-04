using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Frame_Core;
using HalconDotNet;

namespace Plugin.EndBurr
{
    internal class EndBurrAlgorithm
    {
     

        public  void Entry1(HObject Image,double XCali, double YCali, double AreaCali, ParamsEndBurr parameter,
            out List<NgTypeInfo> NgInfo, out HObject OutFlagRegion, out HObject RegionDilation,
             out HTuple Count,out double TuCengHight,out HObject outputTuCengRegion)
        {
            TuCengHight = 0;
            
            HOperatorSet.GenEmptyRegion(out outputTuCengRegion);
            NgInfo = new List<NgTypeInfo>();
            HObject ho_Region, ho_ConnectedRegions, ho_RegionOpening;
            HObject ho_Skeleton, ho_Contours, ho_ContoursSplit, ho_SelectedXLD;
            HObject ho_ObjectSelected = null, ho_Line2, ho_RegionDifference;
            HObject ho_SelectedRegions, ho_outSelectedRegions;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple inputFitLineMinGray = new HTuple(), inputMinXldLength = new HTuple();
            HTuple hv_Number = new HTuple(), hv_R = new HTuple(), hv_C = new HTuple();
            HTuple hv_i = new HTuple(), hv_Row = new HTuple(), hv_Col = new HTuple();
            HTuple hv_RowBegin = new HTuple(), hv_ColBegin = new HTuple();
            HTuple hv_RowEnd = new HTuple(), hv_ColEnd = new HTuple();
            HTuple hv_Nr = new HTuple(), hv_Nc = new HTuple(), hv_Dist = new HTuple();
            HTuple hv_RowCenter = new HTuple(), hv_ColCenter = new HTuple();
            HTuple hv_Length = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_LineLengthleft = new HTuple(), hv_LineLengthRight = new HTuple();
            HTuple hv_Rad = new HTuple(), hv_RowStart = new HTuple();
            HTuple hv_ColStart = new HTuple(), inputHeight = new HTuple();
            HTuple inputMinArea = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out OutFlagRegion);
            HOperatorSet.GenEmptyObj(out RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_Skeleton);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_Line2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_outSelectedRegions);
            Count = 1;
            OutFlagRegion.Dispose();
            HOperatorSet.GenEmptyObj(out OutFlagRegion);



            try
            {

                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(Image, out hv_Width, out hv_Height);

                //median_image (Image, ImageMedian, 'circle', 2, 'mirrored')
                inputFitLineMinGray.Dispose();
                inputFitLineMinGray = parameter.inputFitLineMinGray;
                ho_Region.Dispose();
                HOperatorSet.Threshold(Image, out ho_Region, inputFitLineMinGray, 255);

                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                //开运算
                ho_RegionOpening.Dispose();
                HOperatorSet.OpeningRectangle1(ho_ConnectedRegions, out ho_RegionOpening, 10,
                    5);
                //提取骨架
                ho_Skeleton.Dispose();
                HOperatorSet.Skeleton(ho_RegionOpening, out ho_Skeleton);
                //骨架转轮廓
                ho_Contours.Dispose();
                HOperatorSet.GenContoursSkeletonXld(ho_Skeleton, out ho_Contours, 2, "filter");
                //提取线段轮廓
                ho_ContoursSplit.Dispose();
                HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines", 3,
                    4, 2);
                //按长度筛选
                inputMinXldLength.Dispose();
                inputMinXldLength = parameter.inputMinXldLength;
                ho_SelectedXLD.Dispose();
                HOperatorSet.SelectShapeXld(ho_Contours, out ho_SelectedXLD, "contlength", "and",
                    inputMinXldLength, 10E+9);
                hv_Number.Dispose();
                HOperatorSet.CountObj(ho_SelectedXLD, out hv_Number);

                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    //从区域生成XLD轮廓
                    OutFlagRegion.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionOpening, out OutFlagRegion, "border");
                    ho_Region.Dispose();
                    ho_ConnectedRegions.Dispose();
                    ho_RegionOpening.Dispose();
                    ho_Skeleton.Dispose();
                    ho_Contours.Dispose();
                    ho_ContoursSplit.Dispose();
                    ho_SelectedXLD.Dispose();
                    ho_ObjectSelected.Dispose();
                    ho_Line2.Dispose();
                    ho_RegionDifference.Dispose();
                    ho_SelectedRegions.Dispose();
                    ho_outSelectedRegions.Dispose();

                    hv_Width.Dispose();
                    hv_Height.Dispose();
                    inputFitLineMinGray.Dispose();
                    inputMinXldLength.Dispose();
                    hv_Number.Dispose();
                    hv_R.Dispose();
                    hv_C.Dispose();
                    hv_i.Dispose();
                    hv_Row.Dispose();
                    hv_Col.Dispose();
                    hv_RowBegin.Dispose();
                    hv_ColBegin.Dispose();
                    hv_RowEnd.Dispose();
                    hv_ColEnd.Dispose();
                    hv_Nr.Dispose();
                    hv_Nc.Dispose();
                    hv_Dist.Dispose();
                    hv_RowCenter.Dispose();
                    hv_ColCenter.Dispose();
                    hv_Length.Dispose();
                    hv_Phi.Dispose();
                    hv_LineLengthleft.Dispose();
                    hv_LineLengthRight.Dispose();
                    hv_Rad.Dispose();
                    hv_RowStart.Dispose();
                    hv_ColStart.Dispose();
                    inputHeight.Dispose();
                    inputMinArea.Dispose();

                    return;
                }
                else if ((int)(new HTuple(hv_Number.TupleGreater(1))) != 0)
                {
                    hv_R.Dispose();
                    hv_R = new HTuple();
                    hv_C.Dispose();
                    hv_C = new HTuple();
                    HTuple end_val35 = hv_Number;
                    HTuple step_val35 = 1;
                    for (hv_i = 1; hv_i.Continue(end_val35, step_val35); hv_i = hv_i.TupleAdd(step_val35))
                    {
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_SelectedXLD, out ho_ObjectSelected, hv_i);
                        hv_Row.Dispose(); hv_Col.Dispose();
                        HOperatorSet.GetContourXld(ho_ObjectSelected, out hv_Row, out hv_Col);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_R = hv_R.TupleConcat(
                                    hv_Row);
                                hv_R.Dispose();
                                hv_R = ExpTmpLocalVar_R;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_C = hv_C.TupleConcat(
                                    hv_Col);
                                hv_C.Dispose();
                                hv_C = ExpTmpLocalVar_C;
                            }
                        }
                    }
                    //点集生成XLD
                    ho_SelectedXLD.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_SelectedXLD, hv_R, hv_C);
                }
                //XLD拟合直线
                hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                HOperatorSet.FitLineContourXld(ho_SelectedXLD, "huber", -1, 50, 5, 1, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc, out hv_Dist);

                //gen_region_line (RegionLines, RowBegin, ColBegin, RowEnd, ColEnd)
                //计算一条线的重心、长度和方向。
                hv_RowCenter.Dispose(); hv_ColCenter.Dispose(); hv_Length.Dispose(); hv_Phi.Dispose();
                HOperatorSet.LinePosition(hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, out hv_RowCenter,
                    out hv_ColCenter, out hv_Length, out hv_Phi);

                //********************生成延长线***********************
                //延长线长度

                hv_LineLengthleft.Dispose();
                hv_LineLengthleft = new HTuple(hv_ColCenter);
                hv_LineLengthRight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_LineLengthRight = hv_Width - hv_ColCenter;
                }
                //角度转弧度
                hv_Rad.Dispose();
                HOperatorSet.TupleRad(90, out hv_Rad);
                //起点
                hv_RowStart.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_RowStart = hv_RowCenter - ((((hv_Phi + hv_Rad)).TupleCos()
                        ) * hv_ColCenter);
                }
                hv_ColStart.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ColStart = hv_ColCenter - ((((hv_Phi + hv_Rad)).TupleSin()
                        ) * hv_ColCenter);
                }
                //终点
                hv_RowEnd.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_RowEnd = hv_RowCenter - ((((hv_Phi - hv_Rad)).TupleCos()
                        ) * hv_LineLengthRight);
                }
                hv_ColEnd.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ColEnd = hv_ColCenter - ((((hv_Phi - hv_Rad)).TupleSin()
                        ) * hv_LineLengthRight);
                }
                //生成线
                ho_Line2.Dispose();
                HOperatorSet.GenRegionLine(out ho_Line2, hv_RowStart, hv_ColStart, hv_RowEnd,
                    hv_ColEnd);
                //也可以上下平移,生成检测区测
                //用矩形结构元素展开一个区域
                inputHeight.Dispose();
                inputHeight = parameter.inputHeight / YCali;
                RegionDilation.Dispose();
                HOperatorSet.DilationRectangle1(ho_Line2, out RegionDilation, 5, inputHeight);
                ho_Region.Dispose();
                //找缺陷最小阈值
                HTuple inputFitDefectMinGray = parameter.inputFitDefectMinGray;
                HOperatorSet.Threshold(Image, out ho_Region, inputFitDefectMinGray, 255);
                //减去检测区域
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Region, RegionDilation, out ho_RegionDifference
                    );
                //分开连通域
                ho_SelectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionDifference, out ho_SelectedRegions);
                //通过区域面积筛选
                inputMinArea.Dispose();
                inputMinArea = parameter.inputMinArea / AreaCali;
                //select_shape (ConnectedRegions1, SelectedRegions, 'area', 'and', 40, 99999)

                ho_outSelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_SelectedRegions, out ho_outSelectedRegions, "area",
                    "and", inputMinArea, 10E+9);

                //加入生成涂层区域 2024.5.22 毛刺分类
                ho_Region.Dispose();
                HOperatorSet.Threshold(Image, out ho_Region, parameter.inputTuCengMinGray, parameter.inputTuCengMaxGray);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                //涂层区域
                HObject TuCengRegions;
                HOperatorSet.GenEmptyObj(out TuCengRegions);
                ho_SelectedRegions.Dispose();

                HOperatorSet.ErosionRectangle1(ho_ConnectedRegions, out ho_ConnectedRegions, 3, 3);


                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out TuCengRegions, "max_area",
                    70);
                ;
                HOperatorSet.GetImageSize(Image, out hv_Width, out hv_Height);
                HTuple TuCeng_Row, TuCeng_Column, TuCeng_Phi, TuCeng_Length1, TuCeng_Length2;

                HOperatorSet.SmallestRectangle2(TuCengRegions, out TuCeng_Row, out TuCeng_Column,
                    out TuCeng_Phi, out TuCeng_Length1, out TuCeng_Length2);

                HTuple hv_TopLeft_Row, hv_TopLeft_Col, hv_TopRight_Row, hv_TopRight_Col, hv_LowerRight_Row, hv_LowerRight_Col, hv_LowerLeft_Row, hv_LowerLeft_Col;
                get_Rectangle_point(TuCengRegions, out hv_TopLeft_Row, out hv_TopLeft_Col,
                    out hv_TopRight_Row, out hv_TopRight_Col, out hv_LowerRight_Row, out hv_LowerRight_Col,
                    out hv_LowerLeft_Row, out hv_LowerLeft_Col);
                //涂层上Y
                double TuCeng_UP_Y = (hv_TopLeft_Row[0].D + hv_TopRight_Row[0].D) / 2;
                //涂层下Y
                double TuCeng_Lower_Y = (hv_LowerLeft_Row[0].D + hv_LowerRight_Row[0].D) / 2;

                //涂层高度
                TuCengHight = TuCeng_Lower_Y - TuCeng_UP_Y;

                //生成外接矩形
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {

                    HOperatorSet.GenRegionPolygon(out TuCengRegions, ((((hv_TopLeft_Row.TupleConcat(
                        hv_TopRight_Row))).TupleConcat(hv_LowerRight_Row))).TupleConcat(hv_LowerLeft_Row),
                        ((((hv_TopLeft_Col.TupleConcat(hv_TopRight_Col))).TupleConcat(hv_LowerRight_Col))).TupleConcat(
                        hv_LowerLeft_Col));
                }

                HTuple hv_Row1, hv_Column, hv_Length1, hv_Length2;
                HOperatorSet.SmallestRectangle2(TuCengRegions, out hv_Row1, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);

                outputTuCengRegion = TuCengRegions.CopyObj(1, 1);

                HOperatorSet.GenRectangle2(out TuCengRegions, hv_Row1, hv_Column, hv_Phi, hv_Length1,
                    hv_Length2);


                HObject ImageDomain;
                HOperatorSet.GetDomain(Image, out ImageDomain);
                HOperatorSet.Difference(ImageDomain, TuCengRegions, out TuCengRegions);
                //加入生成涂层区域 2024.5.22 毛刺分类

                HOperatorSet.CountObj(ho_outSelectedRegions, out Count);






                HObject ho_EmptyRegion;
                HOperatorSet.GenEmptyRegion(out ho_EmptyRegion);
                HObject ho_tempRegion;
                HOperatorSet.GenEmptyRegion(out ho_tempRegion);
                //与金属箔材的交集
                HObject ho_RegionIntersection1;
                HOperatorSet.GenEmptyRegion(out ho_RegionIntersection1);
                //与涂层的交集
                HObject ho_RegionIntersection2;
                HOperatorSet.GenEmptyRegion(out ho_RegionIntersection2);
                //把金属箔材区域放大，用于判断是否与箔材连接
                int BoCaiexpansionValue = parameter.BoCaiexpansionValue;


                HOperatorSet.DilationRectangle1(RegionDilation, out ho_EmptyRegion, 1, BoCaiexpansionValue);

                //获取金属箔材,上下左右四个顶点
                HTuple BoCai_TopLeft_Row, BoCai_TopLeft_Col, BoCai_TopRight_Row, BoCai_TopRight_Col, BoCai_LowerRight_Row, BoCai_LowerRight_Col, BoCai_LowerLeft_Row, BoCai_LowerLeft_Col;
                get_Rectangle_point(RegionDilation, out BoCai_TopLeft_Row, out BoCai_TopLeft_Col,
                    out BoCai_TopRight_Row, out BoCai_TopRight_Col, out BoCai_LowerRight_Row, out BoCai_LowerRight_Col,
                    out BoCai_LowerLeft_Row, out BoCai_LowerLeft_Col);


                //金属箔材 上Y
                double BoCai_UP_Y = (BoCai_TopLeft_Row[0].D + BoCai_TopRight_Row[0].D) / 2;
                //金属箔材 下Y
                double BoCai_Lower_Y = (BoCai_LowerLeft_Row[0].D + BoCai_LowerRight_Row[0].D) / 2;
                //截图
                HObject imagePart;
                HOperatorSet.GenEmptyObj(out imagePart);
                HTuple hv_j = 1;
                while ((int)(new HTuple(hv_j.TupleLessEqual(Count))) != 0)
                {
                    //遍历区域
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_outSelectedRegions, out ho_ObjectSelected, hv_j);
                    //计算与金属箔材区域有无交点,有就是B类毛刺
                    ho_RegionIntersection1.Dispose();
                    HOperatorSet.Intersection(ho_EmptyRegion, ho_ObjectSelected, out ho_RegionIntersection1
                        );
                    //
                    ho_RegionIntersection2.Dispose();
                    HOperatorSet.Intersection(TuCengRegions, ho_ObjectSelected, out ho_RegionIntersection2
                        );

                    //不等于  有交点


                    HTuple MinY, MinX, MaxY, MaxX;
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out MinY, out MinX, out MaxY, out MaxX);
                    HTuple area, row, column;
                    HOperatorSet.AreaCenter(ho_ObjectSelected, out area, out row, out column);


                    HOperatorSet.Circularity(ho_ObjectSelected, out HTuple Circularity);
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    double row11 = row1.D - 50 > 0 ? row1.D - 50 : row1.D;
                    double row22 = row2.D + 50 > parameter.ImageInfo.imageHeight ? row2.D : row2.D + 50;

                    double col11 = column1.D - 50 > 0 ? column1.D - 50 : column1.D;
                    double col22 = column2.D + 50 > parameter.ImageInfo.imageWidth ? column2.D : column2.D + 50;

                    if ((int)((new HTuple(ho_RegionIntersection2.TestEqualObj(ho_tempRegion)).TupleNot())) != 0)
                    {
                        HOperatorSet.CropRectangle1(Image, out imagePart, row11, col11, row22, col22);
                        hv_j++;
                        NgTypeInfo testNgInfo = new NgTypeInfo();
                        testNgInfo.m_hRegion = ho_ObjectSelected.CopyObj(1, 1);
                        testNgInfo.NgMidRow = row;
                        testNgInfo.NgMidColumn = column;
                        testNgInfo.m_fNgTypWidth = (MaxX.D - MinX.D) * XCali;
                        testNgInfo.m_fNgTypArea = area.D * AreaCali;
                        testNgInfo.m_hImage = imagePart;
                        testNgInfo.m_nImgIndex = parameter.ImageInfo.image_ID;
                        //毛刺超出下涂层
                        if (MaxY >= TuCeng_Lower_Y - 2)
                        {
                            testNgInfo.m_fNgTypHeight = MaxY.D - TuCeng_Lower_Y;
                            testNgInfo.m_strNgTypeName = Defct1Enum.端面A类毛刺;
                        }
                        //毛刺超出上涂层
                        else if (MinY <= TuCeng_UP_Y - 2)
                        {
                            testNgInfo.m_fNgTypHeight = TuCeng_UP_Y - MinY.D;
                            testNgInfo.m_strNgTypeName = Defct1Enum.端面A类毛刺;
                        }

                        else
                        {
                            double H1 = MaxY.D - TuCeng_Lower_Y;
                            double H2 = TuCeng_UP_Y - MinY.D;
                            if (MaxY.D >= TuCeng_Lower_Y - TuCeng_UP_Y)
                            {
                                testNgInfo.m_fNgTypHeight = H1;
                                testNgInfo.m_strNgTypeName = Defct1Enum.端面A类毛刺;
                            }
                            else
                            {
                                testNgInfo.m_fNgTypHeight = H2;
                                testNgInfo.m_strNgTypeName = Defct1Enum.端面A类毛刺;
                            }
                        }
                        testNgInfo.m_fNgTypHeight = testNgInfo.m_fNgTypHeight * YCali;
                        NgInfo.Add(testNgInfo);

                    }
                    else if ((int)((new HTuple(ho_RegionIntersection1.TestEqualObj(ho_tempRegion)).TupleNot())) != 0)
                    {
                        HOperatorSet.CropRectangle1(Image, out imagePart, row11, col11, row22, col22);
                        NgTypeInfo testNgInfo = new NgTypeInfo();

                        testNgInfo.NgMidRow = row;
                        testNgInfo.NgMidColumn = column;
                        testNgInfo.m_fNgTypWidth = (MaxX.D - MinX.D) * XCali;
                        testNgInfo.m_fNgTypArea = area.D * AreaCali;
                        testNgInfo.m_hImage = imagePart;
                        testNgInfo.m_nImgIndex = parameter.ImageInfo.image_ID;
                        //毛刺连接金属箔材 下层
                        if (MaxY >= BoCai_Lower_Y - 2)
                        {
                            testNgInfo.m_fNgTypHeight = MaxY.D - BoCai_UP_Y;
                            testNgInfo.m_strNgTypeName = Defct1Enum.端面B类毛刺;
                        }
                        //毛刺连接金属箔材 上层
                        else if (MinY <= BoCai_UP_Y - 2)
                        {
                            testNgInfo.m_fNgTypHeight = BoCai_Lower_Y - MinY.D;
                            testNgInfo.m_strNgTypeName = Defct1Enum.端面B类毛刺;
                        }
                        else
                        {
                            double H1 = MaxY.D - BoCai_UP_Y;
                            double H2 = BoCai_Lower_Y - MinY.D;
                            if (H1 > H2)
                            {
                                testNgInfo.m_fNgTypHeight = H1;
                                testNgInfo.m_strNgTypeName = Defct1Enum.端面B类毛刺;
                            }
                            else
                            {
                                testNgInfo.m_fNgTypHeight = H2;
                                testNgInfo.m_strNgTypeName = Defct1Enum.端面B类毛刺;
                            }
                        }


                        if (testNgInfo.m_fNgTypHeight > TuCengHight * parameter.MaoCi_H)
                        {
                            hv_j++;
                            testNgInfo.m_hRegion = ho_ObjectSelected.CopyObj(1, 1);
                            testNgInfo.m_fNgTypHeight = testNgInfo.m_fNgTypHeight * YCali;
                            NgInfo.Add(testNgInfo);
                        }
                        else
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.RemoveObj(ho_outSelectedRegions, out ExpTmpOutVar_0, hv_j);
                                ho_outSelectedRegions.Dispose();
                                ho_outSelectedRegions = ExpTmpOutVar_0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Number = Count - 1;
                                    Count.Dispose();
                                    Count = ExpTmpLocalVar_Number;
                                }
                            }
                        }
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.RemoveObj(ho_outSelectedRegions, out ExpTmpOutVar_0, hv_j);
                            ho_outSelectedRegions.Dispose();
                            ho_outSelectedRegions = ExpTmpOutVar_0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Number = Count - 1;
                                Count.Dispose();
                                Count = ExpTmpLocalVar_Number;
                            }
                        }
                    }

                }

                HOperatorSet.CountObj(ho_outSelectedRegions, out Count);
                HOperatorSet.Union1(ho_outSelectedRegions, out ho_outSelectedRegions);


                //RegionDilation = ho_EmptyRegion.CopyObj(1, 1);

                OutFlagRegion.Dispose();
                OutFlagRegion = new HObject(ho_outSelectedRegions);


                ho_EmptyRegion.Dispose();
                ho_tempRegion.Dispose();
                TuCengRegions.Dispose();
                ImageDomain.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_RegionIntersection2.Dispose();

            }
            catch(Exception ex)
            {

                NgTypeInfo testNgInfo = new NgTypeInfo();

                testNgInfo.NgMidRow = 100;
                testNgInfo.NgMidColumn = 100;
                testNgInfo.m_fNgTypWidth = 9999;
                testNgInfo.m_fNgTypArea = 9999;
                testNgInfo.m_hImage = Image;
                testNgInfo.m_nImgIndex = parameter.ImageInfo.image_ID;
             
                testNgInfo.m_fNgTypHeight = 9999;
                testNgInfo.m_strNgTypeName = Defct1Enum.端面A类毛刺;
                HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
                testNgInfo.m_hRegion = ho_ObjectSelected.CopyObj(1, 1);
                testNgInfo.m_fNgTypHeight =9999;
                NgInfo.Add(testNgInfo);


            }
            finally
            {

                Count.Dispose();

                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_RegionOpening.Dispose();
                ho_Skeleton.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_SelectedXLD.Dispose();
                ho_ObjectSelected.Dispose();
                ho_Line2.Dispose();
                ho_RegionDifference.Dispose();
                ho_SelectedRegions.Dispose();
                ho_outSelectedRegions.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                inputFitLineMinGray.Dispose();
                inputMinXldLength.Dispose();
                hv_Number.Dispose();
                hv_R.Dispose();
                hv_C.Dispose();
                hv_i.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_RowCenter.Dispose();
                hv_ColCenter.Dispose();
                hv_Length.Dispose();
                hv_Phi.Dispose();
                hv_LineLengthleft.Dispose();
                hv_LineLengthRight.Dispose();
                hv_Rad.Dispose();
                hv_RowStart.Dispose();
                hv_ColStart.Dispose();
                inputHeight.Dispose();
                inputMinArea.Dispose();


               
            }
           
        }



        public  void Find_Line_ForRectangle(HObject inputImage, HObject inputRegion,
     out HObject outputContour, HTuple inputXorY, HTuple inputInterval,
     HTuple inputDirection, HTuple inputRoiWidthLen, HTuple inputSigma,
     HTuple inputAmplitudeThreshold, HTuple inputTransition, HTuple inputPosition,
     out HTuple OutPointRow, out HTuple OutPointCol)
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ProcessContour, ho_Rectangle1 = null;
            HObject ho_Cross1 = null, ho_Cross3 = null, ho_Cross2 = null;
            HObject ho_ResultContour = null, ho_Cross = null;

            // Local control variables 

            HTuple hv_Row11 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row21 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_r2 = new HTuple(), hv_c2 = new HTuple(), hv_r4 = new HTuple();
            HTuple hv_c4 = new HTuple(), hv_Row1 = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Col2 = new HTuple();
            HTuple hv_LineRowStart_Measure_01_0 = new HTuple(), hv_LineColumnStart_Measure_01_0 = new HTuple();
            HTuple hv_LineRowEnd_Measure_01_0 = new HTuple(), hv_LineColumnEnd_Measure_01_0 = new HTuple();
            HTuple hv_MeasureLength1 = new HTuple(), hv_r1 = new HTuple();
            HTuple hv_c1 = new HTuple(), hv_r3 = new HTuple(), hv_c3 = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_MetrologyHandle = new HTuple(), hv_Index1 = new HTuple();
            HTuple hv_LineParameter = new HTuple(), hv_DebugMode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out outputContour);
            HOperatorSet.GenEmptyObj(out ho_ProcessContour);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Cross1);
            HOperatorSet.GenEmptyObj(out ho_Cross3);
            HOperatorSet.GenEmptyObj(out ho_Cross2);
            HOperatorSet.GenEmptyObj(out ho_ResultContour);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            OutPointRow = new HTuple();
            OutPointCol = new HTuple();
            //
            //inputXorY:沿Y轴搜索,或者X轴
            //inputInterval:间隔
            //inputDirection:方向 L2R_New从左到右,U2B_New从上到下
            //inputRoiWidthLen:卡尺ROI长度
            //inputSigma:平滑系数
            //inputAmplitudeThreshold:阈值
            //inputTransition:所以’negative‘从亮到暗，‘positive’从暗到亮，‘all’。
            //inputPosition:'first'第一点,'last'最后一个,'all' 所有的边缘
            //
            //
            OutPointRow.Dispose();
            OutPointRow = new HTuple();
            OutPointCol.Dispose();
            OutPointCol = new HTuple();
            ho_ProcessContour.Dispose();
            HOperatorSet.GenEmptyObj(out ho_ProcessContour);
            //
            //*****************************************二维卡尺方法**************************
            if ((int)((new HTuple((new HTuple((new HTuple(inputDirection.TupleEqual("L2R_New"))).TupleOr(
                new HTuple(inputDirection.TupleEqual("R2L_New"))))).TupleOr(new HTuple(inputDirection.TupleEqual(
                "U2B_New"))))).TupleOr(new HTuple(inputDirection.TupleEqual("B2U_New")))) != 0)
            {
                hv_Row11.Dispose(); hv_Column1.Dispose(); hv_Row21.Dispose(); hv_Column2.Dispose();
                HOperatorSet.SmallestRectangle1(inputRegion, out hv_Row11, out hv_Column1,
                    out hv_Row21, out hv_Column2);
                hv_Length1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Length1 = (((hv_Column2 - hv_Column1)).TupleAbs()
                        ) / 2;
                }
                hv_Length2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Length2 = (((hv_Row21 - hv_Row11)).TupleAbs()
                        ) / 2;
                }
                hv_Row.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row = (hv_Row21 + hv_Row11) / 2;
                }
                hv_Column.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Column = (hv_Column2 + hv_Column1) / 2;
                }
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, 0, hv_Length1,
                    hv_Length2);
                //an := rad(Phi)
                if ((int)(new HTuple(inputXorY.TupleEqual("Y"))) != 0)
                {
                    hv_r2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_r2 = hv_Row - hv_Length2;
                    }
                    hv_c2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_c2 = hv_Column - hv_Length1;
                    }
                    hv_r4.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_r4 = hv_Row + hv_Length2;
                    }
                    hv_c4.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_c4 = hv_Column + hv_Length1;
                    }
                    ho_Cross1.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_r2, hv_c2, 100, 0);
                    ho_Cross3.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_r4, hv_c4, 100, 0);
                    hv_Row1.Dispose();
                    hv_Row1 = new HTuple(hv_r2);
                    hv_Col1.Dispose();
                    hv_Col1 = new HTuple(hv_c2);
                    hv_Row2.Dispose();
                    hv_Row2 = new HTuple(hv_r4);
                    hv_Col2.Dispose();
                    hv_Col2 = new HTuple(hv_c4);
                    //
                    if ((int)(new HTuple(inputDirection.TupleEqual("L2R_New"))) != 0)
                    {
                        hv_LineRowStart_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineRowStart_Measure_01_0 = hv_Row2 + (inputRoiWidthLen / 2);
                        }
                        hv_LineColumnStart_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineColumnStart_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                        }
                        hv_LineRowEnd_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineRowEnd_Measure_01_0 = hv_Row1 - (inputRoiWidthLen / 2);
                        }
                        hv_LineColumnEnd_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineColumnEnd_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                        }
                    }
                    else if ((int)(new HTuple(inputDirection.TupleEqual("R2L_New"))) != 0)
                    {
                        hv_LineRowStart_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineRowStart_Measure_01_0 = hv_Row1 - (inputRoiWidthLen / 2);
                        }
                        hv_LineColumnStart_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineColumnStart_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                        }
                        hv_LineRowEnd_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineRowEnd_Measure_01_0 = hv_Row2 + (inputRoiWidthLen / 2);
                        }
                        hv_LineColumnEnd_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineColumnEnd_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                        }
                    }
                    hv_MeasureLength1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MeasureLength1 = ((hv_Col2 - hv_Col1) / 2) - 2;
                    }
                }
                else if ((int)(new HTuple(inputXorY.TupleEqual("X"))) != 0)
                {
                    hv_r1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_r1 = hv_Row - hv_Length2;
                    }
                    hv_c1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_c1 = hv_Column - hv_Length1;
                    }
                    ho_Cross2.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_r1, hv_c1, 100, 0);
                    hv_r3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_r3 = hv_Row + hv_Length2;
                    }
                    hv_c3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_c3 = hv_Column + hv_Length1;
                    }
                    ho_Cross1.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_r1, hv_c1, 100, 0);
                    ho_Cross3.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_r3, hv_c3, 100, 0);
                    //an := deg(Phi)
                    hv_Row1.Dispose();
                    hv_Row1 = new HTuple(hv_r1);
                    hv_Col1.Dispose();
                    hv_Col1 = new HTuple(hv_c1);
                    hv_Row2.Dispose();
                    hv_Row2 = new HTuple(hv_r3);
                    hv_Col2.Dispose();
                    hv_Col2 = new HTuple(hv_c3);
                    //*         endif
                    if ((int)(new HTuple(inputDirection.TupleEqual("U2B_New"))) != 0)
                    {
                        hv_LineRowStart_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineRowStart_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                        }
                        hv_LineColumnStart_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineColumnStart_Measure_01_0 = hv_Col1 - (inputRoiWidthLen / 2);
                        }
                        hv_LineRowEnd_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineRowEnd_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                        }
                        hv_LineColumnEnd_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineColumnEnd_Measure_01_0 = hv_Col2 + (inputRoiWidthLen / 2);
                        }
                    }
                    else if ((int)(new HTuple(inputDirection.TupleEqual("B2U_New"))) != 0)
                    {
                        hv_LineRowStart_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineRowStart_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                        }
                        hv_LineColumnStart_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineColumnStart_Measure_01_0 = hv_Col2 + (inputRoiWidthLen / 2);
                        }
                        hv_LineRowEnd_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineRowEnd_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                        }
                        hv_LineColumnEnd_Measure_01_0.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_LineColumnEnd_Measure_01_0 = hv_Col1 - (inputRoiWidthLen / 2);
                        }
                    }
                    hv_MeasureLength1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MeasureLength1 = (hv_Row2 - hv_Row1) / 2;
                    }
                }
                //
                //
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(inputImage, out hv_Width, out hv_Height);
                //Create the metrology model data structure
                hv_MetrologyHandle.Dispose();
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //The image size is set in advance to speed up the
                //first call of apply_metrology_model.
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                //Define the parameters of the metrology line objects
                //Create One metrology line objects and set parameters
                //gen_contour_polygon_xld (Contour1, [LineRowStart_Measure_01_0,LineRowEnd_Measure_01_0], [LineColumnStart_Measure_01_0, LineColumnEnd_Measure_01_0])
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Index1.Dispose();
                    HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_LineRowStart_Measure_01_0,
                        hv_LineColumnStart_Measure_01_0, hv_LineRowEnd_Measure_01_0, hv_LineColumnEnd_Measure_01_0,
                        hv_MeasureLength1, inputRoiWidthLen / 2, inputSigma, inputAmplitudeThreshold,
                        ((new HTuple("measure_distance")).TupleConcat("measure_transition")).TupleConcat(
                        "measure_select"), ((inputInterval.TupleConcat(inputTransition))).TupleConcat(
                        inputPosition), out hv_Index1);
                }
                //
                //set_metrology_object_param (MetrologyHandle, 'all', 'min_score', 0.3)
                //
                //Create region of interest for the alignment
                HOperatorSet.ApplyMetrologyModel(inputImage, hv_MetrologyHandle);
                hv_LineParameter.Dispose();
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_LineParameter);
                outputContour.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out outputContour, hv_MetrologyHandle,
                    "all", "all", 1.5);
                hv_DebugMode.Dispose();
                hv_DebugMode = 1;
                if ((int)(hv_DebugMode) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(outputContour, ho_ProcessContour, out ExpTmpOutVar_0
                            );
                        ho_ProcessContour.Dispose();
                        ho_ProcessContour = ExpTmpOutVar_0;
                    }
                    ho_ResultContour.Dispose(); OutPointRow.Dispose(); OutPointCol.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_ResultContour, hv_MetrologyHandle,
                        "all", "all", out OutPointRow, out OutPointCol);
                    //concat_obj (ResultContour, ProcessContour, ProcessContour)
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, OutPointRow, OutPointCol,
                            2, (new HTuple(45)).TupleRad());
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Cross, ho_ProcessContour, out ExpTmpOutVar_0);
                        ho_ProcessContour.Dispose();
                        ho_ProcessContour = ExpTmpOutVar_0;
                    }
                }
                //
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                ho_ProcessContour.Dispose();
                ho_Rectangle1.Dispose();
                ho_Cross1.Dispose();
                ho_Cross3.Dispose();
                ho_Cross2.Dispose();
                ho_ResultContour.Dispose();
                ho_Cross.Dispose();

                hv_Row11.Dispose();
                hv_Column1.Dispose();
                hv_Row21.Dispose();
                hv_Column2.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_r2.Dispose();
                hv_c2.Dispose();
                hv_r4.Dispose();
                hv_c4.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_LineRowStart_Measure_01_0.Dispose();
                hv_LineColumnStart_Measure_01_0.Dispose();
                hv_LineRowEnd_Measure_01_0.Dispose();
                hv_LineColumnEnd_Measure_01_0.Dispose();
                hv_MeasureLength1.Dispose();
                hv_r1.Dispose();
                hv_c1.Dispose();
                hv_r3.Dispose();
                hv_c3.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index1.Dispose();
                hv_LineParameter.Dispose();
                hv_DebugMode.Dispose();

                return;
            }
            ho_ProcessContour.Dispose();
            ho_Rectangle1.Dispose();
            ho_Cross1.Dispose();
            ho_Cross3.Dispose();
            ho_Cross2.Dispose();
            ho_ResultContour.Dispose();
            ho_Cross.Dispose();

            hv_Row11.Dispose();
            hv_Column1.Dispose();
            hv_Row21.Dispose();
            hv_Column2.Dispose();
            hv_Length1.Dispose();
            hv_Length2.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_r2.Dispose();
            hv_c2.Dispose();
            hv_r4.Dispose();
            hv_c4.Dispose();
            hv_Row1.Dispose();
            hv_Col1.Dispose();
            hv_Row2.Dispose();
            hv_Col2.Dispose();
            hv_LineRowStart_Measure_01_0.Dispose();
            hv_LineColumnStart_Measure_01_0.Dispose();
            hv_LineRowEnd_Measure_01_0.Dispose();
            hv_LineColumnEnd_Measure_01_0.Dispose();
            hv_MeasureLength1.Dispose();
            hv_r1.Dispose();
            hv_c1.Dispose();
            hv_r3.Dispose();
            hv_c3.Dispose();
            hv_Width.Dispose();
            hv_Height.Dispose();
            hv_MetrologyHandle.Dispose();
            hv_Index1.Dispose();
            hv_LineParameter.Dispose();
            hv_DebugMode.Dispose();

            return;
        }

        public  void genLengthenLine(out HObject inputContour, HTuple inputImageWidth,
            HTuple inputImageHeight, HTuple inputRowBegin, HTuple inputColBegin,
            HTuple inputRowEnd, HTuple inputColEnd, HTuple inputUpAndDown, out HTuple row1,
            out HTuple row2, out HTuple col1, out HTuple col2)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_k = new HTuple(), hv_b = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out inputContour);
            row1 = new HTuple();
            row2 = new HTuple();
            col1 = new HTuple();
            col2 = new HTuple();

            if ((int)(new HTuple(inputUpAndDown.TupleEqual("U"))) != 0)
            {
                //特殊情况水平线
                //y=kx+b
                if ((int)(new HTuple(((inputRowEnd - inputRowBegin)).TupleEqual(0))) != 0)
                {
                    hv_k.Dispose();
                    hv_k = 0;
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        inputContour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out inputContour, inputRowEnd.TupleConcat(
                            inputRowEnd), (new HTuple(0)).TupleConcat(inputImageWidth));
                    }

                    row1 = inputRowEnd;
                    row2 = inputRowEnd;
                    col1 = 0;
                    col2 = inputImageWidth;
                }
                else
                {
                    hv_k.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_k = (inputColEnd - inputColBegin) / (inputRowEnd - inputRowBegin);
                    }
                    hv_b.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_b = inputColEnd - (hv_k * inputRowEnd);
                    }
                    row1.Dispose();
                    row1 = 0;
                    row2.Dispose();
                    row2 = new HTuple(inputImageHeight);
                    col1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        col1 = (hv_k * row1) + hv_b;
                    }
                    col2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        col2 = (hv_k * row2) + hv_b;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        inputContour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out inputContour, row1.TupleConcat(
                            row2), col1.TupleConcat(col2));
                    }
                }

            }
            else
            {
                //特殊情况水平线
                //y=kx+b
                if ((int)(new HTuple(((inputRowEnd - inputRowBegin)).TupleEqual(0))) != 0)
                {
                    hv_k.Dispose();
                    hv_k = 0;
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        inputContour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out inputContour, inputRowEnd.TupleConcat(
                            inputRowEnd), (new HTuple(0)).TupleConcat(inputImageWidth));
                    }


                    row1 = inputRowEnd;
                    row2 = inputRowEnd;
                    col1 = 0;
                    col2 = inputImageWidth;

                }
                else
                {
                    hv_k.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_k = (inputColEnd - inputColBegin) / (inputRowEnd - inputRowBegin);
                    }
                    hv_b.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_b = inputColEnd - (hv_k * inputRowEnd);
                    }
                    col1.Dispose();
                    col1 = 0;
                    col2.Dispose();
                    col2 = new HTuple(inputImageWidth);

                    row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        row1 = (col1 - hv_b) / hv_k;
                    }
                    row2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        row2 = (col2 - hv_b) / hv_k;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        inputContour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out inputContour, row1.TupleConcat(
                            row2), col1.TupleConcat(col2));
                    }
                }



            }


            hv_k.Dispose();
            hv_b.Dispose();

            return;
        }

      


        /// <summary>
        /// 获取矩形四个顶点
        /// </summary>
        /// <param name="ho_inputRectangle"></param>
        /// <param name="hv_TopLeft_Row"></param>
        /// <param name="hv_TopLeft_Col"></param>
        /// <param name="hv_TopRight_Row"></param>
        /// <param name="hv_TopRight_Col"></param>
        /// <param name="hv_LowerRight_Row"></param>
        /// <param name="hv_LowerRight_Col"></param>
        /// <param name="hv_LowerLeft_Row"></param>
        /// <param name="hv_LowerLeft_Col"></param>
        public  void get_Rectangle_point(HObject ho_inputRectangle, out HTuple hv_TopLeft_Row,
      out HTuple hv_TopLeft_Col, out HTuple hv_TopRight_Row, out HTuple hv_TopRight_Col,
      out HTuple hv_LowerRight_Row, out HTuple hv_LowerRight_Col, out HTuple hv_LowerLeft_Row,
      out HTuple hv_LowerLeft_Col)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Tmp = new HTuple();
            HTuple hv_Sin = new HTuple(), hv_Cos = new HTuple(), hv_TopLeft_X = new HTuple();
            HTuple hv_TopLeft_Y = new HTuple(), hv_TopRight_X = new HTuple();
            HTuple hv_TopRight_Y = new HTuple(), hv_LowerRight_X = new HTuple();
            HTuple hv_LowerRight_Y = new HTuple(), hv_LowerLeft_X = new HTuple();
            HTuple hv_LowerLeft_Y = new HTuple();
            // Initialize local and output iconic variables 
            hv_TopLeft_Row = new HTuple();
            hv_TopLeft_Col = new HTuple();
            hv_TopRight_Row = new HTuple();
            hv_TopRight_Col = new HTuple();
            hv_LowerRight_Row = new HTuple();
            hv_LowerRight_Col = new HTuple();
            hv_LowerLeft_Row = new HTuple();
            hv_LowerLeft_Col = new HTuple();
            //计算矩形的中间点、角度、及长宽（一半）
            hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
            HOperatorSet.SmallestRectangle2(ho_inputRectangle, out hv_Row, out hv_Column,
                out hv_Phi, out hv_Length1, out hv_Length2);

            if ((int)(new HTuple(((((hv_Phi.TupleDeg())).TupleAbs())).TupleGreater(45))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Phi = (((hv_Phi.TupleDeg()
                            ) - (90 * (hv_Phi / (hv_Phi.TupleAbs()))))).TupleRad();
                        hv_Phi.Dispose();
                        hv_Phi = ExpTmpLocalVar_Phi;
                    }
                }
                hv_Tmp.Dispose();
                hv_Tmp = new HTuple(hv_Length1);
                hv_Length1.Dispose();
                hv_Length1 = new HTuple(hv_Length2);
                hv_Length2.Dispose();
                hv_Length2 = new HTuple(hv_Tmp);
            }

            hv_Sin.Dispose();
            HOperatorSet.TupleSin(hv_Phi, out hv_Sin);
            hv_Cos.Dispose();
            HOperatorSet.TupleCos(hv_Phi, out hv_Cos);

            //左上角
            hv_TopLeft_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopLeft_X = ((-hv_Length1) * hv_Cos) - (hv_Length2 * hv_Sin);
            }
            hv_TopLeft_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopLeft_Y = ((-hv_Length1) * hv_Sin) + (hv_Length2 * hv_Cos);
            }
            hv_TopLeft_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopLeft_Row = hv_Row - hv_TopLeft_Y;
            }
            hv_TopLeft_Col.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopLeft_Col = hv_Column + hv_TopLeft_X;
            }

            //右上角
            hv_TopRight_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopRight_X = (hv_Length1 * hv_Cos) - (hv_Length2 * hv_Sin);
            }
            hv_TopRight_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopRight_Y = (hv_Length1 * hv_Sin) + (hv_Length2 * hv_Cos);
            }
            hv_TopRight_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopRight_Row = hv_Row - hv_TopRight_Y;
            }
            hv_TopRight_Col.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopRight_Col = hv_Column + hv_TopRight_X;
            }

            //右下角
            hv_LowerRight_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerRight_X = (hv_Length1 * hv_Cos) + (hv_Length2 * hv_Sin);
            }
            hv_LowerRight_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerRight_Y = (hv_Length1 * hv_Sin) - (hv_Length2 * hv_Cos);
            }
            hv_LowerRight_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerRight_Row = hv_Row - hv_LowerRight_Y;
            }
            hv_LowerRight_Col.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerRight_Col = hv_Column + hv_LowerRight_X;
            }

            //左下角
            hv_LowerLeft_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerLeft_X = ((-hv_Length1) * hv_Cos) + (hv_Length2 * hv_Sin);
            }
            hv_LowerLeft_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerLeft_Y = ((-hv_Length1) * hv_Sin) - (hv_Length2 * hv_Cos);
            }
            hv_LowerLeft_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerLeft_Row = hv_Row - hv_LowerLeft_Y;
            }
            hv_LowerLeft_Col.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerLeft_Col = hv_Column + hv_LowerLeft_X;
            }

            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Phi.Dispose();
            hv_Length1.Dispose();
            hv_Length2.Dispose();
            hv_Tmp.Dispose();
            hv_Sin.Dispose();
            hv_Cos.Dispose();
            hv_TopLeft_X.Dispose();
            hv_TopLeft_Y.Dispose();
            hv_TopRight_X.Dispose();
            hv_TopRight_Y.Dispose();
            hv_LowerRight_X.Dispose();
            hv_LowerRight_Y.Dispose();
            hv_LowerLeft_X.Dispose();
            hv_LowerLeft_Y.Dispose();

            return;





        }



    }
}
