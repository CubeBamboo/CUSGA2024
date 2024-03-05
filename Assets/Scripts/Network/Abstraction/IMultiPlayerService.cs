using Cysharp.Threading.Tasks;

namespace Shuile.Network
{
    public interface IMultiPlayerService
    {
        public bool IsConnected { get; }
        public UniTask Init();
        public UniTask<IRoom> CreateRoom();
    }
}
