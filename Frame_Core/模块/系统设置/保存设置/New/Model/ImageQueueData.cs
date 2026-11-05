using FrameCommunity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class ImageQueueData : IDisposable
    {
        public string Name;
        public bool State = false;
        public int QueuelimitCount = 20;
        public int QueueReserveCount = 5;
        public QueuePackage<ShowHalconWindow> DataQueue { get; } = new QueuePackage<ShowHalconWindow>();
        public ImageQueueData(string name)
        {
            Name = name;
        }

        public void Dispose()
        {
            while (DataQueue.Count > 0)
            {
                DataQueue?.Dequeue()?.Dispose();
            }
            DataQueue.Clear();
        }
    }
}
