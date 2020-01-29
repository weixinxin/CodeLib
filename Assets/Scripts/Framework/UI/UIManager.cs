using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Framework.BasePanel;

namespace Framework
{
    public class UIManager : PanelAccessor
    {
        private List<BasePanel> m_Panels = new List<BasePanel>();
        private bool m_IsDirty = false;
        private List<BasePanel> m_UpdateList = new List<BasePanel>();

        public BasePanel TopPanel
        {
            get
            {
                if (m_Panels.Count > 0)
                {
                    return m_Panels[m_Panels.Count - 1];
                }
                return null;
            }
        }

        private GameObject m_Canvas;
        public Transform UIRoot
        {
            get
            {
                if(m_Canvas == null)
                {
                    m_Canvas = GameObject.Find("Canvas");
                }
                return m_Canvas.transform;
            }
        }

        internal override void OnInit(params object[] args)
        {

        }

        internal override void Update(float deltaTime, float unscaledDeltaTime)
        {
            foreach (BasePanel panel in m_UpdateList)
            {
                InvokeUpdate(panel, deltaTime, unscaledDeltaTime);
            }
        }

        internal override void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (BasePanel panel in m_UpdateList)
            {
                InvokeLateUpdate(panel, deltaTime, unscaledDeltaTime);
            }
            if (m_IsDirty)
            {
                m_IsDirty = false;
                //排序
                m_UpdateList.Clear();
                m_UpdateList.AddRange(m_Panels);
                for (int i = 0; i < m_UpdateList.Count; ++i)
                {
                    m_UpdateList[i].gameObject.transform.SetSiblingIndex(i);
                }
                bool visible = true;
                for (int i = m_UpdateList.Count - 1; i >= 0; --i)
                {
                    BasePanel panel = m_UpdateList[i];
                    if (panel.Visible != visible)
                    {
                        SetVisible(panel, visible);
                        if (visible)
                        {
                            InvokeReveal(panel);
                        }
                        else
                        {
                            InvokeCover(panel);
                        }
                    }
                    if (panel.isFullScreen)
                        visible = false;
                }
            }
        }

        internal override void OnDestroy()
        {
            m_IsDirty = false;
            m_Panels.Clear();
            m_UpdateList.Clear();
        }


        /// <summary>
        /// 打开界面，已存在的界面则置顶显示
        /// </summary>
        /// <param name="name">界面名</param>
        /// <param name="userData">用户自定义数据</param>
        public void OpenUI(string name, params object[] userData)
        {
            //检查是否处于场景销毁过程
            SceneManager sceneManager = GameFramework.GetModule<SceneManager>();
            if (sceneManager != null && sceneManager.IsDestroyingScene)
            {
                Debug.LogException(new InvalidOperationException("Scene being destroyed, operation invalid"));
                return;
            }

            //检查界面是否已存在
            for (int i = m_Panels.Count - 1; i >= 0; --i)
            {
                BasePanel _panel = m_Panels[i];
                if (_panel.UIName == name)
                {
                    m_Panels.RemoveAt(i);
                    m_Panels.Add(_panel);
                    m_IsDirty = true;
                    return;
                }
            }

            //创建新界面
            Type type = Type.GetType(name);
            if (type == null)
            {
                //LuaPanel
                type = typeof(LuaPanel);
            }
            else if (type == typeof(LuaPanel))
            {
                Debug.LogException(new NotSupportedException("Not Supported LuaPanel!"));
                return;
            }
            BasePanel panel = (BasePanel)Activator.CreateInstance(type);
            GameObject obj = InstantiatePanel(panel);
            m_Panels.Add(panel);
            InvokeAwake(panel, name, obj, userData);
            m_IsDirty = true;
        }

        /// <summary>
        /// 创建界面实体
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        GameObject InstantiatePanel(BasePanel panel)
        {
            string url = GetURL(panel);
            string path = url.Replace('\\', '/').TrimEnd('/');
            int index = path.LastIndexOf('/');
            if (index == -1)
            {
                var exception = new UriFormatException(string.Format("Invalid URI: The URI scheme is not valid - {0}", url));
                Debug.LogException(exception);
                throw exception;
            }
            string dir = path.Substring(0, index);
            string assetName = path.Substring(index + 1);
            GameObject prefab = ResourceManager.Instance.LoadAsset<GameObject>(dir, assetName);
            GameObject obj = UnityEngine.Object.Instantiate(prefab);
            obj.transform.SetParent(UIRoot,false);
            obj.transform.localScale = Vector3.one;
            return obj;
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="name">界面名称</param>
        public void CloseUI(string name)
        {
            for (int i = m_Panels.Count - 1; i >= 0; --i)
            {
                BasePanel panel = m_Panels[i];
                if (panel.UIName == name)
                {
                    InvokeClose(panel);
                    GameObject.Destroy(m_Panels[i].gameObject);
                    m_Panels.RemoveAt(i);
                    m_IsDirty = true;
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <typeparam name="T">界面类</typeparam>
        public void CloseUI<T>()
        {
            for (int i = m_Panels.Count - 1; i >= 0; --i)
            {
                BasePanel panel = m_Panels[i];
                if (panel is T)
                {
                    InvokeClose(panel);
                    GameObject.Destroy(m_Panels[i].gameObject);
                    m_Panels.RemoveAt(i);
                    m_IsDirty = true;
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭所有打开的界面
        /// </summary>
        public void CloseAll()
        {
            for (int i = m_Panels.Count - 1; i >= 0; --i)
            {
                InvokeClose(m_Panels[i]);
                GameObject.Destroy(m_Panels[i].gameObject);
            }
            m_Panels.Clear();
            m_IsDirty = true;
        }

        /// <summary>
        /// 检测是否存在指定界面
        /// </summary>
        /// <param name="name">界面名</param>
        /// <returns></returns>
        public bool HasUI(string name)
        {
            foreach (BasePanel _panel in m_Panels)
            {
                if (_panel.UIName == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检测是否存在指定界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <returns></returns>
        public bool HasUI<T>() where T : BasePanel
        {

            foreach (BasePanel _panel in m_Panels)
            {
                if (_panel is T)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定界面
        /// </summary>
        /// <param name="name">界面名称</param>
        /// <returns></returns>
        public BasePanel GetUI(string name)
        {
            foreach (BasePanel panel in m_Panels)
            {
                if (panel.UIName == name)
                    return panel;
            }
            return null;
        }

        /// <summary>
        /// 获取指定界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <returns></returns>
        public T GetUI<T>() where T : BasePanel
        {
            foreach (BasePanel _panel in m_Panels)
            {
                if (_panel is T)
                {
                    return _panel as T;
                }
            }
            return null;
        }

        /// <summary>
        /// 尝试获取指定界面
        /// </summary>
        /// <param name="name">界面名称</param>
        /// <param name="panel">返回的界面</param>
        /// <returns>是否存在</returns>
        public bool TryGetUI(string name, out BasePanel panel)
        {
            foreach (BasePanel _panel in m_Panels)
            {
                if (_panel.UIName == name)
                {
                    panel = _panel;
                    return true;
                }
            }
            panel = null;
            return false;
        }

        /// <summary>
        /// 尝试获取指定界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <param name="panel">返回的界面</param>
        /// <returns>是否存在</returns>
        public bool TryGetUI<T>(out T panel) where T : BasePanel
        {
            foreach (BasePanel _panel in m_Panels)
            {
                if (_panel is T)
                {
                    panel = _panel as T;
                    return true;
                }
            }
            panel = null;
            return false;
        }

    }
}
