using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class PoleEarsInfo
    {
        public HObject Image_JRpart = null;
        public long JR_ID = 0;
        public string cameraName = "default Camera";
        public bool IsLeft = true;

        public long image_ID_Begin = 0;       
        public double Row_Begin = 0;
         
        public long image_ID_End = 0;
        public double Row_End = 0;

        public long image_ID_Mid = 0;
        public double Row_Mid = 0;

        
    }
}
