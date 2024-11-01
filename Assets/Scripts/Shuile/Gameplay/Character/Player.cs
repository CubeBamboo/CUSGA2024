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
        private GamePlayScene.GameplayStatics _statics;

        private bool isDie;
        public EasyEvent OnDie = new();
        public EasyEvent OnHurted = new();

        public HearableProperty<int> CurrentHp { get; } = new();
        public PlayerPropertySO Property => property;

        public override void LoadFromParentContext(IReadOnlyServiceLocator context)
        {
            context
                .Resolve(out _statics)
                .Resolve(out _levelStateMachine);
        }

        public override void BuildSelfContext(RuntimeContext context)
        {
            var scheduler = context.RegisterMonoScheduler(this);
            context.RegisterInstance(this);
            context.RegisterInstance(transform);
            context.RegisterInstance(gameObject);
            context.RegisterInstance(_playerModel = new PlayerModel());
            context.RegisterInstance(GetComponent<Rigidbody2D>());
            context.RegisterInstance(GetComponent<NormalPlayerCtrl>());

            context.RegisterFactory(() => new SmoothMoveCtrl(context));
            context.RegisterFactory(() => new PlayerChartManager(context));

            context.Inject(new NormalPlayerFeel(scheduler));
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

            var rawLoss = CurrentHp.Value - attackPoint;
            _statics.HealthLoss += rawLoss > 0 ? rawLoss : 0;

            CurrentHp.Value -= attackPoint;

            if (CurrentHp.Value < 0)
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
