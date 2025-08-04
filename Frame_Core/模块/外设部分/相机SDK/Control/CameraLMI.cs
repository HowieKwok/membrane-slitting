using HalconDotNet;

using IKapBoardClassLibrary;

using Lmi3d.GoSdk;
using Lmi3d.GoSdk.AcceleratorMgr;
using Lmi3d.GoSdk.Messages;
using Lmi3d.GoSdk.Tools;
using Lmi3d.Zen;
using Lmi3d.Zen.Io;

using LogTool;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class CameraLMI : CameraObj
    {
        #region field
        public override bool ConnStatus
        {
            get
            {
                if (sensor != null)
                    connStatus = sensor.IsConnected();
                else
                    connStatus = false;
                return connStatus;
            }
        }
        #endregion

        #region menber
        SensorInformation sensorInformation = new SensorInformation();
        SensorOperation sensorOperation = new SensorOperation();
        SensorToolsInformation sensorToolsInformation = new SensorToolsInformation();

        GoSystem system;
        GoSensor sensor;
        KIpAddress ipAddress;
        uint sensor_id;
        GoDataSet dataSet;
        DataContext context = new DataContext();
        ProfilePoint[] profileBuffer;
        double Range_Z;
        double[] profile_z;
        int profilePointCount;
        bool IsUsedAccelerator = true;         //是否开启加速


        String ip_address;                      //传感器ip地址
        Bitmap showProfile;
        Bitmap BImage;
        Color Point = Color.Red;
        Color Back = Color.Black;
        Color Coo = Color.Gray;

        ArrayList measurementList = new ArrayList();
        ArrayList measurementIDs = new ArrayList();

        ColorMaps colorMaps;
        HObject image = new HObject();
        HObject imagelight = new HObject();
        HObject imagergb = new HObject();
        HObject imageadd = new HObject();
        bool bo_jisuan = false;
        Image3DClass image3d = new Image3DClass();

        SurfacePoint[] point_targets ;
        float[] z_points ;

        double deep_min = 15.5;
        double deep_max = 20.0;

        bool GrabedLightImage = false;
        bool GrabedColorImage = false;
        bool GrabedRainBowImage = false;
        bool GrabedTifImage = false;
        string serial_id = "";//传感器id

        //2021-11-09,lfh
        //int m_nCount = 0;
        DateTime m_DT_Start = DateTime.Now;
        DateTime m_DT_End = DateTime.Now;
        #endregion

        #region function 
        /// <summary>
        /// 代表3D图像数据类
        /// </summary>
        QueuePackage<SurfacePoint[]> point_queue = new QueuePackage<SurfacePoint[]>();

        QueuePackage<Image3DClass> image3d_queue = new QueuePackage<Image3DClass>();

        public override QueuePackage<Image3DClass> QueueGrab3DImage
        {
            get { return image3d_queue; }
        }

        public override QueuePackage<SurfacePoint[]> QueueGrabSurface
        {
            get { return point_queue; }
        }


        //public override List<SurfacePoint> Points
        //{
        //    get { return point_targets; }
        //}

        //public override List<double> Deep_Points
        //{
        //    get { return z_points; }
        //}

        /// <summary>
        /// 设置最小深度
        /// </summary>
        public override double SetDeepMin
        {
            get { return deep_min; }
            set
            { deep_min = value; }
        }

        /// <summary>
        /// 设置最大深度
        /// </summary>
        public override double SetDeepMax
        {
            get { return deep_max; }
            set
            { deep_max = value; }
        }

        public CameraLMI(string CameraSerial_str, string PC_IP1, string Camera_IP1)
        {
            bIs3D = true;
            uint.TryParse(CameraSerial_str, out sensor_id);
            ip_address = Camera_IP1;
            this.serial_id = serial_id;
            HOperatorSet.GenEmptyObj(out image);
            HOperatorSet.GenEmptyObj(out imagelight);
            HOperatorSet.GenEmptyObj(out imagergb);
            HOperatorSet.GenEmptyObj(out imageadd);
            bo_jisuan = false;
     
        }

        /// <summary>
        /// 初始化相机构造函数
        /// </summary>
        public override bool InitCam()
        {
            try
            {
                sensorInformation = new SensorInformation();
                sensorOperation = new SensorOperation();
                sensorToolsInformation = new SensorToolsInformation();

                KApiLib.Construct();
                GoSdkLib.Construct();

                ipAddress = KIpAddress.Parse(ip_address.Trim());
                GoAcceleratorMgrSensorParam goAcceleratorMgrSensorParam = new GoAcceleratorMgrSensorParam();

                if (false)//File.Exists(".\\UsedAccelerator.xml"))      //存在该文件则默认开启加速 20210702yzc取消软件加速
                {
                    IsUsedAccelerator = true;
                }
                else
                {
                    IsUsedAccelerator = false;
                }

                if (IsUsedAccelerator)
                {
                    system = new GoSystem();
                    GoAcceleratorMgr goAcceleratorMgr = new GoAcceleratorMgr();
                    goAcceleratorMgr = new GoAcceleratorMgr();
                    goAcceleratorMgr.SetSystem(system);
                    goAcceleratorMgr.Start();

                    GoAccelSensorPortAllocPorts ports = new GoAccelSensorPortAllocPorts();

                    sensor = system.FindSensorById(sensor_id);
                    //sensor = system.FindSensorByIpAddress(ipAddress);

                    goAcceleratorMgrSensorParam.PlatformIpAddress = KIpAddress.AnyV4;
                    goAcceleratorMgrSensorParam.Ports = ports;
                    goAcceleratorMgr.Accelerate(sensor.Id, goAcceleratorMgrSensorParam);

                    for (int j = 0; j < 30; j++)
                    {
                        GoSensorAccelState kStatus = sensor.AccelerationState;
                        if (kStatus == 2)
                        {
                            Console.WriteLine("Sensor Accelerated");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Waiting for Sensor Accelerated...");
                        }
                        Thread.Sleep(1000);
                    }

                    system.Refresh();
                    sensor.Connect();
                    system.SetDataHandler(onData);
                    system.EnableData(true);
                    //SensorAcclerator.RunAccleratorMgr(SensorNum);

                }
                else
                {
                    system = new GoSystem();
                    
                    //sensor = system.FindSensorById(sensor_id);//20210618yzc改为通过传感器序列号连接
                    sensor = system.FindSensorByIpAddress(ipAddress);
                    system.Refresh();
                    sensor.Connect();
                    system.EnableData(true);
                    GoSetup setup = sensor.Setup;
                    GoSurfaceGeneration sgeneration = setup.GetSurfaceGeneration();
                    system.SetDataHandler(onData);
                }


                //system.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return false;

            }

        }

        public override bool IsCameraOpen
        {
            get
            {
                bool b = false;
                try
                {
                    b = true;
                }
                catch (Exception)
                {

                    b = false;
                }
                return b;
            }
        }

        public override bool OpenCam()
        {
            return true;
        }
        /// <summary>
        /// 获取传感器扫描模式
        /// </summary>
        /// <param name="_sensor"></param>
        /// <returns></returns>
        public string Get_SensorScanMode(GoSensor sensor)
        {
            GoMode sensorScanMode;
            sensorScanMode = sensor.ScanMode;
            return sensorScanMode.ToString();
        }
        /// <summary>
        /// 开始采集图像
        /// </summary>
        /// <returns></returns>
        public override bool StartGrabImage()
        {
            try
            {
                long l = sensor.Encoder();
                Write_Log("采图开始时编码器数值:" + l.ToString());
                //2024.5.9--添加状态判断
                if (sensor.State == GoState.Running)
                {
                    sensor.Stop();
                }
                sensor.Start();
                Thread.Sleep(50);
                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return false;
            }
        }
        /// <summary>
        /// 停止采集
        /// </summary>
        /// <returns></returns>
        public override bool StopGrabImage()
        {
            try
            {
                long l = sensor.Encoder();
                Write_Log("采图完成时编码器数值:" + l.ToString());
                sensor.Stop();
                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 设置触发模式
        /// </summary>
        /// <returns></returns>
        public override bool SetTriggerMode(int mode)//设置触发模式，mode = 0 关闭触发模式；mode = 1 设置硬触发模式
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return false;
            }
        }

        public override bool SetTriggerSource(int mode)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        public override bool SetGrabState(bool bStart)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 释放相机
        /// </summary>
        /// <returns></returns>
        public override void DestroyCam()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        /// <summary>
        /// 设置相机增益
        /// </summary>
        public override bool SetGain(float gain)
        {
            try
            {
                string Exposure = gain.ToString();
                SensorOperation sensorOperation1 = new SensorOperation();
                GoSetup setup = sensor.Setup;
                sensorOperation1.Set_Exposure(sensor, setup, Exposure);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        public override bool SetTriggerDelayTime(float gain)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 设置相机采集行高
        /// </summary>
        /// <param name="YLength"></param>
        /// <returns></returns>
        public override bool SetYLenght(float YLength, string IP_Address)
        {
            try
            {
                system.Stop();
                Thread.Sleep(10);
                sensorOperation = new SensorOperation();

                //ipAddress = KIpAddress.Parse(IP_Address.Trim());
                //sensor = system.FindSensorByIpAddress(ipAddress);
                //sensor.Connect();

                //system.Stop();
                //Thread.Sleep(50);
                GoSetup setup = sensor.Setup;
                GoSurfaceGeneration sgeneration = setup.GetSurfaceGeneration();
                string Length = YLength.ToString();
                sensorOperation.Set_YLength(sensor, setup, sgeneration, Length);
                Thread.Sleep(10);
                system.Start();
                Thread.Sleep(10);

                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
        }
        public override bool SetYLenght1(string Next_FileName)
        {
            try
            {
                //Added below codes by PDN
                #region "NEW CODES REVISED BY PDN"
                sensor.Stop();
                Thread.Sleep(10);
                sensorOperation.Set_ProfileYLength(sensor, Next_FileName);
                string test = null;
                bool changed = false;
                sensor.LoadedJob(ref test, ref changed);//获取当前传感器的job名称
                long lTickBegin = DateTime.Now.Ticks;
                long lWaitTime = 0L;
                long lMaxWaitTimeInSecond = 2;
                while (true)
                {
                    lWaitTime = DateTime.Now.Ticks - lTickBegin;
                    TimeSpan timeSpan1 = new TimeSpan(lWaitTime);
                    // LogName.ErrorLog.Error("LMI 3D 相机 S/N 为:" + serial_id + "的相机准备切换到" + test.ToString() + "作业");
                    if (timeSpan1.Seconds > lMaxWaitTimeInSecond && test != Next_FileName)
                    {
                        Log4netRecord.RunLog.Fatal("LMI 3D 相机 S/N:" + serial_id + " 切换作业:" + Next_FileName + " 超时未完成");
                        throw new Exception("LMI 3D 相机 S/N:" + serial_id + " 切换作业:" + Next_FileName + " 超时未完成");
                    }
                    if (test == Next_FileName)
                    {
                        Log4netRecord.RunLog.Fatal("LMI 3D 相机 S/N 为:" + serial_id + "的相机切换到" + test.ToString() + "作业完毕");
                        break;
                    }
                    Thread.Sleep(5);
                }
                Log4netRecord.RunLog.Fatal("LMI 3D 相机 S/N 为:" + serial_id + "的相机换到" + test.ToString() + "作业已完成");
                sensor.Start();
                Thread.Sleep(10);
                return true;

                #endregion
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
        }

        //private struct SurfacePoint
        //{
        //    public double x;
        //    public double y;
        //    public double z;

        //    //public static implicit operator string(SurfacePoint v)
        //    //{
        //    //    throw new NotImplementedException();
        //    //}
        //}

        /// <summary>
        /// 得到点云数据的深度数据、分辨率和位置等
        /// </summary>
        /// <param name="dataObj">从相机中获得的数据</param>
        /// <param name="point_targets">三维点云数据</param>
        /// <param name="z_points">深度数据</param>
        /// <param name="xResolution">X向分辨率</param>
        /// <param name="yResolution">Y向分辨率</param>
        /// <param name="zResolution">Z向分辨率</param>
        /// <param name="xOffset">原点X坐标</param>
        /// <param name="yOffset">原点Y坐标</param>
        /// <param name="zOffset">原点Z坐标</param>
        private void Get_DataParameter(GoUniformSurfaceMsg dataObj, out SurfacePoint[] point_targets, out float[] z_points, 
            out Cam_Calibration calibration,out double MaxZ, out double MinZ)
        {
           
            MaxZ= 10;
            MinZ = -10;
            GoSurfaceMsg surfaceMsg = (GoSurfaceMsg)dataObj;
            int Width = (int)surfaceMsg.Width;
            int Height = (int)surfaceMsg.Length;


            point_targets = new  SurfacePoint [Width * Height];
            z_points = new float[Width* Height];

            var temp_point_targets = new SurfacePoint[Width * Height];
            var temp_z_points = new float[Width * Height];

            SurfacePoint point_target = new SurfacePoint();
            calibration = new Cam_Calibration();
            double Point_x = 0;
            double Point_y = 0;
            double Point_z = 0;

            try
            {
                

                int Point_width = (int)surfaceMsg.Width; // max X 
                int Point_height = (int)surfaceMsg.Length;// max Y
                int Point_bufferSize = Point_width * Point_height; // max X * max Y

                calibration.ImageWidth = (int)surfaceMsg.Width;
                calibration.ImageHeight = (int)surfaceMsg.Length;
                calibration.xResolution = (double)surfaceMsg.XResolution / 1000000; //nano metre to mm
                calibration.yResolution = (double)surfaceMsg.YResolution / 1000000;
                calibration.zResolution = (double)surfaceMsg.ZResolution / 1000000;
                calibration.xOffset = (double)surfaceMsg.XOffset / 1000;//um to mm
                calibration.yOffset = (double)surfaceMsg.YOffset / 1000;
                calibration.zOffset = (double)surfaceMsg.ZOffset / 1000;

                short[] points = new short[Point_bufferSize];
                IntPtr pointsPtr = surfaceMsg.Data;

                Marshal.Copy(pointsPtr, points, 0, Point_bufferSize);
                #region 旧方法 100ms
                //for (int ii = 0; ii < Point_height; ii++)
                //{
                //    int baseIndex = ii * Point_width;

                //    for (int j = 0; j < Point_width; j++)
                //    {
                //        int idx = baseIndex + j;
                //        Point_x = calibration.xOffset + calibration.xResolution * j;
                //        Point_y = calibration.yOffset + calibration.yResolution * ii;
                //        if (points[idx] <= -32768)
                //        {
                //            Point_z = -32.768;
                //        }
                //        else
                //        {
                //            Point_z = calibration.zOffset + calibration.zResolution * points[ii * Point_width + j];
                //        }
                //        point_target.x = Point_x;
                //        point_target.y = Point_y;
                //        point_target.z = Point_z;
                //        point_targets[idx] = point_target;
                //        z_points[idx] = (float)Point_z;
                //    }
                //}
                #endregion
                #region 新方法40ms
                var tempCalibration = calibration;

                //Parallel.For(0, Point_height, ii =>
                //{
                //    int baseIndex = ii * Point_width;
                //    for (int j = 0; j < Point_width; j++)
                //    {
                //        // 复用索引计算 
                //        int idx = baseIndex + j;
                //        short pointVal = points[idx];
                //        // 直接计算避免分支预测失败 
                //        double point_z = (pointVal <= -32768)
                //            ? -32.768
                //            : tempCalibration.zOffset + tempCalibration.zResolution * pointVal;
                //        // 直接操作数组元素 

                //        temp_point_targets[idx].x = tempCalibration.xOffset + tempCalibration.xResolution * j;
                //        temp_point_targets[idx].x = tempCalibration.yOffset + tempCalibration.yResolution * ii;
                //        temp_point_targets[idx].z = point_z;
                //        temp_z_points[idx] = (float)point_z;
                //    }
                //});


                // 全局极值容器 
                double globalMin = double.MaxValue;
                double globalMax = double.MinValue;
                object lockObj = new object();

                Parallel.For(0, Point_height,

                    // 初始化线程局部变量 
                    () => (Min: double.MaxValue, Max: double.MinValue),

                    // 循环主体 
                    (ii, loopState, localData) =>
                    {
                        int baseIndex = ii * Point_width;
                        for (int j = 0; j < Point_width; j++)
                        {
                            int idx = baseIndex + j;
                            short pointVal = points[idx];

                            // 计算z值 
                            double point_z = (pointVal <= -32768)
                                ? -32.768
                                : tempCalibration.zOffset + tempCalibration.zResolution * pointVal;

                            // 更新目标数组（原逻辑保持不变）
                            temp_point_targets[idx].x = tempCalibration.xOffset + tempCalibration.xResolution * j;
                            temp_point_targets[idx].y = tempCalibration.yOffset + tempCalibration.yResolution * ii;
                            temp_point_targets[idx].z = point_z;
                            temp_z_points[idx] = (float)point_z;

                            // 筛选有效值并更新局部极值 
                            if (point_z > -19)
                            {
                                localData.Min = Math.Min(localData.Min, point_z);
                                localData.Max = Math.Max(localData.Max, point_z);
                            }
                        }
                        return localData;
                    },

                    // 合并线程局部结果 
                    localData =>
                    {
                        lock (lockObj)
                        {
                            if (localData.Min < globalMin) globalMin = localData.Min;
                            if (localData.Max > globalMax) globalMax = localData.Max;
                        }
                    });

                // 处理全无效数据的情况 
                if (globalMin == double.MaxValue)
                {
                    globalMin = -10; // 无有效数据时设为阈值 
                    globalMax = 10; // 根据需求决定是否同步处理Max 
                }
                else
                {
                    globalMin = Math.Max(globalMin, -19); // 确保不低于阈值 
                }



                MaxZ = globalMax;
                MinZ= globalMin;




                point_targets = temp_point_targets;
                z_points = temp_z_points;
                #endregion

               
              
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }



        }

        private void Get_Int2Image(GoUniformSurfaceMsg dataObj, out HObject Int2Image, out Cam_Calibration calibration)
        {
            calibration = new Cam_Calibration();
            HOperatorSet.GenEmptyObj(out Int2Image);
            try
            {
                GoUniformSurfaceMsg surfaceMsg = dataObj;
                //DataContext context = new DataContext();
                int Point_bufferSize = (int)surfaceMsg.Length; // 

                calibration.ImageWidth = (int)surfaceMsg.Width;
                calibration.ImageHeight = (int)surfaceMsg.Length;
                calibration.xResolution = (double)surfaceMsg.XResolution / 1000000; //nano metre to mm
                calibration.yResolution = (double)surfaceMsg.YResolution / 1000000;
                calibration.zResolution = (double)surfaceMsg.ZResolution / 1000000;
                calibration.xOffset = (double)surfaceMsg.XOffset / 1000;//um to mm
                calibration.yOffset = (double)surfaceMsg.YOffset / 1000;
                calibration.zOffset = (double)surfaceMsg.ZOffset / 1000;

                ushort[] GrayArrray = new ushort[Point_bufferSize];
                unsafe
                {
                    var sourcePtr = (short*)surfaceMsg.Data;
                    for (int i = 0; i < Point_bufferSize; ++i)
                    {
                        GrayArrray[i] = (ushort)*sourcePtr++;
                    }
                }
                IntPtr imgPtr = Transform_Image.GetPtrFromUShortArray(GrayArrray);
                HOperatorSet.GenImage1(out Int2Image, "uint2", calibration.ImageWidth, calibration.ImageHeight, imgPtr);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        private static void BitmapToHImage(Bitmap SrcImage, out HObject image)
        {
            try
            {
                Point po = new Point(0, 0);
                Size so = new Size(SrcImage.Width, SrcImage.Height);//template.Width, template.Height
                Rectangle ro = new Rectangle(po, so);

                Bitmap DstImage = new Bitmap(SrcImage.Width, SrcImage.Height, PixelFormat.Format8bppIndexed);
                DstImage = SrcImage.Clone(ro, PixelFormat.Format8bppIndexed);

                int width = DstImage.Width;
                int height = DstImage.Height;

                Rectangle rect = new Rectangle(0, 0, width, height);
                System.Drawing.Imaging.BitmapData dstBmpData =
                    DstImage.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);//pImage.PixelFormat
                int PixelSize = Bitmap.GetPixelFormatSize(dstBmpData.PixelFormat) / 8;
                int stride = dstBmpData.Stride;

                //重点在此
                unsafe
                {
                    int count = height * width;
                    byte[] data = new byte[count];
                    byte* bptr = (byte*)dstBmpData.Scan0;
                    fixed (byte* pData = data)
                    {
                        for (int i = 0; i < height; i++)
                            for (int j = 0; j < width; j++)
                            {
                                data[i * width + j] = bptr[i * stride + j];
                            }
                        HOperatorSet.GenImage1(out image, "byte", width, height, new IntPtr(pData));
                    }
                }

                DstImage.UnlockBits(dstBmpData);

            }
            catch (Exception ex)
            {

                image = null;
                Log4netRecord.ErrorLog.Fatal(ex);

            }


        }

        public void onData(KObject data)
        {
            //2021-11-09,lfh
            m_nCount++;
            m_DT_End = DateTime.Now;
            TimeSpan sw = m_DT_End - m_DT_Start;
            if (sw.TotalMilliseconds > 200)
            {
                Write_Log("警告！两行之间时间超过200ms！行数：" + m_nCount);
            }
            m_DT_Start = m_DT_End;

            dataSet = (GoDataSet)data;
            for (UInt32 i = 0; i < dataSet.Count; i++)
            {
                GoDataMsg dataObj = (GoDataMsg)dataSet.Get(i);
                switch (dataObj.MessageType)
                {
                    case GoDataMessageType.Measurement:
                        {
                            GoMeasurementMsg measurementMsg = (GoMeasurementMsg)dataObj;
                            for (UInt32 k = 0; k < measurementMsg.Count; k++)
                            {
                                GoMeasurementData goMeasurementData = measurementMsg.Get(k);
                                measurementList.Add(goMeasurementData.Value);
                                measurementIDs.Add(measurementMsg.Id);
                            }
                        }
                        break;
                    case GoDataMessageType.UniformSurface:
                        {
                            try
                            {
   
                                //2021-11-09,lfh
                                if (m_nCount == 1)
                                {
                                    Write_Log("每1000行记录回调次数： 当前数据长度为：1");
                                }
                                if (m_nCount % 1000 == 0)
                                {
                                    Write_Log("每1000行记录回调次数： 当前数据长度为：" + m_nCount);
                                }
                              
                                GoUniformSurfaceMsg surfaceMsg = (GoUniformSurfaceMsg)dataObj;
                                if (surfaceMsg.Width > 1)
                                {
                                    //Stopwatch stopwatch = Stopwatch.StartNew();
                                    CameraObj.Cam_Calibration calibration = new Cam_Calibration();                                  
                                
                                    Get_DataParameter(surfaceMsg, out point_targets, out z_points, out calibration, out deep_max,out deep_min);
                                    Array.Clear(point_targets, 0, point_targets.Length);
                                    // point_queue.Enqueue(point_targets);
                                    imagergb = Transform_Image.HeightToRainbow_new(z_points, calibration.ImageWidth, calibration.ImageHeight, deep_max, deep_min, false);     //自定义上下限
                                    // 计时结束        
                                    GrabedColorImage = true;
                                    OutPutImage();

                                    //将数据信息存入类中，进队列
                                    Algorithm_CutImage_3D ac3d = new Algorithm_CutImage_3D();
                                    //image3d.Image3D = ac3d.TransIntImage(z_points, calibration.ImageWidth, calibration.ImageHeight);
                                    
                                    image3d.Image3D=TiffProcessForHalcon.Image2HObject
                                    (z_points, calibration.ImageWidth, calibration.ImageHeight);
                                    
                                    GrabedTifImage = true;
                                    image3d.Info3D = calibration;
                                    //2022-06-06,lfh
                                    //image3d.Image_Light = ac3d.LightToGray(z_points, calibration.ImageWidth, calibration.ImageHeight);
                                    OutPutData();

                                    //弹窗显示耗时
                                    //stopwatch.Stop();
                                    //LogName.ErrorLog.Debug($"转化图片代码执行耗时：{stopwatch.ElapsedMilliseconds} 毫秒");


                                }
                            }
                            catch (Exception ex)
                            {
                                Log4netRecord.ErrorLog.Debug(ex);
                                bo_jisuan = false;
                            }
                        }
                        break;
                    case GoDataMessageType.ResampledProfile:
                        {
                            GoResampledProfileMsg profileMsg = (GoResampledProfileMsg)dataObj;
                            Console.WriteLine("  Resampled Profile Message batch count: {0}", profileMsg.Count);
                            for (UInt32 k = 0; k < profileMsg.Count; ++k)
                            {
                                int validPointCount = 0;
                                int Z_point;
                                profilePointCount = profileMsg.Width;
                                Console.WriteLine("    Item[{0}]: Profile data ({1} points)", k, profileMsg.Width);
                                context.xResolution = (double)profileMsg.XResolution / 1000000;
                                context.zResolution = (double)profileMsg.ZResolution / 1000000;
                                context.xOffset = (double)profileMsg.XOffset / 1000;
                                context.zOffset = (double)profileMsg.ZOffset / 1000;

                                int profileZrange = Convert.ToInt32(profilePointCount * context.xResolution / context.zResolution);
                                showProfile = new Bitmap(profilePointCount, profilePointCount);

                                short[] points = new short[profilePointCount];
                                profile_z = new double[profilePointCount];
                                profileBuffer = new ProfilePoint[profilePointCount];
                                IntPtr pointsPtr = profileMsg.Data;
                                Marshal.Copy(pointsPtr, points, 0, points.Length);

                                for (Int32 arrayIndex = 0; arrayIndex < profilePointCount; ++arrayIndex)
                                {
                                    if (points[arrayIndex] != -32768)
                                    {
                                        profileBuffer[arrayIndex].x = context.xOffset + context.xResolution * arrayIndex;
                                        profileBuffer[arrayIndex].z = context.zOffset + context.zResolution * points[arrayIndex];
                                        profile_z[arrayIndex] = profileBuffer[arrayIndex].z;
                                        validPointCount++;
                                        Z_point = Convert.ToInt32(profileBuffer[arrayIndex].z / context.xResolution);
                                        showProfile.SetPixel(arrayIndex, profilePointCount / 2 - Z_point, Point);
                                    }
                                    else
                                    {
                                        profileBuffer[arrayIndex].x = context.xOffset + context.xResolution * arrayIndex;
                                        profileBuffer[arrayIndex].z = -32768;
                                    }
                                }
                                Range_Z = profile_z.Max() - profile_z.Min();

                            }
                        }
                        break;
                    case GoDataMessageType.Video:
                        {
                            GoVideoMsg videoMsg = (GoVideoMsg)dataObj;
                            GoDataSource videoSource = videoMsg.Source;
                            int width = videoMsg.Width;
                            int height = videoMsg.Height;
                            long pixelSize = videoMsg.PixelSize;
                            long cameraIdx = videoMsg.CameraIndex;
                            byte[] rawData = new byte[width * height - 1];
                            IntPtr videoPtr = videoMsg.RowAt(0);
                            Marshal.Copy(videoPtr, rawData, 0, rawData.Length);


                            Bitmap img = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                            BitmapData imgData = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, img.PixelFormat);
                            Marshal.Copy(rawData, 0, imgData.Scan0, rawData.Length);
                            img.UnlockBits(imgData);
                            ColorPalette nomoPalette = img.Palette;
                            for (Int32 k = 0; k < 256; k++)
                            {
                                nomoPalette.Entries[k] = Color.FromArgb(k, k, k);
                            }
                            img.Palette = nomoPalette;
                            BImage = img;


                            //image.Dispose();
                            //BitmapToHImage(BImage, out image);
                            //Queue_GrabImage.Enqueue(image);//图像进队                                        

                            ////获得数据
                            //GoSurfaceMsg surfaceMsg = (GoSurfaceMsg)dataObj;
                            //Get_DataParameter(surfaceMsg, out point_targets, out z_points, out xResolution, out yResolution, out zResolution, out xOffset, out yOffset, out zOffset);

                            //bo_jisuan = true;
                        }
                        break;
                    case GoDataMessageType.SurfaceIntensity:
                        try
                        {
                            GoSurfaceIntensityMsg goSurfaceMsg = (GoSurfaceIntensityMsg)dataObj;
                            if (goSurfaceMsg.Width > 1)
                            {
                                long width = goSurfaceMsg.Width;
                                long height = goSurfaceMsg.Length;
                                long bufferSize = width * height;
                                IntPtr bufferPointer = goSurfaceMsg.Data;

                                byte[] ranges = new byte[bufferSize];
                                Marshal.Copy(bufferPointer, ranges, 0, ranges.Length);

                                HOperatorSet.GenImage1(out image, "byte", width, height, bufferPointer);
                                HOperatorSet.Compose3(image, image, image, out imagelight);

                                //2022-06-08,lfh
                                image3d.Image_Light = imagelight;

                                GrabedLightImage = true;
                                OutPutImage();

                                //2021-11-09,lfh
                                Write_Log("接收到数据： 当前数据长度为：" + height);
                                //m_nCount = 0;
                            }

                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Debug(ex);
                        }
                        break;
                }
            }
            //if (this.textBox2.InvokeRequired == false)//判断调用函数和主线程在不在同一线程里
            //{
            //    if (measurementList.Count != 0)
            //    {
            //        string result = null;
            //        for (int i = 0; i < measurementList.Count; i++)
            //        {
            //            result = result + measurementList[i] + ",";
            //        }
            //        ShowMsg(result, this.textBox2);
            //        measurementList.Clear();
            //        measurementIDs.Clear();
            //    }
            //}
            //else//调用方法和主线程不在同一个线程里
            //{
            //    measurementList.Clear();
            //    measurementIDs.Clear();
            //    //通过使用Invoke的方法，让子线程告诉窗体线程来完成控件操作
            //    onDataType = new OnDataType(onData);
            //    this.textBox2.Invoke(onDataType, data);
            //}
        }

        private static readonly object LockAddimage = new object();
        /// <summary>
        /// 输出彩虹图和亮度图的叠加图像
        /// </summary>
        private void OutPutImage()
        {
            try
            {
                lock (LockAddimage)
                {
                    if (GrabedColorImage && GrabedLightImage)
                    {
                        GrabedColorImage = false;
                        GrabedLightImage = false;
                        HOperatorSet.AddImage(imagelight, imagergb, out image3d.Image2D, 0.5, 0);
                        GrabedRainBowImage = true;
                        OutPutData();

                    }
                }

            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        private static readonly object LockData = new object();
        private void OutPutData()
        {
            try
            {
                lock (LockData)
                {
                    if (GrabedRainBowImage && GrabedTifImage)
                    {
                        GrabCount++;
                        GrabedRainBowImage = false;
                        GrabedTifImage = false;
                        image3d.image_ID = GrabCount;
                        image3d_queue.Enqueue(image3d);
                        CamCallbackEvent();
                        //2021-12-20,lfh
                        Write_Log("将图像数据，添加到image3d_queue");
                        bo_jisuan = true;
                        Log4netRecord.CamLog.Debug(CamID.ToString() + "-" + GrabCount.ToString());//grabResult.ImageNumber
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        public override void DestroyObjects2()
        {
            throw new NotImplementedException();
        }

        public class DataContext
        {
            public double xResolution;
            public double yResolution;
            public double zResolution;
            public double xOffset;
            public double yOffset;
            public double zOffset;
            public uint serialNumber;
        }
        public struct GoPoints
        {
            public Int16 x;
            public Int16 y;
        }
        public struct ProfilePoint
        {
            public double x;
            public double z;
            byte intensity;
        }
        enum kStatus
        {
            kERROR_STATE = -1000,                                                // Invalid state.
            kERROR_NOT_FOUND = -999,                                             // Item is not found.
            kERROR_COMMAND = -998,                                               // Command not recognized.
            kERROR_PARAMETER = -997,                                             // Parameter is invalid.
            kERROR_UNIMPLEMENTED = -996,                                         // Feature not implemented.
            kERROR_HANDLE = -995,                                                // Handle is invalid.
            kERROR_MEMORY = -994,                                                // Out of memory.
            kERROR_TIMEOUT = -993,                                               // Action timed out.
            kERROR_INCOMPLETE = -992,                                            // Buffer not large enough for data.
            kERROR_STREAM = -991,                                                // Error in stream.
            kERROR_CLOSED = -990,                                                // Resource is no longer avaiable. 
            kERROR_VERSION = -989,                                               // Invalid version number.
            kERROR_ABORT = -988,                                                 // Operation aborted.
            kERROR_ALREADY_EXISTS = -987,                                        // Conflicts with existing item.
            kERROR_NETWORK = -986,                                               // Network setup/resource error.
            kERROR_HEAP = -985,                                                  // Heap error (leak/double-free).
            kERROR_FORMAT = -984,                                                // Data parsing/formatting error. 
            kERROR_READ_ONLY = -983,                                             // Object is read-only (cannot be written).
            kERROR_WRITE_ONLY = -982,                                            // Object is write-only (cannot be read). 
            kERROR_BUSY = -981,                                                  // Agent is busy (cannot service request).
            kERROR_CONFLICT = -980,                                              // State conflicts with another object.
            kERROR_OS = -979,                                                    // Generic error reported by underlying OS.
            kERROR_DEVICE = -978,                                                // Hardware device error.
            kERROR_FULL = -977,                                                  // Resource is already fully utilized.
            kERROR_IN_PROGRESS = -976,                                           // Operation is in progress, but not yet complete.
            kERROR = 0,                                                          // General error. 
            kOK = 1                                                              // Operation successful. 
        }
        enum GoDataMessageTypes
        {
            GO_DATA_MESSAGE_TYPE_UNKNOWN = -1,
            GO_DATA_MESSAGE_TYPE_STAMP = 0,
            GO_DATA_MESSAGE_TYPE_HEALTH = 1,
            GO_DATA_MESSAGE_TYPE_VIDEO = 2,
            GO_DATA_MESSAGE_TYPE_RANGE = 3,
            GO_DATA_MESSAGE_TYPE_RANGE_INTENSITY = 4,
            GO_DATA_MESSAGE_TYPE_PROFILE = 5,
            GO_DATA_MESSAGE_TYPE_PROFILE_INTENSITY = 6,
            GO_DATA_MESSAGE_TYPE_RESAMPLED_PROFILE = 7,
            GO_DATA_MESSAGE_TYPE_SURFACE = 8,
            GO_DATA_MESSAGE_TYPE_SURFACE_INTENSITY = 9,
            GO_DATA_MESSAGE_TYPE_MEASUREMENT = 10,
            GO_DATA_MESSAGE_TYPE_ALIGNMENT = 11,
            GO_DATA_MESSAGE_TYPE_EXPOSURE_CAL = 12
        }
        enum GoOutputSource
        {
            GO_OUTPUT_SOURCE_NONE = 0,  ///< Unknown output source.
            GO_OUTPUT_SOURCE_VIDEO = 1,  ///< Output video data.
            GO_OUTPUT_SOURCE_RANGE = 2,  ///< Output range data.
            GO_OUTPUT_SOURCE_PROFILE = 3,  ///< Output profile data.
            GO_OUTPUT_SOURCE_SURFACE = 4,  ///< Output surface data.
            GO_OUTPUT_SOURCE_SECTION = 9,  ///< Output section data.
            GO_OUTPUT_SOURCE_RANGE_INTENSITY = 5,  ///< Output range intensity data.
            GO_OUTPUT_SOURCE_PROFILE_INTENSITY = 6,  ///< Output profile intensity data.
            GO_OUTPUT_SOURCE_SURFACE_INTENSITY = 7,  ///< Output surface intensity data.
            GO_OUTPUT_SOURCE_SECTION_INTENSITY = 10, ///< Output section intensity data.
            GO_OUTPUT_SOURCE_MEASUREMENT = 8, ///< Output measurement data.
            GO_OUTPUT_SOURCE_TRACHEID
        }
        enum GoRole
        {
            GO_ROLE_MAIN = 0,                                                    // Sensor is operating as a main sensor.
            GO_ROLE_BUDDY = 1                                                    // Sensor is operating as a buddy sensor.
        }


        class SensorInformation
        {
            public SensorInformation()
            {

            }

            /// <summary>
            /// 获取传感器内部Job
            /// </summary>
            /// <param name="_sensor"></param>
            /// <returns></returns>
            public List<string> Get_SeneorJob(GoSensor sensor)
            {
                List<string> sensorJobName = new List<string>();
                for (int i = 0; i < sensor.FileCount; i++)
                {
                    sensorJobName.Add(sensor.GetFileName(i));
                }
                return sensorJobName;
            }
            public string Get_SensorDefaultJob(GoSensor sensor)
            {
                string sensorDefaultJob = sensor.DefaultJob;
                return sensorDefaultJob;
            }

            /// <summary>
            /// 获取传感器状态
            /// </summary>
            /// <param name="_sensor"></param>
            /// <returns></returns>
            public string Get_SensorStatus(GoSensor sensor)
            {
                GoState sensorStatus;
                sensorStatus = sensor.State;
                return sensorStatus.ToString();
            }

            /// <summary>
            /// 获取传感器版本号
            /// </summary>
            /// <param name="_sensor"></param>
            /// <returns></returns>
            public string Get_SensorVersions(GoSystem system)
            {
                //GoSensorInfo goSensorInfo = new GoSensorInfo();
                KVersion sensorVersion = system.SdkVersion();
                return sensorVersion.Format();
            }

            
            /// <summary>
            /// 获取传感器序列号
            /// </summary>
            /// <param name="_sensor"></param>
            /// <returns></returns>
            public string Get_SensorSerialNumber(GoSensor sensor)
            {
                UInt32 sensorSerialNumber;
                sensorSerialNumber = sensor.Id;
                return sensorSerialNumber.ToString();
            }

            /// <summary>
            /// 获取传感器型号
            /// </summary>
            /// <param name="_sensor"></param>
            /// <returns></returns>
            public string Get_SensorType(GoSensor sensor)
            {
                string goMode = null;
                try
                {
                    goMode = sensor.Model;
                    return goMode;
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        class SensorOperation
        {
            public SensorOperation()
            {

            }
            public double Get_MaxFrameRate(GoSensor sensor, GoSetup setup)
            {
                setup = sensor.Setup;
                double speed = setup.FrameRate;
                return speed;
            }
            public string Get_TriggerSource(GoSensor sensor, GoSetup setup)
            {
                setup = sensor.Setup;

                string triggerSource = setup.TriggerSource.ToString();
                return triggerSource;
            }
            public double Get_Exposure(GoSensor sensor, GoSetup setup)
            {
                setup = sensor.Setup;
                double exposure = setup.GetExposure(0);
                return exposure;
            }
            public void Set_Exposure(GoSensor sensor, GoSetup setup, string V_Exposure)
            {
                setup = sensor.Setup;
                double Value_Exposure = Convert.ToDouble(V_Exposure);
                setup.SetExposure(Lmi3d.GoSdk.GoRole.Main, Value_Exposure);

            }
            public double Get_Yspace(GoSensor sensor, GoSetup setup)
            {
                setup = sensor.Setup;
                double YLength = setup.EncoderSpacing;
                return YLength;
            }
            public double Get_XLength(GoSensor sensor, GoSetup setup)
            {
                setup = sensor.Setup;
                double XLength = setup.GetActiveAreaWidth(0);
                return XLength;
            }
            public double Get_Xspace(GoSensor sensor, GoSetup setup)
            {
                setup = sensor.Setup;
                double Xspace = setup.GetSpacingIntervalSystemValue(0);
                return Xspace;
            }
            public double Get_YLength(GoSensor sensor, GoSetup setup, GoSurfaceGeneration sgeneration)
            {
                setup = sensor.Setup;
                sgeneration = setup.GetSurfaceGeneration();
                double YLength = sgeneration.FixedLengthLength;
                return YLength;
            }
            public void Set_YLength(GoSensor sensor, GoSetup setup, GoSurfaceGeneration sgeneration, string V_Ylength)
            {
                setup = sensor.Setup;
                sgeneration = setup.GetSurfaceGeneration();
                double Ylength = Convert.ToDouble(V_Ylength);
                sgeneration.FixedLengthLength = Ylength;
            }
            public void Set_ProfileYLength(GoSensor sensor, string FileName)
            {
                //setup = sensor.Setup;
                //sgeneration = setup.GetSurfaceGeneration();
                //double Ylength = Convert.ToDouble(V_Ylength);
                //sgeneration.FixedLengthLength = Ylength;
                bool Exist = false;
                int Number = 0;
                try
                {
                    GoTools tools = sensor.Tools;
                    for (int i = 0; i < tools.ToolCount; i++)//list all available tools;
                    {
                        GoTool tool = tools.GetTool(i);
                        if (tool.Name.Contains("Custom Profile Sort"))
                        {
                            Number = i;

                            break;
                        }
                    }
                    Exist = true;
                    if (Exist)
                    {
                        //GoTool Selecttool = tools.GetTool(Number);
                        //if (Selecttool.ToolType == GoToolType.Extensible)
                        {
                            //GoExtTool extTool = (GoExtTool)Selecttool;
                            //GoExtParamFloat feature = (GoExtParamFloat)extTool.GetParameter(6);//3：Profile Stitch Count:采集行数
                            //Logger?.Info("Value:  " + V_Ylength);
                            //feature.Value = int.Parse("1");
                            //sensor.CopyFile("_live.job", FileName);
                            sensor.CopyFile(FileName, "_live.job");
                        }
                    }
                    else
                    {
                        Log4netRecord.ErrorLog?.Info("Can not Find Custom Profile Sort!");
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog?.Fatal(ex);
                }
            }
        }

        class SensorToolsInformation
        {
            public SensorToolsInformation()
            {

            }
            public List<string> Get_toolsName(GoSensor sensor)
            {
                List<string> toolsName = new List<string>();
                GoTools tools = sensor.Tools;
                for (int i = 0; i < tools.ToolCount; i++)
                {
                    toolsName.Add(tools.GetTool(i).Name);
                }
                return toolsName;
            }
        }

        class ColorMaps
        {
            /// <summary>
            /// OK return null,else error msg
            /// </summary>
            /// <param name="ZValues">Z Points Value </param>
            /// <param name="Size">Points Count</param>
            /// <param name="nullRange">InPut</param>
            /// <param name="minValue">OutPut</param>
            /// <param name="maxValue">OutPut</param>
            /// <returns></returns>
            public string FindMinMaxForColor(ushort[] ZValues, UInt32 Size, ushort nullRange, ref ushort minValue, ref ushort maxValue)
            {
                string Status = string.Empty;
                try
                {
                    UInt32 i;
                    ushort tmp;
                    minValue = ushort.MaxValue;
                    maxValue = 0;
                    for (i = 0; i < Size; i++)
                    {
                        tmp = ZValues[i];
                        if (tmp != nullRange)
                        {
                            if (tmp > maxValue)
                                maxValue = tmp;
                            if (tmp < minValue)
                                minValue = tmp;
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            /// <summary>
            /// put z Value,it will be changed with Color Value;
            /// 
            /// </summary>
            /// <param name="ZValue"></param>
            /// <param name="minValue"></param>
            /// <param name="maxValue"></param>
            /// <param name="nullValue">short.minvalue</param>
            /// <returns></returns>
            public Color ToColor(ushort ZValue, ushort minValue, ushort maxValue, ushort nullValue)
            {
                Color CC = new Color();
                int Steps = 255 + 1;
                int Edges = 7;
                int Total_Steps = Steps * Edges;
                double valueRange = maxValue - minValue;
                CC = Color.FromArgb(0, 0, 0);
                if (maxValue > minValue)
                {
                    if (ZValue != nullValue)
                    {
                        Int32 x = (Int32)(Total_Steps * (ZValue - minValue) / valueRange);
                        if (x < 0 * Steps)
                            CC = Color.FromArgb(0, 0, 0);
                        else if (x < 1 * Steps)
                            CC = Color.FromArgb(0, 0, x - 0 * Steps);
                        else if (x < 2 * Steps)
                            CC = Color.FromArgb(0, x - 1 * Steps, Steps - 1);
                        else if (x < 3 * Steps)
                            CC = Color.FromArgb(0, Steps - 1, 3 * Steps - x - 1);
                        else if (x < 4 * Steps)
                            CC = Color.FromArgb(x - 3 * Steps, Steps - 1, 0);
                        else if (x < 5 * Steps)
                            CC = Color.FromArgb(Steps - 1, 5 * Steps - x - 1, 0);
                        else if (x < 6 * Steps)
                            CC = Color.FromArgb(Steps - 1, 0, x - 5 * Steps);
                        else if (x < 7 * Steps)
                            CC = Color.FromArgb(Steps - 1, x - 6 * Steps, Steps - 1);
                        else
                            CC = Color.FromArgb(Steps - 1, Steps - 1, Steps - 1);
                    }
                }
                return CC;
            }
            /// <summary>
            /// put z[i] Value,it will be changed with Color Value;
            /// return null, ok; else ng
            /// </summary>
            /// <param name="ZValues"></param>
            /// <param name="minValue"></param>
            /// <param name="maxValue"></param>
            /// <param name="nullValue"></param>
            /// <param name="colors"></param>
            /// <param name="count"></param>
            /// <returns></returns>
            public string ToColors(ushort[] ZValues, ushort minValue, ushort maxValue, ushort nullValue, ref Color[] colors, UInt32 count)
            {
                string Status = string.Empty;
                try
                {
                    int Steps = 255 + 1;
                    int Edges = 7;
                    int Total_Steps = Steps * Edges;
                    double valueRange = maxValue - minValue;
                    if (maxValue <= minValue)
                    {
                        return "Error";
                    }
                    colors = new Color[count];
                    for (UInt32 i = 0; i < count; i++)
                    {
                        if (ZValues[i] != nullValue)
                        {
                            Int32 x = (Int32)(Total_Steps * (ZValues[i] - minValue) / valueRange);
                            if (x < 0 * Steps)
                                colors[i] = Color.FromArgb(0, 0, 0);
                            else if (x < 1 * Steps)
                                colors[i] = Color.FromArgb(0, 0, x - 0 * Steps);
                            else if (x < 2 * Steps)
                                colors[i] = Color.FromArgb(0, x - 1 * Steps, Steps - 1);
                            else if (x < 3 * Steps)
                                colors[i] = Color.FromArgb(0, Steps - 1, 3 * Steps - x - 1);
                            else if (x < 4 * Steps)
                                colors[i] = Color.FromArgb(x - 3 * Steps, Steps - 1, 0);
                            else if (x < 5 * Steps)
                                colors[i] = Color.FromArgb(Steps - 1, 5 * Steps - x - 1, 0);
                            else if (x < 6 * Steps)
                                colors[i] = Color.FromArgb(Steps - 1, 0, x - 5 * Steps);
                            else if (x < 7 * Steps)
                                colors[i] = Color.FromArgb(Steps - 1, x - 6 * Steps, Steps - 1);
                            else
                                colors[i] = Color.FromArgb(Steps - 1, Steps - 1, Steps - 1);
                        }
                        else
                        {
                            colors[i] = Color.FromArgb(0, 0, 0);
                        }
                    }
                    return Status = null;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }



        #endregion
    }
}
