﻿using System;
using System.Collections.Generic;
using System.IO;
namespace Framework
{
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
        
        private IAssetLoader loader;

        internal override void OnInit(params object[] args)
        {

        }

        internal override void OnDestroy()
        {
            Debug.Log("ResourceManager OnDestroy");
        }
        
        public void SetAssetLoader(IAssetLoader assetLoader)
        {
            loader = assetLoader;
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
