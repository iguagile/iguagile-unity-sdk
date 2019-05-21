using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iguagile
{
    public interface IIguagileClient : IDisposable
    {
        event ConnectionEventHandler Open;
        event ConnectionEventHandler Close;
        event ReceivedEventHandler Received;

        bool IsConnect();
        void Connect(string address, int port);
        void Disconnect();
        void Send(byte[] data);
    }
}