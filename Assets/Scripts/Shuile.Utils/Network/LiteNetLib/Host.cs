using LiteNetLib;
using LiteNetLib.Utils;
using Shuile.Network.Packets;
using System.Collections.Generic;

namespace Shuile.Network.LiteNetLib
{
    public class Host : IHost
    {
        private List<int> actors;
        private readonly EventBasedNetListener listener;
        private readonly NetManager netManager;
        private NetPacketProcessor processor;

        public Host(int port)
        {
            ConnectionString = "localhost:" + port;

            listener = new EventBasedNetListener();

            listener.ConnectionRequestEvent += OnConnectionRequest;
            listener.PeerConnectedEvent += OnPeerConnected;
            listener.PeerDisconnectedEvent += OnPeerDisconnect;

            netManager = new NetManager(listener);
            netManager.Start(port);
        }

        public IReadOnlyList<int> Actors => actors.AsReadOnly();
        public string ConnectionString { get; }
        public bool CanJoin { get; set; }

        public void SendTo<T>(int actorId, T packet) where T : Packet, new()
        {
            var idValid = netManager.TryGetPeerById(actorId, out var peer);
            if (!idValid)
            {
                throw new KeyNotFoundException($"Couldn't find the peer by id {actorId}");
            }

            var writer = new NetDataWriter();
            processor.Write(writer, packet);
            peer.Send(writer, 0, DeliveryMethod.ReliableOrdered);
        }

        public void PollEvents()
        {
            netManager.PollEvents();
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            if (!CanJoin)
            {
                request.Reject(); // TODO: Reject info
            }

            request.AcceptIfKey(MultiPlayerService.key);
        }

        private void OnPeerConnected(NetPeer peer)
        {
            actors.Add(peer.Id);
        }

        private void OnPeerDisconnect(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            actors.Remove(peer.Id);
        }
    }
}
