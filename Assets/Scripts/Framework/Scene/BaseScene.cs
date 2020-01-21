using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{

    public abstract class BaseScene
    {
        public bool isActive { get; protected set; }

        protected abstract string url{ get; }

        public IEnumerator Enter(params object[] args)
        {
            isActive = true;
            yield return ResourceManager.Instance.LoadSceneAsync(url);
            yield return OnEnter(args);
        }

        public IEnumerator Exit()
        {
            isActive = false;
            yield return OnExit();
        }

        public abstract IEnumerator OnEnter(params object[] args);

        public virtual void Update(float dt) { }

        public virtual void LateUpdate(float dt) { }

        public abstract IEnumerator OnExit();
    }
}
