using UnityEngine;

namespace Iguagile
{
    public class IguagileTracker : MonoBehaviour
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Previous;
        private bool Received;

        public TrackerTypes TrackerType;

        void Update()
        {
            if (Received)
            {
                //transform.position = Vector3.Lerp(transform.position, Position, Time.deltaTime);
                //transform.rotation = Quaternion.Lerp(transform.rotation, Rotation, Time.deltaTime);
                transform.position = Position;
                transform.rotation = Rotation;
            }
        }

        public void UpdateTransform(IguagileTransform iguagileTransform)
        {
            Position = iguagileTransform.Position;
            Rotation = iguagileTransform.Rotation;
            Received = true;
        }

        public bool IsMove(float threshold)
        {
            var diff = transform.position - Previous;
            if (diff.sqrMagnitude < threshold)
            {
                return false;
            }

            Previous = transform.position;
            return true;
        }
    }
}