using HalconDotNet;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using ViewROI;

namespace TemplateMatchBasePointStep1Demo
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string TextImageSizeUnloaded = "图像大小: 未加载";
        private const string TextGrayEmpty = "灰度: -";
        private const string TextStatusUnloaded = "未加载图像";

        private HWindowControl _halconWindow;
        private HWndCtrl _viewCtrl;
        private HObject _image;
        private HImage _displayImage;
        private HObject _grayImage;
        private int _imageWidth;
        private int _imageHeight;

        private string _imageSizeText = TextImageSizeUnloaded;
        private string _mouseRowText = "Row: -";
        private string _mouseColText = "Col: -";
        private string _grayText = TextGrayEmpty;
        private string _statusText = TextStatusUnloaded;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ImageSizeText
        {
            get { return _imageSizeText; }
            set { SetProperty(ref _imageSizeText, value); }
        }

        public string MouseRowText
        {
            get { return _mouseRowText; }
            set { SetProperty(ref _mouseRowText, value); }
        }

        public string MouseColText
        {
            get { return _mouseColText; }
            set { SetProperty(ref _mouseColText, value); }
        }

        public string GrayText
        {
            get { return _grayText; }
            set { SetProperty(ref _grayText, value); }
        }

        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += OnLoaded;
            Closing += OnClosing;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _halconWindow = new HWindowControl();
            _halconWindow.Dock = DockStyle.Fill;
            HalconHost.Child = _halconWindow;

            _viewCtrl = new HWndCtrl(_halconWindow);

            // 当前步骤只做图像浏览，不启用 ROI 命中操作，避免鼠标拖拽图像时被 ROI 操作拦截。
            _viewCtrl.bl_RoiOperateFlag = false;
            _viewCtrl.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);
            _viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);

            _halconWindow.HMouseMove += OnHalconMouseMove;
            _halconWindow.DoubleClick += OnHalconDoubleClick;
        }

        private void OnLoadImageClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "选择图像";
            dialog.Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.hobj|所有文件|*.*";
            if (dialog.ShowDialog(this) != true)
            {
                return;
            }

            LoadImage(dialog.FileName);
        }

        private void OnFitImageClick(object sender, RoutedEventArgs e)
        {
            FitImageToWindow();
        }

        private void OnHalconDoubleClick(object sender, EventArgs e)
        {
            FitImageToWindow();
        }

        private void LoadImage(string imagePath)
        {
            try
            {
                DisposeImage();

                HOperatorSet.ReadImage(out _image, imagePath);
                HOperatorSet.GetImageSize(_image, out HTuple width, out HTuple height);
                _imageWidth = width.I;
                _imageHeight = height.I;

                _grayImage = BuildGrayImage(_image);

                _displayImage = new HImage();
                HobjectToHimage(_grayImage, ref _displayImage);
                _viewCtrl.addIconicVar(_displayImage);
                _viewCtrl.resetAll();
                _viewCtrl.repaint();

                ImageSizeText = string.Format("图像大小: {0} x {1}", _imageWidth, _imageHeight);
                StatusText = "已加载: " + Path.GetFileName(imagePath);
                MouseRowText = "Row: -";
                MouseColText = "Col: -";
                GrayText = TextGrayEmpty;
            }
            catch (Exception ex)
            {
                StatusText = "加载失败: " + ex.Message;
                System.Windows.MessageBox.Show(this, ex.Message, "加载图像失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private HObject BuildGrayImage(HObject sourceImage)
        {
            HOperatorSet.CountChannels(sourceImage, out HTuple channelCount);
            if (channelCount.I == 1)
            {
                HOperatorSet.CopyImage(sourceImage, out HObject gray);
                return gray;
            }

            HOperatorSet.Rgb1ToGray(sourceImage, out HObject grayImage);
            return grayImage;
        }

        private void HobjectToHimage(HObject hobject, ref HImage image)
        {
            // 与项目内旧插件保持一致：先从 HObject 取图像指针，再生成 HImage 后交给 ViewROI 显示。
            HOperatorSet.GetImagePointer1(hobject, out HTuple pointer, out HTuple type, out HTuple width, out HTuple height);
            image.GenImage1(type, width, height, pointer);
        }

        private void OnHalconMouseMove(object sender, HMouseEventArgs e)
        {
            if (_grayImage == null || !_grayImage.IsInitialized())
            {
                MouseRowText = "Row: -";
                MouseColText = "Col: -";
                GrayText = TextGrayEmpty;
                return;
            }

            double rowDouble = e.Y;
            double colDouble = e.X;
            int row = (int)Math.Round(rowDouble);
            int col = (int)Math.Round(colDouble);

            MouseRowText = string.Format("Row: {0:F2}", rowDouble);
            MouseColText = string.Format("Col: {0:F2}", colDouble);

            if (row < 0 || col < 0 || row >= _imageHeight || col >= _imageWidth)
            {
                GrayText = TextGrayEmpty;
                return;
            }

            try
            {
                HOperatorSet.GetGrayval(_grayImage, row, col, out HTuple grayValue);
                GrayText = "灰度: " + grayValue.ToString();
            }
            catch
            {
                GrayText = TextGrayEmpty;
            }
        }

        private void FitImageToWindow()
        {
            if (_image == null || !_image.IsInitialized())
            {
                StatusText = TextStatusUnloaded;
                return;
            }

            _viewCtrl.resetAll();
            _viewCtrl.repaint();
            StatusText = string.Format("已适应窗口, 图像大小: {0} x {1}", _imageWidth, _imageHeight);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            DisposeImage();
        }

        private void DisposeImage()
        {
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }

            if (_displayImage != null)
            {
                _displayImage.Dispose();
                _displayImage = null;
            }

            if (_grayImage != null)
            {
                _grayImage.Dispose();
                _grayImage = null;
            }
        }

        private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
