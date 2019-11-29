﻿namespace Framework
{
    /// <summary>
    /// 有限状态机事件响应函数。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    /// <param name="fsm">有限状态机引用。</param>
    /// <param name="sender">事件源。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>是否处理完消息</returns>
    public delegate bool FsmEventHandler<T>(IFsm<T> fsm, object sender, object userData) where T : class;
}
