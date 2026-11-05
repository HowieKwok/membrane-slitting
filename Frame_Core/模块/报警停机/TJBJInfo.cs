using FrameCommunity;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/***************************************************************************************************
* 文件名称：
* 摘    要：报警
*
* 当前版本：1.0.0.1
* 作    者：pengjiaqing
* 日    期：2023-10-31
* 备    注：新建
***************************************************************************************************/
namespace Frame_Core
{
    public static class TJBJInfo
    {
        public static FileOperationINI fileOperationINI = new FileOperationINI();
        /// <summary>
        /// 是否显示报警弹窗
        /// </summary>
        public static bool isShow = true;

        /// <summary>
        /// 停机报警路径
        /// </summary>
        public static string TJBJInfoPath = $"{AppDomain.CurrentDomain.BaseDirectory}检测数据\\NewProduct\\停机报警.ini";


        /// <summary>
        /// 报警信息
        /// </summary>
        public static Dictionary<AlarmName, StopAlarmInfo> Alarm_Info = new Dictionary<AlarmName, StopAlarmInfo>();

        /// <summary>
        /// 报警信息
        /// </summary>
        public static Dictionary<CameraEnum, Dictionary<SizeEnum, StopAlarmInfo>> Alarm_SizeInfo = new Dictionary<CameraEnum, Dictionary<SizeEnum, StopAlarmInfo>>();

        /// <summary>
        /// 报警信息
        /// </summary>
        public static Dictionary<CameraEnum, Dictionary<Defct1Enum, StopAlarmInfo>> Alarm_DefectInfo = new Dictionary<CameraEnum, Dictionary<Defct1Enum, StopAlarmInfo>>();

        /// <summary>
        /// 停机信息队列数量
        /// </summary>
        public const int Stop_quantity = 1000;
        /// <summary>
        /// 报警信息队列数量
        /// </summary>
        public const int Alarm_quantity = 1000;
        /// <summary>
        /// 停机信息队列
        /// </summary>
        public static QueuePackage<StopAlarmMessage> m_qStopInfos = new QueuePackage<StopAlarmMessage>();

        /// <summary>
        /// 报警信息队列
        /// </summary>
        public static QueuePackage<StopAlarmMessage> m_qAlarmInfos = new QueuePackage<StopAlarmMessage>();

        private static object AlarmWrite_lock = new object();
        private static object AlarmWriteSize_lock = new object();
        private static object AlarmWriteDefect_lock = new object();

