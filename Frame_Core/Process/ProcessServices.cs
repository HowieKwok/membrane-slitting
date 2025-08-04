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
        public Process process { get; set; }
        //线程控制
        private Thread m_Thread;
        private bool m_ThreadStatus = false;

        /// <summary>
        /// 输入图像原
        /// </summary>
        public ImageInfo ImageSource { get; set; } = new ImageInfo();






        public RunMode RunMode { set; get; } = RunMode.None;//运行模式
        private AutoResetEvent m_AutoResetEvent = new AutoResetEvent(false);//控制流程
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

        public ProductBase GetProductInfo()
        {
            return process.ProductInfo;
        }



        public QueuePackage<ShowHalconWindow> GetShowHalconWindows()
        {
            return process.ShowHalconWindows;
        }

        public QueuePackage<NgTypeInfo> GetNGTypeInfos()
        {
            return process.NGTypeInfos;
        }


        public void OpeningThread()
        {
            m_Thread = new Thread(Workflow_Manual);
            m_Thread.Name = process.processInfo.ProcessName + " : ProduceThread";
            // m_Thread.Priority = ThreadPriority.Normal;
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }
        public void Start()
        {
            m_ThreadStatus = true;
            m_AutoResetEvent.Set();
        }
        public void Stop()
        {
            m_ThreadStatus = false;
        }
        public bool GetThreadStatus()
        {
            return m_ThreadStatus;
        }
        ///// <summary>
        ///// 流程运行
        ///// </summary>
        //private void Workflow()
        //{
        //    while (true)
        //    {
        //        if (m_ThreadStatus == false)
        //        {
        //            m_AutoResetEvent.WaitOne();//阻塞等待
        //        }
        //        else
        //        {
        //            if (RunMode == RunMode.RunCycle) Thread.Sleep(100);//连续运行设置延时时间,以免速度过快

        //            foreach (IPlugin item in m_ModuleObjList)
        //            {
        //                item.ExecuteObj();
        //                if (m_ThreadStatus == false) break;//退出流程
        //            }

        //            if (RunMode == RunMode.RunOnce)
        //            {
        //                RunMode = RunMode.None;
        //                Stop();
        //            }
        //        }
        //    }
        //}

        private int m_nMode = 0;
        /// <summary>
        /// 流程运行--人工执行
        /// </summary>
        private void Workflow_Manual()
        {

            // 创建一个Stopwatch实例
            Stopwatch stopwatch = new Stopwatch();
            // 获取经过的时间（以毫秒为单位）
            TimeSpan ts = stopwatch.Elapsed;
            while (true)
            {
                if (m_ThreadStatus == false && m_nMode == 0)
                {
                    m_nMode = 0;
                    //单次运行时，工艺处理流程多次循环
                    //if (Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阳极单面制片CS
                    //    || Project.Instance.systemInfo.CurrentMaterial == MaterialCrafEnum.叠片阳极双面制片CD)
                    //{
                    //    if (process.processInfo.ProcessID == 1 || process.processInfo.ProcessID == 3)
                    //    {
                    //        m_nMode = 1;
                    //    }
                    //}
                    m_AutoResetEvent.WaitOne();//阻塞等待
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

        public void InitProcess()
        {
            m_eventINIProcess(process);
        }

        public void ExecuteProcess()
        {
            m_eventRUNProcess(process);
        }



        #region 指定线程到不同内核

        //得到当前线程的handler
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        static extern UIntPtr SetThreadAffinityMask(IntPtr hThread,
        UIntPtr dwThreadAffinityMask);

        private void ThreadBindCPU(object obj)
        {
            int core = (int)obj;

            int len = 10000000;
            uint[] data = new uint[len];

            //将当前线程绑定到指定的cpu核心上
            SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(core)));
        }

        //获取cpu的id号
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
