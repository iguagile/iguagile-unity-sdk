using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iguagile
{
    public class IguagileEventManager
    {
        public static event InstantiatedEventHandler Instantiated;
        public static event MigrateHostEventHandler ReceivedHostAuthority;
        public static event NewConnectionEventHandler NewConnection;
        public static event ExitConnectionEventHandler ExitConnection;

        internal static void InvokeInstantiated(int userId, IguagileView view)
        {
            Instantiated?.Invoke(userId, view);
        }

        internal static void InvokeReceivedHostAuthority()
        {
            ReceivedHostAuthority?.Invoke();
        }

        internal static void InvokeNewConnection(IguagileUser user)
        {
            NewConnection?.Invoke(user);
        }

        internal static void InvokeExitConnection(int userId)
        {
            ExitConnection?.Invoke(userId);
        }
    }
}
