﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class AssetBundleLoader : IAssetLoader
    {
        private class LoadAssetTask<T> : IAsyncTask where T : UnityEngine.Object
        {
            public IAsyncTask bundleTask;
            AssetBundleRequest mRequest;
            Action<bool, T> mCallback;
            string mAssetName;
            string mBundleName;
            AssetBundleHelper mAssetBundleHelper;
            public LoadAssetTask(string assetName, string bundleName, AssetBundleHelper helper, Action<bool, T> action)
            {
                mAssetName = assetName;
                mBundleName = bundleName;
                mCallback = action;
                mAssetBundleHelper = helper;
            }

            public TaskStatus Status { get; private set; }

            public bool Cancel()
            {
                if (Status == TaskStatus.Running)
                {
                    if (bundleTask.Cancel())
                    {
                        mAssetBundleHelper.Unload(mBundleName);
                    }
                    Status = TaskStatus.Canceled;
                    mCallback = null;
                    return true;
                }
                return false;
            }

            public void OnAssetBundleLoadCompleted(AssetBundle assetBundle)
            {
                if (Status != TaskStatus.Canceled)
                {
                    mRequest = assetBundle.LoadAssetAsync<T>(mAssetName);
                    mRequest.completed += OnCompleted;
                }
                else
                {
                    mAssetBundleHelper.Unload(mBundleName);
                }
            }

            private void OnCompleted(AsyncOperation asyncOperation)
            {
                if (Status != TaskStatus.Canceled)
                {
                    Status = TaskStatus.Completed;
                    if (mCallback != null)
                    {
                        T asset = mRequest.asset as T;
                        mCallback?.Invoke(asset != null, asset);
                    }
                }
                mAssetBundleHelper.Unload(mBundleName);
            }
        }
        AssetBundleHelper mAssetBundleHelper;

        private Dictionary<string, string> mAssetManifest;
        public AssetBundleLoader()
        {
            mAssetBundleHelper = new AssetBundleHelper(10, 20);
        }


        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            string assetBundleName = mAssetManifest[assetPath];
            AssetBundle assetBundle = mAssetBundleHelper.LoadAssetBundle(assetBundleName);
            return assetBundle.LoadAsset<T>(assetPath);
        }

        public IAsyncTask LoadAssetAsync<T>(string assetPath, Action<bool, T> callback) where T : UnityEngine.Object
        {
            string assetBundleName = mAssetManifest[assetPath];
            LoadAssetTask<T> assetTask = new LoadAssetTask<T>(assetPath, assetBundleName, mAssetBundleHelper, callback);
            IAsyncTask bundleTask = mAssetBundleHelper.LoadAssetBundleAsync(assetBundleName, assetTask.OnAssetBundleLoadCompleted);
            assetTask.bundleTask = bundleTask;
            return assetTask;
        }

        public void SetDataPath(string dataPath)
        {
            mAssetBundleHelper.SetDataPath(dataPath);
            byte[] bytes = File.ReadAllBytes($"{mAssetBundleHelper.dataPath}AssetManifest");
            mAssetManifest = AssetManifest.Import(bytes);
            //获取Platform
            string platform = "Windows";
            mAssetBundleHelper.LoadMainfest(platform);
        }
        
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="isAdditive"></param>
        public void LoadScene(string scenePath, bool isAdditive)
        {
            LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            string assetBundleName = mAssetManifest[scenePath];

            string[] split = scenePath.Replace('\\','/').Split('/');
            string sceneName = split[split.Length - 1].Replace(".unity", "");
            if (!UnityUtility.IsSceneInBuildSetting(sceneName))
            {
                AssetBundle assetBundle = mAssetBundleHelper.LoadAssetBundle(assetBundleName);
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, mode);
                mAssetBundleHelper.Unload(assetBundleName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, mode);
            }
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="isAdditive"></param>
        /// <returns></returns>
        public IEnumerator LoadSceneAsync(string scenePath, bool isAdditive)
        {
            LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            string assetBundleName = mAssetManifest[scenePath];
            string[] split = scenePath.Replace('\\', '/').Split('/');
            string sceneName = split[split.Length - 1].Replace(".unity", "");
            bool needLoadAssetBundle = !UnityUtility.IsSceneInBuildSetting(sceneName);
            if (needLoadAssetBundle)
            {
                IAsyncTask bundleTask = mAssetBundleHelper.LoadAssetBundleAsync(assetBundleName, null);
                while (bundleTask.Status != TaskStatus.Completed)
                {
                    yield return null;
                }
            }
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
            while (!operation.isDone)
            {
                yield return null;
            }
            if (needLoadAssetBundle)
            {
                mAssetBundleHelper.Unload(assetBundleName);
            }
        }

        public void Update(float deltaTime)
        {
            mAssetBundleHelper.Update();
        }

    }
}
