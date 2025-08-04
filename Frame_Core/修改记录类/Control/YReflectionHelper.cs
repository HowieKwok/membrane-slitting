using System;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Frame_Core;
using LogTool;

namespace YYSD
{
    /// <summary>
    /// 反射操作类v1.0
    /// </summary>
    public class YReflectionHelper
    {
        #region >>>对象内的属性与特性
        #region >>>Base Methods
        /// <summary>
        /// 获取类标量的所有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<T>(string MemberName)
        {
            PropertyInfo property = null;
            try
            {
                //Type type = typeof(T);
                //string className = type.Name;
                //PropertyInfo[] ProperInfo = typeof(T).GetProperties();
                //System.Reflection.PropertyInfo[] properties = type.GetProperties(); //获取所有属性
                //foreach (System.Reflection.PropertyInfo property in properties)
                //{
                //    object objVal = property.GetValue(Obj, null);//旧参数类中,属性中存放的值
                //    object[] obj_Description = property.GetCustomAttributes(typeof(DescriptionAttribute), true);//获取对应属性注释
                //    string str_description = ((DescriptionAttribute)obj_Description[0]).Description;
                //    string strValName = property.Name.ToString();
                //    if (property.Name == MemberName)
                //    {
                //        outobj = property;
                //        break;
                //    }
                //}
                ////outobj = type.GetProperty(MemberName);//设置类中的成员属性的值

                Type type = typeof(T);
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                property = type.GetProperty(MemberName);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return property;
        }
        /// <summary>
        /// 获取类标量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(T Obj, string MemberName)
        {
            object getVal = null;
            //object setVal = null;
            try
            {
                Type type = typeof(T);
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                PropertyInfo p = type.GetProperty(MemberName);
                if (p != null)
                {
                    //p.SetValue(Obj, setVal); //设置类中的成员属性的值
                    getVal = p.GetValue(Obj); //获取类中的成员属性的值
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return getVal;
        }
        /// <summary>
        /// 设置类标量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        public static object SetPropertyValue<T>(T Obj, string MemberName, object setVal)
        {
            object getVal = null;
            if (setVal == null) return getVal;
            try
            {
                //Type type = typeof(T);
                //string className = type.Name;
                //PropertyInfo[] ProperInfo = typeof(T).GetProperties();
                //System.Reflection.PropertyInfo[] properties = type.GetProperties(); //获取所有属性
                //PropertyInfo p = type.GetProperty(MemberName);
                //if (p != null)
                //{
                //    //p.SetValue(Obj, setVal); //设置类中的成员属性的值
                //    getVal = p.GetValue(Obj); //获取类中的成员属性的值
                //}

                Type type = typeof(T);
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                PropertyInfo p = type.GetProperty(MemberName);
                if (p != null)
                {
                    getVal = p.GetValue(Obj); //获取类中的成员属性的值
                    if (setVal.GetType().Equals(getVal.GetType()))//防止设置数据混乱
                        p.SetValue(Obj, setVal); //设置类中的成员属性的值
                    getVal = p.GetValue(Obj); //获取类中的成员属性的值
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return getVal;
        }

        /// <summary>
        /// 获取类标量的描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        public static string GetDescription<T>(T objData, string MemberName)
        {
            string getVal = "";
            try
            {
                //Type t = typeof(T);
                //string className = t.Name;
                //PropertyInfo[] ProperInfo = typeof(T).GetProperties();
                ////获取所有属性
                //System.Reflection.PropertyInfo[] properties = t.GetProperties();
                //foreach (System.Reflection.PropertyInfo property in properties)
                //{
                //    object objVal = property.GetValue(objData, null);//旧参数类中,属性中存放的值
                //    object[] obj_Description = property.GetCustomAttributes(typeof(DescriptionAttribute), true);//获取对应属性注释
                //    string str_description = ((DescriptionAttribute)obj_Description[0]).Description;
                //    string strValName = property.Name.ToString();
                //    if (property.Name == MemberName)
                //    {
                //        getVal = str_description;
                //        break;
                //    }
                //}

                Type type = typeof(T);
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                PropertyInfo p = type.GetProperty(MemberName);
                if (p != null)
                {
                    object[] objs = p.GetCustomAttributes(typeof(DescriptionAttribute), true);//获取对应属性注释
                    if (objs.Length > 0) getVal = ((DescriptionAttribute)objs[0]).Description;
                }

            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return getVal;
        }
        /// <summary>
        /// 获取类标量的任意特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        public static A GetAttribute<T, A>(T objData, string MemberName)
        {
            object obj = null;
            A getVal = (A)obj;
            try
            {
                Type type = typeof(T);
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                PropertyInfo p = type.GetProperty(MemberName);
                if (p != null)
                {
                    object[] objs = p.GetCustomAttributes(typeof(A), true);//获取对应属性注释
                    if (objs.Length > 0) getVal = (A)objs[0];
                }

            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return getVal;
        }

        /// <summary>
        /// 获取变量自身的名字，如string name，使用后可以获得变量的名字“name”
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string GetVarName<T>(System.Linq.Expressions.Expression<Func<T>> exp)
        {
            ////测试例子
            //int b = 1;
            //string abc = "123";
            //string name1 = YReflectionHelper.GetVarName(() => abc);
            //string name2 = YReflectionHelper.GetVarName(() => b);

            return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
        }
        /// <summary>
        /// c# 6.0 支持nameof方法去获取变量名
        /// </summary>
        private void GetVarName()
        {
            //string abc = "123";
            //string name = nameof(abc);
        }
        #endregion

        #region >>>调用方法实例
        /// <summary>
        /// 调用方法实例
        /// 例如：object result = ReflectionHelper.GetInvokeMethod("YYSD.dll", "YYSD", "YMath", "Add", new object[] { 1, 4 }).ToString();
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="nameSpace">命名空间</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static T GetInvokeMethod<T>(string assemblyName, string nameSpace, string className, string methodName, object[] paras)
        {
            try
            {
                //命名空间.类名,程序集
                string path = nameSpace + "." + className + "," + assemblyName;
                //加载类型
                Type type = Type.GetType(path);
                //根据类型创建实例
                object obj = Activator.CreateInstance(type, true);
                //加载方法参数类型及方法
                MethodInfo method = null;
                if (paras != null && paras.Length > 0)
                {
                    //加载方法参数类型
                    Type[] paratypes = new Type[paras.Length];
                    for (int i = 0; i < paras.Length; i++)
                    {
                        paratypes[i] = paras[i].GetType();
                    }
                    //加载有参方法
                    method = type.GetMethod(methodName, paratypes);
                }
                else
                {
                    //加载无参方法
                    method = type.GetMethod(methodName);
                }
                //类型转换并返回
                return (T)method.Invoke(obj, paras);
            }
            catch
            {
                //发生异常时，返回类型的默认值。
                return default(T);
            }
        }
        /// <summary>
        /// 调用方法实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="nameSpace">命名空间</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static object GetInvokeMethod(string assemblyPath, string nameSpace, string className, string methodName, object[] paras)
        {
            object result = null;
            try
            {
                //DLL所在的绝对路径                 
                //string path = AppDomain.CurrentDomain.BaseDirectory + "YYSD.dll";
                string path = "YYSD.dll";//相对路径
                path = assemblyPath;
                Assembly assembly = Assembly.LoadFrom(path);
                //注意写法：程序集.类名  
                Type type = assembly.GetType($"{nameSpace}.{className}");
                //获取类中的公共方法GetResule                                              
                MethodInfo methed = type.GetMethod(methodName);
                //创建对象的实例
                object instance = System.Activator.CreateInstance(type);
                //执行方法  new object[]为方法中的参数
                result = methed.Invoke(instance, paras);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return result;
        }
        #endregion
        #endregion

        #region >>>程序集内的对象
        #region >>>创建对象实例
        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="nameSpace">命名空间</param>
        /// <param name="className">类名</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string assemblyName, string nameSpace, string className)
        {
            try
            {
                //命名空间.类名,程序集
                string path = nameSpace + "." + className + "," + assemblyName;
                //加载类型
                Type type = Type.GetType(path);
                //根据类型创建实例
                object obj = Activator.CreateInstance(type, true);
                //类型转换并返回
                return (T)obj;
            }
            catch
            {
                //发生异常时，返回类型的默认值。
                return default(T);
            }
        }

        #endregion

        /// <summary>
        /// 获取程序集内所有对象
        /// </summary>
        /// <param name="assemblyPath">输入：程序集路径</param>
        /// <returns></returns>
        public static List<Type> GetTypes(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            List<Type> Types = assembly.GetTypes().ToList();
            return Types;
        }
        /// <summary>
        /// 获取程序集内具有特定特性的所有对象
        /// </summary>
        /// <typeparam name="attribute">输入：特性类型</typeparam>
        /// <param name="assemblyPath">输入：程序集路径</param>
        /// <returns></returns>
        public static List<Type> GetTypes<attribute>(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            List<Type> Types = assembly.GetTypes()
                                .Where(t => t.GetCustomAttributes(typeof(attribute), false)
                                .Any()).ToList();
            return Types;
        }

        /// <summary>
        /// 获取当前执行程序集内所有对象
        /// </summary>
        /// <param name="assemblyPath">输入：程序集路径</param>
        /// <returns></returns>
        public static List<Type> GetExecutingTypes()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> Types = assembly.GetTypes().ToList();
            return Types;
        }
        /// <summary>
        /// 获取当前执行程序集内具有特定特性的所有对象
        /// </summary>
        /// <typeparam name="attribute">输入：特性类型</typeparam>
        /// <param name="assemblyPath">输入：程序集路径</param>
        /// <returns></returns>
        public static List<Type> GetExecutingTypes<attribute>()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<Type> Types = assembly.GetTypes()
                                .Where(t => t.GetCustomAttributes(typeof(attribute), false)
                                .Any()).ToList();
            return Types;
        }
        /// <summary>
        /// 获取对象内所有对象方法
        /// </summary>
        /// <param name="type">输入：程序集路径</param>
        /// <returns></returns>
        public static List<MethodInfo> GetMethods(Type type)
        {
            //object obj
            //Type type = obj.GetType();
            List<MethodInfo> Methods = type.GetMethods().ToList();
            return Methods;
        }
        /// <summary>
        /// 获取对象内具有特定特性的所有方法
        /// </summary>
        /// <typeparam name="attribute">输入：特性类型</typeparam>
        /// <param name="type">输入：对象类型</param>
        /// <returns></returns>
        public static List<MethodInfo> GetMethods<attribute>(Type type)
        {
            //object obj
            //Type type = obj.GetType();
            List<MethodInfo> Methods = type.GetMethods()
                                       .Where(t => t.GetCustomAttributes(typeof(attribute), false)
                                       .Any()).ToList();
            return Methods;
        }
        /// <summary>
        /// 获取对象内所有方法
        /// </summary>
        /// <typeparam name="T">输入：对象类型</typeparam>
        /// <returns></returns>
        public static List<MethodInfo> GetMethods<T>()
        {
            Type type = typeof(T);
            List<MethodInfo> Methods = type.GetMethods().ToList();

            return Methods;
        }
        /// <summary>
        /// 获取对象内具有特定特性的所有方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="attribute"></typeparam>
        /// <returns></returns>
        public static List<MethodInfo> GetMethods<T, attribute>()
        {
            Type type = typeof(T);
            List<MethodInfo> Methods = type.GetMethods()
                                       .Where(t => t.GetCustomAttributes(typeof(attribute), false)
                                       .Any()).ToList();

            return Methods;
        }
        #endregion

    }
    /// <summary>
    /// 反射操作类v1.1
    /// </summary>
    public class YReflection : YReflectionHelper
    {
        #region >>>其他
        /// <summary>
        /// 保存类的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="filepath"></param>
        public static void SaveClass<T>(T objData, string filepath)
        {
            //================================================================================================================================================
            //======【】如果存在，就不产生
            if (System.IO.File.Exists(filepath)) return;


            System.IO.StreamWriter stream = null;
            StringBuilder DataLine = new StringBuilder();
            DataLine.Append("名称字段,数值,数据类型,注释描述\n");
            try
            {
                Type t = typeof(T);
                string className = t.Name;
                PropertyInfo[] ProperInfo = typeof(T).GetProperties();
                //获取所有方法
                System.Reflection.MethodInfo[] methods = t.GetMethods();
                string strText = "";
                foreach (System.Reflection.MethodInfo method in methods)
                {
                    strText += method.Name + System.Environment.NewLine;
                }
                //获取所有成员
                System.Reflection.MemberInfo[] members = t.GetMembers();

                //获取所有属性
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                foreach (System.Reflection.PropertyInfo property in properties)
                {
                    object objVal = property.GetValue(objData, null);//旧参数类中,属性中存放的值
                    object[] objDescription = property.GetCustomAttributes(typeof(DescriptionAttribute), true);//获取对应属性注释

                    string strValDescription = ((DescriptionAttribute)objDescription[0]).Description;
                    string strValName = property.Name.ToString();
                    string strVal = objVal.ToString();
                    Type typeValue = property.PropertyType;



                    if (objVal is IList)
                    {
                        for (int i = 0; i < ((IList)objVal).Count; i++)
                        {
                            object objVal_Tmp = ((IList)objVal)[i];
                            strValName = property.Name.ToString() + i.ToString();
                            strVal = objVal_Tmp.ToString();
                            strValDescription = ((DescriptionAttribute)objDescription[0]).Description + i.ToString();
                            Type typeVal = objVal_Tmp.GetType();
                            DataLine.Append(strValName + ",[" + strVal + "]," + typeVal.Name + "," + strValDescription + "\n");
                        }
                    }
                    else
                    {
                        DataLine.Append(strValName + ",[" + strVal + "]," + typeValue.Name + "," + strValDescription + "\n");
                    }
                }




                //================================================================================================================================================
                //======【】判断CSV文件是否存在 则删除
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                //stream = new System.IO.StreamWriter(filepath, false, Encoding.UTF8);
                stream = new System.IO.StreamWriter(filepath, true, Encoding.GetEncoding("GB2312"));
                stream.WriteLine(DataLine);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            finally
            {
                DataLine.Clear();
                if (stream != null)
                {
                    stream.Flush();
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
        /// <summary>
        /// 获得类所有属性名字与值，以字典形式输出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetClassInfo<T>(T objData)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Dictionary<string, object> dict_obj = new Dictionary<string, object>();
            try
            {
                Type t = typeof(T);
                string className = t.Name;
                PropertyInfo[] ProperInfo = typeof(T).GetProperties();
                //获取所有方法
                System.Reflection.MethodInfo[] methods = t.GetMethods();
                string strText = "";
                foreach (System.Reflection.MethodInfo method in methods)
                {
                    strText += method.Name + System.Environment.NewLine;
                }
                //获取所有成员
                System.Reflection.MemberInfo[] members = t.GetMembers();

                //获取所有属性
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                foreach (System.Reflection.PropertyInfo property in properties)
                {
                    object objVal = property.GetValue(objData, null);//旧参数类中,属性中存放的值
                    object[] objDescription = property.GetCustomAttributes(typeof(DescriptionAttribute), true);//获取对应属性注释

                    string strValDescription = ((DescriptionAttribute)objDescription[0]).Description;
                    string strValName = property.Name.ToString();
                    string strVal = objVal.ToString();
                    Type typeValue = property.PropertyType;


                    //dict_obj.Add(strValName, objVal);
                    if (objVal is IList)
                    {
                        for (int i = 0; i < ((IList)objVal).Count; i++)
                        {
                            object objVal_Tmp = ((IList)objVal)[i];
                            strValName = property.Name.ToString() + i.ToString();
                            strVal = objVal_Tmp.ToString();
                            strValDescription = ((DescriptionAttribute)objDescription[0]).Description + i.ToString();
                            Type typeVal = objVal_Tmp.GetType();
                            dict.Add(strValName, strVal);
                        }
                    }
                    else
                    {
                        dict.Add(strValName, strVal);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return dict;
        }
        #endregion
    }
}