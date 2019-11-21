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

        private Queue<string> ErrorMessages = new Queue<string>(32);

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
                    mHandler.OnConnectResult(false, socketException.ErrorCode);
                    return;
                }
                else
                {
                    mHandler.OnConnectResult(false, -1);
                }
                throw;
            }

            mSendThread = new SendThread(mHandler, Socket, ErrorMessages);
            mSendThread.Start();

            mReceiveThread = new ReceiveThread(mHandler, Socket,ErrorMessages);
            mReceiveThread.Start();

            Status = NetworkStatus.Connected;
            mHandler.OnConnectResult(true, 0);
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

        void HandleReceivedPackets()
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

        void HandleErrorMessage()
        {
            lock (ErrorMessages)
            {
                while(ErrorMessages.Count > 0)
                {
                    mHandler.OnNetworkError(ErrorMessages.Dequeue());
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
        private Queue<string> ErrorMessages;
        protected readonly Queue<Object> mSendPacketPool = new Queue<object>(32);
        private bool m_Disposed;
        public SendThread(INetworkChannelHandler handler, Socket socket, Queue<string> errorMsg)
        {
            ErrorMessages = errorMsg;
            mHandler = handler;
            mSocket = socket;
            mStream = new SerializerStream(DefaultBufferLength);
        }

        protected override bool MainLoop()
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
                    return false;
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
                        return false;
                    }
                    throw;
                }

            }
            Pause();
            return true;
        }

        public void Send(Object packet)
        {
            lock (mSendPacketPool)
            {
                mSendPacketPool.Enqueue(packet);
            }
            Resume();
        }

        void OnNetworkError(string msg)
        {
            lock (ErrorMessages)
            {
                ErrorMessages.Enqueue(msg);
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

    public class ReceiveThread : ThreadBase,IDisposable
    {
        private const int DefaultBufferLength = 1024 * 64;
        private SerializerStream mStream;
        private Socket mSocket;
        protected INetworkChannelHandler mHandler;

        private bool mProcessPacketHeader = true;

        public Queue<Object> ReceivePackets = new Queue<object>(32);
        private bool m_Disposed;

        private Queue<string> ErrorMessages;

        public ReceiveThread(INetworkChannelHandler handler, Socket socket, Queue<string> errorMsg)
        {
            mHandler = handler;
            mSocket = socket;
            ErrorMessages = errorMsg;
            mStream = new SerializerStream(DefaultBufferLength);
            mProcessPacketHeader = true;
            mStream.SafeSetLength(TcpNetworkChannel.PacketHeadLength);
        }

        protected override bool MainLoop()
        {

            try
            {
                byte[] buffer = new byte[1024];
                int bytesReceived = mSocket.Receive(mStream.GetBuffer(), (int)mStream.Position, (int)(mStream.Length - mStream.Position), SocketFlags.None);
                if (bytesReceived <= 0)
                {
                    //if (mChannel.Status != NetworkStatus.Disconnecting)
                    //    mChannel.Disconnect();
                    return false;
                }

                mStream.SetPosition(mStream.Position + bytesReceived);
                if (mStream.Position < mStream.Length)
                {
                    return true;
                }

                mStream.SetPosition(0);
                if (mProcessPacketHeader)
                {
                    int receiveSize = TcpNetworkChannel.ReadPacketHead(mStream);
                    mStream.SetPosition(0);
                    if (receiveSize > 0)
                    {
                        mStream.SafeSetLength(receiveSize);
                        mProcessPacketHeader = false;
                    }
                    else
                    {
                        OnNetworkError("recieve packet head error,length = " + receiveSize);
                    }
                }
                else
                {
                    Object packet = mHandler.DeserializePacket(mStream);
                    lock(ReceivePackets)
                    {
                        ReceivePackets.Enqueue(packet);
                    }
                    mStream.SetPosition(0);
                    mStream.SafeSetLength(TcpNetworkChannel.PacketHeadLength);
                    mProcessPacketHeader = true;
                }
            }
            catch (Exception exception)
            {
                SocketException socketException = exception as SocketException;
                if (socketException != null)
                {

                    OnNetworkError(string.Format("socket error code = {0} see detail from https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2"
                        , socketException.ErrorCode));
                    return false;
                }
                throw;
            }
            return true;
        }
        void OnNetworkError(string msg)
        {
            lock(ErrorMessages)
            {
                ErrorMessages.Enqueue(msg);
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
