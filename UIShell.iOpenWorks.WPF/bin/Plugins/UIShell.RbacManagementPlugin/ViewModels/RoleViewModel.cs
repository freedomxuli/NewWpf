using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using AutoMapper;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using UIShell.RbacPermissionService.DataAccess;
using UIShell.RbacPermissionService.Model;

namespace UIShell.RbacManagementPlugin.ViewModels
{
    /// <summary>
    /// 角色视图模型。
    /// </summary>
    public class RoleViewModel : ViewModelBase
    {
        private RoleDataAccessor _roleDataAccessor = new RoleDataAccessor();

        /// <summary>
        /// 属性 <see cref="Id" /> 的名称。
        /// </summary>
        public const string IdPropertyName = "Id";

        private Guid _id = Guid.NewGuid();

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
        /// The <see cref="RoleTextBlockVisibility" /> property's name.
        /// </summary>
        public const string RoleTextBlockVisibilityPropertyName = "RoleTextBlockVisibility";

        private Visibility _roleTextBlockVisibility = Visibility.Visible;

        /// <summary>
        /// Sets and gets the RoleTextBlockVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility RoleTextBlockVisibility
        {
            get
            {
                return _roleTextBlockVisibility;
            }
            set
            {
                Set(() => RoleTextBlockVisibility, ref _roleTextBlockVisibility, value);
            }
        }

        /// <summary>
        /// The <see cref="RoleTextBoxVisibility" /> property's name.
        /// </summary>
        public const string RoleTextBoxVisibilityPropertyName = "RoleTextBoxVisibility";

        private Visibility _roleTextBoxVisibility = Visibility.Hidden;

        /// <summary>
        /// Sets and gets the RoleTextBoxVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility RoleTextBoxVisibility
        {
            get
            {
                return _roleTextBoxVisibility;
            }
            set
            {
                Set(() => RoleTextBoxVisibility, ref _roleTextBoxVisibility, value);
            }
        }

        private bool _startSync = false;
        /// <summary>
        /// 开始与数据库保持同步，一旦数据变化，则自动变更。
        /// </summary>
        public void BeginSyncToDatabase()
        {
            _startSync = true;
            PropertyChanged += RoleViewModel_PropertyChanged;
        }

        /// <summary>
        /// 停止与数据库保持同步。
        /// </summary>
        public void StopSyncToDatabase()
        {
            _startSync = false;
            PropertyChanged -= RoleViewModel_PropertyChanged;
        }

        /// <summary>
        /// 一旦属性值发生变更，则同步更新到数据库。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoleViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(RoleTextBoxVisibilityPropertyName) ||
                e.PropertyName.Equals(RoleTextBlockVisibilityPropertyName))
            {
                return;
            }

            _roleDataAccessor.Update(Mapper.Map<Role>(this));
        }
    }
}
