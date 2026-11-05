using DevMgmt.Light.Models;
using Frame_Core;
using FrameCommunity;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    /// <summary>
    /// 线程管理基类
    /// </summary>
    public class AThreadManageBase
    {
        public Thd_WorkClass thd_ImageShowAndSave = new Thd_WorkClass();

        public Thd_WorkClass thd_StopAlarm = new Thd_WorkClass();

        public Thd_WorkClass thd_LightControl = new Thd_WorkClass();

        public Thd_WorkClass thd_SimLoadImage = new Thd_WorkClass();

        public Thd_WorkClass thd_DataSave = new Thd_WorkClass();

        public Thd_WorkClass thd_DataProcess = new Thd_WorkClass();

        public Thd_WorkClass thd_Communication = new Thd_WorkClass();
        public virtual void InitThread()
        {

            TJBJInfo.Init();
            thd_StopAlarm.Start(50, "ThdStopAlarm");
            thd_StopAlarm.PerformWork -= StopAlarm;
            thd_StopAlarm.PerformWork += StopAlarm;

            thd_LightControl.Start(500, "ThdLightControl");
            thd_LightControl.PerformWork -= LightControl;
            thd_LightControl.PerformWork += LightControl;

            //thd_SimLoadImage.Start(300, "ThdSimLoadImage");
            //thd_SimLoadImage.PerformWork -= SimLoadImage;
            //thd_SimLoadImage.PerformWork += SimLoadImage;
        }


        public void StopAlarm()
        {
            try
            {
                if (thd_StopAlarm.ExecuteTime > 20)
                {

                }

                StopAlarmInfo stopInfo = TJBJInfo.Alarm_Info.FirstOrDefault(p => p.Value.CurrentStopAlarmState == StopAlarmStateEnum.停机).Value;
                StopAlarmInfo alarmInfo = TJBJInfo.Alarm_Info.FirstOrDefault(p => p.Value.CurrentStopAlarmState == StopAlarmStateEnum.报警).Value;

                StopAlarmInfo stopSizeInfo = TJBJInfo.Alarm_SizeInfo.SelectMany(p => p.Value.Values).FirstOrDefault(q => q.CurrentStopAlarmState == StopAlarmStateEnum.停机);
                StopAlarmInfo alarmSizeInfo = TJBJInfo.Alarm_SizeInfo.SelectMany(p => p.Value.Values).FirstOrDefault(q => q.CurrentStopAlarmState == StopAlarmStateEnum.报警);

                StopAlarmInfo stopDefectInfo = TJBJInfo.Alarm_DefectInfo.SelectMany(p => p.Value.Values).FirstOrDefault(q => q.CurrentStopAlarmState == StopAlarmStateEnum.停机);
                StopAlarmInfo alarmDefectInfo = TJBJInfo.Alarm_DefectInfo.SelectMany(p => p.Value.Values).FirstOrDefault(q => q.CurrentStopAlarmState == StopAlarmStateEnum.报警);


                //发送停机报警
                if (stopInfo != null || stopSizeInfo != null || stopDefectInfo != null)
                {
                    string message = "";
                    if (stopInfo != null) { stopInfo.CurrentStopAlarmState = StopAlarmStateEnum.正常; message = stopInfo.stopAlarmMessage.describe; }
                    if (stopSizeInfo != null) { stopSizeInfo.CurrentStopAlarmState = StopAlarmStateEnum.正常; message = stopSizeInfo.stopAlarmMessage.describe; }
                    if (stopDefectInfo != null) { stopDefectInfo.CurrentStopAlarmState = StopAlarmStateEnum.正常; message = stopDefectInfo.stopAlarmMessage.describe; }

                    ProjectFactory.F_Server?.WorkServerBase?.SendStopAlarm(message);
                    ProjectFactory.F_Server?.WorkServerBase?.SendStopAlarm(30);

                }
                if (alarmInfo != null || alarmSizeInfo != null || alarmDefectInfo != null)
                {
                    string message = "";
                    if (alarmInfo != null) { alarmInfo.CurrentStopAlarmState = StopAlarmStateEnum.正常; message = alarmInfo.stopAlarmMessage.describe; }
                    if (alarmSizeInfo != null) { alarmSizeInfo.CurrentStopAlarmState = StopAlarmStateEnum.正常; message = alarmSizeInfo.stopAlarmMessage.describe; }
                    if (alarmDefectInfo != null) { alarmDefectInfo.CurrentStopAlarmState = StopAlarmStateEnum.正常; message = alarmDefectInfo.stopAlarmMessage.describe; }

                    ProjectFactory.F_Server?.WorkServerBase?.SendStopAlarm(message);
                    ProjectFactory.F_Server?.WorkServerBase?.SendStopAlarm(30);
                }

                List<StopAlarmMessage> stopMessages = TJBJInfo.m_qStopInfos.DequeueAll();
                List<StopAlarmMessage> alarmMessages = TJBJInfo.m_qAlarmInfos.DequeueAll();

                //保存报警停机日志、显示报警停机日志、添加弹窗显示队列
                if (stopMessages != null)
                {
                    string str = "Stop";
                    List<string> tempInfoList = new List<string>();
                    for (int i = 0; i < stopMessages.Count; i++)
                    {
                        string tempInfo = $"[{stopMessages[i].time}-{str}]-[{stopMessages[i].name}--{stopMessages[i].currentLanguageName}]" + "---" + $"[{stopMessages[i].describe}]";
                        tempInfoList.Add(tempInfo);
                        AddShowStopAlarmInfo(stopMessages[i]);
                        MessageNotify.AddStopAlarm(stopMessages[i]);
                    }
                    if (tempInfoList.Count > 0)
                    {
                        TJBJInfo.fileOperationINI.Write_SendLog_ini(TJBJInfo.TJBJInfoPath, tempInfoList, "\r\n");
                    }
                }
                if (alarmMessages != null)
                {
                    string str = "Alarm";
                    List<string> tempInfoList = new List<string>();
                    for (int i = 0; i < alarmMessages.Count; i++)
                    {
                        string tempInfo = $"[{alarmMessages[i].time}-{str}]-[{alarmMessages[i].name}--{alarmMessages[i].currentLanguageName}]" + "---" + $"[{alarmMessages[i].describe}]";
                        tempInfoList.Add(tempInfo);
                        AddShowStopAlarmInfo(alarmMessages[i]);
                        MessageNotify.AddStopAlarm(alarmMessages[i]);
                    }
                    if (tempInfoList.Count > 0)
                    {
                        TJBJInfo.fileOperationINI.Write_SendLog_ini(TJBJInfo.TJBJInfoPath, tempInfoList, "\r\n");
                    }
                }

                //弹窗显示
                if (!ShowStopAlarm_Flag)
                {
                    if (stopAlarmMessageShow.Count > 0)
                    {
                        StopAlarmMessage stopAlarmessage = stopAlarmMessageShow.Dequeue();
                        switch (stopAlarmessage.Alarm_ShowType)
                        {
                            case AlarmShowType.弹窗确认:
                                ShowStopAlarm_Flag = true;
                                Task.Run(() =>
                                {
                                    ShowStopAlarm(stopAlarmessage);
                                });
                                break;
                            case AlarmShowType.权限确认:
                                break;
                            case AlarmShowType.刷卡确认:
                                break;
                            default:
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        QueuePackage<StopAlarmMessage> stopAlarmMessageShow = new QueuePackage<StopAlarmMessage>();

        public void AddShowStopAlarmInfo(StopAlarmMessage stopAlarmessage)
        {
            stopAlarmMessageShow.Enqueue(stopAlarmessage, 20, true);
        }
        bool ShowStopAlarm_Flag = false;
        public void ShowStopAlarm(StopAlarmMessage stopAlarmessage)
        {
            using (Form_StopAlarm form = new Form_StopAlarm(stopAlarmessage))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
            ShowStopAlarm_Flag = false;
        }


        public void LightControl()
        {
            try
            {
                thd_LightControl.m_semaphore.Wait();
                if (Project.Instance.LightOnOff) Project.Instance.M_LightControllerParam.l_LightControllerDevs.OpenAllLight();
                else Project.Instance.M_LightControllerParam.l_LightControllerDevs.ColseAllLight();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }


    }
}
