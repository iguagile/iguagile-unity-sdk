using System.Collections.Generic;
using System.Reflection;

namespace Iguagile
{
    public class IguagileRpcManager
    {
        private static Dictionary<string, IguagileBehaviour> _rpcMethods = new Dictionary<string, IguagileBehaviour>();

        public static void AddRpc(string methodName, IguagileBehaviour receiver)
        {
            _rpcMethods.Add(methodName, receiver);
        }

        public static void RemoveRpc(string methodName)
        {
            if (_rpcMethods.ContainsKey(methodName))
            {
                _rpcMethods.Remove(methodName);
            }
        }

        public static void RemoveRpc(IguagileBehaviour receiver)
        {
            var removeList = new List<string>();
            foreach (var rpcMethod in _rpcMethods)
            {
                if (ReferenceEquals(rpcMethod.Value, receiver))
                {
                    removeList.Add(rpcMethod.Key);
                }
            }

            foreach (var method in removeList)
            {
                _rpcMethods.Remove(method);
            }
        }
        
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

        internal static void Clear()
        {
            _rpcMethods.Clear();
        }
    }
}
