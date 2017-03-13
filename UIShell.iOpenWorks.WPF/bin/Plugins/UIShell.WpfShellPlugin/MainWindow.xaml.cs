using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Presentation;
using UIShell.OSGi.Utility;
using UIShell.ConfigurationService;
using System.Globalization;
using System.Windows.Controls.Primitives;
using UIShell.NavigationService;
using UIShell.OSGi;

namespace UIShell.WpfShellPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public const string SystemMenuExtensionPoint = "UIShell.SystemMenu";
        private IConfigurationService _configurationService;
        private INavigationService _systemMenuNavigationService;

        public MainWindow()
        {
            InitializeComponent();

            var navigationServiceFactory = BundleActivator.NavigationServiceFactoryTracker.DefaultOrFirstService;
            _systemMenuNavigationService = navigationServiceFactory.CreateNavigationService(SystemMenuExtensionPoint);
            _configurationService = BundleActivator.ConfigurationServiceTracker.DefaultOrFirstService;
            AssertUtility.NotNull(_configurationService);
            AssertUtility.NotNull(_systemMenuNavigationService);

            Closing += (sender, e) => SaveSettings();

            LoadSettings();
        }

        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _systemContextMenu = new ContextMenu();

            InitSystemMenu();
            _systemMenuNavigationService.NavigationChanged += (sender2, e2) => InitSystemMenu();
        }

        private ContextMenu _systemContextMenu;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var child = GetTemplateChild("SystemMenuButton");
            if(child != null && child is Button)
            {
                var systemMenuButton = child as Button;

                CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.ShowSystemMenuCommand, OnShowSystemMenu, OnCanShowSystemMenu));
            }
        }

        private void InitSystemMenu()
        {
            if (BundleRuntime.Instance.State == BundleRuntimeState.Stopping)
            {
                return;
            }

            Action action = () =>
                {
                    _systemContextMenu.Items.Clear();
                    foreach (var node in _systemMenuNavigationService.NavigationNodes)
                    {
                        var menuItem = new MenuItem();
                        if(!string.IsNullOrEmpty(node.Icon))
                        {
                            try
                            {
                                var uri = new Uri(node.Icon, UriKind.RelativeOrAbsolute);
                                BitmapImage logo = new BitmapImage();
                                logo.BeginInit();
                                logo.UriSource = uri;
                                logo.EndInit();
                                var image = new Image();
                                image.Source = logo;
                                image.Height = 16;
                                image.Width = 16;
                                menuItem.Icon = image;
                            }
                            catch(Exception ex)
                            {
                                FileLogUtility.Error(string.Format("Failed to load icon '{0}' from bundle '{1}'", node.Icon, node.Bundle.SymbolicName));
                                FileLogUtility.Error(ex);
                            }
                        }
                        menuItem.Header = node.Name;
                        if(!string.IsNullOrEmpty(node.ToolTip))
                        { 
                            menuItem.ToolTip = node.ToolTip;
                        }
                        menuItem.Tag = node;
                        menuItem.Click += SystemMenuClick;
                        _systemContextMenu.Items.Add(menuItem);
                    }
                };
            Dispatcher.Invoke(action);
        }

        private void SystemMenuClick(object sender, RoutedEventArgs e)
        {
            var node = (sender as MenuItem).Tag as NavigationNode;
            if(!string.IsNullOrEmpty(node.Value))
            {
                var nodeClass = node.Bundle.LoadClass(node.Value);
                if(nodeClass != null)
                {
                    var nodeInstance = System.Activator.CreateInstance(nodeClass);
                    if (typeof(ICommand).IsAssignableFrom(nodeClass))
                    {
                        (nodeInstance as ICommand).Execute(sender);
                    }
                    else if (typeof(UserControl).IsAssignableFrom(nodeClass))
                    {
                        var uc = (nodeInstance as UserControl);
                        var dialog = new ModernWindow
                        {
                            Style = (Style)System.Windows.Application.Current.Resources["BlankWindow"],
                            Title = (sender as MenuItem).Header.ToString(),
                            Content = uc,
                            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
                        };
                        dialog.MinWidth = uc.Width;
                        dialog.MinHeight = uc.Height;
                        dialog.Width = uc.Width;
                        dialog.Height = uc.Height;
                        dialog.Owner = this;
                        dialog.ShowDialog();
                    }
                }
                else
                {
                    FileLogUtility.Error(string.Format("Failed to load class '{0}' from the Bundle '{1}'.", node.Value, node.Bundle.SymbolicName));
                }
            }
        }

        private void OnCanShowSystemMenu(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnShowSystemMenu(object target, ExecutedRoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            button.ContextMenu = _systemContextMenu;
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.Placement = PlacementMode.Bottom;
            button.ContextMenu.IsOpen = true;
        }

        private void LoadSettings()
        {
            // 获取窗体状态
            var windowsStateValue = _configurationService.Get(BundleActivator.Bundle, "WindowState", WindowState.Maximized.ToString());
            WindowState winState;
            if (Enum.TryParse<WindowState>(windowsStateValue, out winState))
            {
                WindowState = winState;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }

            if(WindowState == WindowState.Normal) // 获取大小
            {
                Width = _configurationService.Get(BundleActivator.Bundle, "Width", 1366);
                Height = _configurationService.Get(BundleActivator.Bundle, "Height", 768);
            }

            // 获取颜色
            var color = _configurationService.Get(BundleActivator.Bundle, "Color", "#FF1BA1E2");
            try
            {
                AppearanceManager.Current.AccentColor = (Color)ColorConverter.ConvertFromString(color);
            }
            catch // 设置默认颜色
            {
                AppearanceManager.Current.AccentColor = (Color)ColorConverter.ConvertFromString("#FF1BA1E2");
                _configurationService.Set(BundleActivator.Bundle, "Color", "#FF1BA1E2");
            }
            
            // 获取字体大小
            var fontSize = _configurationService.Get(BundleActivator.Bundle, "FontSize", FirstFloor.ModernUI.Presentation.FontSize.Large.ToString());
            AppearanceManager.Current.FontSize = fontSize.Equals(FirstFloor.ModernUI.Presentation.FontSize.Large.ToString()) ? 
                FirstFloor.ModernUI.Presentation.FontSize.Large : FirstFloor.ModernUI.Presentation.FontSize.Small;

            // 获取皮肤
            var skin = _configurationService.Get(BundleActivator.Bundle, "Skin", "/FirstFloor.ModernUI,Version=1.0.6.0;component/Assets/ModernUI.Light.xaml");
            AppearanceManager.Current.ThemeSource = new Uri(skin, UriKind.RelativeOrAbsolute);
        }

        private void SaveSettings()
        {
            _configurationService.Set(BundleActivator.Bundle, "WindowState", WindowState.ToString());
            _configurationService.Set(BundleActivator.Bundle, "Width", Width);
            _configurationService.Set(BundleActivator.Bundle, "Height", Height);
        }

        private void ModernWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SaveSettings();
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Tag != null && Tag is ICommand)
            {
                return;
            }

            if (ModernDialog.ShowMessage(
                "你确定是否退出系统？",
                "退出系统",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
            }
        }
    }
}
