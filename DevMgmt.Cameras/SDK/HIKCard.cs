using DevMgmt.Cameras.Models;
using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;
using log4net;
using LogTool;
using MvFGCtrlC.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevMgmt.Cameras.SDK
{
    public class HIKCard : CameraObj
    {
        #region menber
        CSystem system = new CSystem();
        CStream.ImageDelegate DelegateImageCallBack = null;
        List<MV_FG_INTERFACE_INFO> lIfaceInfoList = new List<MV_FG_INTERFACE_INFO>();//接口列表
        List<MV_FG_DEVICE_INFO> lDeviceInfoList = new List<MV_FG_DEVICE_INFO>();//设备列表
        CInterface cInterface = null;
        CDevice cDevice = null;
        CStream cStream = null;
        //HObject image = new HObject();
        //List<CBuffer> cBufferList = new List<CBuffer>();
        List<IntPtr> ptrImageBufferList = new List<IntPtr>();   // 记录申请的内存，便于销毁
        public QueuePackage<ImageInfo> Queue_GrabImage = new QueuePackage<ImageInfo>();//采集到的图像进入队列
        // 保存状态
        bool bIsDeviceOpen = false;
        bool bIsGrabbing = false;
        public bool g_bExit = false;
        private string interfaceID;
        public string InterfaceID
        {
            get { return interfaceID; }
            set { interfaceID = value; }
        }
        private string interfaeConfigFile;
        public string InterfaeConfigFile
        {
            get { return interfaeConfigFile; }
            set { interfaeConfigFile = value; }
        }
        public override QueuePackage<ImageInfo> QueueGrabImage
        {
            get { return Queue_GrabImage; }
        }
        private uint devIndex;
        public uint nDevIndex
        {
            get { return devIndex; }
            set { devIndex = value; }
        }

        public override bool IsCameraStatue
        {
            get { return bIsDeviceOpen; }
        }
        HObject EmptyObj = null;
        #endregion
        //static log4net.ILog  Log4netRecord.CamLog = log4net.LogManager.GetLogger("CamLog");

        #region function
        public HIKCard(string InterfaceID, string InterfaeConfigFile, int c_index)
        {
            try
            {
                int index = Convert.ToInt16(c_index);
                uint nDevIndex = (uint)index;
                this.InterfaceID = InterfaceID;
                this.InterfaeConfigFile = InterfaeConfigFile;
                this.nDevIndex = nDevIndex;
                //HOperatorSet.GenEmptyObj(out image);
                HOperatorSet.GenEmptyObj(out EmptyObj);
            }
            catch
            {
                 Log4netRecord.CamLog.Error("线扫相机基础设置参数有误！");
            }
        }
        public HIKCard(string InterfaceID, string InterfaeConfigFile, uint nDevIndex)
        {
            this.InterfaceID = InterfaceID;
            this.InterfaeConfigFile = InterfaeConfigFile;
            this.nDevIndex = nDevIndex;
            //HOperatorSet.GenEmptyObj(out image);
        }
        public override void DestroyCam()
        {
            if (cDevice == null)
            {
                 Log4netRecord.CamLog.Error("Please open device first");
                return;
            }

            if (bIsGrabbing)
            {
                StopGrabImage();
            }

            int nRet = cDevice.CloseDevice();
            if (CErrorCode.MV_FG_SUCCESS != nRet)
            {
                 Log4netRecord.CamLog.Error("MV_FG_CloseDevice failed, nRet:" + nRet.ToString("X"));
            }
            cDevice = null;
            bIsDeviceOpen = false;
        }
        public override void DestroyObjects2()
        {

        }
        public override bool InitCam()
        {
            return true;
        }
        public override bool OpenCam()
        {
            try
            {
                system.OpenInterfaceByID(InterfaceID, 2, out cInterface);
                if(cInterface == null)
                {
                     Log4netRecord.CamLog.Info("cInterface=null");
                    return false;
                }
                bool hh = true;
                cInterface.UpdateDeviceList(ref hh);
                CParam cParam = new CParam(cInterface);
                cParam.FeatureLoad(InterfaeConfigFile);
                int nRet = cInterface.OpenDevice(nDevIndex, out cDevice);
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                     Log4netRecord.CamLog.Error("OpenDevice failed, nRet: " + nRet.ToString("X"));
                    return false;
                }
                bIsDeviceOpen = true;
                return true;
            }
            catch (Exception ex)
            {
                 Log4netRecord.CamLog.Error(ex.Message);
                return false;
            }
        }
        public override bool SetGain(float gain)
        {
            return true;
        }
        public override bool SetTriggerMode(int mode)
        {
            return true;
        }
        public override bool StartGrabImage()
        {
            try
            {
                if (!bIsDeviceOpen)
                {
                     Log4netRecord.CamLog.Error("Please open device first");
                    return false;
                }
                if(bIsGrabbing)
                {
                    return true;
                }
                // 开始取图前，释放所有申请的内存
                //for (int i = 0; i < ptrImageBufferList.Count; i++)
                //{
                //    if (ptrImageBufferList[i] != IntPtr.Zero)
                //    {
                //        //Marshal.FreeHGlobal(ptrImageBufferList[i]);
                //        CSystem.MV_MEM_Free(ptrImageBufferList[i]);
                //        ptrImageBufferList[i] = IntPtr.Zero;
                //    }
                //}
                //ptrImageBufferList.Clear();

                if (cStream != null)
                {
                    cStream.CloseStream();
                    cStream = null;
                }

                // 打开stream
                uint nNumStream = 0;
                int nRet = cDevice.GetNumStreams(ref nNumStream);
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                     Log4netRecord.CamLog.Error("MV_FG_GetNumStreams failed, nRet:" + nRet.ToString("X"));
                    return false;
                }

                if (nNumStream == 0)
                {
                     Log4netRecord.CamLog.Error("Stream number is 0");
                    return false;
                }

                do
                {
                    nRet = cDevice.OpenStream(0, out cStream);
                    if (CErrorCode.MV_FG_SUCCESS != nRet)
                    {
                         Log4netRecord.CamLog.Error("MV_FG_OpenStream failed, nRet:" + nRet.ToString("X"));
                        break;
                    }
                    // ch:设置SDK内部缓存数量 | en:Set internal buffer number
                    const uint nBufNum = 3;
                    nRet = cStream.SetBufferNum(nBufNum);
                    if (CErrorCode.MV_FG_SUCCESS != nRet)
                    {
                        Console.WriteLine("Set buffer number failed:{0:x8}", nRet);
                        break;
                    }
                    bIsGrabbing = true;
                    // 注册回调函数
                    DelegateImageCallBack = new CStream.ImageDelegate(ImageCallbackFunc);
                    int res = cStream.RegisterImageCallBack(DelegateImageCallBack, IntPtr.Zero);
                    // 获取相机宽高，申请bitmap的内存
                    int nWidth = 0;
                    int nHeight = 0;
                    CParam cDeviceParam = new CParam(cDevice);
                    MV_FG_INTVALUE stIntValue = new MV_FG_INTVALUE();
                    cDeviceParam.GetIntValue("Width", ref stIntValue);
                    nWidth = Convert.ToInt32(stIntValue.nCurValue);
                    cDeviceParam.GetIntValue("Height", ref stIntValue);
                    nHeight = Convert.ToInt32(stIntValue.nCurValue);
                    // 相机开始取图
                    nRet = cStream.StartAcquisition();
                    if (CErrorCode.MV_FG_SUCCESS != nRet)
                    {
                         Log4netRecord.CamLog.Error("MV_FG_StartAcquisition failed, nRet:" + nRet.ToString("X"));
                        break;
                    }

                } while (false);
                if (CErrorCode.MV_FG_SUCCESS != nRet)
                {
                    if (cStream != null)
                    {
                        cStream.CloseStream();
                    }
                    //cBufferList.Clear();
                    bIsGrabbing = false;
                     Log4netRecord.CamLog.Debug($"进入:CErrorCode.MV_FG_SUCCESS != nRet");
                    return false;
                }
                return true;

            }
            catch (Exception ex)
            {
                 Log4netRecord.CamLog.Error(ex.Message);
                return false;
            }
        }

        public void ResetImage()
        {
            // 开始取图前，释放所有申请的内存
            for (int i = 0; i < ptrImageBufferList.Count; i++)
            {
                if (ptrImageBufferList[i] != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrImageBufferList[i]);
                    CSystem.MV_MEM_Free(ptrImageBufferList[i]);
                    ptrImageBufferList[i] = IntPtr.Zero;
                }
            }
            ptrImageBufferList.Clear();
        }

        public override bool StopGrabImage()
        {
            try
            {
                if (cStream == null)
                {
                    //MessageBox.Show("Please start  grabbing first");
                    return false;
                }
                bIsGrabbing = false;
                g_bExit = true;
                Thread.Sleep(1000);
                cStream.StopAcquisition();
                cStream.CloseStream();
                cStream = null;

                return true;
            }
            catch (Exception ex)
            {
                 Log4netRecord.CamLog.Error(ex.Message);
                return false;
            }
        }

        public void ImageCallbackFunc(ref MV_FG_BUFFER_INFO stBufferInfo, IntPtr pUser)
        {
            try
            {
               
                if (null != stBufferInfo.pBuffer && stBufferInfo.pBuffer!= IntPtr.Zero)
                {
                    GrabCount++;
                    Log4netRecord.CamLog.Debug($"相机{CamID}回调开始" + "-" + GrabCount.ToString());
                    //image.Dispose();
                    HOperatorSet.GenImage1(out HObject image, "byte", (int)stBufferInfo.nWidth, (int)stBufferInfo.nHeight, stBufferInfo.pBuffer);
                    HOperatorSet.TestEqualObj(image, EmptyObj, out HTuple isEqual);
                    if (image != null && image.IsInitialized() && isEqual != 1) 
                    {
                        Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = image, image_ID = GrabCount, imageWidth = stBufferInfo.nWidth, imageHeight = stBufferInfo.nHeight });
                        Log4netRecord.CamLog.Debug($"相机{CamID}回调结束" + "-" + GrabCount.ToString());
                        //Log4netRecord.CamLog.Error($"相机--{interfaceID}---取图成功--帧数：{stBufferInfo.nFrameID}");
                    }
                }
                else
                {
                    Log4netRecord.CamLog.Error("回调会空~~或者数据无效");

                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.CamLog.Error(ex.Message);
            }
        }

        public override bool SetGrabState(bool bStart)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
