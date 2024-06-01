using CbUtils;
using CbUtils.Preview.Event;
using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using Shuile.Gameplay.Event;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    public class Player : MonoEntity, IHurtable
    {
        public static Player Instance => MonoSingletonProperty<Player>.Instance;

        private PlayerModel _playerModel;
        private LevelStateMachine _levelStateMachine;

        [SerializeField] private PlayerPropertySO property;
        public HearableProperty<int> CurrentHp { get; private set; } = new();
        public EasyEvent OnDie = new();
        public EasyEvent OnHurted = new();

        private bool isDie;

        public PlayerPropertySO Property => property;
        protected override void AwakeOverride()
        {
            MonoSingletonProperty<Player>.InitSingleton(this);
            MonoSingletonProperty<Player>.EnableAutoSpawn = false;
            _playerModel = this.GetModel<PlayerModel>();
            _levelStateMachine = this.GetSystem<LevelStateMachine>();
            _playerModel.moveCtrl = GetComponent<SmoothMoveCtrl>();
        }
        private void Start()
        {
            //TypeEventSystem.Global.Trigger<PlayerSpawnEvent>(new PlayerSpawnEvent() { player = this });
            transform.position = LevelDataGetter.Instance.playerInitPosition.position;

            isDie = false;
            CurrentHp.Value = property.maxHealthPoint;

            gameObject.AddComponent<UpdateEventMono>().OnUpdate += DebugInput; // TODO: for debug
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

        private void DebugInput()
        {
            //TODO: [!] for debug
            if (Keyboard.current.upArrowKey.isPressed && Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                //DebugProperty.Instance.SetInt("PlayerKaiGua", 1);
                Debug.Log("开挂模式");
                CurrentHp.Value = 999999;
            }
            if (Keyboard.current.upArrowKey.isPressed && Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                _playerModel.canInviciable = !_playerModel.canInviciable;
                Debug.Log($"受击无敌变更 -> {_playerModel.canInviciable}");
            }
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                this.OnHurt(20);
                this.OnHurt((int)(CurrentHp.Value * 0.25f));
            }
        }

        public override ModuleContainer GetModule() => GameApplication.Level;
    }

    public static class PlayerExtension
    {
        public static void ForceDie(this Player player)
            => player.OnHurt(player.Property.maxHealthPoint + 1);
    }
}
