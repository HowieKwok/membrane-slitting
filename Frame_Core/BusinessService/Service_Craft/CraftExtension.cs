using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class CraftExtension
    {
        public static string GetProcessFileBasePath(string CraftBasePath, MaterialCrafEnum craftType, string recipeName)
        {
            string typeDir = Path.Combine(CraftBasePath, craftType.ToString(), recipeName);

            return typeDir;
        }

        public static string GetProcessFilePath(string CraftBasePath, MaterialCrafEnum craftType, string recipeName)
        {
            string typeDir = Path.Combine(CraftBasePath, craftType.ToString(), recipeName, "Process");
            //// 自动创建工艺分类文件夹
            //if (!Directory.Exists(typeDir))
            //{
            //    Directory.CreateDirectory(typeDir);
            //}
            return typeDir;
        }

        public static string GetProcessInstanceFilePath(string CraftBasePath, MaterialCrafEnum craftType, string recipeName)
        {
            return Path.Combine(GetProcessFilePath(CraftBasePath, craftType, recipeName), $"Instance.xml");
        }
        public static string GetRecipeFilePath(string CraftBasePath, MaterialCrafEnum craftType, string recipeName, bool isGetSuffix = true)
        {
            string typeDir = Path.Combine(CraftBasePath, craftType.ToString());
            // 自动创建工艺分类文件夹
            if (!Directory.Exists(typeDir))
            {
                Directory.CreateDirectory(typeDir);
            }
            if (isGetSuffix)
            {
                return Path.Combine(typeDir, $"{recipeName}.json");
            }
            else
            {
                return Path.Combine(typeDir, $"{recipeName}");
            }
        }
        public static void SaveCraft(this CraftBase craftBase, string CraftBasePath, MaterialCrafEnum craftType, string recipeName)
        {
            if (craftBase == null) return;
            craftBase.filePath = CraftExtension.GetRecipeFilePath(CraftBasePath, craftType, recipeName, false);
            craftBase.Save();

        }

        public static T ReadCraft<T>(this CraftBase craftBase, string CraftBasePath, MaterialCrafEnum craftType, string recipeName, int length) where T : CraftBase, new()
        {
            if (craftBase == null) craftBase = new CraftBase();
            string recipeFilePath = CraftExtension.GetRecipeFilePath(CraftBasePath, craftType, recipeName, false);
            T craftBase1;
            craftBase.filePath = recipeFilePath;
            craftBase1 = craftBase.Read<T>();
            if (craftBase1 == null)
            {
                craftBase1 = new T();
            }
            craftBase1.CraftType = craftType;
            craftBase1.RecipeName = recipeName;
            craftBase1.filePath = recipeFilePath;
            craftBase1.Init(length);

            return craftBase1;
        }


        public static void SortID(this List<DefectCraft_DieCuttingCut> loadList)
        {
            if (loadList == null) return;

            for (int i = 0; i < loadList.Count; i++) 
            {
                loadList[i].Id = i + 1;
            }
        }

        public static void SortID(this List<SizeCraft_DieCuttingCut> loadList)
        {
            if (loadList == null) return;

            for (int i = 0; i < loadList.Count; i++)
            {
                loadList[i].Id = i + 1;
            }
        }

        public static void SortID(this List<SizeCraftStation_DieCuttingCut> loadList)
        {
            if (loadList == null) return;

            for (int i = 0; i < loadList.Count; i++)
            {
                loadList[i].Id = i + 1;
            }
        }

        /// <summary>
        /// 按指定顺序添加变量到目标集合，优先使用本地读取的缓存数据
        /// </summary>
        /// <param name="targetList">最终存放的全局集合 M_GlobalVariable.l_GlobalVariableItems</param>
        /// <param name="loadList">本地读取上来的原始数据集合</param>
        /// <param name="name">变量唯一名称</param>
        public static void AddVariableByOrder(this List<SizeCraft_DieCuttingCut> targetList, List<SizeCraft_DieCuttingCut> loadList, string name, CalcFormulaEnum calcFormulaEnum, string dataSourceName1, string dataSourceName2, bool isDefault = false)
        {
            if (targetList == null)
                targetList = new List<SizeCraft_DieCuttingCut>();
            if (loadList == null)
                loadList = new List<SizeCraft_DieCuttingCut>();

            // 查找本地读取的同名变量
            SizeCraft_DieCuttingCut loadItem = loadList.FirstOrDefault(p => p.Name == name);
            SizeCraft_DieCuttingCut newItem;

            if (loadItem != null)
            {
                // 本地存在：复用读取到的值
                newItem = loadItem;
                if (isDefault)
                {
                    newItem.Describe = name;
                    newItem.CalcFormula = (int)calcFormulaEnum;             
                    newItem.DataSource1 = dataSourceName1;
                    newItem.DataSource2 = dataSourceName2;
                }
            }
            else
            {
                // 本地不存在：使用传入默认参数
                newItem = new SizeCraft_DieCuttingCut()
                {
                    Name = name,
                    Describe = name,
                    CalcFormula = (int)calcFormulaEnum,
                    DataSource1 = dataSourceName1,
                    DataSource2 = dataSourceName2
                };
            }

            // 按调用顺序追加到目标集合，顺序完全由你调用先后决定
            targetList.Add(newItem);
        }

        /// <summary>
        /// 按指定顺序添加变量到目标集合，优先使用本地读取的缓存数据
        /// </summary>
        /// <param name="targetList">最终存放的全局集合 M_GlobalVariable.l_GlobalVariableItems</param>
        /// <param name="loadList">本地读取上来的原始数据集合</param>
        /// <param name="name">变量唯一名称</param>
        public static void AddVariableByOrder(this List<DefectCraft_DieCuttingCut> targetList, List<DefectCraft_DieCuttingCut> loadList, string name)
        {
            if (targetList == null)
                targetList = new List<DefectCraft_DieCuttingCut>();
            if (loadList == null)
                loadList = new List<DefectCraft_DieCuttingCut>();

            // 查找本地读取的同名变量
            DefectCraft_DieCuttingCut loadItem = loadList.FirstOrDefault(p => p.Name == name);
            DefectCraft_DieCuttingCut newItem;

            if (loadItem != null)
            {
                // 本地存在：复用读取到的值
                newItem = loadItem;
            }
            else
            {
                // 本地不存在：使用传入默认参数
                newItem = new DefectCraft_DieCuttingCut()
                {
                    Name = name,
                    Describe = name,
                };
            }

            // 按调用顺序追加到目标集合，顺序完全由你调用先后决定
            targetList.Add(newItem);
        }

        /// <summary>
        /// 按指定顺序添加变量到目标集合，优先使用本地读取的缓存数据
        /// </summary>
        /// <param name="targetList">最终存放的全局集合 M_GlobalVariable.l_GlobalVariableItems</param>
        /// <param name="loadList">本地读取上来的原始数据集合</param>
        /// <param name="name">变量唯一名称</param>
        public static void AddVariableByOrder(this List<SizeCraftStation_DieCuttingCut> targetList, List<SizeCraftStation_DieCuttingCut> loadList, string name, CalcFormulaEnum calcFormulaEnum, int camIndex1, string dataSourceName1, int camIndex2, string dataSourceName2, bool isDefault = false)
        {
            if (targetList == null)
                targetList = new List<SizeCraftStation_DieCuttingCut>();
            if (loadList == null)
                loadList = new List<SizeCraftStation_DieCuttingCut>();

            // 查找本地读取的同名变量
            SizeCraftStation_DieCuttingCut loadItem = loadList.FirstOrDefault(p => p.Name == name);
            SizeCraftStation_DieCuttingCut newItem;

            if (loadItem != null)
            {
                // 本地存在：复用读取到的值
                newItem = loadItem;
                if (isDefault)
                {
                    newItem.Describe = name;
                    newItem.CalcFormula = (int)calcFormulaEnum;
                    newItem.CameraId1 = camIndex1;
                    newItem.CameraId2 = camIndex2;
                    newItem.DataSource1 = dataSourceName1;
                    newItem.DataSource2 = dataSourceName2;
                }
            }
            else
            {
                // 本地不存在：使用传入默认参数
                newItem = new SizeCraftStation_DieCuttingCut()
                {
                    Name = name,
                    Describe = name,
                    CalcFormula = (int)calcFormulaEnum,
                    CameraId1 = camIndex1,
                    CameraId2 = camIndex2,
                    DataSource1 = dataSourceName1,
                    DataSource2 = dataSourceName2,
                };
            }

            // 按调用顺序追加到目标集合，顺序完全由你调用先后决定
            targetList.Add(newItem);
        }

        /// <summary>
        /// 按指定顺序追加变量到目标集合，优先使用本地读取的缓存数据
        /// </summary>
        /// <param name="targetList">最终存放的全局集合 M_GlobalVariable.l_GlobalVariableItems</param>
        /// <param name="loadList">本地读取上来的原始数据集合</param>
        /// <param name="name">变量唯一名称</param>
        public static void AppVariableByOrder(this  List<DefectCraft_DieCuttingCut> loadList, string name)
        {          
            if (loadList == null)
                loadList = new List<DefectCraft_DieCuttingCut>();

            // 查找本地读取的同名变量
            DefectCraft_DieCuttingCut loadItem = loadList.FirstOrDefault(p => p.Name == name);
            DefectCraft_DieCuttingCut newItem;

            if (loadItem != null)
            {
                // 本地存在：复用读取到的值
                newItem = loadItem;
            }
            else
            {
                // 本地不存在：使用传入默认参数
                newItem = new DefectCraft_DieCuttingCut()
                {
                    Name = name,
                    Describe = name,
                };
                // 按调用顺序追加到目标集合，顺序完全由你调用先后决定
                loadList.Add(newItem);
            }
   
        }

         
        public static void AppVariableByOrder(this List<SizeCraftStation_DieCuttingCut> loadList, string name, CalcFormulaEnum calcFormulaEnum, int camIndex1, string dataSourceName1, int camIndex2, string dataSourceName2, bool isDefault = false)
        {
            if (loadList == null)
                loadList = new List<SizeCraftStation_DieCuttingCut>();

            // 查找本地读取的同名变量
            SizeCraftStation_DieCuttingCut loadItem = loadList.FirstOrDefault(p => p.Name == name);
            SizeCraftStation_DieCuttingCut newItem;


            if (loadItem != null)
            {
                //           
                if (isDefault)
                {
                    loadItem.Describe = name;
                    loadItem.CalcFormula = (int)calcFormulaEnum;
                    loadItem.CameraId1 = camIndex1;
                    loadItem.CameraId2 = camIndex2;
                    loadItem.DataSource1 = dataSourceName1;
                    loadItem.DataSource2 = dataSourceName2;
                }
            }
            else
            {
                // 本地不存在：使用传入默认参数
                newItem = new SizeCraftStation_DieCuttingCut()
                {
                    Name = name,
                    Describe = name,
                    CalcFormula = (int)calcFormulaEnum,
                    CameraId1 = camIndex1,
                    CameraId2 = camIndex2,
                    DataSource1 = dataSourceName1,
                    DataSource2 = dataSourceName2,
                };
                // 按调用顺序追加到目标集合，顺序完全由你调用先后决定
                loadList.Add(newItem);
            }

        }
    }
}
