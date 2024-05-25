using CbUtils;
using CbUtils.Preview.Event;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    public class Player : MonoBehaviour, IHurtable
    {
        [SerializeField] private PlayerPropertySO property;
        public HearableProperty<int> CurrentHp { get; private set; } = new();
        public EasyEvent OnDie = new();
        public EasyEvent OnHurted = new();

        private PlayerModel playerModel;

        private bool isDie;

        public PlayerPropertySO Property => property;
        private void Awake()
        {
            playerModel = GameplayService.Interface.Get<PlayerModel>();
            playerModel.moveCtrl = GetComponent<SmoothMoveCtrl>();

            GameplayService.Interface.Register<Player>(this);
        }
        private void OnDestroy()
        {
            GameplayService.Interface.UnRegister<Player>();
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
    }

    public static class PlayerExtension
    {
        public static Player GetPlayer(this GameObject gameObject)
            => GameplayService.Interface.Get<Player>();
        public static void ForceDie(this Player player)
            => player.OnHurt(player.Property.maxHealthPoint + 1);
    }
}
