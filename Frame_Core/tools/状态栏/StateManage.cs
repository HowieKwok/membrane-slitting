using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Frame_Core
{

    public static class MyMetrics
    {
    
        // 复用同一个 Process 实例
        private static readonly System.Diagnostics.Process _p = System.Diagnostics.Process.GetCurrentProcess();

        private static readonly PerformanceCounter _pcMem =
           new PerformanceCounter("Process", "Working Set - Private", _p.ProcessName);

        public static double fWorkingSetMBValue = 0;
        public static double fCpuValue = 0;

        
        #region  内存：对齐任务管理器“内存”列（专用工作集）
        /// <summary>
        /// 与任务管理器“内存”列完全一致（专用工作集，单位 MB，保留 2 位小数）
        /// </summary>
        public static double GetPrivateWorkingSetMB()
        {
            // 第一次拿值可能为 0，强制 NextValue 一次
            double kb = _pcMem.NextValue();
            fWorkingSetMBValue = Math.Round(kb / 1024.0 / 1024.0, 2);
            return fWorkingSetMBValue;
        }
        #endregion

        #region  CPU：对齐任务管理器“CPU”列（单核归一化）
        private static TimeSpan _prevCpu;
        private static DateTime _prevUtc;
        private static bool _inited;

        /// <summary>
        /// 与任务管理器“CPU”列一致：100 表示单核跑满，保留 2 位小数
        /// </summary>
        public static double GetCpuPercentLikeTaskManager()
        {
            if (!_inited)   // 第一次强制采样 50 ms，避免返回 0
            {
                _p.Refresh();
                _prevCpu = _p.TotalProcessorTime;
                _prevUtc = DateTime.UtcNow;
                Thread.Sleep(50);
                _inited = true;
            }

            _p.Refresh();
            var newCpu = _p.TotalProcessorTime;
            var newUtc = DateTime.UtcNow;

            double cpuMs = (newCpu - _prevCpu).TotalMilliseconds;
            double wallMs = (newUtc - _prevUtc).TotalMilliseconds;

            // 任务管理器把 100 % 定义为“单核跑满”，所以除以核心数
            double singleCorePct = cpuMs / wallMs / Environment.ProcessorCount * 100.0;

            _prevCpu = newCpu;
            _prevUtc = newUtc;
            fCpuValue = Math.Round(singleCorePct, 2);
            return fCpuValue;
        }
        #endregion
    }
    public class StateManage
    {

        public static StatusLevel DiskInterval = StatusLevel.Normal;
        public static string DiskSpace = "";

        static bool firstExecute = true;
        static DateTime diskUpdateTime = DateTime.Now;
        /// <summary>
        /// 驱动器检测空间1为>30%，2为5%-30%，0为<5%
        /// </summary>
        /// <param name="diskInterval"></param>
        /// <param name="diskSpace"></param>
        /// <returns></returns>
        public static bool CheckDriveFreeSpace()
        {
            bool disk_flag = false;
            try
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan t_HourSpan = currentTime.Subtract(diskUpdateTime);
                if (t_HourSpan.Minutes >= 1 || firstExecute)
                {
                    firstExecute = false;
                    diskUpdateTime = currentTime;

                    //检测磁盘状态
                    DriveInfo[] Drives = DriveInfo.GetDrives();
                    foreach (DriveInfo di in Drives)
                    {

                        if (di.Name.Equals(Project.Instance.driveParameter.driveName))
                        {
                            double Percent = (double)di.AvailableFreeSpace / (double)di.TotalSize * 100;
                            DiskSpace = Percent.ToString("F2");
                            DiskSpace = $"{DiskSpace}%";
                            if (Percent > 30)
                            {
                                DiskInterval = StatusLevel.Normal;
                                disk_flag = true;
                            }
                            else if (Percent <= 30 && Percent > 5)
                            {
                                DiskInterval = StatusLevel.Warning;
                                disk_flag = true;
                            }
                            else if (Percent <= 5)
                            {
                                DiskInterval = StatusLevel.Alarm;
                                disk_flag = true;
                            }
                            if (Project.Instance.driveParameter.IsAlarm)
                            {
                                if ((100 - Percent) > Project.Instance.driveParameter.DriveCapacityLimit)
                                {
                                    TJBJInfo.AlarmWriteState(AlarmName.磁盘报警, StopAlarmStateEnum.停机, $"磁盘：{di.Name} 已使用率为 {(100 - Percent):F2} %，已达到设定值 {Project.Instance.driveParameter.DriveCapacityLimit} %，请更换磁盘");
                                 }
                            }



                        }
                    }
                }
            }
            catch (Exception)
            {


            }
       
            return disk_flag;
        }



    }




}
