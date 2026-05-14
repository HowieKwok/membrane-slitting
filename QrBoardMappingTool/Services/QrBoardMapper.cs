using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HalconDotNet;
using QrBoardMappingTool.Models;

namespace QrBoardMappingTool.Services
{
    public sealed class QrBoardMapper
    {
        public BoardCoordinateTransform Transform { get; private set; }
        public BoardMappingResult LastResult { get; private set; } = new BoardMappingResult();

        public BoardMappingResult DetectAndMap(HObject image, HObject roiRegion, BoardMappingSettings settings)
        {
            var result = new BoardMappingResult();
            LastResult = result;
            Transform = null;

            if (image == null || !image.IsInitialized())
            {
                result.Message = "Image is empty.";
                return result;
            }

            HObject imageForCode = null;
            HObject symbolXlds = null;
            HTuple dataCodeHandle = null;

            try
            {
                if (roiRegion != null && roiRegion.IsInitialized())
                {
                    HOperatorSet.ReduceDomain(image, roiRegion, out imageForCode);
                }
                else
                {
                    imageForCode = image.Clone();
                }

                HOperatorSet.CreateDataCode2dModel(
                    "QR Code",
                    new HTuple("default_parameters"),
                    new HTuple(settings.QrDefaultParameters),
                    out dataCodeHandle);

                if (!string.IsNullOrWhiteSpace(settings.QrPolarity))
                {
                    HOperatorSet.SetDataCode2dParam(dataCodeHandle, "polarity", settings.QrPolarity);
                }

                HOperatorSet.FindDataCode2d(
                    imageForCode,
                    out symbolXlds,
                    dataCodeHandle,
                    new HTuple("stop_after_result_num"),
                    new HTuple(settings.StopAfterResultNum),
                    out HTuple resultHandles,
                    out HTuple decodedStrings);

                HOperatorSet.CountObj(symbolXlds, out HTuple countTuple);
                int count = countTuple.I;
                result.DetectedCount = count;

                var points = new List<QrBoardPoint>();
                for (int i = 1; i <= count; i++)
                {
                    HObject oneSymbol = null;
                    try
                    {
                        HOperatorSet.SelectObj(symbolXlds, out oneSymbol, i);
                        HOperatorSet.SmallestRectangle2Xld(
                            oneSymbol,
                            out HTuple row,
                            out HTuple column,
                            out HTuple phi,
                            out HTuple length1,
                            out HTuple length2);

                        string decoded = GetDecodedString(decodedStrings, i - 1);
                        if (!TryParseIndex(decoded, settings, out int serialNumber, out int boardRow, out int boardColumn))
                        {
                            continue;
                        }

                        if (boardRow < 0 || boardColumn < 0 || boardRow >= settings.BoardRows || boardColumn >= settings.BoardColumns)
                        {
                            continue;
                        }

                        double imageRow = row.D;
                        double imageColumn = column.D;
                        if (settings.CenterSource == QrCenterSource.DataCodeCenter && resultHandles != null && resultHandles.Length >= i)
                        {
                            TryReadDataCodeCenter(dataCodeHandle, resultHandles[i - 1], ref imageRow, ref imageColumn);
                        }

                        var point = new QrBoardPoint
                        {
                            Decoded = decoded,
                            SerialNumber = serialNumber,
                            RowIndex = boardRow,
                            ColumnIndex = boardColumn,
                            DetectedImageRow = imageRow,
                            DetectedImageColumn = imageColumn,
                            NominalImageRow = settings.PixelRowFromIndex(boardRow, boardColumn),
                            NominalImageColumn = settings.PixelColumnFromIndex(boardRow, boardColumn),
                            ImageRow = imageRow,
                            ImageColumn = imageColumn,
                            PhiRad = phi.D,
                            WorldX = settings.PhysicalXFromIndex(boardRow, boardColumn),
                            WorldY = settings.PhysicalYFromIndex(boardRow, boardColumn),
                            Used = true
                        };
                        point.PixelError = Math.Sqrt(Sqr(point.DetectedImageRow - point.NominalImageRow) + Sqr(point.DetectedImageColumn - point.NominalImageColumn));

                        points.Add(point);
                    }
                    finally
                    {
                        oneSymbol?.Dispose();
                    }
                }

                points = points
                    .GroupBy(p => p.SerialNumber)
                    .Select(g => g.OrderBy(p => p.PixelError).First())
                    .ToList();

                foreach (QrBoardPoint p in points.OrderBy(p => p.RowIndex).ThenBy(p => p.ColumnIndex))
                {
                    result.Points.Add(p);
                }

                FitResult(result, settings.MappingModel);
                return result;
            }
            catch (HalconException ex)
            {
                result.Message = "HALCON错误: " + ex.Message;
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                imageForCode?.Dispose();
                symbolXlds?.Dispose();
                if (dataCodeHandle != null)
                {
                    try { HOperatorSet.ClearDataCode2dModel(dataCodeHandle); } catch { }
                }
            }
        }

