using FrameCommunity.Data;
using LogTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frame_Core
{
    public delegate void DelegateINIProcess(Process process);
    public delegate void DelegateRUNProcess(Process process);

    public class ProcessServices
    {
        /// <summary>
        /// 当前流程服务持有的流程对象，里面包含模块列表、流程信息和模块输出缓存。
        /// </summary>
        public Process process { get; set; }

        /// <summary>
        /// 流程后台线程。一个 ProcessServices 只允许存在一条流程线程。
        /// </summary>
        private Thread m_Thread;

        /// <summary>
        /// 流程运行状态标志。
        /// true 表示线程被唤醒后会执行流程；false 表示线程进入等待状态。
        /// </summary>
        private volatile bool m_ThreadStatus = false;

        /// <summary>
        /// 线程创建和关闭锁，防止 OpeningThread 与 CloseThread 同时操作线程对象。
        /// </summary>
        private readonly object m_ThreadLock = new object();

        /// <summary>
        /// 线程退出标志。
        /// CloseThread 设置为 true 后，Workflow_Manual 会跳出循环并自然结束。
        /// </summary>
        private volatile bool m_ExitThread = false;

        /// <summary>
        /// 输入图像原
        /// </summary>
        public ImageInfo ImageSource { get; set; } = new ImageInfo();






        /// <summary>
        /// 当前运行模式：None 停止、RunOnce 单次、RunCycle 连续。
        /// </summary>
        public RunMode RunMode { set; get; } = RunMode.None;

        /// <summary>
        /// 流程线程等待/唤醒信号。
        /// Start 或 CloseThread 会 Set，避免线程长期阻塞在 WaitOne。
        /// </summary>
        private AutoResetEvent m_AutoResetEvent = new AutoResetEvent(false);//控制流程

        /// <summary>
        /// 创建流程服务时同步创建一个空 Process，后续由 BuildProcessServiceList 设置流程名、绑定事件并初始化模块。
        /// </summary>
        public ProcessServices()
        {
            process = new Process();
        }
        //public ProcessServices(Process _Process)
        //{
        //    Process = _Process;
        //}
        public event DelegateINIProcess m_eventINIProcess;
        public event DelegateRUNProcess m_eventRUNProcess;

        /// <summary>
        /// 打开流程后台线程。
        /// 如果线程已经存在并存活，则直接返回，防止重复调用 OpeningThread 时创建多条流程线程。
        /// </summary>
        public void OpeningThread()
        {
            lock (m_ThreadLock)
            {
                if (m_Thread != null && m_Thread.IsAlive)
                {
                    return;
                }

                m_ExitThread = false;
                m_Thread = new Thread(Workflow_Manual);
                m_Thread.Name = process.processInfo.ProcessName + " : ProduceThread";
               // m_Thread.Priority = ThreadPriority.Normal;
                m_Thread.IsBackground = true;
                m_Thread.Priority = ThreadPriority.Highest;
                m_Thread.Start();
            }
        }
        /// <summary>
        /// 启动或唤醒流程运行。
        /// 该方法只改变运行状态并唤醒线程，具体连续/单次由 RunMode 决定。
        /// </summary>
        public void Start()
        {
            m_ThreadStatus = true;
            m_AutoResetEvent.Set();
        }
        /// <summary>
        /// 停止流程执行。
        /// 注意：这里只把运行状态置为 false，不关闭线程；线程会等待下一次 Start。
        /// </summary>
        public void Stop()
        {
            m_ThreadStatus = false;
        }
        /// <summary>
        /// 获取当前流程运行状态。
        /// </summary>
        /// <returns>true 表示流程处于运行状态；false 表示已停止或等待。</returns>
        public bool GetThreadStatus()
        {
            return m_ThreadStatus;
        }

        /// <summary>
        /// 安全关闭流程线程。
        /// 通过退出标志和 AutoResetEvent 唤醒阻塞线程，让 Workflow_Manual 自然退出，避免使用 Thread.Suspend/Abort 带来的死锁和资源破坏。
        /// </summary>
        /// <returns>线程已退出或原本不存在返回 true；超时仍未退出返回 false。</returns>
        public bool CloseThread()
        {
            Thread thread = null;

            lock (m_ThreadLock)
            {
                m_ThreadStatus = false;
                RunMode = RunMode.None;
                m_ExitThread = true;
                m_AutoResetEvent.Set();
                thread = m_Thread;
            }

            if (thread != null && thread.IsAlive)
            {
                try
                {
                    thread.Join(1000);
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Error("流程线程关闭异常:" + ex);
                }
            }

            lock (m_ThreadLock)
            {
                // 只有线程确认退出后才清空引用；如果 Join 超时仍在运行，则保留引用，防止 OpeningThread 再开出第二条流程线程。
                if (m_Thread != null && !m_Thread.IsAlive)
                {
                    m_Thread = null;
                }

                if (m_Thread != null && m_Thread.IsAlive)
                {
                    Log4netRecord.ErrorLog.Error("流程线程关闭超时，已请求退出但线程仍在运行:" + m_Thread.Name);
                    return false;
                }
            }

            return true;
        }


        private int m_nMode = 0;

        /// <summary>
        /// 流程后台线程主循环。
        /// 线程空闲时阻塞等待 Start 唤醒；运行时触发 m_eventRUNProcess，由业务类执行具体流程逻辑。
        /// CloseThread 设置退出标志后，本循环会在下一次检查时退出。
        /// </summary>
        private void Workflow_Manual()
        {

            // 创建一个Stopwatch实例
            Stopwatch stopwatch = new Stopwatch();
            // 获取经过的时间（以毫秒为单位）
            TimeSpan ts = stopwatch.Elapsed;
            while (!m_ExitThread)
            {
                if (m_ThreadStatus == false && m_nMode == 0)
                {
                    m_nMode = 0;
    
                    m_AutoResetEvent.WaitOne();//阻塞等待
                    if (m_ExitThread) break;
                }
                else
                {
                    ExecuteProcess();
                    if (RunMode == RunMode.RunCycle) { Thread.Sleep(5); };//连续运行设置延时时间,以免速度过快

                    if (RunMode == RunMode.RunOnce)
                    {
                        RunMode = RunMode.None;
                        Stop();

                    }

                    //Thread.Sleep(300);
                }
            }
        }

        /// <summary>
        /// 触发流程初始化事件。
        /// BuildProcessServiceList 绑定的 m_eventINIProcess 会在这里执行，用于向流程中添加标准模块。
        /// </summary>
        public void InitProcess()
        {
            if (m_eventINIProcess != null)
            {
                m_eventINIProcess(process);
            }
        }

        /// <summary>
        /// 触发流程运行事件。
        /// 业务类在 m_eventRUNProcess 中执行相机取图、算法模块运行、结果输出等具体逻辑。
        /// </summary>
        public void ExecuteProcess()
        {
            if (m_eventRUNProcess != null)
            {
                m_eventRUNProcess(process);
            }
        }



        #region 指定线程到不同内核

        //得到当前线程的handler
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        static extern UIntPtr SetThreadAffinityMask(IntPtr hThread,
        UIntPtr dwThreadAffinityMask);

        /// <summary>
        /// 将当前线程绑定到指定 CPU 核心。
        /// 当前流程线程未启用该能力，保留旧扩展方法。
        /// </summary>
        /// <param name="obj">CPU 核心编号。</param>
        private void ThreadBindCPU(object obj)
        {
            int core = (int)obj;

            int len = 10000000;
            uint[] data = new uint[len];

            //将当前线程绑定到指定的cpu核心上
            SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(core)));
        }

        /// <summary>
        /// 根据 CPU 核心编号生成线程亲和性掩码。
        /// </summary>
        /// <param name="id">CPU 核心编号。</param>
        /// <returns>线程亲和性掩码。</returns>
        static ulong SetCpuID(int id)
        {
            ulong cpuid = 0;
            if (id < 0 || id >= System.Environment.ProcessorCount)
            {
                id = 0;
            }
            cpuid |= 1UL << id;

            return cpuid;
        }
        #endregion
    }
}
