using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class ChartLineInfo
    {
        public double value { get; set; } = 0;
        public Color Color { get; set; } = Color.Aquamarine;

        public string StrLabel { get; set; } = "默认线";

        /// <summary>
        /// 0:水平线 1：实时曲线
        /// </summary>
        public int mode = 0;


    }


    public static class NScotPotUIUserExtension
    {

        public static void InitChart(this ScotPotUIUser scotPotUIUser, string title, List<ChartLineInfo> chartLines, double yAxisMin, double yAxisMax, int xAxisMaxCount = 100)
        {

            scotPotUIUser.BeginInvoke(new Action(() =>
            {
                scotPotUIUser?.Clear();

                foreach (var item in chartLines)
                {
                    if (item.mode == 0)
                    {
                        scotPotUIUser.SetHorizontalLine(item.value, item.Color, item.StrLabel);
                    }
                    else
                    {
                        scotPotUIUser.SetSignalPlotLine(item.value, item.Color, item.StrLabel);
                    }
                }
                scotPotUIUser.SetYAxisRange(yAxisMin, yAxisMax);
                scotPotUIUser.CancelAxisFixed();
            }));
        }

        /// <summary>
        /// 重更新曲线
        /// </summary>
        /// <param name="scotPotUIUser"></param>
        /// <param name="chartLines"></param>
        /// <param name="yAxisMin"></param>
        /// <param name="yAxisMax"></param>
        public static void UpdateHorizontalLine(this ScotPotUIUser scotPotUIUser, List<ChartLineInfo> chartLines, double yAxisMin, double yAxisMax)
        {

            scotPotUIUser.BeginInvoke(new Action(() =>
            {
                scotPotUIUser.RemoveAllHorizontalLines();
                foreach (var item in chartLines)
                {
                    if (item.mode == 0)
                    {
                        scotPotUIUser.SetHorizontalLine(item.value, item.Color, item.StrLabel);
                    }
                    else
                    {
                        scotPotUIUser.SetSignalPlotLine(item.value, item.Color, item.StrLabel);
                    }
                }
                scotPotUIUser.SetYAxisRange(yAxisMin, yAxisMax);
                scotPotUIUser.CancelAxisFixed();
            }));


        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="scotPotUIUser"></param>
        /// <param name="chartLineInfo"></param>
        public static void UpdateDate(this ScotPotUIUser scotPotUIUser, string label, double value)
        {
            scotPotUIUser.BeginInvoke(new Action(() =>
            {
                scotPotUIUser.UpdateCurve(label, value);
            }));

        }

        /// <summary>
        /// 刷新数据组
        /// </summary>
        /// <param name="scotPotUIUser"></param>
        /// <param name="chartLineInfo"></param>
        public static void UpdateDates(this ScotPotUIUser scotPotUIUser, string label, List<double> dates)
        {
            scotPotUIUser.BeginInvoke(new Action(() =>
            {
                for (int i = 0; i < dates.Count(); i++)
                {
                    scotPotUIUser.UpdateCurve(label, dates[i]);
                }

            }));

        }
    }
}
