using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 纠偏控制类
    /// </summary>
    public static class CorrectControl
    {
        /// <summary>
        /// 计算纠偏数据
        /// </summary>
        /// <param name="correctParam"></param>
        /// <param name="correcValue"></param>
        /// <param name="currentFrame"></param>
        /// <param name="imageLength"></param>
        /// <param name="IsSend_Correctdata"></param>
        /// <param name="Send_Correctdata"></param>
        public static void CalculateCorrectData(CorrectTool correctTool, CorrectParam correctParam, double correcValue, int currentFrame, double imageLength, out bool IsSend_Correctdata, out double Send_Correctdata)
        {
            IsSend_Correctdata = false;
            Send_Correctdata = 0;

            try
            {
                if (!correctParam.ENAutoJP) return;

                //换卷N米后自动纠偏
                if (correctParam.EnHJJP)
                {
                    bool result = correctTool.HJJPJudge(currentFrame, correctParam.JP_HJLength * 1000, imageLength);
                    if (!result) return;
                }

                //纠偏数据超出最大值报警判断
                if (correctParam.JP_Max_errEn)
                {
                    if (correctTool.JPMaxErrJudge(correcValue, correctParam.JP_MaxValue, correctParam.JP_Max_errNUM))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.纠偏超最大值报警个数, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机 });                     
                    }
                }
               

                //纠偏数据采样收集,判断纠偏量是否发送    
                int collectionNum = Convert.ToInt32(correctParam.Correct_distance / imageLength);
                correctTool.CorrectDataCollection(correcValue, correctParam.JP_MinValue, correctParam.JP_MaxValue, correctParam.JP_Pro, collectionNum, out IsSend_Correctdata, out Send_Correctdata);
                if (!IsSend_Correctdata) return;

                //纠偏数据符号取反
                if (correctParam.JP_Sign) Send_Correctdata = -Send_Correctdata;
                Send_Correctdata = Convert.ToDouble(Send_Correctdata.ToString("F2"));

                //纠偏数据连续同符号报警判断
                if (correctParam.Sign_LY_En)
                {
                    if (correctTool.JPSameErrJudge(Send_Correctdata, correctParam.Sign_LX_Num))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.纠偏量超同符号报警个数, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机 });
                    }
                }
               
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }
    }
}
