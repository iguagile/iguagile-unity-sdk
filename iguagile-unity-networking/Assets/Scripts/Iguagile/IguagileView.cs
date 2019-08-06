using System;
using System.Linq;
using UnityEngine;

namespace Iguagile
{
    public class IguagileView : MonoBehaviour
    {
        public IguagileTransformView TransformView;
        public bool IsMine { get; internal set; }
        public int ObjectId { get; internal set; } = -1;

        public void UpdateTransform(IguagileTransform iguagileTransform)
        {
            TransformView.UpdateTransform(iguagileTransform);
        }

        void OnDestroy()
        {
            if (!IsMine || ObjectId < 0)
            {
                return;
            }
            var data = new byte[] {(byte) RpcTargets.Server, (byte) MessageTypes.Destroy};
            data = data.Concat(BitConverter.GetBytes(ObjectId)).ToArray();
            IguagileNetwork.Send(data);
        }
    }
}