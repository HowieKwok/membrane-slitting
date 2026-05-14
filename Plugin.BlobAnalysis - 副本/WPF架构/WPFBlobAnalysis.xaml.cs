using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.BlobAnalysis.Core;
using Plugin.BlobAnalysis.Models;
using Plugin.BlobAnalysis.Services;
using Plugin.BlobAnalysis.ViewModels;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;

namespace Plugin.BlobAnalysis.WPFArchitecture
{
    public partial class WPFBlobAnalysis : System.Windows.Controls.UserControl, IPluginWpfView
    {
        // WPF 页面只绑定 ViewModel；ViewModel 内部的参数源必须来自 ParamsBlobAnalysis。
        // BlobAnalysisParams 只作为算法运行时 DTO，不能作为界面保存/读取对象。
        private readonly BlobAnalysisViewModel vm = new BlobAnalysisViewModel();
        private HalconViewRoiAdapter mainDisplay;
        private HalconViewRoiAdapter previewDisplay;
        private HalconViewRoiAdapter resultDisplay;
        private HWindowControl mainHalcon;
        private HWindowControl previewHalcon;
        private HWindowControl resultHalcon;
        private HObject selectedRegion;
        private bool switchingRoi;

        public BlobAnalysisObj PluginObject { get; private set; }
        public PluginParamsBase PluginParams { get; private set; }
        IPlugin IPluginWpfView.PluginObject { get { return PluginObject; } }
        PluginParamsBase IPluginWpfView.PluginParams { get { return PluginParams; } }

        public WPFBlobAnalysis()
        {
            InitializeComponent();
            DataContext = vm;
            vm.PropertyChanged += OnViewModelPropertyChanged;
            vm.AddFilterRequested = ShowAddFilterDialog;
        }

        public void BindPlugin(IPlugin plugin)
        {
            // 宿主 WPFFormPluginBase 打开页面时调用。
            // 这里把插件对象和 ParamsBlobAnalysis 传给界面层，后续所有界面读写都围绕这个参数类。
            PluginObject = plugin as BlobAnalysisObj;
            PluginParams = PluginObject == null ? null : PluginObject.PluginParams;
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p == null) return;

            vm.BindFrameParams(p);
        }

        public void RefreshBindings()
        {
            DataContext = null;
            DataContext = vm;
        }

