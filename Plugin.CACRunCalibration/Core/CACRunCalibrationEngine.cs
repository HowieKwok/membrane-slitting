using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.CACRunCalibration.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plugin.CACRunCalibration.Core
{
    /// <summary>
    /// CAC运行标定核心引擎。
    /// 
    /// 职责：
    /// 1. 根据界面参数识别二维码。
    /// 2. 根据二维码序号生成对应棋盘格物理坐标。
    /// 3. 将像素坐标和棋盘坐标一一配对生成矩阵。
    /// 4. 运行时直接读取参数类中的矩阵数据做坐标转换。
    /// 
    /// 注意：
    /// 运行时不缓存矩阵、不创建 BoardCoordinateTransform。
    /// 矩阵由界面生成后写入 ParamsCACRunCalibration，
    /// 保存参数时随 XML 一起保存。
    /// 后台执行时只读参数类里的 m_lPixelToBoardMatrix / m_lBoardToPixelMatrix。
    /// </summary>
    public sealed class CACRunCalibrationEngine
    {
        /// <summary>
        /// 检查参数中是否存在完整矩阵。
        /// 
        /// 这里只判断参数里有没有矩阵数据，不做缓存、不创建转换对象。
        /// </summary>
        public bool HasValidMatrix(ParamsCACRunCalibration param, out string message)
        {
            message = string.Empty;

            if (param == null)
            {
                message = "参数对象为空";
                return false;
            }

            if (!HasMatrixData(param))
            {
                message = "没有有效矩阵：参数中没有完整矩阵数据，请在插件界面生成矩阵并保存参数";
                param.m_bHasValidMatrix = false;
                param.m_strMatrixStateText = "无有效矩阵";
                param.m_strMatrixCheckMessage = message;
                return false;
            }

            // ★ 关键补充：确保 HTuple 字段被同步
            param.m_hvPixelToBoardHomMat = new HTuple(param.m_lPixelToBoardMatrix.ToArray());
            param.m_hvBoardToPixelHomMat = new HTuple(param.m_lBoardToPixelMatrix.ToArray());


            param.m_bHasValidMatrix = true;
            param.m_strMatrixStateText = "矩阵有效";
            param.m_strMatrixCheckMessage = "矩阵有效";
            message = "矩阵有效";
            return true;
        }

        /// <summary>
        /// 像素坐标转换为棋盘物理坐标。
        /// 
        /// 输入：
        /// row = 像素行坐标
        /// col = 像素列坐标
        /// 
        /// HALCON AffineTransPoint2d 使用的是 X/Y，
        /// 对图像来说 X=Column，Y=Row，
        /// 所以这里传入顺序是 col, row。
        /// </summary>
        public bool TryPixelToBoard(
            ParamsCACRunCalibration param,
            double row,
            double col,
            out double boardX,
            out double boardY,
            out string message)
        {
            boardX = 0;
            boardY = 0;
            message = string.Empty;

            if (!HasValidMatrix(param, out message))
            {
                return false;
            }

            try
            {
                HTuple pixelToBoard = new HTuple(param.m_lPixelToBoardMatrix.ToArray());

                HOperatorSet.AffineTransPoint2d(
                    pixelToBoard,
                    col,
                    row,
                    out HTuple x,
                    out HTuple y);

                boardX = x.D;
                boardY = y.D;

                //param.m_hvPixelToBoardHomMat = pixelToBoard;

                message = string.Format(
                    CultureInfo.InvariantCulture,
                    "转换成功：X={0:F4}, Y={1:F4}",
                    boardX,
                    boardY);

                return true;
            }
            catch (Exception ex)
            {
                message = "矩阵报错：像素坐标转换失败，" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 棋盘物理坐标转换为像素坐标。
        /// 
        /// 输入：
        /// x = 棋盘物理X坐标
        /// y = 棋盘物理Y坐标
        /// 
        /// 输出：
        /// row = 像素行坐标
        /// col = 像素列坐标
        /// </summary>
        public bool TryBoardToPixel(
            ParamsCACRunCalibration param,
            double x,
            double y,
            out double row,
            out double col,
            out string message)
        {
            row = 0;
            col = 0;
            message = string.Empty;

            if (!HasValidMatrix(param, out message))
            {
                return false;
            }

            try
            {
                HTuple boardToPixel = new HTuple(param.m_lBoardToPixelMatrix.ToArray());

                HOperatorSet.AffineTransPoint2d(
                    boardToPixel,
                    x,
                    y,
                    out HTuple pixelCol,
                    out HTuple pixelRow);

                col = pixelCol.D;
                row = pixelRow.D;

               // param.m_hvBoardToPixelHomMat = boardToPixel;

                message = string.Format(
                    CultureInfo.InvariantCulture,
                    "转换成功：Row={0:F4}, Col={1:F4}",
                    row,
                    col);

                return true;
            }
            catch (Exception ex)
            {
                message = "矩阵报错：棋盘坐标转换失败，" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从图像中识别二维码，并根据识别点生成像素到棋盘坐标的矩阵。
        /// 
        /// 矩阵生成成功后会写入：
        /// param.m_lPixelToBoardMatrix
        /// param.m_lBoardToPixelMatrix
        /// 
        /// 这两个 List<double> 会随参数 XML 保存。
        /// </summary>
        public bool GenerateMatrix(ParamsCACRunCalibration param, IList<CalibrationPointRow> pointRows, out string message)
        {
            message = string.Empty;

            if (param == null)
            {
                message = "参数对象为空";
                return false;
            }

            if (param.ho_Image == null || !param.ho_Image.IsInitialized())
            {
                message = "输入图像为空，无法生成矩阵";
                return false;
            }

            HObject symbolXlds = null;

            try
            {
                List<CalibrationPointRow> detectedRows = DetectCalibrationPoints(param, out symbolXlds, out message);

                if (pointRows != null)
                {
                    pointRows.Clear();
                    foreach (CalibrationPointRow row in detectedRows)
                    {
                        pointRows.Add(row);
                    }
                }

                if (param.m_ho_OutQrXlds != null)
                {
                    param.m_ho_OutQrXlds.Dispose();
                    param.m_ho_OutQrXlds = null;
                }

                param.m_ho_OutQrXlds = symbolXlds;

                bool ok = FitMatrixFromRows(param, pointRows ?? detectedRows, out message);
                if (ok)
                {
                    param.m_strRunMessage = param.m_strMatrixCheckMessage + "；矩阵已写入参数，随参数XML保存";
                    message = param.m_strRunMessage;
                }

                return ok;
            }
            catch (Exception ex)
            {
                message = "矩阵报错：" + ex.Message;
                param.m_bHasValidMatrix = false;
                param.m_strMatrixStateText = "矩阵报错";
                param.m_strMatrixCheckMessage = message;
                param.m_strRunMessage = message;
                return false;
            }
        }

        public bool GenerateMatrix(ParamsCACRunCalibration param, out string message)
        {
            return GenerateMatrix(param, null, out message);
        }

        /// <summary>
        /// 根据界面表格中勾选的点重新拟合矩阵。
        /// </summary>
        public bool RefitMatrixFromRows(ParamsCACRunCalibration param, IList<CalibrationPointRow> pointRows, out string message)
        {
            bool ok = FitMatrixFromRows(param, pointRows, out message);
            if (ok)
            {
                param.m_strRunMessage = param.m_strMatrixCheckMessage + "；矩阵已写入参数，随参数XML保存";
                message = param.m_strRunMessage;
            }

            return ok;
        }

        /// <summary>
        /// 识别二维码并构建点表。
        /// 
        /// 每一个 CalibrationPointRow 表示：
        /// 一个二维码序号
        /// 一个像素坐标 ImageRow/ImageColumn
        /// 一个棋盘物理坐标 WorldX/WorldY
        /// </summary>
        public List<CalibrationPointRow> DetectCalibrationPoints(
            ParamsCACRunCalibration param,
            out HObject symbolXlds,
            out string message)
        {
            symbolXlds = null;
            message = string.Empty;

            List<CalibrationPointRow> rows = new List<CalibrationPointRow>();
            HObject imageForCode = null;
            HTuple dataCodeHandle = null;

            try
            {
                if (param.m_bEnableRoi)
                {
                    HObject roi = null;

                    try
                    {
                        HOperatorSet.GenRectangle1(
                            out roi,
                            Math.Min(param.m_fRoiRow1, param.m_fRoiRow2),
                            Math.Min(param.m_fRoiCol1, param.m_fRoiCol2),
                            Math.Max(param.m_fRoiRow1, param.m_fRoiRow2),
                            Math.Max(param.m_fRoiCol1, param.m_fRoiCol2));

                        HOperatorSet.ReduceDomain(param.ho_Image, roi, out imageForCode);
                    }
                    finally
                    {
                        if (roi != null) roi.Dispose();
                    }
                }
                else
                {
                    imageForCode = param.ho_Image.Clone();
                }

                HOperatorSet.CreateDataCode2dModel(
                    "QR Code",
                    new HTuple("default_parameters"),
                    new HTuple(param.m_strQrDefaultParameters),
                    out dataCodeHandle);

                if (!string.IsNullOrWhiteSpace(param.m_strQrPolarity))
                {
                    HOperatorSet.SetDataCode2dParam(dataCodeHandle, "polarity", param.m_strQrPolarity);
                }

                HOperatorSet.FindDataCode2d(
                    imageForCode,
                    out symbolXlds,
                    dataCodeHandle,
                    new HTuple("stop_after_result_num"),
                    new HTuple(param.m_nStopAfterResultNum),
                    out HTuple resultHandles,
                    out HTuple decodedStrings);

                HOperatorSet.CountObj(symbolXlds, out HTuple countTuple);
                param.m_nDetectedCount = countTuple.I;

                rows = BuildSampleRows(
                    param,
                    symbolXlds,
                    decodedStrings,
                    dataCodeHandle,
                    resultHandles,
                    param.m_nDetectedCount);

                message = string.Format(
                    CultureInfo.InvariantCulture,
                    "识别到{0}个二维码，可参与拟合{1}个",
                    param.m_nDetectedCount,
                    rows.Count);

                return rows;
            }
            finally
            {
                if (imageForCode != null) imageForCode.Dispose();

                if (dataCodeHandle != null)
                {
                    try
                    {
                        HOperatorSet.ClearDataCode2dModel(dataCodeHandle);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 根据点表拟合矩阵。
        /// 
        /// 使用 HALCON VectorToHomMat2d 生成两个方向的仿射矩阵：
        /// 1. 像素坐标 -> 棋盘坐标
        /// 2. 棋盘坐标 -> 像素坐标
        /// 
        /// 注意：
        /// 源点 srcX/srcY 使用图像坐标：
        /// srcX = ImageColumn
        /// srcY = ImageRow
        /// </summary>
        private bool FitMatrixFromRows(
            ParamsCACRunCalibration param,
            IList<CalibrationPointRow> pointRows,
            out string message)
        {
            message = string.Empty;

            List<CalibrationPointRow> used = pointRows == null
                ? new List<CalibrationPointRow>()
                : pointRows.Where(p => p.Used).ToList();

            if (used.Count < 3)
            {
                message = string.Format(
                    CultureInfo.InvariantCulture,
                    "至少需要3个已勾选点参与拟合，当前{0}个",
                    used.Count);

                param.m_bHasValidMatrix = false;
                param.m_strMatrixStateText = "无有效矩阵";
                param.m_strMatrixCheckMessage = message;
                param.m_strRunMessage = message;
                param.m_nUsedCount = used.Count;
                return false;
            }

            try
            {
                HTuple srcX = new HTuple(used.Select(p => p.ImageColumn).ToArray());
                HTuple srcY = new HTuple(used.Select(p => p.ImageRow).ToArray());
                HTuple dstX = new HTuple(used.Select(p => p.WorldX).ToArray());
                HTuple dstY = new HTuple(used.Select(p => p.WorldY).ToArray());

                HOperatorSet.VectorToHomMat2d(srcX, srcY, dstX, dstY, out HTuple pixelToBoard);
                HOperatorSet.VectorToHomMat2d(dstX, dstY, srcX, srcY, out HTuple boardToPixel);

                UpdateHomMat(param, pixelToBoard, boardToPixel);
                UpdateErrorInfo(param, used, pixelToBoard);

                param.m_bHasValidMatrix = true;
                param.m_strMatrixStateText = "矩阵有效";
                param.m_strLastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                param.m_strMatrixCheckMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "矩阵生成成功，参与点{0}个，RMS={1:F4}mm",
                    used.Count,
                    param.m_fRmsErrorMm);

                message = param.m_strMatrixCheckMessage;
                return true;
            }
            catch (Exception ex)
            {
                message = "矩阵报错：" + ex.Message;
                param.m_bHasValidMatrix = false;
                param.m_strMatrixStateText = "矩阵报错";
                param.m_strMatrixCheckMessage = message;
                param.m_strRunMessage = message;
                return false;
            }
        }

        /// <summary>
        /// 把 HALCON 生成的 HTuple 矩阵写入参数类。
        /// 
        /// List<double> 用于 XML 保存。
        /// HTuple 用于当前界面运行时直接使用。
        /// </summary>
        private static void UpdateHomMat(
            ParamsCACRunCalibration param,
            HTuple pixelToBoard,
            HTuple boardToPixel)
        {
            param.m_lPixelToBoardMatrix = new List<double>();
            param.m_lBoardToPixelMatrix = new List<double>();

            for (int i = 0; i < pixelToBoard.Length; i++)
            {
                param.m_lPixelToBoardMatrix.Add(pixelToBoard[i].D);
            }

            for (int i = 0; i < boardToPixel.Length; i++)
            {
                param.m_lBoardToPixelMatrix.Add(boardToPixel[i].D);
            }

            param.m_hvPixelToBoardHomMat = pixelToBoard;
            param.m_hvBoardToPixelHomMat = boardToPixel;
        }

        /// <summary>
        /// 计算拟合误差。
        /// 
        /// 对每个参与拟合的像素点，用 pixelToBoard 矩阵反算棋盘坐标，
        /// 再和理论棋盘坐标 WorldX/WorldY 做差。
        /// </summary>
        private static void UpdateErrorInfo(
            ParamsCACRunCalibration param,
            List<CalibrationPointRow> points,
            HTuple pixelToBoard)
        {
            double sum = 0;
            double sumSq = 0;
            double max = 0;

            foreach (CalibrationPointRow point in points)
            {
                try
                {
                    HOperatorSet.AffineTransPoint2d(
                        pixelToBoard,
                        point.ImageColumn,
                        point.ImageRow,
                        out HTuple fittedX,
                        out HTuple fittedY);

                    point.FittedX = fittedX.D;
                    point.FittedY = fittedY.D;

                    double error = Math.Sqrt(
                        Sqr(point.FittedX - point.WorldX) +
                        Sqr(point.FittedY - point.WorldY));

                    point.ErrorMm = error;

                    sum += error;
                    sumSq += error * error;
                    if (error > max) max = error;
                }
                catch
                {
                    continue;
                }
            }

            param.m_nUsedCount = points.Count;
            param.m_fAvgErrorMm = points.Count > 0 ? sum / points.Count : 0;
            param.m_fRmsErrorMm = points.Count > 0 ? Math.Sqrt(sumSq / points.Count) : 0;
            param.m_fMaxErrorMm = max;
        }

        /// <summary>
        /// 生成界面点表行。
        /// 
        /// 这里完成：
        /// 1. 读取二维码中心像素坐标。
        /// 2. 解析二维码序号。
        /// 3. 通过序号换算棋盘行列。
        /// 4. 通过棋盘行列换算物理坐标。
        /// 5. 生成 CalibrationPointRow。
        /// </summary>
        private static List<CalibrationPointRow> BuildSampleRows(
            ParamsCACRunCalibration param,
            HObject symbolXlds,
            HTuple decodedStrings,
            HTuple dataCodeHandle,
            HTuple resultHandles,
            int count)
        {
            List<CalibrationPointRow> points = new List<CalibrationPointRow>();

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

                    if (!TryParseSerial(decoded, out int serial)) continue;
                    if (!TrySerialToBoardIndex(param, serial, out int boardRow, out int boardCol)) continue;

                    double imageRow = row.D;
                    double imageCol = column.D;

                    if (string.Equals(param.m_strCenterSource, "DataCodeCenter", StringComparison.OrdinalIgnoreCase) &&
                        resultHandles != null &&
                        resultHandles.Length >= i)
                    {
                        TryReadDataCodeCenter(dataCodeHandle, resultHandles[i - 1], ref imageRow, ref imageCol);
                    }

                    double worldX = PhysicalXFromIndex(param, boardRow, boardCol);
                    double worldY = PhysicalYFromIndex(param, boardRow, boardCol);

                    double nominalRow = param.m_fOriginY + boardRow * param.m_fSpacingY + param.m_fQrPhysicalHeight / 2.0;
                    double nominalColumn = param.m_fOriginX + boardCol * param.m_fSpacingX + param.m_fQrPhysicalWidth / 2.0;

                    points.Add(new CalibrationPointRow
                    {
                        Used = true,
                        Decoded = decoded,
                        SerialNumber = serial,
                        BoardRow = boardRow,
                        BoardColumn = boardCol,
                        ImageRow = imageRow,
                        ImageColumn = imageCol,
                        DetectedRow = imageRow,
                        DetectedColumn = imageCol,
                        NominalRow = nominalRow,
                        NominalColumn = nominalColumn,
                        PixelError = Math.Sqrt(Sqr(imageRow - nominalRow) + Sqr(imageCol - nominalColumn)),
                        PhiRad = phi.D,
                        WorldX = worldX,
                        WorldY = worldY
                    });
                }
                finally
                {
                    if (oneSymbol != null) oneSymbol.Dispose();
                }
            }

            return points.GroupBy(p => p.SerialNumber)
                         .Select(g => g.First())
                         .OrderBy(p => p.BoardRow)
                         .ThenBy(p => p.BoardColumn)
                         .ToList();
        }

        /// <summary>
        /// 判断参数类中是否存在完整、有效的矩阵 List。
        /// </summary>
        private static bool HasMatrixData(ParamsCACRunCalibration param)
        {
            return param != null &&
                   IsFiniteMatrix(param.m_lPixelToBoardMatrix) &&
                   IsFiniteMatrix(param.m_lBoardToPixelMatrix);
        }

        private static bool IsFiniteMatrix(List<double> values)
        {
            if (values == null || values.Count != 6) return false;

            for (int i = 0; i < values.Count; i++)
            {
                double v = values[i];
                if (double.IsNaN(v) || double.IsInfinity(v)) return false;
            }

            return true;
        }

        private static string GetDecodedString(HTuple decodedStrings, int index)
        {
            if (decodedStrings == null || index < 0 || decodedStrings.Length <= index)
            {
                return string.Empty;
            }

            return decodedStrings[index].S;
        }

        private static bool TryParseSerial(string decoded, out int serial)
        {
            serial = -1;

            if (string.IsNullOrWhiteSpace(decoded)) return false;

            Match match = Regex.Match(decoded, @"\d+");
            return match.Success &&
                   int.TryParse(match.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out serial);
        }

        /// <summary>
        /// 二维码序号转换为棋盘行列。
        /// </summary>
        private static bool TrySerialToBoardIndex(
      ParamsCACRunCalibration param,
      int serial,
      out int rowIndex,
      out int columnIndex)
        {
            rowIndex = -1;
            columnIndex = -1;

            int zeroBased = serial - param.m_nSerialBaseIndex;
            if (zeroBased < 0 || param.m_nQrColumnStep <= 0 || param.m_nBoardColumns <= 0) return false;

            // ★ 改动点：原先是  qrRow = zeroBased / m_nQrPerRow;  qrColumnInRow = zeroBased % m_nQrPerRow;
            //   现在改为按行累计扣减（因为奇数行比偶数行少一个码）
            int qrRow = 0;
            int codeInRow = zeroBased;
            while (true)
            {
                int countInRow = QrCountInRow(param, qrRow);
                if (countInRow <= 0)
                {
                    return false;                       // 该行放不下二维码，参数异常
                }
                if (codeInRow < countInRow)
                {
                    break;
                }
                codeInRow -= countInRow;
                qrRow++;
                if (qrRow * param.m_nQrRowStep >= param.m_nBoardRows)
                {
                    return false;                       // 超出棋盘行数
                }
            }

            rowIndex = qrRow * param.m_nQrRowStep;
            columnIndex = codeInRow * param.m_nQrColumnStep;

            if ((qrRow & 1) == 1)
            {
                columnIndex += param.m_nQrOddRowColumnOffset;
            }

            return rowIndex >= 0 &&
                   columnIndex >= 0 &&
                   rowIndex < param.m_nBoardRows &&
                   columnIndex < param.m_nBoardColumns;
        }

        private static int QrCountInRow(ParamsCACRunCalibration param, int qrRow)
        {
            int offset = (qrRow & 1) == 1 ? Math.Max(0, param.m_nQrOddRowColumnOffset) : 0;
            int step = Math.Max(1, param.m_nQrColumnStep);
            int count = 0;
            for (int col = offset; col < param.m_nBoardColumns; col += step)
            {
                count++;
            }
            return count;
        }

        private static double PhysicalXFromIndex(
            ParamsCACRunCalibration param,
            int rowIndex,
            int columnIndex)
        {
            int col = param.m_bInvertColumnAxis
                ? param.m_nBoardColumns - 1 - columnIndex
                : columnIndex;

            return param.m_fOriginX +
                   col * param.m_fSpacingX +
                   param.m_fQrPhysicalWidth / 2.0;
        }

        private static double PhysicalYFromIndex(
            ParamsCACRunCalibration param,
            int rowIndex,
            int columnIndex)
        {
            int row = param.m_bInvertRowAxis
                ? param.m_nBoardRows - 1 - rowIndex
                : rowIndex;

            return param.m_fOriginY +
                   row * param.m_fSpacingY +
                   param.m_fQrPhysicalHeight / 2.0;
        }

        private static void TryReadDataCodeCenter(
            HTuple dataCodeHandle,
            HTuple resultHandle,
            ref double imageRow,
            ref double imageColumn)
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
            }
        }

        private static double Sqr(double value)
        {
            return value * value;
        }
    }
}