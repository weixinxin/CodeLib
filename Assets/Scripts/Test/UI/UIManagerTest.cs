using System.Collections;
using System.Collections.Generic;
using Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIManagerTest : MonoBehaviour
{
    public string PanelName;

    [Button("Open")]
    void Open()
    {
        UIManager.Instance.OpenUI(PanelName);
    }

    [Button("Close")]
    void Close()
    {
        UIManager.Instance.CloseUI(PanelName);
    }

    [Button("CloseAll")]
    void CloseAll()
    {
        UIManager.Instance.CloseAll();
    }

    [Button("HasUI")]
    void HasUI()
    {
        UnityEngine.Debug.LogFormat("HasUI {0} = {1}", PanelName, UIManager.Instance.HasUI(PanelName));
    }

    [Button("TryGetUI")]
    void TryGetUI()
    {
        if(UIManager.Instance.TryGetUI(PanelName,out BasePanel panle))
        {
            UnityEngine.Debug.LogFormat("TryGetUI {0} = true,isFullScreen = {1}", PanelName, panle.isFullScreen);
        }
        else
        {
            UnityEngine.Debug.LogFormat("TryGetUI {0} = false", PanelName);
        }
    }

    [Button("GetUI")]
    void GetUI()
    {
        BasePanel panle = UIManager.Instance.GetUI(PanelName);
        if (panle != null)
        {
            UnityEngine.Debug.LogFormat("GetUI {0} = true,isFullScreen = {1}", PanelName, panle.isFullScreen);
        }
        else
        {
            UnityEngine.Debug.LogFormat("GetUI {0} = false", PanelName);
        }
    }
}
