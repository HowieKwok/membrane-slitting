using FrameCommunity;
using FrameCommunity.语言;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace DevMgmt.Light.Models
{
    [Serializable]
    public class LightControllerParameterBase : BindableBase, IWpfParameterToView
    {
        private int _ID=0;
        public int ID
        {
            get => _ID;
            set => SetProperty(ref _ID, value);
        }

        private string _lightControllerName="";
        public string LightControllerName
        {
            get => _lightControllerName;
            set => SetProperty(ref _lightControllerName, value);
        }

        private int _model = 0;
        public int Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private int _portName = 0;
        public int PortName
        {
            get => _portName;
            set => SetProperty(ref _portName, value);
        }

        private int _baudRate = 19200;
        public int BaudRate
        {
            get => _baudRate;
            set => SetProperty(ref _baudRate, value);
        }

        private int _dataBits = 8;
        public int DataBits
        {
            get => _dataBits;
            set => SetProperty(ref _dataBits, value);
        }

        private int _parity = 0;
        public int Parity
        {
            get => _parity;
            set => SetProperty(ref _parity, value);
        }

        private int _stopBits = 1;
        public int StopBits
        {
            get => _stopBits;
            set => SetProperty(ref _stopBits, value);
        }


        private List<LightChannelItem> _LightChannelItems;
        public List<LightChannelItem> LightChannelItems
        {
            get => _LightChannelItems;
            set => SetProperty(ref _LightChannelItems, value);
        }

        public LightControllerParameterBase()
        {
            InitDisplayConfig();         
        }
        
        [XmlIgnore]
        [JsonIgnore]
        private Dictionary<string, ControlDisplayBase> _ControlDisplay = new Dictionary<string, ControlDisplayBase>();
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<string, ControlDisplayBase> ControlDisplay
        {
            get => _ControlDisplay;
            set => SetProperty(ref _ControlDisplay, value);
        }

        public void InitDisplayConfig()
        {
            ControlDisplay.Clear();

            // ID
            ControlDisplay.Add(nameof(ID), new ControlDisplayBase
            {
                DisplayName = "ID"
            });

            // 光源控制器名称
            ControlDisplay.Add(nameof(LightControllerName), new ControlDisplayBase
            {
                DisplayName = "光源控制器名称"
            });


            // 光源控制器型号
            ControlDisplay.Add(nameof(Model), new ControlDisplayBase
            {
                ParentName = "基本参数",
                GroupSortOrder = 1,
                DisplayName = "光源控制器型号",
                ShowType = ControlShowTypeEnum.ComboBox,
                ComboBoxItems = EnumHelper.GetEnumComboBoxItems(typeof(LightControllerModelEnum)),

            });

            // 串口号
            ControlDisplay.Add(nameof(PortName), new ControlDisplayBase
            {
                ParentName = "通信参数",
                GroupSortOrder = 2,
                DisplayName = "串口号",
                DecimalPlaces = 0,
                Increment = 1,
                MinValue = 0

            });

            // 波特率
            ControlDisplay.Add(nameof(BaudRate), new ControlDisplayBase
            {
                ParentName = "通信参数",
                GroupSortOrder = 2,
                DisplayName = "波特率",
                ShowType = ControlShowTypeEnum.ComboBox,
                ComboBoxItems = EnumHelper.GetEnumComboBoxItems(typeof(BaudRateEunm)),

            });

            // 数据位
            ControlDisplay.Add(nameof(DataBits), new ControlDisplayBase
            {
                ParentName = "通信参数",
                GroupSortOrder = 2,
                IsEnabled = false,
                DisplayName = "数据位",
                DecimalPlaces = 0,
                Increment = 1,
                MinValue = 0
            });

            // 校验位
            ControlDisplay.Add(nameof(Parity), new ControlDisplayBase
            {
                ParentName = "通信参数",
                GroupSortOrder = 2,
                IsEnabled = false,
                DisplayName = "校验位",
                ShowType = ControlShowTypeEnum.ComboBox,
                ComboBoxItems = EnumHelper.GetEnumComboBoxItems(typeof(ParityEunm)),

            });

            // 停止位
            ControlDisplay.Add(nameof(StopBits), new ControlDisplayBase
            {
                ParentName = "通信参数",
                GroupSortOrder = 2,
                IsEnabled = false,
                DisplayName = "停止位"
            });

            // 使用循环简化通道配置
            if(LightChannelItems==null) LightChannelItems=new List<LightChannelItem>();        
            ControlDisplay.Add(nameof(LightChannelItems), new ControlDisplayBase
            {
                ParentName = "光源参数",
                GroupSortOrder = 3,
                DisplayName = $"通道亮度",
                DecimalPlaces = 0,
                Increment = 1,
                MinValue = 0,
                MaxValue = 255,
                ShowType = ControlShowTypeEnum.NumericBoxAndBotton,
            });          
        }

        public void InitControlDisplay()
        {
            try
            {
                InitIsEnabled();
                // 1. 全局重置：所有字段先统一隐藏，避免旧状态残留
                foreach (var cfg in ControlDisplay.Values)
                {
                    cfg.IsShow = false;
                }

                LightControllerModelEnum modelType = (LightControllerModelEnum)Model;

                // 2. 通用必显示字段（所有类型都展示）
                ControlDisplay.GetControlDisplayBase(nameof(Model)).SetShow(true);
                ControlDisplay.GetControlDisplayBase(nameof(PortName)).SetShow(true);
                ControlDisplay.GetControlDisplayBase(nameof(BaudRate)).SetShow(true);
                ControlDisplay.GetControlDisplayBase(nameof(DataBits)).SetShow(true);
                ControlDisplay.GetControlDisplayBase(nameof(Parity)).SetShow(true);
                ControlDisplay.GetControlDisplayBase(nameof(StopBits)).SetShow(true);
                ControlDisplay.GetControlDisplayBase(nameof(LightChannelItems)).SetShow(true);

                // 3.专属字段
                var areaScanList = new List<LightControllerModelEnum>
        {
            LightControllerModelEnum.OPTv1,
            LightControllerModelEnum.OPTv2,
            LightControllerModelEnum.OPTv3,
            LightControllerModelEnum.OPTControllerAPI,
            LightControllerModelEnum.乐视1,
            LightControllerModelEnum.乐视2,
            LightControllerModelEnum.乐视3,
            LightControllerModelEnum.康视达,
            LightControllerModelEnum.沃德普PSC3线光,
            LightControllerModelEnum.沃德普PD5,
            LightControllerModelEnum.嘉励,
        };


                if (areaScanList.Contains(modelType))
                {
                                   
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"配置参数面板异常：{ex.Message}");
            }
        }

        public void InitIsEnabled()
        {

        }

    }
}
