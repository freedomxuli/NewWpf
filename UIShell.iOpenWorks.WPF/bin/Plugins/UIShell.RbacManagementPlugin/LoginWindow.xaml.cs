using System;
using System.Collections.Generic;
using System.Globalization;
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
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using UIShell.ConfigurationService;
using UIShell.OSGi.Utility;

namespace UIShell.RbacManagementPlugin
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : ModernWindow
    {
        private IConfigurationService ConfigurationService { get; set; }
        public LoginWindow()
        {
            InitializeComponent();


            ConfigurationService = Activator.ConfigurationService;
            AssertUtility.NotNull(ConfigurationService);

            LoadSetting();

            Content = new LoginUserControl();
        }

        private void LoadSetting()
        {
            var shellPlugin = Activator.WpfShellBundle;

            // 获取颜色
            var color = ConfigurationService.Get(shellPlugin, "Color", "#FF1BA1E2");
            try
            {
                AppearanceManager.Current.AccentColor = (Color)ColorConverter.ConvertFromString(color);
            }
            catch // 设置默认颜色
            {
                AppearanceManager.Current.AccentColor = (Color)ColorConverter.ConvertFromString("#FF1BA1E2");
            }

            // 获取字体大小
            var fontSize = ConfigurationService.Get(shellPlugin, "FontSize", FirstFloor.ModernUI.Presentation.FontSize.Large.ToString());
            AppearanceManager.Current.FontSize = fontSize.Equals(FirstFloor.ModernUI.Presentation.FontSize.Large.ToString()) ?
                FirstFloor.ModernUI.Presentation.FontSize.Large : FirstFloor.ModernUI.Presentation.FontSize.Small;

            // 获取皮肤
            var skin = ConfigurationService.Get(shellPlugin, "Skin", "/FirstFloor.ModernUI,Version=1.0.6.0;component/Assets/ModernUI.Light.xaml");
            AppearanceManager.Current.ThemeSource = new Uri(skin, UriKind.RelativeOrAbsolute);
        }
    }
}
