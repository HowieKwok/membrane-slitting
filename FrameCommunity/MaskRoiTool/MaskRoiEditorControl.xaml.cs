using FrameCommunity.UserControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace FrameCommunity.MaskRoiTool
{
    /// <summary>
    /// 多屏蔽 ROI 公共编辑控件。
    /// 宿主只传入 MaskRoiOptions，并通过 SelectedMaskRoiParam 绑定当前 ROI 参数；
    /// 增删改移命令、列表 VM、参数面板切换都封装在控件内部。
    /// </summary>
    public sealed partial class MaskRoiEditorControl : UserControl
    {
        private readonly MaskRoiEditorViewModel viewModel = new MaskRoiEditorViewModel();
        private readonly ContentControl parameterHost = new ContentControl();
        private bool syncingSelection;

        public MaskRoiEditorControl()
        {
            viewModel.PropertyChanged += OnEditorPropertyChanged;
            viewModel.MaskRoisChanged += OnEditorMaskRoisChanged;
            BuildLayout();
        }

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register(
                nameof(Options),
                typeof(MaskRoiOptions),
                typeof(MaskRoiEditorControl),
                new PropertyMetadata(null, OnOptionsChanged));

        public static readonly DependencyProperty SelectedMaskRoiParamProperty =
            DependencyProperty.Register(
                nameof(SelectedMaskRoiParam),
                typeof(MaskRoiParam),
                typeof(MaskRoiEditorControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedMaskRoiParamChanged));

        public MaskRoiOptions Options
        {
            get { return (MaskRoiOptions)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public MaskRoiParam SelectedMaskRoiParam
        {
            get { return (MaskRoiParam)GetValue(SelectedMaskRoiParamProperty); }
            set { SetValue(SelectedMaskRoiParamProperty, value); }
        }

        public void Release()
        {
            viewModel.PropertyChanged -= OnEditorPropertyChanged;
            viewModel.MaskRoisChanged -= OnEditorMaskRoisChanged;
            viewModel.BindOptions(new MaskRoiOptions());
            Options = null;
            SelectedMaskRoiParam = null;
        }

        private static void OnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MaskRoiEditorControl)d;
            control.viewModel.BindOptions(e.NewValue as MaskRoiOptions);
            control.UpdateSelectedParamFromViewModel();
        }

        private static void OnSelectedMaskRoiParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MaskRoiEditorControl)d;
            if (control.syncingSelection) return;
            control.SelectParam(e.NewValue as MaskRoiParam);
        }

        private void OnEditorPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e == null || e.PropertyName == nameof(MaskRoiEditorViewModel.SelectedMaskRoi))
            {
                UpdateSelectedParamFromViewModel();
                RefreshParameterPanel();
            }
        }

        private void OnEditorMaskRoisChanged(object sender, EventArgs e)
        {
            UpdateSelectedParamFromViewModel();
            RefreshParameterPanel();
        }

        private void UpdateSelectedParamFromViewModel()
        {
            try
            {
                syncingSelection = true;
                SetCurrentValue(SelectedMaskRoiParamProperty, viewModel.SelectedMaskRoi == null ? null : viewModel.SelectedMaskRoi.Model);
            }
            finally
            {
                syncingSelection = false;
            }
        }

        private void SelectParam(MaskRoiParam param)
        {
            foreach (MaskRoiItemViewModel item in viewModel.MaskRois)
            {
                if (ReferenceEquals(item.Model, param))
                {
                    viewModel.SelectedMaskRoi = item;
                    return;
                }
            }

            viewModel.SelectedMaskRoi = null;
        }

        private void BuildLayout()
        {
            var root = new StackPanel();
            root.Children.Add(BuildToolbar());
            root.Children.Add(BuildGrid());
            root.Children.Add(parameterHost);
            Content = root;
            RefreshParameterPanel();
        }

        private UIElement BuildToolbar()
        {
            var toolbar = new WrapPanel { Margin = new Thickness(0, 0, 0, 8) };
            toolbar.Children.Add(CreateCheckBox("启用", nameof(MaskRoiEditorViewModel.EnableMaskRoi)));
            toolbar.Children.Add(CreateCheckBox("显示", nameof(MaskRoiEditorViewModel.ShowMaskRoi)));
            toolbar.Children.Add(CreateCheckBox("编辑", nameof(MaskRoiEditorViewModel.EditMaskRoi)));
            toolbar.Children.Add(CreateButton("新增", nameof(MaskRoiEditorViewModel.AddMaskRoiCommand)));
            toolbar.Children.Add(CreateButton("删除", nameof(MaskRoiEditorViewModel.DeleteMaskRoiCommand)));
            toolbar.Children.Add(CreateButton("上移", nameof(MaskRoiEditorViewModel.MoveUpCommand)));
            toolbar.Children.Add(CreateButton("下移", nameof(MaskRoiEditorViewModel.MoveDownCommand)));
            return toolbar;
        }

        private UIElement BuildGrid()
        {
            var grid = new DataGrid
            {
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                Height = 130,
                Margin = new Thickness(0, 0, 0, 8)
            };
            grid.SetBinding(ItemsControl.ItemsSourceProperty, CreateVmBinding(nameof(MaskRoiEditorViewModel.MaskRois), BindingMode.OneWay));
            grid.SetBinding(DataGrid.SelectedItemProperty, CreateVmBinding(nameof(MaskRoiEditorViewModel.SelectedMaskRoi), BindingMode.TwoWay));

            grid.Columns.Add(new DataGridCheckBoxColumn { Header = "启用", Binding = new Binding(nameof(MaskRoiItemViewModel.Enabled)) { Mode = BindingMode.TwoWay }, Width = 48 });
            grid.Columns.Add(new DataGridTextColumn { Header = "序号", Binding = new Binding(nameof(MaskRoiItemViewModel.Index)), Width = 48, IsReadOnly = true });
            grid.Columns.Add(new DataGridTextColumn { Header = "名称", Binding = new Binding(nameof(MaskRoiItemViewModel.Name)) { Mode = BindingMode.TwoWay }, Width = 90 });
            grid.Columns.Add(CreateShapeColumn());
            grid.Columns.Add(new DataGridTextColumn { Header = "摘要", Binding = new Binding(nameof(MaskRoiItemViewModel.Summary)), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            return grid;
        }

        private DataGridTemplateColumn CreateShapeColumn()
        {
            var factory = new FrameworkElementFactory(typeof(ComboBox));
            factory.SetBinding(ItemsControl.ItemsSourceProperty, CreateVmBinding(nameof(MaskRoiEditorViewModel.ShapeTypes), BindingMode.OneWay));
            factory.SetBinding(Selector.SelectedItemProperty, new Binding(nameof(MaskRoiItemViewModel.Shape)) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            factory.SetValue(FrameworkElement.HeightProperty, 24.0);
            return new DataGridTemplateColumn
            {
                Header = "形状",
                Width = 90,
                CellTemplate = new DataTemplate { VisualTree = factory }
            };
        }

        private void RefreshParameterPanel()
        {
            MaskRoiItemViewModel selected = viewModel.SelectedMaskRoi;
            if (selected == null)
            {
                parameterHost.Content = null;
                return;
            }

            switch (selected.Shape)
            {
                case MaskRoiShapeType.Rectangle2:
                    parameterHost.Content = BuildRectangle2Panel();
                    break;
                case MaskRoiShapeType.Circle:
                    parameterHost.Content = BuildCirclePanel();
                    break;
                default:
                    parameterHost.Content = BuildRectangle1Panel();
                    break;
            }
        }

        private UIElement BuildRectangle1Panel()
        {
            var panel = CreateParameterGrid();
            AddNumeric(panel, "Row1", "SelectedMaskRoi.Row1");
            AddNumeric(panel, "Col1", "SelectedMaskRoi.Col1");
            AddNumeric(panel, "Row2", "SelectedMaskRoi.Row2");
            AddNumeric(panel, "Col2", "SelectedMaskRoi.Col2");
            return panel;
        }

        private UIElement BuildRectangle2Panel()
        {
            var panel = CreateParameterGrid();
            AddNumeric(panel, "中心行", "SelectedMaskRoi.CenterRow");
            AddNumeric(panel, "中心列", "SelectedMaskRoi.CenterCol");
            AddNumeric(panel, "角度(rad)", "SelectedMaskRoi.Angle", 0.01, 4);
            AddNumeric(panel, "Length1", "SelectedMaskRoi.Length1");
            AddNumeric(panel, "Length2", "SelectedMaskRoi.Length2");
            return panel;
        }

        private UIElement BuildCirclePanel()
        {
            var panel = CreateParameterGrid();
            AddNumeric(panel, "中心行", "SelectedMaskRoi.CenterRow");
            AddNumeric(panel, "中心列", "SelectedMaskRoi.CenterCol");
            AddNumeric(panel, "半径", "SelectedMaskRoi.Radius");
            return panel;
        }

        private static UniformGrid CreateParameterGrid()
        {
            return new UniformGrid { Columns = 2 };
        }

        private void AddNumeric(Panel panel, string label, string path, double increment = 1, int decimals = 2)
        {
            panel.Children.Add(new Label { Content = label });
            var box = new NumericBox { Increment = increment, DecimalPlaces = decimals };
            box.SetBinding(NumericBox.ValueProperty, CreateVmBinding(path, BindingMode.TwoWay));
            panel.Children.Add(box);
        }

        private CheckBox CreateCheckBox(string text, string propertyName)
        {
            var checkBox = new CheckBox { Content = text };
            checkBox.SetBinding(ToggleButton.IsCheckedProperty, CreateVmBinding(propertyName, BindingMode.TwoWay));
            return checkBox;
        }

        private Button CreateButton(string text, string commandPropertyName)
        {
            var button = new Button { Content = text };
            button.SetBinding(Button.CommandProperty, CreateVmBinding(commandPropertyName, BindingMode.OneWay));
            return button;
        }

        private Binding CreateVmBinding(string path, BindingMode mode)
        {
            return new Binding(path)
            {
                Source = viewModel,
                Mode = mode,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
        }
    }
}
