using System;
using System.Collections.Generic;

namespace Framework
{
    public static class GameFramework
    {
        private static readonly LinkedList<FrameworkModuleBase> sFrameworkModules = new LinkedList<FrameworkModuleBase>();

        public static void Update(float deltaTime, float unscaledDeltaTime)
        {
            foreach (FrameworkModuleBase module in sFrameworkModules)
            {
                module.Update(deltaTime, unscaledDeltaTime);
            }
        }
        public static void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (FrameworkModuleBase module in sFrameworkModules)
            {
                module.LateUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public static void Shutdown()
        {
            for (LinkedListNode<FrameworkModuleBase> current = sFrameworkModules.Last; current != null; current = current.Previous)
            {
                current.Value.OnDestroy();
            }
            sFrameworkModules.Clear();
        }

        internal static void RegisterModule(FrameworkModuleBase module)
        {
            if (module == null)
            {
                throw new Exception("module should not be null");
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
    }
}
