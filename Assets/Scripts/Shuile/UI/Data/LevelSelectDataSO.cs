using System;
using UnityEngine;

namespace Shuile.UI.Data
{
    [CreateAssetMenu(fileName = "LevelSelectDataSO", menuName = "Config/LevelSelectData")]
    public class LevelSelectDataSO : ScriptableObject
    {
        public Data[] levelData;

        [Serializable]
        public struct Data
        {
            public string levelDataLabel;

            // show in select panel
            public string songName;
            public string songArtist;
        }
    }
}
