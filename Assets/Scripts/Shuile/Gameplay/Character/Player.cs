using CbUtils;
using Shuile.Core.Gameplay;
using Shuile.Core.Gameplay.Common;
using Shuile.Framework;
using Shuile.Gameplay.Move;
using Shuile.Rhythm.Runtime;
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

        public override void BuildContext(ServiceLocator context)
        {
            context.RegisterInstance(this);
            context.RegisterInstance(transform);
            context.RegisterInstance(GetComponent<Rigidbody2D>());
            context.RegisterInstance(_playerModel = new PlayerModel());
            context.RegisterMonoScheduler(this);

            context.RegisterFactory(() => new SmoothMoveCtrl(context));
            context.RegisterFactory(() => new PlayerChartManager(context));
        }

        public override void ResolveContext(IReadOnlyServiceLocator context)
        {
            context.Resolve(out _levelStateMachine);
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
    }

    public static class PlayerExtension
    {
        public static void ForceDie(this Player player)
        {
            player.OnHurt(player.Property.maxHealthPoint + 1);
        }
    }
}
