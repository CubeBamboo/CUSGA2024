using Cysharp.Threading.Tasks;

namespace Shuile.Network
{
    public interface IMultiPlayerService
    {
        /// <summary>
        ///     是否已连接到服务
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        ///     初始化服务，使用别的功能前必须调用
        /// </summary>
        public UniTask Init();

        /// <summary>
        ///     创建主机房间
        /// </summary>
        /// <param name="initDescription">房间预定义信息</param>
        /// <returns>主机房间对象</returns>
        /// <exception cref="ConnectionStringFormatInvalidException">连接字符串格式无效</exception>
        public UniTask<IHost> CreateRoom(string initDescription = null);

        /// <summary>
        ///     加入房间
        /// </summary>
        /// <param name="connectionString">通过<see cref="IHost.ConnectionString" />获取</param>
        /// <returns>房间对象（作为客机）</returns>
        /// <exception cref="ConnectionStringFormatInvalidException">连接字符串格式无效</exception>
        public UniTask<IRoom> JoinRoomAsync(string connectionString);
    }
}

// TapSDK中，链接均来自TapServer只有一个连接，能拿到的信息只有ActorId，发包人的Id是唯一可信的
// LiteNetLib中，链接来自各个客户端，可以确保包是谁发的

// TapSDK中，由MasterClient可以被转移，LiteNetLib中需要手动实现此功能（不做这个功能了）

// MasterClient主要体现在游戏逻辑上
