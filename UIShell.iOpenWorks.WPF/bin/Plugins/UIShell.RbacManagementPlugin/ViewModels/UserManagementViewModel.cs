using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UIShell.RbacPermissionService.DataAccess;
using UIShell.RbacPermissionService.Model;
using UIShell.WpfShellPlugin.Utility;

namespace UIShell.RbacManagementPlugin.ViewModels
{
    public class UserManagementViewModel : ListViewModelBase
    {
        private UserDataAccessor _userDataAccessor = new UserDataAccessor();
        private RoleDataAccessor _roleDataAccessor = new RoleDataAccessor();
        static UserManagementViewModel()
        {
            Mapper.CreateMap<User, UserViewModel>();
            Mapper.CreateMap<UserViewModel, User>();
        }

        public UserManagementViewModel() 
        {
            Roles = new List<Role>();
            UserViewModels = new ObservableCollection<UserViewModel>();
        }

        public ObservableCollection<UserViewModel> UserViewModels { get; set; }
        public List<Role> Roles { get; set; }

        /// <summary>
        /// 属性 <see cref="CurrentUserViewModel" /> 的名称。
        /// </summary>
        public const string CurrentUserViewModelPropertyName = "CurrentUserViewModel";

        private UserViewModel _currentUserViewModel;

        /// <summary>
        /// 直接更改 CurrentUserViewModel 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public UserViewModel CurrentUserViewModel
        {
            get
            {
                return _currentUserViewModel;
            }
            set
            {
                if(!Equals(_currentUserViewModel, value))
                {
                    if(_currentUserViewModel != null)
                    {
                        _currentUserViewModel.PropertyChanged -= CurrentUserViewModelPropertyChanged;
                    }
                    _currentUserViewModel = value;

                    ResetPermissionViewModels();

                    RaisePropertyChanged(CurrentUserViewModelPropertyName);
                }
            }
        }

        private void ResetPermissionViewModels()
        {
            if (_currentUserViewModel != null)
            {
                _currentUserViewModel.PropertyChanged += CurrentUserViewModelPropertyChanged;
                _factory.SetCurrentUser(_currentUserViewModel.Id, _currentUserViewModel.RoleId);
                PermissionTreeViewModels = _factory.PermissionTreeViewModels;
                IsTreeViewEnabled = !Role.AdminRoleId.Equals(_currentUserViewModel.RoleId);
            }
            else
            {
                PermissionTreeViewModels = null;
            }
        }

        private void CurrentUserViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("RoleId"))
            {
                ResetPermissionViewModels();
            }
        }

        private static UserPermissionTreeViewModelFactory _factory = new UserPermissionTreeViewModelFactory(Guid.Empty, Guid.Empty);

        private ObservableCollection<PermissionTreeViewModel> _permissionTreeViewModels = _factory.PermissionTreeViewModels;

        /// <summary>
        /// 直接更改 PermissionTreeViewModels 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public ObservableCollection<PermissionTreeViewModel> PermissionTreeViewModels
        {
            get
            {
                return _permissionTreeViewModels;
            }
            set
            {
                Set(() => PermissionTreeViewModels, ref _permissionTreeViewModels, value);
            }
        }

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

        private RelayCommand _loadUsersCommand;

        /// <summary>
        /// 获取 LoadUsersCommand 属性。
        /// </summary>
        public RelayCommand LoadUsersCommand
        {
            get
            {
                return _loadUsersCommand
                    ?? (_loadUsersCommand = new RelayCommand(LoadUsers));
            }
        }

        private void LoadUsers()
        {
            //查询Role信息
            _roleDataAccessor.GetAll().ForEach(role =>
            {
                // 忽略系统管理员角色
                if (role.IsAdminRole())
                {
                    return;
                }
                Roles.Add(role);
            });

            //从持久层获取已经保持的数据
            _userDataAccessor.GetAll().ForEach(user =>
            {
                // 忽略系统管理员用户
                if (user.IsAdminUser())
                {
                    return;
                }
                //构造ViewModel
                var userViewModel = new UserViewModel();
                //从Entity映射ViewModel
                UserViewModels.Add(Mapper.Map<User, UserViewModel>(user, userViewModel));
            });


            if (UserViewModels.Count > 0)
            {
                CurrentUserViewModel = UserViewModels[0];

                //缓存的用户还没有Role信息，只有RoleId，为了正确在View展示Role Name，需要这里做一次处理。
                var pendingUsers = UserViewModels.Where(item => item.RoleId != Guid.Empty && item.Role == null);
                foreach (var userViewModel in pendingUsers)
                {
                    //重新绑定Role
                    userViewModel.Role = Roles.FirstOrDefault(item => item.Id == userViewModel.RoleId);
                    if (userViewModel.Role != null)
                    {
                        userViewModel.RoleId = userViewModel.Role.Id;
                    }
                }
            }
        }

        public override void Add()
        {
            // 显示用户控
            var addUserUserControl = new AddOrEditUserUserControl();
            var addOrEditViewModel = (addUserUserControl.DataContext as AddOrEditUserViewModel);
            addOrEditViewModel.UserViewModel = new UserViewModel() { Id = Guid.NewGuid() };
            addOrEditViewModel.UserViewModels = UserViewModels;
            addOrEditViewModel.UserViewModels.Add(addOrEditViewModel.UserViewModel);
            addOrEditViewModel.Action = ViewModelAction.Add;

            var oldCurrentViewModel = CurrentUserViewModel;

            CurrentUserViewModel = addOrEditViewModel.UserViewModel;

            // 显示侧边框
            Activator.MainWindowService.ShowSidebar(addUserUserControl, 300, "添加用户");

            addOrEditViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals(AddOrEditUserViewModel.ResultPropertyName))
                {
                    if (addOrEditViewModel.Result != true)
                    {
                        CurrentUserViewModel = oldCurrentViewModel;
                    }
                }
            };
        }

        public override void Edit()
        {
            if (CurrentUserViewModel == null)
            {
                ModernDialog.ShowMessage("请选择一个用户。", "操作警告", MessageBoxButton.OK);
                return;
            }
            var addUserUserControl = new AddOrEditUserUserControl();
            var addOrEditViewModel = (addUserUserControl.DataContext as AddOrEditUserViewModel);
            addOrEditViewModel.UserViewModel = CurrentUserViewModel;
            addOrEditViewModel.UserViewModels = UserViewModels;
            addOrEditViewModel.Action = ViewModelAction.Edit;

            // 显示侧边框
            Activator.MainWindowService.ShowSidebar(addUserUserControl, 300, "编辑用户");
        }

        public override void Delete()
        {
            if (CurrentUserViewModel == null)
            {
                ModernDialog.ShowMessage("请选择一个用户。", "操作警告", MessageBoxButton.OK);
                return;
            }

            if (ModernDialog.ShowMessage("确定删除当前用户？", "删除警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (_userDataAccessor.Delete(Mapper.Map<User>(CurrentUserViewModel)))
                {
                    var currentIndex = UserViewModels.IndexOf(CurrentUserViewModel);
                    UserViewModels.Remove(CurrentUserViewModel);

                    if (currentIndex > 0)
                    {
                        CurrentUserViewModel = UserViewModels[currentIndex - 1];
                    }
                    else if (UserViewModels.Count > 0)
                    {
                        CurrentUserViewModel = UserViewModels[0];
                    }
                }
            }
        }
    }
}
