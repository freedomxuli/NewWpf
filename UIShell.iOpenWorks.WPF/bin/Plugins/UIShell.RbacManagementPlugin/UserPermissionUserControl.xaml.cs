using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UIShell.RbacManagementPlugin.ViewModels;

namespace UIShell.RbacManagementPlugin
{
    /// <summary>
    /// Interaction logic for UserPermissionUserControl.xaml
    /// </summary>
    public partial class UserPermissionUserControl : UserControl
    {
        public UserPermissionUserControl()
        {
            InitializeComponent();
            PermissionTreeViewColumn.Width = new GridLength(Activator.ConfigurationService.Get(Activator.Bundle, "UserPermissionTreeViewColumnWidth", 320));
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Activator.ConfigurationService.Set(Activator.Bundle, "UserPermissionTreeViewColumnWidth", PermissionTreeViewColumn.Width.Value);
        }
    }

    public class TotalsBindingConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("用户数：{0}", (int)value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
