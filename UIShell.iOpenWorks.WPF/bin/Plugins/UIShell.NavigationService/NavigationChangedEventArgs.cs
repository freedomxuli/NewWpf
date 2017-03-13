using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIShell.OSGi;

namespace UIShell.NavigationService
{
    /// <summary>
    /// 导航扩展变更事件参数，即插件被启动/停止。
    /// </summary>
    public class NavigationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 发生变更的插件。
        /// </summary>
        public IBundle Bundle { get; internal set; }
        /// <summary>
        /// 变更动作，增加或者删除。
        /// </summary>
        public CollectionChangedAction Action { get; internal set; }
    }
}
