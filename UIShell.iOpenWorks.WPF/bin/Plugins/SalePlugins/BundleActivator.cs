using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UIShell.ConfigurationService;
using UIShell.DbConnectionFactoryService;
using UIShell.OSGi;
using UIShell.PermissionService;
using UIShell.WpfShellPlugin;

namespace SalePlugins
{
    /// <summary>
    /// 插件激活器，用于获取服务、执行初始化等操作。
    /// </summary>
    public class BundleActivator : IBundleActivator
    {
        /// <summary>
        /// 当前插件。
        /// </summary>
        public static IBundle Bundle { get; private set; }
        /// <summary>
        /// 当前插件上下文。
        /// </summary>
        public static IBundleContext BundleContext
        {
            get
            {
                if (Bundle != null)
                {
                    return Bundle.Context;
                }
                return null;
            }
        }


        private static ServiceTracker<IPermissionService> PermissionServiceTracker { get; set; }
        /// <summary>
        /// 权限检测服务，用于在编码时进行权限检测。
        /// </summary>
        public static IPermissionService PermissionService
        {
            get
            {
                if (PermissionServiceTracker != null)
                {
                    return PermissionServiceTracker.DefaultOrFirstService;
                }
                return null;
            }
        }

        private static ServiceTracker<IConfigurationService> ConfigurationServiceTracker { get; set; }
        /// <summary>
        /// 插件持久化配置，用于获取和保存插件的配置信息，比如分隔条位置等。
        /// </summary>
        public static IConfigurationService ConfigurationService
        {
            get
            {
                if (ConfigurationServiceTracker != null)
                {
                    return ConfigurationServiceTracker.DefaultOrFirstService;
                }
                return null;
            }
        }

        public static ServiceTracker<IDbConnectionFactoryService> DbConnectionFactoryServiceTracker { get; private set; }
        /// <summary>
        /// 数据库连接工厂服务，用于获取数据库连接。数据库连接统一由该服务进行管理。
        /// </summary>
        public static IDbConnectionFactoryService DbConnectionFactoryService
        {
            get
            {
                if (DbConnectionFactoryServiceTracker != null)
                {
                    return DbConnectionFactoryServiceTracker.DefaultOrFirstService;
                }
                return null;
            }
        }

        private static ServiceTracker<IMainWindowService> MainWindowServiceTracker { get; set; }
        /// <summary>
        /// 主窗体服务，提供了关闭当前控件、访问状态栏功能。
        /// </summary>
        public static IMainWindowService MainWindowService
        {
            get
            {
                if (MainWindowServiceTracker != null)
                {
                    return MainWindowServiceTracker.DefaultOrFirstService;
                }
                return null;
            }
        }

        /// <summary>
        /// 启动插件时，执行操作。
        /// </summary>
        /// <param name="context">插件上下文。</param>
        public void Start(IBundleContext context)
        {
            Bundle = context.Bundle;

            PermissionServiceTracker = new ServiceTracker<IPermissionService>(context);
            ConfigurationServiceTracker = new ServiceTracker<IConfigurationService>(context);
            DbConnectionFactoryServiceTracker = new ServiceTracker<IDbConnectionFactoryService>(context);
            MainWindowServiceTracker = new ServiceTracker<IMainWindowService>(context);
        }

        /// <summary>
        /// 停止插件时执行操作。
        /// </summary>
        /// <param name="context">插件上下文。</param>
        public void Stop(IBundleContext context)
        {

        }
    }
}
