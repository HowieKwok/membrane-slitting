using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class SizePointInspectionManage
    {
        /// <summary>
        /// 尺寸点检参数
        /// </summary>
        public SizePointInspectionParam sizePointInspectionParam;


        /// <summary>
        /// 保存尺寸点检数据
        /// </summary>
        public FileOperationCSV SaveSizeCheckPointCSV = new FileOperationCSV();//保存尺寸点检数据

        /// <summary>
        /// 相机尺寸实时点检数据
        /// </summary>
        public string SizeRealTimeCheckPointPath = $"{AppDomain.CurrentDomain.BaseDirectory}生产数据\\生产数据";      //存放点检数据


        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitParam(string path)
        {

            try
            {
                if (sizePointInspectionParam == null) sizePointInspectionParam = new SizePointInspectionParam();
                sizePointInspectionParam.filePath = path;
                sizePointInspectionParam = (SizePointInspectionParam)sizePointInspectionParam.Read();
                sizePointInspectionParam.filePath = path;

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
