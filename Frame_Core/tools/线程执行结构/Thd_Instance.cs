using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 线程工作类，用于工作线程。
    /// </summary>
    public class Thd_WorkClass
    {
        #region 将当前线程的亲和性设置为指定的CPU。
        [DllImport("kernel32.dll")]
        static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        /// <summary>
        /// 为指定的CPU ID计算位掩码。
        /// </summary>
        /// <param name="id">CPU ID。</param>
        /// <returns>位掩码。</returns>
        static ulong SetCpuID(int id)
        {
            ulong cpuid = 0;
            if (id < 0 || id >= System.Environment.ProcessorCount)
            {
                id = 0;
            }
            cpuid |= 1UL << id; // 0,1,2,3 => 1,2,4,8
            return cpuid;
        }

        #endregion
        /// <summary>
        /// 将当前线程的亲和性设置为指定的CPU。
        /// </summary>
        /// <param name="cpuId">CPU ID。</param>
        private void SetThreadAffinity(int cpuId)
        {
            ulong mask = SetCpuID(cpuId);
            SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(mask));
        }

        private Thread workerThread;
        private bool isRunning;

        public event Action ThreadStarted;
        public event Action ThreadStopped;
        public event Action PerformWork; // 事件来处理工作内容

        /// <summary>
        /// 信号量最大数量。
        /// </summary>
        public int maxSemaphoreCOunt = 1024;
        /// <summary>
        /// 信号量，用于通知工作线程正确启动。
        /// </summary>
        public SemaphoreSlim m_semaphore = new SemaphoreSlim(0, 1024);


        /// <summary>
        /// 获取工作线程是否正在运行的状态。
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning && workerThread != null && workerThread.IsAlive; }
        }

        public Thd_WorkClass()
        {
            isRunning = false;
        }

        /// <summary>
        /// 发布信号量，通知工作线程启动。
        /// </summary>
        public void ReleaseSemaphore()
        {
            if (m_semaphore.CurrentCount < maxSemaphoreCOunt)
                m_semaphore.Release();
        }
        /// <summary>
        /// 启动工作线程并可选地设置其CPU亲和性。
        /// </summary>
        /// <param name="intervalTime">线程执行间隔时间</param>
        /// <param name="thdName">线程名称</param>
        /// <param name="cpuId">线程应运行的CPU ID（如果设置亲和性）。</param>
        /// <param name="setAffinity">是否设置CPU亲和性。</param>
        public void Start(int intervalTime = 5, string thdName = "", int cpuId = 0, bool setAffinity = false)
        {
            if (workerThread == null || !workerThread.IsAlive)
            {
                if (m_semaphore == null) m_semaphore = new SemaphoreSlim(0, maxSemaphoreCOunt);
                workerThread = new Thread(() => ThreadProc(intervalTime, cpuId, setAffinity))
                {
                    IsBackground = true
                };

                if (!string.IsNullOrEmpty(thdName))
                {
                    workerThread.Name = thdName;
                }

                isRunning = true;
                workerThread.Start();

                ThreadStarted?.Invoke();
            }
        }


        HTuple thread_Startime = new HTuple();
        HTuple thread_Endtime = new HTuple();
        double _ExecuteTime;
        /// <summary>
        /// 线程执行时间
        /// </summary>
        public double ExecuteTime { get { return _ExecuteTime; } }

        /// <summary>
        /// 工作线程的主体方法，执行实际的工作。
        /// </summary>
        /// <param name="intervalTime">线程执行间隔时间</param>
        /// <param name="cpuId">CPU ID（如果设置亲和性）。</param>
        /// <param name="setAffinity">是否设置CPU亲和性。</param>
        private void ThreadProc(int intervalTime, int cpuId, bool setAffinity)
        {
            try
            {
                if (setAffinity)
                {
                    SetThreadAffinity(cpuId);
                }

                while (isRunning)
                {
                    try
                    {
                        HOperatorSet.CountSeconds(out thread_Startime);
                        // 触发执行工作的事件
                        PerformWork?.Invoke();
                        HOperatorSet.CountSeconds(out thread_Endtime);
                        _ExecuteTime = ((thread_Endtime - thread_Startime) * 1000).D;

                        if (Thread.CurrentThread.Name == "Communication")
                        {

                        }
                        // 工作间隔
                        Thread.Sleep(intervalTime);
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 停止工作线程。
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            //workerThread?.Join();
            ThreadStopped?.Invoke();
        }
    }
}
