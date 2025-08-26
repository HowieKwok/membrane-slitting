using Extensions;

using Frame_Core;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public class ChartUIParam
    {
        /// <summary>
        /// 图表名称
        /// </summary>
        public string strChartName { get; set; }

        public List<VLine> lVLines { get; set; } = new List<VLine>();

        public List<HLine> lHLines { get; set; } = new List<HLine>();

        public int TabIndex { get; set; } = 0;


        public float fBarWidth { get; set; } = 0.5f;

        private List<PiePlot> _lPiePlot = new List<PiePlot>();

        // 三维空间数据缓存：dim1 → dim2 → dim3 → Queue<ParamsChart>
        public ConcurrentDictionary<
                int,
                ConcurrentDictionary<
                    int,
                    ConcurrentDictionary<
                        int,
                        Queue<ParamsChart>>>> _data3DCache =
            new ConcurrentDictionary<
                int,
                ConcurrentDictionary<
                    int,
                    ConcurrentDictionary<
                        int,
                        Queue<ParamsChart>>>>();


        private Crosshair Crosshair;
        /// <summary>
        /// 饼图
        /// </summary>
        public List<PiePlot> lPiePlot
        {
            get { return _lPiePlot; }
            set
            {
                _lPiePlot = value;
            }
        }

        private List<SignalPlot> _lSignalPlott = new List<SignalPlot>();

        /// <summary>
        /// 曲线
        /// </summary>
        public List<SignalPlot> lSignalPlot
        {
            get { return _lSignalPlott; }
            set
            {
                _lSignalPlott = value;
            }
        }

        public List<double[]> dSignalPlots { get; set; } = new List<double[]>();








        private List<ScottPlot.Plottable.BarPlot> _lBarPlot = new List<BarPlot>();
        /// <summary>
        /// 柱状图
        /// </summary>
        public List<ScottPlot.Plottable.BarPlot> lBarPlot
        {
            get { return _lBarPlot; }
            set
            {
                _lBarPlot = value;
            }
        }



        List<ScottPlot.Plottable.DataStreamer> _lStreamer = new List<DataStreamer>();

        /// <summary>
        /// 滑动曲线
        /// </summary>
        public List<ScottPlot.Plottable.DataStreamer> lStreamer
        {
            get { return _lStreamer; }
            set
            {
                _lStreamer = value;
            }
        }


        List<ScottPlot.Plottable.DataLogger> _lDataLogger = new List<DataLogger>();

        public List<ScottPlot.Plottable.DataLogger> lDataLogger
        {
            get { return _lDataLogger; }
            set
            {
                _lDataLogger = value;
            }
        }



        public ScottPlot.FormsPlot plotView { get; set; }


        public ChartUIParam(string chartName, FormsPlot plot)
        {
            strChartName = chartName;
            plotView = plot;

            var dataStyles = ScottPlot.Style.GetStyles();
            var dataPalettes = ScottPlot.Palette.GetPalettes();
            plotView.Plot.Title(chartName);
            
            plotView.Plot.Palette = dataPalettes[3];
            plotView.Plot.Style(dataStyles[6]);
        
            //plotView.Plot.Legend(location: Alignment.LowerLeft);
            plotView.MouseMove += ((object sender, MouseEventArgs e) =>
            {
                (double coordinateX, double coordinateY) = plotView.GetMouseCoordinates();

                //XPixelLabel.Text = $"{e.X:0.000}";
                //YPixelLabel.Text = $"{e.Y:0.000}";

                //XCoordinateLabel.Text = $"{coordinateX:0.00000000}";
                //YCoordinateLabel.Text = $"{coordinateY:0.00000000}";
                if (Crosshair == null)
                {
                    Crosshair = plotView.Plot.AddCrosshair(0, 0);

                }
                Crosshair.IsVisible = true;
                Crosshair.X = coordinateX;
                Crosshair.Y = coordinateY;

                plotView.Refresh(lowQuality: true, skipIfCurrentlyRendering: true);
            });

            plotView.MouseLeave += ((object sender, EventArgs e) =>
            {
                if (Crosshair != null)
                {
                    Crosshair.IsVisible = false;
                    plotView.Refresh(lowQuality: true, skipIfCurrentlyRendering: true);
                }
            });

            plotView.Refresh();


        }



        public void InitCurve(ParamsChart param)
        {
            plotView?.Plot?.Clear();
            //clear();
            HLine hLine = plotView.Plot.AddHorizontalLine(param.f_UpLimit);
            hLine.LineColor = Color.Red;
            hLine.Label = "报警上限";
            lHLines.Add(hLine);

            hLine = plotView.Plot.AddHorizontalLine(param.f_StandarValue);
            hLine.LineColor = Color.Black;
            hLine.Label = "标准线";
            lHLines.Add(hLine);

            hLine = plotView.Plot.AddHorizontalLine(param.f_DownLimit);
            hLine.LineColor = Color.Red;
            hLine.Label = "报警下限";
            lHLines.Add(hLine);

            Refresh();
         
            if (lSignalPlot?.Count == 0)
            {
                for (int i = 0; i < param.points.Count(); i++)
                {
                    double[] ys = new double[nSignalCount];
                    for (int j = 0; j < nSignalCount; j++)
                    {
                        ys[j] = param.f_StandarValue;
                    }
                    dSignalPlots.Add(ys);
                    lSignalPlot.Add(plotView.Plot.AddSignal(ys));
                    lSignalPlot[i].Label = param.points[i].CurName;
                    if (i == 0)
                    {
                        lSignalPlot[i].Color = Color.Green;
                    }
                    else
                    {
                        lSignalPlot[i].Color = Color.Blue;
                    }

                    // lSignalPlot[i].ViewSlide(30);
                    // lSignalPlot[i].Plot.SetViewLimits(0);
                    lSignalPlot[i].LineWidth = 1.5;
                    lSignalPlot[i].MarkerSize = (float)0.1;
                    //lSignalPlot[i].MarkerShape = ScottPlot.MarkerShape.openCircle;
                    //lSignalPlot[i].MarkerColor = Color.Green; ;

                }
               // _data3DCache?.Clear();

               // UpDataCurve(param);
                Refresh();
            }

        }


        int nSignalCount = 100;
        int nSignalIndex = -1;
        public void UpDataCurve(ParamsChart param)
        {

            nSignalIndex++;


            for (int i = 0; i < param.points.Count(); i++)
            {
                if (nSignalIndex < nSignalCount - 1)
                {
                    dSignalPlots[i][nSignalIndex] = param.points[i].pointInfo.Row;
                }
                else
                {
                    nSignalIndex = 1000;
                    Array.Copy(dSignalPlots[i], 1, dSignalPlots[i], 0, dSignalPlots[i].Length - 1);
                    dSignalPlots[i][dSignalPlots[i].Length - 1] = param.points[i].pointInfo.Row;
                }


            }

            Refresh();



        }

        /// <summary>
        /// 更新曲线数据,3维数据
        /// </summary>
        /// <param name="param"></param>
        /// <param name="one1D"></param>
        /// <param name="two2D"></param>
        /// <param name="three3D"></param>
        public void UpDataCurve(ParamsChart param, int one1D, int two2D, int three3D,bool isShow=true)
        {
            if (isShow)
            {
                nSignalIndex++;


                for (int i = 0; i < param.points.Count(); i++)
                {
                    if (nSignalIndex < nSignalCount - 1)
                    {
                        dSignalPlots[i][nSignalIndex] = param.points[i].pointInfo.Row;
                    }
                    else
                    {
                        nSignalIndex = 1000;
                        Array.Copy(dSignalPlots[i], 1, dSignalPlots[i], 0, dSignalPlots[i].Length - 1);
                        dSignalPlots[i][dSignalPlots[i].Length - 1] = param.points[i].pointInfo.Row;
                    }


                }
            }

            Enqueue3D(_data3DCache, one1D, two2D, three3D, param);

            Refresh();



        }

        /// <summary>
        /// 初始化3维数据缓存
        /// </summary>
        /// <param name="one1D"></param>
        /// <param name="two2D"></param>
        /// <param name="three3D"></param>
        public void Init3DCache( int[] two2_3D, int three3D)
        {
            ExecuteMethod(() =>
            {
                _data3DCache?.Clear();
                if (two2_3D?.Length>0)
                {
                    for (int i = 0; i < two2_3D.Length; i++)
                    {
                        _data3DCache.GetOrAdd(i, _ => new ConcurrentDictionary<int, ConcurrentDictionary<int, Queue<ParamsChart>>>());
                        for (int j = 0; j < two2_3D[i]; j++)
                        {
                            _data3DCache[i].GetOrAdd(j, _ => new ConcurrentDictionary<int, Queue<ParamsChart>>());
                            for (int k = 0; k < three3D; k++)
                            {
                                _data3DCache[i][j].GetOrAdd(k, _ => new Queue<ParamsChart>());
                            }
                        }
                    }
                }
               
            });
        }

        public void UpData3DCache(int one1D, int two2D, int three3D)
        {

            ExecuteMethod(() =>
            {
                var q = _data3DCache[one1D][two2D][three3D];
                Queue<ParamsChart> temp = new Queue<ParamsChart>();
                int count = q.Count;

               for(int i=0;i<count;i++)
                {
                    //刷新曲线
                    ParamsChart paramsChart = q.Dequeue();
                    UpDataCurve(paramsChart, one1D, two2D, three3D,true);
               
                }
       
             
            });
        }




        /// <summary>线程安全地把数据压入三维缓存；队列长度超过 maxLen 时自动出队。</summary>
        private void Enqueue3D<T>(
              ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, Queue<T>>>> cache,
             int d1, int d2, int d3, T item, int maxLen = 100)
        {
            ExecuteMethod(() =>
            {
                // 一次性拿到第三级队列，不存在则自动创建
                var q = cache
                    .GetOrAdd(d1, _ => new ConcurrentDictionary<int, ConcurrentDictionary<int, Queue<T>>>())
                    .GetOrAdd(d2, _ => new ConcurrentDictionary<int, Queue<T>>())
                    .GetOrAdd(d3, _ => new Queue<T>());

                lock (q)                // Queue<T> 本身非线程安全，需加锁
                {
                  
                    q.Enqueue(item);
                    if (q.Count > maxLen)
                        q.Dequeue();
                }
            });

        }

        public void RootDataCurve()
        {
            lDataLogger?.Clear();
        }


        public void Refresh()
        {
            plotView.Refresh();
        }

        public void clear()
        {
            this.plotView.Plot.Clear();
            this._lBarPlot?.Clear();
            this._lPiePlot?.Clear();
            this._lSignalPlott?.Clear();
            this._lStreamer?.Clear();
            this._lDataLogger?.Clear();
            dSignalPlots.Clear();
            nSignalIndex = -1;
            Refresh();
           // Crosshair = null;

        }


        #region 方法模板
        /// <summary>
        /// 方法调用模板
        /// </summary>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        public void ExecuteMethod(Action action, [CallerMemberName] string methodName = "")
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType?.FullName ?? "UnknownClass";
            try
            {
                // 执行传入的委托
                action();
            }
            catch (Exception ee)
            {
                // 构造完整的异常信息，包含类名、方法名和异常详情
                string fullErrorMessage = $"[{className}.{methodName}] 发生异常: {ee.ToString()}";
                // 输出带调用堆栈的详细错误信息
                MessageNotify.AddErrorLog(fullErrorMessage);
            }
        }
        #endregion
    }

    [Serializable]
    public class ParamsChart:XmlSerializationOperation
    {
        public double f_UpLimit { get; set; } = 10;
        public double f_StandarValue { get; set; } = 0;
        public double f_DownLimit { get; set; } = -10;
        public List<PointClass> points { get; set; }=new  List<PointClass> { };


    }


    public class PointClass
    {
        /// <summary>
        /// 曲线名字
        /// </summary>
        public string CurName { get; set; } = "__";

        public Color Color { get; set; }=Color.Green;
        public Point Point { get; set; }=new Point();

        public PointInfo pointInfo { get; set; } = new PointInfo();
    }
}
