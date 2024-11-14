using CbUtils;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shuile.Core.Gameplay.Data;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class ZakoMachine : Enemy
    {
        [SerializeField] private float checkPlayerRange = 5f;
        [SerializeField] private float checkAttackRange = 1.2f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private int attackPoint = 80;
        private readonly FSM<DefaultEnemyState> _mFsm = new();
        private ZakoChaseBehavior _chaseBehavior;

        private Animator _mAnimator;
        private float _faceDir;

        private SpriteRenderer _mRenderer;

        private ZakoPatrolBehavior _patrolBehavior;

        private void Update()
        {
            _mFsm.Update();
        }

        private void FixedUpdate()
        {
            _mFsm.FixedUpdate();
        }

        protected override void OnAwake()
        {
            _patrolBehavior = new ZakoPatrolBehavior(gameObject, moveController, 5f);
            _chaseBehavior = new ZakoChaseBehavior();
            RegisterState(_mFsm);

            _mAnimator = GetComponent<Animator>();
            _mRenderer = GetComponentInChildren<SpriteRenderer>();
            CurrentType = EnemyType.ZakoRobot;
        }

        protected override async void OnSelfDie()
        {
            _mFsm.SwitchState(DefaultEnemyState.Dead);
            moveController.IsFrozen = true;

            try
            {
                await PlayDieAnim();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                DieFxEnd?.Invoke(this);
            }
        }

        private void OnDestroy()
        {
            _mRenderer.DOKill();
            transform.DOKill();
        }

        private async UniTask PlayDieAnim()
        {
            _mAnimator.Play("Die");
            await UniTask.Delay(TimeSpan.FromSeconds(0.42), cancellationToken: destroyCancellationToken);
        }

        private void Attack()
        {
            var player = GetPlayerOrThrow();

            var targetPos = player.transform.position;
            if ((targetPos - transform.position).sqrMagnitude < attackRange * attackRange)
            {
                moveController.XMove(10f);
                player.OnHurt(attackPoint);
            }
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
                    //UnityAPIExt.DebugLineForRayCast2D(transform.position, Vector2.right * faceDir, checkPlayerRange, LayerMask.GetMask("Player"));

                    _faceDir = _chaseBehavior.FaceDir;
                    _chaseBehavior.Do();

                    if (_chaseBehavior.XCloseEnoughToTarget(checkAttackRange)) // can attack
                    {
                        mFsm.SwitchState(DefaultEnemyState.Attack);
                    }

                    EnemyBehaviorAction.CheckWallAndJump(moveController, _faceDir, 0.8f);
                });

            mFsm.NewEventState(DefaultEnemyState.Attack)
                .OnCustom(Attack)
                .OnFixedUpdate(() =>
                {
                    if (!_chaseBehavior.XCloseEnoughToTarget(checkAttackRange)) // can attack
                    {
                        mFsm.SwitchState(DefaultEnemyState.Chase);
                    }

                    EnemyBehaviorAction.CheckWallAndJump(moveController, _faceDir, 0.8f);
                });

            mFsm.NewEventState(DefaultEnemyState.Dead);

            mFsm.StartState(DefaultEnemyState.Patrol);
        }

        protected override void OnSelfHurt(int oldVal, int newVal)
        {
            _mRenderer.color = Color.white;
            _mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f)
                .OnComplete(() => _mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, 0.2f)
                .OnComplete(() => transform.position = initPos);
        }

        public override void Judge(int frame, bool force)
        {
            _mFsm.Custom();
        }
    }
}
