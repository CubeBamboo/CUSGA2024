using CbUtils;
using CbUtils.Event;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shuile.Framework;
using Shuile.Gameplay;
using Shuile.NoteProduct;
using UnityEngine;

namespace Shuile.Rhythm
{
    public struct NoteData
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

        public float preShowTime;

        public NoteEventType eventType;
        public NoteEventData eventData;

        public static NoteData Create(float targetTime)
            => new() { targetTime = targetTime };
    }

    public static class NoteEventHelper
    {
        // type -> process
        public static void Process(NoteData noteData)
        {
            switch (noteData.eventType)
            {
                case NoteEventType.SingleEnemySpawn:
                    SingleEnemySpawn(noteData.eventData);
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
        public static void SingleEnemySpawn(NoteEventData noteData)
        {
            var levelGrid = LevelGrid.Instance;

            // TODO: store in eventdata
            // get pos (random in a line)
            Vector2 offset = levelGrid.grid.OriginPosition;
            Vector2 rectScale = new Vector2(15, 0);
            levelGrid.GetRandomPosition(out Vector3Int randomGridPos);

            // random enemy
            var randomType = (EnemyType)Random.Range(0, (int)EnemyType.TotalCount);
            // instantiate
            var enemy = EnemyManager.Instance.SpawnEnemy(EnemyType2Prefab(randomType),randomGridPos);

            // TODO: [!][FOR TEST]
            // destroy when touch player
            var evtMono = enemy.gameObject.AddComponent<Collider2DEventMono>();
            evtMono.TriggerEntered += coll =>
            {
                if (coll.gameObject.CompareTag("Player"))
                {
                    enemy.GotoState(EntityStateType.Dead);
                }
            };
        }

        /// <param name="interval">(unit: in seconds)</param>
        public static async void MultiEnemySpawn(float duration, float spawnInterval)
        {
            // 以spawnInterval时间间隔生成敌人，持续duration秒
            //UnityEngine.Debug.Log("MultiEnemySpawn Start");
            float timer = spawnInterval;
            while (timer < duration)
            {
                SingleEnemySpawn(new NoteEventData());
                await UniTask.Delay(System.TimeSpan.FromSeconds(spawnInterval));
                timer += spawnInterval;
            }
            //UnityEngine.Debug.Log("MultiEnemySpawn End");
        }

        // TODO: [!]add pre show time
        // TODO: belongs to mechanism event
        public static void LaserSpawn()
        {
            PrefabConfigSO prefabConfig = GameplayService.Interface.Get<PrefabConfigSO>();
            var levelGrid = LevelGrid.Instance;

            var go = prefabConfig.laser.Instantiate(); // spawn
            levelGrid.GetRandomPosition(out Vector3 randomPos);
            go.SetPosition(randomPos); // init position
            NoteProductController.Laser.Process(go); // laser behavior
        }

        public static UnityEngine.GameObject EnemyType2Prefab(EnemyType enemyType)
        {
            PrefabConfigSO prefabConfig = GameplayService.Interface.Get<PrefabConfigSO>();
            var res = enemyType switch
            {
                EnemyType.ZakoRobot => prefabConfig.zakoRobot,
                EnemyType.Creeper => prefabConfig.creeper,
                EnemyType.MahouDefenseTower => prefabConfig.mahouDefenseTower,
                _ => throw new System.Exception("Invalid EnemyType."),
            };
            return res;
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

    public enum NoteEventType
    {
        SingleEnemySpawn,
        MultiEnemySpawn, // spawn in a certain frequency
        ObjectTransform,
        LaserSpawn,
        MusicOffsetTestLaser,
    }

    public enum EnemyType
    {
        ZakoRobot,
        Creeper,
        MahouDefenseTower,
        TotalCount // for count, not enemy
    }

    // TODO: support event data // 相当于事件传参了，就是我没想好这个参数怎么写才能适应这么多事件(
    public struct NoteEventData
    {
        //public EnemyType enemyType;

    }
}
