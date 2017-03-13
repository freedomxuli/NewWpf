using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;

namespace UIShell.iOpenWorks.WPF
{
    class Program
    {
        static ManualResetEvent ResetSplashCreated { get; set; }
        static Thread SplashThread { get; set; }
        static SplashWindow SplashWindow { get; set; }
        /// <summary>
        /// 是否检查允许多个进程。
        /// </summary>
        static bool OnlySingleProcess
        {
            get
            {
                var setting = System.Configuration.ConfigurationManager.AppSettings["OnlySingleProcess"];
                if(!string.IsNullOrEmpty(setting) && string.Compare(setting, "true", true) == 0)
                {
                    return true;
                }
                return false;
            }
        }

        [STAThreadAttribute()]
        public static void Main()
        {
            if (OnlySingleProcess && ApplicationRunningHelper.AlreadyRunning())
            {
                return;
            }

            ResetSplashCreated = new ManualResetEvent(false);
            SplashThread = new Thread(ShowSplash);
            SplashThread.SetApartmentState(ApartmentState.STA);
            SplashThread.IsBackground = true;
            SplashThread.Name = "Splash Screen";
            SplashThread.Start();

            ResetSplashCreated.WaitOne();

            var app = new App(SplashWindow);
            app.Run();
        }

        private static void ShowSplash()
        {
            SplashWindow = new SplashWindow();
            SplashWindow.Show();
            ResetSplashCreated.Set();
            System.Windows.Threading.Dispatcher.Run();
        }
    }

    public static class ApplicationRunningHelper
    {
        [DllImport("user32.dll")]
        private static extern
            bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern
            bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern
            bool IsIconic(IntPtr hWnd);

        public static bool AlreadyRunning()
        {
            /*
            const int SW_HIDE = 0;
            const int SW_SHOWNORMAL = 1;
            const int SW_SHOWMINIMIZED = 2;
            const int SW_SHOWMAXIMIZED = 3;
            const int SW_SHOWNOACTIVATE = 4;
            const int SW_RESTORE = 9;
            const int SW_SHOWDEFAULT = 10;
            */
            const int swRestore = 9;

            var me = Process.GetCurrentProcess();
            var arrProcesses = Process.GetProcessesByName(me.ProcessName);

            if (arrProcesses.Length > 1)
            {
                for (var i = 0; i < arrProcesses.Length; i++)
                {
                    if (arrProcesses[i].Id != me.Id)
                    {
                        // get the window handle
                        IntPtr hWnd = arrProcesses[i].MainWindowHandle;

                        // if iconic, we need to restore the window
                        if (IsIconic(hWnd))
                        {
                            ShowWindowAsync(hWnd, swRestore);
                        }

                        // bring it to the foreground
                        SetForegroundWindow(hWnd);
                        break;
                    }
                }
                return true;
            }

            return false;
        }
    }
}
