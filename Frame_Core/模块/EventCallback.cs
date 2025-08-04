using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class EventCallback
    {
        /// <summary>
        /// 相机界面窗体关闭回调函数
        /// </summary>
        public static void CamFormClosingCallback()
        {
            try
            {
                //刷新标定界面名称
           
            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }



        ///// <summary>
        ///// 标定界面窗体关闭回调函数
        ///// </summary>
        ///// <param name="_CalibrationManager"></param>
        //public static void CalibFormClosingCallback(CalibrationManager _CalibrationManager)
        //{
        //    try
        //    {
              
        //    }
        //    catch (Exception ex)
        //    {

        //         Log4netRecord.ErrorLog.Debug(ex);
        //    }
        //}
    }
}
