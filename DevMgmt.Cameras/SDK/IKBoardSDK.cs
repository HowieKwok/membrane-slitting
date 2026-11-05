using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IKapBoardClassLibrary;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using LogTool;
using Basler.Pylon;
using FrameCommunity;
using DevMgmt.Cameras.Models;
using FrameCommunity.Data;

namespace DevMgmt.Cameras.SDK
{
    public class IKBoardSDK : CameraObj
    {
        public bool m_bonline = false;       //
        //public int GrabCount = 0;            //采集累计帧数
        public bool bo_jisuan = false;
        //public QueuePackage<HImage> Queue_GrabImage = new QueuePackage<HImage>();//采集到的图像进入队列
        private QueuePackage<ImageInfo> Queue_GrabImage = new QueuePackage<ImageInfo>();//采集到的图像进入队列
        public string Camera_name = "Xtium-CL_MX4_1";       //采集卡名称
        public int m_nCurFrameIndex = 0;    // Current grab frame index 当前抓取帧索引
        public int m_nTotalFrameCount = 5;  // Total frame count 总帧数
        public IntPtr m_hBoard = new IntPtr(-1);
        private HObject ImageGrab1;
        private string m_ConfigFileName;    //配置文件路径
        private int ImgWidth = 4096;        //图像宽度
        private int ImgHeight = 1500;       //图像高度    
        private uint CardPort = 0;          //采集卡端口号
        private bool IsCamera_RGB = false;  //是否彩色相机
        private bool IsStartGrabImage_Status = false;  //是否启动采集图像
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sapolcation"></param>
        /// <param name="ConfigFileName">配置文件名称</param>
        /// <param name="c_index">采集卡端口</param>
        /// <param name="isCamera_RGB">是否彩色相机</param>
        public IKBoardSDK(string sapolcation, string ConfigFileName, int c_index, bool isCamera_RGB)
        {
            try
            {

                CardPort = Convert.ToUInt16(c_index);
                //string Camera_name = sapolcation;
                m_ConfigFileName = ConfigFileName;
                //HOperatorSet.GenEmptyObj(out image);
                bo_jisuan = false;
                IsCamera_RGB = isCamera_RGB;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("线扫相机基础设置参数有误！");
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public override HObject Image
        {
            get { return ImageGrab1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override QueuePackage<ImageInfo> QueueGrabImage
        {
            get { return Queue_GrabImage; }
        }
        /// <summary>
        /// 
        /// </summary>
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
                bool b = false;
                try
                {
                    b = IsStartGrabImage_Status;
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
        /// 释放相机
        /// </summary>
        public override void DestroyCam()
        {
            IsStartGrabImage_Status = false;
            CloseDevice();  //关闭相机

            UnRegisterCallback();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void DestroyObjects2()
        {
            DestroyCam();
        }
        /// <summary>
        /// 相机初始化
        /// </summary>
        /// <returns></returns>
        public override bool InitCam()
        {
            try
            {
                bool result = ConfigureFrameGrabber();
                return result;
            }
            catch (Exception)
            {
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
                IKapBoard.IKapOpen(2, (uint)CardPort);     //打开采集卡  2:表示类型为采集卡
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="gain"></param>
        /// <returns></returns>
        public override bool SetGain(float gain)
        {
            return true;
        }
        /// <summary>
        /// 设置触发模式
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override bool SetTriggerMode(int mode)
        {

            return true;
        }

        /// <summary>
        /// 开始采集图像
        /// </summary>
        /// <returns></returns>
        public override bool StartGrabImage()
        {
            try
            {
                if (IsStartGrabImage_Status)
                {
                    //StopGrabImage();//启动采集前，判断是否已经开启采集。注意关闭
                }

                if (!IsStartGrabImage_Status)
                {

                    m_nCurFrameIndex = 0;
                    int ret = IKapBoard.IKapStartGrab(m_hBoard, 0);
                    if (CheckIKapBoard(ret) == true)
                    {
                        IsStartGrabImage_Status = true;
                        return true;
                    }
                    else
                    {
                        IsStartGrabImage_Status = false;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                IsStartGrabImage_Status = false;
                //throw;
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return IsStartGrabImage_Status;
        }

        /// <summary>
        /// 停止采集图像
        /// </summary>
        /// <returns></returns>
        public override bool StopGrabImage()
        {
            IsStartGrabImage_Status = false;
            try
            {
                int ret = IKapBoard.IKapStopGrab(m_hBoard);
                if (CheckIKapBoard(ret) == true)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);

            }
            return IsStartGrabImage_Status;
        }

        #region >>>Callback
        delegate void IKapCallBackProc(IntPtr pParam);
        private IKapCallBackProc OnGrabStartProc;
        private IKapCallBackProc OnFrameLostProc;
        private IKapCallBackProc OnTimeoutProc;
        private IKapCallBackProc OnFrameReadyProc;
        private IKapCallBackProc OnGrabStopProc;
        #endregion
        #region >>>Callback

        /// <summary>
        /// This callback function will be called on grab start
        /// 这个回调函数将在抓取开始时调用
        /// </summary>
        /// <param name="pParam"></param>
        public void OnGrabStartFunc(IntPtr pParam)
        {
            m_bonline = true;
        }
        /// <summary>
        /// This callback function will be called on frame lost
        /// 这个回调函数会在丢帧时调用
        /// </summary>
        /// <param name="pParam"></param>
        public void OnFrameLostFunc(IntPtr pParam)
        {
        }

        /// <summary>
        /// This callback function will be called on grabbing timeout
        /// 这个回调函数会在抓取超时时调用
        /// 【黑白相机】
        /// </summary>
        /// <param name="pParam"></param>
        public void OnTimeoutFunc(IntPtr pParam)
        {
        }


        /// <summary>
        /// This callback function will be called at the frame ready
        /// 此回调函数将在帧就绪时调用
        /// 【黑白相机】
        /// </summary>
        /// <param name="pParam"></param>
        public void OnFrameReadyFunc_Gray(IntPtr pParam)
        {
            try
            {
                //HImage hImage = new HImage();
                //Console.WriteLine("Grab frame ready");
                Log4netRecord.CamLog.Debug($"相机{CamID}回调开始" + "-" + GrabCount + 1.ToString());
                CamCallbackStartEvent?.Invoke(GrabCount + 1);
                IntPtr hDev = (IntPtr)pParam;
                IntPtr pUserBuffer = IntPtr.Zero;
                int nFrameSize = 0;
                int nFrameCount = 0;
                IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_COUNT, ref nFrameCount);
                IKapBoard.IKapGetBufferStatus(hDev, m_nCurFrameIndex, ref status);
                if (status.uFull == 1)
                {
                    // get frame grabber buffer
                    IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize);
                    IKapBoard.IKapGetBufferAddress(hDev, m_nCurFrameIndex, ref pUserBuffer);
                    HOperatorSet.GenImage1(out ImageGrab1, "byte", ImgWidth, ImgHeight, pUserBuffer);
                    GrabCount++;
                    Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = ImageGrab1, image_ID = GrabCount, imageWidth = ImgWidth, imageHeight = ImgHeight });


                    CamCallbackEndEvent?.Invoke();
                    Log4netRecord.CamLog.Debug($"相机{CamID}回调结束" + "-" + GrabCount.ToString());
          
                    //GrabImg.Set();
                    bo_jisuan = true;
                }
                m_nCurFrameIndex++;
                m_nCurFrameIndex = m_nCurFrameIndex % m_nTotalFrameCount;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                //throw;
            }
        }
        /// <summary>
        /// This callback function will be called at the frame ready
        /// 此回调函数将在帧就绪时调用
        /// 【彩色相机】
        /// </summary>
        /// <param name="pParam"></param>
        public void OnFrameReadyFunc_RGB(IntPtr pParam)
        {
            try
            {
                Log4netRecord.CamLog.Debug($"相机{CamID}回调开始" + "-" + GrabCount + 1.ToString());
                CamCallbackStartEvent?.Invoke(GrabCount + 1);
                IntPtr hDev = (IntPtr)pParam;
                IntPtr pUserBuffer = IntPtr.Zero;
                int nFrameSize = 0;
                int nFrameCount = 0;
                IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_COUNT, ref nFrameCount);
                IKapBoard.IKapGetBufferStatus(hDev, m_nCurFrameIndex, ref status);
                if (status.uFull == 1)
                {

                    IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize); // get frame grabber buffer
                    IKapBoard.IKapGetBufferAddress(hDev, m_nCurFrameIndex, ref pUserBuffer);
                    HOperatorSet.GenImageInterleaved(out ImageGrab1, pUserBuffer, "bgr", ImgWidth, ImgHeight, -1, "byte", 0, 0, 0, 0, -1, 0);
                    GrabCount++;
                    Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = ImageGrab1, image_ID = GrabCount, imageWidth = ImgWidth, imageHeight = ImgHeight });

                    CamCallbackEndEvent?.Invoke();
                    Log4netRecord.CamLog.Debug($"相机{CamID}回调结束" + "-" + GrabCount.ToString());
           
                    //GrabImg.Set();
                    bo_jisuan = true;
                }
                m_nCurFrameIndex++;
                m_nCurFrameIndex = m_nCurFrameIndex % m_nTotalFrameCount;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                //throw;
            }
        }
        /// <summary>
        /// This callback function will be called on grab stop
        /// 该回调函数将在抓取停止时调用
        /// </summary>
        /// <param name="pParam"></param>
        public void OnGrabStopFunc(IntPtr pParam)
        {
            m_bonline = false;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        bool Configstatus = true;
        /// <summary>
        /// 加载采集卡配置参数
        /// </summary>
        private bool ConfigureFrameGrabber()
        {
            bool result = false;
            try
            {
                int ret = (int)ErrorCode.IK_RTN_OK;                                                     // Return value of IKapBoard methods
                uint nPCIeDevCount = 0;

                string sMesg = "";
                uint resourceNameSize = 0;
                IKapBoard.IKAPERRORINFO tIKei = new IKapBoard.IKAPERRORINFO();

                // Get the count of PCIe interface boards
                ret = IKapBoard.IKapGetBoardCount((int)BoardType.IKBoardPCIE, ref nPCIeDevCount);
                CheckIKapBoard(ret);
                StringBuilder resourceName;     //采集卡名称
                if (nPCIeDevCount == 0)//没有发现采集卡
                {
                    System.Windows.Forms.MessageBox.Show("没有发现采集卡");
                }

                for (uint i = 0; i < nPCIeDevCount; i++)
                {
                    resourceNameSize = 0;
                    resourceName = new StringBuilder(0);
                    IKapBoard.IKapGetBoardName((uint)BoardType.IKBoardPCIE, i, resourceName, ref resourceNameSize);
                    IKapBoard.IKapGetLastError(ref tIKei, true);
                    if (tIKei.uErrorCode == (uint)ErrorCode.IKStatus_BufferTooSmall)
                    {
                        resourceName = new StringBuilder((int)resourceNameSize);
                        IKapBoard.IKapGetBoardName((uint)BoardType.IKBoardPCIE, i, resourceName, ref resourceNameSize);
                    }
                    IKapBoard.IKapGetLastError(ref tIKei, true);
                    if (tIKei.uErrorCode != (uint)ErrorCode.IKStatus_Success)
                        sMesg = string.Concat("Get Device Name Fail. Error Code:", tIKei.uErrorCode);
                    else
                        sMesg = string.Concat("PCIE Device- ", i.ToString("d"), "\nName: ", resourceName);
                    //Console.WriteLine(sMesg);
                }
                m_hBoard = IKapBoard.IKapOpen((int)BoardType.IKBoardPCIE, CardPort);
                if (m_hBoard.Equals(-1))
                {
                    System.Windows.Forms.MessageBox.Show("Open device failure");
                    //Console.ReadLine();
                    Environment.Exit(1);
                }

                // Load configuration from file (*.vlcf)
                if (m_ConfigFileName == null)
                {
                    System.Windows.Forms.MessageBox.Show("Fail to get configuration, using default setting!\n");
                }
                else
                {
                    ret = IKapBoard.IKapLoadConfigurationFromFile(m_hBoard, m_ConfigFileName);
                    CheckIKapBoard(ret);
                }
                ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_WIDTH, ref ImgWidth);
                CheckIKapBoard(ret);
                ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_HEIGHT, ref ImgHeight);
                CheckIKapBoard(ret);
                // Configure transfer mode
                int transfer_mode = (int)FrameTransferMode.IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT;
                ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_TRANSFER_MODE, transfer_mode);
                CheckIKapBoard(ret);

                ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_COUNT, m_nTotalFrameCount);
                CheckIKapBoard(ret);

                OnGrabStartProc = new IKapCallBackProc(OnGrabStartFunc);
                ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStart, Marshal.GetFunctionPointerForDelegate(OnGrabStartProc), m_hBoard);
                CheckIKapBoard(ret);

