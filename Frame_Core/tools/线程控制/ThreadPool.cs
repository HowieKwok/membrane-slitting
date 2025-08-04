
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LogTool;

using Priority_Queue;

namespace Frame_Core
{
    /// <summary>
    /// 限定线程池（支持线程模式或任务模式）
    /// 当要执行实时代码时使用线程模式
    /// 当要执行非实时代码时使用任务模式
    /// 默认为任务模式
    /// </summary>
    public class LimitThreadPool
    {
     
        /// <summary>
        /// 任务线程最大限制数
        /// </summary>
        private readonly int taskMaxCount = 1;
        /// <summary>
        /// 起始线程数
        /// </summary>
        private readonly int taskStartCount = 1;
        /// <summary>
        /// 当前任务线程数
        /// </summary>
        private int taskCount = 0;
        /// <summary>
        /// 线程优先级
        /// </summary>
        private readonly ThreadPriority threadPriority = ThreadPriority.Normal;
        /// <summary>
        /// 辅助线程
        /// </summary>
        private Thread thOp = null;
        /// <summary>
        /// 线程列表
        /// </summary>
        private readonly ConcurrentDictionary<int, Thread> threadList = null;
        /// <summary>
        /// 执行任务的模式，线程模式或任务模式
        /// </summary>
        private QueueTaskMode taskMode = QueueTaskMode.Task;
        /// <summary>
        /// 待执行任务代码队列，有优先级
        /// </summary>
        private readonly SimplePriorityQueue<InvokeResult, int> list = new SimplePriorityQueue<InvokeResult, int>();
        /// <summary>
        /// 信号量
        /// </summary>
        private readonly AutoResetEvent autoResetEvent = null;
        /// <summary>
        /// 是否要停止
        /// </summary>
        private bool isStop = false;
        /// <summary>
        /// 正在运行的任务数
        /// </summary>
        private int runningTaskCount = 0;
        /// <summary>
        /// 阶段性正在运行的最大任务数
        /// </summary>
        private int stageRunningMaxTaskCount = 0;
        /// <summary>
        /// 阶段性存储正在运行线程数量队列（使用队列是因为多线程下如果直接计算得到的阶段性最大运行线程数是不准确的）
        /// </summary>
        private ConcurrentQueue<int> stageRunningTaskCountList = null;
        /// <summary>
        /// 辅助线程休眠毫秒
        /// </summary>
        private int sleep = 100;
        /// <summary>
        /// 线程阻塞等待毫秒，可视为检查调整运行线程数量的时间间隔
        /// </summary>
        private int waitTime = 1000;
        /// <summary>
        /// 空闲时间（秒），当运行任务的线程数一定时间内都小于线程数，可认为是空闲状态，可以关闭回收一些线程
        /// </summary>
        private int idleTimeSecond = 60 * 5;
        /// <summary>
        /// 检查增加线程数量的标志位
        /// </summary>
        private int checkAddThreadCountFlag = 0;
        /// <summary>
        /// 检查减少线程数量的标志位
        /// </summary>
        private int checkReduceThreadCountFlag = 0;
        /// <summary>
        /// 检查阶段性最大运行任务线程标志位
        /// </summary>
        private int checkStageRunningMaxTaskCount = 0;
        /// <summary>
        /// 最后繁忙时间
        /// </summary>
        private DateTime busyTime = DateTime.Now;
        /// <summary>
        /// 是否为智能模式（线程数量智能自动调整）
        /// </summary>
        private bool isSmart = false;
        /// <summary>
        /// 增加新线程的Action
        /// </summary>
        private Action addTreadAction = null;


