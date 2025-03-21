using Shuile.Core.Gameplay.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Shuile.Core.Global
{
    [Serializable]
    public struct SpawnEffect
    {
        public GameObject prefab;
        public float duration;
    }

    [Serializable]
    public struct PrefabData
    {
        public string name;
        public GameObject prefab;
    }

    [CreateAssetMenu(fileName = "New PrefabConfig", menuName = "Resources/Prefab Config")]
    public class PrefabConfigSO : ScriptableObject
    {
        [Header("Enemy")] [Tooltip("错乱的机械体")] public GameObject zakoRobot;

        [Tooltip("炸猪")] public GameObject creeper;
        [Tooltip("追踪弹发射炮")] public GameObject mahouDefenseTower;

        [Tooltip("for mahouDefenseTower")] public GameObject mahouBomb;

        [Header("Mechanism")] public GameObject laser;

        [Header("RhythmIndicator")] public Graphic noteIndicator;

        [Header("Effect")] public SpawnEffect enemySpawnEffect;

        [Header("Particles")] public PrefabData[] particles;

        public GameObject GetParticle(string name)
        {
            foreach (var particle in particles)
            {
                if (particle.name == name)
                {
                    return particle.prefab;
                }
            }

            return null;
        }

        public GameObject GetPrefabFromType(EnemyType enemyType)
        {
            var prefabConfig = this;
            var res = enemyType switch
            {
                EnemyType.ZakoRobot => prefabConfig.zakoRobot,
                EnemyType.Creeper => prefabConfig.creeper,
                EnemyType.MahouDefenseTower => prefabConfig.mahouDefenseTower,
                _ => throw new Exception("Invalid EnemyType.")
            };
            return res;
        }
    }
}
