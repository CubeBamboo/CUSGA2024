using Cysharp.Threading.Tasks;
using Shuile.Network.Packets;
using System.Collections.Generic;

namespace Shuile.Network
{
    public interface IRoom
    {
        /// <summary>
        ///     当前已连接的所有角色的Id
        /// </summary>
        public IReadOnlyList<int> Actors { get; }

        /// <summary>
        ///     是否已经进入房间
        /// </summary>
        public bool IsJoined { get; }

        /// <summary>
        ///     发送数据包到主机（加入到待发送队列罢了）
        /// </summary>
        /// <typeparam name="T">数据包类型</typeparam>
        /// <param name="packet">数据包</param>
        public void SendToHost<T>(T packet) where T : Packet, new();

        /// <summary>
        ///     试图加入房间（如果已经连接则无事发生）
        /// </summary>
        public UniTask Join();
    }
}
