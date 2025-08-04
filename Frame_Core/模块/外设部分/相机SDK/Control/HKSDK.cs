/***************************************************************************************************
* 
* 版权信息：版权所有 (c) 2015, 杭州海康威视数字技术股份有限公司, 保留所有权利
* 
* 文件名称：CameraOperator.cs
* 摘    要：相机操作类
*
* 当前版本：1.0.0.0
* 作    者：孙海祥
* 日    期：2016-07-07
* 备    注：新建
***************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvCamCtrl.NET;
using System.Runtime.InteropServices;


namespace Frame_Core
{
    using ImageCallBack = MyCamera.cbOutputdelegate;
    using ImageCallBackEX = MyCamera.cbOutputExdelegate;
    using ExceptionCallBack = MyCamera.cbExceptiondelegate;
    class CameraOperator
    {
        public const int CO_FAIL = -1;
        public const int CO_OK = 0;
        private MyCamera m_pCSI;
        public CameraOperator()
        {
           // m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            m_pCSI = new MyCamera();
        }

        /****************************************************************************
         * @fn           EnumDevices
         * @brief        枚举可连接设备
         * @param        nLayerType       IN         传输层协议：1-GigE; 4-USB;可叠加
         * @param        stDeviceList     OUT        设备列表
         * @return       成功：0；错误：错误码
         ****************************************************************************/
        public static int EnumDevices(uint nLayerType, ref MyCamera.MV_CC_DEVICE_INFO_LIST stDeviceList)
        {
            return MyCamera.MV_CC_EnumDevices_NET(nLayerType, ref stDeviceList);
        }


        /****************************************************************************
         * @fn           Open
         * @brief        连接设备
         * @param        stDeviceInfo       IN       设备信息结构体
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int Open(ref MyCamera.MV_CC_DEVICE_INFO stDeviceInfo)
        {
            if (null == m_pCSI)
            {
                m_pCSI = new MyCamera();
                if (null == m_pCSI)
                {
                    return CO_FAIL;
                }
            }

            int nRet;
            nRet = m_pCSI.MV_CC_CreateDevice_NET(ref stDeviceInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            nRet = m_pCSI.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;

        }

        /// <summary>
        /// 飞拍相机要设置缓存节点20200901yzc
        /// </summary>
        /// <param name="stDeviceInfo"></param>
        /// <returns></returns>
        public int OpenFly(ref MyCamera.MV_CC_DEVICE_INFO stDeviceInfo)
        {
            if (null == m_pCSI)
            {
                m_pCSI = new MyCamera();
                if (null == m_pCSI)
                {
                    return CO_FAIL;
                }
            }

            int nRet;
            nRet = m_pCSI.MV_CC_CreateDevice_NET(ref stDeviceInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            nRet = m_pCSI.MV_CC_OpenDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            //设置缓存节点20200901xjh
            uint nImageNodeNum = 10;
            nRet = m_pCSI.MV_CC_SetImageNodeNum_NET(nImageNodeNum);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;

        }


        /****************************************************************************
         * @fn           Close
         * @brief        关闭设备
         * @param        none
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int Close()
        {
            int nRet;

            nRet = m_pCSI.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            nRet = m_pCSI.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           StartGrabbing
         * @brief        开始采集
         * @param        none
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int StartGrabbing()
        {
            int nRet;
            //开始采集
            nRet = m_pCSI.MV_CC_StartGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }



        /****************************************************************************
         * @fn           StopGrabbing
         * @brief        停止采集
         * @param        none
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int StopGrabbing()
        {
            int nRet;
            nRet = m_pCSI.MV_CC_StopGrabbing_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           RegisterImageCallBack
         * @brief        注册取流回调函数
         * @param        CallBackFunc          IN        回调函数
         * @param        pUser                 IN        用户参数
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int RegisterImageCallBack(ImageCallBack CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterImageCallBack_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /****************************************************************************
        * @fn           RegisterImageCallBack
        * @brief        注册取流回调函数
        * @param        CallBackFunc          IN        回调函数
        * @param        pUser                 IN        用户参数
        * @return       成功：0；错误：-1
        ****************************************************************************/
        public int RegisterImageCallBackEX(ImageCallBackEX CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterImageCallBackEx_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /****************************************************************************
       * @fn           RegisterImageCallBack
       * @brief        注册取流回调函数
       * @param        CallBackFunc          IN        回调函数
       * @param        pUser                 IN        用户参数
       * @return       成功：0；错误：-1
       ****************************************************************************/
        public int RegisterImageCallBackEX_RGB(ImageCallBackEX CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterImageCallBackForRGB_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /****************************************************************************
         * @fn           RegisterExceptionCallBack
         * @brief        注册异常回调函数
         * @param        CallBackFunc          IN        回调函数
         * @param        pUser                 IN        用户参数
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int RegisterExceptionCallBack(ExceptionCallBack CallBackFunc, IntPtr pUser)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_RegisterExceptionCallBack_NET(CallBackFunc, pUser);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetOneFrame
         * @brief        获取一帧图像数据
         * @param        pData                 IN-OUT            数据数组指针
         * @param        pnDataLen             IN                数据大小
         * @param        nDataSize             IN                数组缓存大小
         * @param        pFrameInfo            OUT               数据信息
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetOneFrame(IntPtr pData, ref UInt32 pnDataLen, UInt32 nDataSize, ref MyCamera.MV_FRAME_OUT_INFO pFrameInfo)
        {
            pnDataLen = 0;
            int nRet = m_pCSI.MV_CC_GetOneFrame_NET(pData, nDataSize, ref pFrameInfo);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pnDataLen = (uint)(pFrameInfo.nWidth * pFrameInfo.nWidth * (((((UInt32)pFrameInfo.enPixelType) >> 16) & 0xffff) >> 3));

            return CO_OK;
        }


        /****************************************************************************
         * @fn           Display
         * @brief        显示图像
         * @param        hWnd                  IN        窗口句柄
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int Display(IntPtr hWnd)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_Display_NET(hWnd);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        /****************************************************************************
        * @fn           YUVConverttoRGB
        * @brief        图像格式转换
        * @param        MyCamera.MV_PIXEL_CONVERT_PARAM    IN        转换茶树
        * @return       成功：0；错误：-1
 ****************************************************************************/

        public int YUVConverttoRGB(ref MyCamera.MV_PIXEL_CONVERT_PARAM YUVParam)
         {
            int nRet;
            nRet = m_pCSI.MV_CC_ConvertPixelType_NET(ref YUVParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
         }
        /****************************************************************************
         * @fn           GetIntValue
         * @brief        获取Int型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        pnValue               OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetIntValue(string strKey, ref UInt32 pnValue)
        {

            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = m_pCSI.MV_CC_GetIntValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pnValue = stParam.nCurValue;

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetIntValue
         * @brief        设置Int型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        nValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetIntValue(string strKey, UInt32 nValue)
        {

            int nRet = m_pCSI.MV_CC_SetIntValue_NET(strKey, nValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }



        /****************************************************************************
         * @fn           GetFloatValue
         * @brief        获取Float型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        pValue                OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetFloatValue(string strKey, ref float pfValue)
        {
            MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pCSI.MV_CC_GetFloatValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pfValue = stParam.fCurValue;

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetFloatValue
         * @brief        设置Float型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        fValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetFloatValue(string strKey, float fValue)
        {
            int nRet = m_pCSI.MV_CC_SetFloatValue_NET(strKey, fValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetEnumValue
         * @brief        获取Enum型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        pnValue               OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetEnumValue(string strKey, ref UInt32 pnValue)
        {
            MyCamera.MVCC_ENUMVALUE stParam = new MyCamera.MVCC_ENUMVALUE();
            int nRet = m_pCSI.MV_CC_GetEnumValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            pnValue = stParam.nCurValue;

            return CO_OK;
        }



        /****************************************************************************
         * @fn           SetEnumValue
         * @brief        设置Float型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        nValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetEnumValue(string strKey, UInt32 nValue)
        {
            int nRet = m_pCSI.MV_CC_SetEnumValue_NET(strKey, nValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        public int SetCommandValue(string strKey)
        {
            int nRet = m_pCSI.MV_CC_SetCommandValue_NET(strKey);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /****************************************************************************
         * @fn           GetBoolValue
         * @brief        获取Bool型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        pbValue               OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetBoolValue(string strKey, ref bool pbValue)
        {
            int nRet = m_pCSI.MV_CC_GetBoolValue_NET(strKey, ref pbValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetBoolValue
         * @brief        设置Bool型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        bValue                IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetBoolValue(string strKey,bool bValue)
        {
            int nRet = m_pCSI.MV_CC_SetBoolValue_NET(strKey, bValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           GetStringValue
         * @brief        获取String型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        strValue              OUT       返回值
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int GetStringValue(string strKey, ref string strValue)
        {
            MyCamera.MVCC_STRINGVALUE stParam = new MyCamera.MVCC_STRINGVALUE();
            int nRet = m_pCSI.MV_CC_GetStringValue_NET(strKey, ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }

            strValue = stParam.chCurValue;

            return CO_OK;
        }


        /****************************************************************************
         * @fn           SetStringValue
         * @brief        设置String型参数值
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @param        strValue              IN        设置参数值，具体取值范围参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SetStringValue(string strKey, string strValue)
        {
            int nRet = m_pCSI.MV_CC_SetStringValue_NET(strKey, strValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           CommandExecute
         * @brief        Command命令
         * @param        strKey                IN        参数键值，具体键值名称参考HikCameraNode.xls文档
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int CommandExecute(string strKey)
        {
            int nRet = m_pCSI.MV_CC_SetCommandValue_NET(strKey);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }


        /****************************************************************************
         * @fn           SaveImage
         * @brief        保存图片
         * @param        pSaveParam            IN        保存图片配置参数结构体
         * @return       成功：0；错误：-1
         ****************************************************************************/
        public int SaveImage(ref MyCamera.MV_SAVE_IMAGE_PARAM pSaveParam)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_SaveImage_NET(ref pSaveParam);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /****************************************************************************
        * @fn            ImageDecode
        * @brief       压缩灰度图像解码
        * @param      
        * @return       成功：0；错误：-1
        ****************************************************************************/
        public int ImageDecode(IntPtr pData, MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, ref MyCamera.MV_CC_HB_DECODE_PARAM decode_param)
        {
            MyCamera.MV_CC_HB_DECODE_PARAM pstRecordParam = new MyCamera.MV_CC_HB_DECODE_PARAM();
            pstRecordParam.pSrcBuf = pData;   //输入数据缓存
            pstRecordParam.nSrcLen = pFrameInfo.nFrameLen;     //输入数据大小

            // Mono8为单通道
            UInt32 nPayloadSize = (UInt32)(pFrameInfo.nWidth * pFrameInfo.nHeight);
            IntPtr pDstBuf = Marshal.AllocHGlobal((Int32)nPayloadSize);

            pstRecordParam.pDstBuf = pDstBuf;       //输出的数据缓存
            pstRecordParam.nDstBufSize = nPayloadSize;   //输出的数据缓存缓冲区大小
            //var start = DateTime.Now;
            int nRet = m_pCSI.MV_CC_HB_Decode_NET(ref pstRecordParam);
            decode_param = pstRecordParam;
            // var stop = DateTime.Now;


            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /// <summary>
        /// 获取相机连接状态
        /// </summary>
        /// <returns></returns>
        public bool GetCameraStatus()
        {
            return m_pCSI.MV_CC_IsDeviceConnected_NET();
        }

        /// <summary>
        /// 设置心跳超时时间
        /// </summary>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public int SetHeartBeatTimeout(uint millisecond)
        {
            int nRet;
            nRet = m_pCSI.MV_CC_SetHeartBeatTimeout_NET(millisecond);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="fValue"></param>
        /// <returns></returns>
        public int SetExposureTime(float fValue)
        {
            int nRet = m_pCSI.MV_CC_SetExposureTime_NET(fValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }
        public float GetExposureTime()
        {
            MyCamera.MVCC_FLOATVALUE stFloatValue = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pCSI.MV_CC_GetExposureTime_NET(ref stFloatValue);
            if (MyCamera.MV_OK != nRet)
            { return 0; }
            else
            {
                return stFloatValue.fCurValue;
            }
        }
        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="fValue"></param>
        /// <returns></returns>
        public int SetGain(float fValue)
        {
            int nRet = m_pCSI.MV_CC_SetGain_NET(fValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
            
        }
        /// <summary>
        /// 获取增益
        /// </summary>
        /// <returns></returns>
        public float GetGain()
        {
            MyCamera.MVCC_FLOATVALUE stFloatValue = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pCSI.MV_CC_GetGain_NET(ref stFloatValue);
            if (MyCamera.MV_OK != nRet)
            {return 0; }
            else
            {
                return stFloatValue.fCurValue;
            }

        }

        /// <summary>
        /// 设置相机的触发延时时间
        /// </summary>
        /// <param name="fValue"></param>
        /// <returns></returns>
        public int SetTriggerDelayTime(float fValue)
        {
            int nRet = m_pCSI.MV_CC_SetTriggerDelay_NET(fValue);
            if (MyCamera.MV_OK != nRet)
            {
                return CO_FAIL;
            }
            return CO_OK;
        }

        /// <summary>
        /// 获取相机的触发延时时间
        /// </summary>
        /// <returns></returns>
        public float GetTriggerDelayTime()
        {
            MyCamera.MVCC_FLOATVALUE stFloatValue = new MyCamera.MVCC_FLOATVALUE();
            int nRet = m_pCSI.MV_CC_GetTriggerDelay_NET(ref stFloatValue);
            if (MyCamera.MV_OK != nRet)
            { return 0; }
            else
            {
                return stFloatValue.fCurValue;
            }

        }
    }
}
