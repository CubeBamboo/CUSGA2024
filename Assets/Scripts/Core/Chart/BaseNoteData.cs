// [WIP]

using CbUtils;
using Shuile.Gameplay;
using Shuile.NoteProduct;
using UnityEngine;

namespace Shuile.Rhythm.Runtime
{
    public class BaseNoteData
    {
        public float targetTime;
        public virtual void Process() { }
        public virtual float ToPlayTime() => targetTime * MusicRhythmManager.Instance.BpmInterval; // TODO: get rid of musicrhythmmanager

        public static BaseNoteData Create(float time)
            => new() { targetTime = time };

        public override string ToString()
        {
            return $"{GetType().Name} targetTime: {targetTime}";
        }
    }

    public class SpawnLaserNoteData : BaseNoteData
    {
        public override void Process()
        {
            PrefabConfigSO prefabConfig = GameplayService.Interface.Get<PrefabConfigSO>();
            var levelGrid = LevelGrid.Instance;

            var go = prefabConfig.laser.Instantiate(); // spawn
            levelGrid.TryGetRandomPosition(out Vector3Int randomPos);
            go.SetPosition(randomPos.ToWorld(levelGrid.grid)); // init position
            NoteProductController.Laser.Process(go); // laser behavior
        }

        public override float ToPlayTime()
        {
            return (targetTime - 2) * MusicRhythmManager.Instance.BpmInterval;
        }
    }

    public class SpawnSingleEnemyNoteData : BaseNoteData
    {
        public override void Process()
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
                var enemy = EntityManager.Instance.SpawnEnemy(NoteEventUtils.EnemyType2Prefab(randomType), randomGridPos.ToWorld(levelGrid.grid));
            }
            else
            {
                Debug.LogWarning("No enough space to spawn enemy");
            }
        }
    }
}
