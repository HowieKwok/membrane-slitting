using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using NLog;
using LogTool;

namespace Frame_Core
{
    /// <summary>
    /// CSV文件写入类
    /// </summary>
    public class CsvWriter
    {
        /// <summary>
        /// 静态全局资源，
        /// </summary>
        private static readonly HashSet<CsvWriter> csvList = new HashSet<CsvWriter>();
        //后台线程
        private static Thread csvThread = null;
        /// <summary>
        /// 通知信号量
        /// </summary>
        private static readonly AutoResetEvent csvEvent = new AutoResetEvent(false);
        /// <summary>
        /// 锁对象
        /// </summary>
        private static readonly object objLock = new object();

        /// <summary>
        /// 静态初始化
        /// </summary>
        static CsvWriter()
        {
            //启动线程，注意：线程非后台线程，由于CSV表格为重要数据，不能丢失，后台线程会随窗口关闭自动终止，为防止数据丢失使用前台线程，
            //并检查所有窗口是否都关闭，当所有窗口都关闭后，线程会把所有队列中数据都保存完成后再自动退出，进程完成退出！

            csvThread = new Thread(CSVThreadDoing);
            csvThread.Name = "CSV后台写入线程";
            csvThread.Priority = ThreadPriority.BelowNormal;
            csvThread.Start();
        }

        ~CsvWriter()
        {
            //当对象被释放时，从全局资源中删除引用
            lock (objLock)
                csvList.Remove(this);
        }

        /// <summary>
        /// 文件无新操作真正写入硬盘超时时间
        /// </summary>
        private readonly int flushTime = 5000;
        /// <summary>
        /// 文件无新操作真正关闭文件超时时间
        /// </summary>
        private readonly int closeTime = 30000;
        /// <summary>
        /// 写入文件使用的字符编码
        /// </summary>
        private readonly Encoding encoding = Encoding.UTF8;
        /// <summary>
        /// 缓冲区大小（以字节为单位）
        /// </summary>
        private readonly int bufferSize = 10240;
        /// <summary>
        /// 数据队列
        /// </summary>
        private readonly ConcurrentQueue<CsvData> queueList = new ConcurrentQueue<CsvData>();
        /// <summary>
        /// 
        /// </summary>
        private WriterModel writer = null;
        /// <summary>
        /// 最后文件路径
        /// </summary>
        private string csvFile = string.Empty;


        /// <summary>
        /// 初始化CSV文件写入类
        /// </summary>
        public CsvWriter() : this(5000, 30000, Encoding.UTF8, 10240)
        { }

        /// <summary>
        /// 初始化CSV文件写入类
        /// </summary>
        /// <param name="flushTime">文件无新操作真正写入硬盘超时时间</param>
        /// <param name="closeTime">文件无新操作真正关闭文件超时时间</param>
        public CsvWriter(int flushTime, int closeTime) : this(flushTime, closeTime, Encoding.UTF8, 10240)
        { }

        /// <summary>
        /// 初始化CSV文件写入类
        /// </summary>
        /// <param name="flushTime">文件无新操作真正写入硬盘超时时间</param>
        /// <param name="closeTime">文件无新操作真正关闭文件超时时间</param>
        /// <param name="encoding">写入文件使用的字符编码</param>
        public CsvWriter(int flushTime, int closeTime, Encoding encoding) : this(flushTime, closeTime, encoding, 10240)
        { }

        /// <summary>
        /// 初始化CSV文件写入类
        /// </summary>
        /// <param name="flushTime">文件无新操作真正写入硬盘超时时间</param>
        /// <param name="closeTime">文件无新操作真正关闭文件超时时间</param>
        /// <param name="bufferSize">缓冲区大小（以字节为单位）</param>
        public CsvWriter(int flushTime, int closeTime, int bufferSize) : this(flushTime, closeTime, Encoding.UTF8, bufferSize)
        { }

