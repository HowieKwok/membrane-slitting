using DevMgmt.Cameras.SDK;
using HalconDotNet;

using LogTool;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class Transform_Image
    {
        /// <summary>
        /// 彩虹图转亮度图
        /// </summary>
        /// <param name="RainbowLightImage"></param>
        /// <param name="RainbowImage"></param>
        /// <returns></returns>
        public static HObject RainbowLightToLight(HObject RainbowLightImage, HObject RainbowImage)
        {
            HObject hv_ImageRBL = null, hv_ImageRB = null, hv_Light = null;
            HOperatorSet.GenEmptyObj(out hv_ImageRBL);
            HOperatorSet.GenEmptyObj(out hv_ImageRB);
            HOperatorSet.GenEmptyObj(out hv_Light);
            try
            {
                hv_ImageRBL.Dispose();
                HOperatorSet.ConvertImageType(RainbowLightImage, out hv_ImageRBL, "int2");
                hv_ImageRB.Dispose();
                HOperatorSet.ConvertImageType(RainbowImage, out hv_ImageRB, "int2");
                HOperatorSet.ScaleImage(hv_ImageRBL, out hv_ImageRBL, 2, 0);
                hv_Light.Dispose();
                HOperatorSet.SubImage(hv_ImageRBL, hv_ImageRB, out hv_Light, 1, 0);
                HOperatorSet.Rgb1ToGray(hv_Light, out hv_Light);
                HOperatorSet.ConvertImageType(hv_Light, out hv_Light, "byte");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
                return hv_Light;
            }
            finally
            {
                if (hv_ImageRBL != null)
                    hv_ImageRBL.Dispose();
                if (hv_ImageRB != null)
                    hv_ImageRB.Dispose();
            }
            return hv_Light;
        }

        /// <summary>
        /// 高度数据转彩色图
        /// <para>说明：点云高度数据，图像宽度，图像高度，高度范围上限，高度范围下限,是否使用自定义上下限</para> 
        /// </summary>
        /// <param name="Points_Z"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        /// <returns></returns>
        public static HObject HeightToRainbow(List<double> Points_Z, int ImageWidth, int ImageHeight, double DeepMax, double DeepMin, bool CustomMaxMin)
        {
            //赤（255，0，0）
            //橙（255，128，0）
            //黄（255，255，0）
            //绿（0，255，0）
            //青（0，255，255）
            //蓝（0，0，255）
            //紫（255，0，255）
            const byte d_oneR = 255, _oneG = 0, _oneB = 255;
            const byte _twoR = 0, u_twoG = 0, _twoB = 255;
            const byte _threeR = 0, _threeG = 255, d_threeB = 255;
            const byte u_fourR = 0, _fourG = 255, _fourB = 0;
            const byte _fiveR = 255, d_fiveG = 255, _fiveB = 0;
            const byte _sixR = 255, _sixG = 0, _sixB = 0;

            if (!CustomMaxMin)                                                        //自动找上下限
            {
                double maxVal = -20.0;
                double minVal = 9999.0;
                double limit = -19.0;                                                  //除去小于该值的亮度
                foreach (double thisNum in Points_Z)
                {
                    if (thisNum < limit)
                    {
                        continue;
                    }
                    else if (thisNum > maxVal)
                    {
                        maxVal = thisNum;
                    }
                    if (thisNum < minVal)
                    {
                        minVal = thisNum;
                    }
                }

                DeepMin = minVal - 0.09;                                                    //高度范围下限
                DeepMax = maxVal + 0.09;                                                    //高度范围上限
            }
            int ColorLever = 5 * 256;                                                   //将高度分为5*256级
            HObject ho_ImageRainbow = null;
            HOperatorSet.GenEmptyObj(out ho_ImageRainbow);
            try
            {
                double DeepColor = (DeepMax - DeepMin) / Convert.ToSingle(ColorLever);  //每一级色彩对应的高度

                byte[] byter = new byte[ImageWidth * ImageHeight];                      //红色通道
                byte[] byteg = new byte[ImageWidth * ImageHeight];                      //绿色通道
                byte[] byteb = new byte[ImageWidth * ImageHeight];                      //蓝色通道
                int ipoint = 0;
                foreach (double point_z in Points_Z)
                {
                    if (point_z < DeepMin || point_z > DeepMax)
                    {//判断是否超出设定高度范围
                        byter[ipoint] = 0;
                        byteg[ipoint] = 0;
                        byteb[ipoint] = 0;
                    }
                    else
                    {
                        int it = (int)((point_z - DeepMin) / DeepColor);      //判断在哪一层色彩
                        switch (it / 256)
                        {
                            case 0://紫
                                {
                                    byter[ipoint] = Convert.ToByte(d_oneR - it % 256);
                                    byteg[ipoint] = _oneG;
                                    byteb[ipoint] = _oneB;
                                }
                                break;
                            case 1://蓝
                                {
                                    byter[ipoint] = _twoR;
                                    byteg[ipoint] = Convert.ToByte(u_twoG + it % 256);
                                    byteb[ipoint] = _twoB;
                                }
                                break;
                            case 2://青
                                {
                                    byter[ipoint] = _threeR;
                                    byteg[ipoint] = _threeG;
                                    byteb[ipoint] = Convert.ToByte(d_threeB - it % 256);
                                }
                                break;
                            case 3://绿
                                {
                                    byter[ipoint] = Convert.ToByte(u_fourR + it % 256);
                                    byteg[ipoint] = _fourG;
                                    byteb[ipoint] = _fourB;
                                }
                                break;
                            case 4://黄
                                {
                                    byter[ipoint] = _fiveR;
                                    byteg[ipoint] = Convert.ToByte(d_fiveG - it % 256);
                                    byteb[ipoint] = _fiveB;
                                }
                                break;
                            default://赤
                                {
                                    byter[ipoint] = _sixR;
                                    byteg[ipoint] = _sixG;
                                    byteb[ipoint] = _sixB;
                                }
                                break;
                        }
                    }
                    ipoint++;
                }
                IntPtr imgPtrR = Marshal.AllocHGlobal(byter.Length);
                Marshal.Copy(byter, 0, imgPtrR, byter.Length);

                IntPtr imgPtrG = Marshal.AllocHGlobal(byteg.Length);
                Marshal.Copy(byteg, 0, imgPtrG, byteg.Length);

                IntPtr imgPtrB = Marshal.AllocHGlobal(byteb.Length);
                Marshal.Copy(byteb, 0, imgPtrB, byteb.Length);
                ho_ImageRainbow.Dispose();
                HOperatorSet.GenImage3(out ho_ImageRainbow, "byte", ImageWidth, ImageHeight, imgPtrR, imgPtrG, imgPtrB);
                Marshal.FreeHGlobal(imgPtrR);
                Marshal.FreeHGlobal(imgPtrG);
                Marshal.FreeHGlobal(imgPtrB);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }

            return ho_ImageRainbow;

        }





        /// <summary>
        /// 高度数据转彩色图 转三道通
        /// <para>说明：点云高度数据，图像宽度，图像高度，高度范围上限，高度范围下限,是否使用自定义上下限</para> 
        /// </summary>
        /// <param name="Points_Z"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        /// <returns></returns>
        public static HObject HeightToRainbow(float[] Points, int ImageWidth, int ImageHeight, double DeepMax, double DeepMin, bool CustomMaxMin)
        {
            //赤（255，0，0）
            //橙（255，128，0）
            //黄（255，255，0）
            //绿（0，255，0）
            //青（0，255，255）
            //蓝（0，0，255）
            //紫（255，0，255）
            const byte d_oneR = 255, _oneG = 0, _oneB = 255;
            const byte _twoR = 0, u_twoG = 0, _twoB = 255;
            const byte _threeR = 0, _threeG = 255, d_threeB = 255;
            const byte u_fourR = 0, _fourG = 255, _fourB = 0;
            const byte _fiveR = 255, d_fiveG = 255, _fiveB = 0;
            const byte _sixR = 255, _sixG = 0, _sixB = 0;

            if (!CustomMaxMin)                                                        //自动找上下限
            {
                float maxVal = -20;
                float minVal = 9999;
                float limit = -19;                                                  //除去小于该值的亮度
                foreach (float thisNum in Points)
                {
                    if (thisNum < limit)
                    {
                        continue;
                    }
                    else if (thisNum > maxVal)
                    {
                        maxVal = thisNum;
                    }
                    if (thisNum < minVal)
                    {
                        minVal = thisNum;
                    }
                }

                DeepMin = minVal - 0.09;                                                    //高度范围下限
                DeepMax = maxVal + 0.09;                                                    //高度范围上限
            }
            int ColorLever = 5 * 256;                                                   //将高度分为5*256级
            HObject ho_ImageRainbow = null;
            HOperatorSet.GenEmptyObj(out ho_ImageRainbow);
            try
            {
                double DeepColor = (DeepMax - DeepMin) / Convert.ToSingle(ColorLever);  //每一级色彩对应的高度

                byte[] byter = new byte[ImageWidth * ImageHeight];                      //红色通道
                byte[] byteg = new byte[ImageWidth * ImageHeight];                      //绿色通道
                byte[] byteb = new byte[ImageWidth * ImageHeight];                      //蓝色通道
                int ipoint = 0;
                foreach (float point_z in Points)
                {
                    if (point_z < DeepMin || point_z > DeepMax)
                    {//判断是否超出设定高度范围
                        byter[ipoint] = 0;
                        byteg[ipoint] = 0;
                        byteb[ipoint] = 0;
                    }
                    else
                    {
                        int it = (int)((point_z - DeepMin) / DeepColor);      //判断在哪一层色彩
                        switch (it / 256)
                        {
                            case 0://紫
                                {
                                    byter[ipoint] = Convert.ToByte(d_oneR - it % 256);
                                    byteg[ipoint] = _oneG;
                                    byteb[ipoint] = _oneB;
                                }
                                break;
                            case 1://蓝
                                {
                                    byter[ipoint] = _twoR;
                                    byteg[ipoint] = Convert.ToByte(u_twoG + it % 256);
                                    byteb[ipoint] = _twoB;
                                }
                                break;
                            case 2://青
                                {
                                    byter[ipoint] = _threeR;
                                    byteg[ipoint] = _threeG;
                                    byteb[ipoint] = Convert.ToByte(d_threeB - it % 256);
                                }
                                break;
                            case 3://绿
                                {
                                    byter[ipoint] = Convert.ToByte(u_fourR + it % 256);
                                    byteg[ipoint] = _fourG;
                                    byteb[ipoint] = _fourB;
                                }
                                break;
                            case 4://黄
                                {
                                    byter[ipoint] = _fiveR;
                                    byteg[ipoint] = Convert.ToByte(d_fiveG - it % 256);
                                    byteb[ipoint] = _fiveB;
                                }
                                break;
                            default://赤
                                {
                                    byter[ipoint] = _sixR;
                                    byteg[ipoint] = _sixG;
                                    byteb[ipoint] = _sixB;
                                }
                                break;
                        }
                    }
                    ipoint++;
                }



                IntPtr imgPtrR = Marshal.AllocHGlobal(byter.Length);
                Marshal.Copy(byter, 0, imgPtrR, byter.Length);

                IntPtr imgPtrG = Marshal.AllocHGlobal(byteg.Length);
                Marshal.Copy(byteg, 0, imgPtrG, byteg.Length);

                IntPtr imgPtrB = Marshal.AllocHGlobal(byteb.Length);
                Marshal.Copy(byteb, 0, imgPtrB, byteb.Length);
                ho_ImageRainbow.Dispose();
                HOperatorSet.GenImage3(out ho_ImageRainbow, "byte", ImageWidth, ImageHeight, imgPtrR, imgPtrG, imgPtrB);
                Marshal.FreeHGlobal(imgPtrR);
                Marshal.FreeHGlobal(imgPtrG);
                Marshal.FreeHGlobal(imgPtrB);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }

            return ho_ImageRainbow;

        }

        struct ColorRGB { public byte R; public byte G; public byte B; }
        /// <summary>
        /// 高度数据转彩色图 转三道通
        /// <para>说明：点云高度数据，图像宽度，图像高度，高度范围上限，高度范围下限,是否使用自定义上下限</para> 
        /// </summary>
        /// <param name="Points_Z"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        /// <returns></returns>
        public static HObject HeightToRainbow_new(float[] Points, int ImageWidth, int ImageHeight, double DeepMax, double DeepMin, bool CustomMaxMin)
        {
            //赤（255，0，0）
            //橙（255，128，0）
            //黄（255，255，0）
            //绿（0，255，0）
            //青（0，255，255）
            //蓝（0，0，255）
            //紫（255，0，255）
            const byte d_oneR = 255, _oneG = 0, _oneB = 255;
            const byte _twoR = 0, u_twoG = 0, _twoB = 255;
            const byte _threeR = 0, _threeG = 255, d_threeB = 255;
            const byte u_fourR = 0, _fourG = 255, _fourB = 0;
            const byte _fiveR = 255, d_fiveG = 255, _fiveB = 0;
            const byte _sixR = 255, _sixG = 0, _sixB = 0;
           
            if (!CustomMaxMin)                                                        //自动找上下限
            {
                DeepMin = DeepMin - 0.09;
                DeepMax = DeepMax + 0.09;
            }

            int ColorLever = 5 * 256;                                                   //将高度分为5*256级
            HObject ho_ImageRainbow = null;
            HOperatorSet.GenEmptyObj(out ho_ImageRainbow);

         
            try
            {

                // 预生成1280级颜色梯度表（内存占用仅3.75KB）
                // 计时开始 
              
                ColorRGB[] colorTable = new ColorRGB[1280];
                for (int it = 0; it < 1280; it++)
                {
                    int seg = it / 256;
                    int rem = it % 256;
                    switch (it / 256)
                    {
                        case 0://紫
                            {
                                colorTable[it].R = Convert.ToByte(d_oneR - it % 256);
                                colorTable[it].G = _oneG;
                                colorTable[it].B = _oneB;
                            }
                            break;
                        case 1://蓝
                            {
                                colorTable[it].R = _twoR;
                                colorTable[it].G = Convert.ToByte(u_twoG + it % 256);
                                colorTable[it].B = _twoB;
                            }
                            break;
                        case 2://青
                            {
                                colorTable[it].R = _threeR;
                                colorTable[it].G = _threeG;
                                colorTable[it].B = Convert.ToByte(d_threeB - it % 256);
                            }
                            break;
                        case 3://绿
                            {
                                colorTable[it].R = Convert.ToByte(u_fourR + it % 256);
                                colorTable[it].G = _fourG;
                                colorTable[it].B = _fourB;
                            }
                            break;
                        case 4://黄
                            {
                                colorTable[it].R = _fiveR;
                                colorTable[it].G = Convert.ToByte(d_fiveG - it % 256);
                                colorTable[it].B = _fiveB;
                            }
                            break;
                        default://赤
                            {
                                colorTable[it].R = _sixR;
                                colorTable[it].G = _sixG;
                                colorTable[it].B = _sixB;
                            }
                            break;
                    }

                }


                double DeepColor = (DeepMax - DeepMin) / Convert.ToSingle(ColorLever);  //每一级色彩对应的高度
           
                byte[] byter = new byte[ImageWidth * ImageHeight];                      //红色通道
                byte[] byteg = new byte[ImageWidth * ImageHeight];                      //绿色通道
                byte[] byteb = new byte[ImageWidth * ImageHeight];                      //蓝色通道

                // 预处理 
                float invDeepColor = (float)(1 / DeepColor);
                int maxIndex = colorTable.Length - 1; // 通常为 1279



                Parallel.For(0, Points.Length, i =>
                {
                    float z = Points[i];
                    if (z < DeepMin || z > DeepMax) return;
                    int it = Math.Min((int)((z - DeepMin) / DeepColor), 1279);
                    // 优化后的索引计算 
                    //int it = (int)((z - DeepMin) * invDeepColor);
                    //it = Math.Min(it, maxIndex);
                    ref ColorRGB color = ref colorTable[it];
                    byter[i] = color.R;
                    byteg[i] = color.G;
                    byteb[i] = color.B;
                });


                

                //IntPtr imgPtrR = Marshal.AllocHGlobal(byter.Length);
                //Marshal.Copy(byter, 0, imgPtrR, byter.Length);
                //IntPtr imgPtrG = Marshal.AllocHGlobal(byteg.Length);
                //Marshal.Copy(byteg, 0, imgPtrG, byteg.Length);
                //IntPtr imgPtrB = Marshal.AllocHGlobal(byteb.Length);
                //Marshal.Copy(byteb, 0, imgPtrB, byteb.Length);
                //ho_ImageRainbow.Dispose();
                //HOperatorSet.GenImage3(out ho_ImageRainbow, "byte", ImageWidth, ImageHeight, imgPtrR, imgPtrG, imgPtrB);
                //Marshal.FreeHGlobal(imgPtrR);
                //Marshal.FreeHGlobal(imgPtrG);
                //Marshal.FreeHGlobal(imgPtrB);


                // 创建固定句柄避免内存复制 
                GCHandle handleR = GCHandle.Alloc(byter, GCHandleType.Pinned);
                GCHandle handleG = GCHandle.Alloc(byteg, GCHandleType.Pinned);
                GCHandle handleB = GCHandle.Alloc(byteb, GCHandleType.Pinned);

                try
                {
                    HOperatorSet.GenImage3(out ho_ImageRainbow, "byte",
                        ImageWidth, ImageHeight,
                        handleR.AddrOfPinnedObject(),
                        handleG.AddrOfPinnedObject(),
                        handleB.AddrOfPinnedObject());
                }
                finally
                {
                    handleR.Free();
                    handleG.Free();
                    handleB.Free();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }

            return ho_ImageRainbow;

        }





        /// <summary>
        /// uint2图像转彩虹图
        /// </summary>
        /// <param name="TifImage"></param>
        /// <returns></returns>
        public static HObject HeightToRainbow(HObject TifImage)
        {
            HObject ho_ImageRainbow;

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_pointer = new HTuple(), hv_type = new HTuple();

            //赤（255，0，0）
            //橙（255，128，0）
            //黄（255，255，0）
            //绿（0，255，0）
            //青（0，255，255）
            //蓝（0，0，255）
            //紫（255，0，255）
            const byte d_oneR = 255, _oneG = 0, _oneB = 255;
            const byte _twoR = 0, u_twoG = 0, _twoB = 255;
            const byte _threeR = 0, _threeG = 255, d_threeB = 255;
            const byte u_fourR = 0, _fourG = 255, _fourB = 0;
            const byte _fiveR = 255, d_fiveG = 255, _fiveB = 0;
            const byte _sixR = 255, _sixG = 0, _sixB = 0;
            int ColorLever = 5 * 256;                                                   //将高度分为5*256级

            HOperatorSet.GenEmptyObj(out ho_ImageRainbow);
            try
            {
                HOperatorSet.GetImagePointer1(TifImage, out hv_pointer, out hv_type, out hv_Width, out hv_Height);
                double DeepMin = 32768;                                                //高度范围下限
                double DeepMax = 1;                                                    //高度范围上限
                double limit = 1;
                int length = (int)hv_Width[0].D * (int)hv_Height[0].D;
                ushort[] destination = new ushort[length];
                unsafe
                {
                    var sourcePtr = (ushort*)hv_pointer[0].IP;
                    for (int i = 0; i < length; ++i)
                    {
                        destination[i] = *sourcePtr++;
                        if (destination[i] < limit)
                            continue;
                        if (destination[i] < DeepMin)
                            DeepMin = destination[i];
                        else if (destination[i] > DeepMax)
                            DeepMax = destination[i];
                    }
                }


                double DeepColor = (DeepMax - DeepMin) / Convert.ToSingle(ColorLever);  //每一级色彩对应的高度
                byte[] byter = new byte[length];                      //红色通道
                byte[] byteg = new byte[length];                      //绿色通道
                byte[] byteb = new byte[length];                      //蓝色通道
                int ipoint = 0;
                foreach (ushort point_z in destination)
                {
                    if (point_z < DeepMin || point_z > DeepMax)
                    {//判断是否超出设定高度范围
                        byter[ipoint] = 0;
                        byteg[ipoint] = 0;
                        byteb[ipoint] = 0;
                    }
                    else
                    {
                        int it = (int)((point_z - DeepMin) / DeepColor);      //判断在哪一层色彩
                        switch (it / 256)
                        {
                            case 0://紫
                                {
                                    byter[ipoint] = Convert.ToByte(d_oneR - it % 256);
                                    byteg[ipoint] = _oneG;
                                    byteb[ipoint] = _oneB;
                                }
                                break;
                            case 1://蓝
                                {
                                    byter[ipoint] = _twoR;
                                    byteg[ipoint] = Convert.ToByte(u_twoG + it % 256);
                                    byteb[ipoint] = _twoB;
                                }
                                break;
                            case 2://青
                                {
                                    byter[ipoint] = _threeR;
                                    byteg[ipoint] = _threeG;
                                    byteb[ipoint] = Convert.ToByte(d_threeB - it % 256);
                                }
                                break;
                            case 3://绿
                                {
                                    byter[ipoint] = Convert.ToByte(u_fourR + it % 256);
                                    byteg[ipoint] = _fourG;
                                    byteb[ipoint] = _fourB;
                                }
                                break;
                            case 4://黄
                                {
                                    byter[ipoint] = _fiveR;
                                    byteg[ipoint] = Convert.ToByte(d_fiveG - it % 256);
                                    byteb[ipoint] = _fiveB;
                                }
                                break;
                            default://赤
                                {
                                    byter[ipoint] = _sixR;
                                    byteg[ipoint] = _sixG;
                                    byteb[ipoint] = _sixB;
                                }
                                break;
                        }
                    }
                    ipoint++;
                }
                IntPtr imgPtrR = Marshal.AllocHGlobal(byter.Length);
                Marshal.Copy(byter, 0, imgPtrR, byter.Length);

                IntPtr imgPtrG = Marshal.AllocHGlobal(byteg.Length);
                Marshal.Copy(byteg, 0, imgPtrG, byteg.Length);

                IntPtr imgPtrB = Marshal.AllocHGlobal(byteb.Length);
                Marshal.Copy(byteb, 0, imgPtrB, byteb.Length);
                ho_ImageRainbow.Dispose();
                HOperatorSet.GenImage3(out ho_ImageRainbow, "byte", hv_Width, hv_Height, imgPtrR, imgPtrG, imgPtrB);
                Marshal.FreeHGlobal(imgPtrR);
                Marshal.FreeHGlobal(imgPtrG);
                Marshal.FreeHGlobal(imgPtrB);
                return ho_ImageRainbow;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return ho_ImageRainbow;
            }
        }

        /// <summary>
        /// 高度数组转uint2图像
        /// </summary>
        /// <param name="Points_Z"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        /// <returns></returns>
        public static HObject TransIntImage(List<double> Points_Z, int ImageWidth, int ImageHeight)
        {
            ushort[] intgray = new ushort[ImageWidth * ImageHeight];
            double limit = -32.767;                                                  //除去小于该值的高度
            int ipoint = 0;
            try
            {
                foreach (double point_z in Points_Z)
                {
                    if (point_z <= limit)
                    {//判断是否超出设定高度范围
                        intgray[ipoint] = 0;
                    }
                    else
                    {
                        ushort sdd = (ushort)((point_z * 1000) + 32768);
                        intgray[ipoint] = sdd;
                    }
                    ipoint++;
                }
                IntPtr imgPtr = GetPtrFromUShortArray(intgray);

                HObject ho_ImageGray = null;
                HOperatorSet.GenImage1(out ho_ImageGray, "uint2", ImageWidth, ImageHeight, imgPtr);

                return ho_ImageGray;
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
                HObject ho_ImageGray = null;
                return ho_ImageGray;
            }

        }

        /// <summary>
        /// 将uint2图像信息转为点云类型
        /// </summary>
        /// <param name="ImageClass"></param>
        public static void UInt2ToDatas(Image3DClass imageclass, out List<CameraObj.SurfacePoint> datas)
        {
            HTuple hv_Pointer = new HTuple(), hv_Type = new HTuple(), hv_Width = new HTuple(), hv_Height = new HTuple();
            datas = new List<CameraObj.SurfacePoint>();
            try
            {
                CameraObj.SurfacePoint point = new CameraObj.SurfacePoint();

                HOperatorSet.GetImagePointer1(imageclass.Image3D, out hv_Pointer, out hv_Type, out hv_Width, out hv_Height);

                imageclass.Info3D.ImageHeight = (int)hv_Height[0].D;
                imageclass.Info3D.ImageWidth = (int)hv_Width[0].D;
                int byteSize = imageclass.Info3D.ImageHeight * imageclass.Info3D.ImageWidth * (16 / 8);//存储整张图像数据(16位int)
                IntPtr ptr = hv_Pointer[0].IP;

                //unshort
                int intSize = byteSize / sizeof(ushort);
                ushort[] intArr = new ushort[intSize];
                intArr = GetUShortArrayFromPtr(ptr, intArr.Length);

                for (int i_y = 0; i_y < imageclass.Info3D.ImageHeight; i_y++)
                {
                    for (int i_x = 0; i_x < imageclass.Info3D.ImageWidth; i_x++)
                    {
                        point.x = i_x * imageclass.Info3D.xResolution + imageclass.Info3D.xOffset;
                        point.y = i_y * imageclass.Info3D.yResolution + imageclass.Info3D.yOffset;
                        point.z = ((double)intArr[i_y * imageclass.Info3D.ImageWidth + i_x] - 32768) / 1000.0;

                        datas.Add(point);
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }

        }

        /// <summary>
        /// 亮度数据转灰度图
        /// <para>说明：点云亮度数据，图像宽度，图像高度</para> 
        /// </summary>
        /// <param name="Point_Light"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        /// <returns></returns>
        public static HObject LightToGray(List<int> Point_Light, int ImageWidth, int ImageHeight)
        {
            //ImageWidth = 1600;
            //ImageHeight = 3300;     //图像宽度、高度
            HObject ho_ImageLight = null;
            HOperatorSet.GenEmptyObj(out ho_ImageLight);
            try
            {
                int[] points_Light = new int[ImageWidth * ImageHeight];
                byte[] bpoints_Light = new byte[ImageWidth * ImageHeight];


                int maxVal = 0;
                int minVal = 99999999;
                int limit = 1;                                                     //除去小于该值的亮度
                foreach (int thisNum in Point_Light)
                {
                    if (thisNum < limit)
                    {
                        continue;
                    }
                    else if (thisNum > maxVal)
                    {
                        maxVal = thisNum;
                    }
                    if (thisNum < minVal)
                    {
                        minVal = thisNum;
                    }
                }
                double Lever = Convert.ToSingle(maxVal - minVal) / 255.0;     //将亮度范围压缩至256级

                points_Light = Point_Light.ToArray();

                for (int i = 0; i < points_Light.Length; i++)
                    bpoints_Light[i] = (byte)(points_Light[i] / Lever);

                IntPtr imgPtrLight = Marshal.AllocHGlobal(bpoints_Light.Length);
                Marshal.Copy(bpoints_Light, 0, imgPtrLight, bpoints_Light.Length);
                ho_ImageLight.Dispose();
                HOperatorSet.GenImage3(out ho_ImageLight, "byte", ImageWidth, ImageHeight, imgPtrLight, imgPtrLight, imgPtrLight);
                //HOperatorSet.WriteImage(ho_ImageLight, "bmp", 0, "F:/222.bmp");
                Marshal.FreeHGlobal(imgPtrLight);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
            return ho_ImageLight;
        }

        /// <summary>
        /// ushort数组转指针
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IntPtr GetPtrFromUShortArray(ushort[] value)
        {
            unsafe
            {
                fixed (ushort* pa = &value[0])
                {
                    return new IntPtr(pa);
                }
            }
        }

        /// <summary>
        /// 指针转ushort数组
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static ushort[] GetUShortArrayFromPtr(IntPtr ptr, int length)
        {
            unsafe
            {
                ushort[] destination = new ushort[length];
                var sourcePtr = (ushort*)ptr;
                for (int i = 0; i < length; ++i)
                {
                    destination[i] = *sourcePtr++;
                }
                return destination;
            }
        }


    }
}
