using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;
using WpfBrush = System.Windows.Media.Brush;
using WpfBrushes = System.Windows.Media.Brushes;
using WpfColor = System.Windows.Media.Color;
using WpfCursors = System.Windows.Input.Cursors;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfMouseEventArgs = System.Windows.Input.MouseEventArgs;
using WpfPoint = System.Windows.Point;
using WpfTextBox = System.Windows.Controls.TextBox;
using HalconDotNet;

namespace ConvexPeakWpfDemo;

public partial class MainWindow : Window
{
    private const string DefaultImagePath = @"C:\Users\1\Desktop\qq.jpg";
    private const string DefaultTemplateFileName = "v_template.png";

    private static readonly string[] ImageExtensions = { ".bmp", ".jpg", ".jpeg", ".png", ".tif", ".tiff" };

    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
    private readonly ObservableCollection<string> _imagePaths = new();
    private readonly ObservableCollection<SimulationResultRow> _simulationRows = new();

    private BitmapImage? _bitmap;
    private VCornerRoi _roi = new();
    private VCornerDetectionResult? _result;
    private ImagePoint? _dragStartImagePoint;
    private WpfPoint? _panStartCanvasPoint;
    private WpfPoint _panStartOffset;
    private double _viewZoom = 1.0;
    private WpfPoint _viewPanOffset;
    private Rect _displayedImageRect = Rect.Empty;
    private bool _suppressImageSelection;

    public MainWindow()
    {
        InitializeComponent();
        ImageListBox.ItemsSource = _imagePaths;
        ResultGrid.ItemsSource = _simulationRows;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        ImagePathBox.Text = DefaultImagePath;
        LoadDefaultParameters();

        if (File.Exists(DefaultImagePath))
        {
            AddImagePath(DefaultImagePath, true);
            LoadImage(DefaultImagePath, true);
            TryDetect();
        }
        else
        {
            StatusText.Text = "未找到默认图像，请选择图像。";
            UpdateStats();
        }
    }

