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
