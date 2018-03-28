using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatClient
{
    public class Client
    {
        private ProtocolType _protocolType;
        private SocketType _socketType;
        private Socket _socket;
        public string Name { get; set; }

        private Thread _decodeThread;
        private bool running;

        public Client(Socket socket, ProtocolType protocolType, SocketType socketType)
        {
            _socket = socket;
            _protocolType = protocolType;
            _socketType = socketType;
            Name = "";
            running = false;
        }

        public PackageData Receive()
        {
            PackageData ret = new PackageData();

            int rc = 0;
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
            _socket?.Shutdown(SocketShutdown.Both);
            _socket?.Close();

            _decodeThread?.Abort();
        }

        public void StartDecoding( Action<PackageData, bool> Decode)
        {
            
            _decodeThread = new Thread(() =>
            {
                while (running)
                {
                    Decode(Receive(), running);
                }
            });

            running = true;
            _decodeThread.Start();
        }
    }
}