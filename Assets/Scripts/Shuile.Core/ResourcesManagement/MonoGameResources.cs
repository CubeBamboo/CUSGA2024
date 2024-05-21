using CbUtils;
using Shuile.Core.Gameplay;

namespace Shuile
{
    [System.Obsolete("use LevelResources instead")]
    public class MonoGameResources : MonoSingletons<MonoGameResources>
    {
        public LevelDataMapSO levelDataMap;
    }
}
