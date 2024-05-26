using CbUtils;
using CbUtils.Preview.Event;
using CbUtils.Unity;
using Shuile.Core.Framework;
using Shuile.Core.Framework.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    public class Player : MonoEntity, IHurtable
    {
        public static Player Instance => MonoSingletonProperty<Player>.Instance;

        [SerializeField] private PlayerPropertySO property;
        public HearableProperty<int> CurrentHp { get; private set; } = new();
        public EasyEvent OnDie = new();
        public EasyEvent OnHurted = new();

        private PlayerModel playerModel;

        private bool isDie;

        public PlayerPropertySO Property => property;
        protected override void AwakeOverride()
        {
            // init part
            playerModel = this.GetModel<PlayerModel>();
            playerModel.moveCtrl = GetComponent<SmoothMoveCtrl>();
        }
        private void Start()
        {
            isDie = false;
            CurrentHp.Value = property.maxHealthPoint;

            gameObject.AddComponent<UpdateEventMono>().OnUpdate += DebugInput; // TODO: for debug
        }

        public void OnHurt(int attackPoint)
        {
            if (isDie || playerModel.isInviciable) return;

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
                LevelStateMachine.Instance.State = LevelStateMachine.LevelState.Fail;
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
                playerModel.canInviciable = !playerModel.canInviciable;
                Debug.Log($"受击无敌变更 -> {playerModel.canInviciable}");
            }
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                this.OnHurt(20);
                this.OnHurt((int)(CurrentHp.Value * 0.25f));
            }
        }

        public override LayerableServiceLocator GetLocator() => GameApplication.LevelServiceLocator;
    }

    public static class PlayerExtension
    {
        public static void ForceDie(this Player player)
            => player.OnHurt(player.Property.maxHealthPoint + 1);
    }
}
