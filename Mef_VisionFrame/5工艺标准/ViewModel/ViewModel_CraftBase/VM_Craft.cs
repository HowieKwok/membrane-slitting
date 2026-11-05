using Frame_Core;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public class VM_Craft
    {
        public SystemInfo systemInfo = null;
        public CraftBase M_craftBase = new CraftBase();      
        public string CraftBasePath = "";

        public VM_Craft(SystemInfo _systemInfo, CraftBase craftBase,string craftBasePath)
        {
            systemInfo = _systemInfo;
            CraftBasePath = craftBasePath;
            M_craftBase = craftBase;     
        }


        /// <summary>
        /// 读取配方路径
        /// </summary>
        /// <param name="craftType"></param>
        /// <returns></returns>
        public List<string> ReadRecipePath(MaterialCrafEnum craftType)
        {
            try
            {
                string typeDir = Path.Combine(CraftBasePath, craftType.ToString());
                if (!Directory.Exists(typeDir))
                {
                    return new List<string>();
                }
                // 获取所有json文件，只提取文件名，去掉后缀
                var filePaths = Directory.GetFiles(typeDir, "*.json");
                return filePaths.ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 读取配方名称
        /// </summary>
        /// <param name="craftType"></param>
        /// <returns></returns>
        public List<string> ReadRecipe(MaterialCrafEnum craftType)
        {
            try
            {
                var filePaths = ReadRecipePath(craftType);
                return filePaths.Select(p => Path.GetFileNameWithoutExtension(p)).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 获取配方信息
        /// </summary>
        /// <param name="craftType"></param>
        /// <param name="currentRecipeName"></param>
        /// <returns></returns>
        public List<CraftRow> GetRecipeInfo(MaterialCrafEnum craftType, string currentRecipeName,string searchName)
        {
            List<string> strings = ReadRecipePath(craftType);
            List<CraftRow> craftRows = new List<CraftRow>();
            foreach (var path in strings)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                if (!string.IsNullOrWhiteSpace(searchName) && name.IndexOf(searchName, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                craftRows.Add(new CraftRow
                {
                    CraftName = name,
                    IsCurrent = string.Equals(name, currentRecipeName, StringComparison.OrdinalIgnoreCase),
                    LastWriteTime = File.GetLastWriteTime(path)
                });
            }

            List<CraftRow> data = craftRows
               .OrderByDescending(x => x.IsCurrent)
               .ThenBy(x => x.CraftName)
               .ToList();

            return data;
        }

        /// <summary>
        /// 配方名称是否存在
        /// </summary>
        /// <param name="craftType"></param>
        /// <param name="recipeName"></param>
        /// <returns></returns>
        public bool NameExists(MaterialCrafEnum craftType, string recipeName, bool noMbShow = false)
        {
            // 1. 校验空白名称
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                if (!noMbShow) MessageBox.Show("失败：配方名称不能为空");
                return false;
            }

            List<string> recipeNameList;
            try
            {
                recipeNameList = ReadRecipe(craftType);
            }
            catch (Exception ex)
            {
                if (!noMbShow) MessageBox.Show($"读取配方列表异常：{ex.Message}");
                return false;
            }

            // 2. 无配方数据直接放行
            if (recipeNameList == null || recipeNameList.Count == 0)
            {
                return true;
            }

            // 3. 匹配重复名称（.NET4.7.2 标准委托写法）
            bool isDuplicate = recipeNameList.Exists(name => name == recipeName);
            if (isDuplicate)
            {
                if (!noMbShow) MessageBox.Show("失败：配方名称已存在");
                return false;
            }

            return true;
        }


        /// <summary>
        /// 新建配方
        /// </summary>
        /// <param name="craftType"></param>
        /// <param name="recipeName"></param>
        public void CreateRecipe(MaterialCrafEnum craftType, string recipeName)
        {
            // NameExists返回true代表名称可用，才执行创建
            if (!NameExists(craftType, recipeName))
                return;

            switch (craftType)
            {
                case MaterialCrafEnum.激光模切一体机:
                    CraftParam_DieCuttingCut craftBaseTemp = new CraftParam_DieCuttingCut
                    {
                        CraftType = craftType,
                        RecipeName = recipeName
                    };
                    craftBaseTemp.Init();
                    // 自动保存生成json文件（假设CraftBase内部实现Save）                  
                    craftBaseTemp.SaveCraft(CraftBasePath, craftType, recipeName);
                    break;
                default:
                    CraftParam_DieCuttingCut craftBaseTemp1 = new CraftParam_DieCuttingCut
                    {
                        CraftType = craftType,
                        RecipeName = recipeName
                    };
                    craftBaseTemp1.Init();
                    // 自动保存生成json文件（假设CraftBase内部实现Save）
                    craftBaseTemp1.SaveCraft(CraftBasePath, craftType, recipeName);
                    break;
            }

        }

        /// <summary>
        /// 复制配方
        /// </summary>
        public bool CopyRecipe(MaterialCrafEnum craftType, string recipeName,out string newName)
        {
            newName = "";
            string sourcePath = CraftExtension.GetRecipeFilePath(CraftBasePath, craftType, recipeName);
            // 源文件不存在直接返回
            if (!File.Exists(sourcePath))
            {
                MessageBox.Show($"复制失败：配方【{recipeName}】不存在");
                return false;
            }

            string tempName = $"{recipeName}-复制";
            newName = tempName;
            int index = 2;
            // 校验新名称是否重复
            while (!NameExists(craftType, newName, true))
            {
                newName = tempName + $"{index++}";
            }

            string targetPath = CraftExtension.GetRecipeFilePath(CraftBasePath, craftType, newName);
            try
            {
                File.Copy(sourcePath, targetPath, false);           
                MessageBox.Show($"配方复制成功，新名称：{newName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"复制配方失败：{ex.Message}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 重命名配方（完整实现）
        /// </summary>
        /// <param name="craftType">工艺类型</param>
        /// <param name="oldRecipeName">原配方名</param>
        /// <param name="newRecipeName">新配方名</param>
        public void RenameRecipe(MaterialCrafEnum craftType, string oldRecipeName, string newRecipeName)
        {
            #region 参数前置校验
            if (string.IsNullOrWhiteSpace(oldRecipeName) || string.IsNullOrWhiteSpace(newRecipeName))
            {
                MessageBox.Show("失败：配方名称不能为空");
                return;
            }
            // 新旧名称相同无需操作
            if (oldRecipeName == newRecipeName)
            {
                //MessageBox.Show("新旧名称一致，无需修改");
                return;
            }
            #endregion

            string oldFilePath = CraftExtension.GetRecipeFilePath(CraftBasePath, craftType, oldRecipeName);
            // 原文件不存在
            if (!File.Exists(oldFilePath))
            {
                MessageBox.Show($"重命名失败：配方【{oldRecipeName}】不存在");
                return;
            }

            // 校验新名称是否重复
            if (!NameExists(craftType, newRecipeName))
                return;

            string newFilePath = CraftExtension.GetRecipeFilePath(CraftBasePath, craftType, newRecipeName);
            try
            {
                // 文件重命名
                File.Move(oldFilePath, newFilePath);
                MessageBox.Show($"配方重命名成功：{oldRecipeName} → {newRecipeName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重命名失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 删除配方（完整实现，带确认弹窗）
        /// </summary>
        public void DeleteRecipe(MaterialCrafEnum craftType, string recipeName)
        {
            if (string.IsNullOrWhiteSpace(recipeName))
            {
                MessageBox.Show("删除失败：配方名称不能为空");
                return;
            }

            string filePath = CraftExtension.GetRecipeFilePath(CraftBasePath, craftType, recipeName);
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"删除失败：配方【{recipeName}】不存在");
                return;
            }

            // 二次确认防止误删
            DialogResult confirm = MessageBox.Show(
                $"确认要永久删除配方【{recipeName}】吗？删除后无法恢复！",
                "删除确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                File.Delete(filePath);
            
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除配方失败：{ex.Message}");
            }
        }


        /// <summary>
        /// 打开配方文件夹位置
        /// </summary>
        /// <param name="craftType"></param>
        public void OpenRecipeFolder(MaterialCrafEnum craftType)
        {
            string filePath = Path.Combine(CraftBasePath, craftType.ToString());
            Directory.CreateDirectory(filePath);
            System.Diagnostics.Process.Start(filePath);
        }


    }
}
