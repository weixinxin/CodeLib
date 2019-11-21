using System;
using System.IO;
using System.Net;

namespace Framework
{
    public interface INetworkChannelHandler
    {
        /// <summary>
        /// 连接结果处理
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="errorCode">错误码</param>
        void OnConnectResult(bool success, int errorCode);

        /// <summary>
        /// 将传入的包体序列化到指定的流中
        /// </summary>
        /// <param name="packet">待序列化包体</param>
        /// <param name="destination">指定流</param>
        /// <returns></returns>
        bool SerializePacket(Object packet, Stream destination);

        /// <summary>
        /// 从指定的流中反序列化消息包
        /// </summary>
        /// <param name="source">数据流</param>
        /// <returns></returns>
        Object DeserializePacket(Stream source);

        /// <summary>
        /// 网络错误处理
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        void OnNetworkError(string errorMessage);

        /// <summary>
        /// 网络断开处理
        /// </summary>
        /// <param name="isPassive">是否主动断开</param>
        void OnDisconnected(bool isPassive);

        /// <summary>
        /// 消息包处理
        /// </summary>
        /// <param name="packet">消息包</param>
        void HandlePacket(Object packet);
    }
}
