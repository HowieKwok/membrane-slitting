using LogTool;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using YYSD.UICore.Control;
using YYSD.UICore;

namespace Frame_Core
{
    public partial class Drive_Form : Form
    {
        DriveParameter driveParameter;
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        public Drive_Form(DriveParameter _driveParameter)
        {
            InitializeComponent();
            driveParameter = _driveParameter;
           
        }



        private void CheckDrives(object sender, EventArgs e)
        {
        
            foreach (var drive in allDrives)
            {
                if (drive.Name == driveParameter.driveName)
                {
                    if (drive.IsReady)
                    {
                        var usedPercent = 100 - (drive.TotalFreeSpace * 100.0 / drive.TotalSize);
                        if (usedPercent >= driveParameter.DriveCapacityLimit)
                        {
                            //显示当前磁盘名称及使用率，并警告，并说明要更换磁盘
                           richTextAlarm.Text = $"当前磁盘 {drive.Name} 已使用率为 {usedPercent:F2} %，\n已达到设定值 {driveParameter.DriveCapacityLimit} %，请更换磁盘";
                            richTextAlarm.BackColor = Color.Red;
                           
                        }
                        else
                        {
                            //显示当前磁盘名称及使用率，并正常
                            richTextAlarm.Text = $"当前磁盘 {drive.Name} 已使用率为 {usedPercent:F2} %";
                            richTextAlarm.BackColor = Color.Green;
                        }

                       
                    }
                    else
                    {

                        richTextAlarm.Text = $"当前磁盘 {drive.Name} 未插入";
                        richTextAlarm.BackColor = Color.Red;
                    }


                }
            }

           
        }

        private void Drive_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            driveParameter.IsShowedWarning = false;
            Btn_Save_Click(null, null);
            this?.Dispose();


        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            driveParameter.Save();
           
            //修改记录设置
            driveParameter.FilePath = (Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\");
            string error = YControlAutobind.Saveing<DriveParameter, IYCustomAttribute>(this, driveParameter);
            // 保存修改记录，保存后备份
            driveParameter.saveChangeLog(driveParameter, String.Format("磁盘参数设置:"));
            driveParameter.backup(driveParameter);
            if (error != "") System.Windows.Forms.MessageBox.Show(error);
           // driveParameter.Read();
        }

        private void Btn_Divercfg_Click(object sender, EventArgs e)
        {

            if(Project.Instance.Is_MojuanStart)
            {
                MessageBox.Show("请先结束膜卷！");
                return;
            }
          

            if(driveParameter.driveName!= comDiverName.Text)
            {
                //弹窗确认是否更换磁盘
                DialogResult result = MessageBox.Show("是否更换磁盘？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    driveParameter.driveName = comDiverName.Text;
                    Project.Instance.paramPathSet.initProdDataPath = $"{driveParameter.driveName}\\生产数据\\";
                    Btn_Save_Click(null, null);
                }
            }
            CheckDrives(null, null);
        }

        private void Drive_Form_Load(object sender, EventArgs e)
        {
            try
            {
                allDrives = DriveInfo.GetDrives();
              
                comDiverName.Items.Clear();
                foreach (DriveInfo drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        comDiverName.Items.Add(drive.Name);
                    }
                }
                if (comDiverName.Items.Count > 0)

                {
                    //查找里面是否有CurrentDrive这个磁盘,有就comDiverName选中这个磁盘
                    int index = comDiverName.FindString(driveParameter.driveName);
                    if (index >= 0)
                    {
                        comDiverName.SelectedIndex = index;
                    }

                }
                CheckDrives(null, null);

                //修改记录备份
                driveParameter.backup(driveParameter);
                YControlAutobind.Loading<DriveParameter, IYCustomAttribute>(this, driveParameter);//测试存修改记录
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug(ee.StackTrace);

                //throw;
            }
          
        }

        private void comDiverName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //获取当前选择的磁盘,并显示磁盘所有信息于richTextBox1
            comDiverName.SelectedItem.ToString();
            diverRichText.Clear();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                if (drive.Name == comDiverName.SelectedItem.ToString())
                {
                    if (drive.IsReady)
                    {
                        diverRichText.AppendText($"驱动器名称: {drive.Name}\n");
                        diverRichText.AppendText($"类型: {drive.DriveType}\n");
                        diverRichText.AppendText($"卷标: {drive.VolumeLabel}\n");
                        diverRichText.AppendText($"文件系统: {drive.DriveFormat}\n");
                        diverRichText.AppendText($"总空间: {drive.TotalSize / (1024.0 * 1024 * 1024):F2} GB\n");
                        diverRichText.AppendText($"可用空间: {drive.TotalFreeSpace / (1024.0 * 1024 * 1024):F2} GB\n");
                        diverRichText.AppendText($"已使用率为: {100 - (drive.TotalFreeSpace * 100.0 / drive.TotalSize):F2} %\n");
                        diverRichText.AppendText("----------\n");
                    }
                    else
                    {
                        diverRichText.AppendText($"当前磁盘 {drive.Name} 未插入\n");

                    }
                }
            }
            CheckDrives(null, null);
        }
    }
}
