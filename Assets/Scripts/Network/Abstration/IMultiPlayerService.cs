using Cysharp.Threading.Tasks;

namespace Shuile.Network.Abstration
{
    public interface IMultiPlayerService
    {
        public UniTask Init();
        public UniTask<IRoom> CreateRoom();
    }
}
