using System;
using System.Collections.Generic;
namespace Framework
{
    public interface IRecyclable
    {
        void Reset();
        void Clear();
    }
    public class ObjectPool<T> where T : class, new()
    {
        private readonly Queue<T> mObjects;

        public ObjectPool(int capacity)
        {
            mObjects = new Queue<T>(capacity);
        }
        public T Acquire()
        {
            lock(mObjects)
            {
                T obj;
                if (mObjects.Count > 0)
                {
                    obj = mObjects.Dequeue();
                }
                else
                    obj = new T();
                IRecyclable recyclable = obj as IRecyclable;
                recyclable?.Reset();
                return obj;
            }
        }

        public void Recycle(T obj)
        {
            IRecyclable recyclable = obj as IRecyclable;
            recyclable?.Clear();
            lock (mObjects)
            {
                if (mObjects.Contains(obj))
                {
                    throw new Exception("The object has been released.");
                }
                mObjects.Enqueue(obj);
            }
        }
        
        public void Release(int remaining)
        {
            if (remaining == 0)
            {
                ReleaseAllUnused();
            }
            else
            {
                lock (mObjects)
                {
                    while (mObjects.Count > remaining)
                    {
                        mObjects.Dequeue();
                    }
                }
            }
        }

        public void ReleaseAllUnused()
        {
            lock (mObjects)
            {
                mObjects.Clear();
            }
        }
    }
}
