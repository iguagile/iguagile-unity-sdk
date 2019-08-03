using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Iguagile
{
    internal class IguagileWebSocketClient : IIguagileClient
    {
        private ClientWebSocket client;

        private const int BufferSize = 1024;

        public event ConnectionEventHandler Open;
        public event ConnectionEventHandler Close;
        public event ReceivedEventHandler Received;


        public IguagileWebSocketClient()
        {
            client = new ClientWebSocket();
        }

        public bool IsConnect() => client?.State == WebSocketState.Open;

        public void Connect(string address, int port)
        {
            Task.Run(async () =>
            {
                await client.ConnectAsync(new Uri($"ws://{address}:{port}"), CancellationToken.None);
                Open?.Invoke();
                var buff = new ArraySegment<byte>(new byte[BufferSize]);
                while (client.State == WebSocketState.Open)
                {
                    try
                    {
                        var result = await client.ReceiveAsync(buff, CancellationToken.None);
                        Received?.Invoke(buff.Take(result.Count).ToArray());
                    }
                    catch
                    {
                        break;
                    }
                }

                Disconnect();
            });
        }

        public void Disconnect()
        {
            if (IsConnect())
            {
                client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                Close?.Invoke();
            }
        }

        public void Send(byte[] data)
        {
            if (IsConnect())
            {
                client.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true,
                    CancellationToken.None);
            }
        }

        public void Dispose()
        {
            client.Dispose();
            client = null;
        }
    }
}