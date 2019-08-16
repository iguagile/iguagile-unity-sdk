using UnityEngine;

namespace Iguagile
{
    public class IguagileTransform
    {
        internal Vector3 NextPosition { get; set; }
        internal Vector3 PreviousPosition { get; set; }
        internal Quaternion NextRotation { get; set; }
        internal Quaternion PreviousRotation { get; set; }
        internal int ObjectId { get; set; }
        
        public IguagileTransform(Vector3 position, Quaternion rotation, int objectId)
        {
            PreviousPosition = Vector3.zero;
            PreviousRotation = Quaternion.identity;
            NextPosition = position;
            NextRotation = rotation;
            ObjectId = objectId;
        }

        public IguagileTransform(Transform transform, int objectId)
        {
            PreviousPosition = Vector3.zero;
            PreviousRotation = Quaternion.identity;
            NextPosition = transform.position;
            NextRotation = transform.rotation;
            ObjectId = objectId;
        }

        public void SetNext(Transform transform)
        {
            PreviousPosition = NextPosition;
            PreviousRotation = NextRotation;
            NextPosition = transform.position;
            NextRotation = transform.rotation;
        }

        public bool IsMove(float thresholdPositionSquare, float thresholdRotation)
        {
            return (NextPosition - PreviousPosition).sqrMagnitude > thresholdPositionSquare ||
                   Quaternion.Angle(NextRotation, PreviousRotation) > thresholdRotation;
        }
    }
}