using Shuile.Core.Framework;
using Shuile.Gameplay;

namespace Shuile
{
    public class PlayerModel : IModel
    {
        public float currentHitOffset;

        public float faceDir = 1;
        public SmoothMoveCtrl moveCtrl;

        public bool canInviciable = true;
        public bool isInviciable = false;

        public ModuleContainer GetModule() => GameApplication.Level;
    }
}
