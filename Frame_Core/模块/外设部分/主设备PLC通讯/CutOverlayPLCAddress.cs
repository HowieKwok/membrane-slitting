using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class CutOverlayPLCAddress : XmlSerializationOperation
    {
        #region 上单面检测
        [Description("上单面纠偏相机心跳信号")]
        public CommunicationData<short> Out_CorrectHeart_CCD_TopPlane = new CommunicationData<short>();

        [Description("上单面线扫相机心跳信号")]
        public CommunicationData<short> Out_LineHeart_CCD_TopPlane = new CommunicationData<short>();

        [Description("上单面线扫距离")]
        public CommunicationData<float> In_LineCamDist_TopPlane = new CommunicationData<float>();

        [Description("上单面放卷纠偏相机拍照信号")]
        public CommunicationData<short> In_UnreelingCorrectPhotoSignal_TopPlane = new CommunicationData<short>();

        [Description("上单面放卷纠偏相机返回结果")]
        public CommunicationData<short> Out_UnreelingCorrectReturnResult_TopPlane = new CommunicationData<short>();

        [Description("上单面放卷纠偏相机返回纠偏值")]
        public CommunicationData<float> Out_UnreelingCorrectReturnOffsetValue_TopPlane = new CommunicationData<float>();

        [Description("上单面上线扫相机拍照信号")]
        public CommunicationData<short> In_UpLineCamPhotoSignal_TopPlane = new CommunicationData<short>();

        [Description("上单面上线扫相机返回结果")]
        public CommunicationData<short> Out_UpLineCamReturnResult_TopPlane = new CommunicationData<short>();

        [Description("上单面下线扫相机拍照信号")]
        public CommunicationData<short> In_DownLineCamPhotoSignal_TopPlane = new CommunicationData<short>();

        [Description("上单面下线扫相机返回结果")]
        public CommunicationData<short> Out_DownLineCamReturnResult_TopPlane = new CommunicationData<short>();

        [Description("上单面模切纠偏相机拍照信号")]
        public CommunicationData<short> In_DieCutCorrectPhotoSignal_TopPlane = new CommunicationData<short>();

        [Description("上单面模切纠偏相机返回结果")]
        public CommunicationData<short> Out_DieCutCorrectReturnResult_TopPlane = new CommunicationData<short>();

        [Description("上单面模切纠偏相机返回结果2")]
        public CommunicationData<short> Out_DieCutCorrectReturnResult2_TopPlane = new CommunicationData<short>();

        [Description("上单面模切纠偏相机返回纠偏值X")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueX_TopPlane = new CommunicationData<float>();

        [Description("上单面模切纠偏相机返回纠偏值Y")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueY_TopPlane = new CommunicationData<float>();

        [Description("上单面模切纠偏相机返回纠偏值XR")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueXR_TopPlane = new CommunicationData<float>();

        [Description("上单面模切纠偏相机返回纠偏值YR")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueYR_TopPlane = new CommunicationData<float>();

        [Description("上单面上Tabneck相机拍照信号")]
        public CommunicationData<short> In_UpTabneckCamPhotoSignal_TopPlane = new CommunicationData<short>();

        [Description("上单面上Tabneck相机返回结果")]
        public CommunicationData<short> Out_UpTabneckCamReturnResult_TopPlane = new CommunicationData<short>();

        [Description("上单面上Tabneck相机返回Tabneck值")]
        public CommunicationData<float> Out_UpTabneckCamReturnTabneckValue_TopPlane = new CommunicationData<float>();

        [Description("上单面机器运行状态")]
        public CommunicationData<float> In_MachineRunningState_TopPlane = new CommunicationData<float>();

        //20250307qm添加模切纠偏工位，读取PLC实际纠偏值
        [Description("上单面_触发PC读取纠偏量信号")]
        public CommunicationData<bool> InOut_DieCutCorrectReadOffsetTrigger_TopPlane = new CommunicationData<bool>();

        [Description("上单面_纠偏Y轴纠偏量")]
        public CommunicationData<float> In_DieCutCorrectOffsetValueY_TopPlane = new CommunicationData<float>();

        [Description("上单面_纠偏R轴纠偏量")]
        public CommunicationData<float> In_DieCutCorrectOffsetValueR_TopPlane = new CommunicationData<float>();


        //20250318qm添加PLC下发一键换卷
        [Description("上单面_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_TopPlane = new CommunicationData<bool>();

        [Description("上单面_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_TopPlane = new CommunicationData<bool>();


        #endregion

        #region 下单面检测
        [Description("下单面纠偏相机心跳信号")]
        public CommunicationData<short> Out_CorrectHeart_CCD_BottomPlane = new CommunicationData<short>();

        [Description("下单面线扫相机心跳信号")]
        public CommunicationData<short> Out_LineHeart_CCD_BottomPlane = new CommunicationData<short>();

        [Description("下单面线扫距离")]
        public CommunicationData<float> In_LineCamDist_BottomPlane = new CommunicationData<float>();

        [Description("下单面放卷纠偏相机拍照信号")]
        public CommunicationData<short> In_UnreelingCorrectPhotoSignal_BottomPlane = new CommunicationData<short>();

        [Description("下单面放卷纠偏相机返回结果")]
        public CommunicationData<short> Out_UnreelingCorrectReturnResult_BottomPlane = new CommunicationData<short>();

        [Description("下单面放卷纠偏相机返回纠偏值")]
        public CommunicationData<float> Out_UnreelingCorrectReturnOffsetValue_BottomPlane = new CommunicationData<float>();

        [Description("下单面上线扫相机拍照信号")]
        public CommunicationData<short> In_UpLineCamPhotoSignal_BottomPlane = new CommunicationData<short>();

        [Description("下单面上线扫相机返回结果")]
        public CommunicationData<short> Out_UpLineCamReturnResult_BottomPlane = new CommunicationData<short>();

        [Description("下单面下线扫相机拍照信号")]
        public CommunicationData<short> In_DownLineCamPhotoSignal_BottomPlane = new CommunicationData<short>();

        [Description("下单面下线扫相机返回结果")]
        public CommunicationData<short> Out_DownLineCamReturnResult_BottomPlane = new CommunicationData<short>();

        [Description("下单面模切纠偏相机拍照信号")]
        public CommunicationData<short> In_DieCutCorrectPhotoSignal_BottomPlane = new CommunicationData<short>();

        [Description("下单面模切纠偏相机返回结果")]
        public CommunicationData<short> Out_DieCutCorrectReturnResult_BottomPlane = new CommunicationData<short>();

        [Description("下单面模切纠偏相机返回结果2")]
        public CommunicationData<short> Out_DieCutCorrectReturnResult2_BottomPlane = new CommunicationData<short>();

        [Description("下单面模切纠偏相机返回纠偏值X")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueX_BottomPlane = new CommunicationData<float>();

        [Description("下单面模切纠偏相机返回纠偏值Y")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueY_BottomPlane = new CommunicationData<float>();

        [Description("下单面模切纠偏相机返回纠偏值XR")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueXR_BottomPlane = new CommunicationData<float>();

        [Description("下单面模切纠偏相机返回纠偏值YR")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueYR_BottomPlane = new CommunicationData<float>();

        [Description("下单面上Tabneck相机拍照信号")]
        public CommunicationData<short> In_UpTabneckCamPhotoSignal_BottomPlane = new CommunicationData<short>();

        [Description("下单面上Tabneck相机返回结果")]
        public CommunicationData<short> Out_UpTabneckCamReturnResult_BottomPlane = new CommunicationData<short>();

        [Description("下单面上Tabneck相机返回Tabneck值")]
        public CommunicationData<float> Out_UpTabneckCamReturnTabneckValue_BottomPlane = new CommunicationData<float>();

        [Description("下单面机器运行状态")]
        public CommunicationData<float> In_MachineRunningState_BottomPlane = new CommunicationData<float>();


        //20250307qm添加模切纠偏工位，读取PLC实际纠偏值
        [Description("下单面_触发PC读取纠偏量信号")]
        public CommunicationData<bool> InOut_DieCutCorrectReadOffsetTrigger_BottomPlane = new CommunicationData<bool>();

        [Description("下单面_纠偏Y轴纠偏量")]
        public CommunicationData<float> In_DieCutCorrectOffsetValueY_BottomPlane = new CommunicationData<float>();

        [Description("下单面_纠偏R轴纠偏量")]
        public CommunicationData<float> In_DieCutCorrectOffsetValueR_BottomPlane = new CommunicationData<float>();

        //20250318qm添加PLC下发一键换卷
        [Description("下单面_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_BottomPlane = new CommunicationData<bool>();

        [Description("下单面_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_BottomPlane = new CommunicationData<bool>();

        #endregion

        #region A模切检测
        [Description("A模切纠偏相机心跳信号")]
        public CommunicationData<short> Out_CorrectHeart_CCD_ADie = new CommunicationData<short>();

        [Description("A模切线扫相机心跳信号")]
        public CommunicationData<short> Out_LineHeart_CCD_ADie = new CommunicationData<short>();

        [Description("A模切线扫距离")]
        public CommunicationData<float> In_LineCamDist_ADie = new CommunicationData<float>();

        [Description("A模切放卷纠偏相机拍照信号")]
        public CommunicationData<short> In_UnreelingCorrectPhotoSignal_ADie = new CommunicationData<short>();

        [Description("A模切放卷纠偏相机返回结果")]
        public CommunicationData<short> Out_UnreelingCorrectReturnResult_ADie = new CommunicationData<short>();

        [Description("A模切放卷纠偏相机返回纠偏值")]
        public CommunicationData<float> Out_UnreelingCorrectReturnOffsetValue_ADie = new CommunicationData<float>();

        [Description("A模切上线扫相机拍照信号")]
        public CommunicationData<short> In_UpLineCamPhotoSignal_ADie = new CommunicationData<short>();

        [Description("A模切上线扫相机返回结果")]
        public CommunicationData<short> Out_UpLineCamReturnResult_ADie = new CommunicationData<short>();

        [Description("A模切下线扫相机拍照信号")]
        public CommunicationData<short> In_DownLineCamPhotoSignal_ADie = new CommunicationData<short>();

        [Description("A模切下线扫相机返回结果")]
        public CommunicationData<short> Out_DownLineCamReturnResult_ADie = new CommunicationData<short>();

        [Description("A模切模切纠偏相机拍照信号")]
        public CommunicationData<short> In_DieCutCorrectPhotoSignal_ADie = new CommunicationData<short>();

        [Description("A模切模切纠偏相机返回结果")]
        public CommunicationData<short> Out_DieCutCorrectReturnResult_ADie = new CommunicationData<short>();

        [Description("A模切模切纠偏相机返回结果2")]
        public CommunicationData<short> Out_DieCutCorrectReturnResult2_ADie = new CommunicationData<short>();

        [Description("A模切模切纠偏相机返回纠偏值X")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueX_ADie = new CommunicationData<float>();

        [Description("A模切模切纠偏相机返回纠偏值Y")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueY_ADie = new CommunicationData<float>();

        [Description("A模切模切纠偏相机返回纠偏值XR")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueXR_ADie = new CommunicationData<float>();

        [Description("A模切模切纠偏相机返回纠偏值YR")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueYR_ADie = new CommunicationData<float>();

        [Description("A模切上Tabneck相机拍照信号")]
        public CommunicationData<short> In_UpTabneckCamPhotoSignal_ADie = new CommunicationData<short>();

        [Description("A模切上Tabneck相机返回结果")]
        public CommunicationData<short> Out_UpTabneckCamReturnResult_ADie = new CommunicationData<short>();

        [Description("A模切上Tabneck相机返回Tabneck值")]
        public CommunicationData<float> Out_UpTabneckCamReturnTabneckValue_ADie = new CommunicationData<float>();

        [Description("A模切下Tabneck相机拍照信号")]
        public CommunicationData<short> In_DownTabneckCamPhotoSignal_ADie = new CommunicationData<short>();

        [Description("A模切下Tabneck相机返回结果")]
        public CommunicationData<short> Out_DownTabneckCamReturnResult_ADie = new CommunicationData<short>();

        [Description("A模切下Tabneck相机返回Tabneck值1")]
        public CommunicationData<float> Out_DownTabneckCamReturnTabneckValue1_ADie = new CommunicationData<float>();

        [Description("A模切下Tabneck相机返回Tabneck值2")]
        public CommunicationData<float> Out_DownTabneckCamReturnTabneckValue2_ADie = new CommunicationData<float>();

        [Description("A模切机器运行状态")]
        public CommunicationData<float> In_MachineRunningState_ADie=new CommunicationData<float>();


        //20250307qm添加模切纠偏工位，读取PLC实际纠偏值
        [Description("A模切_触发PC读取纠偏量信号")]
        public CommunicationData<bool> InOut_DieCutCorrectReadOffsetTrigger_ADie = new CommunicationData<bool>();

        [Description("A模切_纠偏Y轴纠偏量")]
        public CommunicationData<float> In_DieCutCorrectOffsetValueY_ADie = new CommunicationData<float>();

        [Description("A模切_纠偏R轴纠偏量")]
        public CommunicationData<float> In_DieCutCorrectOffsetValueR_ADie = new CommunicationData<float>();

        //20250318qm添加PLC下发一键换卷
        [Description("A模切_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_ADie = new CommunicationData<bool>();

        [Description("A模切_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_ADie = new CommunicationData<bool>();

        #endregion

        #region B模切检测
        [Description("B模切纠偏相机心跳信号")]
        public CommunicationData<short> Out_CorrectHeart_CCD_BDie = new CommunicationData<short>();

        [Description("B模切线扫相机心跳信号")]
        public CommunicationData<short> Out_LineHeart_CCD_BDie = new CommunicationData<short>();

        [Description("B模切线扫距离")]
        public CommunicationData<float> In_LineCamDist_BDie = new CommunicationData<float>();

        [Description("B模切放卷纠偏相机拍照信号")]
        public CommunicationData<short> In_UnreelingCorrectPhotoSignal_BDie = new CommunicationData<short>();

        [Description("B模切放卷纠偏相机返回结果")]
        public CommunicationData<short> Out_UnreelingCorrectReturnResult_BDie = new CommunicationData<short>();

        [Description("B模切放卷纠偏相机返回纠偏值")]
        public CommunicationData<float> Out_UnreelingCorrectReturnOffsetValue_BDie = new CommunicationData<float>();

        [Description("B模切上线扫相机拍照信号")]
        public CommunicationData<short> In_UpLineCamPhotoSignal_BDie = new CommunicationData<short>();

        [Description("B模切上线扫相机返回结果")]
        public CommunicationData<short> Out_UpLineCamReturnResult_BDie = new CommunicationData<short>();

        [Description("B模切下线扫相机拍照信号")]
        public CommunicationData<short> In_DownLineCamPhotoSignal_BDie = new CommunicationData<short>();

        [Description("B模切下线扫相机返回结果")]
        public CommunicationData<short> Out_DownLineCamReturnResult_BDie = new CommunicationData<short>();

        [Description("B模切模切纠偏相机拍照信号")]
        public CommunicationData<short> In_DieCutCorrectPhotoSignal_BDie = new CommunicationData<short>();

        [Description("B模切模切纠偏相机返回结果")]
        public CommunicationData<short> Out_DieCutCorrectReturnResult_BDie = new CommunicationData<short>();

        [Description("B模切模切纠偏相机返回结果2")]
        public CommunicationData<short> Out_DieCutCorrectReturnResult2_BDie = new CommunicationData<short>();

        [Description("B模切模切纠偏相机返回纠偏值X")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueX_BDie = new CommunicationData<float>();

        [Description("B模切模切纠偏相机返回纠偏值Y")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueY_BDie = new CommunicationData<float>();

        [Description("B模切模切纠偏相机返回纠偏值XR")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueXR_BDie = new CommunicationData<float>();

        [Description("B模切模切纠偏相机返回纠偏值YR")]
        public CommunicationData<float> Out_DieCutCorrectReturnOffsetValueYR_BDie = new CommunicationData<float>();

        [Description("B模切上Tabneck相机拍照信号")]
        public CommunicationData<short> In_UpTabneckCamPhotoSignal_BDie = new CommunicationData<short>();

        [Description("B模切上Tabneck相机返回结果")]
        public CommunicationData<short> Out_UpTabneckCamReturnResult_BDie = new CommunicationData<short>();

        [Description("B模切上Tabneck相机返回Tabneck值")]
        public CommunicationData<float> Out_UpTabneckCamReturnTabneckValue_BDie = new CommunicationData<float>();

        [Description("B模切下Tabneck相机拍照信号")]
        public CommunicationData<short> In_DownTabneckCamPhotoSignal_BDie = new CommunicationData<short>();

        [Description("B模切下Tabneck相机返回结果")]
        public CommunicationData<short> Out_DownTabneckCamReturnResult_BDie = new CommunicationData<short>();

        [Description("B模切下Tabneck相机返回Tabneck值1")]
        public CommunicationData<float> Out_DownTabneckCamReturnTabneckValue1_BDie = new CommunicationData<float>();

        [Description("B模切下Tabneck相机返回Tabneck值2")]
        public CommunicationData<float> Out_DownTabneckCamReturnTabneckValue2_BDie = new CommunicationData<float>();

        [Description("B模切机器运行状态")]
        public CommunicationData<float> In_MachineRunningState_BDie = new CommunicationData<float>();


        //20250307qm添加模切纠偏工位，读取PLC实际纠偏值
        [Description("B模切_触发PC读取纠偏量信号")]
        public CommunicationData<bool> InOut_DieCutCorrectReadOffsetTrigger_BDie = new CommunicationData<bool>();

        [Description("B模切_纠偏Y轴纠偏量")]
        public CommunicationData<float> In_DieCutCorrectOffsetValueY_BDie = new CommunicationData<float>();

        [Description("B模切_纠偏R轴纠偏量")]
        public CommunicationData<float> In_DieCutCorrectOffsetValueR_BDie = new CommunicationData<float>();


        //20250318qm添加PLC下发一键换卷
        [Description("B模切_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_BDie = new CommunicationData<bool>();

        [Description("B模切_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_BDie = new CommunicationData<bool>();

        #endregion


        #region A面叠片A极片纠偏预定位
        [Description("A面叠片A极片纠偏心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_AAYDW = new CommunicationData<short>();

        [Description("A面叠片A极片纠偏拍照信号")]
        public CommunicationData<short> In_PhotoSignal_AAYDW = new CommunicationData<short>();

        [Description("A面叠片A极片纠偏返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_AAYDW = new CommunicationData<short>();

        [Description("A面叠片A极片纠偏返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_AAYDW = new CommunicationData<short>();

        [Description("A面叠片A极片纠偏拍照偏移值X")]
        public CommunicationData<float> Out_OffsetValueX_AAYDW = new CommunicationData<float>();
        [Description("A面叠片A极片纠偏拍照偏移值Y")]
        public CommunicationData<float> Out_OffsetValueY_AAYDW = new CommunicationData<float>();
        [Description("A面叠片A极片纠偏拍照偏移值R")]
        public CommunicationData<float> Out_OffsetValueR_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏新建膜卷")]
        public CommunicationData<float> Out_NewFilmRoll_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏运行/暂停")]
        public CommunicationData<float> Out_RunOrPause_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏停机信号")]
        public CommunicationData<float> Out_NGHalt_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏报警信号")]
        public CommunicationData<float> Out_NGAlarm_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏二维码")]
        public CommunicationData<string> In_QRCode_AAYDW = new CommunicationData<string>();

        [Description("A面叠片A极片纠偏X轴最大值")]
        public CommunicationData<float> In_XAxisMax_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏X轴最小值")]
        public CommunicationData<float> In_XAxisMin_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏R轴最大值")]
        public CommunicationData<float> In_RAxisMax_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏R轴最小值")]
        public CommunicationData<float> In_RAxisMin_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏全宽")]
        public CommunicationData<float> Out_PoleWidth1_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏宽2")]
        public CommunicationData<float> Out_PoleWidth2_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏全高")]
        public CommunicationData<float> Out_PoleHeight1_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏高2")]
        public CommunicationData<float> Out_PoleHeight2_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏极耳宽")]
        public CommunicationData<float> Out_PoleEarW_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏极耳高")]
        public CommunicationData<float> Out_PoleEarH_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏极耳距边")]
        public CommunicationData<float> Out_PoleEarD_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏X轴当前位置")]
        public CommunicationData<float> In_CorrAxisXPos_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏Y轴当前位置")]
        public CommunicationData<float> In_CorrAxisYPos_AAYDW = new CommunicationData<float>();

        [Description("A面叠片A极片纠偏R轴当前位置")]
        public CommunicationData<float> In_CorrAxisRPos_AAYDW = new CommunicationData<float>();

        //20250318qm添加PLC下发一键换卷
        [Description("A面叠片A极片纠偏_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_AAYDW = new CommunicationData<bool>();

        [Description("A面叠片A极片纠偏_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_AAYDW = new CommunicationData<bool>();

        #endregion

        #region A面叠片B极片纠偏预定位
        [Description("A面叠片B极片纠偏心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_ABYDW = new CommunicationData<short>();

        [Description("A面叠片B极片纠偏拍照信号")]
        public CommunicationData<short> In_PhotoSignal_ABYDW = new CommunicationData<short>();

        [Description("A面叠片B极片纠偏返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_ABYDW = new CommunicationData<short>();

        [Description("A面叠片B极片纠偏返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_ABYDW = new CommunicationData<short>();

        [Description("A面叠片B极片纠偏拍照偏移值X")]
        public CommunicationData<float> Out_OffsetValueX_ABYDW = new CommunicationData<float>();
        [Description("A面叠片B极片纠偏拍照偏移值Y")]
        public CommunicationData<float> Out_OffsetValueY_ABYDW = new CommunicationData<float>();
        [Description("A面叠片B极片纠偏拍照偏移值R")]
        public CommunicationData<float> Out_OffsetValueR_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏新建膜卷")]
        public CommunicationData<float> Out_NewFilmRoll_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏运行/暂停")]
        public CommunicationData<float> Out_RunOrPause_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏停机信号")]
        public CommunicationData<float> Out_NGHalt_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏报警信号")]
        public CommunicationData<float> Out_NGAlarm_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏二维码")]
        public CommunicationData<string> In_QRCode_ABYDW = new CommunicationData<string>();

        [Description("A面叠片B极片纠偏X轴最大值")]
        public CommunicationData<float> In_XAxisMax_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏X轴最小值")]
        public CommunicationData<float> In_XAxisMin_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏R轴最大值")]
        public CommunicationData<float> In_RAxisMax_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏R轴最小值")]
        public CommunicationData<float> In_RAxisMin_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏全宽")]
        public CommunicationData<float> Out_PoleWidth1_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏宽2")]
        public CommunicationData<float> Out_PoleWidth2_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏全高")]
        public CommunicationData<float> Out_PoleHeight1_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏高2")]
        public CommunicationData<float> Out_PoleHeight2_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏极耳宽")]
        public CommunicationData<float> Out_PoleEarW_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏极耳高")]
        public CommunicationData<float> Out_PoleEarH_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏极耳距边")]
        public CommunicationData<float> Out_PoleEarD_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏X轴当前位置")]
        public CommunicationData<float> In_CorrAxisXPos_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏Y轴当前位置")]
        public CommunicationData<float> In_CorrAxisYPos_ABYDW = new CommunicationData<float>();

        [Description("A面叠片B极片纠偏R轴当前位置")]
        public CommunicationData<float> In_CorrAxisRPos_ABYDW = new CommunicationData<float>();

        //20250318qm添加PLC下发一键换卷
        [Description("A面叠片B极片纠偏_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_ABYDW = new CommunicationData<bool>();

        [Description("A面叠片B极片纠偏_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_ABYDW = new CommunicationData<bool>();

        #endregion

        #region A面叠片A极片复检OVH'
        [Description("A面叠片A极片复检心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_AAOVH = new CommunicationData<short>();

        [Description("A面叠片A极片复检拍照信号")]
        public CommunicationData<short> In_PhotoSignal_AAOVH = new CommunicationData<short>();

        [Description("A面叠片A极片复检返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_AAOVH = new CommunicationData<short>();

        [Description("A面叠片A极片复检返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_AAOVH = new CommunicationData<short>();

        [Description("A面叠片A极片复检当前层数")]
        public CommunicationData<float> In_CurrentLayer_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检新建膜卷")]
        public CommunicationData<float> Out_NewFilmRoll_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检运行/暂停")]
        public CommunicationData<float> Out_RunOrPause_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检停机信号")]
        public CommunicationData<float> Out_NGHalt_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检报警信号")]
        public CommunicationData<float> Out_NGAlarm_AAOVH = new CommunicationData<float>();


        [Description("A面叠片A极片复检二维码")]
        public CommunicationData<string> In_QRCode_AAOVH = new CommunicationData<string>();

        [Description("A面叠片A极片复检X轴最大值")]
        public CommunicationData<float> In_XAxisMax_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检X轴最小值")]
        public CommunicationData<float> In_XAxisMin_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检R轴最大值")]
        public CommunicationData<float> In_RAxisMax_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检R轴最小值")]
        public CommunicationData<float> In_RAxisMin_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检上Tabneck值")]
        public CommunicationData<float> In_UpTabneckValue_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检下Tabneck值")]
        public CommunicationData<float> In_DownTabneckValue_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检极片全宽")]
        public CommunicationData<float> In_PoleWidth1_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检极片宽2")]
        public CommunicationData<float> In_PoleWidth2_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检极片全高")]
        public CommunicationData<float> In_PoleHeight1_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检极片高2")]
        public CommunicationData<float> In_PoleHeight2_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检极片极耳宽")]
        public CommunicationData<float> In_PoleEarW_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检极片极耳高")]
        public CommunicationData<float> In_PoleEarH_AAOVH = new CommunicationData<float>();

        [Description("A面叠片A极片复检极片极耳距边")]
        public CommunicationData<float> In_PoleEarD_AAOVH = new CommunicationData<float>();


        #endregion

        #region A面叠片B极片复检OVH
        [Description("A面叠片B极片复检心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_ABOVH = new CommunicationData<short>();

        [Description("A面叠片B极片复检拍照信号")]
        public CommunicationData<short> In_PhotoSignal_ABOVH = new CommunicationData<short>();

        [Description("A面叠片B极片复检返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_ABOVH = new CommunicationData<short>();

        [Description("A面叠片B极片复检返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_ABOVH = new CommunicationData<short>();

        [Description("A面叠片B极片复检当前层数")]
        public CommunicationData<float> In_CurrentLayer_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检新建膜卷")]
        public CommunicationData<float> Out_NewFilmRoll_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检运行/暂停")]
        public CommunicationData<float> Out_RunOrPause_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检停机信号")]
        public CommunicationData<float> Out_NGHalt_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检报警信号")]
        public CommunicationData<float> Out_NGAlarm_ABOVH = new CommunicationData<float>();


        [Description("A面叠片B极片复检二维码")]
        public CommunicationData<string> In_QRCode_ABOVH = new CommunicationData<string>();

        [Description("A面叠片B极片复检X轴最大值")]
        public CommunicationData<float> In_XAxisMax_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检X轴最小值")]
        public CommunicationData<float> In_XAxisMin_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检R轴最大值")]
        public CommunicationData<float> In_RAxisMax_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检R轴最小值")]
        public CommunicationData<float> In_RAxisMin_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检上Tabneck值")]
        public CommunicationData<float> In_UpTabneckValue_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检下Tabneck值")]
        public CommunicationData<float> In_DownTabneckValue_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检极片全宽")]
        public CommunicationData<float> In_PoleWidth1_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检极片宽2")]
        public CommunicationData<float> In_PoleWidth2_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检极片全高")]
        public CommunicationData<float> In_PoleHeight1_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检极片高2")]
        public CommunicationData<float> In_PoleHeight2_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检极片极耳宽")]
        public CommunicationData<float> In_PoleEarW_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检极片极耳高")]
        public CommunicationData<float> In_PoleEarH_ABOVH = new CommunicationData<float>();

        [Description("A面叠片B极片复检极片极耳距边")]
        public CommunicationData<float> In_PoleEarD_ABOVH = new CommunicationData<float>();
        #endregion

        #region A面叠片电芯结果
        [Description("A面叠片电芯结果心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_AOVHResult = new CommunicationData<short>();

        [Description("A面叠片电芯结果结束信号")]
        public CommunicationData<short> In_EndSignal_AOVHResult = new CommunicationData<short>();

        [Description("A面叠片电芯结果返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_AOVHResult = new CommunicationData<short>();

        [Description("A面叠片电芯结果返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_AOVHResult = new CommunicationData<short>();

        [Description("A面叠片电芯结果开始信号")]
        public CommunicationData<float> In_BeginSignal_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果开始信号反馈")]
        public CommunicationData<float> Out_BeginSignalFeedback_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果上或下电芯")]
        public CommunicationData<float> In_UpOrDownCell_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果设备启停信号")]
        public CommunicationData<float> In_StartStopSignal_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果二维码")]
        public CommunicationData<string> In_QRCode_AOVHResult = new CommunicationData<string>();

        [Description("A面叠片电芯结果X轴最大值")]
        public CommunicationData<float> In_XAxisMax_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果X轴最小值")]
        public CommunicationData<float> In_XAxisMin_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果R轴最大值")]
        public CommunicationData<float> In_RAxisMax_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片电芯结果R轴最小值")]
        public CommunicationData<float> In_RAxisMin_AOVHResult = new CommunicationData<float>();

        [Description("A面叠片A极片复检机器运行状态")]
        public CommunicationData<float> In_MachineRunningState_AOVHResult = new CommunicationData<float>();


        //20250318qm添加PLC下发一键换卷
        [Description("A面叠片_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_AOVHResult = new CommunicationData<bool>();

        [Description("A面叠片_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_AOVHResult = new CommunicationData<bool>();

        #endregion

        #region A面叠片A极片标定
        [Description("A面叠片A极片标定进入标定模式")]
        public CommunicationData<short> In_StratCaliSignal_AACali = new CommunicationData<short>();

        [Description("A面叠片A极片标定拍照信号")]
        public CommunicationData<short> In_PhotoSignal_AACali = new CommunicationData<short>();

        [Description("A面叠片A极片标定返回结果")]
        public CommunicationData<short> Out_ReturnResult_AACali = new CommunicationData<short>();

        [Description("A面叠片A极片标定返回X轴坐标")]
        public CommunicationData<float> Out_ReturnXAxisCoord_AACali = new CommunicationData<float>();

        [Description("A面叠片A极片标定返回Y轴坐标")]
        public CommunicationData<float> Out_ReturnYAxisCoord_AACali = new CommunicationData<float>();

        [Description("A面叠片A极片标定返回R轴坐标")]
        public CommunicationData<float> Out_ReturnRAxisCoord_AACali = new CommunicationData<float>();

        [Description("A面叠片A极片标定PLC当前X轴坐标")]
        public CommunicationData<float> In_PLCCurrentXAxisCoord_AACali = new CommunicationData<float>();

        [Description("A面叠片A极片标定PLC当前Y轴坐标")]
        public CommunicationData<float> In_PLCCurrentYAxisCoord_AACali = new CommunicationData<float>();

        [Description("A面叠片A极片标定PLC当前R轴坐标")]
        public CommunicationData<float> In_PLCCurrentRAxisCoord_AACali = new CommunicationData<float>();
        #endregion

        #region A面叠片B极片标定
        [Description("A面叠片B极片标定进入标定模式")]
        public CommunicationData<short> In_StratCaliSignal_ABCali = new CommunicationData<short>();

        [Description("A面叠片B极片标定拍照信号")]
        public CommunicationData<short> In_PhotoSignal_ABCali = new CommunicationData<short>();

        [Description("A面叠片B极片标定返回结果")]
        public CommunicationData<short> Out_ReturnResult_ABCali = new CommunicationData<short>();

        [Description("A面叠片B极片标定返回X轴坐标")]
        public CommunicationData<float> Out_ReturnXAxisCoord_ABCali = new CommunicationData<float>();

        [Description("A面叠片B极片标定返回Y轴坐标")]
        public CommunicationData<float> Out_ReturnYAxisCoord_ABCali = new CommunicationData<float>();

        [Description("A面叠片B极片标定返回R轴坐标")]
        public CommunicationData<float> Out_ReturnRAxisCoord_ABCali = new CommunicationData<float>();

        [Description("A面叠片B极片标定PLC当前X轴坐标")]
        public CommunicationData<float> In_PLCCurrentXAxisCoord_ABCali = new CommunicationData<float>();

        [Description("A面叠片B极片标定PLC当前Y轴坐标")]
        public CommunicationData<float> In_PLCCurrentYAxisCoord_ABCali = new CommunicationData<float>();

        [Description("A面叠片B极片标定PLC当前R轴坐标")]
        public CommunicationData<float> In_PLCCurrentRAxisCoord_ABCali = new CommunicationData<float>();
        #endregion


        #region B面叠片A极片纠偏预定位
        [Description("B面叠片A极片纠偏心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_BAYDW = new CommunicationData<short>();

        [Description("B面叠片A极片纠偏拍照信号")]
        public CommunicationData<short> In_PhotoSignal_BAYDW = new CommunicationData<short>();

        [Description("B面叠片A极片纠偏返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_BAYDW = new CommunicationData<short>();

        [Description("B面叠片A极片纠偏返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_BAYDW = new CommunicationData<short>();

        [Description("B面叠片A极片纠偏拍照偏移值X")]
        public CommunicationData<float> Out_OffsetValueX_BAYDW = new CommunicationData<float>();
        [Description("B面叠片A极片纠偏拍照偏移值Y")]
        public CommunicationData<float> Out_OffsetValueY_BAYDW = new CommunicationData<float>();
        [Description("B面叠片A极片纠偏拍照偏移值R")]
        public CommunicationData<float> Out_OffsetValueR_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏新建膜卷")]
        public CommunicationData<float> Out_NewFilmRoll_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏运行/暂停")]
        public CommunicationData<float> Out_RunOrPause_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏停机信号")]
        public CommunicationData<float> Out_NGHalt_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏报警信号")]
        public CommunicationData<float> Out_NGAlarm_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏二维码")]
        public CommunicationData<string> In_QRCode_BAYDW = new CommunicationData<string>();

        [Description("B面叠片A极片纠偏X轴最大值")]
        public CommunicationData<float> In_XAxisMax_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏X轴最小值")]
        public CommunicationData<float> In_XAxisMin_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏R轴最大值")]
        public CommunicationData<float> In_RAxisMax_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏R轴最小值")]
        public CommunicationData<float> In_RAxisMin_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏全宽")]
        public CommunicationData<float> Out_PoleWidth1_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏宽2")]
        public CommunicationData<float> Out_PoleWidth2_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏全高")]
        public CommunicationData<float> Out_PoleHeight1_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏高2")]
        public CommunicationData<float> Out_PoleHeight2_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏极耳宽")]
        public CommunicationData<float> Out_PoleEarW_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏极耳高")]
        public CommunicationData<float> Out_PoleEarH_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏极耳距边")]
        public CommunicationData<float> Out_PoleEarD_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏X轴当前位置")]
        public CommunicationData<float> In_CorrAxisXPos_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏Y轴当前位置")]
        public CommunicationData<float> In_CorrAxisYPos_BAYDW = new CommunicationData<float>();

        [Description("B面叠片A极片纠偏R轴当前位置")]
        public CommunicationData<float> In_CorrAxisRPos_BAYDW = new CommunicationData<float>();


        //20250318qm添加PLC下发一键换卷
        [Description("B面叠片A极片纠偏_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_BAYDW = new CommunicationData<bool>();

        [Description("B面叠片A极片纠偏_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_BAYDW = new CommunicationData<bool>();

        #endregion

        #region B面叠片B极片纠偏预定位
        [Description("B面叠片B极片纠偏心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_BBYDW = new CommunicationData<short>();

        [Description("B面叠片B极片纠偏拍照信号")]
        public CommunicationData<short> In_PhotoSignal_BBYDW = new CommunicationData<short>();

        [Description("B面叠片B极片纠偏返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_BBYDW = new CommunicationData<short>();

        [Description("B面叠片B极片纠偏返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_BBYDW = new CommunicationData<short>();

        [Description("B面叠片B极片纠偏拍照偏移值X")]
        public CommunicationData<float> Out_OffsetValueX_BBYDW = new CommunicationData<float>();
        [Description("B面叠片B极片纠偏拍照偏移值Y")]
        public CommunicationData<float> Out_OffsetValueY_BBYDW = new CommunicationData<float>();
        [Description("B面叠片B极片纠偏拍照偏移值R")]
        public CommunicationData<float> Out_OffsetValueR_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏新建膜卷")]
        public CommunicationData<float> Out_NewFilmRoll_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏运行/暂停")]
        public CommunicationData<float> Out_RunOrPause_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏停机信号")]
        public CommunicationData<float> Out_NGHalt_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏报警信号")]
        public CommunicationData<float> Out_NGAlarm_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏二维码")]
        public CommunicationData<string> In_QRCode_BBYDW = new CommunicationData<string>();

        [Description("B面叠片B极片纠偏X轴最大值")]
        public CommunicationData<float> In_XAxisMax_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏X轴最小值")]
        public CommunicationData<float> In_XAxisMin_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏R轴最大值")]
        public CommunicationData<float> In_RAxisMax_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏R轴最小值")]
        public CommunicationData<float> In_RAxisMin_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏全宽")]
        public CommunicationData<float> Out_PoleWidth1_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏宽2")]
        public CommunicationData<float> Out_PoleWidth2_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏全高")]
        public CommunicationData<float> Out_PoleHeight1_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏高2")]
        public CommunicationData<float> Out_PoleHeight2_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏极耳宽")]
        public CommunicationData<float> Out_PoleEarW_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏极耳高")]
        public CommunicationData<float> Out_PoleEarH_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏极耳距边")]
        public CommunicationData<float> Out_PoleEarD_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏X轴当前位置")]
        public CommunicationData<float> In_CorrAxisXPos_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏Y轴当前位置")]
        public CommunicationData<float> In_CorrAxisYPos_BBYDW = new CommunicationData<float>();

        [Description("B面叠片B极片纠偏R轴当前位置")]
        public CommunicationData<float> In_CorrAxisRPos_BBYDW = new CommunicationData<float>();


        //20250318qm添加PLC下发一键换卷
        [Description("B面叠片B极片纠偏_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_BBYDW = new CommunicationData<bool>();

        [Description("B面叠片B极片纠偏_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_BBYDW = new CommunicationData<bool>();

        #endregion     

        #region B面叠片A极片复检OVH
        [Description("B面叠片A极片复检心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_BAOVH = new CommunicationData<short>();

        [Description("B面叠片A极片复检拍照信号")]
        public CommunicationData<short> In_PhotoSignal_BAOVH = new CommunicationData<short>();

        [Description("B面叠片A极片复检返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_BAOVH = new CommunicationData<short>();

        [Description("B面叠片A极片复检返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_BAOVH = new CommunicationData<short>();

        [Description("B面叠片A极片复检当前层数")]
        public CommunicationData<float> In_CurrentLayer_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检新建膜卷")]
        public CommunicationData<float> Out_NewFilmRoll_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检运行/暂停")]
        public CommunicationData<float> Out_RunOrPause_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检停机信号")]
        public CommunicationData<float> Out_NGHalt_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检报警信号")]
        public CommunicationData<float> Out_NGAlarm_BAOVH = new CommunicationData<float>();


        [Description("B面叠片A极片复检二维码")]
        public CommunicationData<string> In_QRCode_BAOVH = new CommunicationData<string>();

        [Description("B面叠片A极片复检X轴最大值")]
        public CommunicationData<float> In_XAxisMax_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检X轴最小值")]
        public CommunicationData<float> In_XAxisMin_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检R轴最大值")]
        public CommunicationData<float> In_RAxisMax_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检R轴最小值")]
        public CommunicationData<float> In_RAxisMin_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检上Tabneck值")]
        public CommunicationData<float> In_UpTabneckValue_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检下Tabneck值")]
        public CommunicationData<float> In_DownTabneckValue_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检极片全宽")]
        public CommunicationData<float> In_PoleWidth1_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检极片宽2")]
        public CommunicationData<float> In_PoleWidth2_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检极片全高")]
        public CommunicationData<float> In_PoleHeight1_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检极片高2")]
        public CommunicationData<float> In_PoleHeight2_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检极片极耳宽")]
        public CommunicationData<float> In_PoleEarW_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检极片极耳高")]
        public CommunicationData<float> In_PoleEarH_BAOVH = new CommunicationData<float>();

        [Description("B面叠片A极片复检极片极耳距边")]
        public CommunicationData<float> In_PoleEarD_BAOVH = new CommunicationData<float>();

        #endregion

        #region B面叠片B极片复检OVH
        [Description("B面叠片B极片复检心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_BBOVH = new CommunicationData<short>();

        [Description("B面叠片B极片复检拍照信号")]
        public CommunicationData<short> In_PhotoSignal_BBOVH = new CommunicationData<short>();

        [Description("B面叠片B极片复检返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_BBOVH = new CommunicationData<short>();

        [Description("B面叠片B极片复检返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_BBOVH = new CommunicationData<short>();

        [Description("B面叠片B极片复检当前层数")]
        public CommunicationData<float> In_CurrentLayer_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检新建膜卷")]
        public CommunicationData<float> Out_NewFilmRoll_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检运行/暂停")]
        public CommunicationData<float> Out_RunOrPause_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检停机信号")]
        public CommunicationData<float> Out_NGHalt_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检报警信号")]
        public CommunicationData<float> Out_NGAlarm_BBOVH = new CommunicationData<float>();


        [Description("B面叠片B极片复检二维码")]
        public CommunicationData<string> In_QRCode_BBOVH = new CommunicationData<string>();

        [Description("B面叠片B极片复检X轴最大值")]
        public CommunicationData<float> In_XAxisMax_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检X轴最小值")]
        public CommunicationData<float> In_XAxisMin_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检R轴最大值")]
        public CommunicationData<float> In_RAxisMax_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检R轴最小值")]
        public CommunicationData<float> In_RAxisMin_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检上Tabneck值")]
        public CommunicationData<float> In_UpTabneckValue_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检下Tabneck值")]
        public CommunicationData<float> In_DownTabneckValue_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检极片全宽")]
        public CommunicationData<float> In_PoleWidth1_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检极片宽2")]
        public CommunicationData<float> In_PoleWidth2_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检极片全高")]
        public CommunicationData<float> In_PoleHeight1_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检极片高2")]
        public CommunicationData<float> In_PoleHeight2_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检极片极耳宽")]
        public CommunicationData<float> In_PoleEarW_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检极片极耳高")]
        public CommunicationData<float> In_PoleEarH_BBOVH = new CommunicationData<float>();

        [Description("B面叠片B极片复检极片极耳距边")]
        public CommunicationData<float> In_PoleEarD_BBOVH = new CommunicationData<float>();


        #endregion

        #region B面叠片电芯结果
        [Description("B面叠片电芯结果心跳信号")]
        public CommunicationData<short> Out_Heart_CCD_BOVHResult = new CommunicationData<short>();

        [Description("B面叠片电芯结果结束信号")]
        public CommunicationData<short> In_EndSignal_BOVHResult = new CommunicationData<short>();

        [Description("B面叠片电芯结果返回结果1")]
        public CommunicationData<short> Out_ReturnResult1_BOVHResult = new CommunicationData<short>();

        [Description("B面叠片电芯结果返回结果2")]
        public CommunicationData<short> Out_ReturnResult2_BOVHResult = new CommunicationData<short>();

        [Description("B面叠片电芯结果开始信号")]
        public CommunicationData<float> In_BeginSignal_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果开始信号反馈")]
        public CommunicationData<float> Out_BeginSignalFeedback_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果上或下电芯")]
        public CommunicationData<float> In_UpOrDownCell_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果设备启停信号")]
        public CommunicationData<float> In_StartStopSignal_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果二维码")]
        public CommunicationData<string> In_QRCode_BOVHResult = new CommunicationData<string>();

        [Description("B面叠片电芯结果X轴最大值")]
        public CommunicationData<float> In_XAxisMax_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果X轴最小值")]
        public CommunicationData<float> In_XAxisMin_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果Y轴最大值")]
        public CommunicationData<float> In_YAxisMax_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果Y轴最小值")]
        public CommunicationData<float> In_YAxisMin_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果R轴最大值")]
        public CommunicationData<float> In_RAxisMax_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片电芯结果R轴最小值")]
        public CommunicationData<float> In_RAxisMin_BOVHResult = new CommunicationData<float>();

        [Description("B面叠片A极片复检机器运行状态")]
        public CommunicationData<float> In_MachineRunningState_BOVHResult = new CommunicationData<float>();

        //20250318qm添加PLC下发一键换卷
        [Description("B面叠片_PLC下发一键换卷触发信号")]
        public CommunicationData<bool> InOut_ChangeRollTrigger_BOVHResult = new CommunicationData<bool>();

        [Description("B面叠片_PC一键换卷结果")]
        public CommunicationData<bool> InOut_ChangeRollResult_BOVHResult = new CommunicationData<bool>();

        #endregion

        #region B面叠片A极片标定
        [Description("B面叠片A极片标定进入标定模式")]
        public CommunicationData<short> In_StratCaliSignal_BACali = new CommunicationData<short>();

        [Description("B面叠片A极片标定拍照信号")]
        public CommunicationData<short> In_PhotoSignal_BACali = new CommunicationData<short>();

        [Description("B面叠片A极片标定返回结果")]
        public CommunicationData<short> Out_ReturnResult_BACali = new CommunicationData<short>();

        [Description("B面叠片A极片标定返回X轴坐标")]
        public CommunicationData<float> Out_ReturnXAxisCoord_BACali = new CommunicationData<float>();

        [Description("B面叠片A极片标定返回Y轴坐标")]
        public CommunicationData<float> Out_ReturnYAxisCoord_BACali = new CommunicationData<float>();

        [Description("B面叠片A极片标定返回R轴坐标")]
        public CommunicationData<float> Out_ReturnRAxisCoord_BACali = new CommunicationData<float>();

        [Description("B面叠片A极片标定PLC当前X轴坐标")]
        public CommunicationData<float> In_PLCCurrentXAxisCoord_BACali = new CommunicationData<float>();

        [Description("B面叠片A极片标定PLC当前Y轴坐标")]
        public CommunicationData<float> In_PLCCurrentYAxisCoord_BACali = new CommunicationData<float>();

        [Description("B面叠片A极片标定PLC当前R轴坐标")]
        public CommunicationData<float> In_PLCCurrentRAxisCoord_BACali = new CommunicationData<float>();
        #endregion

        #region B面叠片B极片标定
        [Description("B面叠片B极片标定进入标定模式")]
        public CommunicationData<short> In_StratCaliSignal_BBCali = new CommunicationData<short>();

        [Description("B面叠片B极片标定拍照信号")]
        public CommunicationData<short> In_PhotoSignal_BBCali = new CommunicationData<short>();

        [Description("B面叠片B极片标定返回结果")]
        public CommunicationData<short> Out_ReturnResult_BBCali = new CommunicationData<short>();

        [Description("B面叠片B极片标定返回X轴坐标")]
        public CommunicationData<float> Out_ReturnXAxisCoord_BBCali = new CommunicationData<float>();

        [Description("B面叠片B极片标定返回Y轴坐标")]
        public CommunicationData<float> Out_ReturnYAxisCoord_BBCali = new CommunicationData<float>();

        [Description("B面叠片B极片标定返回R轴坐标")]
        public CommunicationData<float> Out_ReturnRAxisCoord_BBCali = new CommunicationData<float>();

        [Description("B面叠片B极片标定PLC当前X轴坐标")]
        public CommunicationData<float> In_PLCCurrentXAxisCoord_BBCali = new CommunicationData<float>();

        [Description("B面叠片B极片标定PLC当前Y轴坐标")]
        public CommunicationData<float> In_PLCCurrentYAxisCoord_BBCali = new CommunicationData<float>();

        [Description("B面叠片B极片标定PLC当前R轴坐标")]
        public CommunicationData<float> In_PLCCurrentRAxisCoord_BBCali = new CommunicationData<float>();
        #endregion


        public CutOverlayPLCAddress()
        {
            Init();
        }

        public virtual void Init()
        {
            #region 上单面检测       
            Out_CorrectHeart_CCD_TopPlane.Address = "Ac_PC[0].Heartbeat";
            Out_CorrectHeart_CCD_TopPlane.Name = "上单面纠偏相机心跳信号";

            Out_LineHeart_CCD_TopPlane.Address = "Ac_PC[2].Heartbeat";
            Out_LineHeart_CCD_TopPlane.Name = "上单面线扫相机心跳信号";

            In_LineCamDist_TopPlane.Address = "Ac_PC[2].Send_Data[0]";
            In_LineCamDist_TopPlane.Name = "上单面线扫距离";

            In_UnreelingCorrectPhotoSignal_TopPlane.Address = "Ac_PC[0].D_Trigger";
            In_UnreelingCorrectPhotoSignal_TopPlane.Name = "上单面放卷纠偏相机拍照信号";

            Out_UnreelingCorrectReturnResult_TopPlane.Address = "Ac_PC[0].D_Result1";
            Out_UnreelingCorrectReturnResult_TopPlane.Name = "上单面放卷纠偏相机返回结果";

            Out_UnreelingCorrectReturnOffsetValue_TopPlane.Address = "Ac_PC[0].D_Data[0]";
            Out_UnreelingCorrectReturnOffsetValue_TopPlane.Name = "上单面放卷纠偏相机返回纠偏值";

            In_UpLineCamPhotoSignal_TopPlane.Address = "Ac_PC[2].D_Trigger";
            In_UpLineCamPhotoSignal_TopPlane.Name = "上单面上线扫相机拍照信号";

            Out_UpLineCamReturnResult_TopPlane.Address = "Ac_PC[2].D_Result1";
            Out_UpLineCamReturnResult_TopPlane.Name = "上单面上线扫相机返回结果";

            In_DownLineCamPhotoSignal_TopPlane.Address = "Ac_PC[3].D_Trigger";
            In_DownLineCamPhotoSignal_TopPlane.Name = "上单面下线扫相机拍照信号";

            Out_DownLineCamReturnResult_TopPlane.Address = "Ac_PC[3].D_Result1";
            Out_DownLineCamReturnResult_TopPlane.Name = "上单面下线扫相机返回结果";

            In_DieCutCorrectPhotoSignal_TopPlane.Address = "Ac_PC[1].D_Trigger";
            In_DieCutCorrectPhotoSignal_TopPlane.Name = "上单面模切纠偏相机拍照信号";

            Out_DieCutCorrectReturnResult_TopPlane.Address = "Ac_PC[1].D_Result1";
            Out_DieCutCorrectReturnResult_TopPlane.Name = "上单面模切纠偏相机返回结果";

            Out_DieCutCorrectReturnResult2_TopPlane.Address = "Ac_PC[1].D_Result2";
            Out_DieCutCorrectReturnResult2_TopPlane.Name = "上单面模切纠偏相机返回结果2";

            Out_DieCutCorrectReturnOffsetValueX_TopPlane.Address = "Ac_PC[1].D_Data[0]";
            Out_DieCutCorrectReturnOffsetValueX_TopPlane.Name = "上单面模切纠偏相机返回纠偏值X";

            Out_DieCutCorrectReturnOffsetValueY_TopPlane.Address = "Ac_PC[1].D_Data[1]";
            Out_DieCutCorrectReturnOffsetValueY_TopPlane.Name = "上单面模切纠偏相机返回纠偏值Y";

            Out_DieCutCorrectReturnOffsetValueXR_TopPlane.Address = "Ac_PC[1].D_Data[3]";
            Out_DieCutCorrectReturnOffsetValueXR_TopPlane.Name = "上单面模切纠偏相机返回纠偏值XR";

            Out_DieCutCorrectReturnOffsetValueYR_TopPlane.Address = "Ac_PC[1].D_Data[2]";
            Out_DieCutCorrectReturnOffsetValueYR_TopPlane.Name = "上单面模切纠偏相机返回纠偏值YR";

            In_UpTabneckCamPhotoSignal_TopPlane.Address = "Ac_PC[4].D_Trigger";
            In_UpTabneckCamPhotoSignal_TopPlane.Name = "上单面上Tabneck相机拍照信号";

            Out_UpTabneckCamReturnResult_TopPlane.Address = "Ac_PC[4].D_Result1";
            Out_UpTabneckCamReturnResult_TopPlane.Name = "上单面上Tabneck相机返回结果";

            Out_UpTabneckCamReturnTabneckValue_TopPlane.Address = "Ac_PC[4].D_Data[0]";
            Out_UpTabneckCamReturnTabneckValue_TopPlane.Name = "上单面上Tabneck相机返回Tabneck值";

            In_MachineRunningState_TopPlane.Address = "Ac_PC[0].D_Data[10]";
            In_MachineRunningState_TopPlane.Name = "上单面机器运行状态";

            ////20250307qm添加模切纠偏工位，读取PLC实际纠偏值
            InOut_DieCutCorrectReadOffsetTrigger_TopPlane.Address = "Ac_ST[2].M[10]";
            InOut_DieCutCorrectReadOffsetTrigger_TopPlane.Name = "上单面_触发PC读取纠偏量信号";

            In_DieCutCorrectOffsetValueY_TopPlane.Address = "Ac_ST[2].Data.rCache[20]";
            In_DieCutCorrectOffsetValueY_TopPlane.Name = "上单面_纠偏Y轴纠偏量";

            In_DieCutCorrectOffsetValueR_TopPlane.Address = "Ac_ST[2].Data.rCache[21]";
            In_DieCutCorrectOffsetValueR_TopPlane.Name = "上单面_纠偏R轴纠偏量";

            #endregion

            #region 下单面底部检测       
            Out_CorrectHeart_CCD_BottomPlane.Address = "Bc_PC[0].Heartbeat";
            Out_CorrectHeart_CCD_BottomPlane.Name = "下单面纠偏相机心跳信号";

            Out_LineHeart_CCD_BottomPlane.Address = "Bc_PC[2].Heartbeat";
            Out_LineHeart_CCD_BottomPlane.Name = "下单面线扫相机心跳信号";

            In_LineCamDist_BottomPlane.Address = "Bc_PC[2].Send_Data[0]";
            In_LineCamDist_BottomPlane.Name = "下单面线扫距离";

            In_UnreelingCorrectPhotoSignal_BottomPlane.Address = "Bc_PC[0].D_Trigger";
            In_UnreelingCorrectPhotoSignal_BottomPlane.Name = "下单面放卷纠偏相机拍照信号";

            Out_UnreelingCorrectReturnResult_BottomPlane.Address = "Bc_PC[0].D_Result1";
            Out_UnreelingCorrectReturnResult_BottomPlane.Name = "下单面放卷纠偏相机返回结果";

            Out_UnreelingCorrectReturnOffsetValue_BottomPlane.Address = "Bc_PC[0].D_Data[0]";
            Out_UnreelingCorrectReturnOffsetValue_BottomPlane.Name = "下单面放卷纠偏相机返回纠偏值";

            In_UpLineCamPhotoSignal_BottomPlane.Address = "Bc_PC[2].D_Trigger";
            In_UpLineCamPhotoSignal_BottomPlane.Name = "下单面上线扫相机拍照信号";

            Out_UpLineCamReturnResult_BottomPlane.Address = "Bc_PC[2].D_Result1";
            Out_UpLineCamReturnResult_BottomPlane.Name = "下单面上线扫相机返回结果";

            In_DownLineCamPhotoSignal_BottomPlane.Address = "Bc_PC[3].D_Trigger";
            In_DownLineCamPhotoSignal_BottomPlane.Name = "下单面下线扫相机拍照信号";

            Out_DownLineCamReturnResult_BottomPlane.Address = "Bc_PC[3].D_Result1";
            Out_DownLineCamReturnResult_BottomPlane.Name = "下单面下线扫相机返回结果";

            In_DieCutCorrectPhotoSignal_BottomPlane.Address = "Bc_PC[1].D_Trigger";
            In_DieCutCorrectPhotoSignal_BottomPlane.Name = "下单面模切纠偏相机拍照信号";

            Out_DieCutCorrectReturnResult_BottomPlane.Address = "Bc_PC[1].D_Result1";
            Out_DieCutCorrectReturnResult_BottomPlane.Name = "下单面模切纠偏相机返回结果";

            Out_DieCutCorrectReturnResult2_BottomPlane.Address = "Bc_PC[1].D_Result2";
            Out_DieCutCorrectReturnResult2_BottomPlane.Name = "下单面模切纠偏相机返回结果2";

            Out_DieCutCorrectReturnOffsetValueX_BottomPlane.Address = "Bc_PC[1].D_Data[0]";
            Out_DieCutCorrectReturnOffsetValueX_BottomPlane.Name = "下单面模切纠偏相机返回纠偏值X";

            Out_DieCutCorrectReturnOffsetValueY_BottomPlane.Address = "Bc_PC[1].D_Data[1]";
            Out_DieCutCorrectReturnOffsetValueY_BottomPlane.Name = "下单面模切纠偏相机返回纠偏值Y";

            Out_DieCutCorrectReturnOffsetValueXR_BottomPlane.Address = "Bc_PC[1].D_Data[3]";
            Out_DieCutCorrectReturnOffsetValueXR_BottomPlane.Name = "下单面模切纠偏相机返回纠偏值XR";

            Out_DieCutCorrectReturnOffsetValueYR_BottomPlane.Address = "Bc_PC[1].D_Data[2]";
            Out_DieCutCorrectReturnOffsetValueYR_BottomPlane.Name = "下单面模切纠偏相机返回纠偏值YR";

            In_UpTabneckCamPhotoSignal_BottomPlane.Address = "Bc_PC[4].D_Trigger";
            In_UpTabneckCamPhotoSignal_BottomPlane.Name = "下单面上Tabneck相机拍照信号";

            Out_UpTabneckCamReturnResult_BottomPlane.Address = "Bc_PC[4].D_Result1";
            Out_UpTabneckCamReturnResult_BottomPlane.Name = "下单面上Tabneck相机返回结果";

            Out_UpTabneckCamReturnTabneckValue_BottomPlane.Address = "Bc_PC[4].D_Data[0]";
            Out_UpTabneckCamReturnTabneckValue_BottomPlane.Name = "下单面上Tabneck相机返回Tabneck值";

            In_MachineRunningState_BottomPlane.Address = "Bc_PC[0].D_Data[10]";
            In_MachineRunningState_BottomPlane.Name = "下单面机器运行状态";

            ////20250307qm添加模切纠偏工位，读取PLC实际纠偏值
            InOut_DieCutCorrectReadOffsetTrigger_BottomPlane.Address = "Bc_ST[2].M[10]";
            InOut_DieCutCorrectReadOffsetTrigger_BottomPlane.Name = "下单面_触发PC读取纠偏量信号";

            In_DieCutCorrectOffsetValueY_BottomPlane.Address = "Bc_ST[2].Data.rCache[20]";
            In_DieCutCorrectOffsetValueY_BottomPlane.Name = "下单面_纠偏Y轴纠偏量";

            In_DieCutCorrectOffsetValueR_BottomPlane.Address = "Bc_ST[2].Data.rCache[21]";
            In_DieCutCorrectOffsetValueR_BottomPlane.Name = "下单面_纠偏R轴纠偏量";


            #endregion

            #region A模切检测       
            Out_CorrectHeart_CCD_ADie.Address = "Ac_PC[0].Heartbeat";
            Out_CorrectHeart_CCD_ADie.Name = "A模切纠偏相机心跳信号";

            Out_LineHeart_CCD_ADie.Address = "Ac_PC[2].Heartbeat";
            Out_LineHeart_CCD_ADie.Name = "A模切线扫相机心跳信号";

            In_LineCamDist_ADie.Address = "Ac_PC[2].Send_Data[0]";
            In_LineCamDist_ADie.Name = "A模切线扫距离";

            In_UnreelingCorrectPhotoSignal_ADie.Address = "Ac_PC[0].D_Trigger";
            In_UnreelingCorrectPhotoSignal_ADie.Name = "A模切放卷纠偏相机拍照信号";

            Out_UnreelingCorrectReturnResult_ADie.Address = "Ac_PC[0].D_Result1";
            Out_UnreelingCorrectReturnResult_ADie.Name = "A模切放卷纠偏相机返回结果";

            Out_UnreelingCorrectReturnOffsetValue_ADie.Address = "Ac_PC[0].D_Data[0]";
            Out_UnreelingCorrectReturnOffsetValue_ADie.Name = "A模切放卷纠偏相机返回纠偏值";

            In_UpLineCamPhotoSignal_ADie.Address = "Ac_PC[2].D_Trigger";
            In_UpLineCamPhotoSignal_ADie.Name = "A模切上线扫相机拍照信号";

            Out_UpLineCamReturnResult_ADie.Address = "Ac_PC[2].D_Result1";
            Out_UpLineCamReturnResult_ADie.Name = "A模切上线扫相机返回结果";

            In_DownLineCamPhotoSignal_ADie.Address = "Ac_PC[3].D_Trigger";
            In_DownLineCamPhotoSignal_ADie.Name = "A模切下线扫相机拍照信号";

            Out_DownLineCamReturnResult_ADie.Address = "Ac_PC[3].D_Result1";
            Out_DownLineCamReturnResult_ADie.Name = "A模切下线扫相机返回结果";

            In_DieCutCorrectPhotoSignal_ADie.Address = "Ac_PC[1].D_Trigger";
            In_DieCutCorrectPhotoSignal_ADie.Name = "A模切模切纠偏相机拍照信号";

            Out_DieCutCorrectReturnResult_ADie.Address = "Ac_PC[1].D_Result1";
            Out_DieCutCorrectReturnResult_ADie.Name = "A模切模切纠偏相机返回结果";

            Out_DieCutCorrectReturnResult2_ADie.Address = "Ac_PC[1].D_Result2";
            Out_DieCutCorrectReturnResult2_ADie.Name = "A模切模切纠偏相机返回结果2";

            Out_DieCutCorrectReturnOffsetValueX_ADie.Address = "Ac_PC[1].D_Data[0]";
            Out_DieCutCorrectReturnOffsetValueX_ADie.Name = "A模切模切纠偏相机返回纠偏值X";

            Out_DieCutCorrectReturnOffsetValueY_ADie.Address = "Ac_PC[1].D_Data[1]";
            Out_DieCutCorrectReturnOffsetValueY_ADie.Name = "A模切模切纠偏相机返回纠偏值Y";

            Out_DieCutCorrectReturnOffsetValueXR_ADie.Address = "Ac_PC[1].D_Data[3]";
            Out_DieCutCorrectReturnOffsetValueXR_ADie.Name = "A模切模切纠偏相机返回纠偏值XR";

            Out_DieCutCorrectReturnOffsetValueYR_ADie.Address = "Ac_PC[1].D_Data[2]";
            Out_DieCutCorrectReturnOffsetValueYR_ADie.Name = "A模切模切纠偏相机返回纠偏值YR";

            In_UpTabneckCamPhotoSignal_ADie.Address = "Ac_PC[5].D_Trigger";
            In_UpTabneckCamPhotoSignal_ADie.Name = "A模切上Tabneck相机拍照信号";

            Out_UpTabneckCamReturnResult_ADie.Address = "Ac_PC[5].D_Result1";
            Out_UpTabneckCamReturnResult_ADie.Name = "A模切上Tabneck相机返回结果";

            Out_UpTabneckCamReturnTabneckValue_ADie.Address = "Ac_PC[5].D_Data[0]";
            Out_UpTabneckCamReturnTabneckValue_ADie.Name = "A模切上Tabneck相机返回Tabneck值";

            In_DownTabneckCamPhotoSignal_ADie.Address = "Ac_PC[4].D_Trigger";
            In_DownTabneckCamPhotoSignal_ADie.Name = "A模切下Tabneck相机拍照信号";

            Out_DownTabneckCamReturnResult_ADie.Address = "Ac_PC[4].D_Result1";
            Out_DownTabneckCamReturnResult_ADie.Name = "A模切下Tabneck相机返回结果";

            Out_DownTabneckCamReturnTabneckValue1_ADie.Address = "Ac_PC[4].D_Data[0]";
            Out_DownTabneckCamReturnTabneckValue1_ADie.Name = "A模切下Tabneck相机返回Tabneck值1";

            Out_DownTabneckCamReturnTabneckValue2_ADie.Address = "Ac_PC[4].D_Data[1]";
            Out_DownTabneckCamReturnTabneckValue2_ADie.Name = "A模切下Tabneck相机返回Tabneck值2";

            In_MachineRunningState_ADie.Address = "Ac_PC[0].D_Data[10]";
            In_MachineRunningState_ADie.Name = "A模切机器运行状态";


            ////20250307qm添加模切纠偏工位，读取PLC实际纠偏值
            InOut_DieCutCorrectReadOffsetTrigger_ADie.Address = "Ac_ST[2].M[10]";
            InOut_DieCutCorrectReadOffsetTrigger_ADie.Name = "A模切_触发PC读取纠偏量信号";

            In_DieCutCorrectOffsetValueY_ADie.Address = "Ac_ST[2].Data.rCache[20]";
            In_DieCutCorrectOffsetValueY_ADie.Name = "A模切_纠偏Y轴纠偏量";

            In_DieCutCorrectOffsetValueR_ADie.Address = "Ac_ST[2].Data.rCache[21]";
            In_DieCutCorrectOffsetValueR_ADie.Name = "A模切_纠偏R轴纠偏量";


            #endregion

            #region B模切检测       
            Out_CorrectHeart_CCD_BDie.Address = "Bc_PC[0].Heartbeat";
            Out_CorrectHeart_CCD_BDie.Name = "B模切纠偏相机心跳信号";

            Out_LineHeart_CCD_BDie.Address = "Bc_PC[2].Heartbeat";
            Out_LineHeart_CCD_BDie.Name = "B模切线扫相机心跳信号";

            In_LineCamDist_BDie.Address = "Bc_PC[2].Send_Data[0]";
            In_LineCamDist_BDie.Name = "B模切线扫距离";

            In_UnreelingCorrectPhotoSignal_BDie.Address = "Bc_PC[0].D_Trigger";
            In_UnreelingCorrectPhotoSignal_BDie.Name = "B模切放卷纠偏相机拍照信号";

            Out_UnreelingCorrectReturnResult_BDie.Address = "Bc_PC[0].D_Result1";
            Out_UnreelingCorrectReturnResult_BDie.Name = "B模切放卷纠偏相机返回结果";

            Out_UnreelingCorrectReturnOffsetValue_BDie.Address = "Bc_PC[0].D_Data[0]";
            Out_UnreelingCorrectReturnOffsetValue_BDie.Name = "B模切放卷纠偏相机返回纠偏值";

            In_UpLineCamPhotoSignal_BDie.Address = "Bc_PC[2].D_Trigger";
            In_UpLineCamPhotoSignal_BDie.Name = "B模切上线扫相机拍照信号";

            Out_UpLineCamReturnResult_BDie.Address = "Bc_PC[2].D_Result1";
            Out_UpLineCamReturnResult_BDie.Name = "B模切上线扫相机返回结果";

            In_DownLineCamPhotoSignal_BDie.Address = "Bc_PC[3].D_Trigger";
            In_DownLineCamPhotoSignal_BDie.Name = "B模切下线扫相机拍照信号";

            Out_DownLineCamReturnResult_BDie.Address = "Bc_PC[3].D_Result1";
            Out_DownLineCamReturnResult_BDie.Name = "B模切下线扫相机返回结果";

            In_DieCutCorrectPhotoSignal_BDie.Address = "Bc_PC[1].D_Trigger";
            In_DieCutCorrectPhotoSignal_BDie.Name = "B模切模切纠偏相机拍照信号";

            Out_DieCutCorrectReturnResult_BDie.Address = "Bc_PC[1].D_Result1";
            Out_DieCutCorrectReturnResult_BDie.Name = "B模切模切纠偏相机返回结果";

            Out_DieCutCorrectReturnResult2_BDie.Address = "Bc_PC[1].D_Result2";
            Out_DieCutCorrectReturnResult2_BDie.Name = "B模切模切纠偏相机返回结果2";

            Out_DieCutCorrectReturnOffsetValueX_BDie.Address = "Bc_PC[1].D_Data[0]";
            Out_DieCutCorrectReturnOffsetValueX_BDie.Name = "B模切模切纠偏相机返回纠偏值X";

            Out_DieCutCorrectReturnOffsetValueY_BDie.Address = "Bc_PC[1].D_Data[1]";
            Out_DieCutCorrectReturnOffsetValueY_BDie.Name = "B模切模切纠偏相机返回纠偏值Y";

            Out_DieCutCorrectReturnOffsetValueXR_BDie.Address = "Bc_PC[1].D_Data[3]";
            Out_DieCutCorrectReturnOffsetValueXR_BDie.Name = "B模切模切纠偏相机返回纠偏值X";

            Out_DieCutCorrectReturnOffsetValueYR_BDie.Address = "Bc_PC[1].D_Data[2]";
            Out_DieCutCorrectReturnOffsetValueYR_BDie.Name = "B模切模切纠偏相机返回纠偏值Y";

            In_UpTabneckCamPhotoSignal_BDie.Address = "Bc_PC[5].D_Trigger";
            In_UpTabneckCamPhotoSignal_BDie.Name = "B模切上Tabneck相机拍照信号";

            Out_UpTabneckCamReturnResult_BDie.Address = "Bc_PC[5].D_Result1";
            Out_UpTabneckCamReturnResult_BDie.Name = "B模切上Tabneck相机返回结果";

            Out_UpTabneckCamReturnTabneckValue_BDie.Address = "Bc_PC[5].D_Data[0]";
            Out_UpTabneckCamReturnTabneckValue_BDie.Name = "B模切上Tabneck相机返回Tabneck值";

            In_DownTabneckCamPhotoSignal_BDie.Address = "Bc_PC[4].D_Trigger";
            In_DownTabneckCamPhotoSignal_BDie.Name = "B模切下Tabneck相机拍照信号";

            Out_DownTabneckCamReturnResult_BDie.Address = "Bc_PC[4].D_Result1";
            Out_DownTabneckCamReturnResult_BDie.Name = "B模切下Tabneck相机返回结果";

            Out_DownTabneckCamReturnTabneckValue1_BDie.Address = "Bc_PC[4].D_Data[0]";
            Out_DownTabneckCamReturnTabneckValue1_BDie.Name = "B模切下Tabneck相机返回Tabneck值1";

            Out_DownTabneckCamReturnTabneckValue2_BDie.Address = "Bc_PC[4].D_Data[1]";
            Out_DownTabneckCamReturnTabneckValue2_BDie.Name = "B模切下Tabneck相机返回Tabneck值2";

            In_MachineRunningState_BDie.Address = "Bc_PC[0].D_Data[10]";
            In_MachineRunningState_BDie.Name = "B模切机器运行状态";

            ////20250307qm添加模切纠偏工位，读取PLC实际纠偏值
            InOut_DieCutCorrectReadOffsetTrigger_BDie.Address = "Bc_ST[2].M[10]";
            InOut_DieCutCorrectReadOffsetTrigger_BDie.Name = "B模切_触发PC读取纠偏量信号";

            In_DieCutCorrectOffsetValueY_BDie.Address = "Bc_ST[2].Data.rCache[20]";
            In_DieCutCorrectOffsetValueY_BDie.Name = "B模切_纠偏Y轴纠偏量";

            In_DieCutCorrectOffsetValueR_BDie.Address = "Bc_ST[2].Data.rCache[21]";
            In_DieCutCorrectOffsetValueR_BDie.Name = "B模切_纠偏R轴纠偏量";


            #endregion

            #region A面叠片A极片纠偏预定位
            Out_Heart_CCD_AAYDW.Address = "As_PC[0].Heartbeat";
            Out_Heart_CCD_AAYDW.Name = "A面叠片A极片纠偏预定位心跳信号";

            In_PhotoSignal_AAYDW.Address = "As_PC[0].D_Trigger";
            In_PhotoSignal_AAYDW.Name = "A面叠片A极片纠偏预定位拍照信号";

            Out_ReturnResult1_AAYDW.Address = "As_PC[0].D_Result1";
            Out_ReturnResult1_AAYDW.Name = "A面叠片A极片纠偏预定位返回结果1";

            Out_ReturnResult2_AAYDW.Address = "As_PC[0].D_Result2";
            Out_ReturnResult2_AAYDW.Name = "A面叠片A极片纠偏预定位返回结果2";

            Out_OffsetValueX_AAYDW.Address = "As_PC[0].D_Data[0]";
            Out_OffsetValueX_AAYDW.Name = "A面叠片A极片纠偏预定位拍照偏移值X";

            Out_OffsetValueY_AAYDW.Address = "As_PC[0].D_Data[1]";
            Out_OffsetValueY_AAYDW.Name = "A面叠片A极片纠偏预定位拍照偏移值Y";

            Out_OffsetValueR_AAYDW.Address = "As_PC[0].D_Data[2]";
            Out_OffsetValueR_AAYDW.Name = "A面叠片A极片纠偏预定位拍照偏移值R";


            Out_NewFilmRoll_AAYDW.Address = "As_PC[0].D_Data[10]";
            Out_NewFilmRoll_AAYDW.Name = "A面叠片A极片纠偏预定位新建膜卷";

            Out_RunOrPause_AAYDW.Address = "As_PC[0].D_Data[11]";
            Out_RunOrPause_AAYDW.Name = "A面叠片A极片纠偏预定位运行/暂停";

            Out_NGHalt_AAYDW.Address = "As_PC[0].D_Data[12]";
            Out_NGHalt_AAYDW.Name = "A面叠片A极片纠偏预定位停机信号";

            Out_NGAlarm_AAYDW.Address = "As_PC[0].D_Data[13]";
            Out_NGAlarm_AAYDW.Name = "A面叠片A极片纠偏预定位报警信号";


            In_QRCode_AAYDW.Address = "As_PC[0].D_QR";
            In_QRCode_AAYDW.Name = "A面叠片A极片纠偏预定位二维码";

            In_XAxisMax_AAYDW.Address = "As_PC[0].X_Max";
            In_XAxisMax_AAYDW.Name = "A面叠片A极片纠偏预定位X轴最大值";

            In_XAxisMin_AAYDW.Address = "As_PC[0].X_Min";
            In_XAxisMin_AAYDW.Name = "A面叠片A极片纠偏预定位X轴最小值";

            In_YAxisMax_AAYDW.Address = "As_PC[0].R_Max";
            In_YAxisMax_AAYDW.Name = "A面叠片A极片纠偏预定位Y轴最大值";

            In_YAxisMin_AAYDW.Address = "As_PC[0].Y_Min";
            In_YAxisMin_AAYDW.Name = "A面叠片A极片纠偏预定位Y轴最小值";

            In_RAxisMax_AAYDW.Address = "As_PC[0].R_Max";
            In_RAxisMax_AAYDW.Name = "A面叠片A极片纠偏预定位R轴最大值";

            In_RAxisMin_AAYDW.Address = "As_PC[0].R_Min";
            In_RAxisMin_AAYDW.Name = "A面叠片A极片纠偏预定位R轴最小值";

            Out_PoleWidth1_AAYDW.Address = "As_PC[0].D_Data[21]";
            Out_PoleWidth1_AAYDW.Name = "A面叠片A极片纠偏全宽";

            Out_PoleWidth2_AAYDW.Address = "As_PC[0].D_Data[22]";
            Out_PoleWidth2_AAYDW.Name = "A面叠片A极片纠偏宽2";

            Out_PoleHeight1_AAYDW.Address = "As_PC[0].D_Data[23]";
            Out_PoleHeight1_AAYDW.Name = "A面叠片A极片纠偏全高";

            Out_PoleHeight2_AAYDW.Address = "As_PC[0].D_Data[24]";
            Out_PoleHeight2_AAYDW.Name = "A面叠片A极片纠偏高2";

            Out_PoleEarW_AAYDW.Address = "As_PC[0].D_Data[25]";
            Out_PoleEarW_AAYDW.Name = "A面叠片A极片纠偏极耳宽";

            Out_PoleEarH_AAYDW.Address = "As_PC[0].D_Data[26]";
            Out_PoleEarH_AAYDW.Name = "A面叠片A极片纠偏极耳高";

            Out_PoleEarD_AAYDW.Address = "As_PC[0].D_Data[27]";
            Out_PoleEarD_AAYDW.Name = "A面叠片A极片纠偏极耳距边";

            In_CorrAxisXPos_AAYDW.Address = "As_PC[0].Send_Data[0]";
            In_CorrAxisXPos_AAYDW.Name = "A面叠片A极片纠偏X轴当前位置";

            In_CorrAxisYPos_AAYDW.Address = "As_PC[0].Send_Data[1]";
            In_CorrAxisYPos_AAYDW.Name = "A面叠片A极片纠偏Y轴当前位置";

            In_CorrAxisRPos_AAYDW.Address = "As_PC[0].Send_Data[2]";
            In_CorrAxisRPos_AAYDW.Name = "A面叠片A极片纠偏Y轴当前位置";

            #endregion

            #region A面叠片B极片纠偏预定位
            Out_Heart_CCD_ABYDW.Address = "As_PC[1].Heartbeat";
            Out_Heart_CCD_ABYDW.Name = "A面叠片B极片纠偏预定位心跳信号";

            In_PhotoSignal_ABYDW.Address = "As_PC[1].D_Trigger";
            In_PhotoSignal_ABYDW.Name = "A面叠片B极片纠偏预定位拍照信号";

            Out_ReturnResult1_ABYDW.Address = "As_PC[1].D_Result1";
            Out_ReturnResult1_ABYDW.Name = "A面叠片B极片纠偏预定位返回结果1";

            Out_ReturnResult2_ABYDW.Address = "As_PC[1].D_Result2";
            Out_ReturnResult2_ABYDW.Name = "A面叠片B极片纠偏预定位返回结果2";

            Out_OffsetValueX_ABYDW.Address = "As_PC[1].D_Data[0]";
            Out_OffsetValueX_ABYDW.Name = "A面叠片B极片纠偏预定位拍照偏移值X";

            Out_OffsetValueY_ABYDW.Address = "As_PC[1].D_Data[1]";
            Out_OffsetValueY_ABYDW.Name = "A面叠片B极片纠偏预定位拍照偏移值Y";

            Out_OffsetValueR_ABYDW.Address = "As_PC[1].D_Data[2]";
            Out_OffsetValueR_ABYDW.Name = "A面叠片B极片纠偏预定位拍照偏移值R";


            Out_NewFilmRoll_ABYDW.Address = "As_PC[1].D_Data[10]";
            Out_NewFilmRoll_ABYDW.Name = "A面叠片B极片纠偏预定位新建膜卷";

            Out_RunOrPause_ABYDW.Address = "As_PC[1].D_Data[11]";
            Out_RunOrPause_ABYDW.Name = "A面叠片B极片纠偏预定位运行/暂停";

            Out_NGHalt_ABYDW.Address = "As_PC[1].D_Data[12]";
            Out_NGHalt_ABYDW.Name = "A面叠片B极片纠偏预定位停机信号";

            Out_NGAlarm_ABYDW.Address = "As_PC[1].D_Data[13]";
            Out_NGAlarm_ABYDW.Name = "A面叠片B极片纠偏预定位报警信号";


            In_QRCode_ABYDW.Address = "As_PC[1].D_QR";
            In_QRCode_ABYDW.Name = "A面叠片B极片纠偏预定位二维码";

            In_XAxisMax_ABYDW.Address = "As_PC[1].X_Max";
            In_XAxisMax_ABYDW.Name = "A面叠片B极片纠偏预定位X轴最大值";

            In_XAxisMin_ABYDW.Address = "As_PC[1].X_Min";
            In_XAxisMin_ABYDW.Name = "A面叠片B极片纠偏预定位X轴最小值";

            In_YAxisMax_ABYDW.Address = "As_PC[1].R_Max";
            In_YAxisMax_ABYDW.Name = "A面叠片B极片纠偏预定位Y轴最大值";

            In_YAxisMin_ABYDW.Address = "As_PC[1].Y_Min";
            In_YAxisMin_ABYDW.Name = "A面叠片B极片纠偏预定位Y轴最小值";

            In_RAxisMax_ABYDW.Address = "As_PC[1].R_Max";
            In_RAxisMax_ABYDW.Name = "A面叠片B极片纠偏预定位R轴最大值";

            In_RAxisMin_ABYDW.Address = "As_PC[1].R_Min";
            In_RAxisMin_ABYDW.Name = "A面叠片B极片纠偏预定位R轴最小值";

            Out_PoleWidth1_ABYDW.Address = "As_PC[1].D_Data[21]";
            Out_PoleWidth1_ABYDW.Name = "A面叠片B极片纠偏全宽";

            Out_PoleWidth2_ABYDW.Address = "As_PC[1].D_Data[22]";
            Out_PoleWidth2_ABYDW.Name = "A面叠片B极片纠偏宽2";

            Out_PoleHeight1_ABYDW.Address = "As_PC[1].D_Data[23]";
            Out_PoleHeight1_ABYDW.Name = "A面叠片B极片纠偏全高";

            Out_PoleHeight2_ABYDW.Address = "As_PC[1].D_Data[24]";
            Out_PoleHeight2_ABYDW.Name = "A面叠片B极片纠偏高2";

            Out_PoleEarW_ABYDW.Address = "As_PC[1].D_Data[25]";
            Out_PoleEarW_ABYDW.Name = "A面叠片B极片纠偏极耳宽";

            Out_PoleEarH_ABYDW.Address = "As_PC[1].D_Data[26]";
            Out_PoleEarH_ABYDW.Name = "A面叠片B极片纠偏极耳高";

            Out_PoleEarD_ABYDW.Address = "As_PC[1].D_Data[27]";
            Out_PoleEarD_ABYDW.Name = "A面叠片B极片纠偏极耳距边";

            In_CorrAxisXPos_ABYDW.Address = "As_PC[1].Send_Data[0]";
            In_CorrAxisXPos_ABYDW.Name = "A面叠片B极片纠偏X轴当前位置";

            In_CorrAxisYPos_ABYDW.Address = "As_PC[1].Send_Data[1]";
            In_CorrAxisYPos_ABYDW.Name = "A面叠片B极片纠偏Y轴当前位置";

            In_CorrAxisRPos_ABYDW.Address = "As_PC[1].Send_Data[2]";
            In_CorrAxisRPos_ABYDW.Name = "A面叠片B极片纠偏Y轴当前位置";

            #endregion

            #region A面叠片A极片复检OVH
            Out_Heart_CCD_AAOVH.Address = "As_PC[2].Heartbeat";
            Out_Heart_CCD_AAOVH.Name = "A面叠片A极片复检OVH'心跳信号";

            In_PhotoSignal_AAOVH.Address = "As_PC[2].D_Trigger";
            In_PhotoSignal_AAOVH.Name = "A面叠片A极片复检OVH'拍照信号";

            Out_ReturnResult1_AAOVH.Address = "As_PC[2].D_Result1";
            Out_ReturnResult1_AAOVH.Name = "A面叠片A极片复检OVH'返回结果1";

            Out_ReturnResult2_AAOVH.Address = "As_PC[2].D_Result2";
            Out_ReturnResult2_AAOVH.Name = "A面叠片A极片复检OVH'返回结果2";

            In_CurrentLayer_AAOVH.Address = "As_PC[2].D_Data[0]";
            In_CurrentLayer_AAOVH.Name = "A面叠片A极片复检OVH'当前层数";


            Out_NewFilmRoll_AAOVH.Address = "As_PC[2].D_Data[10]";
            Out_NewFilmRoll_AAOVH.Name = "A面叠片A极片复检OVH'新建膜卷";

            Out_RunOrPause_AAOVH.Address = "As_PC[2].D_Data[11]";
            Out_RunOrPause_AAOVH.Name = "A面叠片A极片复检OVH'运行/暂停";

            Out_NGHalt_AAOVH.Address = "As_PC[2].D_Data[12]";
            Out_NGHalt_AAOVH.Name = "A面叠片A极片复检OVH'停机信号";

            Out_NGAlarm_AAOVH.Address = "As_PC[2].D_Data[13]";
            Out_NGAlarm_AAOVH.Name = "A面叠片A极片复检OVH'报警信号";


            In_QRCode_AAOVH.Address = "As_PC[2].D_QR";
            In_QRCode_AAOVH.Name = "A面叠片A极片复检OVH'二维码";

            In_XAxisMax_AAOVH.Address = "As_PC[2].X_Max";
            In_XAxisMax_AAOVH.Name = "A面叠片A极片复检OVH'X轴最大值";

            In_XAxisMin_AAOVH.Address = "As_PC[2].X_Min";
            In_XAxisMin_AAOVH.Name = "A面叠片A极片复检OVH'X轴最小值";

            In_YAxisMax_AAOVH.Address = "As_PC[2].R_Max";
            In_YAxisMax_AAOVH.Name = "A面叠片A极片复检OVH'Y轴最大值";

            In_YAxisMin_AAOVH.Address = "As_PC[2].Y_Min";
            In_YAxisMin_AAOVH.Name = "A面叠片A极片复检OVH'Y轴最小值";

            In_RAxisMax_AAOVH.Address = "As_PC[2].R_Max";
            In_RAxisMax_AAOVH.Name = "A面叠片A极片复检OVH'R轴最大值";

            In_RAxisMin_AAOVH.Address = "As_PC[2].R_Min";
            In_RAxisMin_AAOVH.Name = "A面叠片A极片复检OVH'R轴最小值";

            In_UpTabneckValue_AAOVH.Address = "As_PC[2].Send_Data[0]";
            In_UpTabneckValue_AAOVH.Name = "A面叠片A极片复检上Tabneck值";

            In_DownTabneckValue_AAOVH.Address = "As_PC[2].Send_Data[1]";
            In_DownTabneckValue_AAOVH.Name = "A面叠片A极片复检下Tabneck值";

            In_PoleWidth1_AAOVH.Address = "As_PC[2].Send_Data[2]";
            In_PoleWidth1_AAOVH.Name = "A面叠片A极片复检极片全宽";

            In_PoleWidth2_AAOVH.Address = "As_PC[2].Send_Data[3]";
            In_PoleWidth2_AAOVH.Name = "A面叠片A极片复检极片宽2";

            In_PoleHeight1_AAOVH.Address = "As_PC[2].Send_Data[4]";
            In_PoleHeight1_AAOVH.Name = "A面叠片A极片复检极片全高";

            In_PoleHeight2_AAOVH.Address = "As_PC[2].Send_Data[5]";
            In_PoleHeight2_AAOVH.Name = "A面叠片A极片复检极片高2";

            In_PoleEarW_AAOVH.Address = "As_PC[2].Send_Data[6]";
            In_PoleEarW_AAOVH.Name = "A面叠片A极片复检极片极耳宽";

            In_PoleEarH_AAOVH.Address = "As_PC[2].Send_Data[7]";
            In_PoleEarH_AAOVH.Name = "A面叠片A极片复检极片极耳高";

            In_PoleEarD_AAOVH.Address = "As_PC[2].Send_Data[8]";
            In_PoleEarD_AAOVH.Name = "A面叠片A极片复检极片极耳距边";

            #endregion

            #region A面叠片B极片复检OVH
            Out_Heart_CCD_ABOVH.Address = "As_PC[3].Heartbeat";
            Out_Heart_CCD_ABOVH.Name = "A面叠片B极片复检OVH'心跳信号";

            In_PhotoSignal_ABOVH.Address = "As_PC[3].D_Trigger";
            In_PhotoSignal_ABOVH.Name = "A面叠片B极片复检OVH'拍照信号";

            Out_ReturnResult1_ABOVH.Address = "As_PC[3].D_Result1";
            Out_ReturnResult1_ABOVH.Name = "A面叠片B极片复检OVH'返回结果1";

            Out_ReturnResult2_ABOVH.Address = "As_PC[3].D_Result2";
            Out_ReturnResult2_ABOVH.Name = "A面叠片B极片复检OVH'返回结果2";

            In_CurrentLayer_ABOVH.Address = "As_PC[3].D_Data[0]";
            In_CurrentLayer_ABOVH.Name = "A面叠片B极片复检OVH'当前层数";


            Out_NewFilmRoll_ABOVH.Address = "As_PC[3].D_Data[10]";
            Out_NewFilmRoll_ABOVH.Name = "A面叠片B极片复检OVH'新建膜卷";

            Out_RunOrPause_ABOVH.Address = "As_PC[3].D_Data[11]";
            Out_RunOrPause_ABOVH.Name = "A面叠片B极片复检OVH'运行/暂停";

            Out_NGHalt_ABOVH.Address = "As_PC[3].D_Data[12]";
            Out_NGHalt_ABOVH.Name = "A面叠片B极片复检OVH'停机信号";

            Out_NGAlarm_ABOVH.Address = "As_PC[3].D_Data[13]";
            Out_NGAlarm_ABOVH.Name = "A面叠片B极片复检OVH'报警信号";


            In_QRCode_ABOVH.Address = "As_PC[3].D_QR";
            In_QRCode_ABOVH.Name = "A面叠片B极片复检OVH'二维码";

            In_XAxisMax_ABOVH.Address = "As_PC[3].X_Max";
            In_XAxisMax_ABOVH.Name = "A面叠片B极片复检OVH'X轴最大值";

            In_XAxisMin_ABOVH.Address = "As_PC[3].X_Min";
            In_XAxisMin_ABOVH.Name = "A面叠片B极片复检OVH'X轴最小值";

            In_YAxisMax_ABOVH.Address = "As_PC[3].R_Max";
            In_YAxisMax_ABOVH.Name = "A面叠片B极片复检OVH'Y轴最大值";

            In_YAxisMin_ABOVH.Address = "As_PC[3].Y_Min";
            In_YAxisMin_ABOVH.Name = "A面叠片B极片复检OVH'Y轴最小值";

            In_RAxisMax_ABOVH.Address = "As_PC[3].R_Max";
            In_RAxisMax_ABOVH.Name = "A面叠片B极片复检OVH'R轴最大值";

            In_RAxisMin_ABOVH.Address = "As_PC[3].R_Min";
            In_RAxisMin_ABOVH.Name = "A面叠片B极片复检OVH'R轴最小值";

            In_UpTabneckValue_ABOVH.Address = "As_PC[3].Send_Data[0]";
            In_UpTabneckValue_ABOVH.Name = "A面叠片B极片复检上Tabneck值";

            In_DownTabneckValue_ABOVH.Address = "As_PC[3].Send_Data[1]";
            In_DownTabneckValue_ABOVH.Name = "A面叠片B极片复检下Tabneck值";

            In_PoleWidth1_ABOVH.Address = "As_PC[3].Send_Data[2]";
            In_PoleWidth1_ABOVH.Name = "A面叠片B极片复检极片全宽";

            In_PoleWidth2_ABOVH.Address = "As_PC[3].Send_Data[3]";
            In_PoleWidth2_ABOVH.Name = "A面叠片B极片复检极片宽2";

            In_PoleHeight1_ABOVH.Address = "As_PC[3].Send_Data[4]";
            In_PoleHeight1_ABOVH.Name = "A面叠片B极片复检极片全高";

            In_PoleHeight2_ABOVH.Address = "As_PC[3].Send_Data[5]";
            In_PoleHeight2_ABOVH.Name = "A面叠片B极片复检极片高2";

            In_PoleEarW_ABOVH.Address = "As_PC[3].Send_Data[6]";
            In_PoleEarW_ABOVH.Name = "A面叠片B极片复检极片极耳宽";

            In_PoleEarH_ABOVH.Address = "As_PC[3].Send_Data[7]";
            In_PoleEarH_ABOVH.Name = "A面叠片B极片复检极片极耳高";

            In_PoleEarD_ABOVH.Address = "As_PC[3].Send_Data[8]";
            In_PoleEarD_ABOVH.Name = "A面叠片B极片复检极片极耳距边";
            #endregion

            #region A面叠片电芯结果
            Out_Heart_CCD_AOVHResult.Address = "As_PC[4].Heartbeat";
            Out_Heart_CCD_AOVHResult.Name = "A面叠片电芯结果心跳信号";

            In_EndSignal_AOVHResult.Address = "As_PC[4].D_Trigger";
            In_EndSignal_AOVHResult.Name = "A面叠片电芯结果结束信号";

            Out_ReturnResult1_AOVHResult.Address = "As_PC[4].D_Result1";
            Out_ReturnResult1_AOVHResult.Name = "A面叠片电芯结果返回结果1";

            Out_ReturnResult2_AOVHResult.Address = "As_PC[4].D_Result2";
            Out_ReturnResult2_AOVHResult.Name = "A面叠片电芯结果返回结果2";

            In_BeginSignal_AOVHResult.Address = "As_PC[4].D_Data[0]";
            In_BeginSignal_AOVHResult.Name = "A面叠片电芯结果开始信号";

            Out_BeginSignalFeedback_AOVHResult.Address = "As_PC[4].D_Data[1]";
            Out_BeginSignalFeedback_AOVHResult.Name = "A面叠片电芯结果开始信号反馈";

            In_UpOrDownCell_AOVHResult.Address = "As_PC[4].D_Data[6]";
            In_UpOrDownCell_AOVHResult.Name = "A面叠片电芯结果上或下电芯";

            In_StartStopSignal_AOVHResult.Address = "As_PC[4].D_Data[10]";
            In_StartStopSignal_AOVHResult.Name = "A面叠片电芯结果设备启停信号";

            In_QRCode_AOVHResult.Address = "As_PC[4].D_QR";
            In_QRCode_AOVHResult.Name = "A面叠片电芯结果二维码";

            In_XAxisMax_AOVHResult.Address = "As_PC[4].X_Max";
            In_XAxisMax_AOVHResult.Name = "A面叠片电芯结果X轴最大值";

            In_XAxisMin_AOVHResult.Address = "As_PC[4].X_Min";
            In_XAxisMin_AOVHResult.Name = "A面叠片电芯结果X轴最小值";

            In_YAxisMax_AOVHResult.Address = "As_PC[4].R_Max";
            In_YAxisMax_AOVHResult.Name = "A面叠片电芯结果Y轴最大值";

            In_YAxisMin_AOVHResult.Address = "As_PC[4].Y_Min";
            In_YAxisMin_AOVHResult.Name = "A面叠片电芯结果Y轴最小值";

            In_RAxisMax_AOVHResult.Address = "As_PC[4].R_Max";
            In_RAxisMax_AOVHResult.Name = "A面叠片电芯结果R轴最大值";

            In_RAxisMin_AOVHResult.Address = "As_PC[4].R_Min";
            In_RAxisMin_AOVHResult.Name = "A面叠片电芯结果R轴最小值";

            In_MachineRunningState_AOVHResult.Address = "As_PC[4].D_Data[10]";
            In_MachineRunningState_AOVHResult.Name = "A面叠片A极片复检机器运行状态";
            #endregion

            #region A面叠片A极片标定
            In_StratCaliSignal_AACali.Address = "As_Calibration[0].Calib_Mode";
            In_StratCaliSignal_AACali.Name = "A面叠片A极片标定进入标定模式";

            In_PhotoSignal_AACali.Address = "As_Calibration[0].CaliSendData";
            In_PhotoSignal_AACali.Name = "A面叠片A极片标定拍照信号";

            Out_ReturnResult_AACali.Address = "As_Calibration[0].CaliReceive";
            Out_ReturnResult_AACali.Name = "A面叠片A极片标定返回结果";

            Out_ReturnXAxisCoord_AACali.Address = "As_Calibration[0].Return_X";
            Out_ReturnXAxisCoord_AACali.Name = "A面叠片A极片标定返回X轴坐标";

            Out_ReturnYAxisCoord_AACali.Address = "As_Calibration[0].Return_Y";
            Out_ReturnYAxisCoord_AACali.Name = "A面叠片A极片标定返回Y轴坐标";

            Out_ReturnRAxisCoord_AACali.Address = "As_Calibration[0].Return_R";
            Out_ReturnRAxisCoord_AACali.Name = "A面叠片A极片标定返回R轴坐标";

            In_PLCCurrentXAxisCoord_AACali.Address = "As_Calibration[0].X_Pos";
            In_PLCCurrentXAxisCoord_AACali.Name = "A面叠片A极片标定PLC当前X轴坐标";

            In_PLCCurrentYAxisCoord_AACali.Address = "As_Calibration[0].Y_Pos";
            In_PLCCurrentYAxisCoord_AACali.Name = "A面叠片A极片标定PLC当前Y轴坐标";

            In_PLCCurrentRAxisCoord_AACali.Address = "As_Calibration[0].R_Pos";
            In_PLCCurrentRAxisCoord_AACali.Name = "A面叠片A极片标定PLC当前R轴坐标";
            #endregion

            #region A面叠片B极片标定
            In_StratCaliSignal_ABCali.Address = "As_Calibration[1].Calib_Mode";
            In_StratCaliSignal_ABCali.Name = "A面叠片B极片标定进入标定模式";

            In_PhotoSignal_ABCali.Address = "As_Calibration[1].CaliSendData";
            In_PhotoSignal_ABCali.Name = "A面叠片B极片标定拍照信号";

            Out_ReturnResult_ABCali.Address = "As_Calibration[1].CaliReceive";
            Out_ReturnResult_ABCali.Name = "A面叠片B极片标定返回结果";

            Out_ReturnXAxisCoord_ABCali.Address = "As_Calibration[1].Return_X";
            Out_ReturnXAxisCoord_ABCali.Name = "A面叠片B极片标定返回X轴坐标";

            Out_ReturnYAxisCoord_ABCali.Address = "As_Calibration[1].Return_Y";
            Out_ReturnYAxisCoord_ABCali.Name = "A面叠片B极片标定返回Y轴坐标";

            Out_ReturnRAxisCoord_ABCali.Address = "As_Calibration[1].Return_R";
            Out_ReturnRAxisCoord_ABCali.Name = "A面叠片B极片标定返回R轴坐标";

            In_PLCCurrentXAxisCoord_ABCali.Address = "As_Calibration[1].X_Pos";
            In_PLCCurrentXAxisCoord_ABCali.Name = "A面叠片B极片标定PLC当前X轴坐标";

            In_PLCCurrentYAxisCoord_ABCali.Address = "As_Calibration[1].Y_Pos";
            In_PLCCurrentYAxisCoord_ABCali.Name = "A面叠片B极片标定PLC当前Y轴坐标";

            In_PLCCurrentRAxisCoord_ABCali.Address = "As_Calibration[1].R_Pos";
            In_PLCCurrentRAxisCoord_ABCali.Name = "A面叠片B极片标定PLC当前R轴坐标";
            #endregion

            #region B面叠片A极片纠偏预定位
            Out_Heart_CCD_BAYDW.Address = "Bs_PC[0].Heartbeat";
            Out_Heart_CCD_BAYDW.Name = "B面叠片A极片纠偏预定位心跳信号";

            In_PhotoSignal_BAYDW.Address = "Bs_PC[0].D_Trigger";
            In_PhotoSignal_BAYDW.Name = "B面叠片A极片纠偏预定位拍照信号";

            Out_ReturnResult1_BAYDW.Address = "Bs_PC[0].D_Result1";
            Out_ReturnResult1_BAYDW.Name = "B面叠片A极片纠偏预定位返回结果1";

            Out_ReturnResult2_BAYDW.Address = "Bs_PC[0].D_Result2";
            Out_ReturnResult2_BAYDW.Name = "B面叠片A极片纠偏预定位返回结果2";

            Out_OffsetValueX_BAYDW.Address = "Bs_PC[0].D_Data[0]";
            Out_OffsetValueX_BAYDW.Name = "B面叠片A极片纠偏预定位拍照偏移值X";

            Out_OffsetValueY_BAYDW.Address = "Bs_PC[0].D_Data[1]";
            Out_OffsetValueY_BAYDW.Name = "B面叠片A极片纠偏预定位拍照偏移值Y";

            Out_OffsetValueR_BAYDW.Address = "Bs_PC[0].D_Data[2]";
            Out_OffsetValueR_BAYDW.Name = "B面叠片A极片纠偏预定位拍照偏移值R";


            Out_NewFilmRoll_BAYDW.Address = "Bs_PC[0].D_Data[10]";
            Out_NewFilmRoll_BAYDW.Name = "B面叠片A极片纠偏预定位新建膜卷";

            Out_RunOrPause_BAYDW.Address = "Bs_PC[0].D_Data[11]";
            Out_RunOrPause_BAYDW.Name = "B面叠片A极片纠偏预定位运行/暂停";

            Out_NGHalt_BAYDW.Address = "Bs_PC[0].D_Data[12]";
            Out_NGHalt_BAYDW.Name = "B面叠片A极片纠偏预定位停机信号";

            Out_NGAlarm_BAYDW.Address = "Bs_PC[0].D_Data[13]";
            Out_NGAlarm_BAYDW.Name = "B面叠片A极片纠偏预定位报警信号";


            In_QRCode_BAYDW.Address = "Bs_PC[0].D_QR";
            In_QRCode_BAYDW.Name = "B面叠片A极片纠偏预定位二维码";

            In_XAxisMax_BAYDW.Address = "Bs_PC[0].X_Max";
            In_XAxisMax_BAYDW.Name = "B面叠片A极片纠偏预定位X轴最大值";

            In_XAxisMin_BAYDW.Address = "Bs_PC[0].X_Min";
            In_XAxisMin_BAYDW.Name = "B面叠片A极片纠偏预定位X轴最小值";

            In_YAxisMax_BAYDW.Address = "Bs_PC[0].R_Max";
            In_YAxisMax_BAYDW.Name = "B面叠片A极片纠偏预定位Y轴最大值";

            In_YAxisMin_BAYDW.Address = "Bs_PC[0].Y_Min";
            In_YAxisMin_BAYDW.Name = "B面叠片A极片纠偏预定位Y轴最小值";

            In_RAxisMax_BAYDW.Address = "Bs_PC[0].R_Max";
            In_RAxisMax_BAYDW.Name = "B面叠片A极片纠偏预定位R轴最大值";

            In_RAxisMin_BAYDW.Address = "Bs_PC[0].R_Min";
            In_RAxisMin_BAYDW.Name = "B面叠片A极片纠偏预定位R轴最小值";

            Out_PoleWidth1_BAYDW.Address = "Bs_PC[0].D_Data[21]";
            Out_PoleWidth1_BAYDW.Name = "B面叠片A极片纠偏全宽";

            Out_PoleWidth2_BAYDW.Address = "Bs_PC[0].D_Data[22]";
            Out_PoleWidth2_BAYDW.Name = "B面叠片A极片纠偏宽2";

            Out_PoleHeight1_BAYDW.Address = "Bs_PC[0].D_Data[23]";
            Out_PoleHeight1_BAYDW.Name = "B面叠片A极片纠偏全高";

            Out_PoleHeight2_BAYDW.Address = "Bs_PC[0].D_Data[24]";
            Out_PoleHeight2_BAYDW.Name = "B面叠片A极片纠偏高2";

            Out_PoleEarW_BAYDW.Address = "Bs_PC[0].D_Data[25]";
            Out_PoleEarW_BAYDW.Name = "B面叠片A极片纠偏极耳宽";

            Out_PoleEarH_BAYDW.Address = "Bs_PC[0].D_Data[26]";
            Out_PoleEarH_BAYDW.Name = "B面叠片A极片纠偏极耳高";

            Out_PoleEarD_BAYDW.Address = "Bs_PC[0].D_Data[27]";
            Out_PoleEarD_BAYDW.Name = "B面叠片A极片纠偏极耳距边";

            In_CorrAxisXPos_BAYDW.Address = "Bs_PC[0].Send_Data[0]";
            In_CorrAxisXPos_BAYDW.Name = "B面叠片A极片纠偏X轴当前位置";

            In_CorrAxisYPos_BAYDW.Address = "Bs_PC[0].Send_Data[1]";
            In_CorrAxisYPos_BAYDW.Name = "B面叠片A极片纠偏Y轴当前位置";

            In_CorrAxisRPos_BAYDW.Address = "Bs_PC[0].Send_Data[2]";
            In_CorrAxisRPos_BAYDW.Name = "B面叠片A极片纠偏Y轴当前位置";
            #endregion

            #region B面叠片B极片纠偏预定位
            Out_Heart_CCD_BBYDW.Address = "Bs_PC[1].Heartbeat";
            Out_Heart_CCD_BBYDW.Name = "B面叠片B极片纠偏预定位心跳信号";

            In_PhotoSignal_BBYDW.Address = "Bs_PC[1].D_Trigger";
            In_PhotoSignal_BBYDW.Name = "B面叠片B极片纠偏预定位拍照信号";

            Out_ReturnResult1_BBYDW.Address = "Bs_PC[1].D_Result1";
            Out_ReturnResult1_BBYDW.Name = "B面叠片B极片纠偏预定位返回结果1";

            Out_ReturnResult2_BBYDW.Address = "Bs_PC[1].D_Result2";
            Out_ReturnResult2_BBYDW.Name = "B面叠片B极片纠偏预定位返回结果2";

            Out_OffsetValueX_BBYDW.Address = "Bs_PC[1].D_Data[0]";
            Out_OffsetValueX_BBYDW.Name = "B面叠片B极片纠偏预定位拍照偏移值X";

            Out_OffsetValueY_BBYDW.Address = "Bs_PC[1].D_Data[1]";
            Out_OffsetValueY_BBYDW.Name = "B面叠片B极片纠偏预定位拍照偏移值Y";

            Out_OffsetValueR_BBYDW.Address = "Bs_PC[1].D_Data[2]";
            Out_OffsetValueR_BBYDW.Name = "B面叠片B极片纠偏预定位拍照偏移值R";


            Out_NewFilmRoll_BBYDW.Address = "Bs_PC[1].D_Data[10]";
            Out_NewFilmRoll_BBYDW.Name = "B面叠片B极片纠偏预定位新建膜卷";

            Out_RunOrPause_BBYDW.Address = "Bs_PC[1].D_Data[11]";
            Out_RunOrPause_BBYDW.Name = "B面叠片B极片纠偏预定位运行/暂停";

            Out_NGHalt_BBYDW.Address = "Bs_PC[1].D_Data[12]";
            Out_NGHalt_BBYDW.Name = "B面叠片B极片纠偏预定位停机信号";

            Out_NGAlarm_BBYDW.Address = "Bs_PC[1].D_Data[13]";
            Out_NGAlarm_BBYDW.Name = "B面叠片B极片纠偏预定位报警信号";


            In_QRCode_BBYDW.Address = "Bs_PC[1].D_QR";
            In_QRCode_BBYDW.Name = "B面叠片B极片纠偏预定位二维码";

            In_XAxisMax_BBYDW.Address = "Bs_PC[1].X_Max";
            In_XAxisMax_BBYDW.Name = "B面叠片B极片纠偏预定位X轴最大值";

            In_XAxisMin_BBYDW.Address = "Bs_PC[1].X_Min";
            In_XAxisMin_BBYDW.Name = "B面叠片B极片纠偏预定位X轴最小值";

            In_YAxisMax_BBYDW.Address = "Bs_PC[1].R_Max";
            In_YAxisMax_BBYDW.Name = "B面叠片B极片纠偏预定位Y轴最大值";

            In_YAxisMin_BBYDW.Address = "Bs_PC[1].Y_Min";
            In_YAxisMin_BBYDW.Name = "B面叠片B极片纠偏预定位Y轴最小值";

            In_RAxisMax_BBYDW.Address = "Bs_PC[1].R_Max";
            In_RAxisMax_BBYDW.Name = "B面叠片B极片纠偏预定位R轴最大值";

            In_RAxisMin_BBYDW.Address = "Bs_PC[1].R_Min";
            In_RAxisMin_BBYDW.Name = "B面叠片B极片纠偏预定位R轴最小值";

            Out_PoleWidth1_BBYDW.Address = "Bs_PC[1].D_Data[21]";
            Out_PoleWidth1_BBYDW.Name = "B面叠片B极片纠偏全宽";

            Out_PoleWidth2_BBYDW.Address = "Bs_PC[1].D_Data[22]";
            Out_PoleWidth2_BBYDW.Name = "B面叠片B极片纠偏宽2";

            Out_PoleHeight1_BBYDW.Address = "Bs_PC[1].D_Data[23]";
            Out_PoleHeight1_BBYDW.Name = "B面叠片B极片纠偏全高";

            Out_PoleHeight2_BBYDW.Address = "Bs_PC[1].D_Data[24]";
            Out_PoleHeight2_BBYDW.Name = "B面叠片B极片纠偏高2";

            Out_PoleEarW_BBYDW.Address = "Bs_PC[1].D_Data[25]";
            Out_PoleEarW_BBYDW.Name = "B面叠片B极片纠偏极耳宽";

            Out_PoleEarH_BBYDW.Address = "Bs_PC[1].D_Data[26]";
            Out_PoleEarH_BBYDW.Name = "B面叠片B极片纠偏极耳高";

            Out_PoleEarD_BBYDW.Address = "Bs_PC[1].D_Data[27]";
            Out_PoleEarD_BBYDW.Name = "B面叠片B极片纠偏极耳距边";

            In_CorrAxisXPos_BBYDW.Address = "Bs_PC[1].Send_Data[0]";
            In_CorrAxisXPos_BBYDW.Name = "B面叠片B极片纠偏X轴当前位置";

            In_CorrAxisYPos_BBYDW.Address = "Bs_PC[1].Send_Data[1]";
            In_CorrAxisYPos_BBYDW.Name = "B面叠片B极片纠偏Y轴当前位置";

            In_CorrAxisRPos_BBYDW.Address = "Bs_PC[1].Send_Data[2]";
            In_CorrAxisRPos_BBYDW.Name = "B面叠片B极片纠偏Y轴当前位置";
            #endregion        

            #region B面叠片A极片复检OVH
            Out_Heart_CCD_BAOVH.Address = "Bs_PC[2].Heartbeat";
            Out_Heart_CCD_BAOVH.Name = " B面叠片A极片复检OVH'心跳信号";

            In_PhotoSignal_BAOVH.Address = "Bs_PC[2].D_Trigger";
            In_PhotoSignal_BAOVH.Name = " B面叠片A极片复检OVH'拍照信号";

            Out_ReturnResult1_BAOVH.Address = "Bs_PC[2].D_Result1";
            Out_ReturnResult1_BAOVH.Name = " B面叠片A极片复检OVH'返回结果1";

            Out_ReturnResult2_BAOVH.Address = "Bs_PC[2].D_Result2";
            Out_ReturnResult2_BAOVH.Name = " B面叠片A极片复检OVH'返回结果2";

            In_CurrentLayer_BAOVH.Address = "Bs_PC[2].D_Data[0]";
            In_CurrentLayer_BAOVH.Name = " B面叠片A极片复检OVH'当前层数";


            Out_NewFilmRoll_BAOVH.Address = "Bs_PC[2].D_Data[10]";
            Out_NewFilmRoll_BAOVH.Name = " B面叠片A极片复检OVH'新建膜卷";

            Out_RunOrPause_BAOVH.Address = "Bs_PC[2].D_Data[11]";
            Out_RunOrPause_BAOVH.Name = " B面叠片A极片复检OVH'运行/暂停";

            Out_NGHalt_BAOVH.Address = "Bs_PC[2].D_Data[12]";
            Out_NGHalt_BAOVH.Name = " B面叠片A极片复检OVH'停机信号";

            Out_NGAlarm_BAOVH.Address = "Bs_PC[2].D_Data[13]";
            Out_NGAlarm_BAOVH.Name = " B面叠片A极片复检OVH'报警信号";


            In_QRCode_BAOVH.Address = "Bs_PC[2].D_QR";
            In_QRCode_BAOVH.Name = " B面叠片A极片复检OVH'二维码";

            In_XAxisMax_BAOVH.Address = "Bs_PC[2].X_Max";
            In_XAxisMax_BAOVH.Name = " B面叠片A极片复检OVH'X轴最大值";

            In_XAxisMin_BAOVH.Address = "Bs_PC[2].X_Min";
            In_XAxisMin_BAOVH.Name = " B面叠片A极片复检OVH'X轴最小值";

            In_YAxisMax_BAOVH.Address = "Bs_PC[2].R_Max";
            In_YAxisMax_BAOVH.Name = " B面叠片A极片复检OVH'Y轴最大值";

            In_YAxisMin_BAOVH.Address = "Bs_PC[2].Y_Min";
            In_YAxisMin_BAOVH.Name = " B面叠片A极片复检OVH'Y轴最小值";

            In_RAxisMax_BAOVH.Address = "Bs_PC[2].R_Max";
            In_RAxisMax_BAOVH.Name = " B面叠片A极片复检OVH'R轴最大值";

            In_RAxisMin_BAOVH.Address = "Bs_PC[2].R_Min";
            In_RAxisMin_BAOVH.Name = " B面叠片A极片复检OVH'R轴最小值";

            In_UpTabneckValue_BAOVH.Address = "Bs_PC[2].Send_Data[0]";
            In_UpTabneckValue_BAOVH.Name = "B面叠片A极片复检上Tabneck值";

            In_DownTabneckValue_BAOVH.Address = "Bs_PC[2].Send_Data[1]";
            In_DownTabneckValue_BAOVH.Name = "B面叠片A极片复检下Tabneck值";

            In_PoleWidth1_BAOVH.Address = "Bs_PC[2].Send_Data[2]";
            In_PoleWidth1_BAOVH.Name = "B面叠片A极片复检极片全宽";

            In_PoleWidth2_BAOVH.Address = "Bs_PC[2].Send_Data[3]";
            In_PoleWidth2_BAOVH.Name = "B面叠片A极片复检极片宽2";

            In_PoleHeight1_BAOVH.Address = "Bs_PC[2].Send_Data[4]";
            In_PoleHeight1_BAOVH.Name = "B面叠片A极片复检极片全高";

            In_PoleHeight2_BAOVH.Address = "Bs_PC[2].Send_Data[5]";
            In_PoleHeight2_BAOVH.Name = "B面叠片A极片复检极片高2";

            In_PoleEarW_BAOVH.Address = "Bs_PC[2].Send_Data[6]";
            In_PoleEarW_BAOVH.Name = "B面叠片A极片复检极片极耳宽";

            In_PoleEarH_BAOVH.Address = "Bs_PC[2].Send_Data[7]";
            In_PoleEarH_BAOVH.Name = "B面叠片A极片复检极片极耳高";

            In_PoleEarD_BAOVH.Address = "Bs_PC[2].Send_Data[8]";
            In_PoleEarD_BAOVH.Name = "B面叠片A极片复检极片极耳距边";

            #endregion

            #region B面叠片B极片复检OVH
            Out_Heart_CCD_BBOVH.Address = "Bs_PC[3].Heartbeat";
            Out_Heart_CCD_BBOVH.Name = "B面叠片B极片复检OVH'心跳信号";

            In_PhotoSignal_BBOVH.Address = "Bs_PC[3].D_Trigger";
            In_PhotoSignal_BBOVH.Name = "B面叠片B极片复检OVH'拍照信号";

            Out_ReturnResult1_BBOVH.Address = "Bs_PC[3].D_Result1";
            Out_ReturnResult1_BBOVH.Name = "B面叠片B极片复检OVH'返回结果1";

            Out_ReturnResult2_BBOVH.Address = "Bs_PC[3].D_Result2";
            Out_ReturnResult2_BBOVH.Name = "B面叠片B极片复检OVH'返回结果2";

            In_CurrentLayer_BBOVH.Address = "Bs_PC[3].D_Data[0]";
            In_CurrentLayer_BBOVH.Name = "B面叠片B极片复检OVH'当前层数";


            Out_NewFilmRoll_BBOVH.Address = "Bs_PC[3].D_Data[10]";
            Out_NewFilmRoll_BBOVH.Name = "B面叠片B极片复检OVH'新建膜卷";

            Out_RunOrPause_BBOVH.Address = "Bs_PC[3].D_Data[11]";
            Out_RunOrPause_BBOVH.Name = "B面叠片B极片复检OVH'运行/暂停";

            Out_NGHalt_BBOVH.Address = "Bs_PC[3].D_Data[12]";
            Out_NGHalt_BBOVH.Name = "B面叠片B极片复检OVH'停机信号";

            Out_NGAlarm_BBOVH.Address = "Bs_PC[3].D_Data[13]";
            Out_NGAlarm_BBOVH.Name = "B面叠片B极片复检OVH'报警信号";


            In_QRCode_BBOVH.Address = "Bs_PC[3].D_QR";
            In_QRCode_BBOVH.Name = "B面叠片B极片复检OVH'二维码";

            In_XAxisMax_BBOVH.Address = "Bs_PC[3].X_Max";
            In_XAxisMax_BBOVH.Name = "B面叠片B极片复检OVH'X轴最大值";

            In_XAxisMin_BBOVH.Address = "Bs_PC[3].X_Min";
            In_XAxisMin_BBOVH.Name = "B面叠片B极片复检OVH'X轴最小值";

            In_YAxisMax_BBOVH.Address = "Bs_PC[3].R_Max";
            In_YAxisMax_BBOVH.Name = "B面叠片B极片复检OVH'Y轴最大值";

            In_YAxisMin_BBOVH.Address = "Bs_PC[3].Y_Min";
            In_YAxisMin_BBOVH.Name = "B面叠片B极片复检OVH'Y轴最小值";

            In_RAxisMax_BBOVH.Address = "Bs_PC[3].R_Max";
            In_RAxisMax_BBOVH.Name = "B面叠片B极片复检OVH'R轴最大值";

            In_RAxisMin_BBOVH.Address = "Bs_PC[3].R_Min";
            In_RAxisMin_BBOVH.Name = "B面叠片B极片复检OVH'R轴最小值";

            In_UpTabneckValue_BBOVH.Address = "Bs_PC[3].Send_Data[0]";
            In_UpTabneckValue_BBOVH.Name = "B面叠片B极片复检上Tabneck值";

            In_DownTabneckValue_BBOVH.Address = "Bs_PC[3].Send_Data[1]";
            In_DownTabneckValue_BBOVH.Name = "B面叠片B极片复检下Tabneck值";

            In_PoleWidth1_BBOVH.Address = "Bs_PC[3].Send_Data[2]";
            In_PoleWidth1_BBOVH.Name = "B面叠片B极片复检极片全宽";

            In_PoleWidth2_BBOVH.Address = "Bs_PC[3].Send_Data[3]";
            In_PoleWidth2_BBOVH.Name = "B面叠片B极片复检极片宽2";

            In_PoleHeight1_BBOVH.Address = "Bs_PC[3].Send_Data[4]";
            In_PoleHeight1_BBOVH.Name = "B面叠片B极片复检极片全高";

            In_PoleHeight2_BBOVH.Address = "Bs_PC[3].Send_Data[5]";
            In_PoleHeight2_BBOVH.Name = "B面叠片B极片复检极片高2";

            In_PoleEarW_BBOVH.Address = "Bs_PC[3].Send_Data[6]";
            In_PoleEarW_BBOVH.Name = "B面叠片B极片复检极片极耳宽";

            In_PoleEarH_BBOVH.Address = "Bs_PC[3].Send_Data[7]";
            In_PoleEarH_BBOVH.Name = "B面叠片B极片复检极片极耳高";

            In_PoleEarD_BBOVH.Address = "Bs_PC[3].Send_Data[8]";
            In_PoleEarD_BBOVH.Name = "B面叠片B极片复检极片极耳距边";
            #endregion

            #region B面叠片电芯结果
            Out_Heart_CCD_BOVHResult.Address = "Bs_PC[4].Heartbeat";
            Out_Heart_CCD_BOVHResult.Name = "B面叠片电芯结果心跳信号";

            In_EndSignal_BOVHResult.Address = "Bs_PC[4].D_Trigger";
            In_EndSignal_BOVHResult.Name = "B面叠片电芯结果结束信号";

            Out_ReturnResult1_BOVHResult.Address = "Bs_PC[4].D_Result1";
            Out_ReturnResult1_BOVHResult.Name = "B面叠片电芯结果返回结果1";

            Out_ReturnResult2_BOVHResult.Address = "Bs_PC[4].D_Result2";
            Out_ReturnResult2_BOVHResult.Name = "B面叠片电芯结果返回结果2";

            In_BeginSignal_BOVHResult.Address = "Bs_PC[4].D_Data[0]";
            In_BeginSignal_BOVHResult.Name = "B面叠片电芯结果开始信号";

            Out_BeginSignalFeedback_BOVHResult.Address = "Bs_PC[4].D_Data[1]";
            Out_BeginSignalFeedback_BOVHResult.Name = "B面叠片电芯结果开始信号反馈";

            In_UpOrDownCell_BOVHResult.Address = "Bs_PC[4].D_Data[6]";
            In_UpOrDownCell_BOVHResult.Name = "B面叠片电芯结果上或下电芯";

            In_StartStopSignal_BOVHResult.Address = "Bs_PC[4].D_Data[10]";
            In_StartStopSignal_BOVHResult.Name = "B面叠片电芯结果设备启停信号";

            In_QRCode_BOVHResult.Address = "Bs_PC[4].D_QR";
            In_QRCode_BOVHResult.Name = "B面叠片电芯结果二维码";

            In_XAxisMax_BOVHResult.Address = "Bs_PC[4].X_Max";
            In_XAxisMax_BOVHResult.Name = "B面叠片电芯结果X轴最大值";

            In_XAxisMin_BOVHResult.Address = "Bs_PC[4].X_Min";
            In_XAxisMin_BOVHResult.Name = "B面叠片电芯结果X轴最小值";

            In_YAxisMax_BOVHResult.Address = "Bs_PC[4].R_Max";
            In_YAxisMax_BOVHResult.Name = "B面叠片电芯结果Y轴最大值";

            In_YAxisMin_BOVHResult.Address = "Bs_PC[4].Y_Min";
            In_YAxisMin_BOVHResult.Name = "B面叠片电芯结果Y轴最小值";

            In_RAxisMax_BOVHResult.Address = "Bs_PC[4].R_Max";
            In_RAxisMax_BOVHResult.Name = "B面叠片电芯结果R轴最大值";

            In_RAxisMin_BOVHResult.Address = "Bs_PC[4].R_Min";
            In_RAxisMin_BOVHResult.Name = "B面叠片电芯结果R轴最小值";


            In_MachineRunningState_BOVHResult.Address = "Bs_PC[4].D_Data[10]";
            In_MachineRunningState_BOVHResult.Name = "B面叠片A极片复检机器运行状态";

            #endregion

            #region B面叠片A极片标定
            In_StratCaliSignal_BACali.Address = "Bs_Calibration[0].Calib_Mode";
            In_StratCaliSignal_BACali.Name = "B面叠片A极片标定进入标定模式";

            In_PhotoSignal_BACali.Address = "Bs_Calibration[0].CaliSendData";
            In_PhotoSignal_BACali.Name = "B面叠片A极片标定拍照信号";

            Out_ReturnResult_BACali.Address = "Bs_Calibration[0].CaliReceive";
            Out_ReturnResult_BACali.Name = "B面叠片A极片标定返回结果";

            Out_ReturnXAxisCoord_BACali.Address = "Bs_Calibration[0].Return_X";
            Out_ReturnXAxisCoord_BACali.Name = "B面叠片A极片标定返回X轴坐标";

            Out_ReturnYAxisCoord_BACali.Address = "Bs_Calibration[0].Return_Y";
            Out_ReturnYAxisCoord_BACali.Name = "B面叠片A极片标定返回Y轴坐标";

            Out_ReturnRAxisCoord_BACali.Address = "Bs_Calibration[0].Return_R";
            Out_ReturnRAxisCoord_BACali.Name = "B面叠片A极片标定返回R轴坐标";

            In_PLCCurrentXAxisCoord_BACali.Address = "Bs_Calibration[0].X_Pos";
            In_PLCCurrentXAxisCoord_BACali.Name = "B面叠片A极片标定PLC当前X轴坐标";

            In_PLCCurrentYAxisCoord_BACali.Address = "Bs_Calibration[0].Y_Pos";
            In_PLCCurrentYAxisCoord_BACali.Name = "B面叠片A极片标定PLC当前Y轴坐标";

            In_PLCCurrentRAxisCoord_BACali.Address = "Bs_Calibration[0].R_Pos";
            In_PLCCurrentRAxisCoord_BACali.Name = "B面叠片A极片标定PLC当前R轴坐标";
            #endregion

            #region B面叠片B极片标定
            In_StratCaliSignal_BBCali.Address = "Bs_Calibration[1].Calib_Mode";
            In_StratCaliSignal_BBCali.Name = "B面叠片B极片标定进入标定模式";

            In_PhotoSignal_BBCali.Address = "Bs_Calibration[1].CaliSendData";
            In_PhotoSignal_BBCali.Name = "B面叠片B极片标定拍照信号";

            Out_ReturnResult_BBCali.Address = "Bs_Calibration[1].CaliReceive";
            Out_ReturnResult_BBCali.Name = "B面叠片B极片标定返回结果";

            Out_ReturnXAxisCoord_BBCali.Address = "Bs_Calibration[1].Return_X";
            Out_ReturnXAxisCoord_BBCali.Name = "B面叠片B极片标定返回X轴坐标";

            Out_ReturnYAxisCoord_BBCali.Address = "Bs_Calibration[1].Return_Y";
            Out_ReturnYAxisCoord_BBCali.Name = "B面叠片B极片标定返回Y轴坐标";

            Out_ReturnRAxisCoord_BBCali.Address = "Bs_Calibration[1].Return_R";
            Out_ReturnRAxisCoord_BBCali.Name = "B面叠片B极片标定返回R轴坐标";

            In_PLCCurrentXAxisCoord_BBCali.Address = "Bs_Calibration[1].X_Pos";
            In_PLCCurrentXAxisCoord_BBCali.Name = "B面叠片B极片标定PLC当前X轴坐标";

            In_PLCCurrentYAxisCoord_BBCali.Address = "Bs_Calibration[1].Y_Pos";
            In_PLCCurrentYAxisCoord_BBCali.Name = "B面叠片B极片标定PLC当前Y轴坐标";

            In_PLCCurrentRAxisCoord_BBCali.Address = "Bs_Calibration[1].R_Pos";
            In_PLCCurrentRAxisCoord_BBCali.Name = "B面叠片B极片标定PLC当前R轴坐标";
            #endregion


        }
    }
}
