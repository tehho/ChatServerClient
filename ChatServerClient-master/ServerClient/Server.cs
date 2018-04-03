using System;
using System.Collections.Generic;
using System.Linq;
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

        private ThreadWraper _listeningThread;


        public Server()
        {
            _localPort = 5150;
            _localAddress = IPAddress.Any;
            _socketType = SocketType.Stream;
            _protocolType = ProtocolType.Tcp;
            _client = new List<Client>();
            _listeningThread = null;
        }

        public void StartListning()
        {
            IPEndPoint localEndPoint = new IPEndPoint(_localAddress, _localPort);
            _socket = new Socket(_localAddress.AddressFamily, _socketType, _protocolType);
            _socket.Bind(localEndPoint);
            _socket.Blocking = false;
            _socket.Listen(1);
            
            
            _listeningThread = new ThreadWraper(() =>
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
            });

            _listeningThread.Start();
        }

        public void Close()
        {
            _client?.ForEach(client => client?.Shutdown());

            _listeningThread?.Abort();

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
            for (int i = 0; i < _client.Count; i++)
            {
                try
                {
                    _client[i].Send(data);
                }
                catch (SocketException e)
                {
                    _client.RemoveAt(i);
                    i--;
                }
            }
        }

        void Remove(string name)
        {
            var list = _client.Where((client) => client.Name == name);

            _client.RemoveAll((client) => list.Contains(client));

            foreach (var client in list)
            {
                client.Shutdown();
            }
        }

        void DecodeMessage(PackageData data)
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
                    Send(data);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}