using Frame_Core;
using System;
using System.Reflection;

namespace Mef_VisionFrame.调试设置.WPF
{
    internal enum ModuleRunState
    {
        NotRun,
        Failed,
        Success
    }

    internal sealed class ModuleRunStateReader
    {
        public ModuleRunState GetState(IPlugin module)
        {
            if (module == null) return ModuleRunState.NotRun;

            object parameters = module.PluginParams;
            bool? success = TryGetBoolValue(parameters, "m_bRunSuccess");
            if (!success.HasValue) success = TryGetBoolValue(parameters, "RunSuccess");
            if (!success.HasValue) success = TryGetBoolValue(parameters, "IsRunSuccess");
            if (!success.HasValue) success = TryGetBoolValue(parameters, "Success");

            if (success == true) return ModuleRunState.Success;
            if (success == false && HasRunEvidence(module, parameters)) return ModuleRunState.Failed;
            return ModuleRunState.NotRun;
        }

        public string GetToolTip(IPlugin module, ModuleRunState state)
        {
            string status = state == ModuleRunState.Success ? "执行成功" : (state == ModuleRunState.Failed ? "执行失败" : "未执行");
            object message = module == null ? null : TryGetMemberValue(module.PluginParams, "m_strRunMessage");
            string runMessage = message as string;
            if (!string.IsNullOrWhiteSpace(runMessage)) status += "：" + runMessage;
            return status;
        }


        public string GetElapsedText(IPlugin module)
        {
            double? elapsed = GetElapsedMs(module);
            return elapsed.HasValue ? elapsed.Value.ToString("0.###") + " ms" : "-- ms";
        }

        private double? GetElapsedMs(IPlugin module)
        {
            object parameters = module == null ? null : module.PluginParams;
            object elapsed = TryGetMemberValue(parameters, "m_fElapsedMs");
            if (elapsed is double) return (double)elapsed;
            if (elapsed is float) return (float)elapsed;
            if (elapsed is int) return (int)elapsed;
            if (elapsed is long) return (long)elapsed;

            if (module != null && module.moduleInfo != null && module.moduleInfo.CostTime >= 0)
            {
                return module.moduleInfo.CostTime;
            }

            return null;
        }
        private bool HasRunEvidence(IPlugin module, object parameters)
        {
            // 执行状态以 PluginParams 为唯一来源。moduleInfo.CostTime 只做界面耗时显示，不参与状态灯判断。
            object elapsed = TryGetMemberValue(parameters, "m_fElapsedMs");
            if (elapsed is double && (double)elapsed > 0) return true;
            if (elapsed is float && (float)elapsed > 0) return true;
            if (elapsed is int && (int)elapsed > 0) return true;
            if (elapsed is long && (long)elapsed > 0) return true;

            object message = TryGetMemberValue(parameters, "m_strRunMessage");
            return message is string && !string.IsNullOrWhiteSpace((string)message);
        }

        private bool? TryGetBoolValue(object target, string name)
        {
            object value = TryGetMemberValue(target, name);
            if (value is bool) return (bool)value;
            return null;
        }

        private object TryGetMemberValue(object target, string name)
        {
            if (target == null) return null;

            Type type = target.GetType();
            FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null) return field.GetValue(target);

            PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null && property.CanRead) return property.GetValue(target, null);

            return null;
        }
    }
}
