using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity.PID
{
    /// <summary>
    /// 后段尺寸反馈补偿器。
    /// 这个类只输出“当前这轮尺寸偏差对应的 xComp”。
    /// 注意：xComp 不是累计补偿值；在死区内、样本未凑满、样本异常时输出 0。
    /// 原脚本拿到 xComp 后按以下方式接入：
    /// SendDiff = SendDiff + xComp;
    /// double SendDiff2 = SendDiff - pic;
    /// lastRunLength = lastRunLength + xComp;
    /// </summary>
    public class BackSizeDeviationCompensator
    {
        private readonly Queue<double> _sizeErrors = new Queue<double>();
        private readonly object _syncRoot = new object();

        /// <summary>是否启用后段尺寸反馈。</summary>
        public bool Enabled { get; set; }

        /// <summary>尺寸偏差转换成 xComp 的比例。方向反了改正负号。</summary>
        public double Gain { get; set; }

        /// <summary>多少个有效尺寸样本计算一次 xComp。</summary>
        public int SampleCount { get; set; }

        /// <summary>平均尺寸偏差在该范围内时，xComp 输出 0。</summary>
        public double DeadBand { get; set; }

        /// <summary>单个尺寸偏差超过该值时直接丢弃。</summary>
        public double MaxSingleSizeError { get; set; }

        /// <summary>IQR 异常值剔除倍数。</summary>
        public double IqrFactor { get; set; }

        /// <summary>单次输出的 xComp 最大绝对值。</summary>
        public double MaxOutputCompensation { get; set; }

        /// <summary>
        /// 当前输出给原脚本叠加使用的 xComp。
        /// 死区内、未凑满样本、异常丢弃时会回到 0。
        /// </summary>
        public double CurrentDeviationCompensation { get; private set; }

        public BackSizeDeviationCompensator()
        {
            Enabled = true;
            Gain = 0.3;
            SampleCount = 5;
            DeadBand = 0.02;
            MaxSingleSizeError = 2.0;
            IqrFactor = 1.5;
            MaxOutputCompensation = 0.1;
        }

        /// <summary>
        /// 后段相机有新尺寸时调用。
        /// actualSize 为后段实际尺寸，standardSize 为标准尺寸。
        /// </summary>
        public BackSizeDeviationResult Update(double actualSize, double standardSize)
        {
            return UpdateBySizeError(actualSize - standardSize);
        }

        /// <summary>
        /// 如果外部已经算好尺寸偏差，也可以直接调用这个。
        /// sizeError = actualSize - standardSize。
        /// </summary>
        public BackSizeDeviationResult UpdateBySizeError(double sizeError)
        {
            lock (_syncRoot)
            {
                BackSizeDeviationResult result = new BackSizeDeviationResult();
                result.Accepted = false;
                result.Updated = false;
                result.RawSizeError = sizeError;
                result.CurrentDeviationCompensation = 0;
                result.Message = "未启用";

                CurrentDeviationCompensation = 0;

                if (!Enabled)
                {
                    return result;
                }

                if (double.IsNaN(sizeError) || double.IsInfinity(sizeError))
                {
                    result.Message = "尺寸偏差无效，xComp=0";
                    return result;
                }

                if (Math.Abs(sizeError) > Math.Abs(MaxSingleSizeError))
                {
                    result.Message = "尺寸偏差超过单点上限，已丢弃，xComp=0";
                    return result;
                }

                _sizeErrors.Enqueue(sizeError);
                result.Accepted = true;
                result.PendingSampleCount = _sizeErrors.Count;
                result.Message = "样本已接收，未凑满一组，xComp=0";

                int sampleCount = Math.Max(1, SampleCount);
                if (_sizeErrors.Count < sampleCount)
                {
                    return result;
                }

                double[] raw = _sizeErrors.ToArray();
                _sizeErrors.Clear();

                double[] filtered = RemoveOutliersByIqr(raw, IqrFactor);
                result.RejectedOutlierCount = raw.Length - filtered.Length;
                result.PendingSampleCount = 0;

                if (filtered.Length == 0)
                {
                    result.Message = "本组样本全部异常，xComp=0";
                    return result;
                }

                double averageError = filtered.Average();
                result.AverageSizeError = averageError;

                if (Math.Abs(averageError) <= Math.Abs(DeadBand))
                {
                    CurrentDeviationCompensation = 0;
                    result.Updated = true;
                    result.Step = 0;
                    result.CurrentDeviationCompensation = 0;
                    result.Message = "平均偏差在死区内，xComp=0";
                    return result;
                }

                double xComp = averageError * Gain;
                xComp = Clamp(xComp, -Math.Abs(MaxOutputCompensation), Math.Abs(MaxOutputCompensation));

                CurrentDeviationCompensation = xComp;
                result.Updated = true;
                result.Step = xComp;
                result.CurrentDeviationCompensation = xComp;
                result.Message = "xComp已输出";

                return result;
            }
        }

        /// <summary>
        /// 没有新尺寸时建议不要重复取旧值加到 SendDiff。
        /// 该方法只返回最近一次 Update 产生的 xComp；如果最近一次未更新或在死区内，则为 0。
        /// </summary>
        public double GetCurrentDeviationCompensation()
        {
            lock (_syncRoot)
            {
                return CurrentDeviationCompensation;
            }
        }

        public void Reset()
        {
            lock (_syncRoot)
            {
                _sizeErrors.Clear();
                CurrentDeviationCompensation = 0;
            }
        }

        private  double[] RemoveOutliersByIqr(double[] values, double factor)
        {
            if (values == null || values.Length == 0)
            {
                return new double[0];
            }

            double[] sorted = values.OrderBy(v => v).ToArray();

            if (sorted.Length < 4)
            {
                return sorted.ToArray();
            }

            if (sorted.Length < 6)
            {
                return sorted.Skip(1).Take(sorted.Length - 2).ToArray();
            }

            double q1 = Percentile(sorted, 0.25);
            double q3 = Percentile(sorted, 0.75);
            double iqr = q3 - q1;

            if (Math.Abs(iqr) < 0.000001)
            {
                return sorted.ToArray();
            }

            double lower = q1 - Math.Abs(factor) * iqr;
            double upper = q3 + Math.Abs(factor) * iqr;
            return sorted.Where(v => v >= lower && v <= upper).ToArray();
        }

        private  double Percentile(double[] sortedValues, double percentile)
        {
            double position = (sortedValues.Length - 1) * percentile;
            int left = (int)Math.Floor(position);
            int right = (int)Math.Ceiling(position);

            if (left == right)
            {
                return sortedValues[left];
            }

            double weight = position - left;
            return sortedValues[left] * (1 - weight) + sortedValues[right] * weight;
        }

        private  double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }
    }

    public struct BackSizeDeviationResult
    {
        public bool Accepted;
        public bool Updated;
        public double RawSizeError;
        public double AverageSizeError;
        public double Step;
        public double CurrentDeviationCompensation;
        public int PendingSampleCount;
        public int RejectedOutlierCount;
        public string Message;
    }
}
