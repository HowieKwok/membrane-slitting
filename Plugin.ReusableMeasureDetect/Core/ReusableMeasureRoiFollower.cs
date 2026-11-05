using Frame_Core.pluginParams;
using HalconDotNet;

namespace Plugin.ReusableMeasureDetect.Core
{
    /// <summary>
    /// ROI 跟随后实际参与抓边的矩形描述。
    /// 坐标单位与 HALCON 图像坐标一致：Row/Y 在前，Col/X 在后。
    /// </summary>
    internal struct ReusableMeasureRoiRect2
    {
        public double Row;
        public double Col;
        public double AngleDeg;
        public double HalfLength1;
        public double HalfLength2;
        public int PointCount;
    }

    /// <summary>
    /// 位置跟随计算器。
    /// 根据基准位姿和当前位姿生成刚体变换，将界面 ROI 映射到运行时 ROI，保持与 MeasureLine 插件的位姿跟随逻辑一致。
    /// </summary>
    internal sealed class ReusableMeasureRoiFollower
    {
        private readonly ParamsReusableMeasureDetect parameters;
        private readonly double deltaAngleDeg;
        private readonly HTuple homMat2D;

        /// <summary>
        /// 构造跟随器，并在需要时把当前跟随偏差写回参数，供界面状态栏和输出结果显示。
        /// </summary>
        /// <param name="parameters">插件参数，提供基准位姿、当前位姿和跟随开关。</param>
        /// <param name="writeOffset">是否把 X/Y/R 偏差写回参数。</param>
        public ReusableMeasureRoiFollower(ParamsReusableMeasureDetect parameters, bool writeOffset)
        {
            this.parameters = parameters;
            if (parameters == null || !parameters.m_bUsePositionFollow)
            {
                deltaAngleDeg = 0;
                homMat2D = null;
                if (parameters != null && writeOffset)
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

            //HOperatorSet.VectorAngleToRigid(
            //   parameters.m_fCurrentRow,
            //   parameters.m_fCurrentCol,
            //   ToRad(parameters.m_fCurrentAngle),
            //    parameters.m_fBaseRow,
            //    parameters.m_fBaseCol,
            //    ToRad(parameters.m_fBaseAngle),
            //   out homMat2D);
        }

        /// <summary>
        /// 对 ROI 中心点和角度应用跟随变换；半宽、半高和抓边点数不随位姿改变。
        /// </summary>
        /// <param name="row">原始 ROI 中心 Row。</param>
        /// <param name="col">原始 ROI 中心 Col。</param>
        /// <param name="angleDeg">原始 ROI 角度，单位为度。</param>
        /// <param name="halfLength1">沿 ROI 主轴方向的半长。</param>
        /// <param name="halfLength2">沿 ROI 法向方向的半宽。</param>
        /// <param name="pointCount">MeasureLine 采样点数。</param>
        /// <returns>启用且当前位姿有效时返回跟随后的 ROI，否则返回原始 ROI。</returns>
        public ReusableMeasureRoiRect2 Transform(double row, double col, double angleDeg, double halfLength1, double halfLength2, int pointCount)
        {
            ReusableMeasureRoiRect2 result = new ReusableMeasureRoiRect2
            {
                Row = row,
                Col = col,
                AngleDeg = angleDeg,
                HalfLength1 = halfLength1,
                HalfLength2 = halfLength2,
                PointCount = pointCount
            };

            if (parameters == null || !parameters.m_bUsePositionFollow || !parameters.m_bCurrentPoseValid || homMat2D == null)
                return result;

            HOperatorSet.AffineTransPoint2d(homMat2D, row, col, out HTuple transformedRow, out HTuple transformedCol);
            result.Row = transformedRow.D;
            result.Col = transformedCol.D;
            result.AngleDeg = angleDeg + deltaAngleDeg;
            return result;
        }

        /// <summary>
        /// 使用 HALCON 的角度转换函数把角度从度转为弧度，避免与 HALCON 坐标系计算不一致。
        /// </summary>
        private static double ToRad(double angleDeg)
        {
            HOperatorSet.TupleRad(angleDeg, out HTuple rad);
            return rad.D;
        }
    }
}
