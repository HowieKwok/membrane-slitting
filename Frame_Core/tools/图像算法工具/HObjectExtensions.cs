using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HalconDotNet
{
    /// <summary>
    /// HObject扩展类
    /// </summary>
    public static class HObjectExtensions
    {
        private static ConcurrentDictionary<int, DisposeTimesModel> list = new ConcurrentDictionary<int, DisposeTimesModel>();
        private static int disposeFlag = 0;
        private static DateTime disponseTime = DateTime.Now.AddMinutes(5);
        private static int timeOutMinute = 60 * 12; //默认超时时间为12小时，由于字典中储存的内容不大，就算外部逻辑出现问题，造成内部字典有堆积，也占不了多少内存，并且12小时可以保证程序因为中途停机什么原因造成因为超时而错误

        /// <summary>
        /// 设置超时释放分钟数(默认10分钟超时)
        /// </summary>
        public static int TimeOutMinute
        {
            get { return timeOutMinute; }
            set { timeOutMinute = value; }
        }

        /// <summary>
        /// 图像假克隆，只负责把该HObject的释放次数加1
        /// </summary>
        /// <param name="image">HObject</param>
        /// <param name="dupImage">与上面为同一个对象，因为遵循原函数形式</param>
        /// <returns></returns>
        public static int CopyImage(this HObject image, out HObject dupImage)
        {
            dupImage = image;
            return AddReferenceCount(image);
        }

        /// <summary>
        /// 使图像释放次数加1
        /// </summary>
        /// <param name="image">HObject</param>
        /// <returns></returns>
        public static int AddReferenceCount(this HObject image)
        {
            //获取对象的HashCode，确定对象（不同实例的HashCode是不一样的，可用HashCode来判断是否为同一实例对象）
            //不使用对象直接引用而使用HashCode的原因，防止外部调用释放函数次数不足，并且内部也有HObject也有引用，会造成GC不会释放该对象
            int hashcode = image.GetHashCode();
            int dispCount = 2;
            list.AddOrUpdate(hashcode, new DisposeTimesModel(2), (t, m) =>
            {
                //多次设置，会重新设置释放次数
                dispCount = m.CopyImage();

                return m;
            });
            return dispCount;
        }

        /// <summary>
        /// 使图像释放次数加1
        /// </summary>
        /// <param name="image">HImage</param>
        /// <returns></returns>
        public static int AddReferenceCount(this HImage image)
        {
            HObject obj = image as HObject;
            return AddReferenceCount(obj);
        }

        /// <summary>
        /// 获取图像被引用次数（需要释放次数）
        /// </summary>
        /// <param name="image">HObject</param>
        /// <returns></returns>
        public static int GetReferenceCount(this HObject image)
        {
            //获取对象的HashCode，确定对象（不同实例的HashCode是不一样的，可用HashCode来判断是否为同一实例对象）
            //不使用对象直接引用而使用HashCode的原因，防止外部调用释放函数次数不足，并且内部也有HObject也有引用，会造成GC不会释放该对象
            int hashcode = image.GetHashCode();
            DisposeTimesModel model;
            if (list.TryGetValue(hashcode, out model))
                return model.ReferenceCount;
            //没有实际被引用
            if (image.IsInitialized()) //如果图像可用，返回1，不可用返回0
                return 1;
            return 0;
        }

        /// <summary>
        /// 设置释放次数
        /// </summary>
        /// <param name="image">HObject</param>
        /// <param name="times">释放次数</param>
        public static void SetDisposeTimes(this HObject image, int times)
        {
            //获取对象的HashCode，确定对象（不同实例的HashCode是不一样的，可用HashCode来判断是否为同一实例对象）
            //不使用对象直接引用而使用HashCode的原因，防止外部调用释放函数次数不足，并且内部也有HObject也有引用，会造成GC不会释放该对象
            int hashcode = image.GetHashCode();
            list.AddOrUpdate(hashcode, new DisposeTimesModel(times), (t, m) =>
             {
                 //多次设置，会重新设置释放次数
                 m.SetReferenceCount(times);

                 return m;
             });
            
        }

        /// <summary>
        /// 设置释放次数
        /// </summary>
        /// <param name="image">HImage</param>
        /// <param name="times">释放次数</param>
        public static void SetDisposeTimes(this HImage image, int times)
        {
            HObject obj = image as HObject;
            SetDisposeTimes(obj, times);
        }

        /// <summary>
        /// 扩展释放对象（如果设置了释放次数，只有到次数到0时才真正释放）
        /// </summary>
        /// <param name="image">HObject</param>
        /// <returns>是否真正释放</returns>
        public static bool DisposeEx(this HObject image)
        {
            //获取对象的HashCode，确定对象（不同实例的HashCode是不一样的，可用HashCode来判断是否为同一实例对象）
            //不使用对象直接引用而使用HashCode的原因，防止外部调用释放函数次数不足，并且内部也有HObject也有引用，会造成GC不会释放该对象
            int hashcode = image.GetHashCode();
            DisposeTimesModel model;
            if (list.TryGetValue(hashcode, out model))
            {
                //调用释放检查，返回True时代码释放次数用完，会进行真正释放操作
                if (model.Dispose())
                {
                    image.Dispose();
                    list.TryRemove(hashcode, out model);
                    //异步调用超时检查操作
                    ChearTimeOutList();
                    return true;
                }
                return false;
            }
            else
            {
                //如果对象没有设置过释放次数，会直接释放
                image.Dispose();
                ChearTimeOutList();
                return true;

            }
        }

        /// <summary>
        /// 扩展释放对象（如果设置了释放次数，只有到次数到0时才真正释放）
        /// </summary>
        /// <param name="image">HImage</param>
        /// <returns>是否真正释放</returns>
        public static bool DisposeEx(this HImage image)
        {
            HObject obj = image as HObject;
            return DisposeEx(obj);
        }

        /// <summary>
        /// 内部异步调用清理超时记录（防止设置错释放次数造成记录留存）
        /// </summary>
        private static void ChearTimeOutList()
        {
            //原子判断，保证同一时刻只有一条线程能进入条件代码
            if (Interlocked.CompareExchange(ref disposeFlag, 1, 0) == 0)
            {
                //是否有清楚时间
                if (DateTime.Now >= disponseTime)
                {
                    //设备下次清理时间，5分钟清理一次
                    disponseTime = DateTime.Now.AddMinutes(5);
                    //异步调用清理动作
                    Task.Run(() =>
                    {
                        //获取超时Key
                        int[] codes = list.Where(it => it.Value.IsTimeOut()).Select(it => it.Key).ToArray();
                        //删除超时Key
                        DisposeTimesModel value;
                        foreach (var code in codes)
                            list.TryRemove(code, out value);
                        codes = null;
                    });
                }
                //重置进入标志位
                Interlocked.Exchange(ref disposeFlag, 0);
            }
        }
    }

    /// <summary>
    /// 释放次数设置类
    /// </summary>
    public class DisposeTimesModel
    {
        private int referenceCount = 1;
        private DateTime timeOutTime = DateTime.Now.AddMinutes(HObjectExtensions.TimeOutMinute);

        /// <summary>
        /// 初始化释放次数设置类
        /// </summary>
        /// <param name="count"></param>
        public DisposeTimesModel(int count)
        {
            this.referenceCount = count;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime TimeOutTime
        {
            get { return timeOutTime; }
        }

        /// <summary>
        /// 剩余释放次数
        /// </summary>
        public int ReferenceCount
        {
            get
            {
                return this.referenceCount;
            }
        }

        /// <summary>
        /// 重设释放次数
        /// </summary>
        /// <param name="times"></param>
        public void SetReferenceCount(int times)
        {
            this.referenceCount = times;
            timeOutTime = DateTime.Now.AddMinutes(HObjectExtensions.TimeOutMinute);
        }

        /// <summary>
        /// 调用克隆图像，返回剩余释放次数
        /// </summary>
        /// <returns></returns>
        public int CopyImage()
        {
            int n = Interlocked.Increment(ref referenceCount);
            return n;
        }

        /// <summary>
        /// 使用释放，返回是否需要真正释放
        /// </summary>
        /// <returns></returns>
        public bool Dispose()
        {
            int n = Interlocked.Decrement(ref referenceCount);
            if (n <= 0)
                return true;
            return false;
        }

        /// <summary>
        /// 是否已超时
        /// </summary>
        /// <returns></returns>
        public bool IsTimeOut()
        {
            if (timeOutTime >= DateTime.Now)
                return true;
            return false;
        }
    }
}
