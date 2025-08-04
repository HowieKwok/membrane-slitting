using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace EHolly
{
    public class Detcet_JR
    {
        /// <summary>
        /// 构建极耳区域
        /// </summary>
        /// <param name="IsLeft">是否为左侧极耳</param>
        /// <param name="JRCenter_Row">极耳中心Row行</param>
        /// <param name="RCenter_column">极耳中心column列，等于齐边列坐标</param>
        /// <param name="JRbottomWidth">极耳底部宽度</param>
        /// <param name="bottomAngleR_ridus">底部R角半径</param>
        /// <param name="topAngleR_ridus">顶部R角半径</param>
        /// <param name="bottomAngleR_CompensationAngle">底部R角补偿角度（0-90）</param>
        /// <param name="topAngleR_CompensationAngle">顶部R角补偿角度（0-90）</param>
        /// <param name="hypotenuse_length">斜边长度</param>
        /// <param name="hypotenuse_angle">斜边角度</param>
        /// <param name="bottomAngleR_region">输出：极耳底部R角部分</param>
        /// <param name="hypotenuse_region">输出：极耳斜边部分</param>
        /// <param name="topAngleR_region">输出：极耳顶部R角部分</param>
        public string Build_Standard_JR(bool IsLeft, HTuple JRCenter_Row, HTuple JRCenter_column, HTuple JRbottomWidth, HTuple bottomAngleR_ridus, HTuple topAngleR_ridus,
            HTuple bottomAngleR_CompensationAngle, HTuple topAngleR_CompensationAngle, HTuple hypotenuse_length, HTuple hypotenuse_angle,
            out HObject bottomAngleR_region, out HObject hypotenuse_region, out HObject topAngleR_region)

        {
            #region NEW params
            HTuple hv_pi = 3.14159;
            HTuple hv_pi_to_Rad = hv_pi * 2 / 360;
            HTuple CrossLength = 16;
            HTuple hv_hypotenuse_Row = new HTuple();
            HTuple hv_hypotenuse_Column = new HTuple();
            HObject ho_Cross0 = null;
            HObject ho_Cross1 = null;
            HObject ho_Cross11 = null;
            HObject ho_Cross2 = null;
            HObject ho_Cross21 = null;
            HObject ho_Cross3 = null;
            HObject ho_Cross31 = null;
            HObject ho_Circle3 = null;
            HObject ho_Circle31 = null;

            HObject ho_Cross4 = null;
            HObject ho_Cross41 = null;
            HObject ho_Cross5 = null;
            HObject ho_Cross51 = null;
            HObject ho_Cross6 = null;
            HObject ho_Cross61 = null;
            HObject ho_Circle6 = null;
            HObject ho_Circle61 = null;

            HObject ho_Cross7 = null;
            HObject ho_Cross71 = null;

            HObject rectangle0 = null;
            HObject ho_Contour = null;
            HObject rectangle1 = null;
            HObject rectangle2 = null;
            HObject rectangle3 = null;
            HObject Region_R1 = null;
            HObject Region_R2 = null;

            HOperatorSet.GenEmptyObj(out ho_Cross0);
            HOperatorSet.GenEmptyObj(out ho_Cross1);
            HOperatorSet.GenEmptyObj(out ho_Cross11);
            HOperatorSet.GenEmptyObj(out ho_Cross2);
            HOperatorSet.GenEmptyObj(out ho_Cross21);
            HOperatorSet.GenEmptyObj(out ho_Cross3);
            HOperatorSet.GenEmptyObj(out ho_Cross31);
            HOperatorSet.GenEmptyObj(out ho_Circle3);
            HOperatorSet.GenEmptyObj(out ho_Circle31);

            HOperatorSet.GenEmptyObj(out ho_Cross4);
            HOperatorSet.GenEmptyObj(out ho_Cross41);
            HOperatorSet.GenEmptyObj(out ho_Cross5);
            HOperatorSet.GenEmptyObj(out ho_Cross51);
            HOperatorSet.GenEmptyObj(out ho_Cross6);
            HOperatorSet.GenEmptyObj(out ho_Cross61);
            HOperatorSet.GenEmptyObj(out ho_Circle6);
            HOperatorSet.GenEmptyObj(out ho_Circle61);

            HOperatorSet.GenEmptyObj(out ho_Cross7);
            HOperatorSet.GenEmptyObj(out ho_Cross71);

            HOperatorSet.GenEmptyObj(out rectangle0);
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out rectangle1);
            HOperatorSet.GenEmptyObj(out rectangle2);
            HOperatorSet.GenEmptyObj(out rectangle3);
            HOperatorSet.GenEmptyObj(out Region_R1);
            HOperatorSet.GenEmptyObj(out Region_R2);

            HOperatorSet.GenEmptyObj(out bottomAngleR_region);
            HOperatorSet.GenEmptyObj(out hypotenuse_region);
            HOperatorSet.GenEmptyObj(out topAngleR_region);
            //bottomAngleR_region.Dispose();
            //hypotenuse_region.Dispose();
            //topAngleR_region.Dispose();
            #endregion
            try
            {
                //0、极耳中心点坐标         
                HOperatorSet.GenCrossContourXld(out ho_Cross0, JRCenter_Row, JRCenter_column, CrossLength, 0.785398);
                // X、上上上上上上上
                // X1、下下下下下下下
                // 上下点：行坐标关于极耳中心对称，列坐标相同

                //1、齐边上起点坐标------11、齐边下起点坐标
                HTuple hv_row1 = JRCenter_Row - JRbottomWidth / 2 - 100;
                HTuple hv_row11 = 2 * JRCenter_Row - hv_row1;
                HTuple hv_column1 = JRCenter_column;

                HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_row1, hv_column1, CrossLength, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_Cross11, hv_row11, hv_column1, CrossLength, 0.785398);

                //2、齐边-R角交点坐标
                HTuple hv_row2 = JRCenter_Row - JRbottomWidth / 2;
                HTuple hv_row21 = 2 * JRCenter_Row - hv_row2;
                HTuple hv_column2 = JRCenter_column;


                HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_row2, hv_column2, CrossLength, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_Cross21, hv_row21, hv_column2, CrossLength, 0.785398);

                //3、底部R角圆心坐标-(圆心)
                HTuple hv_row3 = hv_row2 - (bottomAngleR_ridus * (((bottomAngleR_CompensationAngle * hv_pi_to_Rad)).TupleCos()));
                HTuple hv_row31 = 2 * JRCenter_Row - hv_row3;
                //HTuple hv_column3 = hv_column2 + (bottomAngleR_ridus * (((bottomAngleR_CompensationAngle * hv_pi_to_Rad)).TupleSin()));
                HTuple hv_column3 = null;
                if (IsLeft)
                {
                    hv_column3 = hv_column2 - (bottomAngleR_ridus * (((bottomAngleR_CompensationAngle * hv_pi_to_Rad)).TupleSin()));
                }
                else
                {
                    hv_column3 = hv_column2 + (bottomAngleR_ridus * (((bottomAngleR_CompensationAngle * hv_pi_to_Rad)).TupleSin()));
                }

                HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_row3, hv_column3, CrossLength, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_Cross31, hv_row31, hv_column3, CrossLength, 0.785398);

                HOperatorSet.GenCircle(out ho_Circle3, hv_row3, hv_column3, bottomAngleR_ridus);
                HOperatorSet.GenCircle(out ho_Circle31, hv_row31, hv_column3, bottomAngleR_ridus);

                //4、底部R角-斜边交点坐标
                HTuple hv_row4 = hv_row3 + (bottomAngleR_ridus * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleSin()));
                HTuple hv_row41 = 2 * JRCenter_Row - hv_row4;
                //  HTuple hv_column4 = hv_column3 - (bottomAngleR_ridus * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleCos()));
                HTuple hv_column4 = null;
                if (IsLeft)
                {
                    hv_column4 = hv_column3 + (bottomAngleR_ridus * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleCos()));
                }
                else
                {
                    hv_column4 = hv_column3 - (bottomAngleR_ridus * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleCos()));
                }

                HOperatorSet.GenCrossContourXld(out ho_Cross4, hv_row4, hv_column4, CrossLength, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_Cross41, hv_row41, hv_column4, CrossLength, 0.785398);

                //5、斜边-顶部R角交点坐标
                HTuple hv_row5 = hv_row4 + (hypotenuse_length * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleCos()));
                HTuple hv_row51 = 2 * JRCenter_Row - hv_row5;
                //  HTuple hv_column5 = hv_column4 + (hypotenuse_length * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleSin()));
                HTuple hv_column5 = null;
                if (IsLeft)
                {
                    hv_column5 = hv_column4 - (hypotenuse_length * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleSin()));
                }
                else
                {
                    hv_column5 = hv_column4 + (hypotenuse_length * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleSin()));
                }

                HOperatorSet.GenCrossContourXld(out ho_Cross5, hv_row5, hv_column5, CrossLength, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_Cross51, hv_row51, hv_column5, CrossLength, 0.785398);

                //6、顶部R角圆心坐标-(圆心）
                HTuple hv_row6 = hv_row5 + (topAngleR_ridus * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleSin()));
                HTuple hv_row61 = 2 * JRCenter_Row - hv_row6;
                //  HTuple hv_column6 = hv_column5 - (topAngleR_ridus * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleCos()));
                HTuple hv_column6 = null;
                if (IsLeft)
                {
                    hv_column6 = hv_column5 + (topAngleR_ridus * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleCos()));
                }
                else
                {
                    hv_column6 = hv_column5 - (topAngleR_ridus * ((((180 - hypotenuse_angle) * hv_pi_to_Rad)).TupleCos()));
                }

                HOperatorSet.GenCrossContourXld(out ho_Cross6, hv_row6, hv_column6, CrossLength, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_Cross61, hv_row61, hv_column6, CrossLength, 0.785398);
                HOperatorSet.GenCircle(out ho_Circle6, hv_row6, hv_column6, topAngleR_ridus);
                HOperatorSet.GenCircle(out ho_Circle61, hv_row61, hv_column6, topAngleR_ridus);

                //7、极耳R角-顶边交点坐标
                HTuple hv_row7 = hv_row6 - (topAngleR_ridus * (((topAngleR_CompensationAngle * hv_pi_to_Rad)).TupleCos()));
                HTuple hv_row71 = 2 * JRCenter_Row - hv_row7;
                // HTuple hv_column7 = hv_column6 + (topAngleR_ridus * (((topAngleR_CompensationAngle * hv_pi_to_Rad)).TupleSin()));
                HTuple hv_column7 = null;
                if (IsLeft)
                {
                    hv_column7 = hv_column6 - (topAngleR_ridus * (((topAngleR_CompensationAngle * hv_pi_to_Rad)).TupleSin()));
                }
                else
                {
                    hv_column7 = hv_column6 + (topAngleR_ridus * (((topAngleR_CompensationAngle * hv_pi_to_Rad)).TupleSin()));
                }

                HOperatorSet.GenCrossContourXld(out ho_Cross7, hv_row7, hv_column7, CrossLength, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_Cross71, hv_row71, hv_column7, CrossLength, 0.785398);


                //8、极耳顶边中点坐标
                HTuple hv_row8 = JRCenter_Row;
                HTuple hv_row81 = 2 * JRCenter_Row - hv_row8;
                HTuple hv_column8 = hv_column7;
                //参数异常就返回
                if (hv_row7 >= hv_row71)
                {
                    return "参数设置异常";
                }
                if (JRbottomWidth <= 0 || hypotenuse_length <= 0)
                {
                    return "尺寸参数设置异常";
                }
                //交并法求极耳区域
                //1、极耳底部R角区域

                if (IsLeft)
                {
                    HOperatorSet.GenRectangle1(out rectangle0, hv_row2, hv_column4, hv_row21, hv_column2);
                }
                else
                {
                    HOperatorSet.GenRectangle1(out rectangle0, hv_row2, hv_column2, hv_row21, hv_column4);
                }


                // HOperatorSet.GenRectangle1(out rectangle0, hv_row2, hv_column2, hv_row21, hv_column4);
                HOperatorSet.Difference(rectangle0, ho_Circle3, out bottomAngleR_region);
                HOperatorSet.Difference(bottomAngleR_region, ho_Circle31, out bottomAngleR_region);

                //2、极耳斜边区域          
                hv_hypotenuse_Row = hv_hypotenuse_Row.TupleConcat(hv_row4, hv_row5, hv_row51, hv_row41, hv_row4);
                hv_hypotenuse_Column = hv_hypotenuse_Column.TupleConcat(hv_column4, hv_column5, hv_column5, hv_column4, hv_column4);
                HOperatorSet.GenContourPolygonXld(out ho_Contour, hv_hypotenuse_Row, hv_hypotenuse_Column);
                HOperatorSet.GenRegionContourXld(ho_Contour, out hypotenuse_region, "filled");

                //3极耳顶部R角区域
                if (IsLeft)
                {
                    HOperatorSet.GenRectangle1(out rectangle1, hv_row5, hv_column7, hv_row7, hv_column5);
                    HOperatorSet.Intersection(rectangle1, ho_Circle6, out Region_R1); //上顶部R角区域
                    HOperatorSet.GenRectangle1(out rectangle2, hv_row71, hv_column7, hv_row51, hv_column5);
                    HOperatorSet.Intersection(rectangle2, ho_Circle61, out Region_R2); //下顶部R角区域
                    HOperatorSet.GenRectangle1(out rectangle3, hv_row7, hv_column7, hv_row71, hv_column5);
                }
                else
                {
                    HOperatorSet.GenRectangle1(out rectangle1, hv_row5, hv_column5, hv_row7, hv_column7);
                    HOperatorSet.Intersection(rectangle1, ho_Circle6, out Region_R1); //上顶部R角区域
                    HOperatorSet.GenRectangle1(out rectangle2, hv_row71, hv_column5, hv_row51, hv_column7);
                    HOperatorSet.Intersection(rectangle2, ho_Circle61, out Region_R2); //下顶部R角区域
                    HOperatorSet.GenRectangle1(out rectangle3, hv_row7, hv_column5, hv_row71, hv_column7);
                }

                HOperatorSet.Union2(Region_R1, rectangle3, out Region_R1);
                HOperatorSet.Union2(Region_R1, Region_R2, out topAngleR_region);

                /////显示模式：0、不显示。 1、过程拟合点，拟合圆显示。2、网格线显示。3、极耳三个区域显示
                //HOperatorSet.SetColor(hwindowID, "red");
                //HOperatorSet.SetLineWidth(hwindowID, 2);
                //switch (ShowType)
                //{
                //    case 0:
                //        break;
                //    case 1:
                //        HOperatorSet.DispObj(ho_Cross0, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross1, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross11, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross2, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross21, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross3, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross31, hwindowID);
                //        HOperatorSet.DispObj(ho_Circle3, hwindowID);
                //        HOperatorSet.DispObj(ho_Circle31, hwindowID);

                //        HOperatorSet.DispObj(ho_Cross4, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross41, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross5, hwindowID);

                //        HOperatorSet.DispObj(ho_Cross51, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross6, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross61, hwindowID);
                //        HOperatorSet.DispObj(ho_Circle6, hwindowID);
                //        HOperatorSet.DispObj(ho_Circle61, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross7, hwindowID);
                //        HOperatorSet.DispObj(ho_Cross71, hwindowID);
                //        break;
                //    case 2:
                //        HOperatorSet.DispLine(hwindowID, hv_row2, hv_column2, hv_row21, hv_column2);
                //        HOperatorSet.DispLine(hwindowID, hv_row2, hv_column4, hv_row21, hv_column4);
                //        HOperatorSet.DispLine(hwindowID, hv_row2, hv_column5, hv_row21, hv_column5);
                //        HOperatorSet.DispLine(hwindowID, hv_row2, hv_column7, hv_row21, hv_column7);

                //        HOperatorSet.DispLine(hwindowID, hv_row2, hv_column7, hv_row2, hv_column1);
                //        HOperatorSet.DispLine(hwindowID, hv_row21, hv_column7, hv_row21, hv_column1);
                //        HOperatorSet.DispLine(hwindowID, hv_row7, hv_column7, hv_row7, hv_column1);
                //        HOperatorSet.DispLine(hwindowID, hv_row71, hv_column7, hv_row71, hv_column1);
                //        HOperatorSet.DispObj(bottomAngleR_region, hwindowID);
                //        HOperatorSet.DispObj(hypotenuse_region, hwindowID);
                //        HOperatorSet.DispObj(topAngleR_region, hwindowID);
                //        break;
                //    case 3:
                //        HOperatorSet.DispObj(bottomAngleR_region, hwindowID);
                //        HOperatorSet.DispObj(hypotenuse_region, hwindowID);
                //        HOperatorSet.DispObj(topAngleR_region, hwindowID);
                //        break;
                //    default:
                //        break;
                //}
                //HOperatorSet.SetLineWidth(hwindowID, 1);
                return "标准极耳构建已完成";
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }
            finally
            {
                #region params Dispose();  
                ho_Cross0.Dispose();
                ho_Cross1.Dispose();
                ho_Cross11.Dispose();
                ho_Cross2.Dispose();
                ho_Cross21.Dispose();
                ho_Cross3.Dispose();
                ho_Cross31.Dispose();
                ho_Circle3.Dispose();
                ho_Circle31.Dispose();

                ho_Cross4.Dispose();
                ho_Cross41.Dispose();
                ho_Cross5.Dispose();
                ho_Cross51.Dispose();
                ho_Cross6.Dispose();
                ho_Cross61.Dispose();
                ho_Circle6.Dispose();
                ho_Circle61.Dispose();

                ho_Cross7.Dispose();
                ho_Cross71.Dispose();

                rectangle0.Dispose();
                ho_Contour.Dispose();
                rectangle1.Dispose();
                rectangle2.Dispose();
                rectangle3.Dispose();
                Region_R1.Dispose();
                Region_R2.Dispose();
                #endregion
            }
        }

        /// <summary>
        /// 生成检余料区域
        /// </summary>
        /// <param name="bottomAngleR_region">输入：极耳底部R角部分</param>
        /// <param name="hypotenuse_region">输入：极耳底部R角部分</param>
        /// <param name="topAngleR_region">输入：极耳底部R角部分</param>
        /// <param name="Isleft">是否为左侧极耳</param>
        /// <param name="detectionBoundary_Y">极耳检测区Y向外扩</param>
        /// <param name="detectionBoundary_X">极耳检测区X向外扩</param>
        /// <param name="bottomAngleR_Shielded_X">底部R角距离齐边屏蔽-X</param>
        /// <param name="bottomAngleR_Shielded_Y">底部R角外扩屏蔽-Y</param>
        /// <param name="hypotenuse_Shielded_Y">斜边外扩屏蔽-Y</param>
        /// <param name="topAngleR_Shielded_X">顶部R角外扩屏蔽-X</param>
        /// <param name="topAngleR_Shielded_Y">顶部R角外扩屏蔽-Y</param>
        /// <param name="Detect_remnants_region">输出：极耳余料检测区域</param>
        public void Detect_remnants(HObject bottomAngleR_region, HObject hypotenuse_region, HObject topAngleR_region, bool Isleft, HTuple detectionBoundary_Y, HTuple detectionBoundary_X,
            HTuple bottomAngleR_Shielded_X, HTuple bottomAngleR_Shielded_Y, HTuple hypotenuse_Shielded_Y, HTuple topAngleR_Shielded_X, HTuple topAngleR_Shielded_Y,
           out HObject Detect_remnants_region)
        {
            HTuple bottomAngleR_row1 = null;
            HTuple bottomAngleR_column1 = null;
            HTuple bottomAngleR_row11 = null;
            HTuple bottomAngleR_column11 = null;

            HTuple topAngleR_row3 = null;
            HTuple topAngleR_column3 = null;
            HTuple topAngleR_row31 = null;
            HTuple topAngleR_column31 = null;

            HTuple JR_detectBorder_row1 = null;
            HTuple JR_detectBorder_column1 = null;
            HTuple JR_detectBorder_row11 = null;
            HTuple JR_detectBorder_column11 = null;

            HObject JR_detectBorder = null;
            HOperatorSet.GenEmptyObj(out JR_detectBorder);
            HObject topAngleR_shield = null;
            HOperatorSet.GenEmptyObj(out topAngleR_shield);
            Detect_remnants_region = null;
            HOperatorSet.GenEmptyObj(out Detect_remnants_region);

            double zoom_Factor_X = 0;
            double zoom_Factor_X1 = 0;
            bool flag = IsHobjectNUll(bottomAngleR_region) || IsHobjectNUll(hypotenuse_region) || IsHobjectNUll(topAngleR_region);
            if (flag)
            {
                return;
            }

            //底部R角，
            HOperatorSet.SmallestRectangle1(bottomAngleR_region, out bottomAngleR_row1, out bottomAngleR_column1, out bottomAngleR_row11, out bottomAngleR_column11);
            // HOperatorSet.SmallestRectangle1(hypotenuse_region, out bottomAngleR_row2, out bottomAngleR_column2, out bottomAngleR_row21, out bottomAngleR_column21);
            HOperatorSet.SmallestRectangle1(topAngleR_region, out topAngleR_row3, out topAngleR_column3, out topAngleR_row31, out topAngleR_column31);

            if (Isleft)
            {
                bottomAngleR_Shielded_X = bottomAngleR_Shielded_X <= bottomAngleR_column11 - topAngleR_column3 + detectionBoundary_X ? bottomAngleR_Shielded_X : bottomAngleR_column11 - topAngleR_column3 + detectionBoundary_X - 10;
            }
            else
            {
                bottomAngleR_Shielded_X = bottomAngleR_Shielded_X <= topAngleR_column31 - bottomAngleR_column1 + detectionBoundary_X ? bottomAngleR_Shielded_X : topAngleR_column31 - bottomAngleR_column1 + detectionBoundary_X - 10;
            }
            if (Isleft)
            {
                if (bottomAngleR_Shielded_X < bottomAngleR_column11 - bottomAngleR_column1)
                {
                    HOperatorSet.MoveRegion(bottomAngleR_region, out bottomAngleR_region, 0, -bottomAngleR_column1);
                    zoom_Factor_X = (bottomAngleR_column11 - bottomAngleR_column1 - bottomAngleR_Shielded_X) / (bottomAngleR_column11 - bottomAngleR_column1 + 0.00000001);
                    HOperatorSet.ZoomRegion(bottomAngleR_region, out bottomAngleR_region, zoom_Factor_X, 1);
                    HOperatorSet.MoveRegion(bottomAngleR_region, out bottomAngleR_region, 0, bottomAngleR_column1);
                }
                HOperatorSet.MoveRegion(topAngleR_region, out topAngleR_region, 0, -topAngleR_column3);
                zoom_Factor_X1 = (topAngleR_Shielded_X + topAngleR_column31 - topAngleR_column3) / (topAngleR_column31 - topAngleR_column3 + 0.0000001);
                HOperatorSet.ZoomRegion(topAngleR_region, out topAngleR_region, zoom_Factor_X1, 1);
                HOperatorSet.MoveRegion(topAngleR_region, out topAngleR_region, 0, (topAngleR_column31 - topAngleR_column3) * (1 - zoom_Factor_X1) + topAngleR_column3 + 1);
            }
            else
            {
                if (bottomAngleR_Shielded_X < bottomAngleR_column11 - bottomAngleR_column1)
                {
                    HOperatorSet.MoveRegion(bottomAngleR_region, out bottomAngleR_region, 0, -bottomAngleR_column1);
                    zoom_Factor_X = (bottomAngleR_column11 - bottomAngleR_column1 - bottomAngleR_Shielded_X) / (bottomAngleR_column11 - bottomAngleR_column1 + 0.00000001);
                    HOperatorSet.ZoomRegion(bottomAngleR_region, out bottomAngleR_region, zoom_Factor_X, 1);
                    HOperatorSet.MoveRegion(bottomAngleR_region, out bottomAngleR_region, 0, (bottomAngleR_column11 - bottomAngleR_column1) * (1 - zoom_Factor_X) + bottomAngleR_column1+1);
                }
                HOperatorSet.MoveRegion(topAngleR_region, out topAngleR_region, 0, -topAngleR_column3);
                zoom_Factor_X1 = (topAngleR_Shielded_X + topAngleR_column31 - topAngleR_column3) / (topAngleR_column31 - topAngleR_column3 + 0.0000001);
                HOperatorSet.ZoomRegion(topAngleR_region, out topAngleR_region, zoom_Factor_X1, 1);
                HOperatorSet.MoveRegion(topAngleR_region, out topAngleR_region, 0, topAngleR_column3);
            }
            //HOperatorSet.GenRectangle1(out topAngleR_shield, topAngleR_row3, topAngleR_column31, topAngleR_row31, topAngleR_column31+ topAngleR_Shielded_X);
            //HOperatorSet.Union2(topAngleR_region, topAngleR_shield, out topAngleR_region);
            ReshapeRegion(bottomAngleR_region, bottomAngleR_Shielded_Y, out bottomAngleR_region);
            ReshapeRegion(hypotenuse_region, hypotenuse_Shielded_Y, out hypotenuse_region);
            ReshapeRegion(topAngleR_region, topAngleR_Shielded_Y, out topAngleR_region);
            if (Isleft)
            {
                JR_detectBorder_row1 = bottomAngleR_row1 - detectionBoundary_Y;
                JR_detectBorder_column1 = topAngleR_column3 - detectionBoundary_X;
                JR_detectBorder_row11 = bottomAngleR_row11 + detectionBoundary_Y;
                JR_detectBorder_column11 = bottomAngleR_column11 - bottomAngleR_Shielded_X-1; //XLD转region时，有小数与整数的转换，-1是为了避免区域连接不上情况。
                HOperatorSet.GenRectangle1(out JR_detectBorder, JR_detectBorder_row1, JR_detectBorder_column1, JR_detectBorder_row11, JR_detectBorder_column11);
            }
            else
            {
                JR_detectBorder_row1 = bottomAngleR_row1 - detectionBoundary_Y;
                JR_detectBorder_column1 = bottomAngleR_column1 + bottomAngleR_Shielded_X+2;
                JR_detectBorder_row11 = bottomAngleR_row11 + detectionBoundary_Y;
                JR_detectBorder_column11 = topAngleR_column31 + detectionBoundary_X;
                HOperatorSet.GenRectangle1(out JR_detectBorder, JR_detectBorder_row1, JR_detectBorder_column1, JR_detectBorder_row11, JR_detectBorder_column11);
            }
            HOperatorSet.Difference(JR_detectBorder, bottomAngleR_region, out JR_detectBorder);
            HOperatorSet.Difference(JR_detectBorder, hypotenuse_region, out JR_detectBorder);
            HOperatorSet.Difference(JR_detectBorder, topAngleR_region, out Detect_remnants_region);
            HOperatorSet.ClosingCircle(Detect_remnants_region, out Detect_remnants_region, 20);

            JR_detectBorder.Dispose();
            topAngleR_shield.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bottomAngleR_region">输入：极耳底部R角部分</param>
        /// <param name="hypotenuse_region">输入：极耳底部R角部分</param>
        /// <param name="topAngleR_region">输入：极耳底部R角部分</param>
        /// <param name="Isleft">是否为左侧</param>
        /// <param name="detectionBoundary_Y">极耳检测区Y向外扩</param>
        /// <param name="detectionBoundary_X">极耳检测区X向外扩</param>
        /// <param name="bottomAngleR_Shielded_X">底部R角距离齐边屏蔽-X</param>
        /// <param name="bottomAngleR_Shielded_Y">底部R角外扩屏蔽-Y</param>
        /// <param name="hypotenuse_Shielded_Y">斜边外扩屏蔽-Y</param>
        /// <param name="topAngleR_Shielded_X">顶部R角外扩屏蔽-X</param>
        /// <param name="topAngleR_Shielded_Y">顶部R角外扩屏蔽-Y</param>
        /// <param name="Detect_brokens_region">极耳破损检测区域</param>
        public void Detect_brokens(HObject bottomAngleR_region, HObject hypotenuse_region, HObject topAngleR_region, bool Isleft, HTuple detectionBoundary_Y, HTuple detectionBoundary_X,
            HTuple bottomAngleR_Shielded_X, HTuple bottomAngleR_Shielded_Y, HTuple hypotenuse_Shielded_Y, HTuple topAngleR_Shielded_X, HTuple topAngleR_Shielded_Y,
           out HObject Detect_brokens_region)
        {
            HTuple bottomAngleR_row1 = null;
            HTuple bottomAngleR_column1 = null;
            HTuple bottomAngleR_row11 = null;
            HTuple bottomAngleR_column11 = null;

            HTuple topAngleR_row3 = null;
            HTuple topAngleR_column3 = null;
            HTuple topAngleR_row31 = null;
            HTuple topAngleR_column31 = null;

            HTuple JR_detectBorder_row1 = null;
            HTuple JR_detectBorder_column1 = null;
            HTuple JR_detectBorder_row11 = null;
            HTuple JR_detectBorder_column11 = null;

            HObject JR_detectBorder = null;
            HOperatorSet.GenEmptyObj(out JR_detectBorder);
            HObject bottomAngleR_rect = null;
            HOperatorSet.GenEmptyObj(out bottomAngleR_rect);
            HObject topAngleR_shield = null;
            HOperatorSet.GenEmptyObj(out topAngleR_shield);
            HObject JR_topShield = null;
            HOperatorSet.GenEmptyObj(out JR_topShield);

            double zoom_Factor_X = 0;
            double zoom_Factor_X1 = 0;

            Detect_brokens_region = null;
            HOperatorSet.GenEmptyObj(out Detect_brokens_region);
            bool flag = IsHobjectNUll(bottomAngleR_region) || IsHobjectNUll(hypotenuse_region) || IsHobjectNUll(topAngleR_region);
            if (flag)
            {
                return;
            }

            //底部R角，
            HOperatorSet.SmallestRectangle1(bottomAngleR_region, out bottomAngleR_row1, out bottomAngleR_column1, out bottomAngleR_row11, out bottomAngleR_column11);
            // HOperatorSet.SmallestRectangle1(hypotenuse_region, out bottomAngleR_row2, out bottomAngleR_column2, out bottomAngleR_row21, out bottomAngleR_column21);
            HOperatorSet.SmallestRectangle1(topAngleR_region, out topAngleR_row3, out topAngleR_column3, out topAngleR_row31, out topAngleR_column31);

            bottomAngleR_Shielded_X = bottomAngleR_Shielded_X <= detectionBoundary_X ? bottomAngleR_Shielded_X : detectionBoundary_X - 10;
            if (Isleft)
            {
                topAngleR_Shielded_X = topAngleR_Shielded_X <= bottomAngleR_column11 - topAngleR_column3 ? topAngleR_Shielded_X : bottomAngleR_column11 - topAngleR_column3;
            }
            else
            {
                topAngleR_Shielded_X = topAngleR_Shielded_X <= topAngleR_column31 - bottomAngleR_column1 ? topAngleR_Shielded_X : topAngleR_column31 - bottomAngleR_column1;
            }

            if (Isleft)
            {
                HOperatorSet.MoveRegion(bottomAngleR_region, out bottomAngleR_region, 0, -bottomAngleR_column1);
                zoom_Factor_X = (bottomAngleR_column11 - bottomAngleR_column1 + bottomAngleR_Shielded_X) / (bottomAngleR_column11 - bottomAngleR_column1 + 0.00000001);
                HOperatorSet.ZoomRegion(bottomAngleR_region, out bottomAngleR_region, zoom_Factor_X, 1);
                HOperatorSet.MoveRegion(bottomAngleR_region, out bottomAngleR_region, 0, bottomAngleR_column1);

                if (topAngleR_Shielded_X < topAngleR_column31 - topAngleR_column3)
                {
                    HOperatorSet.MoveRegion(topAngleR_region, out topAngleR_region, 0, -topAngleR_column3);
                    zoom_Factor_X1 = (topAngleR_column31 - topAngleR_column3 - topAngleR_Shielded_X) / (topAngleR_column31 - topAngleR_column3 + 0.00000001);
                    HOperatorSet.ZoomRegion(topAngleR_region, out topAngleR_region, zoom_Factor_X1, 1);
                    HOperatorSet.MoveRegion(topAngleR_region, out topAngleR_region, 0, (topAngleR_column31 - topAngleR_column3) * (1 - zoom_Factor_X1) + topAngleR_column3+1);
                }
            }
            else
            {
                HOperatorSet.MoveRegion(bottomAngleR_region, out bottomAngleR_region, 0, -bottomAngleR_column1);
                zoom_Factor_X = (bottomAngleR_column11 - bottomAngleR_column1 + bottomAngleR_Shielded_X) / (bottomAngleR_column11 - bottomAngleR_column1 + 0.00000001);
                HOperatorSet.ZoomRegion(bottomAngleR_region, out bottomAngleR_region, zoom_Factor_X, 1);
                HOperatorSet.MoveRegion(bottomAngleR_region, out bottomAngleR_region, 0, (bottomAngleR_column11 - bottomAngleR_column1) * (1 - zoom_Factor_X) + bottomAngleR_column1+1);

                if (topAngleR_Shielded_X < topAngleR_column31 - topAngleR_column3)
                {
                    HOperatorSet.MoveRegion(topAngleR_region, out topAngleR_region, 0, -topAngleR_column3);
                    zoom_Factor_X1 = (topAngleR_column31 - topAngleR_column3 - topAngleR_Shielded_X) / (topAngleR_column31 - topAngleR_column3 + 0.00000001);
                    HOperatorSet.ZoomRegion(topAngleR_region, out topAngleR_region, zoom_Factor_X1, 1);
                    HOperatorSet.MoveRegion(topAngleR_region, out topAngleR_region, 0, topAngleR_column3);
                }
            }
            //HOperatorSet.GenRectangle1(out bottomAngleR_rect, bottomAngleR_row1, bottomAngleR_column1 - detectionBoundary_X, bottomAngleR_row11, bottomAngleR_column1);
            //HOperatorSet.Union2(bottomAngleR_region, bottomAngleR_rect, out bottomAngleR_region);
            //HOperatorSet.GenRectangle1(out topAngleR_shield, topAngleR_row3, topAngleR_column31 - topAngleR_Shielded_X, topAngleR_row31, topAngleR_column31);
            //HOperatorSet.Difference(topAngleR_region, topAngleR_shield, out topAngleR_region);
            ReshapeRegion(bottomAngleR_region, bottomAngleR_Shielded_Y, out bottomAngleR_region);
            ReshapeRegion(hypotenuse_region, hypotenuse_Shielded_Y, out hypotenuse_region);
            ReshapeRegion(topAngleR_region, topAngleR_Shielded_Y, out topAngleR_region);

            if (Isleft)
            {
                JR_detectBorder_row1 = bottomAngleR_row1 - detectionBoundary_Y;
                JR_detectBorder_column1 = bottomAngleR_column11 + detectionBoundary_X;
                JR_detectBorder_row11 = bottomAngleR_row11 + detectionBoundary_Y;
                JR_detectBorder_column11 = bottomAngleR_column11 + bottomAngleR_Shielded_X;
                HOperatorSet.GenRectangle1(out JR_detectBorder, JR_detectBorder_row1, JR_detectBorder_column11, JR_detectBorder_row11, JR_detectBorder_column1);
                HOperatorSet.GenRectangle1(out JR_topShield, JR_detectBorder_row1, topAngleR_column3, JR_detectBorder_row11, topAngleR_column3 + topAngleR_Shielded_X+2);
            }
            else
            {
                JR_detectBorder_row1 = bottomAngleR_row1 - detectionBoundary_Y;
                JR_detectBorder_column1 = bottomAngleR_column1 - detectionBoundary_X;
                JR_detectBorder_row11 = bottomAngleR_row11 + detectionBoundary_Y;
                JR_detectBorder_column11 = bottomAngleR_column1 - bottomAngleR_Shielded_X;
                HOperatorSet.GenRectangle1(out JR_detectBorder, JR_detectBorder_row1, JR_detectBorder_column1, JR_detectBorder_row11, JR_detectBorder_column11+2);
                HOperatorSet.GenRectangle1(out JR_topShield, JR_detectBorder_row1, topAngleR_column31 - topAngleR_Shielded_X-1, JR_detectBorder_row11, topAngleR_column31);
            }
            HOperatorSet.Union2(JR_detectBorder, bottomAngleR_region, out JR_detectBorder);
            HOperatorSet.Union2(JR_detectBorder, hypotenuse_region, out JR_detectBorder);
            HOperatorSet.Union2(JR_detectBorder, topAngleR_region, out Detect_brokens_region);
            HOperatorSet.Difference(Detect_brokens_region, JR_topShield, out Detect_brokens_region);
            HOperatorSet.ClosingCircle(Detect_brokens_region, out Detect_brokens_region, 20);

            JR_detectBorder.Dispose();
            bottomAngleR_rect.Dispose();
            topAngleR_shield.Dispose();
        }
        /// <summary>
        /// Y方向缩放区域。
        /// </summary>
        /// <param name="JRpart_region">输入区域</param>
        /// <param name="Shielded_Y">缩放值：大于0放大，小于0缩小</param>       
        /// <param name="JRpart_resultRegion">输出：结果区域</param>
        public void ReshapeRegion(HObject JRpart_region, HTuple Shielded_Y, out HObject JRpart_resultRegion)
        {

            if (Shielded_Y > 0)
            {
                HOperatorSet.DilationRectangle1(JRpart_region, out JRpart_resultRegion, 1, Shielded_Y);
            }
            else if (Shielded_Y < 0)
            {
                HOperatorSet.ErosionRectangle1(JRpart_region, out JRpart_resultRegion, 1, -Shielded_Y);
            }
            else
            {
                JRpart_resultRegion = JRpart_region;
            }
        }


        /// <summary>
        ///  判断hobject是否为空，1为空，0不为空
        /// </summary>
        /// <param name="hobject"></param>
        /// <returns></returns>
        public bool IsHobjectNUll(HObject hobject)
        {
            HObject ho_Null;
            HOperatorSet.GenEmptyObj(out ho_Null);//空hobj
            HTuple resulet = 0;
            if (!hobject.IsInitialized())//IsInitialized只能用来判断图像有无初始化,不能判断图像空不空或有没有
            {
                resulet = 0;

            }
            HOperatorSet.TestEqualObj(hobject, ho_Null, out resulet);

            if (resulet == 1)//两个Obj相等
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
