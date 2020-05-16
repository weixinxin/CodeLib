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

        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public IAsyncTask LoadAssetAsync<T>(string assetPath, Action<bool, T> callback) where T : UnityEngine.Object
        {
            T asset = LoadAsset<T>(assetPath);
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
            scenePath = FormatScenePath(scenePath);
            LoadSceneParameters param = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            EditorSceneManager.LoadSceneInPlayMode(scenePath, param);
        }

        public IEnumerator LoadSceneAsync(string scenePath, bool isAdditive)
        {
            scenePath = FormatScenePath(scenePath);
            LoadSceneParameters param = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, param);
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        string FormatScenePath(string scenePath)
        {
            if (!scenePath.StartsWith("Assets/"))
            {
                scenePath = "Assets/" + scenePath;
            }
            return scenePath;
        }

        public void Update(float deltaTime)
        {

        }

    }
}
#endif
