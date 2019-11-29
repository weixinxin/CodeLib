using System;
using UnityEngine;

namespace Framework
{
    public class ResourceLoader : IAssetLoader
    {
        private class LoadTask<T>:IAsyncTask where T : UnityEngine.Object
        {
            ResourceRequest mRequest;
            Action<bool, T> mCallback;
            public LoadTask(ResourceRequest request, Action<bool, T> action)
            {
                mCallback = action;
                mRequest = request;
                request.completed += OnCompleted;
            }

            public TaskStatus Status { get; private set; }

            public bool Cancel()
            {
                if (Status == TaskStatus.Running)
                {
                    Status = TaskStatus.Canceled;
                    mCallback = null;
                    return true;
                }
                return false;
            }

            private void OnCompleted(AsyncOperation asyncOperation)
            {
                if(Status != TaskStatus.Canceled)
                {
                    Status = TaskStatus.Completed;
                    if (mCallback != null)
                    {
                        T asset = mRequest.asset as T;
                        mCallback?.Invoke(asset != null, asset);
                    }
                }
            }
        }
        public void Init()
        {
            Debug.Log("Load asset from 【Resource】 folder");
        }

        public T LoadAsset<T>(string dir, string assetName) where T : UnityEngine.Object
        {
            string path = string.Format("{0}/{1}", dir, assetName);
            return Resources.Load<T>(path);
        }

        public IAsyncTask LoadAssetAsync<T>(string dir, string assetName, Action<bool, T> callback) where T : UnityEngine.Object
        {
            string path = string.Format("{0}/{1}", dir, assetName);
            ResourceRequest reques = Resources.LoadAsync<T>(path);
            IAsyncTask task = new LoadTask<T>(reques, callback);
            return task;
        }

        public void SetDataPath(string dataPath)
        {
            
        }
    }
}
