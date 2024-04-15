using LiteNetLib;
using LiteNetLib.Utils;

using Shuile.Network.Packets;

using System.Collections.Generic;

namespace Shuile.Network.LiteNetLib
{
    public class Host : IHost
    {
        private NetManager netManager;
        private EventBasedNetListener listener;
        private NetPacketProcessor processor;
        private List<int> actors;

        public IReadOnlyList<int> Actors => actors.AsReadOnly();
        public string ConnectionString { get; private set; }
        public bool CanJoin { get; set; }

        public Host(int port)
        {
            ConnectionString = "localhost:" + port.ToString();

            listener = new();

            listener.ConnectionRequestEvent += OnConnectionRequest;
            listener.PeerConnectedEvent += OnPeerConnected;
            listener.PeerDisconnectedEvent += OnPeerDisconnect;

            netManager = new(listener);
            netManager.Start(port);
        }

        public void SendTo<T>(int actorId, T packet) where T : Packet, new()
        {
            var idValid = netManager.TryGetPeerById(actorId, out var peer);
            if (!idValid)
                throw new KeyNotFoundException($"Couldn't find the peer by id {actorId}");

            var writer = new NetDataWriter();
            processor.Write(writer, packet);
            peer.Send(writer, 0, DeliveryMethod.ReliableOrdered);
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            if (!CanJoin)
                request.Reject();  // TODO: Reject info
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

        public void PollEvents()
        {
            netManager.PollEvents();
        }
    }
}
