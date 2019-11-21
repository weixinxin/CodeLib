using System;
using System.Collections.Generic;
namespace Framework
{
    /// <summary>
    /// 设置管理，可开启对设置数据缓存,避免频繁读写
    /// </summary>
    public class SettingManager : FrameworkModule<SettingManager>
    {
        
        private ISettingHelper mSettingHelper;

        private bool mIsDirty = false;
        
        private Dictionary<string, string> mStringCache;
        private Dictionary<string, float> mFloatCache;
        private Dictionary<string, int> mIntCache;

        private bool m_CacheData = false;

        private SettingManager() { }

        /// <summary>
        /// 设置配置辅助器。
        /// </summary>
        /// <param name="settingHelper">配置辅助器。</param>
        public void SetSettingHelper(ISettingHelper settingHelper)
        {
            if (settingHelper == null)
            {
                throw new Exception("Setting helper is invalid.");
            }
            mSettingHelper = settingHelper;
        }

        internal override void Update(float deltaTime, float unscaledDeltaTime){}
        internal override void LateUpdate(float deltaTime, float unscaledDeltaTime){}

        internal override void OnDestroy()
        {
            Save();
            if (m_CacheData)
            {
                mStringCache.Clear();
                mFloatCache.Clear();
                mIntCache.Clear();
            }
        }

        internal override void OnInit(params object[] args)
        {
            int capacity = 16;
            if (args.Length > 1)
            {
                m_CacheData = (bool)args[1];
            }
            if (args.Length > 0)
            {
                ISettingHelper helper = args[0] as ISettingHelper;
                SetSettingHelper(helper);
                Load();
            }
            if(m_CacheData)
            {
                mStringCache = new Dictionary<string, string>(capacity);
                mFloatCache = new Dictionary<string, float>(capacity);
                mIntCache = new Dictionary<string, int>(capacity);
            }
        }



        /// <summary>
        /// 加载配置。
        /// </summary>
        /// <returns>是否加载配置成功。</returns>
        public bool Load()
        {
            return mSettingHelper.Load();
        }

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <returns>是否保存配置成功。</returns>
        public bool Save()
        {
            if(m_CacheData)
            {
                if (mIsDirty)
                {
                    foreach (var kv in mIntCache)
                    {
                        mSettingHelper.SetInt(kv.Key, kv.Value);
                    }
                    foreach (var kv in mFloatCache)
                    {
                        mSettingHelper.SetFloat(kv.Key, kv.Value);
                    }
                    foreach (var kv in mStringCache)
                    {
                        mSettingHelper.SetString(kv.Key, kv.Value);
                    }
                    mIsDirty = false;
                    return mSettingHelper.Save();
                }
                return true;
            }
            else
            {
                return mSettingHelper.Save();
            }
        }

        /// <summary>
        /// 检查是否存在指定配置项
        /// </summary>
        /// <param name="key">要检查配置项的名称</param>
        /// <returns>指定的配置项是否存在</returns>
        public bool HasSetting(string key)
        {
            return mSettingHelper.HasKey(key);
        }

        /// <summary>
        /// 移除指定配置项
        /// </summary>
        /// <param name="key">要移除配置项的名称</param>
        public void DeleteSetting(string key)
        {
            mSettingHelper.DeleteKey(key);
            if (m_CacheData)
            {
                if (!mIntCache.Remove(key))
                {
                    if (!mFloatCache.Remove(key))
                    {
                        mStringCache.Remove(key);
                    }
                }
                mIsDirty = true;

            }
        }

