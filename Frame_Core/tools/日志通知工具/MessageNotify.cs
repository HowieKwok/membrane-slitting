using FrameCommunity;
using LogTool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class MessageNotify
    {
        public static int ShowCount { get; } = 500;
        public static QueuePackage<StopAlarmMessage> StopAlarmQueue { get; } = new QueuePackage<StopAlarmMessage>();
        public static ListPackage<string> DebugLogs { get; } = new ListPackage<string>();
        public static ListPackage<string> ErrorLogs { get; } = new ListPackage<string>();
        public static ListPackage<string> CommLogs { get; } = new ListPackage<string>();



        public static bool DebugLogEnable { get; set; } = true;

        public static bool ErrorLogEnable { get; set; } = true;

        public static bool CommLogEnable { get; set; } = true;

        public static void AddStopAlarm(StopAlarmMessage message)
        {
            StopAlarmQueue.Enqueue(message, (uint)ShowCount);
        }
        public static void AddDebugLog(string log)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            Log4netRecord.RunLog.Debug(time + "   " + log);
            DebugLogs.Add(time + "   " + log + "\r\n", (uint)ShowCount);
            DebugLogEnable = true;
        }

        public static void AddErrorLog(string log)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            Log4netRecord.ErrorLog.Error(time + "   " + log);
            ErrorLogs.Add(time + "   " +log + "\r\n", (uint)ShowCount);
            ErrorLogEnable = true;
        }

        public static void AddErrorLog(string log, Exception ex)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            Log4netRecord.ErrorLog.Error(time + "   " + log, ex);
            ErrorLogs.Add(time + "   " + log + "\r\n", (uint)ShowCount);
            ErrorLogEnable = true;
        }

        public static void AddCommLog(string log)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            Log4netRecord.CommLog.Error(time + "   " + log);
            CommLogs.Add(time + "   " + log + "\r\n", (uint)ShowCount);
            CommLogEnable = true;
        }
    }

}
