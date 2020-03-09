using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;

namespace Framework
{
    public sealed class LuaPanel : BasePanel
    {
        [CSharpCallLua]
        public delegate void LuaPanelAction();
        [CSharpCallLua]
        public delegate void LuaPanelAwake(Transform root, object[] args);
        [CSharpCallLua]
        public delegate void LuaPanelUpdate(float elapseSeconds, float realElapseSeconds);


        private bool m_IsFullScreen;
        public override bool isFullScreen => m_IsFullScreen;

        private string m_Url;
        protected override string url => m_Url;

        private LuaPanelAction m_OnCover;
        private LuaPanelAction m_OnReveal;
        private LuaPanelUpdate m_OnLateUpdate;
        private LuaPanelUpdate m_OnUpdate;

        protected override void OnInit()
        {
            m_Url = LuaManager.Instance.Get<string>(UIName + ".url");
            m_IsFullScreen = LuaManager.Instance.Get<bool>(UIName + ".isFullScreen");
            m_OnCover = LuaManager.Instance.Get<LuaPanelAction>(UIName + ".OnCover");
        }

        protected override void OnAwake(params object[] userData)
        {
            LuaPanelAwake awake = LuaManager.Instance.Get<LuaPanelAwake>(UIName + ".OnAwake");
            awake(gameObject.transform, userData);
        }

        protected override void OnClose()
        {
            LuaManager.Instance.Get<LuaPanelAction>(UIName + ".OnClose")?.Invoke();
        }

        protected override void OnCover()
        {
            m_OnCover?.Invoke();
        }

        protected override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_OnLateUpdate?.Invoke(elapseSeconds, realElapseSeconds);
        }

        protected override void OnReveal()
        {
            m_OnReveal?.Invoke();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_OnUpdate?.Invoke(elapseSeconds, realElapseSeconds);
        }
    }
}