        /// <summary>
        /// 移除所有配置项
        /// </summary>
        public void DeleteAll()
        {
            mSettingHelper.DeleteAll();
            if (m_CacheData)
            {
                mStringCache.Clear();
                mFloatCache.Clear();
                mIntCache.Clear();
                mIsDirty = true;
            }
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值
        /// </summary>
        /// <param name="key">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的浮点数值</returns>
        public float GetFloat(string key, float defaultValue = 0)
        {
            if (m_CacheData)
            {
                if (!mFloatCache.TryGetValue(key, out float res))
                {
                    res = mSettingHelper.GetFloat(key, defaultValue);
                    mFloatCache.Add(key, res);
                }
                return res;
            }
            else
            {
                return mSettingHelper.GetFloat(key, defaultValue);
            }
        }

        /// <summary>
        /// 从指定配置项中读取整数值
        /// </summary>
        /// <param name="key">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的整数值</returns>
        public int GetInt(string key, int defaultValue = 0)
        {
            if (m_CacheData)
            {
                if (!mIntCache.TryGetValue(key, out int res))
                {
                    res = mSettingHelper.GetInt(key, defaultValue);
                    mIntCache.Add(key, res);
                }
                return res;
            }
            else
            {
                return mSettingHelper.GetInt(key, defaultValue);
            }
        }

        /// <summary>
        /// 从指定配置项中读取布尔值
        /// </summary>
        /// <param name="key">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的布尔值</returns>
        public bool GetBool(string key, bool defaultValue = false)
        {
            if (m_CacheData)
            {
                if (!mIntCache.TryGetValue(key, out int res))
                {
                    res = mSettingHelper.GetInt(key, defaultValue ? 1 : 0);
                    mIntCache.Add(key, res);
                }
                return res != 0;
            }
            else
            {
                return mSettingHelper.GetInt(key, defaultValue ? 1 : 0) != 0;
            }
        }

        /// <summary>
        /// 从指定配置项中读取字符串值
        /// </summary>
        /// <param name="key">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的字符串值</returns>
        public string GetString(string key, string defaultValue = "")
        {
            if (m_CacheData)
            {
                if (!mStringCache.TryGetValue(key, out string res))
                {
                    res = mSettingHelper.GetString(key, defaultValue);
                    mStringCache.Add(key, res);
                }
                return res;
            }
            else
            {
                return mSettingHelper.GetString(key, defaultValue);
            }
        }

        /// <summary>
        /// 向指定配置项写入整数值
        /// </summary>
        /// <param name="key">要写入配置项的名称</param>
        /// <param name="value">要写入的整数值</param>
        public void SetInt(string key, int value)
        {
            if (m_CacheData)
            {
                if (mIntCache.ContainsKey(key))
                {
                    mIntCache[key] = value;
                }
                else
                {
                    mIntCache.Add(key, value);
                }
                mIsDirty = true;
            }
            else
            {
                mSettingHelper.SetInt(key, value);
            }
        }

        /// <summary>
        /// 向指定配置项写入布尔值
        /// </summary>
        /// <param name="key">要写入配置项的名称</param>
        /// <param name="value">要写入的布尔值</param>
        public void SetBool(string key, bool value)
        {
            if (m_CacheData)
            {
                if (mIntCache.ContainsKey(key))
                {
                    mIntCache[key] = value ? 1 : 0;
                }
                else
                {
                    mIntCache.Add(key, value ? 1 : 0);
                }
                mIsDirty = true;
            }
            else
            {
                mSettingHelper.SetInt(key, value ? 1 : 0);
            }
        }

        /// <summary>
        /// 向指定配置项写入浮点数值
        /// </summary>
        /// <param name="key">要写入配置项的名称</param>
        /// <param name="value">要写入的浮点数值</param>
        public void SetFloat(string key, float value)
        {
            if (m_CacheData)
            {
                if (mFloatCache.ContainsKey(key))
                {
                    mFloatCache[key] = value;
                }
                else
                {
                    mFloatCache.Add(key, value);
                }
                mIsDirty = true;
            }
            else
            {
                mSettingHelper.SetFloat(key, value);
            }
        }

        /// <summary>
        /// 向指定配置项写入字符串值
        /// </summary>
        /// <param name="key">要写入配置项的名称</param>
        /// <param name="value">要写入的字符串值</param>
        public void SetString(string key, string value)
        {
            if (m_CacheData)
            {
                if (mStringCache.ContainsKey(key))
                {
                    mStringCache[key] = value;
                }
                else
                {
                    mStringCache.Add(key, value);
                }
                mIsDirty = true;
            }
            else
            {
                mSettingHelper.SetString(key, value);
            }
        }
    }
}
