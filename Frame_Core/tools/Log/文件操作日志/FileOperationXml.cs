using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Frame_Core
{
    public class FileOperationXml
    {

        /// <summary>
        /// 序列化（写xml文件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public void SerializeToXml<T>(T obj, string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    xml.Serialize(writer, obj);
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }
        /// <summary>
        /// 反序列化（读xml文件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T DeSerializeFromXml<T>(string path)
        {
            T res = default(T);
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    res = (T)xml.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }
            return res;
        }
    }
}
