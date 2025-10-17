using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class DirectoryHelper
    {
        public delegate bool ActionDirectories(string path);

        /// <summary>
        /// 解析文件路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="driveLetter">盘符</param>
        /// <param name="directoryLevels">目录层级名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileExtension">后缀名</param>
        public static ExecuteResult ParseFilePath(string filePath, out string driveLetter, out List<string> directoryLevels, out string fileName, out string fileExtension)
        {
            ExecuteResult executeResult = new ExecuteResult();
            driveLetter = string.Empty;
            directoryLevels = new List<string>();
            fileName = string.Empty;
            fileExtension = string.Empty;

            if (string.IsNullOrEmpty(filePath))
            {
                executeResult.Message = "文件路径不能为空";
                executeResult.IsSuccess = false;
                return executeResult;
            }


            try
            {
                // 获取盘符
                if (filePath.Length >= 2 && filePath[1] == ':')
                {
                    driveLetter = filePath.Substring(0, 2); // 例如 "E:"
                }
                else if (filePath.StartsWith(@"\\"))
                {
                    // 处理网络路径，如 "\\server\share"
                    int thirdBackslash = filePath.IndexOf('\\', 2);
                    if (thirdBackslash > 2)
                    {
                        driveLetter = filePath.Substring(0, thirdBackslash);
                    }
                    else
                    {
                        driveLetter = filePath;
                    }
                }

                // 获取文件名和扩展名
                fileName = Path.GetFileNameWithoutExtension(filePath);
                fileExtension = Path.GetExtension(filePath);

                // 获取完整目录路径（去掉文件名）
                string directoryPath = Path.GetDirectoryName(filePath);

                if (string.IsNullOrEmpty(directoryPath))
                {
                    executeResult.Message = "完整目录路径不能为空";
                    executeResult.IsSuccess = false;
                    return executeResult;
                }


                // 分割路径为各级目录
                string[] pathParts = directoryPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                // 添加每一级目录名称（跳过空字符串和根目录标识）
                foreach (string part in pathParts)
                {
                    if (!string.IsNullOrEmpty(part) &&
                        !part.EndsWith(":") &&
                        !part.StartsWith(@"\\") &&
                        part != driveLetter)
                    {
                        directoryLevels.Add(part);
                    }
                }
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.Message = ex.Message;
                executeResult.IsSuccess = false;
                Log4netRecord.ErrorLog.Debug(ex);
                // 发生异常时，所有out参数保持初始值
            }
            return executeResult;
        }

        /// <summary>
        /// 获取指定文件夹下一级子文件夹中的所有图像文件完整路径
        /// </summary>
        /// <param name="folderPath">目标文件夹路径</param>     
        /// <param name="imagePaths">输出的图像文件完整路径列表</param>
        public static ExecuteResult GetImagePathsFromSubfolders(string folderPath, out List<string> imagePaths)
        {
            ExecuteResult executeResult = new ExecuteResult();
            imagePaths = new List<string>();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                executeResult.Message = "文件夹路径不存在或为空";
                executeResult.IsSuccess = false;
                return executeResult;
            }

            try
            {
                // 支持的图像文件扩展名
                string[] imageExtensions = { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".tif", ".webp", ".ico" };

                // 获取所有下一级子文件夹
                string[] subFolders = Directory.GetDirectories(folderPath);

                foreach (string subFolder in subFolders)
                {
                    try
                    {
                        // 在每个子文件夹中搜索图像文件
                        var imagesInSubFolder = Directory.EnumerateFiles(subFolder, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()));

                        imagePaths.AddRange(imagesInSubFolder);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        executeResult.Message += $"无权限访问文件夹: {subFolder}-|-";
                    }
                    catch (Exception ex)
                    {
                        executeResult.Message += $"访问文件夹时出错 {subFolder}: {ex.Message}-|-";
                    }
                }
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.Message += $"获取图像路径时出错: {ex.Message}";
                executeResult.IsSuccess = false;
            }
            return executeResult;
        }

        /// <summary>
        /// 获取指定文件夹下一级子文件夹中的所有图像文件完整路径
        /// </summary>
        /// <param name="folderPath">目标文件夹路径</param>
        /// <param name="action">委托方法</param>
        /// <param name="imagePaths">输出的图像文件完整路径列表</param>
        public static ExecuteResult GetImagePathsFromSubfolders(string folderPath, ActionDirectories action, out List<string> imagePaths)
        {
            ExecuteResult executeResult = new ExecuteResult();
            imagePaths = new List<string>();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                executeResult.Message = "文件夹路径不存在或为空";
                executeResult.IsSuccess = false;
                return executeResult;
            }

            bool execute = true;
            try
            {
                // 支持的图像文件扩展名
                string[] imageExtensions = { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".tif", ".webp", ".ico" };

                // 获取所有下一级子文件夹
                string[] subFolders = Directory.GetDirectories(folderPath);

                foreach (string subFolder in subFolders)
                {
                    try
                    {
                        // 在每个子文件夹中搜索图像文件
                        var imagesInSubFolder = Directory.EnumerateFiles(subFolder, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()));

                        if (imagesInSubFolder.Count() <= 0 && execute)
                        {
                            execute = action?.Invoke(subFolder) ?? false;
                        }

                        imagePaths.AddRange(imagesInSubFolder);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        executeResult.Message += $"无权限访问文件夹: {subFolder}-|-";
                    }
                    catch (Exception ex)
                    {
                        executeResult.Message += $"访问文件夹时出错 {subFolder}: {ex.Message}-|-";
                    }
                }

                if (execute)
                {
                    executeResult.IsSuccess = true;
                }
                else
                {
                    executeResult.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                executeResult.Message += $"获取图像路径时出错: {ex.Message}";
                executeResult.IsSuccess = false;
            }
            return executeResult;
        }

        /// <summary>
        /// 获取指定文件夹路径下的图像完整路径
        /// </summary>
        /// <param name="folderPath">目标文件夹路径</param>
        /// <param name="imagePaths">输出的图像文件完整路径列表</param>
        /// <param name="searchSubfolders">是否搜索子文件夹</param>
        /// <param name="maxDepth">最大搜索深度（0表示只搜索当前文件夹）</param>
        /// <param name="extensions">自定义图像扩展名列表</param>
        public static ExecuteResult GetImagePaths(string folderPath, out List<string> imagePaths, bool searchSubfolders = true, int maxDepth = 1, string[] extensions = null)
        {
            ExecuteResult executeResult = new ExecuteResult();
            imagePaths = new List<string>();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                executeResult.Message = "文件夹路径不存在或为空";
                executeResult.IsSuccess = false;
                return executeResult;
            }


            // 默认图像扩展名
            string[] imageExtensions = extensions ?? new[] { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".tif", ".webp", ".ico" };

            try
            {
                if (searchSubfolders && maxDepth > 0)
                {
                    // 搜索下一级子文件夹
                    string[] subFolders = Directory.GetDirectories(folderPath);
                    foreach (string subFolder in subFolders)
                    {
                        try
                        {
                            // 递归搜索子文件夹（深度减1）
                            List<string> subFolderImages;
                            GetImagePaths(subFolder, out subFolderImages, true, maxDepth - 1, imageExtensions);
                            imagePaths.AddRange(subFolderImages);
                        }
                        catch (Exception ex)
                        {
                            executeResult.Message += $"访问子文件夹时出错 {subFolder}: {ex.Message}-|-";
                        }
                    }
                }

                // 搜索当前文件夹中的图像文件
                var currentFolderImages = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()));

                imagePaths.AddRange(currentFolderImages);
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.Message += $"获取图像路径时出错: {ex.Message}";
                executeResult.IsSuccess = false;
            }
            return executeResult;
        }
    }
}
