using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace UIShell.RbacManagementPlugin
{
    public class LogoutCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var currentMain = Application.Current.MainWindow;
            // 用于标识注销
            currentMain.Tag = this;

            var loginWindow = new LoginWindow();
            loginWindow.Loaded += (sender, e) => { currentMain.Close(); };
            loginWindow.Show();
        }
    }
}
