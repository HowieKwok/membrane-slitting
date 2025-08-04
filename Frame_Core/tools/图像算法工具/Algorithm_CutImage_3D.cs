using BitMiracle.LibTiff.Classic;
using HalconDotNet;

using LogTool;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class Algorithm_CutImage_3D
    {
        HObject _Image = null;
        private List<CameraObj.SurfacePoint> _data = null;
        private List<CameraObj.SurfacePoint> _Cutdata = new List<CameraObj.SurfacePoint>();                     //裁剪后的点云数据
        private List<List<CameraObj.SurfacePoint>> _datas = null;                                               //保存3D数据
        private List<double> _points_z = null;                                                                  //读取的3D深度数据 
        //private List<HObject> Img = null;                                                                       //2D图像
        private HObject _split_region = null;
        private int _camid = 0;
        public Image3DClass ImageClass = new Image3DClass();
        public string ProductSeat = "A1";
        public HTuple WindowHandle = null;
        public Algorithm_CutImage_3D(int camid)
        {
            HOperatorSet.GenEmptyObj(out _Image);
            _data = new List<CameraObj.SurfacePoint>();
            _datas = new List<List<CameraObj.SurfacePoint>>();
            _points_z = new List<double>();
            WindowHandle = new HTuple();
            //Img = new List<HObject>();
            _camid = camid;
            ImageClass.Info3D = new CameraObj.Cam_Calibration(10, 10, 0.005, 0.005, 0.00001, 0, 0, 0);
        }

        public Algorithm_CutImage_3D()
        {
            HOperatorSet.GenEmptyObj(out _Image);
            _data = new List<CameraObj.SurfacePoint>();
            _datas = new List<List<CameraObj.SurfacePoint>>();
            _points_z = new List<double>();
            //Img = new List<HObject>();
            WindowHandle = new HTuple();
            _camid = 0;
            ImageClass.Info3D = new CameraObj.Cam_Calibration(10, 10, 0.005, 0.005, 0.00001, 0, 0, 0);

        }

        public void Dispose()
        {
            if (_Image != null)
                _Image.Dispose();
            _data = null;
            _datas = null;
            _points_z = null;
            ImageClass.Dispose();

        }

        public HObject Image
        {
            get { return _Image; }
        }

        public List<List<CameraObj.SurfacePoint>> Datas
        {
            get { return _datas; }
            set { _datas = value; }
        }

        public List<CameraObj.SurfacePoint> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public List<CameraObj.SurfacePoint> CutData
        {
            get { return _Cutdata; }
            set { _Cutdata = value; }
        }

        public List<double> Points_z
        {
            get { return _points_z; }
        }

        public HObject SplitRegion
        {
            get { return _split_region; }
        }

        public HObject TransIntImage(List<double> Points_Z, int ImageWidth, int ImageHeight)
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
                //  HOperatorSet.WriteImage(ho_ImageGray, "tiff", 0, "F:/01.tiff");

                return ho_ImageGray;
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
                HObject ho_ImageGray = null;
                return ho_ImageGray;
            }

        }

        #region//2022-06-06,lfh
        /// <summary>
        /// 亮度数据转灰度图
        /// <para>说明：点云亮度数据，图像宽度，图像高度</para> 
        /// </summary>
        /// <param name="Point_Light"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        /// <returns></returns>
        public HObject LightToGray(List<double> Point_Light, int ImageWidth, int ImageHeight)
        {
            HObject ho_ImageLight = null;

            try
            {
                double[] points_Light = new double[ImageWidth * ImageHeight];
                byte[] bpoints_Light = new byte[ImageWidth * ImageHeight];


                double maxVal = 0;
                double minVal = 99999999;
                double limit = 1;                                                  //除去小于该值的亮度
                foreach (double thisNum in Point_Light)
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
        #endregion

        public HObject DoubleArraysTransIntImage(double[] Points_Z, int ImageWidth, int ImageHeight)
        {
            ushort[] intgray = new ushort[ImageWidth * ImageHeight];
            double limit = -32.767;                                                  //除去小于该值的高度
            int ipoint = 0;
            HObject ho_ImageGray = null;
            HOperatorSet.GenEmptyObj(out ho_ImageGray);
            try
            {
                foreach (double point_z in Points_Z)
                {
                    if (ipoint < intgray.Length)
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
                    }
                    else
                    {
                        break;
                    }

                    ipoint++;
                }
                IntPtr imgPtr = GetPtrFromUShortArray(intgray);


                ho_ImageGray.Dispose();
                HOperatorSet.GenImage1(out ho_ImageGray, "uint2", ImageWidth, ImageHeight, imgPtr);

                return ho_ImageGray;
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
                ho_ImageGray = null;
                return ho_ImageGray;
            }

        }

        public HObject TransIntImage(List<CameraObj.SurfacePoint> Point, int ImageWidth, int ImageHeight)
        {
            #region 初始化
            ushort[] intgray = new ushort[ImageWidth * ImageHeight];
            double limit = -32.767;                                                  //除去小于该值的高度
            int ipoint = 0;
            HObject ho_ImageGray = null;
            HOperatorSet.GenEmptyObj(out ho_ImageGray);
            #endregion

            try
            {
                foreach (CameraObj.SurfacePoint point_z in Point)
                {
                    if (point_z.z <= limit)
                    {
                        intgray[ipoint] = 0;                                        //判断是否超出设定高度范围
                    }
                    else
                    {
                        ushort sdd = (ushort)((point_z.z * 1000) + 32768);
                        intgray[ipoint] = sdd;
                    }
                    ipoint++;
                }
                IntPtr imgPtr = GetPtrFromUShortArray(intgray);
                //用高度生成新图像
                ho_ImageGray.Dispose();
                HOperatorSet.GenImage1(out ho_ImageGray, "uint2", ImageWidth, ImageHeight, imgPtr);
                return ho_ImageGray;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
                return ho_ImageGray;
            }
            finally
            {

            }
        }

        //public void AutoCutImage_3D(HObject image, out int YStart, out int YEnd)
        //{
        //    HObject ho_Region = null, ho_RegionTrans = null;
        //    HTuple hv_YStart = new HTuple(), hv_YEnd = new HTuple();
        //    YStart = 0;
        //    YEnd = 0;

        //    try
        //    {
        //        HOperatorSet.Threshold(image, out ho_Region, 1, 255);
        //        HOperatorSet.ClosingRectangle1(ho_Region, out ho_Region, 1, 100);
        //        HOperatorSet.Connection(ho_Region, out ho_Region);
        //        HOperatorSet.SelectShapeStd(ho_Region, out ho_Region, "max_area", 70);
        //        HOperatorSet.ShapeTrans(ho_Region, out ho_RegionTrans, "rectangle1");
        //        HOperatorSet.RegionFeatures(ho_RegionTrans, "row1", out hv_YStart);
        //        HOperatorSet.RegionFeatures(ho_RegionTrans, "row2", out hv_YEnd);

        //        YStart = (int)hv_YStart[0].D;
        //        YEnd = (int)hv_YEnd[0].D;
        //        string type = _camid == 0 ? "true" : "false";//相机编号为0时从上往下，相机编号为1时从下往上排序
        //        SplitDefectRegion(type);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //public void AutoCutImage_3D(Image3DClass image3d, out int YStart, out int YEnd)
        //{
        //    HObject ho_Region = null, ho_RegionTrans = null;
        //    HTuple hv_YStart = new HTuple(), hv_YEnd = new HTuple();
        //    HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
        //    YStart = 0;
        //    YEnd = 0;
        //    HOperatorSet.GenEmptyObj(out ho_Region);
        //    HOperatorSet.GenEmptyObj(out ho_RegionTrans);
        //    try
        //    {
        //        ho_Region.Dispose();
        //        HOperatorSet.Threshold(image3d.Image2D, out ho_Region, 50, 255);
        //        HOperatorSet.ClosingRectangle1(ho_Region, out ho_Region, 1, 100);
        //        HOperatorSet.Connection(ho_Region, out ho_Region);
        //        HOperatorSet.SelectShapeStd(ho_Region, out ho_Region, "max_area", 70);
        //        ho_RegionTrans.Dispose();
        //        HOperatorSet.ShapeTrans(ho_Region, out ho_RegionTrans, "rectangle1");
        //        HOperatorSet.RegionFeatures(ho_RegionTrans, "row1", out hv_YStart);
        //        HOperatorSet.RegionFeatures(ho_RegionTrans, "row2", out hv_YEnd);
        //        HOperatorSet.GetImageSize(ImageClass.Image2D, out hv_ImageWidth, out hv_ImageHeight);

        //        YStart = (int)hv_YStart[0].D;
        //        YEnd = (int)hv_YEnd[0].D;

        //        for (int iy = (int)hv_YStart[0].D; iy < (int)hv_YEnd[0].D; iy++)
        //        {
        //            int startpoint = Convert.ToInt32(iy * (int)hv_ImageWidth[0].D);//每一行起始点
        //            for (int ix = 0; ix < (int)hv_ImageWidth[0].D; ix++)
        //            {
        //                _Cutdata.Add(_data[startpoint + ix]);
        //            }
        //        }
        //        //_datas.Add(_Cutdata);

        //        ImageClass.Info3D = image3d.Info3D;

        //        ImageClass.Info3D.ImageHeight = (int)hv_ImageHeight[0].D;
        //        ImageClass.Info3D.ImageWidth = (int)hv_ImageWidth[0].D;
        //        string type = _camid == 0 ? "true" : "false";//相机编号为0时从上往下，相机编号为1时从下往上排序
        //        SplitDefectRegion(type);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Fatal(ex);
        //    }
        //    finally
        //    {
        //        if (ho_Region != null)
        //            ho_Region.Dispose();
        //        if (ho_RegionTrans != null)
        //            ho_RegionTrans.Dispose();
        //    }
        //}

        //public bool AutoCutImage_3D(Image3DClass image3d)
        //{
        //    HTuple Time1 = new HTuple();
        //    HTuple Time2 = new HTuple();
        //    HTuple Time3 = new HTuple();
        //    HTuple Time4 = new HTuple();
        //    HTuple Time5 = new HTuple();
        //    if (image3d == null)
        //        return false;
        //    //_datas.Clear();
        //    HOperatorSet.CountSeconds(out Time1);      //////////////////////////////////////////////////////////////////////////////
        //    ImageClass = new Image3DClass();
        //    HObject ho_Region, ho_RegionTrans, ImageReduced, TiffImageReduced;
        //    HObject ho_Rectangle;
        //    HTuple hv_YStart = new HTuple();
        //    HTuple hv_ImageHeight = new HTuple(), hv_ImageWidth = new HTuple();
        //    HTuple hv_ImageHeight1 = new HTuple(), hv_ImageWidth1 = new HTuple();
        //    HOperatorSet.GenEmptyObj(out ho_Region);
        //    HOperatorSet.GenEmptyObj(out ho_RegionTrans);
        //    HOperatorSet.GenEmptyObj(out ImageReduced);
        //    HOperatorSet.GenEmptyObj(out TiffImageReduced);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangle);
        //    try
        //    {
        //        ho_Region.Dispose();
        //        HOperatorSet.Threshold(image3d.Image2D, out ho_Region, 50, 255);
        //        HOperatorSet.ClosingRectangle1(ho_Region, out ho_Region, 1, 100);
        //        HOperatorSet.Connection(ho_Region, out ho_Region);
        //        HOperatorSet.CountSeconds(out Time2);   ///////////////////////////////////////////////////////////////////////////
        //        HOperatorSet.SelectShapeStd(ho_Region, out ho_Region, "max_area", 70);
        //        ho_RegionTrans.Dispose();
        //        HOperatorSet.DilationRectangle1(ho_Region, out ho_RegionTrans, 800, 1);
        //        //HOperatorSet.ShapeTrans(ho_Region, out ho_RegionTrans, "rectangle1");
        //        HOperatorSet.RegionFeatures(ho_RegionTrans, "row1", out hv_YStart);
        //        ImageReduced.Dispose();
        //        HOperatorSet.CountSeconds(out Time3);   ///////////////////////////////////////////////////////////////////////////
        //        HOperatorSet.ReduceDomain(image3d.Image2D, ho_RegionTrans, out ImageReduced);
        //        HOperatorSet.CropDomain(ImageReduced, out ImageClass.Image2D);
        //        TiffImageReduced.Dispose();
        //        HOperatorSet.ReduceDomain(image3d.Image3D, ho_RegionTrans, out TiffImageReduced);

        //        HOperatorSet.CropDomain(TiffImageReduced, out ImageClass.Image3D);
        //        HOperatorSet.GetImageSize(ImageClass.Image2D, out hv_ImageWidth1, out hv_ImageHeight1);

        //        int Cut_Width_Left = 0;
        //        int Cut_Width_Right = 0;
        //        switch (ProductSeat)
        //        {
        //            case "A1":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_A1Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_A1Right[_camid];
        //                break;
        //            case "A2":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_A2Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_A2Right[_camid];
        //                break;
        //            case "A3":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_A3Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_A3Right[_camid];
        //                break;
        //            case "A4":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_A4Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_A4Right[_camid];
        //                break;
        //            case "B1":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_B1Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_B1Right[_camid];
        //                break;
        //            case "B2":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_B2Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_B2Right[_camid];
        //                break;
        //            case "B3":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_B3Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_B3Right[_camid];
        //                break;
        //            case "B4":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_B4Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_B4Right[_camid];
        //                break;
        //        }

        //        //裁剪宽度(尾部)
        //        HOperatorSet.GenRectangle1(out ho_Rectangle, 0, Cut_Width_Left, hv_ImageHeight1, hv_ImageWidth1 - Cut_Width_Right);

        //        HOperatorSet.ReduceDomain(ImageClass.Image2D, ho_Rectangle, out ImageReduced);
        //        HOperatorSet.CropDomain(ImageReduced, out ImageClass.Image2D);

        //        HOperatorSet.ReduceDomain(ImageClass.Image3D, ho_Rectangle, out TiffImageReduced);
        //        HOperatorSet.CropDomain(TiffImageReduced, out ImageClass.Image3D);
        //        HOperatorSet.GetImageSize(ImageClass.Image3D, out hv_ImageWidth, out hv_ImageHeight);

        //        HOperatorSet.CountSeconds(out Time4);   ///////////////////////////////////////////////////////////////////////////
        //        int width1 = hv_ImageWidth1[0].I;//原始宽度
        //        int height1 = hv_ImageHeight1[0].I;//原始高度
        //        int width = hv_ImageWidth[0].I;//裁剪后宽度
        //        int height = hv_ImageHeight[0].I;//裁剪后高度
        //        int ystart = (int)hv_YStart[0].D;

        //        CameraObj.SurfacePoint[] cutpoints = new CameraObj.SurfacePoint[width * height];
        //        for (int iy = 0; iy < height; iy++)
        //        {
        //            int startpoint = (ystart + iy) * width1;//每一行起始点
        //            for (int ix = 0; ix < width; ix++)
        //            {
        //                cutpoints[iy * width + ix] = _data[startpoint + ix];
        //                cutpoints[iy * width + ix].y = cutpoints[iy * width + ix].y - ystart * image3d.Info3D.yResolution;
        //            }
        //        }
        //        _Cutdata = new List<CameraObj.SurfacePoint>(cutpoints);
        //        //_datas.Add(_Cutdata);

        //        //ImageClass = new Image3DClass(TiffImageReduced, ImageReduced, image3d.Info3D);
        //        ImageClass.Info3D = image3d.Info3D;
        //        ImageClass.Info3D.ImageHeight = (int)hv_ImageHeight[0].D;
        //        ImageClass.Info3D.ImageWidth = (int)hv_ImageWidth[0].D;
        //        string type = _camid == 0 ? "true" : "false";//相机编号为0时从上往下，相机编号为1时从下往上排序
        //        SplitDefectRegion(type);
        //        HOperatorSet.CountSeconds(out Time5);   ///////////////////////////////////////////////////////////////////////////

        //        //double t1 = Time1;
        //        //double t2 = Time2;
        //        //double t3 = Time3;
        //        //double t4 = Time4;
        //        //double t5 = Time5;
        //        ////  double t6 = Time6;
        //        ////  double t7 = Time7;

        //        //Log4netRecord.RunningLog.Info("自动裁剪耗时统计：\r\n" +
        //        //                        "阈值分割：" + (Convert.ToInt32((t2 - t1) * 1000)).ToString() + " ms\r\n" +
        //        //                      "选择区域：" + (Convert.ToInt32((t3 - t2) * 1000)).ToString() + " ms\r\n" +
        //        //                        "裁剪：" + (Convert.ToInt32((t4 - t3) * 1000)).ToString() + " ms\r\n" +
        //        //                          "点云数据获取：" + (Convert.ToInt32((t5 - t4) * 1000)).ToString() + " ms\r\n");
        //        //  "轮廓线检测：" + (Convert.ToInt32((t6 - t5) * 1000)).ToString() + " ms\r\n"+
        //        //"显示轮廓差异：" + (Convert.ToInt32((t7 - t6) * 1000)).ToString() + " ms\r\n");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Fatal(ex);
        //        return false;
        //    }
        //    finally
        //    {
        //        if (ho_Region != null)
        //            ho_Region.Dispose();
        //        if (ho_RegionTrans != null)
        //            ho_RegionTrans.Dispose();
        //        if (ImageReduced != null)
        //            ImageReduced.Dispose();
        //        if (TiffImageReduced != null)
        //            TiffImageReduced.Dispose();
        //    }
        //}

        //public bool AutoCutImage_3D(Image3DClass image3d, HObject LightImage, out HObject CutLightImage)
        //{
        //    HOperatorSet.GenEmptyObj(out CutLightImage);
        //    if (image3d == null)
        //        return false;

        //    ImageClass = new Image3DClass();
        //    HObject ho_Region, ho_RegionTrans, ImageReduced, TiffImageReduced;
        //    HObject ho_Rectangle;
        //    HTuple hv_YStart = new HTuple();
        //    HTuple hv_ImageHeight = new HTuple(), hv_ImageWidth = new HTuple();
        //    HTuple hv_ImageHeight1 = new HTuple(), hv_ImageWidth1 = new HTuple();
        //    HTuple hv_RegionWidth = new HTuple();
        //    HOperatorSet.GenEmptyObj(out ho_Region);
        //    HOperatorSet.GenEmptyObj(out ho_RegionTrans);
        //    HOperatorSet.GenEmptyObj(out ImageReduced);
        //    HOperatorSet.GenEmptyObj(out TiffImageReduced);
        //    HOperatorSet.GenEmptyObj(out ho_Rectangle);
        //    try
        //    {
        //        ho_Region.Dispose();
        //        HOperatorSet.Threshold(image3d.Image2D, out ho_Region, 20, 255);
        //        HOperatorSet.OpeningCircle(ho_Region, out ho_Region, 7);
        //        //HOperatorSet.DispObj(ho_Region, WindowHandle);
        //        HOperatorSet.ClosingRectangle1(ho_Region, out ho_Region, 1, 100);
        //        HOperatorSet.FillUp(ho_Region, out ho_Region);
        //        HOperatorSet.Connection(ho_Region, out ho_Region);

        //        HOperatorSet.SelectShapeStd(ho_Region, out ho_Region, "max_area", 70);
        //        HOperatorSet.RegionFeatures(ho_Region, "width", out hv_RegionWidth);
        //        if (hv_RegionWidth[0].D == 0)
        //        {//如图像为黑图
        //            HOperatorSet.Threshold(image3d.Image2D, out ho_Region, 0, 255);
        //            HOperatorSet.RegionFeatures(ho_Region, "width", out hv_RegionWidth);
        //        }
        //        HOperatorSet.OpeningRectangle1(ho_Region, out ho_Region, hv_RegionWidth[0].D / 3, 1);
        //        ho_RegionTrans.Dispose();
        //        HOperatorSet.DilationRectangle1(ho_Region, out ho_RegionTrans, 800, 1);
        //        HOperatorSet.RegionFeatures(ho_RegionTrans, "row1", out hv_YStart);
        //        ImageReduced.Dispose();
        //        HOperatorSet.ReduceDomain(image3d.Image2D, ho_RegionTrans, out ImageReduced);
        //        HOperatorSet.CropDomain(ImageReduced, out ImageClass.Image2D);

        //        TiffImageReduced.Dispose();
        //        HOperatorSet.ReduceDomain(image3d.Image3D, ho_RegionTrans, out TiffImageReduced);
        //        HOperatorSet.CropDomain(TiffImageReduced, out ImageClass.Image3D);

        //        HOperatorSet.ReduceDomain(LightImage, ho_RegionTrans, out CutLightImage);
        //        HOperatorSet.CropDomain(CutLightImage, out CutLightImage);

        //        HOperatorSet.GetImageSize(ImageClass.Image2D, out hv_ImageWidth1, out hv_ImageHeight1);

        //        int Cut_Width_Left = 0;
        //        int Cut_Width_Right = 0;
        //        switch (ProductSeat)
        //        {
        //            case "A1":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_A1Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_A1Right[_camid];
        //                break;
        //            case "A2":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_A2Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_A2Right[_camid];
        //                break;
        //            case "A3":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_A3Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_A3Right[_camid];
        //                break;
        //            case "A4":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_A4Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_A4Right[_camid];
        //                break;
        //            case "B1":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_B1Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_B1Right[_camid];
        //                break;
        //            case "B2":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_B2Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_B2Right[_camid];
        //                break;
        //            case "B3":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_B3Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_B3Right[_camid];
        //                break;
        //            case "B4":
        //                Cut_Width_Left = UIRecipe.CutImageSettingParameter.Cut_Width_B4Left[_camid];
        //                Cut_Width_Right = UIRecipe.CutImageSettingParameter.Cut_Width_B4Right[_camid];
        //                break;
        //        }

        //        //裁剪宽度(尾部)
        //        HOperatorSet.GenRectangle1(out ho_Rectangle, 0, Cut_Width_Left, hv_ImageHeight1, hv_ImageWidth1 - Cut_Width_Right);
        //        //裁剪宽度(尾部)
        //        //HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_ImageHeight1, hv_ImageWidth1 - UIRecipe.CutImageSettingParameter.Cut_Width[_camid]);

        //        HOperatorSet.ReduceDomain(ImageClass.Image2D, ho_Rectangle, out ImageReduced);
        //        HOperatorSet.CropDomain(ImageReduced, out ImageClass.Image2D);

        //        HOperatorSet.ReduceDomain(ImageClass.Image3D, ho_Rectangle, out TiffImageReduced);
        //        HOperatorSet.CropDomain(TiffImageReduced, out ImageClass.Image3D);

        //        HOperatorSet.ReduceDomain(CutLightImage, ho_Rectangle, out CutLightImage);
        //        HOperatorSet.CropDomain(CutLightImage, out CutLightImage);

        //        HOperatorSet.GetImageSize(ImageClass.Image3D, out hv_ImageWidth, out hv_ImageHeight);


        //        int width1 = hv_ImageWidth1[0].I;//原始宽度
        //        int height1 = hv_ImageHeight1[0].I;//原始高度
        //        int width = hv_ImageWidth[0].I;//裁剪后宽度
        //        int height = hv_ImageHeight[0].I;//裁剪后高度
        //        int ystart = (int)hv_YStart[0].D;
        //        int xstart = Cut_Width_Left;

        //        CameraObj.SurfacePoint[] cutpoints = new CameraObj.SurfacePoint[width * height];
        //        for (int iy = 0; iy < height; iy++)
        //        {
        //            int startpoint = (ystart + iy) * width1 + xstart;//每一行起始点
        //            for (int ix = 0; ix < width; ix++)
        //            {
        //                cutpoints[iy * width + ix] = _data[startpoint + ix];
        //                cutpoints[iy * width + ix].y = cutpoints[iy * width + ix].y - ystart * image3d.Info3D.yResolution;
        //            }
        //        }
        //        _Cutdata = new List<CameraObj.SurfacePoint>(cutpoints);

        //        ImageClass.Info3D = image3d.Info3D;
        //        ImageClass.Info3D.ImageHeight = (int)hv_ImageHeight[0].D;
        //        ImageClass.Info3D.ImageWidth = (int)hv_ImageWidth[0].D;

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Fatal(ex);
        //        return false;
        //    }
        //    finally
        //    {
        //        if (ho_Region != null)
        //            ho_Region.Dispose();
        //        if (ho_RegionTrans != null)
        //            ho_RegionTrans.Dispose();
        //        if (ImageReduced != null)
        //            ImageReduced.Dispose();
        //        if (TiffImageReduced != null)
        //            TiffImageReduced.Dispose();
        //    }
        //}

        /// <summary>
        /// 划分缺陷区域
        /// </summary>
        /// <param name="type"></param>
        //public void SplitDefectRegion(string type)
        //{
        //    int CutYStart = UIRecipe.CutImageSettingParameter.Cut_YStart[_camid];
        //    int CutYEnd = UIRecipe.CutImageSettingParameter.Cut_YEnd[_camid];
        //    int RStartHeight = UIRecipe.CutImageSettingParameter.RStart_RegionHeight[_camid];
        //    int REndHeight = UIRecipe.CutImageSettingParameter.REnd_RegionHeight[_camid];

        //    HObject ho_Region = null, ho_StartRegion = null, ho_EndRegion = null;
        //    HTuple hv_Width = new HTuple(), hv_Height = new HTuple();

        //    HOperatorSet.GenEmptyObj(out _split_region);
        //    HOperatorSet.GenEmptyObj(out ho_Region);
        //    HOperatorSet.GenEmptyObj(out ho_StartRegion);
        //    HOperatorSet.GenEmptyObj(out ho_EndRegion);
        //    try
        //    {
        //        HOperatorSet.GetImageSize(ImageClass.Image2D, out hv_Width, out hv_Height);
        //        ho_Region.Dispose();
        //        HOperatorSet.Threshold(ImageClass.Image2D, out ho_Region, 0, 255);
        //        ho_StartRegion.Dispose();
        //        ho_EndRegion.Dispose();
        //        HOperatorSet.GenRectangle1(out ho_StartRegion, 0, 0, CutYStart + RStartHeight, hv_Width);
        //        HOperatorSet.GenRectangle1(out ho_EndRegion, hv_Height[0].D - CutYEnd - REndHeight, 0, hv_Height, hv_Width);
        //        HOperatorSet.Difference(ho_Region, ho_StartRegion, out ho_Region);
        //        HOperatorSet.Difference(ho_Region, ho_EndRegion, out ho_Region);
        //        HOperatorSet.PartitionRectangle(ho_Region, out ho_Region, hv_Width, 500);
        //        HOperatorSet.ConcatObj(ho_Region, ho_StartRegion, out ho_Region);
        //        HOperatorSet.ConcatObj(ho_Region, ho_EndRegion, out ho_Region);
        //        _split_region.Dispose();
        //        HOperatorSet.SortRegion(ho_Region, out _split_region, "upper_left", type, "row");//true为从上到下, false为从下到上
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Fatal(ex);
        //    }
        //    finally
        //    {
        //        if (ho_Region != null)
        //            ho_Region.Dispose();
        //        if (ho_StartRegion != null)
        //            ho_StartRegion.Dispose();
        //        if (ho_EndRegion != null)
        //            ho_EndRegion.Dispose();
        //    }

        //}

        /// <summary>
        /// 将圆形按角度等分分割（顺时针）2020.5.8(叶)
        /// </summary>
        /// <param name="Region_Circle"></param>
        /// <param name="SplitNum"></param>
        /// <param name="StartAngle"></param>
        /// <returns></returns>
        public HObject SplitCircle(HObject Region_Circle, int SplitNum, double StartAngle, out HObject ho_ShowCircle)
        {
            double pi = 3.1415926;
            double anglestep = 2 * pi / SplitNum;       //每次旋转的角度
            double angle = StartAngle;                  //设置初始角度
            HObject ho_Lines = null, ho_RegionLines = null, ho_CircleSplit = null;
            HObject ho_Line = null, ho_Line1 = null;
            HObject ho_RegionUnion = null, ho_RegionTrans = null, ho_RegionIntersection = null;
            HObject ho_DilationCircle = null;
            HTuple hv_CenterY = new HTuple(), hv_CenterX = new HTuple(), hv_Radius = new HTuple();

            ho_ShowCircle = null;
            HOperatorSet.GenEmptyObj(out ho_ShowCircle);
            HOperatorSet.GenEmptyObj(out ho_CircleSplit);

            HOperatorSet.GenEmptyObj(out ho_Lines);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.GenEmptyObj(out ho_Line1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_DilationCircle);
            try
            {
                ho_DilationCircle.Dispose();
                HOperatorSet.DilationCircle(Region_Circle, out ho_DilationCircle, 200);
                HOperatorSet.SmallestCircle(ho_DilationCircle, out hv_CenterY, out hv_CenterX, out hv_Radius);
                double radius = hv_Radius.TupleSelect(0) + 50;

                for (int i = 0; i < SplitNum; i++)
                {
                    double sina = Math.Sin(-angle);     //负号为按照顺时针方向
                    double cosa = Math.Cos(-angle);
                    double lineX = hv_CenterX + radius * sina;
                    double lineY = hv_CenterY + radius * cosa;
                    ho_RegionLines.Dispose();
                    HOperatorSet.GenRegionLine(out ho_RegionLines, hv_CenterY, hv_CenterX, lineY, lineX);
                    HOperatorSet.ConcatObj(ho_Lines, ho_RegionLines, out ho_Lines);
                    angle = angle + anglestep;
                }
                for (int i = 0; i < SplitNum; i++)
                {
                    ho_Line.Dispose();
                    ho_Line1.Dispose();
                    HOperatorSet.SelectObj(ho_Lines, out ho_Line, i + 1);
                    if (i == SplitNum - 1)
                        HOperatorSet.SelectObj(ho_Lines, out ho_Line1, 1);
                    else
                        HOperatorSet.SelectObj(ho_Lines, out ho_Line1, i + 2);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union2(ho_Line, ho_Line1, out ho_RegionUnion);
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionUnion, out ho_RegionTrans, "convex");
                    ho_RegionIntersection.Dispose();
                    HOperatorSet.Intersection(ho_RegionTrans, ho_DilationCircle, out ho_RegionIntersection);
                    HOperatorSet.ConcatObj(ho_CircleSplit, ho_RegionIntersection, out ho_CircleSplit);
                }
                ho_ShowCircle.Dispose();
                HOperatorSet.Intersection(ho_CircleSplit, Region_Circle, out ho_ShowCircle);
                return ho_CircleSplit;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
                return null;
            }
            finally
            {
                if (ho_Lines != null)
                    ho_Lines.Dispose();
                if (ho_RegionLines != null)
                    ho_RegionLines.Dispose();
                if (ho_Line != null)
                    ho_Line.Dispose();
                if (ho_Line1 != null)
                    ho_Line1.Dispose();
                if (ho_RegionUnion != null)
                    ho_RegionUnion.Dispose();
                if (ho_RegionTrans != null)
                    ho_RegionTrans.Dispose();
                if (ho_RegionIntersection != null)
                    ho_RegionIntersection.Dispose();
                if (ho_DilationCircle != null)
                    ho_DilationCircle.Dispose();
            }

        }

        public void Load_3DdataFromCSV(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader SR = new StreamReader(fs, System.Text.Encoding.GetEncoding(936));
            try
            {
                //读取点云           
                _points_z = new List<double>();
                _data = new List<CameraObj.SurfacePoint>();

                CameraObj.SurfacePoint point = new CameraObj.SurfacePoint();

                string nextLine = "";
                string TestCSV = "";
                int LineNum = 0;

                //获取图像参数
                while ((nextLine = SR.ReadLine()) != null)
                {
                    string[] SArray = nextLine.Split(',');
                    if (LineNum == 0)
                    {
                        TestCSV = SArray[0];
                    }

                    if (TestCSV == "WidthCali")
                    {
                        if (LineNum == 1)
                        {

                            ImageClass.Info3D.xResolution = double.Parse(SArray[0]);
                            ImageClass.Info3D.yResolution = double.Parse(SArray[1]);
                            ImageClass.Info3D.zResolution = double.Parse(SArray[2]);
                            ImageClass.Info3D.ImageWidth = int.Parse(SArray[3]);
                            ImageClass.Info3D.ImageHeight = int.Parse(SArray[4]);
                            ImageClass.Info3D.xOffset = double.Parse(SArray[5]);
                            ImageClass.Info3D.yOffset = double.Parse(SArray[6]);
                            ImageClass.Info3D.zOffset = double.Parse(SArray[7]);
                        }
                        else if (LineNum > 0)
                        {

                            for (int i = 0; i < ImageClass.Info3D.ImageWidth; i++)     //int i = 0; i < SArray.Count() - 1; i++
                            {
                                point.x = i * ImageClass.Info3D.xResolution + ImageClass.Info3D.xOffset;
                                point.y = (LineNum - 2) * ImageClass.Info3D.yResolution + ImageClass.Info3D.yOffset;
                                point.z = double.Parse(SArray[i]);
                                if (point.z == -7.9896)
                                {
                                    point.z = -32.768;
                                }
                                _points_z.Add(point.z);
                                _data.Add(point);

                            }
                        }
                    }
                    else if (TestCSV == "Info")
                    {
                        int iy = 1;
                        string rl;
                        double X_Resolution = 0.0, Y_Resolution = 0.0, Z_Resolution = 0.0;
                        double X_Offset = 0.0, Y_Offset = 0.0, Z_Offset = 0.0;
                        int ImageHeight = 0, ImageWidth = 0;

                        while ((rl = SR.ReadLine()) != null)
                        {
                            // 存成二维表结构

                            string[] str = rl.Split(',');
                            for (int ix = 0; ix < str.Length; ix++)
                            {
                                //读取点云基本参数
                                switch (iy)
                                {
                                    case 2:
                                        switch (ix)
                                        {
                                            case 7: X_Resolution = Convert.ToSingle(str[ix]); break;
                                            case 8: Y_Resolution = Convert.ToSingle(str[ix]); break;
                                            case 9: Z_Resolution = Convert.ToSingle(str[ix]); break;
                                        }
                                        break;
                                    case 26:
                                        switch (ix)
                                        {
                                            case 6: ImageHeight = Convert.ToInt32(str[ix]); break;
                                            case 7: ImageWidth = Convert.ToInt32(str[ix]); break;
                                            case 8: X_Offset = Convert.ToSingle(str[ix]); break;
                                            case 9: Y_Offset = Convert.ToSingle(str[ix]); break;
                                            case 10: Z_Offset = Convert.ToSingle(str[ix]); break;
                                        }
                                        break;
                                    default:
                                        {
                                            //读取点云数据
                                            if ((ix >= 1 && ix <= ImageWidth) && (iy >= 28 && iy <= 27 + ImageHeight))
                                            {
                                                if (str[ix] == "")
                                                {
                                                    _points_z.Add(-19.9999);
                                                    point.x = (ix - 1) * ImageClass.Info3D.xResolution + ImageClass.Info3D.xOffset;
                                                    point.y = (iy - 28) * ImageClass.Info3D.yResolution + ImageClass.Info3D.yOffset;
                                                    point.z = -19.9999;
                                                    _data.Add(point);
                                                }
                                                else
                                                {
                                                    _points_z.Add(Convert.ToSingle(str[ix]));
                                                    point.x = (ix - 1) * ImageClass.Info3D.xResolution + ImageClass.Info3D.xOffset;
                                                    point.y = (iy - 28) * ImageClass.Info3D.yResolution + ImageClass.Info3D.yOffset;
                                                    point.z = Convert.ToSingle(str[ix]);
                                                    _data.Add(point);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            iy++;
                        }

                        ////////////////////////////
                        ImageClass.Info3D.xResolution = X_Resolution;
                        ImageClass.Info3D.yResolution = Y_Resolution;
                        ImageClass.Info3D.zResolution = Z_Resolution;
                        ImageClass.Info3D.ImageWidth = ImageWidth;
                        ImageClass.Info3D.ImageHeight = ImageHeight;
                        ImageClass.Info3D.xOffset = X_Offset;
                        ImageClass.Info3D.yOffset = Y_Offset;
                        ImageClass.Info3D.zOffset = Z_Offset;
                    }
                    else
                    {
                        if (LineNum == 0)
                        {

                            ImageClass.Info3D.xResolution = 0.005;
                            ImageClass.Info3D.yResolution = 0.005;
                            ImageClass.Info3D.zResolution = 0.00001;
                            ImageClass.Info3D.ImageWidth = SArray.Count() - 1;
                            ImageClass.Info3D.xOffset = 0;
                            ImageClass.Info3D.yOffset = 0;
                            ImageClass.Info3D.zOffset = 0;
                        }
                        else if (LineNum >= 0)
                        {
                            for (int i = 0; i < ImageClass.Info3D.ImageWidth; i++)     //int i = 0; i < SArray.Count() - 1; i++
                            {
                                point.x = i * ImageClass.Info3D.xResolution + ImageClass.Info3D.xOffset;
                                point.y = (LineNum - 2) * ImageClass.Info3D.yResolution + ImageClass.Info3D.yOffset;
                                point.z = double.Parse(SArray[i]);
                                if (point.z < -50)
                                {
                                    point.z = -32.768;
                                }
                                _points_z.Add(point.z);
                                _data.Add(point);
                            }
                        }
                    }

                    LineNum++;

                }


                if ((TestCSV != "WidthCali") && (TestCSV != "Info"))
                {
                    ImageClass.Info3D.ImageHeight = LineNum;
                }
                _Image = Transform_Image.HeightToRainbow(_points_z, ImageClass.Info3D.ImageWidth, ImageClass.Info3D.ImageHeight, 0, -10.0, false);     //自动上下限

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace, "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                SR.Close();
            }
        }

        public void Load_LinedataFromCSV(string filename)
        {
            try
            {
                //读取点云
                List<int> Point_Z = new List<int>();
                int ZZ = 0;
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);

                StreamReader SR = new StreamReader(fs, System.Text.Encoding.GetEncoding(936));
                string nextLine = "";
                string TestCSV = "";
                int LineNum = 0;
                //获取图像参数
                while ((nextLine = SR.ReadLine()) != null)
                {
                    string[] SArray = nextLine.Split(',');

                    if (LineNum == 0)
                    {
                        TestCSV = SArray[0];
                    }
                    if (TestCSV == "WidthCali")
                    {
                        if (LineNum == 1)
                        {

                            ImageClass.Info3D.xResolution = double.Parse(SArray[0]);
                            ImageClass.Info3D.yResolution = double.Parse(SArray[1]);
                            ImageClass.Info3D.zResolution = double.Parse(SArray[2]);
                            ImageClass.Info3D.ImageWidth = int.Parse(SArray[3]);
                            ImageClass.Info3D.ImageHeight = int.Parse(SArray[4]);
                            ImageClass.Info3D.xOffset = double.Parse(SArray[5]);
                            ImageClass.Info3D.yOffset = double.Parse(SArray[6]);
                            ImageClass.Info3D.zOffset = double.Parse(SArray[7]);
                        }
                        else if (LineNum > 0)
                        {

                            for (int i = 0; i < ImageClass.Info3D.ImageWidth; i++)     //int i = 0; i < SArray.Count() - 1; i++
                            {

                                ZZ = int.Parse(SArray[i]);

                                Point_Z.Add(ZZ);
                            }
                        }
                    }
                    else
                    {
                        if (LineNum == 0)
                        {
                            ImageClass.Info3D.ImageWidth = SArray.Count() - 1;
                        }
                        else if (LineNum >= 0)
                        {
                            for (int i = 0; i < ImageClass.Info3D.ImageWidth; i++)     //int i = 0; i < SArray.Count() - 1; i++
                            {

                                ZZ = int.Parse(SArray[i]);
                                if (ZZ < -50)
                                {
                                    ZZ = 0;
                                }
                                Point_Z.Add(ZZ);
                            }
                        }
                    }

                    LineNum++;
                    ImageClass.Info3D.ImageHeight = LineNum;
                }
                SR.Close();
                _Image = Transform_Image.LightToGray(Point_Z, ImageClass.Info3D.ImageWidth, ImageClass.Info3D.ImageHeight);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace, "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public void Save_Height(List<double> SaveData, int cutwidth, int cutheight, int pictureNum, int Num, string folderpath)
        {
            string filepath;
            try
            {

                filepath = folderpath + "/" + pictureNum.ToString() + "_" + Num.ToString() + "_H.csv";


                File.Create(filepath).Close();
                StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8);


                for (int iy = 0; iy < cutheight; iy++)
                {
                    for (int ix = 0; ix < cutwidth; ix++)
                    {
                        sw.Write(SaveData[iy * cutwidth + ix].ToString() + ",");
                    }
                    sw.Write("\r\n");
                }
                sw.Close();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        public void Save_Light(List<int> SaveData, int cutwidth, int cutheight, int pictureNum, int Num, string folderpath)
        {
            string filepath;
            try
            {

                filepath = folderpath + "/" + pictureNum.ToString() + "_" + Num.ToString() + "_L.csv";


                File.Create(filepath).Close();
                StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8);
                for (int iy = 0; iy < cutheight; iy++)
                {
                    for (int ix = 0; ix < cutwidth; ix++)
                    {
                        sw.Write(SaveData[iy * cutwidth + ix].ToString() + ",");
                    }
                    sw.Write("\r\n");
                }
                sw.Close();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        /// <summary>
        /// 保存裁切后点云数据到指定路径的CSV文件
        /// <para>说明：点云数据，裁剪宽度，裁剪高度，裁剪区域序号，保存路径（文件夹）</para> 
        /// </summary>
        /// <param name="SaveData"></param>
        /// <param name="cutwidth"></param>
        /// <param name="cutheight"></param>
        /// <param name="Num"></param>
        /// <param name="folderpath"></param>
        public void Save_3Ddata(List<CameraObj.SurfacePoint> SaveData, double WidthCali, double HeightCali, double DeepCali,
                               double XOffset, double YOffset, double ZOffset, int cutwidth, int cutheight, int pictureNum,
                               int Num, string folderpath)
        {
            try
            {
                lock (lock_Save)
                {
                    string filepath = folderpath + "/" + pictureNum.ToString() + "_" + Num.ToString() + ".csv";
                    File.Create(filepath).Close();
                    StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8);

                    sw.Write("WidthCali,HeightCali,DeepCali,Width,Height,XOffer,YOffer,ZOffer\r\n");
                    sw.Write(WidthCali.ToString() + "," + HeightCali.ToString() + "," + DeepCali.ToString() + "," + cutwidth.ToString()
                        + "," + cutheight.ToString() + "," + XOffset.ToString() + "," + YOffset.ToString() + "," + ZOffset.ToString() + "\r\n");

                    for (int iy = 0; iy < cutheight; iy++)
                    {
                        for (int ix = 0; ix < cutwidth; ix++)
                        {
                            sw.Write(SaveData[iy * cutwidth + ix].z.ToString() + ",");
                        }
                        sw.Write("\r\n");
                    }
                    sw.Close();
                }

            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
            }


        }

        private static readonly object lock_Save = new object();

        /// <summary>
        /// 保存翻边焊缝检测点云数据到指定路径的CSV文件
        /// <para>说明：点云数据，裁剪宽度，裁剪高度，裁剪区域序号，保存路径（文件夹）</para> 
        /// </summary>
        /// <param name="SaveData"></param>
        /// <param name="cutwidth"></param>
        /// <param name="cutheight"></param>
        /// <param name="Num"></param>
        /// <param name="folderpath"></param>
        public void Save_3Ddata_FB(List<CameraObj.SurfacePoint> SaveData, double WidthCali, double HeightCali, double DeepCali,
                               double XOffset, double YOffset, double ZOffset, int cutwidth, int cutheight, string BarCode,
                               int Num, int All_Num, string folderpath)
        {
            try
            {
                lock (lock_Save)
                {
                    //对二维码进行校验，防止数据保存错误
                    if (BarCode.Length < 24)
                    {
                        BarCode = ("ERROR").PadRight(24, '0');
                    }

                    string filepath = folderpath + "/" + BarCode.Substring(0, 24) + "_" + Num.ToString() + "_" + All_Num.ToString() + ".csv";
                    File.Create(filepath).Close();
                    StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8);

                    sw.Write("WidthCali,HeightCali,DeepCali,Width,Height,XOffer,YOffer,ZOffer\r\n");
                    sw.Write(WidthCali.ToString() + "," + HeightCali.ToString() + "," + DeepCali.ToString() + "," + cutwidth.ToString()
                        + "," + cutheight.ToString() + "," + XOffset.ToString() + "," + YOffset.ToString() + "," + ZOffset.ToString() + "\r\n");

                    for (int iy = 0; iy < cutheight; iy++)
                    {
                        for (int ix = 0; ix < cutwidth; ix++)
                        {
                            sw.Write(SaveData[iy * cutwidth + ix].z.ToString() + ",");
                        }
                        sw.Write("\r\n");
                    }
                    sw.Close();
                }

            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
            }


        }

        /// <summary>
        /// 保存裁切后点云数据到指定路径的CSV文件
        /// <para>说明：点云数据，裁剪宽度，裁剪高度，裁剪区域序号，保存路径（文件夹）</para> 
        /// </summary>
        /// <param name="SaveData"></param>
        /// <param name="cutwidth"></param>
        /// <param name="cutheight"></param>
        /// <param name="Num"></param>
        /// <param name="folderpath"></param>
        public void Save_3Ddata_Select(List<CameraObj.SurfacePoint> SaveData, double WidthCali, double HeightCali, double DeepCali,
                               double XOffset, double YOffset, double ZOffset, int cutwidth, int cutheight, int pictureNum,
                               int Num, string folderpath)
        {
            try
            {
                lock (lock_Save)
                {
                    string filepath = folderpath + "/" + pictureNum.ToString() + "_" + Num.ToString() + ".csv";
                    if (!File.Exists(filepath))
                    {
                        File.Create(filepath).Close();
                    }

                    StreamWriter sw1 = new StreamWriter(filepath, true, Encoding.UTF8);
                    for (int iy = 0; iy < cutheight; iy++)
                    {
                        for (int ix = 0; ix < cutwidth; ix++)
                        {
                            sw1.Write(SaveData[iy * cutwidth + ix].z.ToString() + ",");
                        }
                        sw1.Write("\r\n");
                    }
                    sw1.Close();
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        public void Save_3Ddata_double(List<double> SaveData, string filename, string folderpath)
        {
            try
            {
                lock (lock_Save)
                {
                    string filepath = folderpath + "\\" + filename + ".csv";
                    if (!File.Exists(filepath))
                    {
                        File.Create(filepath).Close();
                    }

                    StreamWriter sw1 = new StreamWriter(filepath, true, Encoding.UTF8);
                    for (int iy = 0; iy < SaveData.Count - 1; iy++)
                    {
                        sw1.Write((SaveData[iy]).ToString());
                        sw1.Write(",");
                    }
                    sw1.Write((SaveData[SaveData.Count - 1]).ToString());
                    sw1.Write("\r\n");
                    sw1.Close();
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        public void Save_3Ddata_double(List<double> SaveData, int pictureNum, int Num, string folderpath)
        {
            try
            {
                lock (lock_Save)
                {
                    string filepath = folderpath + "/" + pictureNum.ToString() + "_" + Num.ToString() + ".csv";
                    if (!File.Exists(filepath))
                    {
                        File.Create(filepath).Close();
                    }

                    StreamWriter sw1 = new StreamWriter(filepath, true, Encoding.UTF8);
                    for (int iy = 0; iy < SaveData.Count - 1; iy++)
                    {
                        sw1.Write((SaveData[iy]).ToString());
                        sw1.Write(",");
                    }
                    sw1.Write((SaveData[SaveData.Count - 1]).ToString());
                    sw1.Write("\r\n");
                    sw1.Close();
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        public void Save_3Ddata_double(List<List<double>> SaveData, int pictureNum, int Num, string folderpath)
        {
            try
            {
                lock (lock_Save)
                {
                    string filepath = folderpath + "/" + pictureNum.ToString() + "_" + Num.ToString() + ".csv";
                    if (!File.Exists(filepath))
                    {
                        File.Create(filepath).Close();
                    }

                    StreamWriter sw1 = new StreamWriter(filepath, true, Encoding.UTF8);
                    for (int iy = 0; iy < SaveData.Count; iy++)
                    {
                        for (int ix = 0; ix < SaveData[iy].Count - 1; ix++)
                        {
                            sw1.Write((SaveData[iy][ix]).ToString());
                            sw1.Write(",");
                        }
                        sw1.Write((SaveData[iy][SaveData[iy].Count - 1]).ToString());
                        sw1.Write("\r\n");
                    }

                    sw1.Close();
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        public void Save_Image(HObject Image, int pictureNum, int Num, string folderpath)
        {
            try
            {
                string filepath = folderpath + "/" + pictureNum.ToString() + "_" + Num.ToString() + ".jpeg";
                SaveAndLoad_Data.DetectIsExist(filepath);
                HOperatorSet.WriteImage(Image, "jpeg", 0, filepath);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }

        }

        public void Save_Image_FB(HObject Image, string Barcode, int Num, int All_Num, string folderpath)
        {
            try
            {
                //对二维码进行校验，防止数据保存错误
                if (Barcode.Length < 24)
                {
                    Barcode = ("ERROR").PadRight(24, '0');
                }
                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
                string filepath = folderpath + "/" + time + "_" + Barcode.Substring(0, 24) + "_" + Num.ToString() + "_" + All_Num.ToString() + ".jpeg";
                SaveAndLoad_Data.DetectIsExist(filepath);
                HOperatorSet.WriteImage(Image, "jpeg", 0, filepath);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }

        }

        public void Save_Image_FB(HObject Image, string Barcode, string folderpath)
        {
            try
            {
                //对二维码进行校验，防止数据保存错误
                if (Barcode.Length < 24)
                {
                    Barcode = ("ERROR").PadRight(24, '0');
                }
                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
                string filepath = folderpath + "/" + time + " " + Barcode.Substring(0, 24) + ".jpeg";
                SaveAndLoad_Data.DetectIsExist(filepath);
                HOperatorSet.WriteImage(Image, "jpeg", 0, filepath);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        #region Tiff操作
        /// <summary>
        /// 将3D数据写入TIFF
        /// </summary>
        /// <param name="Data3D"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        /// <param name="filepath"></param>
        public void WriteTiff(List<double> Data3D, int ImageWidth, int ImageHeight, string filepath, string imageinfo)
        {
            try
            {
                SaveAndLoad_Data.DetectIsExist(filepath);
                ushort[] intdata = new ushort[ImageWidth * ImageHeight];
                for (int i = 0; i < Data3D.Count; i++)
                {
                    intdata[i] = (ushort)(Data3D[i] * 1000 + 32768);
                }
                int intSize = sizeof(ushort) * intdata.Length;
                byte[] bytArr = new byte[intSize];
                //申请一块非托管内存
                IntPtr ptr = Marshal.AllocHGlobal(intSize);
                //复制int数组到该内存块
                ptr = GetPtrFromUShortArray(intdata);
                //复制回byte数组
                Marshal.Copy(ptr, bytArr, 0, bytArr.Length);
                //释放申请的非托管内存
                Marshal.FreeHGlobal(ptr);      //是否需要添加？测试内存占用大的原因  20201125 situ

                Tiff tif = Tiff.Open(filepath, "w");

                int bits = 16;
                tif.SetField(TiffTag.IMAGELENGTH, ImageHeight);
                tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                tif.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);//设置从左上角开始
                tif.SetField(TiffTag.IMAGEWIDTH, ImageWidth);
                tif.SetField(TiffTag.COMPRESSION, Compression.NONE);
                tif.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                tif.SetField(TiffTag.BITSPERSAMPLE, bits);//设置图片数据位数
                tif.SetField(TiffTag.SAMPLESPERPIXEL, 1);//设置图片通道数
                tif.SetField(TiffTag.IMAGEDESCRIPTION, imageinfo);//图像描述
                tif.WriteRawStrip(0, bytArr, ImageWidth * ImageHeight * (bits / 8));
                tif.WriteDirectory();
                tif.Close();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
            }


        }

        public void WriteTiff(List<CameraObj.SurfacePoint> Data3D, int ImageWidth, int ImageHeight, int pictureNum, int Num, string folderpath, string imageinfo)
        {
            string filepath = folderpath + "/" + pictureNum.ToString() + "_" + Num.ToString() + ".tif";
            SaveAndLoad_Data.DetectIsExist(filepath);
            ushort[] intdata = new ushort[ImageWidth * ImageHeight];
            for (int i = 0; i < Data3D.Count; i++)
            {
                intdata[i] = (ushort)(Data3D[i].z * 1000 + 32768);
            }
            int intSize = sizeof(ushort) * intdata.Length;
            byte[] bytArr = new byte[intSize];
            //申请一块非托管内存
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            //复制int数组到该内存块
            ptr = GetPtrFromUShortArray(intdata);
            //复制回byte数组
            Marshal.Copy(ptr, bytArr, 0, bytArr.Length);
            //释放申请的非托管内存
            Marshal.FreeHGlobal(ptr);      //是否需要添加？测试内存占用大的原因  20201125 situ

            Tiff tif = Tiff.Open(filepath, "w");
            int bits = 16;
            tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
            tif.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);//设置从左上角开始
            tif.SetField(TiffTag.IMAGEWIDTH, ImageWidth);
            tif.SetField(TiffTag.IMAGELENGTH, ImageHeight);
            tif.SetField(TiffTag.COMPRESSION, Compression.NONE);
            tif.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
            tif.SetField(TiffTag.BITSPERSAMPLE, bits);//设置图片数据位数
            tif.SetField(TiffTag.SAMPLESPERPIXEL, 1);//设置图片通道数
            tif.SetField(TiffTag.IMAGEDESCRIPTION, imageinfo);//图像描述
            tif.WriteRawStrip(0, bytArr, ImageWidth * ImageHeight * (bits / 8));
            tif.WriteDirectory();
            tif.Close();

        }

        public void WriteTiff_FB(List<CameraObj.SurfacePoint> Data3D, int ImageWidth, int ImageHeight, string Barcode, int Num, int All_Num, string folderpath, string imageinfo)
        {
            //对二维码进行校验，防止数据保存错误
            if (Barcode.Length < 24)
            {
                Barcode = ("ERROR").PadRight(24, '0');
            }

            string filepath = folderpath + "/" + Barcode.Substring(0, 24) + "_" + Num.ToString() + "_" + All_Num.ToString() + ".tif";
            string filepath1 = folderpath + "/" + DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss.fff") + "_" + Num.ToString() + "_" + All_Num.ToString() + ".tif";
            SaveAndLoad_Data.DetectIsExist(filepath1);
            ushort[] intdata = new ushort[ImageWidth * ImageHeight];
            for (int i = 0; i < Data3D.Count; i++)
            {
                intdata[i] = (ushort)(Data3D[i].z * 1000 + 32768);
            }
            int intSize = sizeof(ushort) * intdata.Length;
            byte[] bytArr = new byte[intSize];
            //申请一块非托管内存
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            //复制int数组到该内存块
            ptr = GetPtrFromUShortArray(intdata);
            //复制回byte数组
            Marshal.Copy(ptr, bytArr, 0, bytArr.Length);
            //释放申请的非托管内存
            Marshal.FreeHGlobal(ptr);      //是否需要添加？测试内存占用大的原因  20201125 situ

            Tiff tif = Tiff.Open(filepath1, "w");
            int bits = 16;
            tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
            tif.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);//设置从左上角开始
            tif.SetField(TiffTag.IMAGEWIDTH, ImageWidth);
            tif.SetField(TiffTag.IMAGELENGTH, ImageHeight);
            tif.SetField(TiffTag.COMPRESSION, Compression.NONE);
            tif.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
            tif.SetField(TiffTag.BITSPERSAMPLE, bits);//设置图片数据位数
            tif.SetField(TiffTag.SAMPLESPERPIXEL, 1);//设置图片通道数
            tif.SetField(TiffTag.IMAGEDESCRIPTION, imageinfo);//图像描述
            tif.WriteRawStrip(0, bytArr, ImageWidth * ImageHeight * (bits / 8));
            tif.WriteDirectory();
            tif.Close();

        }

        /// <summary>
        /// 保存亮度图
        /// </summary>
        /// <param name="Data3D"></param>
        /// <param name="ImageWidth"></param>
        /// <param name="ImageHeight"></param>
        /// <param name="filepath"></param>
        /// <param name="imageinfo"></param>
        public void WriteTiff(List<int> Data3D, int ImageWidth, int ImageHeight, string filepath, string imageinfo)
        {
            SaveAndLoad_Data.DetectIsExist(filepath);
            short[] intdata = new short[ImageWidth * ImageHeight];
            for (int i = 0; i < Data3D.Count; i++)
            {
                intdata[i] = (short)Data3D[i];
            }
            int intSize = sizeof(short) * intdata.Length;
            byte[] bytArr = new byte[intSize];
            //申请一块非托管内存
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            //复制int数组到该内存块
            Marshal.Copy(intdata, 0, ptr, intdata.Length);
            //复制回byte数组
            Marshal.Copy(ptr, bytArr, 0, bytArr.Length);
            //释放申请的非托管内存
            Marshal.FreeHGlobal(ptr);      //是否需要添加？测试内存占用大的原因  20201125 situ

            Tiff tif = Tiff.Open(filepath, "w");

            int bits = 16;
            tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
            tif.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);//设置从左上角开始
            tif.SetField(TiffTag.IMAGEWIDTH, ImageWidth);
            tif.SetField(TiffTag.IMAGELENGTH, ImageHeight);
            tif.SetField(TiffTag.COMPRESSION, Compression.NONE);
            tif.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
            tif.SetField(TiffTag.BITSPERSAMPLE, bits);//设置图片数据位数
            tif.SetField(TiffTag.SAMPLESPERPIXEL, 1);//设置图片通道数
            tif.SetField(TiffTag.IMAGEDESCRIPTION, imageinfo);//图像描述
            tif.WriteRawStrip(0, bytArr, ImageWidth * ImageHeight * (bits / 8));
            tif.WriteDirectory();
            tif.Close();
        }

        /// <summary>
        /// 读取TIFF
        /// </summary>
        /// <param name="filename"></param>
        public void ReadTiff(string filename)
        {
            _points_z.Clear();
            _points_z.TrimExcess();
            _data.Clear();
            _data.TrimExcess();
            CameraObj.SurfacePoint point = new CameraObj.SurfacePoint();
            Tiff tif = Tiff.Open(filename, "r");
            if (tif == null)
                return;
            FieldValue[] value = tif.GetField(TiffTag.IMAGEWIDTH);// 获取图像宽度
            ImageClass.Info3D.ImageWidth = value[0].ToInt();
            value = tif.GetField(TiffTag.IMAGELENGTH);//获取图像高度
            ImageClass.Info3D.ImageHeight = value[0].ToInt();
            value = tif.GetField(TiffTag.BITSPERSAMPLE);//获取数据位数
            int bits = value[0].ToInt();

            value = tif.GetField(TiffTag.IMAGEDESCRIPTION);//获取图像描述
            if (value != null)
            {
                string[] imageinfo = value[0].ToString().Split(',');
                for (int i = 0; i < imageinfo.Length; i++)
                {
                    string[] info = imageinfo[i].Split(':');
                    if (info[0] == "WidthCali")
                        double.TryParse(info[1], out ImageClass.Info3D.xResolution);
                    else if (info[0] == "HeightCali")
                        double.TryParse(info[1], out ImageClass.Info3D.yResolution);
                    else if (info[0] == "DeepCali")
                        double.TryParse(info[1], out ImageClass.Info3D.zResolution);
                    else if (info[0] == "XOffset")
                        double.TryParse(info[1], out ImageClass.Info3D.xOffset);
                    else if (info[0] == "YOffset")
                        double.TryParse(info[1], out ImageClass.Info3D.yOffset);
                    else if (info[0] == "ZOffset")
                        double.TryParse(info[1], out ImageClass.Info3D.zOffset);
                }
            }
            else
            {
                ImageClass.Info3D.xResolution = 0.015;
                ImageClass.Info3D.yResolution = 0.07;
                ImageClass.Info3D.zResolution = 0.0002;
                ImageClass.Info3D.xOffset = 4.995;
                ImageClass.Info3D.yOffset = -159.95;
                ImageClass.Info3D.zOffset = -1.646;
            }


            short dirNumb = tif.NumberOfDirectories();//获取页数

            //tif.SetDirectory(i);
            int lineSize = tif.ScanlineSize();
            byte[] rowData = new byte[lineSize];                //存储每行数据
            byte[] data = new byte[ImageClass.Info3D.ImageHeight * ImageClass.Info3D.ImageHeight * (bits / 8)];//存储整张图像数据
            for (int i = 0; i < ImageClass.Info3D.ImageHeight; i++)
            {
                tif.ReadScanline(rowData, i);
                rowData.CopyTo(data, i * lineSize);
            }
            tif.Close();

            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);

            if (bits == 32)
            {
                int intSize = data.Length / sizeof(int);
                int[] intArr = new int[intSize];
                Marshal.Copy(ptr, intArr, 0, intArr.Length);
                Marshal.FreeHGlobal(ptr);
                for (int i = 0; i < intArr.Length; i++)
                {
                    _points_z.Add(intArr[i] / 10000.0);
                }
            }
            else if (bits == 16)
            {//unshort
                int intSize = data.Length / sizeof(short);
                ushort[] intArr = new ushort[intSize];
                //Marshal.Copy(ptr, intArr, 0, intArr.Length);
                intArr = GetUShortArrayFromPtr(ptr, intArr.Length);
                Marshal.FreeHGlobal(ptr);
                for (int i = 0; i < intArr.Length; i++)
                {
                    _points_z.Add(((double)intArr[i] - 32768) / 1000.0);
                }

            }

            for (int i_y = 0; i_y < ImageClass.Info3D.ImageHeight; i_y++)
            {
                for (int i_x = 0; i_x < ImageClass.Info3D.ImageWidth; i_x++)
                {
                    point.x = i_x * ImageClass.Info3D.xResolution + ImageClass.Info3D.xOffset;
                    point.y = i_y * ImageClass.Info3D.yResolution + ImageClass.Info3D.yOffset;
                    point.z = _points_z[i_y * ImageClass.Info3D.ImageWidth + i_x];
                    _data.Add(point);
                }
            }
            _Image = Transform_Image.HeightToRainbow(_points_z, ImageClass.Info3D.ImageWidth, ImageClass.Info3D.ImageHeight, 0, 0, false);

            return;
        }

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

        /// <summary>
        /// 将Tiff图像信息转为点云类型
        /// </summary>
        /// <param name="ImageClass"></param>
        public void TiffToDatas(Image3DClass imageclass)
        {
            try
            {
                _points_z.Clear();
                _points_z.TrimExcess();
                _data.Clear();
                _data.TrimExcess();
                CameraObj.SurfacePoint point = new CameraObj.SurfacePoint();

                ImageClass = imageclass;

                HTuple hv_Pointer = new HTuple(), hv_Type = new HTuple(), hv_Width = new HTuple(), hv_Height = new HTuple();
                HOperatorSet.GetImagePointer1(ImageClass.Image3D, out hv_Pointer, out hv_Type, out hv_Width, out hv_Height);

                ImageClass.Info3D.ImageHeight = (int)hv_Height[0].D;
                ImageClass.Info3D.ImageWidth = (int)hv_Width[0].D;
                byte[] data = new byte[ImageClass.Info3D.ImageHeight * ImageClass.Info3D.ImageWidth * (16 / 8)];//存储整张图像数据(16位int)
                IntPtr ptr = hv_Pointer[0].IP;

                //unshort
                int intSize = data.Length / sizeof(ushort);
                ushort[] intArr = new ushort[intSize];
                intArr = GetUShortArrayFromPtr(ptr, intArr.Length);

                for (int i_y = 0; i_y < ImageClass.Info3D.ImageHeight; i_y++)
                {
                    for (int i_x = 0; i_x < ImageClass.Info3D.ImageWidth; i_x++)
                    {
                        point.x = i_x * ImageClass.Info3D.xResolution + ImageClass.Info3D.xOffset;
                        point.y = i_y * ImageClass.Info3D.yResolution + ImageClass.Info3D.yOffset;
                        point.z = ((double)intArr[i_y * ImageClass.Info3D.ImageWidth + i_x] - 32768) / 1000.0;

                        _points_z.Add(point.z);
                        _data.Add(point);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }

        }

        #endregion

        #region dat操作
        public void WriteImageData(Image3DClass ImageClass, string Barcode, int Num, int All_Num, string folderpath)
        {
            //对二维码进行校验，防止数据保存错误
            if (Barcode.Length < 24)
            {
                Barcode = ("ERROR").PadRight(24, '0');
            }
            string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            string filepath = folderpath + "/" + time + "_" + Barcode.Substring(0, 24) + "_" + Num.ToString() + "_" + All_Num.ToString() + ".dat";
            SaveAndLoad_Data.Save(ImageClass, filepath);
        }

        public void WriteImageData(Image3DClass ImageClass, string Barcode, string folderpath)
        {
            //对二维码进行校验，防止数据保存错误
            if (Barcode.Length < 24)
            {
                Barcode = ("ERROR").PadRight(24, '0');
            }
            string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            string filepath = folderpath + "/" + time + " " + Barcode.Substring(0, 24) + "_" + ".dat";
            SaveAndLoad_Data.Save(ImageClass, filepath);
        }

        public void WriteImageData_Test(Image3DClass ImageClass, string Barcode, int Num, int All_Num, string folderpath)
        {
            //对二维码进行校验，防止数据保存错误
            string filepath = folderpath + "/" + Barcode + "_" + Num.ToString() + "_" + All_Num.ToString() + ".dat";
            SaveAndLoad_Data.Save(ImageClass, filepath);
        }
        #endregion



        public static void LoadRStandradLine(string path, out List<List<double>> RStandradLine)
        {
            string nextLine = "";
            RStandradLine = new List<List<double>>();
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader SR = new StreamReader(fs, System.Text.Encoding.GetEncoding(936));
                while ((nextLine = SR.ReadLine()) != null)
                {
                    string[] SArray = nextLine.Split(',');
                    List<double> points = new List<double>();
                    for (int i = 0; i < SArray.Length; i++)
                    {
                        double point = -20;
                        double.TryParse(SArray[i], out point);
                        points.Add(point);
                    }
                    RStandradLine.Add(points);
                }
                fs.Close();
                SR.Close();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }


        public static void GetPointRGB(Image3DClass image3d, ref List<CameraObj.SurfacePointRGB> PointRGB)
        {
            HTuple hv_Rows = new HTuple(), hv_Columns = new HTuple(), hv_GrayvalRGBs = new HTuple(), hv_GrayvalZs = new HTuple();

            HOperatorSet.GetRegionPoints(image3d.Image2D, out hv_Rows, out hv_Columns);
            HOperatorSet.GetGrayval(image3d.Image2D, hv_Rows, hv_Columns, out hv_GrayvalRGBs);
            HOperatorSet.GetGrayval(image3d.Image3D, hv_Rows, hv_Columns, out hv_GrayvalZs);
            long[] xs = hv_Columns.LArr;
            long[] ys = hv_Rows.LArr;
            long[] zs = hv_GrayvalZs.LArr;
            long[] rgbs = hv_GrayvalRGBs.LArr;
            CameraObj.SurfacePointRGB[] pointrgb = new CameraObj.SurfacePointRGB[xs.Length];
            for (int i = 0; i < xs.Length; i++)
            {
                pointrgb[i].x = xs[i] * image3d.Info3D.xResolution + image3d.Info3D.xOffset;
                pointrgb[i].y = ys[i] * image3d.Info3D.yResolution + image3d.Info3D.yOffset;
                pointrgb[i].z = (zs[i] - 32768) * 0.001;
                pointrgb[i].r = (byte)rgbs[i * 3];
                pointrgb[i].g = (byte)rgbs[i * 3 + 1];
                pointrgb[i].b = (byte)rgbs[i * 3 + 2];
            }
            PointRGB = new List<CameraObj.SurfacePointRGB>(pointrgb);
        }

        public HObject TransIntImage_Max(List<double> Points_Z, int ImageWidth, int ImageHeight)
        {
            UInt16[] intgray = new UInt16[ImageWidth * ImageHeight];
            double maxVal = -20.0;
            double minVal = 9999.0;
            double limit = -10;                                                  //除去小于该值的亮度
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

            double DeepMin = minVal - 0.09;                                                    //高度范围下限
            double DeepMax = maxVal + 0.09;                                                    //高度范围上限
            int ipoint = 0;

            double IntHeight = UInt16.MaxValue / ((DeepMax - DeepMin) / 0.0001);  //z字段每一单位对应int灰度值
            foreach (double point_z in Points_Z)
            {
                if (point_z < DeepMin || point_z > DeepMax)
                {//判断是否超出设定高度范围
                    intgray[ipoint] = 0;
                }
                else
                {
                    UInt16 sdd = (UInt16)((point_z - DeepMin) * IntHeight * 10000);      //判断在哪一层色彩
                    intgray[ipoint] = sdd;
                }
                ipoint++;
            }
            //IntPtr imgPtr = Marshal.AllocHGlobal(intgray.Length * Marshal.SizeOf(typeof(ushort)));//分配内存
            IntPtr imgPtr = GetPtrFromUShortArray(intgray);
            //Marshal.Copy(intgray, 0, imgPtr, intgray.Length);

            HObject ho_ImageGray = null;
            HOperatorSet.GenImage1(out ho_ImageGray, "uint2", ImageWidth, ImageHeight, imgPtr);
            //  HOperatorSet.WriteImage(ho_ImageGray, "tiff", 0, "F:/01.tiff");

            return ho_ImageGray;
        }

        public HObject TransIntImage_Max(List<CameraObj.SurfacePoint> Point, int ImageWidth, int ImageHeight)
        {
            UInt16[] intgray = new UInt16[ImageWidth * ImageHeight];
            double maxVal = -20.0;
            double minVal = 9999.0;
            double limit = -10;                                                  //除去小于该值的亮度
            foreach (CameraObj.SurfacePoint thisNum in Point)
            {
                if (thisNum.z < limit)
                {
                    continue;
                }
                else if (thisNum.z > maxVal)
                {
                    maxVal = thisNum.z;
                }
                if (thisNum.z < minVal)
                {
                    minVal = thisNum.z;
                }
            }

            double DeepMin = minVal - 0.09;                                                    //高度范围下限
            double DeepMax = maxVal + 0.09;                                                    //高度范围上限
            int ipoint = 0;

            double IntHeight = UInt16.MaxValue / ((DeepMax - DeepMin) / 0.0001);  //z字段每一单位对应int灰度值
            foreach (CameraObj.SurfacePoint point_z in Point)
            {
                if (point_z.z < DeepMin || point_z.z > DeepMax)
                {//判断是否超出设定高度范围
                    intgray[ipoint] = 0;
                }
                else
                {
                    UInt16 sdd = (UInt16)((point_z.z - DeepMin) * IntHeight * 10000);      //判断在哪一层色彩
                    intgray[ipoint] = sdd;
                }
                ipoint++;
            }
            IntPtr imgPtr = Marshal.AllocHGlobal(intgray.Length * Marshal.SizeOf(typeof(ushort)));//分配内存
            imgPtr = GetPtrFromUShortArray(intgray);
            //Marshal.Copy(intgray, 0, imgPtr, intgray.Length);
            Marshal.FreeHGlobal(imgPtr);      //是否需要添加？测试内存占用大的原因  20201125 situ
            HObject ho_ImageGray = null;
            HOperatorSet.GenImage1(out ho_ImageGray, "uint2", ImageWidth, ImageHeight, imgPtr);
            //  HOperatorSet.WriteImage(ho_ImageGray, "tiff", 0, "F:/01.tiff");

            return ho_ImageGray;
        }

        /// <summary>
        /// 彩虹图转亮度图
        /// </summary>
        /// <param name="RainbowLightImage"></param>
        /// <param name="RainbowImage"></param>
        /// <returns></returns>
        public HObject RainbowLightToLight(HObject RainbowLightImage, HObject RainbowImage)
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
                //System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
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

        //public void AutoCutImage_3D(Image3DClass image3d, out int CutY)
        //{
        //    CutY = 0;
        //    HTuple Time1 = new HTuple();
        //    HTuple Time2 = new HTuple();
        //    HTuple Time3 = new HTuple();
        //    HTuple Time4 = new HTuple();
        //    HTuple Time5 = new HTuple();
        //    if (image3d == null)
        //        return;
        //    //_datas.Clear();
        //    HOperatorSet.CountSeconds(out Time1);      //////////////////////////////////////////////////////////////////////////////
        //    ImageClass = new Image3DClass();
        //    HObject ho_Region = null, ho_RegionTrans = null, ImageReduced = null, TiffImageReduced = null;
        //    HTuple hv_YStart = new HTuple();
        //    HTuple hv_ImageHeight = new HTuple(), hv_ImageWidth = new HTuple();
        //    HOperatorSet.GenEmptyObj(out ho_Region);
        //    HOperatorSet.GenEmptyObj(out ho_RegionTrans);
        //    HOperatorSet.GenEmptyObj(out ImageReduced);
        //    HOperatorSet.GenEmptyObj(out TiffImageReduced);
        //    try
        //    {
        //        ho_Region.Dispose();
        //        HOperatorSet.Threshold(image3d.Image2D, out ho_Region, 1, 255);
        //        HOperatorSet.ClosingRectangle1(ho_Region, out ho_Region, 1, 100);
        //        HOperatorSet.Connection(ho_Region, out ho_Region);
        //        HOperatorSet.CountSeconds(out Time2);   ///////////////////////////////////////////////////////////////////////////
        //        HOperatorSet.SelectShapeStd(ho_Region, out ho_Region, "max_area", 70);
        //        ho_RegionTrans.Dispose();
        //        HOperatorSet.ShapeTrans(ho_Region, out ho_RegionTrans, "rectangle1");
        //        HOperatorSet.RegionFeatures(ho_RegionTrans, "row1", out hv_YStart);
        //        ImageReduced.Dispose();
        //        HOperatorSet.CountSeconds(out Time3);   ///////////////////////////////////////////////////////////////////////////
        //        HOperatorSet.ReduceDomain(image3d.Image2D, ho_RegionTrans, out ImageReduced);
        //        HOperatorSet.CropDomain(ImageReduced, out ImageClass.Image2D);
        //        TiffImageReduced.Dispose();
        //        HOperatorSet.ReduceDomain(image3d.Image3D, ho_RegionTrans, out TiffImageReduced);

        //        HOperatorSet.CropDomain(TiffImageReduced, out ImageClass.Image3D);
        //        HOperatorSet.GetImageSize(ImageClass.Image2D, out hv_ImageWidth, out hv_ImageHeight);
        //        HOperatorSet.CountSeconds(out Time4);   ///////////////////////////////////////////////////////////////////////////
        //        int width = hv_ImageWidth[0].I;
        //        int height = hv_ImageHeight[0].I;
        //        int ystart = (int)hv_YStart[0].D;
        //        CutY = ystart;
        //        CameraObj.SurfacePoint[] cutpoints = new CameraObj.SurfacePoint[width * height];
        //        for (int iy = 0; iy < height; iy++)
        //        {
        //            int startpoint = (ystart + iy) * width;//每一行起始点
        //            for (int ix = 0; ix < width; ix++)
        //            {
        //                cutpoints[iy * width + ix] = _data[startpoint + ix];
        //                cutpoints[iy * width + ix].y = cutpoints[iy * width + ix].y - ystart * image3d.Info3D.yResolution;
        //            }
        //        }
        //        _Cutdata = new List<CameraObj.SurfacePoint>(cutpoints);
        //        //_datas.Add(_Cutdata);

        //        //ImageClass = new Image3DClass(TiffImageReduced, ImageReduced, image3d.Info3D);
        //        ImageClass.Info3D = image3d.Info3D;
        //        ImageClass.Info3D.ImageHeight = (int)hv_ImageHeight[0].D;
        //        ImageClass.Info3D.ImageWidth = (int)hv_ImageWidth[0].D;
        //        string type = _camid == 0 ? "true" : "false";//相机编号为0时从上往下，相机编号为1时从下往上排序
        //        SplitDefectRegion(type);
        //        HOperatorSet.CountSeconds(out Time5);   ///////////////////////////////////////////////////////////////////////////

        //        double t1 = Time1;
        //        double t2 = Time2;
        //        double t3 = Time3;
        //        double t4 = Time4;
        //        double t5 = Time5;
        //        //  double t6 = Time6;
        //        //  double t7 = Time7;

        //        //Log4netRecord.RunningLog.Info("自动裁剪耗时统计：\r\n" +
        //        //                        "阈值分割：" + (Convert.ToInt32((t2 - t1) * 1000)).ToString() + " ms\r\n" +
        //        //                      "选择区域：" + (Convert.ToInt32((t3 - t2) * 1000)).ToString() + " ms\r\n" +
        //        //                        "裁剪：" + (Convert.ToInt32((t4 - t3) * 1000)).ToString() + " ms\r\n" +
        //        //                          "点云数据获取：" + (Convert.ToInt32((t5 - t4) * 1000)).ToString() + " ms\r\n");
        //        //  "轮廓线检测：" + (Convert.ToInt32((t6 - t5) * 1000)).ToString() + " ms\r\n"+
        //        //"显示轮廓差异：" + (Convert.ToInt32((t7 - t6) * 1000)).ToString() + " ms\r\n");

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Fatal(ex);
        //    }
        //    finally
        //    {
        //        if (ho_Region != null)
        //            ho_Region.Dispose();
        //        if (ho_RegionTrans != null)
        //            ho_RegionTrans.Dispose();
        //        if (ImageReduced != null)
        //            ImageReduced.Dispose();
        //        if (TiffImageReduced != null)
        //            TiffImageReduced.Dispose();
        //    }
        //}

    }
}
