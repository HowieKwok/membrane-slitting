using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Frame_Core;
using HalconDotNet;

namespace Plugin.DaMianBurr
{
    internal class DaMianBurrAlgorithm
    {
     

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

        public  void FindDaMianMaoCi(HObject Image, double XCali, double YCali, double AreaCali, ParamsDaMianBurr parameter,
         out List<NgTypeInfo> NgInfo, out HObject outputContour, out HObject outputRectangle,
         out HObject outputMaoCiRegions, out HTuple outputPoint, out HTuple outputMaoCiNumber, out double DaMianMaoCiHeight)
        {

            NgInfo = new List<NgTypeInfo>();
            HObject imagePart;
            HOperatorSet.GenEmptyObj(out imagePart);
            DaMianMaoCiHeight = 0;
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_inputRectangle, ho_inputReion, ho_Contour;
            HObject ho_RegionUnion, ho_ImageReduced, ho_Region, ho_RegionFillUp;
            HObject ho_ConnectedRegions, ho_SelectedRegions;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_OutPointRow1 = new HTuple(), hv_OutPointCol1 = new HTuple();
            HTuple hv_Count = new HTuple(), hv_RowBegin = new HTuple();
            HTuple hv_ColBegin = new HTuple(), hv_RowEnd = new HTuple();
            HTuple hv_ColEnd = new HTuple(), hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_row1 = new HTuple();
            HTuple hv_row2 = new HTuple(), hv_col1 = new HTuple();
            HTuple hv_col2 = new HTuple(), hv_row21 = new HTuple();
            HTuple hv_row22 = new HTuple(), hv_col21 = new HTuple();
            HTuple hv_col22 = new HTuple(), hv_row31 = new HTuple();
            HTuple hv_row32 = new HTuple(), hv_col31 = new HTuple();
            HTuple hv_col32 = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out outputContour);
            HOperatorSet.GenEmptyObj(out outputRectangle);
            HOperatorSet.GenEmptyObj(out outputMaoCiRegions);
            HOperatorSet.GenEmptyObj(out ho_inputRectangle);
            HOperatorSet.GenEmptyObj(out ho_inputReion);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            outputPoint = 0;
            outputMaoCiNumber = 1;

            HObject ho_ObjectSelected;
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            try
            {
              
                //延长边线
                hv_ImageWidth.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(Image, out hv_ImageWidth, out hv_Height);
                ho_inputRectangle.Dispose();
                HOperatorSet.Threshold(Image, out ho_inputRectangle, parameter.inputMinThreshold,
                    parameter.inputMaxThreshold);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Connection(ho_inputRectangle, out ExpTmpOutVar_0);
                    ho_inputRectangle.Dispose();
                    ho_inputRectangle = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.SelectShapeStd(ho_inputRectangle, out ExpTmpOutVar_0, "max_area",
                        70);
                    ho_inputRectangle.Dispose();
                    ho_inputRectangle = ExpTmpOutVar_0;
                }
                hv_Row11.Dispose(); hv_Column11.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                HOperatorSet.SmallestRectangle1(ho_inputRectangle, out hv_Row11, out hv_Column11,
                    out hv_Row2, out hv_Column2);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_inputReion.Dispose();
                    double Height = 0;
                    if (hv_Row11 - parameter.inputFindLineHeight < 0)
                    {

                        Height = 0;
                    }
                    else
                    {
                        Height = hv_Row11 - parameter.inputFindLineHeight;
                    }
                    HOperatorSet.GenRectangle1(out ho_inputReion, Height,
                        (hv_ImageWidth / 2) - parameter.inputFindLineWidth, hv_Row11 + parameter.inputFindLineHeight,
                        (hv_ImageWidth / 2) + parameter.inputFindLineWidth);
                }

                //inputXorY:沿Y轴搜索,或者X轴
                //inputInterval:间隔
                //inputDirection:方向 L2R_New从左到右,U2B_New从上到下
                //inputRoiWidthLen:卡尺ROI长度
                //inputSigma:平滑系数
                //inputAmplitudeThreshold:阈值
                //inputTransition:所以’negative‘从亮到暗，‘positive’从暗到亮，‘all’。
                //inputPosition:'first'第一点,'last'最后一个,'all' 所有的边缘

