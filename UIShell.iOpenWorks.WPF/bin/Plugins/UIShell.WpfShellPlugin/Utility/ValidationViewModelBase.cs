using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using GalaSoft.MvvmLight;

namespace UIShell.WpfShellPlugin.Utility
{
    public class ValidationViewModelBase : ViewModelBase, IDataErrorInfo
    {
        private readonly Dictionary<string, PropertyInfo> _propertyGetters = new Dictionary<string, PropertyInfo>();

        private readonly Dictionary<string, ValidationAttribute[]> _validators = new Dictionary<string, ValidationAttribute[]>();

        private readonly Type _type;

        public ValidationViewModelBase()
        {
            _type = GetType();
            LoadValidationAttributes();
        }

        private void LoadValidationAttributes()
        {
            PropertyInfo[] properties = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in properties)
            {
                //拥μ有D的?验é证¤特?性?
                object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(ValidationAttribute), true);
                if (customAttributes.Length > 0)
                {
                    _validators.Add(propertyInfo.Name, customAttributes as ValidationAttribute[]);
                    _propertyGetters.Add(propertyInfo.Name, propertyInfo);
                }
            }
        }

        public string Error
        {
            get
            {
                IEnumerable<string> errors = from d in _validators
                                             from v in d.Value
                                             where !v.IsValid(_propertyGetters[d.Key].GetValue(this, null))
                                             select v.ErrorMessage;
                return string.Join(Environment.NewLine, errors.ToArray());
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (_propertyGetters.ContainsKey(columnName))
                {
                    object value = _propertyGetters[columnName].GetValue(this, null);
                    string[] errors = _validators[columnName].Where(v => !v.IsValid(value))
                        .Select(v => v.ErrorMessage).ToArray();
                    RaisePropertyChanged("Error");
                    return string.Join(Environment.NewLine, errors);
                }
                RaisePropertyChanged("Error");
                return string.Empty;
            }
        }
    }

    public class GuidNotEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is Guid)
            {
                return !Guid.Empty.Equals(value);
            }
            return false;
        }
    }

    public enum ViewModelAction
    {
        Add,
        Edit,
        Delete
    }
}
