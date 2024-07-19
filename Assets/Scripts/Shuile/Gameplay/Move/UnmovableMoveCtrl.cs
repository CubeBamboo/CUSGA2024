using System;
using UnityEngine;

namespace Shuile.Gameplay.Move
{
    public class UnmovableMoveCtrl : MonoBehaviour, IMoveController
    {
        public MoveAbility Ability
        {
            get => 0;
            set { }
        }

        public float XMaxSpeed
        {
            get => 0f;
            set { }
        }

        public Vector3 Position
        {
            get => transform.position;
            set { }
        }

        public Vector2 Velocity
        {
            get => Vector2.zero;
            set { }
        }

        public bool IsOnGround => true;

        public float Deceleration
        {
            get => float.PositiveInfinity;
            set { }
        }

        public float Acceleration
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void XMove(float dir)
        {
            throw new NotImplementedException();
        }
    }
}
