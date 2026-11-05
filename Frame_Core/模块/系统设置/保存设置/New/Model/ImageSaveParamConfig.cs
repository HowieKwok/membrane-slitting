using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Frame_Core
{
    public class ImageSaveParamConfig
    {
        private SourceImage_SaveParamer _SourceImageSaveParamer = new SourceImage_SaveParamer();
        public SourceImage_SaveParamer SourceImageSaveParamer
        {
            get => _SourceImageSaveParamer;
            set { _SourceImageSaveParamer = value; }
        }

        private Screenshot_SaveParamer _ScreenshotSaveParamer = new Screenshot_SaveParamer();
        public Screenshot_SaveParamer ScreenshotSaveParamer
        {
            get => _ScreenshotSaveParamer;
            set { _ScreenshotSaveParamer = value; }
        }

        private SourceImage_SaveParamer _EAImageSaveParamer = new SourceImage_SaveParamer();
        public SourceImage_SaveParamer EAImageSaveParamer
        {
            get => _EAImageSaveParamer;
            set { _EAImageSaveParamer = value; }
        }

        private Screenshot_SaveParamer _EAScreenshotSaveParamer = new Screenshot_SaveParamer();
        public Screenshot_SaveParamer EAScreenshotSaveParamer
        {
            get => _EAScreenshotSaveParamer;
            set { _EAScreenshotSaveParamer = value; }
        }

        private BigImage_SaveParamer _BigImageSaveParamer = new BigImage_SaveParamer();
        public BigImage_SaveParamer BigImageSaveParamer
        {
            get => _BigImageSaveParamer;
            set { _BigImageSaveParamer = value; }
        }

        private BigScreenshot_SaveParamer _BigScreenshotSaveParamer = new BigScreenshot_SaveParamer();
        public BigScreenshot_SaveParamer BigScreenshotSaveParamer
        {
            get => _BigScreenshotSaveParamer;
            set { _BigScreenshotSaveParamer = value; }
        }

        private NGImage_SaveParamer _NGImageSaveParamer = new NGImage_SaveParamer();
        public NGImage_SaveParamer NGImageSaveParamer
        {
            get => _NGImageSaveParamer;
            set { _NGImageSaveParamer = value; }
        }

        private Image3D_SaveParamer _Image3DSaveParamer = new Image3D_SaveParamer();
        public Image3D_SaveParamer Image3DSaveParamer
        {
            get => _Image3DSaveParamer;
            set { _Image3DSaveParamer = value; }
        }
    }

    public class SourceImage_SaveParamer
    {
        private ImageSaveFormat _imageFormat = ImageSaveFormat.JPEG20;
        private ImageSaveSelect _saveMode = ImageSaveSelect.不保存;
        private bool _isShow = true;

        public ImageSaveFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public ImageSaveSelect SaveMode
        {
            get { return _saveMode; }
            set { _saveMode = value; }
        }

        public bool IsShow
        {
            get { return _isShow; }
            set { _isShow = value; }
        }
    }

    public class Screenshot_SaveParamer 
    {
        private ImageSaveFormat _imageFormat = ImageSaveFormat.JPEG20;
        private ImageSaveSelect _saveMode = ImageSaveSelect.不保存;
        private bool _isHighDefinitionCapture = false;

        public ImageSaveFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public ImageSaveSelect SaveMode
        {
            get { return _saveMode; }
            set { _saveMode = value; }
        }

        public bool IsHighDefinitionCapture
        {
            get { return _isHighDefinitionCapture; }
            set { _isHighDefinitionCapture = value; }
        }
    }

    public class EAImage_SaveParamer
    {
        private ImageSaveFormat _imageFormat = ImageSaveFormat.JPEG20;
        private ImageSaveSelect _saveMode = ImageSaveSelect.不保存;
        private bool _isShow = true;

        public ImageSaveFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public ImageSaveSelect SaveMode
        {
            get { return _saveMode; }
            set { _saveMode = value; }
        }

        public bool IsShow
        {
            get { return _isShow; }
            set { _isShow = value; }
        }
    }

    public class EAScreenshot_SaveParamer
    {
        private ImageSaveFormat _imageFormat = ImageSaveFormat.JPEG20;
        private ImageSaveSelect _saveMode = ImageSaveSelect.不保存;
        private bool _isHighDefinitionCapture = false;

        public ImageSaveFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public ImageSaveSelect SaveMode
        {
            get { return _saveMode; }
            set { _saveMode = value; }
        }

        public bool IsHighDefinitionCapture
        {
            get { return _isHighDefinitionCapture; }
            set { _isHighDefinitionCapture = value; }
        }
    }

    public class BigImage_SaveParamer
    {
        private ImageSaveFormat _imageFormat = ImageSaveFormat.JPEG20;
        private ImageSaveSelect _saveMode = ImageSaveSelect.不保存;
        private bool _isShow = true;

        public ImageSaveFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public ImageSaveSelect SaveMode
        {
            get { return _saveMode; }
            set { _saveMode = value; }
        }

        public bool IsShow
        {
            get { return _isShow; }
            set { _isShow = value; }
        }
    }

    public class BigScreenshot_SaveParamer
    {
        private ImageSaveFormat _imageFormat = ImageSaveFormat.JPEG20;
        private ImageSaveSelect _saveMode = ImageSaveSelect.不保存;
        private bool _isHighDefinitionCapture = false;

        public ImageSaveFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public ImageSaveSelect SaveMode
        {
            get { return _saveMode; }
            set { _saveMode = value; }
        }

        public bool IsHighDefinitionCapture
        {
            get { return _isHighDefinitionCapture; }
            set { _isHighDefinitionCapture = value; }
        }
    }

    public class NGImage_SaveParamer
    {
        private ImageSaveFormat _imageFormat = ImageSaveFormat.BMP;
        private bool _isSave=false;
        private bool _isShow = false;

        public ImageSaveFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public bool IsSave
        {
            get { return _isSave; }
            set { _isSave = value; }
        }

        public bool IsShow
        {
            get { return _isShow; }
            set { _isShow = value; }
        }
    }

    public class Image3D_SaveParamer
    {
        private ImageDeepthSaveFormat _imageFormat = ImageDeepthSaveFormat.TIF;
        private bool _isCompressed = false;

        public ImageDeepthSaveFormat ImageFormat
        {
            get { return _imageFormat; }
            set { _imageFormat = value; }
        }

        public bool IsCompressed
        {
            get { return _isCompressed; }
            set { _isCompressed = value; }
        }
    }
}
