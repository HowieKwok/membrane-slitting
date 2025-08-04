using FlyCapture2Managed;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Frame_Core
{
    /// <summary>
    /// 点检管理
    /// </summary>
    public class SizeCheckPointManage
    {
        /// <summary>
        /// 尺寸点检参数
        /// </summary>
        public SizeCheckPointParam sizeCheckPointParam;

        /// <summary>
        /// 保存尺寸点检数据
        /// </summary>
        public FileOperationCSV SaveSizeCheckPointCSV = new FileOperationCSV();//保存尺寸点检数据

     
        /// <summary>
        /// 相机尺寸实时点检数据
        /// </summary>
        public string SizeRealTimeCheckPointPath = "E:\\生产数据\\生产数据";      //存放点检数据

        /// <summary>
        /// 初始化相机点检参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitCamSizeCheckPointParam(string path)
        {

            try
            {
                if (sizeCheckPointParam == null) sizeCheckPointParam = new SizeCheckPointParam();
                sizeCheckPointParam.filePath = path;
                sizeCheckPointParam = (SizeCheckPointParam)sizeCheckPointParam.Read();
                sizeCheckPointParam.filePath = path;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        

    }
}
