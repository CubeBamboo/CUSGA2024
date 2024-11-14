using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shuile.Core.Gameplay.Data;
using Shuile.Gameplay.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile.Gameplay.Entity.Enemies
{
    public class MahouDefenseTower : Enemy
    {
        private static Transform bombParent;

        [SerializeField] private int attackPoint = 150;
        [SerializeField] private int explosionDelay = 4;
        [SerializeField] private int bombCount = 3;
        [SerializeField] private float explodeRadius = 4f;

        private GameObject bombPrefab;
        private readonly List<Bomb> bombs = new();

        private int counter;

        private SpriteRenderer mRenderer;

        public int ExplosionDelay
        {
            get => explosionDelay;
            set => explosionDelay = value;
        }

        public static Transform BombParent =>
            bombParent != null ? bombParent : bombParent = new GameObject("Bombs").transform;

        protected override void OnAwake()
        {
            bombPrefab = GameApplication.BuiltInData.globalPrefabs.mahouBomb;
            mRenderer = GetComponentInChildren<SpriteRenderer>();
            CurrentType = EnemyType.MahouDefenseTower;
        }

        public override void GetFromPool()
        {
            base.GetFromPool();
            moveController.IsFrozen = true;

            // enter anim
            EnterAnim().Forget();
        }

        public override void ReleaseFromPool()
        {
            base.ReleaseFromPool();
            mRenderer.DOKill();
            transform.DOKill();
        }

        private async UniTaskVoid EnterAnim()
        {
            transform.localScale = Vector3.one * 0.1f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.4f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.7f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 1f;
        }

        private async UniTask ExitAnim()
        {
            transform.localScale = Vector3.one * 0.7f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.4f;
            await UniTask.Delay(200, cancellationToken: destroyCancellationToken);
            transform.localScale = Vector3.one * 0.1f;
        }

        public override void Judge(int frame, bool force)
        {
            Attack();
            counter++;
        }

        private void Attack()
        {
            if (counter == 0)
            {
                SpawnBombs();
            }

            if (counter < explosionDelay)
            {
                return;
            }

            foreach (var bomb in bombs)
            {
                bomb.Explode(attackPoint, explodeRadius);
            }

            bombs.Clear();
            counter = -1;
        }

        private void SpawnBombs()
        {
            for (var i = 0; i < bombCount; i++)
            {
                var pos = LevelZoneManager.Instance.RandomValidPosition();
                var bomb = Instantiate(bombPrefab, pos, Quaternion.identity, BombParent).GetComponent<Bomb>();
                bombs.Add(bomb);
            }
        }

        private void InterruptAttack()
        {
            foreach (var bomb in bombs)
            {
                bomb.Interrupt();
            }

            bombs.Clear();
        }

        protected override async void BeginDie()
        {
            InterruptAttack();

            try
            {
                await ExitAnim();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                EndDie();
            }
        }

        protected override void OnSelfHurt(int oldVal, int newVal)
        {
            mRenderer.color = Color.white;
            mRenderer.DOColor(new Color(230f / 255f, 73f / 255f, 73f / 255f), 0.2f)
                .OnComplete(() => mRenderer.DOColor(Color.white, 0.2f));
            var initPos = transform.position;
            transform.DOShakePosition(0.2f, 0.2f)
                .OnComplete(() => transform.position = initPos);
        }
    }
}
