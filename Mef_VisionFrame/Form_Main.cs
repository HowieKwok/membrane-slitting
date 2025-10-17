using EHolly.ImageTool;

using Frame_Core;
using HalconDotNet;
using LogTool;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_Main : Form
    {
        /// <summary>
        /// 获取软件编译时间，作用：用作软件界面版本时间
        /// </summary>
        public string Str_LatestTimeProgram = "20200202-xxxx";



        //界面
        public UC_BaseSetting BaseSetting;//系统设置
        public UC_MainDetection MainDetection;//主界面
        public UC_ProcessSetting ProcessSetting;//调试界面
        public Form_CraftSetting CraftSetting;//工艺参数界面

        Form_LoadProgressBar form_LoadProgressBar;
        static Form_Main form_Main;
        public Form_Main()
        {
            InitializeComponent();
            form_Main = this;

            try
            {
                //加载进度条
                this.Hide();
                form_LoadProgressBar = new Form_LoadProgressBar();
                Thread thBar = new Thread(() => { form_LoadProgressBar.SendToBack(); form_LoadProgressBar.ShowDialog(); });
                thBar.IsBackground = true; thBar.Start();
                Project.Instance.INIProject();//初始化项目
                //CraftSetting=new Form_CraftSetting();
                //MainDetection = new UC_MainDetection();
                //ProcessSetting = new UC_ProcessSetting();
                //BaseSetting = new UC_BaseSetting();
                //BIZ_Switchover.InitBizDataAndForm();

                ////初始化流程
                ////Project.Instance.IFactory.INIPro();
                ////Project.Instance.IFactory.INIProduct();

                //////主副机通讯回调函数注册
                ////Project.Instance.m_SlaveMachine.BeginEvent -= MainDetection.Begin;
                ////Project.Instance.m_SlaveMachine.BeginEvent += MainDetection.Begin;

                ////软件版本显示
                //string MainSlaveMachine = "MainMachine";
                //if (!Project.Instance.systemInfo.IsHostMachine)
                //    MainSlaveMachine = "SlaveMachine";
                //ComStationsanwahlEnum comStationsanwahlEnum = (ComStationsanwahlEnum)Project.Instance.systemInfo.ComStationsanwahl;
                //Init_LatestTimeProgram(comStationsanwahlEnum.ToString());

                ////翻译
                //MenuStripTranslate();



                ////初始化状态
                //InitStateBar_Flag = true;
                //form_LoadProgressBar.closingFlag = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                close();
            }

        }


        public static Form_Main FormMain
        {
            get
            {
                if (form_Main == null)
                {
                    form_Main = new Form_Main();
                }
                return form_Main;
            }
        }
        private void Form_Main_Load(object sender, EventArgs e)
        {
            new UIService(85, this, ThreadPriority.Normal);

            EHollyImageTool.Init();
            EHollyImageTool.StartDeleteService();
            EHollyImageTool.Settings.ImageSaveTimeOut = 4000;

            UIService.Instance.AddAction(() =>
            {
                CraftSetting = new Form_CraftSetting();
                MainDetection = new UC_MainDetection();
                ProcessSetting = new UC_ProcessSetting();
                BaseSetting = new UC_BaseSetting();
                BIZ_Switchover.InitBizDataAndForm();

                //软件版本显示
                string MainSlaveMachine = "MainMachine";
                if (!Project.Instance.systemInfo.IsHostMachine)
                    MainSlaveMachine = "SlaveMachine";
                ComStationsanwahlEnum comStationsanwahlEnum = (ComStationsanwahlEnum)Project.Instance.systemInfo.ComStationsanwahl;
                Init_LatestTimeProgram(comStationsanwahlEnum.ToString());

                //翻译
                MenuStripTranslate();

                //初始化状态
                InitStateBar_Flag = true;
                form_LoadProgressBar.closingFlag = true;
                主界面ToolStripMenuItem_Click(null, null);
            });





        }
        /// <summary>
        /// 禁用窗体双击放大缩小
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 0xA3 && m.Msg != 0x0003 && m.WParam != (IntPtr)0xF012)
            {
                base.WndProc(ref m);
            }
        }
        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (Project.Instance.Is_MojuanStart)
                {
                    System.Windows.Forms.MessageBox.Show("请结束膜卷再关闭软件");
                    e.Cancel = true;
                    return;
                }

                DialogResult result = System.Windows.Forms.MessageBox.Show("是否关闭软件？", "询问", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {

                    e.Cancel = true;
                    return;
                }
                //Project.Instance.systemInfo.CurrentCraft = Project.Instance.paramPathSet.CurrentCraft;
                //Project.Instance.systemInfo.CurrentMaterial = Project.Instance.paramPathSet.CurrentMaterial;
                //Project.Instance.systemInfo.IsHostMachine = Project.Instance.paramPathSet.IsHostMachine;

                Project.Instance.systemInfo.Save();
                //释放硬件资源
                Project.Instance.CloseAllDev();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
            close();
        }
        /// <summary>
        /// 关闭程序，释放资源
        /// </summary>
        public void close()
        {
            this.Dispose(true);
            this.Close();
            // Application.Exit();
            Environment.Exit(0);
            System.Diagnostics.Process[] objProcess = System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            if (objProcess != null)
            {
                for (int i = 0; i < objProcess.Length; i++)
                {
                    if (objProcess[i].Id != System.Diagnostics.Process.GetCurrentProcess().Id)
                    {
                        //double T = objProcess[i].TotalProcessorTime.TotalMilliseconds;
                        //System.Windows.Forms.MessageBox.Show("关闭进程,id:" + objProcess[i].Id.ToString() + ",T:" + T.ToString("F3"));
                        objProcess[i].Kill();
                    }
                }
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            //System.Environment.Exit(System.Environment.ExitCode);

        }
        ///<summary>
        ///窗体切换，防止闪烁
        ///</summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        #region 窗体控件重绘，防止闪烁
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;
        #endregion

        #region 底部状态栏

        bool InitStateBar_Flag = false;
        /// <summary>
        /// 状态栏刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_StatusBarUpdate_Tick(object sender, EventArgs e)
        {
            if (InitStateBar_Flag)
            {
                try
                {
                    SoftwareState[] softwareState = BIZ_Switchover.UpdateStatStrip();

                    if (statusStrip1.IsHandleCreated)
                    {
                        NStatusStrip.UpdateStatStrip(statusStrip1, softwareState);
                    }
                }
                catch (Exception)
                {

                }

            }
        }
        #endregion

        private void 系统设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 外联设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panel_Main.Controls.Clear();
            panel_Main.Controls.Add(MainDetection);   //添加窗体
            MainDetection.Dock = DockStyle.Fill;    //设置填充
            MainDetection.InitShowForm();
            MainDetection.Show();
            Project.Instance.activeObject = MainDetection;
        }
        private void 调试设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.调试设置)) { return; }
            panel_Main.Controls.Clear();
            panel_Main.Controls.Add(ProcessSetting);   //添加窗体
            ProcessSetting.Dock = DockStyle.Fill;    //设置填充
            ProcessSetting.IniUC_ProcessSetting();
            ProcessSetting.Show();
            // Project.Instance.activeObject = ProcessSetting;

        }
        private void 工艺标准ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.工艺设置)) { return; }
            panel_Main.Controls.Clear();
            CraftSetting.FormBorderStyle = FormBorderStyle.None;
            CraftSetting.Dock = DockStyle.Fill;
            CraftSetting.TopLevel = false;
            panel_Main.Controls.Add(CraftSetting);   //添加窗体
            CraftSetting.Ini_CraftSetting();
            CraftSetting.Show();
            // Project.Instance.activeObject = CraftSetting;
            //CraftSetting.StartPosition = FormStartPosition.CenterParent;
            //CraftSetting.ShowDialog();
        }



        private void 用户登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (UserLogionForm userLogionForm = new UserLogionForm(Project.Instance.userManage))
            {
                userLogionForm.StartPosition = FormStartPosition.CenterScreen;
                userLogionForm.ShowDialog();
            }
        }

        private void 账户管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (UserManagerForm userManagerForm = new UserManagerForm(Project.Instance.userManage))
            {
                userManagerForm.StartPosition = FormStartPosition.CenterScreen;
                userManagerForm.ShowDialog();
            }
        }

        private void 相机toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.相机设置)) { return; }
            using (Cam_Form cam_Form = new Cam_Form(Project.Instance.cameraManager))
            {
                cam_Form.StartPosition = FormStartPosition.CenterScreen;
                cam_Form.ShowDialog();
            }
        }

        private void 光源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.光源设置)) { return; }
            using (Light_Form light_Form = new Light_Form(Project.Instance.lightManager))
            {
                light_Form.StartPosition = FormStartPosition.CenterScreen;
                light_Form.ShowDialog();
            }
        }

        private void pLC通讯ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.主设备通讯)) { return; }

            using (Form_MainPLC form = new Form_MainPLC(Project.Instance.cutOverlayPLCManage))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void mark设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.Mark设置)) { return; }

            using (Form_MarkSet form = new Form_MarkSet(Project.Instance.cameraManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 相机标定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.相机标定)) { return; }
            //if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阳极单面制片CS)
            //{
            //    using (Form_SelectCamCalibrationPlate form = new Form_SelectCamCalibrationPlate(Project.Instance.cameraManager))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}
            //else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阳极双面制片CD)
            //{
            //    using (Form_SelectCamCalibrationPlate form = new Form_SelectCamCalibrationPlate(Project.Instance.cameraManager))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}
            //else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阴极双面制片AD)
            //{
            //    using (Form_SelectCamCalibrationPlate form = new Form_SelectCamCalibrationPlate(Project.Instance.cameraManager))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}
            //else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片预定位PP)
            //{
            //    using (Form_SelectCamCheckerboard form = new Form_SelectCamCheckerboard(Project.Instance.cameraManager))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}
            //else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片对齐度OVH)
            //{
            //    using (Form_SelectCamCheckerboard form = new Form_SelectCamCheckerboard(Project.Instance.cameraManager))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}

        }

        private void 预定位平台标定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.相机标定)) { return; }
            //if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片预定位PP)
            //{
            //    using (Form_PretargetingCalibration form = new Form_PretargetingCalibration(Project.Instance.cameraManager))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }

            //    //Form_PretargetingCalibration form = new Form_PretargetingCalibration(Project.Instance.cameraManager);
            //    // form.StartPosition = FormStartPosition.CenterScreen;
            //    //form.Show();
            //}

        }

        private void 打标机软件通讯ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //using (DBJCardForm dBJCardForm = new DBJCardForm(Project.Instance.dbjCardManager))
            //{
            //    dBJCardForm.StartPosition = FormStartPosition.CenterScreen;
            //    dBJCardForm.ShowDialog();
            //}

            //using (TCPClientForm dBJCardForm = new TCPClientForm(Project.Instance.dbjCardManager))
            //{
            //    dBJCardForm.StartPosition = FormStartPosition.CenterScreen;
            //    dBJCardForm.ShowDialog();
            //}
        }

        private void 生产统计ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Project.Instance.paramPathSet.ProductMojuanPath);
        }

        private void 纠偏设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.纠偏器)) { return; }
            using (Form_Thick3DSet form = new Form_Thick3DSet(Project.Instance.thick3DManages))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }

        }

        private void 保存设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.保存设置)) { return; }
            using (SaveSetingForm form = new SaveSetingForm(Project.Instance.cameraManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 仿真设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.仿真设置)) { return; }
            using (EmulationSetForm form = new EmulationSetForm(Project.Instance.cameraManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 尺寸点检ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.点检)) { return; }
            using (FormPointInspection form = new FormPointInspection(Project.Instance.systemInfo, Project.Instance.cameraManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void chineseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Instance.systemInfo.m_CurrentLanguageType = LanguageEnum.Chinese;
            Project.Instance.systemInfo.Save();
            MenuStripTranslate();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Instance.systemInfo.m_CurrentLanguageType = LanguageEnum.English;
            Project.Instance.systemInfo.Save();
            MenuStripTranslate();
        }


        /// <summary>
        /// 菜单栏翻译
        /// </summary>
        public void MenuStripTranslate()
        {
            系统设置ToolStripMenuItem.Text = "系统设置";
            保存设置ToolStripMenuItem.Text = "保存设置";
            仿真设置ToolStripMenuItem.Text = "仿真设置";

            用户设置ToolStripMenuItem.Text = "用户设置";
            用户登录ToolStripMenuItem.Text = "用户登录";
            账户管理ToolStripMenuItem.Text = "账户管理";

            主界面ToolStripMenuItem.Text = "主界面";

            调试设置ToolStripMenuItem.Text = "调试设置";

            工艺标准ToolStripMenuItem.Text = "工艺标准";

            外联设置ToolStripMenuItem.Text = "外联设置";
            相机toolStripMenuItem1.Text = "相机设置";
            光源ToolStripMenuItem.Text = "光源设置";
            pLC通讯ToolStripMenuItem.Text = "主设备通讯";
            打标机软件通讯ToolStripMenuItem.Text = "打标机软件通讯";
            纠偏设置ToolStripMenuItem.Text = "纠偏设置";

            点检校验ToolStripMenuItem.Text = "点检校验";
            相机标定ToolStripMenuItem.Text = "相机标定";
            尺寸点检ToolStripMenuItem.Text = "点检";

            数据分析ToolStripMenuItem.Text = "数据分析";
            生产统计ToolStripMenuItem.Text = "生产统计";

            语言ToolStripMenuItem.Text = "语言";
            chineseToolStripMenuItem.Text = "中文";
            englishToolStripMenuItem.Text = "英文";

            帮助ToolStripMenuItem.Text = " 帮助";
            if (Project.Instance.systemInfo.m_CurrentLanguageType == LanguageEnum.English)
            {
                系统设置ToolStripMenuItem.Text = "SystemSetting";
                保存设置ToolStripMenuItem.Text = "SaveSetting";
                仿真设置ToolStripMenuItem.Text = "SimulationSetting";

                用户设置ToolStripMenuItem.Text = "UserSetting";
                用户登录ToolStripMenuItem.Text = "UserLogin";
                账户管理ToolStripMenuItem.Text = "AccountManage";

                主界面ToolStripMenuItem.Text = "MainHMI";

                调试设置ToolStripMenuItem.Text = "DebugSetting";

                工艺标准ToolStripMenuItem.Text = "TechnologicalStandard";

                外联设置ToolStripMenuItem.Text = "ExternalConnectionSetting";
                相机toolStripMenuItem1.Text = "CamSetting";
                光源ToolStripMenuItem.Text = "LightSetting";
                pLC通讯ToolStripMenuItem.Text = "MainDevCommumication";
                打标机软件通讯ToolStripMenuItem.Text = "MarkingMachineCommumication";
                纠偏设置ToolStripMenuItem.Text = "CorrectSetting";

                点检校验ToolStripMenuItem.Text = "SpotCheck";
                相机标定ToolStripMenuItem.Text = "CamCalibration";
                尺寸点检ToolStripMenuItem.Text = "Check";

                数据分析ToolStripMenuItem.Text = "DataAnalyse";
                生产统计ToolStripMenuItem.Text = "ProductionStatistic";

                语言ToolStripMenuItem.Text = "Language";
                chineseToolStripMenuItem.Text = "Chinese";
                englishToolStripMenuItem.Text = "English";

                帮助ToolStripMenuItem.Text = " Help";
            }

            MainDetection.Translate(Project.Instance.Is_simulation, Project.Instance.Is_MojuanStart);
        }

        static DateTime lastTime = DateTime.Now;
        bool isShow = true;
        /// <summary>
        /// 生产数据显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_DataShow_Tick(object sender, EventArgs e)
        {
            //if (true) return;
            if (!isShow) return;
            isShow = false;
            //======================================================================
            //数据表格显示
            #region 数据表格显示
            if (InitStateBar_Flag)
            {
                BIZ_Switchover.IBIZWorkerBase?.UpdateForm();
            }

            isShow = true;
            #endregion
        }

        private void pC通讯设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.PC通讯设置)) { return; }
            using (MainSlaveMachineForm form = new MainSlaveMachineForm())
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        /// <summary>
        /// 仿真加载图像锁标志
        /// </summary>
        bool SimLockFlag = false;
        /// <summary>
        /// 仿真加载图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_SimLoadImage_Tick(object sender, EventArgs e)
        {
            if (SimLockFlag) return;

            SimLockFlag = true;
            BIZ_Switchover.IBIZWorkerBase?.SimLoadImage(Project.Instance.m_SimAutoFlag, ref Project.Instance.m_SimRunFlag);
            SimLockFlag = false;
        }

        private void 磁盘设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.磁盘设置)) { return; }
            if (!Project.Instance.driveParameter.IsShowedWarning)
            {
                Project.Instance.driveParameter.IsShowedWarning = true;
                using (Drive_Form disk_Form = new Drive_Form(Project.Instance.driveParameter))
                {
                    disk_Form.StartPosition = FormStartPosition.CenterScreen;
                    disk_Form.ShowDialog();


                }
            }
        }



        /// <summary>
        /// 获取软件编译时间，作用：用作软件界面版本时间显示  
        /// </summary>
        public void Init_LatestTimeProgram(string millType)
        {
            try
            {
                //===============================================================================================================
                //==========【】获取最后编译时间：
                Str_LatestTimeProgram = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location).ToString("yyyyMMdd-HHmm");

                Str_LatestTimeProgram = "YH_" + Str_LatestTimeProgram + $"({"PFT_CleaningStation"})";
                this.Text = Str_LatestTimeProgram;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        private void 控制卡ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.控制卡)) { return; }

            //using (Form_ControlCard form = new Form_ControlCard(Project.Instance.bIZ_ControlCard))
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}       
        }

        private void 点检校验ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mSA圆点设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.MSA圆点设置)) { return; }
            using (Form_MsaCircle form = new Form_MsaCircle(Project.Instance.cameraManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void mSA线宽设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.MSA线宽设置)) { return; }
            using (Form_MsaLineWidth form = new Form_MsaLineWidth(Project.Instance.cameraManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 数据分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // 判断点击的菜单项是否是 "主界面ToolStripMenuItem"
            if (e.ClickedItem != 主界面ToolStripMenuItem)
            {
                // 如果不是 "主界面ToolStripMenuItem"，则触发新的方法
                Project.Instance.activeObject = null;
            }
            else
            {
                Project.Instance.activeObject = MainDetection;
            }

        }

        private void mSA棋盘格线宽设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.MSA棋盘格线宽设置)) { return; }
            using (Form_MsaCheckBoardLine form = new Form_MsaCheckBoardLine(Project.Instance.paramPathSet.MsaLineCheckBoradParamPath, Project.Instance.cameraManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void mSA3D测厚ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.MSA测厚3D设置)) { return; }
            using (Form_MsaMeasureThick3D form = new Form_MsaMeasureThick3D(Project.Instance.cameraManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();

            }
        }

        private void 基础设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.系统设置)) { return; }
            using (Form_BaseSetting form = new Form_BaseSetting(Project.Instance.systemInfo))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 主副机通讯ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.主副机通讯)) { return; }
            using (SlaveMachineForm form = new SlaveMachineForm(Project.Instance.slaveMachineManager))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 图片管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.主副机通讯)) { return; }
            using (Form_ImageManage form = new Form_ImageManage())
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 数据查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string root = @"E:\生产数据";          // 实际路径可配
            using (FrmCsvViewer form = new FrmCsvViewer(root))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 灰度自适应ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.灰度自适应)) { return; }
            using (FormGrayScaleAdaptive form = new FormGrayScaleAdaptive(Project.Instance.cameraManager))
            {
                LanguageProject.ChangeLanguage(Project.Instance.systemInfo.m_CurrentLanguageType, form);
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void timer_DJ_Tick(object sender, EventArgs e)
        {
            try
            {
                timer_DJ.Enabled = false;
                PointInspectionWorks.PointInspectionHint();
                timer_DJ.Enabled = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
    }
}
