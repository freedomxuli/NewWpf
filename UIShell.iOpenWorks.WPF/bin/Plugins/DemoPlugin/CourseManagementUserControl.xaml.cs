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
using DemoPlugin.ViewModels;

namespace DemoPlugin
{
    /// <summary>
    /// XDataGridSample.xaml 的交互逻辑
    /// </summary>
    public partial class CourseManagementUserControl : UserControl
    {
        public CourseManagementUserControl()
        {
            InitializeComponent();
        }
    }

    public class TotalsBindingConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("数量：{0}", (int)value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
