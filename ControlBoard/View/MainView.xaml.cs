using ControlBoard.Interface;
using ControlBoard.Model;
using ControlBoard.View.IO640;
using ControlBoard.ViewModels;
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

namespace ControlBoard.View
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        MainViewModel DataViewMode { get; set; }
        public MainView(IControlCard controlCard,ParamControlCord  paramControlCord)
        {
            InitializeComponent();
            DataViewMode = new MainViewModel();
            this.DataContext = DataViewMode;
            if (paramControlCord.ControlType==EnumControlType.IO0640)
            {
                IO640UserView UserView = new IO640UserView(controlCard, paramControlCord);
                this.BoardContentControl.Content = UserView;

                // 直接在构造函数注册窗体关闭事件
                this.Closing += (s, args) =>
                {
                    UserView?.ReleaseAll();
                    UserView = null;
                };
            }
        }
    }
}
