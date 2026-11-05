using FrameCommunity.语言;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FrameCommunity
{
    public class ControlDisplayBase: BindableOrdinary
    {
        /// <summary>分组排序权重，数字越小越靠前</summary>
        public int GroupSortOrder { get; set; } = 999;

        private string _ParentName = "";
        /// <summary>
        /// 上一级节点名称
        /// </summary>
        public string ParentName
        {
            get => _ParentName;
            set => SetProperty(ref _ParentName, value);
        }


        private string _DisplayName = "";
        /// <summary> 界面标签文本 </summary>
        public string DisplayName
        {
            get => _DisplayName;
            set => SetProperty(ref _DisplayName, value);
        }

        /// <summary> 是否显示控件 </summary>
        public bool IsShow { get; set; } = false;
        /// <summary> 控件是否启用 </summary>
        public bool IsEnabled { get; set; } = true;

        //=======================================
        //NumericBox
        /// <summary> NumericBox 小数位数 </summary>
        public int DecimalPlaces { get; set; } = 0;
        /// <summary> NumericBox 步长 </summary>
        public double Increment { get; set; } = 1;
        /// <summary> NumericBox 最小值 </summary>
        public double MinValue { get; set; } = 0;
        /// <summary> NumericBox 最小值 </summary>
        public double MaxValue { get; set; } = 99999999;


        /// <summary>  控件显示类型
        public ControlShowTypeEnum ShowType { get; set; } = ControlShowTypeEnum.Normal;


        //=======================================
        //ComboBox       
        private ObservableCollection<ComboBoxEnumItem> _comboBoxItems = new ObservableCollection<ComboBoxEnumItem>();

        public ObservableCollection<ComboBoxEnumItem> ComboBoxItems
        {
            get => _comboBoxItems;
            set => SetProperty(ref _comboBoxItems, value);
        }

        //=======================================
        //ComboBox  
    }

    public class ComboBoxEnumItem : BindableOrdinary
    {
        public int Value { get; set; }          // 枚举实际 int 值
        public string Key { get; set; } = "";   // UiDisplay key
        private string _Text;
        public string Text
        {
            get => _Text;
            set => SetProperty(ref _Text, value);
        }
    }

    public enum ControlShowTypeEnum
    {
        Normal,
        TextBox,
        TextBoxAndSelectFile,//选择文件
        TextBoxAndSelectFolder,//选择文件夹
        NumericBox,
        NumericBoxAndBotton,
        ComboBox
    }

    /// <summary>
    /// 输入框类型
    /// </summary>
    public enum InputBoxType
    {
        /// <summary> 普通文本框（默认） </summary>
        Normal,
        /// <summary> 选择文件 </summary>
        SelectFile,
        /// <summary> 选择文件夹 </summary>
        SelectFolder
    }
}
