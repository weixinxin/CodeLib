using System;
using System.Collections.Generic;
using static Framework.FrameworkModuleBase;

namespace Framework
{
    public class GameFramework: ModuleAccessor
    {
        private static readonly LinkedList<FrameworkModuleBase> sFrameworkModules = new LinkedList<FrameworkModuleBase>();
        

        public static void Update(float deltaTime, float unscaledDeltaTime)
        {
            foreach (FrameworkModuleBase module in sFrameworkModules)
            {
                InvokeUpdate(module,deltaTime, unscaledDeltaTime);
            }
        }

        public static void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (FrameworkModuleBase module in sFrameworkModules)
            {
                InvokeLateUpdate(module,deltaTime, unscaledDeltaTime);
            }
        }

        public static void FixedUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (FrameworkModuleBase module in sFrameworkModules)
            {
                InvokeFixedUpdate(module, deltaTime, unscaledDeltaTime);
            }
        }

        public static void Shutdown()
        {
            for (LinkedListNode<FrameworkModuleBase> current = sFrameworkModules.Last; current != null; current = current.Previous)
            {
                InvokeDestroy(current.Value);
            }
            sFrameworkModules.Clear();
        }

        internal static void Register<T>(T module)where T : FrameworkModuleBase
        {
            if (module == null)
            {
                throw new Exception("module should not be null");
            }
            T m = GetModule<T>();
            if (m != null)
            {
                throw new Exception(string.Format("{0} has been registered !",typeof(T)));
            }
            LinkedListNode<FrameworkModuleBase> current = sFrameworkModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }
                current = current.Next;
            }

            if (current != null)
            {
                sFrameworkModules.AddBefore(current, module);
            }
            else
            {
                sFrameworkModules.AddLast(module);
            }
        }

        public static T GetModule<T>() where T: FrameworkModuleBase
        {
            T res = null;
            for (LinkedListNode<FrameworkModuleBase> current = sFrameworkModules.Last; current != null && res == null; current = current.Previous)
            {
                res = current.Value as T;
            }
            return res;
        }
    }
}
