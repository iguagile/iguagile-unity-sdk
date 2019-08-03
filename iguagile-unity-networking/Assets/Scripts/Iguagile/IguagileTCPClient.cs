using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Iguagile
{
    internal class IguagileTCPClient : IIguagileClient
    {
        private const int BufferSize = 1024;

        private TcpClient client;
        private NetworkStream stream;

        public event ConnectionEventHandler Open;
        public event ConnectionEventHandler Close;
        public event ReceivedEventHandler Received;

        public void Connect(string address, int port)
        {
            client = new TcpClient(address, port);
            Open?.Invoke();
            stream = client.GetStream();
            var messageSize = new byte[2];
            var buff = new byte[BufferSize];
            Task.Run(() =>
            {
                while (client.Connected)
                {
                    stream.Read(messageSize, 0, 2);
                    var size = BitConverter.ToUInt16(messageSize, 0);
                    var n = stream.Read(buff, 0, size);
                    if (n != size)
                    {
                        break;
                    }

                    Received?.Invoke(buff.Take(size).ToArray());
                }

                client.Dispose();
                stream.Dispose();
                Close?.Invoke();
            });
        }

        public void Disconnect()
        {
            if (IsConnect())
            {
                client.Close();
            }
        }

        public bool IsConnect()
        {
            return client?.Connected ?? false;
        }

        public void Send(byte[] data)
        {
            if (IsConnect() && (stream?.CanWrite ?? false))
            {
                var size = data.Length;
                var message = BitConverter.GetBytes((ushort)size);
                message = message.Concat(data).ToArray();
                stream.Write(message, 0, message.Length);
            }
        }

        public void Dispose()
        {
            client.Dispose();
            client = null;
        }
    }
}