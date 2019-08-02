using MessagePack;
using System.Linq;

namespace Iguagile
{
    public class IguagileRpcInvoker
    {
        internal static void UpdateTransform(int userId, byte[] data)
        {
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
            var objects = LZ4MessagePackSerializer.Deserialize<object[]>(data);
            var objectId = (int)objects[0];
            var name = (string)objects[1];
            IguagileObjectManager.Instantiate(userId, objectId, name);
        }

        internal static void Destroy(int userId, byte[] data)
        {
            var objectId = (int)LZ4MessagePackSerializer.Deserialize<object[]>(data)[0];
            IguagileObjectManager.Destroy(userId, objectId);
        }

        internal static void RequestObjectControlAuthority(int userId, byte[] data)
        {

        }

        internal static void TransferObjectControlAuthority(int userId, byte[] data)
        {
            var objectId = (int)LZ4MessagePackSerializer.Deserialize<object[]>(data)[0];
            IguagileObjectManager.TransferObjectControlAuthority(objectId);
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