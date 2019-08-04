using System;
using UnityEngine;

namespace Iguagile
{
    [RequireComponent(typeof(IguagileView))]
    public class IguagileBehaviour : MonoBehaviour
    {
        private IguagileView _view;

        public IguagileView View
        {
            get
            {
                if (_view == null)
                {
                    _view = GetComponent<IguagileView>();
                }

                return _view;
            }
        }

        public int ObjectId => View?.ObjectId ?? 0;

        public bool IsMine => View?.IsMine ?? false;

        /// <summary>
        /// Initialization method to register rpc methods.
        /// </summary>
        public void RegisterRpcMethods()
        {
            foreach (var info in typeof(IguagileBehaviour).GetMethods())
            {
                var attributes = Attribute.GetCustomAttributes(info, typeof(IguagileRpcAttribute));
                foreach (var attribute in attributes)
                {
                    if (attribute is IguagileRpcAttribute)
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
        
        /// <summary>
        /// Call RPC method via the server.
        /// </summary>
        /// <param name="methodName">RPC method name.</param>
        /// <param name="target">RPC target.</param>
        /// <param name="args">RPC method arguments</param>
        public void Rpc(string methodName, RpcTargets target, params object[] args)
        {
            IguagileRpcManager.Rpc(methodName, target, args);
        }

        /// <summary>
        /// Call RPC method via the server.
        /// </summary>
        /// <param name="methodName">RPC method name.</param>
        /// <param name="target">RPC target.</param>
        public void Rpc(string methodName, RpcTargets target)
        {
            IguagileRpcManager.Rpc(methodName, target);
        }

        public void TransferObjectControlAuthority()
        {
            IguagileObjectManager.TransferObjectControlAuthority(ObjectId);
        }
    }
}