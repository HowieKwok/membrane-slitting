using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class PointInspectionStateParam : XmlSerializationOperation
    {
        /// <summary>
        /// 上一次点检OK的时间
        /// </summary>
        public DateTime djLastOKTime_Size = new DateTime(2018, 11, 9, 19, 30, 0);

        /// <summary>
        /// 点检状态
        /// </summary>
        public int PointInspectionState_Size = (int)PointInspectionStateEnum.未点检;

        /// <summary>
        /// 上一次点检OK的时间
        /// </summary>
        public DateTime djLastOKTime_Defect = new DateTime(2018, 11, 9, 19, 30, 0);

        /// <summary>
        /// 点检状态
        /// </summary>
        public int PointInspectionState_Defect = (int)PointInspectionStateEnum.未点检;
    }
}
