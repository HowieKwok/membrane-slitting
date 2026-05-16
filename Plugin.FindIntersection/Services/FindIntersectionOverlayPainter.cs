using System;
using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.FindIntersection.Core;

namespace Plugin.FindIntersection.Services
{
    public sealed class FindIntersectionOverlayPainter
    {
        public void Draw(
            HWindow window,
            ParamsFindIntersection p,
            double crossRow,
            double crossCol,
            bool crosshairEnabled,
            double crossSize)
        {
            if (window == null || p == null) return;

            try
            {
                DrawFollowRoi(window, p);
                DrawFitResult(window, p, crossSize);
                DrawCrosshair(window, crossRow, crossCol, crosshairEnabled);
            }
            catch
            {
            }
        }

        private static void DrawFollowRoi(HWindow window, ParamsFindIntersection p)
        {
            RoiFollower transformer = new RoiFollower(p, false);
            RoiRect2 line1 = transformer.Transform(
                p.hv_Line1_RoiMidRow, p.hv_Line1_RoiMidColumn,
                p.hv_Line1_RoiAngle, p.hv_Line1_RoiHeight,
                p.hv_Line1_RoiWidth, p.hv_Line1_RoiPointsNum);
            RoiRect2 line2 = transformer.Transform(
                p.hv_Line2_RoiMidRow, p.hv_Line2_RoiMidColumn,
                p.hv_Line2_RoiAngle, p.hv_Line2_RoiHeight,
                p.hv_Line2_RoiWidth, p.hv_Line2_RoiPointsNum);

            window.SetDraw("margin");
            window.SetLineWidth(2);
            DrawRect2(window, line1, "green", "抓边1");
            DrawRect2(window, line2, "cyan", "抓边2");
        }

        private static void DrawRect2(HWindow window, RoiRect2 roi, string color, string text)
        {
            window.SetColor(color);
            HOperatorSet.TupleRad(roi.AngleDeg, out HTuple phi);
            HOperatorSet.DispRectangle2(window, roi.Row, roi.Col, -phi, roi.Height / 2.0, roi.Width / 2.0);
            window.SetTposition((int)Math.Round(roi.Row + 10), (int)Math.Round(roi.Col + 10));
            window.WriteString(text);
        }

        private static void DrawFitResult(HWindow window, ParamsFindIntersection p, double crossSize)
        {
            if (p.hv_Line1_CrossRow != null)
            {
                DrawCross(window, p.hv_Line1_CrossRow, p.hv_Line1_CrossColumn, crossSize, "green");
            }

            if (p.hv_Line1_RowBegin != null)
            {
                DrawLine(window, p.hv_Line1_RowBegin, p.hv_Line1_ColumnBegin, p.hv_Line1_RowEnd, p.hv_Line1_ColumnEnd, "yellow");
            }

            if (p.hv_Line2_CrossRow != null)
            {
                DrawCross(window, p.hv_Line2_CrossRow, p.hv_Line2_CrossColumn, crossSize, "cyan");
            }

            if (p.hv_Line2_RowBegin != null)
            {
                DrawLine(window, p.hv_Line2_RowBegin, p.hv_Line2_ColumnBegin, p.hv_Line2_RowEnd, p.hv_Line2_ColumnEnd, "magenta");
            }

            if (p.hv_IntersectionRow != null && p.hv_IsParallel != null && p.hv_IsParallel.I == 0)
            {
                DrawCross(window, p.hv_IntersectionRow, p.hv_IntersectionColumn, 30, "red");
                window.SetTposition((int)Math.Round(p.hv_IntersectionRow.D + 16), (int)Math.Round(p.hv_IntersectionColumn.D + 16));
                window.WriteString("交点");
            }
        }

        private static void DrawCross(HWindow window, HTuple row, HTuple column, double size, string color)
        {
            HObject cross = null;
            try
            {
                HOperatorSet.GenCrossContourXld(out cross, row, column, size, 0);
                window.SetColor(color);
                window.DispObj(cross);
            }
            finally
            {
                if (cross != null) cross.Dispose();
            }
        }

        private static void DrawLine(HWindow window, HTuple row1, HTuple col1, HTuple row2, HTuple col2, string color)
        {
            HObject line = null;
            try
            {
                HOperatorSet.GenRegionLine(out line, row1, col1, row2, col2);
                window.SetColor(color);
                window.DispObj(line);
            }
            finally
            {
                if (line != null) line.Dispose();
            }
        }

        private static void DrawCrosshair(HWindow window, double row, double col, bool enabled)
        {
            if (!enabled || row < 0 || col < 0) return;

            window.SetColor("orange red");
            window.SetDraw("margin");
            window.DispLine(row, 0, row, 99999);
            window.DispLine(0, col, 99999, col);
        }
    }
}
