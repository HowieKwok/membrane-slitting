using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;
using Lmi3d.GoSdk;
using LogTool;
using MvCamCtrl.NET;
using MvCameraControl;
using MvFGCtrlC.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace DevMgmt.Cameras.SDK
{
    public class CameraHKControl : CameraObj
    {
        log4net.ILog LogDebug = Log4netRecord.ErrorLog;

        #region field

        private new bool connStatus = false;
        public new bool ConnStatus
        {
            get
            {
                if (device != null)
                    connStatus = device.IsConnected;
                else
                    connStatus = false;
                return connStatus;
            }
        }

        public override HObject Image
        {
            get { return image; }
        }

        public override bool IsExist
        {

            get { return bo_jisuan; }
            set { bo_jisuan = value; }
        }

        public override QueuePackage<FrameCommunity.Data.ImageInfo> QueueGrabImage
        {
            get { return Queue_GrabImage; }
        }
        QueuePackage<FrameCommunity.Data.ImageInfo> Queue_GrabImage = new QueuePackage<FrameCommunity.Data.ImageInfo>();//采集到的图像进入队列
        //帧号
        public uint FrameNum { set; get; }
        //帧数
        public uint FrameCounter { set; get; }
        //触发计数
        public uint TriggerIndex { set; get; }
        bool isGrab = false;
        #endregion

        #region member
        HObject image;
        bool bo_jisuan = false;
        string CameraSerial = "";
        int ListNum = 0;
        #endregion

        string CardSerial = "";
        private const InterfaceTLayerType IFLayerType = InterfaceTLayerType.MvGigEInterface | InterfaceTLayerType.MvCameraLinkInterface | InterfaceTLayerType.MvCXPInterface
          | InterfaceTLayerType.MvXoFInterface;
        IInterface ifInstance = null;
        List<IInterfaceInfo> IFInfoList;


        readonly DeviceTLayerType enumTLayerType = DeviceTLayerType.MvGigEDevice | DeviceTLayerType.MvUsbDevice
              | DeviceTLayerType.MvGenTLGigEDevice | DeviceTLayerType.MvGenTLCXPDevice | DeviceTLayerType.MvGenTLCameraLinkDevice | DeviceTLayerType.MvGenTLXoFDevice;
        IDevice device = null;
        List<IDeviceInfo> deviceInfos = new List<IDeviceInfo>();
        public CameraHKControl(string cardSerial, string CameraSerial_str)
        {
            CameraSerial = CameraSerial_str;
            CardSerial = cardSerial;
        }

        public override void DestroyCam()
        {
            try
            {
                int ret = device?.Close() ?? -1;
                if (ret != MvError.MV_OK)
                {
                    return;
                }

                device?.Dispose();

                ret = ifInstance?.Close() ?? -1;
                if (ret != MvError.MV_OK)
                {
                    return;
                }

                ifInstance?.Dispose();
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
            }
        }

        public override void DestroyObjects2()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
            }
        }

        public override bool InitCam()
        {
            try
            {
                //【】采集卡
                int ret = InterfaceEnumerator.EnumInterfaces(IFLayerType, out IFInfoList);
                if (ret != MvError.MV_OK)
                {
                    //Console.WriteLine("Enum interface failed:{0:x8}"+ ret);
                    return false;
                }


                //【】设备
                int result = DeviceEnumerator.EnumDevices(enumTLayerType, out deviceInfos);
                if (result != MvError.MV_OK)
                {
                    return false;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }

        public override bool OpenCam()
        {
            try
            {
                //【】采集卡
                IInterfaceInfo interfaceInfo = IFInfoList.Find(p => p.SerialNumber == CardSerial);
                if (interfaceInfo == null) { return false; }
                ifInstance = InterfaceFactory.CreateInterface(interfaceInfo);


                int ret = ifInstance.Open();
                if (ret != MvError.MV_OK)
                {
                    ifInstance.Dispose();
                    ifInstance = null;
                    //Console.WriteLine("Open Interface failed:{0:x8}"+ ret);
                    return false;
                }

                //ch：开启采集卡指定事件 | en: Turn on the specific event of interface
                ret = ifInstance.EventNotificationOn("ReceiveImageFrameStart0");
                if (ret != MvError.MV_OK)
                {
                    //Console.WriteLine("EventNotificationOn failed:{0:x8}"+ ret);
                    return false;
                }

                // ch:注册回调函数并订阅事件 | en:Register Event callback
                ifInstance.EventGrabber.DeviceEvent -= DeviceEventGrabedHandler;
                ifInstance.EventGrabber.DeviceEvent += DeviceEventGrabedHandler;
                ifInstance.EventGrabber.SubscribeEvent("ReceiveImageFrameStart0");


                //【】设备
                IDeviceInfo deviceInfo = deviceInfos.Find(p => p.SerialNumber == CameraSerial);
                if (deviceInfo == null) { return false; }
                device = DeviceFactory.CreateDevice(deviceInfo);


                int result = device.Open();
                if (result != MvError.MV_OK)
                {
                    device.Dispose();
                    device = null;

                    return false;
                }

                //ch: 判断是否为gige设备 | en: Determine whether it is a GigE device
                if (device is IGigEDevice)
                {
                    //ch: 转换为gigE设备 | en: Convert to Gige device
                    IGigEDevice gigEDevice = device as IGigEDevice;

                    // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                    int optionPacketSize;
                    result = gigEDevice.GetOptimalPacketSize(out optionPacketSize);
                    if (result != MvError.MV_OK)
                    {
                        return false;
                    }
                    else
                    {
                        result = device.Parameters.SetIntValue("GevSCPSPacketSize", (long)optionPacketSize);
                        if (result != MvError.MV_OK)
                        {
                            return false;
                        }
                    }
                }

                //ch: 设置合适的缓存节点数量 | en: Setting the appropriate number of image nodes
                int nRet = device.StreamGrabber.SetImageNodeNum(5);

                // ch:注册回调函数 | en:Register image callback

                device.StreamGrabber.FrameGrabedEvent -= get_Image;
                device.StreamGrabber.FrameGrabedEvent += get_Image;



                return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }

        public override bool SetGain(float gain)
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }

        public override bool SetGrabState(bool bStart)
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                //if (bStart)
                //{
                //    int nRet = device.Parameters.SetEnumValue("AcquisitionStart", 0);
                //    if (MyCamera.MV_OK != nRet)
                //    {
                //        LogDebug.Error("Set \"AcquisitionStart\" failed, S / N:" + CameraSerial);
                //        return false;
                //    }
                //}
                //else
                //{
                //    int nRet = device.Parameters.SetEnumValue("AcquisitionStop", 0);
                //    if (MyCamera.MV_OK != nRet)
                //    {
                //        LogDebug.Error("Set \"AcquisitionStop\" failed, S / N:" + CameraSerial);
                //        return false;
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }

        public override bool SetTriggerMode(int mode)
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                int ret = device.Parameters.SetEnumValue("TriggerMode", (uint)mode);
                if (ret != MvError.MV_OK)
                {
                    LogDebug.Debug("Set Trigger Mode fail! nRet {0}\n" + ret.ToString("X"));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }

        public override bool StartGrabImage()
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                // ch:开启抓图 || en: start grab image
                int ret = device.StreamGrabber.StartGrabbing();
                if (ret != MvError.MV_OK)
                {
                    LogDebug.Error("Start grabbing failed:{0:x8}" + ret);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }

        public override bool StopGrabImage()
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                // ch:停止抓图 | en:Stop grabbing
                int ret = device.StreamGrabber.StopGrabbing();
                if (ret != MvError.MV_OK)
                {
                    LogDebug.Error("Stop grabbing failed:{0:x8}" + ret);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }

   

        public void get_Image(object sender, FrameGrabbedEventArgs e)
        {
            try
            {            
                Log4netRecord.CamLog.Debug($"相机{CamID}回调开始" + "-" + (GrabCount + 1).ToString());
                CamCallbackStartEvent?.Invoke(GrabCount + 1);
                bo_jisuan = true;
                HTuple w = new HTuple(), h = new HTuple();
                if (e.FrameOut.Image.PixelType == MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    GrabCount++;
                    HOperatorSet.GenImage1(out image, "byte", e.FrameOut.Image.Width, e.FrameOut.Image.Height, e.FrameOut.Image.PixelDataPtr);
                    HOperatorSet.GetImageSize(image, out w, out h);
                    if (w != 0 && h != 0)
                        Queue_GrabImage.Enqueue(new FrameCommunity.Data.ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = e.FrameOut.Image.Width, imageHeight = e.FrameOut.Image.Height });//图像进队 
                }
                else if (e.FrameOut.Image.PixelType == MvGvspPixelType.PixelType_Gvsp_RGB8_Packed)
                {
                    GrabCount++;
                    HOperatorSet.GenImageInterleaved(out image, e.FrameOut.Image.PixelDataPtr, "rgb", e.FrameOut.Image.Width, e.FrameOut.Image.Height, -1, "byte", e.FrameOut.Image.Width, e.FrameOut.Image.Height, 0, 0, -1, 0);
                    HOperatorSet.GetImageSize(image, out w, out h);
                    if (w != 0 && h != 0)
                        Queue_GrabImage.Enqueue(new FrameCommunity.Data.ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = e.FrameOut.Image.Width, imageHeight = e.FrameOut.Image.Height });//图像进队 
                }
                else
                {
                    LogDebug.Error("Else PixelType：" + e.FrameOut.Image.PixelType.ToString() + ", S/N:" + CameraSerial);
                    bo_jisuan = false;
                    return;
                }
                CamCallbackEndEvent?.Invoke();

                Log4netRecord.CamLog.Debug($"相机{CamID}回调结束" + "-" + GrabCount.ToString());


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return;
            }
        }

        public void DeviceEventGrabedHandler(object sender, DeviceEventArgs e)
        {
            try
            {
                if (FrameStartEvent != null)
                {
                    //Console.WriteLine("EventName[{0}], EventID[{1}]", e.EventInfo.EventName, e.EventInfo.EventID);
                    Log4netRecord.CamLog.Debug($"相机{CamID}帧开始信号回调开始" + "-" + (GrabCount + 1).ToString());
                    FrameStartEvent?.Invoke(GrabCount + 1);
                    Log4netRecord.CamLog.Debug($"相机{CamID}帧开始信号回调结束" + "-" + (GrabCount + 1).ToString());
                }             
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return;
            }

        }

        public void InOutDemo()
        {

            // ch:切换LineSelector 为Line1 | en:Set LineSelector as Line1
            int ret = device.Parameters.SetEnumValueByString("LineSelector", "Line1");
            if (ret != MvError.MV_OK)
            {
                LogDebug.Error("Set line selector fail! nRet {0:x8}" + ret);
                return;
            }
            //ch: 线路模式 |en:Set LineMode as ExposureStartActive
            ret = device.Parameters.SetEnumValueByString("LineMode", "Strobe");
            if (ret != MvError.MV_OK)
            {
                LogDebug.Error("Set LineSource fail! nRet {0:x8}" + ret);
                return;
            }
            //ch: 输出源选择曝光开始 |en:Set LineSource as ExposureStartActive
            ret = device.Parameters.SetEnumValueByString("LineSource", "SoftTriggerActive");
            if (ret != MvError.MV_OK)
            {
                LogDebug.Error("Set LineSource fail! nRet {0:x8}" + ret);
                return;
            }
            // ch:开启输出使能| en: Turn on StrobeEnable
            ret = device.Parameters.SetBoolValue("StrobeEnable", true);
            if (ret != MvError.MV_OK)
            {
                LogDebug.Error("Set strobe enable fail! nRet {0:x8}" + ret);
                return;
            }

            // ch:设置输出线路持续时间（us） | en: Set StrobeLineDuration(us)
            ret = device.Parameters.SetIntValue("StrobeLineDuration", 0);
            if (ret != MvError.MV_OK)
            {
                LogDebug.Error("Set line strobe line duration fail! nRet {0:x8}" + ret);
                return;
            }

            // ch:设置输出线路持续时间（us） | en: Set StrobeLineDuration(us)
            ret = device.Parameters.SetIntValue("StrobeLineDelay", 0);
            if (ret != MvError.MV_OK)
            {
                LogDebug.Error("Set line strobe line delay fail! nRet {0:x8}" + ret);
                return;
            }

            // ch:输出线路预延迟（us） | en: Set StrobeLinePreDelay(us)
            ret = device.Parameters.SetIntValue("StrobeLinePreDelay", 0);
            if (ret != MvError.MV_OK)
            {
                LogDebug.Error("Set line strobe line pre-delay fail! nRet {0:x8}" + ret);
                return;
            }

            ret = device.Parameters.SetCommandValue("LineTriggerSoftware");
            if (ret != MvError.MV_OK)
            {
                LogDebug.Error("LineTriggerSoftware-fail! nRet {0:x8}" + ret);
                return;
            }
        }
    }
}
