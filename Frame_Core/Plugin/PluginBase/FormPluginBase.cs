using Frame_Core.toosls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public partial class FormPluginBase : Form, IPluginActionNotifier
    {

        /// <summary>
        /// 窗体语言翻译
        /// </summary>
        public LanguageEnum FormLanguage /*= Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType*/;
        /// <summary>
        /// ui对应的moduleobj
        /// </summary>
        public IPlugin pluginsObjBase { get; set; }
        /// <summary>
        /// 备份 取消的时候还原
        /// </summary>
        public PluginParamsBase pluginsObjParams_Back { get; set; } //域内缓存参数，也可做备份参数
        public event EventHandler<PluginViewActionResult> ActionCompleted;
        public FormPluginBase()
        {
            InitializeComponent();

        }
        public virtual void Init()
        {
            //使用pluginsObjParams刷新插件，参数控件。。。之类。                      
            this.Text = pluginsObjBase.moduleInfo.ModuleName;
        }

        private void FormPluginBase_Load(object sender, EventArgs e)
        {
            if (pluginsObjBase == null) return;
            //深拷贝对象，可提供撤销操作，暂时使用参数备份，如果引用类型有干扰，就使用深拷贝-2023.03.03 by：QM。
            // pluginsObjParams_Back = CloneObject.DeepCopy(pluginsObjBase.PluginParams);
            // pluginsObjParams_Back = pluginsObjBase.PluginParams;//初始时更新备份参数        
            Init();
            //插件翻译
            LanguageProject.ChangeLanguage(FormLanguage, this);
        }

        public virtual void Btn_RUN_Click(object sender, EventArgs e)
        {
            // 插件 ExecuteObj() 负责把真实运行结果写入 PluginParams。
            // 基类只负责显示本窗体耗时，并通知外层调试界面重新读取 PluginParams。
            Stopwatch watch = new Stopwatch();
            PluginViewActionResult result = null;
            try
            {
                watch.Start();
                bool success = pluginsObjBase.ExecuteObj();
                watch.Stop();

                long mSeconds = watch.ElapsedMilliseconds;
                Tb_CostTime.Text = "耗时：" + mSeconds.ToString("0.0") + "ms";
                result = success ? PluginViewActionResult.Ok("执行完成", mSeconds) : PluginViewActionResult.Fail("执行失败", mSeconds);
                pluginsObjBase.PluginParams.m_bRunSuccess = result.Success;
                pluginsObjBase.PluginParams.m_fElapsedMs = result.ElapsedMs;
                pluginsObjBase.PluginParams.m_strRunMessage=result.Message;
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                long mSeconds = watch.ElapsedMilliseconds;
                Tb_CostTime.Text = "耗时：" + mSeconds.ToString("0.0") + "ms";
                result = PluginViewActionResult.Fail("执行异常：" + ex.Message, mSeconds);
            }
            finally
            {
                RaiseActionCompleted(result);
            }
        }

        protected virtual void RaiseActionCompleted(PluginViewActionResult result)
        {
            EventHandler<PluginViewActionResult> handler = ActionCompleted;
            if (handler != null) handler(this, result);
        }
        public virtual void Btn_Save_Click(object sender, EventArgs e)
        {
            pluginsObjParams_Back = pluginsObjBase.PluginParams; //保存时更新备份参数
            string ProcessPath = Path.Combine(Project.Instance.paramPathSet.ProcessPath, pluginsObjBase.moduleInfo.ProcessName);
            pluginsObjBase.moduleInfo.filePath = Path.Combine(ProcessPath, pluginsObjBase.moduleInfo.ModuleName);
            pluginsObjBase.saveParams();
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            //pluginsObjBase.PluginParams = pluginsObjParams_Back;//保存时更新备份参数
            //Init();//刷新插件
        }

        // 窗体翻译
        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    // 在窗体加载时切换资源
        //    LanguageProject.ChangeLanguage(FormLanguage, this);
        //}

    }
}
