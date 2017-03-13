using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIShell.RbacPermissionService.DataAccess;

namespace UIShell.RbacManagementPlugin
{
    /// <summary>
    /// Interaction logic for ChangePasswordUserControl.xaml
    /// </summary>
    public partial class ChangePasswordUserControl : UserControl
    {
        public ChangePasswordUserControl()
        {
            InitializeComponent();
            CurrentUserTextBlock.Text = Activator.PermissionService.CurrentUserName;

            CurrentPasswordPasswordBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string password = CurrentPasswordPasswordBox.Password;
            if (string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "密码不能为空";
                CurrentPasswordPasswordBox.Focus();
                return;
            }
            var userDataAccessor = new UserDataAccessor();
            if(userDataAccessor.Login(Activator.PermissionService.CurrentUserName, password) == null)
            {
                ErrorTextBlock.Text = "密码错误";
                CurrentPasswordPasswordBox.Focus();
                return;
            }

            string newPassword = NewPasswordPasswordBox.Password;
            ErrorTextBlock.Text = string.Empty;
            if(string.IsNullOrEmpty(newPassword))
            {
                ErrorTextBlock.Text = "新密码不能为空";
                NewPasswordPasswordBox.Focus();
                return;
            }
            if (!NewPasswordPasswordBox.Password.Equals(ConfirmPasswordPasswordBox.Password))
            {
                ErrorTextBlock.Text = "新密码与确认密码不一致";
                NewPasswordPasswordBox.Focus();
                return;
            }

           
            userDataAccessor.UpdatePassword(Activator.PermissionService.CurrentUserId, newPassword);
            ErrorTextBlock.Text = "密码更改成功，2秒后关闭...";

            Timer timer = null;

            timer = new Timer(state => 
            {
                Action action = () =>
                    {
                        Window.GetWindow(this).Close();
                    };
                Dispatcher.Invoke(action);
                timer.Dispose();
            }, null, 1000, Timeout.Infinite);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
