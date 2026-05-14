using Frame_Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;

namespace Plugin.CACRunCalibration.WPF架构
{
    public partial class WPFRunCalibration : UserControl
    {
        public IPlugin PluginObject { get; private set; }
        public PluginParamsBase PluginParams { get; private set; }

        public WPFRunCalibration()
        {
            InitializeComponent();
        }

        public void BindPlugin(IPlugin plugin)
        {
            PluginObject = plugin;
            PluginParams = plugin == null ? null : plugin.PluginParams;
            DataContext = PluginParams;
        }

        public void RefreshBindings()
        {
            DataContext = null;
            DataContext = PluginParams;
        }

        public virtual void Btn_RUN_Click(object sender, EventArgs e)
        {
            CommitBindings();
        }

        public virtual void Btn_Save_Click(object sender, EventArgs e)
        {
            CommitBindings();
        }

        public virtual void Btn_Close_Click(object sender, EventArgs e)
        {
            CommitBindings();
        }

        public virtual void AfterHostRun(object sender, EventArgs e)
        {
            RefreshBindings();
        }

        public virtual void AfterHostSave(object sender, EventArgs e)
        {
            RefreshBindings();
        }

        private void CommitBindings()
        {
            FocusManager.SetFocusedElement(this, this);
            UpdateBindingSources(this);
        }

        private void UpdateBindingSources(DependencyObject parent)
        {
            if (parent == null) return;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                TextBox textBox = child as TextBox;
                if (textBox != null)
                {
                    BindingExpression expression = textBox.GetBindingExpression(TextBox.TextProperty);
                    if (expression != null) expression.UpdateSource();
                }

                CheckBox checkBox = child as CheckBox;
                if (checkBox != null)
                {
                    BindingExpression expression = checkBox.GetBindingExpression(CheckBox.IsCheckedProperty);
                    if (expression != null) expression.UpdateSource();
                }

                ComboBox comboBox = child as ComboBox;
                if (comboBox != null)
                {
                    BindingExpression expression = comboBox.GetBindingExpression(ComboBox.SelectedItemProperty);
                    if (expression != null) expression.UpdateSource();
                    expression = comboBox.GetBindingExpression(ComboBox.SelectedValueProperty);
                    if (expression != null) expression.UpdateSource();
                    expression = comboBox.GetBindingExpression(ComboBox.TextProperty);
                    if (expression != null) expression.UpdateSource();
                }

                Slider slider = child as Slider;
                if (slider != null)
                {
                    BindingExpression expression = slider.GetBindingExpression(Slider.ValueProperty);
                    if (expression != null) expression.UpdateSource();
                }

                UpdateBindingSources(child);
            }
        }
    }
}


