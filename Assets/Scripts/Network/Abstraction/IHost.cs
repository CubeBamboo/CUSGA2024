using Shuile.Network.Packets;

using System.Collections.Generic;

namespace Shuile.Network
{
    public interface IHost
    {
        /// <summary>
        /// 当前已连接的所有角色的Id
        /// </summary>
        public IReadOnlyList<int> Actors { get; }

        /// <summary>
        /// 连接到此房间用到的连接字符串
        /// </summary>
        public string ConnectionString { get; }
        
        /// <summary>
        /// 是否可以加入房间
        /// </summary>
        public bool CanJoin { get; set; }

        /// <summary>
        /// 发送给某个角色
        /// </summary>
        /// <typeparam name="T">数据包类型</typeparam>
        /// <param name="actorId">角色的id</param>
        /// <param name="packet">数据包</param>
        public void SendTo<T>(int actorId, T packet) where T : Packet, new();

        /// <summary>
        /// 处理数据包，在Unity主线程调用
        /// </summary>
        public void PollEvents();
    }
}
