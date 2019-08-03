using UnityEngine;

namespace Iguagile
{
    public class IguagileTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public int ObjectId { get; set; }

        public IguagileTransform(Vector3 position, Quaternion rotation, int objectId)
        {
            Position = position;
            Rotation = rotation;
            ObjectId = objectId;
        }

        public IguagileTransform(Transform transform, int objectId)
        {
            Position = transform.position;
            Rotation = transform.rotation;
            ObjectId = objectId;
        }
    }
}