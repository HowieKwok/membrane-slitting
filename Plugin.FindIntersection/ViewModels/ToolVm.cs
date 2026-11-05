using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using Microsoft.Win32;
using Plugin.FindIntersection.Controls;
using ViewROI;

namespace Plugin.FindIntersection.ViewModels
{
    /// <summary>
    /// 两线交点工具 ViewModel。
    /// VM 只处理状态、参数、命令、插件生命周期，不引用任何 WPF 控件实例。
    /// </summary>
    public sealed class ToolVm : BindableBase, IPluginWpfView
    {
        private FindIntersectionObj pluginObject;
        private ParamsFindIntersection pluginParams;

        private string statusText = "等待加载图片";
        private string mouseInfo = "图像：--  行：--  列：--  灰度：--";
        private string line1Status = "抓边1拟合：--";
        private string line2Status = "抓边2拟合：--";
        private string line1Info = "抓边1：未检测到";
        private string line2Info = "抓边2：未检测到";
        private string parallelText = "是否平行：--";
        private string intersectionText = "交点：R=--  C=--";
        private string followOffsetText = "跟随偏差：X=--  Y=--  R=--";
        private double intersectionRow;
        private double intersectionColumn;
        private double angle;
        private double menuRow;
        private double menuCol;
        private double activeRoiAngle;
        private double crossRow = -1;
        private double crossCol = -1;
        private bool crosshairEnabled;
        private bool crosshairDragActive;
        private bool isFollowMenuOpen;
        private bool showEmptyHint = true;
        private bool fitOnNextDisplay;
        private int displayRevision;
        private HObject displayImage;
        private HObject ownedLoadedImage;
        private double[] horizontalProfile;
        private double[] verticalProfile;

        public ToolVm()
        {
            Params.ParameterChanged += OnParameterChanged;

            LoadImageCommand = new RelayCommand<object>(_ => LoadImage());
            ToggleCrosshairCommand = new RelayCommand<object>(_ => ToggleCrosshair());
            FitWindowCommand = new RelayCommand<object>(_ => FitImage());
            KeepRoiCommand = new RelayCommand<object>(_ => KeepRoi());
            RunCommand = new RelayCommand<object>(_ => Run());
            SetBasePoseCommand = new RelayCommand<object>(_ => SetBasePose());
            SetCurrentPoseCommand = new RelayCommand<object>(_ => SetCurrentPose());

            RoiChangedCommand = new RelayCommand<FindIntersectionRoiChangedArgs>(OnRoiChanged);
            MouseMoveCommand = new RelayCommand<HalconMouseInfoArgs>(OnMouseMove);
            RightMouseDownCommand = new RelayCommand<HalconMouseInfoArgs>(OnRightMouseDown);
            RightMouseUpCommand = new RelayCommand<HalconMouseInfoArgs>(OnRightMouseUp);
        }

        public ToolParams Params { get; } = new ToolParams();

        public ICommand LoadImageCommand { get; private set; }
        public ICommand ToggleCrosshairCommand { get; private set; }
        public ICommand FitWindowCommand { get; private set; }
        public ICommand KeepRoiCommand { get; private set; }
        public ICommand RunCommand { get; private set; }
        public ICommand SetBasePoseCommand { get; private set; }
        public ICommand SetCurrentPoseCommand { get; private set; }
        public ICommand RoiChangedCommand { get; private set; }
        public ICommand MouseMoveCommand { get; private set; }
        public ICommand RightMouseDownCommand { get; private set; }
        public ICommand RightMouseUpCommand { get; private set; }

        public IPlugin PluginObject { get { return pluginObject; } }
        public PluginParamsBase PluginParams { get { return pluginParams; } }

        public HObject DisplayImage { get { return displayImage; } private set { SetProperty(ref displayImage, value); } }
        public int DisplayRevision { get { return displayRevision; } private set { SetProperty(ref displayRevision, value); } }
        public bool FitOnNextDisplay { get { return fitOnNextDisplay; } private set { SetProperty(ref fitOnNextDisplay, value); } }
        public bool ShowEmptyHint { get { return showEmptyHint; } private set { SetProperty(ref showEmptyHint, value); } }
        public double[] HorizontalProfile { get { return horizontalProfile; } private set { SetProperty(ref horizontalProfile, value); } }
        public double[] VerticalProfile { get { return verticalProfile; } private set { SetProperty(ref verticalProfile, value); } }

