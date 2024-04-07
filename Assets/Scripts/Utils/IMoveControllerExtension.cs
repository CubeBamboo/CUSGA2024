using Shuile.Gameplay;

using UnityEngine;

namespace Shuile
{
    public static class IMoveControllerExtension
    {
        public static void TryJump(this IMoveController moveable, float force)
        {
            if (!moveable.Ability.HasFlag(MoveAbility.Jumpable) || !moveable.IsOnGround)
                return;

            moveable.Velocity += new Vector2(0f, force);
        }

        public static void XAddForce(this IMoveController moveable, float force)
        {
            /*if (!moveable.IsOnGround && !moveable.Ability.HasFlag(MoveAbility.Flyable))
                return;*/

            moveable.Velocity += new Vector2(force, 0f);
        }
    }
}
