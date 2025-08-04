using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Defaults;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Drawing;
using Eholly.ZChart.Models;
using Eholly.ZChart;
using System.Windows.Forms;
using Frame_Core;
using LogTool;

namespace Mef_VisionFrame
{
    public class EHGraph
    {
        Color[] colors = new Color[] { Color.Blue };
        public ChartBase chartBase { set; get; }
        public GroupBox groupBox { set; get; }

        public double Upper = 100;

        public double lower = -20;
        public EHGraph(string name)
        {
            Init(name);
        }
        public void DisposeControl()
        {
            try
            {
                if (chartBase != null) chartBase.Dispose();
                if (groupBox != null) groupBox.Dispose();
            }
            catch (Exception)
            {


            }
        }
        private void Init(string name)
        {

            chartBase = new ChartBase();
            chartBase.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(chartBase, true);
            chartBase.BackColor = System.Drawing.Color.White;
            chartBase.Dock = System.Windows.Forms.DockStyle.Fill;
            chartBase.Location = new System.Drawing.Point(0, 0);
            chartBase.Name = name;// "chartBase2";
            chartBase.Size = new System.Drawing.Size(912, 826);
            chartBase.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            //chartBase.AutoSize = true;
         
            groupBox = new GroupBox() { Dock = DockStyle.Fill, Text = name, AutoSize = true };
            groupBox.Controls.Add(chartBase);

        }
        public void ChangeChartName(string name)
        {
            try
            {
                if (groupBox != null)
                {
                    groupBox.Invoke((EventHandler)delegate { groupBox.Text = name; });

                }
            }
            catch (Exception)
            {


            }

        }
        public void AddLine()
        {
            ClearLine();
            if (chartBase.ViewModel.Series == null)
                chartBase.CartesianChart.Series = new ObservableCollection<ISeries>();
            var data1 = new LineEntity { LineName = $"线条{chartBase.ViewModel.LineEntitys.Count + 1}", LineColor = colors[chartBase.ViewModel.LineEntitys.Count], GeometryStrockColor = colors[chartBase.ViewModel.LineEntitys.Count], GeometrySize = 5/*, GeometryStrock = 0*/ };
            chartBase.ViewModel.LineEntitys.Add(data1);
            chartBase.ViewModel.Series.Add(new LineSeries<ObservableValue>
            {
                Values = data1.Points,
                Fill = null,
                Stroke = data1.GetLineColor,
                GeometryStroke = data1.GetGeometryStrockColor,
                GeometrySize = data1.GeometrySize,// data1.GeometrySize,
                GeometryFill = data1.GetGeometryFill,                
            });         
            chartBase.CartesianChart.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.None;
        }

        private void ClearLine()
        {
            try
            {
                if (chartBase.ViewModel.Series == null) { return; }
                if (chartBase.ViewModel.Series.Count == 0) return;
                chartBase.ViewModel.Series.Clear();


                if (chartBase.ViewModel.LineEntitys == null) { return; }
                if (chartBase.ViewModel.LineEntitys.Count == 0) return;
                chartBase.ViewModel.LineEntitys.Clear();
            }
            catch (Exception ex)
            {

            }
        }

        object updateLock = new object();
        public void AddPoint(List<double> lstData, uint showPointNum)
        {
            try
            {
                chartBase.BeginInvoke((EventHandler)delegate
                {
                    try
                    {
                        if (chartBase.ViewModel.Series == null) return;
                        for (int i = 0; i < chartBase.ViewModel.LineEntitys.Count; i++)
                        {

                            for (int j = 0; j < lstData.Count; j++)
                            {
                                if (chartBase.ViewModel.LineEntitys[i].Points.Count > showPointNum) RemovePoint();

                                if (lstData[j] < chartBase.ViewModel.Y_Min) lstData[j] = chartBase.ViewModel.Y_Min;
                                if (lstData[j] > chartBase.ViewModel.Y_Max) lstData[j] = chartBase.ViewModel.Y_Min;

                                chartBase.ViewModel.LineEntitys[i].Points.Add(new ObservableValue(lstData[j]));
                            }
                        }

                        if (chartBase.ViewModel.LineEntitys == null) return;
                        if (chartBase.ViewModel.LineEntitys.Count > 0)
                        {
                            int dMin = 0;
                            int dMax = chartBase.ViewModel.LineEntitys[0].Points.Count;
                            int scale = Convert.ToInt32((dMax - dMin) / 10);
                            int XNum = (dMax - dMin) / scale + 1;
                            chartBase.ViewModel.X_Min = dMin - 10;
                            chartBase.ViewModel.X_Max = dMax + 10;
                            chartBase.ViewModel.X_Axes = new double[XNum];
                            for (int i = 0; i < chartBase.ViewModel.X_Axes.Length; i++)
                            {
                                chartBase.ViewModel.X_Axes[i] = dMin + i * scale;
                            }

                            chartBase.CartesianChart.XAxes = chartBase.ViewModel.XAxes;
                        }
                    }
                    catch (Exception ex)
                    {

                         Log4netRecord.ErrorLog.Debug(ex);
                    }


                });

            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        public void ClearPoint()
        {
            if (chartBase.ViewModel.Series == null) return;
            for (int i = 0; i < chartBase.ViewModel.LineEntitys.Count; i++)
            {
                chartBase.ViewModel.LineEntitys[i].Points.Clear();
            }

        }
        private void RemovePoint()
        {
            if (chartBase.ViewModel.Series == null) return;
            for (int i = 0; i < chartBase.ViewModel.LineEntitys.Count; i++)
            {
                if (chartBase.ViewModel.LineEntitys[i].Points.Count > 0)
                {
                    chartBase.ViewModel.LineEntitys[i].Points.RemoveAt(0);
                }
            }
        }

        public void AdaptYAxes(double dDown, double dUp)
        {
            try
            {
                double scale = (dUp - dDown) * 0.2;
                int YNum = 12;
                int weight = Convert.ToInt32((dUp - dDown) / scale);
                double dMin = dDown - ((YNum - weight) / 2) * scale;
                double dMax = dUp + ((YNum - weight) / 2) * scale;
                chartBase.ViewModel.Y_Min = dMin;
                chartBase.ViewModel.Y_Max = dMax;
                chartBase.ViewModel.Y_Axes = new double[YNum];
                for (int i = 0; i < YNum; i++)
                    chartBase.ViewModel.Y_Axes[i] = dMin + scale * i;
                chartBase.CartesianChart.YAxes = chartBase.ViewModel.YAxes;

                AdaptLimitAxes(dMin, dMax, dDown, dUp);

                if (chartBase.ViewModel.LineEntitys.Count > 0)
                {
                    chartBase.ViewModel.X_Min = -10;
                    chartBase.ViewModel.X_Max = chartBase.ViewModel.LineEntitys[0].Points.Count + 10;
                    chartBase.ViewModel.X_Axes = new double[] { chartBase.ViewModel.X_Min, chartBase.ViewModel.X_Max };
                    chartBase.CartesianChart.XAxes = chartBase.ViewModel.XAxes;
                }
               

            }
            catch (Exception ex)
            {

            }
        }
        private void AdaptLimitAxes(double iMin, double iMax, double dDown, double dUp)
        {
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    chartBase.ViewModel.SetAuxiliaryLines(iMin, iMax, Color.White, (byte)100);//设置上下限
                }

                chartBase.ViewModel.SetAuxiliaryLines(dDown, dUp, Color.DarkGreen, (byte)100);//设置上下限
            }
            catch (Exception ex)
            {

            }
        }
    }
}
