using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using SalePlugins.DataAccessor;
using GalaSoft.MvvmLight;
using UIShell.WpfShellPlugin.Utility;

namespace SalePlugins.ViewModels
{
    public class CourseViewModel : ValidationViewModelBase
    {
        /// <summary>
        /// 属性 <see cref="ModelId" /> 的名称。
        /// </summary>
        public const string ModelIdPropertyName = "ModelId";

        private Guid _modelId;

        /// <summary>
        /// 直接更改 ModelId 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public Guid ModelId
        {
            get
            {
                return _modelId;
            }
            set
            {
                Set(() => ModelId, ref _modelId, value);
            }
        }

        /// <summary>
        /// 属性 <see cref="Id" /> 的名称。
        /// </summary>
        public const string IdPropertyName = "Id";

        private string _id;

        [Required(ErrorMessage = "编号不能为空")]
        /// <summary>
        /// 直接更改 Id 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public string Id
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

        [Required(ErrorMessage = "课程名称不能为空")]
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

        public CourseViewModel()
        {
            _modelId = Guid.NewGuid();
        }
    }
}
