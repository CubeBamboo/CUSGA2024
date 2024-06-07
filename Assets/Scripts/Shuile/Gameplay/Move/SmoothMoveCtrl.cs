using UnityEngine;

namespace Shuile.Gameplay.Move
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SmoothMoveCtrl : MonoBehaviour, IMoveController
    {
        private Rigidbody2D _rb;

        private float xAcc = 0.4f;
        private float xDeAcc = 0.25f;
        private float xMaxSpeed = 5f;

        private float jumpVel = 10f;

        private bool isFrozen;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if(_rb.bodyType != RigidbodyType2D.Static)
                _rb.velocity = new Vector2(Mathf.MoveTowards(_rb.velocity.x, 0, xDeAcc), _rb.velocity.y);
        }

        /// <summary> move next frame </summary>
        public void XMove(float dir)
        {
            // only accelerate when not reach max speed
            if(Mathf.Abs(_rb.velocity.x) < xMaxSpeed)
                _rb.velocity += new Vector2(dir * xAcc, 0);
        }

        public void AddForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
        {
            _rb.AddForce(force, forceMode);
        }

        public void SimpleJump(float jumpScale)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpVel * jumpScale);
        }

        public void HoldJump()
        {
            throw new System.NotImplementedException();
        }

        public bool IsFrozen
        {
            get => isFrozen;
            set
            {
                isFrozen = value;
                _rb.bodyType = isFrozen ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
            }
        }

        public Vector2 Velocity { get => _rb.velocity; set => _rb.velocity = value; }
        public float Acceleration { get => xAcc; set => xAcc = value; }
        public float XMaxSpeed { get => xMaxSpeed; set => xMaxSpeed = value; }
        public float Deceleration { get => xDeAcc; set => xDeAcc = value; }
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        public bool IsOnGround => Mathf.Abs(Velocity.y) < 1e-4 && _rb.attachedColliderCount > 0;
        public float JumpVelocity { get => jumpVel; set => jumpVel = value; }
        public float Gravity { get => _rb.gravityScale; set => _rb.gravityScale = value; }

        public MoveAbility Ability { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
