using System.Threading;
namespace Framework
{
    public abstract class ThreadBase
    {
        Thread  mThread = null;

        bool mStopFlag = false;

        bool mPauseFlag = false;
        ManualResetEvent mManualResetEvent;
        protected abstract void MainLoop();

        void Run()
        {
            while(!mStopFlag)
            {
                if (mPauseFlag)
                {
                    mManualResetEvent = new ManualResetEvent(false);
                    mManualResetEvent.WaitOne();
                    mManualResetEvent = null;
                }
                MainLoop();
            }
        }

        public void Start()
        {
            if (mThread == null)
                mThread = new Thread(Run);
            mThread.Start();
        }

        public void Pause()
        {
            mPauseFlag = true;
        }

        public void Resume()
        {
            if(mPauseFlag)
            {
                mPauseFlag = false;
                if(mManualResetEvent != null)
                    mManualResetEvent.Set();
            }
        }

        protected void ExitLoop()
        {
            mStopFlag = true;
        }

        public void Stop()
        {
            if (mThread != null)
            {
                mStopFlag = true;
                Resume();
                mThread.Join();
                mThread = null;
            }
        }
    }
}
