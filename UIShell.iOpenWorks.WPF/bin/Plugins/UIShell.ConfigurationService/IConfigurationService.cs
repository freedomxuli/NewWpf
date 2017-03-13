using System;
using System.Collections.Generic;
using System.Text;
using UIShell.OSGi;

namespace UIShell.ConfigurationService
{
    /// <summary>
    /// 配置持久化服务。每一个插件的根目录拥有一个plugin.config文件，用于设置插件的个性化信息。
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// 获取插件的一个字符串。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在，返回配置值，否则，返回默认值。</returns>
        string Get(IBundle bundle, string key, string defaultValue);
        /// <summary>
        /// 获取插件的一个整数。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是整数，则返回配置值，否则，返回默认值。</returns>
        int Get(IBundle bundle, string key, int defaultValue);
        /// <summary>
        /// 获取插件的一个浮点值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是有效浮点值，返回配置值，否则，返回默认值。</returns>
        float Get(IBundle bundle, string key, float defaultValue);
        /// <summary>
        /// 获取插件的一个浮点值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是有效浮点值，返回配置值，否则，返回默认值。</returns>
        double Get(IBundle bundle, string key, double defaultValue);
        /// <summary>
        /// 获取插件的一个日期值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是合法日期，返回配置值，否则，返回默认值。</returns>
        DateTime Get(IBundle bundle, string key, DateTime defaultValue);
        /// <summary>
        /// 获取插件的一个布尔值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">字符串的Key。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>如果存在且是合法日期，返回配置值，否则，返回默认值。</returns>
        bool Get(IBundle bundle, string key, bool defaultValue);
        /// <summary>
        /// 设置一个字符串。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        void Set(IBundle bundle, string key, string value);
        /// <summary>
        /// 设置一个整数。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        void Set(IBundle bundle, string key, int value);
        /// <summary>
        /// 设置一个浮点值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        void Set(IBundle bundle, string key, float value);
        /// <summary>
        /// 设置一个浮点值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        void Set(IBundle bundle, string key, double value);
        /// <summary>
        /// 设置一个时间。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        void Set(IBundle bundle, string key, DateTime value);
        /// <summary>
        /// 设置一个布尔值。
        /// </summary>
        /// <param name="bundle">插件。</param>
        /// <param name="key">资源Key。</param>
        /// <param name="value">值。</param>
        void Set(IBundle bundle, string key, bool value);
    }
}
