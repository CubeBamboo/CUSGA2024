using CbUtils;
using Shuile.Core.Gameplay;
using Shuile.Core.Gameplay.Common;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public class Player : MonoContainer, IHurtable
    {
        [SerializeField] private PlayerPropertySO property;
        private LevelStateMachine _levelStateMachine;
        private PlayerModel _playerModel;

        private bool isDie;
        public EasyEvent OnDie = new();
        public EasyEvent OnHurted = new();

        public HearableProperty<int> CurrentHp { get; } = new();
        public PlayerPropertySO Property => property;
        public event Action OnFixedUpdate;

        public override void BuildContext(ServiceLocator context)
        {
            context.RegisterInstance(this);
            context.RegisterInstance(transform);
            context.RegisterInstance(GetComponent<Rigidbody2D>());

            context.RegisterFactory(() => new SmoothMoveCtrl(context));
        }

        public override void Awake()
        {
            base.Awake();
            var scope = LevelScope.Interface;
            _playerModel = scope.GetImplementation<PlayerModel>();
            _levelStateMachine = scope.GetImplementation<LevelStateMachine>();
        }

        private void Start()
        {
            isDie = false;
            CurrentHp.Value = property.maxHealthPoint;
        }

        public void OnHurt(int attackPoint)
        {
            if (isDie || _playerModel.isInviciable)
            {
                return;
            }

            OnHurted.Invoke();
            CurrentHp.Value -= attackPoint;
            if (CurrentHp.Value < 0)
            {
                CurrentHp.Value = 0;
            }

            // check die
            if (CurrentHp.Value <= 0)
            {
                CurrentHp.Value = 0;
                isDie = true;
                OnDie.Invoke();
                _levelStateMachine.State = LevelStateMachine.LevelState.Fail;
            }
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
    }

    public static class PlayerExtension
    {
        public static void ForceDie(this Player player)
        {
            player.OnHurt(player.Property.maxHealthPoint + 1);
        }
    }
}
