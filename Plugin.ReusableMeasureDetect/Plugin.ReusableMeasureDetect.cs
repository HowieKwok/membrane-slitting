using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.ReusableMeasureDetect.Core;
using Plugin.ReusableMeasureDetect.PointFilters.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Plugin.ReusableMeasureDetect
{
    /// <summary>
    /// MEF 插件入口对象。
    /// 框架通过 Export 名称发现该类，运行时由这里串联图像输入、MeasureLine 抓边、
    /// 点集滤波、突变检测、凸点检测、结果输出和 HALCON 对象生命周期清理。
    /// </summary>
    [Category("凸点检测")]
    [DisplayName("凸点测量检测")]
    [Export("ReusableMeasureDetectObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class ReusableMeasureDetectObj : IPlugin
    {
        private bool disposed;

        /// <summary>
        /// 框架监听的运行完成事件；无论正常返回还是异常返回，finally 中都会触发。
        /// </summary>
        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        /// <summary>
        /// 框架模块信息，包含流程 ID、模块名、参数文件路径等上下文。
        /// </summary>
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        /// <summary>
        /// 插件参数对象，实际类型必须是 ParamsReusableMeasureDetect。
        /// </summary>
        public PluginParamsBase PluginParams { get; set; } = new ParamsReusableMeasureDetect();

        /// <summary>
        /// 插件主执行函数。
        /// 执行顺序为：准备输出对象、可选取上游图像、可选 ROI 抓边、点集滤波、凸点检测、突变检测、写出流程变量。
        /// </summary>
        /// <returns>启用检测时返回检测是否整体成功；未启用检测时返回抓边或输入点准备是否成功。</returns>
        public bool ExecuteObj()
        {
            if (disposed) return false;
            ParamsReusableMeasureDetect p = EnsureParams();
            HOperatorSet.CountSeconds(out HTuple start);

            try
            {
                p.EnsureOutputs();
                p.ReleaseOutputs();
                p.m_bRunSuccess = false;
                p.m_strRunMessage = string.Empty;

                if (!p.m_bIsEnable)
                {
                    p.m_bRunSuccess = true;
                    p.m_strRunMessage = "插件未启用，已跳过";
                    return true;
                }

                // ImageSourceIndex=1 时从流程上游取图；ImageSourceIndex=0 时使用界面加载或外部直接写入的图像。
                if (p.ImageSourceIndex == 1)
                    SetInputParams(p.ImageSourceModule, p.ImageSourceName);

                bool grabSuccess = true;
                if (p.EnableMeasureGrab)
                {
                    grabSuccess = MeasureLineGrabEngine.Run(p);
                    if (!grabSuccess && (p.InputPoints == null || p.InputPoints.Count == 0))
                    {
                        p.m_strRunMessage = p.GrabOutput.Message;
                        p.m_bRunSuccess = false;
                        return false;
                    }
                }
                else if (p.InputPoints == null || p.InputPoints.Count == 0)
                {
                    p.m_strRunMessage = "未启用抓边，且没有输入点集";
                    p.m_bRunSuccess = false;
                    return false;
                }

                List<MeasurePointParam> inputPoints = p.InputPoints == null
                    ? new List<MeasurePointParam>()
                    : new List<MeasurePointParam>(p.InputPoints);

                PointFilterPipelineCore.ApplyAndWriteOutput(p, inputPoints);

                bool jumpSuccess = false;
                bool convexSuccess = false;
                string jumpMessage;
                string convexMessage;

                if (p.EnableConvexPeakDetection)
                {
                    convexSuccess = ConvexPeakDetectorCore.DetectAndWriteOverlay(p, !p.EnableJumpDetection);
                    convexMessage = convexSuccess
                        ? string.Format("凸点成功 Row={0:F2}, Col={1:F2}, Score={2:F3}", p.ConvexOutput.PeakRow, p.ConvexOutput.PeakCol, p.ConvexOutput.Score)
                        : "凸点失败：" + p.ConvexOutput.RejectReason;
                }
                else
                {
                    convexMessage = "凸点未启用";
                }

                // 突变与凸点是两个独立检测功能；只有对应启用项为 true 时才参与成功判定。
                if (p.EnableJumpDetection)
                {
                    JumpLineDetector.DetectAndWriteOverlay(p);
                    jumpSuccess = p.JumpOutput.Success;
                    jumpMessage = jumpSuccess
                        ? string.Format("突变成功 主边={0}点 副边={1}点 交点=({2:F2},{3:F2})", p.JumpOutput.MainEdge.Points.Count, p.JumpOutput.SecondaryEdge.Points.Count, p.JumpOutput.IntersectionRow, p.JumpOutput.IntersectionCol)
                        : "突变失败：" + p.JumpOutput.RejectReason;
                }
                else
                {
                    jumpMessage = "突变未启用";
                }

                bool anyDetectorEnabled = p.EnableConvexPeakDetection || p.EnableJumpDetection;
                bool detectorSuccess = (!p.EnableConvexPeakDetection || convexSuccess) && (!p.EnableJumpDetection || jumpSuccess);
                p.m_bRunSuccess = anyDetectorEnabled ? detectorSuccess : grabSuccess;
                p.m_strRunMessage = string.Format(
                    "{0}；滤波步骤={1}，当前点数={2}；{3}；{4}",
                    string.IsNullOrEmpty(p.GrabOutput.Message) ? "点集输入" : p.GrabOutput.Message,
                    string.IsNullOrEmpty(p.FilterOutput.SelectedStepName) ? "未运行" : p.FilterOutput.SelectedStepName,
                    p.FilterOutput.PointCount,
                    convexMessage,
                    jumpMessage);
                if(p.m_bRunSuccess)
                {
                    WriteOutputParams(p);
                }
               
                return p.m_bRunSuccess;
            }
            catch (Exception ex)
            {
                p.m_bRunSuccess = false;
                p.m_strRunMessage = "运行异常：" + ex.Message;
                return false;
            }
            finally
            {
                HOperatorSet.CountSeconds(out HTuple end);
                p.m_fElapsedMs = (end - start) * 1000;
                ExecuteCompleted?.Invoke(this, new PluginExecuteCompletedEventArgs(p, p.m_bRunSuccess, p.m_strRunMessage, (long)p.m_fElapsedMs));
            }
        }

        /// <summary>
        /// 框架初始化入口。同步参数文件路径，保证保存时写回当前模块路径。
        /// </summary>
        public void IniObj()
        {
            ParamsReusableMeasureDetect p = EnsureParams();
            p.filePath = moduleInfo.filePath;
        }

        /// <summary>
        /// 框架保存入口。把当前参数序列化到 moduleInfo 指定的参数文件。
        /// </summary>
        /// <returns>保存成功返回 true；参数对象为空返回 false。</returns>
        public bool saveParams()
        {
            if (PluginParams == null) return false;




            PluginParams.filePath = moduleInfo.filePath;
            return PluginParams.Save();
        }

        /// <summary>
        /// 框架反序列化后回灌参数对象。
        /// </summary>
        /// <param name="param">框架加载出的插件参数。</param>
        public void SetParams(PluginParamsBase param)
        {
            if (param != null) PluginParams = param;
        }

        /// <summary>
        /// 将结果写入当前流程模块输出变量，供后续模块按变量名读取。
        /// </summary>
        /// <param name="varName">输出变量名。</param>
        /// <param name="obj">输出对象。</param>
        public void GetResultParams(string varName, object obj)
        {
            try
            {
                Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
                proc.AddOutPutParam(moduleInfo.ModuleName, varName, obj);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 框架释放入口。释放参数内持有的 HALCON 对象并解除运行完成事件引用。
        /// </summary>
        public void DisPoseObj()
        {
            if (disposed) return;
            disposed = true;
            ParamsReusableMeasureDetect p = PluginParams as ParamsReusableMeasureDetect;
            if (p != null) p.Dispose();
            ExecuteCompleted = null;
        }

        /// <summary>
        /// 获取强类型参数；当框架传入旧参数或空参数时创建默认参数，避免后续空引用。
        /// </summary>
        /// <returns>可复用测量检测参数对象。</returns>
        private ParamsReusableMeasureDetect EnsureParams()
        {
            ParamsReusableMeasureDetect p = PluginParams as ParamsReusableMeasureDetect;
            if (p == null)
            {
                p = new ParamsReusableMeasureDetect();
                PluginParams = p;
            }
            return p;
        }

        /// <summary>
        /// 从流程上游模块读取指定输出变量，并在变量是 HALCON 图像时写入当前参数。
        /// </summary>
        /// <param name="module">上游模块名。</param>
        /// <param name="valueName">上游输出变量名。</param>
        private void SetInputParams(string module, string valueName)
        {
            try
            {
                ParamsReusableMeasureDetect p = EnsureParams();
                Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
                object value = proc.GetOutPutParamByName(module, valueName);
                HObject image = value as HObject;
                if (image != null) p.ho_Image = image;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 统一注册对外输出，包含抓边、滤波、凸点、突变以及最终参与检测的运行点集。
        /// </summary>
        /// <param name="p">当前插件参数。</param>
        private void WriteOutputParams(ParamsReusableMeasureDetect p)
        {
            //GetResultParams("GrabOutput", p.GrabOutput);
            //GetResultParams("FilterOutput", p.FilterOutput);
            //GetResultParams("ConvexOutput", p.ConvexOutput);
            //GetResultParams("JumpOutput", p.JumpOutput);
            //GetResultParams("RuntimeDetectionPoints", p.RuntimeDetectionPoints);
        }
    }
}
