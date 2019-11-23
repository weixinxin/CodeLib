using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Framework;
class ServerThread : ThreadBase
{
    TcpListener listener;
    TcpClient client;
    Byte[] bytes = new Byte[256];
    String data = null;
    int RespondCount = 0;
    public ServerThread(int port)
    {
        listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
    }

    bool listening = true;
    protected override bool MainLoop()
    {
        if(listening)
        {
            if (listener.Pending())
            {
                client = listener.AcceptTcpClient();
                Console.WriteLine("Accept a connection... ");
                listening = false;
                RespondCount = 0;
            }
            else
            {
                Thread.Sleep(300);
                return true;
            }
        }
        NetworkStream stream = client.GetStream();
        int i;
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            // Translate data bytes to a ASCII string.
            data = System.Text.Encoding.ASCII.GetString(bytes, 2, i - 2);
            Console.WriteLine("Received: {0}", data);

            // Process the data sent by the client.
            data = data.ToUpper();

            stream.Write(bytes, 0, i);
            Console.WriteLine("Sent: {0}", data);
            RespondCount++;
            if(RespondCount > 3)
            {
                client.Close();
                break;
            }
        }
        Console.WriteLine("Client Disconnected！");
        listening = true;
        return true;
    }

    protected override void OnEnter()
    {
        listener.Start();
        Console.WriteLine("Waiting for a connection... ");
    }

    protected override void OnExit()
    {
        Console.WriteLine("OnExitLoop... ");
        if (client != null)
        {
            client.Close();
            client = null;
        }

        if (listener != null)
        {
            listener.Stop();
            listener = null;
        }
    }
}