        public bool CrosshairEnabled { get { return crosshairEnabled; } private set { SetProperty(ref crosshairEnabled, value); } }
        public double CrossRow { get { return crossRow; } private set { SetProperty(ref crossRow, value); } }
        public double CrossCol { get { return crossCol; } private set { SetProperty(ref crossCol, value); } }
        public bool IsFollowMenuOpen { get { return isFollowMenuOpen; } set { SetProperty(ref isFollowMenuOpen, value); } }

        public double IntersectionRow { get { return intersectionRow; } private set { SetProperty(ref intersectionRow, value); } }
        public double IntersectionColumn { get { return intersectionColumn; } private set { SetProperty(ref intersectionColumn, value); } }
        public double Angle { get { return angle; } private set { SetProperty(ref angle, value); } }
        public string StatusText { get { return statusText; } private set { SetProperty(ref statusText, value); } }
        public string MouseInfo { get { return mouseInfo; } private set { SetProperty(ref mouseInfo, value); } }
        public string Line1Status { get { return line1Status; } private set { SetProperty(ref line1Status, value); } }
        public string Line2Status { get { return line2Status; } private set { SetProperty(ref line2Status, value); } }
        public string Line1Info { get { return line1Info; } private set { SetProperty(ref line1Info, value); } }
        public string Line2Info { get { return line2Info; } private set { SetProperty(ref line2Info, value); } }
        public string ParallelText { get { return parallelText; } private set { SetProperty(ref parallelText, value); } }
        public string IntersectionText { get { return intersectionText; } private set { SetProperty(ref intersectionText, value); } }
        public string FollowOffsetText { get { return followOffsetText; } private set { SetProperty(ref followOffsetText, value); } }

        public void BindPlugin(IPlugin plugin)
        {
            pluginObject = plugin as FindIntersectionObj ?? new FindIntersectionObj();
            EnsurePluginParams();
            PullParamsToViewModel();
            RefreshFromPluginParams(true);
        }

        public void RefreshBindings()
        {
            OnPropertyChanged(string.Empty);
        }

        public PluginViewActionResult RunFromHost()
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool success = false;

