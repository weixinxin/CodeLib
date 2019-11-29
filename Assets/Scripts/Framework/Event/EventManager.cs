using System;
using System.Collections.Generic;
namespace Framework
{
    public class EventManager : FrameworkModule<EventManager>
    {
        private EventManager() { }

        public override int Priority
        {
            get
            {
                return 10;
            }
        }

        internal override void OnInit(params object[] args)
        {
            int capacity_dic = 64;
            int capacity_que = 16;
            if (args.Length > 0)
            {
                capacity_dic = (int)args[0];
            }
            if (args.Length > 1)
            {
                capacity_que = (int)args[1];
            }
            mEvents = new Dictionary<uint, Delegate>(capacity_dic);
            mEventQueue = new Queue<Event>(capacity_que);
            mCachedEvent = new List<Event>(capacity_que);
        }

        internal override void OnDestroy()
        {
            Debug.Log("EventManager OnDestroy");
            mEvents.Clear();
            mEventQueue.Clear();
            mCachedEvent.Clear();

        }

        internal override void Update(float deltaTime, float unscaledDeltaTime)
        {
            while (mEventQueue.Count > 0)
            {
                Event eventNode = null;
                lock (mEventQueue)
                {
                    eventNode = mEventQueue.Dequeue();
                    mCachedEvent.Add(eventNode);
                }
            }
            for (int i = 0; i < mCachedEvent.Count; ++i)
            {
                mCachedEvent[i].Fire();
            }
            mCachedEvent.Clear();
        }
        

