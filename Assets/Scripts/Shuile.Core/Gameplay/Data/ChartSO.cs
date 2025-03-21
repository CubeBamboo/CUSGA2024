using UnityEngine;

namespace Shuile.Core.Gameplay.Data
{
    /// <summary>
    ///     chart text file and all the data it depend
    /// </summary>
    [CreateAssetMenu(fileName = "New Chart", menuName = "SOData/Chart")]
    public class ChartSO : ScriptableObject
    {
        public TextAsset chartFile;
        public AudioClip clip;
        [Tooltip("in seconds")] public float musicLength;
    }
}
