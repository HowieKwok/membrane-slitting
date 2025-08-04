using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HalconDotNet;
using Frame_Core;
using IKapBoardClassLibrary;
using LogTool;
using NPOI.SS.Formula.Functions;

namespace Frame_Core
{

    public abstract class CameraObj
    {
        #region filed
        protected bool connStatus = false;
        public virtual bool ConnStatus
        {
            get { return connStatus; }
        }
        #endregion


        public delegate void delegateCallBack();
        /// <summary>
        /// 相机回调事件
        /// </summary>
        public delegateCallBack CamCallbackEvent;

        /// <summary>
        /// 相机ID
        /// </summary>
        public int CamID = -1;

        public int m_nCount = 0;

        public bool bIs3D = false;



        /// <summary>
        /// 相机回调计数
        /// </summary>
        public long GrabCount = 0;

        /// <summary>
        /// 图像回调报警
        /// </summary>
        public bool ImageCallbackAlarm = false;

        double deep_min;
        double deep_max;


        public struct SurfacePointRGB
        {
            public double x;
            public double y;
            public double z;
            public byte r;
            public byte g;
            public byte b;
        }


        public struct SurfacePoint
        {
            public double x;
            public double y;
            public double z;

            public SurfacePoint(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public void Clear()
            {
                x = 0;
                y = 0;
                z = 0;
            }
        }

        //与存图序列化有关,3D相机使用
        [Serializable]
        public struct Cam_Calibration
        {
            public int ImageWidth;
            public int ImageHeight;
            public double xResolution;
            public double yResolution;
            public double zResolution;
            public double xOffset;
            public double yOffset;
            public double zOffset;

            public Cam_Calibration(int width = 0, int height = 0, double x_resolution = 0, double y_resolution = 0, double z_resolution = 0,
                                    double x_offset = 0, double y_offset = 0, double z_offset = 0)
            {
                ImageWidth = width;
                ImageHeight = height;
                xResolution = x_resolution;
                yResolution = y_resolution;
                zResolution = z_resolution;
                xOffset = x_offset;
                yOffset = y_offset;
                zOffset = z_offset;
            }

            public Cam_Calibration(Cam_Calibration calibration)
            {
                ImageWidth = calibration.ImageWidth;
                ImageHeight = calibration.ImageHeight;
                xResolution = calibration.xResolution;
                yResolution = calibration.yResolution;
                zResolution = calibration.zResolution;
                xOffset = calibration.xOffset;
                yOffset = calibration.yOffset;
                zOffset = calibration.zOffset;
            }

            public void Clear()
            {
                ImageWidth = 0;
                ImageHeight = 0;
                xResolution = 0;
                yResolution = 0;
                zResolution = 0;
                xOffset = 0;
                yOffset = 0;
                zOffset = 0;
            }
        }


        #region menber   

        /// <summary>
        /// 传递出的图像
        /// </summary>
        public virtual HObject Image
        {
            get;
        }
        public virtual QueuePackage<ImageInfo> QueueGrabImage
        {
            get;
            //set;         
        }

        public virtual QueuePackage<SurfacePoint[]> QueueGrabSurface
        {
            get;
        }

        public virtual QueuePackage<Image3DClass> QueueGrab3DImage
        {
            get;
        }


        /// <summary>
        /// 图像存在标志位
        /// </summary>
        public virtual bool IsExist
        {
            get;
            set;
        }

        /// <summary>
        /// 相机是状态
        /// </summary>
        public virtual bool IsCameraStatue
        {
            get;
        }


        /// <summary>
        /// 相机是否打开
        /// </summary>
        public virtual bool IsCameraOpen
        {
            get;
        }


        #endregion

        #region function

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <returns></returns>
        public abstract bool OpenCam();


        /// <summary>
        /// 初始化相机
        /// </summary>
        /// <returns></returns>
        public abstract bool InitCam();

        /// <summary>
        /// 开始采集图像
        /// </summary>
        /// <returns></returns>
        public abstract bool StartGrabImage();


        /// <summary>
        /// 停止采集
        /// </summary>
        /// <returns></returns>
        public abstract bool StopGrabImage();


        /// <summary>
        /// 释放相机
        /// </summary>
        /// <returns></returns>
        public abstract void DestroyCam();

