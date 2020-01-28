using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine
{
    Stack<IEnumerator> stack;
    public Coroutine(IEnumerator itor)
    {
        stack = new Stack<IEnumerator>();
        stack.Push(itor);
    }

    public bool Update()
    {
        if (stack.Count > 0)
        {
            IEnumerator cur = stack.Peek();
            if (!cur.MoveNext())
            {
                stack.Pop();
            }
            else
            {
                if (cur.Current is IEnumerator)
                {
                    stack.Push(cur.Current as IEnumerator);
                }
                else if(cur.Current is UnityEngine.WaitForSeconds)
                {
                    stack.Push(new ExWaitForSeconds(cur.Current as UnityEngine.WaitForSeconds));
                }
                else if(cur.Current is UnityEngine.AsyncOperation)
                {
                    stack.Push(new ExAsyncOperation(cur.Current as UnityEngine.AsyncOperation));
                }
                else if(cur.Current != null)
                {
                    //不支持的类型
                    throw new NotSupportedException(string.Format("No support type {0}", cur.Current.GetType()));
                }
            }
        }
        return stack.Count > 0;
    }

    class ExWaitForSeconds: IEnumerator
    {
        private float m_Seconds;
        public ExWaitForSeconds(UnityEngine.WaitForSeconds waitForSeconds)
        {
            m_Seconds = waitForSeconds.GetPrivateField<float>("m_Seconds");
        }

        public object Current => null;

        public bool MoveNext()
        {
            m_Seconds -= UnityEngine.Time.deltaTime;
            return m_Seconds > 0;
        }

        public void Reset(){}
    }
    class ExAsyncOperation : IEnumerator
    {
        AsyncOperation m_Request;
        public ExAsyncOperation(AsyncOperation request)
        {
            m_Request = request;
        }
        public object Current => null;

        public bool MoveNext()
        {
            return !m_Request.isDone;
        }

        public void Reset(){}
    }
}