        public void Refit(BoardMappingResult result, BoardMappingModelKind mappingModel)
        {
            Transform = null;
            FitResult(result, mappingModel);
        }

        public bool TryPixelToWorld(double row, double column, out double worldX, out double worldY, out string message)
        {
            worldX = 0;
            worldY = 0;
            message = string.Empty;

            if (Transform == null)
            {
                message = "映射未完成，请先识别并拟合。";
                return false;
            }

            try
            {
                Transform.PixelToWorld(row, column, out worldX, out worldY);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public bool TryWorldToPixel(double worldX, double worldY, out double row, out double column, out string message)
        {
            row = 0;
            column = 0;
            message = string.Empty;

            if (Transform == null)
            {
                message = "映射未完成，请先识别并拟合。";
                return false;
            }

            try
            {
                Transform.WorldToPixel(worldX, worldY, out row, out column);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        private void FitResult(BoardMappingResult result, BoardMappingModelKind mappingModel)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            int minimum = 3;
            var used = result.Points.Where(p => p.Used).ToList();
            result.UsedCount = used.Count;

            if (used.Count < minimum)
            {
                result.Success = false;
                result.Message = $"至少需要 {minimum} 个已识别二维码参与 VectorToHomMat2d 拟合，当前 {used.Count} 个。";
                return;
            }

            Transform = BoardCoordinateTransform.Fit(result.Points.ToList(), mappingModel);

            double sum = 0;
            double sumSq = 0;
            double max = 0;
            foreach (QrBoardPoint point in used)
            {
                Transform.PixelToWorld(point.ImageRow, point.ImageColumn, out double x, out double y);
                point.FittedX = x;
                point.FittedY = y;
                point.ErrorMm = Math.Sqrt(Sqr(x - point.WorldX) + Sqr(y - point.WorldY));
                sum += point.ErrorMm;
                sumSq += point.ErrorMm * point.ErrorMm;
                max = Math.Max(max, point.ErrorMm);
            }

            result.AvgErrorMm = sum / used.Count;
            result.RmsErrorMm = Math.Sqrt(sumSq / used.Count);
            result.MaxErrorMm = max;
            result.UsedCount = used.Count;
            result.Success = true;
            result.Message = $"完成: {used.Count} 个二维码按序号绑定参与 HALCON VectorToHomMat2d 拟合。";
        }

        private static double Sqr(double value) => value * value;

        private static string GetDecodedString(HTuple decodedStrings, int index)
        {
            if (decodedStrings == null || index < 0 || decodedStrings.Length <= index)
            {
                return string.Empty;
            }

            return decodedStrings[index].S;
        }

        private static bool TryParseIndex(string decoded, BoardMappingSettings settings, out int serialNumber, out int row, out int column)
        {
            serialNumber = -1;
            row = -1;
            column = -1;
            if (string.IsNullOrWhiteSpace(decoded)) return false;

            var numbers = Regex.Matches(decoded, @"\d+")
                .Cast<Match>()
                .Select(m => int.Parse(m.Value, CultureInfo.InvariantCulture))
                .ToList();

            if (numbers.Count >= 1)
            {
                serialNumber = numbers[0];
                return settings.TrySerialToBoardIndex(serialNumber, out row, out column);
            }

            return false;
        }

        private static void TryReadDataCodeCenter(HTuple dataCodeHandle, HTuple resultHandle, ref double imageRow, ref double imageColumn)
        {
            try
            {
                HOperatorSet.GetDataCode2dResults(dataCodeHandle, resultHandle, "center_row", out HTuple row);
                HOperatorSet.GetDataCode2dResults(dataCodeHandle, resultHandle, "center_column", out HTuple column);
                if (row.Length > 0 && column.Length > 0)
                {
                    imageRow = row.D;
                    imageColumn = column.D;
                }
            }
            catch
            {
                // Some HALCON versions/data-code results do not expose center_row/center_column.
            }
        }

    }
}
