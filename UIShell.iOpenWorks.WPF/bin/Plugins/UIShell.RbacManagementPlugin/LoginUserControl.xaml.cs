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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using UIShell.OSGi.Utility;
using UIShell.PermissionService;
using UIShell.WpfShellPlugin;

namespace UIShell.RbacManagementPlugin
{
    /// <summary>
    /// Interaction logic for LoginUserControl.xaml
    /// </summary>
    public partial class LoginUserControl : UserControl
    {
        private IPermissionService PermissionService { get; set; }
        public LoginUserControl()
        {
            InitializeComponent();
            PermissionService = Activator.PermissionService;
            AssertUtility.NotNull(PermissionService, "PermissionService is not registered.");

            // 从配置文件中获取已经登录的用户
            var loginUsers = Activator.ConfigurationService.Get(Activator.Bundle, "LoginUsers", string.Empty);
            if(!string.IsNullOrEmpty(loginUsers))
            {
                var users = loginUsers.Split(';').Reverse().ToArray(); // 最新登录的放最后
                UserComboBox.ItemsSource = users;
                UserComboBox.SelectedItem = users[0]; // 选择第一项
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UserComboBox.Focus();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            var user = UserComboBox.Text.Trim();
            var password = PasswordTextBox.Password.Trim();
            ErrorTextBlock.Text = string.Empty;
            if(string.IsNullOrEmpty(user))
            {
                ErrorTextBlock.Text = "用户不能为空";
                UserComboBox.Focus();
                return;
            }

            if(string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "密码不能为空";
                PasswordTextBox.Focus();
                return;
            }

            if(!PermissionService.Login(user, password))
            {
                ErrorTextBlock.Text = "用户或密码错误";
                UserComboBox.Focus();
                return;
            }

            var loginUsers = Activator.ConfigurationService.Get(Activator.Bundle, "LoginUsers", string.Empty);
            if(string.IsNullOrEmpty(loginUsers)) // 如果为空，则直接存储
            {
                loginUsers = user;
                // 保存
                Activator.ConfigurationService.Set(Activator.Bundle, "LoginUsers", loginUsers);
            }
            else
            {
                // 如果当前用户与登录用户不相同或者当前用户的最后一个用户与登录用户不同
                // 则需要将当前登录用户放在最后
                if(!loginUsers.Equals(user) && !loginUsers.EndsWith(";" + user))
                {
                    if (loginUsers.Contains(user + ";")) // 当前登录用户是否在中间
                    {
                        loginUsers = loginUsers.Replace(user + ";", "");
                    }

                    loginUsers += ";" + user; // 放在最后
                    // 保存登录用户
                    Activator.ConfigurationService.Set(Activator.Bundle, "LoginUsers", loginUsers);
                }
            }

            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            Application.Current.MainWindow.Show();

            Window.GetWindow(this).Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
