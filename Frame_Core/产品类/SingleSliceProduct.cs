using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 分切纠偏产品类
    /// </summary>
    public class SingleSliceProduct : ProductBase
    {

        public QueuePackage<ImageDataResult> DataResults = new QueuePackage<ImageDataResult>();
      


    }
}
