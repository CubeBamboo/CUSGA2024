using Shuile.Framework;

using DG.Tweening;
using UnityEngine;
using CbUtils;

namespace Shuile.Gameplay
{
    public interface IPlayerCtrl
    {
        void NormalMove(float xInput);
        void SingleJump();
        void Attack();
    }

    public class NormalPlayerCtrl : MonoBehaviour, IComponent<Player>
    {
        // [normal move]
        [SerializeField] private float acc = 0.4f;
        [SerializeField] private float deAcc = 0.25f;
        [SerializeField] private float xMaxSpeed = 5f;

        // [jump]
        [SerializeField] private float jumpVel = 5f;

        // [attack]
        [SerializeField] private float attackRadius = 2.8f;

        // [anim manage]
        private PlayerAnimCtrl animCtrl;

        private Rigidbody2D _rb;
        public Rigidbody2D Rb => _rb;

        private Player mTarget;
        public Player Target { set => mTarget = value; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            animCtrl = new(gameObject);   
        }

        private void Start()
        {
            mTarget.OnDie.Register(() => animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Die))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            mTarget.CurrentHp.Register((val) => animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Hurt))
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }

        private void FixedUpdate()
        {
            // [normal move]
            _rb.velocity = new Vector2(Mathf.MoveTowards(_rb.velocity.x, 0, deAcc), _rb.velocity.y);

            // [anim logic]
            animCtrl.XVelocity = _rb.velocity.x;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(transform.position, attackRadius);
        }

        /// <summary> update velocity </summary>
        public void NormalMove(float xInput)
        {
            // [normal move]
            _rb.velocity += new Vector2(xInput * acc, 0);
            _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -xMaxSpeed, xMaxSpeed), _rb.velocity.y);

            // [anim logic]
            animCtrl.FlipX = xInput < 0;
        }

        public void TouchGround()
        {
            // [anim logic]
            animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Land);
        }

        public void SingleJump()
        {
            // [jump]
            _rb.velocity = new Vector2(_rb.velocity.x, jumpVel);

            // [anim logic]
            animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Jump);
            MonoAudioCtrl.Instance.PlayOneShot("Player_Jump");
        }

        public void Attack()
        {
            //var grid = LevelGrid.Instance.grid;
            var hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, LayerMask.GetMask("Enemy")); // TODO: static you hua 
            //var (objects, size) = LevelGrid.Instance.GetObjectsIn(transform.position.ToCell(grid), LayerMask.GetMask("Enemy")); //TODO: GCCCCCCCCC
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].GetComponent<IHurtable>().OnAttack(mTarget.Property.attackPoint);
            }

            // [anim logic]
            animCtrl.Trigger(PlayerAnimCtrl.AnimTrigger.Attack);
            MonoAudioCtrl.Instance.PlayOneShot("Player_Attack");
        }
    }
}
