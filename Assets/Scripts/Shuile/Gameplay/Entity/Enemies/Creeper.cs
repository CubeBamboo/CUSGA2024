using CbUtils;
using CbUtils.Event;
using CbUtils.Extension;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class Creeper : Enemy
    {
        [SerializeField] private float jumpVel = 10f;
        [SerializeField] private float xMaxSpeed = 5f;
        [SerializeField] private float checkPlayerRange = 5f;
        [SerializeField] private float checkAttackRange = 1.2f;
        [SerializeField] private float attackRange = 2f;
        private readonly FSM<DefaultEnemyState> _mFsm = new();
        private ZakoChaseBehavior _chaseBehavior;
        private float _faceDir = 1f;

        private SpriteRenderer _mRenderer;

        private ZakoPatrolBehavior _patrolBehavior;

        public override void Awake()
        {
            base.Awake();
            RegisterState(_mFsm);

            _patrolBehavior = new ZakoPatrolBehavior(gameObject, moveController, 5f);
            _chaseBehavior = new ZakoChaseBehavior();
            _mRenderer = GetComponentInChildren<SpriteRenderer>();
            moveController.JumpVelocity = jumpVel;
            moveController.XMaxSpeed = xMaxSpeed;
        }

        private void Start()
        {
            EnterAnim().Forget();
        }

        private void OnDestroy()
        {
            _mRenderer.DOKill();
        }

        private async UniTaskVoid EnterAnim()
        {
            transform.localScale = Vector3.one * 0.1f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.4f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.7f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 1f;
        }

        private async UniTask ExitAnim()
        {
            _mRenderer.DOKill();
            _mRenderer.DOColor(Color.black, 0.4f)
                .OnComplete(() => _mRenderer.DOFade(0, 0.2f));
            await UniTask.Delay(600, cancellationToken: destroyCancellationToken);
        }

        private void Update()
        {
            _mFsm.Update();
        }

        private void FixedUpdate()
        {
            _mFsm.FixedUpdate();
        }

        protected override async void OnSelfDie()
        {
            _mFsm.SwitchState(DefaultEnemyState.Empty);
            moveController.IsFrozen = true;

            try
            {
                await ExitAnim();
            }
            catch (OperationCanceledException)
            {
            }
            Destroy(gameObject);
        }

        protected void RegisterState(FSM<DefaultEnemyState> mFsm)
        {
            mFsm.NewEventState(DefaultEnemyState.Patrol)
                .OnFixedUpdate(() =>
                {
                    _patrolBehavior.Do();
                    _faceDir = _patrolBehavior.FaceDir;
                    if (EnemyBehaviorAction.XRayCastPlayer(transform.position, _faceDir, checkPlayerRange))
                    {
                        mFsm.SwitchState(DefaultEnemyState.Chase);
                    }

                    EnemyBehaviorAction.CheckWallAndJump(moveController, _faceDir);
                });
            mFsm.NewEventState(DefaultEnemyState.Chase)
                .OnEnter(() =>
                {
                    var player = GetPlayerOrThrow();
                    _chaseBehavior.Bind(player.gameObject, moveController);
                })
                .OnFixedUpdate(() =>
                {
                    if (!EnemyBehaviorAction.XRayCastPlayer(transform.position, _faceDir, checkPlayerRange))
                    {
                        mFsm.SwitchState(DefaultEnemyState.Patrol);
                    }

                    UnityAPIExtension.DebugLineForRayCast2D(transform.position, Vector2.right * _faceDir,
                        checkPlayerRange, LayerMask.GetMask("Player"));

                    _faceDir = _chaseBehavior.FaceDir;
                    _chaseBehavior.Do();

                    if (_chaseBehavior.XCloseEnoughToTarget(checkAttackRange))
                    {
                        mFsm.SwitchState(DefaultEnemyState.Attack);
                    }

                    EnemyBehaviorAction.CheckWallAndJump(moveController, _faceDir);
                });
            mFsm.NewEventState(DefaultEnemyState.Attack)
                .OnCustom(() => Attack());
            mFsm.NewEventState(DefaultEnemyState.Empty);

            mFsm.StartState(DefaultEnemyState.Patrol);
        }

        private bool Attack()
        {
            var player = GetPlayerOrThrow();
            if (Vector3.Distance(player.transform.position, MoveController.Position) <= attackRange)
            {
                player.OnHurt((int)(player.Property.maxHealthPoint * 0.6f));
            }

            ForceDie();
            return true;
        }

        protected override void OnSelfHurt(int oldVal, int newVal)
        {
            _mRenderer.color = Color.white;
            _mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f)
                .OnComplete(() => _mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, 0.2f)
                .OnComplete(() => transform.position = initPos);
            gameObject.SetOnDestroy(() => _mRenderer.DOKill(), "mRenderer");
            gameObject.SetOnDestroy(() => transform.DOKill(), "transform");
        }

        public override void Judge(int frame, bool force)
        {
            _mFsm.Custom();
        }
    }
}
