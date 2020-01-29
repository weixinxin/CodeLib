using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public abstract partial class BaseScene
    {
        public abstract class SceneAccessor : FrameworkModule<SceneManager>
        {
            protected string GetURL(BaseScene scene)
            {
                return scene.url;
            }

            protected IEnumerator InvokeEnter(BaseScene scene, params object[] args)
            {
                scene.isActive = true;
                yield return scene.OnEnter(args);
            }

            protected void InvokeUpdate(BaseScene scene, float elapseSeconds, float realElapseSeconds)
            {
                scene.OnUpdate(elapseSeconds);
            }

            protected void InvokeLateUpdate(BaseScene scene, float elapseSeconds, float realElapseSeconds)
            {
                scene.OnLateUpdate(elapseSeconds);
            }

            protected IEnumerator InvokeExit(BaseScene scene)
            {
                scene.isActive = false;
                yield return scene.OnExit();
            }

        }
    }
}
