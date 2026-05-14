using System;
using System.Collections.Generic;
using System.Linq;
using HalconDotNet;
using QrBoardMappingTool.Models;

namespace QrBoardMappingTool.Services
{
    public sealed class BoardCoordinateTransform
    {
        private readonly HTuple pixelToWorld;
        private readonly HTuple worldToPixel;

        private BoardCoordinateTransform(HTuple pixelToWorld, HTuple worldToPixel)
        {
            this.pixelToWorld = pixelToWorld;
            this.worldToPixel = worldToPixel;
        }

        public static BoardCoordinateTransform Fit(IReadOnlyList<QrBoardPoint> points, BoardMappingModelKind modelKind)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));

            var used = points.Where(p => p.Used).ToList();
            if (used.Count < 3)
            {
                throw new InvalidOperationException("HALCON VectorToHomMat2d 至少需要 3 个有效点。");
            }

            HTuple srcX = new HTuple(used.Select(p => p.ImageColumn).ToArray());
            HTuple srcY = new HTuple(used.Select(p => p.ImageRow).ToArray());
            HTuple dstX = new HTuple(used.Select(p => p.WorldX).ToArray());
            HTuple dstY = new HTuple(used.Select(p => p.WorldY).ToArray());

            HOperatorSet.VectorToHomMat2d(srcX, srcY, dstX, dstY, out HTuple pixelToWorld);
            HOperatorSet.VectorToHomMat2d(dstX, dstY, srcX, srcY, out HTuple worldToPixel);
            return new BoardCoordinateTransform(pixelToWorld, worldToPixel);
        }

        public void PixelToWorld(double imageRow, double imageColumn, out double worldX, out double worldY)
        {
            HOperatorSet.AffineTransPoint2d(pixelToWorld, imageColumn, imageRow, out HTuple x, out HTuple y);
            worldX = x.D;
            worldY = y.D;
        }

        public void WorldToPixel(double worldX, double worldY, out double imageRow, out double imageColumn)
        {
            HOperatorSet.AffineTransPoint2d(worldToPixel, worldX, worldY, out HTuple col, out HTuple row);
            imageRow = row.D;
            imageColumn = col.D;
        }
    }
}
