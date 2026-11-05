using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity.DataOperation
{
    /// <summary>
    /// 二进制序列化
    /// </summary>
    public static class BinarySerialization
    {
        public static readonly object LockObject = new object();
        /// <summary>
        /// 将对象序列化为指定Dat文件
        /// </summary>
        /// <param name="filePath">二进制文件路径</param>
        /// <param name="   ">对象</param>
        /// <returns>是否成功</returns>
        public static bool ObjectToBinaryFile(string filePath, object sourceObject)
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
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            binaryFormatter.Serialize(fileStream, sourceObject);
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
        /// 将二进制文件反序列化为对象
        /// </summary>
        /// <param name="filePath">二进制文件路径</param>
        /// <returns>序列化后的对象</returns>
        public static object BinaryFileToObject(string filePath)
        {
            object result = null;
            lock (LockObject)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            result = binaryFormatter.Deserialize(fileStream);
                        }
                    }
                    catch(Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
                return result;
            }
        }
    }
}
