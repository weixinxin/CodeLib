using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Framework
{
    public class ResourceManager : FrameworkModule<ResourceManager>
    {
        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        private IAssetLoader loader;

        protected override void Update(float deltaTime, float unscaledDeltaTime)
        {
            loader.Update(deltaTime);
        }

        protected override void OnDestroy()
        {
            Debug.Log("ResourceManager OnDestroy");
        }

        public void SetAssetLoader(IAssetLoader assetLoader)
        {
            loader = assetLoader;
        }

        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return loader.LoadAsset<T>(assetPath);
        }

        public IAsyncTask LoadAssetAsync<T>(string assetPath, Action<bool, T> callback) where T : UnityEngine.Object
        {
            return loader.LoadAssetAsync(assetPath, callback);
        }

        public void LoadScene(string scenePath, bool isAdditive = false)
        {
            loader.LoadScene(scenePath, isAdditive);
        }

        public IEnumerator LoadSceneAsync(string scenePath, bool isAdditive = false)
        {
            yield return loader.LoadSceneAsync(scenePath, isAdditive);
        }

    }
}
