using UnityEngine;

namespace Shuile
{
    [CreateAssetMenu(fileName = "LevelSelectDataSO", menuName = "Config/LevelSelectData")]
    public class LevelSelectDataSO : ScriptableObject
    {
        [System.Serializable]
        public struct Data
        {
            public string levelDataLabel;
            // show in select panel
            public string songName;
            public string songArtist;
        }

        public Data[] levelData;
    }
}
