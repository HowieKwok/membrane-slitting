using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHolly
{
   public static class PublicAlogClass
    {
        /// <summary>
        /// 卡尺找直线
        /// </summary>
        /// <param name="inputImage">输入图像</param>
        /// <param name="inputRegion">输入区域</param>
        /// <param name="outputContour">输出轮廓</param>
        /// <param name="inputXorY">沿Y轴搜索,或者X轴</param>
        /// <param name="inputInterval">间隔</param>
        /// <param name="inputDirection">方向 L2R_New从左到右,U2B_New从上到下</param>
        /// <param name="inputRoiWidthLen">卡尺ROI长度</param>
        /// <param name="inputSigma">平滑系数</param>
        /// <param name="inputAmplitudeThreshold">阈值</param>
        /// <param name="inputTransition">’negative‘从亮到暗，‘positive’从暗到亮，‘all’</param>
        /// <param name="inputPosition">'first'第一点,'last'最后一个,'all' 所有的边缘</param>
        /// <param name="OutPointRow">输出点Y坐标</param>
        /// <param name="OutPointCol">输出点X坐标</param>
        public static void Find_Line_ForRectangle(HObject inputImage, HObject inputRegion,
      out HObject outputContour, HTuple inputXorY, HTuple inputInterval,
      HTuple inputDirection, HTuple inputRoiWidthLen, HTuple inputSigma,
      HTuple inputAmplitudeThreshold, HTuple inputTransition, HTuple inputPosition,
      out HTuple OutPointRow, out HTuple OutPointCol)
        {

            //HOperatorSet.Emphasize(inputImage, out inputImage, 7, 7, 1);

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
            //
            //inputXorY:沿Y轴搜索,或者X轴
            //inputInterval:间隔
            //inputDirection:方向 L2R_New从左到右,U2B_New从上到下
            //inputRoiWidthLen:卡尺ROI长度
            //inputSigma:平滑系数
            //inputAmplitudeThreshold:阈值
            //inputTransition:所以’negative‘从亮到暗，‘positive’从暗到亮，‘all’。
            //inputPosition:'first'第一点,'last'最后一个,'all' 所有的边缘


            OutPointRow = new HTuple();
            OutPointCol = new HTuple();
            ho_ProcessContour.Dispose();
            HOperatorSet.GenEmptyObj(out ho_ProcessContour);
            //
            //*****************************************二维卡尺方法**************************
            if ((int)((new HTuple((new HTuple((new HTuple(inputDirection.TupleEqual("L2R_New"))).TupleOr(
                new HTuple(inputDirection.TupleEqual("R2L_New"))))).TupleOr(new HTuple(inputDirection.TupleEqual(
                "U2B_New"))))).TupleOr(new HTuple(inputDirection.TupleEqual("B2U_New")))) != 0)
            {
                HOperatorSet.SmallestRectangle1(inputRegion, out hv_Row11, out hv_Column1,
                    out hv_Row21, out hv_Column2);
                hv_Length1 = (((hv_Column2 - hv_Column1)).TupleAbs()) / 2;
                hv_Length2 = (((hv_Row21 - hv_Row11)).TupleAbs()) / 2;
                hv_Row = (hv_Row21 + hv_Row11) / 2;
                hv_Column = (hv_Column2 + hv_Column1) / 2;
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, 0, hv_Length1,
                    hv_Length2);
                //an := rad(Phi)
                if ((int)(new HTuple(inputXorY.TupleEqual("Y"))) != 0)
                {
                    hv_r2 = hv_Row - hv_Length2;
                    hv_c2 = hv_Column - hv_Length1;
                    hv_r4 = hv_Row + hv_Length2;
                    hv_c4 = hv_Column + hv_Length1;
                    ho_Cross1.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_r2, hv_c2, 100, 0);
                    ho_Cross3.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_r4, hv_c4, 100, 0);
                    hv_Row1 = hv_r2.Clone();
                    hv_Col1 = hv_c2.Clone();
                    hv_Row2 = hv_r4.Clone();
                    hv_Col2 = hv_c4.Clone();
                    //
                    if ((int)(new HTuple(inputDirection.TupleEqual("L2R_New"))) != 0)
                    {
                        hv_LineRowStart_Measure_01_0 = hv_Row2 + (inputRoiWidthLen / 2);
                        hv_LineColumnStart_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                        hv_LineRowEnd_Measure_01_0 = hv_Row1 - (inputRoiWidthLen / 2);
                        hv_LineColumnEnd_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                    }
                    else if ((int)(new HTuple(inputDirection.TupleEqual("R2L_New"))) != 0)
                    {
                        hv_LineRowStart_Measure_01_0 = hv_Row1 - (inputRoiWidthLen / 2);
                        hv_LineColumnStart_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                        hv_LineRowEnd_Measure_01_0 = hv_Row2 + (inputRoiWidthLen / 2);
                        hv_LineColumnEnd_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                    }
                    hv_MeasureLength1 = ((hv_Col2 - hv_Col1) / 2) - 2;
                }
                else if ((int)(new HTuple(inputXorY.TupleEqual("X"))) != 0)
                {
                    hv_r1 = hv_Row - hv_Length2;
                    hv_c1 = hv_Column - hv_Length1;
                    ho_Cross2.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_r1, hv_c1, 100, 0);
                    hv_r3 = hv_Row + hv_Length2;
                    hv_c3 = hv_Column + hv_Length1;
                    ho_Cross1.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_r1, hv_c1, 100, 0);
                    ho_Cross3.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_r3, hv_c3, 100, 0);
                    //an := deg(Phi)
                    hv_Row1 = hv_r1.Clone();
                    hv_Col1 = hv_c1.Clone();
                    hv_Row2 = hv_r3.Clone();
                    hv_Col2 = hv_c3.Clone();
                    //*         endif
                    if ((int)(new HTuple(inputDirection.TupleEqual("U2B_New"))) != 0)
                    {
                        hv_LineRowStart_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                        hv_LineColumnStart_Measure_01_0 = hv_Col1 - (inputRoiWidthLen / 2);
                        hv_LineRowEnd_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                        hv_LineColumnEnd_Measure_01_0 = hv_Col2 + (inputRoiWidthLen / 2);
                    }
                    else if ((int)(new HTuple(inputDirection.TupleEqual("B2U_New"))) != 0)
                    {
                        hv_LineRowStart_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                        hv_LineColumnStart_Measure_01_0 = hv_Col2 + (inputRoiWidthLen / 2);
                        hv_LineRowEnd_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                        hv_LineColumnEnd_Measure_01_0 = hv_Col1 - (inputRoiWidthLen / 2);
                    }
                    hv_MeasureLength1 = (hv_Row2 - hv_Row1) / 2;
                }
                //
                //
                HOperatorSet.GetImageSize(inputImage, out hv_Width, out hv_Height);
                //Create the metrology model data structure
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //The image size is set in advance to speed up the
                //first call of apply_metrology_model.
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                //Define the parameters of the metrology line objects
                //Create One metrology line objects and set parameters
                //gen_contour_polygon_xld (Contour1, [LineRowStart_Measure_01_0,LineRowEnd_Measure_01_0], [LineColumnStart_Measure_01_0, LineColumnEnd_Measure_01_0])
                HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_LineRowStart_Measure_01_0,
                    hv_LineColumnStart_Measure_01_0, hv_LineRowEnd_Measure_01_0, hv_LineColumnEnd_Measure_01_0,
                    hv_MeasureLength1, inputRoiWidthLen / 2, inputSigma, inputAmplitudeThreshold,
                    ((new HTuple("measure_distance")).TupleConcat("measure_transition")).TupleConcat(
                    "measure_select").TupleConcat("min_score"), ((inputInterval.TupleConcat(inputTransition))).TupleConcat(
                    inputPosition).TupleConcat(0.3), out hv_Index1);
                //HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "nearest_neighbor");
                //set_metrology_object_param (MetrologyHandle, 'all', 'min_score', 0.3)
                //
                //Create region of interest for the alignment
                HOperatorSet.ApplyMetrologyModel(inputImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_LineParameter);
                outputContour.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out outputContour, hv_MetrologyHandle,
                    "all", "all", 1.5);
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
                    ho_ResultContour.Dispose();
                    HOperatorSet.GetMetrologyObjectMeasures(out ho_ResultContour, hv_MetrologyHandle,
                        "all", "all", out OutPointRow, out OutPointCol);

                    //concat_obj (ResultContour, ProcessContour, ProcessContour)
                    ho_Cross.Dispose();
                    HOperatorSet.GenCrossContourXld(out ho_Cross, OutPointRow, OutPointCol,
                        2, (new HTuple(45)).TupleRad());
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

                return;
            }
            ho_ProcessContour.Dispose();
            ho_Rectangle1.Dispose();
            ho_Cross1.Dispose();
            ho_Cross3.Dispose();
            ho_Cross2.Dispose();
            ho_ResultContour.Dispose();
            ho_Cross.Dispose();

            return;
        }

        /// <summary>
        /// 获取廷长线
        /// </summary>
        /// <param name="outputContour">输出轮廓</param>
        /// <param name="inputImageWidth">输入图像宽度</param>
        /// <param name="inputImageHeight">输入图像高度</param>
        /// <param name="inputRowBegin">输入直线起始点Y</param>
        /// <param name="inputColBegin">输入直线起始点X</param>
        /// <param name="inputRowEnd">输入直线结束点Y</param>
        /// <param name="inputColEnd">输入直线结束点X</param>
        public static void GenLengthenLine(out HObject outputContour, HTuple inputImageWidth,
      HTuple inputImageHeight, HTuple inputRowBegin, HTuple inputColBegin,
      HTuple inputRowEnd, HTuple inputColEnd)
        {
            HOperatorSet.GenEmptyObj(out outputContour);
            try
            {
                HTuple hv_k = new HTuple(), hv_b = new HTuple();
                HTuple hv_row1 = new HTuple(), hv_row2 = new HTuple();
                HTuple hv_col1 = new HTuple(), hv_col2 = new HTuple();
                // Initialize local and output iconic variables 
                
                //特殊情况水平线
                //y=kx+b
                if ((int)(new HTuple(((inputRowEnd - inputRowBegin)).TupleEqual(0))) != 0)
                {
                    hv_k = 0;
                    outputContour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out outputContour, inputRowEnd.TupleConcat(
                        inputRowEnd), (new HTuple(0)).TupleConcat(inputImageWidth));
                }
                else
                {
                    hv_k = (inputColEnd - inputColBegin) / (inputRowEnd - inputRowBegin);
                    hv_b = inputColEnd - (hv_k * inputRowEnd);
                    hv_row1 = 0;
                    hv_row2 = inputImageHeight.Clone();
                    hv_col1 = (hv_k * hv_row1) + hv_b;
                    hv_col2 = (hv_k * hv_row2) + hv_b;
                    outputContour.Dispose();
                    HOperatorSet.GenContourPolygonXld(out outputContour, hv_row1.TupleConcat(
                        hv_row2), hv_col1.TupleConcat(hv_col2));
                }

                return;
            }
            catch (Exception ex)
            {
               
                return;
            } 
        }

        /// <summary>
        /// 合并相近的区域
        /// </summary>
        /// <param name="inputRegion">输入区域</param>
        /// <param name="outputRegion">输出区域</param>
        /// <param name="inputDistance">像素距离</param>
        public static void UnionCloseRegion(HObject inputRegion, out HObject outputRegion,
     HTuple inputDistance)
        {
            HObject ho_ConnectedRegions, ho_RegionDilation;
            HObject ho_RegionUnion, ho_ObjectSelected = null, ho_intersectionRegion = null;
            HObject ho_RegionTrans = null;

            // Local control variables 

            HTuple hv_Number = null, hv_Index = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out outputRegion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_intersectionRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            outputRegion.Dispose();
            HOperatorSet.GenEmptyObj(out outputRegion);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Union1(inputRegion, out inputRegion);
            HOperatorSet.Connection(inputRegion, out ho_ConnectedRegions);
            //使用矩形膨胀,使相近区域联通
            ho_RegionDilation.Dispose();
            HOperatorSet.DilationRectangle1(ho_ConnectedRegions, out ho_RegionDilation, inputDistance,
                inputDistance);
            ho_RegionUnion.Dispose();
            HOperatorSet.Union1(ho_RegionDilation, out ho_RegionUnion);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionUnion, out ho_ConnectedRegions);
            //获取膨胀后的区域数量,遍历
            HOperatorSet.CountObj(ho_ConnectedRegions, out hv_Number);
            HTuple end_val8 = hv_Number;
            HTuple step_val8 = 1;
            for (hv_Index = 1; hv_Index.Continue(end_val8, step_val8); hv_Index = hv_Index.TupleAdd(step_val8))
            {
                ho_ObjectSelected.Dispose();
                HOperatorSet.SelectObj(ho_ConnectedRegions, out ho_ObjectSelected, hv_Index);
                //让联通后的区域和全部膨胀前的取交集，得到的就是相近的膨胀前的
                ho_intersectionRegion.Dispose();
                HOperatorSet.Intersection(inputRegion, ho_ObjectSelected, out ho_intersectionRegion
                    );
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FillUp(ho_intersectionRegion, out ExpTmpOutVar_0);
                    ho_intersectionRegion.Dispose();
                    ho_intersectionRegion = ExpTmpOutVar_0;
                }
                //取凸包
                ho_RegionTrans.Dispose();
                HOperatorSet.ShapeTrans(ho_intersectionRegion, out ho_RegionTrans, "convex");
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(outputRegion, ho_RegionTrans, out ExpTmpOutVar_0);
                    outputRegion.Dispose();
                    outputRegion = ExpTmpOutVar_0;
                }
            }
            ho_ConnectedRegions.Dispose();
            ho_RegionDilation.Dispose();
            ho_RegionUnion.Dispose();
            ho_ObjectSelected.Dispose();
            ho_intersectionRegion.Dispose();
            ho_RegionTrans.Dispose();

            return;


        }

        /// <summary>
        /// 查看选定区域灰度值区域满足一定高度,满足则把区域上移或者下移
        /// 2023.7.22 取消上下移动的方向限定,对照输入的区域的顶点与找到的区域的顶点,大于则上移 小于则下移 
        /// </summary>
        /// <param name="inputImage">输入图像</param>
        /// <param name="inputReion">输入区域</param>
        /// <param name="outputReion">输出平移后的区域</param>
        /// <param name="inputMinThreshold">区域最小灰度</param>
        /// <param name="inputMaxThreshold">区域最大灰度</param>
        /// <param name="inputHeightRatio">输入高度比例</param>
        /// <param name="inputMoveRatio">输入平移距离比例</param>
        /// <param name="inputwidth">输入极耳宽度 防止找错</param>
        public static void genMoveReion(HObject inputImage, HObject inputReion, out HObject outputReion,
     HTuple inputMinThreshold, HTuple inputMaxThreshold, double inputHeightRatio, HTuple inputMoveRatio,
     double inputwidth = 20)
        {
            HObject ho_ImageReduced, ho_Region, ho_ConnectedRegions;
            HObject SelectedRegions;

            // Local control variables 

            HTuple height = null, width=null, inputReionHeight = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out outputReion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out SelectedRegions);
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(inputImage, inputReion, out ho_ImageReduced);
            ho_Region.Dispose();
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, inputMinThreshold, inputMaxThreshold);

            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
            SelectedRegions.Dispose();
            HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out SelectedRegions, "max_area",
                70);
            HOperatorSet.RegionFeatures(SelectedRegions, "height", out height);
            HOperatorSet.RegionFeatures(SelectedRegions, "width", out width);

            HOperatorSet.RegionFeatures(inputReion, "height", out inputReionHeight); 
            HTuple row11, column11, row2, column2;
            HOperatorSet.SmallestRectangle1(SelectedRegions, out  row11, 
                out  column11, out  row2, out  column2);

            HTuple row12, column12, row22, column22;
            HOperatorSet.SmallestRectangle1(inputReion, out  row12,
                out  column12, out  row22, out  column22);

         
            //方向 up向上,向下down
            if (height.D>inputReionHeight.D * inputHeightRatio&& width.D> inputwidth)
            {
                //if (inputDirection=="up")
                //{
                //对照输入的区域的顶点与找到的区域的顶点,大于则上移 小于则下移
                if (row12.D< row11.D)
                { 
                    outputReion.Dispose();
                    HOperatorSet.MoveRegion(inputReion, out outputReion, (-height) * inputMoveRatio,
                        0);
                }
                else
                {
                    outputReion.Dispose();
                    HOperatorSet.MoveRegion(inputReion, out outputReion, height * inputMoveRatio,
                        0);
                }
            }
            else
            {
                outputReion.Dispose();
                outputReion = inputReion.CopyObj(1, -1);
            }
            ho_ImageReduced.Dispose();
            ho_Region.Dispose();
            ho_ConnectedRegions.Dispose();
            SelectedRegions.Dispose();

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="win">窗口</param>
        /// <param name="LorRLine">左右基准直线</param>
        /// <param name="UorLine">上下基准直线</param>
        /// <param name="LAndRTranslationDistance">左右移动距离</param>
        /// <param name="UAndLTranslationDistance">上下移动距离</param>
        /// <param name="IsShow_ResultRegion">是否显示</param>
        /// <param name="LorR">L或者右移</param>
        /// <param name="UorL">上或者下移</param>
        /// <param name="WidthCali">横向标定</param>
        /// <param name="HeightCali">纵向标定</param>
        /// <param name="rectangle_Point"></param>
        public static HObject Get_Rectangle(HTuple win, Line LorRLine, Line UorLine,
          double LAndRTranslationDistance, double UAndLTranslationDistance
          , bool IsShow_ResultRegion, string LorR, string UorL, double WidthCali, double HeightCali,
          ref Rectangle_Point rectangle_Point)
        {
            HObject cross = null;
            HOperatorSet.GenEmptyObj(out cross);

            HObject Region = null;
            HOperatorSet.GenEmptyObj(out Region);

            //HObject Image = null;
            //HOperatorSet.GenEmptyObj(out Image);

            HObject outputRegion = null;
            HOperatorSet.GenEmptyObj(out outputRegion);
            HTuple Row, Column, Phi, Length1, Length2;
            try
            {
                HTuple TopLeft_Row = 0;
                HTuple TopLeft_Col = 0;
                HTuple TopRight_Row = 0;
                HTuple TopRight_Col = 0;
                HTuple LowerLeft_Row = 0;
                HTuple LowerLeft_Col = 0;
                HTuple LowerRight_Row = 0;
                HTuple LowerRight_Col = 0;

                //右上
                Get_Intersection_lines(LorRLine, UorLine,
                    out TopRight_Row, out TopRight_Col);

                //HOperatorSet.GenCrossContourXld(out cross, TopRight_Row, TopRight_Col, TuBu_TranslationDistance / WidthCali, 0);
                //HOperatorSet.DispObj(cross, win);
                //涂布右向左平移
                Line line1 = new Line();

                //左是减,右是加
                if (LorR == "R")
                {
                    line1 = translationLine(LorRLine, LAndRTranslationDistance / WidthCali);
                }
                else
                {
                    line1 = translationLine(LorRLine, -LAndRTranslationDistance / WidthCali);
                }


                //L向下  U向上减
                Line line2 = new Line();
                if (UorL == "L")
                {
                    line2 = translationLine(UorLine, UAndLTranslationDistance / HeightCali, 1);
                }
                else
                {
                    line2 = translationLine(UorLine, -UAndLTranslationDistance / HeightCali, 1);
                }

                //左上
                Get_Intersection_lines(line1, UorLine,
                   out TopLeft_Row, out TopLeft_Col);

                //HOperatorSet.GenCrossContourXld(out cross, TopLeft_Row, TopLeft_Col, 100, 0);
                //HOperatorSet.DispObj(cross, win);

                //左下
                Get_Intersection_lines(line1, line2,
                  out LowerLeft_Row, out LowerLeft_Col);

                //HOperatorSet.GenCrossContourXld(out cross, LowerLeft_Row, LowerLeft_Col, 100, 0);
                //HOperatorSet.DispObj(cross, win);
                //右下
                Get_Intersection_lines(LorRLine, line2,
                  out LowerRight_Row, out LowerRight_Col);

                //HOperatorSet.GenCrossContourXld(out cross, LowerRight_Row, LowerRight_Col, 100, 0);
                //HOperatorSet.DispObj(cross, win);

                HTuple rows = new HTuple();
                rows[0] = TopLeft_Row;
                rows[1] = TopRight_Row;
                rows[2] = LowerRight_Row;
                rows[3] = LowerLeft_Row;
                HTuple columns = new HTuple();

                columns[0] = TopLeft_Col;
                columns[1] = TopRight_Col;
                columns[2] = LowerRight_Col;
                columns[3] = LowerLeft_Col;

                HOperatorSet.GenRegionPolygon(out Region, rows, columns);
                HOperatorSet.SmallestRectangle2(Region, out Row, out Column, out Phi, out Length1, out Length2);


                if (Row.Length > 0)
                {
                    HOperatorSet.GenRectangle2(out outputRegion, Row, Column, Phi, Length1, Length2);
                    if (IsShow_ResultRegion)
                    {
                        HOperatorSet.SetColor(win, "blue");
                        HOperatorSet.DispObj(outputRegion, win);
                    }


                    //获取区域四个有顺序顶点,依次左上 >> 右上 >> 左下 >> 右下
                    rectangle_Point.state = Get_Rectangle_Point(outputRegion, out TopLeft_Row,
                      out TopLeft_Col, out TopRight_Row, out TopRight_Col,
                      out LowerLeft_Row, out LowerLeft_Col, out LowerRight_Row, out LowerRight_Col);

                }


                return outputRegion;

            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
               
                return outputRegion;
            }
            finally
            {
                cross?.Dispose();
                Region?.Dispose();
                //Image?.Dispose();
                //outputRegion?.Dispose();
            }


        }





        /// <summary>
        /// 平移直线
        /// </summary>
        /// <param name="inputLine">输入直线</param>
        /// <param name="value">平移值</param>
        /// <param name="type">1代表左右平移,2代表上下平移</param>
        /// <returns></returns>
        private static Line translationLine(Line inputLine, double value, int type = 0)
        {
            Line outputLine = new Line();
            try
            {
                if (type == 0)
                {
                    outputLine.RowEnd = inputLine.RowEnd;
                    outputLine.RowBegin = inputLine.RowBegin;
                    outputLine.ColEnd = inputLine.ColEnd + value;
                    outputLine.ColBegin = inputLine.ColBegin + value;
                }
                else if (type == 1)
                {
                    outputLine.RowEnd = inputLine.RowEnd + value;
                    outputLine.RowBegin = inputLine.RowBegin + value;
                    outputLine.ColEnd = inputLine.ColEnd;
                    outputLine.ColBegin = inputLine.ColBegin;
                }



                return outputLine;
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                
                return outputLine;
            }


        }


        /// <summary>
        /// 获取区域四个有顺序顶点,依次左上>>右上>>左下>>右下
        /// </summary>
        /// <param name="inputRectangle">输入区域</param>
        /// <param name="TopLeft_Row"></param>
        /// <param name="TopLeft_Col"></param>
        /// <param name="TopRight_Row"></param>
        /// <param name="TopRight_Col"></param>
        /// <param name="LowerRight_Row"></param>
        /// <param name="LowerRight_Col"></param>
        /// <param name="LowerLeft_Row"></param>
        /// <param name="LowerLeft_Col"></param>

        public static bool Get_Rectangle_Point(HObject inputRectangle, out HTuple TopLeft_Row,
    out HTuple TopLeft_Col, out HTuple TopRight_Row, out HTuple TopRight_Col,
    out HTuple LowerLeft_Row, out HTuple LowerLeft_Col,
    out HTuple LowerRight_Row, out HTuple LowerRight_Col)
        {

            TopLeft_Row = 0;
            TopLeft_Col = 0;
            TopRight_Row = 0;
            TopRight_Col = 0;
            LowerRight_Row = 0;
            LowerRight_Col = 0;
            LowerLeft_Row = 0;
            LowerLeft_Col = 0;

            try
            {
                HTuple hv_Row = null, hv_Column = null, hv_Phi = null;
                HTuple hv_Length1 = null, hv_Length2 = null, hv_Tmp = new HTuple();
                HTuple hv_Sin = null, hv_Cos = null, hv_TopLeft_X = null;
                HTuple hv_TopLeft_Y = null, hv_TopRight_X = null, hv_TopRight_Y = null;
                HTuple hv_LowerRight_X = null, hv_LowerRight_Y = null;
                HTuple hv_LowerLeft_X = null, hv_LowerLeft_Y = null;
                // Initialize local and output iconic variables 
                //计算矩形的中间点、角度、及长宽（一半）
                HOperatorSet.SmallestRectangle2(inputRectangle, out hv_Row, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);

                if ((int)(new HTuple(((((hv_Phi.TupleDeg())).TupleAbs())).TupleGreater(45))) != 0)
                {
                    hv_Phi = (((hv_Phi.TupleDeg()) - (90 * (hv_Phi / (hv_Phi.TupleAbs()))))).TupleRad()
                        ;
                    hv_Tmp = hv_Length1.Clone();
                    hv_Length1 = hv_Length2.Clone();
                    hv_Length2 = hv_Tmp.Clone();
                }

                HOperatorSet.TupleSin(hv_Phi, out hv_Sin);
                HOperatorSet.TupleCos(hv_Phi, out hv_Cos);

                //左上角
                hv_TopLeft_X = ((-hv_Length1) * hv_Cos) - (hv_Length2 * hv_Sin);
                hv_TopLeft_Y = ((-hv_Length1) * hv_Sin) + (hv_Length2 * hv_Cos);
                TopLeft_Row = hv_Row - hv_TopLeft_Y;
                TopLeft_Col = hv_Column + hv_TopLeft_X;

                //右上角
                hv_TopRight_X = (hv_Length1 * hv_Cos) - (hv_Length2 * hv_Sin);
                hv_TopRight_Y = (hv_Length1 * hv_Sin) + (hv_Length2 * hv_Cos);
                TopRight_Row = hv_Row - hv_TopRight_Y;
                TopRight_Col = hv_Column + hv_TopRight_X;

                //右下角
                hv_LowerRight_X = (hv_Length1 * hv_Cos) + (hv_Length2 * hv_Sin);
                hv_LowerRight_Y = (hv_Length1 * hv_Sin) - (hv_Length2 * hv_Cos);
                LowerRight_Row = hv_Row - hv_LowerRight_Y;
                LowerRight_Col = hv_Column + hv_LowerRight_X;

                //左下角
                hv_LowerLeft_X = ((-hv_Length1) * hv_Cos) + (hv_Length2 * hv_Sin);
                hv_LowerLeft_Y = ((-hv_Length1) * hv_Sin) - (hv_Length2 * hv_Cos);
                LowerLeft_Row = hv_Row - hv_LowerLeft_Y;
                LowerLeft_Col = hv_Column + hv_LowerLeft_X;

                return true;
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
               
                return false;
            }
            finally
            {

            }

        }


        /// <summary>
        /// 获取两直线交点
        /// </summary>
        /// <param name="line1">直线1</param>
        /// <param name="line2">直线2</param>
        /// <param name="row">交点y</param>
        /// <param name="column">交点x</param>
        /// <returns></returns>
        public static bool Get_Intersection_lines(Line line1, Line line2, out HTuple row, out HTuple column)
        {
            bool IsOverlapping = false;
            HTuple isOverlapping = 0;
            row = 0; column = 0;
            try
            {


                HOperatorSet.IntersectionLines(line1.RowBegin, line1.ColBegin,
                    line1.RowEnd, line1.ColEnd, line2.RowBegin, line2.ColBegin,
                    line2.RowEnd, line2.ColEnd,
                    out row, out column, out isOverlapping);

                if (isOverlapping == 0)
                {
                    IsOverlapping = true;

                }
                if (row.Length < 1 && column.Length < 1)
                {
                    row = 0;
                    column = 0;
                }
                return IsOverlapping;
            }
            catch (Exception ex)
            {

                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
               
                return IsOverlapping;
            }
            finally
            {


            }
        }


    }
}
