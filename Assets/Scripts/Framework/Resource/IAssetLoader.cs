using System;
using System.Collections;

namespace Framework
{

    public interface IAssetLoader
    {
        void SetDataPath(string dataPath);

        T LoadAsset<T>(string assetPath) where T : UnityEngine.Object;

        IAsyncTask LoadAssetAsync<T>(string assetPath, Action<bool, T> callback) where T : UnityEngine.Object;

        void LoadScene(string scenePath, bool isAdditive);

        IEnumerator LoadSceneAsync(string scenePath, bool isAdditive);

        void Update(float deltaTime);
    }
}
