using System;
using System.Linq;
using UnityEngine;

namespace Iguagile
{
    public class IguagileBehaviour : MonoBehaviour
    {
        public void RegisterRpcMethods()
        {
            foreach (var info in typeof(IguagileBehaviour).GetMethods())
            {
                var attributes = Attribute.GetCustomAttributes(info, typeof(IguagileRpcAttribute));
                foreach (var attribute in attributes)
                {
                    var attr = attribute as IguagileRpcAttribute;
                    if (attr != null)
                    {
                        IguagileRpcManager.AddRpc(info.Name, this);
                        break;
                    }
                }
            }
        }

        public void UnregisterRpcMethods()
        {
            IguagileRpcManager.RemoveRpc(this);
        }

        public static void Rpc(string methodName, RpcTargets target, params object[] args)
        {
            var objects = new object[] {methodName};
            objects = objects.Concat(args).ToArray();
            var data = MessageSerializer.Serialize(target, MessageTypes.Rpc, objects);
            IguagileNetwork.Send(data);
        }

        public static void Rpc(string methodName, RpcTargets target)
        {
            var data = MessageSerializer.Serialize(target, MessageTypes.Rpc, methodName);
            IguagileNetwork.Send(data);
        }
    }
}