using Cysharp.Threading.Tasks;

namespace Shuile.Network
{
    public interface IMultiPlayerService
    {
        public UniTask Init();
        public UniTask<IRoom> CreateRoom();
    }
}
