using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;


using LogTool;

namespace Frame_Core
{
    public class SaveAndLoad_Data
    {
        /// <summary>
        /// 将类序列化为二进制文件保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ImageClass"></param>
        /// <param name="Path"></param>
        public static void Save(object ObjClass, string Path)
        {
            object Anyobject = ObjClass;
            using (FileStream fs = new FileStream(Path, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Anyobject);
                fs.Flush();
                fs.Close();
            }
        }

        /// <summary>
        /// 读取二进制文件并将二进制数据反序列化为指定类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static object Load(string Path)
        {
            object anyObj = System.Activator.CreateInstance<object>();
            try
            {
                if (File.Exists(Path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fs = new FileStream(Path, FileMode.Open);
                    anyObj = bf.Deserialize(fs);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal("读取dat文件失败");
            }
            return anyObj;
        }




        public static void Save_Data<T>(T Anyobject, string Path)
        {
            try
            {
                DetectIsExist(Path);
                XmlExSerializer.Serialize<T>(Anyobject, Path);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }

        public static T Load_Data<T>(string Path)
        {
            T anyObj = System.Activator.CreateInstance<T>();
            try
            {
                anyObj = XmlExSerializer.Deserialize<T>(Path);
                return anyObj;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
                return anyObj;
            }
        }


        /// <summary>
        /// 判断路径是否存在，不存在则创建 2021.06.20situ
        /// </summary>
        /// <param name="filepath">路径是文件或文件夹</param>
        public static void DetectIsExist(string filepath)
        {
            try
            {
                //1.去除路径下非法字符
                string[] p = filepath.Split(new char[2] { '/', '\\' });
                string path = p[0] + "\\";
                for (int i = 1; i < p.Length - 1; i++)
                {
                   // CheckFileNameError(p[i], out p[i]);
                    path += p[i] + "\\";
                }
                path += p[p.Length - 1];
                //2.获取路径下文件后缀名（斜杠后字符串最后一个字符"."后面的字符被认为是文件后缀名）
                string Extension = Path.GetExtension(path);
                if (Extension.Length == 0)
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                else
                {
                    //3.判断后缀名是否包含数字,包含则认为不是正常后缀名（.7z除外,不考虑），防止例如：2021.06.20 被判为文件名
                    string name = System.Text.RegularExpressions.Regex.Replace(Extension, @"[^0-9]+", "");
                    if (name.Length == 0)
                    {
                        string pathname = Path.GetDirectoryName(path);
                        if (!Directory.Exists(pathname))
                            Directory.CreateDirectory(pathname);
                    }
                    else
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                    }
                }
            }
            catch (Exception ex)
            {

                //  LogName.ErrorLog.Fatal(ex);
            }
        }

    }
}
