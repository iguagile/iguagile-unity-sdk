using UnityEngine;

namespace Iguagile
{
    public delegate void ConnectionEventHandler();
    public delegate void ReceivedEventHandler(byte[] message);
    public delegate void InstantiatedEventHandler(int userId, IguagileView view);
    public delegate void MigrateHostEventHandler();
    public delegate void NewConnectionEventHandler(IguagileUser user);
}