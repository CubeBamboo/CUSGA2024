using System;
using UnityEngine;

namespace Shuile.Core.Gameplay.Data
{
    /// <summary> data of single round </summary>
    [Serializable]
    public class LevelEnemyData
    {
        public EnemyType[] enemyList; // index -> danger level
    }

    /// <summary>
    ///     define every round's data
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyRoundSO", menuName = "Config/EnemyRoundSO")]
    public class LevelEnemySO : ScriptableObject
    {
        public LevelEnemyData[] enemies;
    }
}