                /*  outputContour.Dispose();*/
                hv_OutPointRow1.Dispose(); hv_OutPointCol1.Dispose();
                Find_Line_ForRectangle(Image, ho_inputReion, out outputContour, "X", 30,
                    "U2B_New", 10, 1, 10, "negative", "first", out hv_OutPointRow1, out hv_OutPointCol1);

                hv_Count.Dispose();
                HOperatorSet.CountObj(outputContour, out hv_Count);
                if ((int)(new HTuple(hv_Count.TupleEqual(0))) != 0)
                {
                    if ((int)(new HTuple((new HTuple(hv_OutPointRow1.TupleLength())).TupleGreater(
                        3))) != 0)
                    {
                        //outputContour.Dispose();
                        HOperatorSet.GenContourPolygonXld(out outputContour, hv_OutPointRow1,
                            hv_OutPointCol1);
                    }
                    else
                    {

                    }
                }

                hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                HOperatorSet.FitLineContourXld(outputContour, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc, out hv_Dist);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Contour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_RowBegin.TupleConcat(hv_RowEnd),
                        hv_ColBegin.TupleConcat(hv_ColEnd));
                }
                //
                //if (|RowBegin|==1 and |RowEnd|==1)
                outputPoint.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    outputPoint = (hv_RowBegin + hv_RowEnd) / 2;
                }
                //else
                //outputPoint := 0
                //endif

                //特殊情况水平线
                //y=kx+b
                ho_Contour.Dispose(); hv_row1.Dispose(); hv_row2.Dispose(); hv_col1.Dispose(); hv_col2.Dispose();
                genLengthenLine(out ho_Contour, hv_ImageWidth, hv_Height, hv_RowBegin, hv_ColBegin,
                    hv_RowEnd, hv_ColEnd, "D", out hv_row1, out hv_row2, out hv_col1, out hv_col2);

                HOperatorSet.GenRegionContourXld(ho_Contour, out ho_Contour, "filled");
                outputContour = ho_Contour.CopyObj(1, 1);
                hv_row21.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_row21 = hv_row1 - parameter.inputFindMaoCiUP1;
                }
                hv_row22.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_row22 = hv_row2 - parameter.inputFindMaoCiUP1;
                }
                hv_col21.Dispose();
                hv_col21 = new HTuple(hv_col1);
                hv_col22.Dispose();
                hv_col22 = new HTuple(hv_col2);

                hv_row31.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_row31 = hv_row1 - parameter.inputFindMaoCiUP2;
                }
                hv_row32.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_row32 = hv_row2 - parameter.inputFindMaoCiUP2;
                }
                hv_col31.Dispose();
                hv_col31 = new HTuple(hv_col1);
                hv_col32.Dispose();
                hv_col32 = new HTuple(hv_col2);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_RegionUnion.Dispose();
                    HOperatorSet.GenRegionPolygon(out ho_RegionUnion, ((((hv_row21.TupleConcat(hv_row22))).TupleConcat(
                        hv_row31))).TupleConcat(hv_row32), ((((hv_col21.TupleConcat(hv_col22))).TupleConcat(
                        hv_col31))).TupleConcat(hv_col32));
                }

                hv_Row1.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                HOperatorSet.SmallestRectangle2(ho_RegionUnion, out hv_Row1, out hv_Column, out hv_Phi,
                    out hv_Length1, out hv_Length2);
                outputRectangle.Dispose();
                HOperatorSet.GenRectangle2(out outputRectangle, hv_Row1, hv_Column, hv_Phi,
                    hv_Length1, hv_Length2);

                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(Image, outputRectangle, out ho_ImageReduced);

                //阀值提取涂布区域
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, parameter.inputMaoCiMinThreshold,
                    parameter.inputMaoCiMaxThreshold);
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_Region, out ho_RegionFillUp);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", parameter.inputMinArea_DaMian, parameter.inputMaxArea);

                outputMaoCiNumber.Dispose();
                HOperatorSet.CountObj(ho_SelectedRegions, out outputMaoCiNumber);

              
                HTuple hv_j = 1;


                while ((int)(new HTuple(hv_j.TupleLessEqual(outputMaoCiNumber))) != 0)
                {
                    //遍历区域
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedRegions, out ho_ObjectSelected, hv_j);

                    HTuple MinY, MinX, MaxY, MaxX;
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out MinY, out MinX, out MaxY, out MaxX);

                    double tempHeight = outputPoint.D - MinY.TupleMin().D;


                    HTuple area, row, column;
                    HOperatorSet.AreaCenter(ho_ObjectSelected, out area, out row, out column);


                    HOperatorSet.Circularity(ho_ObjectSelected, out HTuple Circularity);
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    double row11 = row1.D - 50 > 0 ? row1.D - 50 : row1.D;
                    double row22 = row2.D + 50 > parameter.ImageInfo.imageHeight ? row2.D : row2.D + 50;

                    double col11 = column1.D - 50 > 0 ? column1.D - 50 : column1.D;
                    double col22 = column2.D + 50 > parameter.ImageInfo.imageWidth ? column2.D : column2.D + 50;






                    //不等于  有交点

                    if (tempHeight > parameter.DaMianMaoCiMinHeight)
                    {

                        hv_j++;
                        if (tempHeight > DaMianMaoCiHeight)
                        {
                            DaMianMaoCiHeight = tempHeight;
                        }

                        HOperatorSet.CropRectangle1(Image, out imagePart, row11, col11, row22, col22);
                        NgTypeInfo testNgInfo = new NgTypeInfo();

                        testNgInfo.NgMidRow = row;
                        testNgInfo.NgMidColumn = column;
                        testNgInfo.m_fNgTypWidth = (MaxX.D - MinX.D) * XCali;
                        testNgInfo.m_fNgTypArea = area.D * AreaCali;
                        testNgInfo.m_hImage = imagePart;
                        testNgInfo.m_hRegion = ho_ObjectSelected.CopyObj(1,1);
                        testNgInfo.m_nImgIndex = parameter.ImageInfo.image_ID;
                        testNgInfo.m_fNgTypHeight = tempHeight * YCali;
                        testNgInfo.m_strNgTypeName = Defct1Enum.大面A类毛刺;
                        NgInfo.Add(testNgInfo);

                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.RemoveObj(ho_SelectedRegions, out ExpTmpOutVar_0, hv_j);
                            ho_SelectedRegions.Dispose();
                            ho_SelectedRegions = ExpTmpOutVar_0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Number = outputMaoCiNumber - 1;
                                outputMaoCiNumber.Dispose();
                                outputMaoCiNumber = ExpTmpLocalVar_Number;
                            }
                        }
                    }

                }
                HOperatorSet.CountObj(ho_SelectedRegions, out outputMaoCiNumber);
                outputMaoCiRegions.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions, out outputMaoCiRegions);

            }
            catch(Exception)
            {
                outputPoint = 0;
                NgTypeInfo testNgInfo = new NgTypeInfo();
                testNgInfo.NgMidRow = 100;
                testNgInfo.NgMidColumn = 100;
                testNgInfo.m_fNgTypWidth = 9999 * XCali;
                testNgInfo.m_fNgTypArea = 9999 * AreaCali;
                testNgInfo.m_hImage = Image;
                HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
                testNgInfo.m_hRegion = ho_ObjectSelected.CopyObj(1,1);
                testNgInfo.m_nImgIndex = parameter.ImageInfo.image_ID;
                testNgInfo.m_fNgTypHeight = 9999;
                testNgInfo.m_strNgTypeName = Defct1Enum.大面A类毛刺;
                NgInfo.Add(testNgInfo);
            }
            finally
            {
                ho_ObjectSelected?.Dispose();
                ho_inputRectangle.Dispose();
                ho_inputReion.Dispose();
                ho_Contour.Dispose();
                ho_RegionUnion.Dispose();
                ho_ImageReduced.Dispose();
                ho_Region.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();

                hv_ImageWidth.Dispose();
                hv_Height.Dispose();
                hv_Row11.Dispose();
                hv_Column11.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_OutPointRow1.Dispose();
                hv_OutPointCol1.Dispose();
                hv_Count.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_row1.Dispose();
                hv_row2.Dispose();
                hv_col1.Dispose();
                hv_col2.Dispose();
                hv_row21.Dispose();
                hv_row22.Dispose();
                hv_col21.Dispose();
                hv_col22.Dispose();
                hv_row31.Dispose();
                hv_row32.Dispose();
                hv_col31.Dispose();
                hv_col32.Dispose();
                hv_Row1.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
            }
            //return;
        }



    }
}
