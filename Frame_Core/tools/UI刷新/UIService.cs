using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using LogTool;

namespace Frame_Core
{    /// <summary>
     /// UI更新服务
     /// </summary>
    public class UIService
    {

        private static UIService instance = null;
        private static readonly object instanceObj = new object();

        /// <summary>
        /// UI更新服务实例
        /// </summary>
        public static UIService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceObj)
                        instance = new UIService();
                }
                return instance;
            }
        }

        /// <summary>
        /// 委托
        /// </summary>
        private delegate void updateUIDelegate();

        /// <summary>
        /// 执行代码队列
        /// </summary>
        private readonly ConcurrentQueue<Action> actionList = new ConcurrentQueue<Action>();
        /// <summary>
        /// 信号量
        /// </summary>
        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        /// <summary>
        /// 窗口对象
        /// </summary>
        private readonly Form form = null;
        /// <summary>
        /// 后台刷新线程优先度
        /// </summary>
        private ThreadPriority threadPriority = ThreadPriority.Normal;
        /// <summary>
        /// 后台线程休眠毫秒
        /// </summary>
        private int sleepTime = 100;
        /// <summary>
        /// 当外部生成刷新速度超过UI处理时，开启可以防止UI假死
        /// </summary>
        private bool forceRefresh = false;
        /// <summary>
        /// 后台线程
        /// </summary>
        private Thread thread = null;
        /// <summary>
        /// UI更新委托
        /// </summary>
        private updateUIDelegate updateUI = null;
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// 初始化UI更新服务
        /// </summary>
        /// <param name="refreshTime">刷新间隔(毫秒)</param>
        /// <param name="threadPriority">后台线程的优先级</param>
        /// <param name="forceRefresh">当外部生成刷新速度超过UI处理时，开启可以防止UI假死</param>
        /// <exception cref="Exception"></exception>
        public UIService(int refreshTime = 100, Form form = null, ThreadPriority threadPriority = ThreadPriority.Normal, bool forceRefresh = false)
        {

            if (instance != null)
                throw new Exception("UI更新服务已创建，请使用【UIService.Instance】获取");
            if (Application.OpenForms.Count == 0)
                throw new Exception("请在WinFrom环境下已创建窗口后使用此服务类");
            if (form == null)
            {
                this.form = Application.OpenForms[0];
            }
            else
            {
                this.form = form;
            }
            //this.form = Application.OpenForms[0];
            this.threadPriority = threadPriority;
            this.sleepTime = refreshTime;
            this.forceRefresh = forceRefresh;
            instance = this;
            //
            updateUI = new updateUIDelegate(UIUpdate);
            thread = new Thread(ThreadBackground);
            thread.IsBackground = true;
            thread.Name = "UI更新后台线程";
            thread.Priority = threadPriority;
            thread.Start();
        }

        /// <summary>
        /// 获取或设置刷新间隔（毫秒）
        /// </summary>
        public int RefreshTime
        {
            get
            {
                return sleepTime;
            }
            set
            {
                sleepTime = value;
            }
        }

        /// <summary>
        /// 获取或设置后台线程的优先级
        /// </summary>
        public ThreadPriority ThreadPriority
        {
            get
            {
                return threadPriority;
            }
            set
            {
                threadPriority = value;
                thread.Priority = value;
            }
        }

        /// <summary>
        /// 是否执行完成一次更新后强制刷新UI
        /// </summary>
        public bool ForceRefresh
        {
            get
            {
                return forceRefresh;
            }
            set
            {
                forceRefresh = value;
            }
        }

        #region 私有函数
        /// <summary>
        /// 更新时调用函数
        /// </summary>
        private void UIUpdate()
        {
            if (forceRefresh)
                stopWatch.Restart();
            Action action = null;
            //一次性把队列中待执行的代码全部执行完毕
            while (actionList.TryDequeue(out action))
            {
                try
                {
                    //执行代码
                    action();
                    //在开启强制UI刷新情况下。防止太过频繁调用UI更新，造成UI更新处理比不上外部，造成跳不出循环，后果为UI卡住不再更新，这时采用强制UI更新
                    if (forceRefresh && stopWatch.ElapsedMilliseconds > sleepTime)
                    {
                        Application.DoEvents();
                        stopWatch.Restart();
                    }
                }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Error("UI更新线程执行更新代码出错\n" + ex.Message);

                }

            }
        }

        private void ThreadBackground()
        {
            try
            {
                //由于单例和全局使用让线程跟随程序退出即可
                while (true)
                {
                    try
                    {
                        //等待有新的数据加入
                        autoResetEvent.WaitOne();

                        //调用UI更新
                        form.Invoke(updateUI);

                        //休眠指定时间
                        Thread.Sleep(sleepTime);
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error("UI更新后台线程出错退出\n" + ex.Message);
                    }

                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("UI更新后台线程出错退出\n" + ex.Message);

            }
        }
        #endregion

        #region 公有函数
        /// <summary>
        /// 新增更新代码到执行队列
        /// 由于此操作主要用于UI刷新，非必要不把多余代码加入
        /// </summary>
        /// <param name="action">执行代码（注：不要把多余代码加入）</param>
        public void AddAction(Action action)
        {
            actionList.Enqueue(action);
            autoResetEvent.Set();
        }

        #endregion
    }
}
