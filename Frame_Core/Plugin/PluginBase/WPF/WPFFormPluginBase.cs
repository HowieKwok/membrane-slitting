using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Frame_Core
{
    /// <summary>
    /// WPF plugin base window. It keeps the same plugin object architecture as FormPluginBase:
    /// pluginsObjBase, Init, Run, Save and Close remain the extension points for plugin windows.
    /// </summary>
    [ContentProperty("PluginContent")]
    public class WPFFormPluginBase : Window, INotifyPropertyChanged
    {
        private readonly ContentPresenter pluginContentPresenter;
        private string costTimeText = "耗时：000ms";
        private string statusText = "等待操作";
        private object pluginContent;

        /// <summary>
        /// 窗体语言翻译。
        /// </summary>
        public LanguageEnum FormLanguage;

        /// <summary>
        /// UI 对应的 module obj，保持和 FormPluginBase 一致。
        /// </summary>
        public IPlugin pluginsObjBase { get; set; }

        /// <summary>
        /// 备份参数，保存时同步当前参数引用，保持和 FormPluginBase 一致。
        /// </summary>
        public PluginParamsBase pluginsObjParams_Back { get; set; }

        public ICommand RunCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public object PluginContent
        {
            get { return pluginContent; }
            set
            {
                if (pluginContent == value) return;
                pluginContent = value;
                if (pluginContentPresenter != null) pluginContentPresenter.Content = value;
                OnPropertyChanged();
            }
        }

        public string CostTimeText
        {
            get { return costTimeText; }
            set
            {
                if (costTimeText == value) return;
                costTimeText = value;
                OnPropertyChanged();
            }
        }

        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (statusText == value) return;
                statusText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WPFFormPluginBase()
        {
            RunCommand = new RelayCommand(delegate { Btn_RUN_Click(this, EventArgs.Empty); }, delegate { return pluginsObjBase != null; });
            SaveCommand = new RelayCommand(delegate { Btn_Save_Click(this, EventArgs.Empty); }, delegate { return pluginsObjBase != null; });
            CloseCommand = new RelayCommand(delegate { Btn_Close_Click(this, EventArgs.Empty); });

            DataContext = this;
            Title = "PluginFormBase";
            Width = 1008;
            Height = 811;
            MinWidth = 860;
            MinHeight = 620;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = BrushFrom("#070D14");
            FontFamily = new FontFamily("Microsoft YaHei UI");

            pluginContentPresenter = new ContentPresenter();
            Content = BuildChrome();
            Loaded += WPFFormPluginBase_Loaded;
        }

        public virtual void Init()
        {
            if (pluginsObjBase == null || pluginsObjBase.moduleInfo == null) return;

            Title = pluginsObjBase.moduleInfo.ModuleName;
            StatusText = "模块：" + pluginsObjBase.moduleInfo.ModuleName;
            CommandManager.InvalidateRequerySuggested();
        }

        public virtual void Btn_RUN_Click(object sender, EventArgs e)
        {
            if (pluginsObjBase == null) return;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            pluginsObjBase.ExecuteObj();
            watch.Stop();

            long mSeconds = watch.ElapsedMilliseconds;
            CostTimeText = "耗时：" + mSeconds.ToString("0.0") + "ms";
            StatusText = "执行完成";
        }

        public virtual void Btn_Save_Click(object sender, EventArgs e)
        {
            if (pluginsObjBase == null || pluginsObjBase.moduleInfo == null) return;

            pluginsObjParams_Back = pluginsObjBase.PluginParams;
            string processPath = System.IO.Path.Combine(Project.Instance.paramPathSet.ProcessPath, pluginsObjBase.moduleInfo.ProcessName);
            pluginsObjBase.moduleInfo.filePath = System.IO.Path.Combine(processPath, pluginsObjBase.moduleInfo.ModuleName);
            pluginsObjBase.saveParams();
            StatusText = "参数已保存";
        }

        public virtual void Btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected virtual void WPFFormPluginBase_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private UIElement BuildChrome()
        {
            Grid root = new Grid();
            root.Background = BuildGridBrush();

            Border shell = new Border
            {
                Margin = new Thickness(10),
                BorderBrush = BrushFrom("#2F7E98"),
                BorderThickness = new Thickness(1),
                Background = BrushFrom("#DD0A111C")
            };
            root.Children.Add(shell);

            Grid layout = new Grid();
            layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            layout.RowDefinitions.Add(new RowDefinition { Height = new GridLength(44) });
            shell.Child = layout;

            Border contentFrame = new Border
            {
                Margin = new Thickness(10, 10, 10, 6),
                BorderBrush = BrushFrom("#215D73"),
                BorderThickness = new Thickness(1),
                Background = BrushFrom("#08111B")
            };
            Grid.SetRow(contentFrame, 0);
            contentFrame.Child = pluginContentPresenter;
            layout.Children.Add(contentFrame);

            Border footer = new Border
            {
                BorderBrush = BrushFrom("#255F72"),
                BorderThickness = new Thickness(0, 1, 0, 0),
                Background = BrushFrom("#101923"),
                Padding = new Thickness(10, 0, 10, 0)
            };
            Grid.SetRow(footer, 1);
            layout.Children.Add(footer);

            Grid footerGrid = new Grid();
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            footer.Child = footerGrid;

            Border costBox = new Border
            {
                Height = 30,
                VerticalAlignment = VerticalAlignment.Center,
                BorderBrush = BrushFrom("#2F7E98"),
                BorderThickness = new Thickness(1),
                Background = BrushFrom("#0B1722"),
                Padding = new Thickness(10, 0, 10, 0)
            };
            TextBlock costText = new TextBlock { VerticalAlignment = VerticalAlignment.Center, Foreground = BrushFrom("#7EE6FF") };
            costText.SetBinding(TextBlock.TextProperty, "CostTimeText");
            costBox.Child = costText;
            footerGrid.Children.Add(costBox);

            TextBlock status = new TextBlock
            {
                Margin = new Thickness(12, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = BrushFrom("#9FC7D8")
            };
            status.SetBinding(TextBlock.TextProperty, "StatusText");
            Grid.SetColumn(status, 1);
            footerGrid.Children.Add(status);

            StackPanel buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(buttons, 2);
            footerGrid.Children.Add(buttons);

            buttons.Children.Add(CreateButton("执行", RunCommand));
            buttons.Children.Add(CreateButton("保存", SaveCommand));
            buttons.Children.Add(CreateButton("关闭", CloseCommand));

            return root;
        }

        private static Button CreateButton(string text, ICommand command)
        {
            Button button = new Button
            {
                Content = text,
                Command = command,
                Height = 32,
                MinWidth = 92,
                Margin = new Thickness(6, 0, 0, 0),
                Padding = new Thickness(14, 0, 14, 0),
                Foreground = BrushFrom("#E9FBFF"),
                Background = BrushFrom("#102536"),
                BorderBrush = BrushFrom("#00A9C8"),
                BorderThickness = new Thickness(1),
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                Cursor = Cursors.Hand
            };
            return button;
        }

        private static Brush BuildGridBrush()
        {
            DrawingGroup group = new DrawingGroup();
            GeometryDrawing bg = new GeometryDrawing(BrushFrom("#070D14"), null, new RectangleGeometry(new Rect(0, 0, 42, 42)));
            GeometryDrawing vLine = new GeometryDrawing(BrushFrom("#143847"), null, new RectangleGeometry(new Rect(0, 0, 1, 42)));
            GeometryDrawing hLine = new GeometryDrawing(BrushFrom("#102E3B"), null, new RectangleGeometry(new Rect(0, 0, 42, 1)));
            group.Children.Add(bg);
            group.Children.Add(vLine);
            group.Children.Add(hLine);

            DrawingBrush brush = new DrawingBrush(group)
            {
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, 42, 42),
                ViewportUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(0, 0, 42, 42),
                ViewboxUnits = BrushMappingMode.Absolute
            };
            brush.Freeze();
            return brush;
        }

        private static SolidColorBrush BrushFrom(string color)
        {
            SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            brush.Freeze();
            return brush;
        }

        private class RelayCommand : ICommand
        {
            private readonly Action<object> execute;
            private readonly Func<bool> canExecute;

            public RelayCommand(Action<object> execute, Func<bool> canExecute = null)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return canExecute == null || canExecute();
            }

            public void Execute(object parameter)
            {
                execute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
    }
}
