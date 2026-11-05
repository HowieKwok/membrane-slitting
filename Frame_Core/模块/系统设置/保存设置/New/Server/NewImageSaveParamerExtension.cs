using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class NewImageSaveParamerExtension
    {
        public static ImageSaveParamConfig GetSaveParam(this List<ImageSaveParamConfig> imageSaveParams,int index)
        {
            if (imageSaveParams == null || index < 0 || index >= imageSaveParams.Count)
            {
                return  null;
            }

            return imageSaveParams[index];
        }
    }
}
