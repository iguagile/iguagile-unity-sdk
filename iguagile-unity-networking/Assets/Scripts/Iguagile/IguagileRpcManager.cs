using System.Collections.Generic;
using System.Reflection;

namespace Iguagile
{
    public class IguagileRpcManager
    {
        private static Dictionary<string, IguagileBehaviour> _rpcMethods = new Dictionary<string, IguagileBehaviour>();
        
        internal static void InvokeRpc(string methodName, object[] args)
        {
            if (!_rpcMethods.ContainsKey(methodName))
            {
                return;
            }

            var behaviour = _rpcMethods[methodName];
            var type = behaviour.GetType();
            var flag = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance;
            var method = type.GetMethod(methodName, flag);
            method?.Invoke(behaviour, args);
        }

        internal static void AddRpc(string methodName, IguagileBehaviour receiver)
        {
            _rpcMethods.Add(methodName, receiver);
        }

        internal static void RemoveRpc(string methodName)
        {
            if (_rpcMethods.ContainsKey(methodName))
            {
                _rpcMethods.Remove(methodName);
            }
        }

        internal static void Clear()
        {
            _rpcMethods.Clear();
        }
    }
}
