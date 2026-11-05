using FrameCommunity;
using FrameCommunity.DataOperation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DevMgmt.Cameras.Models
{
    [Serializable]
    public class CameraParameterBase : BindableBase, IWpfParameterToView
    {

        private int _ID = 0;
        public int ID
        {
            get => _ID;
            set => SetProperty(ref _ID, value);
        }

        private string _CameraName = "";
        public string CameraName
        {
            get => _CameraName;
            set => SetProperty(ref _CameraName, value);
        }

        private int _DevType = 0;
        public int DevType
        {
            get => _DevType;
            set => SetProperty(ref _DevType, value);
        }

        private string _CameraSerial = "";
        public string CameraSerial
        {
            get => _CameraSerial;
            set => SetProperty(ref _CameraSerial, value);
        }

        private string _CardSerial = "";
        public string CardSerial
        {
            get => _CardSerial;
            set => SetProperty(ref _CardSerial, value);
        }

        private string _CameraIP = "127.0.0.1";
        public string CameraIP
        {
            get => _CameraIP;
            set => SetProperty(ref _CameraIP, value);
        }

        private string _PCIP = "127.0.0.1";
        public string PCIP
        {
            get => _PCIP;
            set => SetProperty(ref _PCIP, value);
        }

        private string _ConfigFiles = "";
        public string ConfigFiles
        {
            get => _ConfigFiles;
            set => SetProperty(ref _ConfigFiles, value);
        }

        private string _CardModel = "";
        public string CardModel
        {
            get => _CardModel;
            set => SetProperty(ref _CardModel, value);
        }

        private int _Port = 0;
        public int Port
        {
            get => _Port;
            set => SetProperty(ref _Port, value);
        }

        private int _ImageWidth = 4096;
        public int ImageWidth
        {
            get => _ImageWidth;
            set => SetProperty(ref _ImageWidth, value);
        }

        private int _ImageHeight = 5000;
        public int ImageHeight
        {
            get => _ImageHeight;
            set => SetProperty(ref _ImageHeight, value);
        }

        private string _emulationPath = "";
        public string EmulationPath
        {
            get => _emulationPath;
            set => SetProperty(ref _emulationPath, value);
        }

        private double _calibrationValue_X = 1;
        public double CalibrationValue_X
        {
            get => _calibrationValue_X;
            set => SetProperty(ref _calibrationValue_X, value);
        }

        private double _calibrationValue_Y = 1;
        public double CalibrationValue_Y
        {
            get => _calibrationValue_Y;
            set => SetProperty(ref _calibrationValue_Y, value);
        }
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<string, ControlDisplayBase> ControlDisplay { get; set; } = new Dictionary<string, ControlDisplayBase>();

        public CameraParameterBase()
        {
            InitDisplayConfig();
        }
        /// <summary> 初始化所有属性的界面配置 </summary>
        public void InitDisplayConfig()
        {
            ControlDisplay.Clear();
            // ID
            ControlDisplay.Add(nameof(ID), new ControlDisplayBase
            {
                DisplayName = "ID"
            });

            // 相机名称
            ControlDisplay.Add(nameof(CameraName), new ControlDisplayBase
            {
                DisplayName = "相机名称"
            });

            // 相机类型
            ControlDisplay.Add(nameof(DevType), new ControlDisplayBase
            {
                DisplayName = "相机类型",
                ShowType = ControlShowTypeEnum.TextBox,
                IsEnabled = false
            });

            // 相机序列号
            ControlDisplay.Add(nameof(CameraSerial), new ControlDisplayBase
            {
                DisplayName = "相机序列号"
            });

            // 采集卡序列号
            ControlDisplay.Add(nameof(CardSerial), new ControlDisplayBase
            {
                DisplayName = "采集卡序列号"
            });

            // 相机IP
            ControlDisplay.Add(nameof(CameraIP), new ControlDisplayBase
            {
                DisplayName = "相机IP"
            });

            // 上位机IP
            ControlDisplay.Add(nameof(PCIP), new ControlDisplayBase
            {
                DisplayName = "上位机IP"
            });

            // 配置文件
            ControlDisplay.Add(nameof(ConfigFiles), new ControlDisplayBase
            {
                DisplayName = "配置文件",
                ShowType = ControlShowTypeEnum.TextBoxAndSelectFile
            });

            // 采集卡型号
            ControlDisplay.Add(nameof(CardModel), new ControlDisplayBase
            {
                DisplayName = "采集卡型号"
            });

            // 端口
            ControlDisplay.Add(nameof(Port), new ControlDisplayBase
            {
                DisplayName = "端口",
                DecimalPlaces = 0,
                Increment = 1,
                MinValue = 0
            });

            // 图像宽度
            ControlDisplay.Add(nameof(ImageWidth), new ControlDisplayBase
            {
                DisplayName = "图像宽度",
                DecimalPlaces = 0,
                Increment = 1,
                MinValue = 0
            });

            // 图像高度
            ControlDisplay.Add(nameof(ImageHeight), new ControlDisplayBase
            {
                DisplayName = "图像高度",
                DecimalPlaces = 0,
                Increment = 1,
                MinValue = 0
            });

            // 仿真图像路径
            ControlDisplay.Add(nameof(EmulationPath), new ControlDisplayBase
            {
                DisplayName = "仿真图像路径",
                ShowType = ControlShowTypeEnum.TextBoxAndSelectFolder
            });

            // 横向校准值
            ControlDisplay.Add(nameof(CalibrationValue_X), new ControlDisplayBase
            {
                DisplayName = "横向校准值",
                DecimalPlaces = 8,
                Increment = 0.01,
                MinValue = 0
            });

            // 纵向校准值
            ControlDisplay.Add(nameof(CalibrationValue_Y), new ControlDisplayBase
            {
                DisplayName = "纵向校准值",
                DecimalPlaces = 8,
                Increment = 0.01,
                MinValue = 0
            });
        }


        public void InitControlDisplay()
        {
            InitIsEnabled();
        }

        public void InitIsEnabled()
        {

        }
    }
}
