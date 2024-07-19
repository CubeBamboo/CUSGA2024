using System;
using UnityEngine;

namespace Shuile.Gameplay.Move
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SmoothMoveCtrl : MonoBehaviour, IMoveController
    {
        private Rigidbody2D _rb;

        private bool isFrozen;

        public bool IsFrozen
        {
            get => isFrozen;
            set
            {
                isFrozen = value;
                _rb.bodyType = isFrozen ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
            }
        }

        public float JumpVelocity { get; set; } = 10f;
        public float Gravity { get => _rb.gravityScale; set => _rb.gravityScale = value; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (_rb.bodyType != RigidbodyType2D.Static)
            {
                _rb.velocity = new Vector2(Mathf.MoveTowards(_rb.velocity.x, 0, Deceleration), _rb.velocity.y);
            }
        }

        /// <summary> move next frame </summary>
        public void XMove(float dir)
        {
            // only accelerate when not reach max speed
            if (Mathf.Abs(_rb.velocity.x) < XMaxSpeed)
            {
                _rb.velocity += new Vector2(dir * Acceleration, 0);
            }
        }

        public Vector2 Velocity { get => _rb.velocity; set => _rb.velocity = value; }
        public float Acceleration { get; set; } = 0.4f;
        public float XMaxSpeed { get; set; } = 5f;
        public float Deceleration { get; set; } = 0.25f;
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        public bool IsOnGround => Mathf.Abs(Velocity.y) < 1e-4 && _rb.attachedColliderCount > 0;

        public MoveAbility Ability
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void AddForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
        {
            _rb.AddForce(force, forceMode);
        }

        public void SimpleJump(float jumpScale)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, JumpVelocity * jumpScale);
        }

        public void HoldJump()
        {
            throw new NotImplementedException();
        }
    }
}
