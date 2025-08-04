using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class StateManage
    {

        static int oldDiskInterval = 0;
        static string oldDiskSpace = "";

        static bool firstExecute = true;
        static DateTime diskUpdateTime = DateTime.Now;
        /// <summary>
        /// 驱动器检测空间1为>30%，2为5%-30%，0为<5%
        /// </summary>
        /// <param name="diskInterval"></param>
        /// <param name="diskSpace"></param>
        /// <returns></returns>
        public static bool CheckDriveFreeSpace(out int diskInterval, out string diskSpace)
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
                            oldDiskSpace = Percent.ToString("F2");
                            oldDiskSpace = $"{oldDiskSpace}%";
                            if (Percent > 30)
                            {
                                oldDiskInterval = 1;
                                disk_flag = true;
                            }
                            else if (Percent <= 30 && Percent > 5)
                            {
                                oldDiskInterval = 2;
                                disk_flag = true;
                            }
                            else if (Percent <= 5)
                            {
                                oldDiskInterval = 0;
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
            diskInterval = oldDiskInterval;
            diskSpace = oldDiskSpace;
            return disk_flag;
        }



    }
}
