using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using Microsoft.Win32;
using Plugin.MeasureLine.Models;
using Plugin.MeasureLine.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Windows.Input;
using ViewROI;

namespace Plugin.MeasureLine.ViewModels
{
    public sealed class MeasurelineViewModel : BindableBase, IPluginWpfView
    {
        private MeasLineObj pluginObject;
        private ParamsMeasureLine pluginParams;
        private HalconViewRoiAdapter halconViewRoi;
        private readonly HWindowControl hWindowControl = new HWindowControl();

        private string statusText = "等待加载图片";
        private string mouseInfo = "图像：--  行：--  列：--  灰度：--";
        private string line1Status = "抓边拟合：--";
        private string line1Info = "抓边：未检测到";
        private string followOffsetText = "跟随偏差：X=--  Y=--  R=--";
        private double menuRow;
        private double menuCol;
        private double activeRoiAngle;
        private double crossRow = -1;
        private double crossCol = -1;
        private bool crosshairEnabled;
        private bool crosshairDragActive;
        private bool isFollowMenuOpen;
        private double[] horizontalProfile;
        private double[] verticalProfile;

        public MeasurelineViewModel()
        {
            Params.ParameterChanged += OnParameterChanged;
            LoadImageCommand = new RelayCommand<object>(_ => LoadImage());
            ToggleCrosshairCommand = new RelayCommand<object>(_ => ToggleCrosshair());
            FitWindowCommand = new RelayCommand<object>(_ => FitImage());
            SetBasePoseCommand = new RelayCommand<object>(_ => SetBasePose());
            SetCurrentPoseCommand = new RelayCommand<object>(_ => SetCurrentPose());
            CloseCommand = new RelayCommand<object>(_ => CloseFromHost());
        }

        public ModelParams Params { get; } = new ModelParams();
        public ICommand LoadImageCommand { get; }
        public ICommand ToggleCrosshairCommand { get; }
        public ICommand FitWindowCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand SetBasePoseCommand { get; }
        public ICommand SetCurrentPoseCommand { get; }
        public IPlugin PluginObject => pluginObject;
        public PluginParamsBase PluginParams => pluginParams;
        public HWindowControl MainHalconWindow => hWindowControl;
        public string StatusText { get => statusText; private set => SetProperty(ref statusText, value); }
        public string MouseInfo { get => mouseInfo; private set => SetProperty(ref mouseInfo, value); }
        public string Line1Status { get => line1Status; private set => SetProperty(ref line1Status, value); }
        public string Line1Info { get => line1Info; private set => SetProperty(ref line1Info, value); }
        public string FollowOffsetText { get => followOffsetText; private set => SetProperty(ref followOffsetText, value); }
        public bool CrosshairEnabled { get => crosshairEnabled; private set => SetProperty(ref crosshairEnabled, value); }
        public double CrossRow { get => crossRow; private set => SetProperty(ref crossRow, value); }
        public double CrossCol { get => crossCol; private set => SetProperty(ref crossCol, value); }
        public bool IsFollowMenuOpen { get => isFollowMenuOpen; set => SetProperty(ref isFollowMenuOpen, value); }
        public double[] HorizontalProfile { get => horizontalProfile; private set => SetProperty(ref horizontalProfile, value); }
        public double[] VerticalProfile { get => verticalProfile; private set => SetProperty(ref verticalProfile, value); }

        public void BindPlugin(IPlugin plugin)
        {
            pluginObject = plugin as MeasLineObj ?? new MeasLineObj();
            EnsurePluginParams();
            PullParamsToViewModel();
            InitHalconWindow();
            RefreshFromPluginParams(true);
            //FocusHalconWindow();
        }

        public void RefreshBindings() { OnPropertyChanged(string.Empty); }