    private void BrowseImage_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*",
            FileName = ImagePathBox.Text
        };

        if (dialog.ShowDialog() == true)
        {
            AddImagePath(dialog.FileName, true);
            LoadImage(dialog.FileName, true);
        }
    }

    private void BrowseImages_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() != true)
            return;

        foreach (string file in dialog.FileNames)
            AddImagePath(file, false);

        if (dialog.FileNames.Length > 0)
        {
            SelectImagePath(dialog.FileNames[0]);
            LoadImage(dialog.FileNames[0], true);
        }
    }

    private void BrowseFolder_Click(object sender, RoutedEventArgs e)
    {
        using WinForms.FolderBrowserDialog dialog = new()
        {
            Description = "选择仿真图片文件夹",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() != WinForms.DialogResult.OK)
            return;

        List<string> files = Directory.EnumerateFiles(dialog.SelectedPath)
            .Where(IsImageFile)
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (string file in files)
            AddImagePath(file, false);

        if (files.Count > 0)
        {
            SelectImagePath(files[0]);
            LoadImage(files[0], true);
            StatusText.Text = $"已加载文件夹图片 {files.Count} 张。";
        }
        else
        {
            StatusText.Text = "所选文件夹内没有支持的图像文件。";
        }
    }

    private void BrowseTemplate_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Filter = "模板图像|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*",
            FileName = TemplatePathBox.Text
        };

        if (dialog.ShowDialog() == true)
            TemplatePathBox.Text = dialog.FileName;
    }

    private void LoadImage_Click(object sender, RoutedEventArgs e)
    {
        string path = ImagePathBox.Text.Trim();
        AddImagePath(path, true);
        LoadImage(path, true);
    }

    private void PreviousImage_Click(object sender, RoutedEventArgs e)
    {
        if (_imagePaths.Count == 0)
            return;

        int index = Math.Max(0, ImageListBox.SelectedIndex);
        index = (index - 1 + _imagePaths.Count) % _imagePaths.Count;
        ImageListBox.SelectedIndex = index;
    }

    private void NextImage_Click(object sender, RoutedEventArgs e)
    {
        if (_imagePaths.Count == 0)
            return;

        int index = Math.Max(0, ImageListBox.SelectedIndex);
        index = (index + 1) % _imagePaths.Count;
        ImageListBox.SelectedIndex = index;
    }

    private void ClearImages_Click(object sender, RoutedEventArgs e)
    {
        _imagePaths.Clear();
        _simulationRows.Clear();
        _result = null;
        ImageView.Source = null;
        _bitmap = null;
        ResetView(false);
        DrawOverlay();
        UpdateStats();
        StatusText.Text = "已清空图片列表和仿真结果。";
    }

    private void ImageListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_suppressImageSelection || ImageListBox.SelectedItem is not string path)
            return;

        LoadImage(path, false);
    }

    private void Detect_Click(object sender, RoutedEventArgs e)
    {
        TryDetect();
    }

    private void RunBatch_Click(object sender, RoutedEventArgs e)
    {
        RunBatchSimulation();
    }

    private void ExportCsv_Click(object sender, RoutedEventArgs e)
    {
        if (_simulationRows.Count == 0)
        {
            StatusText.Text = "没有可导出的仿真结果。";
            return;
        }

        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            Filter = "CSV 文件|*.csv|所有文件|*.*",
            FileName = "v_corner_simulation_results.csv"
        };

        if (dialog.ShowDialog() != true)
            return;

        StringBuilder csv = new();
        csv.AppendLine("Image,Status,Mode,P1Pixel,P2Pixel,P1Physical,P2Physical,BaseLine,DistanceXParallelBase,DistanceYPerpendicularBase,DistanceTotal,ElapsedMs,Note");
        foreach (SimulationResultRow row in _simulationRows)
        {
            csv.AppendLine(string.Join(",",
                Csv(row.ImageName),
                Csv(row.Status),
                Csv(row.Mode),
                Csv(row.P1Pixel),
                Csv(row.P2Pixel),
                Csv(row.P1Physical),
                Csv(row.P2Physical),
                Csv(row.BaseLine),
                Csv(row.DistanceX),
                Csv(row.DistanceY),
                Csv(row.Distance),
                Csv(row.ElapsedMs),
                Csv(row.Note)));
        }

        File.WriteAllText(dialog.FileName, csv.ToString(), new UTF8Encoding(true));
        StatusText.Text = $"已导出 CSV：{dialog.FileName}";
    }

    private void ApplyRoi_Click(object sender, RoutedEventArgs e)
    {
        ReadRoiFromBoxes();
        _result = null;
        DrawOverlay();
        UpdateStats();
    }

    private void ResetRoi_Click(object sender, RoutedEventArgs e)
    {
        ResetDemoRoi();
        _result = null;
        DrawOverlay();
        UpdateStats();
    }

    private void UseMatchRoi_Click(object sender, RoutedEventArgs e)
    {
        _roi.CenterRow = ReadDouble(MatchRowBox, _roi.CenterRow);
        _roi.CenterCol = ReadDouble(MatchColBox, _roi.CenterCol);
        _roi.PhiDeg = ReadDouble(MatchAngleDegBox, _roi.PhiDeg);
        _roi.Length1 = ReadDouble(RoiLength1Box, _roi.Length1);
        _roi.Length2 = ReadDouble(RoiLength2Box, _roi.Length2);
        WriteRoiBoxes();
        _result = null;
        DrawOverlay();
        UpdateStats();
    }

    private void CreateTemplateFromRoi_Click(object sender, RoutedEventArgs e)
    {
        if (_bitmap == null)
        {
            StatusText.Text = "请先加载图像，再拖拽或输入 ROI。";
            return;
        }

        ReadRoiFromBoxes();
        string imagePath = ImagePathBox.Text.Trim();
        string templatePath = ResolveTemplatePathForSave();

        try
        {
            Mouse.OverrideCursor = WpfCursors.Wait;
            HalconVCornerDetector.SaveTemplateFromRoi(imagePath, _roi, templatePath);
            TemplatePathBox.Text = templatePath;
            UseTemplateBeforeDetectCheck.IsChecked = true;
            StatusText.Text = $"已用当前 ROI 生成模板：{templatePath}";
        }
        catch (Exception ex)
        {
            StatusText.Text = "生成模板失败：" + ToFriendlyMessage(ex);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void RunTemplateMatch_Click(object sender, RoutedEventArgs e)
    {
        if (_bitmap == null)
        {
            StatusText.Text = "请先加载图像。";
            return;
        }

        if (!TryValidateTemplatePath(out string message))
        {
            StatusText.Text = message;
            return;
        }

        try
        {
            Mouse.OverrideCursor = WpfCursors.Wait;
            ShapeMatchResult match = LocateWithCurrentTemplate(ImagePathBox.Text.Trim());
            if (!match.Found)
            {
                StatusText.Text = "模板匹配未找到目标，请检查模板、角度范围或最低分数。";
                return;
            }

            ApplyMatchToRoi(match, _roi);
            WriteRoiBoxes();
            StatusText.Text = $"模板匹配成功：Score={match.Score:F4}";
        }
        catch (Exception ex)
        {
            StatusText.Text = "模板匹配失败：" + ToFriendlyMessage(ex);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }

        _result = null;
        DrawOverlay();
        UpdateStats();
    }

    private void OverlayOption_Changed(object sender, RoutedEventArgs e)
    {
        if (StatsText == null || OverlayCanvas == null)
            return;

        DrawOverlay();
        UpdateStats();
    }

    private void OverlayCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ClampViewPan();
        UpdateImageViewLayout();
        DrawOverlay();
    }

    private void ZoomFit_Click(object sender, RoutedEventArgs e)
    {
        ResetView();
        StatusText.Text = "已适应窗口显示。";
    }

    private void ZoomOneToOne_Click(object sender, RoutedEventArgs e)
    {
        if (_bitmap == null)
            return;

        _viewZoom = 1.0 / Math.Max(1e-9, GetFitScale());
        _viewPanOffset = new WpfPoint(0, 0);
        ClampViewPan();
        UpdateImageViewLayout();
        DrawOverlay();
        UpdateCursorInfo(null);
        StatusText.Text = "已切换到 1:1 像素显示。";
    }

    private void OverlayCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_bitmap == null)
            return;

        WpfPoint canvasPoint = e.GetPosition(OverlayCanvas);
        if (!TryCanvasToImage(canvasPoint, out ImagePoint imagePoint))
            return;

        _dragStartImagePoint = imagePoint;
        OverlayCanvas.CaptureMouse();
        e.Handled = true;
    }

    private void OverlayCanvas_MouseMove(object sender, WpfMouseEventArgs e)
    {
        WpfPoint canvasPoint = e.GetPosition(OverlayCanvas);
        UpdateCursorInfo(canvasPoint);

        if (_panStartCanvasPoint != null)
        {
            WpfPoint start = _panStartCanvasPoint.Value;
            _viewPanOffset = new WpfPoint(
                _panStartOffset.X + canvasPoint.X - start.X,
                _panStartOffset.Y + canvasPoint.Y - start.Y);
            ClampViewPan();
            UpdateImageViewLayout();
            DrawOverlay();
            return;
        }

        if (_dragStartImagePoint == null || e.LeftButton != MouseButtonState.Pressed)
            return;

        if (!TryCanvasToImage(canvasPoint, out ImagePoint current))
            return;

        UpdateAxisAlignedRoi(_dragStartImagePoint.Value, current);
        _result = null;
        DrawOverlay();
        UpdateStats();
    }

    private void OverlayCanvas_MousePanButtonDown(object sender, MouseButtonEventArgs e)
    {
        BeginPan(e.GetPosition(OverlayCanvas));
        e.Handled = true;
    }

    private void OverlayCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            BeginPan(e.GetPosition(OverlayCanvas));
            e.Handled = true;
        }
    }

    private void OverlayCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_dragStartImagePoint != null)
        {
            _dragStartImagePoint = null;
            OverlayCanvas.ReleaseMouseCapture();
            e.Handled = true;
        }
    }

    private void OverlayCanvas_MousePanButtonUp(object sender, MouseButtonEventArgs e)
    {
        EndPan();
        e.Handled = true;
    }

    private void OverlayCanvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            EndPan();
            e.Handled = true;
        }
    }

    private void OverlayCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (_bitmap == null)
            return;

        WpfPoint mousePoint = e.GetPosition(OverlayCanvas);
        if (!TryCanvasToImage(mousePoint, out ImagePoint anchorImagePoint))
            anchorImagePoint = CanvasToImageUnclamped(mousePoint);

        double factor = e.Delta > 0 ? 1.25 : 0.8;
        _viewZoom = Clamp(_viewZoom * factor, 1.0, 80.0);

        Rect baseRect = GetBaseImageRect();
        double imageScale = GetFitScale() * _viewZoom;
        double newLeft = mousePoint.X - anchorImagePoint.Col * imageScale;
        double newTop = mousePoint.Y - anchorImagePoint.Row * imageScale;
        _viewPanOffset = new WpfPoint(newLeft - baseRect.Left, newTop - baseRect.Top);

        ClampViewPan();
        UpdateImageViewLayout();
        DrawOverlay();
        UpdateCursorInfo(mousePoint);
        e.Handled = true;
    }

    private void OverlayCanvas_MouseLeave(object sender, WpfMouseEventArgs e)
    {
        if (_panStartCanvasPoint != null)
            EndPan();

        UpdateCursorInfo(null);
    }

    private void LoadImage(string path, bool resetRoiForImage)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            StatusText.Text = $"图像不存在：{path}";
            return;
        }

        BitmapImage bitmap = new();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(path, UriKind.Absolute);
        bitmap.EndInit();
        bitmap.Freeze();

        _bitmap = bitmap;
        ImageView.Source = bitmap;
        ImagePathBox.Text = path;
        _result = null;
        ResetView(false);

        if (resetRoiForImage)
            ResetDemoRoi();

        UpdateImageViewLayout();
        DrawOverlay();
        StatusText.Text = $"已加载图像：{bitmap.PixelWidth} x {bitmap.PixelHeight}";
        UpdateStats();
    }

    private void TryDetect()
    {
        if (_bitmap == null)
        {
            StatusText.Text = "请先加载图像。";
            return;
        }

        ReadRoiFromBoxes();
        HalconVCornerOptions options = ReadOptionsFromBoxes();
        VCornerRoi usedRoi = CloneRoi(_roi);
        Stopwatch sw = Stopwatch.StartNew();
        string note = string.Empty;

        try
        {
            Mouse.OverrideCursor = WpfCursors.Wait;
            string path = ImagePathBox.Text.Trim();

            if (UseTemplateBeforeDetectCheck.IsChecked == true)
            {
                if (!TryValidateTemplatePath(out string message))
                {
                    StatusText.Text = message;
                    return;
                }

                ShapeMatchResult match = LocateWithCurrentTemplate(path);
                if (!match.Found)
                {
                    StatusText.Text = "模板匹配未找到目标，当前检测未执行。";
                    return;
                }

                ApplyMatchToRoi(match, usedRoi);
                _roi = CloneRoi(usedRoi);
                WriteRoiBoxes();
                note = $"MatchScore={match.Score:F4}";
            }
            //HOperatorSet.CountSeconds(out HTuple start);
            _result = HalconVCornerDetector.Detect(path, usedRoi, options);
            //HOperatorSet.CountSeconds(out HTuple end);
            //end = (end - start) * 1000;
            //Console.WriteLine(end.D.ToString());
            sw.Stop();
            
            SimulationResultRow row = BuildResultRow(path, _result, sw.Elapsed.TotalMilliseconds, note);
            UpsertResultRow(row);
            StatusText.Text = _result.HasResult ? "检测完成，已输出两交点坐标和物理距离。" : "检测完成，但未找到稳定过渡段，请调整 ROI 或参数。";
        }
        catch (Exception ex)
        {
            sw.Stop();
            _result = null;
            string message = ToFriendlyMessage(ex);
            UpsertResultRow(BuildErrorRow(ImagePathBox.Text.Trim(), sw.Elapsed.TotalMilliseconds, message));
            StatusText.Text = "检测失败：" + message;
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }

        DrawOverlay();
        UpdateStats();
    }

    private void RunBatchSimulation()
    {
        if (_imagePaths.Count == 0)
        {
            StatusText.Text = "请先添加一张或多张图片。";
            return;
        }

        ReadRoiFromBoxes();
        HalconVCornerOptions options = ReadOptionsFromBoxes();
        VCornerRoi baseRoi = CloneRoi(_roi);
        _simulationRows.Clear();

        if (UseTemplateBeforeDetectCheck.IsChecked == true && !TryValidateTemplatePath(out string message))
        {
            StatusText.Text = message;
            UpdateStats();
            return;
        }

        try
        {
            Mouse.OverrideCursor = WpfCursors.Wait;

            foreach (string path in _imagePaths.ToList())
            {
                Stopwatch sw = Stopwatch.StartNew();
                string note = string.Empty;

                try
                {
                    VCornerRoi roiForImage = CloneRoi(baseRoi);

                    if (UseTemplateBeforeDetectCheck.IsChecked == true)
                    {
                        ShapeMatchResult match = LocateWithCurrentTemplate(path);
                        if (!match.Found)
                        {
                            sw.Stop();
                            _simulationRows.Add(BuildErrorRow(path, sw.Elapsed.TotalMilliseconds, "模板匹配未找到目标"));
                            continue;
                        }

                        ApplyMatchToRoi(match, roiForImage);
                        note = $"MatchScore={match.Score:F4}";
                    }

                    VCornerDetectionResult result = HalconVCornerDetector.Detect(path, roiForImage, options);
                    sw.Stop();
                    _simulationRows.Add(BuildResultRow(path, result, sw.Elapsed.TotalMilliseconds, note));
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    _simulationRows.Add(BuildErrorRow(path, sw.Elapsed.TotalMilliseconds, ToFriendlyMessage(ex)));
                }
            }

            string firstPath = _imagePaths.FirstOrDefault() ?? string.Empty;
            string firstOkPath = _simulationRows.FirstOrDefault(r => r.Status == "OK")?.ImagePath ?? firstPath;
            if (!string.IsNullOrEmpty(firstOkPath) && File.Exists(firstOkPath))
            {
                SelectImagePath(firstOkPath);
                LoadImage(firstOkPath, false);
                VCornerRoi displayRoi = CloneRoi(baseRoi);
                if (UseTemplateBeforeDetectCheck.IsChecked == true)
                {
                    ShapeMatchResult match = LocateWithCurrentTemplate(firstOkPath);
                    if (match.Found)
                        ApplyMatchToRoi(match, displayRoi);
                }

                _roi = CloneRoi(displayRoi);
                WriteRoiBoxes();
                _result = HalconVCornerDetector.Detect(firstOkPath, _roi, options);
            }

            int okCount = _simulationRows.Count(r => r.Status == "OK");
            StatusText.Text = $"批量仿真完成：OK={okCount}，NG={_simulationRows.Count - okCount}。";
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }

        DrawOverlay();
        UpdateStats();
    }

    private void LoadDefaultParameters()
    {
        _roi = new VCornerRoi
        {
            CenterRow = 2390,
            CenterCol = 2980,
            PhiDeg = 0,
            Length1 = 340,
            Length2 = 240
        };

        WriteRoiBoxes();
        MatchRowBox.Text = "2390";
        MatchColBox.Text = "2980";
        MatchAngleDegBox.Text = "0";
        TemplatePathBox.Text = GetDefaultTemplatePath();
        MatchMinScoreBox.Text = "0.5";
        MatchAngleStartDegBox.Text = "-180";
        MatchAngleExtentDegBox.Text = "360";
        MatchMaxOverlapBox.Text = "0.5";
        MatchGreedinessBox.Text = "0.7";

        PhysicalUnitBox.Text = "mm";
        PixelSizeXBox.Text = "1";
        PixelSizeYBox.Text = "1";
        OriginRowBox.Text = "0";
        OriginColBox.Text = "0";

        CannyAlphaBox.Text = "1.2";
        CannyLowBox.Text = "12";
        CannyHighBox.Text = "35";
        MinContourLengthBox.Text = "80";
        UnionMaxDistAbsBox.Text = "20";
        UnionMaxDistRelBox.Text = "2";
        SegmentSmoothBox.Text = "5";
        SegmentMaxLineDist1Box.Text = "4";
        SegmentMaxLineDist2Box.Text = "3";
        MinArcLengthBox.Text = "10";
        MinArcRadiusBox.Text = "3";
        MaxArcRadiusBox.Text = "300";
        CurvatureStepBox.Text = "8";
        CurvatureAngleThresholdDegBox.Text = "7";
        MinCurveRunPointsBox.Text = "4";
        MinPointDistanceBox.Text = "20";
        TangentFitPointsBox.Text = "45";
        TangentDepartureDistanceBox.Text = "4";
    }

    private void ResetDemoRoi()
    {
        if (_bitmap == null)
        {
            WriteRoiBoxes();
            return;
        }

        bool defaultImageLooksValid = _bitmap.PixelWidth > 3100 && _bitmap.PixelHeight > 2500;
        if (ImagePathBox.Text.Trim().Equals(DefaultImagePath, StringComparison.OrdinalIgnoreCase) && defaultImageLooksValid)
        {
            _roi.CenterRow = 2390;
            _roi.CenterCol = 2980;
            _roi.PhiDeg = 0;
            _roi.Length1 = 340;
            _roi.Length2 = 240;
        }
        else
        {
            _roi.CenterRow = _bitmap.PixelHeight * 0.5;
            _roi.CenterCol = _bitmap.PixelWidth * 0.5;
            _roi.PhiDeg = 0;
            _roi.Length1 = Math.Max(30, _bitmap.PixelWidth * 0.15);
            _roi.Length2 = Math.Max(30, _bitmap.PixelHeight * 0.15);
        }

        WriteRoiBoxes();
    }

    private ShapeMatchResult LocateWithCurrentTemplate(string imagePath)
    {
        ShapeMatchOptions options = new()
        {
            MinScore = ReadDouble(MatchMinScoreBox, 0.5),
            AngleStartDeg = ReadDouble(MatchAngleStartDegBox, -180),
            AngleExtentDeg = ReadDouble(MatchAngleExtentDegBox, 360),
            MaxOverlap = ReadDouble(MatchMaxOverlapBox, 0.5),
            Greediness = ReadDouble(MatchGreedinessBox, 0.7)
        };

        return HalconVCornerDetector.LocateShapeModel(imagePath, TemplatePathBox.Text.Trim(), options);
    }

    private bool TryValidateTemplatePath(out string message)
    {
        string templatePath = TemplatePathBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(templatePath))
        {
            message = "模板路径为空：请先点击“ROI生成模板”，或取消勾选“检测前先模板匹配定位 ROI”。";
            return false;
        }

        if (!File.Exists(templatePath))
        {
            message = $"模板文件不存在：{templatePath}。请先点击“ROI生成模板”，或取消勾选“检测前先模板匹配定位 ROI”。";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private string ResolveTemplatePathForSave()
    {
        string templatePath = TemplatePathBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(templatePath))
            templatePath = GetDefaultTemplatePath();

        string extension = System.IO.Path.GetExtension(templatePath);
        if (string.IsNullOrWhiteSpace(extension))
            templatePath += ".png";

        return templatePath;
    }

    private static string GetDefaultTemplatePath()
    {
        return System.IO.Path.Combine(AppContext.BaseDirectory, DefaultTemplateFileName);
    }

    private static string ToFriendlyMessage(Exception ex)
    {
        string text = ex.Message;
        if (ex is FileNotFoundException fileNotFound)
        {
            if (text.Contains("Template image path", StringComparison.OrdinalIgnoreCase))
                return $"模板文件不存在：{fileNotFound.FileName}。请先点击“ROI生成模板”，或取消勾选“检测前先模板匹配定位 ROI”。";
            if (text.Contains("Image path", StringComparison.OrdinalIgnoreCase))
                return $"图像文件不存在：{fileNotFound.FileName}";
        }

        if (text.Contains("Template image path does not exist", StringComparison.OrdinalIgnoreCase))
            return "模板文件不存在。请先点击“ROI生成模板”，或取消勾选“检测前先模板匹配定位 ROI”。";

        return text;
    }

    private void ApplyMatchToRoi(ShapeMatchResult match, VCornerRoi roi)
    {
        roi.CenterRow = match.Row;
        roi.CenterCol = match.Col;
        roi.PhiDeg = match.AngleDeg;
        roi.Length1 = Math.Max(2, ReadDouble(RoiLength1Box, roi.Length1));
        roi.Length2 = Math.Max(2, ReadDouble(RoiLength2Box, roi.Length2));

        MatchRowBox.Text = Format(match.Row);
        MatchColBox.Text = Format(match.Col);
        MatchAngleDegBox.Text = Format(match.AngleDeg);
    }

    private void ReadRoiFromBoxes()
    {
        _roi.CenterRow = ReadDouble(RoiCenterRowBox, _roi.CenterRow);
        _roi.CenterCol = ReadDouble(RoiCenterColBox, _roi.CenterCol);
        _roi.PhiDeg = ReadDouble(RoiPhiDegBox, _roi.PhiDeg);
        _roi.Length1 = Math.Max(2, ReadDouble(RoiLength1Box, _roi.Length1));
        _roi.Length2 = Math.Max(2, ReadDouble(RoiLength2Box, _roi.Length2));
        WriteRoiBoxes();
    }

    private HalconVCornerOptions ReadOptionsFromBoxes()
    {
        return new HalconVCornerOptions
        {
            CannyAlpha = ReadDouble(CannyAlphaBox, 1.2),
            CannyLow = ReadDouble(CannyLowBox, 12),
            CannyHigh = ReadDouble(CannyHighBox, 35),
            MinContourLength = ReadDouble(MinContourLengthBox, 80),
            UnionMaxDistAbs = ReadDouble(UnionMaxDistAbsBox, 20),
            UnionMaxDistRel = ReadDouble(UnionMaxDistRelBox, 2),
            SegmentSmooth = ReadInt(SegmentSmoothBox, 5),
            SegmentMaxLineDist1 = ReadDouble(SegmentMaxLineDist1Box, 4),
            SegmentMaxLineDist2 = ReadDouble(SegmentMaxLineDist2Box, 3),
            MinArcLength = ReadDouble(MinArcLengthBox, 10),
            MinArcRadius = ReadDouble(MinArcRadiusBox, 3),
            MaxArcRadius = ReadDouble(MaxArcRadiusBox, 300),
            CurvatureStep = ReadInt(CurvatureStepBox, 8),
            CurvatureAngleThresholdDeg = ReadDouble(CurvatureAngleThresholdDegBox, 7),
            MinCurveRunPoints = ReadInt(MinCurveRunPointsBox, 4),
            MinPointDistance = ReadDouble(MinPointDistanceBox, 20),
            TangentFitPoints = ReadInt(TangentFitPointsBox, 45),
            TangentDepartureDistance = ReadDouble(TangentDepartureDistanceBox, 4)
        };
    }

    private CalibrationSettings ReadCalibration()
    {
        return new CalibrationSettings
        {
            Unit = string.IsNullOrWhiteSpace(PhysicalUnitBox.Text) ? "unit" : PhysicalUnitBox.Text.Trim(),
            PixelSizeX = ReadDouble(PixelSizeXBox, 1),
            PixelSizeY = ReadDouble(PixelSizeYBox, 1),
            OriginRow = ReadDouble(OriginRowBox, 0),
            OriginCol = ReadDouble(OriginColBox, 0),
            RowDownIsPositiveY = YDownPositiveCheck.IsChecked == true
        };
    }

    private void WriteRoiBoxes()
    {
        RoiCenterRowBox.Text = Format(_roi.CenterRow);
        RoiCenterColBox.Text = Format(_roi.CenterCol);
        RoiPhiDegBox.Text = Format(_roi.PhiDeg);
        RoiLength1Box.Text = Format(_roi.Length1);
        RoiLength2Box.Text = Format(_roi.Length2);
    }

    private void UpdateAxisAlignedRoi(ImagePoint start, ImagePoint end)
    {
        double row1 = Math.Min(start.Row, end.Row);
        double row2 = Math.Max(start.Row, end.Row);
        double col1 = Math.Min(start.Col, end.Col);
        double col2 = Math.Max(start.Col, end.Col);

        _roi.CenterRow = (row1 + row2) * 0.5;
        _roi.CenterCol = (col1 + col2) * 0.5;
        _roi.PhiDeg = 0;
        _roi.Length1 = Math.Max(2, (col2 - col1) * 0.5);
        _roi.Length2 = Math.Max(2, (row2 - row1) * 0.5);
        WriteRoiBoxes();
    }

    private SimulationResultRow BuildResultRow(string imagePath, VCornerDetectionResult result, double elapsedMs, string note)
    {
        CalibrationSettings calibration = ReadCalibration();
        SimulationResultRow row = new()
        {
            ImagePath = imagePath,
            ImageName = System.IO.Path.GetFileName(imagePath),
            Status = result.HasResult ? "OK" : "NG",
            Mode = result.Mode,
            ElapsedMs = elapsedMs.ToString("0.0", _culture),
            Note = note
        };

        if (!result.HasResult)
        {
            row.Note = string.IsNullOrWhiteSpace(note) ? "未找到稳定过渡段" : note;
            return row;
        }

        PhysicalPoint p1 = ToPhysical(result.Point1, calibration);
        PhysicalPoint p2 = ToPhysical(result.Point2, calibration);
        double distanceX;
        double distanceY;
        string baseLineText;

        if (UseBaseLineCoordinateCheck.IsChecked == true && TryGetPhysicalBaseFrame(result, calibration, p1, out BaseFrame frame))
        {
            double vx = p2.X - p1.X;
            double vy = p2.Y - p1.Y;
            // DistanceX 是 P1->P2 在基准线平行方向的投影；
            // DistanceY 是 P1->P2 在基准线法线方向的投影。
            distanceX = vx * frame.DirX + vy * frame.DirY;
            distanceY = vx * frame.NormalX + vy * frame.NormalY;
            baseLineText = $"A={frame.AngleDeg:0.###}deg 平行/垂直";
        }
        else
        {
            distanceX = p2.X - p1.X;
            distanceY = p2.Y - p1.Y;
            baseLineText = "图像XY";
        }

        double distance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

        row.P1Pixel = FormatPixelPoint(result.Point1);
        row.P2Pixel = FormatPixelPoint(result.Point2);
        row.P1Physical = FormatPhysicalPoint(p1, calibration.Unit);
        row.P2Physical = FormatPhysicalPoint(p2, calibration.Unit);
        row.BaseLine = baseLineText;
        row.DistanceX = $"{distanceX:0.###} {calibration.Unit}";
        row.DistanceY = $"{distanceY:0.###} {calibration.Unit}";
        row.Distance = $"{distance:0.###} {calibration.Unit}";
        return row;
    }

    private SimulationResultRow BuildErrorRow(string imagePath, double elapsedMs, string message)
    {
        return new SimulationResultRow
        {
            ImagePath = imagePath,
            ImageName = System.IO.Path.GetFileName(imagePath),
            Status = "NG",
            Mode = "error",
            ElapsedMs = elapsedMs.ToString("0.0", _culture),
            Note = message
        };
    }

    private void UpsertResultRow(SimulationResultRow row)
    {
        for (int i = 0; i < _simulationRows.Count; i++)
        {
            if (string.Equals(_simulationRows[i].ImagePath, row.ImagePath, StringComparison.OrdinalIgnoreCase))
            {
                _simulationRows[i] = row;
                return;
            }
        }

        _simulationRows.Add(row);
    }

    private PhysicalPoint ToPhysical(ImagePoint point, CalibrationSettings calibration)
    {
        double x = (point.Col - calibration.OriginCol) * calibration.PixelSizeX;
        double ySign = calibration.RowDownIsPositiveY ? 1.0 : -1.0;
        double y = (point.Row - calibration.OriginRow) * calibration.PixelSizeY * ySign;
        return new PhysicalPoint(x, y);
    }

    private ImagePoint FromPhysical(PhysicalPoint point, CalibrationSettings calibration)
    {
        double ySign = calibration.RowDownIsPositiveY ? 1.0 : -1.0;
        double col = point.X / Math.Max(1e-12, calibration.PixelSizeX) + calibration.OriginCol;
        double row = point.Y / Math.Max(1e-12, calibration.PixelSizeY) * ySign + calibration.OriginRow;
        return new ImagePoint(row, col);
    }

    private bool TryGetPhysicalBaseFrame(VCornerDetectionResult result, CalibrationSettings calibration, PhysicalPoint origin, out BaseFrame frame)
    {
        frame = default;
        if (result.BaseLine == null)
            return false;

        ImageLine line = result.BaseLine.Value;
        PhysicalPoint p0 = ToPhysical(line.Point, calibration);
        PhysicalPoint p1 = ToPhysical(new ImagePoint(line.Point.Row + line.DirectionRow, line.Point.Col + line.DirectionCol), calibration);
        double dirX = p1.X - p0.X;
        double dirY = p1.Y - p0.Y;
        double length = Math.Sqrt(dirX * dirX + dirY * dirY);
        if (length < 1e-9)
            return false;

        dirX /= length;
        dirY /= length;

        // 让基准边方向尽量从 P1 指向 P2，避免 ΔX 符号随轮廓采样方向翻转。
        PhysicalPoint p2Physical = ToPhysical(result.Point2, calibration);
        double p1ToP2X = p2Physical.X - origin.X;
        double p1ToP2Y = p2Physical.Y - origin.Y;
        if (p1ToP2X * dirX + p1ToP2Y * dirY < 0)
        {
            dirX = -dirX;
            dirY = -dirY;
        }

        double normalX = -dirY;
        double normalY = dirX;
        frame = new BaseFrame(dirX, dirY, normalX, normalY, Math.Atan2(dirY, dirX) * 180.0 / Math.PI);
        return true;
    }

    private void DrawOverlay()
    {
        if (OverlayCanvas == null)
            return;

        OverlayCanvas.Children.Clear();
        if (_bitmap == null)
            return;

        DrawRoi();

        if (_result == null)
            return;

        if (ShowMainContourCheck.IsChecked == true)
            DrawPolyline(_result.MainContour, WpfColor.FromRgb(255, 168, 64), 2.2, 0.95);

        if (ShowPartsCheck.IsChecked == true)
        {
            foreach (List<ImagePoint> part in _result.ContourParts)
                DrawPolyline(part, WpfColor.FromRgb(80, 170, 255), 1.2, 0.55);
        }

        if (_result.BaseLine != null)
            DrawBaseLine(_result.BaseLine.Value);

        DrawPolyline(_result.FeatureContour, WpfColor.FromRgb(42, 220, 120), 4.0, 0.98);

        if (_result.HasResult)
        {
            DrawDistanceAxes(_result);
            DrawCross(_result.Point1, WpfBrushes.Red, 22, 3);
            DrawCross(_result.Point2, WpfBrushes.Red, 22, 3);
            DrawPointLabel(_result.Point1, "P1");
            DrawPointLabel(_result.Point2, "P2");
        }
    }

    private void DrawRoi()
    {
        ImagePoint[] corners = _roi.GetCorners();
        Polyline roiLine = new()
        {
            Stroke = WpfBrushes.Cyan,
            StrokeThickness = 2,
            Opacity = 0.95
        };

        foreach (ImagePoint corner in corners)
            roiLine.Points.Add(ImageToCanvas(corner));
        roiLine.Points.Add(ImageToCanvas(corners[0]));
        OverlayCanvas.Children.Add(roiLine);

        DrawCross(new ImagePoint(_roi.CenterRow, _roi.CenterCol), WpfBrushes.Cyan, 12, 2);
    }

    private void DrawBaseLine(ImageLine line)
    {
        if (_bitmap == null)
            return;

        double half = Math.Max(80, Math.Min(_bitmap.PixelWidth, _bitmap.PixelHeight) * 0.25);
        ImagePoint a = new(line.Point.Row - line.DirectionRow * half, line.Point.Col - line.DirectionCol * half);
        ImagePoint b = new(line.Point.Row + line.DirectionRow * half, line.Point.Col + line.DirectionCol * half);
        DrawPolyline(new[] { a, b }, WpfColor.FromRgb(255, 70, 210), 2.4, 0.95);
    }

    private void DrawDistanceAxes(VCornerDetectionResult result)
    {
        if (UseBaseLineCoordinateCheck.IsChecked != true || result.BaseLine == null)
            return;

        CalibrationSettings calibration = ReadCalibration();
        PhysicalPoint p1 = ToPhysical(result.Point1, calibration);
        PhysicalPoint p2 = ToPhysical(result.Point2, calibration);
        if (!TryGetPhysicalBaseFrame(result, calibration, p1, out BaseFrame frame))
            return;

        double vx = p2.X - p1.X;
        double vy = p2.Y - p1.Y;
        double distanceX = vx * frame.DirX + vy * frame.DirY;
        double distanceY = vx * frame.NormalX + vy * frame.NormalY;
        PhysicalPoint projection = new(
            p1.X + distanceX * frame.DirX,
            p1.Y + distanceX * frame.DirY);
        ImagePoint projectionImage = FromPhysical(projection, calibration);

        DrawMeasurementSegment(result.Point1, projectionImage, WpfColor.FromRgb(255, 220, 80), $"DistanceX={distanceX:0.###} {calibration.Unit}");
        DrawMeasurementSegment(projectionImage, result.Point2, WpfColor.FromRgb(70, 235, 215), $"DistanceY={distanceY:0.###} {calibration.Unit}");
        DrawCross(projectionImage, new SolidColorBrush(WpfColor.FromRgb(255, 220, 80)), 14, 2);
    }

    private void DrawMeasurementSegment(ImagePoint a, ImagePoint b, WpfColor color, string labelText)
    {
        WpfPoint p1 = ImageToCanvas(a);
        WpfPoint p2 = ImageToCanvas(b);
        WpfBrush brush = new SolidColorBrush(color);

        Line line = new()
        {
            X1 = p1.X,
            Y1 = p1.Y,
            X2 = p2.X,
            Y2 = p2.Y,
            Stroke = brush,
            StrokeThickness = 3,
            Opacity = 0.98,
            StrokeDashArray = new DoubleCollection { 7, 4 },
            SnapsToDevicePixels = true
        };
        OverlayCanvas.Children.Add(line);

        TextBlock label = new()
        {
            Text = labelText,
            Foreground = WpfBrushes.White,
            Background = new SolidColorBrush(WpfColor.FromArgb(190, 15, 17, 20)),
            Padding = new Thickness(5, 2, 5, 2),
            FontFamily = new WpfFontFamily("Consolas"),
            FontSize = 12
        };

        Canvas.SetLeft(label, (p1.X + p2.X) * 0.5 + 8);
        Canvas.SetTop(label, (p1.Y + p2.Y) * 0.5 + 8);
        OverlayCanvas.Children.Add(label);
    }

    private void DrawPolyline(IReadOnlyList<ImagePoint> points, WpfColor color, double thickness, double opacity)
    {
        if (points.Count < 2)
            return;

        Polyline line = new()
        {
            Stroke = new SolidColorBrush(color),
            StrokeThickness = thickness,
            Opacity = opacity,
            SnapsToDevicePixels = true
        };

        foreach (ImagePoint point in points)
            line.Points.Add(ImageToCanvas(point));

        OverlayCanvas.Children.Add(line);
    }

    private void DrawCross(ImagePoint point, WpfBrush brush, double size, double thickness)
    {
        WpfPoint p = ImageToCanvas(point);
        OverlayCanvas.Children.Add(new Line
        {
            X1 = p.X - size,
            Y1 = p.Y,
            X2 = p.X + size,
            Y2 = p.Y,
            Stroke = brush,
            StrokeThickness = thickness
        });
        OverlayCanvas.Children.Add(new Line
        {
            X1 = p.X,
            Y1 = p.Y - size,
            X2 = p.X,
            Y2 = p.Y + size,
            Stroke = brush,
            StrokeThickness = thickness
        });
    }

    private void DrawPointLabel(ImagePoint point, string name)
    {
        CalibrationSettings calibration = ReadCalibration();
        PhysicalPoint physical = ToPhysical(point, calibration);
        WpfPoint p = ImageToCanvas(point);
        TextBlock label = new()
        {
            Text = $"{name}  R={point.Row:F2}, C={point.Col:F2}\nX={physical.X:F3}, Y={physical.Y:F3} {calibration.Unit}",
            Foreground = WpfBrushes.White,
            Background = new SolidColorBrush(WpfColor.FromArgb(180, 20, 20, 20)),
            Padding = new Thickness(5, 2, 5, 2),
            FontFamily = new WpfFontFamily("Consolas"),
            FontSize = 12
        };
        Canvas.SetLeft(label, p.X + 12);
        Canvas.SetTop(label, p.Y - 34);
        OverlayCanvas.Children.Add(label);
    }

    private WpfPoint ImageToCanvas(ImagePoint point)
    {
        Rect imageRect = GetDisplayedImageRect();
        if (imageRect.IsEmpty)
            return new WpfPoint(0, 0);

        double x = imageRect.Left + point.Col * imageRect.Width / Math.Max(1, _bitmap!.PixelWidth);
        double y = imageRect.Top + point.Row * imageRect.Height / Math.Max(1, _bitmap.PixelHeight);
        return new WpfPoint(x, y);
    }

    private bool TryCanvasToImage(WpfPoint canvasPoint, out ImagePoint imagePoint)
    {
        imagePoint = default;
        if (_bitmap == null)
            return false;

        Rect imageRect = GetDisplayedImageRect();
        if (imageRect.IsEmpty || !imageRect.Contains(canvasPoint))
            return false;

        double col = (canvasPoint.X - imageRect.Left) * _bitmap.PixelWidth / Math.Max(1, imageRect.Width);
        double row = (canvasPoint.Y - imageRect.Top) * _bitmap.PixelHeight / Math.Max(1, imageRect.Height);
        imagePoint = new ImagePoint(Clamp(row, 0, _bitmap.PixelHeight - 1), Clamp(col, 0, _bitmap.PixelWidth - 1));
        return true;
    }

    private ImagePoint CanvasToImageUnclamped(WpfPoint canvasPoint)
    {
        if (_bitmap == null)
            return default;

        Rect imageRect = GetDisplayedImageRect();
        if (imageRect.IsEmpty || imageRect.Width <= 1 || imageRect.Height <= 1)
            return default;

        double col = (canvasPoint.X - imageRect.Left) * _bitmap.PixelWidth / imageRect.Width;
        double row = (canvasPoint.Y - imageRect.Top) * _bitmap.PixelHeight / imageRect.Height;
        return new ImagePoint(Clamp(row, 0, _bitmap.PixelHeight - 1), Clamp(col, 0, _bitmap.PixelWidth - 1));
    }

    private void UpdateCursorInfo(WpfPoint? canvasPoint)
    {
        if (CursorInfoText == null)
            return;

        string zoomText = $"Zoom={_viewZoom * 100:0.#}%";
        if (_bitmap == null || canvasPoint == null || !TryCanvasToImage(canvasPoint.Value, out ImagePoint imagePoint))
        {
            CursorInfoText.Text = $"Row=-, Col=-, Gray=-, {zoomText}";
            return;
        }

        int row = (int)Math.Round(imagePoint.Row);
        int col = (int)Math.Round(imagePoint.Col);
        string gray = TryGetGrayValue(row, col, out byte grayValue) ? grayValue.ToString(_culture) : "-";
        string info = $"Row={row}, Col={col}, Gray={gray}, {zoomText}";
        CursorInfoText.Text = info;
        StatusText.Text = info;
    }

    private bool TryGetGrayValue(int row, int col, out byte gray)
    {
        gray = 0;
        if (_bitmap == null || row < 0 || col < 0 || row >= _bitmap.PixelHeight || col >= _bitmap.PixelWidth)
            return false;

        try
        {
            int bytesPerPixel = Math.Max(1, (_bitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixel = new byte[bytesPerPixel];
            _bitmap.CopyPixels(new Int32Rect(col, row, 1, 1), pixel, bytesPerPixel, 0);

            if (_bitmap.Format == PixelFormats.Gray8 || _bitmap.Format == PixelFormats.Indexed8)
            {
                gray = pixel[0];
                return true;
            }

            if (_bitmap.Format == PixelFormats.Bgr24 || _bitmap.Format == PixelFormats.Bgr32 || _bitmap.Format == PixelFormats.Bgra32 || _bitmap.Format == PixelFormats.Pbgra32)
            {
                gray = ToGray(pixel[2], pixel[1], pixel[0]);
                return true;
            }

            if (_bitmap.Format == PixelFormats.Rgb24)
            {
                gray = ToGray(pixel[0], pixel[1], pixel[2]);
                return true;
            }

            if (bytesPerPixel >= 3)
            {
                gray = ToGray(pixel[2], pixel[1], pixel[0]);
                return true;
            }

            gray = pixel[0];
            return true;
        }
        catch
        {
            return false;
        }
    }

    private Rect GetDisplayedImageRect()
    {
        if (_bitmap == null || OverlayCanvas.ActualWidth <= 1 || OverlayCanvas.ActualHeight <= 1)
            return Rect.Empty;

        Rect baseRect = GetBaseImageRect();
        double scale = GetFitScale() * _viewZoom;
        double width = _bitmap.PixelWidth * scale;
        double height = _bitmap.PixelHeight * scale;
        double left = baseRect.Left + _viewPanOffset.X;
        double top = baseRect.Top + _viewPanOffset.Y;
        return new Rect(left, top, width, height);
    }

    private Rect GetBaseImageRect()
    {
        if (_bitmap == null || OverlayCanvas.ActualWidth <= 1 || OverlayCanvas.ActualHeight <= 1)
            return Rect.Empty;

        double scale = GetFitScale();
        double width = _bitmap.PixelWidth * scale;
        double height = _bitmap.PixelHeight * scale;
        double left = (OverlayCanvas.ActualWidth - width) * 0.5;
        double top = (OverlayCanvas.ActualHeight - height) * 0.5;
        return new Rect(left, top, width, height);
    }

    private double GetFitScale()
    {
        if (_bitmap == null || OverlayCanvas.ActualWidth <= 1 || OverlayCanvas.ActualHeight <= 1)
            return 1.0;

        return Math.Min(OverlayCanvas.ActualWidth / _bitmap.PixelWidth, OverlayCanvas.ActualHeight / _bitmap.PixelHeight);
    }

    private void ResetView(bool redraw = true)
    {
        _viewZoom = 1.0;
        _viewPanOffset = new WpfPoint(0, 0);
        ClampViewPan();
        UpdateImageViewLayout();
        UpdateCursorInfo(null);

        if (redraw)
            DrawOverlay();
    }

    private void UpdateImageViewLayout()
    {
        if (_bitmap == null || ImageView == null)
            return;

        _displayedImageRect = GetDisplayedImageRect();
        if (_displayedImageRect.IsEmpty)
        {
            ImageView.Width = 1;
            ImageView.Height = 1;
            ImageView.Margin = new Thickness(0);
            return;
        }

        ImageView.Width = Math.Max(1, _displayedImageRect.Width);
        ImageView.Height = Math.Max(1, _displayedImageRect.Height);
        ImageView.Margin = new Thickness(_displayedImageRect.Left, _displayedImageRect.Top, 0, 0);
    }

    private void ClampViewPan()
    {
        if (_bitmap == null || OverlayCanvas == null || OverlayCanvas.ActualWidth <= 1 || OverlayCanvas.ActualHeight <= 1)
            return;

        Rect baseRect = GetBaseImageRect();
        if (baseRect.IsEmpty)
            return;

        double scale = GetFitScale() * _viewZoom;
        double imageWidth = _bitmap.PixelWidth * scale;
        double imageHeight = _bitmap.PixelHeight * scale;

        double minLeft = Math.Min(0, OverlayCanvas.ActualWidth - imageWidth);
        double maxLeft = Math.Max(0, OverlayCanvas.ActualWidth - imageWidth);
        double minTop = Math.Min(0, OverlayCanvas.ActualHeight - imageHeight);
        double maxTop = Math.Max(0, OverlayCanvas.ActualHeight - imageHeight);

        double left = Clamp(baseRect.Left + _viewPanOffset.X, minLeft, maxLeft);
        double top = Clamp(baseRect.Top + _viewPanOffset.Y, minTop, maxTop);
        _viewPanOffset = new WpfPoint(left - baseRect.Left, top - baseRect.Top);
    }

    private void BeginPan(WpfPoint canvasPoint)
    {
        if (_bitmap == null)
            return;

        _panStartCanvasPoint = canvasPoint;
        _panStartOffset = _viewPanOffset;
        OverlayCanvas.CaptureMouse();
    }

    private void EndPan()
    {
        _panStartCanvasPoint = null;
        if (OverlayCanvas.IsMouseCaptured)
            OverlayCanvas.ReleaseMouseCapture();
    }

    private void UpdateStats()
    {
        if (StatsText == null)
            return;

        CalibrationSettings calibration = ReadCalibration();
        string imageLine = _bitmap == null
            ? "图像：未加载"
            : $"图像：{_bitmap.PixelWidth} x {_bitmap.PixelHeight}";

        string roiLine =
            $"ROI：R={_roi.CenterRow:F2}, C={_roi.CenterCol:F2}, A={_roi.PhiDeg:F2}deg\n" +
            $"     L1={_roi.Length1:F2}, L2={_roi.Length2:F2}";

        string calibrationLine =
            $"像素当量：X={calibration.PixelSizeX:0.######}, Y={calibration.PixelSizeY:0.######} {calibration.Unit}/px\n" +
            $"原点：R={calibration.OriginRow:0.###}, C={calibration.OriginCol:0.###}, Y方向={(calibration.RowDownIsPositiveY ? "Row向下+" : "Row向上+")}\n" +
            $"距离坐标系：{(UseBaseLineCoordinateCheck.IsChecked == true ? "基准线平行/垂直坐标" : "图像XY坐标")}";

        if (_result == null)
        {
            StatsText.Text = imageLine + "\n" + roiLine + "\n" + calibrationLine + "\n\n未执行检测。";
            return;
        }

        if (!_result.HasResult)
        {
            StatsText.Text = imageLine + "\n" + roiLine + "\n" + calibrationLine + "\n\n未找到稳定结果。";
            return;
        }

        SimulationResultRow row = BuildResultRow(ImagePathBox.Text.Trim(), _result, 0, string.Empty);
        StatsText.Text =
            imageLine + "\n" +
            roiLine + "\n" +
            calibrationLine + "\n\n" +
            $"模式：{_result.Mode}\n" +
            $"P1像素：{row.P1Pixel}\n" +
            $"P2像素：{row.P2Pixel}\n" +
            $"P1物理：{row.P1Physical}\n" +
            $"P2物理：{row.P2Physical}\n" +
            $"基准线：{row.BaseLine}\n" +
            $"DistanceX(平行基准)：{row.DistanceX}\n" +
            $"DistanceY(垂直基准)：{row.DistanceY}\n" +
            $"Distance(总长)：{row.Distance}\n\n" +
            $"ROI轮廓数：{_result.NumUnionContours}\n" +
            $"分段数：{_result.NumParts}";
    }

    private void AddImagePath(string path, bool select)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path) || !IsImageFile(path))
            return;

        if (!_imagePaths.Any(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase)))
            _imagePaths.Add(path);

        if (select)
            SelectImagePath(path);
    }

    private void SelectImagePath(string path)
    {
        _suppressImageSelection = true;
        ImageListBox.SelectedItem = _imagePaths.FirstOrDefault(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
        _suppressImageSelection = false;
    }

    private static bool IsImageFile(string path)
    {
        string ext = System.IO.Path.GetExtension(path);
        return ImageExtensions.Any(e => string.Equals(e, ext, StringComparison.OrdinalIgnoreCase));
    }

    private static VCornerRoi CloneRoi(VCornerRoi source)
    {
        return new VCornerRoi
        {
            CenterRow = source.CenterRow,
            CenterCol = source.CenterCol,
            PhiDeg = source.PhiDeg,
            Length1 = source.Length1,
            Length2 = source.Length2
        };
    }

    private double ReadDouble(WpfTextBox box, double fallback)
    {
        string text = box.Text.Trim().Replace("，", ".").Replace(",", ".");
        return double.TryParse(text, NumberStyles.Float, _culture, out double value) ? value : fallback;
    }

    private int ReadInt(WpfTextBox box, int fallback)
    {
        return (int)Math.Round(ReadDouble(box, fallback));
    }

    private string Format(double value)
    {
        return value.ToString("0.###", _culture);
    }

    private string FormatPixelPoint(ImagePoint point)
    {
        return $"R={point.Row:0.###}, C={point.Col:0.###}";
    }

    private string FormatPhysicalPoint(PhysicalPoint point, string unit)
    {
        return $"X={point.X:0.###}, Y={point.Y:0.###} {unit}";
    }

    private static string Csv(string value)
    {
        value ??= string.Empty;
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    private static byte ToGray(byte r, byte g, byte b)
    {
        return (byte)Math.Round(0.299 * r + 0.587 * g + 0.114 * b);
    }
}

public sealed class SimulationResultRow
{
    public string ImagePath { get; set; } = string.Empty;
    public string ImageName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string P1Pixel { get; set; } = string.Empty;
    public string P2Pixel { get; set; } = string.Empty;
    public string P1Physical { get; set; } = string.Empty;
    public string P2Physical { get; set; } = string.Empty;
    public string BaseLine { get; set; } = string.Empty;
    public string DistanceX { get; set; } = string.Empty;
    public string DistanceY { get; set; } = string.Empty;
    public string Distance { get; set; } = string.Empty;
    public string ElapsedMs { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}

public sealed class CalibrationSettings
{
    public string Unit { get; set; } = "mm";
    public double PixelSizeX { get; set; } = 1;
    public double PixelSizeY { get; set; } = 1;
    public double OriginRow { get; set; }
    public double OriginCol { get; set; }
    public bool RowDownIsPositiveY { get; set; } = true;
}

public readonly record struct PhysicalPoint(double X, double Y);

public readonly record struct BaseFrame(double DirX, double DirY, double NormalX, double NormalY, double AngleDeg);
