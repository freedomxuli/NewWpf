using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using FirstFloor.ModernUI.Windows.Controls;
using UIShell.ConfigurationService;
using UIShell.NavigationService;
using UIShell.OSGi;
using UIShell.OSGi.Collection;
using UIShell.OSGi.Utility;
using UIShell.WpfShellPlugin.Utility;

namespace UIShell.WpfShellPlugin.Pages
{
    /// <summary>
    /// 扩展格式界面布局。
    /// </summary>
    public partial class Layout : UserControl, IMainWindowService
    {
        /// <summary>
        /// 导航服务实例。
        /// </summary>
        private INavigationService NavigationService { get; set; }
        private IConfigurationService ConfigurationService { get; set; }
        /// <summary>
        /// 顶层NavigationNode与TreeViewItem的对应关系。
        /// </summary>
        private List<System.Tuple<NavigationNode, TreeViewItem>> TopTreeViewItemNavigationNodeTuples { get; set; }
        /// <summary>
        /// 换肤的TreeViewItem，该Item是由界面框架提供的，需要手动注册。
        /// </summary>
        private TreeViewItem SkinTreeViewItem { get; set; }

        /// <summary>
        /// 保存导航扩展结点与用户控件的关系，避免再次加载已经打开的控件。
        /// </summary>
        private Dictionary<string, UserControl> OpenedPagesCache { get; set; }
        /// <summary>
        /// 用户控件缓存不能从父容器中删除，因此为了达到缓存目的，在切换页面时，必须将前一个页面的Visibility设置Hidden，当前页面设置
        /// 为Visible。此外，它用于保存显示队列，最后一个永远是最近刚打开且是可见的。
        /// </summary>
        private List<UserControl> ContentQueue = new List<UserControl>();

        /// <summary>
        /// 换肤界面，由界面框架提供，手动创建。
        /// </summary>
        private UserControl SkinUserControl;
        private UserControl WelcomeControl;
        private System.Timers.Timer Timer { get; set; }
        public Layout()
        {
            var mainWindowService = BundleActivator.BundleContext.GetFirstOrDefaultService<IMainWindowService>();
            if (mainWindowService != null)
            {
                BundleActivator.BundleContext.RemoveService<IMainWindowService>(mainWindowService);
            }

            InitializeComponent();

            TopTreeViewItemNavigationNodeTuples = new List<System.Tuple<NavigationNode, TreeViewItem>>();
            NavigationService = BundleActivator.NavigationServiceTracker.DefaultOrFirstService;
            ConfigurationService = BundleActivator.ConfigurationServiceTracker.DefaultOrFirstService;
            OpenedPagesCache = new Dictionary<string, UserControl>();

            AssertUtility.NotNull(NavigationService, "NavigationService is not registered.");
            AssertUtility.NotNull(ConfigurationService, "ConfigurationService is not registered.");
            
            // 将导航扩展模型转换成TreeView。
            ResetNavigation();
            // 监听扩展模型变更事件，并动态更新树型导航。
            NavigationService.NavigationChanged += NavigationChanged;
            // 注册导航栏服务。
            BundleActivator.BundleContext.AddService<IMainWindowService>(this);
            
            TreeViewColumn.Width = new GridLength(ConfigurationService.Get(BundleActivator.Bundle, "NavigationTreeViewColumnWidth", 200.0));

            SetNow();
            Timer = new System.Timers.Timer(1000);
            Timer.Elapsed += (sender, e) => SetNow();
            Timer.Start();

            ShowContent(WelcomeControl);
        }

        private void SetNow()
        {
            Action action = ()=> 
            {
                TimeNowTextBlock.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            };
            Dispatcher.Invoke(action);
        }

        private void NavigationChanged(object sender, NavigationChangedEventArgs e)
        {
            ResetNavigation();
        }

        private void ResetNavigation()
        {
            if (BundleRuntime.Instance.State == BundleRuntimeState.Stopping)
            {
                return;
            }

            Action resetNavigation = () =>
            { // 重新初始化TreeView

                SkinUserControl = new Settings();
                WelcomeControl = new Introduction();

                LayoutDockPanel.Children.Clear();
                ContentQueue.Clear();
                OpenedPagesCache.Clear();
                HideSidebar();
                SideBars.Clear();
                SideBarSettings.Clear();
                TreeViewItemSelectionQueue.Clear();
                TopTreeViewItemNavigationNodeTuples.Clear();
                NavigationTreeView.Items.Clear();

                InitializeNavigationTreeView();

                if(BundleActivator.PermissionServiceTracker.IsServiceAvailable)
                {
                    SetCurrentUser(BundleActivator.PermissionServiceTracker.DefaultOrFirstService.CurrentUserName);
                }

                ShowContent(WelcomeControl);
            };
            Dispatcher.Invoke(resetNavigation); // 由界面代理线程来执行UI更新。
        }

