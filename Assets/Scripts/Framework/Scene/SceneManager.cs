using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
namespace Framework
{
    /// <summary>
    /// 场景管理
    /// </summary>
    public class SceneManager : FrameworkModule<SceneManager>
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
                return mRunningCoroutine != null;
            }
        }

        private List<BaseScene> mScenes = new List<BaseScene>();

        private Coroutine mRunningCoroutine;

        /// <summary>
        /// 场景帷幕
        /// </summary>
        private ISceneCurtain mSceneCurtain;

        internal override void OnInit(params object[] args)
        {
            mRunningCoroutine = null;
        }

        internal override void OnDestroy()
        {
            mRunningCoroutine = null;
        }

        internal override void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (mRunningCoroutine != null && !mRunningCoroutine.Update())
            {
                mRunningCoroutine = null;
            }
            if (CurrentScene != null && CurrentScene.isActive)
                CurrentScene.Update(deltaTime);
        }

        internal override void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (CurrentScene != null && CurrentScene.isActive)
                CurrentScene.LateUpdate(deltaTime);
        }

        /// <summary>
        /// 设置帷幕
        /// </summary>
        /// <param name="curtain"></param>
        public void SetSceneCurtain(ISceneCurtain curtain)
        {
            mSceneCurtain = curtain;
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="sceneType"></param>
        /// <param name="args"></param>
        public void SwitchScene(string sceneType, params object[] args)
        {
            Type type = Type.GetType(sceneType);
            if (type == null)
                throw new Exception(string.Format("Can not found scene class {0}", sceneType));
            SwitchScene(type, args);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void SwitchScene<T>(params object[] args) where T : BaseScene
        {
            Type type = typeof(T);
            SwitchScene(type, args);
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        public void SwitchScene(Type type, params object[] args)
        {
            if (mRunningCoroutine != null)
                throw new Exception("The last operation is still in progress !");
            BaseScene nextScene = null;
            foreach (var scene in mScenes)
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
            mRunningCoroutine = new Coroutine(SwitchScene(CurrentScene, nextScene, args));
        }



        IEnumerator SwitchScene(BaseScene origScene, BaseScene nextScene, params object[] args)
        {
            if (origScene == nextScene)
                yield break;
            //落幕
            yield return mSceneCurtain.Falls();
            //卸载旧场景
            if (origScene != null)
                yield return origScene.Exit();
            //加载新场景
            yield return nextScene.Enter(args);
            CurrentScene = nextScene;
            //揭幕
            yield return mSceneCurtain.Raise();
        }
    }
}
