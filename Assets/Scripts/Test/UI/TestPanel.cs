using System;
using System.Collections.Generic;

using Framework;

public abstract class TestPanel : BasePanel
{
    TestPanelView m_View;

    public abstract void OnOpenFullBtnClick();

    public abstract void OnOpenPartialBtnClick();

    void OnCloseBtnClick()
    {
        UIManager.Instance.CloseUI(UIName);
    }

    protected override void OnAwake(params object[] userData)
    {
        m_View = gameObject.GetComponent<TestPanelView>();
        m_View.BackButton.onClick.AddListener(OnCloseBtnClick);
        m_View.FullButton.onClick.AddListener(OnOpenFullBtnClick);
        m_View.PartialButton.onClick.AddListener(OnOpenPartialBtnClick);
        Debug.LogFormat("{0} OnAwake", UIName);
        gameObject.name += UIName;
    }

    protected override void OnClose()
    {
        Debug.LogFormat("{0} OnClose",UIName);
    }

    protected override void OnCover()
    {
        Debug.LogFormat("{0} OnCover", UIName);
        gameObject.SetActive(false);
    }

    protected override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
    {
        
    }

    protected override void OnReveal()
    {
        Debug.LogFormat("{0} OnReveal", UIName);
        gameObject.SetActive(true);
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        
    }
}

public class TestPanelAf : TestPanel
{
    public override bool isFullScreen => true;

    protected override string url => "Assets/Scripts/Test/UI/Prefab/FullPanel.prefab";

    public override void OnOpenFullBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelBf");
    }

    public override void OnOpenPartialBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelBs");
    }
}

public class TestPanelAs : TestPanel
{
    public override bool isFullScreen => false;

    protected override string url => "Assets/Scripts/Test/UI/Prefab/PartialPanel.prefab";

    public override void OnOpenFullBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelBf");
    }

    public override void OnOpenPartialBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelBs");
    }
}

public class TestPanelBf : TestPanel
{
    public override bool isFullScreen => true;

    protected override string url => "Assets/Scripts/Test/UI/Prefab/FullPanel.prefab";

    public override void OnOpenFullBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelCf");
    }

    public override void OnOpenPartialBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelCs");
    }
}

public class TestPanelBs : TestPanel
{
    public override bool isFullScreen => false;

    protected override string url => "Assets/Scripts/Test/UI/Prefab/PartialPanel.prefab";

    public override void OnOpenFullBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelCf");
    }

    public override void OnOpenPartialBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelCs");
    }
}
public class TestPanelCf : TestPanel
{
    public override bool isFullScreen => true;

    protected override string url => "Assets/Scripts/Test/UI/Prefab/FullPanel.prefab";

    public override void OnOpenFullBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelDf");
    }

    public override void OnOpenPartialBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelDs");
    }
}

public class TestPanelCs : TestPanel
{
    public override bool isFullScreen => false;

    protected override string url => "Assets/Scripts/Test/UI/Prefab/PartialPanel.prefab";

    public override void OnOpenFullBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelDf");
    }

    public override void OnOpenPartialBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelDs");
    }
}
public class TestPanelDf : TestPanel
{
    public override bool isFullScreen => true;

    protected override string url => "Assets/Scripts/Test/UI/Prefab/FullPanel.prefab";

    public override void OnOpenFullBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelAf");
    }

    public override void OnOpenPartialBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelAs");
    }
}

public class TestPanelDs : TestPanel
{
    public override bool isFullScreen => false;

    protected override string url => "Assets/Scripts/Test/UI/Prefab/PartialPanel.prefab";

    public override void OnOpenFullBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelAf");
    }

    public override void OnOpenPartialBtnClick()
    {
        UIManager.Instance.OpenUI("TestPanelAs");
    }
}