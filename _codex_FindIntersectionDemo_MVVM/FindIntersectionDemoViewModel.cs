using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Frame_Core;

namespace Plugin.FindIntersection
{
    public class FindIntersectionDemoViewModel : INotifyPropertyChanged
    {
        private DemoPluginInfo selectedPlugin;
        private double line1CenterRow = 200;
        private double line1CenterColumn = 300;
        private double line1Angle = 0;
        private double line1Height = 200;
        private double line1Width = 100;
        private int line1PointCount = 20;
        private double line1Sigma = 3.0;
        private int line1Threshold = 20;
        private int line1NumInstances = 1;
        private double line1DistThreshold = 3.5;
        private double line1MinScore = 0.5;
        private int line1MaxIterations = -1;
        private string line1Transition = "all";
        private string line1Select = "first";

        private double line2CenterRow = 300;
        private double line2CenterColumn = 400;
        private double line2Angle = -90;
        private double line2Height = 200;
        private double line2Width = 100;
        private int line2PointCount = 20;
        private double line2Sigma = 3.0;
        private int line2Threshold = 20;
        private int line2NumInstances = 1;
        private double line2DistThreshold = 3.5;
        private double line2MinScore = 0.5;
        private int line2MaxIterations = -1;
        private string line2Transition = "all";
        private string line2Select = "first";

        private double intersectionRow;
        private double intersectionColumn;
        private double angle;
        private string statusText = "等待加载图片";
        private string mouseInfo = "图像：--   行：--  列：--  灰度：--";
        private string line1Status = "线1拟合: --";
        private string line2Status = "线2拟合: --";
        private string line1Info = "线1: 未检测到";
        private string line2Info = "线2: 未检测到";
        private string parallelText = "是否平行: --";
        private string intersectionText = "交点：R=--  C=--";

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler RoiParameterChanged;

        private readonly ObservableCollection<DemoPluginInfo> plugins = new ObservableCollection<DemoPluginInfo>();
        public ObservableCollection<DemoPluginInfo> Plugins { get { return plugins; } }

        public DemoPluginInfo SelectedPlugin
        {
            get { return selectedPlugin; }
            set { SetField(ref selectedPlugin, value); }
        }

        public double Line1CenterRow { get { return line1CenterRow; } set { SetParameterField(ref line1CenterRow, value); } }
        public double Line1CenterColumn { get { return line1CenterColumn; } set { SetParameterField(ref line1CenterColumn, value); } }
        public double Line1Angle { get { return line1Angle; } set { SetParameterField(ref line1Angle, value); } }
        public double Line1Height { get { return line1Height; } set { SetParameterField(ref line1Height, value); } }
        public double Line1Width { get { return line1Width; } set { SetParameterField(ref line1Width, value); } }
        public int Line1PointCount { get { return line1PointCount; } set { SetParameterField(ref line1PointCount, value); } }
        public double Line1Sigma { get { return line1Sigma; } set { SetParameterField(ref line1Sigma, value); } }
        public int Line1Threshold { get { return line1Threshold; } set { SetParameterField(ref line1Threshold, value); } }
        public int Line1NumInstances { get { return line1NumInstances; } set { SetParameterField(ref line1NumInstances, value); } }
        public double Line1DistThreshold { get { return line1DistThreshold; } set { SetParameterField(ref line1DistThreshold, value); } }
        public double Line1MinScore { get { return line1MinScore; } set { SetParameterField(ref line1MinScore, value); } }
        public int Line1MaxIterations { get { return line1MaxIterations; } set { SetParameterField(ref line1MaxIterations, value); } }
        public string Line1Transition { get { return line1Transition; } set { SetParameterField(ref line1Transition, value); } }
        public string Line1Select { get { return line1Select; } set { SetParameterField(ref line1Select, value); } }

        public double Line2CenterRow { get { return line2CenterRow; } set { SetParameterField(ref line2CenterRow, value); } }
        public double Line2CenterColumn { get { return line2CenterColumn; } set { SetParameterField(ref line2CenterColumn, value); } }
        public double Line2Angle { get { return line2Angle; } set { SetParameterField(ref line2Angle, value); } }
        public double Line2Height { get { return line2Height; } set { SetParameterField(ref line2Height, value); } }
        public double Line2Width { get { return line2Width; } set { SetParameterField(ref line2Width, value); } }
        public int Line2PointCount { get { return line2PointCount; } set { SetParameterField(ref line2PointCount, value); } }
        public double Line2Sigma { get { return line2Sigma; } set { SetParameterField(ref line2Sigma, value); } }
        public int Line2Threshold { get { return line2Threshold; } set { SetParameterField(ref line2Threshold, value); } }
        public int Line2NumInstances { get { return line2NumInstances; } set { SetParameterField(ref line2NumInstances, value); } }
        public double Line2DistThreshold { get { return line2DistThreshold; } set { SetParameterField(ref line2DistThreshold, value); } }
        public double Line2MinScore { get { return line2MinScore; } set { SetParameterField(ref line2MinScore, value); } }
        public int Line2MaxIterations { get { return line2MaxIterations; } set { SetParameterField(ref line2MaxIterations, value); } }
        public string Line2Transition { get { return line2Transition; } set { SetParameterField(ref line2Transition, value); } }
        public string Line2Select { get { return line2Select; } set { SetParameterField(ref line2Select, value); } }

