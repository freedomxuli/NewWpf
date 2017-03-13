using System;
using System.Collections.Generic;
using System.Text;
using UIShell.OSGi;
using UIShell.PermissionService;

namespace UIShell.NavigationService
{
    /// <summary>
    /// 插件激活器。
    /// </summary>
    public class BundleActivator : IBundleActivator
    {
        /// <summary>
        /// 当前插件对象。
        /// </summary>
        public static IBundle Bundle { get; private set; }
        /// <summary>
        /// 权限服务跟踪器。
        /// </summary>
        public static ServiceTracker<IPermissionService> PermissionServiceTracker { get; private set; }

        public void Start(IBundleContext context)
        {
            Bundle = context.Bundle;
            var navigationFactory = new NavigationServiceFactory(context);
            context.AddService<INavigationServiceFactory>(navigationFactory);
            context.AddService<INavigationService>(navigationFactory.CreateNavigationService("UIShell.NavigationService"));
            PermissionServiceTracker = new ServiceTracker<IPermissionService>(context);
        }

        public void Stop(IBundleContext context)
        {
            
        }
    }
}
