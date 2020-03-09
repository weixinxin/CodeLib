using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract partial class BasePanel
    {

        public string UIName
        {
            get;
            private set;
        }

        /// <summary>
        /// 界面资源路径
        /// </summary>
        protected abstract string url
        {
            get;
        }

        /// <summary>
        /// 是否全屏界面
        /// </summary>
        public abstract bool isFullScreen
        {
            get;
        }
        
        private bool m_Visible = true;

        private bool m_Valid = true;

        public GameObject gameObject
        {
            get;
            private set;
        }

        public bool Visible
        {
            get
            {
                return m_Visible;
            }
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        protected virtual void OnInit()
        {

        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="args">界面参数</param>
        protected abstract void OnAwake(params object[] userData);

        /// <summary>
        /// 界面关闭
        /// </summary>
        protected abstract void OnClose();

        /// <summary>
        /// 界面被遮挡
        /// </summary>
        protected abstract void OnCover();

        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        protected abstract void OnReveal();

        /// <summary>
        /// 界面更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        protected abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 界面后更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        protected abstract void OnLateUpdate(float elapseSeconds, float realElapseSeconds);
        
    }
}