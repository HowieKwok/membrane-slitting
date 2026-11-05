using ControlBoard.Interface;
using ControlBoard.Model;
using ControlBoard.ViewModels.IO640;
using FrameCommunity.公共类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ControlBoard.View.IO640
{

    /// <summary>
    /// IO640UserView.xaml 的交互逻辑
    /// </summary>
    public partial class IO640UserView : UserControl
    {
        private ParamControlCord model { get; set; }
        private IControlCard _controlCard { get; set; }
        private IO640UserViewModel Vm { get; set; }   //ViewModel实例化


        public IO640UserView(IControlCard controlCard, ParamControlCord paramControlCord)
        {
            InitializeComponent();
            IO640UserViewModel UserViewModel = new IO640UserViewModel(controlCard, paramControlCord);
            model = paramControlCord;
            Vm = UserViewModel;
            DataContext =UserViewModel;
            Unloaded += OnUserControlUnloaded;
        }

        /// <summary>
        /// 板卡型号切换下拉框事件
        /// </summary>
        
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (DataContext is IO640UserViewModel vm && sender is ComboBox cbx)
            {
                if (cbx.SelectedItem is EnumControlType selectType)
                {
                    vm.BoardTypeChangedCommand?.Execute(selectType);
                }
            }
        }

        // 控件被卸载时释放线程资源
        private void OnUserControlUnloaded(object sender, RoutedEventArgs e)
        {
            Vm?.CleanResource();
            Vm?.Dispose();
            DataContext = null;
        }

        // 对外暴露释放方法，给Window关闭直接调用
        public void ReleaseAll()
        {
            Vm?.CleanResource();
            Vm?.Dispose();
            DataContext = null;
        }
    }
}
