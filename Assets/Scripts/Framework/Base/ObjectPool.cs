using System;
using System.Collections.Generic;
namespace Framework
{
    public interface IRecyclable
    {
        void Clear();
    }
    public class ObjectPool<T> where T : class, IRecyclable, new()
    {
        private readonly Queue<T> mObjects;

        public ObjectPool(int capacity)
        {
            mObjects = new Queue<T>(capacity);
        }
        public T Acquire()
        {
            if (mObjects.Count > 0)
            {
                return mObjects.Dequeue();
            }
            return new T();
        }

        public void Release(T obj)
        {
            obj.Clear();
            if (mObjects.Contains(obj))
            {
                throw new Exception("The object has been released.");
            }
            mObjects.Enqueue(obj);
        }
        
        public void Release(int remaining)
        {
            if(remaining == 0)
            {
                ReleaseAllUnused();
            }
            else
            {
                while (mObjects.Count > remaining)
                {
                    mObjects.Dequeue();
                }
            }
        }

        public void ReleaseAllUnused()
        {
            mObjects.Clear();
        }
    }
}
