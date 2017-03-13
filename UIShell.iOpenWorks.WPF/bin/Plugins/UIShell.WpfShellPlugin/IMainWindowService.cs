using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UIShell.WpfShellPlugin
{
    /// <summary>
    /// 状态栏服务。
    /// </summary
    public interface IMainWindowService
    {
        /// <summary>
        /// 退出主窗口。
        /// </summary>
        void ExitApplication();
        /// <summary>
        /// 关闭当前显示内容。
        /// </summary>
        void CloseCurrentContent();
        /// <summary>
        /// 设置消息。
        /// </summary>
        /// <param name="message">消息。</param>
        void SetMessage(string message);
        /// <summary>
        /// 设置当前用户。
        /// </summary>
        /// <param name="user">当前用户信息。</param>
        void SetCurrentUser(string user);
        /// <summary>
        /// 显示侧边框。
        /// </summary>
        /// <param name="control">显示的元素。</param>
        /// <param name="width">长度。</param>
        void ShowSidebar(FrameworkElement control, double width, string title);
        /// <summary>
        /// 隐藏侧边框。
        /// </summary>
        void HideSidebar(FrameworkElement control);
    }
}
