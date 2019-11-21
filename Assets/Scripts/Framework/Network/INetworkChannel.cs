using System;
using System.Net;

namespace Framework
{
    /// <summary>
    /// 网络地址类型。
    /// </summary>
    public enum AddressFamily
    {
        /// <summary>
        /// 未知。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// IP 版本 4。
        /// </summary>
        IPv4,

        /// <summary>
        /// IP 版本 6。
        /// </summary>
        IPv6
    }

    public enum NetworkStatus
    {
        Disconnected,
        Connected,
        Connecting,
        Disconnecting,
    }

    public interface INetworkChannel
    {
        /// <summary>
        /// 获取网络频道名称。
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// 获取网络地址类型。
        /// </summary>
        AddressFamily AddressFamily
        {
            get;
        }

        NetworkStatus Status
        {
            get;
        }

        /// <summary>
        /// 连接到远程主机。
        /// </summary>
        /// <param name="ipAddress">远程主机的 IP 地址。</param>
        /// <param name="port">远程主机的端口号。</param>
        /// <param name="userData">用户自定义数据。</param>
        void Connect(IPAddress ipAddress, int port, object userData);


        /// <summary>
        /// 关闭网络频道。
        /// </summary>
        void Disconnect();


        /// <summary>
        /// 向远程主机发送消息包。
        /// </summary>
        /// <typeparam name="T">消息包类型。</typeparam>
        /// <param name="packet">要发送的消息包。</param>
        void Send<T>(T packet);


        void Update(float deltaTime);
    }

}
