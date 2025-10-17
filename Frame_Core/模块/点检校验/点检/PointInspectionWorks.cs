using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewROI3;

namespace Frame_Core
{
    public static class PointInspectionWorks
    {
        public static FileOperationINI FileOperationINI = new FileOperationINI();

        /// <summary>
        /// 初始化点检功能
        /// </summary>
        /// <param name="cameraManager"></param>
        public static void InitPointInspectionFunction(Dictionary<CameraEnum, CameraManager> cameraManager)
        {
            cameraManager[CameraEnum.Cam1].m_PointInspectionStateParam.PointInspectionState_Defect = (int)PointInspectionStateEnum.已忽略;
            cameraManager[CameraEnum.Cam2].m_PointInspectionStateParam.PointInspectionState_Defect = (int)PointInspectionStateEnum.已忽略;

            Project.Instance.systemInfo.InitFinishEN = true;
        }

        /// <summary>
        /// 点检提示
        /// </summary>
        public static void PointInspectionHint()
        {
            try
            {
                if (!Project.Instance.systemInfo.InitFinishEN) return;
                if (Project.Instance.systemInfo.djEnabled)
                {
                    DateTime currentTime = DateTime.Now;
                    TimeSpan currentTimePart = currentTime.TimeOfDay;

                    TimeSpan BaibanTimePart = Project.Instance.systemInfo.djCaliTimeBaiban.TimeOfDay;
                    TimeSpan YeBanTimePart = Project.Instance.systemInfo.djCaliTimeYeBan.TimeOfDay;

                    //进入白班时间段
                    if (currentTimePart.Subtract(BaibanTimePart).TotalSeconds > 0 && currentTimePart.Subtract(BaibanTimePart).TotalHours < 1)
                    {
                        //复位点检完成标志
                        foreach (var item in Project.Instance.cameraManager)
                        {
                            ///上一次点检时间超过1小时，进入白班或夜班时间段判断是否需要点检
                            DateTime lastOKTime_Size = item.Value.m_PointInspectionStateParam.djLastOKTime_Size;
                            if (currentTime.Subtract(lastOKTime_Size).TotalHours >= 1)
                            {
                                if (item.Value.m_PointInspectionStateParam.PointInspectionState_Size != (int)PointInspectionStateEnum.已忽略)
                                    item.Value.m_PointInspectionStateParam.PointInspectionState_Size = (int)PointInspectionStateEnum.未点检;
                            }

                            ///上一次点检时间超过1小时，进入白班或夜班时间段判断是否需要点检
                            DateTime lastOKTime_Defect = item.Value.m_PointInspectionStateParam.djLastOKTime_Defect;
                            if (currentTime.Subtract(lastOKTime_Defect).TotalHours >= 1)
                            {
                                if (item.Value.m_PointInspectionStateParam.PointInspectionState_Defect != (int)PointInspectionStateEnum.已忽略)
                                    item.Value.m_PointInspectionStateParam.PointInspectionState_Defect = (int)PointInspectionStateEnum.未点检;
                            }
                        }
                    }
                    //进入夜班时间段
                    else if (currentTimePart.Subtract(YeBanTimePart).TotalSeconds > 0 && currentTimePart.Subtract(YeBanTimePart).TotalHours < 1)
                    {
                        //复位点检完成标志
                        foreach (var item in Project.Instance.cameraManager)
                        {
                            ///上一次点检时间超过1小时，进入白班或夜班时间段判断是否需要点检
                            DateTime lastOKTime_Size = item.Value.m_PointInspectionStateParam.djLastOKTime_Size;
                            if (currentTime.Subtract(lastOKTime_Size).TotalHours >= 1)
                            {
                                if (item.Value.m_PointInspectionStateParam.PointInspectionState_Size != (int)PointInspectionStateEnum.已忽略)
                                    item.Value.m_PointInspectionStateParam.PointInspectionState_Size = (int)PointInspectionStateEnum.未点检;
                            }

                            ///上一次点检时间超过1小时，进入白班或夜班时间段判断是否需要点检
                            DateTime lastOKTime_Defect = item.Value.m_PointInspectionStateParam.djLastOKTime_Defect;
                            if (currentTime.Subtract(lastOKTime_Defect).TotalHours >= 1)
                            {
                                if (item.Value.m_PointInspectionStateParam.PointInspectionState_Defect != (int)PointInspectionStateEnum.已忽略)
                                    item.Value.m_PointInspectionStateParam.PointInspectionState_Defect = (int)PointInspectionStateEnum.未点检;
                            }
                        }
                    }


                    //1分钟内不重复报警
                    if (true/*intervalTimeExecute.GetWhetherToReachIntervalTime(60 * 1000)*/)
                    {
                        if (!GetSizePointInspectionResult())
                        {
                            //点检提示
                            TJBJInfo.AlarmWriteState(AlarmName.已到点检时间请进行尺寸点检操作, StopAlarmStateEnum.报警, "");                         
                        }

                        if (!GetDefectPointInspectionResult())
                        {
                            //点检提示
                            TJBJInfo.AlarmWriteState(AlarmName.已到点检时间请进行缺陷点检操作, StopAlarmStateEnum.报警, "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 获取点检结果
        /// </summary>
        /// <returns></returns>
        public static bool GetPointInspectionResult()
        {
            try
            {
                ////判断是否点检完成
                if (Project.Instance.systemInfo.djEnabled)
                {
                    return GetSizePointInspectionResult() && GetDefectPointInspectionResult();
                }
                //未启用返回true
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// 获取尺寸点检结果
        /// </summary>
        /// <returns></returns>
        public static bool GetSizePointInspectionResult()
        {
            try
            {
              
                bool result = true;
                foreach (var item in Project.Instance.cameraManager)
                {
                    bool b = true;
                    if ((item.Value.m_PointInspectionStateParam.PointInspectionState_Size == (int)PointInspectionStateEnum.已忽略) ||
                        (item.Value.m_PointInspectionStateParam.PointInspectionState_Size == (int)PointInspectionStateEnum.点检成功))
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                    result &= b;
                }
                return result;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// 获取缺陷点检结果
        /// </summary>
        /// <returns></returns>
        public static bool GetDefectPointInspectionResult()
        {
            try
            {

                bool result = true;
                foreach (var item in Project.Instance.cameraManager)
                {
                    bool b = true;
                    if ((item.Value.m_PointInspectionStateParam.PointInspectionState_Defect == (int)PointInspectionStateEnum.已忽略) ||
                        (item.Value.m_PointInspectionStateParam.PointInspectionState_Defect == (int)PointInspectionStateEnum.点检成功))
                    {
                        b = true;
                    }
                    else
                    {
                        b = false;
                    }
                    result &= b;
                }
                return result;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
            return false;
        }

    
    }
}
