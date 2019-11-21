using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class TestThread : MonoBehaviour
{
    class ThreadException : ThreadBase
    {
        protected override bool MainLoop()
        {
            int a = 0;
            int b = 10 / a;
            return false;
        }
        protected override void OnEnter()
        {
            UnityEngine.Debug.Log("OnEnter");
        }
        protected override void OnExit()
        {
            UnityEngine.Debug.Log("OnExit");
        }
    }
    ThreadException thread;
    private void Start()
    {
        thread = new ThreadException();
        thread.Start();
    }

    private void OnDestroy()
    {
        thread.Stop();
    }
}
