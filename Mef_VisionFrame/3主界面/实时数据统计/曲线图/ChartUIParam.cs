using Frame_Core;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public float fBarWidth { get; set; } = 0.5f;

        private List<PiePlot> _lPiePlot = new List<PiePlot>();


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

                //  lSignalPlot[i].Update(dSignalPlots[i].ToArray());



            }
            Refresh();


            //if (lDataLogger[i].Count > 200)
            //{

            //    lDataLogger[i].Clear();
            //    lDataLogger[i].Plot.SetViewLimits(param.points[i].Point.X);
            //}

            //if (lDataLogger?.Count == 0)
            //{
            //    for (int i = 0; i < param.points.Count(); i++)
            //    {
            //        lDataLogger.Add(plotView.Plot.AddDataLogger());
            //        lDataLogger[i].Label = param.points[i].CurName;
            //        lDataLogger[i].ViewSlide(30);
            //        lDataLogger[i].Plot.SetViewLimits(0);

            //        lDataLogger[i].MarkerSize = 5;
            //        lDataLogger[i].MarkerShape = ScottPlot.MarkerShape.openCircle;
            //        lDataLogger[i].MarkerColor = Color.Green; ;
            //        lDataLogger[i].ManageAxisLimits = true;
            //    }
            //}

            //for (int i = 0; i < param.points.Count(); i++)
            //{
            //    lDataLogger[i].Add(param.points[i].Point.X, param.points[i].Point.Y);
            //    if (lDataLogger[i].Count > 200)
            //    {

            //        lDataLogger[i].Clear();
            //        lDataLogger[i].Plot.SetViewLimits(param.points[i].Point.X);
            //    }


            //}

           // plotView.Plot.Legend(location: Alignment.LowerLeft);
            Refresh();

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

    }


    public class ParamsChart
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
