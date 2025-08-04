using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.D3MeasureDepth
{
    internal class Filter
    {
        /// <summary>
        /// 滑动平均数滤波
        /// </summary>
        /// <param name="pointDic"></param>
        /// <param name="xishu"></param>
        /// <returns></returns>
        public static List<double> MoveAvgFilter(List<double> pointDic, int filterNum)
        {
            try
            {
                List<double> a1 = new List<double>();

                for (int i = 0; i < pointDic.Count; i++)
                {
                    List<double> list = new List<double>();

                    if (i < pointDic.Count - (filterNum / 2))
                    {
                        for (int j = filterNum / 2; j >= -(filterNum / 2); j--)
                        {
                            list.Add(pointDic[Math.Abs(i - j)]);
                        }
                    }
                    else
                    {
                        for (int j = filterNum / 2; j >= -(filterNum / 2); j--)
                        {
                            if (j >= 0)
                            {
                                list.Add(pointDic[Math.Abs(i - j)]);
                            }
                            else
                            {
                                list.Add(pointDic[Math.Abs(i + j)]);
                            }
                        }
                    }

                    Dictionary<double, int> quan = new Dictionary<double, int>();
                    for (int l = 0; l < list.Count; l++)
                    {
                        if (quan.ContainsKey(list[l]) == false)
                        {
                            quan[list[l]] = 1;
                        }
                        else
                        {
                            quan[list[l]]++;
                        }
                    }


                    if (list.Count > 0)
                    {
                        double sum = 0;
                        double quansum = 0;

                        for (int k = 0; k < list.Count; k++)
                        {
                            double c = list[k];
                            int a = quan[list[k]];
                            sum += list[k] * a;
                            quansum += a;
                        }

                        a1.Add(sum / quansum);
                    }
                }
                return a1;
            }
            catch (Exception ex)
            {
                return pointDic;
                //LogName.ErrorLog.Fatal(ex);
            }

        }

        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="pointDic"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static List<double> AvgFilter(List<double> pointDic, int factor)
        {
            try
            {
                List<double> a2 = new List<double>();
                for (int i = 0; i < pointDic.Count; i++)
                {
                    List<double> list = new List<double>();
                    if (i < pointDic.Count - (factor / 2))
                    {
                        for (int j = factor / 2; j >= -(factor / 2); j--)
                        {
                            list.Add(pointDic[Math.Abs(i - j)]);
                        }
                    }
                    else
                    {
                        for (int j = factor / 2; j >= -(factor / 2); j--)
                        {
                            if (j >= 0)
                            {
                                list.Add(pointDic[Math.Abs(i - j)]);
                            }
                            else
                            {
                                list.Add(pointDic[Math.Abs(i + j)]);
                            }
                        }
                    }
                    a2.Add(list.Average());
                }
                return a2;
            }
            catch (Exception ex)
            {
                //LogName.ErrorLog.Fatal(ex);
            }
            return null;

        }
        /// <summary>
        /// 对一维数组进行滤波，并输出序号
        /// </summary>
        /// <param name="hv_grayval">输入一维数组</param>
        /// <param name="hv_rateLowRemove">去除高差低的百分比</param>
        /// <param name="hv_rateHighRemove">去除高差高的百分比</param>
        /// <param name="hv_grayvalOut">滤波后的排序后输出一维数组</param>
        /// <param name="hv_dataFilterIndex">滤波后的排序后输出排序前对应的index</param>
        public static void data_filter_output_index(HTuple hv_grayval, HTuple hv_rateLowRemove, HTuple hv_rateHighRemove, out HTuple hv_grayvalOut, out HTuple hv_dataFilterIndex)
        {
            // Local iconic variables 
            // Local control variables 
            HTuple hv_Index = new HTuple(), hv_graySorted = new HTuple();
            HTuple hv_m = new HTuple(), hv_n = new HTuple(), hv_SequenceHighRemove = new HTuple();
            HTuple hv_grayReduced1 = new HTuple(), hv_SequenceLowRemove = new HTuple();
            // Initialize local and output iconic variables 
            hv_grayvalOut = new HTuple();
            hv_dataFilterIndex = new HTuple();
            //* 功能：对一维数组进行滤波，并输出序号
            //* 输入
            //* grayval                     输入一维数组
            //* rateLowRemove               去除高差低的百分比（滤波）  eg:0.1
            //* rateHighRemove              去除高差高的百分比         eg:0.1
            //*** 输出
            //* grayvalOut                  滤波后的排序后输出一维数组
            //* dataFilterIndex             滤波后的排序后输出排序前对应的index
            //* written by guke
            //*--------------------------------------------------------------------------------------
            HOperatorSet.TupleSortIndex(hv_grayval, out hv_Index);
            HOperatorSet.TupleSort(hv_grayval, out hv_graySorted);
            //去除低m%  去除高n%

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_m = (((new HTuple(hv_graySorted.TupleLength()
                    )) * hv_rateLowRemove)).TupleInt();
            }

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_n = (((new HTuple(hv_graySorted.TupleLength()
                    )) * hv_rateHighRemove)).TupleInt();
            }
            if ((int)(new HTuple(hv_n.TupleNotEqual(0))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {

                    HOperatorSet.TupleGenSequence(((new HTuple(hv_graySorted.TupleLength())) - 1) - hv_n,
                        (new HTuple(hv_graySorted.TupleLength())) - 1, 1, out hv_SequenceHighRemove);
                }

                HOperatorSet.TupleRemove(hv_graySorted, hv_SequenceHighRemove, out hv_grayReduced1);
            }
            else
            {

                hv_grayReduced1 = new HTuple(hv_graySorted);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {

                HOperatorSet.TupleGenSequence(0, hv_m - 1, 1, out hv_SequenceLowRemove);
            }

            HOperatorSet.TupleRemove(hv_grayReduced1, hv_SequenceLowRemove, out hv_grayvalOut);

            //Index去除高m%  去除低n%

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_m = (((new HTuple(hv_Index.TupleLength()
                    )) * hv_rateLowRemove)).TupleInt();
            }

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_n = (((new HTuple(hv_Index.TupleLength()
                    )) * hv_rateHighRemove)).TupleInt();
            }
            if ((int)(new HTuple(hv_n.TupleNotEqual(0))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {

                    HOperatorSet.TupleGenSequence(((new HTuple(hv_Index.TupleLength())) - 1) - hv_n,
                        (new HTuple(hv_Index.TupleLength())) - 1, 1, out hv_SequenceHighRemove);
                }

                HOperatorSet.TupleRemove(hv_Index, hv_SequenceHighRemove, out hv_grayReduced1);
            }
            else
            {

                hv_grayReduced1 = new HTuple(hv_Index);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {

                HOperatorSet.TupleGenSequence(0, hv_m - 1, 1, out hv_SequenceLowRemove);
            }

            HOperatorSet.TupleRemove(hv_grayReduced1, hv_SequenceLowRemove, out hv_dataFilterIndex);



            return;
        }

        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="pointDic">输入的数值列表（HTuple）</param>
        /// <param name="factor">滤波窗口大小</param>
        /// <returns>滤波后的结果列表（HTuple）</returns>
        public static HTuple AvgFilter(HTuple pointDic, int factor)
        {
            try
            {
                HTuple a2 = new HTuple();
                int halfFactor = factor / 2;

                for (int i = 0; i < pointDic.Length; i++)
                {
                    HTuple list = new HTuple();

                    for (int j = -halfFactor; j <= halfFactor; j++)
                    {
                        int index = i + j;
                        if (index >= 0 && index < pointDic.Length)
                        {
                            list = list.TupleConcat(pointDic[index]);
                        }
                        else
                        {
                            // 如果超出范围，使用对称值
                            index = i - j;
                            if (index >= 0 && index < pointDic.Length)
                            {
                                list = list.TupleConcat(pointDic[index]);
                            }
                        }
                    }

                    // 计算平均值
                    double average = list.TupleMean().D;
                    a2 = a2.TupleConcat(average);
                }

                return a2;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("Exception occurred: " + ex.Message);
            }

            return null;
        }

        public static HTuple MoveAvgFilterOptimized(HTuple inputTuple, int filterNum)
        {
            int halfWindow = filterNum / 2;
            int length = inputTuple.Length;
            double[] outputArray = new double[length];

            // 预计算权重
            int[] weightCache = new int[length];
            for (int i = 0; i < length; i++)
            {
                weightCache[i] = inputTuple.TupleFind(inputTuple[i]).Length;
            }

            for (int i = 0; i < length; i++)
            {
                double sum = 0;
                double weightSum = 0;

                for (int j = -halfWindow; j <= halfWindow; j++)
                {
                    int index = i + j;

                    // 边界镜像处理
                    if (index < 0) index = -index;
                    else if (index >= length) index = 2 * length - index - 2;

                    double value = inputTuple[index].D;
                    int weight = weightCache[index];

                    sum += value * weight;
                    weightSum += weight;
                }

                outputArray[i] = sum / weightSum;
            }

            return new HTuple(outputArray);
        }
    }
}
