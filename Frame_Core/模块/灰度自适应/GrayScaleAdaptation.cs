using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 进行灰度调节
    /// </summary>
    /// <param name="val_RegulatingVariable"></param>
    /// <returns></returns>
    public delegate ExecuteResult PerformGrayscaleAdjustmentDelegate(CameraEnum cameraEnum, int val_CurrentValue, int val_TargetValue);


    /// <summary>
    /// 灰度自适应算法
    /// 20250912
    /// pjq
    /// </summary>
    public static class GrayScaleAdaptation
    {


        /// <summary>
        /// 条件1：当前帧与上一调节帧间隔大于等于间隔帧数
        /// </summary>
        /// <param name="currentFrame"></param>
        /// <param name="previousFrame"></param>
        /// <param name="intervalFrame"></param>
        /// <returns></returns>
        private static bool CalculateGrayAdjustment_Condition1(long currentFrame, long previousFrame, int intervalFrame)
        {
            return (currentFrame - previousFrame) >= intervalFrame;
        }

        /// <summary>
        /// 条件2：当前灰度值在指定范围内
        /// </summary>
        /// <param name="grayRangeMin"></param>
        /// <param name="grayRangeMax"></param>
        /// <param name="currentGray"></param>
        /// <returns></returns>
        private static bool CalculateGrayAdjustment_Condition2(int grayRangeMin, int grayRangeMax, double currentGray)
        {
            return grayRangeMin <= currentGray && currentGray <= grayRangeMax;
        }

        /// <summary>
        /// 条件3：超出目标灰度公差值
        /// </summary>
        /// <param name="currentGray"></param>
        /// <param name="targetGray"></param>
        /// <param name="adjustmentGrayTolerance"></param>
        /// <returns></returns>
        private static bool CalculateGrayAdjustment_Condition3(double currentGray, double targetGray, int adjustmentGrayTolerance)
        {
            return Math.Abs(currentGray - targetGray) > adjustmentGrayTolerance;
        }

        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="CorrectMeandata"></param>
        /// <returns></returns>
        private static double CalculateStdDev(HTuple CorrectMeandata)
        {
            double standardDeviation = 0;
            //double[] input = CorrectMeandata.ToDArr();
            double[] input = new double[CorrectMeandata.Length];
            for (int i = 0; i < CorrectMeandata.Length; i++)
            {
                input[i] = CorrectMeandata[i];
            }
            if (input.Length > 0)
            {
                //  计算平均数   
                double avg = input.Average();
                //  计算各数值与平均数的差值的平方，然后求和 
                double sum = input.Sum(d => Math.Pow(d - avg, 2));
                //  除以数量，然后开方
                standardDeviation = Math.Sqrt(sum / input.Length);
            }
            return standardDeviation;
        }
        private static HTuple Insert_HTuple(HTuple iTuButubuMean, double iInsertdata, int iInsertNum)
        {
            if (iTuButubuMean.Length == 0) iTuButubuMean = iInsertdata;
            else iTuButubuMean = iTuButubuMean.TupleConcat(iInsertdata);
            while (iTuButubuMean.Length > iInsertNum)
            {
                iTuButubuMean = iTuButubuMean.TupleRemove(0);
            }
            return iTuButubuMean;
        }

        /// <summary>
        /// 计算灰度调节
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="currentFrame"></param>
        /// <param name="result">不存在涂布缺陷结果为true</param>
        /// <param name="currentGrayValid">当前灰度是否有效</param>
        /// <param name="currentGray">当前灰度</param>
        /// <param name="grayScaleAdaptationParameter">自适应参数</param>
        /// <param name="performGrayscaleAdjustment">调节委托</param>
        /// <returns></returns>
        public static ExecuteResult CalculateGrayAdjustment(CameraEnum cameraEnum, long currentFrame, bool result, bool currentGrayValid, double currentGray, GrayScaleAdaptationParameter grayScaleAdaptationParameter, PerformGrayscaleAdjustmentDelegate performGrayscaleAdjustment)
        {
            ExecuteResult executeResult = new ExecuteResult();
            executeResult.IsSuccess = true;
            HTuple start = null, end = null;
            HOperatorSet.CountSeconds(out start);
            try
            {
                //===================================================================================================================
                //======【】记录复位第一次的灰度状态
                if (grayScaleAdaptationParameter.Temp_StartFlag)
                {
                    grayScaleAdaptationParameter.Temp_StartFlag = false;
                    List<string> GrayStatus = new List<string>();
                    GrayStatus.Add(cameraEnum.ToString());
                    GrayStatus.Add("[复位后第一次信息状态]");
                    GrayStatus.Add("[当前EA]" + currentFrame);
                    GrayStatus.Add("[是否启用]" + grayScaleAdaptationParameter.Enable);
                    GrayStatus.Add("[调节灰度范围]" + grayScaleAdaptationParameter.GrayRangeMin + "-" + grayScaleAdaptationParameter.GrayRangeMax);
                    GrayStatus.Add("[自适应间隔调整帧数]" + grayScaleAdaptationParameter.IntervalFrame);
                    GrayStatus.Add("[单次调节比例]" + grayScaleAdaptationParameter.SingleAdjustmentRatio);
                    GrayStatus.Add("[调节系数]" + grayScaleAdaptationParameter.AdjustFactor);
                    GrayStatus.Add("[目标灰度]" + grayScaleAdaptationParameter.TargetGray);
                    GrayStatus.Add("[调节灰度公差]" + grayScaleAdaptationParameter.AdjustmentGrayTolerance);
                    GrayStatus.Add("[目标灰度容差]" + grayScaleAdaptationParameter.TargetGrayTolerance);
                    GrayStatus.Add("[连续N帧图像涂布平均灰度超出目标灰度范围，则进行闭环调节光源亮度]" + grayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGray);
                    GrayStatus.Add("[连续N帧图像涂布平均灰度超出目标灰度范围报警停机]" + grayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGrayAlarm);
                    GrayStatus.Add("[灰度波动大于】" + grayScaleAdaptationParameter.GrayBodong);
                    GrayStatus.Add("[灰度波动报警停机]" + grayScaleAdaptationParameter.GrayBodongAlarm);
                    GrayStatus.Add("[N次未调节完成数量]" + grayScaleAdaptationParameter.AdjustFinishNum);
                    GrayStatus.Add("[N次未调节完成报警]" + grayScaleAdaptationParameter.NNotAdjustFinishAlarm);
                    GrayStatus.Add("[当前灰度]" + currentGray);
                    LogWorks.GrayScaleAdaptationLog.Debug(string.Join(",", GrayStatus.ToArray()));
                }
                if (!grayScaleAdaptationParameter.Enable) return executeResult;// 灰度自适应未启用，直接返回
                if (!currentGrayValid) return executeResult;//当前灰度无效，直接返回

                //===================================================================================================================
                //======【】灰度波动判断报警
                if (result && grayScaleAdaptationParameter.GrayBodongAlarm)
                {
                    grayScaleAdaptationParameter.Temp_OKGrayQueue.Enqueue(currentGray, 5);
                    List<double> dTuBuMeanGray = grayScaleAdaptationParameter.Temp_OKGrayQueue.ToList();
                    if (dTuBuMeanGray.Count >= 2)
                    {
                        double dev = Math.Abs(dTuBuMeanGray[dTuBuMeanGray.Count - 1] - dTuBuMeanGray[dTuBuMeanGray.Count - 2]);
                        if (dev > grayScaleAdaptationParameter.GrayBodong)
                        {
                            TJBJInfo.AlarmWriteState(AlarmName.灰度波动报警, cameraEnum.ToString(), true, $"EA_{currentFrame},偏差{dev},允许波动{grayScaleAdaptationParameter.GrayBodong}");
                        }
                    }
                }

                //==================================================================================================================================
                //====== 【】计算涂布灰度变化规律，预判自适应是否启动
                bool isNowAdjust_NG = false;
                int iTestDataNum = 8;
                double iSigmaData = -100;
                double iMeanData = -100;
                grayScaleAdaptationParameter.Temp_ContinuousGrayQueue = Insert_HTuple(grayScaleAdaptationParameter.Temp_ContinuousGrayQueue, currentGray, iTestDataNum);
                if (grayScaleAdaptationParameter.Temp_ContinuousGrayQueue.Length >= iTestDataNum)
                {
                    iMeanData = grayScaleAdaptationParameter.Temp_ContinuousGrayQueue.TupleMean();
                    iSigmaData = CalculateStdDev(grayScaleAdaptationParameter.Temp_ContinuousGrayQueue) + 0.001;
                    if (iSigmaData > 8) isNowAdjust_NG = true;

                    HTuple piancha = iMeanData - currentGray;
                    if (piancha.TupleAbs() > iSigmaData * 4)
                        isNowAdjust_NG = true;
                }
                if (isNowAdjust_NG) return executeResult;

                //==================================================================================================================================
                //====== 【】光源自适应调节灰度最值范围
                bool isEn_LightGrayRange = CalculateGrayAdjustment_Condition2(grayScaleAdaptationParameter.GrayRangeMin, grayScaleAdaptationParameter.GrayRangeMax, currentGray);

                //==================================================================================================================================
                //====== 【】 连续n帧图像涂布平均灰度与目标灰度偏差 LightGrayRange 以上，则进行闭环调节光源亮度
                bool isEn_outOfTolerance = CalculateGrayAdjustment_Condition3(currentGray, grayScaleAdaptationParameter.TargetGray, grayScaleAdaptationParameter.AdjustmentGrayTolerance);
                if (isEn_outOfTolerance && isEn_LightGrayRange)
                {
                    grayScaleAdaptationParameter.Temp_OutOfScopeGrayCount++;
                    grayScaleAdaptationParameter.Temp_GaryNormalCount = 0;
                    if (grayScaleAdaptationParameter.Temp_OutOfScopeGrayCount > grayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGray)
                    {
                        if (grayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGrayAlarm)
                        {
                            TJBJInfo.AlarmWriteState(AlarmName.连续灰度波动超出范围报警, cameraEnum.ToString(), true, $"EA_{currentFrame},连续超出范围计数{grayScaleAdaptationParameter.Temp_OutOfScopeGrayCount},连续数量{grayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGray}");
                        }
                        grayScaleAdaptationParameter.Temp_OutOfScopeGrayCount = 0;
                        grayScaleAdaptationParameter.Temp_IsAdjust = true;
                        LogWorks.GrayScaleAdaptationLog.Debug($"{cameraEnum}:当前EA_{currentFrame}连续{grayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGray}帧图像涂布平均灰度超出目标灰度范围，开始闭环调节光源亮度");
                    }

                }
                else
                {
                    grayScaleAdaptationParameter.Temp_OutOfScopeGrayCount = 0;
                    grayScaleAdaptationParameter.Temp_GaryNormalCount++;
                }

                //==================================================================================================================================
                //====== 【】启动调节光源
                if (grayScaleAdaptationParameter.Temp_IsAdjust)
                {
                    grayScaleAdaptationParameter.Temp_AdjustCount++;
                    //==================================================================================================================================
                    //====== 【】分切光源调节失败判断
                    if (grayScaleAdaptationParameter.NNotAdjustFinishAlarm)
                    {
                        if (grayScaleAdaptationParameter.Temp_AdjustCount == grayScaleAdaptationParameter.AdjustFinishNum * grayScaleAdaptationParameter.IntervalFrame)
                        {
                            TJBJInfo.AlarmWriteState(AlarmName.灰度调节失败报警, cameraEnum.ToString(), true, $"EA_{currentFrame},调节次数{grayScaleAdaptationParameter.Temp_AdjustCount},最大次数{grayScaleAdaptationParameter.AdjustFinishNum},间隔{grayScaleAdaptationParameter.IntervalFrame}");
                            grayScaleAdaptationParameter.Temp_OutOfScopeGrayCount = 0;
                            LogWorks.GrayScaleAdaptationLog.Debug($"{cameraEnum}：当前EA_{currentFrame},灰度调节失败");
                        }
                    }

                    //==================================================================================================================================
                    //====== 【】计算光源调节量
                    bool IsAdjust = false;
                    double val_AdjustGrayscale = 0;
                    int val_RegulatingVariable = 0;
                    int val_TargetValue = 0;
                    bool IntervalCondition = CalculateGrayAdjustment_Condition1(currentFrame, grayScaleAdaptationParameter.Temp_PreviousFrame, grayScaleAdaptationParameter.IntervalFrame);
                    if (IntervalCondition)
                    {
                        grayScaleAdaptationParameter.Temp_PreviousFrame = currentFrame;

          
                        if (currentGray > (grayScaleAdaptationParameter.TargetGray + grayScaleAdaptationParameter.TargetGrayTolerance) ||
                            currentGray < (grayScaleAdaptationParameter.TargetGray - grayScaleAdaptationParameter.TargetGrayTolerance))//10  目标灰度±LightGrayDist
                        {
                            val_AdjustGrayscale = grayScaleAdaptationParameter.TargetGray - currentGray;
                            val_AdjustGrayscale = val_AdjustGrayscale * grayScaleAdaptationParameter.SingleAdjustmentRatio;//0.5
                            val_AdjustGrayscale = Math.Round(val_AdjustGrayscale, 2);
                            val_RegulatingVariable = (int)(val_AdjustGrayscale * grayScaleAdaptationParameter.AdjustFactor);
                            IsAdjust = true;

                            val_TargetValue = (grayScaleAdaptationParameter.Temp_CurrentLightValue + val_RegulatingVariable);

                            ExecuteResult AdjustmentExecuteResult = performGrayscaleAdjustment?.Invoke(cameraEnum, grayScaleAdaptationParameter.Temp_CurrentLightValue, val_TargetValue);
                            if (AdjustmentExecuteResult != null) LogWorks.GrayScaleAdaptationLog.Debug($"{cameraEnum}：当前EA_{currentFrame},调节结果：{AdjustmentExecuteResult.IsSuccess},调节信息：{AdjustmentExecuteResult.Message},耗时{AdjustmentExecuteResult.Time}ms");
                            else LogWorks.GrayScaleAdaptationLog.Debug($"{cameraEnum}：无调节执行事件");
                        }
                        else
                        {
                            IsAdjust = false;
                        }

                        List<string> GrayStatus = new List<string>();
                        GrayStatus.Add(cameraEnum.ToString());
                        GrayStatus.Add("[当前EA]" + currentFrame);
                        GrayStatus.Add("[是否调节]" + IsAdjust);
                        GrayStatus.Add("[当前灰度]" + currentGray);
                        GrayStatus.Add("[目标灰度]" + grayScaleAdaptationParameter.TargetGray);
                        GrayStatus.Add("[目标灰度容差]" + grayScaleAdaptationParameter.TargetGrayTolerance);
                        GrayStatus.Add("[单次调节比例]" + grayScaleAdaptationParameter.SingleAdjustmentRatio);
                        GrayStatus.Add("[调节系数]" + grayScaleAdaptationParameter.AdjustFactor);
                        GrayStatus.Add("[调节灰度]" + val_AdjustGrayscale);
                        GrayStatus.Add("[调节量]" + val_RegulatingVariable);
                        GrayStatus.Add("[当前光源值]" + grayScaleAdaptationParameter.Temp_CurrentLightValue);
                        GrayStatus.Add("[调整后光源值]" + val_TargetValue);
                        LogWorks.GrayScaleAdaptationLog.Debug(string.Join(",", GrayStatus.ToArray()));


                        //==================================================================================================================================
                        //====== 【】调节成功判断
                        if (!IsAdjust) grayScaleAdaptationParameter.Temp_AdjustOKCount++;
                        else grayScaleAdaptationParameter.Temp_AdjustOKCount = 0;

                        if (grayScaleAdaptationParameter.Temp_AdjustOKCount == 5)
                        {
                            grayScaleAdaptationParameter.Temp_AdjustOKCount = 0;
                            grayScaleAdaptationParameter.Temp_AdjustCount = 0;
                            grayScaleAdaptationParameter.Temp_IsAdjust = false;
                            LogWorks.GrayScaleAdaptationLog.Debug($"{cameraEnum}：当前EA_{currentFrame},灰度调节成功");
                        }
                    }
                }
                else
                {
                    grayScaleAdaptationParameter.Temp_AdjustCount = 0;
                }

                //==================================================================================================================================
                //====== 【】灰度正常
                if (grayScaleAdaptationParameter.Temp_GaryNormalCount > 10)
                {
                    grayScaleAdaptationParameter.Temp_GaryNormalCount = 0;
                }
            }
            catch (Exception ex)
            {
                executeResult.Message = ex.Message;
                executeResult.IsSuccess = false;
                Log4netRecord.ErrorLog.Error(ex);
            }
            finally
            {
                HOperatorSet.CountSeconds(out end);
                executeResult.Time = ((end - start) * 1000).D;
                executeResult.Time = Math.Round(executeResult.Time, 2);
                if (executeResult.Time > 0.1) LogWorks.GrayScaleAdaptationLog.Debug($"{cameraEnum}：当前EA_{currentFrame},调节总耗时{executeResult.Time}ms");
            }
            return executeResult;
        }
    }

}
