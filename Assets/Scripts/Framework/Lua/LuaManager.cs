using System;
using System.Collections.Generic;

namespace Framework
{
    public class LuaManager : FrameworkModule<LuaManager>
    {
        private ILuaInterface m_LuaInterface;
        public LuaManager(ILuaInterface lua)
        {
            m_LuaInterface = lua;
        }

        protected override void OnDestroy()
        {
            m_LuaInterface.OnDestroy();
        }

        protected override void Update(float deltaTime, float unscaledDeltaTime)
        {
            m_LuaInterface.Update(deltaTime, unscaledDeltaTime);
        }

        protected override void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            m_LuaInterface.LateUpdate(deltaTime, unscaledDeltaTime);
        }

        protected override void FixedUpdate(float deltaTime, float unscaledDeltaTime)
        {
            m_LuaInterface.FixedUpdate(deltaTime, unscaledDeltaTime);
        }

        public T GetLuaEnv<T>()where T:class
        {
            return m_LuaInterface.LuaEnv as T;
        }

        public object[] DoString(string chunk, string chunkName)
        {
            return m_LuaInterface.DoString(chunk, chunkName);
        }

        public T Get<T>(string path)
        {
            return m_LuaInterface.Get<T>(path);
        }

        public void Set<T>(string path, T val)
        {
            m_LuaInterface.Set(path, val);
        }

        public void GC()
        {
            m_LuaInterface.GC();
        }
    }
}
