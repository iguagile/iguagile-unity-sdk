using MessagePack;
using UnityEngine;

namespace Iguagile
{
    public static class TransformSerializer
    {
        private const int DataLength = 8;

        public static byte[] Serialize(params IguagileTransform[] iguagileTransforms)
        {
            var objects = iguagileTransforms.ToObjectArray();
            return LZ4MessagePackSerializer.Serialize(objects);
        }

        public static IguagileTransform[] Deserialize(byte[] serialized)
        {
            var objects = LZ4MessagePackSerializer.Deserialize<object[]>(serialized);
            return objects.ToTrackerArray();
        }

        private static object[] ToObjectArray(this IguagileTransform[] iguagileTransforms)
        {
            var length = (byte)iguagileTransforms.Length;
            object[] objects = new object[length * DataLength + 1];
            objects[0] = length;
            for (var i = 0; i < iguagileTransforms.Length; i++)
            {
                objects[i * DataLength + 1] = (byte)iguagileTransforms[i].TrackerType;
                objects[i * DataLength + 2] = iguagileTransforms[i].Position.x;
                objects[i * DataLength + 3] = iguagileTransforms[i].Position.y;
                objects[i * DataLength + 4] = iguagileTransforms[i].Position.z;
                objects[i * DataLength + 5] = iguagileTransforms[i].Rotation.x;
                objects[i * DataLength + 6] = iguagileTransforms[i].Rotation.y;
                objects[i * DataLength + 7] = iguagileTransforms[i].Rotation.z;
                objects[i * DataLength + 8] = iguagileTransforms[i].Rotation.w;
            }

            return objects;
        }

        private static IguagileTransform[] ToTrackerArray(this object[] objects)
        {
            var length = (byte)objects[0];
            IguagileTransform[] iguagileTransforms = new IguagileTransform[length];
            for (var i = 0; i < length; i++)
            {
                var trackerType = (byte)objects[i * DataLength + 1];
                var position = new Vector3(
                    (float)objects[i * DataLength + 2],
                    (float)objects[i * DataLength + 3],
                    (float)objects[i * DataLength + 4]
                    );
                var rotation = new Quaternion(
                    (float)objects[i * DataLength + 5],
                    (float)objects[i * DataLength + 6],
                    (float)objects[i * DataLength + 7],
                    (float)objects[i * DataLength + 8]
                    );
                iguagileTransforms[i] = new IguagileTransform(position, rotation, (TrackerTypes)trackerType);
            }

            return iguagileTransforms;
        }
    }
}