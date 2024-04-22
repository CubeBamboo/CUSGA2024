using UnityEngine;

namespace Shuile.Rhythm
{
    /// <summary>
    /// chart text file and all the data it depend
    /// </summary>
    [CreateAssetMenu(fileName = "New Chart", menuName = "SOData/Chart")]
    public class ChartSO : ScriptableObject
    {
        public TextAsset chartFile;
        public AudioClip clip;
        public float musicLength;
    }
}
