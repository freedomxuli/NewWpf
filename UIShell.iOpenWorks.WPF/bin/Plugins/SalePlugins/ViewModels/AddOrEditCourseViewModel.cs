using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using AutoMapper;
using SalePlugins.DataAccessor;
using SalePlugins.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UIShell.WpfShellPlugin.Utility;

namespace SalePlugins.ViewModels
{
    public class AddOrEditCourseViewModel : ViewModelBase
    {
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

        /// <summary>
        /// 属性 <see cref="CourseViewModel" /> 的名称。
        /// </summary>
        public const string CourseViewModelPropertyName = "CourseViewModel";

        private CourseViewModel _courseViewModel;

        /// <summary>
        /// 直接更改 CourseViewModel 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public CourseViewModel CourseViewModel
        {
            get
            {
                return _courseViewModel;
            }
            set
            {
                Set(() => CourseViewModel, ref _courseViewModel, value);
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

        public ObservableCollection<CourseViewModel> CourseViewModels { get; set; }

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
                            Error = CourseViewModel.Error;
                            if (!string.IsNullOrEmpty(Error))
                            {
                                return;
                            }

                            var course = Mapper.Map<Course>(CourseViewModel);

                            if (Action == ViewModelAction.Add)
                            {
                                Result = _courseDataAccessor.Create(course);
                            }
                            else if (Action == ViewModelAction.Edit)
                            {
                                Result = _courseDataAccessor.Update(course);
                            }

                            BundleActivator.MainWindowService.HideSidebar(control);
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
                            Result = false;
                            BundleActivator.MainWindowService.HideSidebar(control);
                        }
                        ));
            }
        }

        private CourseDataAccessor _courseDataAccessor = new CourseDataAccessor();

        public AddOrEditCourseViewModel()
        {
            Mapper.CreateMap<Course, CourseViewModel>();
            Mapper.CreateMap<CourseViewModel, Course>();
        }

        private void Rollback()
        {
            if (Action == ViewModelAction.Add)
            {
                CourseViewModels.Remove(CourseViewModel);
            }
            else if (Action == ViewModelAction.Edit)
            {
                var course = _courseDataAccessor.GetByKey(CourseViewModel.ModelId);
                Mapper.Map(course, CourseViewModel);
            }
        }
    }
}
