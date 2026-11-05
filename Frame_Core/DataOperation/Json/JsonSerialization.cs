using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 二进制序列化
    /// </summary>
    public static class JsonSerialization
    {
        public static readonly object LockObject = new object();
        /// <summary>
        /// 将对象序列化为指定Dat文件
        /// </summary>
        /// <param name="filePath">二进制文件路径</param>
        /// <param name="   ">对象</param>
        /// <returns>是否成功</returns>
        public static bool ObjectToJsonFile(string filePath, object sourceObject)
        {
            lock (LockObject)
            {
                if (!string.IsNullOrWhiteSpace(filePath) && sourceObject != null)
                {
                    string folderPath = filePath.Substring(0, filePath.LastIndexOf(@"\"));
                    if (Directory.Exists(folderPath) == false)
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    try
                    {
                        // 将User对象序列化为JSON字符串
                        string jsonOutput = JsonConvert.SerializeObject(sourceObject, Formatting.Indented);

                        // 将JSON字符串写入文件
                        using (StreamWriter fileWriter = new StreamWriter(filePath))
                        {
                            fileWriter.Write(jsonOutput);
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 将二进制文件反序列化为对象--泛型方式，使用： testCraft1 = testCraft.Read<testCraft>();
        /// </summary>
        /// <param name="filePath">二进制文件路径</param>
        /// <returns>序列化后的对象</returns>
        public static T JsonFileToObject<T>(string filePath)
        {
            T result = default(T);
            lock (LockObject)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        // 打开并读取文件内容
                        using (StreamReader fileReader = new StreamReader(filePath))
                        {
                            // 从文件中获取JSON字符串
                            string jsonContent = fileReader.ReadToEnd();

                            // 使用JsonConvert.DeserializeObject反序列化JSON字符串为User对象
                            result = JsonConvert.DeserializeObject<T>(jsonContent);
                        }
                    }
                    catch (Exception)
                    {
                        return result;
                    }
                }
                else
                {
                    return result;
                }
                return result;
            }
        }

        ///// <summary>
        ///// 将二进制文件反序列化为对象--强转方式，使用： testCraft1 = (testCraft)(object)testCraft;
        ///// </summary>
        ///// <param name="filePath">二进制文件路径</param>
        ///// <returns>序列化后的对象</returns>
        //public static object JsonFileToObj(string filePath)
        //{
        //    object result = null;
        //    lock (LockObject)
        //    {
        //        if (File.Exists(filePath))
        //        {
        //            try
        //            {
        //                // 打开并读取文件内容
        //                using (StreamReader fileReader = new StreamReader(filePath))
        //                {
        //                    // 从文件中获取JSON字符串
        //                    string jsonContent = fileReader.ReadToEnd();

        //                    // 使用JsonConvert.DeserializeObject反序列化JSON字符串为User对象
        //                    result = JsonConvert.DeserializeObject(jsonContent);
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                return result;
        //            }
        //        }
        //        else
        //        {
        //            return result;
        //        }
        //        return result;
        //    }
        //}
    }
}
