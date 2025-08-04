using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class LightManager 
    {
        public LightPamater LightParam { get; set; }
        public LightControl Light;
        /// <summary>
        /// 初始化状态
        /// </summary>
        public bool initState;

        public bool InitLightParam(string path, Boolean isCopy = false)
        {
            try
            {
                if (!isCopy)
                {
                    if (LightParam == null) LightParam = new LightPamater();
                    LightParam.filePath = path;
                    LightParam.Init();
                    LightParam = (LightPamater)LightParam.Read();
                    LightParam.filePath = path;
                }
                else
                {
                    if (LightParam == null) LightParam = new LightPamater();
                    LightParam.filePath = path;
                    LightParam.Save();
                }
            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }

            return true;

        }
        public bool IniLightSystem()
        {
            bool bool_CamIni = false;
            try
            {

                if (initState) Close();
               
                initState = false;
                Thread.Sleep(200);
                GetLight(LightParam.LightType, "COM" + LightParam.LightCOM.ToString());
                if (initState)
                {
                    OpenColseLight(true);
                }


            }
            catch (Exception ex)
            {

                initState = false;
                 Log4netRecord.ErrorLog.Debug(ex);
            }

            return initState;
        }
        public bool Close()
        {
            try
            {
                if (Light != null)
                {
                    OpenColseLight(false);
                    Light.Dispose();                 
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
          
            return true;
        }
        object lightOnOff_lock = new object();
        public bool openOrCloseFlag = false;
        /// <summary>
        /// 光源开关
        /// </summary>
        /// <param name="OpenOrClose">true为开</param>
        /// <returns></returns>
        public bool OpenColseLight(bool OpenOrClose)
        {
            try
            {
                lock (lightOnOff_lock)
                {
                    if (OpenOrClose)
                    {
                        Light.Open(1);
                        Thread.Sleep(50);
                        Light.ChangeValue(1, LightParam.LightChannel1);
                        Thread.Sleep(50);

                        Light.Open(2);
                        Thread.Sleep(50);
                        Light.ChangeValue(2, LightParam.LightChannel2);
                        Thread.Sleep(50);

                        Light.Open(3);
                        Thread.Sleep(50);
                        Light.ChangeValue(3, LightParam.LightChannel3);
                        Thread.Sleep(50);

                        Light.Open(4);
                        Thread.Sleep(50);
                        Light.ChangeValue(4, LightParam.LightChannel4);
                        Thread.Sleep(50);

                        Light.Open(5);
                        Thread.Sleep(50);
                        Light.ChangeValue(5, LightParam.LightChannel5);
                        Thread.Sleep(50);

                        Light.Open(6);
                        Thread.Sleep(50);
                        Light.ChangeValue(6, LightParam.LightChannel6);
                        Thread.Sleep(50);

                        Light.Open(7);
                        Thread.Sleep(50);
                        Light.ChangeValue(7, LightParam.LightChannel7);
                        Thread.Sleep(50);

                        Light.Open(8);
                        Thread.Sleep(50);
                        Light.ChangeValue(8, LightParam.LightChannel8);
                        Thread.Sleep(50);

                        openOrCloseFlag = true;
                    }
                    else
                    {
                        Light.Shutdown(1);
                        Thread.Sleep(50);
                        Light.Shutdown(2);
                        Thread.Sleep(50);
                        Light.Shutdown(3);
                        Thread.Sleep(50);
                        Light.Shutdown(4);
                        Thread.Sleep(50);
                        Light.Shutdown(5);
                        Thread.Sleep(50);
                        Light.Shutdown(6);
                        Thread.Sleep(50);
                        Light.Shutdown(7);
                        Thread.Sleep(50);
                        Light.Shutdown(8);
                        Thread.Sleep(50);
                        openOrCloseFlag = false;
                    }

                    return true;
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return false;
        }
        /// <summary>
        /// 根据输入类型返回对应相应光源类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private LightControl GetLight(string LightType, string comPort)
        {
            bool ComStatus1 = false;
            switch (LightType)
            {
                case "OPTv1（9600）":
                    Light = new OPT1();
                    ComStatus1 = Light.Connect(comPort, 9600, 1, 0, 8, 0);
                    break;
                case "OPTv2（38400）":
                    Light = new OPT2();
                    ComStatus1 = Light.Connect(comPort, 38400, 1, 0, 8, 0);//当修改了波特率之后用修改后的波特率才能连接上
                    break;
                case "OPTControllerAPI":
                    ComStatus1 = false;
                    Light = new OPTControllerAPI();
                    Light.Connect(comPort);
                    ComStatus1 = true;
                    break;
                case "OPTv3":
                    Light = new OPT3();
                    ComStatus1 = Light.Connect(comPort, 38400, 1, 0, 8, 0);//当修改了波特率之后用修改后的波特率才能连接上                       
                    break;
                case "乐视1（'$'）"://乐视源控制器
                    ComStatus1 = false;
                    Light = new LTSControllerApi();
                    int IsOk1 = Light.Connect(comPort);
                    Light.Setlnitial('$');
                    if (IsOk1 == 0)
                        ComStatus1 = true;
                    break;
                case "乐视2（'#'）":
                    ComStatus1 = false;
                    Light = new LTSControllerApi();
                    int IsOk2 = Light.Connect(comPort);
                    Light.Setlnitial('#');
                    if (IsOk2 == 0)
                        ComStatus1 = true;
                    break;
                case "乐视3（'&'）":
                    ComStatus1 = false;
                    Light = new LTSControllerApi();
                    int IsOk3 = Light.Connect(comPort);
                    Light.Setlnitial('&');
                    if (IsOk3 == 0)
                        ComStatus1 = true;
                    break;
                case "CST（康视达）"://CST  康视达光源控制器
                    Light = new CSTController();
                    ComStatus1 = Light.Connect(comPort, 38400, 1, 0, 8, 0);//当修改了波特率之后用修改后的波特率才能连接上                       
                    break;
                case "WDP（沃德普38400）": //WDP  沃德普源控制器
                    Light = new WDPController(1);
                    ComStatus1 = Light.Connect(comPort, 38400, 1, 0, 8, 0);//当修改了波特率之后用修改后的波特率才能连接上                       
                    break;
                case "WDP（沃德普19200）": //WDP  沃德普源控制器
                    Light = new WDPController(4);
                    ComStatus1 = Light.Connect(comPort, 19200, 1, 0, 8, 0);//当修改了波特率之后用修改后的波特率才能连接上                       
                    break;
                case "嘉励（19200）"://JiaLi  嘉励光源控制器
                    Light = new JiaLiControl();
                    ComStatus1 = Light.Connect(comPort, 19200, 1, 0, 8, 0);//当修改了波特率之后用修改后的波特率才能连接上                   
                    break;
                default:
                    Light = new OPT1();
                    break;
            }
            initState = ComStatus1;
            return null;
        }
    }
}
