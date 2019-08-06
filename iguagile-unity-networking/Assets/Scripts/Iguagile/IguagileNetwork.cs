using System;
using System.Linq;
using UnityEngine;

namespace Iguagile
{
    public class IguagileNetwork
    {
        private static IIguagileClient Client;

        public static event ConnectionEventHandler Open;
        public static event ConnectionEventHandler Close;

        public static bool IsConnect => Client?.IsConnect() ?? false;

        /// <summary>
        /// Connect to a server.
        /// </summary>
        /// <param name="address">The address of the server to connect to.</param>
        /// <param name="port">The port number of the server to connect to.</param>
        /// <param name="protocol">transport layer protocol</param>
        public static void Connect(string address, int port, Protocol protocol)
        {
            if (Client == null)
            {
                Client = NewClient(protocol);
            }

            if (Client.IsConnect())
            {
                throw new Exception("Already connected");
            }

            var obj = new GameObject();
            var dispatcher = obj.AddComponent<IguagileDispatcher>();
            GameObject.DontDestroyOnLoad(obj);
            IguagileDispatcher.Dispatcher = dispatcher;

            Client.Open += Open;
            Client.Close += Close;
            Client.Received += ClientReceived;
            Client.Connect(address, port);
            IguagileObjectSynchronizer.SyncStart();
        }

        /// <summary>
        /// Close the connection and dispose the client.
        /// </summary>
        public static void Disconnect()
        {
            if (Client == null)
            {
                return;
            }

            IguagileObjectSynchronizer.SyncStop();
            Client.Disconnect();
            Client.Dispose();
            Client = null;
            IguagileObjectManager.Clear();
            IguagileRpcManager.Clear();
            IguagileUserManager.Clear();
        }

        private static IIguagileClient NewClient(Protocol protocol)
        {
            switch (protocol)
            {
                case Protocol.TCP:
                    return new IguagileTCPClient();
                case Protocol.WebSocket:
                    return new IguagileWebSocketClient();
                default:
                    throw new ArgumentException("Invalid protocol");
            }
        }

        private const int HeaderSize = 3;

        internal static void ClientReceived(byte[] data)
        {
            var id = BitConverter.ToUInt16(data, 0) << 16;
            var messageType = (MessageTypes) data[2];
            switch (messageType)
            {
                case MessageTypes.Transform:
                    IguagileRpc.UpdateTransform(id, data.Skip(HeaderSize).ToArray());
                    break;
                case MessageTypes.Rpc:
                    IguagileRpc.InvokeRpc(id, data.Skip(HeaderSize).ToArray());
                    break;
                case MessageTypes.Instantiate:
                    IguagileRpc.Instantiate(id, data.Skip(HeaderSize).ToArray());
                    break;
                case MessageTypes.Destroy:
                    IguagileRpc.Destroy(id, data.Skip(3).ToArray());
                    break;
                case MessageTypes.RequestObjectControlAuthority:
                    IguagileRpc.RequestObjectControlAuthority(id, data.Skip(HeaderSize).ToArray());
                    break;
                case MessageTypes.TransferObjectControlAuthority:
                    IguagileRpc.TransferObjectControlAuthority(id, data.Skip(HeaderSize).ToArray());
                    break;
                case MessageTypes.NewConnection:
                    IguagileRpc.AddUser(id);
                    break;
                case MessageTypes.ExitConnection:
                    IguagileRpc.RemoveUser(id);
                    break;
                case MessageTypes.MigrateHost:
                    IguagileRpc.MigrateHost();
                    break;
                case MessageTypes.Register:
                    IguagileRpc.Register(id);
                    break;
            }
        }

        internal static void Send(byte[] data)
        {
            if (data.Length >= (1 << 16) - 16)
            {
                throw new Exception("too long data");
            }

            if (data.Length != 0)
            {
                Client.Send(data);
            }
        }
    }
}