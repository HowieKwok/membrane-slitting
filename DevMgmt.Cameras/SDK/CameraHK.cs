using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
//using MvCamCtrl.NET;
using MvCamCtrl.NET;

using LogTool;
using Basler.Pylon;
using DevMgmt.Cameras.Models;
using FrameCommunity;
using FrameCommunity.Data;

namespace DevMgmt.Cameras.SDK
{
    public class CameraHK : CameraObj
    {
        #region menber

        public bool m_bonline = false;
        //private bool m_grabImages;
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        //log4net.ILog LogDebug = log4net.LogManager.GetLogger("Debug");//记录日志     
        MyCamera.cbExceptiondelegate pCallBackFunc; //异常回调
        bool m_bGrabbing = false;
        Thread m_hReceiveThread = null;

        public MyCamera device;
        CameraOperator m_pOperator;
        QueuePackage<ImageInfo> Queue_GrabImage = new QueuePackage<ImageInfo>();//采集到的图像进入队列
        HObject image;
        bool bo_jisuan = false;
        string CameraSerial = "";
        string PC_IP = "";
        string Camera_IP = "";
        bool isGrab = false;
        #endregion

        #region function

        public bool reconnection = false;

        public CameraHK(string CameraSerial_str, string PC_IP1, string Camera_IP1)
        {

            device = new MyCamera();
            CameraSerial = CameraSerial_str;
            PC_IP = PC_IP1;
            Camera_IP = Camera_IP1;
            HOperatorSet.GenEmptyObj(out image);
            bo_jisuan = false;
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_pOperator = new CameraOperator();
            if (reconnection)
                pCallBackFunc = new MyCamera.cbExceptiondelegate(cbExceptiondelegate);
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
        public override bool InitCam()
        {
            int nRet;
            //注册异常回调
            if (reconnection)
            {
                nRet = device.MV_CC_RegisterExceptionCallBack_NET(pCallBackFunc, IntPtr.Zero);
                GC.KeepAlive(pCallBackFunc);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
            }



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
                return false;
            }
            stGigEDev.chSerialNumber = CameraSerial;
            IntPtr stGigeInfoPtr = Marshal.AllocHGlobal(216);
            Marshal.StructureToPtr(stGigEDev, stGigeInfoPtr, false);
            stDevInfo.SpecialInfo.stGigEInfo = new Byte[540];
            Marshal.Copy(stGigeInfoPtr, stDevInfo.SpecialInfo.stGigEInfo, 0, 540);
            do//2020.08.08 Ouzj
            {
                // ch:创建设备 | en: Create device
                //nRet = device.MV_CC_CreateDevice_NET(ref stDevInfo);
                //if (MyCamera.MV_OK != nRet)
                //{
                //    Console.WriteLine("Create device failed:{0:x8}", nRet);
                //    break;
                //}
                nRet = -1;
                ////打开设备
                nRet = m_pOperator.Open(ref stDevInfo);
                if (MyCamera.MV_OK != nRet)
                {
                    DestroyCam();
                    Thread.Sleep(500);
                    Log4netRecord.CamLog.Debug("序列号：" + stGigEDev.chSerialNumber.ToString() + ",相机打开失败");
                    //MVSlog("序列号：" + stGigEDev.chSerialNumber.ToString() + ",相机打开失败");
                    return false;
                }
                else
                    error = false;

                Thread.Sleep(10);
            } while (error);



            ////注册回调函数
            //ImageCallback = new MyCamera.cbOutputdelegate(get_Image);
            //nRet = m_pOperator.RegisterImageCallBack(ImageCallback, IntPtr.Zero);
            ImageCallbackEX = new MyCamera.cbOutputExdelegate(get_ImageEX);// 
            nRet = m_pOperator.RegisterImageCallBackEX(ImageCallbackEX, IntPtr.Zero);
            if (CameraOperator.CO_OK != nRet)
            {
                return true;
                //System.Windows.Forms.MessageBox.Show("注册回调失败!");
            }
            m_bonline = true;
            if (reconnection)
            {
                //注册异常回调
                nRet = m_pOperator.RegisterExceptionCallBack(pCallBackFunc, IntPtr.Zero);
                GC.KeepAlive(pCallBackFunc);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
            }


            return true;
        }
        //帧号
        public uint FrameNum { set; get; }
        //帧数
        public uint FrameCounter { set; get; }
        //触发计数
        public uint TriggerIndex { set; get; }
        public void get_Image(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo, IntPtr pUser)//
        {
            try
            {
                if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    HOperatorSet.GenImage1(out image, "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, pData);
                    //Queue_GrabImage.Enqueue(image);//图像进队
                    //记录相机采集的信息
                    //FrameNum = pFrameInfo1.nFrameNum;
                    //FrameCounter = pFrameInfo1.nFrameCounter;
                    //TriggerIndex = pFrameInfo1.nTriggerIndex;

                    //UIRecipe.DLog.MVSlog("FrameNum:" + FrameNum.ToString() + " FrameCounter:" + FrameCounter.ToString() + " TriggerIndex:" + TriggerIndex.ToString());
                    bo_jisuan = true;
                }
                else
                {
                    bo_jisuan = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return;
            }
        }

        public void get_ImageEX(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            try
            {
                if (pFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    GrabCount++;
                    Log4netRecord.CamLog.Debug($"相机{CamID}回调开始" + "-" + GrabCount.ToString());
                    CamCallbackStartEvent?.Invoke(GrabCount );
                    HOperatorSet.GenImage1(out image, "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, pData);
                    Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = Image, image_ID = GrabCount, imageWidth = pFrameInfo.nWidth, imageHeight = pFrameInfo.nHeight });//图像进队

                    bo_jisuan = true;

                    //记录相机采集的信息
                    FrameNum = pFrameInfo.nFrameNum;
                    FrameCounter = pFrameInfo.nFrameCounter;
                    TriggerIndex = pFrameInfo.nTriggerIndex;

                    CamCallbackEndEvent?.Invoke();
                    Log4netRecord.CamLog.Debug($"相机{CamID}回调结束" + "-" + GrabCount.ToString());
                    //MVSlog("帧号:" + FrameNum.ToString() + "帧数:" + FrameCounter.ToString() + " 触发计数:" + TriggerIndex.ToString());
                }              
                else
                {
                    bo_jisuan = false;
                    return;
                }
                //Log4netRecord.CamLog.Debug(CamID.ToString() + "-" + GrabCount.ToString());
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        private static object Logpath = new object();
        /// <summary>海康相机日志记录
        /// 
        /// </summary>

      
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
                //System.Windows.Forms.MessageBox.Show("开始取流失败！");
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
            if (!m_bonline)
            {
                return false;
            }
            try
            {
                // m_grabImages = false;
                m_pOperator.StopGrabbing(); // 停止相机采集
                isGrab = false;
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

        /// <summary>
        /// 销毁资源
        /// </summary>
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
        /// 设置触发模式
        /// </summary>
        /// <returns></returns>
        public override bool SetTriggerMode(int mode)//设置触发模式，mode=0关闭触发模式；mode=1设置硬触发模式；
        {
            if (!m_bonline)
            {
                return false;
            }
            try
            {
                m_pOperator.SetEnumValue("TriggerMode", Convert.ToUInt16(mode));//TriggerMode  设置触发模式
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }
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
        public override string GetCameraExposure(string message)
        {
            try
            {
                if (!m_bonline)
                {
                    return "0";
                }


                return "0";
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return "0";
            }
        }

        /// <summary>
        /// 设置相机曝光时间参数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="value"></param>
        public override bool SetCameraExposure(float exptime)
        {
            if (!m_bonline)
            {
                return false;
            }

            //try
            //{
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    Log4netRecord.ErrorLog.Debug(ex);
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        public override float GetGain()
        {
            try
            {
                if (!m_bonline)
                {
                    return 0;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return 0;
            }
        }
        /// <summary>
        /// 获取相机曝光时间
        /// </summary>
        public override bool SetGain(float gain)
        {
            if (!m_bonline)
            {
                return false;
            }

            try
            {
                ;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }
        bool error = false;
        /// <summary>
        /// 异常回调
        /// </summary>
        /// <param name="nMsgType"></param>
        /// <param name="pUser"></param>
        private void cbExceptiondelegate(uint nMsgType, IntPtr pUser)
        {
            if (nMsgType == MyCamera.MV_EXCEPTION_DEV_DISCONNECT)//2020.08.08 Ouzj
            {
                Log4netRecord.CamLog.Debug("海康相机掉线重连中。。。");
                //MVSlog("海康相机掉线重连中。。。");
                error = true;
                DestroyCam();
                Thread.Sleep(500);
                OpenCam();
                StartGrabImage();
                // ch:打开设备 | en:Open Device
                //while (true)
                //{
                //    DeInitCamera();
                //    Thread.Sleep(500);
                //    // ch:获取选择的设备信息 | en:Get Used Device Info
                //    /*创建设备列表*/
                //    CameraOperator.EnumDevices(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref m_pDeviceList);
                //    MyCamera.MV_CC_DEVICE_INFO stDevInfo = new MyCamera.MV_CC_DEVICE_INFO();
                //    stDevInfo.nTLayerType = MyCamera.MV_GIGE_DEVICE;
                //    int nRet = device.MV_CC_CreateDevice_NET(ref stDevInfo);
                //    if (MyCamera.MV_OK != nRet)
                //    {
                //        Thread.Sleep(500);
                //        continue;
                //    }
                //    nRet = device.MV_CC_OpenDevice_NET();
                //    if (MyCamera.MV_OK != nRet)
                //    {
                //        Thread.Sleep(500);
                //        device.MV_CC_DestroyDevice_NET();
                //        continue;
                //    }
                //    else
                //    {
                //        nRet = InitCamera();
                //        if (MyCamera.MV_OK != nRet)
                //        {
                //            Thread.Sleep(500);
                //            device.MV_CC_DestroyDevice_NET();
                //            continue;
                //        }
                //        break;
                //    }
                //}
                Log4netRecord.CamLog.Debug("海康相机掉线重连成功");
                //MVSlog("海康相机掉线重连成功");
            }
        }

        private void DeInitCamera()
        {
            // ch:标志位设为false | en:Set flag bit false
            m_bGrabbing = false;
            m_hReceiveThread.Join();

            // ch:停止采集 | en:Stop Grabbing
            device.MV_CC_StopGrabbing_NET();


            // ch:关闭设备 | en:Close Device
            device.MV_CC_CloseDevice_NET();
            device.MV_CC_DestroyDevice_NET();

        }
        private int InitCamera()
        {
            int nRet = -1;

            //int  nRet = m_pOperator.RegisterExceptionCallBack(pCallBackFunc, IntPtr.Zero);
            //GC.KeepAlive(pCallBackFunc);
            //if (MyCamera.MV_OK != nRet)
            //{
            //    return nRet;
            //}



            // ch:开始采集 | en:Start Grabbing
            nRet = device.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                m_bGrabbing = false;
                m_hReceiveThread.Join();
                return nRet;
            }

            return MyCamera.MV_OK;
        }

        public override bool SetGrabState(bool bStart)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
