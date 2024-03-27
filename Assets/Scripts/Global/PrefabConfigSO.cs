using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    [CreateAssetMenu(fileName = "New PrefabConfig", menuName = "Config/Prefab Config")]
    public class PrefabConfigSO : ScriptableObject
    {
        [Header("Enemy")]
        [Tooltip("错乱的机械体")] public GameObject bakaMachine;
        [Tooltip("炸猪")] public GameObject creeper;
        [Tooltip("追踪弹发射炮")] public GameObject mahouDefenseTower;

        public GameObject vegetableEnemy;
        public GameObject normalEnemy;
    }
}
