using System;
using Frame_Core.pluginParams;
using HalconDotNet;

namespace Plugin.FindIntersection.Core
{
    internal struct RoiRect2
    {
        public double Row;
        public double Col;
        public double AngleDeg;
        public double Height;
        public double Width;
        public int PointCount;
    }

    internal sealed class RoiFollower
    {
        private readonly ParamsFindIntersection parameters;
        private readonly double deltaAngleDeg;
        private readonly HTuple homMat2D;

        public RoiFollower(ParamsFindIntersection parameters, bool writeOffset = true)
        {
            this.parameters = parameters;
            if (parameters == null)
            {
                deltaAngleDeg = 0;
                homMat2D = null;
                return;
            }

            if (!parameters.m_bUsePositionFollow)
            {
                deltaAngleDeg = 0;
                homMat2D = null;
                if (writeOffset)
                {
                    parameters.m_fFollowOffsetY = 0;
                    parameters.m_fFollowOffsetX = 0;
                    parameters.m_fFollowOffsetR = 0;
                }
                return;
            }

            deltaAngleDeg = parameters.m_fCurrentAngle - parameters.m_fBaseAngle;
            if (writeOffset)
            {
                parameters.m_fFollowOffsetY = parameters.m_fCurrentRow - parameters.m_fBaseRow;
                parameters.m_fFollowOffsetX = parameters.m_fCurrentCol - parameters.m_fBaseCol;
                parameters.m_fFollowOffsetR = deltaAngleDeg;
            }

            HOperatorSet.VectorAngleToRigid(
                parameters.m_fBaseRow,
                parameters.m_fBaseCol,
                ToRad(parameters.m_fBaseAngle),
                parameters.m_fCurrentRow,
                parameters.m_fCurrentCol,
                ToRad(parameters.m_fCurrentAngle),
                out homMat2D);
        }

        public RoiRect2 Transform(double row, double col, double angleDeg, double height, double width, int pointCount)
        {
            RoiRect2 result = new RoiRect2
            {
                Row = row,
                Col = col,
                AngleDeg = angleDeg,
                Height = height,
                Width = width,
                PointCount = pointCount
            };

            if (parameters == null || !parameters.m_bUsePositionFollow) return result;

            HOperatorSet.AffineTransPoint2d(homMat2D, row, col, out HTuple transformedRow, out HTuple transformedCol);
            result.Row = transformedRow.D;
            result.Col = transformedCol.D;
            result.AngleDeg = angleDeg + deltaAngleDeg;
            return result;
        }

        private static double ToRad(double angleDeg)
        {
          HOperatorSet.TupleRad(angleDeg,out HTuple rad);
            return rad.D;
           // return    angleDeg * Math.PI / 180.0;
        }
    }
}