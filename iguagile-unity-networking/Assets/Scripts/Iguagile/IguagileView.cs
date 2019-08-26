using UnityEngine;

namespace Iguagile
{
    public class IguagileView : MonoBehaviour
    {
        public IguagileTransformView TransformView;
        public bool IsMine { get; internal set; }
        public int ObjectId { get; internal set; } = -1;

        internal void UpdateTransform(IguagileTransform iguagileTransform)
        {
            TransformView.UpdateTransform(iguagileTransform);
        }
    }
}