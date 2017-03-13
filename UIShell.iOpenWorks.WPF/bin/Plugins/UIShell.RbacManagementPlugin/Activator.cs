using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UIShell.ConfigurationService;
using UIShell.OSGi;
using UIShell.PermissionService;
using UIShell.RbacManagementPlugin.ViewModels;
using UIShell.RbacPermissionService.DataAccess;
using UIShell.RbacPermissionService.Model;
using UIShell.WpfShellPlugin;

namespace UIShell.RbacManagementPlugin
{
    public class Activator : IBundleActivator
    {
        public static IBundle Bundle { get; private set; }
        public static IBundle WpfShellBundle
        {
            get
            {
                return Bundle.Context.GetBundleBySymbolicName("UIShell.WpfShellPlugin");
            }
        }
        private static ServiceTracker<IPermissionService> PermissionServiceTracker { get; set; }
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
        private static ServiceTracker<IPermissionExtensionModelService> PermissionExtensionModelServiceTracker { get; set; }
        public static IPermissionExtensionModelService PermissionExtensionModelService
        {
            get
            {
                if (PermissionExtensionModelServiceTracker != null)
                {
                    return PermissionExtensionModelServiceTracker.DefaultOrFirstService;
                }
                return null;
            }
        }

        private static ServiceTracker<IConfigurationService> ConfigurationServiceTracker { get; set; }
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

        private static ServiceTracker<IMainWindowService> MainWindowServiceTracker { get; set; }
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

        public void Start(IBundleContext context)
        {
            Bundle = context.Bundle;
            var userDataAccessor = new UserDataAccessor();
            userDataAccessor.CreateDefaultSystemManager();
            PermissionServiceTracker = new ServiceTracker<IPermissionService>(context);
            PermissionExtensionModelServiceTracker = new ServiceTracker<IPermissionExtensionModelService>(context);
            ConfigurationServiceTracker = new ServiceTracker<IConfigurationService>(context);
            MainWindowServiceTracker = new ServiceTracker<IMainWindowService>(context);

            context.AddService<Window>(new LoginWindow());
        }

        public void Stop(IBundleContext context)
        {

        }
    }
}
