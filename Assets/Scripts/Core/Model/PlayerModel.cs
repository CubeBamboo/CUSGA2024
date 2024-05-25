using Shuile.Core.Framework;
using Shuile.Framework;
using Shuile.Gameplay;

namespace Shuile
{
    // don't write logic here
    public class PlayerModel : IModel
    {
        public float currentHitOffset;

        public float faceDir = 1;
        public SmoothMoveCtrl moveCtrl;

        public bool canInviciable = true;
        public bool isInviciable = false;

        public LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }
}
