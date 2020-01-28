using System;
using System.Collections.Generic;

namespace Framework
{
    public partial class FsmManager : FrameworkModule<FsmManager>
    {
        private readonly List<FsmBase> mFsms = new List<FsmBase>();
        private readonly List<FsmBase> mTempFsms = new List<FsmBase>();
        internal override void OnInit(params object[] args)
        {

        }

        internal override void OnDestroy()
        {
            foreach(var fsm in mFsms)
            {
                fsm.Shutdown();
            }
            foreach (var fsm in mTempFsms)
            {
                fsm.Shutdown();
            }
            mFsms.Clear();
            mTempFsms.Clear();
        }

        internal override void Update(float deltaTime, float unscaledDeltaTime)
        {
            if (mTempFsms.Count > 0)
            {
                mFsms.AddRange(mTempFsms);
                mTempFsms.Clear();
            }

            if (mFsms.Count <= 0)
            {
                return;
            }
            for(int i =mFsms.Count -1;i >= 0;--i)
            {
                FsmBase fsm = mFsms[i];
                if (fsm.IsDestroyed)
                {
                    mFsms.RemoveAt(i);
                }
                else
                {
                    fsm.Update(deltaTime, unscaledDeltaTime);
                }
            }
        }

        
        public IFsm<T> CreateFsm<T>(string name, T owner, params FsmState<T>[] states) where T : class
        {
            Fsm<T> fsm = Fsm<T>.Create(name, owner, states);
            mTempFsms.Add(fsm);
            return fsm;
        }
    }
}
