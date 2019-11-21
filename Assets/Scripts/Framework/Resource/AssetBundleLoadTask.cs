using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework
{
    public delegate void LoadAssetCallback(AssetBundle ab);
    public enum TaskStatus
    {
        Running,
        Completed,
        Canceled,
    }
    public interface IAssetBundleLoadTask
    {
        TaskStatus Status
        {
            get;
        }
        bool Cancel();
    }
    public partial class AssetBundleHelper
    {
        class AssetBundleLoadTask : IAssetBundleLoadTask
        {
            private TaskStatus mStatus = TaskStatus.Running;
            public TaskStatus Status
            {
                get
                {
                    return mStatus;
                }
            }

            private LoadAssetCallback onComplete;

            public AssetBundleLoadTask(LoadAssetCallback onComplete)
            {
                mStatus = TaskStatus.Running;
                this.onComplete = onComplete;
            }


            public void OnLoadComplete(AssetBundle assetBundle)
            {
                if (mStatus != TaskStatus.Canceled)
                {
                    mStatus = TaskStatus.Completed;
                    if (onComplete != null)
                    {
                        onComplete.Invoke(assetBundle);
                    }
                    onComplete = null;
                }
                //else
                //{
                //    Debug.LogFormat("LoadAssetBundleTask OnLoadComplete but cancel");
                //}
            }

            public bool Cancel()
            {
                if (mStatus == TaskStatus.Running)
                {
                    mStatus = TaskStatus.Canceled;
                    onComplete = null;
                    return true;
                }
                return false;
            }
        }
    }
}