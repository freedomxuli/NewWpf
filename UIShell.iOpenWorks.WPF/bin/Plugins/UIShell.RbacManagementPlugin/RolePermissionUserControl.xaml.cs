using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UIShell.ConfigurationService;
using UIShell.OSGi.Utility;
using UIShell.RbacManagementPlugin.ViewModels;

namespace UIShell.RbacManagementPlugin
{
    /// <summary>
    /// Interaction logic for RolePermissionUserControl.xaml
    /// </summary>
    public partial class RolePermissionUserControl : UserControl
    {
        private IConfigurationService ConfigurationService { get; set; }
        public RolePermissionUserControl()
        {
            ConfigurationService = Activator.ConfigurationService;
            AssertUtility.NotNull(ConfigurationService, "ConfigurationService is not registered.");

            InitializeComponent();

            PermissionTreeViewColumn.Width = new GridLength(ConfigurationService.Get(Activator.Bundle, "PermissionTreeColumnWidth", 320));
        }

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if(textBox.Visibility == System.Windows.Visibility.Visible)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Visibility == System.Windows.Visibility.Visible)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                RoleListView.Focus();
            }
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            ConfigurationService.Set(Activator.Bundle, "PermissionTreeColumnWidth", PermissionTreeViewColumn.Width.Value);
        }
    }
}
