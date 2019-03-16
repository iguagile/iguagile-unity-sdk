using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Iguagile
{
    public class IguagileBehaviour : MonoBehaviour
    {
        public IguagileManager Manager;
        public bool IsAlive => Manager.Client.IsAlive;

        public void AppendTrackers(int playerId, params IguagileTracker[] trackers)
        {
            Manager.AppendTracker(playerId, trackers);
        }

        public void AppendPlayer(int playerId, params IguagileTracker[] trackers)
        {
            Manager.AppendPlayer(playerId, trackers);
        }

        public void AddRpc(string methodName)
        {
            Manager.AddRpc(methodName, this);
        }

        public void Rpc(string methodName, RpcTargets target, params object[] args)
        {
            Manager.Rpc(methodName, target, args);
        }

        public void OnRpc(string methodName, params object[] args)
        {
            var type = GetType();
            var method = type.GetMethod(methodName,
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(this, args);
        }
    }
}