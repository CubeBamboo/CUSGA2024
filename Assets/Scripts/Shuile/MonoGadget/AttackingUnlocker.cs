using Shuile.Gameplay.Character;
using UnityEngine;

namespace Shuile.MonoGadget
{
    public class AttackingUnlocker : MonoBehaviour
    {
        public NormalPlayerCtrl ctrl;

        public void UnlockAttacking()
        {
            ctrl.AttackingLock = false;
        }
    }
}
