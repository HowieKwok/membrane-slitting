using Frame_Core;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Mef_VisionFrame.调试设置.WPF
{
    internal sealed class PluginDialogService
    {
        //public void Open(IPlugin plugin, Window owner, Action<PluginViewActionResult> actionCompleted)
        //{
        //    if (plugin == null) return;

        //    Type wpfContentType = ResolveWpfContentType(plugin);
        //    Type formType = PluginService.pluginsDictionary.ContainsKey(plugin.moduleInfo.PluginName)
        //        ? PluginService.pluginsDictionary[plugin.moduleInfo.PluginName].PluginsFormType
        //        : null;

        //    if (wpfContentType != null)
        //    {
        //        OpenWpfPlugin(plugin, wpfContentType, owner, actionCompleted);
        //        return;
        //    }

        //    if (formType != null && typeof(FormPluginBase).IsAssignableFrom(formType))
        //    {
        //        OpenWinFormsPlugin(plugin, formType, actionCompleted);
        //    }
        //}

        //private void OpenWpfPlugin(IPlugin plugin, Type wpfContentType, Window owner, Action<PluginViewActionResult> actionCompleted)
        //{
        //    WPFFormPluginBase window = new WPFFormPluginBase();
        //    object content = Activator.CreateInstance(wpfContentType);
        //    TryBindPlugin(content, plugin);

        //    EventHandler<PluginViewActionResult> handler = null;
        //    if (actionCompleted != null)
        //    {
        //        // 执行按钮在 WPFFormPluginBase 上，动作完成通知也由基类窗体发出。
        //        handler = (sender, result) => actionCompleted(result);
        //        window.ActionCompleted += handler;
        //    }

        //    window.pluginsObjBase = plugin;
        //    window.PluginContent = content;
        //    window.Init();
        //    window.Owner = owner;

        //    try
        //    {
        //        window.ShowDialog();
        //    }
        //    finally
        //    {
        //        if (handler != null) window.ActionCompleted -= handler;
        //    }
        //}
        //private void OpenWinFormsPlugin(IPlugin plugin, Type formType, Action<PluginViewActionResult> actionCompleted)
        //{
        //    FormPluginBase form = (FormPluginBase)Activator.CreateInstance(formType);
        //    EventHandler<PluginViewActionResult> handler = null;
        //    if (actionCompleted != null)
        //    {
        //        // WinForm 插件也统一由 FormPluginBase 的执行按钮发出动作完成通知。
        //        handler = (sender, result) => actionCompleted(result);
        //        form.ActionCompleted += handler;
        //    }

        //    try
        //    {
        //        form.pluginsObjBase = plugin;
        //        form.Init();
        //        form.ShowDialog();
        //    }
        //    finally
        //    {
        //        if (handler != null) form.ActionCompleted -= handler;
        //        form.Dispose();
        //    }
        //}
        public void Open(IPlugin plugin, Window owner)
        {
            if (plugin == null) return;

            Type wpfContentType = ResolveWpfContentType(plugin);
            Type formType = PluginService.pluginsDictionary.ContainsKey(plugin.moduleInfo.PluginName)
                ? PluginService.pluginsDictionary[plugin.moduleInfo.PluginName].PluginsFormType
                : null;

            if (wpfContentType != null)
            {
                OpenWpfPlugin(plugin, wpfContentType, owner);
                return;
            }

            if (formType != null && typeof(FormPluginBase).IsAssignableFrom(formType))
            {
                OpenWinFormsPlugin(plugin, formType);
            }
        }

        private void OpenWpfPlugin(IPlugin plugin, Type wpfContentType, Window owner)
        {
            WPFFormPluginBase window = new WPFFormPluginBase();
            object content = Activator.CreateInstance(wpfContentType);
            TryBindPlugin(content, plugin);

            window.pluginsObjBase = plugin;
            window.PluginContent = content;
            window.Init();
            window.Owner = owner;
            window.ShowDialog();
        }

        private void OpenWinFormsPlugin(IPlugin plugin, Type formType)
        {
            FormPluginBase form = (FormPluginBase)Activator.CreateInstance(formType);
            try
            {
                form.pluginsObjBase = plugin;
                form.Init();
                form.ShowDialog();
            }
            finally
            {
                form.Dispose();
            }
        }


        private Type ResolveWpfContentType(IPlugin plugin)
        {
            Assembly assembly = plugin.GetType().Assembly;
            return assembly.GetTypes()
                .FirstOrDefault(t => !t.IsAbstract
                    && typeof(UserControl).IsAssignableFrom(t)
                    && t.GetMethod("BindPlugin", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null);
        }

        private void TryBindPlugin(object target, IPlugin plugin)
        {
            if (target == null) return;

            MethodInfo method = target.GetType().GetMethod("BindPlugin", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(target, new object[] { plugin });
                return;
            }

            PropertyInfo property = target.GetType().GetProperty("pluginsObjBase", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null && property.CanWrite) property.SetValue(target, plugin, null);
        }
    }
}
