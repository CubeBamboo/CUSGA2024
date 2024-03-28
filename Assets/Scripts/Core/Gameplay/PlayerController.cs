using UnityEngine;

using CbUtils;
using DG.Tweening;
using Shuile.Framework;
using Shuile.Rhythm;
using static Shuile.PlayerController;

namespace Shuile
{
    public class PlayerController : MonoBehaviour, IAttackable
    {
        [SerializeField] private PlayerPropertySO property;
        [SerializeField] private SpriteRenderer mRenderer;

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

            // init event
            OnHpChangedEvent?.Invoke(property.currentHealthPoint);
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            GUI.skin.label.fontSize = 40;
            //GUILayout.Label($"Health:{property.currentHealthPoint}");
        }
#endif

        // default attack, maybe add weapon feature later.
        public void Attack()
        {
            // 效果反馈
            transform.DOScale(1.2f, 0.1f).OnComplete(
                () => transform.DOScale(1f, 0.1f));

            // 搜索敌人
            /*if (EnemyManager.Instance.TryGetEnemyAtPosition(Mathf.RoundToInt(transform.position.x), out var enemy))
            {
                enemy.OnAttack(property.attackPoint);
            }*/
        }

        public void Move(float xDirection)
        {
            transform.position += Vector3.right * xDirection;
            FaceDir = xDirection > 0 ? 1 : -1;
            //TODO: dotween interrupt process
            //transform.DOMoveX(transform.position.x + xDirection, 0.2f)
            //         .SetEase(Ease.OutSine);
            //grid.Move(transform.position.ToCell(grid),
            //    (transform.position + Vector3.right * xDirection).ToCell(grid));
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
