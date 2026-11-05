using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class ConcurrentQueueExtension
    {
        public static void Enqueue<T>(this ConcurrentQueue<T> queue, T item, uint count)
        {
            if (queue.Count < count)
                queue.Enqueue(item);
        }

        /// <summary>
        /// 批量取出数据（最多 BATCH_SIZE 条）
        /// </summary>
        public static List<T> DequeueBatch<T>(this ConcurrentQueue<T> queue, int batchSize = 1000)
        {
            if (queue == null || batchSize <= 0)
                return new List<T>();

            var list = new List<T>();

            for (int i = 0; i < batchSize; i++)
            {

                if (queue.TryDequeue(out T item) && item != null)
                {
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    // 队列已空，提前退出
                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// 取出所有数据
        /// </summary>
        public static List<T> DequeueAll<T>(this ConcurrentQueue<T> queue)
        {
            if (queue == null)
                return new List<T>();

            var list = new List<T>();

            while (queue.TryDequeue(out T item))
            {
                if (item != null)
                {
                    list.Add(item);
                }
            }

            return list;
        }
    }
}
