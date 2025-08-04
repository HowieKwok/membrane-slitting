using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public  class Slice1To2Paramter : BaseProParam
    {

        public Slice1To2Paramter()
        {
            //m_dicShowHalconWindow.Add(WorkerThreadEnum.工作者1, new Queue<ShowHalconWindow>());
            //m_dicImageDataResult.Add(WorkerThreadEnum.工作者1, new Queue<ImageDataResult>());
        }
        /// <summary>
        /// 相机的图像ID，默认第一片为1，预处理使用
        /// </summary>
        public long m_nCurProImgId = 1;
        /// <summary>
        /// 用于线程工作者显示图像使用
        /// </summary>
        //public Dictionary<WorkerThreadEnum, Queue<ShowHalconWindow>> m_dicShowHalconWindow = new Dictionary<WorkerThreadEnum, Queue<ShowHalconWindow>>();
        ///// <summary>
        ///// 用于各个线程工作者处理结果分配使用
        ///// </summary>
        //public Dictionary<WorkerThreadEnum, Queue<ImageDataResult>> m_dicImageDataResult = new Dictionary<WorkerThreadEnum, Queue<ImageDataResult>>();


        public override void Dispose()
        {
     


           // foreach (var item in m_dicShowHalconWindow)
            //{
            //    while (item.Value.Count > 0)
            //    {
            //        item.Value.Dequeue()?.Dispose();
            //    }
            //}

            //foreach (var item in m_dicImageDataResult)
            //{
            //    while (item.Value.Count > 0)
            //    {
            //        item.Value.Dequeue()?.Dispose();
            //    }
            //}

            m_listImageInfo_Cam.Clear();
          //  m_dicShowHalconWindow.Clear();
            base.Dispose();

        }
    }
}
