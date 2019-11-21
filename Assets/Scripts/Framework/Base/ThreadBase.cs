using System;
using System.Threading;
namespace Framework
{
    public abstract class ThreadBase
    {
        Thread  mThread = null;

        bool mStopFlag = false;

        bool mPauseFlag = false;

        ManualResetEvent mManualResetEvent;
        
        /// <summary>
        /// 线程开始
        /// </summary>
        protected virtual void OnEnter() { }
        
        /// <summary>
        /// 主循环
        /// </summary>
        /// <returns>是否继续</returns>
        protected abstract bool MainLoop();

        /// <summary>
        /// 线程结束退出
        /// </summary>
        protected virtual void OnExit() { }
        void Run()
        {
            try
            {
                OnEnter();
                while (!mStopFlag)
                {
                    if (mPauseFlag)
                    {
                        mManualResetEvent = new ManualResetEvent(false);
                        mManualResetEvent.WaitOne();
                        mManualResetEvent = null;
                    }
                    if (!MainLoop())
                        break;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                OnExit();
            }
        }

        /// <summary>
        /// 开始执行线程
        /// </summary>
        public void Start()
        {
            if (mThread == null)
                mThread = new Thread(Run);
            mThread.Start();
        }

        /// <summary>
        /// 暂停线程
        /// </summary>
        public void Pause()
        {
            mPauseFlag = true;
        }

        /// <summary>
        /// 恢复线程
        /// </summary>
        public void Resume()
        {
            if(mPauseFlag)
            {
                mPauseFlag = false;
                if(mManualResetEvent != null)
                    mManualResetEvent.Set();
            }
        }
        
        /// <summary>
        /// 结束线程
        /// </summary>
        public void Stop()
        {
            if (mThread != null)
            {
                mStopFlag = true;
                Resume();
                mThread.Join(100);
                mThread = null;
            }
        }
    }
}
