using CbUtils;

using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    public class Player : MonoBehaviour, IHurtable
    {
        private NormalPlayerCtrl mPlayerCtrl;
        private NormalPlayerInput mPlayerInput;

        [SerializeField] private PlayerPropertySO property;
        public HearableProperty<int> CurrentHp { get; private set; } = new();
        public EasyEvent OnDie = new();

        private bool isDie;

        public PlayerPropertySO Property => property;
        private void Awake()
        {
            gameObject.AddComponent<NormalPlayerInput>();
            gameObject.AddComponent<NormalPlayerCtrl>();
            gameObject.AddComponent<NormalPlayerFeel>();

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
        }

        private void Update()
        {
            DebugUpdate();
        }

        //TODO: [!] for debug
        private void DebugUpdate()
        {
            if (Keyboard.current.upArrowKey.isPressed && Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                //DebugProperty.Instance.SetInt("PlayerKaiGua", 1);
                Debug.Log("开挂模式");
                CurrentHp.Value = 999999;
            }
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                this.OnAttack(20);
                this.OnAttack((int)(CurrentHp.Value * 0.25f));
            }
        }

        public void OnAttack(int attackPoint)
        {
            if (isDie) return;

            CurrentHp.Value -= attackPoint;
            if (CurrentHp.Value < 0)
                CurrentHp.Value = 0;

            // 检测死亡
            if (CurrentHp.Value <= 0)
            {
                CurrentHp.Value = 0;
                isDie = true;
                OnDie.Invoke();
                LevelRoot.Instance.State = LevelRoot.LevelState.Fail;
                OnDieFunc();
            }
        }

        public void ForceDie()
        {
            OnAttack(property.maxHealthPoint + 1);
        }

        private void OnDieFunc()
        {
            MonoAudioCtrl.Instance.PlayOneShot("Player_Death");
            // 当前音乐淡出
            AudioManager.Instance.OtherSource.DOFade(0, 0.8f).OnComplete(() =>
            {
                AudioManager.Instance.OtherSource.Stop();
            }); // TODO: [!!] NOOOOOOOOO IT'S SHITTTTTTTTTTT FengZhuang YIxia zhe ge musicPlayer!!!!
        }
    }
}
