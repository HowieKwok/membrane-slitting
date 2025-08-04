using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

//扩展方法类
namespace Extensions
{

    /// <summary>
    /// 使用扩展,好用的方法 XuChang 2023.7.7
    /// </summary>
    public static class ExtensionsObject
    {
        /// <summary>
        /// 深度拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myClass"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T myClass)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, myClass);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// 初始化 HObject
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        public static HObject Init(this HObject hObject)
        {
            HOperatorSet.GenEmptyObj(out hObject);
            return hObject;
        }

        /// <summary>
        /// 通过索引获取枚举的值
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public static T GetEnumValueAtIndex<T>(int index) where T : Enum
        {
            T[] enumValues = (T[])Enum.GetValues(typeof(T));

            if (index < 0 || index >= enumValues.Length)
            {
                return enumValues[0];
                    
            }

            return enumValues[index];
        }


        public static string GetDescription<T>(this T value) where T : Enum
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var field = type.GetField(name);
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute != null ? attribute.Description : name;
        }

    }



    public static class ControlClone
    {
        public static T Clone<T>(this T controlToClone)
            where T : Control
        {
            PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            T instance = Activator.CreateInstance<T>();

            foreach (PropertyInfo propInfo in controlProperties)
            {
                if (propInfo.CanWrite)
                {
                    if (propInfo.Name != "WindowTarget" && propInfo.Name != "Name")
                        propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
                }
            }

            return instance;
        }
    }





    /// <summary>
    /// 取点方式
    /// </summary>
    public enum Position
    {
        /// <summary>
        ///第一点
        /// </summary>
        first,
        /// <summary>
        ///最后一个
        /// </summary>
        last,
        /// <summary>
        //所有的边缘
        /// </summary>
        all,
    }

    /// <summary>
    /// 寻边方式
    /// </summary>
    public enum Transition
    {
        /// <summary>
        /// 从暗到亮
        /// </summary>
        positive,
        /// <summary>
        /// 从亮到暗
        /// </summary>
        negative,
        /// <summary>
        /// 所有的
        /// </summary>
        all

    }

    /// <summary>
    /// 搜索方向
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// 从左到右
        /// </summary>
        L2R_New,
        /// <summary>
        /// 从右到左
        /// </summary>
        R2L_New,
        /// <summary>
        /// 从上到下
        /// </summary>
        U2B_New,
        /// <summary>
        /// 从下到上
        /// </summary>
        B2U_New

    }

    /// <summary>
    /// 逻辑运算符
    /// </summary>
    public enum LogicalOperation
    {
        或,
        且
    }











    public static class DictionaryExtensions
    {
        /// <summary>
        /// 字典中如果键已存在则修改值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }


    }
