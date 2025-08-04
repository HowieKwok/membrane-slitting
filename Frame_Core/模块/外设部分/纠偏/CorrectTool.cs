using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static remoteObj.RemotingObject;

namespace Frame_Core
{
    /// <summary>
    ///纠偏工具类 
    ///作者： pjq
    ///日期： 2024-07-02
    ///描述：
    /// </summary>
    public class CorrectTool
    {
        int JP_Max_err_Count = 0;//纠偏超最大值计数
        HTuple JP_Data = null;//分切纠偏采集数据
        int JP_Data_Num = 0;//当前采样数量
        bool bool_Correctdata_tmp = true;//发送一次纠编数值后要等待取样距离走过了之后在收取纠偏数据  
        string upCorrectSign = "";//上一次纠偏量符号
        int continueSign_Num = 0;//连续相同符号数量
        /// <summary>
        /// 换卷N米后自动纠偏判断，如果当前帧数大于等于换卷帧数，则启用自动纠偏
        /// </summary>
        /// <param name="currentFrame">当前帧数</param>
        /// <param name="hjDistance">换卷距离(单位：mm)</param>
        /// <param name="imageLength">图片长度(单位：mm)</param>
        /// <returns>true为达到换卷距离</returns>
        public bool HJJPJudge(int currentFrame, double hjDistance, double imageLength)
        {
            bool result = false;

            if (imageLength <= 0) imageLength = 1;
            int hjNum = Convert.ToInt32(hjDistance / imageLength);
            if (currentFrame > hjNum)
            {
                result = true;
            }
            return result;
        }


