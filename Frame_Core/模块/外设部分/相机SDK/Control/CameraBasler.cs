using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using Basler.Pylon;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Frame_Core;
using LogTool;

namespace Frame_Core
{
    public class CameraBasler : CameraObj
    {
        #region menber
        List<ICameraInfo> allCameras = null;//创建ICameraInfo对象的列表，用于保存遍历到的所有相机信息
        private PixelDataConverter converter = new PixelDataConverter();
        Camera myCamera = null;//创建相机对象
        HImage image = null;
        private string strUserID = null;
        private IntPtr latestFrameAddress = IntPtr.Zero;
        private bool bo_jisuan = false;

        public long imageWidth = 0;         // 图像宽
        public long imageHeight = 0;        // 图像高
        public long minExposureTime = 0;    // 最小曝光时间
        public long maxExposureTime = 0;    // 最大曝光时间
        public long minGain = 0;            // 最小增益
        public long maxGain = 0;            // 最大增益

        public HObject Image = new HObject();



        public bool m_bonline = false;
        bool m_bGrabbing = false;

        public override bool IsCameraStatue
        {
            get
            {
                bool b = false;
                try
                {
                    if (myCamera != null)
                        b = myCamera.StreamGrabber.IsGrabbing;


                }
                catch (Exception)
                {
                    b = false;
                }
                return b;
            }
        }

        public override bool IsCameraOpen
        {
            get
            {
                bool b = false;
                try
                {
                    if (myCamera != null)
                        b = myCamera.IsOpen && myCamera.IsConnected;
                }
                catch (Exception)
                {
                    b = false;
                }
                return b;
            }
        }

        QueuePackage<ImageInfo> Queue_GrabImage = new QueuePackage<ImageInfo>();//采集到的图像进入队列 

        public override QueuePackage<ImageInfo> QueueGrabImage
        {
            get { return Queue_GrabImage; }
        }
        string CameraSerial = "";
        string PC_IP = "";
        string Camera_IP = "";
        #endregion

        #region function


        public CameraBasler()
        {
            try
            {
                myCamera = new Camera();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }

        }
        public CameraBasler(string UserID)
        {
            try
            {

                DestroyCamera();
                allCameras = CameraFinder.Enumerate();//获取所有相机设备

                foreach (ICameraInfo cameraInfo in allCameras)
                {
                    string str = "";
                    str = cameraInfo[CameraInfoKey.SerialNumber].ToString();
                    str = cameraInfo[CameraInfoKey.FriendlyName].ToString();
                    str = cameraInfo[CameraInfoKey.UserDefinedName].ToString();
                    if (UserID == cameraInfo[CameraInfoKey.SerialNumber])//CameraInfoKey.UserDefinedName\CameraInfoKey.FriendlyName
                    {
                        myCamera = new Camera(cameraInfo);
                        strUserID = UserID;

                        bo_jisuan = false;

                    }
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);

            }
        }


