using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LogTool;

namespace Frame_Core
{
    public class Thick3DManage
    {
        public Param_Thick3DSet correctParam = new Param_Thick3DSet();

        public CorrectTool correctTool = new CorrectTool();
        public Action<int,float> SendData;


        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitParam(string path)
        {
            try
            {
                if (correctParam == null) correctParam = new Param_Thick3DSet();
                correctParam.filePath = path;
                correctParam = (Param_Thick3DSet)correctParam.Read();
                correctParam.filePath = path;

            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }

            return true;

        }


        public bool WriteCorrectValue(double Correctvalue, int collectionNum, int currentFrame, double imgHeight, out double DealCorrvalue)
        {
            DealCorrvalue = 0; // 默认值
            try
            {
                if (!correctParam.EnHJJP || correctTool.HJJPJudge(currentFrame, correctParam.JP_HJLength, imgHeight))
                {
                    return GetcorrectValue(Correctvalue, collectionNum, out DealCorrvalue);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug(ee);
                return false;
            }

        }

        public bool WriteCorrectValue(double Correctvalue, int currentFrame, out double DealCorrvalue)
        {
            DealCorrvalue = 0; // 默认值
            try
            {
                if (!correctParam.EnHJJP || correctTool.HJJPJudge(currentFrame,(int)correctParam.JP_HJLength))
                {
                    return GetcorrectValue(Correctvalue, correctParam.Correct_distance, out DealCorrvalue);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug(ee);
                return false;
            }

        }




        public bool GetcorrectValue(double Correctvalue, int collectionNum, out double Dealvalue)
        {
            Dealvalue = 0; // 默认值
            try
            {
                correctTool.CorrectDataCollection(Correctvalue, correctParam.JP_MinValue, correctParam.JP_MaxValue, correctParam.JP_Pro, collectionNum, out bool sendflag, out double value);
                if (sendflag)
                {
                    if (correctParam.JP_Sign) value = -value;
                    if (JPMaxErrJudge(value))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.纠偏超最大值报警个数, StopAlarmStateEnum.停机, $"当前纠偏值为{value}");
                    }

                    if (JPSameErrJudge(value))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.纠偏量超同符号报警个数, StopAlarmStateEnum.停机, $"当前纠偏值为{value}");
                    }
                }
                Dealvalue = value;
                return sendflag;
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug(ee);
                return false;
            }
        }

        public bool JPSameErrJudge(double sendData)
        {
            try
            {
                return correctParam.Sign_LY_En && correctTool.JPSameErrJudge(sendData, correctParam.Sign_LX_Num);
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug(ee);
                return false;
            }
        }


        public bool JPMaxErrJudge(double sendData)
        {
            try
            {
                return correctParam.JP_Max_errEn && correctTool.JPMaxErrJudge(sendData, correctParam.JP_MaxValue, correctParam.JP_Max_errNUM);
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug(ee);
                return false;
                // throw;
            }
        }
    }
}
