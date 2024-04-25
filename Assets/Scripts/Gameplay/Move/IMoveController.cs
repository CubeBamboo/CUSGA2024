using System;

using UnityEngine;

namespace Shuile.Gameplay
{
    [Flags]
    public enum MoveAbility
    {
        Flyable = 1,
        Jumpable = 2
    }

    public interface IMoveController
    {
        public MoveAbility Ability { get; set; }

        public float Acceleration { get; set; }
        public float XMaxSpeed { get; set; }
        public float Deceleration { get; set; }
        public Vector3 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public bool IsOnGround { get; }
        /// <param name="dir"> lower than 0: left, higher than 0: right </param>
        public void XMove(float dir);
    }
}
