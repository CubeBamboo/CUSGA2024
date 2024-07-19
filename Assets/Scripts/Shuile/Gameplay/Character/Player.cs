using CbUtils;
using Shuile.Core.Gameplay;
using Shuile.Core.Gameplay.Common;
using Shuile.Gameplay.Move;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public class Player : MonoBehaviour, IHurtable
    {
        [SerializeField] private PlayerPropertySO property;
        private LevelStateMachine _levelStateMachine;
        private PlayerModel _playerModel;

        private bool isDie;
        public EasyEvent OnDie = new();
        public EasyEvent OnHurted = new();

        public HearableProperty<int> CurrentHp { get; } = new();
        public PlayerPropertySO Property => property;

        private void Awake()
        {
            var scope = LevelScope.Interface;
            _playerModel = scope.GetImplementation<PlayerModel>();
            _levelStateMachine = scope.GetImplementation<LevelStateMachine>();

            _playerModel.moveCtrl = GetComponent<SmoothMoveCtrl>();
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
