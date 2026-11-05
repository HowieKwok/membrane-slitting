using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrameCommunity
{
    /// <summary>
    /// WPF 窗口生命周期 & 资源管控统一接口
    /// 规范所有自定义窗口的生命周期、事件、资源释放行为
    /// </summary>
    public interface IWpfWindowBase 
    {
        #region 生命周期回调
        /// <summary>
        /// 窗口视觉树加载完成（首次渲染后）执行初始化
        /// </summary>
        void OnLoadedComplete();

        /// <summary>
        /// 窗口从视觉树移除时执行（临时资源释放）
        /// </summary>
        void OnUnloadedRelease();

        /// <summary>
        /// 窗口即将关闭前，最终资源释放入口
        /// </summary>
        void OnClosingFinalRelease();
        #endregion

        #region 通用UI清理方法
        /// <summary>
        /// 清空当前窗口及内部所有 DataContext，Command,Click,切断绑定与事件订阅
        /// </summary>
        void ClearAllDataContext();
        #endregion

        #region 事件解绑管理
        /// <summary>
        /// 注册自定义解绑动作，窗口关闭时自动执行
        /// </summary>
        /// <param name="unbindAction">解绑委托</param>
        void RegisterUnbindAction(Action unbindAction);

        #endregion

        #region 非托管资源专属
        /// <summary>
        /// 专门释放非托管资源：Halcon、相机句柄、原生控件、SDK 等
        /// </summary>
        void ReleaseUnmanagedResource();
        #endregion


    }
}
