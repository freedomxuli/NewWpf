using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using UIShell.OSGi;
using UIShell.PermissionService;
using UIShell.RbacPermissionService.DataAccess;
using UIShell.RbacPermissionService.Model;

namespace UIShell.RbacManagementPlugin.ViewModels
{
    /// <summary>
    /// 权限树工厂，每一个角色都有一个不同的权限树。权限树，根据权限扩展模型动态构建。
    /// </summary>
    public abstract class PermissionTreeViewModelFactory
    {
        /// <summary>
        /// 权限扩展模型。
        /// </summary>
        private IPermissionExtensionModelService ExtensionModelService { get; set; }
        /// <summary>
        /// 当前UI Dispatcher。
        /// </summary>
        private Dispatcher _currentDispatcher;

        /// <summary>
        /// 权限树集合。只有一个节点。集合是用于绑定。
        /// </summary>
        public ObservableCollection<PermissionTreeViewModel> PermissionTreeViewModels { get; set; }

        public PermissionTreeViewModelFactory()
        {
            PermissionTreeViewModels = new ObservableCollection<PermissionTreeViewModel>();
            ExtensionModelService = Activator.PermissionExtensionModelService;

            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        protected void InitializeExtension()
        {
            if(ExtensionModelService == null)
            {
                return;
            }

            ExtensionModelService.PermissionChanged += _extensionModelService_PermissionChanged;
            
            // 初始化权限树视图模型。
            Initialize(CreateRoot());
        }

        private void _extensionModelService_PermissionChanged(object sender, PermissionModelEventArgs e)
        {
            if (BundleRuntime.Instance.State == BundleRuntimeState.Stopping)
            {
                return;
            }

            // 权限扩展变更，重新初始化权限树视图模型。
            var root = CreateRoot();
            Action init = () => Initialize(root);
            _currentDispatcher.Invoke(init);
        }

        /// <summary>
        /// 创建权限树视图根节点，根节点即“所有权限”。
        /// </summary>
        /// <returns></returns>
        private PermissionTreeViewModel CreateRoot()
        {
            var root = new PermissionTreeViewModel() { Text = "所有权限" };

            ExtensionModelService.PermissionGroups.ForEach(pair =>
            {
                // 创建二级视图节点，即插件
                var bundle = new PermissionTreeViewModel() { Text = pair.Key.Name, Parent = root };
                root.Children.Add(bundle);

                pair.Value.ForEach(group => // 创建子视图节点
                {
                    var groupPermissionTreeViewModel = new PermissionTreeViewModel() { Text = group.Name, Parent = bundle, Tag = group };
                    bundle.Children.Add(groupPermissionTreeViewModel);
                    Initialize(groupPermissionTreeViewModel, group);
                }
                    );
            });

            return root;
        }

        private void Initialize(PermissionTreeViewModel root) // 初始化权限树视图模型。
        {
            PermissionTreeViewModels.Clear(); // 清空原有节点

            PermissionTreeViewModels.Add(root); // 创建树

            InitializePermissionSelected(); // 根据当前角色，选择其拥有的权限
        }

        /// <summary>
        /// 递归创建权限树视图模型。
        /// </summary>
        /// <param name="groupTreeViewModel"></param>
        /// <param name="group"></param>
        private void Initialize(PermissionTreeViewModel groupTreeViewModel, PermissionGroupData group)
        {
            group.Children.ForEach(childGroup =>
            {
                // 如果是组，则需要递归初始化视图模型
                var childGroupTreeViewModel = new PermissionTreeViewModel() { Text = childGroup.Name, Parent = groupTreeViewModel, Tag = childGroup };
                groupTreeViewModel.Children.Add(childGroupTreeViewModel);

                Initialize(childGroupTreeViewModel, childGroup);
            });
            group.Permissions.ForEach(permission =>
            {
                // 如果是权限，则已经是叶子节点
                var permissionTreeViewModel = new PermissionTreeViewModel() { Text = permission.Name, Parent = groupTreeViewModel, Tag = permission };
                groupTreeViewModel.Children.Add(permissionTreeViewModel);
            });
        }

        protected abstract bool SelectAll();

        protected abstract bool UnselectAll();

        private volatile bool _initializePermissionSelected = false;
        protected void InitializePermissionSelected()
        {
            _initializePermissionSelected = true;
            // 如果当前角色为空，则默认不选择
            if (UnselectAll())
            {
                foreach (var permissionTreeViewModel in PermissionTreeViewModels)
                {
                    permissionTreeViewModel.IsSelected = false;
                }
            }
            // 如果是管理员角色，默认全选
            else if (SelectAll())
            {
                foreach (var permissionTreeViewModel in PermissionTreeViewModels)
                {
                    permissionTreeViewModel.IsSelected = true;
                }
            }
            else // 根据当前角色拥有的权限进行处理
            {
                // 获取当前角色的权限
                // 将递归转换成非递归，来设置子节点的选中情况
                var permissionViewModels = new List<PermissionTreeViewModel>();
                permissionViewModels.AddRange(PermissionTreeViewModels.ToArray());

                PermissionTreeViewModel current = null;
                PermissionData permissionDefinition = null;
                while (permissionViewModels.Count > 0) // 如果列表不为空，继续初始化
                {
                    current = permissionViewModels[0];
                    permissionViewModels.RemoveAt(0);
                    permissionViewModels.AddRange(current.Children);

                    // 如果当前节点对应于PermissionData，则设置选中状态
                    if (current.Tag != null && !(current.Tag is PermissionGroupData))
                    {
                        permissionDefinition = current.Tag as PermissionData;
                        // 选中状态由数据库是否存在当前权限为准
                        current.IsSelected = IsSelected(permissionDefinition);
                        // 监听当前叶子节点的选择变更事件，并自动同步数据库
                        current.PropertyChanged -= PermissionTreeViewModelPropertyChanged;
                        current.PropertyChanged += PermissionTreeViewModelPropertyChanged;
                    }
                }
            }

            _initializePermissionSelected = false;
        }

        protected abstract bool IsSelected(PermissionData permissionDefinition);

        private void PermissionTreeViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_initializePermissionSelected)
            {
                return;
            }

