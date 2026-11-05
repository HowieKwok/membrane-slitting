using DevMgmt.Light.SDK;
using FrameCommunity;
using FrameCommunity.错误代码;
using LogTool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace DevMgmt.Light.Models
{
    public static class LightControllerExtensions
    {
        #region 设备连接、关闭
        /// <summary>
        /// 连接所有设备
        /// </summary>
        public static void ConnectAllDev(this ObservableCollection<LightControllerDev> Devs)
        {
            try
            {
                foreach (var item in Devs)
                {
                    ConnectDev(item);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 关闭所有设备
        /// </summary>
        public static void CloseAllDev(this ObservableCollection<LightControllerDev> Devs)
        {
            try
            {
                foreach (var item in Devs)
                {
                    if (item.LightController != null)
                    {
                        CloseDev(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="cameraDev"></param>
        public static void ConnectDev(this LightControllerDev Dev)
        {
            if (Dev == null)
            {
                ErrorWorks.ErrorMessageShow("cameraDev", ErrorWorks.GetErrorInfo((uint)ErrorCodeEnum.参数为空));
                return;
            }

            if (Dev.LightControllerParameter == null)
            {
                ErrorWorks.ErrorMessageShow("CameraParameter", ErrorWorks.GetErrorInfo((uint)ErrorCodeEnum.参数为空));
                return;
            }

            LightControllerModelEnum modelTypeEnum = (LightControllerModelEnum)Dev.LightControllerParameter.Model;


            try
            {
                CloseDev(Dev);

                // 
                if (modelTypeEnum == LightControllerModelEnum.OPTv1)
                {
                    Dev.LightController = new OPT1();
                    Dev.ConnStatus = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}",
                        Dev.LightControllerParameter.BaudRate,
                        Dev.LightControllerParameter.StopBits,
                        Dev.LightControllerParameter.Parity,
                        Dev.LightControllerParameter.DataBits,
                        0);

                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.OPTv2)
                {
                    Dev.LightController = new OPT2();
                    Dev.ConnStatus = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}",
                         Dev.LightControllerParameter.BaudRate,
                         Dev.LightControllerParameter.StopBits,
                         Dev.LightControllerParameter.Parity,
                         Dev.LightControllerParameter.DataBits,
                         0);
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.OPTv3)
                {
                    Dev.LightController = new OPT3();
                    Dev.ConnStatus = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}",
                        Dev.LightControllerParameter.BaudRate,
                        Dev.LightControllerParameter.StopBits,
                        Dev.LightControllerParameter.Parity,
                        Dev.LightControllerParameter.DataBits,
                        0);
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.OPTControllerAPI)
                {
                    Dev.LightController = new OPTControllerAPI();
                    Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}");
                    Dev.ConnStatus = true;
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.乐视1)
                {
                    Dev.LightController = new LTSControllerApi();
                    int IsOk = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}");
                    Dev.LightController.Setlnitial('$');
                    if (IsOk == 0) Dev.ConnStatus = true;
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.乐视2)
                {
                    Dev.LightController = new LTSControllerApi();
                    int IsOk = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}");
                    Dev.LightController.Setlnitial('#');
                    if (IsOk == 0) Dev.ConnStatus = true;
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.乐视3)
                {
                    Dev.LightController = new LTSControllerApi();
                    int IsOk = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}");
                    Dev.LightController.Setlnitial('&');
                    if (IsOk == 0) Dev.ConnStatus = true;
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.康视达)
                {
                    Dev.LightController = new CSTController();
                    Dev.ConnStatus = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}",
                        Dev.LightControllerParameter.BaudRate,
                        Dev.LightControllerParameter.StopBits,
                        Dev.LightControllerParameter.Parity,
                        Dev.LightControllerParameter.DataBits,
                        0);
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.沃德普PSC3线光)
                {
                    Dev.LightController = new WDPController(1);
                    Dev.ConnStatus = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}",
                        Dev.LightControllerParameter.BaudRate,
                        Dev.LightControllerParameter.StopBits,
                        Dev.LightControllerParameter.Parity,
                        Dev.LightControllerParameter.DataBits,
                        0);
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.沃德普PD5)
                {
                    Dev.LightController = new WDPController(4);
                    Dev.ConnStatus = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}",
                        Dev.LightControllerParameter.BaudRate,
                        Dev.LightControllerParameter.StopBits,
                        Dev.LightControllerParameter.Parity,
                        Dev.LightControllerParameter.DataBits,
                        0);
                    return;
                }

                // 
                if (modelTypeEnum == LightControllerModelEnum.嘉励)
                {
                    Dev.LightController = new JiaLiControl();
                    Dev.ConnStatus = Dev.LightController.Connect($"COM{Dev.LightControllerParameter.PortName}",
                        Dev.LightControllerParameter.BaudRate,
                        Dev.LightControllerParameter.StopBits,
                        Dev.LightControllerParameter.Parity,
                        Dev.LightControllerParameter.DataBits,
                        0);
                    return;
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log4netRecord.ErrorLog.Error(ex);
                Dev.ConnStatus = false;
                return;
            }

            // 不支持的相机类型
            ErrorWorks.ErrorMessageShow($"{modelTypeEnum}", ErrorWorks.GetErrorInfo((uint)ErrorCodeEnum.不支持此相机类型));
            return;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="cameraDev"></param>
        public static void CloseDev(this LightControllerDev Dev)
        {
            try
            {
                if (Dev.LightController != null)
                {

                    Dev.LightController.Dispose();

                }
                Dev.ConnStatus = false;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public static void OpenAllLight(this ObservableCollection<LightControllerDev> Devs)
        {
            try
            {
                foreach (var item in Devs)
                {
                    OpenAllLight(item);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        public static void ColseAllLight(this ObservableCollection<LightControllerDev> Devs)
        {
            try
            {
                foreach (var item in Devs)
                {
                    ColseAllLight(item);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public static void OpenAllLight(this LightControllerDev Dev)
        {
            try
            {
                foreach (var item in Dev.LightControllerParameter.LightChannelItems)
                {
                    Dev.LightController.Open(item.PortNo);
                    Dev.LightController.ChangeValue(item.PortNo, item.Brightness);
                }

            }
            catch (Exception ex)
            {               
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        public static void ColseAllLight(this LightControllerDev Dev)
        {
            try
            {
                foreach (var item in Dev.LightControllerParameter.LightChannelItems)
                {
                    Dev.LightController.Shutdown(item.PortNo);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public static void OpenLight(this LightControllerDev Dev, int channel)
        {
            try
            {
                Dev.LightController.Open(channel);
                if (Dev.LightControllerParameter.LightChannelItems.Find(item => item.PortNo == channel) != null)
                {
                    Dev.LightControllerParameter.LightChannelItems.Find(item => item.PortNo == channel).SwitchState = true;
                }             
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        public static void ColseLight(this LightControllerDev Dev, int channel)
        {
            try
            {
                Dev.LightController.Shutdown(channel);
                if (Dev.LightControllerParameter.LightChannelItems.Find(item => item.PortNo == channel) != null)
                {
                    Dev.LightControllerParameter.LightChannelItems.Find(item => item.PortNo == channel).SwitchState = false;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public static bool GetAllLightOpenState(this ObservableCollection<LightControllerDev> Devs)
        {
            bool state = true;

            try
            {
                foreach (var item in Devs)
                {
                    foreach (var item1 in item.LightControllerParameter.LightChannelItems)
                    {
                        state &= item1.SwitchState;
                    }                 
                }
            }
            catch (Exception ex)
            {
                state = false;
                Log4netRecord.ErrorLog.Error(ex);
            }
            return state;
        }
        #endregion
    }
}
