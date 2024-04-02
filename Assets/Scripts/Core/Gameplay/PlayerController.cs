using CbUtils;
using CbUtils.Collections;

using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

namespace Shuile
{
    public class PlayerController : MonoBehaviour, IHurtable
    {
        [SerializeField] private PlayerPropertySO property;
        [SerializeField] private SpriteRenderer mRenderer;
        private Vector3Int gridPos;

        public Vector3Int GridPos
        {
            get => gridPos;
            set
            {
                gridPos = value;
                transform.DOMove(gridPos.ToWorld(levelGrid.grid), 0.1f)
                         .SetEase(Ease.OutSine);
            }
        }

        private LevelGrid levelGrid => LevelGrid.Instance;

        public int FaceDir { get; private set; }
        public event System.Action<int> OnHpChangedEvent;
        public int MaxHP => property.maxHealthPoint;
        private bool isHpUIShowing = false; // TODO: maybe put it in a other place

        private void Start()
        {
            property.currentHealthPoint = property.maxHealthPoint;

            // init pos
            transform.position = levelGrid.grid.SnapToGrid(transform.position);
            gridPos = transform.position.ToCell(levelGrid.grid);
            if (levelGrid.grid.HasContent(transform.position.ToCell(levelGrid.grid)))
                Debug.LogWarning("grid has content at player's position.");
            levelGrid.grid.Add(transform.position.ToCell(levelGrid.grid), gameObject);

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
            if (Keyboard.current.upArrowKey.isPressed && Keyboard.current.downArrowKey.isPressed
                && DebugProperty.Instance.GetInt("PlayerKaiGua") == 0)
            {
                DebugProperty.Instance.SetInt("PlayerKaiGua", 1);
                Debug.Log("开挂模式");
                property.currentHealthPoint = 999999;
            }
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                this.OnAttack(20);
                this.OnAttack((int)(property.currentHealthPoint * 0.25f));
            }
        }

#if UNITY_EDITOR
        //private void OnGUI()
        //{
        //    //GUI.skin.label.fontSize = 40;
        //    //GUILayout.Label($"Health:{property.currentHealthPoint}");
        //}
#endif

        // default attack, maybe add weapon feature later.
        public void Attack()
        {
            // 效果反馈
            transform.DOScale(1.2f, 0.1f).OnComplete(
                () => transform.DOScale(1f, 0.1f));

            // 搜索敌人
            Vector3Int targetPos = GridPos + new Vector3Int(FaceDir, 0, 0);
            if (levelGrid.grid.TryGet(targetPos, out var targetGo))
            {
                //TODO: 检查是否是敌人
                targetGo.GetComponent<IHurtable>().OnAttack(property.attackPoint);
            }
            /*if (EnemyManager.Instance.TryGetEnemyAtPosition(Mathf.RoundToInt(transform.position.x), out var enemy))
            {
                enemy.OnAttack(property.attackPoint);
            }*/
        }

        public void Move(float xDirection)
        {
            var oldPos = GridPos;
            var nextPos = oldPos + new Vector3Int((int)xDirection, 0, 0);
            FaceDir = xDirection > 0 ? 1 : -1;

            if (levelGrid.grid.HasContent(nextPos))
                return;

            GridPos += Vector3Int.right * (int)xDirection;
            levelGrid.grid.Move(oldPos, nextPos);
        }

        public void Jump()
        {
            transform.DOJump(transform.position, 1f, 1, 1.0f);
        }

        /// <param name="xDirection"> -1: left, 1: right </param>
        public void Dash(int xDirection)
        {
            int dashDist = 2;
            transform.position += dashDist * new Vector3(xDirection, 0);
            Debug.Log("dash"); // TODO: [!bugs:] you can't modify transform when dotween.DOJump() controlling it.
        }

        public void OnAttack(int attackPoint)
        {
            property.currentHealthPoint -= attackPoint;
            if(property.currentHealthPoint < 0)
                property.currentHealthPoint = 0;

            OnHpChangedEvent?.Invoke(property.currentHealthPoint);
            if (!isHpUIShowing)
            {
                //UICtrl.Instance.Get<HUDHpBarElement>().Show(); // sorry but i only call once here
                isHpUIShowing = true;
            }

            // 受伤反馈
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f/255f, 73f/255f, 73f/255f), 0.2f).OnComplete(()=>
                mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(()=>
                    transform.position = initPos);

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
