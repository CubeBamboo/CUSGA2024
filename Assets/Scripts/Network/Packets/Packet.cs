using LiteNetLib.Utils;

namespace Shuile.Network.Packets
{
    public abstract class Packet : INetSerializable  // 破坏了抽象行为，但是demo就算了
    {
        public Packet() { }

        public abstract void Deserialize(NetDataReader reader);

        public abstract void Serialize(NetDataWriter writer);
    }
}
