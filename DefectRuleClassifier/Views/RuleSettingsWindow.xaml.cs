using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using DefectRuleClassifier.ViewModels;

namespace DefectRuleClassifier.Views
{
    public partial class RuleSettingsWindow : Window
    {
        public RuleSettingsWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => EndAllDataGridEdits();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            EndAllDataGridEdits();
        }

        private void OpenClassifierToolButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as RuleSettingsViewModel;
            var ngTypes = vm?.FeatureRuleConfig?.NgTypes;
            var featureLabels = ngTypes == null
                ? null
                : ngTypes.Where(r => r != null && !string.IsNullOrWhiteSpace(r.Name)).Select(r => r.Name.Trim());
            var window = new ClassifierToolWindow(vm?.ModelRuleConfig, featureLabels, vm?.RuleConfigPath)
            {
                Owner = this,
                OnBindingSaved = () => (DataContext as RuleSettingsViewModel)?.RaiseSaved()
            };
            window.Show();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Keyboard.ClearFocus();
            EndAllDataGridEdits();
            base.OnClosing(e);
        }

        private void EndAllDataGridEdits()
        {
            foreach (DataGrid grid in FindVisualChildren<DataGrid>(this))
            {
                try
                {
                    grid.CommitEdit(DataGridEditingUnit.Cell, true);
                    grid.CommitEdit(DataGridEditingUnit.Row, true);
                }
                catch
                {
                    try
                    {
                        grid.CancelEdit(DataGridEditingUnit.Cell);
                        grid.CancelEdit(DataGridEditingUnit.Row);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject root) where T : DependencyObject
        {
            if (root == null) yield break;

            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root, i);
                T typed = child as T;
                if (typed != null)
                    yield return typed;

                foreach (T nested in FindVisualChildren<T>(child))
                    yield return nested;
            }
        }
    }
}