            if (e.PropertyName.Equals("IsSelected"))
            {
                var permissionTreeViewModel = sender as PermissionTreeViewModel;
                var permissionDefinition = permissionTreeViewModel.Tag as PermissionData;

                if (permissionTreeViewModel.IsSelected) // 如果选择一个权限，则Invoke该权限
                {
                    Invoke(permissionDefinition);
                }
                else // 否则，回收当前角色的指定权限。
                {
                    Revoke(permissionDefinition);
                }
            }
        }

        protected abstract void Invoke(PermissionData permissionDefinition);

        protected abstract void Revoke(PermissionData permissionDefinition);
    }

    /// <summary>
    /// 权限树工厂，每一个角色都有一个不同的权限树。权限树，根据权限扩展模型动态构建。
    /// </summary>
    public class RolePermissionTreeViewModelFactory : PermissionTreeViewModelFactory
    {
        /// <summary>
        /// 当前角色。
        /// </summary>
        private Guid _currentRoleId;
        /// <summary>
        /// 角色数据访问类。
        /// </summary>
        private RoleDataAccessor _roleDataAccessor = new RoleDataAccessor();

        private List<Permission> _rolePermissions;

        public RolePermissionTreeViewModelFactory(Guid currentRoleId)
        {
            _currentRoleId = currentRoleId;
            ResetPermissions();

            InitializeExtension();
        }

        public void SetCurrentRole(Guid roleId)
        {
            if(!roleId.Equals(_currentRoleId))
            {
                _currentRoleId = roleId;
                ResetPermissions();

                InitializePermissionSelected();
            }
        }

        private void ResetPermissions()
        {
            _rolePermissions = _roleDataAccessor.GetPermissionsByRole(_currentRoleId);
        }

        protected override bool SelectAll()
        {
            return Role.AdminRoleId.Equals(_currentRoleId);
        }

        protected override bool UnselectAll()
        {
            return _currentRoleId == null || _currentRoleId.Equals(Guid.Empty);
        }

        protected override bool IsSelected(PermissionData permissionDefinition)
        {
            return _rolePermissions.Count > 0 && _rolePermissions.Exists(p => p.Id.Equals(permissionDefinition.Id) && p.BundleSymbolicName.Equals(permissionDefinition.Owner.SymbolicName));
        }

        protected override void Invoke(PermissionData permissionDefinition)
        {
            _roleDataAccessor.Invoke(_currentRoleId, permissionDefinition.Owner.SymbolicName, permissionDefinition.Id);
            ResetPermissions();
        }

        protected override void Revoke(PermissionData permissionDefinition)
        {
            _roleDataAccessor.Revoke(_currentRoleId, permissionDefinition.Owner.SymbolicName, permissionDefinition.Id);
            ResetPermissions();
        }
    }

    public class UserPermissionTreeViewModelFactory : PermissionTreeViewModelFactory
    {
        /// <summary>
        /// 当前角色。
        /// </summary>
        private Guid _currentRoleId;
        /// <summary>
        /// 角色数据访问类。
        /// </summary>
        private RoleDataAccessor _roleDataAccessor = new RoleDataAccessor();
        private List<Permission> _rolePermissions;

        public Guid _currentUserId;
        private UserDataAccessor _userDataAccessor = new UserDataAccessor();
        private List<Permission> _includePermissions;
        private List<Permission> _excludePermissions;
        public UserPermissionTreeViewModelFactory(Guid currentUserId, Guid currentRoleId)
        {
            _currentUserId = currentUserId;
            _currentRoleId = currentRoleId;

            ResetRolePermissions();
            ResetUserPermissions();

            InitializeExtension();
        }

        private void ResetRolePermissions()
        {
            if (_currentRoleId == Guid.Empty)
            {
                _rolePermissions = new List<Permission>();
                return;
            }
            
            _rolePermissions = _roleDataAccessor.GetPermissionsByRole(_currentRoleId);
        }

        private void ResetUserPermissions()
        {
            if (!Guid.Empty.Equals(_currentUserId))
            {
                var user = _userDataAccessor.GetByKeyWithInExcludePermissions(_currentUserId);

                if (user != null)
                {
                    _includePermissions = new List<Permission>(user.IncludePermissions);
                    _excludePermissions = new List<Permission>(user.ExcludePermissions);
                    return;
                }
            }

            _includePermissions = new List<Permission>();
            _excludePermissions = new List<Permission>();
        }

        public void SetCurrentUser(Guid userId, Guid roleId)
        {
            if(userId.Equals(_currentUserId) && roleId.Equals(_currentRoleId))
            {
                return;
            }
            _currentUserId = userId;
            _currentRoleId = roleId;

            ResetUserPermissions();
            ResetRolePermissions();

            InitializePermissionSelected();
        }

        protected override bool SelectAll()
        {
            return User.DefaultSystemManagerGuid.Equals(_currentUserId) || Role.AdminRoleId.Equals(_currentRoleId);
        }

        protected override bool UnselectAll()
        {
            return Guid.Empty.Equals(_currentUserId);
        }

        protected override bool IsSelected(PermissionData permissionDefinition)
        {
            bool include = _includePermissions.Exists(p => p.BundleSymbolicName.Equals(permissionDefinition.Owner.SymbolicName) && p.Id.Equals(permissionDefinition.Id));
            if(include)
            {
                return true;
            }
            bool exclude = _excludePermissions.Exists(p => p.BundleSymbolicName.Equals(permissionDefinition.Owner.SymbolicName) && p.Id.Equals(permissionDefinition.Id));
            if(exclude)
            {
                return false;
            }

            return _rolePermissions.Exists(p => p.BundleSymbolicName.Equals(permissionDefinition.Owner.SymbolicName) && p.Id.Equals(permissionDefinition.Id));
        }

        protected override void Invoke(PermissionData permissionDefinition)
        {
            _userDataAccessor.IncludePermission(_currentUserId, permissionDefinition.Owner.SymbolicName, permissionDefinition.Id);
            ResetUserPermissions();
        }

        protected override void Revoke(PermissionData permissionDefinition)
        {
            _userDataAccessor.ExcludePermission(_currentUserId, permissionDefinition.Owner.SymbolicName, permissionDefinition.Id);
            ResetUserPermissions();
        }
    }

    /// <summary>
    /// 权限树模型，支持Redo/Undo。
    /// </summary>
    public class PermissionTreeViewModel : ViewModelBase
    {
        /// <summary>
        /// 属性 <see cref="Parent" /> 的名称。
        /// </summary>
        public const string ParentPropertyName = "Parent";

        private PermissionTreeViewModel _parent;

        /// <summary>
        /// 直接更改 Parent 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public PermissionTreeViewModel Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                Set(() => Parent, ref _parent, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="Children" /> 的名称。
        /// </summary>
        public const string ChildrenPropertyName = "Children";

        private ObservableCollection<PermissionTreeViewModel> _children;

        /// <summary>
        /// 直接更改 Children 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public ObservableCollection<PermissionTreeViewModel> Children
        {
            get
            {
                return _children;
            }
            set
            {
                Set(() => Children, ref _children, value);
            }
        }
        /// <summary>
        /// 显示文本。
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 关联的PermissionData或者PermissionGroup扩展对象模型。
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 属性 <see cref="IsSelected" /> 的名称。
        /// </summary>
        public const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelected = false;

        /// <summary>
        /// 直接更改 IsSelected 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    // 当先则一个父节点时，需要选择所有子节点
                    ChangeChildNodes(this);
                    // 如果其父检点当前选中了所有子节点，则父节点也要选中
                    ChangedParentNodes(this);
                    // 发出变更通知
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        public PermissionTreeViewModel()
        {
            Children = new ObservableCollection<PermissionTreeViewModel>();
        }

        /// <summary>
        /// 选择所有子节点。
        /// </summary>
        /// <param name="currentPermissionTreeViewModel">当前节点模型。</param>
        public void ChangeChildNodes(PermissionTreeViewModel currentPermissionTreeViewModel)
        {
            if (currentPermissionTreeViewModel.Children != null)
            {
                foreach (var data in currentPermissionTreeViewModel.Children)
                {
                    // 更改子节点的选择属性
                    data._isSelected = currentPermissionTreeViewModel.IsSelected;
                    data.RaisePropertyChanged("IsSelected");
                    if (data.Children != null)
                    {
                        // 递归更改子节点的子节点
                        data.ChangeChildNodes(data);
                    }
                }
            }
        }

        /// <summary>
        /// 向上遍历,更改父节点状态。
        /// 注意：这里的父节点不是属性而是字段。
        /// 采用字段的原因是因为不想让父节点触发访问器而触发Setter。
        /// </summary>
        /// <param name="currentPermissionTreeViewModel">当前节点。</param>
        public void ChangedParentNodes(PermissionTreeViewModel currentPermissionTreeViewModel)
        {
            if (currentPermissionTreeViewModel.Parent != null)
            {
                bool parentNodeState = true;
                int selectedCount = 0;  //被选中的个数
                int noSelectedCount = 0;    //不被选中的个数

                foreach (var data in currentPermissionTreeViewModel.Parent.Children)
                {
                    if (data.IsSelected == true)
                    {
                        selectedCount++;
                    }
                    else if (data.IsSelected == false)
                    {
                        noSelectedCount++;
                    }
                }

                //如果全部被选中,则修改父节点为选中
                if (selectedCount ==
                    currentPermissionTreeViewModel.Parent.Children.Count)
                {
                    parentNodeState = true;
                }
                else
                {
                    parentNodeState = false;
                }

                currentPermissionTreeViewModel.Parent._isSelected = parentNodeState;
                currentPermissionTreeViewModel.Parent.RaisePropertyChanged("IsSelected");

                if (currentPermissionTreeViewModel.Parent.Parent != null)
                {
                    ChangedParentNodes(currentPermissionTreeViewModel.Parent);
                }
            }
        }
    }
}
