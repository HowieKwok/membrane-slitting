using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using LogTool;
using Plugin.CACRunCalibration.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace Plugin.CACRunCalibration
{
    [Category("标定工具")]
    [DisplayName("CAC运行标定")]
    [Export("CACRunCalibrationObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class CACRunCalibrationObj : IPlugin
    {
        private readonly CACRunCalibrationEngine engine = new CACRunCalibrationEngine();

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public PluginParamsBase PluginParams { get; set; } = new ParamsCACRunCalibration();

        public bool ExecuteObj()
        {
            ParamsCACRunCalibration p = PluginParams as ParamsCACRunCalibration;
            if (p == null) return false;

            HOperatorSet.CountSeconds(out HTuple start);
            try
            {
                p.m_bRunSuccess = false;
                p.m_bTransformSuccess = false;

                if (!p.m_bIsEnable)
                {
                    p.m_bRunSuccess = true;
                    p.m_strRunMessage = "插件未启用，跳过执行";
                    return true;
                }

                bool ok = engine.TryPixelToBoard(
                    p,
                    p.m_fInputPixelRow,
                    p.m_fInputPixelCol,
                    out double boardX,
                    out double boardY,
                    out string message);

                p.m_fOutputBoardX = boardX;
                p.m_fOutputBoardY = boardY;
                p.m_bTransformSuccess = ok;
                p.m_bRunSuccess = ok;
                p.m_strRunMessage = ok ? message : "执行失败：" + message;
                return ok;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("CAC运行标定执行失败：" + ex);
                p.m_bRunSuccess = false;
                p.m_bTransformSuccess = false;
                p.m_strRunMessage = "矩阵报错：" + ex.Message;
                return false;
            }
            finally
            {
                HOperatorSet.CountSeconds(out HTuple end);
                p.m_fElapsedMs = (end.D - start.D) * 1000.0;
                ExecuteCompleted?.Invoke(
                this,
                new PluginExecuteCompletedEventArgs(
                    p,
                    p.m_bRunSuccess,
                    p.m_strRunMessage,
                    (long)p.m_fElapsedMs));

                 }
        }





        public void IniObj()
        {
            ParamsCACRunCalibration p = PluginParams as ParamsCACRunCalibration;
            if (p == null) return;

            p.filePath = moduleInfo.filePath;

            if (engine.HasValidMatrix(p, out string message))
            {
                p.m_bHasValidMatrix = true;
                p.m_strMatrixStateText = "矩阵有效";
                p.m_strMatrixCheckMessage = message;
            }
            else
            {
                p.m_bHasValidMatrix = false;
                p.m_strMatrixStateText = "无有效矩阵";
                p.m_strMatrixCheckMessage = message;

            }
        }

        public bool saveParams()
        {
            ParamsCACRunCalibration p = PluginParams as ParamsCACRunCalibration;
            if (p == null) return false;

            p.filePath = moduleInfo.filePath;
            return p.Save();
        }

        public void SetParams(PluginParamsBase param)
        {
            if (param != null) PluginParams = param;
        }

        public void GetResultParams(string resultParamsName, object obj)
        {
        }

        public void DisPoseObj()
        {
            ParamsCACRunCalibration p = PluginParams as ParamsCACRunCalibration;
            if (p == null) return;

            if (p.m_ho_OutImgShow != null) p.m_ho_OutImgShow.Dispose();
            if (p.m_ho_OutQrXlds != null) p.m_ho_OutQrXlds.Dispose();
            p.m_ho_OutImgShow = null;
            p.m_ho_OutQrXlds = null;
        }


    }
}