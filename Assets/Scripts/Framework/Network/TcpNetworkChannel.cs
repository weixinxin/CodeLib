using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Framework
{
    public class TcpNetworkChannel : INetworkChannel
    {
        public static readonly int PacketHeadLength = sizeof(ushort);

        public static void WritePacketHead(int length, Stream stream)
        {
            ushort len = (ushort)length;
            stream.Write(BitConverter.GetBytes(len), 0, PacketHeadLength);
        }

        private static byte[] sBuffer = new byte[PacketHeadLength];
        public static int ReadPacketHead(Stream stream)
        {
            stream.Read(sBuffer, 0, PacketHeadLength);
            return BitConverter.ToInt16(sBuffer, 0);
        }

        public Socket Socket { get; protected set; }

        protected SendThread mSendThread;

        protected ReceiveThread mReceiveThread;

        protected INetworkChannelHandler mHandler;

        

        public string Name { get; protected set; }

        public AddressFamily AddressFamily { get; protected set; }

        public NetworkStatus Status { get; protected set; }

        public TcpNetworkChannel(string name, INetworkChannelHandler handler)
        {
            Name = name;
            mHandler = handler;
            Status = NetworkStatus.Disconnected;
            AddressFamily = AddressFamily.Unknown;
        }

        void Close()
        {
            if (Socket != null)
            {
                if (Socket.Connected)
                    Socket.Close();
                Socket = null;
            }

            if (mSendThread != null)
            {
                mSendThread.Stop();
                mSendThread = null;
            }

            if (mReceiveThread != null)
            {
                mReceiveThread.Stop();
                mReceiveThread = null;
            }
            Status = NetworkStatus.Disconnected;
        }

        public void Disconnect()
        {
            if(Status == NetworkStatus.Connected)
                Status = NetworkStatus.Disconnecting;
            Close();
        }

        public void Connect(IPAddress ipAddress, int port, object userData)
        {
            Close();

            switch (ipAddress.AddressFamily)
            {
                case System.Net.Sockets.AddressFamily.InterNetwork:
                    AddressFamily = AddressFamily.IPv4;
                    break;

                case System.Net.Sockets.AddressFamily.InterNetworkV6:
                    AddressFamily = AddressFamily.IPv6;
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not supported address family '{0}'.", ipAddress.AddressFamily.ToString()));
            }
            Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Status = NetworkStatus.Connecting;
            Socket.BeginConnect(ipAddress, port, ConnectCallback, userData);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket.EndConnect(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                Status = NetworkStatus.Disconnected;
                SocketException socketException = exception as SocketException;
                if (socketException != null)
                {

                    mHandler.OnNetworkError(string.Format("socket error code = {0} see detail from https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2"
                        , socketException.ErrorCode));
                    return;
                }
                throw;
            }

            mSendThread = new SendThread(mHandler, Socket);
            mSendThread.Start();

            mReceiveThread = new ReceiveThread(mHandler,this);
            mReceiveThread.Start();

            Status = NetworkStatus.Connected;
        }

        public void Send<T>(T packet)
        {
            if (Socket == null || !Socket.Connected)
            {
                throw new NullReferenceException("You must connect first.");
            }

            if (packet == null)
            {
                throw new NullReferenceException("Packet is invalid.");
            }

            mSendThread.Send(packet);
        }

        public void Update(float deltaTime)
        {
            HandleReceivedPackets();
        }

        public void HandleReceivedPackets()
        {
            if (mReceiveThread != null)
            {
                while (mReceiveThread.ReceivePackets.Count > 0)
                {
                    Object packet;
                    lock (mReceiveThread.ReceivePackets)
                    {
                        packet = mReceiveThread.ReceivePackets.Dequeue();
                    }
                    mHandler.HandlePacket(packet);
                }
            }
        }
    }
    public class SendThread : ThreadBase, IDisposable
    {
        private const int DefaultBufferLength = 1024 * 64;
        protected INetworkChannelHandler mHandler;
        private SerializerStream mStream;
        private Socket mSocket;
        protected readonly Queue<Object> mSendPacketPool = new Queue<object>(32);
        private bool m_Disposed;
        public SendThread(INetworkChannelHandler handler, Socket socket)
        {
            mHandler = handler;
            mSocket = socket;
            mStream = new SerializerStream(DefaultBufferLength);
        }

        protected override void MainLoop()
        {
            while (mSendPacketPool.Count > 0)
            {
                Object packet = null;
                lock (mSendPacketPool)
                {
                    packet = mSendPacketPool.Dequeue();
                }
                
                mStream.SetPosition(TcpNetworkChannel.PacketHeadLength);
                if (!mHandler.SerializePacket(packet, mStream))
                {
                    mHandler.OnNetworkError("Serialized packet failure.");
                    return;
                }
                int packetLength = (int)mStream.Position - TcpNetworkChannel.PacketHeadLength;
                mStream.SetPosition(0);
                TcpNetworkChannel.WritePacketHead(packetLength, mStream);
                try
                {
                    mSocket.Send(mStream.GetBuffer(), 0, packetLength + TcpNetworkChannel.PacketHeadLength, SocketFlags.None);
                }
                catch (Exception exception)
                {
                    SocketException socketException = exception as SocketException;
                    if (socketException != null)
                    {

                        mHandler.OnNetworkError(string.Format("socket error code = {0} see detail from https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2"
                            , socketException.ErrorCode));
                        return;
                    }
                    throw;
                }

            }
            Pause();
        }

        public void Send(Object packet)
        {
            lock (mSendPacketPool)
            {
                mSendPacketPool.Enqueue(packet);
            }
            Resume();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (mStream != null)
                {
                    mStream.Dispose();
                    mStream = null;
                }
            }

            m_Disposed = true;
        }
    }

    public class ReceiveThread : ThreadBase,IDisposable
    {
        private const int DefaultBufferLength = 1024 * 64;
        private SerializerStream mStream;
        private TcpNetworkChannel mChannel;
        protected INetworkChannelHandler mHandler;

        private bool mProcessPacketHeader = true;

        public Queue<Object> ReceivePackets = new Queue<object>(32);

        private int mReceiveSize = 0;
        private bool m_Disposed;
        public ReceiveThread(INetworkChannelHandler handler, TcpNetworkChannel channel)
        {
            mHandler = handler;
            mChannel = channel;
            mStream = new SerializerStream(DefaultBufferLength);
            mProcessPacketHeader = true;
            mReceiveSize = TcpNetworkChannel.PacketHeadLength;
        }

        protected override void MainLoop()
        {

            try
            {
                byte[] buffer = new byte[1024];
                int bytesReceived = mChannel.Socket.Receive(buffer, (int)mStream.Position, (int)(mReceiveSize - mStream.Position), SocketFlags.None);
                if (bytesReceived <= 0)
                {
                    //if (mChannel.Status != NetworkStatus.Disconnecting)
                    //    mChannel.Disconnect();
                    return;
                }

                mStream.SetPosition(mStream.Position + bytesReceived);
                if (mStream.Position < mReceiveSize)
                {
                    return;
                }

                mStream.SetPosition(0);


                if (mProcessPacketHeader)
                {
                    mReceiveSize = TcpNetworkChannel.ReadPacketHead(mStream);
                    mStream.SafeSetLength(mReceiveSize);
                    mProcessPacketHeader = false;
                    mStream.SetPosition(0);
                }
                else
                {
                    Object packet = mHandler.DeserializePacket(mStream);
                    lock(ReceivePackets)
                    {
                        ReceivePackets.Enqueue(packet);
                    }
                    mReceiveSize = TcpNetworkChannel.PacketHeadLength;
                    mProcessPacketHeader = true;
                }
            }
            catch (Exception exception)
            {
                SocketException socketException = exception as SocketException;
                if (socketException != null)
                {

                    mHandler.OnNetworkError(string.Format("socket error code = {0} see detail from https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2"
                        , socketException.ErrorCode));
                    return;
                }
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (mStream != null)
                {
                    mStream.Dispose();
                    mStream = null;
                }
            }

            m_Disposed = true;
        }
    }
}
