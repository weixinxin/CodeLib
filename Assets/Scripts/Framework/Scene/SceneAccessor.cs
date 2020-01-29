using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public abstract partial class BaseScene
    {
        public abstract class SceneAccessor : FrameworkModule<SceneManager>
        {
            public string GetURL(BaseScene scene)
            {
                return scene.url;
            }

            public IEnumerator InvokeEnter(BaseScene scene, params object[] args)
            {
                scene.isActive = true;
                yield return scene.OnEnter(args);
            }

            public void InvokeUpdate(BaseScene scene, float elapseSeconds, float realElapseSeconds)
            {
                scene.OnUpdate(elapseSeconds);
            }

            public void InvokeLateUpdate(BaseScene scene, float elapseSeconds, float realElapseSeconds)
            {
                scene.OnLateUpdate(elapseSeconds);
            }

            public IEnumerator InvokeExit(BaseScene scene)
            {
                scene.isActive = false;
                yield return scene.OnExit();
            }

        }
    }
}