        public void BeforeRun()
        {
            // 运行前提交界面绑定和当前 ViewROI 参数，再回写到 ParamsBlobAnalysis。
            CommitBindings();
            SaveCurrentViewRoi();
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p == null) return;
            vm.CommitToFrameParams();
        }

        public void AfterRun(bool success, long elapsedMs)
        {
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p != null)
            {
                vm.LoadResultsFromParams(p);
                vm.StatusText = success ? "Blob Analysis OK" : "Blob Analysis failed: " + p.m_strRunMessage;
                vm.RunSummary = string.Format(CultureInfo.InvariantCulture, "Success: {0}    Blob count: {1}    Cost: {2:0.###} ms", p.m_bRunSuccess, p.m_nRegionCount, p.m_fElapsedMs);
            }
            UpdateSelectedRegion();
            mainDisplay?.Repaint();
            previewDisplay?.Repaint();
            resultDisplay?.Repaint();
            RefreshBindings();
        }

        public void BeforeSave()
        {
            // 保存前只回写 ParamsBlobAnalysis，实际保存由 PluginObject.saveParams()/PluginParams.Save() 完成。
            CommitBindings();
            SaveCurrentViewRoi();
            vm.CommitToFrameParams();
        }

        public void AfterSave(bool success)
        {
            vm.StatusText = success ? "Save params OK" : "Save params failed";
            RefreshBindings();
        }

        public void BeforeClose()
        {
            BeforeSave();
            DisposeView();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitHalconViews();
        }

        private void InitHalconViews()
        {
            if (mainDisplay != null) return;
            mainHalcon = CreateHalconControl();
            previewHalcon = CreateHalconControl();
            resultHalcon = CreateHalconControl();
            MainHost.Child = mainHalcon;
            PreviewHost.Child = previewHalcon;
            ResultHost.Child = resultHalcon;
            mainDisplay = new HalconViewRoiAdapter(mainHalcon, true);
            previewDisplay = new HalconViewRoiAdapter(previewHalcon, false);
            resultDisplay = new HalconViewRoiAdapter(resultHalcon, false);
            mainDisplay.Redraw += DrawMainOverlay;
            previewDisplay.Redraw += DrawPreviewOverlay;
            resultDisplay.Redraw += DrawResultOverlay;
            mainDisplay.RoiChanged += OnRoiChanged;
            mainHalcon.HMouseMove += OnMainMouseMove;
            mainHalcon.HMouseDown += OnMainMouseDown;
        }

        private static HWindowControl CreateHalconControl()
        {
            return new HWindowControl { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.FromArgb(1, 3, 8) };
        }

        private void ShowAddFilterDialog()
        {
            AddFilterRuleDialog dialog = new AddFilterRuleDialog { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true) vm.AddFilter(dialog.Rule);
        }

        private void OnLoadImageClick(object sender, RoutedEventArgs e)
        {
            // 加载图像后直接写入 ParamsBlobAnalysis.ho_Image，保持和原框架界面层习惯一致。
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog { Title = "Load Image", Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All Files|*.*" };
            if (dialog.ShowDialog(Window.GetWindow(this)) != true) return;
            vm.LoadImageToFrameParams(dialog.FileName);
            mainDisplay.ShowImage(vm.Image, true);
            previewDisplay.ShowImage(vm.Image, true);
            resultDisplay.ShowImage(vm.Image, true);
            EnsureViewRoi();
        }

        private void OnRunClick(object sender, RoutedEventArgs e)
        {
            BeforeRun();
            bool success = PluginObject != null ? PluginObject.ExecuteObj() : RunLocalOnly();
            AfterRun(success, 0);
        }

        private bool RunLocalOnly()
        {
            vm.RunLocalOnly();
            return vm.LastResult != null && vm.LastResult.Success;
        }

        private void OnSaveParamsClick(object sender, RoutedEventArgs e)
        {
            BeforeSave();
            bool ok = PluginObject != null ? PluginObject.saveParams() : (PluginParams != null && PluginParams.Save());
            AfterSave(ok);
        }

        private void OnLoadParamsClick(object sender, RoutedEventArgs e)
        {
            // 读取框架参数 XML，读出来的是 ParamsBlobAnalysis，不再读取 BlobAnalysisParams。
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p == null) return;
            object read = p.Read();
            ParamsBlobAnalysis loaded = read as ParamsBlobAnalysis;
            if (loaded == null)
            {
                vm.StatusText = "Read params failed";
                return;
            }
            loaded.ho_Image = p.ho_Image;
            loaded.filePath = p.filePath;
            if (PluginObject != null) PluginObject.SetParams(loaded);
            PluginParams = loaded;
            vm.BindFrameParams(loaded);
            if (loaded.ho_Image != null && loaded.ho_Image.IsInitialized())
            {
                mainDisplay?.ShowImage(loaded.ho_Image, true);
                previewDisplay?.ShowImage(loaded.ho_Image, true);
                resultDisplay?.ShowImage(loaded.ho_Image, true);
            }
            EnsureViewRoi();
            mainDisplay?.Repaint();
            vm.StatusText = "Read params OK";
            RefreshBindings();
        }

        private void OnFitImageClick(object sender, RoutedEventArgs e)
        {
            mainDisplay?.FitImage();
            EnsureViewRoi();
        }

        private void OnFitPreviewClick(object sender, RoutedEventArgs e) { previewDisplay?.FitImage(); }
        private void OnFitResultClick(object sender, RoutedEventArgs e) { resultDisplay?.FitImage(); }

        private void OnRoiChanged(int eventValue)
        {
            if (switchingRoi) return;
            if (eventValue != ViewROI.ROIController.EVENT_MOVING_ROI && eventValue != ViewROI.ROIController.EVENT_UPDATE_ROI && eventValue != ViewROI.ROIController.EVENT_CREATED_ROI) return;
            SaveCurrentViewRoi();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(vm.EditRole) || e.PropertyName == nameof(vm.ShowViewRoi) || e.PropertyName == nameof(vm.EnableLocalRoi) || e.PropertyName == nameof(vm.EnableMaskRoi)) EnsureViewRoi();
            if (e.PropertyName == nameof(vm.SelectedRegion))
            {
                UpdateSelectedRegion();
                mainDisplay?.Repaint();
                previewDisplay?.Repaint();
            }
        }

        private void EnsureViewRoi()
        {
            if (mainDisplay == null || vm.Image == null || !vm.Image.IsInitialized()) return;
            switchingRoi = true;
            try
            {
                mainDisplay.SetRoiOperate(vm.ShowViewRoi);
                if (!vm.ShowViewRoi) { mainDisplay.ClearRoi(); return; }
                if (vm.EditRole == RoiEditRole.Mask)
                {
                    if (vm.SelectedMaskRoi == null || !vm.EnableMaskRoi) { mainDisplay.ClearRoi(); return; }
                    RoiParam r = vm.SelectedMaskRoi.Model;
                    mainDisplay.EnsureRectangleRoi(r.Row1, r.Col1, r.Row2, r.Col2);
                }
                else
                {
                    if (!vm.EnableLocalRoi) { mainDisplay.ClearRoi(); return; }
                    mainDisplay.EnsureRectangleRoi(vm.LocalRow1, vm.LocalCol1, vm.LocalRow2, vm.LocalCol2);
                }
            }
            finally { switchingRoi = false; }
        }

        private void SaveCurrentViewRoi()
        {
            if (mainDisplay == null || !mainDisplay.TryGetRectangleRoi(out double row1, out double col1, out double row2, out double col2)) return;
            if (vm.EditRole == RoiEditRole.Mask && vm.SelectedMaskRoi != null)
            {
                vm.SelectedMaskRoi.Row1 = row1; vm.SelectedMaskRoi.Col1 = col1; vm.SelectedMaskRoi.Row2 = row2; vm.SelectedMaskRoi.Col2 = col2;
            }
            else
            {
                vm.LocalRow1 = row1; vm.LocalCol1 = col1; vm.LocalRow2 = row2; vm.LocalCol2 = col2;
            }
        }

        private void OnMainMouseMove(object sender, HMouseEventArgs e)
        {
            if (vm.Image == null || !vm.Image.IsInitialized()) return;
            try
            {
                int row = (int)Math.Round(e.Y);
                int col = (int)Math.Round(e.X);
                HOperatorSet.GetImageSize(vm.Image, out HTuple width, out HTuple height);
                if (row < 0 || col < 0 || row >= height.I || col >= width.I) return;
                HOperatorSet.GetGrayval(vm.Image, row, col, out HTuple gray);
                vm.UpdateMouse(row, col, gray.ToString());
            }
            catch { }
        }

        private void OnMainMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            vm.SetLastRightPoint(e.Y, e.X);
            mainDisplay?.Repaint();
        }

        private void DrawMainOverlay()
        {
            if (mainDisplay == null || vm.Image == null || !vm.Image.IsInitialized()) return;
            DrawRois(mainDisplay.HalconWindow);
            DrawFollowPoints(mainDisplay.HalconWindow);
            DrawResultRegions(mainDisplay.HalconWindow);
        }

        private void DrawPreviewOverlay()
        {
            if (previewDisplay == null || selectedRegion == null || !selectedRegion.IsInitialized()) return;
            try { HOperatorSet.SetColor(previewDisplay.HalconWindow, "magenta"); HOperatorSet.SetDraw(previewDisplay.HalconWindow, "margin"); HOperatorSet.SetLineWidth(previewDisplay.HalconWindow, 2); HOperatorSet.DispObj(selectedRegion, previewDisplay.HalconWindow); } catch { }
        }

        private void DrawResultOverlay()
        {
            if (resultDisplay == null) return;
            DrawResultRegions(resultDisplay.HalconWindow);
        }

        private void DrawRois(HWindow window)
        {
            try
            {
                BlobAnalysisFollowTransformer transformer = new BlobAnalysisFollowTransformer(vm.Parameters);
                HOperatorSet.SetDraw(window, "margin");
                HOperatorSet.SetLineWidth(window, 2);
                if (vm.EnableLocalRoi)
                {
                    RoiParam r = transformer.TransformRect(vm.Parameters.LocalRoi);
                    HOperatorSet.SetColor(window, "cyan");
                    HOperatorSet.DispRectangle1(window, r.Row1, r.Col1, r.Row2, r.Col2);
                    HOperatorSet.SetTposition(window, r.Row1 + 8, r.Col1 + 8);
                    HOperatorSet.WriteString(window, "Local ROI");
                }
                if (vm.EnableMaskRoi)
                {
                    HOperatorSet.SetColor(window, "magenta");
                    foreach (RoiParam mask in vm.Parameters.MaskRois)
                    {
                        if (!mask.Enabled) continue;
                        RoiParam r = transformer.TransformRect(mask);
                        HOperatorSet.DispRectangle1(window, r.Row1, r.Col1, r.Row2, r.Col2);
                        HOperatorSet.SetTposition(window, r.Row1 + 8, r.Col1 + 8);
                        HOperatorSet.WriteString(window, "Mask #" + r.Index);
                    }
                }
            }
            catch { }
        }

        private void DrawFollowPoints(HWindow window)
        {
            try
            {
                HOperatorSet.SetDraw(window, "fill");
                HOperatorSet.SetColor(window, "green");
                HOperatorSet.DispCircle(window, vm.BaseRow, vm.BaseCol, 8);
                HOperatorSet.SetTposition(window, vm.BaseRow + 12, vm.BaseCol + 12);
                HOperatorSet.WriteString(window, "Base");
                HOperatorSet.SetColor(window, "yellow");
                HOperatorSet.DispCircle(window, vm.CurrentRow, vm.CurrentCol, 8);
                HOperatorSet.SetTposition(window, vm.CurrentRow + 12, vm.CurrentCol + 12);
                HOperatorSet.WriteString(window, "Current");
            }
            catch { }
        }

        private void DrawResultRegions(HWindow window)
        {
            HObject regions = null;
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p != null) regions = p.m_ho_OutSelectedRegion;
            if ((regions == null || !regions.IsInitialized()) && vm.LastResult != null) regions = vm.LastResult.ResultRegions;
            if (regions == null || !regions.IsInitialized()) return;
            try
            {
                HOperatorSet.SetDraw(window, "margin");
                HOperatorSet.SetLineWidth(window, 2);
                HOperatorSet.SetColor(window, "orange");
                HOperatorSet.DispObj(regions, window);
                if (selectedRegion != null && selectedRegion.IsInitialized())
                {
                    HOperatorSet.SetColor(window, "magenta");
                    HOperatorSet.SetLineWidth(window, 4);
                    HOperatorSet.DispObj(selectedRegion, window);
                    HOperatorSet.SetLineWidth(window, 2);
                }
                HOperatorSet.SetColor(window, "white");
                foreach (BlobRegionFeature item in vm.Results)
                {
                    HOperatorSet.SetTposition(window, item.Row, item.Col);
                    HOperatorSet.WriteString(window, "Blob #" + item.Index);
                }
            }
            catch { }
        }

        private void UpdateSelectedRegion()
        {
            selectedRegion?.Dispose();
            selectedRegion = null;
            HObject regions = null;
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p != null) regions = p.m_ho_OutSelectedRegion;
            if ((regions == null || !regions.IsInitialized()) && vm.LastResult != null) regions = vm.LastResult.ResultRegions;
            if (vm.SelectedRegion == null || regions == null) return;
            try
            {
                HOperatorSet.CountObj(regions, out HTuple count);
                if (vm.SelectedRegion.Index > 0 && vm.SelectedRegion.Index <= count.I) HOperatorSet.SelectObj(regions, out selectedRegion, vm.SelectedRegion.Index);
            }
            catch { selectedRegion = null; }
        }

        public void DisposeView()
        {
            // selectedRegion 是界面为了高亮选中 Blob 额外 SelectObj 出来的预览对象，可以释放。
            // 输入图像 vm.Image/ParamsBlobAnalysis.ho_Image 不在这里释放。
            selectedRegion?.Dispose();
            vm.ReleaseViewResources();
        }

        private void CommitBindings()
        {
            System.Windows.Input.FocusManager.SetFocusedElement(this, this);
            UpdateBindingSources(this);
        }

        private void UpdateBindingSources(DependencyObject parent)
        {
            if (parent == null) return;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                System.Windows.Controls.TextBox textBox = child as System.Windows.Controls.TextBox;
                if (textBox != null)
                {
                    BindingExpression expression = textBox.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty);
                    if (expression != null) expression.UpdateSource();
                }
                System.Windows.Controls.CheckBox checkBox = child as System.Windows.Controls.CheckBox;
                if (checkBox != null)
                {
                    BindingExpression expression = checkBox.GetBindingExpression(System.Windows.Controls.CheckBox.IsCheckedProperty);
                    if (expression != null) expression.UpdateSource();
                }
                System.Windows.Controls.ComboBox comboBox = child as System.Windows.Controls.ComboBox;
                if (comboBox != null)
                {
                    BindingExpression expression = comboBox.GetBindingExpression(System.Windows.Controls.ComboBox.SelectedItemProperty);
                    if (expression != null) expression.UpdateSource();
                    expression = comboBox.GetBindingExpression(System.Windows.Controls.ComboBox.SelectedValueProperty);
                    if (expression != null) expression.UpdateSource();
                    expression = comboBox.GetBindingExpression(System.Windows.Controls.ComboBox.TextProperty);
                    if (expression != null) expression.UpdateSource();
                }
                UpdateBindingSources(child);
            }
        }
    }
}
