using DevMgmt.Cameras.Models;
using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;
using IKapBoardClassLibrary;
using IKapC.NET;
using log4net;
using LogTool;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevMgmt.Cameras.SDK
{
    public class IKCXPBoard : CameraObj
    {

        //软件运行日志
        const string RunningName = "Run";
        /// <summary>
        /// 软件运行日志
        /// </summary>
       // public static ILog RunningLog = LogManager.GetLogger(RunningName);
        #region 属性字段
        // 采集卡设备句柄。
        public IntPtr m_hBoard = new IntPtr(-1);
        // 相机设备句柄。
        public IntPtr m_hCamera = new IntPtr(-1);
        public int m_nCurFrameIndex = 0;
        // 图像缓冲区申请的帧数。
        public int m_nTotalFrameCount = 5;
        //采集累计帧数
        //public int GrabCount = 0;
        public bool bo_jisuan = false;
        public bool m_bonline = false;
        public string configFileName = "";
        private string cameraSerialNumber = string.Empty;
        //采集卡端口号
        //采集到的图像进入队列
        private QueuePackage<ImageInfo> Queue_GrabImage = new QueuePackage<ImageInfo>();

        //是否彩色相机															 
        private bool IsCamera_RGB = false;
        private HObject ImageGrab1;
        //是否启动采集图像
        public bool IsStartGrabImage_Status = false;
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

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public override bool IsExist
        {
            get { return bo_jisuan; }
            set { bo_jisuan = value; }
        }
        public override bool IsCameraOpen
        {
            get
            {
                bool b = true;

                return b;
            }
        }

        public IKCXPBoard(string CameraSerialNumber, string ConfigFileName, bool isCamera_RGB)
        {
            cameraSerialNumber = CameraSerialNumber;
            configFileName = ConfigFileName;
            bo_jisuan = false;
            IsCamera_RGB = isCamera_RGB;
        }
        public override void DestroyCam()
        {
            if (IsStartGrabImage_Status)
                StopGrabImage();
            // 关闭采集卡设备。
            if (!m_hBoard.Equals(-1))
            {
                UnRegisterCallback();
                // 关闭采集卡设备。
                if (!m_hBoard.Equals(-1))
                {
                    IKapBoard.IKapClose(m_hBoard);
                    m_hBoard = (IntPtr)(-1);
                }

                // 关闭相机设备。
                if (!m_hCamera.Equals(-1))
                {
                    IKapCLib.ItkDevClose(m_hCamera);
                    m_hCamera = (IntPtr)(-1);
                }
                IKapCLib.ItkManTerminate();
                IsStartGrabImage_Status = false;
            }
        }

        public override void DestroyObjects2()
        {
            DestroyCam();
        }

        public override bool InitCam()
        {
            return InitEnvironment();
        }

        public override bool OpenCam()
        {
            bool res = ConfigureCamera();
            res &= ConfigureFrameGrabber();
            return res;
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
                if (!IsStartGrabImage_Status)
                {
                    uint res = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                    int ret = (int)ErrorCode.IK_RTN_OK;
                    res = IKapCLib.ItkDevExecuteCommand(m_hCamera, "AcquisitionStop");
                    if (!CheckIKapC(res))
                    {
                        IsStartGrabImage_Status = false;
                        return false;
                    }
                    ret = IKapBoard.IKapStartGrab(m_hBoard, 0);
                    if (!CheckIKapBoard(ret))
                    {
                        IsStartGrabImage_Status = false;
                        return false;
                    }
                    res = IKapCLib.ItkDevExecuteCommand(m_hCamera, "AcquisitionStart");
                    if (!CheckIKapC(res))
                    {
                        IsStartGrabImage_Status = false;
                        return false;
                    }
                    IsStartGrabImage_Status = true;
                    return true;
                }
                return true;
            }
            catch (Exception)
            {
                IsStartGrabImage_Status = false;
                return false;
            }
        }

        public override bool StopGrabImage()
        {
            uint res = IKapCLib.ItkDevExecuteCommand(m_hCamera, "AcquisitionStop");
            if (!CheckIKapC(res))
            {
                IsStartGrabImage_Status = false;
                //return false;
            }
            int ret = (int)ErrorCode.IK_RTN_OK;
            ret = IKapBoard.IKapStopGrab(m_hBoard);
            if (ret == (int)ErrorCode.IK_RTN_OK)
            {
                IsStartGrabImage_Status = false;
                return true;
            }
            else
            {
                CheckIKapBoard(ret);
                return false;
            }
        }

        private bool ConfigureFrameGrabber()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            ret = IKapBoard.IKapLoadConfigurationFromFile(m_hBoard, configFileName);
            if (!CheckIKapBoard(ret))
                return false;
            // 设置图像缓冲区帧数。
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_COUNT, m_nTotalFrameCount);
            if (!CheckIKapBoard(ret))
                return false;
            // 设置超时时间。
            int timeout = -1;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_TIME_OUT, timeout);
            if (!CheckIKapBoard(ret))
                return false;
            // 设置采集模式。
            int grab_mode = (int)GrabMode.IKP_GRAB_NON_BLOCK;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_GRAB_MODE, grab_mode);
            if (!CheckIKapBoard(ret))
                return false;
            // 设置传输模式。
            int transfer_mode = (int)FrameTransferMode.IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_TRANSFER_MODE, transfer_mode);
            if (!CheckIKapBoard(ret))
                return false;
            // 注册回调函数
            OnGrabStartProc = new IKapCallBackProc(OnGrabStartFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStart, Marshal.GetFunctionPointerForDelegate(OnGrabStartProc), m_hBoard);
            if (!CheckIKapBoard(ret))
                return false;
            if (IsCamera_RGB)
                OnFrameReadyProc = new IKapCallBackProc(OnFrameReadyFunc_RGB);
            else
                OnFrameReadyProc = new IKapCallBackProc(OnFrameReadyFunc_Gray);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameReady, Marshal.GetFunctionPointerForDelegate(OnFrameReadyProc), m_hBoard);
            if (!CheckIKapBoard(ret))
                return false;
            OnFrameLostProc = new IKapCallBackProc(OnFrameLostFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameLost, Marshal.GetFunctionPointerForDelegate(OnFrameLostProc), m_hBoard);
            if (!CheckIKapBoard(ret))
                return false;
            OnTimeoutProc = new IKapCallBackProc(OnTimeoutFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_TimeOut, Marshal.GetFunctionPointerForDelegate(OnTimeoutProc), m_hBoard);
            if (!CheckIKapBoard(ret))
                return false;
            OnGrabStopProc = new IKapCallBackProc(OnGrabStopFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStop, Marshal.GetFunctionPointerForDelegate(OnGrabStopProc), m_hBoard);
            if (!CheckIKapBoard(ret))
                return false;
            return true;
        }
        public bool CheckIKapBoard(int ret)
        {
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                string sErrMsg = "";
                IKapBoard.IKAPERRORINFO tIKei = new IKapBoardClassLibrary.IKapBoard.IKAPERRORINFO();

                // 获取错误码信息。
                //
                // Get error code message.
                IKapBoard.IKapGetLastError(ref tIKei, true);

                // 打印错误信息。
                //
                // Print error message.
                sErrMsg = string.Concat("Error",
                                        sErrMsg,
                                        "Board Type\t = 0x", tIKei.uBoardType.ToString("X4"), "\n",
                                        "Board Index\t = 0x", tIKei.uBoardIndex.ToString("X4"), "\n",
                                        "Error Code\t = 0x", tIKei.uErrorCode.ToString("X4"), "\n"
                                        );
                Console.Write(sErrMsg);
                //Console.ReadLine();
                //Environment.Exit(1);
                return false;
            }
            return true;
        }


        #region Callback
        delegate void IKapCallBackProc(IntPtr pParam);

        /* @brief：本函数被注册为一个回调函数。当图像采集开始时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When starting grabbing images, the function will be called. */
        private IKapCallBackProc OnGrabStartProc;

        /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called. */
        private IKapCallBackProc OnFrameLostProc;

        /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called. */
        private IKapCallBackProc OnTimeoutProc;

        /* @brief：本函数被注册为一个回调函数。当一帧图像采集完成时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When a frame of image grabbing ready, the function will be called. */
        private IKapCallBackProc OnFrameReadyProc;

        /* @brief：本函数被注册为一个回调函数。当图像采集停止时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When stopping grabbing images, the function will be called. */
        private IKapCallBackProc OnGrabStopProc;
        #endregion

        #region Callback

        /* @brief：清除回调函数。
		*
		* @brief：Unregister callback functions. */
        private void UnRegisterCallback()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;

            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStart);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameReady);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameLost);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_TimeOut);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStop);
        }

        /* @brief：本函数被注册为一个回调函数。当图像采集开始时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When starting grabbing images, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnGrabStartFunc(IntPtr pParam)
        {
            m_bonline = true;
        }

        /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnFrameLostFunc(IntPtr pParam)
        {
            //Console.WriteLine("Image frame lost");
        }

        /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnTimeoutFunc(IntPtr pParam)
        {
            //Console.WriteLine("Grab image timeout");
        }

        /* @brief：本函数被注册为一个回调函数。当一帧图像采集完成时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When a frame of image grabbing ready, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnFrameReadyFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab frame ready");

            IntPtr hDev = (IntPtr)pParam;
            IntPtr pUserBuffer = IntPtr.Zero;
            int nFrameSize = 0;
            int nFrameCount = 0;
            IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();

            IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_COUNT, ref nFrameCount);
            IKapBoard.IKapGetBufferStatus(hDev, m_nCurFrameIndex, ref status);

            // 当图像缓冲区满时。
            //
            // When the buffer is full.
            if (status.uFull == 1)
            {
                // 获取一帧图像的大小。
                //
                // Get the size of a frame of image.
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize);

                // 获取缓冲区地址。
                //
                // Get the buffer address.
                IKapBoard.IKapGetBufferAddress(hDev, m_nCurFrameIndex, ref pUserBuffer);

                // 保存图像。
                //
                // Save image.
                /*
                IKapBoard.IKapSaveBuffer(hDev,m_nCurFrameIndex,m_strFileName,(int)ImageCompressionFalg.IKP_DEFAULT_COMPRESSION);
                 */
            }
            m_nCurFrameIndex++;
            m_nCurFrameIndex = m_nCurFrameIndex % m_nTotalFrameCount;
        }

        private System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        private System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();



        /// This callback function will be called at the frame ready
        /// 此回调函数将在帧就绪时调用
        /// 【黑白相机】
        /// </summary>
        /// <param name="pParam"></param>
        public void OnFrameReadyFunc_Gray(IntPtr pParam)
        {

            try
            {
                //Log4netRecord.CamLog.Debug("回调开始" + CamID.ToString() + "-" + GrabCount.ToString());
                sw.Stop();              
                Log4netRecord.CamLog.Info($"{cameraSerialNumber}第{GrabCount + 1}帧:两次回调间隔时间: {sw.Elapsed.TotalMilliseconds}ms,队列长度:{Queue_GrabImage.Count}");
                sw.Restart();
                Log4netRecord.CamLog.Debug($"相机{CamID}回调开始" + "-" + GrabCount + 1.ToString());
                CamCallbackStartEvent?.Invoke(GrabCount + 1);

                IntPtr hDev = (IntPtr)pParam;
                IntPtr pUserBuffer = IntPtr.Zero;
                int nFrameSize = 0;
                int nFrameCount = 0;
                IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_COUNT, ref nFrameCount);
                int ImgWidth = 0;
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_IMAGE_WIDTH, ref ImgWidth);
                int ImgHeight = 0;
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_IMAGE_HEIGHT, ref ImgHeight);
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_CURRENT_BUFFER_INDEX, ref m_nCurFrameIndex);
                IKapBoard.IKapGetBufferStatus(hDev, m_nCurFrameIndex, ref status);
                if (status.uFull == 1)
                {
                    //IntPtr pUserBuffer = Marshal.AllocHGlobal(ImgWidth * ImgHeight);
                    // get frame grabber buffer
                    IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize);
                    IKapBoard.IKapGetBufferAddress(hDev, m_nCurFrameIndex, ref pUserBuffer);
                    sw1.Restart();
                    HOperatorSet.GenImage1(out ImageGrab1, "byte", ImgWidth, ImgHeight, pUserBuffer);

                    //IKapBoard.IKapSetBufferAddress(m_hBoard, m_nCurFrameIndex, pUserBuffer);

                    //HOperatorSet.GenImage1Extern(out ImageGrab1, "byte", ImgWidth, ImgHeight, pUserBuffer, 0);



                    sw1.Stop();
                    Log4netRecord.CamLog.Info($"{cameraSerialNumber}:第{GrabCount}帧图像转换耗时: {sw.Elapsed.TotalMilliseconds}ms,队列长度:{Queue_GrabImage.Count}");
                    //Queue_GrabImage.Enqueue(ImageGrab1);

                    GrabCount++;
                    Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = ImageGrab1, image_ID = GrabCount, imageWidth = ImgWidth, imageHeight = ImgHeight });
                    //GrabImg.Set();
                    bo_jisuan = true;
                    CamCallbackEndEvent?.Invoke();
                    Log4netRecord.CamLog.Debug($"相机{CamID}回调结束" + "-" + GrabCount.ToString());
                    //Console.WriteLine(GrabCount);
                }
                m_nCurFrameIndex++;
                m_nCurFrameIndex = m_nCurFrameIndex % m_nTotalFrameCount;
                //Log4netRecord.CamLog.Debug(CamID.ToString() + "-" + GrabCount.ToString());
            }
            catch (Exception)
            {
                //LogDebug.Error(ex);
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
                IntPtr hDev = (IntPtr)pParam;
                IntPtr pUserBuffer = IntPtr.Zero;
                int nFrameSize = 0;
                int nFrameCount = 0;
                IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_COUNT, ref nFrameCount);
                int ImgWidth = 0;
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_IMAGE_WIDTH, ref ImgWidth);
                int ImgHeight = 0;
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_IMAGE_HEIGHT, ref ImgHeight);
                IKapBoard.IKapGetBufferStatus(hDev, m_nCurFrameIndex, ref status);
                if (status.uFull == 1)
                {

                    IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize); // get frame grabber buffer
                    IKapBoard.IKapGetBufferAddress(hDev, m_nCurFrameIndex, ref pUserBuffer);
                    HOperatorSet.GenImageInterleaved(out ImageGrab1, pUserBuffer, "bgr", ImgWidth, ImgHeight, -1, "byte", 0, 0, 0, 0, -1, 0);
                    //Queue_GrabImage.Enqueue(ImageGrab1);
                    GrabCount++;
                    Queue_GrabImage.Enqueue(new ImageInfo() { ho_image = ImageGrab1, image_ID = GrabCount, imageWidth = ImgWidth, imageHeight = ImgHeight });//图像进队

                    //GrabImg.Set();
                    bo_jisuan = true;
                }
                m_nCurFrameIndex++;
                m_nCurFrameIndex = m_nCurFrameIndex % m_nTotalFrameCount;
            }
            catch (Exception)
            {
                //LogDebug.Error(ex);
            }
        }

        /* @brief：本函数被注册为一个回调函数。当图像采集停止时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When stopping grabbing images, the function will be called.
         * @param[in] pParam：Input parameter. */
        public void OnGrabStopFunc(IntPtr pParam)
        {
            m_bonline = false;
        }

        /* @brief：初始化IKapC 运行环境。
	  *
	  * @brief：Initialize IKapC runtime environment. */
        private bool InitEnvironment()
        {
            // IKapC 函数返回值。
            //
            // Return value of IKapC functions.
            uint res = (uint)ItkStatusErrorId.ITKSTATUS_OK;

            res = IKapCLib.ItkManInitialize();
            return CheckIKapC(res);
        }

        /* @brief：释放 IKapC 运行环境。
         *
         * @brief：Release IKapC runtime environment. */
        private void ReleaseEnvironment()
        {
            IKapCLib.ItkManTerminate();
        }

        /* @brief：配置相机设备。
         *
         * @brief：Configure camera device. */
        private bool ConfigureCamera()
        {
            uint res = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            uint numCameras = 0;
            // 枚举可用相机的数量。在打开相机前，必须调用 ItkManGetDeviceCount() 函数。
            res = IKapCLib.ItkManGetDeviceCount(ref numCameras);
            if (!CheckIKapC(res))
                return false;
            // 当没有连接的相机时。
            if (numCameras == 0)
            {
                Console.Write("No camera.\n");
                IKapCLib.ItkManTerminate();
                //Console.ReadLine();
                //Environment.Exit(1);
            }
            // 打开CoaXPress相机。
            for (uint i = 0; i < numCameras; i++)
            {
                IKapCLib.ITKDEV_INFO di = new IKapCLib.ITKDEV_INFO();

                // 获取相机设备信息。
                res = IKapCLib.ItkManGetDeviceInfo(i, ref di);
                Console.Write("Using camera: serial: {0}, name: {1}, interface: {2}.\n", di.SerialNumber, di.FullName, di.DeviceClass);
                if (!CheckIKapC(res))
                    return false;
                // 当设备为CoaXPress相机且序列号正确时。
                if (di.DeviceClass == "CoaXPress" && di.SerialNumber == cameraSerialNumber)
                {
                    IKapCLib.ITK_CXP_DEV_INFO cxpDevInfo = new IKapCLib.ITK_CXP_DEV_INFO();
                    IKapBoard.IKAP_CXP_BOARD_INFO cxpBoardInfo = new IKapBoard.IKAP_CXP_BOARD_INFO();

                    // 获取CoaXPress相机设备信息。
                    res = IKapCLib.ItkManGetCXPDeviceInfo(i, ref cxpDevInfo);
                    if (!CheckIKapC(res))
                        return false;
                    for (int j = 0; j < 5; j++)
                    {
                        // 打开相机。
                        res = IKapCLib.ItkDevOpen(i, (int)ItkDeviceAccessMode.ITKDEV_VAL_ACCESS_MODE_EXCLUSIVE, ref m_hCamera);
                        if (res == 0)
                        {
                            break;
                        }
                    }

                    if (!CheckIKapC(res))
                        return false;

                    // 打开采集卡。
                    CopyCXPInfo(cxpDevInfo, ref cxpBoardInfo);
                    m_hBoard = IKapBoard.IKapOpenCXP((uint)BoardType.IKBoardALL, cxpBoardInfo.BoardIndex, cxpBoardInfo);
                    if (m_hBoard.Equals(-1))
                        CheckIKapBoard((int)ErrorCode.IKStatus_OpenBoardFail);
                    break;
                }
            }
            return true;
        }

        /* @brief：复制 CXP 信息。
         * @param[in] srcInfo：IKapC库信息。
         * @param[in] dstInfo：IKapBoard库信息。
         *
         * @brief：Copy CXP information.
         * @param[in] srcInfo：IKapC library information.
         * @param[in] dstInfo：IKapBoard library information. */
        private void CopyCXPInfo(IKapCLib.ITK_CXP_DEV_INFO srcInfo, ref IKapBoard.IKAP_CXP_BOARD_INFO dstInfo)
        {
            dstInfo.BoardIndex = srcInfo.BoardIndex;
            dstInfo.CameraIndex = srcInfo.CameraId;
            dstInfo.MasterPort = srcInfo.MasterPort;
            dstInfo.SlaveCount = srcInfo.SlaveCount;

            dstInfo.Reserved = new byte[252];
            for (int j = 0; j < srcInfo.Reserved.Length; j++)
            {
                dstInfo.Reserved[j] = (byte)srcInfo.Reserved.ElementAt(j);
            }

            dstInfo.SlavePort = new uint[7];
            for (int j = 0; j < srcInfo.SlavePort.Length; j++)
            {
                dstInfo.SlavePort[j] = srcInfo.SlavePort[j];
            }
        }

        /* @brief：判断 IKapC 函数是否成功调用。
		* @param[in] res：函数返回值。
		*
		* @brief：Determine whether the IKapC function is called successfully.
		* @param[in] res：Function return value. */
        private bool CheckIKapC(uint res)
        {
            if (res != (uint)ItkStatusErrorId.ITKSTATUS_OK)
            {
                Console.Error.WriteLine("Error Code: {0}.\n", res.ToString("x8"));
                IKapCLib.ItkManTerminate();
                //Console.ReadLine();
                //Environment.Exit(1);
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
