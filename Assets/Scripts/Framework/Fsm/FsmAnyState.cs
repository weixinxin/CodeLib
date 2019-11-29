using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 有限状态机任意状态基类。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    public interface IFsmAnyState<T> where T : class
    {
        /// <summary>
        /// 响应有限状态机事件时缺省调用。
        /// </summary>
        /// <param name="fsm">有限状态机引用。</param>
        /// <param name="sender">事件源。</param>
        /// <param name="eventId">事件编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        void OnEvent(IFsm<T> fsm, object sender, int eventId, object userData);
    }
}
