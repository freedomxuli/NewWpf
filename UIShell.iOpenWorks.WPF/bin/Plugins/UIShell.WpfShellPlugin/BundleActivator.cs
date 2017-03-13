using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using UIShell.ConfigurationService;
using UIShell.NavigationService;
using UIShell.OSGi;
using UIShell.PermissionService;

namespace UIShell.WpfShellPlugin
{
    /// <summary>
    /// 插件激活器。
    /// </summary>
    public class BundleActivator : IBundleActivator
    {
        public static IBundle Bundle { get; private set; }
        public static IBundleContext BundleContext { get; private set; }
        /// <summary>
        /// 导航服务跟踪器。
        /// </summary>
        public static ServiceTracker<INavigationService> NavigationServiceTracker { get; private set; }
        public static ServiceTracker<IConfigurationService> ConfigurationServiceTracker { get; private set; }
        public static ServiceTracker<IPermissionService> PermissionServiceTracker { get; private set; }
        public static ServiceTracker<INavigationServiceFactory> NavigationServiceFactoryTracker { get; private set; }
        public static ServiceTracker<IMainWindowService> MainWindowServiceTracker { get; private set; }
        public void Start(IBundleContext context)
        {
            BundleContext = context;
            Bundle = context.Bundle;
            NavigationServiceTracker = new ServiceTracker<INavigationService>(context);
            ConfigurationServiceTracker = new ServiceTracker<IConfigurationService>(context);
            PermissionServiceTracker = new ServiceTracker<IPermissionService>(context);
            NavigationServiceFactoryTracker = new ServiceTracker<INavigationServiceFactory>(context);
            MainWindowServiceTracker = new ServiceTracker<IMainWindowService>(context);

            FirstFloor.ModernUI.Resources.Culture = new CultureInfo("zh-CN");

            var app = context.GetFirstOrDefaultService<Application>();
            if (app != null)
            {
                try
                {
                    // 注册ModernUI皮肤资源。
                    ResourceDictionary r = new ResourceDictionary();
                    r.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/FirstFloor.ModernUI,Version=1.0.6.0;component/Assets/ModernUI.xaml", UriKind.RelativeOrAbsolute) });
                    r.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/FirstFloor.ModernUI,Version=1.0.6.0;component/Assets/ModernUI.Light.xaml", UriKind.RelativeOrAbsolute) });
                    app.Resources = r;
                }
                catch
                {
                }
            }
        }

        public void Stop(IBundleContext context)
        {
            
        }
    }
}
