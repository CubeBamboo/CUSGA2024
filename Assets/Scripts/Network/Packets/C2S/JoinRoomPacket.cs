using System.Drawing;

namespace Shuile.Network.Packets.C2S
{
    public class JoinRoomPacket : BasePacket
    {
        public string Name { get; set; }
        public Color Color { get; set; }
    }
}
