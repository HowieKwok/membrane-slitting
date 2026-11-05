using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Frame_Core;
using FrameCommunity.Data;
using HalconDotNet;
using Microsoft.Win32;
using Plugin.CropPart.Services;

namespace Plugin.CropPart.ViewModels
{
    public sealed class CropPartViewModel : BindableBase, IPluginWpfView
    {
        private CropPartObj pluginObject;
        private ParamsCropPart pluginParams;
        private CropPartHalconViewAdapter mainView;
        private CropPartHalconViewAdapter cropView;
        private int currentImageIndex = -1;

        private string titleText = "裁拼图工具";
        private string statusText = "等待加载图像";
        private string imageSummaryText = "图像数量：0";
        private string mouseInfoText = "Size  R:-   C:-\nR:-  C:-  Value:-";
        private string imageInfoText = "图像数量：0\nSize  R:-   C:-\nR:-  C:-  Value:-";
        private string outputText = "裁图结果：-";
        private string simulationText = "仿真：未开始";

        private double partCenterID;
        private double partCenterRow;
        private double partCenterCol;
        private double partSizeRow;
        private double partSizeCol;
        private double cropShaftRow;
        private double cropShaftCol;
        /// <summary>
        /// 判断是否仿真
        /// </summary>
        private bool isSimLoad=false;

        public CropPartViewModel()
        {
            LoadImageCommand = new RelayCommand<object>(_ => LoadImages());
            StepSimulationCommand = new RelayCommand<object>(_ => StepSimulation());
            FitWindowCommand = new RelayCommand<object>(_ => FitWindows());
        }

        public ICommand LoadImageCommand { get; private set; }
        public ICommand StepSimulationCommand { get; private set; }
        public ICommand FitWindowCommand { get; private set; }

        public IPlugin PluginObject
        {
            get { return pluginObject; }
        }

        public PluginParamsBase PluginParams
        {
            get { return pluginParams; }
        }

        public string TitleText
        {
            get { return titleText; }
            private set { SetProperty(ref titleText, value); }
        }

        public string StatusText
        {
            get { return statusText; }
            private set { SetProperty(ref statusText, value); }
        }

        public string ImageInfoText
        {
            get { return imageInfoText; }
            private set { SetProperty(ref imageInfoText, value); }
        }

        public string OutputText
        {
            get { return outputText; }
            private set { SetProperty(ref outputText, value); }
        }

        public string SimulationText
        {
            get { return simulationText; }
            private set { SetProperty(ref simulationText, value); }
        }

        public double PartCenterID
        {
            get { return partCenterID; }
            set
            {
                if (SetProperty(ref partCenterID, value)) CommitToParams();
            }
        }

        public double PartCenterRow
        {
            get { return partCenterRow; }
            set
            {
                if (SetProperty(ref partCenterRow, value)) CommitToParams();
            }
        }

        public double PartCenterCol
        {
            get { return partCenterCol; }
            set
            {
                if (SetProperty(ref partCenterCol, value)) CommitToParams();
            }
        }

        public double PartSizeRow
        {
            get { return partSizeRow; }
            set
            {
                if (SetProperty(ref partSizeRow, value)) CommitToParams();
            }
        }

        public double PartSizeCol
        {
            get { return partSizeCol; }
            set
            {
                if (SetProperty(ref partSizeCol, value)) CommitToParams();
            }
        }

        public double CropShaftRow
        {
            get { return cropShaftRow; }
            set
            {
                if (SetProperty(ref cropShaftRow, value)) CommitToParams();
            }
        }

        public double CropShaftCol
        {
            get { return cropShaftCol; }
            set
            {
                if (SetProperty(ref cropShaftCol, value)) CommitToParams();
            }
        }

