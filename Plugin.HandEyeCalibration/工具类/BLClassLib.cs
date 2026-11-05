using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace BatteryLugClassLibrary
{
    public class BLClassLib
    {
        ////////////////////////////////极耳余料检测//////////////////////////////////////////////
        //   /// <summary>
        //    /// 头部缺陷检测
        //    /// </summary>
        //    /// <param name="ho_Image"></输入图片>
        //    /// <param name="hv_HminArea"></param>
        //    /// <param name="hv_HminWidth"></param>
        //    /// <param name="hv_HminHeigth"></param>
        //    /// <param name="hv_RegionWidth"></输入极耳得宽度>
        //    /// <param name="ho_FlawRegion"></param>
        //    public void EntryFlaw(HObject ho_Image, HObject ho_RectangImage, out HObject ho_FlawRegion,
        //HTuple hv_RegionWidth, HTuple hv_RegionHeight, HTuple hv_HminArea, HTuple hv_HminWidth,
        //HTuple hv_HminHeigth)
        //    {

        //        // Local iconic variables 

        //        HObject ho_GapRegion, ho_ImageMedian, ho_HoleRegion;
        //        HObject ho_Region, ho_RegionFillUp, ho_Border, ho_SelectedXLD;
        //        HObject ho_ContoursSplit, ho_UDRegion, ho_ObjectSelected1;
        //        HObject ho_ObjectSelected2, ho_Skeleton, ho_EndPoints, ho_JuncPoints;
        //        HObject ho_Rectangle, ho_ImageReduced, ho_RegionClosing = null;
        //        HObject ho_Contours, ho_ContoursDifference, ho_Region3;
        //        HObject ho_SelectedRegions, ho_RegionUnion1, ho_Rectangle3;
        //        HObject ho_RegionDilation, ho_LTypeRegion, ho_Region4, ho_Rectangle4;
        //        HObject ho_RegionOpening3, ho_ObjectSelected0 = null, ho_RegionDilation0 = null;
        //        HObject ho_RegionDifference6 = null, ho_ImageReduced0 = null;
        //        HObject ho_Region5 = null, ho_ConnectedRegions = null, ho_ObjectSelected3 = null;
        //        HObject ho_Rectangle1, ho_Rectangle2, ho_RegionUnion, ho_UDRRegion;
        //        HObject ho_VerticalRegion, ho_ImageMean = null, ho_Region1 = null;
        //        HObject ho_RegionDifference = null, ho_RegionDilation1 = null;
        //        HObject ho_RegionIntersection;

        //        // Local control variables 

        //        HTuple hv_Number = new HTuple(), hv_Attrib1 = new HTuple();
        //        HTuple hv_Attrib2 = new HTuple(), hv_Length1 = new HTuple();
        //        HTuple hv_Length2 = new HTuple(), hv_Attrib = new HTuple();
        //        HTuple hv_Rows = new HTuple(), hv_Columns = new HTuple();
        //        HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
        //        HTuple hv_Row21 = new HTuple(), hv_Column21 = new HTuple();
        //        HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
        //        HTuple hv_Phi2 = new HTuple(), hv_Length12 = new HTuple();
        //        HTuple hv_Length22 = new HTuple(), hv_temNumber = new HTuple();
        //        HTuple hv_i = new HTuple(), hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
        //        HTuple hv_Phi1 = new HTuple(), hv_Length11 = new HTuple();
        //        HTuple hv_Length21 = new HTuple(), hv_Value0 = new HTuple();
        //        HTuple hv_Value1 = new HTuple(), hv_flg = new HTuple();
        //        HTuple hv_Value4 = new HTuple(), hv_Row12 = new HTuple();
        //        HTuple hv_Column12 = new HTuple(), hv_Row23 = new HTuple();
        //        HTuple hv_Column23 = new HTuple(), hv_RowsVertical = new HTuple();
        //        HTuple hv_Rect1 = new HTuple(), hv_Rect2 = new HTuple();
        //        HTuple hv_Rows1 = new HTuple(), hv_Columns1 = new HTuple();
        //        HTuple hv_Rows2 = new HTuple(), hv_Columns2 = new HTuple();
        //        HTuple hv_IsNr = new HTuple(), hv_Nr1 = new HTuple(), hv_Nr2 = new HTuple();
        //        HTuple hv_RowBegin = new HTuple(), hv_ColBegin = new HTuple();
        //        HTuple hv_RowEnd = new HTuple(), hv_ColEnd = new HTuple();
        //        HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Attrib3 = new HTuple();
        //        HTuple hv_Length3 = new HTuple(), hv_Rows3 = new HTuple();
        //        HTuple hv_Columns3 = new HTuple(), hv_Row11 = new HTuple();
        //        HTuple hv_Column11 = new HTuple(), hv_Row22 = new HTuple();
        //        HTuple hv_Column22 = new HTuple(), hv_Row = new HTuple();
        //        HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
        //        HTuple hv_Length13 = new HTuple(), hv_Length23 = new HTuple();
        //        // Initialize local and output iconic variables 

        //        HOperatorSet.GenEmptyObj(out ho_ImageMedian);
        //        HOperatorSet.GenEmptyObj(out ho_HoleRegion);
        //        HOperatorSet.GenEmptyObj(out ho_Region);
        //        HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
        //        HOperatorSet.GenEmptyObj(out ho_Border);
        //        HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
        //        HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
        //        HOperatorSet.GenEmptyObj(out ho_UDRegion);
        //        HOperatorSet.GenEmptyObj(out ho_ObjectSelected1);
        //        HOperatorSet.GenEmptyObj(out ho_ObjectSelected2);
        //        HOperatorSet.GenEmptyObj(out ho_Skeleton);
        //        HOperatorSet.GenEmptyObj(out ho_EndPoints);
        //        HOperatorSet.GenEmptyObj(out ho_JuncPoints);
        //        HOperatorSet.GenEmptyObj(out ho_Rectangle);
        //        HOperatorSet.GenEmptyObj(out ho_ImageReduced);
        //        HOperatorSet.GenEmptyObj(out ho_RegionClosing);
        //        HOperatorSet.GenEmptyObj(out ho_Contours);
        //        HOperatorSet.GenEmptyObj(out ho_ContoursDifference);
        //        HOperatorSet.GenEmptyObj(out ho_Region3);
        //        HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
        //        HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
        //        HOperatorSet.GenEmptyObj(out ho_Rectangle3);
        //        HOperatorSet.GenEmptyObj(out ho_RegionDilation);
        //        HOperatorSet.GenEmptyObj(out ho_LTypeRegion);
        //        HOperatorSet.GenEmptyObj(out ho_Region4);
        //        HOperatorSet.GenEmptyObj(out ho_Rectangle4);
        //        HOperatorSet.GenEmptyObj(out ho_RegionOpening3);
        //        HOperatorSet.GenEmptyObj(out ho_ObjectSelected0);
        //        HOperatorSet.GenEmptyObj(out ho_RegionDilation0);
        //        HOperatorSet.GenEmptyObj(out ho_RegionDifference6);
        //        HOperatorSet.GenEmptyObj(out ho_ImageReduced0);
        //        HOperatorSet.GenEmptyObj(out ho_Region5);
        //        HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
        //        HOperatorSet.GenEmptyObj(out ho_ObjectSelected3);
        //        HOperatorSet.GenEmptyObj(out ho_Rectangle1);
        //        HOperatorSet.GenEmptyObj(out ho_Rectangle2);
        //        HOperatorSet.GenEmptyObj(out ho_RegionUnion);
        //        HOperatorSet.GenEmptyObj(out ho_UDRRegion);
        //        HOperatorSet.GenEmptyObj(out ho_VerticalRegion);
        //        HOperatorSet.GenEmptyObj(out ho_ImageMean);
        //        HOperatorSet.GenEmptyObj(out ho_Region1);
        //        HOperatorSet.GenEmptyObj(out ho_RegionDifference);
        //        HOperatorSet.GenEmptyObj(out ho_RegionDilation1);
        //        HOperatorSet.GenEmptyObj(out ho_RegionIntersection);


        //        HOperatorSet.GenEmptyRegion(out ho_FlawRegion);
        //        HOperatorSet.GenEmptyRegion(out ho_GapRegion);
        //        HOperatorSet.MedianImage(ho_Image, out ho_ImageMedian, "circle", 7, "cyclic");
        //        //**************计算是否有小孔缺陷***********************

        //        HOperatorSet.GenEmptyRegion(out ho_HoleRegion);
        //        HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, 128, 255);
        //        HOperatorSet.Connection(ho_Region, out ho_Region);
        //        HOperatorSet.SelectShapeStd(ho_Region, out ho_Region, "max_area", 70);


        //        HOperatorSet.FillUp(ho_Region, out ho_RegionFillUp);
        //        HOperatorSet.Difference(ho_RegionFillUp, ho_Region, out ho_RegionFillUp);
        //        HOperatorSet.Connection(ho_RegionFillUp, out ho_RegionFillUp);
        //        HOperatorSet.SelectShape(ho_RegionFillUp, out ho_HoleRegion, "area", "and", 20, 99999);


        //        //****************计算轮廓信息*******************

        //        HOperatorSet.ThresholdSubPix(ho_ImageMedian, out ho_Border, 120);
        //        HOperatorSet.SelectShapeXld(ho_Border, out ho_SelectedXLD, "contlength", "and", 50, 99999);
        //        HOperatorSet.SegmentContoursXld(ho_SelectedXLD, out ho_ContoursSplit, "lines_circles", 3, 5, 4);
        //        HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_ContoursSplit, "contlength", "and", 20, 99999);
        //        HOperatorSet.SortContoursXld(ho_ContoursSplit, out ho_ContoursSplit, "upper_left", "true", "row");
        //        HOperatorSet.CountObj(ho_ContoursSplit, out hv_Number);


        //        //********************计算上下直线边缘缺陷***************************

        //        HOperatorSet.GenEmptyRegion(out ho_UDRegion);
        //        HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected1, 1);
        //        HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected2, hv_Number);
        //        HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected1, "cont_approx", out hv_Attrib1);
        //        HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected2, "cont_approx", out hv_Attrib2);


        //        HOperatorSet.LengthXld(ho_ObjectSelected1, out hv_Length1);
        //        HOperatorSet.LengthXld(ho_ObjectSelected2, out hv_Length2);
        //        hv_Attrib = 100;
        //        if (hv_Length1 > hv_Length2)
        //        {
        //            HOperatorSet.GenRegionContourXld(ho_ObjectSelected1, out ho_Region, "filled");
        //            hv_Attrib = new HTuple(hv_Attrib1);
        //        }
        //        else
        //        {
        //            HOperatorSet.GenRegionContourXld(ho_ObjectSelected2, out ho_Region, "filled");
        //            hv_Attrib = new HTuple(hv_Attrib2);
        //        }

        //        HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
        //        HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
        //        HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows, out hv_Columns);
        //        HOperatorSet.TupleSort(hv_Columns, out hv_Columns);
        //        HOperatorSet.TupleSort(hv_Rows, out hv_Rows);
        //        HOperatorSet.GenRectangle1(out ho_Rectangle, 0, hv_Columns.TupleSelect(0), hv_RegionHeight, hv_Columns.TupleSelect(1));
        //        HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
        //        HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 180, 255);
        //        if (hv_Attrib < 0)
        //        {
        //            HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing, 15, 25);
        //        }
        //        else
        //        {
        //            HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, 25);
        //        }

        //        HOperatorSet.Difference(ho_RegionClosing, ho_Region, out ho_UDRegion);
        //        HOperatorSet.GenContourRegionXld(ho_Rectangle, out ho_Contours, "border");
        //        HOperatorSet.DifferenceClosedContoursXld(ho_SelectedXLD, ho_Contours, out ho_ContoursDifference);
        //        HOperatorSet.GenRegionContourXld(ho_ContoursDifference, out ho_Region3, "filled");
        //        HOperatorSet.SelectShape(ho_Region3, out ho_SelectedRegions, "height", "and", 80, 99999);
        //        HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion1);
        //        HOperatorSet.SmallestRectangle1(ho_RegionUnion1, out hv_Row1, out hv_Column1, out hv_Row21, out hv_Column21);
        //        HOperatorSet.GenRectangle1(out ho_Rectangle3, hv_Row1, hv_Column1, hv_Row21, hv_Column21);
        //        HOperatorSet.DilationRectangle1(ho_Rectangle3, out ho_RegionDilation, 100, 5);

        //        //************判断是否又L型的口**************
        //        HOperatorSet.GenEmptyRegion(out ho_LTypeRegion);
        //        HOperatorSet.ReduceDomain(ho_ImageMedian, ho_RegionDilation, out ho_ImageReduced);
        //        HOperatorSet.Threshold(ho_ImageReduced, out ho_Region4, 0, 125);
        //        HOperatorSet.Connection(ho_Region4, out ho_Region4);
        //        HOperatorSet.SelectShapeStd(ho_Region4, out ho_Region4, "max_area", 70);

        //        HOperatorSet.SmallestRectangle2(ho_Region4, out hv_Row3, out hv_Column3, out hv_Phi2, out hv_Length12, out hv_Length22);
        //        HOperatorSet.GenRectangle2(out ho_Rectangle4, hv_Row3, hv_Column3, hv_Phi2, hv_Length12, hv_Length22);
        //        HOperatorSet.Difference(ho_Rectangle4, ho_Region4, out ho_RegionOpening3);
        //        HOperatorSet.OpeningRectangle1(ho_RegionOpening3, out ho_RegionOpening3, 20, 10);
        //        HOperatorSet.Connection(ho_RegionOpening3, out ho_RegionOpening3);

        //        HOperatorSet.SelectShape(ho_RegionOpening3, out ho_RegionOpening3, "area", "and", 1000, 99999);
        //        HOperatorSet.CountObj(ho_RegionOpening3, out hv_temNumber);

        //        for (hv_i = 1; hv_i <= hv_temNumber; ++hv_i)
        //        {
        //            HOperatorSet.SelectObj(ho_RegionOpening3, out ho_ObjectSelected0, hv_i);
        //            HOperatorSet.SmallestRectangle2(ho_ObjectSelected0, out hv_Row2, out hv_Column2, out hv_Phi1, out hv_Length11, out hv_Length21);
        //            if (hv_Length11 > 130 || hv_Length21 > 130)
        //            {
        //                continue;
        //            }

        //            HOperatorSet.DilationRectangle1(ho_ObjectSelected0, out ho_RegionDilation0, 5, 5);
        //            HOperatorSet.Difference(ho_RegionDilation0, ho_ObjectSelected0, out ho_RegionDifference6);
        //            HOperatorSet.ReduceDomain(ho_Image, ho_RegionDifference6, out ho_ImageReduced0);
        //            HOperatorSet.Threshold(ho_ImageReduced0, out ho_Region5, 0, 128);
        //            HOperatorSet.RegionFeatures(ho_Region5, "area", out hv_Value0);
        //            HOperatorSet.RegionFeatures(ho_RegionDifference6, "area", out hv_Value1);
        //            hv_flg = hv_Value0 / hv_Value1;

        //            if (hv_flg <= 0.70 && hv_flg >= 0.44)
        //            {

        //                HOperatorSet.DilationRectangle1(ho_RegionDilation0, out ho_RegionDilation0, 15, 30);
        //                HOperatorSet.Union2(ho_RegionDilation0, ho_LTypeRegion, out ho_LTypeRegion);
        //            }
        //        }



        //        //***********************计算上下拐角处的缺陷******************
        //        HOperatorSet.RegionFeatures(ho_Rectangle3, "width", out hv_Value4);
        //        if (hv_Value4 > 10)
        //        {
        //            HOperatorSet.DilationRectangle1(ho_Rectangle3, out ho_RegionDilation, 3, 100);
        //        }
        //        else
        //        {

        //            HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
        //            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_ConnectedRegions, "area", "and", 1000, 99999);
        //            HOperatorSet.SmallestRectangle1(ho_ConnectedRegions, out hv_Row12, out hv_Column12, out hv_Row23, out hv_Column23);
        //            HOperatorSet.TupleSort(hv_Row12, out hv_Row12);
        //            HOperatorSet.TupleSort(hv_Row23, out hv_Row23);
        //            HOperatorSet.GenRectangle1(out ho_RegionDilation, (hv_Row23.TupleSelect(0)) + 3, 0, (hv_Row12.TupleSelect(1)) - 3, hv_Columns.TupleSelect(1));

        //        }


        //        HOperatorSet.ReduceDomain(ho_Image, ho_RegionDilation, out ho_ImageReduced);
        //        HOperatorSet.ThresholdSubPix(ho_ImageReduced, out ho_Border, 150);
        //        HOperatorSet.SelectShapeXld(ho_Border, out ho_SelectedXLD, "contlength", "and", 50, 99999);
        //        HOperatorSet.SegmentContoursXld(ho_SelectedXLD, out ho_ContoursSplit, "lines_circles", 1, 7, 4);
        //        HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_ContoursSplit, "contlength", "and", 35, 99999);
        //        HOperatorSet.SortContoursXld(ho_ContoursSplit, out ho_ContoursSplit, "upper_left", "true", "row");


        //        HOperatorSet.CountObj(ho_ContoursSplit, out hv_Number);
        //        hv_RowsVertical = new HTuple();
        //        hv_Rect1 = new HTuple();
        //        hv_Rect2 = new HTuple();
        //        hv_Attrib = 100;
        //        if (hv_Number > 0)
        //{

        //            HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected1, 1);
        //            HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected1, "cont_approx", out hv_Attrib1);
        //            HOperatorSet.LengthXld(ho_ObjectSelected1, out hv_Length1);
        //            HOperatorSet.GenRegionContourXld(ho_ObjectSelected1, out ho_Region, "filled");
        //            HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
        //            HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
        //            HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows1, out hv_Columns1);
        //            HOperatorSet.TupleSort(hv_Columns1, out hv_Columns1);
        //            HOperatorSet.TupleSort(hv_Rows1, out hv_Rows1);

        //            if (hv_Number > 1)
        //            {

        //                HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected2, hv_Number);
        //                HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected2, "cont_approx", out hv_Attrib2);
        //                HOperatorSet.LengthXld(ho_ObjectSelected2, out hv_Length2);
        //                HOperatorSet.GenRegionContourXld(ho_ObjectSelected2, out ho_Region, "filled");
        //                HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
        //                HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
        //                HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows2, out hv_Columns2);
        //                HOperatorSet.TupleSort(hv_Columns2, out hv_Columns2);
        //                HOperatorSet.TupleSort(hv_Rows2, out hv_Rows2);

        //                hv_IsNr = 0;
        //                if (hv_Attrib2 != hv_Attrib1)
        //                {

        //                    hv_Nr1 = 0;
        //                    hv_Nr2 = 0;
        //                    if (hv_Attrib2 < 0)
        //                    {
        //                        HOperatorSet.FitLineContourXld(ho_ObjectSelected2, "huber", -1, 5, 5, 2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr2, out hv_Nc, out hv_Dist);
        //                    }
        //                    else
        //                    {
        //                        HOperatorSet.FitLineContourXld(ho_ObjectSelected1, "huber", -1, 5, 5, 2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr1, out hv_Nc, out hv_Dist);
        //                    }

        //                    hv_Attrib = 1;
        //                    if (hv_Nr1.TupleAbs() > 0.99)
        //                    {
        //                        if (hv_Length1 >= 70|| hv_Nr1.TupleAbs()> 0.999)
        //                         {
        //                            hv_IsNr = 1;
        //                            if (hv_Number >= 3)
        //                            {

        //                                HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected3, hv_Number - 1);
        //                                HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected3, "cont_approx", out hv_Attrib3);
        //                                HOperatorSet.LengthXld(ho_ObjectSelected3, out hv_Length3);
        //                                HOperatorSet.GenRegionContourXld(ho_ObjectSelected3, out ho_Region, "filled");
        //                                HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
        //                                HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
        //                                HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows3, out hv_Columns3);
        //                                HOperatorSet.TupleSort(hv_Columns3, out hv_Columns3);
        //                                HOperatorSet.TupleSort(hv_Rows3, out hv_Rows3);


        //                                if (hv_Attrib3 == hv_Attrib1)
        //                                {
        //                                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows3.TupleSelect(0));
        //                                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2.TupleSelect(0));
        //                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
        //                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows1.TupleSelect(0)) + 1);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows3.TupleSelect(0)) - 1);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

        //                                }
        //                                else
        //                                {
        //                                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows2.TupleSelect(0));
        //                                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2.TupleSelect(0));
        //                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
        //                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows1.TupleSelect(1)) + 1);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows2.TupleSelect(0)) - 2);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

        //                                }

        //                            }
        //                            else
        //                            {
        //                                hv_Rect1 = new HTuple();
        //                                hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows2.TupleSelect(
        //                                    0));
        //                                hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2.TupleSelect(
        //                                    0));
        //                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows2.TupleSelect(
        //                                    1)) + 100);
        //                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns2.TupleSelect(
        //                                    1)) + 100);

        //                                hv_RowsVertical = new HTuple();
        //                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows1.TupleSelect(1)) + 1);
        //                                hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows2.TupleSelect(0)) - 2);
        //                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

        //                            }
        //                        }
        //                    }
        //                    else if (hv_Nr2.TupleAbs() > 0.99)
        //                    {
        //                        if (hv_Length2 >= 70 || hv_Nr2.TupleAbs() > 0.999)
        //                        {
        //                            hv_IsNr = 1;
        //                            if (hv_Number >= 3)
        //                            {
        //                                HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected3, 2);
        //                                HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected3, "cont_approx", out hv_Attrib3);
        //                                HOperatorSet.LengthXld(ho_ObjectSelected3, out hv_Length3);
        //                                HOperatorSet.GenRegionContourXld(ho_ObjectSelected3, out ho_Region, "filled");
        //                                HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
        //                                HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
        //                                HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows3, out hv_Columns3);
        //                                HOperatorSet.TupleSort(hv_Columns3, out hv_Columns3);

        //                                HOperatorSet.TupleSort(hv_Rows3, out hv_Rows3);


        //                                if (hv_Attrib3 == hv_Attrib1)
        //                                {
        //                                    hv_Rect1[0] = 0;
        //                                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(0));
        //                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows3.TupleSelect(1)) - 5);
        //                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);


        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows3.TupleSelect(1)) + 1);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows2.TupleSelect(0)) - 1);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

        //                                }
        //                                else
        //                                {

        //                                    hv_Rect1[0] = 0;
        //                                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(0));
        //                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(1)) - 3);
        //                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(0));
        //                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

        //                                }


        //                            }
        //                            else
        //                            {


        //                                hv_Rect1[0] = 0;
        //                                hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(0));
        //                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(1)) - 3);
        //                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);


        //                                hv_RowsVertical = new HTuple();
        //                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows1.TupleSelect(1)) + 3);
        //                                hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows2.TupleSelect(0)) - 3);
        //                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

        //                            }
        //                        }
        //                    }
        //                }
        //                if (hv_IsNr == 0)
        //                {
        //                    if (hv_Attrib1 == hv_Attrib2 && (hv_Length1 - hv_Length2).TupleAbs() < 10)
        //      {
        //                        hv_Attrib = new HTuple(hv_Attrib1);



        //                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(
        //                            0)) - 100);
        //                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(
        //                            0));
        //                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(
        //                            1)) - 5);
        //                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(
        //                            1)) + 100);



        //                        hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(0)) + 5);
        //                        hv_Rect2 = hv_Rect2.TupleConcat(hv_Columns2.TupleSelect(0));
        //                        hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
        //                        hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);





        //                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
        //                        hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(0));
        //                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);


        //                    }
        //      else
        //                    {
        //                        if (hv_Length1 > hv_Length2)
        //                        {
        //                            hv_Attrib = new HTuple(hv_Attrib1);

        //                            hv_flg = (hv_Rows2.TupleSelect(1)) - ((hv_Rows1.TupleSelect(1)) - (hv_Rows1.TupleSelect(0)));


        //                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(0)) - 100);
        //                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(0)) + 3);
        //                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(1)) - 5);
        //                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

        //                            hv_Rect2 = hv_Rect2.TupleConcat(hv_flg + 5);
        //                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns1.TupleSelect(0)) + 3);
        //                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
        //                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);


        //                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
        //                            hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_flg);
        //                            hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

        //                        }
        //                        else
        //                        {

        //                            hv_Attrib = new HTuple(hv_Attrib2);

        //                            hv_flg = (hv_Rows1.TupleSelect(0)) + ((hv_Rows2.TupleSelect(1)) - (hv_Rows2.TupleSelect(0)));

        //                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(0)) - 100);
        //                            hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2.TupleSelect(0));
        //                            hv_Rect1 = hv_Rect1.TupleConcat(hv_flg - 5);
        //                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);


        //                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(0)) + 5);
        //                            hv_Rect2 = hv_Rect2.TupleConcat(hv_Columns2.TupleSelect(0));
        //                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
        //                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

        //                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_flg);
        //                            hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
        //                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(
        //                                0));
        //                            hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(
        //                                1)) + 100);


        //                        }
        //                    }

        //                }
        //            }
        //            else
        //            {


        //                hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows1.TupleSelect(0));
        //                hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(0)) - 20);
        //                hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows1.TupleSelect(1));
        //                hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

        //                hv_Attrib = new HTuple(hv_Attrib1);
        //            }
        //        }

        //        HOperatorSet.GenEmptyRegion(out ho_Rectangle1);
        //        HOperatorSet.GenEmptyRegion(out ho_Rectangle2);
        //        if (hv_Rect1.TupleLength() > 0)
        //        {

        //            HOperatorSet.GenRectangle1(out ho_Rectangle1, hv_Rect1.TupleSelect(0), hv_Rect1.TupleSelect(1), hv_Rect1.TupleSelect(2), hv_Rect1.TupleSelect(3));

        //        }
        //        if (hv_Rect2.TupleLength() > 0)
        //        {

        //            HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Rect2.TupleSelect(0), hv_Rect2.TupleSelect(1), hv_Rect2.TupleSelect(2), hv_Rect2.TupleSelect(3));

        //        }
        //        HOperatorSet.Union2(ho_Rectangle1, ho_Rectangle2, out ho_RegionUnion);
        //        if (hv_Number > 1)
        //        {
        //            if (hv_RowsVertical.TupleSelect(2) - hv_RowsVertical.TupleSelect(0) <= 100)
        //            {

        //                HOperatorSet.SmallestRectangle1(ho_RegionUnion, out hv_Row11, out hv_Column11, out hv_Row22, out hv_Column22);
        //                HOperatorSet.GenRectangle1(out ho_RegionUnion, hv_Row11, hv_Column11, hv_Row22, hv_Column22);
        //                hv_RowsVertical = new HTuple();
        //            }
        //        }


        //        HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionUnion, out ho_ImageReduced);
        //        HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 180, 255);
        //        if (hv_Attrib < 0)
        //        {

        //            HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing, 25, 15);
        //        }
        //        else
        //        {
        //            HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, 30);
        //        }

        //        HOperatorSet.Difference(ho_RegionClosing, ho_Region, out ho_ConnectedRegions);
        //        HOperatorSet.Connection(ho_ConnectedRegions, out ho_UDRRegion);


        //        //***************计算数值边缘****************
        //        HOperatorSet.GenEmptyRegion(out ho_VerticalRegion);

        //        if (hv_Number > 2 && hv_RowsVertical.TupleLength() > 0)
        //        {


        //            HOperatorSet.GenRectangle1(out ho_Rectangle, (hv_RowsVertical.TupleSelect(0)) + 3, hv_RowsVertical.TupleSelect(1),
        //                                        (hv_RowsVertical.TupleSelect(2)) - 3, hv_RowsVertical.TupleSelect(3));


        //            HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
        //            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 160, 255);
        //            //*****计算是否有缺口***********

        //            HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean, 9, 9);
        //            HOperatorSet.Threshold(ho_ImageMean, out ho_Region1, 0, 155);
        //            HOperatorSet.Connection(ho_Region1, out ho_Region1);
        //            HOperatorSet.SelectShapeStd(ho_Region1, out ho_Region1, "max_area", 70);
        //            HOperatorSet.SmallestRectangle2(ho_Region1, out hv_Row, out hv_Column, out hv_Phi, out hv_Length13, out hv_Length23);
        //            HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row, hv_Column, hv_Phi, hv_Length13, hv_Length23);
        //            HOperatorSet.Difference(ho_Rectangle2, ho_Region1, out ho_RegionDifference);
        //            HOperatorSet.OpeningRectangle1(ho_RegionDifference, out ho_RegionDifference, 21, 7);

        //            HOperatorSet.Connection(ho_RegionDifference, out ho_ConnectedRegions);

        //            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_ConnectedRegions, (new HTuple("area")).TupleConcat(
        //                                  "height"), "and", (new HTuple(350)).TupleConcat(5), (new HTuple(99999)).TupleConcat(30));

        //            //********筛选缺陷余料************

        //            HOperatorSet.DilationRectangle1(ho_ConnectedRegions, out ho_RegionDilation1, 5, 11);
        //            HOperatorSet.Difference(ho_Region, ho_RegionDilation1, out ho_RegionDifference);
        //            HOperatorSet.ClosingRectangle1(ho_RegionDifference, out ho_RegionClosing, 11, 30);
        //            HOperatorSet.FillUp(ho_RegionClosing, out ho_RegionClosing);
        //            HOperatorSet.Difference(ho_RegionClosing, ho_RegionDifference, out ho_VerticalRegion);
        //            HOperatorSet.Union2(ho_VerticalRegion, ho_ConnectedRegions, out ho_VerticalRegion);


        //        }



        //        HOperatorSet.Union2(ho_UDRegion, ho_UDRRegion, out ho_RegionUnion);
        //        HOperatorSet.Union2(ho_VerticalRegion, ho_RegionUnion, out ho_RegionUnion);
        //        HOperatorSet.Union2(ho_HoleRegion, ho_RegionUnion, out ho_RegionUnion);
        //        HOperatorSet.Intersection(ho_LTypeRegion, ho_RegionUnion, out ho_RegionIntersection);
        //        HOperatorSet.Difference(ho_RegionUnion, ho_RegionIntersection, out ho_ConnectedRegions);
        //        HOperatorSet.Connection(ho_ConnectedRegions, out ho_ConnectedRegions);

        //        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_FlawRegion, ((new HTuple("area")).TupleConcat(
        //            "width")).TupleConcat("height"), "and", ((hv_HminArea.TupleConcat(hv_HminWidth))).TupleConcat(
        //            hv_HminHeigth), ((new HTuple(99999)).TupleConcat(999999)).TupleConcat(9999999));

        //        ho_GapRegion.Dispose();
        //        ho_ImageMedian.Dispose();
        //        ho_HoleRegion.Dispose();
        //        ho_Region.Dispose();
        //        ho_RegionFillUp.Dispose();
        //        ho_Border.Dispose();
        //        ho_SelectedXLD.Dispose();
        //        ho_ContoursSplit.Dispose();
        //        ho_UDRegion.Dispose();
        //        ho_ObjectSelected1.Dispose();
        //        ho_ObjectSelected2.Dispose();
        //        ho_Skeleton.Dispose();
        //        ho_EndPoints.Dispose();
        //        ho_JuncPoints.Dispose();
        //        ho_Rectangle.Dispose();
        //        ho_ImageReduced.Dispose();
        //        ho_RegionClosing.Dispose();
        //        ho_Contours.Dispose();
        //        ho_ContoursDifference.Dispose();
        //        ho_Region3.Dispose();
        //        ho_SelectedRegions.Dispose();
        //        ho_RegionUnion1.Dispose();
        //        ho_Rectangle3.Dispose();
        //        ho_RegionDilation.Dispose();
        //        ho_LTypeRegion.Dispose();
        //        ho_Region4.Dispose();
        //        ho_Rectangle4.Dispose();
        //        ho_RegionOpening3.Dispose();
        //        ho_ObjectSelected0.Dispose();
        //        ho_RegionDilation0.Dispose();
        //        ho_RegionDifference6.Dispose();
        //        ho_ImageReduced0.Dispose();
        //        ho_Region5.Dispose();
        //        ho_ConnectedRegions.Dispose();
        //        ho_ObjectSelected3.Dispose();
        //        ho_Rectangle1.Dispose();
        //        ho_Rectangle2.Dispose();
        //        ho_RegionUnion.Dispose();
        //        ho_UDRRegion.Dispose();
        //        ho_VerticalRegion.Dispose();
        //        ho_ImageMean.Dispose();
        //        ho_Region1.Dispose();
        //        ho_RegionDifference.Dispose();
        //        ho_RegionDilation1.Dispose();
        //        ho_RegionIntersection.Dispose();

        //        hv_Number.Dispose();
        //        hv_Attrib1.Dispose();
        //        hv_Attrib2.Dispose();
        //        hv_Length1.Dispose();
        //        hv_Length2.Dispose();
        //        hv_Attrib.Dispose();
        //        hv_Rows.Dispose();
        //        hv_Columns.Dispose();
        //        hv_Row1.Dispose();
        //        hv_Column1.Dispose();
        //        hv_Row21.Dispose();
        //        hv_Column21.Dispose();
        //        hv_Row3.Dispose();
        //        hv_Column3.Dispose();
        //        hv_Phi2.Dispose();
        //        hv_Length12.Dispose();
        //        hv_Length22.Dispose();
        //        hv_temNumber.Dispose();
        //        hv_i.Dispose();
        //        hv_Row2.Dispose();
        //        hv_Column2.Dispose();
        //        hv_Phi1.Dispose();
        //        hv_Length11.Dispose();
        //        hv_Length21.Dispose();
        //        hv_Value0.Dispose();
        //        hv_Value1.Dispose();
        //        hv_flg.Dispose();
        //        hv_Value4.Dispose();
        //        hv_Row12.Dispose();
        //        hv_Column12.Dispose();
        //        hv_Row23.Dispose();
        //        hv_Column23.Dispose();
        //        hv_RowsVertical.Dispose();
        //        hv_Rect1.Dispose();
        //        hv_Rect2.Dispose();
        //        hv_Rows1.Dispose();
        //        hv_Columns1.Dispose();
        //        hv_Rows2.Dispose();
        //        hv_Columns2.Dispose();
        //        hv_IsNr.Dispose();
        //        hv_Nr1.Dispose();
        //        hv_Nr2.Dispose();
        //        hv_RowBegin.Dispose();
        //        hv_ColBegin.Dispose();
        //        hv_RowEnd.Dispose();
        //        hv_ColEnd.Dispose();
        //        hv_Nc.Dispose();
        //        hv_Dist.Dispose();
        //        hv_Attrib3.Dispose();
        //        hv_Length3.Dispose();
        //        hv_Rows3.Dispose();
        //        hv_Columns3.Dispose();
        //        hv_Row11.Dispose();
        //        hv_Column11.Dispose();
        //        hv_Row22.Dispose();
        //        hv_Column22.Dispose();
        //        hv_Row.Dispose();
        //        hv_Column.Dispose();
        //        hv_Phi.Dispose();
        //        hv_Length13.Dispose();
        //        hv_Length23.Dispose();

        //        return;









        //    }



        //    /// <summary>
        //    /// 计算尾部缺陷
        //    /// </summary>
        //    /// <param name="ho_Image"></输入图片>
        //    /// <param name="hv_RminArea"></param>
        //    /// <param name="hv_RminWidth"></param>
        //    /// <param name="hv_RminHeigth"></param>
        //    /// <param name="ho_FlawRegionR"></param>
        //    public void EntryFlawR(HObject ho_Image, out HObject ho_FlawRegionR, HTuple hv_RminArea,
        //        HTuple hv_RminWidth, HTuple hv_RminHeigth)
        //    {


        //        // Local iconic variables 

        //        HObject ho_Region, ho_ConnectedRegions3, ho_EmptyObjectR;
        //        HObject ho_ObjectSelectedR = null, ho_RegionClosingR = null;
        //        HObject ho_RegionDifference1R = null, ho_ConnectedRegions2R = null;
        //        HObject ho_SelectedRegions2;

        //        // Local control variables 

        //        HTuple hv_Number = new HTuple(), hv_i = new HTuple();
        //        // Initialize local and output iconic variables 
        //        HOperatorSet.GenEmptyObj(out ho_Region);
        //        HOperatorSet.GenEmptyObj(out ho_ConnectedRegions3);
        //        HOperatorSet.GenEmptyObj(out ho_ObjectSelectedR);
        //        HOperatorSet.GenEmptyObj(out ho_RegionClosingR);
        //        HOperatorSet.GenEmptyObj(out ho_RegionDifference1R);
        //        HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2R);
        //        HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
        //        //**************计算缺陷*****************************

        //        HOperatorSet.GenEmptyRegion(out ho_FlawRegionR);


        //        HOperatorSet.Threshold(ho_Image, out ho_Region, 200, 255);
        //        HOperatorSet.FillUp(ho_Region, out ho_Region);
        //        HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions3);
        //        HOperatorSet.CountObj(ho_ConnectedRegions3, out hv_Number);


        //        HOperatorSet.GenEmptyObj(out ho_EmptyObjectR);

        //        for (hv_i = 1; hv_i <= hv_Number; ++hv_i)
        //        {

        //            HOperatorSet.SelectObj(ho_ConnectedRegions3, out ho_ObjectSelectedR, hv_i);
        //            HOperatorSet.ClosingCircle(ho_ObjectSelectedR, out ho_RegionClosingR, 75);
        //            HOperatorSet.Difference(ho_RegionClosingR, ho_Region, out ho_RegionDifference1R);
        //            HOperatorSet.Connection(ho_RegionDifference1R, out ho_ConnectedRegions2R);
        //            HOperatorSet.ConcatObj(ho_EmptyObjectR, ho_ConnectedRegions2R, out ho_EmptyObjectR);


        //        }


        //        //*************输出结果******************
        //        HOperatorSet.SelectShape(ho_EmptyObjectR, out ho_SelectedRegions2, ((new HTuple("area")).TupleConcat(
        //            "width")).TupleConcat("height"), "and", ((hv_RminArea.TupleConcat(hv_RminWidth))).TupleConcat(
        //            hv_RminHeigth), ((new HTuple(99999)).TupleConcat(999999)).TupleConcat(999999));

        //        //*************************************
        //        ho_FlawRegionR.Dispose();
        //        ho_FlawRegionR = new HObject(ho_SelectedRegions2);
        //        ho_Region.Dispose();
        //        ho_ConnectedRegions3.Dispose();
        //        ho_EmptyObjectR.Dispose();
        //        ho_ObjectSelectedR.Dispose();
        //        ho_RegionClosingR.Dispose();
        //        ho_RegionDifference1R.Dispose();
        //        ho_ConnectedRegions2R.Dispose();
        //        ho_SelectedRegions2.Dispose();

        //        hv_Number.Dispose();
        //        hv_i.Dispose();

        //        return;
        //    }


        //    /// <summary>
        //    /// 进入主函数
        //    /// </summary>
        //    /// <param name="ho_Image"></输入图片>
        //    /// <param name="hv_RminArea"></R角缺陷最小面积>
        //    /// <param name="hv_RminWidth"></R角缺陷最小宽度>
        //    /// <param name="hv_RminHeigth"></R角缺陷最小高度>
        //    /// <param name="hv_HminArea"></头部缺陷最小面积>
        //    /// <param name="hv_HminWidth"></头部缺陷最小宽度>
        //    /// <param name="hv_HminHeigth"></头部缺陷最小高度>
        //    /// <param name="ho_Contours"></param>
        //    /// <param name="hv_AreaValue"></param>
        //    /// <param name="hv_WidthValue"></param>
        //    /// <param name="hv_HeightValue"></param>
        //    public void EntryBattery(HObject ho_Image,  HTuple hv_RminArea,
        //        HTuple hv_RminWidth, HTuple hv_RminHeigth, HTuple hv_HminArea, HTuple hv_HminWidth,
        //        HTuple hv_HminHeigth, out HObject out_BatRegion, out HTuple hv_AreaValue, out HTuple hv_WidthValue, out HTuple hv_HeightValue)
        //    {

        //        // Local iconic variables 

        //        HObject ho_GrayImage, ho_TempImage = null, ho_Region;
        //        HObject ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle;
        //        HObject ho_RegionDifference, ho_RegionOpening, ho_ConnectedRegions1;
        //        HObject ho_SelectedRegions1, ho_Rectangle1, ho_Rectangle2;
        //        HObject ho_ImageReduced, ho_ImageReduced1, ho_RegionR, ho_FlawRegion;
        //        HObject ho_RegionUnion, ho_ObjectsConcat;

        //        // Local control variables 

        //        HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
        //        HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
        //        HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
        //        HTuple hv_Length2 = new HTuple(), hv_Area = new HTuple();
        //        HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
        //        HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
        //        HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
        //        HTuple hv_LeftOrRight = new HTuple(), hv_flg = new HTuple();
        //        HTuple hv_Value = new HTuple();
        //        // Initialize local and output iconic variables 
        //        HOperatorSet.GenEmptyObj(out ho_GrayImage);
        //        HOperatorSet.GenEmptyObj(out ho_TempImage);
        //        HOperatorSet.GenEmptyObj(out ho_Region);
        //        HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
        //        HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
        //        HOperatorSet.GenEmptyObj(out ho_Rectangle);
        //        HOperatorSet.GenEmptyObj(out ho_RegionDifference);
        //        HOperatorSet.GenEmptyObj(out ho_RegionOpening);
        //        HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
        //        HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
        //        HOperatorSet.GenEmptyObj(out ho_Rectangle1);
        //        HOperatorSet.GenEmptyObj(out ho_Rectangle2);
        //        HOperatorSet.GenEmptyObj(out ho_ImageReduced);
        //        HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
        //        HOperatorSet.GenEmptyObj(out ho_RegionR);
        //        HOperatorSet.GenEmptyObj(out ho_FlawRegion);
        //        HOperatorSet.GenEmptyObj(out ho_RegionUnion);
        //        HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);


        //        //********************输出结果参数******************

        //        hv_AreaValue = 0;
        //        hv_WidthValue = 0;
        //        hv_HeightValue = 0;
        //        HOperatorSet.GenEmptyObj(out out_BatRegion);
        //        //****************************************************
        //        HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
        //        HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
        //        ho_TempImage = new HObject(ho_GrayImage);
        //        //*****************选择亮色区域****************

        //        HOperatorSet.Threshold(ho_TempImage, out ho_Region, 115, 255);
        //        HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
        //        HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area", 70);
        //        HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
        //        HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
        //        HOperatorSet.Difference(ho_Rectangle, ho_SelectedRegions, out ho_RegionDifference);
        //        HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpening, 11);
        //        HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions1);
        //        //*****************定位到极耳********************

        //        HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1, "max_area", 70);
        //        HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_Area, out hv_Row3, out hv_Column3);
        //        //*****************定位检测极耳的位置******************
        //        HOperatorSet.SmallestRectangle1(ho_SelectedRegions1, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);




        //        //******是否镜像*************
        //        hv_LeftOrRight = 0;
        //        if (hv_Column3 > hv_Column)
        //        {
        //            HOperatorSet.MirrorImage(ho_GrayImage, out ho_TempImage, "column");
        //            hv_flg = new HTuple(hv_Column2);
        //            hv_Column2 = hv_Width - hv_Column1;
        //            hv_Column1 = hv_Width - hv_flg;
        //            hv_LeftOrRight = 1;

        //        }
        //        //////////裁切出左右两边极耳************************************
        //        HOperatorSet.GenRectangle1(out ho_Rectangle1, 2, hv_Column1 - 30, hv_Height - 2, hv_Column1 + ((hv_Column2 - hv_Column1) / 2));
        //        HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Row1-200, (hv_Column1 + ((hv_Column2 - hv_Column1) / 2)) - 50, hv_Row2+200, hv_Column2 + 50);

        //        HOperatorSet.ReduceDomain(ho_TempImage, ho_Rectangle1, out ho_ImageReduced);
        //        HOperatorSet.ReduceDomain(ho_TempImage, ho_Rectangle2, out ho_ImageReduced1);

        //        //**********************检测极耳缺陷*************************************
        //        EntryFlawR(ho_ImageReduced, out ho_RegionR, hv_RminArea, hv_RminWidth, hv_RminHeigth);
        //        HOperatorSet.RegionFeatures(ho_SelectedRegions1, "width", out hv_Value);
        //        EntryFlaw(ho_ImageReduced1, ho_Rectangle2, out ho_FlawRegion, hv_Value, hv_Height, hv_HminArea, hv_HminWidth, hv_HminHeigth);


        //        HOperatorSet.Union2(ho_RegionR, ho_FlawRegion, out ho_RegionUnion);
        //        HOperatorSet.Connection(ho_RegionUnion, out ho_ObjectsConcat);
        //        //**********************************************************************

        //        if (hv_LeftOrRight == 1)
        //        {


        //            HOperatorSet.MirrorImage(ho_TempImage, out ho_TempImage, "column");
        //            HOperatorSet.MirrorRegion(ho_ObjectsConcat, out ho_ObjectsConcat, "column", hv_Width);


        //        }

        //        //********************输出结果********************

        //        HOperatorSet.RegionFeatures(ho_ObjectsConcat, "area", out hv_AreaValue);
        //        HOperatorSet.RegionFeatures(ho_ObjectsConcat, "width", out hv_WidthValue);
        //        HOperatorSet.RegionFeatures(ho_ObjectsConcat, "height", out hv_HeightValue);
        //        out_BatRegion = new HObject(ho_ObjectsConcat);
        //        //******************************************************




        //        ho_GrayImage.Dispose();
        //        ho_TempImage.Dispose();
        //        ho_Region.Dispose();
        //        ho_ConnectedRegions.Dispose();
        //        ho_SelectedRegions.Dispose();
        //        ho_Rectangle.Dispose();
        //        ho_RegionDifference.Dispose();
        //        ho_RegionOpening.Dispose();
        //        ho_ConnectedRegions1.Dispose();
        //        ho_SelectedRegions1.Dispose();
        //        ho_Rectangle1.Dispose();
        //        ho_Rectangle2.Dispose();
        //        ho_ImageReduced.Dispose();
        //        ho_ImageReduced1.Dispose();
        //        ho_RegionR.Dispose();
        //        ho_FlawRegion.Dispose();
        //        ho_RegionUnion.Dispose();
        //        ho_ObjectsConcat.Dispose();

        //        hv_Width.Dispose();
        //        hv_Height.Dispose();
        //        hv_Row.Dispose();
        //        hv_Column.Dispose();
        //        hv_Phi.Dispose();
        //        hv_Length1.Dispose();
        //        hv_Length2.Dispose();
        //        hv_Area.Dispose();
        //        hv_Row3.Dispose();
        //        hv_Column3.Dispose();
        //        hv_Row1.Dispose();
        //        hv_Column1.Dispose();
        //        hv_Row2.Dispose();
        //        hv_Column2.Dispose();
        //        hv_LeftOrRight.Dispose();
        //        hv_flg.Dispose();
        //        hv_Value.Dispose();

        //        return;
        //    }





        public void EntryBattery(HObject ho_Image, HTuple hv_RminArea,HTuple hv_RminWidth, HTuple hv_RminHeigth, 
            HTuple hv_HminArea, HTuple hv_HminWidth,HTuple hv_HminHeigth,
            HTuple hv_HyminArea, HTuple hv_HyminWidth, HTuple hv_HyminHeigth,
            out HObject ho_outRegion, out HTuple hv_AreaValue, out HTuple hv_WidthValue, out HTuple hv_HeightValue)
        {

           // HOperatorSet.CropDomain(ho_Image,out HObject img123456);
           //  HOperatorSet.CountSeconds(out HTuple s1);
           // HOperatorSet.WriteImage(ho_Image, "jpeg", -1, "E:/极耳/极耳余料2/" + s1 + ".jpg");

            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_GrayImage, ho_TempImage = null, ho_Region;
            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle;
            HObject ho_RegionDifference, ho_RegionOpening, ho_ConnectedRegions1;
            HObject ho_BattyRegions, ho_Rectangle1, ho_Rectangle2, ho_ImageReduced;
            HObject ho_ImageReduced1, ho_RegionR, ho_FlawRegion, ho_RegionUnion;
            HObject ho_ObjectsConcat, ho_SelectedRegions2, hoRegiontemp;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_LeftOrRight = new HTuple(), hv_flg = new HTuple();
            HTuple hv_Value = new HTuple();
            HTuple abc = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_outRegion);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_TempImage);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_BattyRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_RegionR);
            HOperatorSet.GenEmptyObj(out ho_FlawRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out hoRegiontemp);
            //********************输出结果参数******************
            HOperatorSet.GenEmptyObj(out ho_outRegion);
            hv_AreaValue = new HTuple();
            hv_WidthValue = new HTuple();
            hv_HeightValue = new HTuple();
            //****************************************************
            try
            {
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                ho_TempImage = new HObject(ho_GrayImage);
                //*****************选择亮色区域****************
                HOperatorSet.Threshold(ho_TempImage, out ho_Region, 115, 255);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area", 70);
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle, ho_SelectedRegions, out ho_RegionDifference);
                HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpening, 11);
                HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions1);
                //*****************定位到极耳********************
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_BattyRegions, "max_area", 70);
                HOperatorSet.AreaCenter(ho_BattyRegions, out hv_Area, out hv_Row3, out hv_Column3);
                //*****************定位检测极耳的位置******************
                HOperatorSet.SmallestRectangle1(ho_BattyRegions, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);

                if (hv_Row2 - hv_Row1 <= 200 || hv_Column2 - hv_Column1 < 125)
                {
                   
                
                    HOperatorSet.RegionFeatures(ho_BattyRegions, "area", out abc);
                    if (abc > 0)
                    {
                        hv_AreaValue = new HTuple(abc);
                        HOperatorSet.RegionFeatures(ho_BattyRegions, "width", out hv_WidthValue);
                        HOperatorSet.RegionFeatures(ho_BattyRegions, "height", out hv_HeightValue);
                    }
                    else
                    {
                        hv_AreaValue = new HTuple();
                        hv_WidthValue = new HTuple();
                        hv_HeightValue = new HTuple();
                    }


                    



                    ho_outRegion = new HObject(ho_BattyRegions);
                    return;

                }
                //******镜像*************
                hv_LeftOrRight = 0;
                if (hv_Column3 > hv_Column)
                {
                    HOperatorSet.MirrorImage(ho_GrayImage, out ho_TempImage, "column");
                    hv_flg = new HTuple(hv_Column2);
                    hv_Column2 = hv_Width - hv_Column1;
                    hv_Column1 = hv_Width - hv_flg;
                    hv_LeftOrRight = 1;

                }

                ///////////////////检测出小缺口的区域20240409
                HOperatorSet.Threshold(ho_TempImage,out  hoRegiontemp,0,200);
                HOperatorSet.OpeningRectangle1(hoRegiontemp, out  hoRegiontemp, 10, 10);
                HOperatorSet.ClosingRectangle1(hoRegiontemp,out HObject hoclostemp,45,45);
                HOperatorSet.Difference(hoclostemp, hoRegiontemp,out hoRegiontemp);
                HOperatorSet.Connection(hoRegiontemp, out hoRegiontemp);
                HOperatorSet.SelectShape(hoRegiontemp,out hoRegiontemp,"area","and",50,3000);
                HOperatorSet.CountObj(hoRegiontemp, out HTuple temnumber);
                HOperatorSet.SmallestRectangle2(hoRegiontemp,out HTuple row1,out HTuple col1,out HTuple phi1, out HTuple retcel1,out HTuple retcel2);
                HTuple ht_temp = new HTuple();
                for(int i=1;i<= temnumber;++i)
                {
                    if (retcel1.TupleSelect(i - 1) > retcel2.TupleSelect(i - 1))
                    {
                        if (retcel2.TupleSelect(i - 1) / retcel1.TupleSelect(i - 1) > 0.31)
                            ht_temp = ht_temp.TupleConcat(i);
                    }
                    else
                    {
                        if (retcel1.TupleSelect(i - 1) / retcel2.TupleSelect(i - 1) > 0.31)
                            ht_temp = ht_temp.TupleConcat(i);
                    }

                }
                HOperatorSet.SelectObj(hoRegiontemp,out hoRegiontemp, ht_temp);
                HOperatorSet.Union1(hoRegiontemp, out hoRegiontemp);
                ht_temp.Dispose();
                row1.Dispose();
                col1.Dispose(); phi1.Dispose(); retcel1.Dispose(); retcel2.Dispose();
                hoclostemp.Dispose();
                /////////////////////////////////////////////////////////////////////////////



                HOperatorSet.GenRectangle1(out ho_Rectangle1, 2, hv_Column1 - 30, hv_Height - 2, hv_Column1 + ((hv_Column2 - hv_Column1) / 2));
                HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Row1 - 200, (hv_Column1 + ((hv_Column2 - hv_Column1) / 2)) - 80, hv_Row2 + 200, hv_Column2 + 50);
                HOperatorSet.ReduceDomain(ho_TempImage, ho_Rectangle1, out ho_ImageReduced);
                HOperatorSet.ReduceDomain(ho_TempImage, ho_Rectangle2, out ho_ImageReduced1);

                //**********************检测极耳缺陷*************************************


                
                EntryFlawR(ho_ImageReduced, hoRegiontemp, hv_Height, out ho_RegionR, hv_HyminArea, hv_HyminWidth, hv_HyminHeigth);
                HOperatorSet.RegionFeatures(ho_BattyRegions, "width", out hv_Value);

               // HOperatorSet.WriteImage(ho_ImageReduced, "jpeg", 255, "E:/极耳/新建文件夹/23.jpeg");
                EntryFlaw(ho_ImageReduced1, ho_Rectangle2, hoRegiontemp, out ho_FlawRegion, hv_Value, hv_Height,
                    hv_HminArea, hv_HminWidth, hv_HminHeigth,
                    hv_HyminArea, hv_HyminWidth, hv_HyminHeigth,
                    hv_RminArea, hv_RminWidth, hv_RminHeigth);
               // HOperatorSet.WriteImage(ho_ImageReduced1, "jpeg", 255, "E:/极耳/新建文件夹/123.jpeg");





                HOperatorSet.Union2(ho_RegionR, ho_FlawRegion, out ho_ObjectsConcat);

                ///////////////////20240409
                ///由于增加缺口，会存在将非余料区域选择，这里进行筛选
                HOperatorSet.ReduceDomain(ho_TempImage, ho_ObjectsConcat,out HObject horeduceImage);
                HOperatorSet.Threshold(horeduceImage,out ho_ObjectsConcat, 0,220);

               // HOperatorSet.Union1(ho_RegionR, out ho_ObjectsConcat);
                // HOperatorSet.Connection(ho_RegionUnion, out ho_ObjectsConcat);
                //**********************************************************************

                if (hv_LeftOrRight == 1)
                {

                    //HOperatorSet.MirrorImage(ho_TempImage, out ho_TempImage, "column");
                    HOperatorSet.MirrorRegion(ho_ObjectsConcat, out ho_ObjectsConcat, "column", hv_Width);

                }
                // HOperatorSet.SelectShape(ho_ObjectsConcat, out ho_SelectedRegions2, ((new HTuple("area")).TupleConcat(
                //    "height")).TupleConcat("width"), "and", ((new HTuple(10)).TupleConcat(4)).TupleConcat(
                //   4), ((new HTuple(10000)).TupleConcat(1000)).TupleConcat(500));
                //********************输出结果********************
               





                //HOperatorSet.Union1(ho_ObjectsConcat,out ho_ObjectsConcat);
                //HOperatorSet.RegionFeatures(ho_ObjectsConcat, "area", out abc);
                HOperatorSet.Union1(ho_ObjectsConcat, out ho_ObjectsConcat);
                HOperatorSet.RegionFeatures(ho_ObjectsConcat, "area", out abc);
                if (abc > 0)
                {
                    hv_AreaValue = new HTuple(abc);
                    HOperatorSet.RegionFeatures(ho_ObjectsConcat, "width", out hv_WidthValue);
                    HOperatorSet.RegionFeatures(ho_ObjectsConcat, "height", out hv_HeightValue);
                }
                else
                {
                    hv_AreaValue = new HTuple();
                    hv_WidthValue = new HTuple();
                    hv_HeightValue = new HTuple();
                }


               



                ho_outRegion = new HObject(ho_ObjectsConcat);


            }
            catch (Exception)
            {

            }
            finally
            {
                abc.Dispose();
                ho_GrayImage.Dispose();
                ho_TempImage.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_Rectangle.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_BattyRegions.Dispose();
                ho_Rectangle1.Dispose();
                ho_Rectangle2.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageReduced1.Dispose();
                ho_RegionR.Dispose();
                ho_FlawRegion.Dispose();
                ho_RegionUnion.Dispose();
                ho_ObjectsConcat.Dispose();
                ho_SelectedRegions2.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Area.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_LeftOrRight.Dispose();
                hv_flg.Dispose();
                hv_Value.Dispose();
                hoRegiontemp.Dispose();
            }
           
        }

        public  void EntryFlaw(HObject ho_Image, HObject ho_RectangImage, HObject GapRegion, out HObject ho_FlawRegion,
          HTuple hv_RegionWidth, HTuple hv_RegionHeight,HTuple hv_HminArea, HTuple hv_HminWidth, HTuple hv_HminHeigth,
          HTuple hv_HyminArea, HTuple hv_HyminWidth, HTuple hv_HyminHeigth, HTuple hv_RminArea, HTuple hv_RminWidth, HTuple hv_RminHeigth)
        {

            // Local iconic variables 

            HObject ho_GapRegion, ho_HoleRegion, ho_Region;
            HObject ho_RegionFillUp, ho_Border, ho_SelectedXLD, ho_ContoursSplit;
            HObject ho_SelectedXLD1 = null, ho_Region2, ho_ObjectSelected = null;
            HObject ho_Region1, ho_ObjectSelected2, ho_RegionIntersection = null;
            HObject ho_RegionUnion, ho_Rectangle, ho_ImageReduced, ho_UDRegion;
            HObject ho_ObjectSelected1, ho_Skeleton, ho_EndPoints, ho_JuncPoints;
            HObject ho_RegionClosing = null, ho_Contours, ho_ContoursDifference;
            HObject ho_Region3, ho_SelectedRegions, ho_RegionUnion1;
            HObject ho_Rectangle3, ho_RegionDilation, ho_ConnectedRegions = null;
            HObject ho_ObjectSelected3 = null, ho_Rectangle1, ho_Rectangle2;
            HObject ho_CircleRegion, ho_ImageAffineTrans = null, ho_TEMPRectangle = null;
            HObject ho_UDRRegion, ho_VerticalRegion, ho_SecondVerticalRegion;
            HObject ho_Border1 = null, ho_ContoursSplit1 = null, ho_SortedContours = null;
            HObject ho_RegionUnion2 = null, ho_Region5 = null, ho_Rectangle5 = null;
            HObject ho_RegionClosing1 = null, ho_RegionDifference = null;
            HObject ho_SelectedRegions1 = null, ho_Rectangle4 = null, ho_RegionDifference1 = null;
            HObject ho_ImageReduced1 = null, ho_Region4 = null, ho_ObjectsReduced = null;
            HObject ho_RegionFillUp1 = null, ho_RegionDilation1 = null;
            HObject ho_ObjectSelected4 = null, ho_UDVerticalRegion;

            // Local control variables 

            HTuple hv_ImgWidth = new HTuple(), hv_ImgHeight = new HTuple();
            HTuple hv_Row13 = new HTuple(), hv_Column13 = new HTuple();
            HTuple hv_flg = new HTuple(), hv_i = new HTuple(), hv_Number = new HTuple();
            HTuple hv_Value = new HTuple(), hv_Number1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_Rows4 = new HTuple(), hv_Columns4 = new HTuple();
            HTuple hv_Phi1 = new HTuple(), hv_Length11 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_Attrib1 = new HTuple();
            HTuple hv_Attrib2 = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Attrib = new HTuple();
            HTuple hv_upRows1 = new HTuple(), hv_upColumns1 = new HTuple();
            HTuple hv_DRows2 = new HTuple(), hv_DColumns2 = new HTuple();
            HTuple hv_Columns = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row21 = new HTuple();
            HTuple hv_Column21 = new HTuple(), hv_Value4 = new HTuple();
            HTuple hv_Row12 = new HTuple(), hv_Column12 = new HTuple();
            HTuple hv_Row23 = new HTuple(), hv_Column23 = new HTuple();
            HTuple hv_RowsVertical = new HTuple(), hv_Rect1 = new HTuple();
            HTuple hv_Rect2 = new HTuple(), hv_Rows1 = new HTuple();
            HTuple hv_Columns1 = new HTuple(), hv_Rows2 = new HTuple();
            HTuple hv_Columns2 = new HTuple(), hv_IsNr = new HTuple();
            HTuple hv_Nr1 = new HTuple(), hv_Nr2 = new HTuple(), hv_RowBegin = new HTuple();
            HTuple hv_ColBegin = new HTuple(), hv_RowEnd = new HTuple();
            HTuple hv_ColEnd = new HTuple(), hv_Nc = new HTuple();
            HTuple hv_Dist = new HTuple(), hv_Attrib3 = new HTuple();
            HTuple hv_Length3 = new HTuple(), hv_Rows3 = new HTuple();
            HTuple hv_Columns3 = new HTuple(), hv_Row11 = new HTuple();
            HTuple hv_Column11 = new HTuple(), hv_Row22 = new HTuple();
            HTuple hv_Column22 = new HTuple(), hv_HomMat2DIdentity = new HTuple();
            HTuple hv_W = new HTuple(), hv_H = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column3 = new HTuple(), hv_Length12 = new HTuple();
            HTuple hv_Length22 = new HTuple(), hv_HomMat2DTranslate = new HTuple();
            HTuple hv_HomMat2DRotate = new HTuple(), hv_OutBatFlg1 = new HTuple();
            HTuple hv_OutBatFlg2 = new HTuple(), hv_Number2 = new HTuple();
            HTuple hv_Length = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Value1 = new HTuple(), hv_Value2 = new HTuple();
            HTuple hv_Number3 = new HTuple(), hv_Value3 = new HTuple();
            HTuple hv_AreaRegion = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_FlawRegion);
            HOperatorSet.GenEmptyObj(out ho_GapRegion);
            HOperatorSet.GenEmptyObj(out ho_HoleRegion);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_Border);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD1);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected2);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_UDRegion);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected1);
            HOperatorSet.GenEmptyObj(out ho_Skeleton);
            HOperatorSet.GenEmptyObj(out ho_EndPoints);
            HOperatorSet.GenEmptyObj(out ho_JuncPoints);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_ContoursDifference);
            HOperatorSet.GenEmptyObj(out ho_Region3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected3);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_CircleRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_TEMPRectangle);
            HOperatorSet.GenEmptyObj(out ho_UDRRegion);
            HOperatorSet.GenEmptyObj(out ho_VerticalRegion);
            HOperatorSet.GenEmptyObj(out ho_SecondVerticalRegion);
            HOperatorSet.GenEmptyObj(out ho_Border1);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit1);
            HOperatorSet.GenEmptyObj(out ho_SortedContours);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_Region5);
            HOperatorSet.GenEmptyObj(out ho_Rectangle5);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle4);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Region4);
            HOperatorSet.GenEmptyObj(out ho_ObjectsReduced);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp1);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation1);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected4);
            HOperatorSet.GenEmptyObj(out ho_UDVerticalRegion);
            HOperatorSet.GenEmptyRegion(out ho_FlawRegion);
            HOperatorSet.GenEmptyRegion(out ho_GapRegion);

            HOperatorSet.GetImageSize(ho_Image, out hv_ImgWidth, out hv_ImgHeight);
            //**************计算是否有小孔缺陷***********************

            HOperatorSet.GenEmptyRegion(out ho_HoleRegion);
            HOperatorSet.Threshold(ho_Image, out ho_Region, 150, 255);
            HOperatorSet.Connection(ho_Region, out ho_Region);
            HOperatorSet.SelectShapeStd(ho_Region, out ho_Region, "max_area", 70);
            HOperatorSet.FillUp(ho_Region, out ho_RegionFillUp);
            HOperatorSet.Difference(ho_RegionFillUp, ho_Region, out ho_RegionFillUp);
            HOperatorSet.Connection(ho_RegionFillUp, out ho_RegionFillUp);
            // HOperatorSet.SelectShape(ho_RegionFillUp, out ho_HoleRegion, "area", "and", 20, 99999);
            HOperatorSet.SelectShape(ho_RegionFillUp, out ho_HoleRegion, ((new HTuple("area")).TupleConcat("rect2_len1")).TupleConcat("rect2_len1"), "and",
                                                                          ((hv_HminArea.TupleConcat(hv_HminWidth / 2))).TupleConcat(hv_HminHeigth / 2),
                                                                         ((new HTuple(9999999)).TupleConcat(99999999)).TupleConcat(999999999));

            //****************计算轮廓信息*******************
            HOperatorSet.Threshold(ho_Image,out ho_Border,0,168);
            HOperatorSet.SmallestRectangle1(ho_Border, out hv_Row12, out hv_Column12, out hv_Row23, out hv_Column23);
            HOperatorSet.ThresholdSubPix(ho_Image, out ho_Border, 168);
            HOperatorSet.SelectShapeXld(ho_Border, out ho_SelectedXLD, "contlength", "and", 50, 999999999);
            HOperatorSet.SegmentContoursXld(ho_SelectedXLD, out ho_ContoursSplit, "lines_circles", 3, 5, 4);
            HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_ContoursSplit, "contlength", "and", 20, 99999);
            HOperatorSet.SortContoursXld(ho_ContoursSplit, out ho_ContoursSplit, "upper_left", "true", "row");

            HOperatorSet.CountObj(ho_ContoursSplit, out hv_Number);
            //********************计算上下直线边缘缺陷***************************

            HOperatorSet.GenEmptyRegion(out ho_UDRegion);
            HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected1, 1);
            HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected2, hv_Number);
            HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected1, "cont_approx", out hv_Attrib1);
            HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected2, "cont_approx", out hv_Attrib2);
            HOperatorSet.LengthXld(ho_ObjectSelected1, out hv_Length1);
            HOperatorSet.LengthXld(ho_ObjectSelected2, out hv_Length2);
            hv_Attrib = 100;
            HOperatorSet.GenRegionContourXld(ho_ObjectSelected1, out ho_Region1, "filled");

            HOperatorSet.Skeleton(ho_Region1, out ho_Skeleton);
            HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
            HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_upRows1, out hv_upColumns1);
            HOperatorSet.TupleSort(hv_upColumns1, out hv_upColumns1);
            HOperatorSet.TupleSort(hv_upRows1, out hv_upRows1);



            HOperatorSet.GenRegionContourXld(ho_ObjectSelected2, out ho_Region2, "filled");
            HOperatorSet.Skeleton(ho_Region2, out ho_Skeleton);
            HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
            HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_DRows2, out hv_DColumns2);
            HOperatorSet.TupleSort(hv_DColumns2, out hv_DColumns2);
            HOperatorSet.TupleSort(hv_DRows2, out hv_DRows2);


            Double max_lenght = 0;
            if (hv_Length1 > hv_Length2)
            {
                max_lenght = hv_Length1.D;
                ho_Region = new HObject(ho_Region1);
                hv_Attrib = new HTuple(hv_Attrib1);
                hv_Columns = new HTuple(hv_upColumns1);
            }
            else
            {
                max_lenght = hv_Length2.D;
                ho_Region = new HObject(ho_Region2);
                hv_Attrib = new HTuple(hv_Attrib2);
                hv_Columns = new HTuple(hv_DColumns2);
            }

            HOperatorSet.GenRectangle1(out ho_Rectangle, 0, hv_Columns.TupleSelect(0), hv_RegionHeight, hv_Columns.TupleSelect(1) + 10);
            if (hv_Attrib < 0 && max_lenght < 180)
            {
                HOperatorSet.GenRectangle1(out ho_Rectangle1, 0, hv_Columns.TupleSelect(0), hv_RegionHeight, hv_Column23-30);
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle1, out ho_ImageReduced);
                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 180, 255);
            }

            else
            {
                ho_Rectangle1 = new HObject(ho_Rectangle);

                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle1, out ho_ImageReduced);
                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 180, 255);
                //  HOperatorSet.WriteObject(ho_Region, "E://E.hobj");
                ///////////////////////20240409
                HOperatorSet.Connection(ho_Region, out ho_Region);
                HOperatorSet.RegionFeatures(ho_Region, "area", out hv_AreaRegion);
                HOperatorSet.TupleMin(hv_AreaRegion, out hv_AreaRegion);
                //HOperatorSet.Union1(ho_Region, out ho_Region);
                HOperatorSet.Difference(ho_Region, GapRegion, out ho_Region);
                HOperatorSet.Connection(ho_Region, out ho_Region);
                hv_AreaRegion = hv_AreaRegion - 1000;
                if (hv_AreaRegion < 0)
                    hv_AreaRegion = 0;
                HOperatorSet.SelectShape(ho_Region, out ho_Region, "area", "and", hv_AreaRegion, 9999999);
                HOperatorSet.Union1(ho_Region, out ho_Region);
            }
            ///////////////////////////////////////
            
            if (hv_Attrib < 0)
            {

                //if (max_lenght >= 130)
                //{
                //    HOperatorSet.GenRectangle1(out ho_Rectangle, 0, hv_Columns.TupleSelect(0), hv_RegionHeight, hv_Columns.TupleSelect(1) + 20);
                //    HOperatorSet.GenRectangle1(out ho_Rectangle1, 0, hv_Columns.TupleSelect(0), hv_RegionHeight, hv_Columns.TupleSelect(1) + 150);
                //    HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle1, out ho_ImageReduced);
                //    HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 180, 255);

                //    HOperatorSet.Connection(ho_Region, out ho_Region);
                //    HOperatorSet.RegionFeatures(ho_Region, "area", out hv_AreaRegion);
                //    HOperatorSet.TupleMin(hv_AreaRegion, out hv_AreaRegion);
                //    HOperatorSet.Union1(ho_Region, out ho_Region);
                //    HOperatorSet.Difference(ho_Region, GapRegion, out ho_Region);
                //    HOperatorSet.Connection(ho_Region, out ho_Region);
                //    hv_AreaRegion = hv_AreaRegion - 1000;
                //    if (hv_AreaRegion < 0)
                //        hv_AreaRegion = 0;
                //    HOperatorSet.SelectShape(ho_Region, out ho_Region, "area", "and", hv_AreaRegion, 999999);
                //    HOperatorSet.Union1(ho_Region, out ho_Region);

                //    HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing, 100, 35);
                //}
                //else
                //{
                    HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing, 50, 35);
               // }


            }
            else
            {
                HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, 30);
            }

            

            HOperatorSet.Difference(ho_RegionClosing, ho_Region, out ho_RegionClosing);

            
            HOperatorSet.SelectShape(ho_RegionClosing, out ho_UDRegion, ((new HTuple("area")).TupleConcat(
                 "rect2_len1")).TupleConcat("rect2_len2"), "and", ((hv_HyminArea.TupleConcat(hv_HyminWidth / 2))).TupleConcat(
                 hv_HyminHeigth / 2), ((new HTuple(9999999)).TupleConcat(99999999)).TupleConcat(999999999));

          
            //*********************************************************************************************
            HOperatorSet.GenContourRegionXld(ho_Rectangle, out ho_Contours, "border");
            HOperatorSet.DifferenceClosedContoursXld(ho_SelectedXLD, ho_Contours, out ho_ContoursDifference);
            HOperatorSet.GenRegionContourXld(ho_ContoursDifference, out ho_Region3, "filled");
            HOperatorSet.SelectShape(ho_Region3, out ho_SelectedRegions, "height", "and", 80, 99999);
            HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion1);
            HOperatorSet.SmallestRectangle1(ho_RegionUnion1, out hv_Row1, out hv_Column1, out hv_Row21, out hv_Column21);
            if (hv_Row1.Length != 0)
            {
                HOperatorSet.GenRectangle1(out ho_Rectangle3, hv_Row1, hv_Column1, hv_Row21, hv_Column21);
                HOperatorSet.GenRectangle1(out ho_RegionDilation, hv_Row1 - 50, hv_Column1, hv_Row21 + 50, hv_Column21 + 5);
            }



            //***********************计算上下拐角处的缺陷******************

            HOperatorSet.RegionFeatures(ho_Rectangle3, "width", out hv_Value4);
            if (hv_Value4 < 10)
            {
                HOperatorSet.OpeningRectangle1(ho_Region, out ho_ConnectedRegions, 20, 10);
                HOperatorSet.Connection(ho_ConnectedRegions, out ho_ConnectedRegions);
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_ConnectedRegions, "area", "and", 1000, 99999);
                HOperatorSet.Union1(ho_ConnectedRegions, out ho_ConnectedRegions);
                HOperatorSet.SmallestRectangle1(ho_ConnectedRegions, out hv_Row12, out hv_Column12, out hv_Row23, out hv_Column23);
                HOperatorSet.GenRectangle1(out HObject Rectangle4, hv_Row12, hv_Column12, hv_Row23, hv_Column23);
                HOperatorSet.Difference(Rectangle4, ho_ConnectedRegions, out ho_ConnectedRegions);
                HOperatorSet.Connection(ho_ConnectedRegions, out ho_ConnectedRegions);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_ConnectedRegions, "max_area", 70);
                HOperatorSet.SmallestRectangle1(ho_ConnectedRegions, out hv_Row12, out hv_Column12, out hv_Row23, out hv_Column23);
                HOperatorSet.GenRectangle1(out ho_RegionDilation, hv_Row12 + 35, 0, hv_Row23 - 35, hv_Column23 + 100);
                Rectangle4.Dispose();

            }

            HOperatorSet.ReduceDomain(ho_Image, ho_RegionDilation, out ho_ImageReduced);
            HOperatorSet.ThresholdSubPix(ho_ImageReduced, out ho_Border, 150);
            HOperatorSet.SelectShapeXld(ho_Border, out ho_SelectedXLD, "contlength", "and", 50, 99999);
            HOperatorSet.SegmentContoursXld(ho_SelectedXLD, out ho_ContoursSplit, "lines_circles", 2, 5, 2);
            HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_ContoursSplit, "contlength", "and", 15, 99999);
            HOperatorSet.SortContoursXld(ho_ContoursSplit, out ho_ContoursSplit, "upper_left", "true", "row");
            HOperatorSet.CountObj(ho_ContoursSplit, out hv_Number);
            //HOperatorSet.WriteObject(ho_ContoursSplit, "E://E1.hobj");

            hv_Rect1 = new HTuple();
            hv_Rect2 = new HTuple();
            hv_Attrib = 100;
            int IsRorH = 0;
            if (hv_Number > 0)
            {

                if (hv_Number <= 3)
                {
                    HTuple hv_Number4 = new HTuple();
                    HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_SelectedXLD, "contlength", "and", 50, 99999);
                    HOperatorSet.CountObj(ho_SelectedXLD, out hv_Number4);
                    if (hv_Number4 == 1)
                    {
                        hv_Number = new HTuple(hv_Number4);
                        ho_ContoursSplit = new HObject(ho_SelectedXLD);
                    }
                    hv_Number4.Dispose();
                }



                HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected1, 1);
                HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected1, "cont_approx", out hv_Attrib1);
                HOperatorSet.LengthXld(ho_ObjectSelected1, out hv_Length1);
                HOperatorSet.GenRegionContourXld(ho_ObjectSelected1, out ho_Region, "filled");
                HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
                HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows1, out hv_Columns1);
                HOperatorSet.TupleSort(hv_Columns1, out hv_Columns1);
                HOperatorSet.TupleSort(hv_Rows1, out hv_Rows1);

                if (hv_Number > 1)
                {
                    HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected2, hv_Number);
                    HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected2, "cont_approx", out hv_Attrib2);
                    HOperatorSet.LengthXld(ho_ObjectSelected2, out hv_Length2);
                    HOperatorSet.GenRegionContourXld(ho_ObjectSelected2, out ho_Region, "filled");
                    HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
                    HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                    HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows2, out hv_Columns2);
                    HOperatorSet.TupleSort(hv_Columns2, out hv_Columns2);
                    HOperatorSet.TupleSort(hv_Rows2, out hv_Rows2);
                    hv_IsNr = 0;
                    if (hv_Attrib2 != hv_Attrib1)
                    {

                        hv_Nr1 = 0;
                        hv_Nr2 = 0;
                        if (hv_Attrib2 < 0)
                        {

                            HOperatorSet.FitLineContourXld(ho_ObjectSelected2, "huber", -1, 5, 5, 2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr2, out hv_Nc, out hv_Dist);
                        }
                        else
                        {
                            HOperatorSet.FitLineContourXld(ho_ObjectSelected1, "huber", -1, 5, 5, 2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr1, out hv_Nc, out hv_Dist);
                        }
                        hv_Attrib = 1;
                        if (hv_Nr1.TupleAbs() >= 0.99)
                        {
                            if ((hv_Length1 >= 70 || hv_Nr1.TupleAbs() > 0.999) && hv_Rows1[0] - hv_upRows1[0] > 20)
                            {
                                hv_IsNr = 1;
                                if (hv_Number >= 3)
                                {


                                    HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected3, hv_Number - 1);
                                    HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected3, "cont_approx", out hv_Attrib3);
                                    HOperatorSet.LengthXld(ho_ObjectSelected3, out hv_Length3);
                                    HOperatorSet.GenRegionContourXld(ho_ObjectSelected3, out ho_Region, "filled");
                                    HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
                                    HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                                    HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows3, out hv_Columns3);
                                    HOperatorSet.TupleSort(hv_Columns3, out hv_Columns3);
                                    HOperatorSet.TupleSort(hv_Rows3, out hv_Rows3);




                                    if (hv_Attrib3 != hv_Attrib1)
                                    {
                                        hv_Rect1.Dispose();

                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows2.TupleSelect(0));
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2.TupleSelect(0));
                                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
                                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                                        hv_RowsVertical.Dispose();
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows1.TupleSelect(1)) + 1);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows2.TupleSelect(0)) - 2);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                                    }
                                    else

                                    {
                                        hv_Rect1.Dispose();
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows3[0]);
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2[0]);
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows2[1] + 100);
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2[1] + 100);


                                        hv_RowsVertical.Dispose();
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1[0] + 1);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows3[0] - 1);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Columns2[1] + 100);
                                    }
                                }

                            }
                            else
                            {
                                hv_Rect1.Dispose();
                                hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows2.TupleSelect(0));
                                hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2.TupleSelect(0));
                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                                hv_RowsVertical.Dispose();
                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows1.TupleSelect(1)) + 1);
                                hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows2.TupleSelect(0)) - 2);
                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                            }
                        }
                        else if (hv_Nr2.TupleAbs() >= 0.99)
                        {

                            if ((hv_Length2 >= 70 || hv_Nr2.TupleAbs() > 0.999) && hv_DRows2[0] - hv_Rows2[1] > 20)
                            {

                                hv_IsNr = 1;
                                if (hv_Number >= 3)
                                {

                                    HOperatorSet.SelectObj(ho_ContoursSplit, out ho_ObjectSelected3, 2);
                                    HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected3, "cont_approx", out hv_Attrib3);
                                    HOperatorSet.LengthXld(ho_ObjectSelected3, out hv_Length3);
                                    HOperatorSet.GenRegionContourXld(ho_ObjectSelected3, out ho_Region, "filled");
                                    HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
                                    HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                                    HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows3, out hv_Columns3);
                                    HOperatorSet.TupleSort(hv_Columns3, out hv_Columns3);
                                    HOperatorSet.TupleSort(hv_Rows3, out hv_Rows3);


                                    if (hv_Attrib3 == hv_Attrib1)
                                    {


                                        hv_Rect1.Dispose();
                                        hv_Rect1 = hv_Rect1.TupleConcat(0);
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(0));
                                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows3.TupleSelect(1)) - 5);
                                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

                                        hv_RowsVertical.Dispose();
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows3.TupleSelect(1)) + 1);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows2.TupleSelect(0)) - 10);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                                    }
                                    else
                                    {
                                        hv_Rect1.Dispose();
                                        hv_Rect1 = hv_Rect1.TupleConcat(0);
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(0));
                                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(1)) - 3);
                                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

                                        hv_RowsVertical.Dispose();

                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(0));
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                                    }


                                }
                                else
                                {
                                    hv_Rect1.Dispose();
                                    hv_Rect1 = hv_Rect1.TupleConcat(0);
                                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(0));
                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(1)) - 3);
                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

                                    hv_RowsVertical.Dispose();

                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows1.TupleSelect(1)) + 3);
                                    hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Rows2.TupleSelect(0)) - 3);
                                    hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                                }
                            }

                        }
                    }

                    if (hv_IsNr == 0)
                    {
                        if (hv_Attrib1 == hv_Attrib2 && (hv_Length1 - hv_Length2).TupleAbs() < 10)
                        {
                            hv_Attrib = hv_Attrib1;

                            hv_Rect1.Dispose();
                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(0)) - 100);
                            hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(hv_Rect1.TupleConcat(0)));
                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(hv_Rect1.TupleConcat(1)) - 5));
                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(hv_Rect1.TupleConcat(1)) + 100));

                            hv_Rect2.Dispose();
                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(0)) + 5);
                            hv_Rect2 = hv_Rect2.TupleConcat(hv_Columns2.TupleSelect(0));
                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                            hv_RowsVertical.Dispose();
                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
                            hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(0));
                            hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);


                        }
                        else
                        {
                            //////////////202404909
                            if (hv_Rows1[0] - hv_upRows1[1] > 20 || hv_DRows2[1] - hv_Rows2[1] > 20)
                            {
                                if (hv_Length1 > hv_Length2)
                                {
                                    hv_Attrib = hv_Attrib1;
                                }
                                else
                                {
                                    hv_Attrib = hv_Attrib2;
                                }
                                hv_Rect1.Dispose();

                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(0)) - 100);
                                hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns1.TupleSelect(0));
                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(1)) - 5);
                                hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

                                hv_Rect2.Dispose();

                                hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(0)) + 5);
                                hv_Rect2 = hv_Rect2.TupleConcat(hv_Columns2.TupleSelect(0));
                                hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
                                hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);

                                hv_RowsVertical.Dispose();
                                hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
                                hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(0));
                                hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);


                            }
                            else
                            {

                                HOperatorSet.SegmentContoursXld(ho_SelectedXLD, out HObject ho_ContoursSplitTemp, "lines_circles", 2, 2, 2);
                                HOperatorSet.SortContoursXld(ho_ContoursSplitTemp, out ho_ContoursSplitTemp, "upper_left", "true", "row");
                                HOperatorSet.CountObj(ho_ContoursSplitTemp, out HTuple hv_Number4);

                                if (hv_Length1 > hv_Length2)
                                {

                                    hv_Attrib = hv_Attrib1;
                                    hv_Rect1.Dispose();
                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(0)) - 100);
                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(0)) + 3);
                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(1)) - 5);
                                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);
                                    hv_flg = (hv_Rows2.TupleSelect(1)) - ((hv_Rows1.TupleSelect(1)) - (hv_Rows1.TupleSelect(0)));

                                    //////////////20240428,分析是否属于mark孔
                                    if (hv_Number - 2 > 1 && hv_Attrib2 != hv_Attrib1 && hv_Columns1.TupleSelect(1) - hv_Columns2.TupleSelect(1) >= 40)
                                    {


                                        for (int i = 2; i < hv_Number4 - 1; ++i)
                                        {
                                            HOperatorSet.SelectObj(ho_ContoursSplitTemp, out ho_ObjectSelected1, hv_Number4 - i);

                                            HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected1, "cont_approx", out hv_Attrib2);
                                            HOperatorSet.LengthXld(ho_ObjectSelected1, out hv_Length2);
                                            HOperatorSet.GenRegionContourXld(ho_ObjectSelected1, out ho_Region, "filled");
                                            HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
                                            HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                                            HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows4, out hv_Columns4);
                                            HOperatorSet.TupleSort(hv_Columns4, out hv_Columns4);
                                            HOperatorSet.TupleSort(hv_Rows4, out hv_Rows4);
                                            if (hv_Attrib2 < 0 && (hv_Rows4.TupleSelect(0) - hv_Rows4.TupleSelect(1)).TupleAbs() <= 5 && hv_Length2>16)
                                            {
                                                break;
                                            }
                                        }



                                        if (hv_Attrib2 < 0 && (hv_Rows4.TupleSelect(0) - hv_Rows4.TupleSelect(1)).TupleAbs() <= 5)
                                        {
                                            hv_Attrib = -1;
                                            hv_RowsVertical.Dispose();
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows4.TupleSelect(0));
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);
                                        }
                                        else
                                        {

                                            hv_Rect2.Dispose();
                                            hv_Rect2 = hv_Rect2.TupleConcat(hv_flg + 5);
                                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns1.TupleSelect(0)) + 3);
                                            hv_Rect2 = hv_Rect2.TupleConcat(hv_Rows2.TupleSelect(1));
                                            hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

                                            hv_RowsVertical.Dispose();
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_flg);
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);


                                        }

                                    }
                                    else
                                    {




                                        hv_Rect2.Dispose();
                                        hv_Rect2 = hv_Rect2.TupleConcat(hv_flg + 5);
                                        hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns1.TupleSelect(0)) + 3);
                                        hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
                                        hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

                                        hv_RowsVertical.Dispose();
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows1.TupleSelect(1));
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_flg);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);
                                    }

                                }
                                else
                                {




                                    hv_Attrib = hv_Attrib2;

                                    hv_flg = (hv_Rows1.TupleSelect(0)) + ((hv_Rows2.TupleSelect(1)) - (hv_Rows2.TupleSelect(0)));


                                    hv_Rect2.Dispose();
                                    hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(0)) + 5);
                                    hv_Rect2 = hv_Rect2.TupleConcat(hv_Columns2.TupleSelect(0));
                                    hv_Rect2 = hv_Rect2.TupleConcat((hv_Rows2.TupleSelect(1)) + 100);
                                    hv_Rect2 = hv_Rect2.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);


                                    if (hv_Number > 3 && hv_Attrib2 != hv_Attrib1 && hv_Columns2.TupleSelect(1) - hv_Columns1.TupleSelect(1) >= 40)
                                    {

                                        for (int i = 3; i < hv_Number4-1; ++i)
                                        {
                                            HOperatorSet.SelectObj(ho_ContoursSplitTemp, out ho_ObjectSelected1, i);
                                            HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected1, "cont_approx", out hv_Attrib1);
                                            HOperatorSet.LengthXld(ho_ObjectSelected1, out hv_Length2);
                                            HOperatorSet.GenRegionContourXld(ho_ObjectSelected1, out ho_Region, "filled");
                                            HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
                                            HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                                            HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows4, out hv_Columns4);
                                            HOperatorSet.TupleSort(hv_Columns4, out hv_Columns4);
                                            HOperatorSet.TupleSort(hv_Rows4, out hv_Rows4);
                                            if (hv_Attrib1 < 0 && (hv_Rows4.TupleSelect(0) - hv_Rows4.TupleSelect(1)).TupleAbs() <= 5  && hv_Length2>16)
                                            {
                                                break;
                                            }
                                        }


                                        if (hv_Attrib1 < 0 && (hv_Rows4.TupleSelect(0) - hv_Rows4.TupleSelect(1)).TupleAbs() <= 5)
                                        {
                                            hv_Attrib = -1;
                                            hv_RowsVertical.Dispose();
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows4.TupleSelect(1));
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(0));
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);
                                        }
                                        else
                                        {
                                            hv_Rect1.Dispose();
                                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(0)) - 100);
                                            hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2.TupleSelect(0));
                                            hv_Rect1 = hv_Rect1.TupleConcat(hv_flg - 5);
                                            hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);


                                            hv_RowsVertical.Dispose();
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_flg);
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(0));
                                            hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);
                                        }

                                    }
                                    else
                                    {


                                        hv_Rect1.Dispose();
                                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Rows1.TupleSelect(0)) - 100);
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_Columns2.TupleSelect(0));
                                        hv_Rect1 = hv_Rect1.TupleConcat(hv_flg - 5);
                                        hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns2.TupleSelect(1)) + 100);


                                        hv_RowsVertical.Dispose();
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_flg);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(0);
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat(hv_Rows2.TupleSelect(0));
                                        hv_RowsVertical = hv_RowsVertical.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);

                                    }
                                }
                                //////////////////////////////////////





                            }
                        }
                    }
                }
                else
                {

                    hv_Rect1.Dispose();
                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows1.TupleSelect(0));
                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(0)) - 20);
                    hv_Rect1 = hv_Rect1.TupleConcat(hv_Rows1.TupleSelect(1));
                    hv_Rect1 = hv_Rect1.TupleConcat((hv_Columns1.TupleSelect(1)) + 100);
                    hv_Attrib = hv_Attrib1;
                    IsRorH = 1;
                }

            }



            HOperatorSet.GenEmptyRegion(out ho_Rectangle1);
            HOperatorSet.GenEmptyRegion(out ho_Rectangle2);
            if (hv_Rect1.TupleLength() > 0)
            {
                HOperatorSet.GenRectangle1(out ho_Rectangle1, hv_Rect1.TupleSelect(0), hv_Rect1.TupleSelect(1), hv_Rect1.TupleSelect(2), hv_Rect1.TupleSelect(3));

            }
            if (hv_Rect2.TupleLength() > 0)
            {
                HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Rect2.TupleSelect(0), hv_Rect2.TupleSelect(1), hv_Rect2.TupleSelect(2), hv_Rect2.TupleSelect(3));

            }
            HOperatorSet.Union2(ho_Rectangle1, ho_Rectangle2, out ho_RegionUnion);
            if (hv_Number > 1)
            {

                if (hv_RowsVertical.TupleSelect(2) - hv_RowsVertical.TupleSelect(0) < 100)
                {

                    HOperatorSet.SmallestRectangle1(ho_RegionUnion, out hv_Row11, out hv_Column11, out hv_Row22, out hv_Column22);
                    HOperatorSet.GenRectangle1(out ho_RegionUnion, hv_Row11, hv_Column11, hv_Row22, hv_Column22);

                    hv_RowsVertical = new HTuple();
                }
            }

            HOperatorSet.ReduceDomain(ho_Image, ho_RegionUnion, out ho_ImageReduced);
           // HOperatorSet.WriteObject(ho_ImageReduced, "E:\\ho_ImageReduced.hobj");
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 180, 255);
            ///////////////////20240409lzm
            HOperatorSet.Connection(ho_Region, out ho_Region);
            HOperatorSet.RegionFeatures(ho_Region, "area", out hv_AreaRegion);
            HOperatorSet.TupleMin(hv_AreaRegion, out hv_AreaRegion);
            HOperatorSet.Union1(ho_Region, out ho_Region);
            HOperatorSet.Difference(ho_Region, GapRegion, out ho_Region);
            HOperatorSet.Connection(ho_Region, out ho_Region);
            hv_AreaRegion = hv_AreaRegion - 1000;
            if (hv_AreaRegion < 0)
                hv_AreaRegion = 0;
            HOperatorSet.SelectShape(ho_Region, out ho_Region, "area", "and", hv_AreaRegion, 999999);
            HOperatorSet.Union1(ho_Region, out ho_Region);
            ///////////////////////////////

            if (hv_Attrib < 0)
            {

                HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing, 35, 25);
            }
            else
            {
                HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, 30);
            }

            HOperatorSet.Difference(ho_RegionClosing, ho_Region, out ho_ConnectedRegions);
            HOperatorSet.Connection(ho_ConnectedRegions, out ho_ConnectedRegions);

            if (IsRorH == 0)//0：表示R角；1：表示顶部
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_CircleRegion, ((new HTuple("area")).TupleConcat("rect2_len1")).TupleConcat("rect2_len2"), "and",
                                                                            ((hv_RminArea.TupleConcat(hv_RminWidth / 2))).TupleConcat(hv_RminHeigth / 2),
                                                                                ((new HTuple(9999999)).TupleConcat(9999999)).TupleConcat(9999999));
            else
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_CircleRegion, ((new HTuple("area")).TupleConcat("rect2_len1")).TupleConcat("rect2_len2"), "and",
                                                                                   ((hv_HminArea.TupleConcat(hv_HminWidth / 2))).TupleConcat(hv_HminHeigth / 2),
                                                                                   ((new HTuple(9999999)).TupleConcat(9999999)).TupleConcat(9999999));

            HOperatorSet.CountObj(ho_CircleRegion, out hv_Number1);
            //HOperatorSet.WriteObject(ho_CircleRegion, "E:\\ho_UDRRegion.hobj");


            hv_flg.Dispose();
            hv_flg = new HTuple();
            if (hv_Attrib > 0)
            {
                hv_HomMat2DIdentity.Dispose();
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                hv_W = hv_ImgWidth / 2;
                hv_H = hv_ImgHeight / 2;

                for (hv_i = 1; hv_i <= hv_Number1; ++hv_i)
                {
                    HOperatorSet.SelectObj(ho_CircleRegion, out ho_ObjectSelected1, hv_i);
                    HOperatorSet.SmallestRectangle2(ho_ObjectSelected1, out hv_Row3, out hv_Column3, out hv_Phi1, out hv_Length12, out hv_Length22);
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row3, hv_Column3, out hv_HomMat2DTranslate);
                    HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, -hv_Phi1, hv_Row3, hv_Column3, out hv_HomMat2DRotate);
                    HOperatorSet.AffineTransImage(ho_Image, out ho_ImageAffineTrans, hv_HomMat2DRotate, "constant", "false");
                    HOperatorSet.AffineTransRegion(ho_ObjectSelected1, out ho_TEMPRectangle, hv_HomMat2DRotate, "nearest_neighbor");
                    TestForKill(ho_ImageAffineTrans, ho_TEMPRectangle, out hv_OutBatFlg1);
                    TestForKill(ho_Image, ho_ObjectSelected1, out hv_OutBatFlg2);
                    if (hv_OutBatFlg1 != 0 || hv_OutBatFlg2 != 0)
                    {

                        hv_flg = hv_flg.TupleConcat(hv_i);

                    }
                }
            }
            else
            {
                for (hv_i = 1; hv_i <= hv_Number1; ++hv_i)
                {
                    HOperatorSet.SelectObj(ho_CircleRegion, out ho_ObjectSelected, hv_i);
                    TestForKill(ho_Image, ho_ObjectSelected, out hv_OutBatFlg1);
                    if (hv_OutBatFlg1 != 0)
                    {

                        hv_flg = hv_flg.TupleConcat(hv_i);

                    }
                }
            }

            HOperatorSet.SelectObj(ho_CircleRegion, out ho_UDRRegion, hv_flg);


            //***************计算数值边缘****************
            HOperatorSet.GenEmptyRegion(out ho_VerticalRegion);
            HOperatorSet.GenEmptyRegion(out ho_SecondVerticalRegion);
            if (hv_Number > 2 && hv_RowsVertical.TupleLength() > 0)
            {
                HOperatorSet.GenRectangle1(out ho_Rectangle, (hv_RowsVertical.TupleSelect(0)) - 10,
                    (hv_RowsVertical.TupleSelect(1)) + 3, hv_RowsVertical.TupleSelect(2), hv_RowsVertical.TupleSelect(3));

                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
                HOperatorSet.ThresholdSubPix(ho_ImageReduced, out ho_Border1, 168);
                HOperatorSet.SegmentContoursXld(ho_Border1, out ho_ContoursSplit1, "lines_circles", 2, 3, 4);
                HOperatorSet.SelectShapeXld(ho_ContoursSplit1, out ho_SelectedXLD1, "contlength", "and", 25, 99999);
                HOperatorSet.CountObj(ho_SelectedXLD1, out hv_Number2);

                if (hv_Number2 > 2)
                {

                    HOperatorSet.SortContoursXld(ho_SelectedXLD1, out ho_SortedContours, "upper_left", "true", "row");
                    HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected1, 1);
                    HOperatorSet.LengthXld(ho_ObjectSelected1, out hv_Length);
                    HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected1, "cont_approx", out hv_Attrib1);
                    if (hv_Attrib1 > 0 && hv_Length >= 80)
                    {

                        HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected2, 2);
                        HOperatorSet.GetContourGlobalAttribXld(ho_ObjectSelected2, "cont_approx", out hv_Attrib2);
                        if (hv_Attrib != hv_Attrib2)
                        {

                            HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected3, hv_Number2);
                            HOperatorSet.GenRegionContourXld(ho_ObjectSelected3, out ho_Region, "filled");
                            HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
                            HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                            HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows3, out hv_Columns3);
                            HOperatorSet.TupleSort(hv_Columns3, out hv_Columns3);
                            HOperatorSet.TupleSort(hv_Rows3, out hv_Rows3);

                            if ((hv_Rows3.TupleSelect(1) - 10 - hv_RowsVertical.TupleSelect(2)).TupleAbs() < 10)
                            {

                                HOperatorSet.GenRegionContourXld(ho_ObjectSelected1, out ho_Region, "filled");
                                HOperatorSet.Skeleton(ho_Region, out ho_Skeleton);
                                HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                                HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_Rows3, out hv_Columns3);
                                HOperatorSet.TupleSort(hv_Columns3, out hv_Columns3);
                                HOperatorSet.TupleSort(hv_Rows3, out hv_Rows3);
                                HOperatorSet.GenRectangle1(out ho_Rectangle1, hv_Rows3.TupleSelect(0), hv_Columns3.TupleSelect(0), hv_Rows3.TupleSelect(1), (hv_Columns3.TupleSelect(1)) + 20);
                                HOperatorSet.GenRectangle1(out ho_Rectangle2, (hv_RowsVertical.TupleSelect(
                                    2)) - ((hv_Rows3.TupleSelect(1)) - (hv_Rows3.TupleSelect(0))), hv_Columns3.TupleSelect(
                                    0), (hv_RowsVertical.TupleSelect(2)) + 10, (hv_Columns3.TupleSelect(1)) + 20);


                                HOperatorSet.Union2(ho_Rectangle1, ho_Rectangle2, out ho_RegionUnion2);
                                HOperatorSet.ReduceDomain(ho_Image, ho_RegionUnion2, out ho_ImageReduced);
                                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region5, 165, 255);
                                ///////////20240409
                                HOperatorSet.Connection(ho_Region5, out ho_Region5);
                                HOperatorSet.RegionFeatures(ho_Region5, "area", out hv_AreaRegion);
                                HOperatorSet.TupleMin(hv_AreaRegion, out hv_AreaRegion);
                                HOperatorSet.Union1(ho_Region5, out ho_Region5);
                                HOperatorSet.Difference(ho_Region5, GapRegion, out ho_Region5);
                                HOperatorSet.Connection(ho_Region5, out ho_Region5);
                                hv_AreaRegion = hv_AreaRegion - 1000;
                                if (hv_AreaRegion < 0)
                                    hv_AreaRegion = 0;
                                HOperatorSet.SelectShape(ho_Region5, out ho_Region5, "area", "and", hv_AreaRegion, 999999);
                                HOperatorSet.Union1(ho_Region5, out ho_Region5);
                                ///////////////////////////////////////////////////////////////
                                HOperatorSet.ClosingCircle(ho_Region5, out ho_RegionClosing, 30);
                                HOperatorSet.Difference(ho_RegionClosing, ho_Region5, out ho_ConnectedRegions);
                                HOperatorSet.Connection(ho_ConnectedRegions, out ho_ConnectedRegions);

                                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SecondVerticalRegion,
                                    ((new HTuple("area")).TupleConcat("width")).TupleConcat("height"),
                                    "and", ((hv_HminArea.TupleConcat(hv_HminWidth))).TupleConcat(hv_HminHeigth),
                                    ((new HTuple(99999)).TupleConcat(999999)).TupleConcat(9999999));
                                HOperatorSet.CountObj(ho_SecondVerticalRegion, out HTuple vernum);
                                hv_flg.Dispose();
                                hv_flg = new HTuple();
                                hv_HomMat2DIdentity.Dispose();
                                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                                hv_W = hv_ImgWidth / 2;
                                hv_H = hv_ImgHeight / 2;

                                for (hv_i = 1; hv_i <= hv_Number1; ++hv_i)
                                {
                                    HOperatorSet.SelectObj(ho_SecondVerticalRegion, out ho_ObjectSelected1, hv_i);
                                    HOperatorSet.SmallestRectangle2(ho_ObjectSelected1, out hv_Row3, out hv_Column3, out hv_Phi1, out hv_Length12, out hv_Length22);
                                    HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row3, hv_Column3, out hv_HomMat2DTranslate);
                                    HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, -hv_Phi1, hv_Row3, hv_Column3, out hv_HomMat2DRotate);
                                    HOperatorSet.AffineTransImage(ho_Image, out ho_ImageAffineTrans, hv_HomMat2DRotate, "constant", "false");
                                    HOperatorSet.AffineTransRegion(ho_ObjectSelected1, out ho_TEMPRectangle, hv_HomMat2DRotate, "nearest_neighbor");
                                    TestForKill(ho_ImageAffineTrans, ho_TEMPRectangle, out hv_OutBatFlg1);
                                    TestForKill(ho_Image, ho_ObjectSelected1, out hv_OutBatFlg2);
                                    if (hv_OutBatFlg1 != 0 || hv_OutBatFlg2 != 0)
                                    {

                                        hv_flg = hv_flg.TupleConcat(hv_i);

                                    }
                                }
                                vernum.Dispose();
                                HOperatorSet.SelectObj(ho_SecondVerticalRegion, out ho_SecondVerticalRegion, hv_flg);


                                hv_RowsVertical = (((((((hv_Rows3.TupleSelect(
                                    1)) + 3)).TupleConcat(0))).TupleConcat(((hv_RowsVertical.TupleSelect(
                                    2)) - ((hv_Rows3.TupleSelect(1)) - (hv_Rows3.TupleSelect(0)))) - 3))).TupleConcat(
                                    hv_RowsVertical.TupleSelect(3));
                                HOperatorSet.GenRectangle1(out ho_Rectangle5, hv_RowsVertical.TupleSelect(
                                    0), hv_RowsVertical.TupleSelect(1), hv_RowsVertical.TupleSelect(2), hv_RowsVertical.TupleSelect(3));
                                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle5, out ho_ImageReduced);

                            }
                        }

                    }


                }



                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 160, 255);
                ////////////////20240409
                HOperatorSet.Connection(ho_Region, out ho_Region);
                HOperatorSet.RegionFeatures(ho_Region, "area", out hv_AreaRegion);
                HOperatorSet.TupleMin(hv_AreaRegion, out hv_AreaRegion);
                HOperatorSet.Union1(ho_Region, out ho_Region);
                HOperatorSet.Difference(ho_Region, GapRegion, out ho_Region);
                HOperatorSet.Connection(ho_Region, out ho_Region);
                hv_AreaRegion = hv_AreaRegion - 1000;
                if (hv_AreaRegion < 0)
                    hv_AreaRegion = 0;
                HOperatorSet.SelectShape(ho_Region, out ho_Region, "area", "and", hv_AreaRegion, 999999);
                HOperatorSet.Union1(ho_Region, out ho_Region);


                /////////////////////////////////////////////////////////////////////////////////////////


                //********计算余料************
                HOperatorSet.ClosingRectangle1(ho_Region, out ho_RegionClosing, 11, 50);

                HOperatorSet.FillUp(ho_RegionClosing, out ho_RegionClosing);
                HOperatorSet.Difference(ho_RegionClosing, ho_Region, out ho_RegionFillUp1);
                //HOperatorSet.OpeningRectangle1(ho_RegionFillUp1, out ho_RegionFillUp1, 2, 4);
                HOperatorSet.FillUp(ho_RegionFillUp1, out ho_RegionFillUp1);
                HOperatorSet.Connection(ho_RegionFillUp1, out ho_RegionFillUp1);
                HOperatorSet.SelectShape(ho_RegionFillUp1, out ho_RegionFillUp1, ((new HTuple("area")).TupleConcat(
                   "rect2_len1")).TupleConcat("rect2_len2"), "and", ((hv_HminArea.TupleConcat(hv_HminWidth / 2))).TupleConcat(
                   hv_HminHeigth / 2), ((new HTuple(9999999)).TupleConcat(99999999)).TupleConcat(999999999));
                HOperatorSet.CountObj(ho_RegionFillUp1, out hv_Number3);
                hv_flg.Dispose();
                hv_flg = new HTuple();
                for (hv_i = 1; hv_i <= hv_Number3; ++hv_i)
                {

                    HOperatorSet.SelectObj(ho_RegionFillUp1, out ho_ObjectSelected, hv_i);
                    TestForKill(ho_Image, ho_ObjectSelected, out hv_OutBatFlg1);
                    if (hv_OutBatFlg1 != 0)
                    {
                        hv_flg = hv_flg.TupleConcat(hv_i);

                    }
                }

                HOperatorSet.SelectObj(ho_RegionFillUp1, out ho_VerticalRegion, hv_flg);

            }


            HOperatorSet.Connection(ho_UDRegion, out ho_RegionUnion);
            HOperatorSet.CountObj(ho_RegionUnion, out hv_Number);


            hv_flg.Dispose();
            hv_flg = new HTuple();
            for (hv_i = 1; hv_i <= hv_Number; ++hv_i)
            {

                HOperatorSet.SelectObj(ho_RegionUnion, out ho_ObjectSelected, hv_i);
                TestForKill(ho_Image, ho_ObjectSelected, out hv_OutBatFlg1);
                if (hv_OutBatFlg1 != 0)
                {
                    hv_flg = hv_flg.TupleConcat(hv_i);

                }
            }

            HOperatorSet.SelectObj(ho_RegionUnion, out ho_UDRegion, hv_flg);



            HOperatorSet.Union2(ho_VerticalRegion, ho_SecondVerticalRegion, out ho_RegionUnion1);
            HOperatorSet.Union2(ho_HoleRegion, ho_UDRRegion, out ho_RegionUnion2);
            HOperatorSet.Union2(ho_UDRegion, ho_RegionUnion2, out ho_RegionUnion);
            HOperatorSet.Union2(ho_RegionUnion, ho_RegionUnion1, out ho_RegionUnion1);
            ////////判断R角的余料
            HOperatorSet.Connection(ho_RegionUnion1, out ho_RegionUnion2);
            HOperatorSet.CountObj(ho_RegionUnion2, out hv_Number);
            if (hv_Number > 0)
            {
                //////////////////极耳位置尺寸信息
                HOperatorSet.BinaryThreshold(ho_Image, out ho_Region, "max_separability", "dark", out hv_Phi);
                HOperatorSet.Connection(ho_Region, out ho_Region);
                HOperatorSet.SelectShapeStd(ho_Region, out ho_Region, "max_area", 70);
                HOperatorSet.SmallestRectangle1(ho_Region, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                HOperatorSet.SmallestRectangle2(ho_Region, out hv_Row, out hv_Column, out hv_Phi, out hv_Rect1, out hv_Rect2);
                ////////////////////筛选出余料
                HOperatorSet.SelectShape(ho_RegionUnion2, out ho_Region1, "column", "and", hv_Column2 - 10, 999999);
                HOperatorSet.CountObj(ho_Region1, out hv_Number);
                if (hv_Number > 0)
                {
                    HObject gapRegion, gapRect, XLDgap, gapImage, gapdarkRegion, onegapXLD;
                    HOperatorSet.GenEmptyObj(out gapRegion); HOperatorSet.GenEmptyObj(out gapRect); HOperatorSet.GenEmptyObj(out XLDgap); HOperatorSet.GenEmptyObj(out gapImage);
                    HOperatorSet.GenEmptyObj(out gapdarkRegion); HOperatorSet.GenEmptyObj(out onegapXLD);

                    HTuple hv_garRow1 = new HTuple(), hv_gapColumn1 = new HTuple(), hv_gapRow2 = new HTuple(), hv_gapColumn2 = new HTuple();
                    HTuple gapRectArea = new HTuple(), gapdarkRegionArea = new HTuple();
                    HTuple gapcenterR = new HTuple(), gapcenterC = new HTuple();

                    hv_flg = new HTuple();
                    for (int i = 1; i <= hv_Number; ++i)
                    {
                        HOperatorSet.SelectObj(ho_Region1, out gapRegion, i);
                        HOperatorSet.SmallestRectangle1(gapRegion, out hv_garRow1, out hv_gapColumn1, out hv_gapRow2, out hv_gapColumn2);
                        //////////////计算余料的左边区域是否全部黑色，表示在顶部
                        HOperatorSet.GenRectangle1(out gapRect, hv_garRow1, hv_gapColumn1 - 50, hv_gapRow2, hv_gapColumn1);
                        HOperatorSet.ReduceDomain(ho_Image, gapRect, out gapImage);
                        HOperatorSet.BinaryThreshold(gapImage, out gapdarkRegion, "max_separability", "dark", out hv_Phi);
                        HOperatorSet.RegionFeatures(gapRect, "area", out gapRectArea);
                        HOperatorSet.RegionFeatures(gapdarkRegion, "area", out gapdarkRegionArea);
                        if (gapdarkRegionArea.D / gapRectArea.D > 0.9)
                        {


                            int isdelet = 0;
                            HOperatorSet.SmallestRectangle2(gapRect, out gapcenterR, out gapcenterC, out hv_Phi, out hv_Rect1, out hv_Rect2);
                            //////根据上还是下的位置来判断，对应余料上左或下左边缘有一条垂直直线
                            if (gapcenterR > hv_Row1 && gapcenterR <= hv_Row1 + 55)
                            {


                                HOperatorSet.GenRectangle1(out gapRect, hv_garRow1 - 10, hv_gapColumn1 - 50, hv_garRow1, hv_gapColumn1);
                                HOperatorSet.ReduceDomain(ho_Image, gapRect, out gapImage);
                                HOperatorSet.ThresholdSubPix(gapImage, out XLDgap, 125);
                                HOperatorSet.SegmentContoursXld(XLDgap, out XLDgap, "lines_circles", 1, 1, 1);
                                HOperatorSet.SortContoursXld(XLDgap, out XLDgap, "character", "true", "column");
                                HOperatorSet.CountObj(XLDgap, out hv_Number1);
                                if (hv_Number1 > 0)
                                {
                                    HOperatorSet.SelectObj(XLDgap, out onegapXLD, hv_Number1);
                                    HOperatorSet.GenRegionContourXld(onegapXLD, out ho_Region2, "filled");
                                    HOperatorSet.Skeleton(ho_Region2, out ho_Skeleton);
                                    HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                                    HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_DRows2, out hv_DColumns2);
                                    HOperatorSet.TupleSort(hv_DColumns2, out hv_DColumns2);
                                    HOperatorSet.TupleSort(hv_DRows2, out hv_DRows2);
                                    HOperatorSet.LengthXld(onegapXLD, out hv_Nr1);
                                    if (new HTuple(hv_DColumns2[0] - hv_DColumns2[1]).TupleAbs() >= 3 && hv_Nr1 <= 12 && new HTuple(hv_DRows2[0] - hv_DRows2[1]).TupleAbs() / 10 >= 0.9)
                                    {

                                    }
                                    else
                                    {
                                        isdelet = 1;
                                    }

                                }

                            }
                            else if (gapcenterR < hv_Row2 && gapcenterR >= hv_Row2 - 55)
                            {

                                HOperatorSet.GenRectangle1(out gapRect, hv_gapRow2, hv_gapColumn1 - 50, hv_gapRow2 + 10, hv_gapColumn1);
                                HOperatorSet.ReduceDomain(ho_Image, gapRect, out gapImage);
                                HOperatorSet.ThresholdSubPix(gapImage, out XLDgap, 125);
                                HOperatorSet.SegmentContoursXld(XLDgap, out XLDgap, "lines_circles", 1, 1, 1);
                                HOperatorSet.SortContoursXld(XLDgap, out XLDgap, "character", "true", "column");
                                HOperatorSet.CountObj(XLDgap, out hv_Number1);
                                if (hv_Number1 > 0)
                                {
                                    HOperatorSet.SelectObj(XLDgap, out onegapXLD, 1);
                                    HOperatorSet.GenRegionContourXld(onegapXLD, out ho_Region2, "filled");
                                    HOperatorSet.Skeleton(ho_Region2, out ho_Skeleton);
                                    HOperatorSet.JunctionsSkeleton(ho_Skeleton, out ho_EndPoints, out ho_JuncPoints);
                                    HOperatorSet.GetRegionPoints(ho_EndPoints, out hv_DRows2, out hv_DColumns2);
                                    HOperatorSet.TupleSort(hv_DColumns2, out hv_DColumns2);
                                    HOperatorSet.TupleSort(hv_DRows2, out hv_DRows2);
                                    HOperatorSet.LengthXld(onegapXLD, out hv_Nr1);
                                    if (new HTuple(hv_DColumns2[0] - hv_DColumns2[1]).TupleAbs() >= 3 && hv_Nr1 <= 12 && new HTuple(hv_DRows2[0] - hv_DRows2[1]).TupleAbs() / 10 >= 0.9)
                                    {

                                    }
                                    else
                                    {
                                        isdelet = 1;
                                    }

                                }

                            }
                            if (isdelet == 1)
                            {
                                hv_flg = hv_flg.TupleConcat(i);
                            }

                        }
                    }
                    HOperatorSet.Union1(ho_Region1, out ho_Region2);
                    HOperatorSet.Difference(ho_RegionUnion1, ho_Region2, out ho_Region2);

                    HOperatorSet.RemoveObj(ho_Region1, out ho_Region1, hv_flg);
                    HOperatorSet.Union1(ho_Region1, out ho_Region1);
                    HOperatorSet.Union2(ho_Region2, ho_Region1, out ho_FlawRegion);
                    gapRegion.Dispose(); gapRect.Dispose(); XLDgap.Dispose(); gapImage.Dispose(); gapdarkRegion.Dispose(); onegapXLD.Dispose();
                    hv_garRow1.Dispose(); hv_gapColumn1.Dispose(); hv_gapRow2.Dispose(); hv_gapColumn2.Dispose();
                    gapRectArea.Dispose(); gapdarkRegionArea.Dispose();
                    gapcenterR.Dispose(); gapcenterC.Dispose();

                }
                else
                    ho_FlawRegion = new HObject(ho_RegionUnion1);


            }
            else
                ho_FlawRegion = new HObject();


            hv_AreaRegion.Dispose();
            ho_GapRegion.Dispose();
            ho_HoleRegion.Dispose();
            ho_Region.Dispose();
            ho_RegionFillUp.Dispose();
            ho_Border.Dispose();
            ho_SelectedXLD.Dispose();
            ho_ContoursSplit.Dispose();
            ho_SelectedXLD1.Dispose();
            ho_Region2.Dispose();
            ho_ObjectSelected.Dispose();
            ho_Region1.Dispose();
            ho_ObjectSelected2.Dispose();
            ho_RegionIntersection.Dispose();
            ho_RegionUnion.Dispose();
            ho_Rectangle.Dispose();
            ho_ImageReduced.Dispose();
            ho_UDRegion.Dispose();
            ho_ObjectSelected1.Dispose();
            ho_Skeleton.Dispose();
            ho_EndPoints.Dispose();
            ho_JuncPoints.Dispose();
            ho_RegionClosing.Dispose();
            ho_Contours.Dispose();
            ho_ContoursDifference.Dispose();
            ho_Region3.Dispose();
            ho_SelectedRegions.Dispose();
            ho_RegionUnion1.Dispose();
            ho_Rectangle3.Dispose();
            ho_RegionDilation.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_ObjectSelected3.Dispose();
            ho_Rectangle1.Dispose();
            ho_Rectangle2.Dispose();
            ho_CircleRegion.Dispose();
            ho_ImageAffineTrans.Dispose();
            ho_TEMPRectangle.Dispose();
            ho_UDRRegion.Dispose();
            ho_VerticalRegion.Dispose();
            ho_SecondVerticalRegion.Dispose();
            ho_Border1.Dispose();
            ho_ContoursSplit1.Dispose();
            ho_SortedContours.Dispose();
            ho_RegionUnion2.Dispose();
            ho_Region5.Dispose();
            ho_Rectangle5.Dispose();
            ho_RegionClosing1.Dispose();
            ho_RegionDifference.Dispose();
            ho_SelectedRegions1.Dispose();
            ho_Rectangle4.Dispose();
            ho_RegionDifference1.Dispose();
            ho_ImageReduced1.Dispose();
            ho_Region4.Dispose();
            ho_ObjectsReduced.Dispose();
            ho_RegionFillUp1.Dispose();
            ho_RegionDilation1.Dispose();
            ho_ObjectSelected4.Dispose();
            ho_UDVerticalRegion.Dispose();

            hv_ImgWidth.Dispose();
            hv_ImgHeight.Dispose();
            hv_Row13.Dispose();
            hv_Column13.Dispose();
            hv_flg.Dispose();
            hv_i.Dispose();
            hv_Number.Dispose();
            hv_Value.Dispose();
            hv_Number1.Dispose();
            hv_Row2.Dispose();
            hv_Column2.Dispose();
            hv_Phi1.Dispose();
            hv_Length11.Dispose();
            hv_Length21.Dispose();
            hv_Attrib1.Dispose();
            hv_Attrib2.Dispose();
            hv_Length1.Dispose();
            hv_Length2.Dispose();
            hv_Attrib.Dispose();
            hv_upRows1.Dispose();
            hv_upColumns1.Dispose();
            hv_DRows2.Dispose();
            hv_DColumns2.Dispose();
            hv_Columns.Dispose();
            hv_Row1.Dispose();
            hv_Column1.Dispose();
            hv_Row21.Dispose();
            hv_Column21.Dispose();
            hv_Value4.Dispose();
            hv_Row12.Dispose();
            hv_Column12.Dispose();
            hv_Row23.Dispose();
            hv_Column23.Dispose();
            hv_RowsVertical.Dispose();
            hv_Rect1.Dispose();
            hv_Rect2.Dispose();
            hv_Rows1.Dispose();
            hv_Columns1.Dispose();
            hv_Rows2.Dispose();
            hv_Columns2.Dispose();
            hv_IsNr.Dispose();
            hv_Nr1.Dispose();
            hv_Nr2.Dispose();
            hv_RowBegin.Dispose();
            hv_ColBegin.Dispose();
            hv_RowEnd.Dispose();
            hv_ColEnd.Dispose();
            hv_Nc.Dispose();
            hv_Dist.Dispose();
            hv_Attrib3.Dispose();
            hv_Length3.Dispose();
            hv_Rows3.Dispose();
            hv_Columns3.Dispose();
            hv_Row11.Dispose();
            hv_Column11.Dispose();
            hv_Row22.Dispose();
            hv_Column22.Dispose();
            hv_HomMat2DIdentity.Dispose();
            hv_W.Dispose();
            hv_H.Dispose();
            hv_Row3.Dispose();
            hv_Column3.Dispose();
            hv_Length12.Dispose();
            hv_Length22.Dispose();
            hv_HomMat2DTranslate.Dispose();
            hv_HomMat2DRotate.Dispose();
            hv_OutBatFlg1.Dispose();
            hv_OutBatFlg2.Dispose();
            hv_Number2.Dispose();
            hv_Length.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Phi.Dispose();
            hv_Value1.Dispose();
            hv_Value2.Dispose();
            hv_Number3.Dispose();
            hv_Value3.Dispose();
            hv_Rows4.Dispose();
            hv_Columns4.Dispose();
            return;









        }

        public  void EntryFlawR(HObject ho_Image, HObject GapRegion, HTuple Image_H, out HObject ho_FlawRegionR, HTuple hv_HyminArea, HTuple hv_HyminWidth, HTuple hv_HyminHeigth)
        {
           // HOperatorSet.CountSeconds(out HTuple s1);
           // HOperatorSet.WriteImage(ho_Image, "jpeg", -1, "E:/极耳/极耳余料2/" + s1 + ".jpg");
            //******************************************

            //*****************************************
            HObject ho_Region, ho_ConnectedRegions3, ho_EmptyObjectR;
            HObject ho_ObjectSelectedR = null, ho_RegionClosingR = null;
            HObject ho_RegionDifference1R = null, ho_ConnectedRegions2R = null;

            // Local control variables 

            HTuple hv_Number = new HTuple(), hv_i = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_FlawRegionR);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_EmptyObjectR);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelectedR);
            HOperatorSet.GenEmptyObj(out ho_RegionClosingR);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference1R);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2R);
            //**************计算缺陷*****************************

            HOperatorSet.GenEmptyRegion(out ho_FlawRegionR);

            HOperatorSet.Threshold(ho_Image, out ho_Region, 200, 255);
            HOperatorSet.FillUp(ho_Region, out ho_Region);
            HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions3);
            HOperatorSet.CountObj(ho_ConnectedRegions3, out hv_Number);
            HOperatorSet.SortRegion(ho_ConnectedRegions3,out ho_ConnectedRegions3, "upper_left", "true", "row");
            HOperatorSet.GenEmptyObj(out ho_EmptyObjectR);
            for (hv_i = 1; hv_i <= hv_Number; ++hv_i)
            {

                HOperatorSet.SelectObj(ho_ConnectedRegions3, out ho_ObjectSelectedR, hv_i);


                ///删除有缺口的孔
                HOperatorSet.Difference(ho_ObjectSelectedR, GapRegion,out ho_ObjectSelectedR);
                HOperatorSet.Connection(ho_ObjectSelectedR,out ho_ObjectSelectedR);
                HOperatorSet.SelectShapeStd(ho_ObjectSelectedR, out ho_ObjectSelectedR, "max_area",70);

                ////计算是否有mark孔
                HOperatorSet.SmallestRectangle1(ho_ObjectSelectedR,out HTuple hr1, out HTuple hc1, out HTuple hr2, out HTuple hc2);
                HOperatorSet.GenRectangle1(out HObject h0_Rect,  hr1,   hc1,   hr2,   hc2);
                HOperatorSet.Difference(h0_Rect, ho_ObjectSelectedR,out HObject ho_diff);
                HOperatorSet.OpeningRectangle1(ho_diff,out  ho_diff,10,10);
                HOperatorSet.Connection(ho_diff, out ho_diff);
                HOperatorSet.SelectShapeStd(ho_diff,out ho_diff, "max_area",70);
                HOperatorSet.RegionFeatures(ho_diff,"area",out HTuple hv_area1);
                HOperatorSet.RegionFeatures(h0_Rect, "area", out HTuple hv_area2);
                if(hv_area1.D/ hv_area2.D>0.1)
                {
                    HOperatorSet.SmallestRectangle1(ho_diff, out  hr1, out  hc1, out  hr2, out  hc2);
                    if (hv_i == 1)
                        hr1 = 10;
                    else
                        hr2 = Image_H - 10;
                    HOperatorSet.GenRectangle1(out  h0_Rect, hr1, hc1, hr2, hc2-5);
                    HOperatorSet.ReduceDomain(ho_Image, h0_Rect,out ho_diff);
                    HOperatorSet.Threshold(ho_diff,out ho_ObjectSelectedR,200,255);
                }
                hr1.Dispose();
                hc1.Dispose();
                hr2.Dispose();
                hc2.Dispose();
                h0_Rect.Dispose();
                ho_diff.Dispose();
                hv_area1.Dispose();
                hv_area2.Dispose();
                HOperatorSet.ClosingCircle(ho_ObjectSelectedR, out ho_RegionClosingR, 75);
                HOperatorSet.Difference(ho_RegionClosingR, ho_Region, out ho_RegionDifference1R);
                HOperatorSet.Connection(ho_RegionDifference1R, out ho_ConnectedRegions2R);




                HOperatorSet.ConcatObj(ho_EmptyObjectR, ho_ConnectedRegions2R, out ho_EmptyObjectR);


            }
            HOperatorSet.SelectShape(ho_EmptyObjectR, out ho_EmptyObjectR, ((new HTuple("area")).TupleConcat("rect2_len1")).TupleConcat("rect2_len2"), "and",
                                                                           ((new HTuple(hv_HyminArea)).TupleConcat(hv_HyminHeigth/2)).TupleConcat(hv_HyminWidth/2),
                                                                           ((new HTuple(1000000)).TupleConcat(1000000)).TupleConcat(1000000));



            
            HOperatorSet.Union1(ho_EmptyObjectR, out ho_EmptyObjectR);
            ////////////////////////输出结果
            ho_FlawRegionR = new HObject(ho_EmptyObjectR);

           

            ho_Region.Dispose();
            ho_ConnectedRegions3.Dispose();
            ho_EmptyObjectR.Dispose();
            ho_ObjectSelectedR.Dispose();
            ho_RegionClosingR.Dispose();
            ho_RegionDifference1R.Dispose();
            ho_ConnectedRegions2R.Dispose();

            hv_Number.Dispose();
            hv_i.Dispose();

            



            return;
        }

        public  void TestForKill(HObject ho_Image, HObject ho_BatteyRegion, out HTuple hv_OutBatFlg)
        {



            // Local iconic variables 

            HObject ho_RegionUnion = null, ho_Regiondila;
            HObject ho_Rectangle, ho_ImageReduced, ho_Border1, ho_Border;
            HObject ho_RegionLines = null, ho_ContoursSplit = null;

            // Local control variables 

            HTuple hv_WHValue = new HTuple(), hv_Row13 = new HTuple();
            HTuple hv_Column13 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Length = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_RigthLine = new HTuple();
            HTuple hv_DownLine = new HTuple(), hv_index = new HTuple();
            HTuple hv_m = new HTuple(), hv_row = new HTuple(), hv_column = new HTuple();
            HTuple hv_IsOverlapping = new HTuple(), hv_Number = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_Regiondila);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Border1);
            HOperatorSet.GenEmptyObj(out ho_Border);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            hv_OutBatFlg = new HTuple();
            //******************检测是否过杀*********************

            //*输出缺陷区域
            //*****************************************************


            ho_RegionUnion = new HObject(ho_BatteyRegion);
            hv_OutBatFlg = 0;
            try
            {
                HOperatorSet.RegionFeatures(ho_RegionUnion, (new HTuple("width")).TupleConcat("height"), out hv_WHValue);
                if (hv_WHValue.TupleSelect(0) <= 3 || hv_WHValue.TupleSelect(1) <= 3 || hv_WHValue.TupleSelect(0) / hv_WHValue.TupleSelect(1) < 0.2)
                {


                    ho_RegionUnion.Dispose();
                    ho_Regiondila.Dispose();
                    ho_Rectangle.Dispose();
                    ho_ImageReduced.Dispose();
                    ho_Border1.Dispose();
                    ho_Border.Dispose();
                    ho_RegionLines.Dispose();
                    ho_ContoursSplit.Dispose();

                    hv_WHValue.Dispose();
                    hv_Row13.Dispose();
                    hv_Column13.Dispose();
                    hv_Row2.Dispose();
                    hv_Column2.Dispose();
                    hv_Length.Dispose();
                    hv_Indices.Dispose();
                    hv_RigthLine.Dispose();
                    hv_DownLine.Dispose();
                    hv_index.Dispose();
                    hv_m.Dispose();
                    hv_row.Dispose();
                    hv_column.Dispose();
                    hv_IsOverlapping.Dispose();
                    hv_Number.Dispose();

                    return;
                }

                HOperatorSet.DilationRectangle1(ho_RegionUnion, out ho_Regiondila, 9, 9);
                HOperatorSet.SmallestRectangle1(ho_Regiondila, out hv_Row13, out hv_Column13, out hv_Row2, out hv_Column2);
                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row13, hv_Column13, hv_Row2, hv_Column2);
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);
                //HOperatorSet.ThresholdSubPix(ho_ImageReduced, out ho_Border1, 145);
                HOperatorSet.BinaryThreshold(ho_ImageReduced, out ho_Border1, "max_separability", "dark",out HTuple UsedThreshold1);
                HOperatorSet.ThresholdSubPix(ho_ImageReduced, out ho_Border1, UsedThreshold1);
                UsedThreshold1.Dispose();
                HOperatorSet.LengthXld(ho_Border1, out hv_Length);
                HOperatorSet.TupleSortIndex(hv_Length, out hv_Indices);
                HOperatorSet.SelectObj(ho_Border1, out ho_Border, (hv_Indices.TupleSelect((hv_Indices.TupleLength() - 1)) + 1));

                hv_RigthLine = hv_RigthLine.TupleConcat(hv_Row13, hv_Column13, hv_Row2, hv_Column13);
                hv_DownLine = hv_DownLine.TupleConcat(hv_Row13, hv_Column13, hv_Row13, hv_Column2);

                hv_index = 0;
                hv_m = 0;
                while (hv_m <= hv_Column2 + 1)
                {

                    HOperatorSet.GenRegionLine(out ho_RegionLines, hv_RigthLine.TupleSelect(0), hv_RigthLine.TupleSelect(1), hv_RigthLine.TupleSelect(2), hv_RigthLine.TupleSelect(3));
                    HOperatorSet.IntersectionLineContourXld(ho_Border, hv_RigthLine.TupleSelect(0), hv_RigthLine.TupleSelect(1), hv_RigthLine.TupleSelect(2), hv_RigthLine.TupleSelect(3), out hv_row, out hv_column, out hv_IsOverlapping);

                    if (hv_row.TupleLength() > 1)
                    {

                        hv_index = hv_index + 1;

                    }

                    hv_m = hv_RigthLine.TupleSelect(1) + 1;
                    hv_RigthLine[1] = hv_m;
                    hv_RigthLine[3] = hv_m;
                }
                if (hv_WHValue[0] <= 7)
                {
                    if (hv_index / hv_WHValue.TupleSelect(0) >= 0.8)
                    {
                        HOperatorSet.SegmentContoursXld(ho_Border, out ho_ContoursSplit, "lines_circles", 1, 2, 1);
                        HOperatorSet.CountObj(ho_Border, out hv_Number);
                        if (hv_Number > 1)
                        {
                            hv_OutBatFlg = 1;
                            return;
                        }
                    }
                }
                else
                {
                    if (hv_index / hv_WHValue.TupleSelect(0) >= 0.5)
                    {
                        hv_OutBatFlg = 1;

                        return;


                    }
                }



                hv_m = 0;
                hv_index = 0;
                while (hv_m <= hv_Row2 + 1)
                {

                    //HOperatorSet.GenRegionLine(out ho_RegionLines, hv_DownLine[0], hv_DownLine[1], hv_DownLine[2], hv_DownLine[3]);
                    HOperatorSet.IntersectionLineContourXld(ho_Border, hv_DownLine[0], hv_DownLine[1], hv_DownLine[2], hv_DownLine[3], out hv_row, out hv_column, out hv_IsOverlapping);

                    if (hv_column.TupleLength() > 1)
                    {

                        hv_index = hv_index + 1;


                    }

                    hv_m = hv_DownLine[0] + 1;
                    hv_DownLine[0] = hv_m;
                    hv_DownLine[2] = hv_m;
                }
                if (hv_WHValue[1] <= 7)
                {
                    if (hv_index / hv_WHValue[1] >= 0.8)

                    {
                        HOperatorSet.SegmentContoursXld(ho_Border, out ho_ContoursSplit, "lines_circles", 1, 2, 1);
                        HOperatorSet.CountObj(ho_ContoursSplit, out hv_Number);
                        if (hv_Number > 1)
                        {
                            hv_OutBatFlg = 1;
                        }

                    }
                }
                else
                {
                    if (hv_index / hv_WHValue[1] >= 0.5)
                    {
                        hv_OutBatFlg = 1;
                    }
                }
            }
            catch (Exception)
            {

            }

            finally
            {
                ho_RegionUnion.Dispose();
                ho_Regiondila.Dispose();
                ho_Rectangle.Dispose();
                ho_ImageReduced.Dispose();
                ho_Border1.Dispose();
                ho_Border.Dispose();
                ho_RegionLines.Dispose();
                ho_ContoursSplit.Dispose();

                hv_WHValue.Dispose();
                hv_Row13.Dispose();
                hv_Column13.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_Length.Dispose();
                hv_Indices.Dispose();
                hv_RigthLine.Dispose();
                hv_DownLine.Dispose();
                hv_index.Dispose();
                hv_m.Dispose();
                hv_row.Dispose();
                hv_column.Dispose();
                hv_IsOverlapping.Dispose();
                hv_Number.Dispose();


            }
        }


    }
}
