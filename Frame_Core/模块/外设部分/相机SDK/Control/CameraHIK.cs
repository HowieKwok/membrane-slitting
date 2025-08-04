using EHolly;
using HalconDotNet;
using LogTool;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 海康U口相机SDK
    /// </summary>
    public class CameraHIK : CameraObj
    {
        log4net.ILog LogDebug = Log4netRecord.ErrorLog;

        #region field

        private bool connStatus = false;
        public bool ConnStatus
        {
            get
            {
                if (m_pOperator != null)
                    connStatus = m_pOperator.GetCameraStatus();
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

        public override QueuePackage<ImageInfo> QueueGrabImage
        {
            get { return Queue_GrabImage; }
        }
        QueuePackage<ImageInfo> Queue_GrabImage = new QueuePackage<ImageInfo>();//采集到的图像进入队列
        //帧号
        public uint FrameNum { set; get; }
        //帧数
        public uint FrameCounter { set; get; }
        //触发计数
        public uint TriggerIndex { set; get; }
        bool isGrab = false;
        #endregion

        #region member
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        CameraOperator m_pOperator;
        MyCamera.MV_CC_DEVICE_INFO deviceInfo = new MyCamera.MV_CC_DEVICE_INFO();
        MyCamera.cbOutputdelegate ImageCallback;
        MyCamera.cbOutputExdelegate ImageCallbackEX;
        HObject image;
        bool bo_jisuan = false;
        string CameraSerial = "";
        CameraType myCameraType;
        int ListNum = 0;
        #endregion

        #region function
        public CameraHIK(string CameraSerial, CameraType cameraType)
        {
            this.CameraSerial = CameraSerial;
            myCameraType = cameraType;
            HOperatorSet.GenEmptyObj(out image);
            bo_jisuan = false;
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_pOperator = new CameraOperator();
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsCameraStatue
        {
            get
            {
                return isGrab;
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


        /// <summary>
        /// 初始化相机构造函数
        /// </summary>
        public override bool InitCam(/*int InitType*/)
        {
            try
            {
                int nRet;
                bool CamExist = false;
                /*创建设备列表*/
                nRet = CameraOperator.EnumDevices(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
                if (0 != nRet)
                {
                    LogDebug.Error("EnumDevices Failed!");
                    return false;
                }
                else
                {
                    if (m_pDeviceList.nDeviceNum == 0)
                        LogDebug.Info("There is not exist camera in the system!");
                    for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
                    {
                        MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                        switch (myCameraType)
                        {
                            case CameraType.GigeColor:
                            case CameraType.GigeMono:
                                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                                {
                                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));

                                    if (gigeInfo.chSerialNumber == CameraSerial)
                                    {
                                        deviceInfo = device;
                                        LogDebug.Info("Found Hik GigE camera in the system, S/N:" + CameraSerial);
                                        ListNum = i;
                                        CamExist = true;
                                        break;
                                    }
                                }
                                break;
                            case CameraType.USB3Mono:
                            case CameraType.USB3Color:
                                if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                                {
                                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                                    if (usbInfo.chSerialNumber == CameraSerial)
                                    {
                                        deviceInfo = device;
                                        LogDebug.Info("Found Hik U3 camera in the system, S/N:" + CameraSerial);
                                        ListNum = i;
                                        CamExist = true;
                                        break;
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                    }
                    if (!CamExist) LogDebug.Info("Can not found Hik camera in the system, S/N:" + CameraSerial);
                    return CamExist;
                }
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <returns></returns>
        public override bool OpenCam()
        {
            try
            {
                int nRet = MyCamera.MV_OK;
                MyCamera device = new MyCamera();

                MyCamera.MV_CC_DEVICE_INFO test = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[ListNum], typeof(MyCamera.MV_CC_DEVICE_INFO));
                do
                {
                    // ch:创建设备 | en: Create device
                    nRet = device.MV_CC_CreateDevice_NET(ref deviceInfo);
                    if (MyCamera.MV_OK != nRet)
                    {
                        LogDebug.Error(string.Format("Create device failed:{0:x8}", nRet) + ", S / N:" + CameraSerial);
                        break;
                    }
                } while (false);

                nRet = -1;
                ////打开设备
                if (myCameraType == CameraType.GigeMono || myCameraType == CameraType.GigeColor)
                    nRet = m_pOperator.Open(ref deviceInfo);
                else
                    nRet = m_pOperator.OpenFly(ref deviceInfo);
                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error(string.Format("Open device failed:{0:x8}", nRet) + ", S / N:" + CameraSerial);
                    return false;
                }
                //设置心跳超时时间
                nRet = m_pOperator.SetHeartBeatTimeout(3000);
                ////注册回调函数
                if (myCameraType == CameraType.GigeMono || myCameraType == CameraType.USB3Mono)
                {
                    ImageCallback = new MyCamera.cbOutputdelegate(get_Image);
                    nRet = m_pOperator.RegisterImageCallBack(ImageCallback, IntPtr.Zero);
                    if (CameraOperator.CO_OK != nRet)
                    {
                        LogDebug.Error(string.Format("RegisterImageCallBack failed:{0:x8}", nRet) + ", S / N:" + CameraSerial);
                        return false;
                    }
                }
                else
                {
                    ImageCallbackEX = new MyCamera.cbOutputExdelegate(get_Image_EX);
                    nRet = m_pOperator.RegisterImageCallBackEX_RGB(ImageCallbackEX, IntPtr.Zero);
                    if (CameraOperator.CO_OK != nRet)
                    {
                        LogDebug.Error(string.Format("RegisterImageCallBack failed:{0:x8}", nRet) + ", S / N:" + CameraSerial);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }

        }

        /// <summary>
        /// 取像回调
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pFrameInfo"></param>
        /// <param name="pUser"></param>
        private void get_Image(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser)
        {
            try
            {
                Log4netRecord.CamLog.Debug($"相机{CamID}进入回调" + "-" + (GrabCount + 1).ToString());
                if (!pFrameInfo.enPixelType.ToString().Contains("Packed"))
                {
                    if (pFrameInfo.enPixelType.ToString().Contains("Mono8") || pFrameInfo.enPixelType.ToString().Contains("Mono10") || pFrameInfo.enPixelType.ToString().Contains("Mono12"))
                    {
                        //image.Dispose();
                        if (pFrameInfo.enPixelType.ToString().Contains("Mono8"))
                            HOperatorSet.GenImage1(out image, "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, pData);
                        else if (pFrameInfo.enPixelType.ToString().Contains("Mono10") || pFrameInfo.enPixelType.ToString().Contains("Mono12"))
                            HOperatorSet.GenImage1(out image, "uint2", pFrameInfo.nWidth, pFrameInfo.nHeight, pData);


                        HTuple w = new HTuple(), h = new HTuple();
                        HOperatorSet.GetImageSize(image, out w, out h);
                        if (w != 0 && h != 0)
                        {
                            GrabCount++;
                            Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = pFrameInfo.nWidth, imageHeight = pFrameInfo.nHeight });//图像进队 
                            bo_jisuan = true;
                        }
                    }
                    else
                    {
                        LogDebug.Error("Else PixelType：" + pFrameInfo.enPixelType.ToString() + ", S/N:" + CameraSerial);
                        bo_jisuan = false;
                        return;
                    }
                }
                else
                {
                    LogDebug.Error("Else PixelType：" + pFrameInfo.enPixelType.ToString() + ", S/N:" + CameraSerial);
                    bo_jisuan = false;
                    return;
                }
                CamCallbackEvent?.Invoke();
                Log4netRecord.CamLog.Debug($"相机{CamID}回调结束" + "-" + GrabCount.ToString());
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return;
            }
        }

        /// <summary>
        /// 取像回调
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pFrameInfo"></param>
        /// <param name="pUser"></param>
        private void get_Image_EX(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            try
            {
                Log4netRecord.CamLog.Debug($"相机{CamID}进入回调" + "-" + (GrabCount + 1).ToString());
                if (pFrameInfo.enPixelType.ToString().Contains("RGB") || pFrameInfo.enPixelType.ToString().Contains("BGR"))
                {
                    //image.Dispose();
                    HOperatorSet.GenImageInterleaved(out image, pData, "rgb", pFrameInfo.nWidth, pFrameInfo.nHeight, -1, "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, 0, 0, -1, 0);
                    HTuple w = new HTuple(), h = new HTuple();
                    HOperatorSet.GetImageSize(image, out w, out h);
                    if (w != 0 && h != 0)
                    {
                        GrabCount++;
                        Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = pFrameInfo.nWidth, imageHeight = pFrameInfo.nHeight });//图像进队 
                        bo_jisuan = true;
                    }
                }
                else
                {
                    LogDebug.Error("Else PixelType：" + pFrameInfo.enPixelType.ToString() + ", S/N:" + CameraSerial);
                    bo_jisuan = false;
                    return;
                }
                CamCallbackEvent?.Invoke();
                Log4netRecord.CamLog.Debug($"相机{CamID}回调结束" + "-" + GrabCount.ToString());
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return;
            }
        }

        /// <summary>
        /// 开始采集图像
        /// </summary>
        /// <returns></returns>
        public override bool StartGrabImage()
        {
            int nRet;
            //开始采集
            nRet = m_pOperator.StartGrabbing();
            if (MyCamera.MV_OK != nRet)
            {
                LogDebug.Error("StartGrabbing Failed, S / N:" + CameraSerial);
                return false;
            }
            else
            {
                isGrab = true;
                return true;
            }

        }
        /// <summary>
        /// 停止采集
        /// </summary>
        /// <returns></returns>
        public override bool StopGrabImage()
        {
            if (!ConnStatus)
            {
                return false;
            }
            int nRet;
            try
            {
                nRet = m_pOperator.StopGrabbing(); // 停止相机采集

                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error("StopGrabbing Failed, S / N:" + CameraSerial);
                    return false;
                }
                else
                {
                    isGrab = false;
                    return true;
                }

            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
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
            int nRet;
            try
            {
                nRet = m_pOperator.Close();
                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error("Close camera Failed, S / N:" + CameraSerial);
                }
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
            }
        }

        /// <summary>
        /// 软件触发一次
        /// </summary>
        /// <returns></returns>
        public override bool SoftwareTrigger()
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                int nRet;
                // ch:触发命令 | en:Trigger command
                nRet = m_pOperator.SetCommandValue("TriggerSoftware");
                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error("TriggerSoftware failed, S / N:" + CameraSerial);
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

        #region 设置参数
        /// <summary>
        /// 设置触发模式，mode = 0 设置内触发模式；mode = 1 设置外触发模式
        /// </summary>
        /// <returns></returns>
        public override bool SetTriggerMode(int mode)//设置触发模式，mode=0关闭触发模式；mode=1设置硬触发模式
        {

            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                int nRet = m_pOperator.SetEnumValue("TriggerMode", Convert.ToUInt16(mode));//TriggerMode  设置触发模式
                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error("Set \"TriggerMode\" failed, S / N:" + CameraSerial);
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

        /// <summary>
        /// 设置触发源  0 - Line0，4 - Counter， 7 - Software;
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool SetTriggerSource(int mode)
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                uint itemp = Convert.ToUInt16(mode);
                int nRet = m_pOperator.SetEnumValue("TriggerSource", itemp);//TriggerMode  设置触发源，0为Line0，3为Line2，4为Counter0，7为Software
                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error("Set \"TriggerSource\" failed, S / N:" + CameraSerial);
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

        //public bool SetGrabState(bool bStart)
        //{
        //    if (!ConnStatus)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        if (bStart)
        //        {
        //            int nRet = m_pOperator.SetCommandValue("AcquisitionStart");
        //            if (MyCamera.MV_OK != nRet)
        //            {
        //                LogDebug.Error("Set \"AcquisitionStart\" failed, S / N:" + CameraSerial);
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            int nRet = m_pOperator.SetCommandValue("AcquisitionStop");
        //            if (MyCamera.MV_OK != nRet)
        //            {
        //                LogDebug.Error("Set \"AcquisitionStop\" failed, S / N:" + CameraSerial);
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogDebug.Error(ex);
        //        return false;
        //    }

        //}

        /// <summary>
        /// 设置相机曝光时间参数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="value"></param>
        public override bool SetCameraExposure(float exptime)
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                int nRet = m_pOperator.SetExposureTime(exptime);
                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error("Set \"ExposureTime\" failed, S / N:" + CameraSerial);
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

        /// <summary>
        /// 获取相机增益
        /// </summary>
        public override bool SetGain(float gain)
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                int nRet = m_pOperator.SetGain(gain);
                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error("Set \"Gain\" failed, S / N:" + CameraSerial);
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

        public bool SetTriggerDelayTime(float exptime)
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                int nRet = m_pOperator.SetTriggerDelayTime(exptime);
                if (MyCamera.MV_OK != nRet)
                {
                    LogDebug.Error("Set \"TriggerDelayTime\" failed, S / N:" + CameraSerial);
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
        #endregion

        #region 读取参数
        /// <summary>
        /// 获取MER触发源或触发模式
        /// </summary>
        /// <param name="strFeatureName">输入需要获得的对象</param>
        /// <param name="enumEntryList">ref 对象的枚举</param>
        /// <returns>返回当前值</returns>
        public override string GetCammerMode(string strFeatureName, ref List<string> enumEntryList)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取MER相机增益、曝光时间
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override float GetCameraExposure()
        {
            if (!ConnStatus)
            {
                return 0;
            }
            try
            {
                return m_pOperator.GetExposureTime();
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// 读取相机增益
        /// </summary>
        public override float GetGain()
        {
            try
            {
                if (!ConnStatus)
                {
                    return 0;
                }
                try
                {
                    return m_pOperator.GetGain();
                }
                catch (Exception ex)
                {
                    LogDebug.Error(ex);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return 0;
            }
        }

        /// <summary>
        /// 读取触发延时
        /// </summary>
        /// <returns></returns>
        public float GetTriggerDelayTime()
        {
            try
            {
                if (!ConnStatus)
                {
                    return 0;
                }
                try
                {
                    return m_pOperator.GetTriggerDelayTime();
                }
                catch (Exception ex)
                {
                    LogDebug.Error(ex);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return 0;
            }
        }

        public float GetTriggerTime()
        {
            try
            {
                if (!ConnStatus)
                {
                    return 0;
                }
                try
                {
                    return m_pOperator.GetTriggerDelayTime();
                }
                catch (Exception ex)
                {
                    LogDebug.Error(ex);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return 0;
            }
        }

        public override void DestroyObjects2()
        {

        }

        public override bool SetGrabState(bool bStart)
        {
            if (!ConnStatus)
            {
                return false;
            }
            try
            {
                if (bStart)
                {
                    int nRet = m_pOperator.SetCommandValue("AcquisitionStart");
                    if (MyCamera.MV_OK != nRet)
                    {
                        LogDebug.Error("Set \"AcquisitionStart\" failed, S / N:" + CameraSerial);
                        return false;
                    }
                }
                else
                {
                    int nRet = m_pOperator.SetCommandValue("AcquisitionStop");
                    if (MyCamera.MV_OK != nRet)
                    {
                        LogDebug.Error("Set \"AcquisitionStop\" failed, S / N:" + CameraSerial);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
                return false;
            }
        }
        #endregion

        #endregion
    }

    public enum CameraType
    {
        GigeMono,
        GigeColor,
        USB3Mono,
        USB3Color,
    }
}
