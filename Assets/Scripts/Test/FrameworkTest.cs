using System;
using System.Collections.Generic;
using UnityEngine;
using Framework;


public class FrameworkTest : MonoBehaviour
{
    static class TestEvent
    {
        public const uint left = 1;
        public const uint right = 2;
        public const uint other = 3;
    }
    public class Logger : Framework.ILogger
    {
        public void Log(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void LogError(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }

        public void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }

        public void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        public void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }
    }
    class SettingHelper : ISettingHelper
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
            UnityEngine.Debug.LogFormat("GetFloat {0} defaultValue = {1}", key,defaultValue);
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


    private void Awake()
    {
        Framework.Debug.SetLogger(new Logger());
        SettingManager.Initialize(new SettingHelper(),true);
        EventManager.Initialize();
        ResourceManager.Initialize();

        EventManager.Instance.AddListener<string>(TestEvent.left, OnEvent);
        EventManager.Instance.AddListener<string>(TestEvent.right, OnEvent2);
        EventManager.Instance.AddListener(TestEvent.other, OnEvent3);
    }

    private void Update()
    {
        GameFramework.Update(Time.deltaTime, Time.unscaledDeltaTime);
        if(Input.anyKeyDown && Input.GetMouseButton(0))
        {
            EventManager.Instance.EnqueueEvent(TestEvent.left, "left");
        }
        if (Input.anyKeyDown && Input.GetMouseButton(1))
        {
            EventManager.Instance.TriggerEvent(TestEvent.right, "right");
        }
    }

    private void LateUpdate()
    {
        GameFramework.LateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener<string>(TestEvent.left, OnEvent);
        EventManager.Instance.RemoveListener<string>(TestEvent.right, OnEvent2);
        EventManager.Instance.RemoveListener(TestEvent.other, OnEvent3);
        GameFramework.Shutdown();
    }
    private int count
    {
        get
        {
            return SettingManager.Instance.GetInt("count", 0);
        }
        set
        {
            SettingManager.Instance.SetInt("count",value);
        }
    }

    void OnEvent(string arg)
    {
        Framework.Debug.Log("OnEvent" + arg);
        count++;
        EventManager.Instance.EnqueueEvent(TestEvent.other, EventArgs.Empty);
    }
    void OnEvent2(string arg)
    {
        Framework.Debug.Log("OnEvent2" + arg);
    }

    void OnEvent3(EventArgs arg)
    {
        Framework.Debug.Log("OnEvent3" + arg);
    }
}
