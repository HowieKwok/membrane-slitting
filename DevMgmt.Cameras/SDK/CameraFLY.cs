using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyCapture2Managed;
using HalconDotNet;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

using LogTool;

namespace DevMgmt.Cameras.SDK
{
    public class CameraFly : CameraObj
    {
        #region menber

        public bool m_bonline = false;
        private bool m_grabImages;
        public AutoResetEvent m_grabThreadExited;
        private BackgroundWorker m_grabThread;
        private ManagedImage m_rawImage = new ManagedImage();
        private ManagedImage m_processedImage = new ManagedImage();
        ManagedGigECamera m_camera = new ManagedGigECamera();
        ManagedBusManager busMgr = new ManagedBusManager();
        ManagedPGRGuid guid = new ManagedPGRGuid();


        HObject image;
        bool bo_jisuan = false;
        uint CameraSerial =0;
        #endregion

        #region function
        public CameraFly(string CameraSerial_str)
        {
            try
            {
            CameraSerial = Convert.ToUInt32(CameraSerial_str);
            HOperatorSet.GenEmptyObj(out image);
            bo_jisuan = false;

            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("灰点相机序列号格式错误，请重新设置！");
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
        //public override string IsCameraStatue
        //{
        //    get { return camerastatue; }
        //    set { camerastatue = value; }
        //}

        /// <summary>
        /// 初始化相机构造函数
        /// </summary>
        public override bool InitCam()
        {
           
            m_grabThreadExited = new AutoResetEvent(false);
            try
            {
                guid = busMgr.GetCameraFromSerialNumber(CameraSerial);//根据相机索引号获取相机标识
            }
            catch (FC2Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
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
                m_camera.Connect(guid); //连接相机
            }
            catch (FC2Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            m_bonline = true;
            return true;
        }
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
                m_camera.StartCapture();// 开始相机采集

                m_grabImages = true;

                m_grabThread = new BackgroundWorker();
                m_grabThread.DoWork += new DoWorkEventHandler(GrabLoop);
                m_grabThread.WorkerReportsProgress = true;
                m_grabThread.RunWorkerAsync();
                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
        }
        private unsafe void GrabLoop(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            IntPtr BufAddr;

            while (m_grabImages)
            {
                try
                {
                    m_camera.RetrieveBuffer(m_rawImage);
                }
                catch (FC2Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex);
                    continue;
                }

                lock (this)
                {
                    BufAddr = (IntPtr)m_rawImage.data;
                    HOperatorSet.GenImage1(out image, "byte", (HTuple)m_rawImage.cols,
                                           (HTuple)m_rawImage.rows, BufAddr);
                    bo_jisuan = true;
                    m_grabThreadExited.Set();
                }
                worker.ReportProgress(0);
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

                m_camera.StopCapture(); // 停止相机采集
            }
            catch (FC2Exception ex)
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
        /// 销毁资源
        /// </summary>
        public override void DestroyObjects2()
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
            TriggerMode triggermode = new TriggerMode();
            switch (mode)
            {
                case 0:
                    triggermode.onOff = false;
                    triggermode.mode = 0;
                    triggermode.parameter = 0;
                    triggermode.source = 0;
                    break;
                case 1:
                    triggermode.onOff = true;
                    triggermode.mode = 0;
                    triggermode.parameter = 0;
                    triggermode.source = 0;
                    break;
                case 2:
                    triggermode.onOff = true;
                    triggermode.mode = 5;
                    triggermode.parameter = 10;
                    triggermode.source = 7;
                    break;
                default:
                    break;
            }
            try
            {
                m_camera.SetTriggerMode(triggermode);
            }
            catch (FC2Exception ex)
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
                CameraProperty shutterProperty = m_camera.GetProperty(PropertyType.Shutter);
                float shutter = shutterProperty.absValue;
                return shutter.ToString();
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
            CameraProperty shutterProperty = m_camera.GetProperty(PropertyType.Shutter);
            shutterProperty.absControl = true;
            shutterProperty.autoManualMode = false;
            shutterProperty.absValue = exptime;
            try
            {
                m_camera.SetProperty(shutterProperty);
            }
            catch (FC2Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
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
                CameraProperty gainProperty = m_camera.GetProperty(PropertyType.Gain);
                float gain = gainProperty.absValue;
                return gain;
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
            CameraProperty gainProperty = m_camera.GetProperty(PropertyType.Gain);
            gainProperty.absControl = true;
            gainProperty.autoManualMode = false;
            gainProperty.absValue = gain;
            try
            {
                m_camera.SetProperty(gainProperty);
            }
            catch (FC2Exception ex)
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

        #endregion
    }
}
