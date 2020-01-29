using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static Framework.BaseScene;

namespace Framework
{
    /// <summary>
    /// 场景管理
    /// </summary>
    public class SceneManager : SceneAccessor
    {
        /// <summary>
        /// 当前场景
        /// </summary>
        public BaseScene CurrentScene { get; private set; }

        /// <summary>
        /// 是否转场中
        /// </summary>
        public bool IsSwitching
        {
            get
            {
                return m_RunningCoroutine != null;
            }
        }

        private bool m_IsDestroyingScene;

        /// <summary>
        /// 销毁场景阶段,谨慎处理该阶段的创建操作
        /// </summary>
        public bool IsDestroyingScene
        {
            get
            {
                return IsSwitching && m_IsDestroyingScene;
            }
        }

        private List<BaseScene> m_Scenes = new List<BaseScene>();

        private Coroutine m_RunningCoroutine;

        /// <summary>
        /// 场景帷幕
        /// </summary>
        private ISceneCurtain m_DefaultCurtain;

        internal override void OnInit(params object[] args)
        {
            m_RunningCoroutine = null;
        }

        internal override void OnDestroy()
        {
            m_RunningCoroutine = null;
        }

        internal override void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (m_RunningCoroutine != null && !m_RunningCoroutine.Update())
            {
                m_RunningCoroutine = null;
            }
            if (CurrentScene != null && CurrentScene.isActive)
                InvokeUpdate(CurrentScene, deltaTime, unscaledDeltaTime);
        }

        internal override void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (CurrentScene != null && CurrentScene.isActive)
                InvokeLateUpdate(CurrentScene, deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// 设置默认帷幕
        /// </summary>
        /// <param name="curtain"></param>
        public void SetDefaultSceneCurtain(ISceneCurtain curtain)
        {
            m_DefaultCurtain = curtain;
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="sceneType">指定的场景类名</param>
        /// <param name="args">初始化参数</param>
        public void SwitchScene(string sceneType, params object[] args)
        {
            Type type = Type.GetType(sceneType);
            if (type == null)
                throw new Exception(string.Format("Can not found scene class {0}", sceneType));
            SwitchScene(type, m_DefaultCurtain, args);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="sceneType">指定的场景类名</param>
        /// <param name="curtain">指定的帷幕</param>
        /// <param name="args">初始化参数</param>
        public void SwitchScene(string sceneType, ISceneCurtain curtain, params object[] args)
        {
            Type type = Type.GetType(sceneType);
            if (type == null)
                throw new Exception(string.Format("Can not found scene class {0}", sceneType));
            SwitchScene(type, curtain, args);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <typeparam name="T">指定的场景类型</typeparam>
        /// <param name="args">初始化参数</param>
        public void SwitchScene<T>(params object[] args) where T : BaseScene
        {
            Type type = typeof(T);
            SwitchScene(type, m_DefaultCurtain, args);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <typeparam name="T">指定的场景类型</typeparam>
        /// <param name="curtain">指定的帷幕</param>
        /// <param name="args">初始化参数</param>
        public void SwitchScene<T>(ISceneCurtain curtain, params object[] args) where T : BaseScene
        {
            Type type = typeof(T);
            SwitchScene(type, curtain, args);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="type">指定的场景类型</param>
        /// <param name="curtain">指定的帷幕</param>
        /// <param name="args">初始化参数</param>
        public void SwitchScene(Type type, ISceneCurtain curtain, params object[] args)
        {
            if (m_RunningCoroutine != null)
                throw new Exception("The last operation is still in progress !");
            BaseScene nextScene = null;
            foreach (var scene in m_Scenes)
            {
                if (scene.GetType() == type)
                {
                    nextScene = scene;
                    break;
                }
            }
            if (nextScene == null)
            {
                nextScene = (BaseScene)Activator.CreateInstance(type);
            }
            m_RunningCoroutine = new Coroutine(SwitchScene(CurrentScene, nextScene, curtain == null ? m_DefaultCurtain : curtain, args));
        }



        IEnumerator SwitchScene(BaseScene origScene, BaseScene nextScene, ISceneCurtain curtain, params object[] args)
        {
            if (origScene == nextScene)
                yield break;
            m_IsDestroyingScene = true;
            //落幕
            if (curtain != null)
                yield return curtain.Falls();
            //关闭现有界面
            UIManager uiManager = GameFramework.GetModule<UIManager>();
            if (uiManager != null)
                uiManager.CloseAll();
            //卸载旧场景
            if (origScene != null)
                yield return InvokeExit(origScene);
            //加载新场景
            yield return ResourceManager.Instance.LoadSceneAsync(GetURL(nextScene));
            m_IsDestroyingScene = false;
            yield return InvokeEnter(nextScene, args);
            CurrentScene = nextScene;
            //揭幕
            if (curtain != null)
                yield return curtain.Raise();
        }
    }
}
