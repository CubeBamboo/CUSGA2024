using UnityEngine;

namespace Shuile
{
    [CreateAssetMenu(fileName = "New PrefabConfig", menuName = "Config/Prefab Config")]
    public class PrefabConfigSO : ScriptableObject
    {
        [Header("Enemy")]
        [Tooltip("错乱的机械体")] public GameObject zakoRobot;
        [Tooltip("炸猪")] public GameObject creeper;
        [Tooltip("追踪弹发射炮")] public GameObject mahouDefenseTower;

        [Header("Mechanism")]
        public GameObject laser;

        [Header("RhythmIndicator")]
        public GameObject noteIndicator;
    }
}
