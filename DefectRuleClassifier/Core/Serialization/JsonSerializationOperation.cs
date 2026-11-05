using System;
using System.IO;

namespace DefectRuleClassifier.Core.Serialization
{
    /// <summary>
    /// 分类规则参数 JSON 保存基类。
    /// 接口形态对齐框架 JsonSerializationOperation：filePath 不带扩展名，Save/Read 自动使用 filePath + ".json"。
    /// </summary>
    [Serializable]
    public class JsonSerializationOperation
    {
        /// <summary>不带 .json 扩展名的 JSON 文件路径。</summary>
        public string filePath = string.Empty;

        /// <summary>实际 JSON 文件完整路径，即 filePath + ".json"。</summary>
        public string fileFullPath = string.Empty;

        public bool Save()
        {
            try
            {
                fileFullPath = filePath + ".json";
                return JsonSerialization.ObjectToJsonFile(fileFullPath, this);
            }
            catch
            {
                return false;
            }
        }

        public T Read<T>()
        {
            fileFullPath = filePath + ".json";
            try
            {
                if (File.Exists(fileFullPath))
                    return JsonSerialization.JsonFileToObject<T>(fileFullPath);

                if (Save())
                    return Read<T>();
            }
            catch
            {
                return default(T);
            }
            return default(T);
        }
    }
}
