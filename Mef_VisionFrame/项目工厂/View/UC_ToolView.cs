using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevMgmt.Light.Models;

namespace Mef_VisionFrame
{
    public partial class UC_ToolView : UCViewBase
    {
        VMBase VM;
        public UC_ToolView(VMBase _VM)
        {
            InitializeComponent();
            VM = _VM;
            VM.BaseServer.RefreshToolView -= RefreshView;
            VM.BaseServer.RefreshToolView += RefreshView;
        }
        private void UC_MainView_Load(object sender, EventArgs e)
        {
            RefreshView();
            panel_DataShow.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.panel_DataShow, true);
        }

        private void btn_AutoSim_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("切换运行模式？", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No) return;

            VM.BaseServer.RunModelSwitch();
            RefreshView();
        }

        private void btn_Prev_Click(object sender, EventArgs e)
        {
            if (Project.Instance.Is_simulation)
                VM.BaseData.M_SimRunCommand = SimRunCommand.SimPrev;
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            ExecuteMethodClass.ExecuteMethodContainErrShow(() =>
            {
                if (Project.Instance.Is_Run)
                {
                    VM.BaseServer.StopRun();
                }
                else
                {
                    //自动
                    if (!Project.Instance.Is_simulation)
                    {
                        VM.BaseServer.StartRun();
                    }
                    //仿真
                    else
                    {

                        foreach (ProcessServices item in Project.Instance.m_ProcessList)
                        {
                            Project.Instance.StartRun(item.process.processInfo.ProcessID);
                        }

                        VM.BaseData.M_SimRunCommand = SimRunCommand.SimRun;
                        RefreshView();
                    }
                }


            });

        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            if (Project.Instance.Is_simulation)
                VM.BaseData.M_SimRunCommand = SimRunCommand.SimNext;
        }

        private void switch_LightOnOff_Click(object sender, EventArgs e)
        {
            Project.Instance.LightOnOff = switch_LightOnOff.Checked;
            VM.BaseServer?.thd_LightControl.ReleaseSemaphore();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("CCD软件复位？", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No) return;
            VM.BaseServer.Reset();
        }

        private void btn_FilmRollSwitch_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.Is_MojuanStart)
            {
                //开始膜卷
                VM.BaseServer.StartFilmRoll(input_FilmRoll.Text);
            }
            else
            {
                //结束膜卷
                VM.BaseServer.StopFilmRoll();
            }
        }


        public override void RefreshView()
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                this.Invoke((EventHandler)delegate
                {
                    btn_run.Checked = Project.Instance.Is_Run;
                    btn_AutoSim.Checked = Project.Instance.Is_simulation;
                    btn_FilmRollSwitch.Checked = Project.Instance.Is_MojuanStart;
                    switch_LightOnOff.Checked = Project.Instance.LightOnOff;

                    if (Project.Instance.Is_simulation)
                    {
                        if (Project.Instance.Is_MojuanStart)
                        {
                            input_FilmRoll.Text = Project.Instance.systemInfo.currentMojuanhao;
                            btn_AutoSim.Enabled = false;
                            btn_Prev.Enabled = false;
                            btn_next.Enabled = false;
                            input_FilmRoll.Enabled = false;
                            btn_Reset.Enabled = false;
                        }
                        else
                        {
                            btn_AutoSim.Enabled = true;
                            btn_Prev.Enabled = true;
                            btn_next.Enabled = true;
                            input_FilmRoll.Enabled = true;
                            btn_Reset.Enabled = true;
                        }

                        if (Project.Instance.Is_Run)
                        {
                            switch_LightOnOff.Enabled = false;
                            btn_AutoSim.Enabled = false;
                            btn_Prev.Enabled = false;
                            btn_next.Enabled = false;
                        }
                        else
                        {
                            switch_LightOnOff.Enabled = true;
                            btn_AutoSim.Enabled = true;
                            btn_Prev.Enabled = true;
                            btn_next.Enabled = true;
                        }
                    }
                    else
                    {
                        if (Project.Instance.Is_MojuanStart)
                        {
                            input_FilmRoll.Text = Project.Instance.systemInfo.currentMojuanhao;
                            btn_AutoSim.Enabled = false;
                            btn_Prev.Enabled = false;
                            btn_next.Enabled = false;
                            input_FilmRoll.Enabled = false;
                            btn_Reset.Enabled = false;
                        }
                        else
                        {
                            btn_AutoSim.Enabled = true;
                            btn_Prev.Enabled = true;
                            btn_next.Enabled = true;
                            input_FilmRoll.Enabled = true;
                            btn_Reset.Enabled = true;
                        }

                        if (Project.Instance.Is_Run)
                        {
                            switch_LightOnOff.Enabled = false;                        
                        }
                        else
                        {
                            switch_LightOnOff.Enabled = true;
                        }
                    }
                
                });

            });
        }

        private void btn_ClearCutData_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("是否清除纠偏队列？", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {       
                return;
            }
            VM.BaseServer.ClearCutData(true);
        }
    }


}
