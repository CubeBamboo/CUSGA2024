using CbUtils;
using CbUtils.Collections;
using Shuile.Rhythm.Runtime;
using Shuile.Gameplay;
using Shuile.Framework;

using UnityEngine;
using DG.Tweening;

namespace Shuile
{
    /*public class PlayerController : MonoBehaviour, IComponent<Player>, IPlayerCtrl
    {
        //private PlayerPropertySO property;
        private SpriteRenderer mRenderer;
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

        private PlayerPropertySO property => mTarget.Property;
        public int MaxHP => property.maxHealthPoint;

        private Player mTarget;
        public Player Target { set => mTarget = value; }

        private void Awake()
        {
            mRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            // init pos
            transform.position = levelGrid.grid.SnapToGrid(transform.position);
            gridPos = transform.position.ToCell(levelGrid.grid);
            if (levelGrid.grid.HasContent(transform.position.ToCell(levelGrid.grid)))
                Debug.LogWarning("grid has content at player's position.");
            levelGrid.grid.Add(transform.position.ToCell(levelGrid.grid), gameObject);

            mTarget.CurrentHp.Register(OnHurt);
        }

        private void OnDestroy()
        {
            mTarget.CurrentHp.UnRegister(OnHurt);
        }

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
        }

        public void NormalMove(float xDirection)
        {
            var oldPos = GridPos;
            var nextPos = oldPos + new Vector3Int((int)xDirection, 0, 0);
            FaceDir = xDirection > 0 ? 1 : -1;

            if (levelGrid.grid.HasContent(nextPos))
                return;

            GridPos += Vector3Int.right * (int)xDirection;
            levelGrid.grid.Move(oldPos, nextPos);
        }

        public void SingleJump()
        {
            transform.DOJump(transform.position, 1f, 1, MusicRhythmManager.Instance.BpmInterval);
        }

        private void OnHurt(int val)
        {
            // 受伤反馈
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f).OnComplete(() =>
                mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, strength: 0.2f).OnComplete(() =>
                    transform.position = initPos);
        }
    }
    */
}
