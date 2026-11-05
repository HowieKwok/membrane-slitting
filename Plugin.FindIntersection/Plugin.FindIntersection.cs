using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using LogTool;
using Plugin.FindIntersection.Core;

namespace Plugin.FindIntersection
{
    [Category("尺寸测量")]
    [DisplayName("两线交点工具")]
    [Export("FindIntersectionObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class FindIntersectionObj : IPlugin
    {
        private readonly FindEngine engine = new FindEngine();
        private ParamsFindIntersection findParams;

        public PluginParamsBase PluginParams { get; set; } = new ParamsFindIntersection();
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public bool isExistModel = false;

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public bool ExecuteObj()
        {
            findParams = PluginParams as ParamsFindIntersection;
            if (findParams == null)
            {
                Log4netRecord.ErrorLog.Debug("两线交点工具：参数类型错误");
                return false;
            }

            HOperatorSet.CountSeconds(out HTuple start);
            bool success = false;
            try
            {
                if (findParams.ImageSourceIndex == 1)
                {
                    SetInPutParams(findParams.ImageSourceModule, findParams.ImageSourceName);
                }

                success = engine.Run(findParams);
                if (success)
                {
                    //PublishResultParams();
                }

                return success;
            }
            catch (Exception ex)
            {
                findParams.bl_IntersectionSuccess = false;
                findParams.m_strRunMessage = ex.Message;
                Log4netRecord.ErrorLog.Debug("两线交点工具执行异常：" + ex);
                return false;
            }
            finally
            {
                HOperatorSet.CountSeconds(out HTuple end);
                findParams.m_fElapsedMs = (end - start).D * 1000.0;
                findParams.m_bRunSuccess = success;
                if (string.IsNullOrWhiteSpace(findParams.m_strRunMessage))
                {
                    findParams.m_strRunMessage = success ? "运行成功" : "交点计算失败";
                }

                ExecuteCompleted?.Invoke(
                  this,
                  new PluginExecuteCompletedEventArgs(
                      findParams,
                      findParams.m_bRunSuccess,
                     findParams.m_strRunMessage,
                      (long)findParams.m_fElapsedMs));

            
        }
        }

        private void PublishResultParams()
        {
            try
            {
                GetResultParams("IntersectionRow", findParams.hv_IntersectionRow);
                GetResultParams("IntersectionColumn", findParams.hv_IntersectionColumn);
                GetResultParams("IntersectionAngle", findParams.hv_IntersectionAngle);
                GetResultParams("IsParallel", findParams.hv_IsParallel);
                GetResultParams("Line1RowBegin", findParams.hv_Line1_RowBegin);
                GetResultParams("Line1ColumnBegin", findParams.hv_Line1_ColumnBegin);
                GetResultParams("Line1RowEnd", findParams.hv_Line1_RowEnd);
                GetResultParams("Line1ColumnEnd", findParams.hv_Line1_ColumnEnd);
                GetResultParams("Line2RowBegin", findParams.hv_Line2_RowBegin);
                GetResultParams("Line2ColumnBegin", findParams.hv_Line2_ColumnBegin);
                GetResultParams("Line2RowEnd", findParams.hv_Line2_RowEnd);
                GetResultParams("Line2ColumnEnd", findParams.hv_Line2_ColumnEnd);
                GetResultParams("FollowOffsetX", findParams.m_fFollowOffsetX);
                GetResultParams("FollowOffsetY", findParams.m_fFollowOffsetY);
                GetResultParams("FollowOffsetR", findParams.m_fFollowOffsetR);
                GetResultParams("偏差X", findParams.m_fFollowOffsetX);
                GetResultParams("偏差Y", findParams.m_fFollowOffsetY);
                GetResultParams("偏差R", findParams.m_fFollowOffsetR);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("两线交点工具发布结果异常：" + ex);
            }
        }

        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath;
            return PluginParams.Save();
        }

        public void SetParams(PluginParamsBase param)
        {
            PluginParams = (ParamsFindIntersection)param;
            findParams = PluginParams as ParamsFindIntersection;
        }

        public void GetResultParams(string varName, object obj)
        {
            Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
            if (proc == null)
            {
                Log4netRecord.ErrorLog.Debug("两线交点工具：发布输出失败，未找到指定流程");
                return;
            }

            proc.AddOutPutParam(moduleInfo.ModuleName, varName, obj);
        }

        public void SetInPutParams(string module, string valueName)
        {
            try
            {
                Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
                if (proc == null)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点工具：未找到指定流程");
                    return;
                }

                object imgObj = proc.GetOutPutParamByName(module, valueName);
                HObject image = imgObj as HObject;
                if (image != null)
                {
                    findParams.ho_Image = image;
                    return;
                }

                Log4netRecord.ErrorLog.Debug("两线交点工具：未获取到有效图像源 " + module + "/" + valueName);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("两线交点工具设置输入异常：" + ex);
            }
        }

        public void IniObj()
        {
            findParams = PluginParams as ParamsFindIntersection;
        }

        public void DisPoseObj()
        {
        }
    }
}