using UnityEngine;

namespace Iguagile
{
    public class IguagileTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public TrackerTypes TrackerType { get; set; }

        public IguagileTransform(Vector3 position, Quaternion rotation, TrackerTypes trackerType)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.TrackerType = trackerType;
        }

        public IguagileTransform(Transform transform, TrackerTypes trackerType)
        {
            Position = transform.position;
            Rotation = transform.rotation;
            this.TrackerType = trackerType;
        }
    }
}