using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Globalization;
using UIShell.OSGi.Utility;
using UIShell.ConfigurationService;

namespace UIShell.WpfShellPlugin.Content
{
    /// <summary>
    /// 皮肤与外观视图模型。
    /// </summary>
    public class SettingsAppearanceViewModel
        : NotifyPropertyChanged
    {
        // 字体文本
        private readonly string FontSmall;
        private readonly string FontLarge;

        // 20 个可换的颜色
        private Color[] _accentColors = new Color[]{
            Color.FromRgb(0xa4, 0xc4, 0x00),   // lime
            Color.FromRgb(0x60, 0xa9, 0x17),   // green
            Color.FromRgb(0x00, 0x8a, 0x00),   // emerald
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x1b, 0xa1, 0xe2),   // cyan
            Color.FromRgb(0x00, 0x50, 0xef),   // cobalt
            Color.FromRgb(0x6a, 0x00, 0xff),   // indigo
            Color.FromRgb(0xaa, 0x00, 0xff),   // violet
            Color.FromRgb(0xf4, 0x72, 0xd0),   // pink
            Color.FromRgb(0xd8, 0x00, 0x73),   // magenta
            Color.FromRgb(0xa2, 0x00, 0x25),   // crimson
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xfa, 0x68, 0x00),   // orange
            Color.FromRgb(0xf0, 0xa3, 0x0a),   // amber
            Color.FromRgb(0xe3, 0xc8, 0x00),   // yellow
            Color.FromRgb(0x82, 0x5a, 0x2c),   // brown
            Color.FromRgb(0x6d, 0x87, 0x64),   // olive
            Color.FromRgb(0x64, 0x76, 0x87),   // steel
            Color.FromRgb(0x76, 0x60, 0x8a),   // mauve
            Color.FromRgb(0x87, 0x79, 0x4e),   // taupe
        };

        // 选择的外观颜色
        private Color _selectedAccentColor;

        // 皮肤列表
        private LinkCollection _themes = new LinkCollection();

        // 选择的皮肤
        private Link _selectedTheme;

        // 选择的字体大小
        private string _selectedFontSize;

        private IConfigurationService _configurationService;

        public SettingsAppearanceViewModel()
        {
            _configurationService = BundleActivator.ConfigurationServiceTracker.DefaultOrFirstService;
            AssertUtility.NotNull(_configurationService);

            FontSmall = "小";
            FontLarge = "大";

            // 默认皮肤
            this._themes.Add(new Link { DisplayName = "深色", Source = AppearanceManager.DarkThemeSource });
            this._themes.Add(new Link { DisplayName = "浅色", Source = AppearanceManager.LightThemeSource });

            // 其它皮肤
            this._themes.Add(new Link { DisplayName = "Hello Kitty", Source = new Uri("/UIShell.WpfShellPlugin,Version=1.0.0.0;component/Assets/ModernUI.HelloKitty.xaml", UriKind.Relative) });
            this._themes.Add(new Link { DisplayName = "心型", Source = new Uri("/UIShell.WpfShellPlugin,Version=1.0.0.0;component/Assets/ModernUI.Love.xaml", UriKind.Relative) });
            this._themes.Add(new Link { DisplayName = "雪花", Source = new Uri("/UIShell.WpfShellPlugin,Version=1.0.0.0;component/Assets/ModernUI.Snowflakes.xaml", UriKind.Relative) });

            // 选择的字体和皮肤
            this.SelectedFontSize = AppearanceManager.Current.FontSize == FontSize.Large ? FontLarge : FontSmall;
            SyncThemeAndColor();
            // 外观变更处理
            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }

        private void SyncThemeAndColor()
        {
            // synchronizes the selected viewmodel theme with the actual theme used by the appearance manager.
            this.SelectedTheme = this._themes.FirstOrDefault(l => l.Source.Equals(AppearanceManager.Current.ThemeSource));

            // and make sure accent color is up-to-date
            this.SelectedAccentColor = AppearanceManager.Current.AccentColor;
        }

        private void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor") {
                SyncThemeAndColor();
            }
        }

        public LinkCollection Themes
        {
            get { return this._themes; }
        }

        public string[] FontSizes
        {
            get { return new string[] { FontSmall, FontLarge }; }
        }

        public Color[] AccentColors
        {
            get { return this._accentColors; }
        }

        public Link SelectedTheme
        {
            get { return this._selectedTheme; }
            set
            {
                if (this._selectedTheme != value) {
                    this._selectedTheme = value;
                    OnPropertyChanged("SelectedTheme");

                    // and update the actual theme
                    AppearanceManager.Current.ThemeSource = value.Source;

                    _configurationService.Set(BundleActivator.Bundle, "Skin", AppearanceManager.Current.ThemeSource.ToString());
                }
            }
        }

        public string SelectedFontSize
        {
            get { return this._selectedFontSize; }
            set
            {
                if (this._selectedFontSize != value) {
                    this._selectedFontSize = value;
                    OnPropertyChanged("SelectedFontSize");

                    AppearanceManager.Current.FontSize = value == FontLarge ? FontSize.Large : FontSize.Small;

                    _configurationService.Set(BundleActivator.Bundle, "FontSize", AppearanceManager.Current.FontSize.ToString());
                }
            }
        }

        public Color SelectedAccentColor
        {
            get { return this._selectedAccentColor; }
            set
            {
                if (this._selectedAccentColor != value) {
                    this._selectedAccentColor = value;
                    OnPropertyChanged("SelectedAccentColor");

                    AppearanceManager.Current.AccentColor = value;

                    _configurationService.Set(BundleActivator.Bundle, "Color", AppearanceManager.Current.AccentColor.ToString());
                }
            }
        }
    }
}
