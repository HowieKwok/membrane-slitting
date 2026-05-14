using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using HalconDotNet;
using Microsoft.Win32;
using QrBoardMappingTool.Models;
using QrBoardMappingTool.Services;

namespace QrBoardMappingTool
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel = new MainViewModel();
        private readonly QrBoardMapper mapper = new QrBoardMapper();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;

            InitComboBoxes();

            Viewer.RoiChanged += (s, e) => UpdateRoiInfo();
            UpdateStatus();
            UpdateRoiInfo();
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "图像|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*"
            };

            if (dialog.ShowDialog(this) == true)
            {
                viewModel.Settings.ImagePath = dialog.FileName;
                LoadImage();
            }
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            LoadImage();
        }

        private void FitImage_Click(object sender, RoutedEventArgs e)
        {
            Viewer.FitImage();
        }

        private void AddRectRoi_Click(object sender, RoutedEventArgs e)
        {
            RunInteractiveRoi(() => Viewer.AddRectangleRoiInteractive());
        }

        private void AddCircleRoi_Click(object sender, RoutedEventArgs e)
        {
            RunInteractiveRoi(() => Viewer.AddCircleRoiInteractive());
        }

        private void AddPolygonRoi_Click(object sender, RoutedEventArgs e)
        {
            RunInteractiveRoi(() => Viewer.AddPolygonRoiInteractive());
        }

        private void UndoRoi_Click(object sender, RoutedEventArgs e)
        {
            Viewer.RemoveLastRoi();
        }

        private void ClearRoi_Click(object sender, RoutedEventArgs e)
        {
            Viewer.ClearRois();
        }

        private void DetectAndFit_Click(object sender, RoutedEventArgs e)
        {
            if (Viewer.Image == null || !Viewer.Image.IsInitialized())
            {
                LoadImage();
            }

            HObject roi = null;
            try
            {
                roi = Viewer.BuildRoiRegion();
                viewModel.Result = mapper.DetectAndMap(Viewer.Image, roi, viewModel.Settings);
                DrawResultOverlay();
                UpdateStatus();
            }
            catch (Exception ex)
            {
                viewModel.Result.Message = ex.Message;
                viewModel.Result.Success = false;
                UpdateStatus();
            }
            finally
            {
                roi?.Dispose();
            }
        }

        private void Refit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mapper.Refit(viewModel.Result, viewModel.Settings.MappingModel);
                DrawResultOverlay();
                UpdateStatus();
            }
            catch (Exception ex)
            {
                viewModel.Result.Success = false;
                viewModel.Result.Message = ex.Message;
                UpdateStatus();
            }
        }

        private void PixelToBoard_Click(object sender, RoutedEventArgs e)
        {
            if (!TryReadDouble(PixelRowBox.Text, out double row) || !TryReadDouble(PixelColBox.Text, out double col))
            {
                PixelToBoardText.Text = "像素输入无效。";
                return;
            }

            if (mapper.TryPixelToWorld(row, col, out double x, out double y, out string message))
            {
                PixelToBoardText.Text = $"X={x:F4} mm, Y={y:F4} mm";
            }
            else
            {
                PixelToBoardText.Text = message;
            }
        }

        private void BoardToPixel_Click(object sender, RoutedEventArgs e)
        {
            if (!TryReadDouble(WorldXBox.Text, out double x) || !TryReadDouble(WorldYBox.Text, out double y))
            {
                BoardToPixelText.Text = "物理坐标输入无效。";
                return;
            }

            if (mapper.TryWorldToPixel(x, y, out double row, out double col, out string message))
            {
                BoardToPixelText.Text = $"Row={row:F4}, Col={col:F4}";
            }
            else
            {
                BoardToPixelText.Text = message;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Viewer.Dispose();
        }

        private void LoadImage()
        {
            if (string.IsNullOrWhiteSpace(viewModel.Settings.ImagePath) || !File.Exists(viewModel.Settings.ImagePath))
            {
                MessageBox.Show(this, "图像路径不存在。", "二维码棋盘格坐标映射", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Viewer.LoadImage(viewModel.Settings.ImagePath);
            viewModel.Result = new BoardMappingResult { Message = "图像已加载。" };
            UpdateStatus();
        }

        private void RunInteractiveRoi(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ROI", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DrawResultOverlay()
        {
            HObject crosses = null;
            HObject concat = null;

            try
            {
                HOperatorSet.GenEmptyObj(out concat);

                foreach (QrBoardPoint point in viewModel.Result.Points.Where(p => p.Used))
                {
                    HObject cross = null;
                    HOperatorSet.GenCrossContourXld(out cross, point.ImageRow, point.ImageColumn, 14, point.PhiRad);
                    HOperatorSet.ConcatObj(concat, cross, out HObject next);
                    cross.Dispose();
                    concat.Dispose();
                    concat = next;
                }

                crosses = concat;
                concat = null;
                Viewer.SetOverlay(crosses, false);
            }
            catch
            {
                concat?.Dispose();
                crosses?.Dispose();
            }
        }

        private void UpdateStatus()
        {
            StatusText.Text = viewModel.Result.Message ?? string.Empty;
            StatsText.Text =
                $"识别数量 : {viewModel.Result.DetectedCount}\n" +
                $"参与数量 : {viewModel.Result.UsedCount}\n" +
                $"平均误差 : {viewModel.Result.AvgErrorMm:F4} mm\n" +
                $"RMS误差  : {viewModel.Result.RmsErrorMm:F4} mm\n" +
                $"最大误差 : {viewModel.Result.MaxErrorMm:F4} mm";
        }

        private void UpdateRoiInfo()
        {
            if (Viewer.RoiCount == 0)
            {
                RoiInfoText.Text = "ROI: 整图";
                return;
            }

            string kinds = string.Join(", ", Viewer.GetRoiKinds().Select(GetRoiKindName));
            RoiInfoText.Text = $"ROI: {Viewer.RoiCount} ({kinds})";
        }

        private static string GetRoiKindName(ViewerRoiKind kind)
        {
            if (kind == ViewerRoiKind.Circle) return "圆形";
            if (kind == ViewerRoiKind.Polygon) return "多边形";
            return "矩形";
        }

        private void InitComboBoxes()
        {
            SetComboItems(CenterSourceBox, new[]
            {
                new ComboOption(QrCenterSource.SmallestRectangleCenter, "最小外接矩形中心"),
                new ComboOption(QrCenterSource.DataCodeCenter, "HALCON结果中心")
            });
        }

        private static void SetComboItems(System.Windows.Controls.ComboBox comboBox, IEnumerable<ComboOption> items)
        {
            comboBox.DisplayMemberPath = nameof(ComboOption.Name);
            comboBox.SelectedValuePath = nameof(ComboOption.Value);
            comboBox.ItemsSource = items.ToList();
        }

        private static bool TryReadDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }
    }

    public sealed class MainViewModel : NotifyObject
    {
        private BoardMappingResult result = new BoardMappingResult();

        public BoardMappingSettings Settings { get; } = new BoardMappingSettings();

        public BoardMappingResult Result
        {
            get => result;
            set => Set(ref result, value ?? new BoardMappingResult());
        }
    }

    public sealed class ComboOption
    {
        public ComboOption(object value, string name)
        {
            Value = value;
            Name = name;
        }

        public object Value { get; }
        public string Name { get; }
    }
}
