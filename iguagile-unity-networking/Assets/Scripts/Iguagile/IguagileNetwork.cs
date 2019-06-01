﻿using System;
using System.Linq;

namespace Iguagile
{
    public class IguagileNetwork
    {
        private static IIguagileClient Client;

        public static event ConnectionEventHandler Open;
        public static event ConnectionEventHandler Close;

        public static bool IsConnect => Client?.IsConnect() ?? false;

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

            Client.Open += Open;
            Client.Close += Close;
            Client.Received += ClientReceived;
            Client.Connect(address, port);
        }

        public static void Disconnect()
        {
            if (Client == null)
            {
                return;
            }

            Client.Disconnect();
            Client.Dispose();
            Client = null;
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

        internal static void ClientReceived(byte[] data)
        {
            var uuid = data.Take(16).ToArray();
            var userId = Convert.ToBase64String(uuid);
            var messageType = (MessageTypes) data[16];
            switch (messageType)
            {
                case MessageTypes.Transform:
                    IguagileManager.UpdateTransform(userId, data.Skip(17).ToArray());
                    break;
                case MessageTypes.Rpc:
                    IguagileManager.InvokeRpc(userId, data.Skip(17).ToArray());
                    break;
                case MessageTypes.Instantiate:
                    IguagileManager.Instantiate(userId, data);
                    break;
                case MessageTypes.NewConnection:
                    IguagileManager.AddUser(userId);
                    break;
                case MessageTypes.ExitConnection:
                    IguagileManager.RemoveUser(userId);
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