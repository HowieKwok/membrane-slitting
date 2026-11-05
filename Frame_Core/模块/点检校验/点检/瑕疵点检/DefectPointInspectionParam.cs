using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public  class DefectPointInspectionParam : XmlSerializationOperation
    {
        /// <summary>
        /// 点检图片路径
        /// </summary>
        public string PointCheckImgPath = $"{AppDomain.CurrentDomain.BaseDirectory}点检图像\\缺陷点检图像";

      

    }
}
