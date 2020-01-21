using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public interface ISceneCurtain
    {
        /// <summary>
        /// 落幕
        /// </summary>
        /// <returns></returns>
        IEnumerator Falls();

        /// <summary>
        /// 揭幕
        /// </summary>
        /// <returns></returns>
        IEnumerator Raise();
    }
}
