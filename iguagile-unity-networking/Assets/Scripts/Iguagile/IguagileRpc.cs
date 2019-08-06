using MessagePack;
using System;
using System.Linq;
using System.Text;
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
            Debug.Log(data.Length);
            var objectId = BitConverter.ToInt32(data, 0);
            var name = Encoding.UTF8.GetString(data, 4, data.Length - 4);
            IguagileDispatcher.BeginInvoke(() => IguagileObjectManager.Instantiate(userId, objectId, name));
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
            var objectId = (int) LZ4MessagePackSerializer.Deserialize<object[]>(data)[0];
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