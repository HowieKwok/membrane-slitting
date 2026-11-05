using System;
using System.Windows;
using System.Windows.Media;

namespace FrameCommunity.UserControls
{
    /// <summary>
    /// 灰度曲线显示控件。
    /// 只接收 Values 并绘制，不知道插件、不知道 Halcon、不做任何算法。
    /// </summary>
    public sealed class ProfileCurveControl : FrameworkElement
    {
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register(
                nameof(Values),
                typeof(double[]),
                typeof(ProfileCurveControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                nameof(Stroke),
                typeof(Brush),
                typeof(ProfileCurveControl),
                new FrameworkPropertyMetadata(Brushes.Cyan, FrameworkPropertyMetadataOptions.AffectsRender));

        public double[] Values
        {
            get { return (double[])GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double[] data = Values;
            if (data == null || data.Length < 2) return;

            double width = Math.Max(1, ActualWidth);
            double height = Math.Max(1, ActualHeight);
            double min = double.MaxValue;
            double max = double.MinValue;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] < min) min = data[i];
                if (data[i] > max) max = data[i];
            }

            if (Math.Abs(max - min) < 0.0001)
            {
                max = min + 1;
            }

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                int step = Math.Max(1, data.Length / 900);
                bool first = true;

                for (int i = 0; i < data.Length; i += step)
                {
                    double x = width * i / Math.Max(1, data.Length - 1);
                    double y = height - (data[i] - min) / (max - min) * height;

                    if (first)
                    {
                        context.BeginFigure(new Point(x, y), false, false);
                        first = false;
                    }
                    else
                    {
                        context.LineTo(new Point(x, y), true, false);
                    }
                }
            }

            geometry.Freeze();
            drawingContext.DrawGeometry(null, new Pen(Stroke, 1.3), geometry);
        }
    }
}

/*
改动说明：
1. 新增纯显示控件，用依赖属性 Values 接收 VM 的灰度数组。
2. 控件只负责绘制曲线，不参与参数同步、图像处理和插件运行。
3. 替代原 View.xaml.cs 中 DrawProfile/Canvas.Children.Clear 的代码。
*/
