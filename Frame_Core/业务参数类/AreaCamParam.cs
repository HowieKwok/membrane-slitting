using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Frame_Core
{
    public class AreaCamParam : BaseProParam
    {
        public SemaphoreSlim semaphoreSlimCam = new SemaphoreSlim(0, 100);
        /// <summary>
        /// 相机名字
        /// </summary>
        private string strCamreaName { get; }
        /// <summary>
        /// 相机的图像ID，默认第一片为1，预处理使用
        /// </summary>
        public long nCurProImgId = 1;

        /// <summary>
        /// 销毁的图像数
        /// </summary>
        public int disposeImageCount = 0;

        /// <summary>
        /// 设别通讯的EA单片条码，每个单片一个码
        /// </summary>
        public string strEaCode = "";

        /// <summary>
        /// 虚拟条码列表
        /// </summary>
        private List<string> strVirtualCodeList = new List<string>();


        /// <summary>
        /// 第几个电芯包
        /// </summary>
        public int nPackgeNum = 1;

        /// <summary>
        /// 第几个单片EA，绝对计数，单工位处理EA的总数
        /// </summary>
        public int nEANum = 1;

        /// <summary>
        /// 单片编号，周期计数。单片在电芯包中的编号。
        /// </summary>
        public int nEAIndex = 1;

        /// <summary>
        /// 判断第一片是否开始，用于防呆
        /// </summary>
        public bool bIsStartShoot = false;

        /// <summary>
        /// 判断第二片是否开始，用于防呆
        /// </summary>
        public bool bIsEndShoot = false;



        public AreaCamParam(string cameraName)
        {
            strCamreaName = cameraName;
        }


        public void SetVirtualCodeList(string strEaCode)
        {
            strVirtualCodeList.Add(strEaCode);
        }



        public override void CallBackImgDataDeal()
        {

            if (QueueGrabImage.Count > 0)
            {
                //预定位自动标定功能
                if (Project.Instance.IsAutoCalibrationModel)
                {
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 ||
                        Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位 ||
                        Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 ||
                        Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                    {
                        ImageInfo InfoImgInfo = QueueGrabImage.Dequeue();
                        //进入自动标定模式

                        AutoCalibrationDelegate?.Invoke((CameraEnum)cameraEnum, InfoImgInfo.ho_image);
                        return;
                    }
                }

                //没有运行（暂停），且 不是仿真模式（在线模式）
                if (!Project.Instance.GetStates() && !Project.Instance.Is_simulation)
                {
                    ImageInfo imageInfo = QueueGrabImage.Dequeue();
                    disposeImageCount++;
                    Log4netRecord.RunLog.Debug(strCamreaName + "销毁回调的图像，图像帧号为:" + imageInfo.image_ID + "\n" + "销毁的图像数：" + disposeImageCount);
                    imageInfo.Dispose();
                }
                else
                {
                    disposeImageCount = 0;
                    ImageInfo InfoImgInfo = QueueGrabImage.Dequeue();
                    try
                    {
                        if (InfoImgInfo != null)
                        {
                            if (InfoImgInfo.image_ID != 0)
                            {
                                InfoImgInfo.image_ID = nCurProImgId;


                                //OVH工位需要绑定设备信息
                                bool isSimImgDispose = false;
                                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位 ||
                                    Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                                {
                                    ReadDevInfo(ref InfoImgInfo, out isSimImgDispose);
                                }


                                if (!isSimImgDispose)
                                {
                                    alreadyDealGrabImage.Enqueue(InfoImgInfo);
                                    nCurProImgId++;
                                    semaphoreSlimCam.Release();
                                }

                            }
                        }
                        else
                        {
                            MessageBox.Show(strCamreaName + "图像来源为空");
                        }

                    }
                    catch (Exception ee)
                    {

                        MessageBox.Show(strCamreaName + "图像来源为空:" + ee.ToString());
                    }

                }

            }

        }

        public override void Dispose()
        {
            nCurProImgId = 1;
            disposeImageCount = 0;
            strVirtualCodeList.Clear();
            base.Dispose();
            int ct = semaphoreSlimCam.CurrentCount;
            for (int i = 0; i < ct; i++)
            {
                semaphoreSlimCam.Wait();
            }
            nPackgeNum = 0;
            nEANum = 1;
            nEAIndex = 1;
            bIsStartShoot = true;
            bIsEndShoot = false;

        }



        #region OVH工位业务

        public void ReadDevInfo(ref ImageInfo infoImgInfo, out bool isSimImgDispose)
        {
            isSimImgDispose = false;
            try
            {

                if (Project.Instance.Is_simulation)
                {
                    return;
                }
                else
                {
                    infoImgInfo.otherInfo.PackgeNum = Project.Instance.cutOverlayPLCManage.OVH_ReadUpOrDownCell();

                    //相机1到相机4是A叠台，相机5到相机8是B叠台
                    if (cameraEnum == CameraEnum.Cam1 ||
                        cameraEnum == CameraEnum.Cam2 ||
                        cameraEnum == CameraEnum.Cam3 ||
                        cameraEnum == CameraEnum.Cam4)
                    {
                        infoImgInfo.otherInfo.CurrentLayer = Project.Instance.cutOverlayPLCManage.OVH_ReadCurrentLayerA();
                        infoImgInfo.otherInfo.AffiliationStation = 1;

                        Project.Instance.cutOverlayPLCManage.OVH_ReadUpAndDownTabneckValue_APlatform(out infoImgInfo.otherInfo.UpTabneckValue, out infoImgInfo.otherInfo.DownTabneckValue);

                        Project.Instance.cutOverlayPLCManage.OVHX_A_ReadYDWSizeValue(out infoImgInfo.otherInfo.PoleWidth1, out infoImgInfo.otherInfo.PoleWidth2, out infoImgInfo.otherInfo.PoleHeight1, out infoImgInfo.otherInfo.PoleHeight2,
                                                                                        out infoImgInfo.otherInfo.PoleEarW, out infoImgInfo.otherInfo.PoleEarH, out infoImgInfo.otherInfo.PoleEarD);


                    }
                    else if (cameraEnum == CameraEnum.Cam5 ||
                             cameraEnum == CameraEnum.Cam6 ||
                             cameraEnum == CameraEnum.Cam7 ||
                             cameraEnum == CameraEnum.Cam8)
                    {
                        infoImgInfo.otherInfo.CurrentLayer = Project.Instance.cutOverlayPLCManage.OVH_ReadCurrentLayerB();
                        infoImgInfo.otherInfo.AffiliationStation = 2;

                        Project.Instance.cutOverlayPLCManage.OVH_ReadUpAndDownTabneckValue_BPlatform(out infoImgInfo.otherInfo.UpTabneckValue, out infoImgInfo.otherInfo.DownTabneckValue);
                        Project.Instance.cutOverlayPLCManage.OVHX_B_ReadYDWSizeValue(out infoImgInfo.otherInfo.PoleWidth1, out infoImgInfo.otherInfo.PoleWidth2, out infoImgInfo.otherInfo.PoleHeight1, out infoImgInfo.otherInfo.PoleHeight2,
                                                                                      out infoImgInfo.otherInfo.PoleEarW, out infoImgInfo.otherInfo.PoleEarH, out infoImgInfo.otherInfo.PoleEarD);
                    }
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.RunLog.Error("读取设备信息失败：", ex);
                TJBJInfo.AlarmWriteState(AlarmName.读取设备信息错误, StopAlarmStateEnum.停机, "");
            }
        }

        #endregion
    }
}
