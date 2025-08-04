using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HalconDotNet;

namespace Frame_Core
{
    public static class OtherAlgorithm
    {
        /// <summary>
        /// 计算一个点是否在圆外
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        /// <returns></returns>
        public static bool IsPointOutsideCircle(double centerX, double centerY, double radius, double pointX, double pointY, out double distance)
        {
            // 计算点到圆心的距离
            distance = Math.Sqrt(Math.Pow(pointX - centerX, 2) + Math.Pow(pointY - centerY, 2));
            distance = Math.Abs(distance);

            // 如果距离大于半径，则点在圆外
            return distance > radius;
        }


        /// <summary>
        /// 校准数据,使数据波动更小
        /// </summary>
        /// <param name="inputOpenDecCalibration">是否启用</param>
        /// <param name="inputSpecification">输入的基准值</param>
        /// <param name="inputTolerance">基准公差</param>
        /// <param name="inputToleranceCalibration">校准公差</param>
        /// <param name="inputValue">需要修改的值</param>
        public static void Calibration(bool inputOpenDecCalibration, double inputSpecification,
            double inputTolerance, double inputToleranceCalibration,
          ref double inputValue)
        {
            if (inputOpenDecCalibration)
            {
                double tempMin = inputSpecification - inputTolerance;
                double tempMax = inputSpecification + inputTolerance;

                double tempCalibrationMin = inputSpecification -
                    inputToleranceCalibration;
                double tempCalibrationMax = inputSpecification +
                   inputToleranceCalibration;

                //当值超过基准公差范围时,才校正
                if (tempMin > inputValue || inputValue > tempMax)
                {
                    if (tempCalibrationMin <= inputValue && inputValue <= tempCalibrationMax)
                    {

                        Random random = new Random();
                        inputValue = random.NextDouble() * (tempMax - tempMin) + tempMin;

                    }
                }

            }


        }

        public static void data_filter_output_index(HTuple hv_grayval, HTuple hv_rateLowRemove,
        HTuple hv_rateHighRemove, out HTuple hv_grayvalOut, out HTuple hv_dataFilterIndex)
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
    }
}
