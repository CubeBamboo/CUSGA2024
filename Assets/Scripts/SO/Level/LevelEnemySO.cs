using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile
{
    /// <summary> data of single round </summary>
    [System.Serializable]
    public class LevelEnemyData
    {
        public EnemyType[] enemyList; // index -> danger level
    }

    /// <summary>
    /// define every round's data
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyRoundSO", menuName = "Config/EnemyRoundSO")]
    public class LevelEnemySO : ScriptableObject
    {
        public LevelEnemyData[] enemies;
    }
}
