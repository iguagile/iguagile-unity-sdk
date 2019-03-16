using System;
using System.Linq;
using UnityEngine;
using WebSocketSharp;

namespace Iguagile
{
    public class IguagileClient
    {
        private WebSocket webSocket;

        public IguagileManager Manager;
        public bool IsAlive;

        public event EventHandler<EventArgs> Open;
        public event EventHandler<CloseEventArgs> Close;
        public event EventHandler<ErrorEventArgs> Error;

        protected virtual void OnOpen(EventArgs e)
        {
            IsAlive = true;
            Open?.Invoke(this, e);
        }

        protected virtual void OnClose(CloseEventArgs e)
        {
            IsAlive = false;
            Close?.Invoke(this, e);
        }

        protected virtual void OnError(ErrorEventArgs e)
        {
            Debug.Log(e.Exception);
            Error?.Invoke(this, e);
        }

        public void Connect(string url)
        {
            webSocket = new WebSocket(url);
            webSocket.OnMessage += WebSocket_OnMessage;
            webSocket.OnError += (s, e) => OnError(e);
            webSocket.OnOpen += (s, e) => OnOpen(e);
            webSocket.OnClose += (s, e) => { OnClose(e); };
            webSocket.ConnectAsync();
        }

        internal void Send(byte[] data)
        {
            if (!IsAlive)
            {
                return;
            }
            webSocket.Send(data);
        }

        internal void Disconnect()
        {
            webSocket.Close();
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            var playerId = BitConverter.ToInt32(e.RawData, 0);
            var messageType = (MessageTypes)e.RawData[4];
            switch (messageType)
            {
                case MessageTypes.Trackers:
                    var trackerData = e.RawData.Skip(5).ToArray();
                    Manager.ReceivedTracker(playerId, trackerData);
                    break;
                case MessageTypes.Rpc:
                    var rpcData = e.RawData.Skip(5).ToArray();
                    Manager.ReceivedRpcAsync(playerId, rpcData);
                    break;
                case MessageTypes.Open:
                    Manager.ReceivedOpenMessage(playerId);
                    break;
                case MessageTypes.Close:
                    Manager.ReceivedCloseMessage(playerId);
                    break;
            }
        }
    }

}