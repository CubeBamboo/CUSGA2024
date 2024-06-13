using Shuile.Core.Framework;
using Shuile.Gameplay;
using Shuile.Gameplay.Move;

namespace Shuile
{
    public class PlayerModel
    {
        public float currentHitOffset;

        public float faceDir = 1;
        public SmoothMoveCtrl moveCtrl;

        public bool canInviciable = true;
        public bool isInviciable = false;
    }
}
