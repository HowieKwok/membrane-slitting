using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class DataSaveWorkClass<T> where T : DataSaveBase
    {
        public ConcurrentQueue<T> Data { get; } = new ConcurrentQueue<T>();
    }


    public class DataSaveClass : DataSaveBase
    {
        public List<List<RowData>> rowDatas = new List<List<RowData>>();
    }

    public class DataSaveBase
    {
        public string Source = "";

        public string FileSave_Path = "";

        public List<string> FileSave_Title = new List<string>();
    }
}