        private TreeViewItem CreateTreeViewItem(string text, string icon, string tooltip, NavigationNode node)
        {
            TreeViewItem child = new TreeViewItem();
            child.IsExpanded = true;
            child.Tag = node;
            DockPanel pan = new DockPanel();

            if(!string.IsNullOrEmpty(tooltip))
            {
                child.ToolTip = tooltip;
            }

            if (string.IsNullOrEmpty(icon))
            {
                icon = "/UIShell.WpfShellPlugin;component/Assets/DefaultNode.png";
            }

            if(!string.IsNullOrEmpty(icon))
            {
                var uri = new Uri(icon, UriKind.RelativeOrAbsolute);
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = uri;
                logo.EndInit();
                var image = new Image();
                image.Source = logo;
                image.Height = 16;
                image.Width = 16;
                image.Margin = new Thickness(0, 0, 4, 0);
                pan.Children.Add(image);
            }

            pan.Children.Add(new TextBlock(new Run(text)));
            child.Header = pan;
            return child;
        }

        private void InitializeNavigationTreeView()
        {
            // 遍历所有导航节点扩展模型对象。
            foreach(var node in NavigationService.NavigationNodes)
            {
                // TODO: 如果没有权限则隐藏 => 没有权限则Disable
                //if (!node.HasPermission())
                //{
                //    continue;
                //}
                // 为导航节点创建相应的树节点
                var treeViewItem = CreateTreeViewItem(node);
                treeViewItem.IsEnabled = node.HasPermission();
                TopTreeViewItemNavigationNodeTuples.Add(new System.Tuple<NavigationNode, TreeViewItem>(node, treeViewItem));
                // 创建子节点
                InitializeNavigationNode(node, treeViewItem);
                // 添加到树
                NavigationTreeView.Items.Add(treeViewItem);
            }
            // 添加换肤项
            SkinTreeViewItem = CreateTreeViewItem("系统设置",
                "/UIShell.WpfShellPlugin;component/Assets/Settings.png", 
                string.Empty, 
                null);
            NavigationTreeView.Items.Add(SkinTreeViewItem);
        }

        /// <summary>
        /// 递归初始化导航扩展子节点。
        /// </summary>
        /// <param name="node">导航节点扩展模型。</param>
        /// <param name="treeViewItem">树节点。</param>
        private void InitializeNavigationNode(NavigationNode node, TreeViewItem treeViewItem)
        {
            // 遍历所有导航子节点
            foreach(var child in node.Children) 
            {
                // TODO: 如果没有权限则隐藏 => 没有权限则Disable
                //if(!child.HasPermission()) // 判断是否有权限，如果没有权限则忽略
                //{
                //    continue;
                //}

                // 创建子节点
                var childTreeViewItem = CreateTreeViewItem(child);
                childTreeViewItem.IsEnabled = child.HasPermission();
                treeViewItem.Items.Add(childTreeViewItem);
                // 递归子节点的子节点
                InitializeNavigationNode(child, childTreeViewItem);
            }
        }

        /// <summary>
        /// 将导航节点扩展模型对象转换成树节点。
        /// </summary>
        /// <param name="node">导航节点扩展模型对象。</param>
        /// <returns>树节点。</returns>
        private TreeViewItem CreateTreeViewItem(NavigationNode node)
        {
            if(node == null)
            {
                return null;
            }
            return CreateTreeViewItem(node.Name, node.Icon, node.ToolTip, node);
        }

        /// <summary>
        /// 显示用户控件，并隐藏前一个显示的用户控件。因此，必须确保当前显示的用户控件是在ContentQueue的最后一个。
        /// </summary>
        /// <param name="content">当前要呈现的用户控件。</param>
        private void ShowContent(UserControl content)
        {
            content.Loaded += frameworkElement_Loaded;

            if(ContentQueue.Count > 0) // 隐藏前一个，显示当前控件
            {
                ContentQueue[ContentQueue.Count - 1].Visibility = System.Windows.Visibility.Hidden;
                content.Visibility = System.Windows.Visibility.Visible;
            }

            if(ContentQueue.Contains(content)) // 如果包含当前控件，则将其移到最后一个
            {
                ContentQueue.Remove(content);
                ContentQueue.Add(content);
            }
            else
            {
                // 如果没有包含当前控件，则直接添加到最后一个并显示
                ContentQueue.Add(content);
                LayoutDockPanel.Children.Add(content);
            }

            if(SideBars.ContainsKey(content))
            {
                var control = SideBars[content];
                var settings = SideBarSettings[control];
                ShowSidebar(control, settings.Item1, settings.Item2);
            }
        }