        /// <summary>
        /// 销毁资源
        /// </summary>
        public abstract void DestroyObjects2();
        /// <summary>
        /// 设置触发模式,，mode = 0 设置内触发模式；mode = 1 设置外触发模式
        /// </summary>
        /// <returns></returns>
        public abstract bool SetTriggerMode(int mode);


        /// <summary>
        /// 设置触发源  0 - Line0，4 - Counter， 7 - Software;
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public virtual bool SetTriggerSource(int mode) { return true; }
        /// <summary>
        /// 设置相机取流开始或停止
        /// </summary>
        /// <param name="bStart"></param>
        /// <returns></returns>
        public abstract bool SetGrabState(bool bStart);


        /// <summary>
        /// 获取触发源或触发模式,虚方法，可重载
        /// <returns>返回当前值</returns>
        public virtual void GetCammerMode()
        {
            ;
        }
        /// <summary>
        /// 获取曝光时间和增益，可重载
        /// <returns>返回当前值</returns>
        public virtual float GetCameraExposure()
        {
            return 0;
        }


        /// <summary>
        /// 设置触发源或触发模式
        /// </summary>
        public virtual void SetTriggerSource() {; }


        /// <summary>
        /// 设置相机曝光参数
        /// </summary>
        public virtual void SetCameraExposure() {; }


        /// <summary>
        ///  获取相机曝光时间
        /// </summary>
        public virtual float GetGain() { return 0; }

        /// <summary>
        ///  设置相机曝光时间
        /// </summary>
        public abstract bool SetGain(float gain);

        #region MER

        /// <summary>
        /// 获取触发源或触发模式 显示重载
        /// </summary>
        /// <param name="strFeatureName">输入需要获得的对象</param>
        /// <param name="enumEntryList">ref 对象的枚举</param>
        /// <returns>返回当前值</returns>
        public virtual string GetCammerMode(string strFeatureName, ref List<string> enumEntryList) { return null; }


        /// <summary>
        /// 获取曝光时间和增益
        /// </summary>
        /// <returns></returns>
        public virtual string GetCameraExposure(string message) { return null; }

        /// <summary>
        /// 设置相机参数
        /// </summary>
        public virtual void SetCameraMode(string message, string value) {; }

        /// <summary>
        /// 设置相机触发模式
        /// </summary>
        /// <param name="message"></param>
        /// <param name="value"></param>
        public virtual bool SetCameraExposure(float exptime) { return true; }



        //2021-11-09,lfh
        /// <summary>
        /// 采集是否完成
        /// </summary>
        /// <returns></returns>
        public virtual bool GrabDone()
        {
            return true;
        }


        public virtual float GetTriggerTime() { return 0; }


        public virtual bool SetTriggerDelayTime(float fTime) { return true; }

        public virtual float GetTriggerDelayTime() { return 0; }


        /// <summary>
        /// 设置相机采集行高
        /// </summary>
        public virtual bool SetYLenght(float YLength, string IP_Address)
        {
            return true;
        }


        /// <summary>
        /// 设置相机工作站
        /// </summary>
        public virtual bool SetYLenght1(string JobName)
        {
            return true;
        }

        #endregion

        #endregion

        /// <summary>
        /// 软触发
        /// </summary>
        /// <returns></returns>
        public virtual bool SoftwareTrigger() { return false; }


        /// <summary>
        /// 设置最小深度
        /// </summary>
        public virtual double SetDeepMin
        {
            get { return deep_min; }
            set
            { deep_min = value; }
        }

        /// <summary>
        /// 设置最大深度
        /// </summary>
        public virtual double SetDeepMax
        {
            get { return deep_max; }
            set
            { deep_max = value; }
        }


        /// <summary>
        /// 设置软件触发IO
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns>0:失败，1成功</returns>
        public virtual int OutIO(int nValue)
        {
            return 0;
        }


        public void Write_Log(string strLog)
        {
            switch (CamID)
            {
                case 0:
                    Log4netRecord.RunLog.Info(strLog);
                    break;
                case 1:
                    Log4netRecord.RunLog.Info(strLog);
                    break;
                case 2:
                    Log4netRecord.RunLog.Info(strLog);
                    break;
                case 3:
                    Log4netRecord.RunLog.Info(strLog);
                    break;
            }
        }
    }
}
