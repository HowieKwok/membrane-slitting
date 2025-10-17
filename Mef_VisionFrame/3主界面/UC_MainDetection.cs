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
using ViewROI2;
using System.IO;
using System.Windows.Controls.Primitives;
using LogTool;
using Mef_VisionFrame.Properties;
using HalconDotNet;
using System.Drawing.Drawing2D;

namespace Mef_VisionFrame
{
    public partial class UC_MainDetection : UserControl
    {
        public static UC_MainDetection form_MainDetection;
        public event Action<string> StartMojuanEvent;
        public event Action StopMojuanEvent;
        public event Action StartRunEvent;
        public event Action StopRunEvent;
        public event Action AlarmResetEvent;
        public UC_MainDetection()
        {
            InitializeComponent();
            form_MainDetection = this;
        }
        private void UC_MainDetection_Load(object sender, EventArgs e)
        {
            InitShowForm();
            panel_DataShow.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.panel_DataShow, true);
            //foreach (ProcessServices item in Project.Instance.m_ProcessList)
            //{
            //    item.OpeningThread();
            //}
            tslb_CurrentStation.Text ="PFT_CleaningStation";
            toolStrip_EmployeeID.Text = Project.Instance.systemInfo.EmployeeID;
        }

        public void InitShowForm()
        {
            TLPanal_Main.GetType().GetProperty("DoubleBuffered",
                   System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(TLPanal_Main,
                   true, null);
            try
            {

                Project.Instance.baseProDevice.INIPro();

                // Tb_Craft.Text = Project.Instance.systemInfo.CurrentCraft;
                toolStripBegin_Roll.Text = Project.Instance.systemInfo.currentMojuanhao;

                TLPanal_Main.SuspendLayout();



                TLPanal_Main.ResumeLayout();

                if (Project.Instance.Is_simulation)
                {
                    Translate(Project.Instance.Is_simulation, Project.Instance.Is_MojuanStart);
                    this.toolstripBtn_Simulation.BackColor = Color.Transparent;
                }
                else
                {
                    Translate(Project.Instance.Is_simulation, Project.Instance.Is_MojuanStart);
                    this.toolstripBtn_Simulation.BackColor = Color.DeepSkyBlue;
                }

                if (Project.Instance.Is_SaveModelImage == true)
                {
                    toolstripBtn_SaveModel1.Text = "正在保存";
                    this.toolstripBtn_SaveModel1.BackColor = Color.DeepSkyBlue;
                }
                else
                {
                    toolstripBtn_SaveModel1.Text = "保存模版图";
                    this.toolstripBtn_SaveModel1.BackColor = Color.Transparent;
                }

            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }

        }

        private void toolstripBtn_RunOnce_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.Is_simulation)
            {
                System.Windows.Forms.MessageBox.Show(" “自动运行”状态禁止单步执行。");
                return;
            }
            ((ToolStripButton)sender).Enabled = false;

            //仿真运行信号
            Project.Instance.m_SimRunFlag = true;

