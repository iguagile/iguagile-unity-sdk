using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Iguagile
{
    public class IguagileRpcManager
    {
        private static Dictionary<string, IguagileBehaviour> _rpcMethods = new Dictionary<string, IguagileBehaviour>();
        
        /// <summary>
        /// Call RPC method via the server.
        /// </summary>
        /// <param name="methodName">RPC method name.</param>
        /// <param name="target">RPC target.</param>
        /// <param name="args">RPC method arguments</param>
        public static void Rpc(string methodName, RpcTargets target, params object[] args)
        {
            var objects = new object[] {methodName};
            objects = objects.Concat(args).ToArray();
            var data = MessageSerializer.Serialize(target, MessageTypes.Rpc, objects);
            IguagileNetwork.Send(data);
        }

        /// <summary>
        /// Call RPC method via the server.
        /// </summary>
        /// <param name="methodName">RPC method name.</param>
        /// <param name="target">RPC target.</param>
        public static void Rpc(string methodName, RpcTargets target)
        {
            var data = MessageSerializer.Serialize(target, MessageTypes.Rpc, methodName);
            IguagileNetwork.Send(data);
        }
        
        internal static void AddRpc(string methodName, IguagileBehaviour receiver)
        {
            _rpcMethods.Add(methodName, receiver);
        }
        
        internal static void RemoveRpc(IguagileBehaviour receiver)
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