        public void AttachViews(CropPartHalconViewAdapter main, CropPartHalconViewAdapter crop)
        {
            if (mainView != null) mainView.MouseInfoChanged -= View_MouseInfoChanged;
            if (cropView != null) cropView.MouseInfoChanged -= View_MouseInfoChanged;

            mainView = main;
            cropView = crop;

            if (mainView != null) mainView.MouseInfoChanged += View_MouseInfoChanged;

            RefreshDisplay(true);
        }

        public void BindPlugin(IPlugin plugin)
        {
            pluginObject = plugin as CropPartObj ?? new CropPartObj();
            if (pluginObject.moduleInfo != null && !string.IsNullOrWhiteSpace(pluginObject.moduleInfo.ModuleName))
            {
                TitleText = "裁拼图工具";/*pluginObject.moduleInfo.ModuleName;*/
            }
            else if (pluginObject.moduleInfo != null && !string.IsNullOrWhiteSpace(pluginObject.moduleInfo.PluginName))
            {
                TitleText = "裁拼图工具";/* pluginObject.moduleInfo.PluginName;*/
            }
            else
            {
                TitleText = "裁拼图工具";
            }

            EnsureParams();
            PullFromParams();
            RefreshDisplay(true);
        }

        public void RefreshBindings()
        {
            PullFromParams();
            OnPropertyChanged(string.Empty);
        }

        public PluginViewActionResult RunFromHost()
        {
            return StepSimulation();
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
                return success
                    ? PluginViewActionResult.Ok("参数已保存", watch.ElapsedMilliseconds)
                    : PluginViewActionResult.Fail("参数保存失败", watch.ElapsedMilliseconds);
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
            SimRelease();
            if (mainView != null) mainView.MouseInfoChanged -= View_MouseInfoChanged;
            if (cropView != null) cropView.MouseInfoChanged -= View_MouseInfoChanged;
            cropView?.Dispose();
            cropView?.Dispose();
            mainView = null;
            cropView = null;
        }

        public void BeforeRun()
        {
            CommitToParams();
        }

        public void AfterRun(bool success, long elapsedMs)
        {
            RefreshResultText(success, elapsedMs);
            RefreshDisplay(false);
        }

        public void BeforeSave()
        {
            CommitToParams();
        }

        public void AfterSave(bool success)
        {
            StatusText = success ? "参数已保存" : "参数保存失败";
        }

        public void BeforeClose()
        {
        }

        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            if (e == null || pluginParams == null) return;
            if (!ReferenceEquals(e.Param, pluginParams)) return;
            AfterRun(e.Success, e.ElapsedMs);
        }

        public void LoadImages()
        {
            isSimLoad = false;
            EnsureParams();
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*"
            };

            if (dialog.ShowDialog() != true) return;

            ReleasePreviousCropResult();
            ReleaseLoadedImages();