        public static void Init()
        {
            try
            {
                foreach (CameraEnum item in Enum.GetValues(typeof(CameraEnum)))
                {
                    Alarm_SizeInfo[item] = new Dictionary<SizeEnum, StopAlarmInfo>();
                    foreach (SizeEnum item1 in Enum.GetValues(typeof(SizeEnum)))
                    {
                        Alarm_SizeInfo[item][item1] = new StopAlarmInfo()
                        {
                            CurrentStopAlarmState = StopAlarmStateEnum.正常,
                            stopAlarmMessage = new StopAlarmMessage() { name = $"{item}-{item1}" }
                        };
                    }
                }

                foreach (CameraEnum item in Enum.GetValues(typeof(CameraEnum)))
                {
                    Alarm_DefectInfo[item] = new Dictionary<Defct1Enum, StopAlarmInfo>();
                    foreach (Defct1Enum item1 in Enum.GetValues(typeof(Defct1Enum)))
                    {
                        Alarm_DefectInfo[item][item1] = new StopAlarmInfo()
                        {
                            CurrentStopAlarmState = StopAlarmStateEnum.正常,
                            stopAlarmMessage = new StopAlarmMessage() { name = $"{item}-{item1}" }
                        };
                    }
                }


                foreach (AlarmName item in Enum.GetValues(typeof(AlarmName)))
                {
                    Alarm_Info[item] = new StopAlarmInfo()
                    {
                        CurrentStopAlarmState = StopAlarmStateEnum.正常,
                        stopAlarmMessage = new StopAlarmMessage() { name = $"{item}" }
                    };
                }

                Alarm_Info[AlarmName.打标机软件连接失败].stopAlarmMessage.solution = "打标机PLC连接中断~！ 请检查网络/重新连接PLC！";

                StringBuilder AlarmStr = new StringBuilder();
                //出标故障报警
                AlarmStr.Clear();
                //AlarmStr.Append("报错提示：外侧打标机1出标故障\r\n");
                AlarmStr.Append("打标机收到出标指令，并开始执行出标动作，但出标时间超过了保护时间\r\n");
                AlarmStr.Append("1.观察标签纸是否用完，如用完请更换标签纸；\r\n");
                AlarmStr.Append("2.观察标签纸是否断带，如断带请接好标签纸；\r\n");
                AlarmStr.Append("3.对射光电是否故障，对射光电在白纸处应显示黄绿灯，在黄标处应显示绿灯，如状态不一致，\r\n那么就请把对射调好；\r\n");
                AlarmStr.Append("4.打标机主动轮是否因积碳，打滑。如打滑则把打标机拆下来清理一遍。\r\n");
                AlarmStr.Append("请员工手动补标！\r\n");
                AlarmStr.Append("对射的调法：");
                AlarmStr.Append("\r\n①.把标签纸去掉然后将投光侧（没有灯侧）和受光侧（有两个灯侧）的调节开关都逆时针调为最小。\r\n此时灯的情况为只亮一个灯");
                AlarmStr.Append("\r\n②.将投光侧的调节开关顺时针慢慢地调到两个灯一起亮的地方就立马停止并记录下来调节开关的位置");
                AlarmStr.Append("\r\n③.把标签纸放入对射开关中间（与正常工作状态时一样）");
                AlarmStr.Append("\r\n④.再顺时针旋转投光侧的调节开关直到两个灯一起亮的地方就立马停止并记录下来调节开关的位置");
                AlarmStr.Append("\r\n⑤.把投光侧的调节开关调到两个记录位置的中间位置（此位置为最佳位置）。");
                AlarmStr.Append("\r\n⑥.把标签纸放上去后对射感应到黄标可能还会两个灯一起亮（是因为对射的光太强了），\r\n这时把投光侧和受光侧的位置稍微错点位就行了（此操作为减弱投光侧的亮度）");

                Alarm_Info[AlarmName.打标机出标故障报警].stopAlarmMessage.solution = $"报错提示：{AlarmName.打标机出标故障报警}\r\n" + AlarmStr.ToString();

                //标签纸故障报警
                AlarmStr.Clear();
                //AlarmStr.Append("报错提示：外侧打标机1标签纸故障报警\r\n");
                AlarmStr.Append("打标机出标执行完成，在规定范围内，出标检测色标没有检测到打标机执行出的黄标\r\n");
                AlarmStr.Append("提示内容：\r\n");
                AlarmStr.Append("1.出标检测色标是否故障、试教是否正确；\r\n");
                AlarmStr.Append("2.标签纸是否在出标嘴处卡住，如是卡住了则把出标嘴那块的转动轮调一下位置，\r\n使得出标时能把标贴在涂布上\r\n");
                AlarmStr.Append("3.打标机主动轮是否因积碳，打滑。如打滑则把打标机拆下来清理一遍。\r\n");
                AlarmStr.Append("请员工手动补标！\r\n");
                AlarmStr.Append("光纤的调法：");
                AlarmStr.Append("\r\n①.首先把光纤恢复出厂设置长按TUNE键和NO / NC键，再按MODE键，然后再按 + 键，再按MODE键");
                AlarmStr.Append("\r\n②.色彩模式：在初始化状态下按住MODE 3S进入设置，点动MODE找到dist cont，\r\n然后按一下+变成dist col再常按MODE出来然后把黄标放在光纤头下按住TUNE 3S,");
                AlarmStr.Append("\r\n这时将黄标放在光纤头时输出指示灯亮，拿走时灭，调试完成。");

                Alarm_Info[AlarmName.打标机标签纸故障报警].stopAlarmMessage.solution = $"报错提示：{AlarmName.打标机标签纸故障报警}\r\n" + AlarmStr.ToString();


                //标签纸末料报警
                AlarmStr.Clear();
                //AlarmStr.Append("报错提示：外侧打标机1标签纸末料报警\r\n");
                AlarmStr.Append("对射没有感应到黄标5秒以上就会报此警\r\n");
                AlarmStr.Append("提示内容：\r\n");
                AlarmStr.Append("1.对射感应处是否没有黄标或在两黄标中间的空白处。\r\n");
                AlarmStr.Append("如对射感应处没有黄标请把对射前面的标拉掉，把空白后面的第一个标拉至出标嘴处\r\n");
                AlarmStr.Append("如对射感应处在两黄标之间，那么请把标签纸拉直并把黄标置于对射感应处。\r\n");
                AlarmStr.Append("2.观察是否标签纸用完了，如用完了请更换标签纸。\r\n");
                AlarmStr.Append("请员工手动补标！\r\n");

                Alarm_Info[AlarmName.打标机标签纸末料故障报警].stopAlarmMessage.solution = $"报错提示：{AlarmName.打标机标签纸末料故障报警}\r\n" + AlarmStr.ToString();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 获取停机报警消息
        /// </summary>
        /// <param name="alarmName"></param>
        /// <returns></returns>
        public static StopAlarmMessage GetStopAlarmMessage(AlarmName alarmName)
        {
            StopAlarmMessage stopAlarmMessage = new StopAlarmMessage();
            try
            {
                if (Alarm_Info.ContainsKey(alarmName))
                {
                    if (Alarm_Info[alarmName].stopAlarmMessage != null)
                    {
                        stopAlarmMessage = Alarm_Info[alarmName].stopAlarmMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(alarmName.ToString(), ex);
            }
            return stopAlarmMessage;
        }


        /// <summary>
        /// 报警状态写入（显示报警）
        /// </summary>
        /// <param name="alarmName"></param>
        /// <param name="state"></param>
        public static void AlarmWriteState(AlarmName alarmName, StopAlarmStateEnum stopAlarmStateEnum, string alarmStr)
        {
            lock (AlarmWrite_lock)
            {
                try
                {
                    if (stopAlarmStateEnum == StopAlarmStateEnum.正常)
                    {
                        return;
                    }

                    StopAlarmMessage alarmMessage = new StopAlarmMessage();
                    alarmMessage.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    if (alarmMessage.name == "default" || alarmMessage.name == "") alarmMessage.name = alarmName.ToString();
                    alarmMessage.describe = alarmStr;
                    alarmMessage.StopAlarmState = stopAlarmStateEnum;

                    if (Project.Instance.systemInfo.m_CurrentLanguageType != LanguageEnum.Chinese)
                    {
                        try
                        {
                            string str = LanguageProject.GetLangeuageAttribute(alarmName, Project.Instance.systemInfo.m_CurrentLanguageType);
                            alarmMessage.currentLanguageName = str;
                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Debug(alarmName.ToString(), ex);
                        }
                    }

                    StopAlarmInfo alarmInfo = Alarm_Info[alarmName];

                    if ((alarmInfo.CurrentStopAlarmState != StopAlarmStateEnum.停机) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.停机))
                    {
                        m_qStopInfos.Enqueue(alarmMessage, Stop_quantity, true);
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }

                    if ((alarmInfo.CurrentStopAlarmState == StopAlarmStateEnum.正常) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.报警))
                    {
                        if (alarmInfo.CurrentStopAlarmState != alarmMessage.StopAlarmState)
                        {
                            m_qAlarmInfos.Enqueue(alarmMessage, Alarm_quantity, true);
                        }
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }


                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex + alarmName.ToString());
                }
            }
        }

        /// <summary>
        /// 报警状态写入（显示报警）
        /// </summary>
        /// <param name="alarmName"></param>
        /// <param name="state"></param>
        public static void AlarmWriteState(AlarmName alarmName, string source, bool state, string alarmStr, bool isShowForm = true)
        {
            lock (AlarmWrite_lock)
            {
                try
                {
                    if (!state) return;

                    StopAlarmMessage alarmMessage = new StopAlarmMessage();
                    alarmMessage.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    if (alarmMessage.name == "default" || alarmMessage.name == "") alarmMessage.name = alarmName.ToString() + "-" + source;
                    alarmMessage.describe = alarmStr;
                    alarmMessage.StopAlarmState = StopAlarmStateEnum.停机;

                    if (Project.Instance.systemInfo.m_CurrentLanguageType != LanguageEnum.Chinese)
                    {
                        try
                        {
                            string str = LanguageProject.GetLangeuageAttribute(alarmName, Project.Instance.systemInfo.m_CurrentLanguageType);
                            alarmMessage.currentLanguageName = str;
                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Debug(alarmName.ToString(), ex);
                        }
                    }

                    StopAlarmInfo alarmInfo = Alarm_Info[alarmName];

                    if ((alarmInfo.CurrentStopAlarmState != StopAlarmStateEnum.停机) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.停机))
                    {
                        m_qStopInfos.Enqueue(alarmMessage, Stop_quantity, true);
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }

                    if ((alarmInfo.CurrentStopAlarmState == StopAlarmStateEnum.正常) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.报警))
                    {
                        if (alarmInfo.CurrentStopAlarmState != alarmMessage.StopAlarmState)
                        {
                            m_qAlarmInfos.Enqueue(alarmMessage, Alarm_quantity, true);
                        }
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }


                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex + alarmName.ToString());
                }
            }
        }
        /// <summary>
        /// 报警状态写入
        /// </summary>
        /// <param name="alarmName"></param>
        /// <param name="state"></param>
        public static void AlarmWriteState(AlarmName alarmName, StopAlarmMessage stopAlarmMessage)
        {
            lock (AlarmWrite_lock)
            {
                try
                {
                    StopAlarmMessage alarmMessage = stopAlarmMessage.Clone();
                    alarmMessage.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    if (alarmMessage.name == "default" || alarmMessage.name == "") alarmMessage.name = alarmName.ToString();

                    if (alarmMessage.StopAlarmState == StopAlarmStateEnum.正常)
                    {
                        return;
                    }

                    if (Project.Instance.systemInfo.m_CurrentLanguageType != LanguageEnum.Chinese)
                    {
                        try
                        {
                            string str = LanguageProject.GetLangeuageAttribute(alarmName, Project.Instance.systemInfo.m_CurrentLanguageType);
                            alarmMessage.currentLanguageName = str;
                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Debug(alarmName.ToString(), ex);
                        }
                    }

                    StopAlarmInfo alarmInfo = Alarm_Info[alarmName];
                    if ((alarmInfo.CurrentStopAlarmState != StopAlarmStateEnum.停机) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.停机))
                    {
                        m_qStopInfos.Enqueue(alarmMessage, Stop_quantity, true);
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }

                    if ((alarmInfo.CurrentStopAlarmState == StopAlarmStateEnum.正常) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.报警))
                    {
                        if (alarmInfo.CurrentStopAlarmState != alarmMessage.StopAlarmState)
                        {
                            m_qAlarmInfos.Enqueue(alarmMessage, Alarm_quantity, true);
                        }
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }

                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(alarmName.ToString(), ex);
                }
            }
        }


        /// <summary>
        /// 报警状态写入
        /// </summary>
        /// <param name="alarmName"></param>
        /// <param name="state"></param>
        public static void AlarmWriteState(CameraEnum cameraEnum, SizeEnum alarmName, StopAlarmMessage stopAlarmMessage)
        {
            lock (AlarmWriteSize_lock)
            {
                try
                {
                    StopAlarmMessage alarmMessage = stopAlarmMessage.Clone();
                    alarmMessage.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    if (alarmMessage.name == "default" || alarmMessage.name == "") alarmMessage.name = $"{cameraEnum}-{alarmName}";

                    if (alarmMessage.StopAlarmState == StopAlarmStateEnum.正常)
                    {
                        return;
                    }

                    if (Project.Instance.systemInfo.m_CurrentLanguageType != LanguageEnum.Chinese)
                    {
                        try
                        {
                            string str = LanguageProject.GetLangeuageAttribute(alarmName, Project.Instance.systemInfo.m_CurrentLanguageType);
                            alarmMessage.currentLanguageName = cameraEnum.ToString() + "-" + str;
                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Debug(alarmName.ToString(), ex);
                        }
                    }

                    StopAlarmInfo alarmInfo = Alarm_SizeInfo[cameraEnum][alarmName];
                    if ((alarmInfo.CurrentStopAlarmState != StopAlarmStateEnum.停机) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.停机))
                    {
                        m_qStopInfos.Enqueue(alarmMessage, Stop_quantity, true);
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }

                    if ((alarmInfo.CurrentStopAlarmState == StopAlarmStateEnum.正常) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.报警))
                    {
                        if (alarmInfo.CurrentStopAlarmState != alarmMessage.StopAlarmState)
                        {
                            m_qAlarmInfos.Enqueue(alarmMessage, Alarm_quantity, true);
                        }
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug($"{cameraEnum}-{alarmName}", ex);
                }
            }
        }


        /// <summary>
        /// 报警状态写入
        /// </summary>
        /// <param name="alarmName"></param>
        /// <param name="state"></param> 
        public static void AlarmWriteState(CameraEnum cameraEnum, Defct1Enum alarmName, StopAlarmMessage stopAlarmMessage)
        {
            lock (AlarmWriteDefect_lock)
            {
                try
                {
                    StopAlarmMessage alarmMessage = stopAlarmMessage.Clone();
                    alarmMessage.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    if (alarmMessage.name == "default" || alarmMessage.name == "") alarmMessage.name = $"{cameraEnum}-{alarmName}";

                    if (alarmMessage.StopAlarmState == StopAlarmStateEnum.正常)
                    {
                        return;
                    }

                    if (Project.Instance.systemInfo.m_CurrentLanguageType != LanguageEnum.Chinese)
                    {
                        try
                        {
                            string str = LanguageProject.GetLangeuageAttribute(alarmName, Project.Instance.systemInfo.m_CurrentLanguageType);
                            alarmMessage.currentLanguageName = cameraEnum.ToString() + "-" + str;
                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Debug(alarmName.ToString(), ex);
                        }
                    }

                    StopAlarmInfo alarmInfo = Alarm_DefectInfo[cameraEnum][alarmName];
                    if ((alarmInfo.CurrentStopAlarmState != StopAlarmStateEnum.停机) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.停机))
                    {
                        m_qStopInfos.Enqueue(alarmMessage, Stop_quantity, true);
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }

                    if ((alarmInfo.CurrentStopAlarmState == StopAlarmStateEnum.正常) && (alarmMessage.StopAlarmState == StopAlarmStateEnum.报警))
                    {
                        if (alarmInfo.CurrentStopAlarmState != alarmMessage.StopAlarmState)
                        {
                            m_qAlarmInfos.Enqueue(alarmMessage, Alarm_quantity, true);
                        }
                        alarmInfo.CurrentStopAlarmState = alarmMessage.StopAlarmState;
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug($"{cameraEnum}-{alarmName}", ex);
                }
            }
        }
    }

    /// <summary>
    /// 停机报警信息
    /// </summary>
    public class StopAlarmInfo
    {
        /// <summary>
        /// 当前停机报警状态
        /// </summary>
        public StopAlarmStateEnum CurrentStopAlarmState = StopAlarmStateEnum.正常;

        /// <summary>
        /// 停机报警消息
        /// </summary>
        public StopAlarmMessage stopAlarmMessage = new StopAlarmMessage();
    }

    /// <summary>
    /// 停机报警消息
    /// 备注：里面有浅拷贝的方法，所有字段成员只能定义值类型，不能定义引用类型
    /// </summary>
    public class StopAlarmMessage
    {
        /// <summary>
        /// 停机报警名称
        /// </summary>
        public string name = "default";

        /// <summary>
        /// 停机报警名称(当前语言)
        /// </summary>
        public string currentLanguageName = "";

        /// <summary>
        /// 时间
        /// </summary>
        public string time = "time";
        /// <summary>
        /// 停机报警状态
        /// </summary>
        public StopAlarmStateEnum StopAlarmState = StopAlarmStateEnum.正常;

        /// <summary>
        /// 描述
        /// </summary>
        public string describe = "";

        /// <summary>
        /// 解决方案
        /// </summary>
        public string solution = "";

        /// <summary>
        /// 报警显示类型
        /// </summary>
        public AlarmShowType Alarm_ShowType { get; set; } = AlarmShowType.弹窗确认;


        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <returns></returns>
        public StopAlarmMessage Clone()
        {
            return (StopAlarmMessage)this.MemberwiseClone();
        }
    }


}