        /// <summary>
        /// 打开相机
        /// </summary>
        public bool openCam()
        {

            try
            {
                if (myCamera == null) return false;
                myCamera.Open();
                imageWidth = myCamera.Parameters[PLCamera.Width].GetValue();               // 获取图像宽 
                imageHeight = myCamera.Parameters[PLCamera.Height].GetValue();              // 获取图像高
                GetMinMaxExposureTime();
                //GetMinMaxGain();
                myCamera.StreamGrabber.ImageGrabbed -= OnImageGrabbed;
                myCamera.StreamGrabber.ImageGrabbed += OnImageGrabbed;                      // 注册采集回调函数
                //myCamera.ConnectionLost -= OnConnectionLost;
                //myCamera.ConnectionLost += OnConnectionLost;

                //myCamera.CameraOpened += Configuration.AcquireContinuous;

                // Register for the events of the image provider needed for proper operation.
                //myCamera.ConnectionLost += OnConnectionLost;
                //myCamera.CameraOpened += OnCameraOpened;
                //myCamera.CameraClosed += OnCameraClosed;
                //myCamera.StreamGrabber.GrabStarted += OnGrabStarted;
                //myCamera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                //myCamera.StreamGrabber.GrabStopped += OnGrabStopped;
                //myCamera.Open();//打开相机
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool IsExist
        {
            get { return bo_jisuan; }
            set { bo_jisuan = value; }
        }



        /// <summary>
        /// 单张采集
        /// </summary>
        public bool GrabOne()
        {
            try
            {
                if (myCamera.StreamGrabber.IsGrabbing)
                {
                    System.Windows.Forms.MessageBox.Show("相机正在抓取图像！！！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    myCamera.Parameters[PLCamera.AcquisitionMode].SetValue("SingleFrame");
                    myCamera.StreamGrabber.Start(1, GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// 开始连续采集
        /// </summary>
        public bool StartGrabbing()
        {

            try
            {
                if (myCamera.StreamGrabber.IsGrabbing)
                {
                    System.Windows.Forms.MessageBox.Show("相机正在抓取图像！！！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    myCamera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                    myCamera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }

            //========================================================
            //===【】方式二
            try
            {
                // Start the grabbing of images until grabbing is stopped.
                Configuration.AcquireContinuous(myCamera, null);
                myCamera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 停止连续采集
        /// </summary>
        public bool StopGrabbing()
        {
            try
            {
                myCamera.StreamGrabber.Stop();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 断开相机
        public int DestroyCamera()//关闭相机，返回-1为失败，1为成功
        {
            // 断开相机
            try
            {
                if (myCamera != null)
                {
                    myCamera.Close();
                    myCamera.Dispose();
                    myCamera = null;
                }


            }
            catch (Exception)
            {
                return -1;
            }

            return 0;
        }

        public HImage ReadBuffer()//读取相机buffer并生成HImage格式的图像
        {
            IGrabResult grabResult = myCamera.StreamGrabber.RetrieveResult(4000, TimeoutHandling.ThrowException);//读取buffer，超时时间为4000ms
            image = new HImage();
            using (grabResult)
            {
                if (grabResult.GrabSucceeded)
                {
                    if (IsMonoData(grabResult))
                    {
                        //如果是黑白图像，则利用GenImage1算子生成黑白图像
                        byte[] buffer = grabResult.PixelData as byte[];
                        IntPtr p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                        image.GenImage1("byte", grabResult.Width, grabResult.Height, p);
                    }
                    else
                    {
                        if (grabResult.PixelTypeValue != PixelType.RGB8packed)
                        {
                            //如果图像不是RGB8格式，则将图像转换为RGB8，然后生成彩色图像
                            //因为GenImageInterleaved算子能够生成的图像的数据，常见的格式只有RGB8
                            //如果采集的图像是RGB8则不需转换，具体生成图像方法请自行测试编写。
                            byte[] buffer_rgb = new byte[grabResult.Width * grabResult.Height * 3];
                            Basler.Pylon.PixelDataConverter convert = new PixelDataConverter();
                            convert.OutputPixelFormat = PixelType.RGB8packed;
                            convert.Convert(buffer_rgb, grabResult);
                            IntPtr p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer_rgb, 0);
                            image.GenImageInterleaved(p, "rgb", grabResult.Width, grabResult.Height, 0, "byte", grabResult.Width, grabResult.Height, 0, 0, -1, 0);
                        }
                    }
                    return image;
                }
                else
                {
                    return null;
                }
            }
        }

        // 相机取像回调函数.
        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e.GrabResult;
                if (grabResult.GrabSucceeded)
                {
                    GrabCount++;
                    if (IsMonoData(grabResult))
                    {
                        if (latestFrameAddress == IntPtr.Zero)
                        {
                            latestFrameAddress = Marshal.AllocHGlobal((Int32)grabResult.PayloadSize);
                        }
                        converter.OutputPixelFormat = PixelType.Mono8;
                        converter.Convert(latestFrameAddress, grabResult.PayloadSize, grabResult);
                        // 转换为Halcon图像显示
                        HOperatorSet.GenImage1(out Image, "byte", (HTuple)grabResult.Width, (HTuple)grabResult.Height, (HTuple)latestFrameAddress);
                        Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = grabResult.Width, imageHeight = grabResult.Height });//图像进队

                        // 抛出图像处理事件
                        bo_jisuan = true;
                    }
                    else if (grabResult.PixelTypeValue == PixelType.BayerBG8 || grabResult.PixelTypeValue == PixelType.BayerGB8
                               || grabResult.PixelTypeValue == PixelType.BayerRG8 || grabResult.PixelTypeValue == PixelType.BayerGR8)
                    {
                        int imageWidth = grabResult.Width - 1;
                        int imageHeight = grabResult.Height - 1;
                        int payloadSize = imageWidth * imageHeight;
                        if (latestFrameAddress == IntPtr.Zero)
                        {
                            latestFrameAddress = Marshal.AllocHGlobal((Int32)(3 * payloadSize));
                        }
                        converter.OutputPixelFormat = PixelType.BGR8packed;
                        // 根据bayer格式不同切换以下代码
                        converter.Parameters[PLPixelDataConverter.InconvertibleEdgeHandling].SetValue("Clip");
                        converter.Convert(latestFrameAddress, 3 * payloadSize, grabResult);

                        HOperatorSet.GenImageInterleaved(out Image, latestFrameAddress, "bgr", (HTuple)imageWidth, (HTuple)imageHeight, -1, "byte", (HTuple)imageWidth, (HTuple)imageHeight, 0, 0, -1, 0);
                        Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = grabResult.Width, imageHeight = grabResult.Height });//图像进队

                        bo_jisuan = true;
                    }
                    else if (grabResult.PixelTypeValue == PixelType.YUV422packed)
                    {
                        if (latestFrameAddress == IntPtr.Zero)
                        {
                            latestFrameAddress = Marshal.AllocHGlobal((Int32)grabResult.PayloadSize);
                        }
                        converter.OutputPixelFormat = PixelType.Mono8;
                        converter.Convert(latestFrameAddress, grabResult.PayloadSize, grabResult);
                        // 转换为Halcon图像显示
                        HOperatorSet.GenImage1(out Image, "byte", (HTuple)grabResult.Width, (HTuple)grabResult.Height, (HTuple)latestFrameAddress);
                        Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = grabResult.Width, imageHeight = grabResult.Height });//图像进队

                        // 抛出图像处理事件
                        bo_jisuan = true;
                    }
                    CamCallbackEvent?.Invoke();
                    Log4netRecord.CamLog.Debug(CamID.ToString() + "-" + GrabCount.ToString());//grabResult.ImageNumber
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Grab faild!\n" + grabResult.ErrorDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
              
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
            finally
            {
                e.DisposeGrabResultIfClone();
            }
        }

        /*********************************************************/

        /******************    相机参数设置   ********************/
        /// <summary>
        /// 设置Gige相机心跳时间
        /// </summary>
        /// <param name="value"></param>
        public void SetHeartBeatTime(long value)
        {
            try
            {
                // 判断是否是网口相机
                if (myCamera.GetSfncVersion() < Sfnc2_0_0)
                {
                    myCamera.Parameters[PLGigECamera.GevHeartbeatTimeout].SetValue(value);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }
        /// <summary>
        /// 获取最小最大曝光时间
        /// </summary>
        public double GetMinMaxExposureTime()
        {

            if (myCamera.GetSfncVersion() < Sfnc2_0_0)
            {
                minExposureTime = myCamera.Parameters[PLCamera.ExposureTimeRaw].GetMinimum();
                maxExposureTime = myCamera.Parameters[PLCamera.ExposureTimeRaw].GetMaximum();
                return myCamera.Parameters[PLCamera.ExposureTimeRaw].GetValue();
            }
            else
            {
                minExposureTime = (long)myCamera.Parameters[PLUsbCamera.ExposureTime].GetMinimum();
                maxExposureTime = (long)myCamera.Parameters[PLUsbCamera.ExposureTime].GetMaximum();
                return myCamera.Parameters[PLUsbCamera.ExposureTime].GetValue();
            }

        }
        public void setExposureTime(long Value)//设置曝光时间us
        {
            if (myCamera != null)
            {

                myCamera.Parameters[PLCamera.ExposureAuto].TrySetValue(PLCamera.ExposureAuto.Off);
                myCamera.Parameters[PLCamera.ExposureMode].TrySetValue(PLCamera.ExposureMode.Timed);
                if (myCamera.GetSfncVersion() < Sfnc2_0_0)
                {
                    long value;
                    long min = myCamera.Parameters[PLCamera.ExposureTimeRaw].GetMinimum();
                    long max = myCamera.Parameters[PLCamera.ExposureTimeRaw].GetMaximum();
                    long incr = myCamera.Parameters[PLCamera.ExposureTimeRaw].GetIncrement();
                    if (Value < min)
                    {
                        value = min;
                    }
                    else if (Value > max)
                    {
                        value = max;
                    }
                    else
                    {
                        value = min + ((((long)Value - min) / incr) * incr);
                    }
                    myCamera.Parameters[PLCamera.ExposureTimeRaw].SetValue(value);
                }
                else
                {
                    if (Value <= myCamera.Parameters[PLUsbCamera.ExposureTime].GetMaximum() && Value >= myCamera.Parameters[PLUsbCamera.ExposureTime].GetMinimum())
                    {
                        myCamera.Parameters[PLUsbCamera.ExposureTime].SetValue((long)Value);
                    }

                }
            }
        }

        public void pixelFormat(string pixelType)
        {

            switch (pixelType)
            {
                case "Mono8":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono8);
                    break;
                case "Mono8Signed":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono8Signed);
                    break;
                case "Mono10":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono10);
                    break;
                case "Mono10p":
                    //myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono10p);
                    break;
                case "Mono10packed":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono10Packed);
                    break;
                case "Mono12":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono12);
                    break;
                case "Mono12p":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono12p);
                    break;
                case "Mono12Packed":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono12Packed);
                    break;
                case "Mono16":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono16);
                    break;
                case "YUV422Packed":
                    myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.YUV422Packed);//彩色图像
                    break;
            }


        }

