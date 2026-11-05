using System;

namespace FrameCommunity.MaskRoiTool
{
    public sealed class MaskRoiFollowContext
    {
        public bool EnablePositionFollow { get; set; }
        public double BaseRow { get; set; }
        public double BaseCol { get; set; }
        public double BaseAngleDeg { get; set; }
        public double CurrentRow { get; set; }
        public double CurrentCol { get; set; }
        public double CurrentAngleDeg { get; set; }
    }

    public sealed class MaskRoiFollowTransformer
    {
        private readonly MaskRoiFollowContext context;

        public MaskRoiFollowTransformer(MaskRoiFollowContext context)
        {
            this.context = context ?? new MaskRoiFollowContext();
        }

        public MaskRoiParam Transform(MaskRoiParam roi)
        {
            if (roi == null) return null;
            if (!context.EnablePositionFollow) return roi.Clone();

            MaskRoiParam result = roi.Clone();
            switch (roi.Shape)
            {
                case MaskRoiShapeType.Rectangle2:
                    TransformPoint(roi.CenterRow, roi.CenterCol, out double rect2Row, out double rect2Col);
                    result.CenterRow = rect2Row;
                    result.CenterCol = rect2Col;
                    result.Angle = roi.Angle + DegToRad(context.CurrentAngleDeg - context.BaseAngleDeg);
                    break;
                case MaskRoiShapeType.Circle:
                    TransformPoint(roi.CenterRow, roi.CenterCol, out double circleRow, out double circleCol);
                    result.CenterRow = circleRow;
                    result.CenterCol = circleCol;
                    break;
                default:
                    TransformRectangle1(roi, result);
                    break;
            }
            return result;
        }

        public void TransformPoint(double row, double col, out double outRow, out double outCol)
        {
            if (!context.EnablePositionFollow)
            {
                outRow = row;
                outCol = col;
                return;
            }

            double angle = DegToRad(context.CurrentAngleDeg - context.BaseAngleDeg);
            double dr = row - context.BaseRow;
            double dc = col - context.BaseCol;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            outRow = context.CurrentRow + dr * cos - dc * sin;
            outCol = context.CurrentCol + dr * sin + dc * cos;
        }

        private void TransformRectangle1(MaskRoiParam source, MaskRoiParam result)
        {
            TransformPoint(source.Row1, source.Col1, out double r1, out double c1);
            TransformPoint(source.Row1, source.Col2, out double r2, out double c2);
            TransformPoint(source.Row2, source.Col1, out double r3, out double c3);
            TransformPoint(source.Row2, source.Col2, out double r4, out double c4);
            result.Row1 = Math.Min(Math.Min(r1, r2), Math.Min(r3, r4));
            result.Row2 = Math.Max(Math.Max(r1, r2), Math.Max(r3, r4));
            result.Col1 = Math.Min(Math.Min(c1, c2), Math.Min(c3, c4));
            result.Col2 = Math.Max(Math.Max(c1, c2), Math.Max(c3, c4));
            result.NormalizeFromRectangle1();
        }

        private static double DegToRad(double value)
        {
            return value * Math.PI / 180.0;
        }
    }
}
