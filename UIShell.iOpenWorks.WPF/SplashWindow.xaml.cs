using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UIShell.iOpenWorks.WPF
{
    public interface IApplicationLoading
    {
        void AddMessage(string message);
        void PercentComplete(int current);
        void LoadComplete();
    }
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window, IApplicationLoading
    {
        int i = 1;

        public SplashWindow()
        {
            InitializeComponent();
        }

        private bool _completed = false;

        public void AddMessage(string message)
        {
            i = i + 4;
            if (i > 100)
                i = 100;
            if (_completed)
            {
                return;
            }
            Action action = () =>
                {
                    //MessageTextBlock.Text = message;
                    MessageTextBlock.Text = "程序加载中...." + i + "%";
                };
            Dispatcher.Invoke(action);
        }

        public void PercentComplete(int current)
        {
            
        }

        public void LoadComplete()
        {
            if (_completed)
            {
                return;
            }
            _completed = true;
            Dispatcher.InvokeShutdown();
        }
    }
}