        void frameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            frameworkElement.Loaded -= frameworkElement_Loaded;

            frameworkElement.IsVisibleChanged += frameworkElement_IsVisibleChanged;
            frameworkElement.IsEnabledChanged += frameworkElement_IsEnabledChanged;

            SwitchInputBindings(frameworkElement);
        }

        void frameworkElement_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            if (!frameworkElement.IsEnabled)
            {
                var window = Window.GetWindow(frameworkElement);
                if (window == null)
                {
                    return;
                }

                window.InputBindings.Clear();
            }
            else
            {
                SwitchInputBindings(frameworkElement);
            }
        }

        void frameworkElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            if(!frameworkElement.IsVisible)
            {
                return;
            }

            SwitchInputBindings(frameworkElement);
        }

        private void SwitchInputBindings(FrameworkElement frameworkElement)
        {
            var window = Window.GetWindow(frameworkElement);
            if (window == null)
            {
                return;
            }

            window.InputBindings.Clear();

            for (int i = frameworkElement.InputBindings.Count - 1; i >= 0; i--)
            {
                var inputBinding = (InputBinding)frameworkElement.InputBindings[i];
                window.InputBindings.Add(inputBinding);
            }
        }

        /// <summary>
        /// 关闭当前用户控件。
        /// </summary>
        public void CloseCurrentContent()
        {
            if(ContentQueue.Count < 2)
            {
                return;
            }

            bool confirm = ConfigurationService.Get(BundleActivator.Bundle, "ConfirmClosingWindow", true);

            if (confirm && ModernDialog.ShowMessage(
                "你确实是否要关闭当前窗口？",
                "关闭窗口",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            HideSidebar();

            // 删除当前显示的控件，即最后一个
            var content = ContentQueue[ContentQueue.Count - 1];
            ContentQueue.Remove(content);
            // 删除缓存
            var key = string.Empty;
            foreach(var pair in OpenedPagesCache)
            {
                if(pair.Value.Equals(content))
                {
                    key = pair.Key;
                }
            }
            if(!string.IsNullOrEmpty(key))
            {
                OpenedPagesCache.Remove(key);
            }
            
            // 从容器中删除该控件
            LayoutDockPanel.Children.Remove(content);
            // 显示倒数第二个用户控件
            ShowContent(ContentQueue[ContentQueue.Count - 1]);

            if(TreeViewItemSelectionQueue.Count > 0) // 设置当前节点选中状态为false，并且删除其选中队列
            {
                (TreeViewItemSelectionQueue[TreeViewItemSelectionQueue.Count - 1] as TreeViewItem).IsSelected = false;
                TreeViewItemSelectionQueue.RemoveAt(TreeViewItemSelectionQueue.Count - 1);
            }
            if(TreeViewItemSelectionQueue.Count > 0) // 选中前一个节点
            {
                (TreeViewItemSelectionQueue[TreeViewItemSelectionQueue.Count - 1] as TreeViewItem).IsSelected = true;
            }
        }

        private List<object> TreeViewItemSelectionQueue = new List<object>();

        /// <summary>
        /// 树点击事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeViewItem = e.NewValue;

            if (treeViewItem == null || !(treeViewItem is TreeViewItem))
            {
                return;
            }

            HideSidebar();

            // 跟踪树形节点选中队列，保持刚选中的项为最后一项
            TreeViewItemSelectionQueue.Remove(treeViewItem);
            TreeViewItemSelectionQueue.Add(treeViewItem);
            // 如果是换肤节点，则显示换肤控件
            if (treeViewItem == SkinTreeViewItem)
            {
                ShowContent(SkinUserControl);
                return;
            }

            var node = (treeViewItem as TreeViewItem).Tag as NavigationNode;
            if (!string.IsNullOrEmpty(node.Value))
            {
                if (OpenedPagesCache.ContainsKey(node.Id))
                {
                    ShowContent(OpenedPagesCache[node.Id]);
                    return;
                }

                LoadingTextBlock.Visibility = System.Windows.Visibility.Visible;
                LayoutDockPanel.Visibility = System.Windows.Visibility.Hidden;

                ThreadPool.QueueUserWorkItem(state =>
                {
                    Action action = () =>
                    {
                        bool cached = false;
                        // 缓存中是否存在当前打开的页面，如果有，直接显示


                        // 获取该树节点的Tag属性，其值为导航节点

                        // 加载导航节点定义的用户控件的类型，这个类型由插件提供。
                        // 因此，必须使用插件来加载类型。
                        if (!cached)
                        {
                            var contentClass = node.Bundle.LoadClass(node.Value);
                            if (contentClass != null)
                            {
                                try
                                {
                                    // 创建实例，并转换成用户控件。
                                    var component = System.Activator.CreateInstance(contentClass);
                                    var userControl = component as UserControl;
                                    if (userControl == null)
                                    {
                                        FileLogUtility.Error(string.Format("The type '{0}' in the Bundle '{1}' does not inherit from UIElement.", node.Value, node.Bundle.SymbolicName));
                                        return;
                                    }

                                    if(userControl.DataContext != null && userControl.DataContext is ListViewModelBase)
                                    {
                                        userControl.InputBindings.AddRange((userControl.DataContext as ListViewModelBase).InputKeyBindings);
                                    }

                                    // 添加到缓存
                                    OpenedPagesCache[node.Id] = userControl;

                                    // 将动态创建控件添加到显示区域
                                    ShowContent(userControl);
                                }
                                catch (Exception ex)
                                {
                                    FileLogUtility.Error(string.Format("Failed to create UIElement for the type '{0}' in the Bundle '{1}'.", node.Value, node.Bundle.SymbolicName));
                                    FileLogUtility.Error(ex);
                                }
                            }
                            else
                            {
                                FileLogUtility.Error(string.Format("Failed to load class '{0}' from the Bundle '{1}'.", node.Value, node.Bundle.SymbolicName));
                            }
                        }

                        LoadingTextBlock.Visibility = System.Windows.Visibility.Hidden;
                        LayoutDockPanel.Visibility = System.Windows.Visibility.Visible;
                    };

                    DispatcherOperation op = Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        action);

                    DispatcherOperationStatus status = op.Status;
                    while (status != DispatcherOperationStatus.Completed)
                    {
                        status = op.Wait(TimeSpan.FromMilliseconds(1000));
                        if (status == DispatcherOperationStatus.Aborted)
                        {
                            // Alert Someone
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 设置消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public void SetMessage(string message)
        {
            // 必须由UI线程来处理界面显示
            Action action = () => { MessageTextBlock.Text = message; };
            Dispatcher.Invoke(action);
        }

        /// <summary>
        /// 设置当前用户。
        /// </summary>
        /// <param name="user">当前用户信息。</param>
        public void SetCurrentUser(string user)
        {
            // 必须由UI线程来处理界面显示
            Action action = () => { CurrentUserTextBlock.Text = string.Format("当前用户：{0}", user); };
            Dispatcher.Invoke(action);
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            ConfigurationService.Set(BundleActivator.Bundle, "NavigationTreeViewColumnWidth", TreeViewColumn.Width.Value);
        }

        public void ExitApplication()
        {
            Application.Current.MainWindow.Close();
        }

        public Dictionary<UserControl, FrameworkElement> SideBars = new Dictionary<UserControl, FrameworkElement>();
        public Dictionary<FrameworkElement, System.Tuple<double, string>> SideBarSettings = new Dictionary<FrameworkElement, System.Tuple<double, string>>();

        /// <summary>
        /// 显示侧边框。
        /// </summary>
        /// <param name="control">显示的元素。</param>
        /// <param name="width">长度。</param>
        public void ShowSidebar(FrameworkElement control, double width, string title)
        {
            SideBarTitleTextBlock.Text = title;

            LayoutDockPanel.IsEnabled = false;
            SideBarDockPanelContent.Children.Add(control);
            SideBarDockPanel.Width = 0;
            SideBarDockPanel.Visibility = Visibility.Visible;

            DoubleAnimation animation = new DoubleAnimation(width, TimeSpan.FromSeconds(0.3));
            SideBarDockPanel.BeginAnimation(Rectangle.WidthProperty, animation);

            if (!SideBars.ContainsValue(control))
            {
                SideBars.Add(ContentQueue[ContentQueue.Count - 1], control);
            }
            if(!SideBarSettings.ContainsKey(control))
            {
                SideBarSettings.Add(control, new System.Tuple<double, string>(width, title));
            }
        }

        /// <summary>
        /// 隐藏侧边框。
        /// </summary>
        public void HideSidebar(FrameworkElement control)
        {
            if (SideBars.ContainsValue(control))
            {
                var pair = SideBars.Single(p => p.Value.Equals(control));
                SideBars.Remove(pair.Key);
            }

            if (SideBarSettings.ContainsKey(control))
            {
                SideBarSettings.Remove(control);
            }

            HideSidebar();
        }

        private void HideSidebar()
        {
            LayoutDockPanel.IsEnabled = true;
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.3));
            animation.Completed += (sender, e) =>
            {
                SideBarDockPanelContent.Children.Clear();
            };

            SideBarDockPanel.BeginAnimation(Rectangle.WidthProperty, animation);
        }
    }
}
