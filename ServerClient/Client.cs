using System;
using System.Net;
using System.Net.Sockets;

namespace ChatClient
{
    public class Client
    {
        private ProtocolType _protocolType;
        private SocketType _socketType;
        private Socket _socket;
        public string Name { get; set; }

        private ThreadWraper _thread;

        public Client(Socket socket, ProtocolType protocolType, SocketType socketType)
        {
            _socket = socket;
            _socket.Blocking = false;
            _protocolType = protocolType;
            _socketType = socketType;
            Name = "";
        }

        public PackageData Receive()
        {
            PackageData ret = new PackageData();

            int rc;
            byte[] buffer = new byte[PackageData.BufferSize];

            if (_protocolType == ProtocolType.Tcp)
            {
                while (true)
                {
                    try
                    {
                        rc = _socket.Receive(buffer);
                        ret.Add(buffer, rc);
                        if (rc == PackageData.BufferSize)
                            continue;
                        break;
                    }
                    catch (ObjectDisposedException ode)
                    {
                        break;
                    }
                    catch (SocketException e)
                    {
                    }
                }
            }

            return ret;
        }

        public void Send(PackageData data)
        {
            if (_protocolType == ProtocolType.Tcp)
            {
                _socket.Send(data.Data);
            }
        }

        public void Shutdown()
        {
            _thread?.Abort();
            _socket?.Shutdown(SocketShutdown.Both);
            _socket?.Close();
        }

        public void StartDecoding(Action<PackageData> Decode)
        {
            
            _thread = new ThreadWraper(() =>
            {
                    Decode(Receive());
            });
            _thread.Start();
        }
    }
}