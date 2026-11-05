using Newtonsoft.Json;
using System;
using System.IO;

namespace DefectRuleClassifier.Core.Serialization
{
    /// <summary>
    /// 分类规则 DLL 内部使用的 JSON 序列化工具。
    /// 写法对齐框架 JsonSerialization：统一把对象保存为缩进 JSON 文件，但不引用 Frame_Core，避免分类规则 DLL 和主框架强耦合。
    /// </summary>
    public static class JsonSerialization
    {
        private static readonly object LockObject = new object();

        public static bool ObjectToJsonFile(string filePath, object sourceObject)
        {
            lock (LockObject)
            {
                if (string.IsNullOrWhiteSpace(filePath) || sourceObject == null)
                    return false;

                string folderPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(folderPath) && !Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    };
                    string jsonOutput = JsonConvert.SerializeObject(sourceObject, settings);
                    File.WriteAllText(filePath, jsonOutput);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static T JsonFileToObject<T>(string filePath)
        {
            lock (LockObject)
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                    return default(T);

                try
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    };
                    string jsonContent = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<T>(jsonContent, settings);
                }
                catch
                {
                    return default(T);
                }
            }
        }
    }
}
