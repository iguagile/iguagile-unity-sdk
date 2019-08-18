using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iguagile
{
    public class IguagileEventManager
    {
        public static event InstantiatedEventHandler Instantiated;

        public static void InvokeInstantiated(int userId, IguagileView view)
        {
            Instantiated?.Invoke(userId, view);
        }
    }
}
