namespace Framework
{
    public interface ILuaInterface
    {
        object LuaEnv { get; }
        void OnDestroy();
        void Update(float deltaTime, float unscaledDeltaTime);
        void LateUpdate(float deltaTime, float unscaledDeltaTime);
        void FixedUpdate(float deltaTime, float unscaledDeltaTime);
        object[] DoString(string chunk, string chunkName);
        T Get<T>(string path);
        void Set<T>(string path, T val);
        void GC();
    }
}
