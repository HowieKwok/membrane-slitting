using Frame_Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    /// <summary>
    /// 生产数据记录
    /// </summary>
    public class ProdDataRecord : XmlSerializationOperation
    {
        /// <summary>
        /// 相机1片数据
        /// </summary>
        public List<CamImageData> Cam1_ImageSliceDatas = new List<CamImageData>();

        /// <summary>
        /// 相机2片数据
        /// </summary>
        public List<CamImageData> Cam2_ImageSliceDatas = new List<CamImageData>();

        /// <summary>
        /// 相机3片数据
        /// </summary>
        public List<CamImageData> Cam3_ImageSliceDatas = new List<CamImageData>();

        /// <summary>
        /// 相机4片数据
        /// </summary>
        public List<CamImageData> Cam4_ImageSliceDatas = new List<CamImageData>();
   

        public void CleraProdDataRecord()
        {
            if (Cam1_ImageSliceDatas != null) { Cam1_ImageSliceDatas.Clear();}
            if (Cam2_ImageSliceDatas != null) { Cam2_ImageSliceDatas.Clear(); }   
            if (Cam3_ImageSliceDatas != null) { Cam3_ImageSliceDatas.Clear(); };
            if (Cam3_ImageSliceDatas != null) { Cam3_ImageSliceDatas.Clear(); };

        }
    }
}
