using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UIShell.RbacPermissionService.DataAccess;
using UIShell.RbacPermissionService.Model;
using UIShell.WpfShellPlugin.Utility;

namespace UIShell.RbacManagementPlugin.ViewModels
{
    public class AddOrEditUserViewModel : ViewModelBase
    {
        public ObservableCollection<UserViewModel> UserViewModels { get; set; }

        public List<Role> Roles { get; set; }

        /// <summary>
        /// 属性 <see cref="Action" /> 的名称。
        /// </summary>
        public const string ActionPropertyName = "Action";

        private ViewModelAction _action;

        /// <summary>
        /// 直接更改 Action 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public ViewModelAction Action
        {
            get
            {
                return _action;
            }
            set
            {
                Set(() => Action, ref _action, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="UserViewModel" /> 的名称。
        /// </summary>
        public const string UserViewModelPropertyName = "UserViewModel";

        private UserViewModel _userViewModel;

        /// <summary>
        /// 直接更改 UserViewModel 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public UserViewModel UserViewModel
        {
            get
            {
                return _userViewModel;
            }
            set
            {
                Set(() => UserViewModel, ref _userViewModel, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="Error" /> 的名称。
        /// </summary>
        public const string ErrorPropertyName = "Error";

        private string _error;

        /// <summary>
        /// 直接更改 Error 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                Set(() => Error, ref _error, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="Result" /> 的名称。
        /// </summary>
        public const string ResultPropertyName = "Result";

        private bool? _result;

        /// <summary>
        /// 直接更改 Result 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public bool? Result
        {
            get
            {
                return _result;
            }
            set
            {
                Set(() => Result, ref _result, value);
            }
        }

        private RelayCommand<FrameworkElement> _okCommand;

        /// <summary>
        /// 获取 OkCommand 属性。
        /// </summary>
        public RelayCommand<FrameworkElement> OkCommand
        {
            get
            {
                return _okCommand
                    ?? (_okCommand = new RelayCommand<FrameworkElement>(
                        control =>
                        {
                            Error = UserViewModel.Error;
                            if(!string.IsNullOrEmpty(Error))
                            {
                                return;
                            }

                            var user = Mapper.Map<User>(UserViewModel);
                            if(Action == ViewModelAction.Add && _userDataAccessor.Create(user))
                            {
                                Activator.MainWindowService.HideSidebar(control);
                                Result = true;
                            }

                            if (Action == ViewModelAction.Edit && _userDataAccessor.Update(user))
                            {
                                Activator.MainWindowService.HideSidebar(control);
                                Result = true;
                            }
                        }
                        ));
            }
        }

        private RelayCommand<FrameworkElement> _cancelCommand;

        /// <summary>
        /// 获取 CancelCommand 属性。
        /// </summary>
        public RelayCommand<FrameworkElement> CancelCommand
        {
            get
            {
                return _cancelCommand
                    ?? (_cancelCommand = new RelayCommand<FrameworkElement>(
                        control =>
                        {
                            Rollback();

                            Activator.MainWindowService.HideSidebar(control);
                            Result = false;
                        }
                        ));
            }
        }

        private RoleDataAccessor _roleDataAccessor = new RoleDataAccessor();
        private UserDataAccessor _userDataAccessor = new UserDataAccessor();
        public AddOrEditUserViewModel()
        {
            Mapper.CreateMap<UserViewModel, User>();
            Mapper.CreateMap<User, UserViewModel>();
            Roles = _roleDataAccessor.GetAll();
            Roles.RemoveAll(r => Role.AdminRoleId.Equals(r.Id));
        }

        private void Rollback()
        {
            if (UserViewModel != null)
            {
                if(Action == ViewModelAction.Add)
                {
                    UserViewModels.Remove(UserViewModel);
                }
                else
                {
                    Mapper.Map(_userDataAccessor.GetByKey(UserViewModel.Id), UserViewModel);
                }
            }
        }
    }
}
