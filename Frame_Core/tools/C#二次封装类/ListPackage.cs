using NPOI.HPSF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Frame_Core
{
    public class ListPackage<T>
    {
        private readonly object lockObject = new object();
        private List<T> list = new List<T>();

        // 添加元素
        public void Add(T item)
        {
            lock (lockObject)
            {
                list.Add(item);
            }
        }

        // 入队方法
        public void Add(T item, uint maxEnqueueCount, bool isUseOld = false)
        {
            lock (lockObject)
            {
                if (isUseOld)//使用旧数据
                {
                    if (list.Count < maxEnqueueCount)
                    {
                        list.Add(item);
                    }
                }
                else//使用新数据
                {
                    list.Add(item);
                    if (list.Count > maxEnqueueCount)
                    {
                        list.RemoveAt(0);
                    }
                }

            }
        }
        // 添加多个元素
        public void AddRange(IEnumerable<T> collection)
        {
            lock (lockObject)
            {
                list.AddRange(collection);
            }
        }

        // 入队方法
        public void AddRange(IEnumerable<T> collection, uint maxEnqueueCount, bool isUseOld = false)
        {
            lock (lockObject)
            {
                if (isUseOld)//使用旧数据
                {
                    if (list.Count < maxEnqueueCount)
                    {
                        list.AddRange(collection);
                    }
                }
                else//使用新数据
                {
                    list.AddRange(collection);
                    if (list.Count > maxEnqueueCount)
                    {
                        if (list.Count > collection.Count())
                        {
                            list.RemoveRange(0, collection.Count() - 1);
                        }
                    }
                }

            }
        }


        public T[] GetListAndClear()
        {
            T[] listTemp = new T[0];
            lock (lockObject)
            {
                listTemp = new T[list.Count];
                list.CopyTo(listTemp);
                list.Clear();
            }
            return listTemp;
        }

        // 将列表的内容复制到数组中
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (lockObject)
            {
                list.CopyTo(array, arrayIndex);
            }
        }

        // 清空列表
        public void Clear()
        {
            lock (lockObject)
            {
                list.Clear();
            }
        }

        // 检查列表是否包含某个元素
        public bool Contains(T item)
        {
            lock (lockObject)
            {
                return list.Contains(item);
            }
        }

        // 查找指定元素的索引
        public int IndexOf(T item)
        {
            lock (lockObject)
            {
                return list.IndexOf(item);
            }
        }

        // 在指定位置插入元素
        public void Insert(int index, T item)
        {
            lock (lockObject)
            {
                list.Insert(index, item);
            }
        }

        // 在指定位置插入多个元素
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            lock (lockObject)
            {
                list.InsertRange(index, collection);
            }
        }

        // 移除元素
        public bool Remove(T item)
        {
            lock (lockObject)
            {
                return list.Remove(item);
            }
        }

        // 移除指定索引处的元素
        public void RemoveAt(int index)
        {
            lock (lockObject)
            {
                list.RemoveAt(index);
            }
        }

        // 移除指定范围的元素
        public void RemoveRange(int index, int count)
        {
            lock (lockObject)
            {
                list.RemoveRange(index, count);
            }
        }

        // 获取或设置指定索引处的元素
        public T this[int index]
        {
            get
            {
                lock (lockObject)
                {
                    return list[index];
                }
            }
            set
            {
                lock (lockObject)
                {
                    list[index] = value;
                }
            }
        }

        // 获取列表中元素的数量
        public int Count
        {
            get
            {
                lock (lockObject)
                {
                    return list.Count;
                }
            }
        }

        // 对列表进行排序
        public void Sort()
        {
            lock (lockObject)
            {
                list.Sort();
            }
        }

        // 对列表使用自定义比较器进行排序
        public void Sort(IComparer<T> comparer)
        {
            lock (lockObject)
            {
                list.Sort(comparer);
            }
        }

        // 对列表的一部分进行排序
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            lock (lockObject)
            {
                list.Sort(index, count, comparer);
            }
        }

        // 转换为数组
        public T[] ToArray()
        {
            lock (lockObject)
            {
                return list.ToArray();
            }
        }

        // 查找所有符合条件的元素
        public List<T> FindAll(Predicate<T> match)
        {
            lock (lockObject)
            {
                return list.FindAll(match);
            }
        }

        // 实现 IEnumerable<T> 接口
        public IEnumerator<T> GetEnumerator()
        {
            lock (lockObject)
            {
                // 由于 List<T> 已经实现了 IEnumerable<T>，
                // 我们可以直接返回其迭代器。
                return list.GetEnumerator();
            }
        }
      
    }
}
