using System;
using System.Collections.Generic;
using System.Text;
using UIShell.OSGi;
using UIShell.OSGi.Collection;

namespace UIShell.NavigationService
{
    /// <summary>
    /// 界面框架导航扩展服务。每一个插件通过以下格式来注册扩展信息：
    /// <![CDATA[
    /// <Extension Point="UIShell.NavigationService">
    ///   <Node  Id="" Name="" Order="" Permission="" Icon="" Tooltip="">
    ///     <Node Id="" Name="" Value="" Order="" Permission=""  Icon="" Tooltip="" />
    ///   </Node>
    ///   <Node  Id="" Name="" Order="" Permission=""  Icon="" Tooltip="">
    ///     <Node Id="" Name="" Value="" Order="" Permission=""  Icon="" Tooltip="" />
    ///   </Node>
    /// </Extension>
    /// ]]>
    /// 这样的扩展信息将会被该服务处理，并转换为对象。
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// 排序的所有顶层节点。
        /// </summary>
        SortedSet<NavigationNode> NavigationNodes { get; }
        /// <summary>
        /// 扩展发生变更，即有插件被启动或者插件被停止。
        /// </summary>
        event EventHandler<NavigationChangedEventArgs> NavigationChanged;
    }

    /// <summary>
    /// 导航服务工厂。为指定名称的扩展点创建一个导航服务。
    /// </summary>
    public interface INavigationServiceFactory
    {
        /// <summary>
        /// 为指定扩展点创建一个导航服务。
        /// </summary>
        /// <param name="extensionPoint">扩展点。</param>
        /// <returns>导航服务。</returns>
        INavigationService CreateNavigationService(string extensionPoint);
        /// <summary>
        /// 删除指定扩展点的导航服务。
        /// </summary>
        /// <param name="extensionPoint">扩展点。</param>
        void DisposeNavigationService(string extensionPoint);
    }
}
