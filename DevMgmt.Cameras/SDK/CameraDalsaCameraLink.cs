using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using DALSA.SaperaLT.SapClassBasic;
using System.Windows.Forms;

using LogTool;
using Basler.Pylon;
using FrameCommunity;
using DevMgmt.Cameras.Models;
using FrameCommunity.Data;

namespace DevMgmt.Cameras.SDK
{
    public class CameraDalsaCameraLink : CameraObj
    {
        #region menber

        public bool m_bonline = false;
        private bool m_grabImages;//采集图像标志
        public HObject ImageGrab1, ImageGrab2;//, ImageGrab3;
        public QueuePackage<ImageInfo> Queue_GrabImage = new QueuePackage<ImageInfo>();//采集到的图像进入队列
        private SapAcquisition m_Acquisition;
        public SapAcqToBuf m_Xfer;
        public SapBuffer m_Buffers;
        public String m_ConfigFileName;
        public string Camera_name = "Xtium-CL_MX4_1";
        SapLocation m_ServerLocation;
        int index = 0;
        public bool bo_jisuan = false;
        HObject image;

        #endregion

        #region function
        public CameraDalsaCameraLink(string sapolcation, string ConfigFileName, int c_index)
        {
            try
            {

                index = Convert.ToInt16(c_index);
                Camera_name = sapolcation;
                m_ConfigFileName = ConfigFileName;

                HOperatorSet.GenEmptyObj(out image);
                bo_jisuan = false;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("线扫相机基础设置参数有误！");
            }
        }



