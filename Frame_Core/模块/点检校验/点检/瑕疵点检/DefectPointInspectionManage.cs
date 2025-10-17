using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class DefectPointInspectionManage
    {
        /// <summary>
        /// 点检参数(菲林片)
        /// </summary>
        public DefectPointInspectionParam defectPointInspectionParam;


        /// <summary>
        /// 保存点检数据
        /// </summary>
        public FileOperationCSV SaveDefectCheckPointCSV = new FileOperationCSV();//保存点检数据


        /// <summary>
        /// 初始化相机点检参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitParam(string path)
        {

            try
            {
                if (defectPointInspectionParam == null) defectPointInspectionParam = new DefectPointInspectionParam();
                defectPointInspectionParam.filePath = path;
                defectPointInspectionParam = (DefectPointInspectionParam)defectPointInspectionParam.Read();
                defectPointInspectionParam.filePath = path;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// 创建缺陷类型文件夹
        ///// </summary>
        ///// <param name="basePath"></param>
        ///// <param name="cameraEnum"></param>
        ///// <param name="craftInfo"></param>
        //public static void CreationDefectFolder(string basePath,CameraEnum cameraEnum, CraftInfo craftInfo)
        //{
        //    try
        //    {
        //        foreach (var item in craftInfo.dicCamCraft[cameraEnum].DefectInfoList)
        //        {
        //            if (item.Enable)
        //            {
        //                Defct1Enum defct1Enum = (Defct1Enum)Enum.Parse(typeof(Defct1Enum), item.Name);
        //                string m_strNgTypeName = LanguageProject.GetLangeuageAttribute(defct1Enum, LanguageEnum.Chinese);
        //                m_strNgTypeName += $"_{LanguageProject.GetLangeuageAttribute(defct1Enum, LanguageEnum.English)}";

        //                string path = basePath + $"\\{m_strNgTypeName}";
        //                if (!(Directory.Exists(path)))
        //                {
        //                    Directory.CreateDirectory(path);
        //                }
        //            }
                  
        //        }                           
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //    }
        //}
    }
}
