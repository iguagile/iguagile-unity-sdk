using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Iguagile
{
    public static class MessageSerializer
    {
        public static byte[] Serialize(RpcTargets target, MessageTypes messageType, params object[] message)
        {
            var serialized = LZ4MessagePackSerializer.Serialize(message);
            var data = new byte[] { (byte)target, (byte)messageType };
            return data.Concat(serialized).ToArray();
        }
    }
}