        public double IntersectionRow { get { return intersectionRow; } set { SetField(ref intersectionRow, value); } }
        public double IntersectionColumn { get { return intersectionColumn; } set { SetField(ref intersectionColumn, value); } }
        public double Angle { get { return angle; } set { SetField(ref angle, value); } }
        public string StatusText { get { return statusText; } set { SetField(ref statusText, value); } }
        public string MouseInfo { get { return mouseInfo; } set { SetField(ref mouseInfo, value); } }
        public string Line1Status { get { return line1Status; } set { SetField(ref line1Status, value); } }
        public string Line2Status { get { return line2Status; } set { SetField(ref line2Status, value); } }
        public string Line1Info { get { return line1Info; } set { SetField(ref line1Info, value); } }
        public string Line2Info { get { return line2Info; } set { SetField(ref line2Info, value); } }
        public string ParallelText { get { return parallelText; } set { SetField(ref parallelText, value); } }
        public string IntersectionText { get { return intersectionText; } set { SetField(ref intersectionText, value); } }

        public void LoadPluginsFromMef()
        {
            Plugins.Clear();
            try
            {
                foreach (var item in PluginService.pluginsDictionary)
                {
                    object info = item.Value;
                    Plugins.Add(new DemoPluginInfo
                    {
                        DisplayName = ReadString(info, "PluginsDisplayName", item.Key),
                        BuildName = ReadString(info, "PluginsBuildName", string.Empty),
                        FormTypeName = ReadTypeName(info, "PluginsFormType"),
                        FormType = ReadType(info, "PluginsFormType"),
                        Source = "MEF"
                    });
                }
            }
            catch
            {
            }

            if (Plugins.Count == 0)
            {
                Plugins.Add(new DemoPluginInfo
                {
                    DisplayName = "两线交点工具",
                    BuildName = "FindIntersectionObj",
                    FormTypeName = "FindIntersectionDemoDialog",
                    FormType = typeof(FindIntersectionDemoDialog),
                    Source = "Demo"
                });
            }

            SelectedPlugin = Plugins[0];
        }

        public void UpdateFromRoi(int index, double row, double column, double angleDegree, double height, double width, int pointCount)
        {
            if (index == 1)
            {
                SetField(ref line1CenterRow, row, "Line1CenterRow");
                SetField(ref line1CenterColumn, column, "Line1CenterColumn");
                SetField(ref line1Angle, angleDegree, "Line1Angle");
                SetField(ref line1Height, height, "Line1Height");
                SetField(ref line1Width, width, "Line1Width");
                SetField(ref line1PointCount, pointCount, "Line1PointCount");
            }
            else
            {
                SetField(ref line2CenterRow, row, "Line2CenterRow");
                SetField(ref line2CenterColumn, column, "Line2CenterColumn");
                SetField(ref line2Angle, angleDegree, "Line2Angle");
                SetField(ref line2Height, height, "Line2Height");
                SetField(ref line2Width, width, "Line2Width");
                SetField(ref line2PointCount, pointCount, "Line2PointCount");
            }
        }

        public void ResetResult()
        {
            Line1Status = "线1拟合: --";
            Line2Status = "线2拟合: --";
            Line1Info = "线1: 未检测到";
            Line2Info = "线2: 未检测到";
            ParallelText = "是否平行: --";
            IntersectionText = "交点：R=--  C=--";
            Angle = 0;
        }

        private void NotifyRoiParameterChanged()
        {
            var handler = RoiParameterChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private static string ReadString(object source, string propertyName, string defaultValue)
        {
            if (source == null) return defaultValue;
            var property = source.GetType().GetProperty(propertyName);
            object value = property == null ? null : property.GetValue(source, null);
            return value == null ? defaultValue : value.ToString();
        }

        private static string ReadTypeName(object source, string propertyName)
        {
            if (source == null) return string.Empty;
            var property = source.GetType().GetProperty(propertyName);
            var type = property == null ? null : property.GetValue(source, null) as Type;
            return type == null ? string.Empty : type.Name;
        }

        private static Type ReadType(object source, string propertyName)
        {
            if (source == null) return null;
            var property = source.GetType().GetProperty(propertyName);
            return property == null ? null : property.GetValue(source, null) as Type;
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void SetParameterField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (SetField(ref field, value, propertyName)) NotifyRoiParameterChanged();
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DemoPluginInfo
    {
        public string DisplayName { get; set; }
        public string BuildName { get; set; }
        public string FormTypeName { get; set; }
        public Type FormType { get; set; }
        public string Source { get; set; }

        public string UiKind
        {
            get
            {
                if (FormType == null) return "Demo";
                if (typeof(WPFFormPluginBase).IsAssignableFrom(FormType) || typeof(Window).IsAssignableFrom(FormType)) return "WPF";
                if (typeof(System.Windows.Forms.Form).IsAssignableFrom(FormType)) return "WinForm";
                return "Unknown";
            }
        }
    }
}