            List<string> files = dialog.FileNames
                .OrderBy(Path.GetFileName, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(p => p, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            int loaded = 0;
            foreach (string file in files)
            {
                HObject image = null;
                try
                {
                    HOperatorSet.ReadImage(out image, file);
                    HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                    pluginParams.list_ImageInfo.Add(new ImageInfo
                    {
                        ho_image = image,
                        image_ID = loaded,
                        cameraName = Path.GetFileName(file),
                        imageWidth = width.D,
                        imageHeight = height.D
                    });
                    image = null;
                    loaded++;
                }
                finally
                {
                    if (image != null) image.Dispose();
                }
            }

            currentImageIndex = loaded > 0 ? 0 : -1;
            if (loaded > 0)
            {
                pluginParams.PartCenterID = pluginParams.list_ImageInfo[currentImageIndex].image_ID;
                partCenterID = pluginParams.PartCenterID;
                OnPropertyChanged(nameof(PartCenterID));
            }

            UpdateImageInfoText();
            StatusText = loaded > 0 ? "已加载 " + loaded.ToString(CultureInfo.InvariantCulture) + " 张图像" : "未加载有效图像";
            SimulationText = "仿真：等待单步执行";
            OutputText = "裁图结果：-";
            RefreshDisplay(true);
            isSimLoad = true;
        }

        /// <summary>
        /// 释放仿真数据
        /// </summary>
        public void SimRelease()
        {
            try
            {
                if (isSimLoad)
                {
                    ReleasePreviousCropResult();
                    ReleaseLoadedImages();
                    isSimLoad = false;
                    string name = pluginObject.moduleInfo.ModuleName;
                }
            }
            catch (Exception ee)
            {

                LogTool.Log4netRecord.ErrorLog.Debug(pluginObject.moduleInfo.ModuleName+":仿真未正常释放__\n"+ee.ToString());
            }
        }
        public PluginViewActionResult StepSimulation()
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                EnsureParams();
                if (pluginParams.list_ImageInfo == null || pluginParams.list_ImageInfo.Count == 0)
                {
                    StatusText = "请先加载图像";
                    return PluginViewActionResult.Fail("请先加载图像");
                }

                BeforeRun();
                NormalizeCurrentIndex();
                ReleasePreviousCropResult();
                pluginParams.PartCenterID = pluginParams.list_ImageInfo[currentImageIndex].image_ID;
                partCenterID = pluginParams.PartCenterID;
                OnPropertyChanged(nameof(PartCenterID));

                bool success = pluginObject != null && pluginObject.ExecuteObj();
                watch.Stop();
                AfterRun(success, watch.ElapsedMilliseconds);

                int displayIndex = currentImageIndex + 1;
                int total = pluginParams.list_ImageInfo.Count;
                SimulationText = string.Format(CultureInfo.InvariantCulture, "仿真：{0}/{1}，图像ID {2}", displayIndex, total, pluginParams.PartCenterID);

                if (/*success &&*/ total > 0)
                {
                    currentImageIndex = (currentImageIndex + 1) % total;
                }

                return success
                    ? PluginViewActionResult.Ok("单步仿真完成", watch.ElapsedMilliseconds)
                    : PluginViewActionResult.Fail("单步仿真失败", watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                StatusText = "仿真异常：" + ex.Message;
                return PluginViewActionResult.Fail("仿真异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        public void FitWindows()
        {
            RefreshDisplay(true);
        }

        private void EnsureParams()
        {
            if (pluginObject == null) pluginObject = new CropPartObj();
            pluginParams = pluginObject.PluginParams as ParamsCropPart;
            if (pluginParams == null)
            {
                pluginParams = new ParamsCropPart();
                pluginObject.PluginParams = pluginParams;
            }
            if (pluginParams.list_ImageInfo == null) pluginParams.list_ImageInfo = new List<ImageInfo>();
        }

        private void PullFromParams()
        {
            EnsureParams();
            partCenterID = pluginParams.PartCenterID;
            partCenterRow = pluginParams.PartCenterRow;
            partCenterCol = pluginParams.PartCenterCol;
            partSizeRow = pluginParams.PartSizeRow;
            partSizeCol = pluginParams.PartSizeCol;
            cropShaftRow = pluginParams.CropShaftRow;
            cropShaftCol = pluginParams.CropShaftCol;
            OnPropertyChanged(nameof(PartCenterID));
            OnPropertyChanged(nameof(PartCenterRow));
            OnPropertyChanged(nameof(PartCenterCol));
            OnPropertyChanged(nameof(PartSizeRow));
            OnPropertyChanged(nameof(PartSizeCol));
            OnPropertyChanged(nameof(CropShaftRow));
            OnPropertyChanged(nameof(CropShaftCol));
            UpdateImageInfoText();
        }

        private void CommitToParams()
        {
            if (pluginParams == null) return;
            pluginParams.PartCenterID = Convert.ToInt64(Math.Round(partCenterID, MidpointRounding.AwayFromZero));
            pluginParams.PartCenterRow = partCenterRow;
            pluginParams.PartCenterCol = partCenterCol;
            pluginParams.PartSizeRow = partSizeRow;
            pluginParams.PartSizeCol = partSizeCol;
            pluginParams.CropShaftRow = cropShaftRow;
            pluginParams.CropShaftCol = cropShaftCol;
        }

        private void RefreshResultText(bool success, long elapsedMs)
        {
            if (pluginParams == null)
            {
                OutputText = "裁图结果：-";
                return;
            }

            StatusText = success ? "裁图成功" : "裁图失败";
            OutputText = string.Format(
                CultureInfo.InvariantCulture,
                "耗时 {0} ms\n起点：ID={1}, Row={2}, Col={3}\n终点：ID={4}, Row={5}, Col={6}",
                elapsedMs,
                pluginParams.hv_ImagePartBeginID,
                pluginParams.hv_ImagePartBeginRow,
                pluginParams.hv_ImagePartBeginCol,
                pluginParams.hv_ImagePartEndID,
                pluginParams.hv_ImagePartEndRow,
                pluginParams.hv_ImagePartEndCol);
        }

        private void RefreshDisplay(bool fit)
        {
            if (pluginParams == null) return;

            ImageInfo current = GetCurrentImage();
            if (mainView != null) mainView.ShowImage(current == null ? null : current.ho_image, fit);
            if (cropView != null) cropView.ShowImage(pluginParams.Image_part, fit);
        }

        private ImageInfo GetCurrentImage()
        {
            if (pluginParams == null || pluginParams.list_ImageInfo == null || pluginParams.list_ImageInfo.Count == 0) return null;
            NormalizeCurrentIndex();
            return pluginParams.list_ImageInfo[currentImageIndex];
        }

        private void NormalizeCurrentIndex()
        {
            int count = pluginParams.list_ImageInfo.Count;
            if (count == 0)
            {
                currentImageIndex = -1;
                return;
            }

            if (currentImageIndex < 0) currentImageIndex = 0;
            if (currentImageIndex >= count) currentImageIndex = 0;
        }

        private void UpdateImageInfoText()
        {
            if (pluginParams == null || pluginParams.list_ImageInfo == null || pluginParams.list_ImageInfo.Count == 0)
            {
                imageSummaryText = "图像数量：0";
                ImageInfoText = BuildImageInfoText();
                return;
            }

            NormalizeCurrentIndex();
            ImageInfo current = pluginParams.list_ImageInfo[currentImageIndex];
            imageSummaryText = string.Format(
                CultureInfo.InvariantCulture,
                "图像数量：{0}\n当前：{1}/{0}\nID：{2}\n尺寸：R={3}, C={4}",
                pluginParams.list_ImageInfo.Count,
                currentImageIndex + 1,
                current.image_ID,
                current.imageHeight,
                current.imageWidth);
            ImageInfoText = BuildImageInfoText();
        }

        private void ReleaseLoadedImages()
        {
            if (pluginParams == null || pluginParams.list_ImageInfo == null) return;

            foreach (ImageInfo imageInfo in pluginParams.list_ImageInfo)
            {
                if (imageInfo == null) continue;
                try
                {
                    imageInfo.Dispose();
                }
                catch
                {
                }
            }

            pluginParams.list_ImageInfo.Clear();
            currentImageIndex = -1;
            UpdateImageInfoText();
            if (mainView != null) mainView.Clear();
        }

        private void ReleasePreviousCropResult()
        {
            if (pluginParams == null || pluginParams.Image_part == null) return;

            try
            {
                pluginParams.Image_part.Dispose();
            }
            catch
            {
            }

            pluginParams.Image_part = null;
            if (cropView != null) cropView.Clear();
        }

        private void View_MouseInfoChanged(object sender, HalconImageMouseInfoEventArgs e)
        {
            if (e == null) return;
            mouseInfoText = e.SizeText + "\n" + e.PositionText;
            ImageInfoText = BuildImageInfoText();
        }

        private string BuildImageInfoText()
        {
            if (string.IsNullOrWhiteSpace(mouseInfoText)) return imageSummaryText;
            return imageSummaryText + "\n" + mouseInfoText;
        }
    }
}
