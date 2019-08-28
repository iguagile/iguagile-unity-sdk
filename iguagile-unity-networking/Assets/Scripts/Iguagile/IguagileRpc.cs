using MessagePack;
using System;
using System.Linq;
using UnityEngine;

namespace Iguagile
{
    public class IguagileRpc
    {
        internal static void UpdateTransform(int userId, byte[] data)
        {
            var transforms = IguagileTransformSerializer.Deserialize(data);
            IguagileObjectSynchronizer.UpdateTransform(transforms);
        }

        internal static void InvokeRpc(int userId, byte[] data)
        {
            var objects = LZ4MessagePackSerializer.Deserialize<object[]>(data);
            var methodName = (string) objects[0];
            var args = objects.Skip(1).ToArray();
            IguagileRpcManager.InvokeRpc(methodName, args);
        }

        internal static void Instantiate(int userId, byte[] data)
        {
            var objectId = BitConverter.ToInt32(data, 0);
            var objects = LZ4MessagePackSerializer.Deserialize<object[]>(data.Skip(4).ToArray());
            var path = (string)objects[0];
            var position = new Vector3((float)objects[1], (float)objects[2], (float)objects[3]);
            var rotation = new Quaternion((float)objects[4], (float)objects[5], (float)objects[6], (float)objects[7]);
            IguagileDispatcher.BeginInvoke(() => IguagileObjectManager.Instantiate(userId, objectId, path, position, rotation));
        }

        internal static void Destroy(int userId, byte[] data)
        {
            var objectId = BitConverter.ToInt32(data, 0);
            IguagileDispatcher.BeginInvoke((() => IguagileObjectManager.Destroy(objectId)));
        }

        internal static void RequestObjectControlAuthority(int userId, byte[] data)
        {

        }

        internal static void TransferObjectControlAuthority(int userId, byte[] data)
        {
            var objectId = BitConverter.ToInt32(data, 0);
            IguagileObjectManager.ReceiveObjectControlAuthority(objectId);
        }

        internal static void AddUser(int userId)
        {
            IguagileUserManager.AddUser(userId);
        }

        internal static void RemoveUser(int userId)
        {
            IguagileUserManager.RemoveUser(userId);
        }

        internal static void MigrateHost()
        {
            IguagileUserManager.MigrateHost();
        }

        internal static void Register(int userId)
        {
            IguagileUserManager.Register(userId);
        }
    }
}