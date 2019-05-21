using UnityEngine;

namespace Iguagile
{
    public class IguagileTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public IguagileTransformTypes TransformType { get; set; }

        public IguagileTransform(Vector3 position, Quaternion rotation, IguagileTransformTypes transformType)
        {
            Position = position;
            Rotation = rotation;
            TransformType = transformType;
        }

        public IguagileTransform(Transform transform, IguagileTransformTypes transformType)
        {
            Position = transform.position;
            Rotation = transform.rotation;
            TransformType = transformType;
        }
    }
}