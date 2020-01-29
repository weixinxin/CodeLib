using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using UnityEngine;
using Debug = UnityEngine.Debug;
namespace Framework
{

    public class TcpNetworkChannel : INetworkChannel,IDisposable
    {
        class NetMessage :IRecyclable
        {
            int type = -1;
            string text;
            bool boolean;
            int errorCode;
            object packet;
            public void Send(INetworkChannelHandler mHandler)
            {
                switch(type)
                {
                    case 0:
                        mHandler.OnNetworkError(text);
                        break;
                    case 1:
                        mHandler.OnConnectResult(boolean, errorCode);
                        break;
                    case 2:
                        mHandler.OnDisconnected(boolean);
                        break;
                    case 3:
                        mHandler.HandlePacket(packet);
                        break;
                }
            }
            public NetMessage ErrorMessage(string msg)
            {
                type = 0;
                this.text = msg;
                return this;
            }

            public NetMessage ConnectMessage(bool success, int errorCode)
            {
                type = 1;
                boolean = success;
                this.errorCode = errorCode;
                return this;
            }

            public NetMessage DisconnectedMessage(bool isPassive)
            {
                type = 2;
                boolean = isPassive;
                return this;
            }

            public NetMessage PacketMessage(object packet)
            {
                type = 3;
                this.packet = packet;
                return this;
            }

            public void Clear()
            {
                type = -1;
                text = null;
                packet = null;
            }
        }
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

        protected TcpClient TcpClient = null;

        protected SendThread mSendThread = null;

        protected ReceiveData mReceiveData = null;

        protected INetworkChannelHandler mHandler;

        

        public string Name { get; protected set; }

        public AddressFamily AddressFamily { get; protected set; }

        public NetworkStatus Status { get; protected set; }

        private Queue<NetMessage> mMessageQueue = new Queue<NetMessage>(32);

        private ObjectPool<NetMessage> mObjectPool = new ObjectPool<NetMessage>(4);

        public TcpNetworkChannel(string name, INetworkChannelHandler handler)
        {
            Name = name;
            mHandler = handler;
            Status = NetworkStatus.Disconnected;
            AddressFamily = AddressFamily.Unknown;
        }


        public void Disconnect()
        {
            while(Status == NetworkStatus.Connecting)
            {
                Thread.Sleep(50);
            }
            Debug.Log("Disconnect");
            if (Status == NetworkStatus.Connected)
            {
                Status = NetworkStatus.Disconnecting;
                Close(false);
            }

        }

        public void Connect(IPAddress ipAddress, int port, object userData)
        {
            Disconnect();
            Debug.Log("@@@@@ Start Connect");
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
            TcpClient = new TcpClient(ipAddress.AddressFamily);
            Status = NetworkStatus.Connecting;
            TcpClient.BeginConnect(ipAddress, port, ConnectCallback, userData);
        }

        public void Send<T>(T packet)
        {
            if (TcpClient == null || !TcpClient.Connected)
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
            
            while (mMessageQueue.Count > 0)
            {
                NetMessage netMessage = null;
                lock (mMessageQueue)
                {
                    netMessage = mMessageQueue.Dequeue();
                }
                netMessage.Send(mHandler);
                mObjectPool.Recycle(netMessage);
            }
        }

        void ConnectCallback(IAsyncResult ar)
        {
            Debug.Log("@@@@@ ConnectCallback");
            try
            {
                TcpClient.EndConnect(ar);
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
                    OnNetworkError(string.Format("Connect socket error code = {0} see detail from https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2"
                    , socketException.ErrorCode));
                    OnConnectResult(false, socketException.ErrorCode);
                    return;
                }
                else
                {
                    OnConnectResult(false, -1);
                }
                throw;
            }

            mSendThread = new SendThread(mHandler, TcpClient.GetStream(), OnNetworkError);
            mSendThread.Start();

            mReceiveData = new ReceiveData();

            Status = NetworkStatus.Connected;
            OnConnectResult(true, 0);
            Receive();
        }
        void OnConnectResult(bool success, int errorCode)
        {

            lock (mMessageQueue)
            {
                mMessageQueue.Enqueue(mObjectPool.Acquire().ConnectMessage(success, errorCode));
            }
        }

        void OnNetworkError(string msg)
        {
            lock (mMessageQueue)
            {

                mMessageQueue.Enqueue(mObjectPool.Acquire().ErrorMessage(msg));
            }
        }
        
        ManualResetEvent mManualResetEvent;
        bool Receiving = false;
        void Close(bool isPassive)
        {

            Debug.Log("Close");
            if (TcpClient != null)
            {
                if (TcpClient.Connected)
                {
                    TcpClient.Close();
                }
                if (Receiving)
                {
                    mManualResetEvent = new ManualResetEvent(false);
                    mManualResetEvent.WaitOne();
                    mManualResetEvent = null;
                }
                TcpClient = null;

            }

            if (mSendThread != null)
            {
                mSendThread.Stop();
                mSendThread = null;
            }
            Status = NetworkStatus.Disconnected;
            lock (mMessageQueue)
            {
                mMessageQueue.Enqueue(mObjectPool.Acquire().DisconnectedMessage(isPassive));
            }
            Debug.Log("Close Done");
        }

