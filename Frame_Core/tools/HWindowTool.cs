using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// halcon窗体工具
    /// </summary>
    public static class HWindowTool
    {
        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="_hWindowHandle"></param>
        /// <param name="_hoImage"></param>
        public static void DispImage(HWindow _hWindowHandle, HObject _hoImage)
        {
            try
            {

                if (_hWindowHandle == null) return;
                if (_hoImage == null) return;

                HTuple hvWidth = new HTuple();
                HTuple hvHeight = new HTuple();

                HOperatorSet.GetImageSize(_hoImage, out hvWidth, out hvHeight);
                HOperatorSet.SetPart(_hWindowHandle, 0, 0, hvHeight - 1, hvWidth - 1);
                HOperatorSet.DispObj(_hoImage, _hWindowHandle);
            }
            catch (Exception)
            {
            }
          
        }
    }
}