        public PluginViewActionResult RunFromHost()
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                BeforeRun();
                bool success = pluginObject != null && pluginObject.ExecuteObj();
                watch.Stop();
                AfterRun(success, watch.ElapsedMilliseconds);
                return new PluginViewActionResult { Success = success, Message = success ? "运行完成" : "运行失败：" + GetRunMessage(), ElapsedMs = watch.ElapsedMilliseconds };
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                StatusText = "运行异常：" + ex.Message;
                return PluginViewActionResult.Fail("运行异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        public PluginViewActionResult SaveFromHost(string filePath)
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                BeforeSave();
                bool success;
                if (pluginObject != null)
                {
                    pluginObject.moduleInfo.filePath = filePath;
                    if (pluginObject.PluginParams != null) pluginObject.PluginParams.filePath = filePath;
                    success = pluginObject.saveParams();
                }
                else
                {
                    pluginParams.filePath = filePath;
                    success = pluginParams.Save();
                }
                watch.Stop();
                AfterSave(success);
                return success ? PluginViewActionResult.Ok("参数已保存", watch.ElapsedMilliseconds) : PluginViewActionResult.Fail("参数保存失败", watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                StatusText = "保存异常：" + ex.Message;
                return PluginViewActionResult.Fail("保存异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        public void CloseFromHost()
        {
            BeforeClose();
            if (halconViewRoi != null)
            {
                halconViewRoi.RoiChanged -= OnRoiChanged;
                halconViewRoi.MouseMoved -= OnMouseMoved;
                halconViewRoi.RightMouseDown -= OnRightMouseDown;
                halconViewRoi.RightMouseUp -= OnRightMouseUp;
                halconViewRoi.GrayProfileChanged -= OnGrayProfileChanged;
                


                halconViewRoi.Dispose();
                halconViewRoi = null;

            }
            
        }

        public void BeforeRun() { CommitToPluginParams(); }
        public void AfterRun(bool success, long elapsedMs)
        {
            LoadResultsFromParams(pluginParams);
            RefreshOverlayOnly();
            StatusText = success ? "运行完成" : "运行失败：" + GetRunMessage();
        }
        public void BeforeSave() { CommitToPluginParams(); }
        public void AfterSave(bool success) { StatusText = success ? "参数已保存" : "参数保存失败"; }
        public void BeforeClose() { IsFollowMenuOpen = false; HorizontalProfile = null; VerticalProfile = null; Params.ParameterChanged -= OnParameterChanged; }

        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            if (e == null || pluginParams == null) return;
            if (!ReferenceEquals(e.Param, pluginParams)) return;
            AfterRun(e.Success, e.ElapsedMs);
        }

        public void LoadImage()
        {
            EnsurePluginParams();
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog { Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*" };
            if (dialog.ShowDialog() != true) return;
            pluginParams.ho_Image?.Dispose();
            HOperatorSet.ReadImage(out pluginParams.ho_Image, dialog.FileName);
            PullParamsToViewModel();
            ResetResult();
            SyncOverlay(true);
            StatusText = "图片已加载：" + System.IO.Path.GetFileName(dialog.FileName);
            FocusHalconWindow();
        }

        public void ToggleCrosshair()
        {
            CrosshairEnabled = !CrosshairEnabled;
            halconViewRoi.SetRoiOperate(!CrosshairEnabled);
            SyncOverlay(false);
            StatusText = CrosshairEnabled ? "十字光标已开启" : "十字光标已关闭";
        }

        public void FitImage()
        {
            SyncOverlay(true);
            StatusText = "已适应窗口";
            FocusHalconWindow();
        }

        public void SetBasePose()
        {
            Params.BaseRow = menuRow;
            Params.BaseColumn = menuCol;
            Params.BaseAngle = activeRoiAngle;
            IsFollowMenuOpen = false;
            CommitToPluginParams();
            SyncOverlay(false);
        }

        public void SetCurrentPose()
        {
            Params.CurrentRow = menuRow;
            Params.CurrentColumn = menuCol;
            Params.CurrentAngle = activeRoiAngle;
            Params.CurrentPoseValid = true;
            Params.EnablePositionFollow = true;
            IsFollowMenuOpen = false;
            CommitToPluginParams();
            SyncOverlay(false);
        }

        private void InitHalconWindow()
        {
            if (halconViewRoi != null) return;
            halconViewRoi = new HalconViewRoiAdapter(hWindowControl, true);
            halconViewRoi.RoiChanged += OnRoiChanged;
            halconViewRoi.MouseMoved += OnMouseMoved;
            halconViewRoi.RightMouseDown += OnRightMouseDown;
            halconViewRoi.RightMouseUp += OnRightMouseUp;
            halconViewRoi.GrayProfileChanged += OnGrayProfileChanged;
            halconViewRoi.RefreshFromParams(pluginParams,true);
        }

        private void OnMouseMoved(HalconMouseInfoArgs args)
        {
            if (args == null) return;
            MouseInfo = args.Text;
            if (CrosshairEnabled && crosshairDragActive)
            {
                CrossRow = args.Row;
                CrossCol = args.Column;
                SyncOverlay(false);
            }
        }

        private void OnRightMouseDown(HalconMouseInfoArgs args)
        {
            if (args == null) return;
            menuRow = args.Row;
            menuCol = args.Column;
            if (CrosshairEnabled)
            {
                crosshairDragActive = true;
                CrossRow = args.Row;
                CrossCol = args.Column;
                SyncOverlay(false);
            }

            ROIRectangle2 roi = halconViewRoi == null ? null : halconViewRoi.GetActiveRectangleRoi();
            if (roi != null)
            {
                HOperatorSet.TupleDeg(roi.phi, out HTuple deg);
                activeRoiAngle = deg.D;
            }

            IsFollowMenuOpen = true;
        }

        private void OnRightMouseUp(HalconMouseInfoArgs args) { crosshairDragActive = false; }
        private void OnGrayProfileChanged(double[] horizontal, double[] vertical) { HorizontalProfile = horizontal; VerticalProfile = vertical; }

        private void OnRoiChanged(int eventCode)
        {
            ROIRectangle2 roi = halconViewRoi == null ? null : halconViewRoi.GetActiveRectangleRoi();
            if (roi == null) return;

            HOperatorSet.TupleDeg(roi.phi, out HTuple deg);
            activeRoiAngle = deg.D;
            Params.UpdateFromRoi(roi.midR, roi.midC, activeRoiAngle, roi.length1 * 2.0, roi.length2 * 2.0, roi.PointNum);
            Params.Apply();
            CommitToPluginParams();
            if(eventCode==ROIController.EVENT_UPDATE_ROI)
            {
                RunFromHost();
                SyncOverlay(false);
            }
         
        }

        private void OnParameterChanged(object sender, EventArgs e)
        {
            EnsurePluginParams();
            CommitToPluginParams();
            if (halconViewRoi != null)
            {
                halconViewRoi.SyncActiveRoi(
                    Params.Line1CenterRow,
                    Params.Line1CenterColumn,
                    Params.Line1Angle,
                    Params.Line1Height,
                    Params.Line1Width,
                    (int)Math.Round(Params.Line1PointCount, MidpointRounding.AwayFromZero));
            }
            SyncOverlay(false);
            RunFromHost();
        }

        private void RefreshFromPluginParams(bool fit)
        {
            EnsurePluginParams();
            PullParamsToViewModel();
            LoadResultsFromParams(pluginParams);
            SyncOverlay(fit);
        }

        private void SyncOverlay(bool fit)
        {
            if (halconViewRoi == null) return;
            halconViewRoi.SetOverlayState(pluginParams, CrosshairEnabled, CrossRow, CrossCol);
            halconViewRoi.RefreshFromParams(pluginParams, fit);
        }

        private void RefreshOverlayOnly()
        {
            if (halconViewRoi == null) return;
            halconViewRoi.SetOverlayState(pluginParams, CrosshairEnabled, CrossRow, CrossCol);
            halconViewRoi.Repaint();
            FocusHalconWindow();
        }

        private void FocusHalconWindow()
        {
            try
            {
                if (hWindowControl != null)
                {
                    hWindowControl.Focus();
                }
            }
            catch
            {
            }
        }

        private void LoadResultsFromParams(ParamsMeasureLine source)
        {
            if (source == null) { ResetResult(); return; }
            FollowOffsetText = string.Format("跟随偏差：X={0:F3}  Y={1:F3}  R={2:F3}°", source.m_fFollowOffsetX, source.m_fFollowOffsetY, source.m_fFollowOffsetR);
            Line1Status = source.bl_FitSuccess ? "抓边拟合：成功" : "抓边拟合：失败";
            Line1Info = FormatLine("抓边", source.bl_FitSuccess, source.hv_LineRowBegin, source.hv_LineColumnBegin, source.hv_LineRowEnd, source.hv_LineColumnEnd);
        }

        private void ResetResult()
        {
            Line1Status = "抓边拟合：--";
            Line1Info = "抓边：未检测到";
            FollowOffsetText = "跟随偏差：X=--  Y=--  R=--";
        }

        private string GetRunMessage() { return pluginParams == null ? string.Empty : pluginParams.m_strRunMessage; }
        private void PullParamsToViewModel() { EnsurePluginParams(); Params.Bind(pluginParams); }
        private void CommitToPluginParams() { EnsurePluginParams(); Params.Apply(); }

        private ParamsMeasureLine EnsurePluginParams()
        {
            if (pluginObject == null) pluginObject = new MeasLineObj();
            if (pluginObject.PluginParams == null) pluginObject.PluginParams = new ParamsMeasureLine();
            pluginParams = pluginObject.PluginParams as ParamsMeasureLine ?? new ParamsMeasureLine();
            pluginObject.PluginParams = pluginParams;
            return pluginParams;
        }

        private static string FormatLine(string name, bool success, HTuple rowBegin, HTuple columnBegin, HTuple rowEnd, HTuple columnEnd)
        {
            if (!success || rowBegin == null || columnBegin == null || rowEnd == null || columnEnd == null) return name + "：未检测到";
            return string.Format("{0}：({1:F1},{2:F1}) -> ({3:F1},{4:F1})", name, rowBegin.D, columnBegin.D, rowEnd.D, columnEnd.D);
        }
    }
}
