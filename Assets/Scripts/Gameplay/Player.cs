using UnityEngine;
using UnityEngine.InputSystem;

namespace Shuile.Gameplay
{
    public class Player : MonoBehaviour, IHurtable
    {
        [SerializeField] private PlayerPropertySO property;
        public event System.Action<int> OnHpChangedEvent;

        public PlayerPropertySO Property => property;
        private void Awake()
        {
            gameObject.AddComponent<NormalPlayerInput>().Target = this;
            gameObject.AddComponent<NormalPlayerCtrl>().Target = this;

        }

        private void Start()
        {
            // init event
            OnHpChangedEvent?.Invoke(property.currentHealthPoint);
        }

        private void Update()
        {
            DebugUpdate();
        }

        //TODO: [!] for debug
        private void DebugUpdate()
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame && Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                //DebugProperty.Instance.SetInt("PlayerKaiGua", 1);
                Debug.Log("开挂模式");
                property.currentHealthPoint = 999999;
            }
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                this.OnAttack(20);
                this.OnAttack((int)(property.currentHealthPoint * 0.25f));
            }
        }

        public void OnAttack(int attackPoint)
        {
            property.currentHealthPoint -= attackPoint;
            if (property.currentHealthPoint < 0)
                property.currentHealthPoint = 0;

            OnHpChangedEvent?.Invoke(property.currentHealthPoint);

            // 检测死亡
            if (property.currentHealthPoint <= 0)
            {
                property.currentHealthPoint = 0;

                // 触发死亡事件
                LevelRoot.Instance.State = LevelRoot.LevelState.End;
            }
        }

        public void ForceDie()
        {
            OnAttack(property.maxHealthPoint + 1);
        }
    }
}
