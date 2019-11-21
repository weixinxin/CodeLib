using System;
using System.Collections.Generic;
namespace Framework
{
    public class ResourceManager : FrameworkModule<ResourceManager>
    {
        private ResourceManager() { }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        internal override void OnInit(params object[] args)
        {
            Debug.Log("ResourceManager OnInitialize");

        }

        internal override void OnDestroy()
        {
            Debug.Log("ResourceManager OnDestroy");
        }

        internal override void Update(float deltaTime, float unscaledDeltaTime)
        {

        }

        internal override void LateUpdate(float deltaTime, float unscaledDeltaTime)
        {

        }
    }
}
