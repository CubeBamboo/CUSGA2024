using UnityEngine;

namespace Shuile.Gameplay.Move
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidbodyMoveCtrl : MonoBehaviour, IMoveController
    {
        private Rigidbody2D rb;
        private MoveAbility _ability;

        public MoveAbility Ability
        {
            get => _ability;
            set
            {
                if (value == _ability)
                    return;

                _ability = value;
            }
        }

        public float XMaxSpeed { get; set; }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector2 Velocity
        {
            get => rb.velocity;
            set
            {
                var vel = value.With(x: Mathf.Clamp(value.x, -XMaxSpeed, XMaxSpeed));
                rb.velocity = vel;
            }
        }

        public bool IsOnGround => Mathf.Approximately(Velocity.y, 0f) && rb.attachedColliderCount > 0;

        public float Deceleration { get; set; }
        public float Acceleration { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void XMove(float dir)
        {
            throw new System.NotImplementedException();
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            rb.velocity = rb.velocity.With(x: Mathf.MoveTowards(rb.velocity.x, 0, Deceleration));
        }
    }
}
