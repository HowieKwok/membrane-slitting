using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Frame_Core
{
    /// <summary>
    /// XML序列化
    /// </summary>
    public static class XmlSerialization
    {
        public static readonly object LockObject = new object();
        private static Hashtable hashtable = new Hashtable();
        private static XmlSerializer xmlSerializer;

        /// <summary>
        /// 将对象序列化为指定XML文件
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <param name="sourceObject">对象</param>
        /// <returns>是否成功</returns>
        public static bool ObjectToXml(string filePath, object sourceObject)
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
                        using (StreamWriter streamWriter = new StreamWriter(filePath))
                        {
                            Type type = sourceObject.GetType();
                            CreateXMLSerial(type).Serialize(streamWriter, sourceObject);
                            streamWriter.Close();
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
        /// 将指定XML反序列化为对象
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <param name="type">类型</param>
        /// <returns>反序列化后的对象</returns>
        public static object XmlToObject(string filePath, Type type)
        {
            object result = null;
            lock (LockObject)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        using (StreamReader streamWriter = new StreamReader(filePath))
                        {
                            result = CreateXMLSerial(type).Deserialize(streamWriter);
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {

                }
                return result;
            }
        }

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static XmlSerializer CreateXMLSerial(Type type)
        {
            object key = type.Name;
            xmlSerializer = (XmlSerializer)hashtable[key];
            if (xmlSerializer == null)
            {
                xmlSerializer = string.IsNullOrWhiteSpace(type.Name) ? new XmlSerializer(type) :
                            new XmlSerializer(type, new XmlRootAttribute(type.Name));
                hashtable[key] = xmlSerializer;
            }
            return xmlSerializer;
        }
    }
}
