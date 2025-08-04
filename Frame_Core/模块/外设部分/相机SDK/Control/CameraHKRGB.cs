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
    public class CameraHKRGB : CameraObj
    {
        #region menber
        public bool m_bonline = false;
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        CameraOperator m_pOperator;

        QueuePackage<ImageInfo> Queue_GrabImage = new QueuePackage<ImageInfo>();//采集到的图像进入队列

        HObject image;
        bool bo_jisuan = false;
        string CameraSerial = "";
        string PC_IP = "";
        string Camera_IP = "";
        #endregion

        #region function
        public CameraHKRGB(string CameraSerial_str, string PC_IP1, string Camera_IP1)
        {
            CameraSerial = CameraSerial_str;
            PC_IP = PC_IP1;
            Camera_IP = Camera_IP1;
            HOperatorSet.GenEmptyObj(out image);
            bo_jisuan = false;
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_pOperator = new CameraOperator();
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
        public override bool InitCam()
        {
            int nRet;
            /*创建设备列表*/
            nRet = CameraOperator.EnumDevices(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
            if (0 != nRet)
            {
                return false;
            }
            else
                return true;
        }

        public override QueuePackage<ImageInfo> QueueGrabImage
        {
            get { return Queue_GrabImage; }
        }

        MyCamera.cbOutputExdelegate ImageCallbackEX;
        //帧号
        public uint FrameNum { set; get; }
        //帧数
        public uint FrameCounter { set; get; }
        //触发计数
        public uint TriggerIndex { set; get; }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <returns></returns>
        public override bool OpenCam()
        {
            MyCamera.MV_CC_DEVICE_INFO stDevInfo = new MyCamera.MV_CC_DEVICE_INFO();
            stDevInfo.nTLayerType = MyCamera.MV_GIGE_DEVICE;
            MyCamera.MV_GIGE_DEVICE_INFO stGigEDev = new MyCamera.MV_GIGE_DEVICE_INFO();
            int nRet = MyCamera.MV_OK;
            MyCamera device = new MyCamera();
            do
            {
                var parts = Camera_IP.Split('.');
                try
                {
                    int nIp1 = Convert.ToInt32(parts[0]);
                    int nIp2 = Convert.ToInt32(parts[1]);
                    int nIp3 = Convert.ToInt32(parts[2]);
                    int nIp4 = Convert.ToInt32(parts[3]);
                    stGigEDev.nCurrentIp = (uint)((nIp1 << 24) | (nIp2 << 16) | (nIp3 << 8) | nIp4);

                    parts = PC_IP.Split('.');
                    nIp1 = Convert.ToInt32(parts[0]);
                    nIp2 = Convert.ToInt32(parts[1]);
                    nIp3 = Convert.ToInt32(parts[2]);
                    nIp4 = Convert.ToInt32(parts[3]);
                    stGigEDev.nNetExport = (uint)((nIp1 << 24) | (nIp2 << 16) | (nIp3 << 8) | nIp4);
                }
                catch
                {
                    Console.Write("Invalid Input!\n");
                    break;
                }
                stGigEDev.chSerialNumber = CameraSerial;
                IntPtr stGigeInfoPtr = Marshal.AllocHGlobal(216);
                Marshal.StructureToPtr(stGigEDev, stGigeInfoPtr, false);
                stDevInfo.SpecialInfo.stGigEInfo = new Byte[540];
                Marshal.Copy(stGigeInfoPtr, stDevInfo.SpecialInfo.stGigEInfo, 0, 540);
                Marshal.FreeHGlobal(stGigeInfoPtr);
                // ch:创建设备 | en: Create device
                nRet = device.MV_CC_CreateDevice_NET(ref stDevInfo);
                if (MyCamera.MV_OK != nRet)
                {
                    Console.WriteLine("Create device failed:{0:x8}", nRet);
                    break;
                }
            } while (false);

            nRet = -1;
            ////打开设备
            nRet = m_pOperator.Open(ref stDevInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }
            ////注册回调函数
            ImageCallbackEX = new MyCamera.cbOutputExdelegate(get_ImageEX_RGB);
            nRet = m_pOperator.RegisterImageCallBackEX_RGB(ImageCallbackEX, IntPtr.Zero);
            if (CameraOperator.CO_OK != nRet)
            {
                return false;
            }
            m_bonline = true;
            return true;
        }

        public void get_ImageEX_RGB(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            try
            {               
                if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed)
                {
                    GrabCount++;
                    HOperatorSet.GenImageInterleaved(out image, pData, "rgb", pFrameInfo.nWidth, pFrameInfo.nHeight, -1, "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, 0, 0, -1, 0);
                    Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = pFrameInfo.nWidth, imageHeight = pFrameInfo.nHeight });//图像进队

                    bo_jisuan = true;

                    //记录相机采集的信息
                    FrameNum = pFrameInfo.nFrameNum;
                    FrameCounter = pFrameInfo.nFrameCounter;
                    TriggerIndex = pFrameInfo.nTriggerIndex;

                    CamCallbackEvent?.Invoke();
                 
                    // DebugLog.MVSlog("FrameNum:" + FrameNum.ToString() + " FrameCounter:" + FrameCounter.ToString() + " TriggerIndex:" + TriggerIndex.ToString());
                }
                else
                {
                    bo_jisuan = false;
                    return;
                }
                Log4netRecord.CamLog.Debug(CamID.ToString() + "-" + GrabCount.ToString());
            }
            catch (Exception ex)
            {
               Log4netRecord.ErrorLog.Debug(ex);
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
            //m_grabImages = true;
            nRet = m_pOperator.StartGrabbing();
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        /// <returns></returns>
        public override bool StopGrabImage()
        {
            if (!m_bonline)
            {
                return false;
            }
            try
            {
                // m_grabImages = false;
                m_pOperator.StopGrabbing(); // 停止相机采集
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
                m_pOperator.Close();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }
        public override void DestroyObjects2()
        {
            try
            {
                m_pOperator.Close();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        /// <summary>
        /// 设置触发模式，mode = 0 设置内触发模式；mode = 1 设置外触发模式
        /// </summary>
        /// <returns></returns>
        public override bool SetTriggerMode(int mode)
        {

            if (!m_bonline)
            {
                return false;
            }
            try
            {
                m_pOperator.SetEnumValue("TriggerMode", Convert.ToUInt16(mode));//TriggerMode  关闭外触发模式，使用内触发连续采集
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// 设置触发源  0 - Line0，4 - Counter， 7 - Software;
        ///// </summary>
        ///// <param name="mode"></param>
        ///// <returns></returns>
        //public override bool SetTriggerSource(int mode)
        //{
        //    if (!m_bonline)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        uint itemp = Convert.ToUInt16(mode);
        //        m_pOperator.SetEnumValue("TriggerSource", itemp);//TriggerMode  设置触发源，0为Line0，3为Line2，4为Counter0，7为Software
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return false;
        //    }
        //    return true;
        //}


        /// <summary>
        /// 软件触发一次
        /// </summary>
        /// <returns></returns>
        public override bool SoftwareTrigger()
        {
            if (!m_bonline)
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
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
        }
      
        //public override bool SetGrabState(bool bStart)
        //{
        //    if (!m_bonline)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        if (bStart)
        //        {
        //            m_pOperator.SetCommandValue("AcquisitionStart");
        //        }
        //        else
        //        {
        //            m_pOperator.SetCommandValue("AcquisitionStop");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return false;
        //    }
        //    return true;
        //}

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
                Log4netRecord.ErrorLog.Debug(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取MER相机增益、曝光时间
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        //public override string GetCameraExposure(string message)
        //{
        //    if (!m_bonline)
        //    {
        //        return "0";
        //    }
        //    try
        //    {
        //        return m_pOperator.GetExposureTime().ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return "0";
        //    }
        //}

        /// <summary>
        /// 设置相机曝光时间参数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="value"></param>
        //public override bool SetCameraExposure(float exptime)
        //{
        //    if (!m_bonline)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        m_pOperator.SetExposureTime(exptime);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 设置相机增益
        /// </summary>
        //public override float GetGain()
        //{
        //    try
        //    {

        //        if (!m_bonline)
        //        {
        //            return 0;
        //        }
        //        try
        //        {
        //            return m_pOperator.GetGain();
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4netRecord.ErrorLog.Debug(ex);
        //            return 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return 0;
        //    }
        //}
        /// <summary>
        /// 获取相机增益
        /// </summary>
        public override bool SetGain(float gain)
        {
            if (!m_bonline)
            {
                return false;
            }

            try
            {
                m_pOperator.SetGain(gain);
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
            throw new NotImplementedException();
        }



        //public override bool SetTriggerDelayTime(float exptime)
        //{
        //    if (!m_bonline)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        m_pOperator.SetTriggerDelayTime(exptime);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return false;
        //    }
        //    return true;
        //}

        //public override float GetTriggerDelayTime()
        //{
        //    try
        //    {

        //        if (!m_bonline)
        //        {
        //            return 0;
        //        }
        //        try
        //        {
        //            return m_pOperator.GetTriggerDelayTime();
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4netRecord.ErrorLog.Debug(ex);
        //            return 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return 0;
        //    }
        //}

        //public override float GetTriggerTime()
        //{
        //    try
        //    {

        //        if (!m_bonline)
        //        {
        //            return 0;
        //        }
        //        try
        //        {
        //            return m_pOperator.GetTriggerDelayTime();
        //        }
        //        catch (Exception ex)
        //        {
        //            Log4netRecord.ErrorLog.Debug(ex);
        //            return 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return 0;
        //    }
        //}

        #endregion
    }
}
