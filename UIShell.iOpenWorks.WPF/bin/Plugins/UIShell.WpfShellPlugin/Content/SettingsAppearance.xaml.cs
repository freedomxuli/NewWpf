using FirstFloor.ModernUI.Presentation;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIShell.WpfShellPlugin.Content
{
    /// <summary>
    /// Interaction logic for SettingsAppearance.xaml
    /// </summary>
    public partial class SettingsAppearance : UserControl
    {
        public SettingsAppearance()
        {
            InitializeComponent();

            // a simple view model for appearance configuration
            this.DataContext = new SettingsAppearanceViewModel();
            var confirm = BundleActivator.ConfigurationServiceTracker.DefaultOrFirstService.Get(BundleActivator.Bundle, "ConfirmClosingWindow", true);
            ConfirmCloseCheckBox.IsChecked = confirm;
        }

        private void ConfirmCloseCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool confirm = ConfirmCloseCheckBox.IsChecked == true;
            BundleActivator.ConfigurationServiceTracker.DefaultOrFirstService.Set(BundleActivator.Bundle, "ConfirmClosingWindow", confirm);
        }
    }
}