        Dictionary<uint, Delegate> mEvents;
        Queue<Event> mEventQueue;
        List<Event> mCachedEvent;
        public void AddListener(uint id, Action messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("AddListener messageEvent is null !");
            }
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action act = evts as Action;
                if (act != null)
                {
                    act += messageEvent;
                }
                else
                {
                    throw new Exception("AddListener messageEvent type error !");
                }
            }
            else
            {
                mEvents.Add(id, messageEvent);
            }
        }
        public void AddListener(uint id, Action<EventArgs> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("AddListener messageEvent is null !");
            }
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<EventArgs> act = evts as Action<EventArgs>;
                if (act != null)
                {
                    act += messageEvent;
                }
                else
                {
                    throw new Exception("AddListener messageEvent type error !");
                }
            }
            else
            {
                mEvents.Add(id, messageEvent);
            }
        }


        public void AddListener<T>(uint id, Action<T> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("AddListener messageEvent is null !");
            }
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<T> act = evts as Action<T>;
                if (act != null)
                {
                    act += messageEvent;
                }
                else
                {
                    throw new Exception("AddListener messageEvent type error !");
                }
            }
            else
            {
                mEvents.Add(id, messageEvent);
            }
        }

        public void AddListener<T, U>(uint id, Action<T, U> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("AddListener messageEvent is null !");
            }
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<T, U> act = evts as Action<T, U>;
                if (act != null)
                {
                    act += messageEvent;
                }
                else
                {
                    throw new Exception("AddListener messageEvent type error !");
                }
            }
            else
            {
                mEvents.Add(id, messageEvent);
            }
        }

        public void AddListener<T, U, V>(uint id, Action<T, U, V> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("AddListener messageEvent is null !");
            }
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<T, U, V> act = evts as Action<T, U, V>;
                if (act != null)
                {
                    act += messageEvent;
                }
                else
                {
                    throw new Exception("AddListener messageEvent type error !");
                }
            }
            else
            {
                mEvents.Add(id, messageEvent);
            }
        }

        public void AddListener<T, U, V, W>(uint id, Action<T, U, V, W> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("AddListener messageEvent is null !");
            }
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<T, U, V, W> act = evts as Action<T, U, V, W>;
                if (act != null)
                {
                    act += messageEvent;
                }
                else
                {
                    throw new Exception("AddListener messageEvent type error !");
                }
            }
            else
            {
                mEvents.Add(id, messageEvent);
            }
        }

        public void RemoveListener(uint id, Action messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("RemoveListener messageEvent is null !");
            }

            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action act = evts as Action;
                if (act != null)
                {
                    act -= messageEvent;
                    if (act == null)
                    {
                        mEvents.Remove(id);
                    }
                    else
                    {
                        mEvents[id] = act;
                    }
                }
                else
                {
                    throw new Exception("RemoveListener messageEvent type error !");
                }
            }
        }
        public void RemoveListener(uint id, Action<EventArgs> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("RemoveListener messageEvent is null !");
            }

            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<EventArgs> act = evts as Action<EventArgs>;
                if (act != null)
                {
                    act -= messageEvent;
                    if (act == null)
                    {
                        mEvents.Remove(id);
                    }
                    else
                    {
                        mEvents[id] = act;
                    }
                }
                else
                {
                    throw new Exception("RemoveListener messageEvent type error !");
                }
            }
        }

        public void RemoveListener<T>(uint id, Action<T> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("RemoveListener messageEvent is null !");
            }

            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<T> act = evts as Action<T>;
                if (act != null)
                {
                    act -= messageEvent;
                    if (act == null)
                    {
                        mEvents.Remove(id);
                    }
                    else
                    {
                        mEvents[id] = act;
                    }
                }
                else
                {
                    throw new Exception("RemoveListener messageEvent type error !");
                }
            }
        }

        public void RemoveListener<T, U>(uint id, Action<T, U> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("RemoveListener messageEvent is null !");
            }

            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<T, U> act = evts as Action<T, U>;
                if (act != null)
                {
                    act -= messageEvent;
                    if (act == null)
                    {
                        mEvents.Remove(id);
                    }
                    else
                    {
                        mEvents[id] = act;
                    }
                }
                else
                {
                    throw new Exception("RemoveListener messageEvent type error !");
                }
            }
        }

        public void RemoveListener<T, U, V>(uint id, Action<T, U, V> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("RemoveListener messageEvent is null !");
            }

            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<T, U, V> act = evts as Action<T, U, V>;
                if (act != null)
                {
                    act -= messageEvent;
                    if (act == null)
                    {
                        mEvents.Remove(id);
                    }
                    else
                    {
                        mEvents[id] = act;
                    }
                }
                else
                {
                    throw new Exception("RemoveListener messageEvent type error !");
                }
            }
        }

        public void RemoveListener<T, U, V, W>(uint id, Action<T, U, V, W> messageEvent)
        {
            if (messageEvent == null)
            {
                throw new Exception("RemoveListener messageEvent is null !");
            }

            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Action<T, U, V, W> act = evts as Action<T, U, V, W>;
                if (act != null)
                {
                    act -= messageEvent;
                    if (act == null)
                    {
                        mEvents.Remove(id);
                    }
                    else
                    {
                        mEvents[id] = act;
                    }
                }
                else
                {
                    throw new Exception("RemoveListener messageEvent type error !");
                }
            }
        }

        public void TriggerEvent(uint id)
        {
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Delegate[] list = evts.GetInvocationList();
                for (int i = 0; i < list.Length; ++i)
                {
                    Action act = list[i] as Action;
                    if (act != null)
                    {
                        act();
                    }
                    else
                    {
                        throw new Exception("TriggerEvent messageEvent type error !");
                    }
                }
            }
        }
        public void TriggerEvent(uint id, EventArgs args)
        {
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Delegate[] list = evts.GetInvocationList();
                for (int i = 0; i < list.Length; ++i)
                {
                    Action<EventArgs> act = list[i] as Action<EventArgs>;
                    if (act != null)
                    {
                        act(args);
                    }
                    else
                    {
                        throw new Exception("TriggerEvent messageEvent type error !");
                    }
                }
            }
        }

        public void TriggerEvent<T>(uint id, T t)
        {
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Delegate[] list = evts.GetInvocationList();
                for (int i = 0; i < list.Length; ++i)
                {
                    Action<T> act = list[i] as Action<T>;
                    if (act != null)
                    {
                        act(t);
                    }
                    else
                    {
                        throw new Exception("TriggerEvent messageEvent type error !");
                    }
                }
            }
        }

        public void TriggerEvent<T, U>(uint id, T t, U u)
        {
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Delegate[] list = evts.GetInvocationList();
                for (int i = 0; i < list.Length; ++i)
                {
                    Action<T, U> act = list[i] as Action<T, U>;
                    if (act != null)
                    {
                        act(t, u);
                    }
                    else
                    {
                        throw new Exception("TriggerEvent messageEvent type error !");
                    }
                }
            }
        }

        public void TriggerEvent<T, U, V>(uint id, T t, U u, V v)
        {
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Delegate[] list = evts.GetInvocationList();
                for (int i = 0; i < list.Length; ++i)
                {
                    Action<T, U, V> act = list[i] as Action<T, U, V>;
                    if (act != null)
                    {
                        act(t, u, v);
                    }
                    else
                    {
                        throw new Exception("TriggerEvent messageEvent type error !");
                    }
                }
            }
        }
        public void TriggerEvent<T, U, V, W>(uint id, T t, U u, V v, W w)
        {
            Delegate evts;
            if (mEvents.TryGetValue(id, out evts))
            {
                Delegate[] list = evts.GetInvocationList();
                for (int i = 0; i < list.Length; ++i)
                {
                    Action<T, U, V, W> act = list[i] as Action<T, U, V, W>;
                    if (act != null)
                    {
                        act(t, u, v, w);
                    }
                    else
                    {
                        throw new Exception("TriggerEvent messageEvent type error !");
                    }
                }
            }
        }

        public void EnqueueEvent(uint id)
        {
            lock (mEventQueue)
            {
                mEventQueue.Enqueue(new Event() { id = id });
            }
        }
        public void EnqueueEvent(uint id, EventArgs args)
        {
            lock (mEventQueue)
            {
                mEventQueue.Enqueue(new Event<EventArgs>() { id = id, t = args });
            }
        }

        public void EnqueueEvent<T>(uint id, T t)
        {
            lock (mEventQueue)
            {
                mEventQueue.Enqueue(new Event<T>() { id = id, t = t });
            }
        }
        public void EnqueueEvent<T, U>(uint id, T t, U u)
        {
            lock (mEventQueue)
            {
                mEventQueue.Enqueue(new Event<T, U>() { id = id, t = t, u = u });
            }
        }
        public void EnqueueEvent<T, U, V>(uint id, T t, U u, V v)
        {
            lock (mEventQueue)
            {
                mEventQueue.Enqueue(new Event<T, U, V>() { id = id, t = t, u = u, v = v });
            }
        }
        public void EnqueueEvent<T, U, V, W>(uint id, T t, U u, V v, W w)
        {
            lock (mEventQueue)
            {
                mEventQueue.Enqueue(new Event<T, U, V, W>() { id = id, t = t, u = u, v = v, w = w });
            }
        }
    }

    public class Event
    {
        public uint id;

        public virtual void Fire()
        {
            EventManager.Instance.TriggerEvent(id);
        }
    }

    public class Event<T> : Event
    {
        public T t;
        public override void Fire()
        {
            EventManager.Instance.TriggerEvent(id, t);
        }
    }

    public class Event<T, U> : Event
    {
        public T t;
        public U u;
        public override void Fire()
        {
            EventManager.Instance.TriggerEvent(id, t, u);
        }
    }
    public class Event<T, U, V> : Event
    {
        public T t;
        public U u;
        public V v;
        public override void Fire()
        {
            EventManager.Instance.TriggerEvent(id, t, u, v);
        }
    }
    public class Event<T, U, V, W> : Event
    {
        public T t;
        public U u;
        public V v;
        public W w;
        public override void Fire()
        {
            EventManager.Instance.TriggerEvent(id, t, u, v, w);
        }
    }

}