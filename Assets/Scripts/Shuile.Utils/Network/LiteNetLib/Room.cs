using Cysharp.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using Shuile.Network.Packets;
using System;
using System.Collections.Generic;
using System.Net;

namespace Shuile.Network.LiteNetLib
{
    public class Room : IRoom
    {
        private readonly NetManager client;
        private readonly IPEndPoint hostEndpoint;
        private NetPeer hostPeer;
        private readonly EventBasedNetListener listener;
        private NetPacketProcessor processor;

        public Room(IPEndPoint hostEndpoint)
        {
            listener = new EventBasedNetListener();
            client = new NetManager(listener);
            this.hostEndpoint = hostEndpoint;
        }

        public IReadOnlyList<int> Actors { get; }
        public bool IsJoined => hostPeer != null && hostPeer.ConnectionState == ConnectionState.Connected;

        public void SendToHost<T>(T packet) where T : Packet, new()
        {
            if (!IsJoined)
            {
                throw new Exception("Not connected");
            }

            var writer = new NetDataWriter();
            processor.Write(writer, packet);
            hostPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public async UniTask Join()
        {
            if (IsJoined || (hostPeer != null && hostPeer.ConnectionState == ConnectionState.Outgoing))
            {
                return;
            }

            await UniTask.RunOnThreadPool(async () =>
            {
                client.Start();
                var peer = client.Connect(hostEndpoint, MultiPlayerService.key);

                // TODO: Use listener event to replace wait unitl
                await UniTask.WaitUntil(() => peer.ConnectionState != ConnectionState.Outgoing);
                hostPeer = peer.ConnectionState == ConnectionState.Connected ? peer : null;
            });
        }
    }
}
