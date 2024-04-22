using CbUtils;
using Cysharp.Threading.Tasks;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.Rhythm.Runtime;
using Shuile.Root;
using UnityEngine;

namespace Shuile
{
    /* 按波次，每波敌人在EnemyRoundsSO对象中
     * 当场上敌人少于1个 或 等待时间过长时进入下一波
     * 生成敌人时，每个敌人间隔2个节拍依次生成
     */
    public class EnemyRoundManager : MonoNonAutoSpawnSingletons<EnemyRoundManager>
    {
        [HideInInspector] public EnemyRoundsSO currentEnemyRoundsData;

        private int currentRoundIndex = 0;
        private bool isEnd;
        private bool isWaitingProcess;

        private bool IsInLastRound => currentRoundIndex >= currentEnemyRoundsData.rounds.Length - 1;

        public int CurrentRoundIndex => currentRoundIndex;

        protected override void OnAwake()
        {
            currentEnemyRoundsData = LevelDataBinder.Instance.enemyRoundsData;
        }
        private void Start()
        {
            currentRoundIndex = 0;
            CbLogger.Log("Rounds Start", "EnemyRoundManager.cs");
            if (currentEnemyRoundsData.rounds.Length == 0)
            {
                isEnd = true;
            }
        }
        private void FixedUpdate()
        {
            if (isEnd || isWaitingProcess) return;

            // process 已有enemy
            bool IsNextTimeOut = IsInLastRound
                || MusicRhythmManager.Instance.CurrentTime > currentEnemyRoundsData.rounds[currentRoundIndex].latestSpawnTime;
            bool CurrentEnemyCountIsTooLow = EntityManager.Instance.Enemies.Count <= 1;
            if (!IsInLastRound && (IsNextTimeOut || CurrentEnemyCountIsTooLow)) // process this round's enemy
            {
                isWaitingProcess = true;
                AutoPlayChartManager.Instance.OnNextRhythm(async () =>
                {
                    currentRoundIndex++;
                    try
                    {
                        await QueueSpawnEnemies(currentRoundIndex);
                    }
                    catch (System.Exception e)
                    {
                        CbLogger.LogWarning(e.Message, "EnemyRoundManager.cs");
                    }
                    isWaitingProcess = false;
                });
            }

            // check game end
            if (IsInLastRound && EntityManager.Instance.Enemies.Count <= 0)
            {
                CbLogger.Log("[EnemyManager] it's end", "EnemyRoundManager.cs");
                isEnd = true;
                LevelStateMachine.Instance.State = LevelStateMachine.LevelState.Win;
            }
        }

        private async UniTask QueueSpawnEnemies(int index)
        {
            if (index >= currentEnemyRoundsData.rounds.Length)
                return;

            var enemies = currentEnemyRoundsData.rounds[index].enemyList;
            foreach (var enemy in enemies)
            {
                EntityFactory.Instance.SpawnEnemyWithEffectDelay(enemy, LevelZoneManager.Instance.RandomValidPosition());
                await UniTask.Delay(2 * (int)(GameplayService.Interface.LevelModel.BpmIntervalInSeconds * 1000)
                    , cancellationToken: this.destroyCancellationToken);
            }
        }
    }
}
