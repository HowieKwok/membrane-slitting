using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity.DataOperation
{
    public static class JsonSerializationOperationExtension
    {
        public static void SaveExtension(this JsonSerializationOperation param, string path, string fileName)
        {
            if (param == null) return;
            string strPath = Path.Combine(path, fileName);
            param.filePath = strPath;
            param.Save();

        }

        public static T ReadExtension<T>(this JsonSerializationOperation param, string path, string fileName) where T : JsonSerializationOperation, new()
        {
            if (param == null) param = new T();
            string strPath = Path.Combine(path, fileName);
            T paramTemp;
            param.filePath = strPath;
            paramTemp = param.Read<T>();
            if (paramTemp == null)
            {
                paramTemp = new T();
                //ErrorWorks.ErrorMessageShow(strPath, ErrorWorks.GetErrorInfo((uint)ErrorCodeEnum.参数加载失败));
                Log4netRecord.ErrorLog.Error($"参数加载失败\r\n路径：{strPath}");
            }

            paramTemp.filePath = strPath;
            paramTemp.Init();
            return paramTemp;
        }

        public static T ReadExtension<T>(this JsonSerializationOperation param, string path, string fileName,int length) where T : JsonSerializationOperation, new()
        {
            if (param == null) param = new T();
            string strPath = Path.Combine(path, fileName);
            T paramTemp;
            param.filePath = strPath;
            paramTemp = param.Read<T>();
            if (paramTemp == null)
            {
                paramTemp = new T();
                //ErrorWorks.ErrorMessageShow(strPath, ErrorWorks.GetErrorInfo((uint)ErrorCodeEnum.参数加载失败));
                Log4netRecord.ErrorLog.Error($"参数加载失败\r\n路径：{strPath}");
            }

            paramTemp.filePath = strPath;
            paramTemp.Init(length);
            return paramTemp;
        }
    }
}
