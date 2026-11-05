using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class ScotPotUIUser : UserControl
    {
        #region 私有字段
        private readonly FormsPlot _plotView;
        private readonly ConcurrentDictionary<string, int> dicSignalIndexs = new ConcurrentDictionary<string, int>();
        public int nMaxCount = 100;

        // 坐标轴锁定标记（解决SetYAxisRange刷新被覆盖失效）
        private bool _isAxisFixed = false;
        private double _fixedXMin, _fixedXMax;
        private double _fixedYMin, _fixedYMax;
        #endregion

        #region 构造函数
        /// <summary>代码动态实例化构造</summary>
        public ScotPotUIUser(FormsPlot formsPlot, string title, int paletteIndex = 6, int styleIndex = 8, int maxCount = 100)
        {
            InitializeComponent();

            // FormsPlot铺满整个UserControl面板
            _plotView = formsPlot;
            //_plotView.Dock = DockStyle.Fill;
            //this.Controls.Add(_plotView);
            nMaxCount = maxCount;
            InitPlotStyle(title, paletteIndex, styleIndex);
        }

        /// <summary>窗体设计器拖拽使用无参构造</summary>
        public ScotPotUIUser()
        {
            InitializeComponent();
            _plotView = new FormsPlot();
            _plotView.Dock = DockStyle.Fill;
            this.Controls.Add(_plotView);
        }
        #endregion

        #region 图表初始化样式
        private void InitPlotStyle(string title, int PaletteIndex, int StyleIndex)
        {
            Plot plt = _plotView.Plot;

            // 防止鼠标事件重复绑定
            _plotView.MouseMove -= Plot_MouseMove;
            _plotView.MouseLeave -= Plot_MouseLeave;

            // 图表标题
            plt.Title(title);

            // 配色与主题样式
            var allStyles = Style.GetStyles();
            var allPalettes = Palette.GetPalettes();
            if (PaletteIndex < allPalettes.Count() && StyleIndex < allStyles.Count())
            {
                plt.Palette = allPalettes[PaletteIndex];
                plt.Style(allStyles[StyleIndex]);
            }
            else
            {
                plt.Palette = allPalettes[6];
                plt.Style(allStyles[8]);
            }

            // 十字光标初始化样式
            Crosshair cross = GetOrCreateCrosshair(_plotView);
            cross.Color = Color.Red;
            cross.LineWidth = 1;

            // 绑定鼠标交互
            _plotView.MouseMove += Plot_MouseMove;
            _plotView.MouseLeave += Plot_MouseLeave;

            // 清理多余无用DataLogger图层
            var loggerList = plt.GetPlottables().OfType<DataLogger>().ToList();
            loggerList.ForEach(x => plt.Remove(x));

            RefreshUI();
        }
        #endregion

        #region 对外业务接口
        /// <summary>新增/更新水平阈值线</summary>
        public void SetHorizontalLine(double Value, Color color, string LineName, LineStyle lineStyle=LineStyle.Solid)
        {
            HLine hLine = GetHLineByLabel(_plotView.Plot, LineName);
            if (hLine != null)
            {
                hLine.Color = color;
                hLine.Y = Value;
            }
            else
            {
                hLine = _plotView.Plot.AddHorizontalLine(Value);
                hLine.LineColor = color;
                hLine.Label = LineName;
                hLine.LineStyle = lineStyle;
            }
            RefreshUI();
        }

        /// <summary>创建初始化波形曲线</summary>
        public void SetSignalPlotLine(double initValue, Color color, string LineName)
        {
            SignalPlot signalPlot = GetSignalPlotByLabel(_plotView.Plot, LineName);
            double[] buffer = Enumerable.Repeat(initValue, nMaxCount).ToArray();

            if (signalPlot != null)
            {
                signalPlot.Color = color;
                signalPlot.Ys = buffer;
            }
            else
            {
                signalPlot = _plotView.Plot.AddSignal(buffer);
                signalPlot.LineColor = color;
                signalPlot.Label = LineName;
            }

            signalPlot.LineWidth = 1.5f;
            signalPlot.MarkerColor = Color.AntiqueWhite;
            signalPlot.MarkerShape = MarkerShape.openCircle;
            dicSignalIndexs[LineName] = -1;
            RefreshUI();
        }

        /// <summary>滑动窗口刷新曲线数据（跨线程安全）</summary>
        public void UpdateCurve(string label, double value)
        {
            //if (_plotView == null || !_plotView.IsHandleCreated) return;
            if (!Form_Main.FormMain.IsHandleCreated) return;
            Form_Main.FormMain.BeginInvoke(new Action(() =>
        {
            //if (this.InvokeRequired)
            //{
            //    this.BeginInvoke(new Action(() => UpdateCurve(label, value)));
            //    return;
            //}

            SignalPlot tempPlot = GetSignalPlotByLabel(_plotView.Plot, label);
            if (tempPlot == null) return;

            if (!dicSignalIndexs.ContainsKey(label))
                dicSignalIndexs[label] = -1;

            int index = ++dicSignalIndexs[label];
            double[] ys = tempPlot.Ys;
            int windowLen = ys.Length;

            if (index < windowLen - 1)
            {
                ys[index] = value;
            }
            else
            {
                Array.Copy(ys, 1, ys, 0, windowLen - 1);
                ys[windowLen - 1] = value;
                dicSignalIndexs[label] = windowLen - 1;
            }

            tempPlot.Ys = ys;
            RefreshUI();

        }));



        }

        /// <summary>清空整张图表所有图层</summary>
        public void Clear()
        {
            _plotView.Plot.Clear();
            dicSignalIndexs.Clear();
            _isAxisFixed = false;
            RefreshUI();
        }
        #endregion

        #region Y轴/XY轴范围设置（修复刷新覆盖问题）
        /// <summary>仅锁定Y轴显示区间</summary>
        public void SetYAxisRange(double yMin, double yMax)
        {
            _isAxisFixed = true;
            _fixedYMin = yMin;
            _fixedYMax = yMax;
            // 保留当前X轴边界不变
            var limits = _plotView.Plot.GetAxisLimits();
            _fixedXMin = limits.XMin;
            _fixedXMax = limits.XMax;

            _plotView.Plot.SetAxisLimits(yMin: yMin, yMax: yMax);
            RefreshUI();
        }

        /// <summary>同时锁定X轴与Y轴完整范围</summary>
        public void SetAxisRange(double xMin, double xMax, double yMin, double yMax)
        {
            _isAxisFixed = true;
            _fixedXMin = xMin;
            _fixedXMax = xMax;
            _fixedYMin = yMin;
            _fixedYMax = yMax;

            double yMargin = Math.Abs(yMax - yMin) * 0.01;
            _plotView.Plot.SetAxisLimits(xMin, xMax, yMin - yMargin, yMax + yMargin);
            RefreshUI();
        }

        /// <summary>取消坐标轴锁定，恢复数据自适应缩放</summary>
        public void CancelAxisFixed()
        {
            _isAxisFixed = false;
            RefreshUI();
        }
        #endregion

        #region 图层查找与删除工具方法
        private HLine GetHLineByLabel(Plot plot, string label)
        {
            if (plot == null || string.IsNullOrWhiteSpace(label)) return null;
            foreach (var item in plot.GetPlottables())
            {
                if (item is HLine h && h.Label == label)
                    return h;
            }
            return null;
        }


        /// <summary>移除图表全部水平标线(HLine)</summary>
        public void RemoveAllHorizontalLines()
        {
            Plot plt = _plotView.Plot;
            // 先查找到所有HLine集合，再遍历删除（不要直接foreach删除集合）
            var allHLines = plt.GetPlottables().OfType<HLine>().ToList();
            foreach (var hLine in allHLines)
            {
                plt.Remove(hLine);
            }
            RefreshUI();
        }
        private SignalPlot GetSignalPlotByLabel(Plot plot, string label)
        {
            if (plot == null || string.IsNullOrWhiteSpace(label)) return null;
            foreach (var item in plot.GetPlottables())
            {
                if (item is SignalPlot sp && sp.Label == label)
                    return sp;
            }
            return null;
        }

        public void RemoveHLineByLabel(string label)
        {
            var line = GetHLineByLabel(_plotView.Plot, label);
            if (line != null) _plotView.Plot.Remove(line);
            RefreshUI();
        }

        public void RemoveSignalPlotByLabel(string label)
        {
            var sp = GetSignalPlotByLabel(_plotView.Plot, label);
            if (sp != null) _plotView.Plot.Remove(sp);
            dicSignalIndexs.TryRemove(label, out _);
            RefreshUI();
        }
        #endregion

        #region 十字光标鼠标交互
        private void Plot_MouseMove(object sender, MouseEventArgs e)
        {
            FormsPlot plotView = sender as FormsPlot;
            if (plotView == null) return;

            var (coordinateX, coordinateY) = plotView.GetMouseCoordinates();
            Crosshair cross = GetOrCreateCrosshair(plotView);
            cross.IsVisible = true;
            cross.X = coordinateX;
            cross.Y = coordinateY;
            plotView.Refresh(lowQuality: true, skipIfCurrentlyRendering: true);
        }

        private void Plot_MouseLeave(object sender, EventArgs e)
        {
            FormsPlot plotView = sender as FormsPlot;
            if (plotView == null) return;
            HideCrosshair(plotView);
            plotView.Refresh(lowQuality: true, skipIfCurrentlyRendering: true);
        }

        private Crosshair GetOrCreateCrosshair(FormsPlot plotView)
        {
            Plot plt = plotView.Plot;
            foreach (var plottable in plt.GetPlottables())
            {
                if (plottable is Crosshair ch)
                    return ch;
            }
            return plt.AddCrosshair(0, 0);
        }

        private void HideCrosshair(FormsPlot plotView)
        {
            Crosshair cross = GetOrCreateCrosshair(plotView);
            cross.IsVisible = false;
        }

        /// <summary>解绑鼠标事件，用于控件销毁</summary>
        public void ClearPlotMouseEvent()
        {
            _plotView.MouseMove -= Plot_MouseMove;
            _plotView.MouseLeave -= Plot_MouseLeave;
        }

        /// <summary>清除全部十字光标</summary>
        public void RemoveAllCrosshair()
        {
            var crossList = _plotView.Plot.GetPlottables().OfType<Crosshair>().ToList();
            crossList.ForEach(ch => _plotView.Plot.Remove(ch));
            RefreshUI();
        }
        #endregion

        #region 图表刷新核心（关键：锁定轴禁止Auto覆盖）
        public void RefreshUI()
        {
            Plot plt = _plotView.Plot;

            if (_isAxisFixed)
            {
                // 固定坐标轴强制重设边界，不会被自适应覆盖
                plt.SetAxisLimits(_fixedXMin, _fixedXMax, _fixedYMin, _fixedYMax);
            }
            else
            {
                // 自动适配波形，忽略水平线避免量程被横线撑开
                plt.AxisAuto();
            }

            // 紧凑布局，Y轴刻度贴左侧边缘
            plt.Layout(left: 12, right: 5, top: 20, bottom: 20);


            _plotView.Refresh(lowQuality: true, skipIfCurrentlyRendering: true);
        }
        #endregion

        #region 资源释放
        public void clear(bool disposing)
        {
            if (disposing)
            {
                ClearPlotMouseEvent();
                dicSignalIndexs.Clear();
                _plotView?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        /// <summary>
        /// 获取轴坐标
        /// </summary>
        /// <param name="dDown"></param>
        /// <param name="dUp"></param>
        /// <param name="Min_Value">轴坐标最小值</param>
        /// <param name="Max_Value">轴坐标最大值</param>
        /// <param name="LineWidthCefficient">轴坐标线宽系数</param>
        /// <returns></returns>
        public double[] GetAxess(double dDown, double dUp, out double Min_Value, out double Max_Value, out double LineWidthCefficient)
        {
            double[] Y_Axes = new double[2] { 0.0, 100.0 };
            Min_Value = 0.0;
            Max_Value = 100.0;
            LineWidthCefficient = 1;
            try
            {
                double scale = (dUp - dDown) * 0.2;
                int YNum = 12;
                int weight = Convert.ToInt32((dUp - dDown) / scale);
                double dMin = dDown - ((YNum - weight) / 2) * scale;
                double dMax = dUp + ((YNum - weight) / 2) * scale;
                Min_Value = dMin;
                Max_Value = dMax;
                Y_Axes = new double[YNum];
                for (int i = 0; i < YNum; i++)
                    Y_Axes[i] = dMin + scale * i;

                LineWidthCefficient = (Max_Value - Min_Value) / 100;
                if (LineWidthCefficient <= 0) LineWidthCefficient = 1;
            }
            catch (Exception)
            {

            }
            return Y_Axes;

        }

        /// <summary>
        /// 获取轴坐标
        /// </summary>
        /// <param name="dDown"></param>
        /// <param name="dUp"></param>
        /// <param name="Min_Value">轴坐标最小值</param>
        /// <param name="Max_Value">轴坐标最大值</param>
        /// <param name="LineWidthCefficient">轴坐标线宽系数</param>
        /// <returns></returns>
        public void GetAxess(double dDown, double dUp, out double Min_Value, out double Max_Value)
        {
            double[] Y_Axes = new double[2] { 0.0, 100.0 };
            Min_Value = 0.0;
            Max_Value = 100.0;

            try
            {

                Min_Value = dDown - 0.5;
                Max_Value = dUp + 0.5;
             

            }
            catch (Exception)
            {

            }
      
        }
    }
}
