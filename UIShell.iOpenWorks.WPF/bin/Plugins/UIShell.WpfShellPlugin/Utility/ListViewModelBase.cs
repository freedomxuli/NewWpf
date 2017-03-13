using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using UIShell.OSGi;

namespace UIShell.WpfShellPlugin.Utility
{
    public abstract class ListViewModelBase : ViewModelBase
    {
        public List<KeyBinding> InputKeyBindings = new List<KeyBinding>();

        public ListViewModelBase()
        {
            InputKeyBindings.Add(new KeyBinding { Modifiers = ModifierKeys.Alt, Key = Key.A, Command = AddCommand });
            InputKeyBindings.Add(new KeyBinding { Modifiers = ModifierKeys.Alt, Key = Key.E, Command = EditCommand });
            InputKeyBindings.Add(new KeyBinding { Key = Key.F2, Command = EditCommand });
            InputKeyBindings.Add(new KeyBinding { Modifiers = ModifierKeys.Alt, Key = Key.D, Command = DeleteCommand });
            InputKeyBindings.Add(new KeyBinding { Modifiers = ModifierKeys.Alt, Key = Key.C, Command = CloseCommand });

            Message = "增加 Alt + A; 编辑 Alt + E 或者 F2; 删除 Alt + D; 关闭 Alt + C";
        }

        private RelayCommand _closeCommand;

        /// <summary>
        /// 获取 CloseCommand 属性。
        /// </summary>
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand
                    ?? (_closeCommand = new RelayCommand(
                        () =>
                        {
                            var service = BundleRuntime.Instance.GetFirstOrDefaultService<IMainWindowService>();
                            if(service != null)
                                service.CloseCurrentContent();
                        }
                        ));
            }
        }

        private RelayCommand _addCommand;

        /// <summary>
        /// 获取 AddCourseCommand 属性。
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand(Add));
            }
        }

        public abstract void Add();

        private RelayCommand _editCommand;

        /// <summary>
        /// 获取 EditCourseCommand 属性。
        /// </summary>
        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand
                    ?? (_editCommand = new RelayCommand(Edit));
            }
        }

        public abstract void Edit();

        private RelayCommand _deleteCommand;

        /// <summary>
        /// 获取 DeleteCommand 属性。
        /// </summary>
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand
                    ?? (_deleteCommand = new RelayCommand(Delete));
            }
        }

        public abstract void Delete();

        /// <summary>
        /// 属性 <see cref="Message" /> 的名称。
        /// </summary>
        public const string MessagePropertyName = "Message";

        private string _message;

        /// <summary>
        /// 直接更改 Message 属性，触发PropertyChanged事件，
        /// 但是不对更改进行跟踪，不支持UndoRedo。
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                Set(() => Message, ref _message, value);
            }
        }
    }
}
