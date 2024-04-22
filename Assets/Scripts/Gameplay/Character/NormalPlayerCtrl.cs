using CbUtils;
using Shuile.Framework;
using Shuile.Rhythm.Runtime;
using Shuile.Root;
using UnityEngine;

namespace Shuile.Gameplay
{
    public interface IPlayerCtrl
    {
        void NormalMove(float xInput);
        void SingleJump();
        void Attack();
    }

    public class NormalPlayerCtrl : MonoBehaviour
    {
        public enum State
        {
            Normal,
            JumpUp,
            Drop
        }
        private FSM<State> mFsm = new();

        private Player player;
        private NormalPlayerInput mPlayerInput;
        private Rigidbody2D _rb;

        // [normal move]
        [SerializeField] private float acc = 0.4f;
        [SerializeField] private float deAcc = 0.25f;
        [SerializeField] private float xMaxSpeed = 5f;

        // [jump]
        [SerializeField] private float jumpVel = 18f;
        [SerializeField] private float normalGravity = 3f;
        [SerializeField] private float dropGravity = 4f;
        
        // [attack]
        [SerializeField] private float attackRadius = 2.8f;

        private readonly PlayerModel playerModel = GameplayService.Interface.Get<PlayerModel>();

        public EasyEvent OnTouchGround { get; } = new();
        public PlayerAttackCommand attackCommand { get; } = new();
        public PlayerJumpCommand jumpCommand { get; } = new();
        public PlayerMoveCommand moveCommand { get; } = new();

        public bool CheckRhythm =>
            MusicRhythmManager.Instance.CheckBeatRhythm(
                MusicRhythmManager.Instance.CurrentTime, out playerModel.currentHitOffset);

        public Rigidbody2D Rb => _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            player = GetComponent<Player>();

            mPlayerInput = GetComponent<NormalPlayerInput>();
            ConfigureInputEvent();
            InitFSM();
        }

        private void OnDestroy()
        {
            ClearInputEvent();
        }

        private void Start()
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }

        private void FixedUpdate()
        {
            mFsm.FixedUpdate();

            // [normal move update]
            _rb.velocity = new Vector2(Mathf.MoveTowards(_rb.velocity.x, 0, deAcc), _rb.velocity.y);
        }

        /// <summary> update velocity </summary>
        public void NormalMove(float xInput)
        {
            moveCommand
                .Bind(new()
                {
                    xInput = xInput,
                    rb = _rb,
                    acc = acc,
                    xMaxSpeed = xMaxSpeed
                })
                .Execute();
        }

        public void SingleJump()
        {
            if (mFsm.CurrentStateId == State.JumpUp || mFsm.CurrentStateId == State.Drop) return;

            jumpCommand
                .Bind(new()
                {
                    rb = _rb,
                    jumpVel = jumpVel
                })
                .Execute();
            mFsm.SwitchState(State.JumpUp);
        }

        public void Attack()
        {
            if (LevelRoot.Instance.needHitWithRhythm && !CheckRhythm) return;

            attackCommand
                .Bind(new()
                {
                    position = transform.position,
                    attackRadius = attackRadius,
                    attackPoint = player.Property.attackPoint
                })
                .Execute();
        }

        private void CheckGround()
        {
            // easy way to fix groundcheck is true on jumping first frame, but it maybe lead to other bug
            var groundCheck = Rb.velocity.y < 0 &&
                  Physics2D.Raycast(transform.position + new Vector3(0, -1.3f, 0), Vector2.down, 0.3f, LayerMask.GetMask("Ground"));
            if (groundCheck)
            {
                mFsm.SwitchState(State.Normal);
                OnTouchGround.Invoke();
            }
        }

        private void ConfigureInputEvent()
        {
            mPlayerInput.OnJump += SingleJump;
            mPlayerInput.OnAttack += Attack;
            mPlayerInput.OnMove += NormalMove;
        }

        private void ClearInputEvent()
        {
            mPlayerInput.OnJump -= SingleJump;
            mPlayerInput.OnAttack -= Attack;
            mPlayerInput.OnMove -= NormalMove;
        }

        private void InitFSM()
        {
            mFsm.NewEventState(State.Normal)
                .OnEnter(() =>
                {
                    _rb.gravityScale = normalGravity;
                });
            mFsm.NewEventState(State.JumpUp)
                .OnFixedUpdate(() =>
                {
                    CheckGround();

                    if (_rb.velocity.y < 0)
                    {
                        mFsm.SwitchState(State.Drop);
                    }
                });
            mFsm.NewEventState(State.Drop)
                .OnEnter(() =>
                {
                    _rb.gravityScale = dropGravity;
                })
                .OnFixedUpdate(() =>
                {
                    CheckGround();
                });
            mFsm.StartState(State.Normal);
        }
    }

    public struct PlayerAttackCommandData
    {
        public Vector2 position;
        public float attackRadius;
        public int attackPoint;
    }
    public class PlayerAttackCommand : BaseCommand<PlayerAttackCommandData>
    {
        public override void OnExecute()
        {
            var hits = Physics2D.OverlapCircleAll(state.position, state.attackRadius, LayerMask.GetMask("Enemy")); // TODO: static you hua 
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].GetComponent<IHurtable>().OnHurt(state.attackPoint);
            }
        }
    }

    public struct PlayerJumpCommandData
    {
        public Rigidbody2D rb;
        public float jumpVel;
    }
    public class PlayerJumpCommand : BaseCommand<PlayerJumpCommandData>
    {
        public override void OnExecute()
        {
            state.rb.velocity = new Vector2(state.rb.velocity.x, state.jumpVel);
        }
    }

    public struct PlayerMoveCommandData
    {
        public float xInput;
        public Rigidbody2D rb;
        public float acc;
        public float xMaxSpeed;
    }
    public class PlayerMoveCommand : BaseCommand<PlayerMoveCommandData>
    {
        public override void OnExecute()
        {
            state.rb.velocity += new Vector2(state.xInput * state.acc, 0);
            state.rb.velocity = new Vector2(Mathf.Clamp(state.rb.velocity.x, -state.xMaxSpeed, state.xMaxSpeed), state.rb.velocity.y);
        }
    }
}
