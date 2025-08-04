using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using LogTool;

namespace Frame_Core
{
    public class XmlExSerializer
    {
        /// <summary>
        /// 将对象T序列化到文件中
        /// </summary>
        /// <typeparam name="T">对象标志</typeparam>
        /// <param name="obj">要序列的对象</param>
        /// <param name="path">文件路径</param>
        public static void Serialize<T>(T obj, string xmlPath)
        {
            //获得当前实例的类型
            Type tempType = obj.GetType();
            //实例化要写入XML的文档
            System.IO.TextWriter writer = new System.IO.StreamWriter(xmlPath);

            try
            {
                //创建与输入类型一致的序列化器
                XmlSerializer serialize = new XmlSerializer(tempType);
                //将对象序列化并写入到指定的XML文档中
                serialize.Serialize(writer, obj);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                writer.Close();
                writer.Dispose();
            }
        }

        public static T Deserialize<T>(string xmlPath)
        {
            if (!System.IO.File.Exists(xmlPath))
            {
                return default(T);
            }
            System.IO.TextReader reader = new System.IO.StreamReader(xmlPath);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
                return default(T);
            }
            finally
            {
                reader.Close();
                reader.Dispose();
            }
        }
    }
}
