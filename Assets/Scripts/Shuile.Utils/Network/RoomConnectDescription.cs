using System.Net;

namespace Shuile.Network
{
    public class RoomConnectDescription
    {
        public string UniqueId { get; set; }
        public IPEndPoint EndPoint { get; set; }
    }
}