        public override HObject Image
        {
            get { return image; }
        }
        public override QueuePackage<ImageInfo> QueueGrabImage
        {
            get { return Queue_GrabImage; }
            //set { Queue_GrabImage = value; }   
        }
        public override bool IsExist
        {

            get { return bo_jisuan; }
            set { bo_jisuan = value; }
        }
        public override bool IsCameraStatue
        {
            get
            {
                bool b = false;
                try
                {
                    if (m_Xfer != null)
                        b = m_Xfer.Grabbing;
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
                    if (m_Xfer != null)
                        b = m_Xfer.Initialized;
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
            m_ServerLocation = new SapLocation(Camera_name, index);
            return true;
        }
        /// <summary>
        /// 打开相机
        /// </summary>
        /// <returns></returns>
        public override bool OpenCam()
        {
            try
            {
                DestroyObjects();//重连时销毁
                DisposeObjects();
                m_Acquisition = new SapAcquisition(m_ServerLocation, m_ConfigFileName);
                if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
                    m_Buffers = new SapBufferWithTrash(4, m_Acquisition, SapBuffer.MemoryType.ScatterGather);
                else
                    m_Buffers = new SapBufferWithTrash(4, m_Acquisition, SapBuffer.MemoryType.ScatterGatherPhysical);
                m_Xfer = new SapAcqToBuf(m_Acquisition, m_Buffers);

                m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
                m_Xfer.XferNotify += new SapXferNotifyHandler(xfer_XferNotify);
                m_Xfer.XferNotifyContext = this;



                if (!CreateObjects())
                {
                    DisposeObjects();
                    return false;
                }
                m_bonline = true;

                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
        }

        Task task = null;
        //private delegate void DisplayFrameAcquired(int number, bool trash);
        /// <summary>
        /// 回调获取缓存图片 midd_image
        /// </summary>
        /// <param name="message"></param>
        /// <param name="n"></param>
        /// <param name="driver"></param>
        void xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {
            try
            {
                Log4netRecord.CamLog.Debug($"相机{CamID}回调开始" + "-" + GrabCount + 1.ToString());
                CamCallbackStartEvent?.Invoke(GrabCount + 1);
                if (task == null || (task != null && task.IsCompleted))
                {
                    if (!m_grabImages)
                    {
                        Log4netRecord.CamLog.Debug($"相机{CamID}Abort");
                        return;
                    }
                    task = Task.Run(() =>
                    {
                        try
                        {
                            CameraDalsaCameraLink GrabDlg = argsNotify.Context as CameraDalsaCameraLink;

                            if (!argsNotify.Trash)
                            {
                                IntPtr BufAddr;

                                GrabDlg.m_Buffers.GetAddress(out BufAddr);
                                HTuple Xsize = (HTuple)GrabDlg.m_Buffers.XferParams.Width;
                                HTuple Ysize = (HTuple)GrabDlg.m_Buffers.XferParams.Height;
                                // GrabDlg.midd_image.Dispose();
                                HOperatorSet.GenImage1(out GrabDlg.image, "byte", Xsize, Ysize, BufAddr);

                                //队列入队时只是存地址，需开辟多个图像空间内存
                                GrabDlg.GrabCount++;
                                if (GrabDlg.GrabCount % 2 == 0)
                                {
                                    //HOperatorSet.CopyImage(GrabDlg.image, out GrabDlg.ImageGrab1);
                                    GrabDlg.Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = GrabDlg.Image, image_ID = GrabDlg.GrabCount, imageWidth = Xsize, imageHeight = Ysize });//图像进队

                                    //Form1.WriteCameraGrabLog("xfer_XferNotify Grab0回调采图序号: " + GrabDlg.imageCount.ToString());
                                }
                                else// if (GrabDlg.GrabCount == 2)
                                {
                                    //HOperatorSet.CopyImage(GrabDlg.image, out GrabDlg.ImageGrab2);
                                    GrabDlg.Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = GrabDlg.Image, image_ID = GrabDlg.GrabCount, imageWidth = Xsize, imageHeight = Ysize });//图像进队

                                    //Form1.WriteCameraGrabLog("xfer_XferNotify Grab1回调采图序号: " + GrabDlg.imageCount.ToString());
                                    //GrabDlg.GrabCount = 0;
                                }
                                CamCallbackEndEvent?.Invoke();

                                Log4netRecord.CamLog.Debug($"相机{GrabDlg.CamID}回调结束" + "-" + GrabDlg.GrabCount.ToString());
                 
                                GrabDlg.bo_jisuan = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.CamLog.Debug($"相机{CamID}" + ex);
                        }

                    });
                }
                else
                {
                    ImageCallbackAlarm = true;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.CamLog.Debug($"相机{CamID}" + ex);
            }
        }

        private bool CreateObjects()
        {
            // Create acquisition object
            if (m_Acquisition != null && !m_Acquisition.Initialized)
            {
                if (m_Acquisition.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }
            // Create buffer object
            if (m_Buffers != null && !m_Buffers.Initialized)
            {
                if (m_Buffers.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
                m_Buffers.Clear();
            }
            // Create Xfer object
            if (m_Xfer != null && !m_Xfer.Initialized)
            {
                if (m_Xfer.Create() == false)
                {
                    DestroyObjects();
                    return false;
                }
            }
            return true;
        }

        public void DestroyObjects()
        {
            if (m_Xfer != null && m_Xfer.Initialized)
                m_Xfer.Destroy();
            if (m_Buffers != null && m_Buffers.Initialized)
                m_Buffers.Destroy();
            if (m_Acquisition != null && m_Acquisition.Initialized)
                m_Acquisition.Destroy();
        }

        public override void DestroyObjects2()
        {
            if (m_Xfer != null && m_Xfer.Initialized)
                m_Xfer.Destroy();
            if (m_Buffers != null && m_Buffers.Initialized)
                m_Buffers.Destroy();
            if (m_Acquisition != null && m_Acquisition.Initialized)
                m_Acquisition.Destroy();
        }

        public void DisposeObjects()
        {
            if (m_Xfer != null)
            { m_Xfer.Dispose(); m_Xfer = null; }
            if (m_Buffers != null)
            { m_Buffers.Dispose(); m_Buffers = null; }
            if (m_Acquisition != null)
            { m_Acquisition.Dispose(); m_Acquisition = null; }

        }
        //-----------------------
        /// <summary>
        /// 开始采集图像
        /// </summary>
        /// <returns></returns>
        public override bool StartGrabImage()
        {
            if (!m_bonline)
            {
                return false;
            }
            try
            {
                m_grabImages = true;
                return m_Xfer.Grab();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
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
                m_grabImages = false;

                m_Xfer.Freeze(); // 停止相机采集

                m_Xfer.Abort();
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
                ;
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
        public override bool SetTriggerMode(int mode)//设置触发模式，mode=0关闭触发模式；mode=1设置硬触发模式；mode=2设置软触发模式
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

            return true;
        }

        public override bool SetGrabState(bool bStart)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
