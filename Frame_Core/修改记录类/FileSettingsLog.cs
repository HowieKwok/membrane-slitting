using Frame_Core;
using HalconDotNet;
using LogTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace Extensions
{
    /// <summary>
    /// 修改记录日志类 2023.7.22 XuChang
    /// </summary>
    [Serializable]
    public class FileSettingsLog
    {

        /// <summary>
        /// 拷贝的类对象
        /// </summary>
        private object back;

        /// <summary>
        /// 日志路径
        /// </summary>
        [NonSerialized]
        public string FilePath = System.Windows.Forms.Application.StartupPath + "\\record\\参数修改记录\\默认\\";
        /// <summary>
        /// 拷贝一份,然后设置日志路径 再保存修改记录就可以了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void backup<T>(T obj)
        {
            try
            {
                back = obj.DeepClone();
            }
            catch(Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal("错误:" + ex.Message);
            }

        }
        /// <summary>
        /// 保存修改日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">输入类对象</param>
        /// <param name="WorkCard">用户ID</param>
        public void saveChangeLog<T>(T obj, string WorkCard)
        {
            try
            {
                WorkCard = String.Format("权限：{0} ->用户ID: {1}  将 {2}", Project.Instance.userManage.userData.CurrentUser.ToString(), Project.Instance.userManage.userData.CurrentUserID, WorkCard);
                getChange(back, obj, WorkCard);
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal("saveChangeLog 错误:" + ex.Message);
            }

        }

        private static object lockObject = new object();
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg"></param>
        private void WriteLog(string msg)
        {
            try
            {
                lock (lockObject)
                {
                    string tempMsg = string.Format(" {0} , {1} ", DateTime.Now.ToString("yyyy-MM-dd-HH:mm:sss"), msg);

                    string fileName = FilePath + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    DetectIsExist(fileName);
                    //用默认编码和缓冲区大小，,为指定的文件初始化 StreamWriter 类的一个新实例。 如果该文件存在，则可以将其覆盖或向其追加。 如果该文件不存在，则此构造函数将创建一个新文件。
                    try
                    {
                        var logStreamWriter = new StreamWriter(fileName, true, Encoding.GetEncoding("GB2312"));
                        logStreamWriter.WriteLine(tempMsg);
                        logStreamWriter.Close();
                    }
                    catch (Exception ex)
                    {
                         Log4netRecord.ErrorLog.Fatal("WriteLog 错误:" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal(ex);
            }
        }
        /// <summary>
        /// 创建文件夹及文件
        /// </summary>
        /// <param name="filepath">路径</param>
        private void DetectIsExist(string filepath)
        {
            try
            {
                //获取路径文件名
                string str = Path.GetFileName(filepath);
                //如果没有文件名再看是否有目录路径
                if (str.Length == 0)
                {
                    //判断路径的目录是否存在
                    string path = Path.GetDirectoryName(filepath);
                    //如果目录也没存在
                    if (!Directory.Exists(path))
                    {
                        //没有则创建目录
                        Directory.CreateDirectory(path);
                    }
                }
                //如果有文件名
                else
                {
                    //返回目录路径信息
                    string path = Path.GetDirectoryName(filepath);
                    //判断是否有目录
                    if (!Directory.Exists(path))
                    {
                        //没有则创建目录
                        Directory.CreateDirectory(path);
                    }
                    string fillpath = Path.Combine(path, str);
                    if (!File.Exists(fillpath))
                        File.Create(fillpath).Close();
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal("DetectIsExist 错误:" + ex.Message);
            }
        }


        /// <summary>
        /// 获取修改前后对象值
        /// </summary>
        /// <param name="oldMyClass">修改前</param>
        /// <param name="newMyClass">修改后</param>
        /// <param name="WorkCard">用户</param>
        /// <param name="ClassName">用自定义类型名户</param>
        private void getChange(object oldMyClass, object newMyClass, string WorkCard, string ClassName = "")
        {
            try
            {
                string list1name = string.Empty;
                string list2name = string.Empty;
                //新增判断是否是list，或者list<list>,最多两层嵌套
                if (newMyClass != null && typeof(System.Collections.IList).IsAssignableFrom(newMyClass.GetType()))
                {
                    IList newMyClassList = (IList)newMyClass;
                    IList oldMyClassList = (IList)oldMyClass;
                    for (int i = 0; i < newMyClassList.Count; i++)
                    {
                        try
                        {
                            list1name = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.CameraName;
                        }
                        catch (Exception ee)
                        {
                            list1name = (i + 1).ToString();
                        }
                        //新增判断是否是list
                        if (newMyClassList[i] != null && typeof(System.Collections.IList).IsAssignableFrom(newMyClassList[i].GetType()))
                        {
                            IList newMyClassList2 = (IList)newMyClassList[i];
                            IList oldMyClassList2 = (IList)oldMyClassList[i];
                            for (int j = 0; j < newMyClassList2.Count; j++)
                            {
                                list1name = (j + 1).ToString();
                                var isLine = newMyClassList2[j].GetType() == typeof(LineInfo);
                                var isSize = newMyClassList2[j].GetType() == typeof(SizeItemInfo);
                                var isDefect = newMyClassList2[j].GetType() == typeof(DefectItemInfo);
                                if (isLine)
                                {
                                    LineInfo tp = newMyClassList2[j] as LineInfo;
                                    list2name = tp.StrLineName;
                                }
                                if (isSize)
                                {
                                    SizeItemInfo tp = newMyClassList2[j] as SizeItemInfo;
                                    list2name = tp.Name;
                                }
                                if (isDefect)
                                {
                                    DefectItemInfo tp = newMyClassList2[j] as DefectItemInfo;
                                    list2name = tp.Name;
                                }

                                Change(oldMyClassList2[j], newMyClassList2[j], WorkCard, string.Format("{0} _{1} _{2} _{3}", ClassName, list1name, list2name, "_"));
                                // Change(oldMyClassList2[j], newMyClassList2[j], WorkCard, string.Format("{0} 数组序号 {1}-{2}-{3}", ClassName, i + 1, j + 1, "_"));
                            }
                        }
                        else
                        {
                            list1name = (i + 1).ToString();
                            var isLine = oldMyClassList[i].GetType() == typeof(LineInfo);
                            var isSize = oldMyClassList[i].GetType() == typeof(SizeItemInfo);
                            var isDefect = oldMyClassList[i].GetType() == typeof(DefectItemInfo);
                            if (isLine)
                            {
                                LineInfo tp = oldMyClassList[i] as LineInfo;
                                list1name = tp.StrLineName;
                            }
                            if (isSize)
                            {
                                SizeItemInfo tp = oldMyClassList[i] as SizeItemInfo;
                                list1name = tp.Name;
                            }
                            if (isDefect)
                            {
                                DefectItemInfo tp = oldMyClassList[i] as DefectItemInfo;
                                list1name = tp.Name;
                            }

                            Change(oldMyClassList[i], newMyClassList[i], WorkCard, string.Format("{0} _{1} _{2}", ClassName, list1name, "_"));
                            // Change(oldMyClassList[i], newMyClassList[i], WorkCard, string.Format("{0} 数组序号 {1}-{2}", ClassName, i + 1, "_"));
                        }
                    }
                }

                //新增判断是否是数组
                else if (newMyClass != null && newMyClass.GetType().IsArray || typeof(System.Collections.IList).IsAssignableFrom(newMyClass.GetType()))
                {

                    object[] newMyClassArray = (object[])newMyClass;

                    object[] oldMyClassArray = (object[])oldMyClass;

                    for (int i = 0; i < newMyClassArray.Length; i++)
                    {
                        Change(oldMyClassArray[i], newMyClassArray[i], WorkCard, string.Format("{0} 数组序号 {1}{2}", ClassName, i + 1, "_"));
                    }

                }
                else
                {
                    Change(oldMyClass, newMyClass, WorkCard, ClassName);
                }

            }

            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal("getChange 错误:" + ex.Message);
            }

        }


        private void Change(object oldMyClass, object newMyClass, string WorkCard, string ClassName = "")
        {


            // 获取MyClass的Type对象
            Type typeOld = newMyClass.GetType();

            PropertyInfo[] properties = typeOld.GetProperties();

            // 遍历类的所有实例字段（包括私有字段）
            foreach (FieldInfo field in typeOld.GetFields(BindingFlags.DeclaredOnly |
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                //2024.1.8加入检查字段是否被标记为 [NonSerialized]
                if (field.GetCustomAttribute<NonSerializedAttribute>() != null)
                {
                    continue; // 忽略被标记为 [NonSerialized] 的字段
                }

                try
                {

                    // 获取实例字段的值
                    object oldValue = field.GetValue(oldMyClass);
                    //
                    string fieldName = field.Name;
                    // 检查是否为自动生成的后备字段
                    if (fieldName.StartsWith("<") && fieldName.EndsWith(">k__BackingField"))
                    {
                        // 去掉前缀和后缀以获取属性名称
                        string propertyName = fieldName.Substring(1, fieldName.IndexOf(">") - 1);
                        //属性名称
                        fieldName = propertyName;
                    }


                    MemberInfo member = typeOld.GetMember(fieldName)[0];
                    PropertyInfo property = null;
                    string tempFieldName = "未设置Description属性";

                    object newValue = null;

                    if (member.MemberType == MemberTypes.Property)
                    {
                        property = typeOld.GetProperty(fieldName);
                        DescriptionAttribute description = property.GetCustomAttribute<DescriptionAttribute>();

                        if (description != null)
                        {
                            tempFieldName = description.Description;
                        }
                        else
                        {
                            tempFieldName = fieldName;
                        }

                        //通过反射获取新值
                        newValue = property.GetValue(newMyClass);


                        // 判断属性值是否为自定义类型  2024.3.1 XuChang
                        if (IsCustomType(property))
                        {
                            //对于自定义类型加上 自定义类型Description属性
                            tempFieldName += "_";
                            // 是自定义类型，递归调用 getChange 方法
                            getChange(oldValue, newValue, WorkCard, tempFieldName);
                        }
                        else
                        {
                            // 判断属性的值是否有修改，有修改则记录下来
                            IsFieldValueModified(WorkCard, ClassName + tempFieldName, oldValue, newValue);
                        }
                    }
                    else if (member.MemberType == MemberTypes.Field)
                    {
                        var propert = typeOld.GetField(fieldName);
                        //通过反射获取新值
                        newValue = propert.GetValue(newMyClass);
                        DescriptionAttribute description = propert.GetCustomAttribute<DescriptionAttribute>();

                        if (description != null)
                        {
                            tempFieldName = description.Description;
                        }
                        else
                        {
                            tempFieldName = fieldName;
                        }

                        // 判断字段是否为自定义类型  2024.3.1 XuChang
                        if (IsCustomType(propert.FieldType))
                        {
                            //对于自定义类型加上 自定义类型Description属性
                            tempFieldName += "_";
                            // 是自定义类型，递归调用 getChange 方法
                            getChange(oldValue, newValue, WorkCard, tempFieldName);
                        }
                        else
                        {
                            // 判断属性的值是否有修改，有修改则记录下来
                            IsFieldValueModified(WorkCard, ClassName + tempFieldName, oldValue, newValue);
                        }
                    }



                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Fatal("getChange 错误:" + ex.Message);
                }


            }
        }


        /// <summary>
        /// 判断字段是否为自定义类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCustomType(Type type)
        {
            return type.IsClass && type.Namespace != null && !type.Namespace.StartsWith("System") && type != typeof(HTuple);
        }


        /// <summary>
        /// 判断属性是否为自定义类型
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsCustomType(PropertyInfo property)
        {
            return property.PropertyType.IsClass && property.PropertyType != typeof(string)
                                && property.PropertyType != typeof(HTuple);
        }





        // 递归查找带有 DescriptionAttribute 特性的属性
        static string GetDescriptionForCustomType(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                DescriptionAttribute description = (DescriptionAttribute)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute));
                if (description != null)
                {
                    return description.Description;
                }

                // 如果属性的类型为自定义类型，则递归查找该类型的属性
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    string str = GetDescriptionForCustomType(property.PropertyType);
                    if (!string.IsNullOrEmpty(str))
                    {
                        return str;
                    }
                }
            }

            return "未设置Description属性";
        }



        public void IsFieldValueModified(string WorkCard, string fieldName, object oldValue, object newValue)
        {
            try
            {
                string oldValueString = FormatObjectToString(oldValue); // 记录修改前的值


                Type oldValueType = newValue.GetType();
                // 如果是数组类型
                if (oldValue is Array oldArray && newValue is Array newArray)
                {

                    if (!IsArrayEqual(oldArray, newArray))
                    {
                        // 记录修改了的数组元素的序号和修改前值
                        for (int i = 0; i < oldArray.Length; i++)
                        {
                            if (!Equals(oldArray.GetValue(i), newArray.GetValue(i)))
                            {
                                WriteLog(string.Format("用户{0} :  《 {1} 》参数值,\r\n     序号: {2},\r\n     修改前值: {3},\r\n     修改后值: {4}",
                                    WorkCard, fieldName, i + 1, FormatObjectToString(oldArray.GetValue(i)), FormatObjectToString(newArray.GetValue(i))));
                            }
                        }
                    }
                }
                //字典
                else if (oldValueType.IsGenericType && oldValueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {

                    // 记录字典中被修改的键值对
                    foreach (var key in ((IDictionary)oldValue).Keys)
                    {
                        //
                        string tempFieldName = "";
                        var type=((IDictionary)oldValue)[key].GetType();
                        if (IsCustomType(type))
                        {

                            //对于自定义类型加上 自定义类型Description属性
                            tempFieldName =fieldName + "_Key:" + key+"_";

                            var tempOldValue = ((IDictionary)oldValue)[key];
                            var tempNewValue= ((IDictionary)newValue)[key];
                            // 是自定义类型，递归调用 getChange 方法
                            getChange(tempOldValue, tempNewValue, WorkCard, tempFieldName);

                        }
                        else
                        {
                            if (!((IDictionary)newValue).Contains(key) || !Equals(((IDictionary)oldValue)[key], ((IDictionary)newValue)[key]))
                            {
                                WriteLog(string.Format("用户{0} : 《 {1} 》参数值,\r\n     Key: {2},\r\n     修改前值: {3},\r\n     修改后值: {4}",
                                    WorkCard, fieldName, key, FormatObjectToString(((IDictionary)oldValue)[key]), FormatObjectToString(((IDictionary)newValue)[key])));
                            }
                        }
                    }
                }
                else if (oldValue is IList oldList && newValue is IList newList)
                {
                    if (oldList.Count != newList.Count)
                    {
                        WriteLog(string.Format("用户{0} :  《 {1} 》参数值，列表长度不一致", WorkCard, fieldName));
                    }
                    else
                    {
                        for (int i = 0; i < oldList.Count; i++)
                        {
                            var type = oldList[i].GetType();
                            string tempFieldName = "";
                            if (IsCustomType(type))
                            {

                                //对于自定义类型加上 自定义类型Description属性
                                tempFieldName = fieldName+"_";
                                // 是自定义类型，递归调用 getChange 方法
                                getChange(oldList[i], newList[i], WorkCard, tempFieldName);


                            }
                            else
                            {
                                if (!Equals(oldList[i], newList[i]))
                                {
                                    WriteLog(string.Format("用户{0} :  《 {1} 》参数值,\r\n     List队列: {2},\r\n     修改前值: {3},\r\n     修改后值: {4}",
                                        WorkCard, fieldName, i + 1, FormatObjectToString(oldList[i]), FormatObjectToString(newList[i])));
                                }
                            }
                                
                        }
                    }
                }
                else if (oldValue is HTuple oldTuple && newValue is HTuple newTuple)
                {
                    if (!oldTuple.TupleEqual(newTuple))
                    {
                        string oldTupleString = oldTuple.ToString();
                        string newTupleString = newTuple.ToString();

                        WriteLog(string.Format("用户{0} : 《 {1} 》参数值,\r\n     HTuple,\r\n     修改前值: {2},\r\n     修改后值: {3}",
                                    WorkCard, fieldName, oldTupleString, newTupleString));
                    }

                }
                else
                {
                    // 非数组类型的逻辑保持不变
                    bool isEquality = Equals(oldValue, newValue);
                    if (!isEquality)
                    {
                        WriteLog(string.Format("用户{0} :  《 {1} 》参数值,\r\n     修改前值: {2},\r\n     修改后值:{3}",
                            WorkCard, fieldName, oldValueString, FormatObjectToString(newValue)));
                    }
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal("IsFieldValueModified 错误:" + ex.Message);
            }
        }

        /// <summary>
        /// 比较字典是否相等
        /// </summary>
        /// <param name="oldDict"></param>
        /// <param name="newDict"></param>
        /// <returns></returns>
        private bool IsDictionaryEqual(IDictionary oldDict, IDictionary newDict)
        {
            if (oldDict.Count != newDict.Count)
            {
                return false;
            }

            foreach (var key in oldDict.Keys)
            {
                if (!newDict.Contains(key) || !Equals(oldDict[key], newDict[key]))
                {
                    return false;
                }
            }

            return true;
        }





        /// <summary>
        /// 比较数组是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool IsArrayEqual(Array a, Array b)
        {
            if (a.Rank != b.Rank)
            {
                return false;
            }

            for (int i = 0; i < a.Rank; i++)
            {
                if (a.GetLength(i) != b.GetLength(i))
                {
                    return false;
                }
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (!Equals(a.GetValue(i), b.GetValue(i)))
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// 数组则拼在一起
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string FormatObjectToString(object obj)
        {
            if (obj is Array array)
            {
                string[] stringArray = new string[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    stringArray[i] = array.GetValue(i)?.ToString();
                }
                return string.Join(", ", stringArray);
            }
            else
            {
                return obj?.ToString();
            }
        }


        /// <summary>
        /// 读取xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlPath"></param>
        public void Load<T>(string xmlPath)
        {
            try
            {
                if (!System.IO.File.Exists(xmlPath))
                {
                    Save(xmlPath);
                }
                Type tempType = this.GetType();
                System.IO.TextReader reader = new System.IO.StreamReader(xmlPath);
                XmlSerializer serialize = new XmlSerializer(tempType);
                T tempRecipe = (T)serialize.Deserialize(reader);
                System.Reflection.PropertyInfo[] propertyInfos = tempType.GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    if (propertyInfos[i].CanWrite)
                    {
                        propertyInfos[i].SetValue(this, propertyInfos[i].GetValue(tempRecipe, null), null);
                    }
                }
                reader.Close();
                reader.Dispose();
                reader = null;
            }
            catch (Exception ex)
            {
                Save(xmlPath);
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
        }
        /// <summary>
        ///保存xml
        /// </summary>
        /// <param name="xmlPath"></param>
        public void Save(string xmlPath)
        {
            try
            {
                DetectIsExist(xmlPath);
                Type tempType = this.GetType();
                System.IO.TextWriter writer = new System.IO.StreamWriter(xmlPath, false, Encoding.UTF8);
                XmlSerializer serialize = new XmlSerializer(tempType);
                serialize.Serialize(writer, this);
                writer.Close();
                writer.Dispose();
                writer = null;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
        }



    }
}