            try
            {
                BeforeRun();
                success = pluginObject != null && pluginObject.ExecuteObj();
                watch.Stop();
                AfterRun(success, watch.ElapsedMilliseconds);

                string message = success ? "两线交点工具运行完成" : "两线交点工具运行失败：" + GetRunMessage();
                return new PluginViewActionResult { Success = success, Message = message, ElapsedMs = watch.ElapsedMilliseconds };
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                if (pluginParams != null)
                {
                    pluginParams.m_bRunSuccess = false;
                    pluginParams.m_strRunMessage = ex.Message;
                    pluginParams.m_fElapsedMs = watch.ElapsedMilliseconds;
                }

                StatusText = "运行异常：" + ex.Message;
                return PluginViewActionResult.Fail("两线交点工具运行异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        public PluginViewActionResult SaveFromHost(string filePath)
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool success = false;

            try
            {
                BeforeSave();
                if (pluginObject != null)
                {
                    pluginObject.moduleInfo.filePath = filePath;
                    if (pluginObject.PluginParams != null) pluginObject.PluginParams.filePath = filePath;
                    success = pluginObject.saveParams();
                }
                else if (pluginParams != null)
                {
                    pluginParams.filePath = filePath;
                    success = pluginParams.Save();
                }

                watch.Stop();
                AfterSave(success);
                return success
                    ? PluginViewActionResult.Ok("两线交点参数已保存", watch.ElapsedMilliseconds)
                    : PluginViewActionResult.Fail("两线交点参数保存失败", watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                StatusText = "保存异常：" + ex.Message;
                return PluginViewActionResult.Fail("两线交点参数保存异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        public void CloseFromHost()
        {
            BeforeClose();
        }

        public void BeforeRun()
        {
            CommitToPluginParams();
        }

        public void AfterRun(bool success, long elapsedMs)
        {
            LoadResultsFromParams(pluginParams);
            StatusText = success ? "运行完成" : "运行失败：" + GetRunMessage();
            RequestRepaint(false);
        }

        public void BeforeSave()
        {
            CommitToPluginParams();
        }

        public void AfterSave(bool success)
        {
            StatusText = success ? "参数已保存" : "参数保存失败";
        }

        public void BeforeClose()
        {
            IsFollowMenuOpen = false;
            HorizontalProfile = null;
            VerticalProfile = null;
        }

        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            if (e == null || pluginParams == null) return;
            if (!ReferenceEquals(e.Param, pluginParams)) return;
            RefreshFromPluginParams(false);
            AfterRun(e.Success, e.ElapsedMs);
        }

        public void LoadImage()
        {
            EnsurePluginParams();

            try
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*"
                };

                if (dialog.ShowDialog() != true) return;

                ReplaceOwnedImage(new HImage(dialog.FileName));
                DisplayImage = HasValidImage(pluginParams.ho_Image) ? pluginParams.ho_Image : null;
                ShowEmptyHint = DisplayImage == null;
                PullParamsToViewModel();
                ResetResult();
                RequestRepaint(true);
                StatusText = "图片已加载：" + System.IO.Path.GetFileName(dialog.FileName);
            }
            catch (Exception ex)
            {
                StatusText = "加载图片失败：" + ex.Message;
            }
        }

        public void ToggleCrosshair()
        {
            CrosshairEnabled = !CrosshairEnabled;
            StatusText = CrosshairEnabled ? "十字光标已开启，按住右键查看灰度曲线" : "十字光标已关闭";
            RequestRepaint(false);
        }

        public void FitImage()
        {
            RequestRepaint(true);
            StatusText = "已适应窗口";
        }

        public void KeepRoi()
        {
            CommitToPluginParams();
            RequestRepaint(false);
            StatusText = "ROI 已写入参数，点击运行刷新结果";
        }

        public void Run()
        {
            RunFromHost();
        }

        public void SetBasePose()
        {
            Params.BaseRow = menuRow;
            Params.BaseColumn = menuCol;
            Params.BaseAngle = activeRoiAngle;
            IsFollowMenuOpen = false;
            CommitToPluginParams();
            RequestRepaint(false);
            StatusText = string.Format("已设置基坐标基准：R={0:F2}  C={1:F2}  A={2:F3}", menuRow, menuCol, activeRoiAngle);
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
            RequestRepaint(false);
            StatusText = string.Format("已设置当前输入坐标：R={0:F2}  C={1:F2}  A={2:F3}", menuRow, menuCol, activeRoiAngle);
        }

        private void OnRoiChanged(FindIntersectionRoiChangedArgs args)
        {
            if (args == null) return;

            Params.UpdateFromRoi(1, args.Line1.Row, args.Line1.Column, args.Line1.AngleDegree, args.Line1.Height, args.Line1.Width, args.Line1.PointCount);
            Params.UpdateFromRoi(2, args.Line2.Row, args.Line2.Column, args.Line2.AngleDegree, args.Line2.Height, args.Line2.Width, args.Line2.PointCount);
            activeRoiAngle = args.ActiveAngleDegree;
            CommitToPluginParams();
            RequestRepaint(false);

            if (args.EventCode == ROIController.EVENT_UPDATE_ROI)
            {
                pluginObject.ExecuteObj();
                StatusText = "ROI 已更新，点击运行刷新结果";
            }
        }

        private void OnMouseMove(HalconMouseInfoArgs args)
        {
            if (args == null) return;
            MouseInfo = args.Text;

            if (CrosshairEnabled && crosshairDragActive)
            {
                UpdateCrosshair(args.Row, args.Column);
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
                UpdateCrosshair(args.Row, args.Column);
            }

            IsFollowMenuOpen = true;
        }

        private void OnRightMouseUp(HalconMouseInfoArgs args)
        {
            crosshairDragActive = false;
        }

        private ParamsFindIntersection EnsurePluginParams()
        {
            if (pluginObject == null) pluginObject = new FindIntersectionObj();
            if (pluginObject.PluginParams == null) pluginObject.PluginParams = new ParamsFindIntersection();

            pluginParams = pluginObject.PluginParams as ParamsFindIntersection;
            if (pluginParams == null)
            {
                pluginParams = new ParamsFindIntersection();
                pluginObject.PluginParams = pluginParams;
            }

            return pluginParams;
        }

        private void PullParamsToViewModel()
        {
            EnsurePluginParams();
            Params.Bind(pluginParams);
        }

        private void CommitToPluginParams()
        {
            EnsurePluginParams();
            Params.Apply();
        }

        private void RefreshFromPluginParams(bool fit)
        {
            EnsurePluginParams();
            PullParamsToViewModel();
            DisplayImage = HasValidImage(pluginParams.ho_Image) ? pluginParams.ho_Image : null;
            ShowEmptyHint = DisplayImage == null;
            LoadResultsFromParams(pluginParams);
            RequestRepaint(fit);
        }

        private void RequestRepaint(bool fit)
        {
            FitOnNextDisplay = fit;
            DisplayRevision++;
            if (fit)
            {
                FitOnNextDisplay = false;
            }
        }

        private void OnParameterChanged(object sender, EventArgs e)
        {
            CommitToPluginParams();
            RequestRepaint(false);
        }

        private void UpdateCrosshair(double row, double col)
        {
            CrossRow = row;
            CrossCol = col;
            ExtractGrayProfiles(row, col);
            RequestRepaint(false);
        }

        private void ExtractGrayProfiles(double row, double col)
        {
            try
            {
                if (pluginParams == null || pluginParams.ho_Image == null || !pluginParams.ho_Image.IsInitialized())
                {
                    return;
                }

                HObject profileImage = pluginParams.ho_Image;
                HObject channelImage = null;

                try
                {
                    HOperatorSet.CountChannels(profileImage, out HTuple channels);
                    if (channels.I > 1)
                    {
                        HOperatorSet.AccessChannel(profileImage, out channelImage, 1);
                        profileImage = channelImage;
                    }

                    HOperatorSet.GetImageSize(profileImage, out HTuple width, out HTuple height);
                    int w = Math.Max(0, width.I);
                    int h = Math.Max(0, height.I);
                    if (w <= 0 || h <= 0) return;

                    int r = Math.Max(0, Math.Min(h - 1, (int)Math.Round(row)));
                    int c = Math.Max(0, Math.Min(w - 1, (int)Math.Round(col)));

                    List<double> hValues = new List<double>(w);
                    for (int x = 0; x < w; x++)
                    {
                        HOperatorSet.GetGrayval(profileImage, r, x, out HTuple gray);
                        hValues.Add(gray.D);
                    }

                    List<double> vValues = new List<double>(h);
                    for (int y = 0; y < h; y++)
                    {
                        HOperatorSet.GetGrayval(profileImage, y, c, out HTuple gray);
                        vValues.Add(gray.D);
                    }

                    HorizontalProfile = hValues.ToArray();
                    VerticalProfile = vValues.ToArray();
                }
                finally
                {
                    channelImage?.Dispose();
                }
            }
            catch
            {
            }
        }

        private void LoadResultsFromParams(ParamsFindIntersection source)
        {
            if (source == null)
            {
                ResetResult();
                return;
            }

            FollowOffsetText = string.Format("跟随偏差：X={0:F3}  Y={1:F3}  R={2:F3}°", source.m_fFollowOffsetX, source.m_fFollowOffsetY, source.m_fFollowOffsetR);
            Line1Status = source.bl_Line1_FitSuccess ? "抓边1拟合：成功" : "抓边1拟合：失败";
            Line2Status = source.bl_Line2_FitSuccess ? "抓边2拟合：成功" : "抓边2拟合：失败";
            Line1Info = FormatLine("抓边1", source.bl_Line1_FitSuccess, source.hv_Line1_RowBegin, source.hv_Line1_ColumnBegin, source.hv_Line1_RowEnd, source.hv_Line1_ColumnEnd);
            Line2Info = FormatLine("抓边2", source.bl_Line2_FitSuccess, source.hv_Line2_RowBegin, source.hv_Line2_ColumnBegin, source.hv_Line2_RowEnd, source.hv_Line2_ColumnEnd);

            if (source.bl_IntersectionSuccess &&
                source.hv_IntersectionRow != null &&
                source.hv_IntersectionColumn != null &&
                source.hv_IsParallel != null &&
                source.hv_IsParallel.I == 0)
            {
                IntersectionRow = source.hv_IntersectionRow.D;
                IntersectionColumn = source.hv_IntersectionColumn.D;
                Angle = source.hv_IntersectionAngle == null ? 0 : source.hv_IntersectionAngle.D;
                IntersectionText = string.Format("交点：R={0:F2}  C={1:F2}", IntersectionRow, IntersectionColumn);
                ParallelText = "是否平行：否";
                return;
            }

            IntersectionText = "交点：R=--  C=--";
            ParallelText = source.hv_IsParallel != null && source.hv_IsParallel.I != 0 ? "是否平行：是" : "是否平行：--";
        }

        private void ResetResult()
        {
            Line1Status = "抓边1拟合：--";
            Line2Status = "抓边2拟合：--";
            Line1Info = "抓边1：未检测到";
            Line2Info = "抓边2：未检测到";
            ParallelText = "是否平行：--";
            IntersectionText = "交点：R=--  C=--";
            IntersectionRow = 0;
            IntersectionColumn = 0;
            Angle = 0;
            FollowOffsetText = "跟随偏差：X=--  Y=--  R=--";
        }

        private string GetRunMessage()
        {
            return pluginParams == null ? string.Empty : pluginParams.m_strRunMessage;
        }

        private static string FormatLine(string name, bool success, HTuple rowBegin, HTuple columnBegin, HTuple rowEnd, HTuple columnEnd)
        {
            if (!success || rowBegin == null || columnBegin == null || rowEnd == null || columnEnd == null)
            {
                return name + "：未检测到";
            }

            return string.Format("{0}：({1:F1},{2:F1}) -> ({3:F1},{4:F1})", name, rowBegin.D, columnBegin.D, rowEnd.D, columnEnd.D);
        }

        private static bool HasValidImage(HObject image)
        {
            if (image == null || !image.IsInitialized()) return false;

            try
            {
                HOperatorSet.CountObj(image, out HTuple count);
                if (count.I <= 0) return false;
                HOperatorSet.GetObjClass(image, out HTuple objClass);
                return string.Equals(objClass.S, "image", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void ReplaceOwnedImage(HObject newImage)
        {
            EnsurePluginParams();
            pluginParams.ho_Image?.Dispose();
            HObject oldOwned = ownedLoadedImage;
            ownedLoadedImage = newImage;
            pluginParams.ho_Image = newImage;

            if (oldOwned != null && !ReferenceEquals(oldOwned, newImage))
            {
                try
                {
                    oldOwned.Dispose();
                }
                catch
                {
                }
            }
        }
    }

    /// <summary>
    /// WPF 参数转接类。
    /// 它承接框架参数，供 XAML 绑定，避免 XAML 直接绑 ParamsFindIntersection 字段。
    /// </summary>
    public sealed class ToolParams : BindableBase
    {
        private ParamsFindIntersection frameParams;
        private bool syncing;
        private double line1CenterRow;
        private double line1CenterColumn;
        private double line1Angle;
        private double line1Height;
        private double line1Width;
        private double line1PointCount;
        private double line1Sigma;
        private double line1Threshold;
        private double line1NumInstances;
        private double line1DistThreshold;
        private double line1MinScore;
        private double line1MaxIterations;
        private string line1Transition;
        private string line1Select;
        private double line2CenterRow;
        private double line2CenterColumn;
        private double line2Angle;
        private double line2Height;
        private double line2Width;
        private double line2PointCount;
        private double line2Sigma;
        private double line2Threshold;
        private double line2NumInstances;
        private double line2DistThreshold;
        private double line2MinScore;
        private double line2MaxIterations;
        private string line2Transition;
        private string line2Select;
        private bool enablePositionFollow;
        private double baseRow;
        private double baseColumn;
        private double baseAngle;
        private double currentRow;
        private double currentColumn;
        private double currentAngle;
        private bool currentPoseValid;

        public event EventHandler ParameterChanged;

        public void Bind(ParamsFindIntersection source)
        {
            frameParams = source ?? new ParamsFindIntersection();
            syncing = true;

            try
            {
                Line1CenterRow = frameParams.hv_Line1_RoiMidRow;
                Line1CenterColumn = frameParams.hv_Line1_RoiMidColumn;
                Line1Angle = frameParams.hv_Line1_RoiAngle;
                Line1Height = frameParams.hv_Line1_RoiHeight;
                Line1Width = frameParams.hv_Line1_RoiWidth;
                Line1PointCount = frameParams.hv_Line1_RoiPointsNum;
                Line1Sigma = frameParams.hv_Line1_MeasureSigma;
                Line1Threshold = frameParams.hv_Line1_MeasureThreshold;
                Line1NumInstances = frameParams.hv_Line1_MeasureNumInstances;
                Line1DistThreshold = frameParams.hv_Line1_MeasureDistThreshold;
                Line1MinScore = frameParams.hv_Line1_MeasureMinScore;
                Line1MaxIterations = frameParams.hv_Line1_MaxNumIterations;
                Line1Transition = frameParams.hv_Line1_MeasureTransition;
                Line1Select = frameParams.hv_Line1_MeasureSelect;

                Line2CenterRow = frameParams.hv_Line2_RoiMidRow;
                Line2CenterColumn = frameParams.hv_Line2_RoiMidColumn;
                Line2Angle = frameParams.hv_Line2_RoiAngle;
                Line2Height = frameParams.hv_Line2_RoiHeight;
                Line2Width = frameParams.hv_Line2_RoiWidth;
                Line2PointCount = frameParams.hv_Line2_RoiPointsNum;
                Line2Sigma = frameParams.hv_Line2_MeasureSigma;
                Line2Threshold = frameParams.hv_Line2_MeasureThreshold;
                Line2NumInstances = frameParams.hv_Line2_MeasureNumInstances;
                Line2DistThreshold = frameParams.hv_Line2_MeasureDistThreshold;
                Line2MinScore = frameParams.hv_Line2_MeasureMinScore;
                Line2MaxIterations = frameParams.hv_Line2_MaxNumIterations;
                Line2Transition = frameParams.hv_Line2_MeasureTransition;
                Line2Select = frameParams.hv_Line2_MeasureSelect;

                EnablePositionFollow = frameParams.m_bUsePositionFollow;
                BaseRow = frameParams.m_fBaseRow;
                BaseColumn = frameParams.m_fBaseCol;
                BaseAngle = frameParams.m_fBaseAngle;
                CurrentRow = frameParams.m_fCurrentRow;
                CurrentColumn = frameParams.m_fCurrentCol;
                CurrentAngle = frameParams.m_fCurrentAngle;
                CurrentPoseValid = frameParams.m_bCurrentPoseValid;
            }
            finally
            {
                syncing = false;
            }
        }

        public void Apply()
        {
            if (frameParams == null) return;

            frameParams.hv_Line1_RoiMidRow = Line1CenterRow;
            frameParams.hv_Line1_RoiMidColumn = Line1CenterColumn;
            frameParams.hv_Line1_RoiAngle = Line1Angle;
            frameParams.hv_Line1_RoiHeight = Line1Height;
            frameParams.hv_Line1_RoiWidth = Line1Width;
            frameParams.hv_Line1_RoiPointsNum = ToInt(Line1PointCount);
            frameParams.hv_Line1_MeasureSigma = Line1Sigma;
            frameParams.hv_Line1_MeasureThreshold = ToInt(Line1Threshold);
            frameParams.hv_Line1_MeasureNumInstances = ToInt(Line1NumInstances);
            frameParams.hv_Line1_MeasureDistThreshold = Line1DistThreshold;
            frameParams.hv_Line1_MeasureMinScore = Line1MinScore;
            frameParams.hv_Line1_MaxNumIterations = ToInt(Line1MaxIterations);
            frameParams.hv_Line1_MeasureTransition = Line1Transition;
            frameParams.hv_Line1_MeasureSelect = Line1Select;

            frameParams.hv_Line2_RoiMidRow = Line2CenterRow;
            frameParams.hv_Line2_RoiMidColumn = Line2CenterColumn;
            frameParams.hv_Line2_RoiAngle = Line2Angle;
            frameParams.hv_Line2_RoiHeight = Line2Height;
            frameParams.hv_Line2_RoiWidth = Line2Width;
            frameParams.hv_Line2_RoiPointsNum = ToInt(Line2PointCount);
            frameParams.hv_Line2_MeasureSigma = Line2Sigma;
            frameParams.hv_Line2_MeasureThreshold = ToInt(Line2Threshold);
            frameParams.hv_Line2_MeasureNumInstances = ToInt(Line2NumInstances);
            frameParams.hv_Line2_MeasureDistThreshold = Line2DistThreshold;
            frameParams.hv_Line2_MeasureMinScore = Line2MinScore;
            frameParams.hv_Line2_MaxNumIterations = ToInt(Line2MaxIterations);
            frameParams.hv_Line2_MeasureTransition = Line2Transition;
            frameParams.hv_Line2_MeasureSelect = Line2Select;

            frameParams.m_bUsePositionFollow = EnablePositionFollow;
            frameParams.m_fBaseRow = BaseRow;
            frameParams.m_fBaseCol = BaseColumn;
            frameParams.m_fBaseAngle = BaseAngle;
            frameParams.m_fCurrentRow = CurrentRow;
            frameParams.m_fCurrentCol = CurrentColumn;
            frameParams.m_fCurrentAngle = CurrentAngle;
            frameParams.m_bCurrentPoseValid = CurrentPoseValid;
        }

        public void UpdateFromRoi(int index, double row, double column, double angleDegree, double height, double width, int pointCount)
        {
            syncing = true;

            try
            {
                if (index == 1)
                {
                    Line1CenterRow = row;
                    Line1CenterColumn = column;
                    Line1Angle = angleDegree;
                    Line1Height = height;
                    Line1Width = width;
                    Line1PointCount = pointCount;
                }
                else
                {
                    Line2CenterRow = row;
                    Line2CenterColumn = column;
                    Line2Angle = angleDegree;
                    Line2Height = height;
                    Line2Width = width;
                    Line2PointCount = pointCount;
                }
            }
            finally
            {
                syncing = false;
            }
        }

        public double Line1CenterRow { get { return line1CenterRow; } set { SetValue(ref line1CenterRow, value); } }
        public double Line1CenterColumn { get { return line1CenterColumn; } set { SetValue(ref line1CenterColumn, value); } }
        public double Line1Angle { get { return line1Angle; } set { SetValue(ref line1Angle, value); } }
        public double Line1Height { get { return line1Height; } set { SetValue(ref line1Height, value); } }
        public double Line1Width { get { return line1Width; } set { SetValue(ref line1Width, value); } }
        public double Line1PointCount { get { return line1PointCount; } set { SetValue(ref line1PointCount, value); } }
        public double Line1Sigma { get { return line1Sigma; } set { SetValue(ref line1Sigma, value); } }
        public double Line1Threshold { get { return line1Threshold; } set { SetValue(ref line1Threshold, value); } }
        public double Line1NumInstances { get { return line1NumInstances; } set { SetValue(ref line1NumInstances, value); } }
        public double Line1DistThreshold { get { return line1DistThreshold; } set { SetValue(ref line1DistThreshold, value); } }
        public double Line1MinScore { get { return line1MinScore; } set { SetValue(ref line1MinScore, value); } }
        public double Line1MaxIterations { get { return line1MaxIterations; } set { SetValue(ref line1MaxIterations, value); } }
        public string Line1Transition { get { return line1Transition; } set { SetValue(ref line1Transition, value); } }
        public string Line1Select { get { return line1Select; } set { SetValue(ref line1Select, value); } }

        public double Line2CenterRow { get { return line2CenterRow; } set { SetValue(ref line2CenterRow, value); } }
        public double Line2CenterColumn { get { return line2CenterColumn; } set { SetValue(ref line2CenterColumn, value); } }
        public double Line2Angle { get { return line2Angle; } set { SetValue(ref line2Angle, value); } }
        public double Line2Height { get { return line2Height; } set { SetValue(ref line2Height, value); } }
        public double Line2Width { get { return line2Width; } set { SetValue(ref line2Width, value); } }
        public double Line2PointCount { get { return line2PointCount; } set { SetValue(ref line2PointCount, value); } }
        public double Line2Sigma { get { return line2Sigma; } set { SetValue(ref line2Sigma, value); } }
        public double Line2Threshold { get { return line2Threshold; } set { SetValue(ref line2Threshold, value); } }
        public double Line2NumInstances { get { return line2NumInstances; } set { SetValue(ref line2NumInstances, value); } }
        public double Line2DistThreshold { get { return line2DistThreshold; } set { SetValue(ref line2DistThreshold, value); } }
        public double Line2MinScore { get { return line2MinScore; } set { SetValue(ref line2MinScore, value); } }
        public double Line2MaxIterations { get { return line2MaxIterations; } set { SetValue(ref line2MaxIterations, value); } }
        public string Line2Transition { get { return line2Transition; } set { SetValue(ref line2Transition, value); } }
        public string Line2Select { get { return line2Select; } set { SetValue(ref line2Select, value); } }

        public bool EnablePositionFollow { get { return enablePositionFollow; } set { SetValue(ref enablePositionFollow, value); } }
        public double BaseRow { get { return baseRow; } set { SetValue(ref baseRow, value); } }
        public double BaseColumn { get { return baseColumn; } set { SetValue(ref baseColumn, value); } }
        public double BaseAngle { get { return baseAngle; } set { SetValue(ref baseAngle, value); } }
        public double CurrentRow { get { return currentRow; } set { SetCurrentValue(ref currentRow, value); } }
        public double CurrentColumn { get { return currentColumn; } set { SetCurrentValue(ref currentColumn, value); } }
        public double CurrentAngle { get { return currentAngle; } set { SetCurrentValue(ref currentAngle, value); } }
        public bool CurrentPoseValid { get { return currentPoseValid; } set { SetValue(ref currentPoseValid, value); } }

        private void SetValue<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!SetProperty(ref storage, value, propertyName)) return;
            if (!syncing) ParameterChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetCurrentValue(ref double storage, double value, [CallerMemberName] string propertyName = null)
        {
            if (!SetProperty(ref storage, value, propertyName)) return;

            if (!syncing && !CurrentPoseValid)
            {
                CurrentPoseValid = true;
                return;
            }

            if (!syncing) ParameterChanged?.Invoke(this, EventArgs.Empty);
        }

        private static int ToInt(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return 0;
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}

/*
ToolVm 改动说明：
1. ToolVm 不接收 View，不保存任何控件引用，符合 MVVM。
2. 所有 UI 可变状态都暴露为属性：DisplayImage、DisplayRevision、FitOnNextDisplay、CrossRow、CrossCol、曲线数组、状态文本。
3. Halcon/ViewROI 的鼠标和 ROI 事件通过 ICommand 传入 VM，不通过控件实例调用。
4. ROI 改变只更新参数并请求重绘，不直接 ExecuteObj。
5. RunFromHost 是唯一手动运行入口；外部后台执行完成只刷新显示和结果，不重复运行。
6. CloseFromHost 不保存，只关闭菜单等轻状态；控件资源由 HalconIntersectionView 自己释放。

ToolParams 改动说明：
1. 继续作为 ParamsFindIntersection 和 WPF 绑定之间的参数承接类。
2. XAML 不直接绑定框架参数字段，减少耦合。
3. 通过 UpdateSourceTrigger=PropertyChanged 实时更新 VM 属性，不再需要 CommitBindings 递归扫控件树。
*/
