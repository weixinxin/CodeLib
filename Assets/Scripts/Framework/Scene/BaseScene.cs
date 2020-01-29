using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public abstract partial class BaseScene
    {
        public bool isActive { get; protected set; }

        protected abstract string url{ get; }

        /// <summary>
        /// 场景切换完成，揭幕前的操作
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        protected abstract IEnumerator OnEnter(params object[] args);

        protected virtual void OnUpdate(float dt) { }

        protected virtual void OnLateUpdate(float dt) { }

        /// <summary>
        /// 落幕后开始切换场景前的操作
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator OnExit();
    }
}
