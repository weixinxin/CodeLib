using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework
{
    public partial class AssetBundleHelper
    {
        class AssetBundleObject:IRecyclable
        {
            /// <summary>
            /// 资源包名
            /// </summary>
            public string bundleName;

            /// <summary>
            /// 用户主动加载的引用数
            /// </summary>
            public int userRefCount;

            /// <summary>
            /// 所有加载引用数，包括用户主动加载和被依赖产生的加载
            /// </summary>
            public int refCount;

            /// <summary>
            /// 异步加载的任务列表
            /// </summary>
            public List<AssetBundleLoadTask> taskList = new List<AssetBundleLoadTask>();

            /// <summary>
            /// 存储依赖于本资源包的依赖者等待队列，用于加载完成时通知依赖者完成
            /// </summary>
            public List<Action> waitingList = new List<Action>();

            /// <summary>
            /// 加载请求
            /// </summary>
            public AssetBundleCreateRequest assetBundleCreateRequest;

            /// <summary>
            /// 资源包
            /// </summary>
            public AssetBundle assetBundle;

            /// <summary>
            /// 加载中的依赖项个数
            /// </summary>
            public int loadingDependenciesCount;

            /// <summary>
            /// 卸载延迟计数器
            /// </summary>
            public int releaseDelay;

            public void Clear()
            {
                taskList.Clear();
                waitingList.Clear();
                assetBundle = null;
                assetBundleCreateRequest = null;
                loadingDependenciesCount = 0;
                refCount = 0;
                userRefCount = 0;
                releaseDelay = 0;
            }

            /// <summary>
            /// 依赖项加载完成时的回调
            /// </summary>
            public void OnDependencyLoaded()
            {

                if (loadingDependenciesCount <= 0)
                {
                    throw new Exception("LoadAssetbundle Dependencies count error !");
                }
                loadingDependenciesCount--;
            }

            public void Unload()
            {
                if (assetBundle == null)
                {
                    throw new Exception("Unload error! assetBundle == null");
                }
                assetBundle.Unload(false);
                assetBundle = null;
            }
        }

        /// <summary>
        /// 同时加载的最大数量
        /// </summary>
        private int mMaxLoadingCount = 10;

        /// <summary>
        /// 延迟卸载帧数
        /// </summary>
        private int mDelayReleaseFrameCount = 60;

        private List<AssetBundleObject> mTempList = new List<AssetBundleObject>(16);

        private ObjectPool<AssetBundleObject> objectPool = new ObjectPool<AssetBundleObject>(32);
        /// <summary>
        /// 依赖关系数据
        /// </summary>
        private Dictionary<string, string[]> mDependenciesDataList;

        private string[] mEmptyArray = new string[0];

        /// <summary>
        /// 等待加载队列
        /// </summary>
        private Dictionary<string, AssetBundleObject> mWaitingList;

        /// <summary>
        /// 加载中队列
        /// </summary>
        private Dictionary<string, AssetBundleObject> mLoadingList;

        /// <summary>
        /// 加载完成队列
        /// </summary>
        private Dictionary<string, AssetBundleObject> mLoadedList;

        /// <summary>
        /// 等待卸载队列
        /// </summary>
        private List<AssetBundleObject> mUnloadList;

        private string mDataPath;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="LoadingLimit">同时加载的最大任务限制数</param>
        /// <param name="delayReleaseFrameCount">资源释放延迟帧数</param>
        public AssetBundleHelper(uint LoadingLimit,uint delayReleaseFrameCount)
        {
            mMaxLoadingCount = (int)LoadingLimit;
            mDelayReleaseFrameCount = (int)delayReleaseFrameCount;

            mDependenciesDataList = new Dictionary<string, string[]>();
            mWaitingList = new Dictionary<string, AssetBundleObject>();
            mLoadingList = new Dictionary<string, AssetBundleObject>();
            mLoadedList = new Dictionary<string, AssetBundleObject>();
            mUnloadList = new List<AssetBundleObject>();

        }

        /// <summary>
        /// 设置资源包加载根目录
        /// </summary>
        /// <param name="dataPath"></param>
        public void SetDataPath(string dataPath)
        {
            mDataPath = dataPath.Replace('\\', '/');
            if (!mDataPath.EndsWith("/"))
            {
                mDataPath += "/";
            }
        }

        /// <summary>
        /// 加载依赖关系数据文件
        /// </summary>
        /// <param name="platform"></param>
        public void LoadMainfest(string platform)
        {
            if (string.IsNullOrEmpty(platform))
            {
                throw new Exception("LoadMainfest path is null or empty !");
            }
            string path = mDataPath + platform;
            mDependenciesDataList.Clear();
            AssetBundle ab = AssetBundle.LoadFromFile(path);

            if (ab == null)
            {
                throw new Exception("LoadMainfest AssetBundle.LoadFromFile return null !");
            }

            AssetBundleManifest mainfest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            if (mainfest == null)
            {
                throw new Exception("LoadMainfest AssetBundleManifest is null !");
            }
            foreach (string bundleName in mainfest.GetAllAssetBundles())
            {
                string[] dps = mainfest.GetDirectDependencies(bundleName);
                mDependenciesDataList.Add(bundleName, dps);
            }

            ab.Unload(true);
            ab = null;

            //Debug.Log("AssetBundleLoadMgr dependsCount=" + mDependenciesDataList.Count);
        }

        /// <summary>
        ///  每帧调用
        /// </summary>
        public void Update()
        {
            UpdateLoadingList();
            UpdateWaitingList();
            UpdateUnloadList();
        }

        /// <summary>
        /// 同步加载AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string bundleName)
        {
            //Debug.Log("##User Call LoadAssetBundle " + bundleName);
            var abObj = LoadAssetBundleSync(bundleName);
            abObj.userRefCount++;
            return abObj.assetBundle;
        }


        /// <summary>
        /// 异步加载AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="callback"></param>
        /// <returns>任务接口</returns>
        public IAsyncTask LoadAssetBundleAsync(string bundleName, LoadAssetCallback callback)
        {
            //Debug.Log("##User Call LoadAssetBundleAsync " + bundleName);
            AssetBundleLoadTask task = new AssetBundleLoadTask(callback);
            LoadAssetBundleAsync(bundleName, task);
            return task;
        }

        /// <summary>
        /// 卸载AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        public void Unload(string bundleName)
        {
            //Debug.Log("##User Call Unload " + bundleName);
            UnloadAssetBundleAsync(bundleName, true,null);
        }

        /// <summary>
        /// 获取所有依赖项
        /// </summary>
        /// <param name="hashName"></param>
        /// <returns></returns>
        private string[] GetDirectDependencies(string hashName)
        {
            if (mDependenciesDataList.TryGetValue(hashName, out string[] res))
            {
                return res;
            }
            return mEmptyArray;
        }

        /// <summary>
        /// 获取资源路径
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private string GetAssetBundlePath(string bundleName)
        {
            return mDataPath + bundleName;
        }

        /// <summary>
        /// 同步加载AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        private AssetBundleObject LoadAssetBundleSync(string bundleName)
        {
            AssetBundleObject abObj = null;
            if (mLoadedList.TryGetValue(bundleName, out abObj)) //已经加载
            {
                abObj.refCount++;
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleSync(dpBundleName);//加载全部依赖项
                }
                //Debug.LogFormat("LoadAssetBundle {0} from LoadedList", bundleName);
                return abObj;
            }
            else if (mLoadingList.TryGetValue(bundleName, out abObj)) //在加载中,异步改同步
            {
                abObj.refCount++;
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleSync(dpBundleName);//加载全部依赖项
                }
                //Debug.LogFormat("LoadAssetBundle {0} from LoadingList", bundleName);
                //强制完成，回调
                abObj.assetBundle = abObj.assetBundleCreateRequest.assetBundle; //如果没加载完，会异步转同步
                abObj.assetBundleCreateRequest = null;
                mLoadingList.Remove(bundleName);
                mLoadedList.Add(abObj.bundleName, abObj);
                OnAssetBundleObjectLoaded(abObj);
                return abObj;
            }
            else if (mWaitingList.TryGetValue(bundleName, out abObj)) //在准备加载中
            {
                abObj.refCount++;
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleSync(dpBundleName);//加载全部依赖项
                }
                //Debug.LogFormat("LoadAssetBundle {0} from WaitingList", bundleName);
                string ab_path = GetAssetBundlePath(bundleName);
                abObj.assetBundle = AssetBundle.LoadFromFile(ab_path);
                mWaitingList.Remove(abObj.bundleName);
                mLoadedList.Add(abObj.bundleName, abObj);
                OnAssetBundleObjectLoaded(abObj); //强制完成，回调
                return abObj;
            }

            //创建一个加载
            abObj = objectPool.Acquire();
            abObj.bundleName = bundleName;
            abObj.userRefCount = 0;
            abObj.loadingDependenciesCount = 0;
            abObj.refCount = 1;
            //Debug.LogFormat("LoadAssetBundle {0} LoadFromFile", bundleName);
            foreach (var dpHashName in GetDirectDependencies(bundleName))
            {
                LoadAssetBundleSync(dpHashName);//加载全部依赖项
            }
            string path = GetAssetBundlePath(bundleName);
            abObj.assetBundle = AssetBundle.LoadFromFile(path);
            if (abObj.assetBundle == null)
            {
                throw new Exception(string.Format("AssetBundle.LoadFromFile({0}) return null!!", path));
            }
            mLoadedList.Add(abObj.bundleName, abObj);

            return abObj;
        }

        /// <summary>
        /// 加载完成处理
        /// </summary>
        /// <param name="abObj"></param>
        private void OnAssetBundleObjectLoaded(AssetBundleObject abObj)
        {
            if (abObj.assetBundle == null)
            {
                throw new Exception(string.Format("LoadAssetbundle name:{0} assetBundle == null !!", abObj.bundleName));
            }

            //运行回调
            foreach (var task in abObj.taskList)
            {
                task.OnLoadComplete(abObj.assetBundle);
            }
            abObj.taskList.Clear();

            //通知等待的依赖者
            foreach (var callback in abObj.waitingList)
            {
                callback();
            }
            abObj.waitingList.Clear();
        }

        /// <summary>
        /// 异步加载依赖的AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="rootObj"></param>
        private void LoadAssetBundleDependencyAsync(string bundleName, Action callBack)
        {
            AssetBundleObject abObj = null;
            if (mLoadedList.TryGetValue(bundleName, out abObj))
            {
                //已经加载完成，依赖项引用增加，直接回调
                abObj.refCount++;
                //Debug.LogFormat("LoadAssetBundleDependencyAsync {0} from LoadedList", bundleName);
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleDependencyAsync(dpBundleName, null);//加载全部依赖项
                }
                callBack?.Invoke();
            }
            else if (mLoadingList.TryGetValue(bundleName, out abObj)) //在加载中
            {
                abObj.refCount++;
                if (callBack != null)
                    abObj.waitingList.Add(callBack);
                //Debug.LogFormat("LoadAssetBundleDependencyAsync {0} from LoadingList", bundleName);
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleDependencyAsync(dpBundleName, null);//加载全部依赖项
                }
            }
            else if (mWaitingList.TryGetValue(bundleName, out abObj)) //在准备加载中
            {
                abObj.refCount++;
                if (callBack != null)
                    abObj.waitingList.Add(callBack);
                //Debug.LogFormat("LoadAssetBundleDependencyAsync {0} from WaitingList", bundleName);
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleDependencyAsync(dpBundleName, null);//加载全部依赖项
                }
            }
            else
            {
                //创建一个加载
                abObj = objectPool.Acquire();
                abObj.bundleName = bundleName;
                abObj.userRefCount = 0;
                abObj.refCount = 1;
                if (callBack == null)
                    throw new Exception("callBack must not be null !!");
                abObj.waitingList.Add(callBack);
                string[] dependencies = GetDirectDependencies(bundleName);
                abObj.loadingDependenciesCount = dependencies.Length;
                foreach (var dpHashName in dependencies)
                {
                    LoadAssetBundleDependencyAsync(dpHashName, abObj.OnDependencyLoaded);
                }

                if (mLoadingList.Count < mMaxLoadingCount) //正在加载的数量不能超过上限
                {
                    StartLoading(abObj);

                    mLoadingList.Add(bundleName, abObj);
                }
                else
                {
                    //Debug.LogFormat("LoadAssetBundleDependencyAsync {0} wait", bundleName);
                    mWaitingList.Add(bundleName, abObj);
                }
            }


        }

        /// <summary>
        /// 异步加载AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        private AssetBundleObject LoadAssetBundleAsync(string bundleName, AssetBundleLoadTask task)
        {
            AssetBundleObject abObj = null;
            if (mLoadedList.TryGetValue(bundleName, out abObj))
            {
                //已经加载完成，依赖项引用增加，直接回调
                abObj.refCount++;
                abObj.userRefCount++;
                //Debug.LogFormat("LoadAssetBundleAsync {0} mLoadedList", bundleName);
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleDependencyAsync(dpBundleName, null);
                }
                task.OnLoadComplete(abObj.assetBundle);
                return abObj;
            }
            else if (mLoadingList.TryGetValue(bundleName, out abObj)) //在加载中
            {
                abObj.refCount++;
                abObj.userRefCount++;
                abObj.taskList.Add(task);
                //Debug.LogFormat("LoadAssetBundleAsync {0} mLoadingList", bundleName);
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleDependencyAsync(dpBundleName, null);
                }
                return abObj;
            }
            else if (mWaitingList.TryGetValue(bundleName, out abObj)) //在准备加载中
            {
                abObj.refCount++;
                abObj.userRefCount++;
                abObj.taskList.Add(task);
                //Debug.LogFormat("LoadAssetBundleAsync {0} mWaitingList", bundleName);
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    LoadAssetBundleDependencyAsync(dpBundleName, null);
                }
                return abObj;
            }

            //创建一个加载
            abObj = objectPool.Acquire();
            abObj.bundleName = bundleName;
            abObj.userRefCount = 1;
            abObj.refCount = 1;
            abObj.taskList.Add(task);

            //加载依赖项
            string[] dependencies = GetDirectDependencies(bundleName);
            abObj.loadingDependenciesCount = dependencies.Length;
            foreach (var dpHashName in dependencies)
            {
                LoadAssetBundleDependencyAsync(dpHashName, abObj.OnDependencyLoaded);
            }

            if (mLoadingList.Count < mMaxLoadingCount) //正在加载的数量不能超过上限
            {
                //Debug.LogFormat("LoadAssetBundleAsync {0} loading", bundleName);
                StartLoading(abObj);
                mLoadingList.Add(bundleName, abObj);
            }
            else
            {
                //Debug.LogFormat("LoadAssetBundleAsync {0} wait", bundleName);
                mWaitingList.Add(bundleName, abObj);
            }

            return abObj;
        }

        /// <summary>
        /// 开始调用底层异步加载
        /// </summary>
        /// <param name="abObj"></param>
        private void StartLoading(AssetBundleObject abObj)
        {
            //Debug.LogFormat("{0} StartLoading", abObj.bundleName);
            string path = GetAssetBundlePath(abObj.bundleName);
            abObj.assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(path);

            if (abObj.assetBundleCreateRequest == null)
            {
                throw new Exception("AssetBundle.LoadFromFileAsync return null !!");
            }

        }

        /// <summary>
        /// 异步卸载AssetBundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="recursive"></param>
        private void UnloadAssetBundleAsync(string bundleName,bool userCall, AssetBundleObject rootObj)
        {
            AssetBundleObject abObj = null;
            bool isWaiting = false;
            if (mWaitingList.TryGetValue(bundleName, out abObj))
            {
                isWaiting = true;
            }
            else if (!mLoadedList.TryGetValue(bundleName, out abObj) && !mLoadingList.TryGetValue(bundleName, out abObj))
            {
                throw new Exception(string.Format("UnLoadAssetbundle can not find {0}", bundleName));
            }
            if(userCall)
            {
                if (abObj.userRefCount < 1)
                {
                    //确保调用卸载指定AssetBundle前肯定执行过对应的加载操作，从而避免误卸载被依赖而加载的AssetBundle
                    throw new Exception(string.Format("Make sure assetBundle {0} has been loaded before unloading", bundleName));
                }
                else
                {
                    abObj.userRefCount--;
                }
            }

            if (rootObj != null)
            {
                //rootObj即依赖此包的节点，有值表示rootObj已经移除
                //rootObj被移除则从等待通知列表中移除对应回调
                Action callback = rootObj.OnDependencyLoaded;
                if (abObj.waitingList.Contains(callback))
                {
                    abObj.waitingList.Remove(callback);
                }
            }

            if (abObj.refCount < 1)
            {
                //确保引用计数正确
                throw new Exception(string.Format("UnLoadAssetbundle refCount error ! assetName:{0}", bundleName));
            }
            abObj.refCount--;
            //Debug.LogFormat("{0} refCount--  = {1}", abObj.bundleName, abObj.refCount);
            if (abObj.refCount == 0)
            {
                if (isWaiting)
                {
                    //等待队列中则直接移除
                    mWaitingList.Remove(abObj.bundleName);
                    //Debug.Log("Remove from mWaitingList" + bundleName);
                    //通知依赖项引用减少，并传递自己来取消依赖加载完成消息
                    foreach (var dpBundleName in GetDirectDependencies(bundleName))
                    {
                        UnloadAssetBundleAsync(dpBundleName,false, abObj);
                    }
                    objectPool.Release(abObj);
                }
                else
                {
                    //仅仅加入延迟卸载队列,并不从原本队列中移除,加载中的也会顺利走完加载流程最终进入完成队列才进行延迟移除
                    foreach (var dpBundleName in GetDirectDependencies(bundleName))
                    {
                        UnloadAssetBundleAsync(dpBundleName, false, null);
                    }
                    abObj.releaseDelay = mDelayReleaseFrameCount;
                    if (!mUnloadList.Contains(abObj))
                    {
                        mUnloadList.Add(abObj);
                        //Debug.Log("mUnloadList Add " + bundleName);
                    }
                    //else
                    //{
                    //    Debug.Log("mUnloadList Exist " + bundleName);
                    //}
                }
            }
            else
            {
                //通知依赖项引用减少
                foreach (var dpBundleName in GetDirectDependencies(bundleName))
                {
                    UnloadAssetBundleAsync(dpBundleName, false, null);
                }
            }
        }

        /// <summary>
        /// 遍历加载中队列，将完成的任务转入加载完成列表并触发完成回调
        /// </summary>
        private void UpdateLoadingList()
        {
            if (mLoadingList.Count == 0) return;
            //检测加载完的
            mTempList.Clear();
            foreach (var abObj in mLoadingList.Values)
            {
                //依赖项都加载完成且自身ab加载完成
                if (abObj.loadingDependenciesCount == 0 && abObj.assetBundleCreateRequest.isDone)
                {
                    //Debug.LogFormat("{0} LoadComplete", abObj.bundleName);
                    mTempList.Add(abObj);
                    abObj.assetBundle = abObj.assetBundleCreateRequest.assetBundle;
                    abObj.assetBundleCreateRequest = null;
                }
            }

            //加载完成的执行回调
            foreach (var abObj in mTempList)
            {
                mLoadingList.Remove(abObj.bundleName);
                mLoadedList.Add(abObj.bundleName, abObj);
                OnAssetBundleObjectLoaded(abObj);
            }

        }

        /// <summary>
        /// 尝试将等待中的任务开始加载
        /// </summary>
        private void UpdateWaitingList()
        {
            if (mWaitingList.Count == 0) return;
            if (mLoadingList.Count >= mMaxLoadingCount) return;

            mTempList.Clear();
            foreach (var abObj in mWaitingList.Values)
            {
                StartLoading(abObj);

                mTempList.Add(abObj);
                mLoadingList.Add(abObj.bundleName, abObj);

                if (mLoadingList.Count >= mMaxLoadingCount) break;
            }

            foreach (var abObj in mTempList)
            {
                mWaitingList.Remove(abObj.bundleName);
            }
        }

        /// <summary>
        /// 遍历卸载队列，尝试将到期的资源移除，将重新被引用的任务移除卸载队列
        /// </summary>
        private void UpdateUnloadList()
        {
            if (mUnloadList.Count == 0) return;
            for (int i = mUnloadList.Count - 1; i >= 0; --i)
            {
                var abObj = mUnloadList[i];

                //保证加载过程已经完成才进行移除
                if (abObj.refCount == 0 && abObj.assetBundle != null)
                {
                    if (--abObj.releaseDelay <= 0)
                    {
                        //Debug.LogFormat("UpdateUnloadList {0} really Unload", abObj.bundleName);
                        abObj.Unload();
                        mUnloadList.RemoveAt(i);
                        mLoadedList.Remove(abObj.bundleName);
                        objectPool.Release(abObj);
                    }
                }

                //等待移除的过程中重新被引用，从等待队列中移除
                if (abObj.refCount > 0)
                {
                    //Debug.LogFormat("UpdateUnloadList {0} abObj.refCount >0 ,remove from UnloadList", abObj.bundleName);
                    mUnloadList.RemoveAt(i);
                }
            }
        }


    }
}
