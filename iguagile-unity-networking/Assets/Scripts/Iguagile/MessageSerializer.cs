using MessagePack;
using System.Linq;

namespace Iguagile
{
    public static class MessageSerializer
    {
        public static byte[] Serialize(RpcTargets target, MessageTypes messageType, params object[] message)
        {
            var serialized = LZ4MessagePackSerializer.Serialize(message);
            var data = new byte[] {(byte) target, (byte) messageType};
            return data.Concat(serialized).ToArray();
        }
    }
}