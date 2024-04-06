using System;

using UnityEngine;

namespace Shuile.Gameplay
{
    [Flags]
    public enum MoveAbility
    {
        Flyable,
        Jumpable,
    }

    public interface IMoveController
    {
        public MoveAbility Ability { get; set; }
        public float MaxSpeed { get; set; }
        public float Deceleration { get; set; }
        public Vector3 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public bool IsOnGround { get; }
    }
}
