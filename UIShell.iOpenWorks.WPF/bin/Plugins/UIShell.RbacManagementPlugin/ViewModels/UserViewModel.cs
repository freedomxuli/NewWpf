using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight;
using UIShell.RbacPermissionService.DataAccess;
using UIShell.RbacPermissionService.Model;
using System.ComponentModel.DataAnnotations;
using UIShell.WpfShellPlugin.Utility;

namespace UIShell.RbacManagementPlugin.ViewModels
{
    public class UserViewModel : ValidationViewModelBase
    {
        private UserDataAccessor _userDataAccessor = new UserDataAccessor();
        /// <summary>
        /// 属性 <see cref="Id" /> 的名称。
        /// </summary>
        public const string IdPropertyName = "Id";

        private Guid _id;

        /// <summary>
        /// 直接更改 Id 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                Set(() => Id, ref _id, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="Name" /> 的名称。
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name;

        [Required(ErrorMessage = "登录名不能为空")]
        /// <summary>
        /// 直接更改 Name 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(() => Name, ref _name, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="DisplayName" /> 的名称。
        /// </summary>
        public const string DisplayNamePropertyName = "DisplayName";

        private string _displayName;

        [Required(ErrorMessage = "姓名不能为空")]
        /// <summary>
        /// 直接更改 DisplayName 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                Set(() => DisplayName, ref _displayName, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="Password" /> 的名称。
        /// </summary>
        public const string PasswordPropertyName = "Password";

        private string _password;

        [Required(ErrorMessage = "密码不能为空")]
        /// <summary>
        /// 直接更改 Password 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                Set(() => Password, ref _password, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="RoleId" /> 的名称。
        /// </summary>
        public const string RoleIdPropertyName = "RoleId";

        private Guid _roleId;

        [GuidNotEmpty(ErrorMessage = "角色不能为空")]
        /// <summary>
        /// 直接更改 RoleId 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public Guid RoleId
        {
            get
            {
                return _roleId;
            }
            set
            {
                Set(() => RoleId, ref _roleId, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="Role" /> 的名称。
        /// </summary>
        public const string RolePropertyName = "Role";

        private Role _role;

        /// <summary>
        /// 直接更改 Role 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public Role Role
        {
            get
            {
                return _role;
            }
            set
            {
                Set(() => Role, ref _role, value);
            }
        }
    }
}
