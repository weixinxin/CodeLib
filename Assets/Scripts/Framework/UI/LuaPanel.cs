using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
namespace Framework
{
    public sealed class LuaPanel : BasePanel
    {
        public override bool isFullScreen => throw new NotImplementedException();

        protected override string url => throw new NotImplementedException();

        protected override void OnAwake(params object[] userData)
        {
            throw new NotImplementedException();
        }

        protected override void OnClose()
        {
            throw new NotImplementedException();
        }

        protected override void OnCover()
        {
            throw new NotImplementedException();
        }

        protected override void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {
            throw new NotImplementedException();
        }

        protected override void OnReveal()
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            throw new NotImplementedException();
        }
    }
}
