using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.RightsManagement;
using System.Text;
using System.IO;
using System.Windows.Input;
using AutoMapper;
using SalePlugins.DataAccessor;
using SalePlugins.Model;
using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Threading;
using UIShell.WpfShellPlugin.Utility;
using FirstFloor.ModernUI.Windows.Controls;

namespace SalePlugins.ViewModels
{
    public class CourseManagementViewModel : ListViewModelBase
    {
        private CourseDataAccessor _courseDataAccessor;

        public CourseManagementViewModel()
        {
            CourseViewModels = new ObservableCollection<CourseViewModel>();
            _courseDataAccessor = new CourseDataAccessor();

            Mapper.CreateMap<Course, CourseViewModel>();
            Mapper.CreateMap<CourseViewModel, Course>();

            LoadCourses();
        }

        private void LoadCourses()
        {
            //从持久层获取数据
            _courseDataAccessor.GetAll().ForEach(course =>
            {
                var courseViewModel = new CourseViewModel();
                Mapper.Map<Course, CourseViewModel>(course, courseViewModel);
                CourseViewModels.Add(courseViewModel);
            });


            if (CourseViewModels.Count > 0)
            {
                CurrentCourseViewModel = CourseViewModels[0];
            }
        }

        public ObservableCollection<CourseViewModel> CourseViewModels { get; set; }

        /// <summary>
        /// 属性 <see cref="CurrentCourseViewModel" /> 的名称。
        /// </summary>
        public const string CurrentCourseViewModelPropertyName = "CurrentCourseViewModel";

        private CourseViewModel _currentCourseViewModel;

        /// <summary>
        /// 直接更改 CurrentCourseViewModel 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public CourseViewModel CurrentCourseViewModel
        {
            get
            {
                return _currentCourseViewModel;
            }
            set
            {
                Set(() => CurrentCourseViewModel, ref _currentCourseViewModel, value);
            }
        }

        // For AddCommand
        public override void Add()
        {
            // 显示用户控
            var addOrEditUserControl = new AddOrEditCourseUserControl();
            var addOrEditViewModel = (addOrEditUserControl.DataContext as AddOrEditCourseViewModel);
            addOrEditViewModel.CourseViewModel = new CourseViewModel();
            addOrEditViewModel.CourseViewModels = CourseViewModels;
            addOrEditViewModel.CourseViewModels.Add(addOrEditViewModel.CourseViewModel);
            addOrEditViewModel.Action = ViewModelAction.Add;

            var oldCurrentViewModel = CurrentCourseViewModel;

            CurrentCourseViewModel = addOrEditViewModel.CourseViewModel;

            // 显示侧边框
            BundleActivator.MainWindowService.ShowSidebar(addOrEditUserControl, 300, "添加课程");

            addOrEditViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals(AddOrEditCourseViewModel.ResultPropertyName))
                {
                    if (addOrEditViewModel.Result != true)
                    {
                        CurrentCourseViewModel = oldCurrentViewModel;
                    }
                    else
                    {
                        Message = "添加课程成功";
                    }
                }
            };
        }

        // For EditCommand
        public override void Edit()
        {
            if (CurrentCourseViewModel == null)
            {
                ModernDialog.ShowMessage("请选择一个课程。", "操作警告", MessageBoxButton.OK);
                return;
            }
            var addOrEditUserControl = new AddOrEditCourseUserControl();
            var addOrEditViewModel = (addOrEditUserControl.DataContext as AddOrEditCourseViewModel);
            addOrEditViewModel.CourseViewModel = CurrentCourseViewModel;
            addOrEditViewModel.CourseViewModels = CourseViewModels;
            addOrEditViewModel.Action = ViewModelAction.Edit;

            // 显示侧边框
            BundleActivator.MainWindowService.ShowSidebar(addOrEditUserControl, 300, "编辑课程");

            addOrEditViewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName.Equals(AddOrEditCourseViewModel.ResultPropertyName))
                {
                    if (addOrEditViewModel.Result == true)
                    {
                        Message = "编辑课程成功";
                    }
                }
            };
        }

        // For DeleteCommand
        public override void Delete()
        {
            if (CurrentCourseViewModel == null)
            {
                ModernDialog.ShowMessage("请选择一个课程。", "操作警告", MessageBoxButton.OK);
                return;
            }

            if (ModernDialog.ShowMessage("确定删除当前课程？", "删除警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (_courseDataAccessor.Delete(Mapper.Map<Course>(CurrentCourseViewModel)))
                {
                    var currentIndex = CourseViewModels.IndexOf(CurrentCourseViewModel);
                    CourseViewModels.Remove(CurrentCourseViewModel);

                    if (currentIndex > 0)
                    {
                        CurrentCourseViewModel = CourseViewModels[currentIndex - 1];
                    }
                    else if (CourseViewModels.Count > 0)
                    {
                        CurrentCourseViewModel = CourseViewModels[0];
                    }

                    Message = "删除课程成功";
                }
            }
        }
    }
}
