using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatClient
{
    public class Client
    {
        private Socket _socket;
        private SocketType _socketType;
        private ProtocolType _protocolType;
        private bool _udpConnect;
        private IPEndPoint _destination;
        private string _name;
        private ThreadWraper _receiveThread;

        public Client(string name)
        {
            _socket = null;
            _socketType = SocketType.Stream;
            _protocolType = ProtocolType.Tcp;
            _udpConnect = false;
            _destination = null;
            _name = name;
        }

        public bool Connect(string remoteName, int remotePort)
        {
            try
            {
                Program.StatusMessage("Querying for host IP from DNS");
                var resolvedHost = Dns.GetHostEntry(remoteName);

                foreach (var addr in resolvedHost.AddressList)
                {
                    _socket = new Socket(addr.AddressFamily, _socketType, _protocolType);
                    try
                    {
                        Program.StatusMessage($"Attempting to connect to server: {addr}");
                        _destination = new IPEndPoint(addr, remotePort);

                        if (_protocolType == ProtocolType.Udp && !_udpConnect)
                        {
                            break;
                        }

                        _socket.Connect(_destination);
                        Program.StatusMessage($"Connection is OK...");
                        _socket.Blocking = false;
                    }
                    catch (SocketException e)
                    {
                        Program.StatusMessage($"Connection failed...");
                        _socket.Close();
                        _socket = null;
                        continue;
                    }
                }
            }
            catch (Exception e)
            {
                Program.ErrorMessage(e.Message);
            }

            return _socket != null && _destination != null;
        }

        public void StartReceiving(Action<PackageData> Decode)
        {
            _receiveThread = new ThreadWraper(() =>
            {
                    Decode(Receive());
            });
            _receiveThread.Start();
        }

        public void SendMessage(string message)
        {
            PackageData data = new PackageData();

            data.Add(_name);
            data.Add(message);

            Send(data);
        }

        public void Send(PackageData data)
        {
            if (_socket == null || _destination == null)
            {
                throw new InvalidOperationException("You need to establish a connection first");
            }

            if (_protocolType == ProtocolType.Udp && !_udpConnect)
            {
                _socket.SendTo(data.Data, _destination);
            }
            else
            {
                _socket.Send(data.Data);
            }
        }

        public PackageData Receive()
        {
            PackageData ret = new PackageData();

            try
            {
                while (true)
                {
                    int rc = 0;
                    byte[] buffer = new byte[PackageData.BufferSize];

                    if (_protocolType == ProtocolType.Tcp || _udpConnect)
                    {
                        rc = _socket.Receive(buffer);

                    }
                    else
                    {
                        IPEndPoint fromEndPoint = new IPEndPoint(_destination.Address, 0);
                        EndPoint castFromEndPoint = fromEndPoint;
                        rc = _socket.ReceiveFrom(buffer, ref castFromEndPoint);
                        fromEndPoint = (IPEndPoint) castFromEndPoint;
                    }

                    ret.Add(buffer, rc);

                    if (rc == PackageData.BufferSize)
                    {
                        continue;
                    }

                    break;
                }
            }
            catch (Exception e)
            {

            }

            return ret;
        }

        public void Close()
        {
            if (_socket != null)
            {
                PackageData test = new PackageData();

                test.Add("command");
                test.Add("shutdown");
                test.Add(_name);

                Send(test);

                _receiveThread.Abort();

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
        }
    }
}