        /// <summary>
        /// 初始化限定线程池
        /// </summary>
        /// <param name="taskMaxCount">限定最大同时执行数量</param>
        /// <param name="taskMode">使用任务模式或线程模式</param>
        /// <param name="priotity">当第2参数为线程模式时，设置线程的优先度.为任务模式时为辅助线程的优先度</param>
        /// <param name="sleep">当第2参数为任务模式时，辅助线程休眠毫秒</param>
        public LimitThreadPool(int taskMaxCount, QueueTaskMode taskMode = QueueTaskMode.Task, ThreadPriority priotity = ThreadPriority.Normal, int sleep = 100)
        {
            this.taskMaxCount = this.taskStartCount = taskMaxCount;
            this.taskMode = taskMode;
            this.threadPriority = priotity;
            this.sleep = sleep;
            if (taskMode == QueueTaskMode.Task)
            {
                thOp = new Thread(ThreadOP);
                thOp.IsBackground = true;
                thOp.Name = "队列线程池辅助线程";
                thOp.Priority = priotity;
                thOp.Start();
            }
            else
            {
                addTreadAction = new Action(AddThread);
                autoResetEvent = new AutoResetEvent(false);
                threadList = new ConcurrentDictionary<int, Thread>();
                for (int i = 0; i < taskMaxCount; i++)
                {
                    Thread th = new Thread(ThreadInvoke);
                    th.IsBackground = true;
                    th.Name = "智能限定线程固定池执行线程";
                    th.Priority = priotity;
                    th.Start();
                    threadList.TryAdd(th.ManagedThreadId, th);
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// 初始化限定线程池（此初始化函数只会使用线程模式）
        /// </summary>
        /// <param name="taskStartCount">起始线程数量</param>
        /// <param name="taskMaxCount">限定最大线程数量</param>
        /// <param name="priotity">线程池中线程的优先级</param>
        public LimitThreadPool(int taskStartCount, int taskMaxCount, ThreadPriority priotity = ThreadPriority.Normal)
        {
            this.taskStartCount = taskStartCount;
            this.taskMaxCount = taskMaxCount;
            this.taskMode = QueueTaskMode.Thread;
            this.threadPriority = priotity;

            this.isSmart = true;
            addTreadAction = new Action(AddThread);
            autoResetEvent = new AutoResetEvent(false);
            threadList = new ConcurrentDictionary<int, Thread>();
            stageRunningTaskCountList = new ConcurrentQueue<int>();
            //启动线程
            for (int i = 0; i < taskStartCount; i++)
            {
                Thread th = new Thread(ThreadInvoke);
                th.IsBackground = true;
                th.Name = "智能限定线程池起始执行线程";
                th.Priority = priotity;
                th.Start();
                threadList.TryAdd(th.ManagedThreadId, th);
                Thread.Sleep(5);
            }
        }

        #region 属性

        /// <summary>
        /// 起始线程数量
        /// </summary>
        public int TaskStartCount
        {
            get
            {
                return taskStartCount;
            }
        }

        /// <summary>
        /// 限定最大线程数量
        /// </summary>
        public int TaskMaxCount
        {
            get
            {
                return taskMaxCount;
            }
        }

        /// <summary>
        /// 任务模式
        /// </summary>
        public QueueTaskMode TaskMode
        {
            get
            {
                return taskMode;
            }
        }

        /// <summary>
        /// 线程池线程优先度
        /// </summary>
        public ThreadPriority ThreadPriority
        {
            get
            {
                return threadPriority;
            }
        }

        /// <summary>
        /// 当前任务线程数量
        /// </summary>
        public int CurrentTaskCount
        {
            get
            {
                return taskCount;
            }
        }

        /// <summary>
        /// 此刻正在执行任务的线程数量
        /// </summary>
        public int RunningTaskCount
        {
            get
            {
                return runningTaskCount;
            }
        }

        /// <summary>
        /// 当执行队列中没有数据，线程阻塞超时时间
        /// 该值影响检查空闲线程的频率（此值只在线程空闲时才起效）
        /// </summary>
        public int WaitTime
        {
            get
            {
                return waitTime;
            }
            set
            {
                waitTime = value;
            }
        }

        /// <summary>
        /// 空闲清理时间（单位秒）
        /// 线程池中有多余空闲线程时，如果空闲时间达到该值，会发起清理空闲线程功能
        /// </summary>
        public int IdleTimeSecond
        {
            get
            {
                return idleTimeSecond;
            }
            set
            {
                idleTimeSecond = value;
            }
        }

        #endregion

        #region 私有函数
        //任务模式下辅助线程
        private void ThreadOP()
        {
            try
            {
                while (!isStop)
                {
                    if (runningTaskCount < taskMaxCount)
                    {
                        TaskInvoke(true);
                    }

                    Thread.Sleep(sleep);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error( "任务模式下辅助线程运行出错\n"+ex.Message);
                
            }
        }

        //任务模式
        private void TaskInvoke(bool newTask)
        {
            InvokeResult action;
            if (list.TryDequeue(out action))
            {
                if (newTask)
                    Interlocked.Increment(ref runningTaskCount);
                Task task = new Task(() =>
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error("任务模式下执行出错\n"+ ex.Message);
                      
                    }
                });
                //任务完成后立即检查是否还有未被执行代码，加快队列执行
                task.ContinueWith((t) => TaskInvoke(false));
                task.Start();
            }
            else if (!newTask)
            {
                Interlocked.Decrement(ref runningTaskCount);
            }
        }

        //线程模式下线程循环函数
        private void ThreadInvoke()
        {
            //线程数加1
            int count = Interlocked.Increment(ref taskCount);
            //当前线程ID
            int threadId = Thread.CurrentThread.ManagedThreadId;
            //使用Debu等级打印，日志默认显示等级为Info，会忽略此消息打印，如想查看请把日志等级调整为Debug
            Log4netRecord.ErrorLog.Debug($"智能限定线程池已创建执行线程=> [线程ID：{threadId}, 当前线程数：{count}条]");
       
            InvokeResult action = null;
            int runCount = 0;
            try
            {
                while (!isStop)
                {
                    if (autoResetEvent.WaitOne(waitTime))
                    {
                        //由于收到信号量而通过阻塞，即有数据

                        if (isSmart)
                        {
                            //正在运行的线程数量加1
                            count = Interlocked.Increment(ref runningTaskCount);
                            if (count >= taskCount)
                            {
                                //设置繁忙时间
                                busyTime = DateTime.Now;
                            }
                            else
                            {
                                //把正在运行任务的线程数量加入队列，为计算阶段性最大运行任务线程数做准备
                                stageRunningTaskCountList.Enqueue(count);

                            }
                        }
                        //循环处理工作
                        while (list.TryDequeue(out action))
                        {
                            try
                            {
                                //执行代码
                                action.Invoke();
                            }
                            catch (ThreadAbortException)
                            {
                                return;
                            }
                            catch (Exception e)
                            {
                                Log4netRecord.ErrorLog.Error("线程模式下执行出错\n"+ e.Message);

                               
                            }

                            //运行完一个任务代码，开始检查是要增加线程数量
                            CheckAddThread();

                        }
                        if (isSmart)
                        {
                            //正在运行的线程数量减1
                            Interlocked.Decrement(ref runningTaskCount);
                        }
                    }
                    else if (isSmart)  //只有在智能模式下才做检查
                    {
                        //保证只有一条线程能进入判断条件
                        if (Interlocked.CompareExchange(ref checkStageRunningMaxTaskCount, 1, 0) == 0)
                        {
                            //从队列中获取运行线程数并计算最大值
                            while (stageRunningTaskCountList.TryDequeue(out runCount))
                                stageRunningMaxTaskCount = Math.Max(stageRunningMaxTaskCount, runCount);
                            //恢复标志位
                            Interlocked.Exchange(ref checkStageRunningMaxTaskCount, 0);
                        }

                        //因为超时通过阻塞，这时是没有任务的，可以认为是空闲时刻，可以用来做检查
                        if (CheckReduceThread())
                        {
                            //满足可减少线程条件，结束当前线程
                            return;
                        }
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("线程模式下执行线程出错\n"+ ex.Message);
              
            }
            finally
            {
                //线程数减1
                count = Interlocked.Decrement(ref taskCount);
                //从线程列表中删除
                Thread th;
                threadList.TryRemove(threadId, out th);
                Log4netRecord.ErrorLog.Debug($"智能限定线程池中线程ID为：{threadId}的线程已结束！剩余线程数：{count}");
                //线程退出后恢复检查标志位
                Interlocked.Exchange(ref checkReduceThreadCountFlag, 0);
            }
        }

        /// <summary>
        /// 检查是否要减少线程
        /// </summary>
        private bool CheckReduceThread()
        {
            /* 线程数量大于起始线程数量
             * 正在运行任务线程小线程数量减1
             * 最后的繁忙时间已过去指定的空闲秒数，到达结束和可减少线程
             * 当正在运行任务线程数为0或正在运行任务线程数少阶段性最大任务运行数
             */
            if (taskCount > taskStartCount && runningTaskCount < taskStartCount - 1 && DateTime.Now >= busyTime.AddSeconds(idleTimeSecond) && (runningTaskCount == 0 || runningTaskCount < stageRunningMaxTaskCount))
            {
                if (Interlocked.CompareExchange(ref checkReduceThreadCountFlag, 1, 0) == 0)
                {
                    //重置阶段性最大运行任务线程数
                    Interlocked.Exchange(ref stageRunningMaxTaskCount, 0);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否要增加线程
        /// </summary>
        private void CheckAddThread()
        {
            //不是智能模式直接跳过
            if (!isSmart)
                return;
            /* 线程数量小于最大限制数量
             * 正在执行线程数量大于等于线程数量
             * 待处理任务代码大于1个（该线程已运行完一个任务代码了，立即可以运行下一个）
             * 此条件为激进模式，发现待处理任务代码数量有可能多出会立即创建线程去处理
             */
            if (taskCount < taskMaxCount && runningTaskCount >= taskCount && list.Count > 1)
            {
                //原子判断，保证在创建一条线程后其它线程才能进入条件
                if (Interlocked.CompareExchange(ref checkAddThreadCountFlag, 1, 0) == 0)
                {
                    //从线程池中运行创建线程代码，创建线程比较耗资源时间，交给后台线程去处理，不耗当前线程工作
                    Task.Run(addTreadAction);
                }
            }
        }


        /// <summary>
        /// 添加新线程
        /// </summary>
        private void AddThread()
        {
            Thread th = new Thread(ThreadInvoke);
            th.IsBackground = true;
            th.Name = "智能限定线程池动态执行线程";
            th.Priority = threadPriority;
            threadList.TryAdd(th.ManagedThreadId, th);
            th.Start();

            //新创建的任务有可能因为没有新数据而被信号量卡住，不能立即处理，所以判断如果还有任务数可以立即设置信息量
            if (list.Count > 0)
                autoResetEvent.Set();

            //要错开创建线程时间
            Thread.Sleep(5);
            //把可以再创建线程的标志位恢复
            Interlocked.Exchange(ref checkAddThreadCountFlag, 0);
        }
        #endregion

        #region 公有函数
        /// <summary>
        /// 添加执行代码
        /// </summary>
        /// <param name="action">执行代码</param>
        /// <param name="priority">优先度，数值越小越先执行</param>
        /// <returns>返回一个执行结果类，可以判断是否执行完成或等待执行完成</returns>
        public InvokeResult RunAction(Action action, int priority = 10)
        {
            InvokeResult result = new InvokeResult(action);
            list.Enqueue(result, priority);
            autoResetEvent?.Set();
            return result;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Close()
        {
            try
            {
                isStop = true;
                list.Clear();
                if (taskMode == QueueTaskMode.Thread)
                {
                    list.Clear();
                    for (int i = 0; i < taskMaxCount; i++)
                        autoResetEvent.Set();
                    Thread.Sleep(5);
                    foreach (Thread th in threadList.Values)
                    {
                        try
                        {
                            th.Abort();
                            th.Join(100);

                        }
                        catch { }
                    }
                    threadList.Clear();
                    if (isSmart)
                    {
                        int n;
                        while (stageRunningTaskCountList.TryDequeue(out n)) ;
                        stageRunningTaskCountList = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("停止智能限定线程池出错\n" + ex.Message);
             
            }
            finally
            {
                autoResetEvent?.Close();
                thOp = null;
            }
        }
        #endregion

    }

    /// <summary>
    /// 执行完成结果类
    /// </summary>
    public class InvokeResult
    {
        /// <summary>
        /// 等待锁队列，等待锁可重复利用
        /// </summary>
        private static readonly ConcurrentQueue<ManualResetEvent> eventList = new ConcurrentQueue<ManualResetEvent>();
        /// <summary>
        /// 是否执行完成
        /// </summary>
        private bool isCompleted = false;
        private int completedFlag = 0;
        private Action action = null;
        private ManualResetEvent manualResetEvent = null;
        private object objLock = new object();


        internal InvokeResult(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            this.action = action;
        }

        internal void Invoke()
        {
            try
            {
                action();

            }
            finally
            {
                //设置已完成
                isCompleted = true;
                Interlocked.Exchange(ref completedFlag, 1);
                manualResetEvent?.Set();
            }
        }

        /// <summary>
        /// 任务代码是否已执行完成
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
        }

        /// <summary>
        /// 等待任务执行完成
        /// </summary>
        /// <returns>是否是执行完成，true:执行完成,false:超时</returns>
        public bool WaitOne()
        {
            return WaitOne(0);
        }

        /// <summary>
        /// 等待任务执行完成
        /// </summary>
        /// <param name="millisecondsTimeout">超时时间</param>
        /// <returns>是否是执行完成，true:执行完成,false:超时</returns>
        public bool WaitOne(int millisecondsTimeout = 0)
        {
            try
            {
                //如果任务代码已执行完成，立即返回
                if (Interlocked.CompareExchange(ref completedFlag, 1, 1) == 1)
                    return true;
                //防止外部多处调用等待，上锁
                lock (objLock)
                {
                    //判断等待锁
                    if (manualResetEvent == null)
                    {
                        //尝试从等待锁队列获取等待锁
                        if (eventList.TryDequeue(out manualResetEvent))
                        {
                            //重置
                            manualResetEvent.Reset();
                            Debug.WriteLine("从队列中获取并重置");
                        }
                        else
                        {
                            //没有获取到，新建一个
                            manualResetEvent = new ManualResetEvent(false);
                            Debug.WriteLine("新创建等待锁");
                        }
                    }
                }
                //如果任务代码已执行完成，立即返回
                if (Interlocked.CompareExchange(ref completedFlag, 1, 1) == 1)
                    return true;
                //调用等待
                if (millisecondsTimeout <= 0)
                    return manualResetEvent.WaitOne();
                return manualResetEvent.WaitOne(millisecondsTimeout);
            }
            finally
            {
                //把等待锁放入队列，以便可以重复利用
                if (manualResetEvent != null)
                    eventList.Enqueue(manualResetEvent);
            }
        }
    }

    /// <summary>
    /// 执行模式（线程或任务）
    /// </summary>
    public enum QueueTaskMode
    {
        Thread = 1,
        Task = 2
    }
}
