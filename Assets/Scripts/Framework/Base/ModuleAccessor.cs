
using System;

namespace Framework
{
    public abstract partial class FrameworkModuleBase
    {
        public abstract class ModuleAccessor
        {
            protected static void InvokeUpdate(FrameworkModuleBase mudule, float elapseSeconds, float realElapseSeconds)
            {
                mudule.Update(elapseSeconds, realElapseSeconds);
            }

            protected static void InvokeLateUpdate(FrameworkModuleBase mudule, float elapseSeconds, float realElapseSeconds)
            {
                mudule.LateUpdate(elapseSeconds, realElapseSeconds);
            }

            protected static void InvokeDestroy(FrameworkModuleBase mudule)
            {
                mudule.OnDestroy();
            }
        }
    }
}