        public bool HJJPJudge(int currentFrame, int hjNum)
        {
            bool result = false;

            if (currentFrame > hjNum)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 纠偏数据超出最大值报警判断
        /// </summary>
        /// <param name="correcValue">纠偏值</param>
        /// <param name="jp_MaxValue">纠偏最大值</param>
        /// <param name="jp_Max_errNUM">纠偏最大报警数量</param>
        /// <returns>true为报警</returns>
        public bool JPMaxErrJudge(double correcValue, double jp_MaxValue, int jp_Max_errNUM)
        {
            bool result = false;
            if (jp_Max_errNUM > 0)
            {
                if (Math.Abs(correcValue) > jp_MaxValue)
                {
                    JP_Max_err_Count++;
                    if (JP_Max_err_Count > jp_Max_errNUM)
                    {
                        result = true;
                        JP_Max_err_Count = 0;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 纠偏数据连续同符号报警判断
        /// </summary>
        /// <param name="send_Correctdata">发送的纠偏值</param>
        /// <param name="JPSameSign_errNum">纠偏同符号报警数量</param>
        /// <returns></returns>
        public bool JPSameErrJudge(double send_Correctdata,int JPSameSign_errNum)
        {
            bool result = false;
            if (JPSameSign_errNum > 0)//连续纠偏量同符号报警个数>0才执行
            {
                string currentCorrectSign;
                if (send_Correctdata > 0) currentCorrectSign = "+";
                else currentCorrectSign = "-";


                if (currentCorrectSign == upCorrectSign) continueSign_Num++;
                else continueSign_Num = 0;

                if (send_Correctdata > 0) upCorrectSign = "+";
                else upCorrectSign = "-";

                if (continueSign_Num > JPSameSign_errNum)
                {
                    result = true;
                    continueSign_Num = 0;
                
                }
            }
            return result;
        }

        /// <summary>
        /// 纠偏数据采样收集
        /// </summary>
        /// <param name="correcValue">纠偏值</param>
        /// <param name="jp_MinValue">纠偏最小值</param>
        /// <param name="jp_MaxValue">纠偏最大值</param>
        /// <param name="jp_Proportion">纠偏比例系数</param>
        /// <param name="collectionNum">采样数量</param>
        /// <param name="isSend_Correctdata">是否发送纠偏值</param>
        /// <param name="send_Correctdata">发送的纠偏值</param>
        public void CorrectDataCollection(double correcValue, double jp_MinValue, double jp_MaxValue, double jp_Proportion, int collectionNum, out bool isSend_Correctdata, out double send_Correctdata)
        {
            isSend_Correctdata = false;
            send_Correctdata = 0;

            //==============================================================================================================================================
            //=======【】采集数据限制
            if (JP_Data == null) JP_Data = new HTuple();
            if (Math.Abs(correcValue) < jp_MaxValue)
            {
                
                if (JP_Data.Length == 0)
                {
                    JP_Data = correcValue;
                }
                else
                {
                    JP_Data = JP_Data.TupleConcat(correcValue);
                }
            }

            //==============================================================================================================================================
            //=======【】
            JP_Data_Num++;//执行次数
            if (JP_Data_Num > collectionNum)
            {
                JP_Data_Num = 0;
                if (bool_Correctdata_tmp)
                {
                    if (JP_Data.Length > 3)
                    {
                        //===============================================================================================================
                        //=======【】     
                        send_Correctdata = Struggle_Mean_data_new(JP_Data) * jp_Proportion;//纠偏发送量 = 纠编值平均数 * 纠偏系数    

                        //===============================================================================================================
                        //======【】
                        if (send_Correctdata > jp_MaxValue) send_Correctdata = jp_MaxValue / 2; //当纠偏发送量大于纠偏最大值时，纠偏发送量 = 纠偏最大值一半
                        if (send_Correctdata < -jp_MaxValue) send_Correctdata = -jp_MaxValue / 2; //当纠偏发送量大于纠偏最大值时，纠偏发送量 = 纠偏最大值一半

                        if (jp_MinValue < 0.05)
                            jp_MinValue = 0.05;

                        if (Math.Abs(send_Correctdata) > jp_MinValue && Math.Abs(send_Correctdata) < jp_MaxValue)//大于最小值且小于最大值才发生纠编数值
                        {
                            isSend_Correctdata = true;
                            bool_Correctdata_tmp = false;
                        }
                    }
                }
                else
                {
                    bool_Correctdata_tmp = true;
                }

                //==============================================================================================================================================
                //=======【】清空纠偏缓存数据集合
                JP_Data = new HTuple();
            }
        }


        /// <summary>
        /// 纠偏采样数据 去噪 滤波
        /// </summary>
        /// <param name="CorrectMeandata"></param>
        /// <returns></returns>
        private HTuple Struggle_Mean_data(HTuple CorrectMeandata)
        {
            HOperatorSet.TupleSort(CorrectMeandata, out CorrectMeandata);
            HOperatorSet.TupleRemove(CorrectMeandata, 0, out CorrectMeandata);
            HOperatorSet.TupleRemove(CorrectMeandata, CorrectMeandata.Length - 1, out CorrectMeandata);
            HOperatorSet.TupleMean(CorrectMeandata, out CorrectMeandata);
            return CorrectMeandata;
        }
        private double Struggle_Mean_data_new(HTuple CorrectMeandata)
        {
            double SendVal = 0;
            HOperatorSet.TupleSort(CorrectMeandata, out CorrectMeandata);
            HOperatorSet.TupleRemove(CorrectMeandata, 0, out CorrectMeandata);
            HOperatorSet.TupleRemove(CorrectMeandata, CorrectMeandata.Length - 1, out CorrectMeandata);

            double sigma = CalculateStdDev(CorrectMeandata);
            double avg = CorrectMeandata.TupleMean();
            HTuple Sigma_Data = null;
            for (int i = 0; i < CorrectMeandata.Length; i++)
            {
                if ((CorrectMeandata.TupleSelect(i) - avg).TupleAbs() <= sigma * 2)
                {
                    if (Sigma_Data == null)
                        Sigma_Data = CorrectMeandata.TupleSelect(i);
                    else
                        Sigma_Data = Sigma_Data.TupleConcat(CorrectMeandata.TupleSelect(i));
                }
            }
            if (Sigma_Data != null)
            {
                if (Sigma_Data.Length > 0)
                {
                    sigma = CalculateStdDev(Sigma_Data);
                    //=============================================================================================================
                    //======【】数据稳定时
                    if (sigma < 0.2)
                    {
                        SendVal = Sigma_Data.TupleMean();
                    }
                }
            }
            return SendVal;
        }

        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="CorrectMeandata"></param>
        /// <returns></returns>
        private double CalculateStdDev(HTuple CorrectMeandata)
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
    }
}
