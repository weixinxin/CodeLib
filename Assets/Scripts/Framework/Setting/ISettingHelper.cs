using System;
namespace Framework
{
    public interface ISettingHelper
    {

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns>是否加载配置成功</returns>
        bool Load();

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <returns>是否保存配置成功</returns>
        bool Save();

        /// <summary>
        /// 移除所有配置项
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// 移除指定配置项
        /// </summary>
        /// <param name="key">要移除配置项的名称</param>
        void DeleteKey(string key);

        /// <summary>
        /// 从指定配置项中读取浮点数值
        /// </summary>
        /// <param name="key">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的浮点数值</returns>
        float GetFloat(string key, float defaultValue);

        /// <summary>
        /// 从指定配置项中读取整数值
        /// </summary>
        /// <param name="key">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的整数值</returns>
        int GetInt(string key, int defaultValue);

        /// <summary>
        /// 从指定配置项中读取字符串值
        /// </summary>
        /// <param name="key">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的字符串值</returns>
        string GetString(string key, string defaultValue);


        /// <summary>
        /// 检查是否存在指定配置项
        /// </summary>
        /// <param name="key">要检查配置项的名称</param>
        /// <returns>指定的配置项是否存在</returns>
        bool HasKey(string key);

        /// <summary>
        /// 向指定配置项写入浮点数值
        /// </summary>
        /// <param name="key">要写入配置项的名称</param>
        /// <param name="value">要写入的浮点数值</param>
        void SetFloat(string key, float value);

        /// <summary>
        /// 向指定配置项写入整数值
        /// </summary>
        /// <param name="key">要写入配置项的名称</param>
        /// <param name="value">要写入的整数值</param>
        void SetInt(string key, int value);

        /// <summary>
        /// 向指定配置项写入字符串值
        /// </summary>
        /// <param name="key">要写入配置项的名称</param>
        /// <param name="value">要写入的字符串值</param>
        void SetString(string key, string value);

    }
}
