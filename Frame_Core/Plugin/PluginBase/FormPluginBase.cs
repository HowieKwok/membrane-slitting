using Frame_Core.toosls;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Frame_Core
{
    /// <summary>
    /// WinForm 插件窗体基类。
    /// 
    /// 与 WPFFormPluginBase 一样，WinForm 基类也不再通过 ActionCompleted / RaiseActionCompleted
    /// 通知调试界面刷新。统一由插件对象的 IPlugin.ExecuteCompleted 事件通知外部。
    /// 
    /// 保留原来的按钮功能：
    /// 1. 执行按钮仍然调用 pluginsObjBase.ExecuteObj()。
    /// 2. 保存按钮仍然调用 pluginsObjBase.saveParams()。
    /// 3. 耗时标签仍然由基类显示。
    /// </summary>
    public partial class FormPluginBase : Form
    {
        private bool executeCompletedSubscribed;

        /// <summary>
        /// 窗体语言翻译。
        /// </summary>
        public LanguageEnum FormLanguage;

        /// <summary>
        /// 当前窗体绑定的插件对象。
        /// </summary>
        public IPlugin pluginsObjBase { get; set; }

        /// <summary>
        /// 备份参数，保留原字段，避免破坏旧窗体逻辑。
        /// </summary>
        public PluginParamsBase pluginsObjParams_Back { get; set; }

        public FormPluginBase()
        {
            InitializeComponent();
            FormClosed += FormPluginBase_FormClosed;
        }

        public virtual void Init()
        {
            if (pluginsObjBase == null || pluginsObjBase.moduleInfo == null) return;

            Text = pluginsObjBase.moduleInfo.ModuleName;
            SubscribeExecuteCompleted();
        }

        private void FormPluginBase_Load(object sender, EventArgs e)
        {
            if (pluginsObjBase == null) return;

            Init();
            LanguageProject.ChangeLanguage(FormLanguage, this);
        }

        public virtual void Btn_RUN_Click(object sender, EventArgs e)
        {
            if (pluginsObjBase == null)
            {
                Tb_CostTime.Text = "耗时：000ms";
                return;
            }

            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                bool success = pluginsObjBase.ExecuteObj();
                watch.Stop();

                long elapsedMs = watch.ElapsedMilliseconds;
                Tb_CostTime.Text = "耗时：" + elapsedMs.ToString("0.0") + "ms";

                if (pluginsObjBase.PluginParams != null)
                {
                    pluginsObjBase.PluginParams.m_bRunSuccess = success;
                    pluginsObjBase.PluginParams.m_fElapsedMs = elapsedMs;
                    if (string.IsNullOrWhiteSpace(pluginsObjBase.PluginParams.m_strRunMessage))
                    {
                        pluginsObjBase.PluginParams.m_strRunMessage = success ? "执行完成" : "执行失败";
                    }
                }
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                Tb_CostTime.Text = "耗时：" + watch.ElapsedMilliseconds.ToString("0.0") + "ms";

                if (pluginsObjBase.PluginParams != null)
                {
                    pluginsObjBase.PluginParams.m_bRunSuccess = false;
                    pluginsObjBase.PluginParams.m_fElapsedMs = watch.ElapsedMilliseconds;
                    pluginsObjBase.PluginParams.m_strRunMessage = "执行异常：" + ex.Message;
                }
            }
        }

        public virtual void Btn_Save_Click(object sender, EventArgs e)
        {
            if (pluginsObjBase == null || pluginsObjBase.moduleInfo == null) return;

            pluginsObjParams_Back = pluginsObjBase.PluginParams;
            string processPath = Path.Combine(Project.Instance.paramPathSet.ProcessPath, pluginsObjBase.moduleInfo.ProcessName);
            pluginsObjBase.moduleInfo.filePath = Path.Combine(processPath, pluginsObjBase.moduleInfo.ModuleName);
            if (pluginsObjBase.PluginParams != null) pluginsObjBase.PluginParams.filePath = pluginsObjBase.moduleInfo.filePath;
            pluginsObjBase.saveParams();
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
        }

        private void SubscribeExecuteCompleted()
        {
            if (pluginsObjBase == null || executeCompletedSubscribed) return;

            pluginsObjBase.ExecuteCompleted += OnPluginExecuteCompleted;
            executeCompletedSubscribed = true;
        }

        private void UnsubscribeExecuteCompleted()
        {
            if (pluginsObjBase == null || !executeCompletedSubscribed) return;

            try
            {
                pluginsObjBase.ExecuteCompleted -= OnPluginExecuteCompleted;
            }
            catch
            {
            }

            executeCompletedSubscribed = false;
        }

        private void OnPluginExecuteCompleted(object sender, PluginExecuteCompletedEventArgs e)
        {
            if (!ReferenceEquals(sender, pluginsObjBase)) return;

            if (IsHandleCreated && InvokeRequired)
            {
                BeginInvoke(new Action(() => OnPluginExecuteCompleted(sender, e)));
                return;
            }

            Tb_CostTime.Text = "耗时：" + e.ElapsedMs.ToString("0.0") + "ms";
            OnPluginExecutedFromOutside(e);
        }

        /// <summary>
        /// WinForm 插件外部执行完成后的扩展刷新点。
        /// 旧 WinForm 插件不重写也不影响原功能；需要后台刷新界面的 WinForm 插件可以重写此方法。
        /// </summary>
        protected virtual void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
        }

        private void FormPluginBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnsubscribeExecuteCompleted();
        }
    }
}
