using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using UIShell.RbacPermissionService.Model;
using UIShell.RbacPermissionService.DataAccess;
using System.Collections.Specialized;
using UIShell.PermissionService;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UIShell.WpfShellPlugin.Utility;

namespace UIShell.RbacManagementPlugin.ViewModels
{
    /// <summary>
    /// 角色管理视图模型。由角色列表、当前角色及当前角色的权限树视图模型来定义。
    /// </summary>
    public class RoleManagementViewModel : ListViewModelBase
    {
        private RoleDataAccessor _roleDataAccessor = new RoleDataAccessor();

        public RoleManagementViewModel()
        {
            RoleViewModels = new ObservableCollection<RoleViewModel>();
        }

        private RelayCommand _loadRolesCommand;

        /// <summary>
        /// 获取 LoadRolesCommand 属性。
        /// </summary>
        public RelayCommand LoadRolesCommand
        {
            get
            {
                return _loadRolesCommand
                    ?? (_loadRolesCommand = new RelayCommand(ExecuteLoadRolesCommand));
            }
        }

        /// <summary>
        /// 属性 <see cref="TotalsMessage" /> 的名称。
        /// </summary>
        public const string TotalsMessagePropertyName = "TotalsMessage";

        private string _totalsMessage;

        /// <summary>
        /// 直接更改 TotalsMessage 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public string TotalsMessage
        {
            get
            {
                return _totalsMessage;
            }
            set
            {
                Set(() => TotalsMessage, ref _totalsMessage, value);
            }
        }

        /// <summary>
        /// 角色角色视图列表。
        /// </summary>
        private void ExecuteLoadRolesCommand()
        {
            // 使用AutoMapper实现实体间的映射
            Mapper.CreateMap<RoleViewModel, Role>();
            Mapper.CreateMap<Role, RoleViewModel>();

            // 获取所有Roles。
            var roles = _roleDataAccessor.GetAll();
            roles.ForEach(role => 
                {
                    // 忽略系统管理员角色
                    if(role.IsAdminRole())
                    {
                        return;
                    }
                    // 为每一个Role创建一个RoleViewModel，用于界面绑定显示
                    var roleViewModel = new RoleViewModel();

                    Mapper.Map<Role, RoleViewModel>(role, roleViewModel);

                    RoleViewModels.Add(roleViewModel);

                    // 跟踪角色视图的属性变更，一旦发生变更则同步数据库
                    roleViewModel.BeginSyncToDatabase();
                }
                );

            if(RoleViewModels.Count > 0) // 默认选中第一个
            {
                CurrentRoleViewModel = RoleViewModels[0];
            }

            // 监听列表集合事件，并同步数据库
            RoleViewModels.CollectionChanged += RoleViewModelsCollectionChanged;

            UpdateTotals(RoleViewModels.Count);
        }

        private void UpdateTotals(int totals)
        {
            TotalsMessage = string.Format("角色数：{0}", totals);
        }

        private void RoleViewModelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add) // 新增一个角色视图，为其动态创建一个角色实体
            {
                foreach(RoleViewModel roleViewModel in e.NewItems)
                {
                    CurrentRoleViewModel = roleViewModel;
                    _roleDataAccessor.Create(Mapper.Map<Role>(roleViewModel));
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove) // 删除一个角色视图，将角色从数据库删除
            {
                foreach (RoleViewModel roleViewModel in e.OldItems)
                {
                    if(e.OldStartingIndex > 0) // 默认选中前一个或者第一个
                    {
                        CurrentRoleViewModel = RoleViewModels[e.OldStartingIndex - 1];
                    }
                    else if (RoleViewModels.Count > 0)
                    {
                        CurrentRoleViewModel = RoleViewModels[0];
                    }
                    _roleDataAccessor.Delete(roleViewModel.Id); // 同步数据库
                }
            }

            UpdateTotals(RoleViewModels.Count);
        }

        /// <summary>
        /// 属性 <see cref="CurrentRoleViewModel" /> 的名称。
        /// </summary>
        public const string CurrentViewModelPropertyName = "CurrentRoleViewModel";

        private RoleViewModel _currentRoleViewModel = null;

        /// <summary>
        /// 直接更改 CurrentRoleViewModel 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public RoleViewModel CurrentRoleViewModel
        {
            get
            {
                return _currentRoleViewModel;
            }
            set
            {
                if(Set(() => CurrentRoleViewModel, ref _currentRoleViewModel, value))
                {
                    if (CurrentRoleViewModel != null)
                    {
                        IsTreeViewEnabled = !CurrentRoleViewModel.Id.Equals(Role.AdminRoleId);
                    }

                    _factory.SetCurrentRole(_currentRoleViewModel != null ? _currentRoleViewModel.Id : Guid.Empty);
                    PermissionViewModels = _factory.PermissionTreeViewModels;
                }
            }
        }

