using Shuile.Rhythm.Runtime;

using UnityEngine;

namespace Shuile
{


    /// <summary> data of single round </summary>
    [System.Serializable]
    public class EnemyRoundData
    {
        public EnemyType[] enemyList;
        /// <summary> (unit: seconds) </summary>
        public float latestSpawnTime;
    }

    /// <summary>
    /// define every round's data
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyRoundSO", menuName = "Config/EnemyRoundSO")]
    public class EnemyRoundsSO : ScriptableObject
    {
        public EnemyRoundData[] rounds;
    }
}
