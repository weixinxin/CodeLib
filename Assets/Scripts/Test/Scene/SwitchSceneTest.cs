﻿using System.Collections;
using System.Collections.Generic;
using Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class SwitchSceneTest : MonoBehaviour
{
    public enum LoadMode
    {
        AB,
        Res,
        Editor
    }
    [OnValueChanged("SetLoader")]
    public LoadMode mode;
    
    public string SceneName;
    
    static bool inited = false;
    [Button("Switch")]
    void SwitchScene()
    {
        SceneManager.Instance.SwitchScene(SceneName);
        //UIManager.Instance.OpenUI("TestPanelAf");
    }

    [Button("Load")]
    void Load()
    {
        TextAsset text = ResourceManager.Instance.LoadAsset<TextAsset>("Assets/LuaScripts/ExamplePanel.lua");
        UnityEngine.Debug.Log(text.text);
    }

    private void Awake()
    {
        if(!inited)
        {
            inited = true;
            Framework.Debug.SetLogger(new Logger());
            GameFramework.Register(new ResourceManager());
            GameFramework.Register(new UIManager());
            GameFramework.Register(new SceneManager());
            SetLoader();
            GameObject prefab = ResourceManager.Instance.LoadAsset<GameObject>("Assets/Scripts/Test/Scene/Curtain/SceneCurtain.prefab");
            GameObject obj =  GameObject.Instantiate(prefab);
            DefaultSceneCurtain sceneCurtain = obj.GetComponent<DefaultSceneCurtain>();
            DontDestroyOnLoad(obj);
            SceneManager.Instance.SetDefaultSceneCurtain(sceneCurtain);
        }
    }

    void SetLoader()
    {
        if(mode == LoadMode.AB)
        {
            AssetBundleLoader loader = new AssetBundleLoader();
            loader.SetDataPath(Application.dataPath.Replace("Assets","AseetBundles"));
            ResourceManager.Instance.SetAssetLoader(loader);
        }
        else if (mode == LoadMode.Res)
        {
            ResourceManager.Instance.SetAssetLoader(new ResourceLoader());
        }
#if UNITY_EDITOR
        else if (mode == LoadMode.Editor)
        {
            ResourceManager.Instance.SetAssetLoader(new EditorLoader());
        }
#endif

    }

    private void Update()
    {
        GameFramework.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }
    private void LateUpdate()
    {
        GameFramework.LateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }
}