        void Receive()
        {
            try
            {
                TcpClient.GetStream().BeginRead(mReceiveData.Stream.GetBuffer(), (int)mReceiveData.Stream.Position, (int)(mReceiveData.Stream.Length - mReceiveData.Stream.Position), ReceiveCallback, TcpClient);
                Receiving = true;
            }
            catch (Exception exception)
            {
                SocketException socketException = exception as SocketException;
                if (socketException != null)
                {

                    mHandler.OnNetworkError(string.Format("SendThread socket error code = {0} see detail from https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2"
                        , socketException.ErrorCode));
                    return;
                }
                throw;
            }
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            if (mManualResetEvent != null)
            {
                Debug.Log("ReceiveCallback mManualResetEvent != null");
                mManualResetEvent.Set();

                Receiving = false;
                return;
            }
            try
            {
                SerializerStream Stream = mReceiveData.Stream;
                int bytesReceived = TcpClient.GetStream().EndRead(ar);
                if (bytesReceived <= 0)
                {
                    //if (mChannel.Status != NetworkStatus.Disconnecting)
                    //    mChannel.Disconnect();
                    Debug.Log("ReceiveCallback bytesReceived <= 0");

                    Receiving = false;
                    Close(true);
                    return;
                }

                Stream.SetPosition(Stream.Position + bytesReceived);
                if (Stream.Position < Stream.Length)
                {
                    Receive();
                    return;
                }

                Stream.SetPosition(0);
                if (mReceiveData.ProcessPacketHeader)
                {
                    int receiveSize = TcpNetworkChannel.ReadPacketHead(Stream);
                    Stream.SetPosition(0);
                    if (receiveSize > 0)
                    {
                        Stream.SafeSetLength(receiveSize);
                        mReceiveData.ProcessPacketHeader = false;
                    }
                    else
                    {
                        OnNetworkError("ReceiveCallback packet head error,length = " + receiveSize);
                    }
                }
                else
                {
                    System.Object packet = mHandler.DeserializePacket(Stream);
                    lock (mMessageQueue)
                    {
                        mMessageQueue.Enqueue(mObjectPool.Acquire().PacketMessage(packet));
                    }
                    Stream.SetPosition(0);
                    Stream.SafeSetLength(TcpNetworkChannel.PacketHeadLength);
                    mReceiveData.ProcessPacketHeader = true;
                }
                Receive();
            }
            catch (Exception exception)
            {
                SocketException socketException = exception as SocketException;
                if (socketException != null)
                {

                    OnNetworkError(string.Format("SendThread socket error code = {0} see detail from https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2"
                        , socketException.ErrorCode));

                }
                else
                {
                    OnNetworkError(exception.ToString());
                }
                Receiving = false;
                Close(true);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close(false);
                    mReceiveData.Dispose();
                    mSendThread.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    public class SendThread : ThreadBase, IDisposable
    {
        public delegate void MessageHandler(string msg);
        private const int DefaultBufferLength = 1024 * 64;
        protected INetworkChannelHandler mHandler;
        private SerializerStream mStream;
        private NetworkStream mNetworkStream;
        MessageHandler mMessageHandler = null;
        protected readonly Queue<System.Object> mSendPacketPool = new Queue<object>(32);
        private bool m_Disposed;
        public SendThread(INetworkChannelHandler handler, NetworkStream networkStream, MessageHandler messageHandler)
        {
            mMessageHandler = messageHandler;
            mHandler = handler;
            mNetworkStream = networkStream;
            mStream = new SerializerStream(DefaultBufferLength);
        }

        protected override bool MainLoop()
        {
            while (mSendPacketPool.Count > 0)
            {
                System.Object packet = null;
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
                    mNetworkStream.Write(mStream.GetBuffer(), 0, packetLength + TcpNetworkChannel.PacketHeadLength);
                }
                catch (Exception exception)
                {
                    SocketException socketException = exception as SocketException;
                    if (socketException != null)
                    {

                        mMessageHandler(string.Format("SendThread socket error code = {0} see detail from https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2"
                            , socketException.ErrorCode));
                        return false;
                    }
                    throw;
                }

            }
            Pause();
            return true;
        }

        public void Send(System.Object packet)
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

    public class ReceiveData : IDisposable
    {
        private const int DefaultBufferLength = 1024 * 64;
        public SerializerStream Stream;

        public bool ProcessPacketHeader = true;
        
        private bool m_Disposed;
        

        public ReceiveData()
        {
            Stream = new SerializerStream(DefaultBufferLength);
            ProcessPacketHeader = true;
            Stream.SafeSetLength(TcpNetworkChannel.PacketHeadLength);
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
                if (Stream != null)
                {
                    Stream.Dispose();
                    Stream = null;
                }
            }

            m_Disposed = true;
        }
    }
}
