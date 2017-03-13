using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UIShell.OSGi;
using UIShell.OSGi.Logging;
using UIShell.iOpenWorks.Bootstrapper;
using UIShell.OSGi.Utility;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UIShell.iOpenWorks.WPF
{
    /// <summary>
    /// WPF startup class.
    /// </summary>
    public partial class App : Application
    {
        // Use object type to avoid load UIShell.OSGi.dll before update.
        private object _bundleRuntime;
        private IApplicationLoading _loading;

        public App(IApplicationLoading loading)
        {
            _loading = loading;
            _loading.AddMessage("检查更新......");
            UpdateCore();
            _loading.AddMessage("启动插件内核......");
            StartBundleRuntime();
        }

        private void UpdateCore() // Update Core Files, including BundleRepositoryOpenAPI, PageFlowService and OSGi Core assemblies.
        {
            if (AutoUpdateCoreFiles)
            {
                new CoreFileUpdater().UpdateCoreFiles(CoreFileUpdateCheckType.Daily);
            }
        }

        private void StartBundleRuntime() // Start OSGi Core.
        {
            FileLogUtility.SetLogLevel(LogLevel);
            FileLogUtility.SetMaxFileSizeByMB(MaxLogFileSize);
            FileLogUtility.SetCreateNewFileOnMaxSize(CreateNewLogFileOnMaxSize);

            var bundleRuntime = new BundleRuntime();
            bundleRuntime.EnableAssemblyMultipleVersions = false;
            bundleRuntime.Framework.EventManager.AddBundleEventListener(BundleStateChangedHandler, true);
            bundleRuntime.Framework.EventManager.AddFrameworkEventListener(FrameworkStateChangedHandler);

            bundleRuntime.AddService<Application>(this);
            bundleRuntime.Start();
            bundleRuntime.Framework.EventManager.RemoveBundleEventListener(BundleStateChangedHandler, true);
            bundleRuntime.Framework.EventManager.RemoveFrameworkEventListener(FrameworkStateChangedHandler);

            Startup += App_Startup;
            Exit += App_Exit;
            _bundleRuntime = bundleRuntime;
        }

        private void BundleStateChangedHandler(object sender, BundleStateChangedEventArgs e)
        {
            if(e.CurrentState == BundleState.Installed)
            {
                _loading.AddMessage(string.Format("安装插件'{0}'......", e.Bundle.SymbolicName));
            }
            else if (e.CurrentState == BundleState.Starting)
            {
                _loading.AddMessage(string.Format("正在启动插件'{0}'......", e.Bundle.SymbolicName));
            }
            else if (e.CurrentState == BundleState.Active)
            {
                _loading.AddMessage(string.Format("插件'{0}'启动完成......", e.Bundle.SymbolicName));
            }
        }

        private void FrameworkStateChangedHandler(object sender, FrameworkEventArgs e)
        {
            if(e.EventType == FrameworkEventType.Started)
            {
                _loading.AddMessage("插件内核启动完成，开始初始化数据库......");
            }
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            if (_bundleRuntime == null)
            {
                return;
            }

            _loading.AddMessage("启动完成......");
            _loading.LoadComplete();

            Application app = Application.Current;
            app.ShutdownMode = ShutdownMode.OnLastWindowClose;
            var bundleRuntime = _bundleRuntime as BundleRuntime;

            var loginWindow = bundleRuntime.GetFirstOrDefaultService<Window>();
            loginWindow.Loaded += (sender2, e2) => 
            {
                loginWindow.Activate();
            };
            loginWindow.Show();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            if (_bundleRuntime != null)
            {
                var bundleRuntime = _bundleRuntime as BundleRuntime;
                bundleRuntime.Stop();
                _bundleRuntime = null;
            }
        }

        #region Settings
        /// <summary>
        /// 日志级别。
        /// </summary>
        private static LogLevel LogLevel
        {
            get
            {
                string level = ConfigurationManager.AppSettings["LogLevel"];
                if (!string.IsNullOrEmpty(level))
                {
                    try
                    {
                        object result = Enum.Parse(typeof(LogLevel), level);
                        if (result != null)
                        {
                            return (LogLevel)result;
                        }
                    }
                    catch { }
                }
                return LogLevel.Debug;
            }
        }

        /// <summary>
        /// 日志文件限制的大小。
        /// </summary>
        private static int MaxLogFileSize
        {
            get
            {
                string size = ConfigurationManager.AppSettings["MaxLogFileSize"];
                if (!string.IsNullOrEmpty(size))
                {
                    try
                    {
                        return int.Parse(size);
                    }
                    catch { }
                }

                return 10;
            }
        }

        /// <summary>
        /// 当日志大小超过限制时，是否新建一个。
        /// </summary>
        private static bool CreateNewLogFileOnMaxSize
        {
            get
            {
                string createNew = ConfigurationManager.AppSettings["CreateNewLogFileOnMaxSize"];
                if (!string.IsNullOrEmpty(createNew))
                {
                    try
                    {
                        return bool.Parse(createNew);
                    }
                    catch { }
                }

                return false;
            }
        }

        /// <summary>
        /// 当日志大小超过限制时，是否新建一个。
        /// </summary>
        private static bool AutoUpdateCoreFiles
        {
            get
            {
                string autoUpdateCoreFiles = ConfigurationManager.AppSettings["AutoUpdateCoreFiles"];
                if (!string.IsNullOrEmpty(autoUpdateCoreFiles))
                {
                    try
                    {
                        return bool.Parse(autoUpdateCoreFiles);
                    }
                    catch { }
                }

                return false;
            }
        }
        #endregion
    }

    
}
