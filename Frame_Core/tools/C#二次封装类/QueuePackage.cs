using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class QueuePackage<T>
    {
        private readonly object lockObject = new object();
        private Queue<T> queue = new Queue<T>();

        // 入队方法
        public void Enqueue(T item)
        {
            lock (lockObject)
            {
                queue.Enqueue(item);
            }
        }

        // 入队方法
        public void Enqueue(T item, uint maxEnqueueCount, bool isUseOld = false)
        {
            lock (lockObject)
            {
                if (isUseOld)//使用旧数据
                {
                    if (queue.Count < maxEnqueueCount)
                    {
                        queue.Enqueue(item);
                    }
                }
                else//使用新数据
                {
                    queue.Enqueue(item);
                    if (queue.Count > maxEnqueueCount)
                    {
                        queue.Dequeue();
                    }
                }

            }
        }

        // 出队方法
        public T Dequeue()
        {
            lock (lockObject)
            {
                if (queue.Count == 0)
                {
                    throw new InvalidOperationException("队列为空");
                }
                return queue.Dequeue();
            }
        }

        // 出队方法
        public List<T> DequeueAll()
        {
            lock (lockObject)
            {
                List<T> list = new List<T>();
                for (int i = 0; i < queue.Count; i++)
                {
                    T temp = queue.Dequeue();
                    if (temp != null)
                    {
                        list.Add(temp);
                    }
                }
                return list;
            }
        }

        // 出队方法
        public List<T> ToList()
        {
            lock (lockObject)
            {
                List<T> list = queue.ToList();
                return list;
            }
        }

        // 查看队首元素
        public T Peek()
        {
            lock (lockObject)
            {
                if (queue.Count == 0)
                {
                    throw new InvalidOperationException("队列为空");
                }
                return queue.Peek();
            }
        }

        // 获取队列中元素的数量
        public int Count
        {
            get
            {
                lock (lockObject)
                {
                    return queue.Count;
                }
            }
        }

        // 清空队列
        public void Clear()
        {
            lock (lockObject)
            {
                queue.Clear();
            }
        }

        // 检查队列是否包含某个元素
        public bool Contains(T item)
        {
            lock (lockObject)
            {
                return queue.Contains(item);
            }
        }

        // 将队列的内容复制到数组中
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (lockObject)
            {
                queue.CopyTo(array, arrayIndex);
            }
        }

        // 转换为数组
        public T[] ToArray()
        {
            lock (lockObject)
            {
                return queue.ToArray();
            }
        }

        // 修剪队列，优化内存使用
        public void TrimExcess()
        {
            lock (lockObject)
            {
                queue.TrimExcess();
            }
        }
    }
}