        /// <summary>
        /// 初始化CSV文件写入类
        /// </summary>
        /// <param name="flushTime">文件无新操作真正写入硬盘超时时间</param>
        /// <param name="closeTime">文件无新操作真正关闭文件超时时间</param>
        /// <param name="encoding">写入文件使用的字符编码</param>
        /// <param name="bufferSize">缓冲区大小（以字节为单位）</param>
        /// <exception cref="Exception">需在WinFrom环境下已创建窗口后才可使用此类，否则报错</exception>
        public CsvWriter(int flushTime, int closeTime, Encoding encoding, int bufferSize)
        {
            this.flushTime = flushTime;
            this.closeTime = closeTime;
            this.encoding = encoding;
            this.bufferSize = bufferSize;

            //
            if (Application.OpenForms.Count == 0)
                throw new Exception("请在WinFrom环境下已创建窗口后使用此服务类");

            //添加到全局列表
            csvList.Add(this);
        }

        /// <summary>
        /// 字符编码
        /// </summary>
        public Encoding Encoding
        {
            get { return encoding; }
        }

        /// <summary>
        /// 缓冲区大小（以字节为单位）
        /// </summary>
        public int BufferSize
        {
            get { return bufferSize; }
        }

        /// <summary>
        /// 将字符串写入
        /// </summary>
        /// <param name="csvFile">CSV文件路径</param>
        /// <param name="value">字符串</param>
        public void Write(string csvFile, string value)
        {
            CsvData data = new CsvData(csvFile, value, false);
            queueList.Enqueue(data);
            csvEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAppend { get; set; } = true;

        /// <summary>
        /// 将字符串写入
        /// </summary>
        /// <param name="csvFile">CSV文件路径</param>
        /// <param name="format">复合格式字符串</param>
        /// <param name="args">参数数组</param>
        public void Write(string csvFile, string format, params object[] args)
        {
            string value = string.Format(format, args);
            Write(csvFile, value);
        }

        /// <summary>
        /// 将字符串写入并换行
        /// </summary>
        /// <param name="csvFile">CSV文件路径</param>
        /// <param name="value">字符串</param>
        public void WriteLine(string csvFile, string value)
        {
            CsvData data = new CsvData(csvFile, value, true);
            queueList.Enqueue(data);
            csvEvent.Set();
        }

        /// <summary>
        /// 将字符串写入并换行
        /// </summary>
        /// <param name="csvFile"></param>
        /// <param name="sringBuilder"></param>
        public void WriteLine(string csvFile, StringBuilder sringBuilder)
        {
            WriteLine(csvFile, sringBuilder.ToString());
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }

        /// <summary>
        /// 将字符串写入并换行
        /// </summary>
        /// <param name="csvFile">CSV文件路径</param>
        /// <param name="format">复合格式字符串</param>
        /// <param name="args">参数数组</param>
        public void WriteLine(string csvFile, string format, params object[] args)
        {
            string value = string.Format(format, args);
            WriteLine(csvFile, value);
        }

        internal void WriteData(bool tryCatchOne = false)
        {
            try
            {
                CsvData data = null;
                //一次把队列中的所有数据都写入
                while (queueList.TryDequeue(out data))
                {
                    if (data.CsvFile != this.csvFile)
                    {
                        //如果写入文件更新，代表切换了文档，可以立即把前一个文档立即关闭

                        if (writer != null)
                        {
                            writer.Dispose();
                            writer = null;
                        }
                        this.csvFile = data.CsvFile;
                    }
                    //如果写入类为空，将创建新的写入类
                    if (writer == null)
                        writer = new WriterModel(data.CsvFile, encoding, bufferSize, IsAppend);
                    //写入队列数据
                    if (tryCatchOne)
                    {
                        //只有在线程线束前才会传入每一项都try-catch
                        try
                        {
                            writer.WriteData(data);
                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Error("CSV后台线程写入文件出错:"+ex);
                        }
                    }
                    else
                    {
                        writer.WriteData(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("CSV后台线程写入文件出错:" + ex);
            }
        }

        internal void CheckFinish()
        {
            if (writer != null && queueList.Count == 0)
            {
                //调用检查，到时间会刷新，超时会关闭
                if (writer.CheckFinish(flushTime, closeTime))
                {
                    writer.Dispose();
                    writer = null;
                    this.csvFile = string.Empty;
                }
            }
        }

        /// <summary>
        /// 后台线程函数
        /// </summary>
        private static void CSVThreadDoing()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        //没有数据时，只阻塞500毫秒，即在没有新数据时，会每500毫秒检查一次
                        if (csvEvent.WaitOne(500))
                        {
                            //循环调用把队列中的数据写入
                            lock (objLock)
                            {
                                foreach (CsvWriter writer in csvList)
                                    writer.WriteData();
                            }
                        }
                        //检测写入刷新、关闭超时文档
                        lock (objLock)
                        {
                            foreach (CsvWriter writer in csvList)
                                writer.CheckFinish();
                        }
                        //如果程序所有窗口都已关闭，退出线程
                        if (Application.OpenForms.Count == 0)
                        {
                            return;
                        }
                    }
                    catch (ThreadAbortException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error("CSV后台线程写入文件出错:" + ex);
                        Thread.Sleep(200);
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("CSV后台线程写入文件出错:" + ex);
            }
            finally
            {
                //线程退出前要把所有队列中数据都写入
                lock (objLock)
                {
                    foreach (CsvWriter writer in csvList)
                    {
                        writer.WriteData(true);
                        writer.Close();
                    }
                }
            }
        }
    }

    /// <summary>
    /// CSV文件真实写入类
    /// </summary>
    internal class WriterModel : IDisposable
    {
        private StreamWriter writer = null;
        private string csvFile = string.Empty;
        private DateTime lastWriteTime;
        private bool isFlush = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="csvFile">CSV文件路径</param>
        /// <param name="encoding"></param>
        /// <param name="bufferSize"></param>
        public WriterModel(string csvFile, Encoding encoding, int bufferSize, bool isAppend = true)
        {
            this.csvFile = csvFile;
            //如果CSV文件对应的目录不存在则创建
            FileInfo fileInfo = new FileInfo(csvFile);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            //创建写入类
            if (File.Exists(csvFile))
                File.SetAttributes(csvFile, FileAttributes.Normal);
            writer = new StreamWriter(csvFile, isAppend, encoding, bufferSize);
            lastWriteTime = DateTime.Now;
            isFlush = false;
            //Debug等级写入，如不想真正写入日志，可把日志最小等级设置为Info
            //Log4netRecord.ErrorLog.Debug($"开始写入CSV文档：【{csvFile}】");
        }

        /// <summary>
        /// 返回是否要刷新
        /// </summary>
        public bool IsFlush
        {
            get { return isFlush; }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="data"></param>
        public void WriteData(CsvData data)
        {
            if (data.IsLine)
                WriteLine(data.Value);
            else
                Write(data.Value);
        }

        /// <summary>
        /// 将字符串写入
        /// </summary>
        /// <param name="value">字符串</param>
        public void Write(string value)
        {
            writer.Write(value);
            lastWriteTime = DateTime.Now;
            isFlush = false;
        }

        /// <summary>
        /// 将字符串写入并换行
        /// </summary>
        /// <param name="value">字符串</param>
        public void WriteLine(string value)
        {
            writer.WriteLine(value);
            lastWriteTime = DateTime.Now;
            isFlush = false;
        }

        /// <summary>
        /// 把缓冲区的全部内容写入到硬盘
        /// </summary>
        public void Flush()
        {
            if (!isFlush)
            {
                writer.Flush();
                isFlush = true;
                //Log4netRecord.ErrorLog.Debug($"文档刷新写入硬盘【{csvFile}】");
            }

        }

        /// <summary>
        /// 检测文件是否超时完成
        /// </summary>
        /// <param name="flushTime">刷新超时时间</param>
        /// <param name="closeTime">完成超时时间</param>
        /// <returns></returns>
        public bool CheckFinish(int flushTime, int closeTime)
        {
            if (DateTime.Now >= lastWriteTime.AddMilliseconds(flushTime))
                Flush();
            if (DateTime.Now >= lastWriteTime.AddMilliseconds(closeTime))
                return true;
            return false;
        }

        /// <summary>
        /// 关闭并释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                    File.SetAttributes(csvFile, FileAttributes.ReadOnly);
                   // Log4netRecord.run.Debug($"CSV文档关闭【{csvFile}】");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("关闭CSV文档出错:" + ex);
            }
        }
    }


    internal class CsvData
    {
        public string CsvFile = null;
        public string Value = string.Empty;
        public bool IsLine = false;

        public CsvData(string csvFile, string value, bool isLine)
        {
            CsvFile = csvFile;
            Value = value;
            IsLine = isLine;
        }
    }
}
