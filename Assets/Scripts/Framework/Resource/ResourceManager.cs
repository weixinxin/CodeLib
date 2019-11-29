using System;
using System.Collections.Generic;
using System.IO;
namespace Framework
{
    public enum ResourceMode
    {
        Editor,
        AssetBundle,
        Resource,
    }
    public class ResourceManager : FrameworkModule<ResourceManager>
    {
        private ResourceManager() { }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        public ResourceMode mode
        {
            get;
            protected set;
        }

        private IAssetLoader loader;

        internal override void OnInit(params object[] args)
        {
            if (args.Length > 0)
            {
                mode = (ResourceMode)args[0];
            }
            else
            {
                mode = ResourceMode.Editor;
            }
            switch(mode)
            {
                case ResourceMode.Editor:
                    loader = new EditorLoader();
                    break;
                case ResourceMode.AssetBundle:
                    loader = new AssetBundleLoader();
                    break;
                case ResourceMode.Resource:
                    loader = new ResourceLoader();
                    break;
            }
            loader.Init();
        }

        internal override void OnDestroy()
        {
            Debug.Log("ResourceManager OnDestroy");
        }
        
        public void SetDataPath(string dataPath)
        {
            loader.SetDataPath(dataPath);
        }

        public T LoadAsset<T>(string dir, string assetName) where T : UnityEngine.Object
        {
            return loader.LoadAsset<T>(dir, assetName);
        }

        public IAsyncTask LoadAssetAsync<T>(string dir, string assetName, Action<bool, T> callback) where T : UnityEngine.Object
        {
            return loader.LoadAssetAsync(dir, assetName, callback);
        }
    }
}
