using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework
{
    public abstract partial class BasePanel
    {

        public abstract class PanelAccessor: FrameworkModule<UIManager>
        {

            public string GetURL(BasePanel panel)
            {
                return panel.url;
            }

            protected void InvokeInit(BasePanel panel, string name)
            {
                panel.m_Valid = true;
                panel.UIName = name;
                panel.OnInit();
            }

            protected void InvokeAwake(BasePanel panel, GameObject obj, params object[] userData)
            {
                panel.gameObject = obj;
                panel.OnAwake(userData);
            }

            protected void InvokeClose(BasePanel panel)
            {
                if(panel.m_Valid)
                {
                    panel.OnClose();
                    panel.m_Valid = false;
                }
            }

            protected void InvokeCover(BasePanel panel)
            {
                if (panel.m_Valid)
                    panel.OnCover();
            }

            protected void InvokeReveal(BasePanel panel)
            {
                if (panel.m_Valid)
                    panel.OnReveal();
            }

            protected void InvokeUpdate(BasePanel panel, float elapseSeconds, float realElapseSeconds)
            {
                if (panel.m_Valid)
                    panel.OnUpdate(elapseSeconds, realElapseSeconds);
            }

            protected void InvokeLateUpdate(BasePanel panel, float elapseSeconds, float realElapseSeconds)
            {
                if (panel.m_Valid)
                    panel.OnLateUpdate(elapseSeconds, realElapseSeconds);
            }

            protected void SetVisible(BasePanel panel,bool visible)
            {
                panel.m_Visible = visible;
            }
            
        }
    }
}