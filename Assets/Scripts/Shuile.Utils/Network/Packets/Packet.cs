using LiteNetLib.Utils;

namespace Shuile.Network.Packets
{
    public abstract class Packet : INetSerializable  // �ƻ��˳�����Ϊ������demo������
    {
        public Packet() { }

        public abstract void Deserialize(NetDataReader reader);

        public abstract void Serialize(NetDataWriter writer);
    }
}