            if (Project.Instance.Is_simulation)
            {
                if (Project.Instance.m_SimAutoFlag)
                {
                    foreach (ProcessServices item in Project.Instance.m_ProcessList)
                    {
                        Project.Instance.StartRun(item.process.processInfo.ProcessID);
                    }

                }
            }
        }

        private void toolstripBtn_RunLoop_Click(object sender, EventArgs e)
        {
            StartRun();

        }
        private void toolstripBtn_RunStop_Click(object sender, EventArgs e)
        {
            StopRun();

        }


        /// <summary>
        /// 定时器-刷新主流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Project.Instance.GetStates() == true)
            {
                toolstripBtn_RunOnce.Enabled = false;
                toolstripBtn_RunLoop.Enabled = false;
                toolstripBtn_RunStop.Enabled = true;
            }
            else
            {
                toolstripBtn_RunOnce.Enabled = true;
                toolstripBtn_RunLoop.Enabled = true;
                toolstripBtn_RunStop.Enabled = false;
            }

        }

        private void toolstripBtn_ChangeMojuan_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.Is_MojuanStart)
            {
                //开始膜卷
                StartMojuan(toolStripBegin_Roll.Text);


            }
            else
            {
                //结束膜卷
                StopMojuan();


            }
        }

        object ChangeMojuanLock = new object();


        public void UpdataMJH(string Str_Mojuan)
        {
            toolStrip1.BeginInvoke((EventHandler)delegate
            {
                toolStripBegin_Roll.Text = Str_Mojuan;
            });
        }


        private void toolstripBtn_Simulation_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("切换运行模式？", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

            toolStrip1.BeginInvoke((EventHandler)delegate
            {
                if (Project.Instance.Is_simulation == true)
                {
                    //切换为自动模式
                    Project.Instance.Is_simulation = false;
                    Translate(Project.Instance.Is_simulation, Project.Instance.Is_MojuanStart);
                    this.toolstripBtn_Simulation.BackColor = Color.DeepSkyBlue;

                    //切换为自动模式清除仿真数据
                    Project.Instance.baseProDevice.InitProduct_Sim();

                    if (Project.Instance.Is_MojuanStart)
                    {
                        Project.Instance.NewMujuanhao(Project.Instance.systemInfo.CurrentCraft, Project.Instance.systemInfo.currentMojuanhao, Project.Instance.systemInfo.CurrentMaterial);
                    }

                }
                else
                {
                    //切换为仿真模式
                    Project.Instance.Is_simulation = true;
                    Translate(Project.Instance.Is_simulation, Project.Instance.Is_MojuanStart);
                    this.toolstripBtn_Simulation.BackColor = Color.Transparent;

                    Project.Instance.NewMujuanhao(Project.Instance.systemInfo.CurrentCraft, "SimTest", Project.Instance.systemInfo.CurrentMaterial);
                }
            });
        }

        /// <summary>
        /// 延时刷新图标函数
        /// </summary>
        /// <param name="milliSecond">毫秒</param>
        private void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {

                //可执行某无聊的操作
            }
        }

        private void toolstripBtn_SaveModel1_Click(object sender, EventArgs e)
        {
            toolStrip1.BeginInvoke((EventHandler)delegate
            {
                if (Project.Instance.Is_SaveModelImage == true)
                {
                    Project.Instance.Is_SaveModelImage = false;
                    toolstripBtn_SaveModel1.Text = "保存模版图";
                    this.toolstripBtn_SaveModel1.BackColor = Color.Transparent;
                }
                else
                {
                    Project.Instance.Is_SaveModelImage = true;
                    toolstripBtn_SaveModel1.Text = "正在保存";
                    this.toolstripBtn_SaveModel1.BackColor = Color.DeepSkyBlue;
                }
            });
        }

        private void toolstripBtn_Refresh_Click(object sender, EventArgs e)
        {
            try
            {
                Project.Instance.baseProDevice.INIProduct();
                if (Project.Instance.systemInfo.IsHostMachine)
                {

                    //Project.Instance.dBJPLCManager.dBJParamAddress.DBJWriteParam[(int)DBJSendParamName.伺服复位].value = 1;
                    //Project.Instance.dBJPLCManager.dBJParamAddress.DBJWriteParam[(int)DBJSendParamName.伺服复位].execute = true;

                    //Project.Instance.dBJPLCManager.dBJParamAddress.DBJWriteParam[(int)DBJSendParamName.报警复位].value = 1;
                    //Project.Instance.dBJPLCManager.dBJParamAddress.DBJWriteParam[(int)DBJSendParamName.报警复位].execute = true;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());

            }

        }


        /// <summary>
        /// 翻译
        /// </summary>
        public void Translate(bool AutoOrSimulation, bool BeginMembrane = false)
        {
            if (AutoOrSimulation)
            {
                if (Project.Instance.systemInfo.m_CurrentLanguageType == LanguageEnum.English)
                {
                    toolstripBtn_Simulation.Text = "SimulationRun";
                }
                else
                {
                    toolstripBtn_Simulation.Text = "仿真运行";
                }
            }
            else
            {
                if (Project.Instance.systemInfo.m_CurrentLanguageType == LanguageEnum.English)
                {
                    toolstripBtn_Simulation.Text = "AutoRun";
                }
                else
                {
                    toolstripBtn_Simulation.Text = "自动运行";
                }
            }

            if (Project.Instance.systemInfo.m_CurrentLanguageType == LanguageEnum.English)
            {
                toolstripBtn_Refresh.Text = "Reset";
                //label1.Text = "Output:";
                //label2.Text = "MembraneName:";
                //label3.Text = "Recipe:";
                //label4.Text = "User:";
            }
            else
            {
                toolstripBtn_Refresh.Text = "复位";
                //label1.Text = "产量:";
                //label2.Text = "膜卷号:";
                //label3.Text = "配方:";
                //label4.Text = "用户:";
            }

            if (BeginMembrane)
            {
                if (Project.Instance.systemInfo.m_CurrentLanguageType == LanguageEnum.English)
                {
                    toolstripBtn_ChangeMojuan.Text = "EndMembrane";
                }
                else
                {
                    toolstripBtn_ChangeMojuan.Text = "结束膜卷";
                }
            }
            else
            {
                if (Project.Instance.systemInfo.m_CurrentLanguageType == LanguageEnum.English)
                {
                    toolstripBtn_ChangeMojuan.Text = "BeginMembrane";
                }
                else
                {
                    toolstripBtn_ChangeMojuan.Text = "开始膜卷";
                }
            }




        }

        /// <summary>
        /// 开始膜卷
        /// </summary>
        public bool StartMojuan(string Str_Mojuan)
        {
            bool result = false;
            try
            {
                lock (ChangeMojuanLock)
                {
                  
                    //用正则表达式判断是否为合法的膜卷号，合法的膜卷号由数字和字母和正常符号（-）并且无空格
                    if (!System.Text.RegularExpressions.Regex.IsMatch(Str_Mojuan, "^[a-zA-Z0-9-]+$"))
                    {
                        // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机开始膜卷失败,膜卷号不合法");
                        System.Windows.Forms.MessageBox.Show("膜卷号不合法，只能包含字母、数字、-，不能有空格");
                        return false;
                    }
                    string strPath = Project.Instance.paramPathSet.ProductRootPath + $"{Str_Mojuan}";
                    if (Directory.Exists(strPath))
                    {
                        System.Windows.Forms.MessageBox.Show("膜卷号重复");
                        return false;
                       
                    
                    }

                    Project.Instance.Is_MojuanStart = true;

                    toolStrip1.BeginInvoke((EventHandler)delegate
                    {
                        Translate(Project.Instance.Is_simulation, Project.Instance.Is_MojuanStart);
                        toolStripBegin_Roll.Text = Str_Mojuan;
                        toolStripBegin_Roll.ReadOnly = true;
                        toolStrip_EmployeeID.Enabled = false;
                        toolstripBtn_Refresh.Enabled = false;
                        // Tb_ProductCount.Text = "新膜卷就绪";
                    });

                    Project.Instance.systemInfo.EmployeeID = toolStrip_EmployeeID.Text.Trim();
                    Project.Instance.systemInfo.currentMojuanhao = Str_Mojuan;
                    Project.Instance.systemInfo.Save();


                    Project.Instance.NewMujuanhao(Project.Instance.systemInfo.CurrentCraft, Str_Mojuan, Project.Instance.systemInfo.CurrentMaterial);
                    InitShowForm();

                    //清除业务数据
                    Project.Instance.baseProDevice.INIPro();
                    Project.Instance.baseProDevice.INIProduct();
                    Project.Instance.baseProDevice.InitProduct_Sim();

                    //清除界面显示
                    UIService.Instance.AddAction(() => { BIZ_Switchover.IBIZWorkerBase.ClearForm(); });

                    //清除业务数据
                    BIZ_Switchover.IBIZWorkerBase.InitVariable();
                    BIZ_Switchover.IBIZWorkerBase.DisposeImageAndClearImageQueue();

                    //相机回调计数清零
                    foreach (var item in Project.Instance.cameraManager)
                    {
                        //相机回调计数清零
                        item.Value.Camera.GrabCount = 0;

                        //实时点检初始化
                        item.Value.m_SizePointInspectionManage.sizePointInspectionParam.sizePointInspectionWorkParam.ResetStartDJ();
                    }
                    // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_Start_MJ", "副机开始膜卷");

                    StartMojuanEvent?.Invoke(Str_Mojuan);

                    UIDelegate.LightParamChange();
                }
            }
            catch (Exception ex)
            {
                //Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机开始膜卷失败,请手动开始膜卷");
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show("开始膜卷失败" + ex.ToString());
                return false;
            }

            return true;
        }
        public bool StopMojuan()
        {
            try
            {
                //结束膜卷
                if (Project.Instance.Is_Run)
                {
                    //Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机结束膜卷失败");
                    System.Windows.Forms.MessageBox.Show("CCD正在运行中，请先暂停");
                    return false;
                }

                //Project.Instance.dBDataManage.SaveDBData();

                Project.Instance.Is_MojuanStart = false;
                toolStrip1.Invoke((EventHandler)delegate
                {
                    Translate(Project.Instance.Is_simulation, Project.Instance.Is_MojuanStart);
                    toolStripBegin_Roll.ReadOnly = false;
                    toolstripBtn_Refresh.Enabled = true;
                    toolStrip_EmployeeID.Enabled = true;
                });


                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_Stop_MJ", "副机停止膜卷");
                this.Invoke((EventHandler)delegate
                {
                    StopMojuanEvent?.Invoke();
                });
             
            }
            catch (Exception ex)
            {
                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机结束膜卷失败");
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show("结束膜卷失败" + ex.ToString());
                return false;
            }
            return true;

        }
        public bool StartRun()
        {
            bool result = false;
            try
            {
                if (Project.Instance.Is_simulation)
                {
                    // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机开始运行失败");
                    System.Windows.Forms.MessageBox.Show(" “仿真运行”状态禁止自动运行。");
                    return false;
                }

                if (!Project.Instance.Is_MojuanStart)
                {
                    // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机开始运行失败");
                    System.Windows.Forms.MessageBox.Show("请点击“开始膜卷”按钮。");
                    return false;
                }
                toolStrip1.Invoke(new Action(()=>
                {
                    toolstripBtn_Simulation.Enabled = false;
                    toolstripBtn_ChangeMojuan.Enabled = false;
                    toolstripBtn_RunLoop.Enabled = false;
                }));

                Form_Main.FormMain.menuStrip1.Invoke(new Action(() =>
                {
                    Form_Main.FormMain.menuStrip1.Enabled = false;
                }));
        

                foreach (ProcessServices item in Project.Instance.m_ProcessList)
                {
                    Project.Instance.StartRun(item.process.processInfo.ProcessID);
                }

                foreach (var item in Project.Instance.cameraManager)
                {
                    if (item.Value.initState)
                        if (!item.Value.Camera.IsCameraStatue)
                            item.Value.Camera.StartGrabImage();
                }
                result = true;
                this.BeginInvoke((EventHandler)delegate
                { StartRunEvent?.Invoke(); });

            
                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_RUN", "副机运行中");
            }
            catch (Exception ex)
            {
                //Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机开始运行失败");
                result = false;
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());

            }
            return result;
        }
        public bool StopRun()
        {
            bool result = false;
            try
            {

                if (Project.Instance.cutOverlayPLCManage.ReadMachineRunningState() == 1)
                {
                    // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机暂停运行失败");
                    System.Windows.Forms.MessageBox.Show("主设备运行中，CCD禁止暂停");
                    return false;
                }
                if(Project.Instance.slaveMachineManager.initState)
                {
                    if (((SlaveMachineParam)Project.Instance.slaveMachineManager.tcpClientParam).RunningFlag == 1)
                    {
                        System.Windows.Forms.MessageBox.Show("主软件运行中，CCD禁止暂停");
                        return false;
                    }
                }

             

                if (Project.Instance.systemInfo.IsHostMachine)
                {
                    //Project.Instance.mDevPLCManager.mDevParamAddress.CCDRUN.value = 0;
                    //Project.Instance.mDevPLCManager.mDevParamAddress.CCDRUN.execute = true;

                    //Project.Instance.dBJPLCManager.dBJParamAddress.DBJWriteParam[(int)DBJSendParamName.运行暂停].value = 0;
                    //Project.Instance.dBJPLCManager.dBJParamAddress.DBJWriteParam[(int)DBJSendParamName.运行暂停].execute = true;
                }
                else
                {

                }
            
                Project.Instance.m_SimRunFlag = false;
                this.BeginInvoke((EventHandler)delegate
                {
                    toolstripBtn_Simulation.Enabled = true;
                    toolstripBtn_RunStop.Enabled = false;
                    toolstripBtn_ChangeMojuan.Enabled = true;
                });

                Form_Main.FormMain.menuStrip1.Invoke(new Action(() =>
                {
                    Form_Main.FormMain.menuStrip1.Enabled = true;
                   
                }));
               
                foreach (ProcessServices item in Project.Instance.m_ProcessList)
                {
                    Project.Instance.StopRun(item.process.processInfo.ProcessID);
                }



                //foreach (var item in Project.Instance.cameraManager)
                //{
                //   if (item.Value.initState)
                //     item.Value.Camera.StopGrabImage();
                //}
                result = true;
                this.BeginInvoke((EventHandler)delegate {
                    StopRunEvent?.Invoke();
                });

               
                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_STOP", "副机暂停中");

            }
            catch (Exception ex)
            {
                // Project.Instance.m_SlaveMachine.Remo.SendMsg("FJ_ERROR", "副机暂停运行失败");
                result = false;
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// 自动换卷
        /// </summary>
        /// <param name="Str_Mojuan"></param>
        /// <returns></returns>
        public bool AutoHJ(string Str_Mojuan)
        {
            try
            {
                bool result = true;
                //停止运行
                if (Project.Instance.Is_Run)
                {
                    result = StopRun();
                    if (!result) return false;
                }

                //结束膜卷
                if (Project.Instance.Is_MojuanStart)
                {
                    result = StopMojuan();
                    if (!result) return false;
                }
                //开始膜卷
                result = StartMojuan(Str_Mojuan);
                if (!result) return false;

                //开始运行
                result = StartRun();
                if (!result) return false;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex.ToString());
                return false;
            }
            return true;
        }

        private void toolstripBtn_AutoSimFlag_Click(object sender, EventArgs e)
        {
            if (Project.Instance.m_SimAutoFlag)
            {
                toolstripBtn_AutoSimFlag.Checked = false;
                Project.Instance.m_SimAutoFlag = false;
                toolstripBtn_AutoSimFlag.Image = Resources.开关关;
            }
            else
            {
                toolstripBtn_AutoSimFlag.Checked = true;
                Project.Instance.m_SimAutoFlag = true;
                toolstripBtn_AutoSimFlag.Image = Resources.开关开;
            }
        }

        private void toolstripBtn_Light_Click(object sender, EventArgs e)
        {
            BIZ_Switchover.LightOnOff = !BIZ_Switchover.GetLightState();
            BIZ_Switchover.IBIZWorkerBase?.ThreadManage?.thd_LightControl.ReleaseSemaphore();
        }

        private void toolstripBtn_ShowLine_Click(object sender, EventArgs e)
        {


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                AlarmResetEvent?.Invoke();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }

        }

        public void ToolStripsUpdateShow_Light(bool lightState)
        {
            //显示灯光
            bool flag = lightState;
            //string str = LanguageProject.GetLangeuageAttribute(ToolStripEnum.显示灯光, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
            toolStrip1.Invoke((EventHandler)delegate
            {
                //toolstripBtn_Light.ToolTipText = str;
                toolstripBtn_Light.Image = flag ? Resources.开关开 : Resources.开关关;
            });

        }

        private void toolBtnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                
                //当前用户为空时，不允许退出登录,添加弹窗显示
                if (Project.Instance.userManage.userData.CurrentUser == UserType.Null)
                {
                    MessageBox.Show("当前用户为空，请先登录！");
                    return;

                }
                //添加弹窗确认是否退出功能
                DialogResult result = MessageBox.Show("确认退出登录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Project.Instance.userManage.UserLogin(UserType.Null, "用户未登录", "");
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }
    }
}
