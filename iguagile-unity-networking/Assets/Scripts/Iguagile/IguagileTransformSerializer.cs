using System;
using System.Linq;
using MessagePack;
using UnityEngine;

namespace Iguagile
{
    public static class IguagileTransformSerializer
    {
        private const int DataLength = 11;

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
            var length = (byte) transforms.Length;
            object[] objects = new object[length * DataLength + 1];
            objects[0] = length;
            for (var i = 0; i < length; i++)
            {
                var j = i * DataLength;
                var idByte = BitConverter.GetBytes(transforms[i].ObjectId);
                objects[j + 1] = idByte[0];
                objects[j + 2] = idByte[1];
                objects[j + 3] = idByte[2];
                objects[j + 4] = idByte[3];
                objects[j + 5] = transforms[i].Position.x;
                objects[j + 6] = transforms[i].Position.y;
                objects[j + 7] = transforms[i].Position.z;
                objects[j + 8] = transforms[i].Rotation.x;
                objects[j + 9] = transforms[i].Rotation.y;
                objects[j + 10] = transforms[i].Rotation.z;
                objects[j + 11] = transforms[i].Rotation.w;
            }

            return objects;
        }

        private static IguagileTransform[] ToTransformArray(this object[] objects)
        {
            var length = (byte) objects[0];
            IguagileTransform[] transforms = new IguagileTransform[length];
            for (var i = 0; i < length; i++)
            {
                var j = i * DataLength;
                var idByte = objects.Skip(j + 1).Take(4).Select(x => (byte) x).ToArray();
                var objectId = BitConverter.ToInt32(idByte, 0);
                var position = new Vector3(
                    (float) objects[j + 5],
                    (float) objects[j + 6],
                    (float) objects[j + 7]
                );
                var rotation = new Quaternion(
                    (float) objects[j + 8],
                    (float) objects[j + 9],
                    (float) objects[j + 10],
                    (float) objects[j + 11]
                );
                transforms[i] = new IguagileTransform(position, rotation, objectId);
            }

            return transforms;
        }
    }
}