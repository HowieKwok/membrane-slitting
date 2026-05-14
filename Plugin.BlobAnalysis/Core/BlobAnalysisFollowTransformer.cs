using Plugin.BlobAnalysis.Models;
using System;

namespace Plugin.BlobAnalysis.Core
{
    public sealed class BlobAnalysisFollowTransformer
    {
        private readonly BlobAnalysisParams parameters;

        public BlobAnalysisFollowTransformer(BlobAnalysisParams parameters)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public void TransformPoint(double row, double col, out double outRow, out double outCol)
        {
            if (!parameters.EnablePositionFollow)
            {
                outRow = row;
                outCol = col;
                return;
            }

            double angle = (parameters.CurrentAngle - parameters.BaseAngle) * Math.PI / 180.0;
            double dr = row - parameters.BaseRow;
            double dc = col - parameters.BaseCol;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            outRow = parameters.CurrentRow + dr * cos - dc * sin;
            outCol = parameters.CurrentCol + dr * sin + dc * cos;
        }


        public RoiParam TransformRoi(RoiParam roi)
        {
            if (roi.Shape == RoiShapeType.Rectangle1)
            {
                return TransformRect(roi);
            }

            TransformPoint(roi.CenterRow, roi.CenterCol, out double centerRow, out double centerCol);
            RoiParam result = roi.Clone();
            result.CenterRow = centerRow;
            result.CenterCol = centerCol;
            if (parameters.EnablePositionFollow && roi.Shape == RoiShapeType.Rectangle2)
            {
                result.Angle = roi.Angle + (parameters.CurrentAngle - parameters.BaseAngle) * Math.PI / 180.0;
            }
            return result;
        }
        public RoiParam TransformRect(RoiParam roi)
        {
            TransformPoint(roi.Row1, roi.Col1, out double r1, out double c1);
            TransformPoint(roi.Row1, roi.Col2, out double r2, out double c2);
            TransformPoint(roi.Row2, roi.Col1, out double r3, out double c3);
            TransformPoint(roi.Row2, roi.Col2, out double r4, out double c4);
            return new RoiParam
            {
                Enabled = roi.Enabled,
                Index = roi.Index,
                Name = roi.Name,
                Shape = roi.Shape,
                Row1 = Math.Min(Math.Min(r1, r2), Math.Min(r3, r4)),
                Row2 = Math.Max(Math.Max(r1, r2), Math.Max(r3, r4)),
                Col1 = Math.Min(Math.Min(c1, c2), Math.Min(c3, c4)),
                Col2 = Math.Max(Math.Max(c1, c2), Math.Max(c3, c4))
            };
        }
    }
}


