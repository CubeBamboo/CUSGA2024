using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shuile
{
    [CreateAssetMenu(fileName = "New PrefabConfig", menuName = "Config/Prefab Config")]
    public class PrefabConfigSO : ScriptableObject
    {
        [Header("Enemy")]
        public GameObject vegetableEnemy;
        public GameObject normalEnemy;
    }
}
