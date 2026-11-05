using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class DataExtension
    {
        // 转换为 List<List<RowData>>
        public static List<List<RowData>> ToRowDataList(this IEnumerable<QualityData> source)
        {
            if (source == null) return new List<List<RowData>>();

            return source.Select(p => new List<RowData>
            {
                new RowData { dataStr = p.Name ?? "" },
                new RowData { dataStr = p.Count.ToString() },
                new RowData { dataStr = p.Ratio ?? "0%" }
            }).ToList();
        }

        // 转换为 List<List<string>>
        public static List<string[]> ToDataList(this IEnumerable<QualityData> source)
        {
            if (source == null) return new List<string[]>();

            return source.Select(p => new List<string>
            {
                p.Name ?? "" ,
                 p.Count.ToString(),
                p.Ratio ?? "0%"
            }.ToArray()).ToList();
        }

        // 刷新数据
        public static void UdpateData(this BindingList<QualityData> datas, List<QualityData> source)
        {
            if (datas == null) return;
            if (source == null) return;

            // 创建 Dictionary 加速查找
            var sourceDict = source.ToDictionary(x => x.Name);

            datas.RaiseListChangedEvents = false;

            try
            {
                // 从后往前遍历，同时处理更新和删除
                for (int i = datas.Count - 1; i >= 0; i--)
                {
                    var existing = datas[i];
                    if (sourceDict.TryGetValue(existing.Name, out var sourceItem))
                    {
                        // ✅ 更新：source 中存在
                        existing.Count = sourceItem.Count;
                        existing.Ratio = sourceItem.Ratio;
                        sourceDict.Remove(existing.Name);
                    }
                    else
                    {
                        // ❌ 删除：source 中不存在
                        datas.RemoveAt(i);
                    }
                }

                // 添加新增的项
                foreach (var sourceItem in sourceDict.Values)
                {
                    datas.Add(new QualityData
                    {
                        Name = sourceItem.Name,
                        Count = sourceItem.Count,
                        Ratio = sourceItem.Ratio
                    });
                }
            }
            finally
            {
                datas.RaiseListChangedEvents = true;
                datas.ResetBindings();
            }
        }
    }
}
