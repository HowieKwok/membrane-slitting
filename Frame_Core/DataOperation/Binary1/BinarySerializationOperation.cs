using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 二进制文件操作类
    /// </summary>
    [Serializable]
    public class BinarySerializationOperation : FileSettingsLog
    {

        public BinarySerializationOperation() { }

        /// <summary>
        /// 二进制文件路径
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
                return BinarySerialization.ObjectToBinaryFile(fileFullPath, this);
            }
            catch (Exception)
            {
                
                return false;
            }
        }

        /// <summary>
        /// 读取二进制文件
        /// </summary>
        /// <returns>读取结果</returns>
        public object Read()
        {
            try
            {
                if (File.Exists(fileFullPath))
                {
                    object obj = BinarySerialization.BinaryFileToObject(fileFullPath);
                    return obj;
                }
                else
                {
                    if (Save())
                    {
                       
                    }
                    return BinarySerialization.BinaryFileToObject(fileFullPath);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