        /// <summary>
        /// 属性 <see cref="PermissionViewModels" /> 的名称。
        /// </summary>
        public const string PermissionViewModelsPropertyName = "PermissionViewModels";

        private static RolePermissionTreeViewModelFactory _factory = new RolePermissionTreeViewModelFactory(Guid.Empty);
        private ObservableCollection<PermissionTreeViewModel> _permissionTreeViewModels = _factory.PermissionTreeViewModels;

        /// <summary>
        /// 直接更改 PermissionViewModels 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public ObservableCollection<PermissionTreeViewModel> PermissionViewModels
        {
            get
            {
                return _permissionTreeViewModels;
            }
            set
            {
                Set(() => PermissionViewModels, ref _permissionTreeViewModels, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="IsTreeViewEnabled" /> 的名称。
        /// </summary>
        public const string IsTreeViewEnabledPropertyName = "IsTreeViewEnabled";

        private bool _isTreeViewEnabled;

        /// <summary>
        /// 直接更改 IsTreeViewEnabled 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public bool IsTreeViewEnabled
        {
            get
            {
                return _isTreeViewEnabled;
            }
            set
            {
                Set(() => IsTreeViewEnabled, ref _isTreeViewEnabled, value);
            }
        }

        public ObservableCollection<RoleViewModel> RoleViewModels { get; set; }

        public override void Add()
        {
            // 添加角色模型，并开始同步数据库
            var roleViewModel = new RoleViewModel() { RoleTextBlockVisibility = Visibility.Hidden, RoleTextBoxVisibility = Visibility.Visible, Name = _roleDataAccessor.GetUniqueRoleName("新建角色") };
            RoleViewModels.Add(roleViewModel);
            roleViewModel.BeginSyncToDatabase();
        }

        private RelayCommand<TextBox> _roleTextBoxLostFocusCommand;

        public RelayCommand<TextBox> RoleTextBoxLostFocusCommand
        {
            get
            {
                return _roleTextBoxLostFocusCommand
                    ?? (_roleTextBoxLostFocusCommand = new RelayCommand<TextBox>(ExecuteRoleTextBoxLostFocusCommand));
            }
        }

        private void ExecuteRoleTextBoxLostFocusCommand(TextBox roleTextBox)
        {
            if(roleTextBox == null)
            {
                return;
            }
            // 一旦FocusLost，则角色变为不可编辑
            var roleViewModel = roleTextBox.Tag as RoleViewModel;
            roleViewModel.RoleTextBoxVisibility = System.Windows.Visibility.Hidden;
            roleViewModel.RoleTextBlockVisibility = System.Windows.Visibility.Visible;
        }

        public override void Edit()
        {
            if (CurrentRoleViewModel == null)
            {
                return;
            }
            // 设置当前角色为可编辑状态
            CurrentRoleViewModel.RoleTextBlockVisibility = Visibility.Hidden;
            CurrentRoleViewModel.RoleTextBoxVisibility = Visibility.Visible;
        }

        public override void Delete()
        {
            if (CurrentRoleViewModel == null)
            {
                return;
            }

            if(ModernDialog.ShowMessage("是否要删除当前角色？", "操作警告", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            if (CurrentRoleViewModel.Id.Equals(Role.AdminRoleId))
            {
                ModernDialog.ShowMessage("系统默认管理员角色不能被删除。",
                    "角色管理",
                    MessageBoxButton.OK);
                return;
            }

            if (_roleDataAccessor.HasAssociatedUsers(CurrentRoleViewModel.Id))
            {
                ModernDialog.ShowMessage("当前角色已经分配到用户，无法被删除。",
                    "角色管理",
                    MessageBoxButton.OK);
                return;
            }

            // 停止同步数据库
            CurrentRoleViewModel.StopSyncToDatabase();
            // 删除角色视图模型
            RoleViewModels.Remove(CurrentRoleViewModel);
        }
    }

    public class PermissionTreeViewItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// 选择权限树节点模板。
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            DataTemplate template = null;
            var permissionTreeViewModel = item as PermissionTreeViewModel;
            if (permissionTreeViewModel.Tag == null || permissionTreeViewModel.Tag is PermissionGroupData)
            {
                // PermissionGroup模板
                template = element.FindResource("PermissionGroupTemplate") as HierarchicalDataTemplate;
            }
            else if (permissionTreeViewModel.Tag is PermissionData)
            {
                // Permission模板
                template = element.FindResource("PermissionTemplate") as DataTemplate;
            }
            return template;
        }
    }
}
