using LogTool;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{

    // 4. 扩展：给 IEnumerable<T> 加 ForEach
    public static class Ext
    {
        public static void ForEach<T>(this IEnumerable<T> src, Action<T> act)
        {
            foreach (var t in src) act(t);
        }
    }
    public class Other
    {
        public static Type[] allTypes = new Type[] {typeof(bool), typeof(bool[]), typeof(short), typeof(short[]), typeof(int), typeof(int[]), typeof(long), typeof(long[]),
                                                                         typeof(ushort), typeof(ushort[]), typeof(uint), typeof(uint[]), typeof(ulong), typeof(ulong[]),
                                                                         typeof(float), typeof(float[]), typeof(double), typeof(double[]), typeof(byte), typeof(byte[]),
                                                                         typeof(char), typeof(char[]), typeof(string), typeof(string[]),
                                                                         /*typeof(decimal), typeof(decimal[]), typeof(sbyte), typeof(sbyte[]),  */ };
        public static List<string[]> GetFieldsAndProperties(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            List<string[]> ls = new List<string[]>();
            Type type = obj.GetType();
            int index = 0;
            try
            {

                // 获取所有字段
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (FieldInfo field in fields)
                {
                    index++;
                    if (index == 9)
                    {
                    }
                    if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(CommunicationData<>))
                    {
                        Type genericArgumentType = field.FieldType.GetGenericArguments()[0];
                        if (Array.Exists(allTypes, t => t == genericArgumentType))
                        {
                            // 对于实例字段，我们需要一个对象实例来获取值
                            // 对于静态字段，我们可以传递 null 或使用 field.GetValue(null)
                            object fieldValue = field.IsStatic ? field.GetValue(null) : field.GetValue(obj);
                            // 这里我们不能直接转换为 CommunicationData<object>，因为这样会丢失泛型类型的类型安全
                            // 我们需要以一种类型安全的方式处理这个值，或者接受类型转换带来的风险
                            // 例如，我们可以将其转换为动态类型，但这样会失去编译时类型检查的好处
                            dynamic communicationDataValue = fieldValue;
                            // 或者，我们可以定义一个非泛型的接口或基类来处理这个值
                            // 假设我们有一个 ICommunicationData 接口
                            // ICommunicationData communicationDataValue = (ICommunicationData)fieldValue;

                            // 这里我们假设使用了动态类型，并继续处理
                            if (communicationDataValue.Value == null)
                            {
                                ls.Add(new string[] { communicationDataValue.Name, communicationDataValue.Address, "", genericArgumentType.Name, communicationDataValue.Value?.ToString() });
                            }
                            else
                            {
                                ls.Add(new string[] { communicationDataValue.Name, communicationDataValue.Address, "", communicationDataValue.Value.GetType().Name, communicationDataValue.Value?.ToString() });
                            }

                        }
                    }
                }

                // 获取所有属性
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(CommunicationData<>))
                    {
                        Type genericArgumentType = property.PropertyType.GetGenericArguments()[0];
                        if (Array.Exists(allTypes, t => t == genericArgumentType))
                        {
                            // 对于实例字段，我们需要一个对象实例来获取值
                            // 对于静态字段，我们可以传递 null 或使用 field.GetValue(null)
                            object fieldValue = property.GetValue(obj);
                            // 这里我们不能直接转换为 CommunicationData<object>，因为这样会丢失泛型类型的类型安全
                            // 我们需要以一种类型安全的方式处理这个值，或者接受类型转换带来的风险
                            // 例如，我们可以将其转换为动态类型，但这样会失去编译时类型检查的好处
                            dynamic communicationDataValue = fieldValue;
                            // 或者，我们可以定义一个非泛型的接口或基类来处理这个值
                            // 假设我们有一个 ICommunicationData 接口
                            // ICommunicationData communicationDataValue = (ICommunicationData)fieldValue;

                            // 这里我们假设使用了动态类型，并继续处理
                            if (communicationDataValue.Value is string)
                            {
                                ls.Add(new string[] { communicationDataValue.Name, communicationDataValue.Address, "", "string", communicationDataValue.Value.ToString() });
                            }
                            else
                            {
                                ls.Add(new string[] { communicationDataValue.Name, communicationDataValue.Address, "", communicationDataValue.Value.GetType().Name, communicationDataValue.Value.ToString() });
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }


            return ls;
        }
        public static void WriteTag(string filePath, object obj)
        {
            try
            {
                List<string[]> ls = GetFieldsAndProperties(obj);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                FileOperationCSV.WriteData_CSV(filePath, new string[] { "名称", "地址", "PLC数据类型", "PC数据类型", "值" }, ls, "");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }


        /// <summary>
        /// 计算两条线的距离
        /// </summary>
        /// <param name="line1_X1"></param>
        /// <param name="line1_Y1"></param>
        /// <param name="line1_X2"></param>
        /// <param name="line1_Y2"></param>
        /// <param name="line2_X1"></param>
        /// <param name="line2_Y1"></param>
        /// <param name="line2_X2"></param>
        /// <param name="line2_Y2"></param>
        /// <param name="isLine2toLine1"></param>
        /// <param name="calibrationValue_X"></param>
        /// <param name="calibrationValue_Y"></param>
        /// <param name="realFixRito">系数</param>
        /// <param name="distancePix_X"></param>
        /// <param name="distanceMM_X"></param>
        /// <param name="distancePix_Y"></param>
        /// <param name="distanceMM_Y"></param>
        public static void LineToLineDistance(double line1_X1, double line1_Y1, double line1_X2, double line1_Y2,
                                              double line2_X1, double line2_Y1, double line2_X2, double line2_Y2,
                                              bool isLine2toLine1, double calibrationValue_X, double calibrationValue_Y, double realFixRito,
                                              out double distancePix_X, out double distanceMM_X, out double distancePix_Y, out double distanceMM_Y)
        {
            //计算两条线的X距离
            if (isLine2toLine1)
            {
                distancePix_X = ((line2_X1 + line2_X2) / 2) - ((line1_X1 + line1_X2) / 2);
                distanceMM_X = distancePix_X * calibrationValue_X * realFixRito;
            }
            else
            {
                distancePix_X = ((line1_X1 + line1_X2) / 2) - ((line2_X1 + line2_X2) / 2);
                distanceMM_X = distancePix_X * calibrationValue_X * realFixRito;
            }


            //计算两条线的Y距离
            if (isLine2toLine1)
            {
                distancePix_Y = ((line2_Y1 + line2_Y2) / 2) - ((line1_Y1 + line1_Y2) / 2);
                distanceMM_Y = distancePix_Y * calibrationValue_Y * realFixRito;
            }
            else
            {
                distancePix_Y = ((line1_Y1 + line1_Y2) / 2) - ((line2_Y1 + line2_Y2) / 2);
                distanceMM_Y = distancePix_Y * calibrationValue_Y * realFixRito;
            }

        }

      
       
    }


}
