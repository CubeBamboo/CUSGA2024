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
            PrefabConfigSO prefabConfig = LevelResources.Instance.globalPrefabs;

            var go = prefabConfig.laser.Instantiate(); // spawn
            go.SetPosition(LevelZoneManager.Instance.RandomValidPosition()); // init position
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
            var randomType = (EnemyType)Random.Range(0, (int)EnemyType.TotalCount);
            EntityFactory.Instance.SpawnEnemy(randomType, LevelZoneManager.Instance.RandomValidPosition());
        }
    }
}
