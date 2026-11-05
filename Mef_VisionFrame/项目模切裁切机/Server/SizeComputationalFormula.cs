using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public static class SizeComputationalFormula
    {
        /// <summary>
        /// 计算点线距离，预定位工位使用此方法。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="coefficient"></param>
        /// <param name="RealFixOffset"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public static bool DistancePl(LineInfo lineInfo1, LineInfo lineInfo2, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {

                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                HOperatorSet.DistancePl(line1midrow, line1midcol, lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple dist);
                Val = dist.D;
                Val = Val * coefficient + RealFixOffset;

                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 计算点线距离，预定位工位使用此方法。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="coefficient"></param>
        /// <param name="RealFixOffset"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public static bool DistancePl(PointInfo pointInfo, LineInfo lineInfo2, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {

                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                HOperatorSet.DistancePl(pointInfo.Row, pointInfo.Col, lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple dist);
                Val = dist.D;
                Val = Val * coefficient + RealFixOffset;

                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 计算点线距离，预定位工位使用此方法。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="coefficient"></param>
        /// <param name="RealFixOffset"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public static bool DistancePp(PointInfo pointInfo1, PointInfo pointInfo2, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {
                HOperatorSet.DistancePp(pointInfo1.Row, pointInfo1.Col, pointInfo2.Row, pointInfo2.Col, out HTuple dist);
                Val = dist.D;
                Val = Val * coefficient + RealFixOffset;

                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 计算点线距离，预定位工位使用此方法。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="coefficient"></param>
        /// <param name="RealFixOffset"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public static bool DistancePp(PointInfo pointInfo1, PointInfo pointInfo2, SizeDirectionEnum sizeDirectionEnum, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {
                if (sizeDirectionEnum == SizeDirectionEnum.X)
                {
                    Val = pointInfo2.Col - pointInfo1.Col;
                    Val = Val * coefficient + RealFixOffset;
                }

                else if (sizeDirectionEnum == SizeDirectionEnum.Y)
                {
                    Val = pointInfo2.Row - pointInfo1.Row;
                    Val = Val * coefficient + RealFixOffset;
                }

                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 计算线线距离，预定位工位使用此方法。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="coefficient"></param>
        /// <param name="RealFixOffset"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public static bool DistanceLl(LineInfo lineInfo1, LineInfo lineInfo2, SizeDirectionEnum sizeDirectionEnum, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {
                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                if (sizeDirectionEnum == SizeDirectionEnum.X)
                {
                    Val = line2midcol - line1midcol;
                    Val = Val * coefficient + RealFixOffset;
                }

                else if (sizeDirectionEnum == SizeDirectionEnum.Y)
                {
                    Val = line2midrow - line1midrow;
                    Val = Val * coefficient + RealFixOffset;
                }


                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 计算线线距离，预定位工位使用此方法。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="coefficient"></param>
        /// <param name="RealFixOffset"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public static bool DistanceLl(LineInfo lineInfo1, LineInfo lineInfo2, SizeDirectionEnum sizeDirectionEnum, double calibValueX, double calibValueY, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {
                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                if (sizeDirectionEnum == SizeDirectionEnum.X)
                {
                    Val = line2midcol - line1midcol;
                    Val = Val * calibValueX * coefficient + RealFixOffset;
                }

                else if (sizeDirectionEnum == SizeDirectionEnum.Y)
                {
                    Val = line2midrow - line1midrow;
                    Val = Val * calibValueY * coefficient + RealFixOffset;
                }


                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 计算线线距离，预定位工位使用此方法。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <returns></returns>
        public static bool DistanceLl(LineInfo lineInfo1, LineInfo lineInfo2, SizeDirectionEnum sizeDirectionEnum, out double Val)
        {
            Val = -999;
            try
            {

                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                if (sizeDirectionEnum == SizeDirectionEnum.X)
                {
                    Val = line2midcol - line1midcol;
                }

                else if (sizeDirectionEnum == SizeDirectionEnum.Y)
                {
                    Val = line2midrow - line1midrow;
                }
                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 计算角度。
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <returns></returns>
        public static bool DistanceAngle(LineInfo lineInfo1, LineInfo lineInfo2,  out double Val)
        {
            Val = -999;
            try
            {

                HOperatorSet.AngleLl(lineInfo1.RowBegin, lineInfo1.ColBegin, lineInfo1.RowEnd, lineInfo1.ColEnd, lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple angle);
                HOperatorSet.TupleDeg(angle, out HTuple deg);
                Val = deg.D;
                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
