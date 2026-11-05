using ControlBoard.服务类;
using DevMgmt.Cameras.View;
using DevMgmt.Light.View;
using Eholly.ZChart;
using EHolly.ImageTool;

using Frame_Core;
using FrameCommunity.DataOperation;
using HalconDotNet;
using LogTool;
using Mef_VisionFrame.调试设置.WPF;
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

        Form_LoadProgressBar form_LoadProgressBar;
        static Form_Main form_Main;
        public Form_Main()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
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


            ProjectFactory.Load();
            uC_StateView1.LoadItems(ProjectFactory.F_Server?.WorkServerBase?.GetStatusBarInfo());

            panel_Main.Controls.Clear();
            panel_Main.Controls.Add(ProjectFactory.F_View.ToolView);   //添加窗体  


            //软件版本显示
            string MainSlaveMachine = "HSVISONMAIN";
            if (!Project.Instance.systemInfo.IsHostMachine)
                MainSlaveMachine = "HSVISONSLAVE";

            Init_LatestTimeProgram("");
            UapateSimPath();
            //初始化状态
            InitStateBar_Flag = true;
            form_LoadProgressBar.closingFlag = true;

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
                Project.Instance.Close_Dev();


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



                    if (uC_StateView1.IsHandleCreated)
                    {
                        uC_StateView1.RefreshStatus(ProjectFactory.F_Server.WorkServerBase.GetStatusBarInfo());
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

        WPFDebugProcessWindow _wpfDebugWindow;
        private void 调试设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.调试设置)) { return; }

            //WPFDebugProcessWindow wpfDebugWindow = new WPFDebugProcessWindow
            //{
            //    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
            //};
            //wpfDebugWindow.ShowDialog();
            //return;

            // 1. 首次初始化窗体 + 拦截关闭行为（关键：关闭改为隐藏，不销毁）
            if (_wpfDebugWindow == null)
            {
                _wpfDebugWindow = new WPFDebugProcessWindow
                {
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
                };


                // 拦截Closing事件：取消原生关闭，改为隐藏窗口（两种关闭方式都会进入该事件）
                _wpfDebugWindow.Closing += (sender1, cancelArgs) =>
                {
                    // 取消系统默认关闭销毁逻辑
                    cancelArgs.Cancel = true;
                    // 隐藏窗口，保留实例
                    _wpfDebugWindow.Hide();
                    this.Enabled = true;
                };
            }

            // 2. 窗口隐藏状态直接唤起弹窗
            if (!_wpfDebugWindow.IsVisible)
            {
                _wpfDebugWindow.Show();
                this.Enabled = false;
            }


            //// 3. 最小化窗口恢复正常状态
            //if (_wpfDebugWindow.WindowState == System.Windows.WindowState.Minimized)
            //{
            //    _wpfDebugWindow.WindowState = System.Windows.WindowState.Normal;
            //}

            //// 4. 前置激活置顶
            //_wpfDebugWindow.Activate();

        }

        private void 工艺标准ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.工艺设置)) { return; }
            //panel_Main.Controls.Clear();
            //CraftSetting.FormBorderStyle = FormBorderStyle.None;
            //CraftSetting.Dock = DockStyle.Fill;
            //CraftSetting.TopLevel = false;
            //panel_Main.Controls.Add(CraftSetting);   //添加窗体

            // CraftSetting.Show();
            using (Form_CraftFrame form = new Form_CraftFrame(Project.Instance.systemInfo, Project.Instance.craftBase, Project.Instance.paramPathSet.CraftPath))
            {
                form.StartPosition = FormStartPosition.CenterParent;
                form.WindowState = FormWindowState.Maximized;
                form.ShowDialog();
            }
            ProjectFactory.F_Server.WorkServerBase.Udpate();

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

        public void UapateSimPath()
        {
            try
            {
                foreach (var item in Project.Instance.M_CameraRelevancies)
                {
                    Service_Camera.Get_CameraRelevancy(item.Key).m_ImageSimManage.ReadImageSimPath(Service_Camera.Get_CameraDev(item.Key).CameraParameter.EmulationPath);
                    Service_Camera.Get_CameraRelevancy(item.Key).m_ImageSimManage.InitSimFlag();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void 相机toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.相机设置)) { return; }
                WpfCameraDevWindow window = new WpfCameraDevWindow(Project.Instance.M_CamParam);
                window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                window.HideSomeControls();
                window.ShowDialog();

                UapateSimPath();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void 光源ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.光源设置)) { return; }
            WpfLightDevWindow window = new WpfLightDevWindow(Project.Instance.M_LightControllerParam);
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.ShowDialog();

        }

        private void pLC通讯ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.主设备通讯)) { return; }

            using (Form_DieCuttingCutPLC form = new Form_DieCuttingCutPLC(Project.Instance.server_DiecuttingCutPLC))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void mark设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.Mark设置)) { return; }

            //using (Form_MarkSet form = new Form_MarkSet())
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}
        }

        private void 相机标定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.相机标定)) { return; }
            //if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阳极单面制片CS)
            //{
            //    using (Form_SelectCamCalibrationPlate form = new Form_SelectCamCalibrationPlate(Project.Instance.M_CameraRelevancies))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}
            //else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阳极双面制片CD)
            //{
            //    using (Form_SelectCamCalibrationPlate form = new Form_SelectCamCalibrationPlate(Project.Instance.M_CameraRelevancies))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}
            //else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阴极双面制片AD)
            //{
            //    using (Form_SelectCamCalibrationPlate form = new Form_SelectCamCalibrationPlate(Project.Instance.M_CameraRelevancies))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}
            //else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片预定位PP)
            //{
            //    using (Form_SelectCamCheckerboard form = new Form_SelectCamCheckerboard(Project.Instance.M_CameraRelevancies))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}
            //else if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片对齐度OVH)
            //{
            //    using (Form_SelectCamCheckerboard form = new Form_SelectCamCheckerboard(Project.Instance.M_CameraRelevancies))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }
            //}

        }

        private void 预定位平台标定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.相机标定)) { return; }
            //if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片预定位PP)
            //{
            //    using (Form_PretargetingCalibration form = new Form_PretargetingCalibration(Project.Instance.M_CameraRelevancies))
            //    {
            //        form.StartPosition = FormStartPosition.CenterScreen;
            //        form.ShowDialog();
            //    }

            //    //Form_PretargetingCalibration form = new Form_PretargetingCalibration(Project.Instance.M_CameraRelevancies);
            //    // form.StartPosition = FormStartPosition.CenterScreen;
            //    //form.Show();
            //}

        }

        private void 打标机软件通讯ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.控制卡)) { return; }
            ServiceCardManage.Instance.ShowWindow();
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
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.纠偏器)) { return; }
            //using (Form_Thick3DSet form = new Form_Thick3DSet(Project.Instance.thick3DManages))
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}

        }

        private void 保存设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.保存设置)) { return; }
            //using (SaveSetingForm form = new SaveSetingForm())
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}

            WindowImageSave window = new WindowImageSave(Project.Instance.M_NewImageSaveParamer);
            window.ShowDialog();
        }

        private void 仿真设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            相机toolStripMenuItem1_Click(sender, e);
        }

        private void 尺寸点检ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.点检)) { return; }
            //using (FormPointInspection form = new FormPointInspection(Project.Instance.M_param_PointInspection))
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}
        }

        private void chineseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Instance.systemInfo.m_CurrentLanguageType = LanguageEnum.Chinese;
            Project.Instance.systemInfo.Save();

        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Instance.systemInfo.m_CurrentLanguageType = LanguageEnum.English;
            Project.Instance.systemInfo.Save();

        }



        private void pC通讯设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.PC通讯设置)) { return; }
            //using (MainSlaveMachineForm form = new MainSlaveMachineForm())
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}
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
            ProjectFactory.F_Server?.WorkServerBase?.SimLoadImage();
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

                Str_LatestTimeProgram = "HSVISION_" + Str_LatestTimeProgram + $"({"LaserCutMachine"})";
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
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.MSA圆点设置)) { return; }
            //using (Form_MsaCircle form = new Form_MsaCircle())
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}
        }

        private void mSA线宽设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.MSA线宽设置)) { return; }
            //using (Form_MsaLineWidth form = new Form_MsaLineWidth())
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}
        }

        private void 数据分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {


        }

        private void mSA棋盘格线宽设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.MSA棋盘格线宽设置)) { return; }
            //using (Form_MsaCheckBoardLine form = new Form_MsaCheckBoardLine(Project.Instance.paramPathSet.MsaLineCheckBoradParamPath))
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}
        }

        private void mSA3D测厚ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.MSA测厚3D设置)) { return; }

        }

        private void 基础设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.基础设置)) { return; }
            using (Form_BaseSetting form = new Form_BaseSetting(Project.Instance.M_param_PointInspection))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 主副机通讯ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.主副机通讯)) { return; }
            //using (SlaveMachineForm form = new SlaveMachineForm(Project.Instance.slaveMachineManager))
            //{
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}
        }

        private void 图片管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.图片管理)) { return; }
            using (Form_ImageManage form = new Form_ImageManage())
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 数据查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "生产数据");          // 实际路径可配
            using (FrmCsvViewer form = new FrmCsvViewer(root))
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private void 灰度自适应ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (!Project.Instance.purviewManage.GetWhetherPurview(Project.Instance.userManage.userData.CurrentUser, ModelName.灰度自适应)) { return; }
            //using (FormGrayScaleAdaptive form = new FormGrayScaleAdaptive())
            //{
            //    LanguageProject.ChangeLanguage(Project.Instance.systemInfo.m_CurrentLanguageType, form);
            //    form.StartPosition = FormStartPosition.CenterScreen;
            //    form.ShowDialog();
            //}
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

        private void 注销账户ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Instance.userManage.UserLogin(UserType.Null, "用户未登录", "");
        }


        bool backuppState = false;
        private void timer_CraftBack_Tick(object sender, EventArgs e)
        {
            if (backuppState) return;
            backuppState = true;
            Task.Run(() => { BackCraftParam(); });
         
        }

        public void BackCraftParam()
        {
            try
            {

                string path = CraftExtension.GetRecipeFilePath(Project.Instance.paramPathSet.CraftPath, Project.Instance.systemInfo.CraftType, Project.Instance.systemInfo.RecipeName);
                string NewPath = Path.Combine(Project.Instance.paramPathSet.CaraftBackPath, Project.Instance.systemInfo.CraftType.ToString());
                Directory.CreateDirectory(NewPath);
                NewPath = Path.Combine(NewPath, $"{Project.Instance.systemInfo.RecipeName}.json");
                File.Copy(path, NewPath, true);

                string DirectoryPath = CraftExtension.GetRecipeFilePath(Project.Instance.paramPathSet.CraftPath, Project.Instance.systemInfo.CraftType, Project.Instance.systemInfo.RecipeName, false);
                string NewDirectoryPath = Path.Combine(Project.Instance.paramPathSet.CaraftBackPath, Project.Instance.systemInfo.CraftType.ToString(), $"{Project.Instance.systemInfo.RecipeName}");
                CopyDirectory(DirectoryPath, NewDirectoryPath);


            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog(ex.Message + "\r\n" + ex.StackTrace);
            }
            backuppState = false;
        }
        /// <summary>
        /// 递归复制目录及其所有子文件/文件夹
        /// </summary>
        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);
            foreach (string file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);
            foreach (string dir in Directory.GetDirectories(sourceDir))
                CopyDirectory(dir, Path.Combine(destDir, Path.GetFileName(dir)));
        }
    }
}
