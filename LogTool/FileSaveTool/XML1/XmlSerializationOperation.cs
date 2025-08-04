using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
namespace LogTool
{
    /// <summary>
    /// XML文件操作类
    /// </summary>
    [Serializable]
    public class XmlSerializationOperation
    {
       // public XmlSerializationOperation() { }

        /// <summary>
        /// XML文件路径
        /// </summary>
        public string filePath = string.Empty;
        private string fileFullPath = string.Empty;
        /// <summary>
        /// 保存XML文件
        /// </summary>
        /// <returns>true=保存成功，false=保存失败</returns>
        public bool Save()
        {
            try
            {
                fileFullPath = string.Format(filePath + ".xml");
                var result = XmlSerialization.ObjectToXml(fileFullPath, this);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 读取XML文件
        /// </summary>
        public object Read()
        {
            object result = null;
            fileFullPath = string.Format(filePath + ".xml");
            try
            {
                if (File.Exists(fileFullPath))
                {
                    result = XmlSerialization.XmlToObject(fileFullPath, this.GetType());
                }
                else
                {
                    if (Save())
                    {
                        return Read();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return result;
        }
    }
}
