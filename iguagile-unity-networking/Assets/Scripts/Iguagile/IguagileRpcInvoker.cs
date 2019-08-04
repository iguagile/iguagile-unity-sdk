using System;
using MessagePack;
using System.Linq;
using System.Text;

namespace Iguagile
{
    public class IguagileRpcInvoker
    {
        internal static void UpdateTransform(int userId, byte[] data)
        {
            var serialized = (byte[])LZ4MessagePackSerializer.Deserialize<object[]>(data)[0];
            var transforms = IguagileTransformSerializer.Deserialize(serialized);
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
            var name = Encoding.UTF8.GetString(data, 4, data.Length - 4);
            IguagileObjectManager.Instantiate(userId, objectId, name);
        }

        internal static void Destroy(int userId, byte[] data)
        {
            var objectId = BitConverter.ToInt32(data, 0);
            IguagileObjectManager.Destroy(objectId);
        }

        internal static void RequestObjectControlAuthority(int userId, byte[] data)
        {

        }

        internal static void TransferObjectControlAuthority(int userId, byte[] data)
        {
            var objectId = (int)LZ4MessagePackSerializer.Deserialize<object[]>(data)[0];
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