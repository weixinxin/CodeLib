#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public class EditorLoader : IAssetLoader
    {
        class EditorTask : IAsyncTask
        {
            public TaskStatus Status
            {
                get
                {
                    return TaskStatus.Completed;
                }
            }

            public bool Cancel()
            {
                return false;
            }
        }

        string mDataPath;
        public EditorLoader()
        {

        }

        public T LoadAsset<T>(string dir, string assetName) where T : UnityEngine.Object
        {
            string path = mDataPath + GetAseetPath(dir, assetName);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }

        public IAsyncTask LoadAssetAsync<T>(string dir, string assetName, Action<bool, T> callback) where T : UnityEngine.Object
        {
            T asset = LoadAsset<T>(dir, assetName);
            //可以在这里模拟加载延迟
            callback?.Invoke(asset != null, asset);
            return new EditorTask();
        }

        string GetAseetPath(string dir, string assetName)
        {
            dir = dir.Replace('\\', '/');
            return string.Format("{0}{1}/{2}", mDataPath, dir, assetName);
        }
        public void SetDataPath(string dataPath)
        {
            mDataPath = dataPath.Replace('\\', '/');
            if (!mDataPath.EndsWith("/"))
            {
                mDataPath += "/";
            }
        }

        public void LoadScene(string scenePath, bool isAdditive)
        {
            LoadSceneParameters param = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            EditorSceneManager.LoadSceneInPlayMode(scenePath, param);
        }

        public IEnumerator LoadSceneAsync(string scenePath, bool isAdditive)
        {
            LoadSceneParameters param = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, param);
            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}
#endif
