using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using LogTool;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;

namespace Plugin.CACRunCalibration
{
    [Category("标定工具")]
    [DisplayName("CAC运行标定")]
    [Export("CACRunCalibrationObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class CACRunCalibrationObj : IPlugin
    {
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public PluginParamsBase PluginParams { get; set; } = new ParamsFindModel();

        public bool ExecuteObj()
        {
            ParamsFindModel p = PluginParams as ParamsFindModel;
            if (p == null) return false;

            try
            {
                p.m_strGoalName = "CACRunCalibration";
                p.m_bOverTime = false;

                if (p.ho_Image != null && p.ho_Image.IsInitialized())
                {
                    p.m_ho_OutImgShow?.Dispose();
                    p.m_ho_OutImgShow = p.ho_Image.Clone();

                    HTuple width;
                    HTuple height;
                    HOperatorSet.GetImageSize(p.ho_Image, out width, out height);
                    p.m_fGoalX = width.D / 2.0;
                    p.m_fGoalY = height.D / 2.0;
                }

                p.m_fScoreMatch = 1.0;
                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("CAC运行标定执行失败：" + ex);
                return false;
            }
        }

        public void IniObj()
        {
            ParamsFindModel p = PluginParams as ParamsFindModel;
            if (p == null) return;

            string filePath = Project.Instance.paramPathSet.ProcessPath;
            filePath = filePath.Replace("Process", "");
            filePath = Path.Combine(filePath, moduleInfo.ProcessName ?? string.Empty, moduleInfo.ModuleName ?? string.Empty);
            p.m_strModelGroupPath = filePath;
            p.filePath = moduleInfo.filePath;
        }

        public bool saveParams()
        {
            if (PluginParams == null) return false;
            PluginParams.filePath = moduleInfo.filePath;
            return PluginParams.Save();
        }

        public void SetParams(PluginParamsBase param)
        {
            if (param != null)
            {
                PluginParams = param;
            }
        }

        public void GetResultParams(string resultParamsName, object obj)
        {
            // 按现有框架接口保留，具体结果通过 PluginParams 读取。
        }

        public void DisPoseObj()
        {
            ParamsFindModel p = PluginParams as ParamsFindModel;
            if (p == null) return;

            p.m_ho_OutImgShow?.Dispose();
            p.m_ho_OutCropRegion?.Dispose();
            p.m_ho_OutRegion?.Dispose();
            p.m_ho_OutCircleRegion?.Dispose();
        }
    }
}


