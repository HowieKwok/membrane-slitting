using DevMgmt.Cameras.Models;
using DevMgmt.Cameras.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevMgmt.Cameras.View
{
    /// <summary>
    /// AddCameraWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddCameraWindow : Window
    {
        public AddCameraViewModel ViewModel { get; set; }

        public AddCameraWindow(CameraManagerViewModel _CameraManager)
        {
            InitializeComponent();
            ViewModel = new AddCameraViewModel(_CameraManager);
            DataContext = ViewModel;

            // 关闭窗口
            ViewModel.RequestClose -= Close;
            ViewModel.RequestClose += Close;          
        }
 
    }
}
