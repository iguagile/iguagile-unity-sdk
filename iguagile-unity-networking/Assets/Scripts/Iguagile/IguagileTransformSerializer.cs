using MessagePack;
using UnityEngine;

namespace Iguagile
{
    public static class IguagileTransformSerializer
    {
        private const int DataLength = 8;

        public static byte[] Serialize(params IguagileTransform[] transforms)
        {
            var objects = transforms.ToObjectArray();
            return MessageSerializer.Serialize(RpcTargets.OtherClients, MessageTypes.Transform, objects);
        }

        public static IguagileTransform[] Deserialize(byte[] serialized)
        {
            var objects = LZ4MessagePackSerializer.Deserialize<object[]>(serialized);
            return objects.ToTransformArray();
        }

        private static object[] ToObjectArray(this IguagileTransform[] transforms)
        {
            var length = (byte)transforms.Length;
            object[] objects = new object[length * DataLength + 1];
            objects[0] = length;
            for (var i = 0; i < length; i++)
            {
                var j = i * DataLength;
                objects[j + 1] = (byte)transforms[i].TransformType;
                objects[j + 2] = transforms[i].Position.x;
                objects[j + 3] = transforms[i].Position.y;
                objects[j + 4] = transforms[i].Position.z;
                objects[j + 5] = transforms[i].Rotation.x;
                objects[j + 6] = transforms[i].Rotation.y;
                objects[j + 7] = transforms[i].Rotation.z;
                objects[j + 8] = transforms[i].Rotation.w;
            }

            return objects;
        }

        private static IguagileTransform[] ToTransformArray(this object[] objects)
        {
            var length = (byte)objects[0];
            IguagileTransform[] transforms = new IguagileTransform[length];
            for (var i = 0; i < length; i++)
            {
                var j = i * DataLength;
                var transformType = (byte)objects[j + 1];
                var position = new Vector3(
                    (float)objects[j + 2],
                    (float)objects[j + 3],
                    (float)objects[j + 4]
                    );
                var rotation = new Quaternion(
                    (float)objects[j + 5],
                    (float)objects[j + 6],
                    (float)objects[j + 7],
                    (float)objects[j + 8]
                    );
                transforms[i] = new IguagileTransform(position, rotation, (IguagileTransformTypes)transformType);
            }

            return transforms;
        }
    }
}