                if (IsCamera_RGB)
                    OnFrameReadyProc = new IKapCallBackProc(OnFrameReadyFunc_RGB);
                else
                    OnFrameReadyProc = new IKapCallBackProc(OnFrameReadyFunc_Gray);
                ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameReady, Marshal.GetFunctionPointerForDelegate(OnFrameReadyProc), m_hBoard);
                CheckIKapBoard(ret);

                OnFrameLostProc = new IKapCallBackProc(OnFrameLostFunc);
                ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameLost, Marshal.GetFunctionPointerForDelegate(OnFrameLostProc), m_hBoard);
                CheckIKapBoard(ret);

                OnTimeoutProc = new IKapCallBackProc(OnTimeoutFunc);
                ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_TimeOut, Marshal.GetFunctionPointerForDelegate(OnTimeoutProc), m_hBoard);
                CheckIKapBoard(ret);

                OnGrabStopProc = new IKapCallBackProc(OnGrabStopFunc);
                ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStop, Marshal.GetFunctionPointerForDelegate(OnGrabStopProc), m_hBoard);
                CheckIKapBoard(ret);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Log4netRecord.ErrorLog.Debug(ex);
                //throw;
            }
            return result;
        }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="ret"></param>
        public bool CheckIKapBoard(int ret)
        {
            bool status = true; //错误标志位
            try
            {
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    string sErrMsg = "";
                    IKapBoard.IKAPERRORINFO tIKei = new IKapBoardClassLibrary.IKapBoard.IKAPERRORINFO();
                    IKapBoard.IKapGetLastError(ref tIKei, true);
                    sErrMsg = string.Concat("Error",
                                            sErrMsg,
                                            "Board Type\t = 0x", tIKei.uBoardType.ToString("X4"), "\n",
                                            "Board Index\t = 0x", tIKei.uBoardIndex.ToString("X4"), "\n",
                                            "Error Code\t = 0x", tIKei.uErrorCode.ToString("X4"), "\n"
                                            );
                    status = false;
                    System.Windows.Forms.MessageBox.Show("IKapBoard 用户开发手册.请查看相机驱动报错代码：\n" + sErrMsg);
                    //Environment.Exit(1);
                }
                else
                {
                    status = true;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                status = false;
                //throw;
            }
            return status;
        }
        /// <summary>
        /// 
        /// </summary>
        public void UnRegisterCallback()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStart);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameReady);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameLost);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_TimeOut);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStop);
        }
        /// <summary>
        ///  This function will be close device and release buffer
        ///  关闭设备
        /// </summary>
        public void CloseDevice()
        {
            try
            {
                // Close frame grabber
                bool b = m_hBoard.Equals(-1);
                if (!m_hBoard.Equals(-1))
                {
                    IKapBoard.IKapClose(m_hBoard);
                    m_hBoard = (IntPtr)(-1);
                }

                // free buffer
                //m_bmpImage.ReleaseImage();
            }
            catch (Exception)
            {

                throw;
            }

        }



        /// <summary>
        /// 灰度图像 HObject -> HImage
        /// </summary>
        public HImage HObject_To_HImage1(HObject hObj)
        {
            HImage hImage = new HImage();
            HTuple type, width, height, pointer;
            HOperatorSet.GetImagePointer1(hObj, out pointer, out type, out width, out height);
            hImage.GenImage1(type, width, height, pointer);
            return hImage;
        }
        /// <summary>
        /// 彩色图像 HObject -> HImage3
        /// </summary>
        public HImage HObject_To_HImage3(HObject hObj)
        {
            HImage image = new HImage();
            HTuple type, width, height, pointerRed, pointerGreen, pointerBlue;
            HOperatorSet.GetImagePointer3(hObj, out pointerRed, out pointerGreen, out pointerBlue,
                                          out type, out width, out height);
            image.GenImage3(type, width, height, pointerRed, pointerGreen, pointerBlue);
            return image;
        }

        /// <summary>
        /// 设置软件触发IO
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns>0:失败，1成功</returns>
        public override int OutIO(int nValue)
        {
            nValue = 1;
            int snt = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_SOFTWARE_TRIGGER_START, nValue);
            //CheckIKapBoard(snt);
            return snt;
        }

        public override bool SetGrabState(bool bStart)
        {
            throw new NotImplementedException();
        }
    }
}
