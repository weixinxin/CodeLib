using UnityEngine;
using Framework;
public class SettingHelper : ISettingHelper
{
    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public float GetFloat(string key, float defaultValue)
    {
        UnityEngine.Debug.LogFormat("GetFloat {0} defaultValue = {1}", key, defaultValue);
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public int GetInt(string key, int defaultValue)
    {
        UnityEngine.Debug.LogFormat("GetInt {0} defaultValue = {1}", key, defaultValue);
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public string GetString(string key, string defaultValue)
    {
        UnityEngine.Debug.LogFormat("GetString {0} defaultValue = {1}", key, defaultValue);
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public bool Load()
    {
        return true;
    }

    public bool Save()
    {
        UnityEngine.Debug.Log("PlayerPrefs.Save()");
        PlayerPrefs.Save();
        return true;
    }

    public void SetFloat(string key, float value)
    {
        UnityEngine.Debug.LogFormat("SetFloat {0} = {1}", key, value);
        PlayerPrefs.SetFloat(key, value);
    }

    public void SetInt(string key, int value)
    {
        UnityEngine.Debug.LogFormat("SetInt {0} = {1}", key, value);
        PlayerPrefs.SetInt(key, value);
    }

    public void SetString(string key, string value)
    {
        UnityEngine.Debug.LogFormat("SetString {0} = {1}", key, value);
        PlayerPrefs.SetString(key, value);
    }
}