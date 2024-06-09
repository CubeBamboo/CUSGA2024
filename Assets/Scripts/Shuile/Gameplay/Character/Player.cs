using CbUtils;
using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay.Manager;
using Shuile.Gameplay.Move;
using System;
using UnityEngine;

namespace Shuile.Gameplay.Character
{
    public class Player : MonoBehaviour, IEntity, IHurtable
    {
        public static Player Instance => MonoSingletonProperty<Player>.Instance;

        private PlayerModel _playerModel;
        private LevelStateMachine _levelStateMachine;

        public HearableProperty<int> CurrentHp { get; private set; } = new();
        public EasyEvent OnDie = new();
        public EasyEvent OnHurted = new();

        private bool isDie;

        [SerializeField] private PlayerPropertySO property;
        public PlayerPropertySO Property => property;

        private void Awake()
        {
            MonoSingletonProperty<Player>.InitSingleton(this);
            MonoSingletonProperty<Player>.EnableAutoSpawn = false;
            _playerModel = this.GetModel<PlayerModel>();
            _levelStateMachine = this.GetSystem<LevelStateMachine>();
            _playerModel.moveCtrl = GetComponent<SmoothMoveCtrl>();
        }
        private void Start()
        {
            isDie = false;
            CurrentHp.Value = property.maxHealthPoint;
        }

        public void OnHurt(int attackPoint)
        {
            if (isDie || _playerModel.isInviciable) return;

            OnHurted.Invoke();
            CurrentHp.Value -= attackPoint;
            if (CurrentHp.Value < 0)
                CurrentHp.Value = 0;

            // check die
            if (CurrentHp.Value <= 0)
            {
                CurrentHp.Value = 0;
                isDie = true;
                OnDie.Invoke();
                _levelStateMachine.State = LevelStateMachine.LevelState.Fail;
            }
        }

        public ModuleContainer GetModule() => GameApplication.Level;
    }

    public static class PlayerExtension
    {
        public static void ForceDie(this Player player)
            => player.OnHurt(player.Property.maxHealthPoint + 1);
    }
}