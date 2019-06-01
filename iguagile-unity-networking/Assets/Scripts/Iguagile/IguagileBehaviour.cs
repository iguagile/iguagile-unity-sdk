using System.Linq;
using UnityEngine;

namespace Iguagile
{
    public class IguagileBehaviour : MonoBehaviour
    {
        void OnDestroy()
        {
            IguagileManager.RemoveRpc(this);
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