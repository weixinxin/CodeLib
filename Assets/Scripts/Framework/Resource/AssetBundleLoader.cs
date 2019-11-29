using System;
using UnityEngine;

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
            public LoadAssetTask(string assetName, string bundleName, AssetBundleHelper helper,Action<bool, T> action)
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
                    if(bundleTask.Cancel())
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
        public void Init()
        {
            mAssetBundleHelper = new AssetBundleHelper(10,20);
        }

        public T LoadAsset<T>(string dir, string assetName) where T : UnityEngine.Object
        {
            string assetBundleName = FormatAssetBundleName(dir);
            AssetBundle assetBundle = mAssetBundleHelper.LoadAssetBundle(assetBundleName);
            return assetBundle.LoadAsset<T>(assetName);
        }

        public IAsyncTask LoadAssetAsync<T>(string dir, string assetName, Action<bool, T> callback) where T : UnityEngine.Object
        {
            string assetBundleName = FormatAssetBundleName(dir);
            LoadAssetTask<T> assetTask = new LoadAssetTask<T>(assetName, assetBundleName, mAssetBundleHelper,callback);
            IAsyncTask bundleTask = mAssetBundleHelper.LoadAssetBundleAsync(assetBundleName, assetTask.OnAssetBundleLoadCompleted);
            assetTask.bundleTask = bundleTask;
            return assetTask;
        }

        public void SetDataPath(string dataPath)
        {
            mAssetBundleHelper.SetDataPath(dataPath);
            //获取Platform
            string platform = "Windows";
            mAssetBundleHelper.LoadMainfest(platform);
        }

        string FormatAssetBundleName(string dir)
        {
            string assetBundleName = dir.Replace('/', '-');
            assetBundleName = assetBundleName.Replace('\\', '-');
            return assetBundleName;
        }
    }
}
