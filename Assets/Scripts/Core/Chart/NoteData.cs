using CbUtils;
using Cysharp.Threading.Tasks;

using Shuile.Gameplay;
using Shuile.NoteProduct;
using Shuile.Rhythm.Runtime;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    // TODO: maybe not use in future
    public class NoteData
    {
        /// <summary>
        /// integer part - beat count, decimal part - where in a beat. 
        /// for example: targetTimeArray = { 0 + 0f / 4f, 1 + 0f / 4f, 2 + 0f / 4f, 3 + 0f / 4f };
        /// </summary>
        public float targetTime;
        /// <summary>
        /// only for long note, format is same as targetTime
        /// </summary>
        public float? endTime;

        public NoteEventType eventType;
        //public NoteEventData eventData;

        public static NoteData Create(float targetTime)
            => new() { targetTime = targetTime };
    }

    public enum NoteEventType
    {
        SingleEnemySpawn,
        MultiEnemySpawn, // spawn in a certain frequency
        ObjectTransform,
        LaserSpawn,
        MusicOffsetTestLaser,
    }

    [System.Obsolete("...")]
    // TODO: support event data // 相当于事件传参了，就是我没想好这个参数怎么写才能适应这么多事件(
    public struct NoteEventData
    {
        //public EnemyType enemyType;
    }

    [System.Obsolete("use BaseNoteData instead")]
    public static class NoteEventHelper
    {
        // type -> process
        public static void Process(NoteData noteData)
        {
            switch (noteData.eventType)
            {
                case NoteEventType.SingleEnemySpawn:
                    SingleEnemySpawn();
                    break;
                case NoteEventType.ObjectTransform:
                    throw new System.NotImplementedException();
                case NoteEventType.MultiEnemySpawn:
                    float bpmInterval = MusicRhythmManager.Instance.BpmInterval;
                    var duration = (noteData.endTime.Value - noteData.targetTime) * bpmInterval;
                    var interval = 1f * bpmInterval; // TODO: set interval
                    MultiEnemySpawn(duration, interval);
                    break;
                case NoteEventType.LaserSpawn:
                    LaserSpawn();
                    break;
                case NoteEventType.MusicOffsetTestLaser:
                    MusicOffsetTestLaser();
                    break;
                default:
                    throw new System.Exception("unknown note event type");
            }
        }

        // spawn random enemy in random position, destroy when touch player
        public static void SingleEnemySpawn()
        {
            var levelGrid = LevelGrid.Instance;

            // TODO: store in eventdata
            // get pos (random in a line)
            if (levelGrid.TryGetRandomPosition(out Vector3Int randomGridPos, true))
            {
                // random enemy
                var randomType = (EnemyType)Random.Range(0, (int)EnemyType.TotalCount);
                //var randomType = EnemyType.; // TODO: [!]for test
                // instantiate
                var enemy = EntityFactory.Instance.SpawnEnemy(randomType,randomGridPos.ToWorld(levelGrid.grid));
            }
            else
            {
                Debug.LogWarning("No enough space to spawn enemy");
            }
        }

        /// <param name="spawnInterval">(unit: in seconds)</param>
        public static async void MultiEnemySpawn(float duration, float spawnInterval)
        {
            // 以spawnInterval时间间隔生成敌人，持续duration秒
            //UnityEngine.Debug.Log("MultiEnemySpawn Start");
            float timer = spawnInterval;
            while (timer < duration)
            {
                SingleEnemySpawn();
                await UniTask.Delay(System.TimeSpan.FromSeconds(spawnInterval));
                timer += spawnInterval;
            }
            //UnityEngine.Debug.Log("MultiEnemySpawn End");
        }

        // TODO: belongs to mechanism event
        public static void LaserSpawn()
        {
            PrefabConfigSO prefabConfig = GameplayService.Interface.Get<PrefabConfigSO>();
            var levelGrid = LevelGrid.Instance;

            var go = prefabConfig.laser.Instantiate(); // spawn
            levelGrid.TryGetRandomPosition(out Vector3Int randomPos);
            go.SetPosition(randomPos.ToWorld(levelGrid.grid)); // init position
            NoteProductController.Laser.Process(go); // laser behavior
        }

        public static void MusicOffsetTestLaser()
        {
            PrefabConfigSO prefabConfig = GameplayService.Interface.Get<PrefabConfigSO>();
            var levelGrid = LevelGrid.Instance;

            var go = prefabConfig.laser.Instantiate(); // spawn
            var pos = levelGrid.grid.CellToWorld(new Vector3Int(levelGrid.width / 2, 0));
            go.SetPosition(pos); // init position
            TestExt.DelayDestroy(2 * MusicRhythmManager.Instance.BpmInterval, go); // auto destroy
        }
    }

    public static class NoteEventUtils
    {
        public static float DefaultPlayTimeConvert(NoteData noteData)
        {
            float preshowTime = 0f;
            float preshowRealTime = 0f;
            //if (noteData.eventType == NoteEventType.LaserSpawn) preshowTime = 2;
            float res = (noteData.targetTime - preshowTime) * MusicRhythmManager.Instance.BpmInterval - preshowRealTime;
            return res;
        }
    }
}
