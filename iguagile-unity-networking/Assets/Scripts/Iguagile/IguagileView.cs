using System;
using System.Linq;

namespace Iguagile
{
    public class IguagileView : IguagileBehaviour
    {
        public IguagileTransformView TransformView;
        public bool IsMine { get; internal set; }
        public int ObjectId { get; internal set; }

        public void UpdateTransform(IguagileTransform iguagileTransform)
        {
            TransformView.UpdateTransform(iguagileTransform);
        }

        void OnDestroy()
        {
            var data = new byte[] {(byte) RpcTargets.Server, (byte) MessageTypes.Destroy};
            data = data.Concat(BitConverter.GetBytes(ObjectId)).ToArray();
            IguagileNetwork.Send(data);
        }
    }
}