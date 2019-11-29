
using System;

namespace Framework
{
    public abstract class FrameworkModuleBase
    {
        public virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        internal abstract void OnInit(params object[] args);

        internal virtual void LateUpdate(float deltaTime, float unscaledDeltaTime) { }

        internal abstract void OnDestroy();

        internal virtual void Update(float deltaTime, float unscaledDeltaTime) { }
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
                    //报错
                    throw new Exception(string.Format( "you should call {0}.Initialize() first!!" , typeof(T).Name));
                }
                return sInstance;
            }
        }

        public static void Initialize(params object[] args)
        {
            if (null == sInstance)
            {
                sInstance = Activator.CreateInstance(typeof(T), true) as T;
                GameFramework.RegisterModule(sInstance);
                sInstance.OnInit(args);
            }
        }

    }
}
