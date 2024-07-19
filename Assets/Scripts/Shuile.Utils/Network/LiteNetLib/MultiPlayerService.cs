using Cysharp.Threading.Tasks;
using System.Globalization;
using System.Net;
using UnityEngine.Assertions;

namespace Shuile.Network.LiteNetLib
{
    public class MultiPlayerService : IMultiPlayerService
    {
        public static readonly int defaultPort = 50721;
        public static readonly string key = "CUSGA2024";

        public bool IsConnected { get; private set; }

        public UniTask<IHost> CreateRoom(string initDescription = null)
        {
            var port = ParseInitDescription(initDescription);
            var host = new Host(port);
            return UniTask.FromResult((IHost)host);
        }

        public UniTask Init()
        {
            IsConnected = true;
            return UniTask.CompletedTask;
        }

        public async UniTask<IRoom> JoinRoomAsync(string connectionString)
        {
            Assert.IsNotNull(connectionString);
            var room = new Room(ParseConnectionString(connectionString));
            await room.Join();
            return room;
        }

        private int ParseInitDescription(string initDescription)
        {
            var port = defaultPort; // TODO: Random default port
            if (initDescription != null)
            {
                var succeeded = int.TryParse(initDescription, out port);
                if (!succeeded)
                {
                    throw new ConnectionStringFormatInvalidException(initDescription);
                }
            }

            return port;
        }

        private static IPEndPoint ParseConnectionString(string connectionString)
        {
            var ep = connectionString.Split(':');
            if (ep.Length < 2)
            {
                throw new ConnectionStringFormatInvalidException(connectionString);
            }

            IPAddress ip;
            if (ep.Length > 2)
            {
                // Maybe ipv6
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new ConnectionStringFormatInvalidException(connectionString);
                }
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new ConnectionStringFormatInvalidException(connectionString);
                }
            }

            int port;
            if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new ConnectionStringFormatInvalidException(connectionString);
            }

            return new IPEndPoint(ip, port);
        }
    }
}
