
using System;

namespace Framework
{
    public abstract partial class FrameworkModuleBase
    {
        public virtual int Priority
        {
            get
            {
                return 0;
            }
        }
        protected virtual void LateUpdate(float deltaTime, float unscaledDeltaTime) { }

        protected abstract void OnDestroy();

        protected virtual void Update(float deltaTime, float unscaledDeltaTime) { }

        protected virtual void FixedUpdate(float deltaTime, float unscaledDeltaTime) { }

        
    }

    public abstract class FrameworkModule<T> : FrameworkModuleBase where T : FrameworkModuleBase
    {
        protected static T sInstance = null;

        public static T Instance
        {
            get
            {
                if(sInstance == null)
                {
                    sInstance = GameFramework.GetModule<T>();
                    if (sInstance == null)
                    {
                        //报错
                        throw new Exception(string.Format("you should call GameFramework.Register({0}) first!!", typeof(T).Name));
                    }
                }
                return sInstance;
            }
        }
        
    }
}
