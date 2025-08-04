using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    /// <summary>
    /// 生产数据
    /// </summary>
    public static class ProdImageData
    {
        /// <summary>
        /// 当前生产数据路径
        /// </summary>
        public static ProdDataRecord currentProdDataRecord = new ProdDataRecord();

        /// <summary>
        /// 历史生产数据路径
        /// </summary>
        public static ProdDataRecord historyProdDataRecord = new ProdDataRecord();

    }
}
