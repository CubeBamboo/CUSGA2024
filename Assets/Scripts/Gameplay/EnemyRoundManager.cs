using CbUtils;
using Cysharp.Threading.Tasks;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;

using UDebug = UnityEngine.Debug;

namespace Shuile
{
    public class EnemyRoundManager : MonoSingletons<EnemyRoundManager>
    {
        public EnemyRoundsSO currentEnemyRoundsData;

        private int nextRoundIndex = 0;
        private bool isEnd;
        private bool isWaitingProcess;

        protected override void OnAwake()
        {
            currentEnemyRoundsData = LevelDataBinder.Instance.enemyRoundsData;
        }
        private void Start()
        {
            nextRoundIndex = 0;
            //UDebug.Log("Rounds Start");
            if (currentEnemyRoundsData.rounds.Length == 0)
            {
                isEnd = true;
                //UDebug.Log("Rounds Count == 0");
            }
        }
        private void FixedUpdate()
        {
            if (isEnd || isWaitingProcess) return;

            bool IsTimeOut = nextRoundIndex >= currentEnemyRoundsData.rounds.Length
                || MusicRhythmManager.Instance.CurrentTime > currentEnemyRoundsData.rounds[nextRoundIndex].latestSpawnTime;
            bool NoEnemy = EntityManager.Instance.Enemies.Count == 0;
            if (IsTimeOut || NoEnemy)
            {
                //UDebug.Log("ready to process");
                isWaitingProcess = true;
                AutoPlayChartManager.Instance.OnNextRhythm(async () =>
                {
                    await ProcessCurrentRound();
                    nextRoundIndex++;
                    isWaitingProcess = false;
                });
            }
        }

        private async UniTask ProcessCurrentRound()
        {
            if (nextRoundIndex > currentEnemyRoundsData.rounds.Length)
            {
                //UDebug.Log("[EnemyManager] it's end");
                isEnd = true;
                return;
            }

            if (nextRoundIndex == currentEnemyRoundsData.rounds.Length)
            {
                //UDebug.Log("it's last round");
                return;
            }

            //UDebug.Log("Process Current Round");
            await QueueSpawnEnemies(nextRoundIndex);
        }

        private async UniTask QueueSpawnEnemies(int index)
        {
            var enemies = currentEnemyRoundsData.rounds[index].enemyList;
            foreach (var enemy in enemies)
            {
                LevelGrid.Instance.TryGetRandomPosition(out var res);
                EntityFactory.Instance.SpawnEnemyWithEffectDelay(enemy, res.ToWorld(LevelGrid.Instance.grid));
                await UniTask.Delay((int)(MusicRhythmManager.Instance.BpmInterval*1000)); // TODO: feng zhuang
            }
        }
    }
}
