using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using LogTool;
using Plugin.CombineLineCross.Core;


namespace Plugin.CombineLineCross
{
    [Category("尺寸测量")]
    [DisplayName("组合交点工具")]
    [Export("CombineLineCrossObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class CombineLineCrossObj : IPlugin
    {
        //private readonly FindEngine engine = new FindEngine();
        private ParamsCombPoint findParams;

        public PluginParamsBase PluginParams { get; set; } = new ParamsCombPoint();
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public bool isExistModel = false;

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public bool ExecuteObj()
        {
            findParams = PluginParams as ParamsCombPoint;
            if (findParams == null)
            {
                Log4netRecord.ErrorLog.Debug("组合交点工具：参数类型错误");
                return false;
            }

            HOperatorSet.CountSeconds(out HTuple start);
            bool success = false;
            try
            {
                if (!findParams.m_bIsEnable)
                {
                    findParams.m_strRunMessage = moduleInfo.ModuleName + "_未启用";
                    return false;
                }
                

                success = CombineLineCrossEngine.Run(findParams);
                if (success)
                {
                   
                    //PublishResultParams();
                }

                return success;
            }
            catch (Exception ex)
            {
                findParams.m_strRunMessage = ex.Message;
                Log4netRecord.ErrorLog.Debug("组合工具执行异常：" + ex);
                return false;
            }
            finally
            {
                HOperatorSet.CountSeconds(out HTuple end);
                HTuple elapsed = end - start;
                findParams.m_fElapsedMs = elapsed.D * 1000.0;
                findParams.m_bRunSuccess = success;
                if (string.IsNullOrWhiteSpace(findParams.m_strRunMessage))
                {
                    findParams.m_strRunMessage = success ? "运行成功" : "交点计算失败";
                }

                elapsed.Dispose();
                start.Dispose();
                end.Dispose();

                ExecuteCompleted?.Invoke(
                  this,
                  new PluginExecuteCompletedEventArgs(
                      findParams,
                      findParams.m_bRunSuccess,
                      findParams.m_strRunMessage,
                      (long)findParams.m_fElapsedMs));
        }
        }

        //private void PublishResultParams()
        //{
        //    try
        //    {
        //        GetResultParams("IntersectionRow", findParams.hv_IntersectionRow);
        //        GetResultParams("IntersectionColumn", findParams.hv_IntersectionColumn);
        //        GetResultParams("IntersectionAngle", findParams.hv_IntersectionAngle);
        //        GetResultParams("IsParallel", findParams.hv_IsParallel);
        //        GetResultParams("Line1RowBegin", findParams.hv_Line1_RowBegin);
        //        GetResultParams("Line1ColumnBegin", findParams.hv_Line1_ColumnBegin);
        //        GetResultParams("Line1RowEnd", findParams.hv_Line1_RowEnd);
        //        GetResultParams("Line1ColumnEnd", findParams.hv_Line1_ColumnEnd);
        //        GetResultParams("Line2RowBegin", findParams.hv_Line2_RowBegin);
        //        GetResultParams("Line2ColumnBegin", findParams.hv_Line2_ColumnBegin);
        //        GetResultParams("Line2RowEnd", findParams.hv_Line2_RowEnd);
        //        GetResultParams("Line2ColumnEnd", findParams.hv_Line2_ColumnEnd);
        //        GetResultParams("FollowOffsetX", findParams.m_fFollowOffsetX);
        //        GetResultParams("FollowOffsetY", findParams.m_fFollowOffsetY);
        //        GetResultParams("FollowOffsetR", findParams.m_fFollowOffsetR);
        //        GetResultParams("偏差X", findParams.m_fFollowOffsetX);
        //        GetResultParams("偏差Y", findParams.m_fFollowOffsetY);
        //        GetResultParams("偏差R", findParams.m_fFollowOffsetR);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug("两线交点工具发布结果异常：" + ex);
        //    }
        //}

        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath;
            return PluginParams.Save();
        }

        public void SetParams(PluginParamsBase param)
        {
            PluginParams = (ParamsCombPoint)param;
            findParams = PluginParams as ParamsCombPoint;
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
                findParams = PluginParams as ParamsCombPoint;
                if (findParams == null)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点工具：参数类型错误");
                    return;
                }

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
            findParams = PluginParams as ParamsCombPoint;
        }

        public void DisPoseObj()
        {
            // 只释放插件执行时创建的输出元组，不释放流程传入的 ho_Image。
            if (findParams == null) findParams = PluginParams as ParamsCombPoint;
            if (findParams == null) return;
            DisposeTuple(ref findParams.hv_IntersectionRow);
            DisposeTuple(ref findParams.hv_IntersectionColumn);
            DisposeTuple(ref findParams.hv_LineAngle);
            DisposeTuple(ref findParams.hv_IsParallel);
        }

        private static void DisposeTuple(ref HTuple value)
        {
            if (value != null)
            {
                value.Dispose();
                value = null;
            }
        }
    }
}
