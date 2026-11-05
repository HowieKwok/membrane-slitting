using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// json序列化操作类
    /// </summary>
    [Serializable]
    public class JsonSerializationOperation : FileSettingsLog
    {
        /// <summary>
        /// json文件路径
        /// </summary>
        public string filePath = string.Empty;
        /// <summary>
        /// json文件路径+  filePath +".json"
        /// </summary>
        public string fileFullPath = string.Empty;
        /// <summary>
        /// 保存二进制文件
        /// </summary>
        /// <returns>true=保存成功，false=保存失败</returns>
        public bool Save()
        {
            try
            {
                fileFullPath = string.Format(filePath + ".json");
                return JsonSerialization.ObjectToJsonFile(fileFullPath, this);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 读取二进制文件 --泛型方式，使用： testCraft1 = testCraft.Read<testCraft>();
        /// </summary>
        /// <returns>读取结果</returns>
        public T Read<T>()
        {
            T result = default(T);
            fileFullPath = string.Format(filePath + ".json");
            try
            {
                if (File.Exists(fileFullPath))
                {
                    result = JsonSerialization.JsonFileToObject<T>(fileFullPath);
                }
                else
                {
                    if (Save())
                    {
                        return Read<T>();
                    }
                }
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }

        ///// <summary>
        ///// 读取二进制文件 --强转方式，使用： testCraft1 = (testCraft)(object)testCraft;
        ///// </summary>
        ///// <returns>读取结果</returns>
        //public object ReadObj()
        //{
        //    object result = null;
        //    fileFullPath = string.Format(filePath + ".json");
        //    try
        //    {
        //        if (File.Exists(fileFullPath))
        //        {
        //            result = JsonSerialization.JsonFileToObj(fileFullPath);
        //        }
        //        else
        //        {
        //            if (Save())
        //            {
        //                return ReadObj();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return result;
        //    }
        //    return result;
        //}
    }
}
