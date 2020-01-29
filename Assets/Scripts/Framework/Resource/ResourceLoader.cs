using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class ResourceLoader : IAssetLoader
    {
        private class LoadTask<T> : IAsyncTask where T : UnityEngine.Object
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
                if (Status != TaskStatus.Canceled)
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

        public ResourceLoader()
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

        public void LoadScene(string scenePath, bool isAdditive)
        {
            //非AB模式则必须在Build Setting中
            LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            string sceneName = GetSceneName(scenePath);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, mode);
        }

        public IEnumerator LoadSceneAsync(string scenePath, bool isAdditive)
        {
            //非AB模式则必须在Build Setting中
            LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
            string sceneName = GetSceneName(scenePath);
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        public void Update(float deltaTime){}

        string GetSceneName(string scenePath)
        {
            scenePath = scenePath.Replace('/', '-').Replace('\\', '-').Replace(".unity","");
            string[] split = scenePath.Split('-');
            return split[split.Length - 1];
        }
    }
}
