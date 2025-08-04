using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frame_Core;
using LogTool;

namespace Mef_VisionFrame
{
    public partial class UC_BaseSetting : UserControl
    {
        public UC_BaseSetting()
        {
            InitializeComponent();
        }

        //  public SystemInfo SystemInfo;
       // public ParamPathSet ParamPathSet;

        private void UC_BaseSetting_Load(object sender, EventArgs e)
        {
            //SystemInfo = Project.Instance.systemInfo;
            //ShowBaseInfo(SystemInfo);
        }
        //public void ShowBaseInfo(SystemInfo SystemInfo)
        //{
        //    try
        //    {
        //        //NUD_oldFileLifetime.Value = (decimal)SystemInfo.oldFileLifetime;
        //        //NUD_diskWarningPercentage.Value = (decimal)SystemInfo.diskWarningPercentage;

        //        //tb_SystemPath.Text = SystemInfo.SystemPath;
        //        //tb_CraftPath.Text = SystemInfo.CraftPath;
        //        //tb_LogPath.Text = SystemInfo.LogPath;
        //        //tb_ProductPath.Text = SystemInfo.ProductPath;
        //        //tb_MesPath.Text = SystemInfo.MesPath;

        //        //CB_ClearProdct_PerRoll.Checked = SystemInfo.ClearProdct_PerRoll;
        //        //CB_ClearProdct_PerShift.Checked = SystemInfo.ClearProdct_PerShift;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //    }
        //}

        //public void RefreshBaseSetting()
        //{
        //    try
        //    {
        //        //SystemInfo.oldFileLifetime = (int)NUD_oldFileLifetime.Value;
        //        //SystemInfo.diskWarningPercentage = (int)NUD_diskWarningPercentage.Value;

        //        ////把文本框的值赋值给基础路径设置，按道理需要防呆，以免输入不对。
        //        //SystemInfo.SystemPath = tb_SystemPath.Text;
        //        //SystemInfo.CraftPath = tb_CraftPath.Text;
        //        //SystemInfo.LogPath = tb_LogPath.Text;
        //        //SystemInfo.ProductPath = tb_ProductPath.Text;
        //        //SystemInfo.MesPath = tb_MesPath.Text;

        //        //SystemInfo.ClearProdct_PerRoll = CB_ClearProdct_PerRoll.Checked;
        //        //SystemInfo.ClearProdct_PerShift = CB_ClearProdct_PerShift.Checked;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //    }
        //}

        private void btn_SystemPath_Click(object sender, EventArgs e)
        {
            // System.Diagnostics.Process.Start(SystemInfo.SystemPath);
        }

        private void btn_CraftPath_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(SystemInfo.CraftPath);
        }

        private void btn_LogPath_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(SystemInfo.LogPath);
        }

        private void btn_ProductPath_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(SystemInfo.ProductPath);
        }

        private void btn_MesPath_Click(object sender, EventArgs e)
        {
            // System.Diagnostics.Process.Start(SystemInfo.MesPath);
        }

        private void btn_SaveSystemInfo_Click(object sender, EventArgs e)
        {
            //RefreshBaseSetting();
            //Project.Instance.systemInfo = SystemInfo;
            //Project.Instance.systemInfo.Save();
        }
    }
}
