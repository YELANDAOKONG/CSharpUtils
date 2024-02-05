using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public interface CMTPHandler
{
    public void Handle(TcpClient client, IPEndPoint address, StreamReader reader, StreamWriter writer);
}

public class CMTP
{
    private TcpListener _server;
    private bool _isRunning;
    private CMTPHandler _handler;

    public CMTP(int port, CMTPHandler handler)
    {
        _handler = handler;
        _server = new TcpListener(IPAddress.Any, port);
        // _server.Start();
        // _isRunning = false;
        //
        // LoopClients();
    }

    public void Start()
    {
        _server.Start();
        _isRunning = false;

        LoopClients();
    }

    private void LoopClients()
    {
        while (_isRunning)
        {
            // wait for client connection
            TcpClient newClient = _server.AcceptTcpClient();

            // client found.
            // create a thread to handle communication
            Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
            t.Start(newClient);
        }
    }

    private void HandleClient(object obj)
    {
        // retrieve client from parameter passed to thread
        TcpClient client = (TcpClient)obj;

        // sets two streams
        StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
        StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
        bool bClientConnected = true;
        string sData = null;

        var clientAddress = (IPEndPoint)client.Client.RemoteEndPoint;

        while (bClientConnected)
        {
            // reads from stream
            sData = sReader.ReadLine();
            
            _handler.Handle(client, clientAddress, sReader, sWriter);

            // // shows content on the console.
            // Console.WriteLine("Client (" + clientAddress.Address.ToString() + ") > " + sData);
            //
            // // to write something back.
            // sWriter.WriteLine("Server > " + sData);
            sWriter.Flush();
        }
    }
}
