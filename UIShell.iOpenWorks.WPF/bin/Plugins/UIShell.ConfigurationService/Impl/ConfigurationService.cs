using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using UIShell.OSGi;
using UIShell.OSGi.Collection;
using UIShell.OSGi.Utility;

namespace UIShell.ConfigurationService
{
    /// <summary>
    /// 配置服务实现。
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        /// <summary>
        /// 插件默认配置文件。
        /// </summary>
        private const string ConfigurationFile = "Bundle.config";
        /// <summary>
        /// 空配置文件内容。
        /// </summary>
        private const string EmptyConfiguration = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration></configuration>";
        /// <summary>
        /// 插件与配置对应关系。
        /// </summary>
        private ThreadSafeDictionary<IBundle, Configuration> _configurations = new ThreadSafeDictionary<IBundle, Configuration>();

        /// <summary>
        /// 确保配置文件存在。
        /// </summary>
        /// <param name="bundle">所属插件。</param>
        private void AssureConfigFileExist(IBundle bundle)
        {
            var configFile = Path.Combine(bundle.Location, ConfigurationFile);
            if(!File.Exists(configFile))
            {
                File.WriteAllText(configFile, EmptyConfiguration, UTF8Encoding.UTF8);
            }
        }

        /// <summary>
        /// 获取插件的配置。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <returns>插件的配置对象。</returns>
        private Configuration Get(IBundle bundle)
        {
            AssureConfigFileExist(bundle); // 确保文件存在

            using (var locker = _configurations.Lock()) // 从缓存中获取
            {
                if (locker.ContainsKey(bundle))
                {
                    return locker[bundle];
                }
            }

            var configFile = Path.Combine(bundle.Location, ConfigurationFile);

            try
            {
                // 获取配置对象
                var configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = configFile;
                var bundleConfiguration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                using (var locker = _configurations.Lock()) // 保存到缓存
                {
                    locker[bundle] = bundleConfiguration;
                }
                return bundleConfiguration;
            }
            catch (ConfigurationErrorsException ex)
            {
                FileLogUtility.Error(string.Format("Failed to load the Bundle configuration file '{0}' for bundle '{1}'.", configFile, bundle.SymbolicName));
                FileLogUtility.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// 获取字符串。
        /// </summary>
        /// <param name="bundle">所属插件。</param>
        /// <param name="key">键值。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在，返回配置值，否则返回默认值。</returns>
        public string Get(IBundle bundle, string key, string defaultValue)
        {
            var config = Get(bundle); // 获取配置
            if (config.AppSettings.Settings[key] == null || string.IsNullOrEmpty(config.AppSettings.Settings[key].Value))
            {
                return defaultValue; // 使用默认值
            }

            var strValue = config.AppSettings.Settings[key].Value; // 获取配置值
            return strValue.ToString();
        }

        /// <summary>
        /// 获取插件的一个整数。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是整数，则返回配置值，否则，返回默认值。</returns>
        public int Get(IBundle bundle, string key, int defaultValue)
        {
            var config = Get(bundle);
            if (config.AppSettings.Settings[key] == null || string.IsNullOrEmpty(config.AppSettings.Settings[key].Value))
            {
                return defaultValue; // 使用默认值
            }

            int intValue;

            // 解析整数
            if (int.TryParse(config.AppSettings.Settings[key].Value, out intValue))
            {
                return intValue;
            }
            else // 记录异常
            {
                FileLogUtility.Warn(string.Format("The setting '{0}' with value '{1}' in the Bundle '{2}' is not a int value.", key, config.AppSettings.Settings[key].Value, bundle.SymbolicName));
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取插件的一个浮点值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是有效浮点值，返回配置值，否则，返回默认值。</returns>
        public float Get(IBundle bundle, string key, float defaultValue)
        {
            var config = Get(bundle);
            if (config.AppSettings.Settings[key] == null || string.IsNullOrEmpty(config.AppSettings.Settings[key].Value))
            {
                return defaultValue;
            }

            float floatValue;

            if (float.TryParse(config.AppSettings.Settings[key].Value, out floatValue))
            {
                return floatValue;
            }
            else
            {
                FileLogUtility.Warn(string.Format("The setting '{0}' with value '{1}' in the Bundle '{2}' is not a float value.", key, config.AppSettings.Settings[key].Value, bundle.SymbolicName));
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取插件的一个浮点值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是有效浮点值，返回配置值，否则，返回默认值。</returns>
        public double Get(IBundle bundle, string key, double defaultValue)
        {
            var config = Get(bundle);
            if (config.AppSettings.Settings[key] == null || string.IsNullOrEmpty(config.AppSettings.Settings[key].Value))
            {
                return defaultValue;
            }

            double doubleValue;

            if (double.TryParse(config.AppSettings.Settings[key].Value, out doubleValue))
            {
                return doubleValue;
            }
            else
            {
                FileLogUtility.Warn(string.Format("The setting '{0}' with value '{1}' in the Bundle '{2}' is not a double value.", key, config.AppSettings.Settings[key].Value, bundle.SymbolicName));
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取插件的一个日期值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是合法日期，返回配置值，否则，返回默认值。</returns>
        public DateTime Get(IBundle bundle, string key, DateTime defaultValue)
        {
            var config = Get(bundle);
            if (config.AppSettings.Settings[key] == null || string.IsNullOrEmpty(config.AppSettings.Settings[key].Value))
            {
                return defaultValue;
            }

            DateTime intValue;

            if (DateTime.TryParse(config.AppSettings.Settings[key].Value, out intValue))
            {
                return intValue;
            }
            else
            {
                FileLogUtility.Warn(string.Format("The setting '{0}' with value '{1}' in the Bundle '{2}' is not a int value.", key, config.AppSettings.Settings[key].Value, bundle.SymbolicName));
                return defaultValue;
            }
        }

        /// <summary>
        /// 获取插件的一个布尔值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是合法日期，返回配置值，否则，返回默认值。</returns>
        public bool Get(IBundle bundle, string key, bool defaultValue)
        {
            var config = Get(bundle);
            if (config.AppSettings.Settings[key] == null || string.IsNullOrEmpty(config.AppSettings.Settings[key].Value))
            {
                return defaultValue;
            }

            bool intValue;

            if (bool.TryParse(config.AppSettings.Settings[key].Value, out intValue))
            {
                return intValue;
            }
            else
            {
                FileLogUtility.Warn(string.Format("The setting '{0}' with value '{1}' in the Bundle '{2}' is not a int value.", key, config.AppSettings.Settings[key].Value, bundle.SymbolicName));
                return defaultValue;
            }
        }

        /// <summary>
        /// 设置一个字符串。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        public void Set(IBundle bundle, string key, string value)
        {
            SetAsString<string>(bundle, key, value);
        }

        /// <summary>
        /// 设置一个整数。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        public void Set(IBundle bundle, string key, int value)
        {
            SetAsString<int>(bundle, key, value);
        }

        /// <summary>
        /// 设置一个浮点值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        public void Set(IBundle bundle, string key, float value)
        {
            SetAsString<float>(bundle, key, value);
        }

        /// <summary>
        /// 设置一个浮点值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        public void Set(IBundle bundle, string key, double value)
        {
            SetAsString<double>(bundle, key, value);
        }

        /// <summary>
        /// 设置一个时间。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        public void Set(IBundle bundle, string key, DateTime value)
        {
            SetAsString<DateTime>(bundle, key, value);
        }

        /// <summary>
        /// 设置一个布尔值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        public void Set(IBundle bundle, string key, bool value)
        {
            SetAsString<bool>(bundle, key, value);
        }

        /// <summary>
        /// 将值保存为字符串。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="bundle">所属插件。</param>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        private void SetAsString<T>(IBundle bundle, string key, T value)
        {
            var config = Get(bundle);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value == null ? string.Empty : value.ToString());
            config.Save();
        }
    }
}
