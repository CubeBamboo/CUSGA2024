using Shuile.Framework;
using Shuile.Gameplay;

namespace Shuile
{
    // don't write logic here
    public class PlayerModel
    {
        public float currentHitOffset;

        public float faceDir = 1;

        public SmoothMoveCtrl moveCtrl;

        public bool isInviciable = false;
    }
}
