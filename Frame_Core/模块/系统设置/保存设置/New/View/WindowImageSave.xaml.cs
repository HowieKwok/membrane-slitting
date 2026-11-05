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

namespace Frame_Core
{
    /// <summary>
    /// ImageSave.xaml 的交互逻辑
    /// </summary>
    public partial class WindowImageSave : Window
    {
        VM_ImageSave VM = new VM_ImageSave();
        public WindowImageSave(NewImageSaveParamer newImageSaveParamer)
        {
            InitializeComponent();
            this.DataContext = VM;
            VM.Init(newImageSaveParamer);
            Init_ItemsControl();
        }
        public void Init_ItemsControl()
        {

            LeftItemsControl.ItemsSource = Service_Camera.GetCam_Names().ToList();

            if (LeftItemsControl.Items.Count > 0)
            {
                LeftItemsControl.SelectedIndex = 0;
            }
        }

        private void LeftItemsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = LeftItemsControl.SelectedIndex;

            if (index < 0)
            {
                return;
            }

            if (VM.Paramer.ImageSaveConfigItems == null)
            {
                return;
            }

            if (index >= VM.Paramer.ImageSaveConfigItems.Count)
            {
                return;
            }

            Update_View();

            ImageSaveParamConfig config = VM.Paramer.ImageSaveConfigItems[index];

            RightConfigGrid.DataContext = config;
        }

        private void Update_View()
        {
            int index = LeftItemsControl.SelectedIndex;

            if (index < 0)
            {
                return;
            }

            SourceImageRow.Visibility = Visibility.Visible;
            ScreenshotRow.Visibility = Visibility.Visible;
            ProductImageRow.Visibility = Visibility.Visible;
            ProductScreenshotRow.Visibility = Visibility.Visible;
            BigImageRow.Visibility = Visibility.Visible;
            BigScreenshotRow.Visibility = Visibility.Visible;
            NGSmallImageRow.Visibility = Visibility.Visible;
            Image3DRow.Visibility = Visibility.Visible;
            if (index == 0)
            {
                ScreenshotRow.Visibility = Visibility.Collapsed;
                BigScreenshotRow.Visibility = Visibility.Collapsed;
                Image3DRow.Visibility = Visibility.Collapsed;
            }
            else if (index == 1)
            {
                ScreenshotRow.Visibility = Visibility.Collapsed;
                BigScreenshotRow.Visibility = Visibility.Collapsed;
                Image3DRow.Visibility = Visibility.Collapsed;
            }
            else if (index == 2)
            {
                ProductImageRow.Visibility = Visibility.Collapsed;
                ProductScreenshotRow.Visibility = Visibility.Collapsed;
                BigImageRow.Visibility = Visibility.Collapsed;
                BigScreenshotRow.Visibility = Visibility.Collapsed;
                NGSmallImageRow.Visibility = Visibility.Collapsed;
                Image3DRow.Visibility = Visibility.Collapsed;
            }
            else if (index == 3)
            {
                ProductImageRow.Visibility = Visibility.Collapsed;
                ProductScreenshotRow.Visibility = Visibility.Collapsed;
                BigImageRow.Visibility = Visibility.Collapsed;
                BigScreenshotRow.Visibility = Visibility.Collapsed;          
                Image3DRow.Visibility = Visibility.Collapsed;
            }
            else if (index == 4)
            {
                ProductImageRow.Visibility = Visibility.Collapsed;
                ProductScreenshotRow.Visibility = Visibility.Collapsed;
                BigImageRow.Visibility = Visibility.Collapsed;
                BigScreenshotRow.Visibility = Visibility.Collapsed;
                Image3DRow.Visibility = Visibility.Collapsed;
            }
            else if (index == 5)
            {
                ProductImageRow.Visibility = Visibility.Collapsed;
                ProductScreenshotRow.Visibility = Visibility.Collapsed;
                BigImageRow.Visibility = Visibility.Collapsed;
                BigScreenshotRow.Visibility = Visibility.Collapsed;
                Image3DRow.Visibility = Visibility.Collapsed;
            }
           
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            BindingOperations.ClearAllBindings(this);  // 清除所有子元素的绑定
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
