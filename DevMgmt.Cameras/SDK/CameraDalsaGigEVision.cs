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
using System.Collections;

using LogTool;

namespace DevMgmt.Cameras.SDK
{
    public class CameraDalsaGigEVision : CameraObj
    {
        #region menber
        public bool m_bonline = false;
        private SapAcqDevice L_AcqDevice;
        public SapBuffer L_Buffers;
        public SapAcqDeviceToBuf L_Xfer;
        private SapFlatField L_FlatField;
        // private SapLocation L_ServerLocation;
        public String L_ConfigFileName;
        public String L_ConfigFileName1;
        //   L_Buffers[0].Clear();清除相机缓存
        //  L_Xfer[ID].Grab();开始采集
        //  Freeze();停止采集   
        public string Camera_name = "Xtium-CL_MX4_1";
        SapLocation m_ServerLocation;
        int index = 0;
        public bool bo_jisuan = false;
        HObject image;
     
        #endregion

        #region function
        public CameraDalsaGigEVision(string sapolcation, string ConfigFileName, string c_index)
        {
            index = Convert.ToInt16(c_index); 
            Camera_name = sapolcation;
            L_ConfigFileName = ConfigFileName;

            HOperatorSet.GenEmptyObj(out image);
            bo_jisuan = false;
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
        public virtual new bool IsCameraStatue
        {
            get { return L_Xfer.Grabbing; }
        }
        /// <summary>
        /// 相机连接 返回bool_Cam1Ini
        /// </summary>
        //====================================================================
        //找相机回调函数   
        public void SapCallback_New(object sender, SapServerNotifyEventArgs argsNotify)
        {
            byte[] ServerName = new byte[128];
            byte[] DeviceName = new byte[128];

            //string strDeviceName;
            //string strServerName;


            int iServerIndex = argsNotify.ServerIndex;
            int iEventType = (int)argsNotify.EventType;
            int iDeviceIndex = argsNotify.ResourceIndex;

            string serverNameTemp = "";
            serverNameTemp = SapManager.GetServerName(iServerIndex);
            if (serverNameTemp != "")
            {
                //获取相机名称
                string DeviceNameTemp = "";
                DeviceNameTemp = SapManager.GetResourceName(serverNameTemp, SapManager.ResourceType.AcqDevice, iDeviceIndex);
                string str = "";
                if (DeviceNameTemp != "")
                {

                    if (iEventType == 1)//ServerNew = 1
                    {
                        str = "@@ ServerNew CallBack DeviceCount " + SapManager.GetServerCount() + " List" +
                       "  iServerIndex = " + iServerIndex +
                       "  DeviceIndex =" + iDeviceIndex +
                       "  DeivceServer =" + serverNameTemp +
                       "  DeviceName =" + DeviceNameTemp;

                    }
                    else if (iEventType == 4)//ServerConnected = 4
                    {
                        str = "@@ ServerConnected CallBack DeviceCount " + SapManager.GetServerCount() + " List" +
                        "  iServerIndex = " + iServerIndex +
                        "  DeviceIndex =" + iDeviceIndex +
                        "  DeivceServer =" + serverNameTemp +
                        "  DeviceName =" + DeviceNameTemp;
                    }
                    else if (iEventType == 2)//ServerDisconnected = 2,
                    {
                        str = "@@ ServerDisconnected CallBack DeviceCount " + SapManager.GetServerCount() + " List" +
                        "  iServerIndex = " + iServerIndex +
                        "  DeviceIndex =" + iDeviceIndex +
                        "  DeivceServer =" + serverNameTemp +
                        "  DeviceName =" + DeviceNameTemp;
                    }
                }

                // this.Invoke(new ListBoxItemAddDelegate(abc), new object[] { str });

            }

        }
        //列举所有相机
        static ArrayList m_vecDeviceName = new ArrayList();
        static ArrayList m_vecServerName = new ArrayList();
        static ArrayList m_vecServerIndex = new ArrayList();
        static int m_iDeviceCount = 0;
        static int m_iServerCount = 0;
        public bool ListAllCams(int iMode)
        {
            //清空数组里内容
            m_vecDeviceName.Clear();
            m_vecServerName.Clear();
            int serverCount = SapManager.GetServerCount();
            for (int i = 0; i < serverCount; i++)
            {
                m_iServerCount++;

                int iAcqDeviceNum = SapManager.GetResourceCount(i, SapManager.ResourceType.AcqDevice);
                int iAcqNum = SapManager.GetResourceCount(i, SapManager.ResourceType.Acq);
                if (iAcqDeviceNum > 0 || iAcqNum > 0)
                {
                    //获取服务器名字
                    string strDeviceName;
                    string strServerName;

                    byte[] bServerName = new byte[128];
                    byte[] bDeviceName = new byte[128];

                    string serverNameTemp = "";
                    serverNameTemp = SapManager.GetServerName(i);
                    if (serverNameTemp != "")
                    {
                        strServerName = serverNameTemp;
                        m_vecServerName.Add(strServerName);
                    }

                    /*遍历该Server下所有的采集卡*/
                    int iResourceNumTemp = SapManager.GetResourceCount(serverNameTemp, SapManager.ResourceType.Acq);
                    for (int iDeviceIndex = 0; iDeviceIndex < iResourceNumTemp; iDeviceIndex++)
                    {
                        /*获取相机名称*/
                        string DeviceNameTemp = "";
                        DeviceNameTemp = SapManager.GetResourceName(serverNameTemp, SapManager.ResourceType.Acq, iDeviceIndex);
                        if (DeviceNameTemp != "")
                        {
                            strDeviceName = DeviceNameTemp;
                            m_vecDeviceName.Add(strDeviceName);
                            m_vecServerIndex.Add(iDeviceIndex);
                        }
                        m_iDeviceCount++;
                    }

                    //遍历该server下所有的设备
                    int iDeviceNumTemp = SapManager.GetResourceCount(serverNameTemp, SapManager.ResourceType.AcqDevice);
                    for (int iDeviceIndex = 0; iDeviceIndex < iDeviceNumTemp; iDeviceIndex++)
                    {

                        //获取相机名称
                        string DeviceNameTemp = "";
                        DeviceNameTemp = SapManager.GetResourceName(serverNameTemp, SapManager.ResourceType.AcqDevice, iDeviceIndex);
                        if (DeviceNameTemp != "")
                        {
                            strDeviceName = DeviceNameTemp;
                            m_vecDeviceName.Add(strDeviceName);
                            m_vecServerIndex.Add(iDeviceIndex);
                        }
                    }
                }
            }
            if (m_vecDeviceName.Capacity == 0)
            {
                System.Windows.Forms.MessageBox.Show("没有发现相机！");
                return false;
            }
            int iCamNum = m_vecServerName.Count;   //将设备数量传给全局变量

            return true;
        }
        private void find_camera()
        {
            SapManager.DetectAllServers(SapManager.DetectServerType.All);
            //ServerNotify
            SapManager.ServerEventType = SapManager.EventType.ServerNew | SapManager.EventType.ServerConnected | SapManager.EventType.ServerDisconnected;
            SapManager.ServerNotify += new SapServerNotifyHandler(SapCallback_New);
            SapManager.ServerNotifyContext = this;
            ///SapManager::RegisterServerCallback(SapManager::EventServerNew|SapManager::EventServerConnected|SapManager::EventServerDisconnected,SapCallback_New,this);
            ListAllCams(0);
        }
        /// <summary>
        /// 初始化相机构造函数
        /// </summary>
        public override bool InitCam()
        {
            find_camera();
            
            for (int iDeviceIndex = 0; iDeviceIndex < m_vecDeviceName.Capacity; iDeviceIndex++)
            {
                if (string.Equals(Camera_name, m_vecDeviceName[iDeviceIndex]))
                    Camera_name = (string)m_vecServerName[iDeviceIndex];
            }
            //m_ServerLocation = new SapLocation(Camera_name, index);
            m_ServerLocation = new SapLocation(Camera_name, (int)m_vecServerIndex[index]);
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
                L_AcqDevice = new SapAcqDevice(m_ServerLocation, L_ConfigFileName);
                if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
                    L_Buffers = new SapBufferWithTrash(2, L_AcqDevice, SapBuffer.MemoryType.ScatterGather);
                else
                    L_Buffers = new SapBufferWithTrash(2, L_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);
                L_Xfer = new SapAcqDeviceToBuf(L_AcqDevice, L_Buffers);


                L_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
                L_Xfer.XferNotify += new SapXferNotifyHandler(Lxfer_XferNotify);
                L_Xfer.XferNotifyContext = this;

                if (!LCreateObjects(true, true))
                {
                    LDisposeObjects();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;

            }
            m_bonline = true;
            return true;
        }

        public new int GrabCount = 0;
        void Lxfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {

            try
            {
                CameraDalsaGigEVision GrabDlg = argsNotify.Context as CameraDalsaGigEVision;

                if (argsNotify.Trash) { }
                else
                {
                    IntPtr BufAddr;
                    // camera_init_done = 0;
                    GrabDlg.L_Buffers.GetAddress(out BufAddr);
                    HTuple H_sizeX = (HTuple)GrabDlg.L_Buffers.XferParams.Width;
                    HTuple H_sizeY = (HTuple)GrabDlg.L_Buffers.XferParams.Height;
                    HOperatorSet.GenImage1(out GrabDlg.image, "byte", H_sizeX, H_sizeY, BufAddr);
                    bo_jisuan = true;

                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }
        private void LDisposeObjects()
        {

            if (L_FlatField != null)
            { L_FlatField.Dispose(); L_FlatField = null; }
            if (L_Xfer != null)
            { L_Xfer.Dispose(); L_Xfer = null; }

            if (L_Buffers != null)
            { L_Buffers.Dispose(); L_Buffers = null; }
            if (L_AcqDevice != null)
            { L_AcqDevice.Dispose(); L_AcqDevice = null; }
        }
        private bool LCreateObjects(bool createAcq, bool createFlatField)
        {
            if (createAcq)
            {
                if (L_AcqDevice != null && !L_AcqDevice.Initialized)
                {
                    if (!L_AcqDevice.Create())
                    {

                        return false;
                    }
                }
            }
            if (L_Buffers != null && !L_Buffers.Initialized)
            {
                if (L_Buffers.Create() == false)
                {

                    return false;
                }
                L_Buffers.Clear();
            }
            if (L_Xfer != null && !L_Xfer.Initialized)
            {
                if (L_Xfer.Create() == false)
                {

                    return false;
                }
            }
            return true;
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
                return L_Xfer.Grab();
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
                L_Xfer.Freeze(); // 停止相机采集
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
