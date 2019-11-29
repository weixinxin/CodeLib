using System;

namespace Framework
{

    public interface IAssetLoader
    {
        void Init();

        void SetDataPath(string dataPath);

        T LoadAsset<T>(string dir, string assetName) where T : UnityEngine.Object;

        IAsyncTask LoadAssetAsync<T>(string dir, string assetName, Action<bool,T> callback) where T : UnityEngine.Object;
    }
}
