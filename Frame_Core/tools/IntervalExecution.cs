using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 延时执行流程
    /// </summary>
    /// <returns></returns>
    public delegate ProcessExecutionResult DelayExecuteProcess();

    /// <summary>
    /// 延时执行流程
    /// </summary>
    /// <returns></returns>
    public delegate ProcessExecutionResult DelayExecuteProductBase(ProductBase productBase);

    /// <summary>
    /// 间隔执行
    /// </summary>
    public class IntervalExecution
    {
        DateTime LastTime = DateTime.Now;

        /// <summary>
        /// 获取是否达到间隔时间
        /// </summary>
        /// <param name="intervalTime">间隔时间（单位：ms）</param>
        /// <returns></returns>
        public bool GetWhetherToReachIntervalTime(int intervalTime)
        {
            DateTime date1 = DateTime.Now;
            if (date1.Subtract(LastTime).TotalMilliseconds > intervalTime)
            {
                LastTime = date1;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获取是否达到间隔时间
        /// </summary>
        /// <param name="intervalTime">间隔时间（单位：ms）</param>
        /// <returns></returns>
        public bool GetWhetherToReachIntervalTime_Heartbeat(int intervalTime)
        {
            DateTime date1 = DateTime.Now;
            if (date1.Subtract(LastTime).TotalMilliseconds > intervalTime)
            {              
                return true;
            }
            return false;
        }

        public  void Set_DateTime()
        {
            LastTime = DateTime.Now;
        }


        /// <summary>
        /// 超过延时时间后返回
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="delayTime">延时时间(单位：ms)</param>
        public static ProcessExecutionResult DelayExecute(DateTime startTime, int delayTime,ProductBase productBase, DelayExecuteProductBase delayExecuteProcess)
        {
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < delayTime)
            {
                //判断是否开始执行
                ProcessExecutionResult result = delayExecuteProcess(productBase);

                if (result == ProcessExecutionResult.Success || result == ProcessExecutionResult.Exception)
                {
                    //执行成功或失败返回
                    return result;
                }

            }
            return ProcessExecutionResult.Overtime;
        }

        /// <summary>
        /// 超过延时时间后返回
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="delayTime">延时时间(单位：ms)</param>
        public static ProcessExecutionResult DelayExecute(DateTime startTime, int delayTime, DelayExecuteProcess delayExecuteProcess)
        {
            while (DateTime.Now.Subtract(startTime).TotalMilliseconds < delayTime)
            {
                //判断是否开始执行
                ProcessExecutionResult result = delayExecuteProcess();

                if (result == ProcessExecutionResult.Success || result == ProcessExecutionResult.Exception)
                {
                    //执行成功或失败返回
                    return result;
                }

            }
            return ProcessExecutionResult.Overtime;
        }

    }

    public enum ProcessExecutionResult
    {
        /// <summary>
        /// 异常
        /// </summary>
        Exception,
        /// <summary>
        /// 执行成功
        /// </summary>
        Success,
        /// <summary>
        /// 超时
        /// </summary>
        Overtime,
        /// <summary>
        /// 继续执行
        /// </summary>
        ContinueExecute,

    }
}