        public void setHeight(long height)//设置图像高度
        {
            myCamera.Parameters[PLCamera.Height].TrySetValue(height);
        }

        public void setWidth(long width)//设置图像宽度
        {
            myCamera.Parameters[PLCamera.Width].TrySetValue(width);
        }


        /// <summary>
        /// if >= Sfnc2_0_0,说明是ＵＳＢ３的相机
        /// </summary>
        static Version Sfnc2_0_0 = new Version(2, 0, 0);

        /// <summary>
        /// 获取增益
        /// </summary>
        /// <returns></returns>
        public double GetGain()
        {
            if (myCamera.GetSfncVersion() < Sfnc2_0_0)
            {
                minGain = myCamera.Parameters[PLCamera.GainRaw].GetMinimum();
                maxGain = myCamera.Parameters[PLCamera.GainRaw].GetMaximum();
                return myCamera.Parameters[PLCamera.GainRaw].GetValue();
            }
            else
            {
                minGain = (long)myCamera.Parameters[PLUsbCamera.Gain].GetMinimum();
                maxGain = (long)myCamera.Parameters[PLUsbCamera.Gain].GetMaximum();
                return myCamera.Parameters[PLUsbCamera.Gain].GetValue();
            }
        }

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="value"></param>
        public bool setGain(long value)
        {
            try
            {
                myCamera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Off);

                if (myCamera.GetSfncVersion() < Sfnc2_0_0)
                {
                    long min = myCamera.Parameters[PLCamera.GainRaw].GetMinimum();
                    long max = myCamera.Parameters[PLCamera.GainRaw].GetMaximum();
                    long incr = myCamera.Parameters[PLCamera.GainRaw].GetIncrement();
                    if (value < min)
                    {
                        value = min;
                    }
                    else if (value > max)
                    {
                        value = max;
                    }
                    else
                    {
                        value = min + (((value - min) / incr) * incr);
                    }
                    myCamera.Parameters[PLCamera.GainRaw].SetValue(value);
                    return true;
                }
                else
                {
                    myCamera.Parameters[PLUsbCamera.Gain].SetValue(value);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void setOffsetX(long offsetX)//设置图像水平偏移
        {
            myCamera.Parameters[PLCamera.OffsetX].TrySetValue(offsetX);
        }

        public void setOffsetY(long offsetY)//设置图像竖直偏移
        {
            myCamera.Parameters[PLCamera.OffsetY].TrySetValue(offsetY);
        }

        public void softTrigger()//发送软触发命令
        {
            myCamera.ExecuteSoftwareTrigger();
        }

        public bool setTriggerMode(uint TriggerModeNum)//设置触发模式
        {
            //1:为On 触发模式
            //0:Off 触发模式
            try
            {
                if (myCamera.Parameters[PLCamera.TriggerMode].TrySetValue(TriggerModeNum == 1 ? "On" : "Off"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        ///// <summary>
        ///// 获取触发模式
        ///// </summary>
        ///// <returns></returns>
        //public TriggerMode GetTriggerMode()
        //{
        //    if (myCamera != null)
        //    {
        //        return (TriggerMode)Enum.Parse(typeof(TriggerMode), myCamera.Parameters[PLCamera.TriggerMode].GetValue());
        //    }
        //    return TriggerMode.Off;
        //}
        ///// <summary>
        ///// 获取触发源
        ///// </summary>
        ///// <returns></returns>
        //public TriggerSource GetTriggerSource()
        //{
        //    if (myCamera != null)
        //    {
        //        return (TriggerSource)Enum.Parse(typeof(TriggerSource), myCamera.Parameters[PLCamera.TriggerSource].GetValue());
        //    }
        //    return TriggerSource.Software;
        //}

        public bool setTriggerSource(uint TriggerSourceNum)//设置触发源
        {
            //直接改为软触发，此处已经写死
            try
            {
                if (myCamera.Parameters[PLCamera.TriggerSource].TrySetValue("Software"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private Boolean IsMonoData(IGrabResult iGrabResult)//判断图像是否为黑白格式
        {
            switch (iGrabResult.PixelTypeValue)
            {
                case PixelType.Mono1packed:
                case PixelType.Mono2packed:
                case PixelType.Mono4packed:
                case PixelType.Mono8:
                case PixelType.Mono8signed:
                case PixelType.Mono10:
                case PixelType.Mono10p:
                case PixelType.Mono10packed:
                case PixelType.Mono12:
                case PixelType.Mono12p:
                case PixelType.Mono12packed:
                case PixelType.Mono16:
                    return true;
                default:

                    return false;
            }
        }

        #region 相机绑定事件         

        public QueuePackage<structCmaeraData> QuestructCmaeraData = new QueuePackage<structCmaeraData>();

        //意外断开连接时发生
        public void OnConnectionLost(Object sender, EventArgs e)
        {
            try
            {

                structCmaeraData data = new structCmaeraData();

                data.time = DateTime.Now.ToString("yyyyMMdd HH-mm-ss-fff");

                data.CmaeraData = "Error:The camera has been accidentally disconnected!!!";

                data.EventType = "ConnectionLost";

                QuestructCmaeraData.Enqueue(data);

                myCamera.Close();

                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        myCamera.Open();
                        if (myCamera.IsOpen)
                        {
                            System.Windows.Forms.MessageBox.Show("已重新连接上序列号为“" + strUserID + "”的相机！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        Thread.Sleep(200);
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("请重新连接序列号为“" + strUserID + "”的相机！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (myCamera == null)
                {
                    System.Windows.Forms.MessageBox.Show("重连超时20s:未识别到序列号为“" + strUserID + "”的相机！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                SetHeartBeatTime(5000);

                myCamera.CameraOpened -= Configuration.AcquireContinuous;
                myCamera.CameraOpened += Configuration.AcquireContinuous;
                // Register for the events of the image provider needed for proper operation.
                myCamera.ConnectionLost -= OnConnectionLost;
                myCamera.ConnectionLost += OnConnectionLost;

                myCamera.CameraOpened -= OnCameraOpened;
                myCamera.CameraOpened += OnCameraOpened;

                myCamera.CameraClosed -= OnCameraClosed;
                myCamera.CameraClosed += OnCameraClosed;

                myCamera.StreamGrabber.GrabStarted -= OnGrabStarted;
                myCamera.StreamGrabber.GrabStarted += OnGrabStarted;

                myCamera.StreamGrabber.ImageGrabbed -= OnImageGrabbed;
                myCamera.StreamGrabber.ImageGrabbed += OnImageGrabbed;

                myCamera.StreamGrabber.GrabStopped -= OnGrabStopped;
                myCamera.StreamGrabber.GrabStopped += OnGrabStopped;


            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        //相机打开后发生
        public void OnCameraOpened(Object sender, EventArgs e)
        {
            try
            {
                structCmaeraData data = new structCmaeraData();

                data.time = DateTime.Now.ToString("yyyyMMdd HH-mm-ss-fff");

                data.CmaeraData = "Event: Camera is turned on!";

                data.EventType = "CameraOpened";

                QuestructCmaeraData.Enqueue(data);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        //相机关闭后发生
        public void OnCameraClosed(Object sender, EventArgs e)
        {
            try
            {
                structCmaeraData data = new structCmaeraData();

                data.time = DateTime.Now.ToString("yyyyMMdd HH-mm-ss-fff");

                data.CmaeraData = "Event: Camera is turned off!";

                data.EventType = "CameraClosed";

                QuestructCmaeraData.Enqueue(data);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        //开始抓图后发生
        public void OnGrabStarted(Object sender, EventArgs e)
        {
            try
            {
                structCmaeraData data = new structCmaeraData();

                data.time = DateTime.Now.ToString("yyyyMMdd HH-mm-ss-fff");

                data.CmaeraData = "Event: The camera began to take pictures!";

                data.EventType = "GrabStarted";

                QuestructCmaeraData.Enqueue(data);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        //抓完图后发生
        public void OnGrabStopped(Object sender, EventArgs e)
        {
            try
            {
                structCmaeraData data = new structCmaeraData();

                data.time = DateTime.Now.ToString("yyyyMMdd HH-mm-ss-fff");

                data.CmaeraData = "Event:  Camera shot completed!";

                data.EventType = "GrabStopped";

                QuestructCmaeraData.Enqueue(data);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        private void ShowException(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        public override bool OpenCam()
        {
            return true;
            //return openCam();
        }

        public override bool InitCam()
        {
            bool init = true;
            init = init && openCam();
            //init = init && SetTriggerMode(1);
            //init = init && setTriggerSource(1);
            return init;
        }

        public override bool StartGrabImage()
        {
            return StartGrabbing();
        }

        public override bool StopGrabImage()
        {
            return StopGrabbing();
        }

        public override void DestroyCam()
        {
            DestroyCamera();
        }

        public override void DestroyObjects2()
        {
            DestroyCamera();
        }

        public override bool SetTriggerMode(int mode)
        {
            return setTriggerMode((uint)mode);

        }

        public override bool SetGain(float gain)
        {
            return setGain((long)gain);
        }

        public override bool SetGrabState(bool bStart)
        {
            throw new NotImplementedException();
        }


        #endregion


    }
    public struct structCmaeraData
    {
        public string time;
        public string CmaeraData;
        public string EventType;
    }
}
