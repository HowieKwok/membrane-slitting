using Plugin.DefectCoat.Models;
using HalconDotNet;
using System;
using Frame_Core.pluginParams;

using FrameCommunity.公共类;
namespace Plugin.DefectCoat.Core.Detection
{
    /// <summary>
    /// 位置跟随转换器。
    /// 参数界面仍使用 Rectangle1 方便手动设置；运行检测时必须用 HALCON 仿射矩阵旋转 Region，
    /// 不能把旋转后的四个点再包成最大外接矩形，否则检测范围会被错误放大。
    /// </summary>
    public sealed class DefectRoiFollowTransformer
    {
        /// <summary>本次检测请求。只读使用位姿、基准点和当前位置，不在转换器内修改请求参数。</summary>
        private readonly DefectDetectRequest request;

        /// <summary>创建位置跟随转换器，request 必须是本次检测的运行请求。</summary>
        public DefectRoiFollowTransformer(DefectDetectRequest request)
        {
            this.request = request ?? throw new ArgumentNullException(nameof(request));
        }

        /// <summary>
        /// 将一个点从基准坐标系转换到当前坐标系。
        /// 未启用位置跟随时直接返回原点；启用时按 BaseRow/BaseCol/BaseAngle 到 CurrentRow/CurrentCol/CurrentAngle 做刚性变换。
        /// </summary>
        public void TransformPoint(double row, double col, out double outRow, out double outCol)
        {
            if (!request.EnablePositionFollow)
            {
                outRow = row;
                outCol = col;
                return;
            }

            double angle = (request.CurrentAngle - request.BaseAngle) * Math.PI / 180.0;
            double dr = row - request.BaseRow;
            double dc = col - request.BaseCol;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            outRow = request.CurrentRow + dr * cos - dc * sin;
            outCol = request.CurrentCol + dr * sin + dc * cos;
        }

        /// <summary>
        /// 返回 ROI 参数副本。
        /// 这里不直接改四坐标，因为旋转后的真实检测范围要由 CreateRegion 用 HALCON 仿射生成。
        /// </summary>
        public CoatRoiParam TransformRoi(CoatRoiParam roi)
        {
            if (roi == null) return null;
            return roi.Clone();
        }

        /// <summary>
        /// 生成检测实际使用的 ROI Region。
        /// 启用位置跟随时通过 VectorAngleToRigid 生成刚性矩阵，再用 AffineTransRegion 得到真实旋转区域。
        /// </summary>
        public void CreateRegion(CoatRoiParam roi, out HObject region)
        {
            HObject baseRegion = null;
            try
            {
                CreateBaseRegion(roi, out baseRegion);
                if (!request.EnablePositionFollow)
                {
                    region = baseRegion;
                    baseRegion = null;
                    return;
                }

                HTuple homMat2d = CreateRigidHomMat2d();
                HOperatorSet.AffineTransRegion(baseRegion, out region, homMat2d, "nearest_neighbor");
            }
            finally
            {
                baseRegion?.Dispose();
            }
        }

        /// <summary>
        /// 使用 HALCON 根据基准 Row/Col/角度 和 当前 Row/Col/角度 创建刚性变换矩阵。
        /// 这里的角度单位从界面度数转换为弧度。
        /// </summary>
        public HTuple CreateRigidHomMat2d()
        {
            double baseAngleRad = request.BaseAngle * Math.PI / 180.0;
            double currentAngleRad = request.CurrentAngle * Math.PI / 180.0;
            HOperatorSet.VectorAngleToRigid(
                request.BaseRow,
                request.BaseCol,
                baseAngleRad,
                request.CurrentRow,
                request.CurrentCol,
                currentAngleRad,
                out HTuple homMat2d);
            return homMat2d;
        }

        /// <summary>
        /// 根据 Rectangle1 四坐标创建未跟随前的基础 Region。
        /// 当前模块不再开放 Rectangle2/圆形 ROI，避免无效参数参与保存和界面绑定。
        /// </summary>
        private static void CreateBaseRegion(CoatRoiParam roi, out HObject region)
        {
            if (roi == null)
            {
                HOperatorSet.GenEmptyRegion(out region);
                return;
            }

            HOperatorSet.GenRectangle1(
                out region,
                Math.Min(roi.Row1, roi.Row2),
                Math.Min(roi.Col1, roi.Col2),
                Math.Max(roi.Row1, roi.Row2),
                Math.Max(roi.Col1, roi.Col2));
        }
    }
}

