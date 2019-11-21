using Framework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class TcpNetTest : MonoBehaviour,INetworkChannelHandler
{
    public struct Msg
    {
        public string text;
        public int id;
    }
    public string ip = "";
    public int port = 8080;

    public string sendMsg = "";

    public bool connect = false;

    public bool discinnect = false;

    public bool send = false;

    TcpNetworkChannel channel;

    
    private void Awake()
    {
        Framework.Debug.SetLogger(new FrameworkTest.Logger());

        NetworkManager.Initialize();

        channel = new TcpNetworkChannel("testTCP", this);

    }
    void Update()
    {
        if(connect)
        {
            connect = false;
            Connect();
        }
        if (discinnect)
        {
            discinnect = false;
            Disconnect();
        }
        if (send)
        {
            send = false;
            SendMessage();
        }
        channel.Update(Time.deltaTime);
    }

    void Connect()
    {
        IPAddress[] ips = Dns.GetHostAddresses(ip);
        channel.Connect(ips[0], port, null);
    }

    void Disconnect()
    {
        if(channel.Status == NetworkStatus.Connected)
            channel.Disconnect();
    }

    void SendMessage()
    {
        if (channel.Status == NetworkStatus.Connected)
        {
            Msg msg = new Msg() { text = sendMsg, id = 100 };
            channel.Send(msg);
        }
    }


    public object DeserializePacket(Stream source)
    {
        byte[] buffer = new byte[source.Length - source.Position];
        source.Read(buffer, 0, buffer.Length);
        string data = System.Text.Encoding.ASCII.GetString(buffer, 0, buffer.Length);
        return data;
    }

    public void HandlePacket(object packet)
    {
        string data = (string)packet;
        UnityEngine.Debug.Log("HandlePacket" + data);
    }

    public void OnConnectResult(bool success, int errorCode)
    {
        UnityEngine.Debug.Log("OnConnectResult " + success);
    }

    public void OnDisconnected(bool isPassive)
    {
        UnityEngine.Debug.Log("OnDisconnected  isPassive = " + isPassive);
    }

    public void OnNetworkError(string errorMessage)
    {
        UnityEngine.Debug.Log("OnNetworkError " + errorMessage);
    }

    public bool SerializePacket(object packet, Stream destination)
    {
        string data = (string)packet;
        byte[] bs = System.Text.Encoding.ASCII.GetBytes(data);
        destination.Write(bs, 0, bs.Length);
        return true;
    }

}
