using System;
using MessagePack;
using MessagePack.Resolvers;

namespace NZookeeper
{
    public class MsgPackSerializer
    {
        public byte[] Serialize(object obj)
        {
            return MessagePackSerializer.Serialize(obj, ContractlessStandardResolver.Options);
        }

        public T Deserialize<T>(Memory<byte> data)
        {
            return MessagePackSerializer.Deserialize<T>(data, ContractlessStandardResolver.Options);
        }
    }
}