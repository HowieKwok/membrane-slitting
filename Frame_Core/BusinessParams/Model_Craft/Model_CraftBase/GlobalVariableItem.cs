using NPOI.OpenXmlFormats.Dml.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class GlobalVariable
    {
        public List<GlobalVariableItem> l_GlobalVariableItems = new List<GlobalVariableItem>();
    }
    public static class GlobalVariableCheckHelper
    {
        #region 枚举 ↔ CLR真实类型映射
        /// <summary>枚举转对应CLR真实Type</summary>
        public static Type GetClrType(DataTypeEnum dataType)
        {
            switch (dataType)
            {
                case DataTypeEnum.Short:
                    return typeof(short);
                case DataTypeEnum.Int:
                    return typeof(int);
                case DataTypeEnum.Long:
                    return typeof(long);
                case DataTypeEnum.Float:
                    return typeof(float);
                case DataTypeEnum.Double:
                    return typeof(double);
                case DataTypeEnum.Bool:
                    return typeof(bool);
                case DataTypeEnum.String:
                default:
                    return typeof(string);
            }
        }

        /// <summary>真实Type反向转DataTypeEnum</summary>
        public static DataTypeEnum GetDataTypeEnum(Type type)
        {
            if (type == typeof(short))
                return DataTypeEnum.Short;
            if (type == typeof(int))
                return DataTypeEnum.Int;
            if (type == typeof(long))
                return DataTypeEnum.Long;
            if (type == typeof(float))
                return DataTypeEnum.Float;
            if (type == typeof(double))
                return DataTypeEnum.Double;
            if (type == typeof(bool))
                return DataTypeEnum.Bool;
            return DataTypeEnum.String;
        }
        #endregion

    }
    public static class GlobalVariableExtension
    {
        /// <summary>
        /// 按指定顺序添加变量到目标集合，优先使用本地读取的缓存数据
        /// </summary>
        /// <param name="targetList">最终存放的全局集合 M_GlobalVariable.l_GlobalVariableItems</param>
        /// <param name="loadList">本地读取上来的原始数据集合</param>
        /// <param name="name">变量唯一名称</param>
        /// <param name="defaultDesc">不存在时使用的默认描述</param>
        /// <param name="defaultType">不存在时使用的默认类型</param>
        /// <param name="defaultValue">不存在时使用的默认值</param>
        public static void AddVariableByOrder(
          this List<GlobalVariableItem> targetList,
            List<GlobalVariableItem> loadList,
            string name,
            string defaultDesc,
            DataTypeEnum defaultType,
            string defaultValue)
        {
            if (targetList == null)
                targetList = new List<GlobalVariableItem>();
            if (loadList == null)
                loadList = new List<GlobalVariableItem>();

            // 查找本地读取的同名变量
            GlobalVariableItem loadItem = loadList.FirstOrDefault(p => p.Name == name);
            GlobalVariableItem newItem;

            if (loadItem != null)
            {
                // 本地存在：复用读取到的值
                newItem = loadItem;
             
            }
            else
            {
                // 本地不存在：使用传入默认参数
                newItem = new GlobalVariableItem()
                {
                    Name = name,
                    Describe = defaultDesc,
                    DataType = defaultType,
                    Value = defaultValue
                };
            }

            // 按调用顺序追加到目标集合，顺序完全由你调用先后决定
            targetList.Add(newItem);
        }
    }

    /// <summary>基础变量数据类型枚举||||顺序不要随意修改</summary>
    public enum DataTypeEnum
    {
        Short = 0,
        Int = 1,
        Long = 2,
        Float = 3,
        Double = 4,
        Bool = 5,
        String = 6,
    }

    public class GlobalVariableItem
    {
        private int _ID = 0;
        public int ID
        {
            get => _ID;
            set { _ID = value; }
        }

        private string _Name = "";
        public string Name
        {
            get => _Name;
            set { _Name = value; }
        }

        private string _Describe = "";
        public string Describe
        {
            get => _Describe;
            set { _Describe = value; }
        }

        private DataTypeEnum _DataType;
        public DataTypeEnum DataType
        {
            get => _DataType;
            set { _DataType = value; }
        }

        private string _Value = null;
        public string Value
        {
            get => _Value;
            set { _Value = value; }
        }
    }
}
