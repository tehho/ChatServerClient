using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatClient
{
    public class Server
    {
        private int _localPort;
        private IPAddress _localAddress;
        private SocketType _socketType;
        private ProtocolType _protocolType;
        private Socket _socket;
        private List<Client> _client;

        private Thread _listingnThread;
        private bool running;


        public Server()
        {
            _localPort = 5150;
            _localAddress = IPAddress.Any;
            _socketType = SocketType.Stream;
            _protocolType = ProtocolType.Tcp;
            _client = new List<Client>();
            _listingnThread = null;
        }

        public void StartListning()
        {
            IPEndPoint localEndPoint = new IPEndPoint(_localAddress, _localPort);
            _socket = new Socket(_localAddress.AddressFamily, _socketType, _protocolType);
            _socket.Bind(localEndPoint);
            _socket.Blocking = false;
            _socket.Listen(1);
            
            
            _listingnThread = new Thread(() =>
            {
                while (running)
                {
                    try
                    {
                        Socket stemp = _socket.Accept();

                        Client client = new Client(stemp, _protocolType, _socketType);

                        Program.StatusMessage("Client connected");

                        client.StartDecoding(DecodeMessage);

                        _client.Add(client);
                    }
                    catch (SocketException e)
                    {

                    }
                        
                }
            });

            running = true;
            _listingnThread.Start();
        }

        public void Close()
        {
            _client?.ForEach(client => client.Shutdown());

            running = false;
            _socket?.Close();
        }

        public void SendMessage(string message)
        {
            PackageData data = new PackageData();

            data.Add("Server");
            data.Add(message);

            Send(data);

        }

        public void Send(PackageData data)
        {
            _client.ForEach(client => client.Send(data));
        }

        void Remove(string name)
        {
            _client.RemoveAll((client) => client.Name == name);
        }

        void DecodeMessage(PackageData data, bool thread_running)
        {
            try
            {
                string name = data.GetString();
                string message = data.GetString();

                if (name == "command")
                {
                    if (message == "shutdown")
                    {
                        Remove(data.GetString());
                    }
                }
                else
                {
                    Program.StatusMessage($"{name} said {message}");
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}